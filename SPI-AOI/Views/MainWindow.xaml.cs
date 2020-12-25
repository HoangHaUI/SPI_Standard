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

namespace SPI_AOI.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        System.Timers.Timer mTimer = new System.Timers.Timer(20);
        Properties.Settings mParam = Properties.Settings.Default;
        List<Utils.SummaryInfo> mSummary = new List<Utils.SummaryInfo>();
        Utils.FOVDisplayInfo mFOVDisplay = new Utils.FOVDisplayInfo();
        MyPLC mPLC = new MyPLC();
        IOT.HikCamera mCamera = null;
        DKZ224V4ACCom mLight = null;
        bool mIsRunning = false;
        bool mIsIdle = false;
        bool mIsInTimer = false;
        Model mModel = null;
        public MainWindow()
        {
            InitializeComponent();
        }
        
        private void Window_Initialized(object sender, EventArgs e)
        {
            InitSummary();
            dgwSummary.ItemsSource = mSummary;
            dgwSummary.Items.Refresh();
            UpdateChartCount(chartYeildRate, txtPass, txtFail, 0, 0);
            LoadModelsName();
            UpdateStatus(Utils.LabelMode.PLC, Utils.LabelStatus.READY);
            UpdateStatus(Utils.LabelMode.DOOR, Utils.LabelStatus.CLOSED);
            UpdateStatus(Utils.LabelMode.RUNNING_MODE, Utils.LabelStatus.TEST);
            UpdateStatus(Utils.LabelMode.MACHINE_STATUS, Utils.LabelStatus.READY);
            UpdateStatus(Utils.LabelMode.PRODUCT_STATUS, Utils.LabelStatus.READY);
            //ColInfo.Width = new GridLength(0);
            //ColStatistical.Width = new GridLength(1, GridUnitType.Star);
        }
        private void OnTimedEvent(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (mIsInTimer)
                return;
            mIsInTimer = true;
            System.Timers.Timer timer = sender as System.Timers.Timer;
            timer.Enabled = false;
            int val = mPLC.Get_Has_Product_Top();
            if(val == 1)
            {
                mIsIdle = false;
                Processing();
                mIsIdle = true;
                mPLC.Reset_Has_Product_Top();
                mPLC.Set_Pass();
            }
            mIsInTimer = false;
            timer.Enabled = mIsRunning;
        }
        private void CaptureMark()
        {
            System.Drawing.Point[] markPoint = mModel.GetRealMarkPosition();
            for (int i = 0; i < markPoint.Length; i++)
            {
                System.Drawing.Point mark = markPoint[i];
                int x = mark.X;
                int y = mark.Y;
                int setX = 0;
                int setY = 0;
                do
                {
                    setX = mPLC.Set_X_Top(x);
                }
                while (setX != x);
                do
                {
                    setY = mPLC.Set_Y_Top(y);
                }
                while (setY != y);
                mPLC.Set_Write_Coordinates_Finish_Top();
                int goFinish = 0;
                do
                {
                    goFinish = mPLC.Get_Go_Coordinates_Finish_Top();
                }
                while (goFinish != 1);
                mPLC.Reset_Go_Coordinates_Finish_Top();
                mLight.ActiveFour(1, 1, 1, 1);
                Thread.Sleep(50);
                using (System.Drawing.Bitmap bm = mCamera.GetOneBitmap(1000))
                {
                    if (bm != null)
                    {
                        System.Drawing.Rectangle ROI = mModel.GetRectROIMark();
                        using (Image<Bgr, byte> img = new Image<Bgr, byte>(bm))
                        {
                            CvInvoke.Imwrite("turn_" + (i + 1).ToString() + ".png", img);
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
                }
                mLight.ActiveFour(0, 0, 0, 0);
            }
        }
        private void CaptureFOV()
        {
            bool activeLight = false;
            System.Drawing.Point[] fovs = mModel.GetFOVPosition();
            for (int i = 0; i < fovs.Length; i++)
            {
                System.Drawing.Point fov = fovs[i];
                int x = fov.X;
                int y = fov.Y;
                int setX = 0;
                int setY = 0;
                do
                {
                    setX = mPLC.Set_X_Top(x);
                }
                while (setX != x);
                do
                {
                    setY = mPLC.Set_Y_Top(y);
                }
                while (setY != y);
                mPLC.Set_Write_Coordinates_Finish_Top();
                int goFinish = 0;
                do
                {
                    goFinish = mPLC.Get_Go_Coordinates_Finish_Top();
                }
                while (goFinish != 1);
                mPLC.Reset_Go_Coordinates_Finish_Top();
                if(!activeLight)
                {
                    mLight.ActiveFour(1, 1, 1, 1);
                    Thread.Sleep(50);
                    activeLight = true;
                }
                using (System.Drawing.Bitmap bm = mCamera.GetOneBitmap(1000))
                {
                    if (bm != null)
                    {
                        using (Image<Bgr, byte> img = new Image<Bgr, byte>(bm))
                        {

                            this.Dispatcher.Invoke(() => {
                                BitmapSource bms = Utils.Convertor.Bitmap2BitmapSource(img.Bitmap);
                                ImbCameraView.Source = bms;
                                lbcountFovs.Content = (i + 1).ToString();
                            });
                        }
                    }
                }
            }
            mLight.ActiveFour(0, 0, 0, 0);
        }
        
        private void Processing()
        {
            UpdateStatus(Utils.LabelMode.MACHINE_STATUS, Utils.LabelStatus.PROCESSING);
            CaptureMark();
            CaptureFOV();
            Thread.Sleep(1000);
            UpdateChartCount(chartYeildRate, txtPass, txtFail, 1, 0);
            UpdateStatus(Utils.LabelMode.PRODUCT_STATUS, Utils.LabelStatus.PASS);
            UpdateStatus(Utils.LabelMode.MACHINE_STATUS, Utils.LabelStatus.IDLE);
        }
        public void SetImageToImb(Image imb, System.Drawing.Bitmap bm)
        {
            this.Dispatcher.Invoke(() => {
                BitmapSource bms = Utils.Convertor.Bitmap2BitmapSource(bm);
                imb.Source = bms;
            });
        }
        private void UpdateChartCount(Chart Chart, TextBox TxtPass, TextBox TxtFail, int Pass, int Fail)
        {
            Chart.BackColor = System.Drawing.Color.Transparent;
            int cvPass = Pass == 0 && Fail == 0 ? 1 : Pass;
            int cvFail = Fail;
            double ratePass = Math.Round((double)cvPass * 100 / (cvPass + cvFail), 1) ;
            double rateFail = Math.Round((double)cvFail * 100 / (cvPass + cvFail), 1);
            this.Dispatcher.Invoke(()=> {
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
        private void UpdateStatus(Utils.LabelMode Label, Utils.LabelStatus Status)
        {
            SolidColorBrush colorMode = GetColorStatus(Status);
            string strMode = GetStringStatus(Status);
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
        private string GetStringStatus(Utils.LabelStatus mode)
        {
            switch (mode)
            {
                case Utils.LabelStatus.PASS:
                    return "PASS";
                case Utils.LabelStatus.FAIL:
                    return "FAIL";
                case Utils.LabelStatus.GOOD:
                    return "GOOD";
                case Utils.LabelStatus.OK:
                    return "OK";
                case Utils.LabelStatus.CLOSED:
                    return "CLOSED";
                case Utils.LabelStatus.OPEN:
                    return "OPEN";
                case Utils.LabelStatus.RUNNING:
                    return "RUNNING";
                case Utils.LabelStatus.CONTROL_RUN:
                    return "CONTROL RUN";
                case Utils.LabelStatus.IDLE:
                    return "IDLE";
                case Utils.LabelStatus.READY:
                    return "READY";
                case Utils.LabelStatus.WAITTING:
                    return "WAITTING";
                case Utils.LabelStatus.PROCESSING:
                    return "PROCESSING";
                case Utils.LabelStatus.ERROR:
                    return "ERROR";
                case Utils.LabelStatus.TEST:
                    return "TESTING";
                default:
                    return "NOT DEFINE";
            }
        }
        private SolidColorBrush GetColorStatus(Utils.LabelStatus mode)
        {
            switch (mode)
            {
                case Utils.LabelStatus.PASS:
                    return Brushes.Green;
                case Utils.LabelStatus.FAIL:
                    return Brushes.Red;
                case Utils.LabelStatus.GOOD:
                    return Brushes.Green;
                case Utils.LabelStatus.OK:
                    return Brushes.Green;
                case Utils.LabelStatus.CLOSED:
                    return Brushes.Green;
                case Utils.LabelStatus.OPEN:
                    return Brushes.Orange;
                case Utils.LabelStatus.RUNNING:
                    return Brushes.YellowGreen;
                case Utils.LabelStatus.CONTROL_RUN:
                    return Brushes.Green;
                case Utils.LabelStatus.IDLE:
                    return Brushes.Gray;
                case Utils.LabelStatus.READY:
                    return Brushes.DeepSkyBlue;
                case Utils.LabelStatus.WAITTING:
                    return Brushes.Orange;
                case Utils.LabelStatus.PROCESSING:
                    return Brushes.Orange;
                case Utils.LabelStatus.ERROR:
                    return Brushes.Red;
                case Utils.LabelStatus.TEST:
                    return Brushes.Orange;
                default:
                    return Brushes.Green;
            }
        }
        private void LoadDetails()
        {
            this.Dispatcher.Invoke(() => {
                lbModelName.Content = mModel.Name;
                lbLoadTime.Content = DateTime.Now.ToString("HH:mm:ss   dd/MM/yyyy");
                lbFovs.Content = mModel.Gerber.FOVs.Count.ToString() + " FOVs";
                lbGerberFile.Content = mModel.Gerber.FileName;
                lbCircleTime.Content = "30.5 seconds";
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
        private void UpdateFOVDisplay()
        {
            if(mModel != null && mIsRunning)
            {
                using (Image<Bgr, byte> imgDigram = mModel.GetDiagramImage())
                {
                    System.Drawing.Point[] anchors = mModel.GetAngchorsDiagram();
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
                        mFOVDisplay.StartPoint = new System.Windows.Point[anchors.Length];
                        mFOVDisplay.Witdh = showDisplayWidth;
                        mFOVDisplay.Height = showDisplayHeight;
                        for (int i = 0; i < mFOVDisplay.StartPoint.Length; i++)
                        {
                            mFOVDisplay.StartPoint[i] = new Point(
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
                    MessageBox.Show(string.Format("Cant load {0} model", modelName), "Error", MessageBoxButton.OK);
                    return;
                }
                using (Image<Bgr, byte> imgDigram = mModel.GetDiagramImage())
                {
                    SetImageToImb(imbDiagram, imgDigram.Bitmap);
                    UpdateFOVDisplay();
                    SetDisplayFOV(-1);
                }
                wait.LabelContent = "Connecting to PLC...";
                int ping = mPLC.Ping();
                if (ping != 0)
                {
                   
                    ReleaseResource();
                    wait.KillMe = true;
                    UpdateStatus(Utils.LabelMode.PLC, Utils.LabelStatus.FAIL);
                    MessageBox.Show(string.Format("Cant ping to PLC  IP :{0}:{1}...", mParam.PLC_IP, mParam.PLC_PORT), "Error", MessageBoxButton.OK);
                    return;
                }
                UpdateStatus(Utils.LabelMode.PLC, Utils.LabelStatus.OK);
                wait.LabelContent = "Connecting to Camera...";
                mCamera = MyCamera.GetInstance();
                if (mCamera == null)
                {

                    ReleaseResource();
                    wait.KillMe = true;
                    MessageBox.Show(string.Format("Not found camera!"), "Error", MessageBoxButton.OK);
                    return;
                }
                int stOpenCamera = mCamera.Open();
                if (stOpenCamera != 0)
                {

                    ReleaseResource();
                    MessageBox.Show(string.Format("Cant open the camera!"), "Error", MessageBoxButton.OK);
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
                    MessageBox.Show(string.Format("Cant connect to light source controller!"), "Error", MessageBoxButton.OK);
                    return;
                }
                int[] intensity = mModel.HardwareSettings.LightIntensity;
                mLight.SetFour(intensity[0], intensity[1], intensity[2], intensity[3]);
                mLight.ActiveFour(0, 0, 0, 0);
                mPLC.Logout();
                int conveyorPulse = mPLC.Get_Conveyor();
                if (conveyorPulse != mModel.HardwareSettings.Conveyor)
                {
                    wait.LabelContent = "Moving Conveyor...";
                    mPLC.Set_Speed_Run_Conveyor(25000);

                    mPLC.Set_Conveyor(Convert.ToInt32(mModel.HardwareSettings.Conveyor));
                    mPLC.Set_Write_Coordinates_Finish_Conveyor();

                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    int val = mPLC.Get_Go_Coordinates_Finish_Conveyor();
                    while (val != 1 && sw.ElapsedMilliseconds < 120000)
                    {
                        Thread.Sleep(100);
                        val = mPLC.Get_Go_Coordinates_Finish_Conveyor();
                    }
                    
                    if (sw.ElapsedMilliseconds > 120000)
                    {
                        MessageBox.Show("Timeout move conveyor, please try again!", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                        ReleaseResource();
                        wait.KillMe = true;
                        return;
                    }
                    Thread.Sleep(500);
                    mPLC.Reset_Go_Coordinates_Finish_Conveyor();
                }
                wait.LabelContent = "Init Parameter...";
                mPLC.Set_Speed_Run_X_Top(200000);
                mPLC.Set_Speed_Run_Y_Top(200000);
                FileInfo fi = new FileInfo("icon/stop.png");
                imbBtRun.Source = new BitmapImage(new Uri(fi.FullName));
                if (mParam.RUNNING_MODE == "TEST")
                {
                    UpdateStatus(Utils.LabelMode.RUNNING_MODE, Utils.LabelStatus.TEST);
                }
                else
                {
                    UpdateStatus(Utils.LabelMode.RUNNING_MODE, Utils.LabelStatus.CONTROL_RUN);
                }
                UpdateStatus(Utils.LabelMode.MACHINE_STATUS, Utils.LabelStatus.IDLE);
                LoadDetails();
                mIsIdle = true;
                mIsRunning = true;
                mTimer.Elapsed += OnTimedEvent;
                mTimer.Enabled = true;
                wait.KillMe = true;
            });
            startThread.Start();
            wait.ShowDialog();
        }
        private void StopRunMode()
        {
            if(mIsIdle)
            {
                mIsRunning = false;
                this.Dispatcher.Invoke(() => {
                    
                    FileInfo fi = new FileInfo("icon/start.png");
                    imbBtRun.Source = new BitmapImage(new Uri(fi.FullName));
                    });
                UpdateStatus(Utils.LabelMode.MACHINE_STATUS, Utils.LabelStatus.READY);
                UpdateStatus(Utils.LabelMode.PLC, Utils.LabelStatus.READY);
                mLight.ActiveFour(0, 0, 0, 0);
                ReleaseResource();
                ResetDetails();
                mModel.Dispose();
                mModel = null;
            }
           else
            {
                MessageBox.Show(string.Format("Cant stop because the machine is a progress..."), "Error", MessageBoxButton.OK);
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
    }
}
