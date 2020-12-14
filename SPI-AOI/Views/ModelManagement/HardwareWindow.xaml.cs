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
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using SPI_AOI.Models;
using System.Threading;


namespace SPI_AOI.Views.ModelManagement
{
    /// <summary>
    /// Interaction logic for HardwareWindow.xaml
    /// </summary>
    public partial class HardwareWindow : Window
    {
        Model mModel = null;
        PadItem mPadMark = null;
        IOT.HikCamera mCamera = Devices.MyCamera.GetInstance();
        System.Timers.Timer mTimer = new System.Timers.Timer(20);
        Devices.DKZ224V4ACCom mLightSource = new Devices.DKZ224V4ACCom(Properties.Settings.Default.LIGHT_COM);
        Devices.MyPLC mPLC = new Devices.MyPLC();
        bool mIsTimerRunning = false;
        bool mLoaded = false;
        int mCount = 10;
        public HardwareWindow(Model model)
        {
            mModel = model;
            if(mModel.Gerber.MarkPoint.PadMark[0] > 0)
            {
                mPadMark = mModel.Gerber.PadItems[mModel.Gerber.MarkPoint.PadMark[0]];
            }
            InitializeComponent();
            LoadUI();
        }
        private void Window_Initialized(object sender, EventArgs e)
        {
            
        }
        public void LoadUI()
        {
            grSettings.IsEnabled = false;
            // check the connect

            Thread threadCheck = new Thread(() => {
                if (mPadMark == null)
                {
                    MessageBox.Show("Please settings Mark in gerber settings before do this!", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (mCamera == null)
                {
                    MessageBox.Show("Not found camera, please check the cable!", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if(mPLC.Ping() != 0)
                {
                    MessageBox.Show("Cant ping to PLC, please check the cable and try again!", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if(mLightSource.Open() != 0)
                {
                    MessageBox.Show("Cant open COM light source, please check the cable and try again!", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                int r = mCamera.Open();
                if(r != 0)
                {
                    MessageBox.Show("Cant open camera, please check the cable!", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                    mLightSource.Close();
                    return;
                }
                mCamera.StartGrabbing();
                mIsTimerRunning = true;
                mTimer.Elapsed += OnTimedEvent;
                mTimer.Enabled = true;
                mLoaded = true;
                this.Dispatcher.Invoke(() => {
                    grSettings.IsEnabled = true;
                    nExposureTime.Value = Convert.ToInt32(mModel.HardwareSettings.ExposureTime);
                    nGain.Value = Convert.ToDecimal(mModel.HardwareSettings.Gain);
                    nGamma.Value = Convert.ToDecimal(mModel.HardwareSettings.Gamma);
                    nLightIntensity.Value = Convert.ToDecimal(mModel.HardwareSettings.LightIntensity);

                    // teach matching
                    nSearchX.Value = Convert.ToDecimal(mModel.Gerber.MarkPoint.SearchX);
                    nSearchY.Value = Convert.ToDecimal(mModel.Gerber.MarkPoint.SearchY);
                    nMatchingScore.Value = Convert.ToDecimal(mModel.Gerber.MarkPoint.Score);
                    nGrayLevel.Value = Convert.ToDecimal(mModel.Gerber.MarkPoint.ThresholdValue);
                });
            });
            threadCheck.Start();
        }
        private void OnTimedEvent(object sender, System.Timers.ElapsedEventArgs e)
        {
            mTimer.Enabled = false;
            int searchW = Convert.ToInt32(mModel.Gerber.MarkPoint.SearchX * mModel.DPI / 25.4);
            int searchH = Convert.ToInt32(mModel.Gerber.MarkPoint.SearchY * mModel.DPI / 25.4);
            int threshold = Convert.ToInt32(mModel.Gerber.MarkPoint.ThresholdValue);
            double score = mModel.Gerber.MarkPoint.Score;
            VectorOfPoint template = mModel.Gerber.PadItems[ mModel.Gerber.MarkPoint.PadMark[0]].Contour;
            using (System.Drawing.Bitmap bm = mCamera.GetOneBitmap(1000))
            {
                if(bm != null)
                {
                    using (Image<Bgr, byte> img = new Image<Bgr, byte>(bm))
                    {
                        System.Drawing.Rectangle rectSearch = new System.Drawing.Rectangle(img.Width / 2 - searchW / 2, img.Height / 2 - searchH / 2, searchW, searchH);
                        img.ROI = rectSearch;
                        using (Image<Gray, byte> imgSearchBinary = new Image<Gray, byte>(rectSearch.Size))
                        using (Image<Bgr, byte> imgSearchBgr = new Image<Bgr, byte>(rectSearch.Size))
                        {
                            CvInvoke.CvtColor(img, imgSearchBinary, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);
                            CvInvoke.Threshold(imgSearchBinary, imgSearchBinary, threshold, 255, Emgu.CV.CvEnum.ThresholdType.BinaryInv);
                            CvInvoke.CvtColor(imgSearchBinary, imgSearchBgr, Emgu.CV.CvEnum.ColorConversion.Gray2Bgr);
                            Tuple<VectorOfPoint, double> markInfo = Mark.MarkDetection(imgSearchBinary, template);
                            double realScore = markInfo.Item2;
                            realScore = Math.Round((1 - realScore) * 100.0, 2);
                            if(realScore > score)
                            {
                                using (VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint())
                                {
                                    if (markInfo.Item1 != null)
                                    {
                                        contours.Push(markInfo.Item1);
                                        CvInvoke.DrawContours(imgSearchBgr, contours, -1, new MCvScalar(0, 0, 255), 2);
                                    }
                                }
                            }
                            
                            img.ROI = System.Drawing.Rectangle.Empty;
                            
                            CvInvoke.Line(img, new System.Drawing.Point(0, img.Height / 2), new System.Drawing.Point(img.Width, img.Height / 2), new MCvScalar(255, 0, 0), 3);
                            CvInvoke.Line(img, new System.Drawing.Point(img.Width / 2, 0), new System.Drawing.Point(img.Width / 2, img.Height), new MCvScalar(255, 0, 0), 3);

                            CvInvoke.Rectangle(img, rectSearch, new MCvScalar(0, 255, 0), 3);

                            this.Dispatcher.Invoke(() => {
                                BitmapSource bmsMark = Utils.Convertor.Bitmap2BitmapSource(imgSearchBgr.Bitmap);
                                imbBinaryMark.Source = bmsMark;
                                if(mCount == 10)
                                {
                                    lbRealScore.Content = realScore.ToString();
                                    mCount = 0;
                                }
                                BitmapSource bms = Utils.Convertor.Bitmap2BitmapSource(img.Bitmap);
                                imbCameraShow.Source = bms;
                            });
                        }
                        
                    }
                }
            }
            mCount++;
            mTimer.Enabled = mIsTimerRunning;
        }
        private void btUp_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!mLoaded)
                return;
            if (rbTopAxis.IsChecked == true)
            {
                mPLC.Set_Go_Up_Top();
            }
            if (rbBotAxis.IsChecked == true)
            {
                mPLC.Set_Go_Up_Bot();
            }
        }

        private void btLeft_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!mLoaded)
                return;
            if (rbTopAxis.IsChecked == true)
            {
                mPLC.Set_Go_Left_Top();
            }
            if (rbBotAxis.IsChecked == true)
            {
                mPLC.Set_Go_Left_Bot();
            }
        }

        private void btRight_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!mLoaded)
                return;
            if (rbTopAxis.IsChecked == true)
            {
                mPLC.Set_Go_Right_Top();
            }
            if (rbBotAxis.IsChecked == true)
            {
                mPLC.Set_Go_Right_Bot();
            }
        }

        private void btDown_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!mLoaded)
                return;
            if (rbTopAxis.IsChecked == true)
            {
                mPLC.Set_Go_Down_Top();
            }
            if (rbBotAxis.IsChecked == true)
            {
                mPLC.Set_Go_Down_Bot();
            }
        }
        private void btDown_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!mLoaded)
                return;
            if (rbTopAxis.IsChecked == true)
            {
                mPLC.Reset_Go_Down_Top();
            }
            if (rbBotAxis.IsChecked == true)
            {
                mPLC.Reset_Go_Down_Bot();
            }
        }

        private void btRight_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!mLoaded)
                return;
            if (rbTopAxis.IsChecked == true)
            {
                mPLC.Reset_Go_Right_Top();
            }
            if (rbBotAxis.IsChecked == true)
            {
                mPLC.Reset_Go_Right_Bot();
            }
        }

        private void btLeft_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!mLoaded)
                return;
            if (rbTopAxis.IsChecked == true)
            {
                mPLC.Reset_Go_Left_Top();
            }
            if (rbBotAxis.IsChecked == true)
            {
                mPLC.Reset_Go_Left_Bot();
            }
        }

        private void btUp_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!mLoaded)
                return;
            if (rbTopAxis.IsChecked == true)
            {
                mPLC.Reset_Go_Up_Top();
            }
            if (rbBotAxis.IsChecked == true)
            {
                mPLC.Reset_Go_Up_Bot();
            }
        }

        private void btHome_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!mLoaded)
                return;
            mPLC.Set_Go_Home();
        }

        private void slSpeed_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!mLoaded)
                return;
            slSpeed.Value = (int)slSpeed.Value;
            mPLC.Set_Speed(Convert.ToInt32(slSpeed.Value));
        }
        private void GetPositionAxis()
        {
            int xTop = mPLC.Get_X_Top();
            int yTop = mPLC.Get_Y_Top();
            int xBot = mPLC.Get_X_Bot();
            int yBot = mPLC.Get_Y_Bot();
            lock (mModel)
            {
                mModel.HardwareSettings.MarkPosition = 
                    new System.Drawing.Point(xTop, yTop);
                mModel.HardwareSettings.ReadCodePosition = 
                    new System.Drawing.Point(xBot, yBot);
            }
        }
        private void nExposureTime_ValueChanged(object sender, EventArgs e)
        {
            if (!mLoaded)
                return;
            lock(mModel)
            {
                mModel.HardwareSettings.ExposureTime = Convert.ToDouble((sender as System.Windows.Forms.NumericUpDown).Value);
            }
            mCamera.SetParameter(IOT.KeyName.ExposureTime, Convert.ToInt32(mModel.HardwareSettings.ExposureTime));
        }

        private void nGain_ValueChanged(object sender, EventArgs e)
        {
            if (!mLoaded)
                return;
            lock (mModel)
            {
                mModel.HardwareSettings.Gain = Convert.ToDouble((sender as System.Windows.Forms.NumericUpDown).Value);
            }
            mCamera.SetParameter(IOT.KeyName.Gain, (float)(mModel.HardwareSettings.Gain));
        }

        private void nGamma_ValueChanged(object sender, EventArgs e)
        {
            if (!mLoaded)
                return;
            lock (mModel)
            {
                mModel.HardwareSettings.Gamma = Convert.ToDouble((sender as System.Windows.Forms.NumericUpDown).Value);
            }
            mCamera.SetParameter(IOT.KeyName.Gamma, (float)(mModel.HardwareSettings.Gamma));
        }

        private void nLightIntensity_ValueChanged(object sender, EventArgs e)
        {
            if (!mLoaded)
                return;
            lock (mModel)
            {
                mModel.HardwareSettings.LightIntensity = Convert.ToInt32((sender as System.Windows.Forms.NumericUpDown).Value);
            }
            int value = mModel.HardwareSettings.LightIntensity;
            mLightSource.SetFour(value, value, value, value);
        }

        private void nSearchX_ValueChanged(object sender, EventArgs e)
        {
            if (!mLoaded)
                return;
            lock (mModel)
            {
                mModel.Gerber.MarkPoint.SearchX = Convert.ToDouble((sender as System.Windows.Forms.NumericUpDown).Value);
            }
        }

        private void nSearchY_ValueChanged(object sender, EventArgs e)
        {
            if (!mLoaded)
                return;
            lock (mModel)
            {
                mModel.Gerber.MarkPoint.SearchY = Convert.ToDouble((sender as System.Windows.Forms.NumericUpDown).Value);
            }
        }

        private void nMatchingScore_ValueChanged(object sender, EventArgs e)
        {
            if (!mLoaded)
                return;
            lock (mModel)
            {
                mModel.Gerber.MarkPoint.Score = Convert.ToDouble((sender as System.Windows.Forms.NumericUpDown).Value);
            }
        }

        private void nGrayLevel_ValueChanged(object sender, EventArgs e)
        {
            if (!mLoaded)
                return;
            lock (mModel)
            {
                mModel.Gerber.MarkPoint.ThresholdValue = Convert.ToDouble((sender as System.Windows.Forms.NumericUpDown).Value);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            GetPositionAxis();
            mIsTimerRunning = false;
            if(mCamera != null)
            {
                if(mCamera.IsGrab)
                {
                    mCamera.StopGrabbing();
                }
                if (mCamera.IsOpen)
                {
                    mCamera.Close();
                }
            }
            mLightSource.Close();
           
        }
    }
}
