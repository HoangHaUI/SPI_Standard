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
        // system variable
        System.Timers.Timer mTimer = new System.Timers.Timer(20);
        System.Timers.Timer mTimerCheckStatus = new System.Timers.Timer(50);
        Properties.Settings mParam = Properties.Settings.Default;
        Logger mLog = Heal.LogCtl.GetInstance();

        // info and device variable
        List<Utils.SummaryInfo> mSummary = new List<Utils.SummaryInfo>();
        Utils.FOVDisplayInfo mFOVDisplay = new Utils.FOVDisplayInfo();
        CalibrateInfo mCalibImage = CalibrateLoader.GetIntance();
        DB.Result mMyDatabase = new DB.Result();
        PLCComm mPlcComm = new PLCComm();
        IOT.HikCamera mCamera = null;
        DKZ224V4ACCom mLight = null;
        Devices.MyScaner mScaner = Devices.MyScaner.GetInstance();

        // proccessing vatiabe
        Image<Bgr, byte> mImageGraft = null;
        List<System.Drawing.Rectangle> mROIFOVImage = new List<System.Drawing.Rectangle>();
        List<Utils.PadErrorDetail> mPadErrorDetails = new List<Utils.PadErrorDetail>();
        int mSumPadModel = 0;
        // status variable
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
            //string id = mMyDatabase.GetNewID();
            //mMyDatabase.InsertNewPanelResult(id,
            //    "Model123", DateTime.Now, "SN123", "Test", "FAIL", "FAIL");
            //mMyDatabase.InsertNewImage(id, DateTime.Now,
            //    @"D:\Save\2021_01_19\10_02_45\FOV_10_10_02_59.png", 0, new System.Drawing.Rectangle(), new System.Drawing.Rectangle(), "FOV");
            //mMyDatabase.InsertNewPadError(id, "Model123", DateTime.Now, 123, 0, new System.Drawing.Rectangle(208, 315, 35, 57), new System.Drawing.Rectangle(), "FAIL", "FAIL", "C152", "Brigde", 150.65462354, 260, 60, 18.546132, 370,
            //    19.5461321354, 370);
            //int count1 = CvInvoke.CountNonZero(image);
            //VectorOfVectorOfPoint cnt = new VectorOfVectorOfPoint();
            //CvInvoke.FindContours(image, cnt, null, Emgu.CV.CvEnum.RetrType.External, Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxSimple);
            //double s = ImageProcessingUtils.ContourArea(cnt[0]);
            //Console.WriteLine("{0}, {1}", count1, s);
            //MainConfigWindow.AlarmForm alarm = new MainConfigWindow.AlarmForm();
            //alarm.ShowDialog();
            //NameValueCollection data = new NameValueCollection();
            //data.Add("Type", "Decode");
            //data.Add("FOV", (1 + 1).ToString());
            //data.Add("Debug", Convert.ToString(mParam.Debug));
            ////string fileName = @"D:\Heal\Projects\B06\SPI\Source code\Python\Test\Decode\3.png";
            //for (int i = 0; i < 50; i++)
            //{
            //    Utils.PadErrorControl item = new Utils.PadErrorControl(null, i);
            //    item.Click += EventFOVClick;
            //    item.ID = i;
            //    stackPadError.Items.Add(item);
            //}

            //VI.ServiceResults serviceResults = VI.ServiceComm.Sendfile(mParam.ServiceURL, new string[] { fileName }, data);
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
            int valRsScan = mPlcComm.Get_Bit_Reset_Scan();
            {
                mScaner.ReleaseBuffer();
                mPlcComm.Reset_Bit_Reset_Scan();
            }
            int val = mPlcComm.Get_Has_Product_Top();
            if (val == 1 && !mIsShowError)
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
            System.Drawing.Point[] markPointXYPLC = mModel.GetPulseXYMark();
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
                using (Image<Bgr, byte> image = VI.MoveXYAxis.CaptureFOV(mPlcComm, mCamera, mLight, mark, LightStrobe, 100))
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
                        Thread isDB = new Thread(() =>
                        {
                            mMyDatabase.InsertNewImage(ID, DateTime.Now, fileName, i, ROI, new System.Drawing.Rectangle(), "MARK");
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
        private string[] CaptureSN(string ID, string SavePath, bool LightStrobe)
        {
            Models.ReadCodePosition[] readCodePosition = mModel.GetPulseXYReadCode();
            string[] sn = new string[readCodePosition.Length];
            PadItem[] PadMark = new PadItem[2];
            for (int i = 0; i < readCodePosition.Length; i++)
            {
                System.Drawing.Point point = readCodePosition[i].Origin;
                int x = point.X;
                int y = point.Y;
                if(readCodePosition[i].Surface == Surface.TOP)
                {
                    mLog.Info(string.Format("{0}, Position Name : {1},  X = {2}, Y = {3}", "Moving TOP Axis", "ReadCode " + (i + 1).ToString(), x, y));
                    using (Image<Bgr, byte> image = VI.MoveXYAxis.CaptureFOV(mPlcComm, mCamera, mLight, point, LightStrobe, 100))
                    {
                        if (image != null)
                        {
                            System.Drawing.Rectangle ROI = readCodePosition[i].ROIImage;

                            string fileName = string.Format("{0}//ReadCode_{1}.png", SavePath, i + 1);
                            image.ROI = ROI;
                            CvInvoke.Imwrite(fileName, image);
                            VI.ServiceResults serviceResults = VI.ServiceComm.Decode(mParam.ServiceURL, new string[] { fileName }, mParam.Debug);
                            sn[i] = serviceResults.SN;
                            Thread isDB = new Thread(() => {
                                mMyDatabase.InsertNewImage(ID, DateTime.Now, fileName, i, ROI, new System.Drawing.Rectangle(), "ReadCode");
                            });
                            isDB.Start();
                        }
                        else
                        {
                            mLog.Info(string.Format("Cant read code : {0}", i + 1));
                        }
                    }
                }
                else if (readCodePosition[i].Surface == Surface.BOT)
                {
                    mLog.Info(string.Format("{0}, Position Name : {1},  X = {2}, Y = {3}", "Moving Bot Axis", "ReadCode " + (i + 1).ToString(), x, y));
                    int moveAxisStatus = VI.MoveXYAxis.ReadCodeBot(mPlcComm, point, 0);
                    if(moveAxisStatus == 0)
                    {
                        sn[i] = mScaner.ReadCode(point);
                    }
                    else
                    {
                        mLog.Info(string.Format("Cant read code : {0}", i + 1));
                    }
                }
            }
            return sn;
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
        private int CaptureFOV(string ID, string SN, DateTime LoadTime, string SavePath, int XDeviation, int YDeviation, double Angle, bool LightStrobe)
        {
            // 0 pass
            // -1 fail
            // -2 capture fail
            // -3 scan fail
            // -4 move xy fail
            int result = -1;
            System.Drawing.Point[] xyAxisPosition = mModel.GetPulseXYFOVs();
            System.Drawing.Point[] Fovs = mModel.GetAnchorsFOV(false);
            Thread[] publishThread = new Thread[Fovs.Length];
            bool[] threadStatus = new bool[Fovs.Length];
            ReleasePadErrorAndFOVImage();
            if(mImageGraft != null)
            {
                mImageGraft.Dispose();
                mImageGraft = null;
            }
            for (int i = 0; i < xyAxisPosition.Length; i++)
            {
                DateTime now = DateTime.Now;
                System.Drawing.Point fov = xyAxisPosition[i];
                int x = fov.X;
                int y = fov.Y;
                mLog.Info(string.Format("{0}, Position Name : {1},  X = {2}, Y = {3}", "Moving TOP Axis", "FOV " + (i + 1).ToString(), x, y));
                if (x < 0 || y < 0 || x > mParam.PLC_LIMIT_X_TOP || y > mParam.PLC_LIMIT_Y_TOP)
                {
                    MessageBox.Show(string.Format("Cant move XY Axis because out range limit of TOYO ... x = {0}, y = {1}", x, y), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    result = - 4; // move xy fail
                    break;
                }
                using (Image<Bgr, byte> image = VI.MoveXYAxis.CaptureFOV(mPlcComm, mCamera, mLight, fov, LightStrobe, 50))
                {
                    if (image != null)
                    {
                        double angle = -mModel.AngleAxisCamera - Angle;
                        using (Image<Bgr, byte> imgRotated = ImageProcessingUtils.ImageRotation(image, new System.Drawing.Point(image.Width / 2, image.Height / 2), angle * Math.PI / 180.0))
                        using (Image<Bgr, byte> imgTransform = ImageProcessingUtils.ImageTransformation(image, XDeviation, YDeviation))
                        {
                            
                            SetDisplayFOV(i);
                            var modelFov = mModel.FOV;
                            System.Drawing.Rectangle ROIFOVOnGerber = new System.Drawing.Rectangle(
                                                    Fovs[i].X - modelFov.Width / 2,
                                                    Fovs[i].Y - modelFov.Height / 2,
                                                    modelFov.Width, modelFov.Height);
                            System.Drawing.Rectangle ROI = mModel.Gerber.FOVs[i].ROI;
                            imgTransform.ROI = ROI;
                            string fileName = string.Format("{0}//FOV_{1}_{2}.png", SavePath, i + 1, now.ToString("HH_mm_ss"));
                            CvInvoke.Imwrite(fileName, imgTransform);
                            publishThread[i] = new Thread(() =>
                            {
                                string panelID = ID;
                                DateTime loadTime = LoadTime;
                                string sn = SN;
                                string imageCapturePath = fileName;
                                string imageSegmentPath = fileName.Replace(".png", "_Segment.png");
                                int id = i;
                                Image<Bgr, byte> imgCapture = new Image<Bgr, byte>(imageCapturePath);
                                if (id > 2)
                                {
                                    publishThread[i - 3].Join();
                                }
                                VI.ServiceResults serviceResults = VI.ServiceComm.SegmentFOV(mParam.ServiceURL, new string[] { fileName }, id, mParam.Debug);
                                if (serviceResults != null)
                                {
                                    lock (serviceResults)
                                    {
                                        threadStatus[id] = true;
                                    }
                                    
                                    Image<Gray, byte> imageSegment = serviceResults.ImgMask;
                                    CvInvoke.Imwrite(imageSegmentPath, imageSegment);
                                    imageSegment = VI.Predictor.ReleaseNoise(imageSegment);
                                    Utils.PadSegmentInfo[] padSegmentInfo = VI.Predictor.GetPadSegmentInfo(imageSegment, ROIFOVOnGerber, id, imageCapturePath, imageSegmentPath);
                                    lock(mModel)
                                    {
                                        Utils.PadErrorDetail[] padError = VI.Predictor.ComparePad(mModel, padSegmentInfo, id);
                                        if(padError.Length > 0)
                                        {
                                            foreach (var item in padError)
                                            {
                                                item.PanelID = panelID;
                                                item.LoadTime = loadTime;
                                                item.SN = sn;
                                                item.ModelName = mModel.Name;
                                                item.MachineResult = "FAIL";
                                                item.ConfirmResult = "FAIL";
                                                item.ImageCapturePath = imageCapturePath;
                                                item.ImageSegmentPath = imageSegmentPath;
                                                string component = mModel.GetComponentName(item.Pad);
                                                item.Component = component;
                                            }
                                            VI.Predictor.GetImagePadError(imgCapture, padError, ROIFOVOnGerber, mParam.LIMIT_SHOW_ERROR);
                                            lock (mPadErrorDetails)
                                            {
                                                mPadErrorDetails.AddRange(padError);
                                            }
                                        }
                                    }
                                }
                                imgCapture.Dispose();
                                imgCapture = null;
                            });
                            publishThread[i].Start();
                            this.Dispatcher.Invoke(() =>
                            {
                                BitmapSource bms = Utils.Convertor.Bitmap2BitmapSource(imgTransform.Bitmap);
                                ImbCameraView.Source = bms;
                                lbcountFovs.Content = (i + 1).ToString();
                            });
                            Thread isDB = new Thread(() =>
                            {
                                int id = i;
                                mMyDatabase.InsertNewImage(ID, now, fileName, id, ROI, ROIFOVOnGerber, "FOV");
                            });
                            isDB.Start();
                        }
                    }
                    else
                    {
                        mLog.Info(string.Format("Cant Capture image in FOV : {0}", i + 1));
                        result = -2; // capture fail
                        break;
                    }
                }
            }
                
            if(result != -2)
            {
                for (int i = 0; i < publishThread.Length; i++)
                {
                    publishThread[i].Join();
                }
                if (mPadErrorDetails.Count == 0 && !threadStatus.Contains(false))
                    return 0;
                else
                    return -1;
            }
            return result;
        }

        private int Processing()
        {
            this.Dispatcher.Invoke(() =>
            {
                lbSN.Content = "-----";
            });
            int result = -1;
            DateTime now = DateTime.Now;
            string date = now.ToString("yyyy_MM_dd");
            string time = now.ToString("HH_mm_ss");
            string[] sn = new string[0];
            string savePath = mParam.SAVE_IMAGE_PATH + "\\" + date + "\\" + time;
            string ID = mMyDatabase.GetNewID();
            if (!Directory.Exists(savePath))
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
            Utils.MarkAdjust markAdjustInfo = CaptureMark(ID, savePath, lightStrobe);
            if (markAdjustInfo.Status == Utils.ActionStatus.Successfully)
            {
                mLight.SetFour(mParam.LIGHT_VI_DEFAULT_INTENSITY_CH1,
                mParam.LIGHT_VI_DEFAULT_INTENSITY_CH2,
                mParam.LIGHT_VI_DEFAULT_INTENSITY_CH3,
                mParam.LIGHT_VI_DEFAULT_INTENSITY_CH4);
                mCamera.SetParameter(IOT.KeyName.ExposureTime, (float)mParam.CAMERA_VI_EXPOSURE_TIME);

                sn = CaptureSN(ID, savePath, lightStrobe);

                string allsn = "";
                for (int i = 0; i < sn.Length; i++)
                {
                    allsn += sn[i];
                    if (i != sn.Length - 1)
                        allsn += ",";
                }
                this.Dispatcher.Invoke(() =>
                {
                    lbSN.Content = allsn;
                });
                int status = CaptureFOV(ID, allsn, now, savePath, markAdjustInfo.X, markAdjustInfo.Y, markAdjustInfo.Angle, lightStrobe);
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
                        pass = status == 0;
                    }
                    else if (mParam.RUNNING_MODE == 2)
                    {
                        pass = true; // by pass
                    }
                    string runningMode = GetRunningModeString();
                    string viResult = pass ? "PASS" : "FAIL";
                    result = pass ? 0 : -1;
                    if(pass)
                    {
                        // insert db
                        mMyDatabase.InsertNewPanelResult(ID, mModel.Name, now, allsn, runningMode, viResult, viResult);
                    }
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
                int valAlarmMachine = mPlcComm.Get_Error_Machine();
                if (valAlarmMachine == 1 && !mIsShowError)
                {
                    MainConfigWindow.AlarmForm alarm = new MainConfigWindow.AlarmForm();
                    alarm.ShowDialog();
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
        private string GetRunningModeString()
        {
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
            return runningMode;
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
            string[] modelNames = mMyDatabase.GetModelName();
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
                    DateTime[] dt = GetStartnEndTime();
                    int pass = mMyDatabase.CountPass(modelName, dt[0], dt[1]);
                    int fail = mMyDatabase.CountFail(modelName, dt[0], dt[1]);
                    UpdateChartCount(chartYeildRate, txtPass, txtFail, pass, fail);


                    long sumOfPad = (pass + fail) * 2000 ;
                    // sum defect
                    int missing = mMyDatabase.CountDefect(modelName, VI.ErrorType.Missing, dt[0], dt[1]);
                    int insufficient = mMyDatabase.CountDefect(modelName, VI.ErrorType.Insufficient, dt[0], dt[1]);
                    int excess = mMyDatabase.CountDefect(modelName, VI.ErrorType.Excess, dt[0], dt[1]);
                    int bridge = mMyDatabase.CountDefect(modelName, VI.ErrorType.Bridge, dt[0], dt[1]);
                    int overArea = mMyDatabase.CountDefect(modelName, VI.ErrorType.OverArea, dt[0], dt[1]);
                    int lowArea = mMyDatabase.CountDefect(modelName, VI.ErrorType.LowArea, dt[0], dt[1]);
                    int shiftX = mMyDatabase.CountDefect(modelName, VI.ErrorType.ShiftX, dt[0], dt[1]);
                    int shiftY = mMyDatabase.CountDefect(modelName, VI.ErrorType.ShiftY, dt[0], dt[1]);
                    int padAreaError = mMyDatabase.CountDefect(modelName, VI.ErrorType.PadAreaError, dt[0], dt[1]);
                    int sum = missing + insufficient + bridge + excess + overArea + lowArea + shiftX + shiftY + padAreaError;
                    sum = sum == 0 ? 1 : sum;
                    double scalePPM = 1000000 / sumOfPad;
                    mSummary.Clear();
                    mSummary.Add(new Utils.SummaryInfo() { Field = VI.ErrorType.Missing, Count = missing,           PPM = Convert.ToInt32(missing * scalePPM), Rate = Math.Round((double)missing *100 / sum, 2) });
                    mSummary.Add(new Utils.SummaryInfo() { Field = VI.ErrorType.Insufficient, Count = insufficient, PPM = Convert.ToInt32(insufficient * scalePPM), Rate = Math.Round((double)insufficient * 100 / sum, 2) });
                    mSummary.Add(new Utils.SummaryInfo() { Field = VI.ErrorType.Excess, Count = excess,             PPM = Convert.ToInt32(excess * scalePPM), Rate = Math.Round((double)excess * 100 / sum, 2) });
                    mSummary.Add(new Utils.SummaryInfo() { Field = VI.ErrorType.Bridge, Count = bridge,             PPM = Convert.ToInt32(bridge * scalePPM), Rate = Math.Round((double)bridge * 100 / sum, 2) });
                    mSummary.Add(new Utils.SummaryInfo() { Field = VI.ErrorType.OverArea, Count = overArea,         PPM = Convert.ToInt32(overArea * scalePPM), Rate = Math.Round((double)overArea * 100 / sum, 2) });
                    mSummary.Add(new Utils.SummaryInfo() { Field = VI.ErrorType.LowArea, Count = lowArea,           PPM = Convert.ToInt32(lowArea * scalePPM), Rate = Math.Round((double)lowArea * 100 / sum, 2) });
                    mSummary.Add(new Utils.SummaryInfo() { Field = VI.ErrorType.ShiftX, Count = shiftX,             PPM = Convert.ToInt32(shiftX * scalePPM), Rate = Math.Round((double)shiftX * 100 / sum, 2) });
                    mSummary.Add(new Utils.SummaryInfo() { Field = VI.ErrorType.ShiftY, Count = shiftY,             PPM = Convert.ToInt32(shiftY * scalePPM), Rate = Math.Round((double)shiftY * 100 / sum, 2) });
                    mSummary.Add(new Utils.SummaryInfo() { Field = VI.ErrorType.PadAreaError, Count = padAreaError, PPM = Convert.ToInt32(padAreaError * scalePPM), Rate = Math.Round((double)padAreaError * 100 / sum, 2) });
                    mSummary.Add(new Utils.SummaryInfo() { Field = VI.ErrorType.SumOfDefects, Count = sum,          PPM = Convert.ToInt32(sum * scalePPM), Rate = Math.Round((double)sum * 100 / sum, 2) });
                    dgwSummary.Items.Refresh();
                });
                

            }
            else
            {
                UpdateChartCount(chartYeildRate, txtPass, txtFail, 0, 0);
            }
        }
        private DateTime[] GetStartnEndTime()
        {
            DateTime now = DateTime.Now;
            DateTime endTime = now;
            DateTime startTime = now;
            if (rbShift.IsChecked == true)
            {
                DateTime lastDay = now - new TimeSpan(hours: 24, minutes: 0, seconds: 0);
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
            else if (rbDay.IsChecked == true)
            {
                startTime = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
            }
            else if (rbTotal.IsChecked == true)
            {
                startTime = new DateTime(2020, 1, 1, 0, 0, 0);
            }
            return new DateTime[] { startTime, endTime };
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
            mSummary.Add(new Utils.SummaryInfo() { Field = VI.ErrorType.Missing, Count = 0, PPM = 0 , Rate = 0});
            mSummary.Add(new Utils.SummaryInfo() { Field = VI.ErrorType.Insufficient, Count = 0, PPM = 0, Rate = 0 });
            mSummary.Add(new Utils.SummaryInfo() { Field = VI.ErrorType.Excess, Count = 0, PPM = 0, Rate = 0 });
            mSummary.Add(new Utils.SummaryInfo() { Field = VI.ErrorType.Bridge, Count = 0, PPM = 0, Rate = 0 });
            mSummary.Add(new Utils.SummaryInfo() { Field = VI.ErrorType.OverArea, Count = 0, PPM = 0, Rate = 0 });
            mSummary.Add(new Utils.SummaryInfo() { Field = VI.ErrorType.LowArea, Count = 0, PPM = 0, Rate = 0 });
            mSummary.Add(new Utils.SummaryInfo() { Field = VI.ErrorType.ShiftX, Count = 0, PPM = 0, Rate = 0 });
            mSummary.Add(new Utils.SummaryInfo() { Field = VI.ErrorType.ShiftY, Count = 0, PPM = 0, Rate = 0 });
            mSummary.Add(new Utils.SummaryInfo() { Field = VI.ErrorType.PadAreaError, Count = 0, PPM = 0, Rate = 0 });
            mSummary.Add(new Utils.SummaryInfo() { Field = VI.ErrorType.SumOfDefects, Count = 0, PPM = 0, Rate = 0 });
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
                lbSN.Content = "-----";
                lbNoPad.Content = "-----";
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

                mScaner = Devices.MyScaner.GetInstance();
                int stOpenScan = mScaner.Open(mParam.SCANER_COM);
                if (stOpenScan != 0)
                {

                    ReleaseResource();
                    MessageBox.Show(string.Format("Cant open Scanner!"), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    wait.KillMe = true;
                    return;
                }
                
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
                mPlcComm.Set_Speed_Run_X_Bot(mParam.RUN_X_BOT_SPEED);
                mPlcComm.Set_Speed_Run_Y_Bot(mParam.RUN_Y_BOT_SPEED);
                SetButtonRun(Utils.RunMode.START);
                UpdateStatus(Utils.LabelMode.PRODUCT_STATUS, Utils.LabelStatus.READY);
                LoadDetails();
                mIsRunning = true;
                mTimer.Elapsed += OnMainEvent;
                mTimer.Enabled = true;
                mIsInTimer = false;
                wait.KillMe = true;
                mSumPadModel = mModel.Gerber.PadItems.Count;


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
            if (mScaner != null)
            {
                mScaner.Close();
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
                        stackPadError.Items.Add(item);
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
                    stackPadError.Items.Clear();
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
                    lbErrorType.Content = "---";

                });
                UpdateCountStatistical();

            }
        }
        public void EventFOVClick(object sender, RoutedEventArgs e)
        {
            Utils.PadErrorControl item = sender as Utils.PadErrorControl;
            
            if (item != null)
            {
                stackPadError.SelectedIndex = item.ID;
                var padEr = mPadErrorDetails[item.ID];
                int idFov = padEr.FOVNo;
                using (Image<Bgr, byte> imgFOV = new Image<Bgr, byte>(padEr.ImageCapturePath))
                {
                    var loc = padEr.ROIOnImage;
                    loc.Inflate(10, 10);
                    CvInvoke.Rectangle(imgFOV, loc, new MCvScalar(0, 255, 255), 2);
                    BitmapSource bms = Utils.Convertor.Bitmap2BitmapSource(imgFOV.Bitmap);
                    imbFOVError.Source = bms;
                }
                lbErrorType.Content = padEr.ErrorType;
                lbPadErrorArea.Content = string.Format("{0} | ({1} ~ {2})", Math.Round(padEr.Area, 3), padEr.AreaStdLow, padEr.AreaStdHight);
                lbPadErrorShiftX.Content = string.Format("{0} | ({1} ~ {2})", Math.Round(padEr.ShiftX, 3), 0, padEr.ShiftXStduM);
                lbPadErrorShiftY.Content = string.Format("{0} | ({1} ~ {2})", Math.Round(padEr.ShiftY, 3), 0, padEr.ShiftYStduM);
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
            if(mPadErrorDetails.Count > 0)
            {
                string panelStatus = "PASS";
                for (int i = 0; i < mPadErrorDetails.Count; i++)
                {
                    if (mPadErrorDetails[i].ConfirmResult == "FAIL")
                    {
                        panelStatus = "FAIL";
                    }
                    mMyDatabase.InsertNewPadError(
                        mPadErrorDetails[i].PanelID,
                        mPadErrorDetails[i].ModelName,
                        mPadErrorDetails[i].LoadTime,
                        mPadErrorDetails[i].Pad.NoID,
                        mPadErrorDetails[i].FOVNo,
                        mPadErrorDetails[i].ROIOnImage,
                        mPadErrorDetails[i].ROIOnGerber,
                        mPadErrorDetails[i].MachineResult,
                        mPadErrorDetails[i].ConfirmResult,
                        mPadErrorDetails[i].Component,
                        mPadErrorDetails[i].ErrorType,
                        mPadErrorDetails[i].Area,
                        mPadErrorDetails[i].AreaStdHight,
                        mPadErrorDetails[i].AreaStdLow,
                        mPadErrorDetails[i].ShiftX,
                        mPadErrorDetails[i].ShiftXStduM,
                        mPadErrorDetails[i].ShiftY,
                        mPadErrorDetails[i].ShiftYStduM
                        );
                }
                string runningMode = GetRunningModeString();
                mMyDatabase.InsertNewPanelResult(mPadErrorDetails[0].PanelID, mModel.Name, mPadErrorDetails[0].LoadTime, mPadErrorDetails[0].SN, runningMode, "FAIL", panelStatus);
                if(panelStatus == "PASS")
                {
                    mPlcComm.Set_Bit_Comfirm_Pass();
                }
            }
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

        private void stackPadError_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox listBox = sender as ListBox;
            EventFOVClick(listBox.SelectedItem, null);
        }

        private void btPASS_Click(object sender, RoutedEventArgs e)
        {
            int id = stackPadError.SelectedIndex;
            if(id >= 0 && id < stackPadError.Items.Count)
            {
                var r = MessageBox.Show(string.Format("Set PASS for Pad '{0}' ?",mPadErrorDetails[id].Pad.NoID), "Question", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                if(r == MessageBoxResult.Yes)
                {
                    Utils.PadErrorControl item = stackPadError.Items[id] as Utils.PadErrorControl;
                    item.SetStatus(0);
                    mPadErrorDetails[id].ConfirmResult = "PASS";
                }
            }
        }

        private void btAllPASS_Click(object sender, RoutedEventArgs e)
        {
            var r = MessageBox.Show("Set PASS for All Pad?", "Question", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
            if (r == MessageBoxResult.Yes)
            {
                for (int i = 0; i < mPadErrorDetails.Count; i++)
                {
                    Utils.PadErrorControl item = stackPadError.Items[i] as Utils.PadErrorControl;
                    item.SetStatus(0);
                    mPadErrorDetails[i].ConfirmResult = "PASS";
                }
            }
        }

        private void btFAIL_Click(object sender, RoutedEventArgs e)
        {
            int id = stackPadError.SelectedIndex;
            if (id >= 0 && id < stackPadError.Items.Count)
            {
                var r = MessageBox.Show(string.Format("Set NG for Pad '{0}' ?", mPadErrorDetails[id].Pad.NoID), "Question", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                if (r == MessageBoxResult.Yes)
                {
                    Utils.PadErrorControl item = stackPadError.Items[id] as Utils.PadErrorControl;
                    item.SetStatus(0);
                    mPadErrorDetails[id].ConfirmResult = "FAIL";
                }
            }
        }

        private void btPASSFAIL_Click(object sender, RoutedEventArgs e)
        {
            var r = MessageBox.Show("Set NG for All Pad?", "Question", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
            if (r == MessageBoxResult.Yes)
            {
                for (int i = 0; i < mPadErrorDetails.Count; i++)
                {
                    Utils.PadErrorControl item = stackPadError.Items[i] as Utils.PadErrorControl;
                    item.SetStatus(0);
                    mPadErrorDetails[i].ConfirmResult = "FAIL";
                }
            }
        }

        private void btHistory_Click(object sender, RoutedEventArgs e)
        {
            var userType = Login();
            if (userType == UserManagement.UserType.Admin ||
                userType == UserManagement.UserType.Designer ||
                userType == UserManagement.UserType.Engineer)
            {
                Histories.HistoryWindow hist = new Histories.HistoryWindow();
                hist.ShowDialog();
            }
        }
    }
}
