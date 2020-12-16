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



namespace SPI_AOI.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        System.Timers.Timer mTimer = new System.Timers.Timer(10);
        List<Utils.SummaryInfo> mSummary = new List<Utils.SummaryInfo>();
        bool mIsRunning = false;
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
            System.Timers.Timer timer = sender as System.Timers.Timer;
            timer.Enabled = false;
            timer.Enabled = mIsRunning;
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
        private void UpdateBackgroundImage(Border border, Image image)
        {
            if (image.Source != null)
            {
                border.Background = new SolidColorBrush(Color.FromRgb(0x00, 0x32, 0x00));
            }
            else
            {
                border.Background = Brushes.Gray;
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
    }
}
