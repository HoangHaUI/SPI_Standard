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
        bool mIsRunning = false;
        bool mIsProcessing = false;
        bool mIsInTimer = false;
        bool mIsInCheckTimer = false;
        bool mPingPLCOK = true;
        bool mIsCheck = true;
        bool mIsLoaded = false;
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
            //ColInfo.Width = new GridLength(0);
            //ColStatistical.Width = new GridLength(1, GridUnitType.Star);
            //mMyDBResult.InsertNewImage("sdvajshdasd", DateTime.Now, "now", new System.Drawing.Rectangle(1,2,3,5), new System.Drawing.Rectangle(4,3,2,1), "Image");
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
            if(val == 1)
            {
                mIsProcessing = true;
                Processing();
                mIsProcessing = false; 
                mPlcComm.Reset_Has_Product_Top();
                mPlcComm.Set_Pass();
            }
            mIsInTimer = false;
            timer.Enabled = mIsRunning;
        }
        
        private int CaptureMark(string SavePath, bool LightStrobe)
        {
            System.Drawing.Point[] markPoint = mModel.GetPLCMarkPosition();
            bool captureError = false;
            for (int i = 0; i < markPoint.Length; i++)
            {
                System.Drawing.Point mark = markPoint[i];
                int x = mark.X;
                int y = mark.Y;
                mLog.Info(string.Format("{0}, Position Name : {1},  X = {2}, Y = {3}", "Moving TOP Axis", "Mark " + (i+1).ToString(), x, y));
                bool ret = mPlcComm.SetXYTop(x, y);
                if (!ret)
                {
                    captureError = false;
                    break;
                }
                mPlcComm.Set_Write_Coordinates_Finish_Top();
                ret = mPlcComm.GoFinishTop();
                if (!ret)
                {
                    captureError = false;
                    break;
                }
                mPlcComm.Reset_Go_Coordinates_Finish_Top();
                if(LightStrobe)
                {
                    mLight.ActiveFour(1, 1, 1, 1);
                    Thread.Sleep(20);
                }
                
                using (System.Drawing.Bitmap bm = mCamera.GetOneBitmap(1000))
                {
                    if (bm != null)
                    {
                        System.Drawing.Rectangle ROI = mModel.GetRectROIMark();
                        using (Image<Bgr, byte> img = new Image<Bgr, byte>(bm))
                        {
                            string fileName = string.Format("{0}//Mark_{1}_{2}_{3}_{4}_{5}.png", SavePath, i + 1, ROI.X, ROI.Y, ROI.Width, ROI.Height);
                            CvInvoke.Imwrite(fileName, img);
                            img.ROI = ROI;
                           this.Dispatcher.Invoke(() => {
                            BitmapSource bms = Utils.Convertor.Bitmap2BitmapSource(img.Bitmap);
                                if(i == 0)
                                {
                                    imbMark1.Source = bms;
                                }
                                else
                                {
                                    imbMark2.Source = bms;
                                }
                            });
                        }
                    }
                    else
                    {
                        mLog.Info(string.Format("Cant Capture image in Mark : {0}", i + 1));
                    }
                }
                if(LightStrobe)
                {
                    mLight.ActiveFour(0, 0, 0, 0);
                }
            }
            if (captureError)
            {
                return -1;
            }
            return 0;
        }
        private int CaptureFOV(string SavePath, bool LightStrobe)
        {
            bool captureError = false;
            System.Drawing.Point[] xyAxisPosition = mModel.GetPulseXYFOVs();
            System.Drawing.Point[] Fovs = mModel.GetAnchorsFOV();
            mModel.Gerber.ProcessingGerberImage.ROI = mModel.Gerber.ROI;
            using (Image<Bgr, byte> imgGraft = new Image<Bgr, byte>(mModel.Gerber.ROI.Size))
            using (Image<Gray, byte> imgGerber = mModel.Gerber.ProcessingGerberImage.Copy())
            {
                for (int i = 0; i < xyAxisPosition.Length; i++)
                {
                    System.Drawing.Point fov = xyAxisPosition[i];
                    int x = fov.X;
                    int y = fov.Y;
                    mLog.Info(string.Format("{0}, Position Name : {1},  X = {2}, Y = {3}", "Moving TOP Axis", "FOV " + (i + 1).ToString(), x, y));
                    bool ret = mPlcComm.SetXYTop(x, y);
                    if(!ret)
                    {
                        captureError = false;
                        break;
                    }
                    mPlcComm.Set_Write_Coordinates_Finish_Top();
                    ret = mPlcComm.GoFinishTop();
                    if (!ret)
                    {
                        captureError = false;
                        break;
                    }
                    mPlcComm.Reset_Go_Coordinates_Finish_Top();
                    if (LightStrobe)
                    {
                        mLight.ActiveFour(1, 1, 1, 1);
                        Thread.Sleep(30);
                    }
                    using (System.Drawing.Bitmap bm = mCamera.GetOneBitmap(1000))
                    {
                        if (bm != null)
                        {
                            SetDisplayFOV(i);
                            
                            Image<Bgr, byte> imgCap = new Image<Bgr, byte>(bm);
                            {
                                var modelFov = mModel.FOV;
                                System.Drawing.Rectangle ROI = new System.Drawing.Rectangle(
                                imgCap.Width / 2 - modelFov.Width / 2, imgCap.Height / 2 - modelFov.Height / 2,
                                modelFov.Width, modelFov.Height);
                                System.Drawing.Rectangle ROIGerber = new System.Drawing.Rectangle(
                                    Fovs[i].X - modelFov.Width / 2, Fovs[i].Y - modelFov.Height / 2,
                                    modelFov.Width, modelFov.Height);
                                
                                using (Image<Bgr, byte> imgRotated = ImageProcessingUtils.ImageRotation(imgCap, new System.Drawing.Point(imgCap.Width / 2, imgCap.Height / 2), -mModel.AngleAxisCamera * Math.PI / 180.0))
                                using (Image<Bgr, byte> imgUndis = new Image<Bgr, byte>(imgRotated.Size))
                                {
                                    CvInvoke.Undistort(imgRotated, imgUndis, mCalibImage.CameraMatrix, mCalibImage.DistCoeffs, mCalibImage.NewCameraMatrix);
                                    imgUndis.ROI = ROI;
                                    imgGerber.ROI = ROIGerber;
                                    string fileName = string.Format("{0}//Image_{1}_ROI({2}_{3}_{4}_{5})_ROI_GERBER({6}_{7}_{8},{9}).png", 
                                        SavePath, i + 1, ROI.X, ROI.Y, ROI.Width, ROI.Height,
                                        ROIGerber.X, ROIGerber.Y, ROIGerber.Width, ROIGerber.Height);
                                    CvInvoke.Imwrite(fileName, imgUndis);
                                    imgGerber.ROI = System.Drawing.Rectangle.Empty;
                                    this.Dispatcher.Invoke(() => {
                                        BitmapSource bms = Utils.Convertor.Bitmap2BitmapSource(imgUndis.Bitmap);
                                        ImbCameraView.Source = bms;
                                        lbcountFovs.Content = (i + 1).ToString();
                                    });
                                }
                            }
                        }
                        else
                        {
                            mLog.Info(string.Format("Cant Capture image in FOV : {0}", i + 1));
                        }
                    }
                    if(LightStrobe)
                    {
                        mLight.ActiveFour(0, 0, 0, 0);
                    }
                }
            }
            mModel.Gerber.ProcessingGerberImage.ROI = System.Drawing.Rectangle.Empty;
            
            if (captureError)
            {
                return -1;
            }
            return 0;
        }
        
        private void Processing()
        {
            string date = DateTime.Now.ToString("yyyy_MM_dd");
            string time = DateTime.Now.ToString("HH_mm_ss");
            string sn = "NOT FOUND";
            string savePath = mParam.SAVE_IMAGE_PATH + "\\" + date + "\\TIME(" + time + ")_._SN(" + sn + ")";
            if(!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }
            bool lightStrobe = Convert.ToBoolean(mParam.LIGHT_MODE);
            ResetUI();
            UpdateStatus(Utils.LabelMode.MACHINE_STATUS, Utils.LabelStatus.PROCESSING);
            UpdateStatus(Utils.LabelMode.PRODUCT_STATUS, Utils.LabelStatus.PROCESSING);
            if(!lightStrobe)
            {
                mLight.ActiveFour(1, 1, 1, 1);
                Thread.Sleep(30);
            }
            int capMarkStatus = CaptureMark(savePath, lightStrobe);
            if(capMarkStatus != -1)
            {
                int capFOVStatus = CaptureFOV(savePath, lightStrobe);
                if(capFOVStatus != -1)
                {
                    // capture fail

                }
                if (!lightStrobe)
                {
                    mLight.ActiveFour(0, 0, 0, 0);
                }
                if(mParam.RUNNING_MODE == 0)
                {
                    // control run or test

                }
                else if(mParam.RUNNING_MODE == 2)
                {
                    // bypass
                    UpdateStatus(Utils.LabelMode.PRODUCT_STATUS, Utils.LabelStatus.PASS);
                }
            }
            
            SetDisplayFOV(-1);
            UpdateChartCount(chartYeildRate, txtPass, txtFail, 10, 1);
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
                if (cbModelStatistical.Items.Contains(selected))
                {
                    cbModelStatistical.SelectedItem = selected;
                }
            });
            UpdateCountStatistical();
        }
        public void UpdateCountStatistical()
        {
            if (cbModelStatistical.SelectedIndex >= 0)
            {
                string modelName = cbModelStatistical.SelectedItem.ToString();
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
            }
            else
            {
                UpdateChartCount(chartYeildRate, txtPass, txtFail, 0, 0);
            }
        }
        private void UpdateChartCount(Chart Chart, TextBox TxtPass, TextBox TxtFail, int Pass, int Fail)
        {
            Chart.BackColor = System.Drawing.Color.Transparent;
            int cvPass = Pass == 0 && Fail == 0 ? 1 : Pass;
            int cvFail = Fail;
            double ratePass = Math.Round((double)cvPass * 100 / (cvPass + cvFail), 1);
            double rateFail = Math.Round((double)cvFail * 100 / (cvPass + cvFail), 1);
            this.Dispatcher.Invoke(() => {
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
            mSummary.Add(new Utils.SummaryInfo() { Field = "Shift X Hight", Count = 0, PPM = 0 });
            mSummary.Add(new Utils.SummaryInfo() { Field = "Shift X Low", Count = 0, PPM = 0 });
            mSummary.Add(new Utils.SummaryInfo() { Field = "Shift Y Hight", Count = 0, PPM = 0 });
            mSummary.Add(new Utils.SummaryInfo() { Field = "Shift Y Low", Count = 0, PPM = 0 });
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
                lbCircleTime.Content = "20.0 seconds";
                lbTotalCountFovs.Content = mModel.Gerber.FOVs.Count.ToString();
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
    }
}
