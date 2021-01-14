using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using SPI_AOI.Models;
using SPI_AOI.Devices;
using NLog;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System.Threading;

namespace SPI_AOI.Views.ModelManagement
{
    /// <summary>
    /// Interaction logic for AutoAdjustFOVWindow.xaml
    /// </summary>
    public partial class AutoAdjustFOVWindow : Window
    {
        private Model mModel = null;
        private Properties.Settings mParam = Properties.Settings.Default;
        private Logger mLog = Heal.LogCtl.GetInstance();
        private CalibrateInfo mCalibImage = CalibrateLoader.GetIntance();
        private PLCComm mPlcComm = new PLCComm();
        private IOT.HikCamera mCamera = null;
        private DKZ224V4ACCom mLight = null;
        private bool mLoaded = false;
        private bool mAdjustPad = false;
        private Image<Bgr, byte> mImage = null;
        private System.Drawing.Point[] mAnchorFOV = null;
        private System.Drawing.Point[] mAnchorROIGerber = null;
        public System.Drawing.Point[] mMark = null;
        Utils.MarkAdjust mMarkAdjust = new Utils.MarkAdjust();
        public AutoAdjustFOVWindow(Model model)
        {
            mModel = model;
            
            InitializeComponent();
        }
        private void Window_Initialized(object sender, EventArgs e)
        {
            grConfig.IsEnabled = false;
            LoadUI();
            int ping = mPlcComm.Ping();
            if (ping != 0)
            {
                ReleaseResource();
                MessageBox.Show(string.Format("Cant ping to PLC  IP  {0}:{1}", mParam.PLC_IP, mParam.PLC_PORT), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            mCamera = MyCamera.GetInstance();
            if (mCamera == null)
            {
                ReleaseResource();
                MessageBox.Show(string.Format("Not found camera!"), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            int stOpenCamera = mCamera.Open();
            if (stOpenCamera != 0)
            {
                ReleaseResource();
                MessageBox.Show(string.Format("Cant open the camera!"), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            mCamera.StartGrabbing();
            mCamera.SetParameter(IOT.KeyName.ExposureTime, (float)mModel.HardwareSettings.ExposureTime);
            mCamera.SetParameter(IOT.KeyName.Gain, (float)mModel.HardwareSettings.Gain);
            mLight = new DKZ224V4ACCom(mParam.LIGHT_COM);
            int stOpenLightCtl = mLight.Open();
            if (stOpenLightCtl != 0)
            {
                ReleaseResource();
                MessageBox.Show(string.Format("Cant connect to light source controller!"), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            int[] intensity = mModel.HardwareSettings.LightIntensity;
            mLight.SetFour(intensity[0], intensity[1], intensity[2], intensity[3]);
            mLight.ActiveFour(0, 0, 0, 0);
            mPlcComm.Logout();
            mLoaded = true;
            grConfig.IsEnabled = true;
        }
        private void ReleaseResource()
        {
            if (mCamera != null)
            {
                if (mCamera.IsGrab)
                {
                    mCamera.StopGrabbing();
                }
                if (mCamera.IsOpen)
                {
                    mCamera.Close();
                }
                mCamera = null;
            }
            if (mLight != null)
            {
                if (mLight.Serial.IsOpen)
                {
                    mLight.Close();
                    mLight = null;
                }
            }
        }
        private void LoadUI()
        {
            if (mModel != null)
            {
                if (mModel.Gerber != null)
                {
                    int nfov = mModel.Gerber.FOVs.Count;
                    lbNumFOV.Content = nfov;
                    for (int i = 0; i < nfov; i++)
                    {
                        cbFOV.Items.Add(i + 1);
                    }
                    mAnchorFOV = mModel.GetPulseXYFOVs();
                    mAnchorROIGerber = mModel.GetAnchorsFOV(false);
                }
            }
        }

        private void btBack_Click(object sender, RoutedEventArgs e)
        {
            if (!mLoaded)
                return;
            if(cbFOV.SelectedIndex > 0)
            {
                cbFOV.SelectedIndex = cbFOV.SelectedIndex - 1;
            }
        }

        private void btNext_Click(object sender, RoutedEventArgs e)
        {
            if (!mLoaded)
                return;
            if (cbFOV.SelectedIndex < cbFOV.Items.Count - 1)
            {
                cbFOV.SelectedIndex = cbFOV.SelectedIndex + 1;
            }
        }
        private void CaptureMark()
        {
            mPlcComm.Logout();
            bool lightStrobe = !Convert.ToBoolean(mParam.LIGHT_MODE);
            System.Drawing.Point[] markPointXYPLC = mModel.GetPLCMarkPosition();
            PadItem[] PadMark = new PadItem[2];
            for (int i = 0; i < 2; i++)
            {
                PadMark[i] = mModel.Gerber.PadItems[mModel.Gerber.MarkPoint.PadMark[i]];
            }
            mMark =  new System.Drawing.Point[2];
            double matchingScore = mModel.Gerber.MarkPoint.Score;
            for (int i = 0; i < markPointXYPLC.Length; i++)
            {
                System.Drawing.Point mark = markPointXYPLC[i];
                int x = mark.X;
                int y = mark.Y;
                mLog.Info(string.Format("{0}, Position Name : {1},  X = {2}, Y = {3}", "Moving TOP Axis", "Mark " + (i + 1).ToString(), x, y));
                using (Image<Bgr, byte> image = VI.CaptureImage.CaptureFOV(mPlcComm, mCamera, mLight, mark, lightStrobe))
                {
                    if (image != null)
                    {
                        System.Drawing.Rectangle ROI = mModel.GetRectROIMark();
                        image.ROI = ROI;
                        using (Image<Gray, byte> imgGray = new Image<Gray, byte>(image.Size))
                        {
                            CvInvoke.CvtColor(image, imgGray, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);
                            CvInvoke.Threshold(imgGray, imgGray, mModel.Gerber.MarkPoint.ThresholdValue, 255, Emgu.CV.CvEnum.ThresholdType.Binary);
                            VectorOfPoint cnt = new VectorOfPoint(PadMark[i].Contour);
                                var markInfo = Mark.MarkDetection(imgGray, cnt);
                            cnt.Dispose();
                            cnt = null;
                            double realScore = markInfo.Item2;
                            realScore = Math.Round((1 - realScore) * 100.0, 2);
                            if (realScore > matchingScore)
                            {
                                using (VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint())
                                {
                                    if (markInfo.Item1 != null)
                                    {
                                        contours.Push(markInfo.Item1);
                                        Moments mm = CvInvoke.Moments(markInfo.Item1);
                                        if (mm.M00 != 0)
                                        {
                                            mMark[i] = new System.Drawing.Point(Convert.ToInt32(mm.M10 / mm.M00), Convert.ToInt32(mm.M01 / mm.M00));
                                        }
                                    }
                                }
                                if (i == 1)
                                {
                                    mMarkAdjust.Status = Utils.ActionStatus.Successfully;
                                    System.Drawing.Point ct = new System.Drawing.Point(image.Width / 2, image.Height / 2);
                                    mMarkAdjust.X = ct.X - mMark[0].X;
                                    mMarkAdjust.Y = ct.Y - mMark[0].Y;
                                }
                            }
                            else
                            {
                                mLog.Info(string.Format("Score matching is lower score standard... {0} < {1}", realScore, matchingScore));
                                mMarkAdjust.Status = Utils.ActionStatus.Fail;
                                break;
                            }
                        }
                    }
                    else
                    {
                        mLog.Info(string.Format("Cant Capture image in Mark : {0}", i + 1));
                        mMarkAdjust.Status = Utils.ActionStatus.Fail;
                        break;
                    }
                }
            }
        }
        private void cbFOV_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!mLoaded)
                return;
            if(mMark == null)
            {
                CaptureMark();
            }
            int id = -1;
            this.Dispatcher.Invoke(() =>
            {
                id = cbFOV.SelectedIndex;
            });
            
            if (cbFOV.SelectedIndex > -1)
            {
                System.Drawing.Point fov = mAnchorFOV[id];
                int x = fov.X;
                int y = fov.Y;

                bool lightStrobe = !Convert.ToBoolean(mParam.LIGHT_MODE);
                if (mImage != null)
                {
                    mImage.Dispose();
                    mImage = null;
                }
                mLog.Info(string.Format("{0}, Position Name : {1},  X = {2}, Y = {3}", "Moving TOP Axis", "FOV " + (id + 1).ToString(), x, y));
                using (Image<Bgr, byte> image = VI.CaptureImage.CaptureFOV(mPlcComm, mCamera, mLight, fov, lightStrobe, 1000))
                {
                    mImage = ImageProcessingUtils.ImageRotation(image, new System.Drawing.Point(image.Width / 2, image.Height / 2), -mModel.AngleAxisCamera * Math.PI / 180.0).Copy();
                    mImage = ImageProcessingUtils.ImageTransformation(mImage, mMarkAdjust.X, mMarkAdjust.Y);
                    ShowDetail();
                }
            }
        }
        private void ShowDetail()
        {
            int id = -1;
            this.Dispatcher.Invoke(() =>
            {
                id = cbFOV.SelectedIndex;
            });
            System.Drawing.Rectangle ROI = mModel.Gerber.FOVs[id].ROI;
            this.Dispatcher.Invoke(() =>
            {
                txtROIX.Text = ROI.X.ToString();
                txtROIY.Text = ROI.Y.ToString();
                txtROIWidth.Text = ROI.Width.ToString();
                txtROIHeight.Text = ROI.Height.ToString();
            });
            
            if (mImage != null)
            {
                var modelFov = mModel.FOV;
                System.Drawing.Rectangle ROIGerber = new System.Drawing.Rectangle(
                    mAnchorROIGerber[id].X - modelFov.Width / 2, mAnchorROIGerber[id].Y - modelFov.Height / 2,
                    modelFov.Width, modelFov.Height);
                mImage.ROI = ROI;
                using (Image<Bgr, byte> imgGerberBgr = new Image<Bgr, byte>(ROIGerber.Size))
                using (VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint())
                {
                    for (int i = 0; i < mModel.Gerber.PadItems.Count; i++)
                    {
                        PadItem item = mModel.Gerber.PadItems[i];
                        if (item.FOVs.Count > 0)
                        {
                            if (item.FOVs[0] == id)
                            {
                                System.Drawing.Point[] cntPointSub = new System.Drawing.Point[item.ContourAdjust.Length];
                                for (int j = 0; j < cntPointSub.Length; j++)
                                {
                                    cntPointSub[j] = new System.Drawing.Point(item.ContourAdjust[j].X - ROIGerber.X, item.ContourAdjust[j].Y - ROIGerber.Y);

                                }
                                contours.Push(new VectorOfPoint(cntPointSub));
                            }
                        }
                    }
                    CvInvoke.DrawContours(imgGerberBgr, contours, -1, new MCvScalar(255), -1);
                    CvInvoke.AddWeighted(imgGerberBgr, 0.5, mImage, 0.5, 1, imgGerberBgr);
                    this.Dispatcher.Invoke(() =>
                    {
                        BitmapSource bms = Utils.Convertor.Bitmap2BitmapSource(imgGerberBgr.Bitmap);
                        imb.Source = bms;
                    });
                }
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            else
            {
                mLog.Info(string.Format("Cant Capture image in FOV : {0}", id + 1));
            }

        }

        private void btAdjust_Click(object sender, RoutedEventArgs e)
        {
            int id = -1;
            this.Dispatcher.Invoke(() =>
            {
                id = cbFOV.SelectedIndex;
            });
            if(id > -1)
            {
                System.Drawing.Rectangle ROI = new System.Drawing.Rectangle() ;
                this.Dispatcher.Invoke(() => {
                    ROI = new System.Drawing.Rectangle(
                        Convert.ToInt32(txtROIX.Text),
                        Convert.ToInt32(txtROIY.Text),
                        Convert.ToInt32(txtROIWidth.Text),
                        Convert.ToInt32(txtROIHeight.Text)
                        );
                });
                mImage.ROI = ROI;
                var modelFov = mModel.FOV;
                System.Drawing.Rectangle ROIGerber = new System.Drawing.Rectangle(
                    mAnchorROIGerber[id].X - modelFov.Width / 2, mAnchorROIGerber[id].Y - modelFov.Height / 2,
                    modelFov.Width, modelFov.Height);
                mModel.Gerber.ProcessingGerberImage.ROI = ROIGerber;
                using (Image<Gray, byte> imgGerber = mModel.Gerber.ProcessingGerberImage.Copy())
                {
                    ROI = VI.AutoAdjustROIFOV.Adjust(mImage, imgGerber, new Hsv(145, 110, 130), new Hsv(255, 255, 255), mModel.FOV, ROI);
                    mModel.Gerber.FOVs[id].ROI = ROI;
                }
                if(mAdjustPad)
                {
                    List<System.Drawing.Rectangle> padBoundInFOV = new List<System.Drawing.Rectangle>();
                    List<PadItem> padInFOV = new List<PadItem>();
                    for (int i = 0; i < mModel.Gerber.PadItems.Count; i++)
                    {
                        PadItem item = mModel.Gerber.PadItems[i];
                        if (item.FOVs.Count > 0)
                        {
                            if (item.FOVs[0] == id)
                            {
                                System.Drawing.Rectangle padBound = item.Bouding;
                                padBound.X -= ROIGerber.X;
                                padBound.Y -= ROIGerber.Y;
                                padBoundInFOV.Add(padBound);
                                padInFOV.Add(mModel.Gerber.PadItems[i]);
                            }
                        }
                    }
                    var adjustPadResult = VI.AutoAdjustROIFOV.AdjustPad(mImage, padBoundInFOV, new Hsv(145, 110, 130), new Hsv(255, 255, 255), mModel.Gerber.FOVs[id].ROI);
                    for (int i = 0; i < padInFOV.Count; i++)
                    {
                        System.Drawing.Rectangle bounding = padInFOV[i].Bouding;
                        System.Drawing.Point[] cntPoint = padInFOV[i].Contour;
                        bounding.X += adjustPadResult[i].X;
                        bounding.Y += adjustPadResult[i].Y;
                        for (int j = 0; j < cntPoint.Length; j++)
                        {
                            cntPoint[j].X += adjustPadResult[i].X;
                            cntPoint[j].Y += adjustPadResult[i].Y;
                        }
                        padInFOV[i].BoudingAdjust = bounding;
                        padInFOV[i].ContourAdjust = cntPoint;
                    }

                    mAdjustPad = false;
                }
                mModel.Gerber.ProcessingGerberImage.ROI = new System.Drawing.Rectangle();
                ShowDetail();
            }
        }

        private void btAutoAdjust_Click(object sender, RoutedEventArgs e)
        {
            Thread auto = new Thread(() => {
                if(mAnchorFOV != null)
                {
                    for (int i = 0; i < mAnchorFOV.Length; i++)
                    {
                        this.Dispatcher.Invoke(() => {

                            cbFOV.SelectedIndex = i;
                        });
                        btAdjust_Click(null, null);
                        Thread.Sleep(500);
                    }
                }
                MessageBox.Show("Done!");
            });
            auto.Start();
        }

        private void btAutoAdjustPad_Click(object sender, RoutedEventArgs e)
        {
            Thread auto = new Thread(() => {
                if (mAnchorFOV != null)
                {
                    for (int i = 0; i < mAnchorFOV.Length; i++)
                    {
                        mAdjustPad = true;
                        this.Dispatcher.Invoke(() => {

                            cbFOV.SelectedIndex = i;
                        });
                        btAdjust_Click(null, null);
                        Thread.Sleep(500);
                    }
                }
                MessageBox.Show("Done!");
            });
            auto.Start();
        }

        private void btLoad_Click(object sender, RoutedEventArgs e)
        {
            mPlcComm.Login();
            mPlcComm.Set_Load_Product();
            mPlcComm.Logout();
        }

        private void btUnload_Click(object sender, RoutedEventArgs e)
        {
            mPlcComm.Login();
            mPlcComm.Set_Unload_Product();
            mPlcComm.Logout();
        }

        private void btAdjustPad_Click(object sender, RoutedEventArgs e)
        {
            int id = -1;
            mAdjustPad = true;
            this.Dispatcher.Invoke(() =>
            {
                id = cbFOV.SelectedIndex;
            });
            if (id > -1)
            {
                System.Drawing.Rectangle ROI = mModel.Gerber.FOVs[id].ROI;
                mImage.ROI = ROI;
                var modelFov = mModel.FOV;
                System.Drawing.Rectangle ROIGerber = new System.Drawing.Rectangle(
                    mAnchorROIGerber[id].X - modelFov.Width / 2, mAnchorROIGerber[id].Y - modelFov.Height / 2,
                    modelFov.Width, modelFov.Height);
                if (mAdjustPad)
                {
                    List<System.Drawing.Rectangle> padBoundInFOV = new List<System.Drawing.Rectangle>();
                    List<PadItem> padInFOV = new List<PadItem>();
                    for (int i = 0; i < mModel.Gerber.PadItems.Count; i++)
                    {
                        PadItem item = mModel.Gerber.PadItems[i];
                        if (item.FOVs.Count > 0)
                        {
                            if (item.FOVs[0] == id)
                            {
                                System.Drawing.Rectangle padBound = item.Bouding;
                                
                                padBound.X -= ROIGerber.X;
                                padBound.Y -= ROIGerber.Y;
                                padBoundInFOV.Add(padBound);
                                padInFOV.Add(mModel.Gerber.PadItems[i]);
                            }
                        }
                    }
                    var adjustPadResult = VI.AutoAdjustROIFOV.AdjustPad(mImage, padBoundInFOV, new Hsv(145, 110, 130), new Hsv(255, 255, 255), mModel.Gerber.FOVs[id].ROI);
                    for (int i = 0; i < padInFOV.Count; i++)
                    {
                        System.Drawing.Rectangle bounding = new System.Drawing.Rectangle(padInFOV[i].Bouding.X, padInFOV[i].Bouding.Y, padInFOV[i].Bouding.Width, padInFOV[i].Bouding.Height);
                        System.Drawing.Point[] cntPoint = new System.Drawing.Point[padInFOV[i].Contour.Length];
                        bounding.X += adjustPadResult[i].X;
                        bounding.Y += adjustPadResult[i].Y;
                        for (int j = 0; j < cntPoint.Length; j++)
                        {
                            cntPoint[j] = new System.Drawing.Point(padInFOV[i].Contour[j].X + adjustPadResult[i].X, padInFOV[i].Contour[j].Y + adjustPadResult[i].Y);
                        }
                        padInFOV[i].BoudingAdjust = bounding;
                        padInFOV[i].ContourAdjust = cntPoint;
                    }
                    mAdjustPad = false;
                }
                ShowDetail();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (mCamera != null)
            {
                if (mCamera.IsGrab)
                {
                    mCamera.StopGrabbing();
                }
                if (mCamera.IsOpen)
                {
                    mCamera.Close();
                }
                mCamera = null;
            }
            if (mLight != null)
            {
                if (mLight.Serial.IsOpen)
                {
                    mLight.Close();
                    mLight = null;
                }
            }
        }
    }
}
