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
using System.ComponentModel;

namespace SPI_AOI.Views.MainConfigWindow
{
    /// <summary>
    /// Interaction logic for PLCBitDefineWindow.xaml
    /// </summary>
    public partial class PLCBitDefineWindow : Window
    {
        private Properties.Settings mParam = Properties.Settings.Default;
        
        public PLCBitDefineWindow()
        {
            InitializeComponent();
        }
    }
}
