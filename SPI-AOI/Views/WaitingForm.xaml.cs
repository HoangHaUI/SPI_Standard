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


namespace SPI_AOI.Views
{
    /// <summary>
    /// Interaction logic for WaitingForm.xaml
    /// </summary>
    public partial class WaitingForm : Window
    {
        private bool mKillMe = false;
        private string mContent = "Processing...";

        private System.Timers.Timer mTimer = new System.Timers.Timer(1000);
        private int mCountSecond = 0;
        private int mTimeOut = 180;
        public string LabelContent
        {
            get
            {
                return mContent;
            }
            set
            {
                mContent = value;
                mCountSecond = 0;
                this.Dispatcher.Invoke(() => {
                    lbStatus.Content = mContent;
                });
            }
        }
        private void OntimedEvent(object sender, System.Timers.ElapsedEventArgs e)
        {
            mCountSecond++;
            if(mCountSecond > mTimeOut)
            {
                mTimer.Enabled = false;
                KillMe = true;
            }
        }
        public WaitingForm(string Content = "Processing...", int Timeout = 180)
        {
            InitializeComponent();
            this.LabelContent = Content;
            mTimeOut = Timeout;
            mTimer.Elapsed += OntimedEvent;
            mTimer.Enabled = true;
        }
        public WaitingForm(string Content = "Processing...")
        {
            InitializeComponent();
            this.LabelContent = Content;
            mTimer.Elapsed += OntimedEvent;
            mTimer.Enabled = true;
        }
        public bool KillMe
        {
            get { return mKillMe; }
            set
            {
                mKillMe = value;
                if(mKillMe == true)
                {
                    this.Dispatcher.Invoke(() => {
                        mTimer.Enabled = false;
                        this.Close();
                    });
                }
            }
        }
    }
}
