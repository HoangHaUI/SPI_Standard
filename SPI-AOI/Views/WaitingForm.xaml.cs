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
        public string LabelContent
        {
            get
            {
                return mContent;
            }
            set
            {
                mContent = value;
                this.Dispatcher.Invoke(() => {
                    lbStatus.Content = mContent;
                });
            }
        }
        public WaitingForm(string Content = "Processing...")
        {
            InitializeComponent();
            this.LabelContent = Content;

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
                        this.Close();
                    });
                }
            }
        }
    }
}
