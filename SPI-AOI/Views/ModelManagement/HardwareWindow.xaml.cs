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
using System.Diagnostics;
using NLog;


namespace SPI_AOI.Views.ModelManagement
{
    /// <summary>
    /// Interaction logic for HardwareWindow.xaml
    /// </summary>
    public partial class HardwareWindow : Window
    {
        Model mModel = null;
        Logger mLog = Heal.LogCtl.GetInstance();
        IOT.HikCamera mCamera = Devices.MyCamera.GetInstance();
        System.Timers.Timer mTimer = new System.Timers.Timer(10);
        Devices.DKZ224V4ACCom mLightSource = new Devices.DKZ224V4ACCom(Properties.Settings.Default.LIGHT_COM);
        CalibrateInfo mCalibImage = CalibrateLoader.GetIntance();
        Devices.MyPLC mPLC = new Devices.MyPLC();
        bool mIsTimerRunning = false;
        bool mLoaded = false;
        int mCount = 10;
        PadItem mPadMark = null;
        double mScanWidth = 10;
        double mScanHeight = 10;
        List<ReadCodePosition> mReadCodePosition = new List<ReadCodePosition>();
        double mExposureTime = 0;
        double mGain = 0;
        int[] mLightIntensity = new int[4];
        System.Drawing.Point mMarkPoint = new System.Drawing.Point();
        System.Drawing.Point mMarkPointOnImage = new System.Drawing.Point();
        double mConveyor = 0;
        double mMatchingScore = 0;
        double mGrayLevel = 127;
        double mSearchX = 5;
        double mSearchY = 5;
        float mDPI = 500;
        public HardwareWindow(Model model)
        {
            mModel = model;
            LoadVariables();
            InitializeComponent();
            LoadUI();
        }
        private void Window_Initialized(object sender, EventArgs e)
        {
            
        }
        public void LoadVariables()
        {
            if (mModel.Gerber.MarkPoint.PadMark[0] > 0)
            {
                mPadMark = mModel.Gerber.PadItems[mModel.Gerber.MarkPoint.PadMark[0]];
            }
            var readCode = mModel.HardwareSettings.ReadCodePosition;
            for (int i = 0; i < readCode.Count; i++)
            {
                mReadCodePosition.Add(readCode[i].Copy());
            }
            mExposureTime = mModel.HardwareSettings.ExposureTime;
            mGain = mModel.HardwareSettings.Gain;
            for (int i = 0; i < 4; i++)
                mLightIntensity[i] = mModel.HardwareSettings.LightIntensity[i];
            mMatchingScore = mModel.Gerber.MarkPoint.Score;
            mGrayLevel = mModel.Gerber.MarkPoint.ThresholdValue;
            mSearchX = mModel.Gerber.MarkPoint.SearchX;
            mSearchY = mModel.Gerber.MarkPoint.SearchY;
            System.Drawing.Point p = mModel.HardwareSettings.MarkPosition;
            mMarkPoint = new System.Drawing.Point(p.X, p.Y);
            mConveyor = mModel.HardwareSettings.Conveyor;
            mDPI = mModel.DPI;
        }
        private void ApplyModelSettings()
        {
            mCamera.SetParameter(IOT.KeyName.ExposureTime, Convert.ToInt32(mExposureTime));
            mLightSource.SetFour(mLightIntensity[0], mLightIntensity[1], mLightIntensity[2], mLightIntensity[3]);
            mLightSource.ActiveFour(1,1,1,1);
            mCamera.SetParameter(IOT.KeyName.Gain, (float)(mGain));
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
                this.Dispatcher.Invoke(() => {
                LoadIndexReadCodePosition();
                mPLC.Login();
                nExposureTime.Value = Convert.ToInt32(mExposureTime);
                nGain.Value = Convert.ToDecimal(mGain);
                nLightIntensity1.Value = Convert.ToDecimal(mLightIntensity[0]);
                nLightIntensity2.Value = Convert.ToDecimal(mLightIntensity[1]);
                nLightIntensity3.Value = Convert.ToDecimal(mLightIntensity[2]);
                nLightIntensity4.Value = Convert.ToDecimal(mLightIntensity[3]);
                nScanWidth.Value = Convert.ToDecimal(mScanWidth);
                nScanHeight.Value = Convert.ToDecimal(mScanHeight);
                
                // teach matching
                nSearchX.Value = Convert.ToDecimal(mSearchX);
                nSearchY.Value = Convert.ToDecimal(mSearchY);
                nMatchingScore.Value = Convert.ToDecimal(mMatchingScore);
                nGrayLevel.Value = Convert.ToDecimal(mGrayLevel);
                grSettings.IsEnabled = true;
                });
                mIsTimerRunning = true;
                mTimer.Elapsed += OnTimedEvent;
                mTimer.Enabled = true;
                ApplyModelSettings();
                mLoaded = true;
            });
            threadCheck.Start();
        }
        private void OnTimedEvent(object sender, System.Timers.ElapsedEventArgs e)
        {
            mTimer.Enabled = false;
            int searchW = Convert.ToInt32(mSearchX * mModel.DPI / 25.4);
            int searchH = Convert.ToInt32(mSearchY * mModel.DPI / 25.4);
            int scanW = Convert.ToInt32(mScanWidth * mModel.DPI / 25.4);
            int scanH = Convert.ToInt32(mScanHeight * mModel.DPI / 25.4);
            int threshold = Convert.ToInt32(mGrayLevel);
            double score = mMatchingScore;
            using (VectorOfPoint template = new VectorOfPoint(mPadMark.Contour))
            using (System.Drawing.Bitmap bm = mCamera.GetOneBitmap(1000))
            {
                if(bm != null)
                {
                    using (Image<Bgr, byte> imgDis = new Image<Bgr, byte>(bm))
                    using (Image<Bgr, byte> img = new Image<Bgr, byte>(bm))
                    {
                        CvInvoke.Undistort(imgDis, img, mCalibImage.CameraMatrix, mCalibImage.DistCoeffs, mCalibImage.NewCameraMatrix);
                        System.Drawing.Rectangle rectSearch = new System.Drawing.Rectangle(img.Width / 2 - searchW / 2, img.Height / 2 - searchH / 2, searchW, searchH);
                        System.Drawing.Rectangle rectScan = new System.Drawing.Rectangle(img.Width / 2 - scanW / 2, img.Height / 2 - scanH / 2, scanW, scanH);
                        img.ROI = rectSearch;
                        using (Image<Gray, byte> imgSearchBinary = new Image<Gray, byte>(rectSearch.Size))
                        using (Image<Bgr, byte> imgSearchBgr = new Image<Bgr, byte>(rectSearch.Size))
                        {
                            CvInvoke.CvtColor(img, imgSearchBinary, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);
                            CvInvoke.Threshold(imgSearchBinary, imgSearchBinary, threshold, 255, Emgu.CV.CvEnum.ThresholdType.Binary);
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
                                        CvInvoke.DrawContours(imgSearchBgr, contours, 0, new MCvScalar(0, 255, 0), 1);
                                        Moments mm = CvInvoke.Moments(markInfo.Item1);
                                        if (mm.M00 != 0)
                                        {
                                            mMarkPointOnImage = new System.Drawing.Point(Convert.ToInt32(mm.M10 / mm.M00), Convert.ToInt32(mm.M01 / mm.M00));
                                        }
                                    }
                                }
                            }
                            img.ROI = System.Drawing.Rectangle.Empty;
                            CvInvoke.Line(img, new System.Drawing.Point(0, img.Height / 2), new System.Drawing.Point(img.Width, img.Height / 2), new MCvScalar(255, 0, 0), 1);
                            CvInvoke.Line(img, new System.Drawing.Point(img.Width / 2, 0), new System.Drawing.Point(img.Width / 2, img.Height), new MCvScalar(255, 0, 0), 1);
                            CvInvoke.Rectangle(img, rectScan, new MCvScalar(0, 255, 0), 1);
                            CvInvoke.Rectangle(img, rectSearch, new MCvScalar(0, 255, 255), 1);
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
                            if(markInfo.Item1 != null)
                            {
                                markInfo.Item1.Dispose();
                            }
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
            mPLC.Login();
            if (rbTopAxis.IsChecked == true)
            {
                mPLC.Set_Go_Up_Top();
            }
            if (rbBotAxis.IsChecked == true)
            {
                mPLC.Set_Go_Up_Bot();
            }
            if(rbConveyor.IsChecked == true)
            {
                mPLC.Set_Go_Up_Conveyor();
            }
        }

        private void btLeft_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!mLoaded)
                return;
            mPLC.Login();
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
            mPLC.Login();
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
            mPLC.Login();
            if (rbTopAxis.IsChecked == true)
            {
                mPLC.Set_Go_Down_Top();
            }
            if (rbBotAxis.IsChecked == true)
            {
                mPLC.Set_Go_Down_Bot();
            }
            if (rbConveyor.IsChecked == true)
            {
                mPLC.Set_Go_Down_Conveyor();
            }
        }
        private void btDown_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!mLoaded)
                return;
            if (rbTopAxis.IsChecked == true)
            {
                mPLC.Reset_Go_Down_Top();
                mMarkPoint = GetTopCoordinates();
            }
            if (rbBotAxis.IsChecked == true)
            {
                mPLC.Reset_Go_Down_Bot();
            }
            if (rbConveyor.IsChecked == true)
            {
                mPLC.Reset_Go_Down_Conveyor();
                mConveyor = GetConveyorCoordinates();
            }
        }

        private void btRight_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!mLoaded)
                return;
            if (rbTopAxis.IsChecked == true)
            {
                mPLC.Reset_Go_Right_Top();
                mMarkPoint = GetTopCoordinates();
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
                mMarkPoint = GetTopCoordinates();
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
                mMarkPoint = GetTopCoordinates();
            }
            if (rbBotAxis.IsChecked == true)
            {
                mPLC.Reset_Go_Up_Bot();
            }
            if (rbConveyor.IsChecked == true)
            {
                mPLC.Reset_Go_Up_Conveyor();
                mConveyor = GetConveyorCoordinates();
            }
        }
        

        private void slSpeed_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!mLoaded)
                return;
            slSpeed.Value = (int)slSpeed.Value;
            if (rbTopAxis.IsChecked == true)
            {
                mPLC.Set_Speed_Top(Convert.ToInt32(slSpeed.Value * 5));
            }
            if (rbBotAxis.IsChecked == true)
            {
                mPLC.Set_Speed_Bot(Convert.ToInt32(slSpeed.Value * 5));
            }
            if (rbConveyor.IsChecked == true)
            {
                mPLC.Set_Speed_Conveyor(Convert.ToInt32(slSpeed.Value * 8));
            }
        }
        private void nExposureTime_ValueChanged(object sender, EventArgs e)
        {
            if (!mLoaded)
                return;
            mExposureTime = Convert.ToDouble((sender as System.Windows.Forms.NumericUpDown).Value);
            mCamera.SetParameter(IOT.KeyName.ExposureTime, Convert.ToInt32(mExposureTime));
        }

        private void nGain_ValueChanged(object sender, EventArgs e)
        {
            if (!mLoaded)
                return;
            mGain = Convert.ToDouble((sender as System.Windows.Forms.NumericUpDown).Value);
            mCamera.SetParameter(IOT.KeyName.Gain, (float)(mGain));
        }

        private void nLightIntensity1_ValueChanged(object sender, EventArgs e)
        {
            if (!mLoaded)
                return;
            mLightIntensity[0] = Convert.ToInt32((sender as System.Windows.Forms.NumericUpDown).Value);
            int[] value = mLightIntensity;
            mLightSource.SetFour(value[0], value[1], value[2], value[3]);
        }


        private void nLightIntensity2_ValueChanged(object sender, EventArgs e)
        {
            if (!mLoaded)
                return;
            mLightIntensity[1] = Convert.ToInt32((sender as System.Windows.Forms.NumericUpDown).Value);
            int[] value = mLightIntensity;
            mLightSource.SetFour(value[0], value[1], value[2], value[3]);
        }

        private void nLightIntensity3_ValueChanged(object sender, EventArgs e)
        {
            if (!mLoaded)
                return;
            mLightIntensity[2] = Convert.ToInt32((sender as System.Windows.Forms.NumericUpDown).Value);
            int[] value = mLightIntensity;
            mLightSource.SetFour(value[0], value[1], value[2], value[3]);
        }

        private void nLightIntensity4_ValueChanged(object sender, EventArgs e)
        {
            if (!mLoaded)
                return;
            mLightIntensity[3] = Convert.ToInt32((sender as System.Windows.Forms.NumericUpDown).Value);
            int[] value = mLightIntensity;
            mLightSource.SetFour(value[0], value[1], value[2], value[3]);
        }
        private void nSearchX_ValueChanged(object sender, EventArgs e)
        {
            if (!mLoaded)
                return;
            mSearchX = Convert.ToDouble((sender as System.Windows.Forms.NumericUpDown).Value);
        }

        private void nSearchY_ValueChanged(object sender, EventArgs e)
        {
            if (!mLoaded)
                return;
            mSearchY = Convert.ToDouble((sender as System.Windows.Forms.NumericUpDown).Value);
        }

        private void nMatchingScore_ValueChanged(object sender, EventArgs e)
        {
            if (!mLoaded)
                return;
            mMatchingScore = Convert.ToDouble((sender as System.Windows.Forms.NumericUpDown).Value);
        }

        private void nGrayLevel_ValueChanged(object sender, EventArgs e)
        {
            if (!mLoaded)
                return;
            mGrayLevel = Convert.ToDouble((sender as System.Windows.Forms.NumericUpDown).Value);
        }

        private bool SearchChanged()
        {
            var readCode = mModel.HardwareSettings.ReadCodePosition;
            if (mReadCodePosition != readCode)
                return true;
            if (mExposureTime != mModel.HardwareSettings.ExposureTime)
                return true;
            if (mGain != mModel.HardwareSettings.Gain)
                return true;
            if (mLightIntensity != mModel.HardwareSettings.LightIntensity)
                return true;
            if (mMatchingScore != mModel.Gerber.MarkPoint.Score)
                return true;
            if (mGrayLevel != mModel.Gerber.MarkPoint.ThresholdValue)
                return true;
            if (mSearchX != mModel.Gerber.MarkPoint.SearchX)
                return true;
            if(mSearchY != mModel.Gerber.MarkPoint.SearchY)
                return true;
            System.Drawing.Point p = mModel.HardwareSettings.MarkPosition;
            if (mMarkPoint != p)
                return true;
            if (mConveyor != mModel.HardwareSettings.Conveyor)
                return true;
            return false;
        }
        private void SaveChanged()
        {
            mMarkPoint = GetTopCoordinates();
            mConveyor = GetConveyorCoordinates();
            mModel.HardwareSettings.ReadCodePosition = new List<ReadCodePosition>();
            for (int i = 0; i < mReadCodePosition.Count; i++)
            {
                mModel.HardwareSettings.ReadCodePosition.Add(mReadCodePosition[i].Copy());
            }
            mModel.HardwareSettings.ExposureTime = mExposureTime;
            mModel.HardwareSettings.Gain = mGain;
            for (int i = 0; i < 4; i++)
            {
                mModel.HardwareSettings.LightIntensity[i] = mLightIntensity[i];
            }
            mModel.Gerber.MarkPoint.Score = mMatchingScore;
            mModel.Gerber.MarkPoint.ThresholdValue = mGrayLevel;
            mModel.Gerber.MarkPoint.SearchX = mSearchX;
            mModel.Gerber.MarkPoint.SearchY = mSearchY;
            
            mModel.HardwareSettings.Conveyor = mConveyor;


            int searchW = Convert.ToInt32(mSearchX * mModel.DPI / 25.4);
            int searchH = Convert.ToInt32(mSearchY * mModel.DPI / 25.4);
            
            int subX = searchW / 2 - mMarkPointOnImage.X;
            int subY = searchH / 2 - mMarkPointOnImage.Y;
            System.Drawing.Point markPulse = new System.Drawing.Point(
                mMarkPoint.X + Convert.ToInt32(-subX * mModel.PulseXPerPixel),
                mMarkPoint.Y + Convert.ToInt32(-subY * mModel.PulseYPerPixel)
                );
            mModel.HardwareSettings.MarkPosition = markPulse;
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(mIsTimerRunning)
            {
                if (SearchChanged())
                {
                    var r = MessageBox.Show("Are you want to save changed ?", "Question", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                    if(r == MessageBoxResult.Yes)
                    {
                        SaveChanged();
                    }
                    else if(r == MessageBoxResult.Cancel)
                    {
                        e.Cancel = true;
                        return;
                    }
                    mIsTimerRunning = false;
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
                    }
                    mLightSource.ActiveFour(0, 0, 0, 0);
                    mLightSource.Close();
                }
            }
        }

        private void rbConveyor_Checked(object sender, RoutedEventArgs e)
        {
            if (!mLoaded)
                return;
            btLeft.Visibility = Visibility.Hidden;
            btRight.Visibility = Visibility.Hidden;
            slSpeed_ValueChanged(slSpeed, null);
        }

        private void rbTopAxis_Checked(object sender, RoutedEventArgs e)
        {
            if (!mLoaded)
                return;
            btLeft.Visibility = Visibility.Visible;
            btRight.Visibility = Visibility.Visible;
            slSpeed_ValueChanged(slSpeed, null);
        }

        private void rbBotAxis_Checked(object sender, RoutedEventArgs e)
        {
            if (!mLoaded)
                return;
            btLeft.Visibility = Visibility.Visible;
            btRight.Visibility = Visibility.Visible;
            slSpeed_ValueChanged(slSpeed, null);
        }

        private void nScanWidth_ValueChanged(object sender, EventArgs e)
        {
            if (!mLoaded)
                return;
            mScanWidth = Convert.ToDouble((sender as System.Windows.Forms.NumericUpDown).Value);
        }

        private void nScanHeight_ValueChanged(object sender, EventArgs e)
        {
            if (!mLoaded)
                return;
            if (!mLoaded)
                return;
            mScanHeight = Convert.ToDouble((sender as System.Windows.Forms.NumericUpDown).Value);
        }
        private void SetConveyor(int x)
        {
            mPLC.Reset_Go_Coordinates_Finish_Setup_Conveyor();
            mPLC.Set_Conveyor(x);
            mPLC.Set_Write_Coordinates_Finish_Setup_Conveyor();
        }
        private void SetTopAxis(int x, int y)
        {
            mPLC.Reset_Go_Coordinates_Finish_Setup_Top();
            mPLC.Set_X_Top(x);
            mPLC.Set_Y_Top(y);
            mPLC.Set_Write_Coordinates_Finish_Setup_Top();
        }
        private void SetBotAxis(int x, int y)
        {
            mPLC.Reset_Go_Coordinates_Finish_Setup_Bot();
            mPLC.Set_X_Bot(x);
            mPLC.Set_Y_Bot(y);
            mPLC.Set_Write_Coordinates_Finish_Setup_Bot();
        }
        public System.Drawing.Point GetTopCoordinates()
        {
            int x = mPLC.Get_X_Top();
            int y = mPLC.Get_Y_Top();
            return new System.Drawing.Point(x, y);
        }
        public double GetConveyorCoordinates()
        {
            return mPLC.Get_Conveyor();
        }
        private void cbScanPointID_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(cbScanPointID.SelectedIndex >= 0)
            {
                ReadCodePosition readCodeInfo = mModel.HardwareSettings.ReadCodePosition[cbScanPointID.SelectedIndex];
                if(readCodeInfo.Surface ==  Surface.TOP)
                {
                    SetTopAxis(readCodeInfo.Origin.X, readCodeInfo.Origin.Y);
                } 
                else if (readCodeInfo.Surface == Surface.BOT)
                {
                    SetBotAxis(readCodeInfo.Origin.X, readCodeInfo.Origin.Y);
                }
                nScanWidth.Value = Convert.ToDecimal(readCodeInfo.Width);
                nScanHeight.Value = Convert.ToDecimal(readCodeInfo.Height);
            }
        }
        private void LoadIndexReadCodePosition()
        {
            cbScanPointID.Items.Clear();
            for (int i = 0; i < mReadCodePosition.Count; i++)
            {
                cbScanPointID.Items.Add(i + 1);
            }
        }
        private void btAddScan_Click(object sender, RoutedEventArgs e)
        {
            int x = 0;
            int y = 0;
            Surface surface = Surface.TOP;
            if(rbBotAxis.IsChecked == true)
            {
                x = mPLC.Get_X_Bot();
                y = mPLC.Get_Y_Bot();
                surface = Surface.BOT;
            }
            if(rbTopAxis.IsChecked == true)
            {
                x = mPLC.Get_X_Top();
                y = mPLC.Get_Y_Top();
                surface = Surface.TOP;
            }
            var r = MessageBox.Show(string.Format("Are you want to add\nPoint ({0},{1}) \nSurface {2}\n to read code position?", x, y, surface), "Information", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
            if (r == MessageBoxResult.Yes)
            {
                ReadCodePosition readCodeInfo = new ReadCodePosition();
                readCodeInfo.Origin = new System.Drawing.Point(x, y);
                readCodeInfo.Surface = surface;
                readCodeInfo.Height = mScanHeight;
                readCodeInfo.Width = mScanWidth;
                mReadCodePosition.Add(readCodeInfo);
                LoadIndexReadCodePosition();
                MessageBox.Show("Add successfully!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btDelScan_Click(object sender, RoutedEventArgs e)
        {
            int id = cbScanPointID.SelectedIndex;
            if (id >= 0)
            {
                var p = mReadCodePosition[id];
                int x = p.Origin.X;
                int y = p.Origin.Y;
                Surface surface = p.Surface;
                var r = MessageBox.Show(string.Format("Are you want to add\nPoint ({0},{1}) \nSurface {2}\n to read code position?", x, y, surface), "Information", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                if (r == MessageBoxResult.Yes)
                {
                    mReadCodePosition.RemoveAt(id);
                    LoadIndexReadCodePosition();
                    MessageBox.Show(string.Format("Delete successfully!"), "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void btGoXYZ_Click(object sender, RoutedEventArgs e)
        {
            mPLC.Login();
            mPLC.Set_Speed_Top(3000);
            mPLC.Set_Speed_Bot(6000);
            mPLC.Set_Speed_Conveyor(8000);
            
            int conveyorPulse = mPLC.Get_Conveyor();
            mLog.Info(string.Format("Current Pulse conveyor {0} => {1}", conveyorPulse, mConveyor));
            if (conveyorPulse != mConveyor)
            {
                SetConveyor(Convert.ToInt32(mConveyor));
                grSettings.IsEnabled = true;
                WaitingForm wait = new WaitingForm("Moving conveyor...");
                Stopwatch sw = new Stopwatch();
                Thread a = new Thread(() => {

                    sw.Start();
                    while (mPLC.Get_Go_Coordinates_Finish_Setup_Conveyor() != 1 && sw.ElapsedMilliseconds < 180000)
                    {
                        Thread.Sleep(100);
                    }
                    wait.KillMe = true;
                });

                a.Start();
                wait.ShowDialog();

                if (sw.ElapsedMilliseconds > 180000)
                {
                    MessageBox.Show("Timeout move conveyor!", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                Thread.Sleep(500);
                mPLC.Reset_Go_Coordinates_Finish_Setup_Conveyor();
            }
            SetTopAxis(mMarkPoint.X, mMarkPoint.Y);
            if (mReadCodePosition.Count > 0)
            {
                if (mReadCodePosition[0].Surface == Surface.BOT)
                {
                    SetBotAxis(mReadCodePosition[0].Origin.X, mReadCodePosition[0].Origin.Y);
                }
            }
        }

        private void btLoad_Click(object sender, RoutedEventArgs e)
        {
            mPLC.Login();
            mPLC.Set_Load_Product();
        }

        private void btUnload_Click(object sender, RoutedEventArgs e)
        {
            mPLC.Login();
            mPLC.Set_Unload_Product();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            
        }

        private void btApply_Click(object sender, RoutedEventArgs e)
        {
            SaveChanged();
            MessageBox.Show("Done!");

        }

        private void btAutoAdjust_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
