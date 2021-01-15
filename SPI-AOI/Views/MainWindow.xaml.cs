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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms.DataVisualization.Charting;
using System.Threading;
using SPI_AOI.Models;
using SPI_AOI.Devices;
using System.IO;
using System.Diagnostics;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using NLog;
using System.Collections.Specialized;

namespace SPI_AOI.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        System.Timers.Timer mTimer = new System.Timers.Timer(20);
        System.Timers.Timer mTimerCheckStatus = new System.Timers.Timer(50);
        Properties.Settings mParam = Properties.Settings.Default;
        Logger mLog = Heal.LogCtl.GetInstance();
        List<Utils.SummaryInfo> mSummary = new List<Utils.SummaryInfo>();
        Utils.FOVDisplayInfo mFOVDisplay = new Utils.FOVDisplayInfo();
        CalibrateInfo mCalibImage = CalibrateLoader.GetIntance();
        DB.Result mMyDBResult = new DB.Result();
        PLCComm mPlcComm = new PLCComm();
        IOT.HikCamera mCamera = null;
        DKZ224V4ACCom mLight = null;
        Image<Bgr, byte> mImageGraft = null;
        List<System.Drawing.Rectangle> mROIFOVImage = new List<System.Drawing.Rectangle>();
        List<Utils.PadErrorDetail> mPadErrorDetails = new List<Utils.PadErrorDetail>();
        bool mIsRunning = false;
        bool mIsProcessing = false;
        bool mIsInTimer = false;
        bool mIsInCheckTimer = false;
        bool mPingPLCOK = true;
        bool mIsCheck = true;
        bool mIsLoaded = false;
        bool mIsShowError = false;
        Model mModel = null;
        public MainWindow()
        {
            InitializeComponent();
            LoadUI();
            mIsLoaded = true;
        }
        private void Window_Initialized(object sender, EventArgs e)
        {
            InitSummary();
            dgwSummary.ItemsSource = mSummary;
            dgwSummary.Items.Refresh();
            LoadModelsName();
            UpdateStatus(Utils.LabelMode.PLC, Utils.LabelStatus.READY);
            UpdateStatus(Utils.LabelMode.DOOR, Utils.LabelStatus.CLOSED);
            UpdateRunningMode();
            UpdateStatus(Utils.LabelMode.MACHINE_STATUS, Utils.LabelStatus.READY);
            UpdateStatus(Utils.LabelMode.PRODUCT_STATUS, Utils.LabelStatus.READY);
            UpdatePanelPosition(0);
            mTimerCheckStatus.Elapsed += OnCheckStatusEvent;
            mTimerCheckStatus.Enabled = true;
            ShowError(false);
            //NameValueCollection data = new NameValueCollection();
            //data.Add("Type", "Segment");
            //data.Add("VI_", "True");
            //data.Add("FOV", (1 + 1).ToString());
            //data.Add("Debug", Convert.ToString(mParam.Debug));
            //string fileName = @"D:\Save\2021_01_08\TIME(10_24_40)_._SN(NOT FOUND)\Image_2).png";

            //VI.ServiceResults serviceResults = VI.ServiceComm.Sendfile(mParam.ServiceURL, new string[] { fileName }, data, true);
        }
        public void LoadUI()
        {
            btReloadModelStatistical_Click(null, null);
        }
        private void OnMainEvent(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (mIsInTimer)
                return;
            mIsInTimer = true;
            System.Timers.Timer timer = sender as System.Timers.Timer;
            timer.Enabled = false;
            int val = mPlcComm.Get_Has_Product_Top();
            if(val == 1 && !mIsShowError)
            {
                mIsProcessing = true;
                ResetUI();
                UpdateStatus(Utils.LabelMode.MACHINE_STATUS, Utils.LabelStatus.PROCESSING);
                UpdateStatus(Utils.LabelMode.PRODUCT_STATUS, Utils.LabelStatus.PROCESSING);
                int result = Processing();
                mIsProcessing = false; 
                mPlcComm.Reset_Has_Product_Top();
                if(result == 0)
                {
                    mPlcComm.Set_Pass();
                    UpdateStatus(Utils.LabelMode.PRODUCT_STATUS, Utils.LabelStatus.PASS);
                    
                }
                else
                {
                    mPlcComm.Set_Fail();
                    UpdateStatus(Utils.LabelMode.PRODUCT_STATUS, Utils.LabelStatus.FAIL);
                    ShowError(true);
                }

                GC.Collect();
            }
            mIsInTimer = false;
            timer.Enabled = mIsRunning;
        }
        
        private Utils.MarkAdjust CaptureMark(string ID, string SavePath, bool LightStrobe)
        {
            System.Drawing.Point[] markPointXYPLC = mModel.GetPLCMarkPosition();
            Utils.MarkAdjust markAdjust = new Utils.MarkAdjust();
            PadItem[] PadMark = new PadItem[2];
            for (int i = 0; i < 2; i++)
            {
                PadMark[i] = mModel.Gerber.PadItems[mModel.Gerber.MarkPoint.PadMark[i]];
            }
            System.Drawing.Point[] markPointImage = new System.Drawing.Point[2];
            double matchingScore = mModel.Gerber.MarkPoint.Score;
            for (int i = 0; i < markPointXYPLC.Length; i++)
            {
                System.Drawing.Point mark = markPointXYPLC[i];
                int x = mark.X;
                int y = mark.Y;
                mLog.Info(string.Format("{0}, Position Name : {1},  X = {2}, Y = {3}", "Moving TOP Axis", "Mark " + (i+1).ToString(), x, y));
                using (Image<Bgr, byte> image = VI.CaptureImage.CaptureFOV(mPlcComm, mCamera, mLight, mark, LightStrobe, 200))
                {
                    if (image != null)
                    {
                        System.Drawing.Rectangle ROI = mModel.GetRectROIMark();
                        
                        string fileName = string.Format("{0}//Mark_{1}.png", SavePath, i + 1);
                        image.ROI = ROI;
                        ShowMarkImage(image.Bitmap, i);
                        using (Image<Gray, byte> imgGray = new Image<Gray, byte>(image.Size))
                        {
                            CvInvoke.CvtColor(image, imgGray, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);
                            CvInvoke.Threshold(imgGray, imgGray, mModel.Gerber.MarkPoint.ThresholdValue, 255, Emgu.CV.CvEnum.ThresholdType.Binary);
                            using (VectorOfPoint padContour = new VectorOfPoint(PadMark[i].Contour))
                            {
                                var markInfo = Mark.MarkDetection(imgGray, padContour);
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
                                                markPointImage[i] = new System.Drawing.Point(Convert.ToInt32(mm.M10 / mm.M00), Convert.ToInt32(mm.M01 / mm.M00));
                                            }
                                            CvInvoke.DrawContours(image, contours, -1, new MCvScalar(255, 0, 0), 2);
                                            ShowMarkImage(image.Bitmap, i);
                                        }
                                    }
                                    if (i == 1)
                                    {
                                        markAdjust.Status = Utils.ActionStatus.Successfully;
                                        System.Drawing.Point ct = new System.Drawing.Point(image.Width / 2, image.Height / 2);
                                        markAdjust.X = ct.X - markPointImage[0].X;
                                        markAdjust.Y = ct.Y - markPointImage[0].Y;
                                    }
                                }
                                else
                                {
                                    mLog.Info(string.Format("Score matching is lower score standard... {0} < {1}", realScore, matchingScore));
                                    markAdjust.Status = Utils.ActionStatus.Fail;
                                    break;
                                }
                            }
                            
                        }
                        CvInvoke.Imwrite(fileName, image);
                        Thread isDB = new Thread(() => {
                            mMyDBResult.InsertNewImage(ID, DateTime.Now, fileName, ROI, new System.Drawing.Rectangle(), "MARK");
                        });
                        isDB.Start();
                    }
                    else
                    {
                        mLog.Info(string.Format("Cant Capture image in Mark : {0}", i + 1));
                        markAdjust.Status = Utils.ActionStatus.Fail;
                        break;
                    }
                }
            }
            return markAdjust;
        }
        private void ShowMarkImage(System.Drawing.Bitmap bm, int id)
        {
            this.Dispatcher.Invoke(() => {
                BitmapSource bms = Utils.Convertor.Bitmap2BitmapSource(bm);
                if (id == 0)
                {
                    imbMark1.Source = bms;
                }
                else
                {
                    imbMark2.Source = bms;
                }
            });
        }
        private int CaptureFOV(string ID, string SavePath, int XDeviation, int YDeviation, double Angle, bool LightStrobe)
        {
            int result = -1;
            System.Drawing.Point[] xyAxisPosition = mModel.GetPulseXYFOVs();
            System.Drawing.Point[] Fovs = mModel.GetAnchorsFOV();
            mModel.Gerber.ProcessingGerberImage.ROI = mModel.Gerber.ROI;
            Thread[] publishThread = new Thread[Fovs.Length];
            bool[] threadStatus = new bool[Fovs.Length];
            ReleasePadErrorAndFOVImage();
            if(mImageGraft != null)
            {
                mImageGraft.Dispose();
                mImageGraft = null;
            }
            mImageGraft = new Image<Bgr, byte>(mModel.Gerber.ROI.Size);
            using (Image<Gray, byte> maskSegmentGraft = new Image<Gray, byte>(mModel.Gerber.ROI.Size))
            using (Image<Gray, byte> imgGerber = mModel.Gerber.ProcessingGerberImage.Copy())
            {
                for (int i = 0; i < xyAxisPosition.Length; i++)
                {
                    System.Drawing.Point fov = xyAxisPosition[i];
                    int x = fov.X;
                    int y = fov.Y;
                    mLog.Info(string.Format("{0}, Position Name : {1},  X = {2}, Y = {3}", "Moving TOP Axis", "FOV " + (i + 1).ToString(), x, y));
                    using (Image<Bgr, byte> image = VI.CaptureImage.CaptureFOV(mPlcComm, mCamera, mLight, fov, LightStrobe, 300))
                    {
                        if (image != null)
                        {
                            double angle = -mModel.AngleAxisCamera - Angle;
                            using (Image<Bgr, byte> imgRotated = ImageProcessingUtils.ImageRotation(image, new System.Drawing.Point(image.Width / 2, image.Height / 2), angle * Math.PI / 180.0))
                            using (Image<Bgr, byte> imgTransform = ImageProcessingUtils.ImageTransformation(image, XDeviation, YDeviation))
                            {
                                SetDisplayFOV(i);
                                var modelFov = mModel.FOV;
                                System.Drawing.Rectangle ROI = mModel.Gerber.FOVs[i].ROI;
                                System.Drawing.Rectangle ROIGerber = new System.Drawing.Rectangle(
                                    Fovs[i].X - modelFov.Width / 2, Fovs[i].Y - modelFov.Height / 2,
                                    modelFov.Width, modelFov.Height);
                                imgTransform.ROI = ROI;
                                imgGerber.ROI = ROIGerber;
                                mImageGraft.ROI = ROIGerber;
                                imgTransform.CopyTo(mImageGraft);
                                string fileName = string.Format("{0}//Image_{1}.png", SavePath, i + 1);
                                CvInvoke.Imwrite(fileName, imgTransform);
                                publishThread[i] = new Thread(() => {
                                    int id = i;
                                    System.Drawing.Rectangle ROIGraft = ROIGerber;
                                    NameValueCollection data = new NameValueCollection();
                                    data.Add("Type", "Segment");
                                    data.Add("VI_", "True");
                                    data.Add("FOV", (id + 1).ToString());
                                    data.Add("Debug", Convert.ToString(mParam.Debug));
                                    if(id > 0)
                                    {
                                        publishThread[i - 1].Join();
                                    }
                                    VI.ServiceResults serviceResults = VI.ServiceComm.Sendfile(mParam.ServiceURL, new string[] { fileName }, data, true);
                                    if (serviceResults != null)
                                    {
                                        lock (serviceResults)
                                        {
                                            threadStatus[id] = true;
                                        }
                                        lock (maskSegmentGraft)
                                        {
                                            maskSegmentGraft.ROI = ROIGraft;
                                            CvInvoke.BitwiseOr(maskSegmentGraft, serviceResults.ImgMask, maskSegmentGraft);
                                            maskSegmentGraft.ROI = new System.Drawing.Rectangle();
                                        }
                                    }
                                });
                                publishThread[i].Start();
                                imgGerber.ROI = System.Drawing.Rectangle.Empty;
                                mImageGraft.ROI = System.Drawing.Rectangle.Empty;
                                this.Dispatcher.Invoke(() =>
                                {
                                    BitmapSource bms = Utils.Convertor.Bitmap2BitmapSource(imgTransform.Bitmap);
                                    ImbCameraView.Source = bms;
                                    lbcountFovs.Content = (i + 1).ToString();
                                });
                                Thread isDB = new Thread(() =>
                                {
                                    mMyDBResult.InsertNewImage(ID, DateTime.Now, fileName, ROI, ROIGerber, "FOV");
                                });
                                isDB.Start();
                                mROIFOVImage.Add(ROIGerber);
                            }
                        }
                        else
                        {
                            mLog.Info(string.Format("Cant Capture image in FOV : {0}", i + 1));
                            result = -2;
                            break;
                        }
                    }
                }
                if(result != -2)
                {
                    mLog.Info("waiting predict");
                    for (int i = 0; i < publishThread.Length; i++)
                    {
                        publishThread[i].Join();
                    }
                    mLog.Info("write image");
                    string fileNameGraft = string.Format("{0}//Image_Graft.png", SavePath);
                    CvInvoke.Imwrite(fileNameGraft, mImageGraft);
                    if (mParam.Debug)
                    {
                        string fileNameSegmentGraft = string.Format("{0}//Image_Segment_Graft.png", SavePath);
                        CvInvoke.Imwrite(fileNameSegmentGraft, maskSegmentGraft);
                        string fileNameGerberGraft = string.Format("{0}//Image_Gerber.png", SavePath);
                        CvInvoke.Imwrite(fileNameGerberGraft, imgGerber);
                    }
                    mLog.Info("predict");
                    Image<Gray, byte> maskReleaseNoise = VI.Predictor.ReleaseNoise(maskSegmentGraft);
                    Utils.PadSegmentInfo[] pads = VI.Predictor.GetPadSegmentInfo(maskReleaseNoise, mModel.Gerber.ROI);
                    mLog.Info("PadSegmentInfo");
                    Utils.PadErrorDetail[] padError = VI.Predictor.ComparePad(mModel, pads);
                    mLog.Info(string.Format("has {0} pad error...", padError.Length));
                    padError = VI.Predictor.GetImagePadError(mImageGraft, padError, mModel.Gerber.ROI, mParam.LIMIT_SHOW_ERROR);
                    for (int i = 0; i < padError.Length; i++)
                    {
                        mPadErrorDetails.Add(padError[i]);
                    }
                    if (mPadErrorDetails.Count > 0)
                    {
                        result = -1;
                    }
                    else
                    {
                        result = 0;
                        if (mImageGraft != null)
                        {
                            mImageGraft.Dispose();
                            mImageGraft = null;
                        }
                    }
                    mLog.Info("show error");
                }
            }
            mModel.Gerber.ProcessingGerberImage.ROI = System.Drawing.Rectangle.Empty;
            return result;
        }
        
        private int Processing()
        {
            int result = -1;
            string date = DateTime.Now.ToString("yyyy_MM_dd");
            string time = DateTime.Now.ToString("HH_mm_ss");
            string sn = "NOT FOUND";
            string savePath = mParam.SAVE_IMAGE_PATH + "\\" + date + "\\TIME(" + time + ")_._SN(" + sn + ")";
            string ID = mMyDBResult.GetNewID();
            if(!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }
            bool lightStrobe = !Convert.ToBoolean(mParam.LIGHT_MODE);
            
            mLight.SetFour(mModel.HardwareSettings.LightIntensity[0],
                mModel.HardwareSettings.LightIntensity[1],
                mModel.HardwareSettings.LightIntensity[2],
                mModel.HardwareSettings.LightIntensity[3]);
            mCamera.SetParameter(IOT.KeyName.ExposureTime, (float)mModel.HardwareSettings.ExposureTime);
            if (!lightStrobe)
            {
                mLight.ActiveFour(1, 1, 1, 1);
            }
            Utils.MarkAdjust markAdjustInfo = CaptureMark(ID,savePath, lightStrobe);
            if(markAdjustInfo.Status == Utils.ActionStatus.Successfully)
            {
                mLight.SetFour(mParam.LIGHT_VI_DEFAULT_INTENSITY_CH1,
                mParam.LIGHT_VI_DEFAULT_INTENSITY_CH2,
                mParam.LIGHT_VI_DEFAULT_INTENSITY_CH3,
                mParam.LIGHT_VI_DEFAULT_INTENSITY_CH4);
                mCamera.SetParameter(IOT.KeyName.ExposureTime, (float)mParam.CAMERA_VI_EXPOSURE_TIME);
                int status = CaptureFOV(ID, savePath, markAdjustInfo.X, markAdjustInfo.Y, markAdjustInfo.Angle, lightStrobe);
                if (!lightStrobe)
                {
                    mLight.ActiveFour(0, 0, 0, 0);
                }
                bool pass = false;
                if (status != -2)
                {
                    // capture fail
                    if (mParam.RUNNING_MODE == 0 || mParam.RUNNING_MODE == 1)
                    {
                        // control run || test
                        if (status == 0)
                            pass = true;
                        else if (status == -1)
                        {
                            pass = false;
                        }
                    }
                    else if (mParam.RUNNING_MODE == 2)
                    {
                        // bypass
                        pass = true;

                    }
                    // insert db
                    string runningMode = "";
                    switch (mParam.RUNNING_MODE)
                    {
                        case 0:
                            runningMode = Utils.LabelStatus.CONTROL_RUN.ToString();
                            break;
                        case 1:
                            runningMode = Utils.LabelStatus.TEST.ToString();
                            break;
                        case 2:
                            runningMode = Utils.LabelStatus.BY_PASS.ToString();
                            break;
                        default:
                            break;
                    }
                    string viResult = pass ? "PASS" : "FAIL";
                    result = pass ? 0 : -1;
                    mMyDBResult.InsertNewProduct(ID, mModel.Name, DateTime.Now, "NOT FOUND", runningMode, viResult);
                }
                else
                {
                    result = -2;
                }
            }
            else
            {
                result = -2;
            }
            SetDisplayFOV(-1);
            UpdateCountStatistical();
            return result;
        }
        public void ReleasePadErrorAndFOVImage()
        {
            for (int i = 0; i < mPadErrorDetails.Count; i++)
            {
                if(mPadErrorDetails[i] != null)
                {
                    mPadErrorDetails[i].Dispose();
                    mPadErrorDetails[i] = null;
                }
            }
            mPadErrorDetails.Clear();
        }
        private void OnCheckStatusEvent(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (mIsInCheckTimer)
                return;
            mIsInCheckTimer = true;
            mTimerCheckStatus.Enabled = false;
            int ping = mPlcComm.Ping();
            if (ping != 0)
            {
                UpdateStatus(Utils.LabelMode.PLC, Utils.LabelStatus.FAIL);
                string msg = string.Format("Ping PLC failed, IP :{0}:{1}...", mParam.PLC_IP, mParam.PLC_PORT);
                mLog.Error(msg);
                mTimerCheckStatus.Interval = 1000 * 60;
                if(mPingPLCOK)
                {
                    MessageBox.Show(msg, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                    mPingPLCOK = false;
                }
            }
            else
            {
                mTimerCheckStatus.Interval = 100;
                mPingPLCOK = true;
                Thread.Sleep(20);
                if (!mIsCheck)
                    return;
                int valDoor = mPlcComm.Get_Door_Status();
                UpdateStatus(Utils.LabelMode.PLC, Utils.LabelStatus.OK);
                if (valDoor == 0)
                {
                    UpdateStatus(Utils.LabelMode.DOOR, Utils.LabelStatus.CLOSED);
                }
                else if (valDoor == 1)
                {
                    UpdateStatus(Utils.LabelMode.DOOR, Utils.LabelStatus.OPEN);
                }
                else
                {
                    UpdateStatus(Utils.LabelMode.DOOR, Utils.LabelStatus.WARNING);
                    mPingPLCOK = false;
                }
                Thread.Sleep(20);
                if (!mIsCheck)
                    return;
                int valPanelPosition = mPlcComm.Get_PanelPosition_Status();
                if (valPanelPosition >= 0 && valPanelPosition < 8)
                {
                    UpdatePanelPosition(valPanelPosition);
                }
                Thread.Sleep(20);
                if (!mIsCheck)
                    return;
                int valMachineStatus = mPlcComm.Get_Machine_Status();
                if (valMachineStatus == 0)
                {
                    UpdateStatus(Utils.LabelMode.MACHINE_STATUS, Utils.LabelStatus.STOP);
                }
                else
                {
                    if (valMachineStatus == 1 && mIsProcessing)
                    {
                        UpdateStatus(Utils.LabelMode.MACHINE_STATUS, Utils.LabelStatus.PROCESSING);
                    }
                    else if (!mIsProcessing && valPanelPosition == 0)
                    {
                        UpdateStatus(Utils.LabelMode.MACHINE_STATUS, Utils.LabelStatus.IDLE);
                    }
                    else if (!mIsProcessing && valPanelPosition > 0 && valPanelPosition < 8)
                    {
                        UpdateStatus(Utils.LabelMode.MACHINE_STATUS, Utils.LabelStatus.RUNNING);
                    }
                }
            }
            mIsInCheckTimer = false;
            mTimerCheckStatus.Enabled = mIsCheck;
        }
        public void SetImageToImb(Image imb, System.Drawing.Bitmap bm)
        {
            this.Dispatcher.Invoke(() => {

                BitmapSource bms = null;
                if(bm != null)
                    bms = Utils.Convertor.Bitmap2BitmapSource(bm);
                imb.Source = bms;
            });
        }
        public void ResetUI()
        {
            SetImageToImb(imbMark1, null);
            SetImageToImb(imbMark2, null);
            SetImageToImb(ImbCameraView, null);
            UpdateStatus(Utils.LabelMode.PRODUCT_STATUS, Utils.LabelStatus.READY);
            this.Dispatcher.Invoke(() => {
                lbcountFovs.Content = "--";
            });
        }

        private void btReloadModelStatistical_Click(object sender, RoutedEventArgs e)
        {
            string[] modelNames = mMyDBResult.GetModelName();
            string[] modelMachine = Model.GetModelNames();
            this.Dispatcher.Invoke(() =>
            {
                string selected = "_____________";
                if (cbModelStatistical.SelectedIndex >= 0)
                {
                    selected = cbModelStatistical.SelectedItem.ToString();
                }
                cbModelStatistical.Items.Clear();
                for (int i = 0; i < modelNames.Length; i++)
                {
                    cbModelStatistical.Items.Add(modelNames[i]);
                }
                for (int i = 0; i < modelMachine.Length; i++)
                {
                    if(!cbModelStatistical.Items.Contains(modelMachine[i]))
                        cbModelStatistical.Items.Add(modelMachine[i]);
                }
                if (cbModelStatistical.Items.Contains(selected))
                {
                    cbModelStatistical.SelectedItem = selected;
                }
            });
            UpdateCountStatistical();
        }
        public void UpdateCountStatistical()
        {
            int selectId = -1;
            this.Dispatcher.Invoke(() => {
                selectId = cbModelStatistical.SelectedIndex;
            });
            if (selectId >= 0)
            {
                string modelName = string.Empty;
                this.Dispatcher.Invoke(() => {
                    modelName = cbModelStatistical.SelectedItem.ToString();
                
                    DateTime now = DateTime.Now;
                    DateTime endTime = now;
                    DateTime startTime = now;
                    if(rbShift.IsChecked == true)
                    {
                        DateTime lastDay = now - new TimeSpan(24);
                        DateTime endLastDay = new DateTime(lastDay.Year, lastDay.Month, lastDay.Day, 19, 30, 0);
                        TimeSpan subTime = now - endLastDay;
                        if (subTime.Hours < 12)
                        {
                            startTime = endLastDay;
                        }
                        else if (endLastDay.Hour >= 12 && endLastDay.Hour < 24)
                        {
                            startTime = new DateTime(now.Year, now.Month, now.Day, 7, 30, 0);
                        }
                        else
                        {
                            startTime = new DateTime(now.Year, now.Month, now.Day, 19, 30, 0);
                        }
                    }
                    else if(rbDay.IsChecked == true)
                    {
                        startTime = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
                    }
                    else if(rbTotal.IsChecked == true)
                    {
                        startTime = new DateTime(2020, 1, 1, 0, 0, 0);
                    }
                    int pass = mMyDBResult.CountPass(modelName, startTime, endTime);
                    int fail = mMyDBResult.CountFail(modelName, startTime, endTime);
                    UpdateChartCount(chartYeildRate, txtPass, txtFail, pass, fail);
                });
                
            }
            else
            {
                UpdateChartCount(chartYeildRate, txtPass, txtFail, 0, 0);
            }
        }
        private void UpdateChartCount(Chart Chart, TextBox TxtPass, TextBox TxtFail, int Pass, int Fail)
        {
            int cvPass = Pass == 0 && Fail == 0 ? 1 : Pass;
            int cvFail = Fail;
            double ratePass = Math.Round((double)cvPass * 100 / (cvPass + cvFail), 1);
            double rateFail = Math.Round((double)cvFail * 100 / (cvPass + cvFail), 1);
            this.Dispatcher.Invoke(() => {
                Chart.BackColor = System.Drawing.Color.Transparent;
                TxtPass.Text = Pass.ToString();
                TxtFail.Text = Fail.ToString();
                Chart.Series["YeildRate"].Points[0].SetValueXY(ratePass.ToString() + "%", cvPass);
                Chart.Series["YeildRate"].Points[1].SetValueXY(rateFail.ToString() + "%", cvFail);
                Chart.Update();
            });
        }
        public void InitSummary()
        {
            mSummary.Add(new Utils.SummaryInfo() { Field = "Area Hight", Count = 0, PPM = 0 });
            mSummary.Add(new Utils.SummaryInfo() { Field = "Area Low", Count = 0, PPM = 0 });
            mSummary.Add(new Utils.SummaryInfo() { Field = "Shift X", Count = 0, PPM = 0 });
            mSummary.Add(new Utils.SummaryInfo() { Field = "Shift X Area", Count = 0, PPM = 0 });
            mSummary.Add(new Utils.SummaryInfo() { Field = "Shift Y", Count = 0, PPM = 0 });
            mSummary.Add(new Utils.SummaryInfo() { Field = "Shift Y Area", Count = 0, PPM = 0 });
        }
        private Views.UserManagement.UserType Login()
        {
            Views.UserManagement.LoginWindow loginWD = new UserManagement.LoginWindow();
            loginWD.ShowDialog();
            return loginWD.UserType;
        }
        private void btModelManager_Click(object sender, RoutedEventArgs e)
        {
            var userType = Login();
            if(userType == UserManagement.UserType.Admin ||
                userType == UserManagement.UserType.Designer ||
                userType == UserManagement.UserType.Engineer)
            {
                ModelManagement.DashBoard dbWD = new ModelManagement.DashBoard();
                dbWD.ShowDialog();
            }
            LoadModelsName();
        }
        private void UpdateResult()
        {

        }
        private void btPLCConfig_Click(object sender, RoutedEventArgs e)
        {
            var userType = Login();
            if (userType == UserManagement.UserType.Admin ||
                userType == UserManagement.UserType.Designer ||
                userType == UserManagement.UserType.Engineer)
            {
                MainConfigWindow.PLCBitconfigForm mainConfig = new MainConfigWindow.PLCBitconfigForm();
                mainConfig.ShowDialog();
            }
        }

        private void btMachineIssue_Click(object sender, RoutedEventArgs e)
        {
            var userType = Login();
            if (userType == UserManagement.UserType.Admin ||
                userType == UserManagement.UserType.Designer ||
                userType == UserManagement.UserType.Engineer)
            {
                 Heal.UI.MachineIssueForm machineIssueForm = new Heal.UI.MachineIssueForm();
                machineIssueForm.ShowDialog();
            }
        }
        private void LoadModelsName()
        {
            string selected = Convert.ToString(cbModelsName.SelectedItem);
            cbModelsName.Items.Clear();
            string[] modelNames = Model.GetModelNames();
            if (modelNames != null)
            {
                for (int i = 0; i < modelNames.Length; i++)
                {
                    cbModelsName.Items.Add(modelNames[i]);
                }
            }
            if (modelNames.Contains(selected))
            {
                cbModelsName.SelectedItem = selected;
            }
        }
        private void UpdateRunningMode()
        {
            switch (mParam.RUNNING_MODE)
            {
                case 0:
                    UpdateStatus(Utils.LabelMode.RUNNING_MODE, Utils.LabelStatus.CONTROL_RUN);
                    break;
                case 1:
                    UpdateStatus(Utils.LabelMode.RUNNING_MODE, Utils.LabelStatus.TEST);
                    break;
                case 2:
                    UpdateStatus(Utils.LabelMode.RUNNING_MODE, Utils.LabelStatus.BY_PASS);
                    break;
                default:
                    break;
            }

        }
        private void UpdateStatus(Utils.LabelMode Label, Utils.LabelStatus Status)
        {
            SolidColorBrush colorMode = Utils.MyState.GetColorStatus(Status);
            string strMode = Utils.MyState.GetStringStatus(Status);
            this.Dispatcher.Invoke(() => {
                switch (Label)
                {
                    case Utils.LabelMode.PLC:
                        bdPLC.Background = colorMode;
                        lbPLC.Content = strMode;
                        break;
                    case Utils.LabelMode.DOOR:
                        bdDoor.Background = colorMode;
                        lbDoor.Content = strMode;
                        break;
                    case Utils.LabelMode.RUNNING_MODE:
                        bdRunningMode.Background = colorMode;
                        lbRunningMode.Content = strMode;
                        break;
                    case Utils.LabelMode.MACHINE_STATUS:
                        bdMachineStatus.Background = colorMode;
                        lbMachineStatus.Content = strMode;
                        break;
                    case Utils.LabelMode.PRODUCT_STATUS:
                        lbProductStatus.Foreground = colorMode;
                        lbProductStatus.Content = strMode;
                        lbProductStatus.Opacity = 1;
                        if (Status == Utils.LabelStatus.PROCESSING | Status == Utils.LabelStatus.READY)
                        {
                            lbProductStatus.Opacity = 0;
                        }
                        break;
                    default:
                        break;
                }
            });
            
        }
        
        private void LoadDetails()
        {
            this.Dispatcher.Invoke(() => {
                lbModelName.Content = mModel.Name;
                lbLoadTime.Content = DateTime.Now.ToString("HH:mm:ss   dd/MM/yyyy");
                lbFovs.Content = mModel.Gerber.FOVs.Count.ToString() + " FOVs";
                lbGerberFile.Content = mModel.Gerber.FileName;
                lbTotalCountFovs.Content = mModel.Gerber.FOVs.Count.ToString();
                lbNoPad.Content = mModel.Gerber.PadItems.Count.ToString() + " Pad";
                cbModelStatistical.SelectedItem = cbModelsName.SelectedItem;
            });
        }
        private void ResetDetails()
        {
            this.Dispatcher.Invoke(() => {
                lbModelName.Content = "-----";
                lbLoadTime.Content = "-----";
                lbFovs.Content = "-----";
                lbGerberFile.Content = "-----";
                lbCircleTime.Content = "-----";
                lbTotalCountFovs.Content = "0";
            });
            
        }
        private void UpdatePanelPosition(bool Position1, bool Position2, bool Position3)
        {
            this.Dispatcher.Invoke(() => {
                rectPosition1.Visibility = Position1 ? Visibility.Visible : Visibility.Hidden;
                rectPosition2.Visibility = Position2 ? Visibility.Visible : Visibility.Hidden;
                rectPosition3.Visibility = Position3 ? Visibility.Visible : Visibility.Hidden;
            });
        }
        private void UpdatePanelPosition(int Val)
        {
            int val1 = (Val >> 2) % 2;
            int val2 = (Val >> 1) % 2;
            int val3 = Val % 2;
            UpdatePanelPosition(val3 == 1, val2 == 1, val1 == 1);
        }
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateFOVDisplay();
        }
        private void UpdateFOVDisplay()
        {
            if(mModel != null)
            {
                using (Image<Bgr, byte> imgDigram = mModel.GetDiagramImage())
                {
                    System.Drawing.Point[] anchors = mModel.GetAnchorsDiagram();
                    double imgWidth = imgDigram.Width;
                    double imgHeight = imgDigram.Height;
                    double fovWidth = mModel.FOV.Width;
                    double fovHeight = mModel.FOV.Height;
                    double imbWidth = imbDiagram.ActualWidth;
                    double imbHeight = imbDiagram.ActualHeight;
                    double bdImbWidth = bdImbDiagram.ActualWidth;
                    double bdImbHeight = bdImbDiagram.ActualHeight;
                    double scaleWidth = imbWidth / imgWidth;
                    double scaleHeight = imbHeight / imgHeight;
                    double showDisplayWidth = fovWidth * scaleWidth;
                    double showDisplayHeight = fovHeight * scaleHeight;
                    lock(mFOVDisplay)
                    {
                        mFOVDisplay = new Utils.FOVDisplayInfo();
                        mFOVDisplay.StartPoint = new System.Windows.Point[anchors.Length];
                        mFOVDisplay.Witdh = showDisplayWidth;
                        mFOVDisplay.Height = showDisplayHeight;
                        for (int i = 0; i < mFOVDisplay.StartPoint.Length; i++)
                        {
                            mFOVDisplay.StartPoint[i] = new Point
                                (
                                anchors[i].X * scaleWidth - mFOVDisplay.Witdh / 2 +( bdImbWidth - imbWidth) / 2,
                                anchors[i].Y * scaleHeight - mFOVDisplay.Height / 2 + (bdImbHeight - imbHeight) / 2
                                );
                        }
                    }
                }
            }
        }
        private void SetDisplayFOV(int id)
        {
            this.Dispatcher.Invoke(() => {
                if (id == -1)
                {
                    bdFOV.Visibility = Visibility.Hidden;
                }
                else
                {
                    bdFOV.Width = mFOVDisplay.Witdh;
                    bdFOV.Height = mFOVDisplay.Height;
                    bdFOV.Margin = new Thickness(mFOVDisplay.StartPoint[id].X, mFOVDisplay.StartPoint[id].Y, 0, 0);
                    bdFOV.Visibility = Visibility.Visible;
                }
            });
        }
        private void StartRunMode()
        {
            string modelName = cbModelsName.SelectedItem.ToString();
            WaitingForm wait = new WaitingForm("Load model...");
            Thread startThread = new Thread(() => {
                mModel = Model.LoadModelByName(modelName);
                if (mModel == null)
                {
                    ReleaseResource();
                    wait.KillMe = true;
                    MessageBox.Show(string.Format("Cant load {0} model", modelName), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                using (Image<Bgr, byte> imgDigram = mModel.GetDiagramImage())
                {
                    SetImageToImb(imbDiagram, imgDigram.Bitmap);
                    UpdateFOVDisplay();
                    SetDisplayFOV(-1);
                }
                wait.LabelContent = "Connecting to PLC...";
                int ping = mPlcComm.Ping();
                if (ping != 0)
                {
                    ReleaseResource();
                    wait.KillMe = true;
                    UpdateStatus(Utils.LabelMode.PLC, Utils.LabelStatus.FAIL);
                    MessageBox.Show(string.Format("Cant ping to PLC  IP  {0}:{1}", mParam.PLC_IP, mParam.PLC_PORT), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                mPingPLCOK = true;
                OnCheckStatusEvent(mIsInCheckTimer, null);
                wait.LabelContent = "Connecting to Camera...";
                mCamera = MyCamera.GetInstance();
                if (mCamera == null)
                {
                    ReleaseResource();
                    wait.KillMe = true;
                    MessageBox.Show(string.Format("Not found camera!"), "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                    return;
                }
                int stOpenCamera = mCamera.Open();
                if (stOpenCamera != 0)
                {

                    ReleaseResource();
                    wait.KillMe = true;
                    MessageBox.Show(string.Format("Cant open the camera!"), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                mCamera.StartGrabbing();
                mCamera.SetParameter(IOT.KeyName.ExposureTime, (float)mModel.HardwareSettings.ExposureTime);
                mCamera.SetParameter(IOT.KeyName.Gain, (float)mModel.HardwareSettings.Gain);
                wait.LabelContent = "Connecting to Lightsource...";
                mLight = new DKZ224V4ACCom(mParam.LIGHT_COM);
                int stOpenLightCtl = mLight.Open();
                if (stOpenLightCtl != 0)
                {

                    ReleaseResource();
                    MessageBox.Show(string.Format("Cant connect to light source controller!"), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    wait.KillMe = true;
                    return;
                }
                int[] intensity = mModel.HardwareSettings.LightIntensity;
                mLight.SetFour(intensity[0], intensity[1], intensity[2], intensity[3]);
                mLight.ActiveFour(0, 0, 0, 0);
                mPlcComm.Logout();
                int conveyorPulse = mPlcComm.Get_Conveyor();
                if (conveyorPulse != mModel.HardwareSettings.Conveyor)
                {
                    wait.LabelContent = "Moving Conveyor...";
                    mPlcComm.Set_Speed_Run_Conveyor(mParam.RUN_CONVEYOR_SPEED);
                    mPlcComm.Set_Conveyor(Convert.ToInt32(mModel.HardwareSettings.Conveyor));
                    mPlcComm.Set_Write_Coordinates_Finish_Conveyor();

                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    int val = mPlcComm.Get_Go_Coordinates_Finish_Conveyor();
                    while (val != 1 && sw.ElapsedMilliseconds < 180000)
                    {
                        Thread.Sleep(50);
                        val = mPlcComm.Get_Go_Coordinates_Finish_Conveyor();
                    }
                    
                    if (sw.ElapsedMilliseconds > 180000)
                    {
                        MessageBox.Show("Timeout move conveyor, please try again!", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                        ReleaseResource();
                        wait.KillMe = true;
                        return;
                    }
                    Thread.Sleep(5);
                    mPlcComm.Reset_Go_Coordinates_Finish_Conveyor();
                }
                wait.LabelContent = "Init Parameter...";
                mPlcComm.Set_Speed_Run_X_Top(mParam.RUN_X_TOP_SPEED);
                mPlcComm.Set_Speed_Run_Y_Top(mParam.RUN_Y_TOP_SPEED);
                SetButtonRun(Utils.RunMode.START);
                UpdateStatus(Utils.LabelMode.PRODUCT_STATUS, Utils.LabelStatus.READY);
                LoadDetails();
                mIsRunning = true;
                mTimer.Elapsed += OnMainEvent;
                mTimer.Enabled = true;
                mIsInTimer = false;
                wait.KillMe = true;
               
            });
            startThread.Start();
            wait.ShowDialog();
        }
        private void StopRunMode()
        {
            mIsRunning = false;
            ResetUI();
            mLight.ActiveFour(0, 0, 0, 0);
            ReleaseResource();
            ResetDetails();
            SetDisplayFOV(-1);
            SetButtonRun(Utils.RunMode.STOP);
            ShowError(false);
        }
        private void SetButtonRun(Utils.RunMode mode)
        {
            if(mode == Utils.RunMode.START)
            {
                this.Dispatcher.Invoke(() => {
                    FileInfo fi = new FileInfo("icon/stop.png");
                    imbBtRun.Source = new BitmapImage(new Uri(fi.FullName));
                    btRun.ToolTip = "Stop";
                });
            }
            else
            {
                this.Dispatcher.Invoke(() => {
                    FileInfo fi = new FileInfo("icon/start.png");
                    imbBtRun.Source = new BitmapImage(new Uri(fi.FullName));
                    btRun.ToolTip = "Run";
                });
            }
            
        }
        private void ReleaseResource()
        {
            if(mModel != null)
            {
                mModel.Dispose();
                mModel = null;
            }
            if(mCamera != null)
            {
                if(mCamera.IsGrab)
                {
                    mCamera.StopGrabbing();
                }
                if(mCamera.IsOpen)
                {
                    mCamera.Close();
                }
                mCamera = null;
            }
            if(mLight != null)
            {
                if (mLight.Serial.IsOpen)
                {
                    mLight.Close();
                    mLight = null;
                }
            }
            this.Dispatcher.Invoke(() =>
            {
                imbDiagram.Source = null ;
            });
        }
        private void btRun_Click(object sender, RoutedEventArgs e)
        {
            if(!mIsRunning)
            {
                if (cbModelsName.SelectedIndex > -1)
                {
                    StartRunMode();
                }
            }
            else
            {
                StopRunMode();
            }
            
        }
        private void btSetupCamera_Click(object sender, RoutedEventArgs e)
        {

        }
        private void btIOConfig_Click(object sender, RoutedEventArgs e)
        {
            var userType = Login();
            if (userType == UserManagement.UserType.Admin ||
                userType == UserManagement.UserType.Designer ||
                userType == UserManagement.UserType.Engineer)
            {
                Views.MainConfigWindow.IOConfigForm ioForm = new Views.MainConfigWindow.IOConfigForm();
                ioForm.ShowDialog();
                mCalibImage = CalibrateLoader.UpdateInstance();
            }
        }
        private void btAlgorithmSettings_Click(object sender, RoutedEventArgs e)
        {
            var userType = Login();
            if (userType == UserManagement.UserType.Admin ||
                userType == UserManagement.UserType.Designer ||
                userType == UserManagement.UserType.Engineer)
            {
                Views.MainConfigWindow.AlgorithmSettings algorithmForm = new Views.MainConfigWindow.AlgorithmSettings();
                algorithmForm.ShowDialog();
                UpdateRunningMode();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            mIsCheck = false;
            mTimerCheckStatus.Enabled = false;
            Thread.Sleep(500);
        }

        private void btExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void rbShift_Checked(object sender, RoutedEventArgs e)
        {
            if (mIsLoaded)
                UpdateCountStatistical();
        }

        private void rbDay_Checked(object sender, RoutedEventArgs e)
        {
            if (mIsLoaded)
                UpdateCountStatistical();
        }

        private void rbTotal_Checked(object sender, RoutedEventArgs e)
        {
            if(mIsLoaded)
                UpdateCountStatistical();
        }

        private void cbModelStatistical_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateCountStatistical();
        }

        private void btLightCtl_Click(object sender, RoutedEventArgs e)
        {
            var userType = Login();
            if (userType == UserManagement.UserType.Admin ||
                userType == UserManagement.UserType.Designer ||
                userType == UserManagement.UserType.Engineer)
            {
                Views.MainConfigWindow.LightCtlForm lightForm = new Views.MainConfigWindow.LightCtlForm();
                lightForm.ShowDialog();
            }
        }
        private void ShowError(bool status)
        {
            if(status == true)
            {
                mIsShowError = true;
                this.Dispatcher.Invoke(() => {
                    int lm = mPadErrorDetails.Count > mParam.LIMIT_SHOW_ERROR ? mParam.LIMIT_SHOW_ERROR : mPadErrorDetails.Count; // 
                    for (int i = 0; i < lm; i++)
                    {
                        Utils.PadErrorControl item = new Utils.PadErrorControl(mPadErrorDetails[i].PadImage.Bitmap, mPadErrorDetails[i].Pad.NoID);
                        item.ID = i;
                        item.Click += EventFOVClick;
                        stackPadError.Children.Add(item);
                    }
                    ColError.Width = new GridLength(850);
                    ColInfo.Width = new GridLength(0);
                    ColStatistical.Width = new GridLength(0);
                    chartForm.Visibility = Visibility.Hidden;
                    bdFOVError.Visibility = Visibility.Visible;
                    
                });
            }
            else
            {
                mIsShowError = false; this.Dispatcher.Invoke(() =>
                {
                    if(mImageGraft != null)
                    {
                        mImageGraft.Dispose();
                        mImageGraft = null;
                    }
                    stackPadError.Children.Clear();
                    bdFOVError.Visibility = Visibility.Hidden;
                    ColInfo.Width = new GridLength(500);
                    ColStatistical.Width = new GridLength(350);
                    ColError.Width = new GridLength(0);
                    chartForm.Visibility = Visibility.Visible;
                    ShowComponentPosition(System.Drawing.Rectangle.Empty);
                    lbPadErrorArea.Content = "---";
                    lbPadErrorShiftX.Content = "---";
                    lbPadErrorShiftY.Content = "---";
                    lbPadErrorID.Content = "---";
                    lbPadErrorComponent.Content = "---";

                });
            }
        }
        public void EventFOVClick(object sender, RoutedEventArgs e)
        {
            Utils.PadErrorControl item = sender as Utils.PadErrorControl;
            if(mImageGraft != null)
            {
                var padEr = mPadErrorDetails[item.ID];
                int idFov = padEr.FOVNo;
                mImageGraft.ROI = mROIFOVImage[idFov];
                BitmapSource bms = Utils.Convertor.Bitmap2BitmapSource(mImageGraft.Bitmap);
                imbFOVError.Source = bms;
                mImageGraft.ROI = new System.Drawing.Rectangle();
                lbPadErrorArea.Content = string.Format("{0} | ({1} ~ {2})", padEr.Area, padEr.AreaStdLow, padEr.AreaStdHight);
                lbPadErrorShiftX.Content = string.Format("{0} | ({1} ~ {2})", padEr.ShiftX, 0, padEr.ShiftXStduM);
                lbPadErrorShiftY.Content = string.Format("{0} | ({1} ~ {2})", padEr.ShiftY, 0, padEr.ShiftYStduM);
                string component = mModel.GetComponentName(padEr.Pad);
                lbPadErrorID.Content = padEr.Pad.NoID.ToString();
                lbPadErrorComponent.Content = component;
                var rect = mModel.GetRectangleComponent(padEr.Pad);
                rect.Inflate(20, 20);
                rect.X -= mModel.Gerber.ROI.X;
                rect.Y -= mModel.Gerber.ROI.Y;
                ShowComponentPosition(rect);
                UpdateStatus(Utils.LabelMode.PRODUCT_STATUS, Utils.LabelStatus.READY);
            }
        }
        private void btFinish_Click(object sender, RoutedEventArgs e)
        {
            ShowError(false);
        }
        public void ShowComponentPosition(System.Drawing.Rectangle Component)
        {
            if(Component == null || Component == System.Drawing.Rectangle.Empty || imbDiagram.Source == null)
            {
                bdComponentError.Visibility = Visibility.Hidden;
            }
            else
            {
                double imgWidth = mModel.Gerber.ROI.Width;
                double imgHeight = mModel.Gerber.ROI.Height;
                double fovWidth = Component.Width;
                double fovHeight = Component.Height;
                double imbWidth = imbDiagram.ActualWidth;
                double imbHeight = imbDiagram.ActualHeight;
                double bdImbWidth = bdImbDiagram.ActualWidth;
                double bdImbHeight = bdImbDiagram.ActualHeight;


                double scaleWidth = imbWidth / imgWidth;
                double scaleHeight = imbHeight / imgHeight;
                double showDisplayWidth = fovWidth * scaleWidth;
                double showDisplayHeight = fovHeight * scaleHeight;
                double addX = 0;
                double addY = 0;
                if(showDisplayWidth < 15)
                {
                    addX = 15 - showDisplayWidth;
                }
                if(showDisplayHeight < 15)
                {
                    addY = 15 - showDisplayHeight;
                }
                Point startPoint = new Point (
                               Component.X * scaleWidth + (bdImbWidth - imbWidth) / 2 - addX /2,
                               Component.Y * scaleHeight  + (bdImbHeight - imbHeight) / 2 - addY / 2
                               );
                bdComponentError.Margin = new Thickness(startPoint.X, startPoint.Y, 0 , 0);
                bdComponentError.Width = showDisplayWidth + addX;
                bdComponentError.Height = showDisplayHeight + addY;
                bdComponentError.Visibility = Visibility.Visible;

            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Thread a = new Thread(() =>
            {
                mModel = Model.LoadModelByName("UBN2304T10_BOT");
                using (Image<Bgr, byte> imgDigram = mModel.GetDiagramImage())
                {
                    SetImageToImb(imbDiagram, imgDigram.Bitmap);
                }
                System.Drawing.Point[] Fovs = mModel.GetAnchorsFOV();
                var modelFov = mModel.FOV;
                for (int i = 0; i < mModel.Gerber.FOVs.Count; i++)
                {

                    System.Drawing.Rectangle ROIGerber = new System.Drawing.Rectangle(
                                    Fovs[i].X - modelFov.Width / 2, Fovs[i].Y - modelFov.Height / 2,
                                    modelFov.Width, modelFov.Height);
                    mROIFOVImage.Add(ROIGerber);
                }
                Image<Gray, byte> imgMask = new Image<Gray, byte>(@"D:\Heal\Projects\B06\SPI\Data\Pad\2021_01_14\TIME(10_51_54)_._SN(NOT FOUND)\Image_Segment_Graft.png");
                mImageGraft = new Image<Bgr, byte>(@"D:\Heal\Projects\B06\SPI\Data\Pad\2021_01_14\TIME(10_51_54)_._SN(NOT FOUND)\Image_Graft.png");
                Stopwatch sw = new Stopwatch();
                sw.Start();
                imgMask = VI.Predictor.ReleaseNoise(imgMask);
                Utils.PadSegmentInfo[] pads = VI.Predictor.GetPadSegmentInfo(imgMask, mModel.Gerber.ROI);
                Console.WriteLine(sw.ElapsedMilliseconds);
                Utils.PadErrorDetail[] padError = VI.Predictor.ComparePad(mModel, pads);
                padError = VI.Predictor.GetImagePadError(mImageGraft, padError, mModel.Gerber.ROI, 100);
                for (int i = 0; i < padError.Length; i++)
                {
                    mPadErrorDetails.Add(padError[i]);
                }
                ShowError(true);
                Console.WriteLine(sw.ElapsedMilliseconds);
            });
            a.Start();
        }
    }
}
