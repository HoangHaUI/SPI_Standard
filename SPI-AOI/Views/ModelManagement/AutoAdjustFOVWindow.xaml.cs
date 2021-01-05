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
using SPI_AOI.Models;

namespace SPI_AOI.Views.ModelManagement
{
    /// <summary>
    /// Interaction logic for AutoAdjustFOVWindow.xaml
    /// </summary>
    public partial class AutoAdjustFOVWindow : Window
    {
        Model mModel = null;
        public AutoAdjustFOVWindow(Model model)
        {
            mModel = model;
            InitializeComponent();
        }

        private void Window_Initialized(object sender, EventArgs e)
        {

        }
    }
}
