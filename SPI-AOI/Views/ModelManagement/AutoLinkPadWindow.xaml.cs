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

namespace SPI_AOI.Views.ModelManagement
{
    /// <summary>
    /// Interaction logic for AutoLinkPadWindow.xaml
    /// </summary>
    public partial class AutoLinkPadWindow : Window
    {
        public Utils.AutoLinkMode ModeLinkPad = Utils.AutoLinkMode.NotLink;
        public AutoLinkPadWindow()
        {
            InitializeComponent();
        }

        private void btApply_Click(object sender, RoutedEventArgs e)
        {
            if(rbRnC.IsChecked == true)
            {
                ModeLinkPad = Utils.AutoLinkMode.RnC;
            }
            else if(rb2Pad.IsChecked == true)
            {
                ModeLinkPad = Utils.AutoLinkMode.TwoPad;
            }
            else if(rbAll.IsChecked == true)
            {
                ModeLinkPad = Utils.AutoLinkMode.All;
            }
            this.Close();
        }
    }
    
}
