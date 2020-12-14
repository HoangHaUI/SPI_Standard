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
            UpdateChartCount(chartYeildRate, txtPass, txtFail, 15, 5);
        }
        private void OnTimedEvent(object sender, System.Timers.ElapsedEventArgs e)
        {
            System.Timers.Timer timer = sender as System.Timers.Timer;
            timer.Enabled = false;
            

            timer.Enabled = mIsRunning;
        }
        private void UpdateChartCount(Chart Chart, TextBox TxtPass, TextBox TxtFail, int Pass, int Fail)
        {
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
                MainConfigWindow.PLCBitDefineWindow mainConfig = new MainConfigWindow.PLCBitDefineWindow();
                mainConfig.ShowDialog();
            }
        }
    }
}
