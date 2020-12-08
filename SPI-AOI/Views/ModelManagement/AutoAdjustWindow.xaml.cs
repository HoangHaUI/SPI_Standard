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
using System.Collections;
using SPI_AOI.Models;

namespace SPI_AOI.Views.ModelManagement
{
    /// <summary>
    /// Interaction logic for AutoAdjustWindow.xaml
    /// </summary>
    public partial class AutoAdjustWindow : Window
    {
        List<GerberFile> mGerbers = new List<GerberFile>();
        List<CadFile> mCads = new List<CadFile>();
        GerberTools mGerberToolsWindow = null;
        public AutoAdjustWindow(List<GerberFile> gerbers, List<CadFile> cads, GerberTools gerberTools)
        {
            mGerbers = gerbers;
            mCads = cads;
            mGerberToolsWindow = gerberTools;
            InitializeComponent();
            
        }
        private void Window_Initialized(object sender, EventArgs e)
        {
            cbCad.ItemsSource = mCads;
            cbGerber.ItemsSource = mGerbers;
            cbCad.Items.Refresh();
            cbGerber.Items.Refresh();
            if (cbCad.Items.Count > 0)
            {
                cbCad.SelectedIndex = 0;
            }
            if (cbGerber.Items.Count > 0)
            {
                cbGerber.SelectedIndex = 0;
            }
            cbGerber_SelectionChanged(null, null);
        }
        private void cbGerber_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(cbGerber.SelectedIndex > -1 && cbCad.SelectedIndex > -1)
            {
                button.IsEnabled = true;
            }
            else
            {
                button.IsEnabled = false;
            }
        }

        private void cbCad_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cbGerber_SelectionChanged(null, null);
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            
            GerberFile gerber = mGerbers[cbGerber.SelectedIndex];
            CadFile cad = mCads[cbCad.SelectedIndex]; ;
            System.Drawing.Point cGerber = gerber.GetCenterPadsSelected();
            System.Drawing.Point cCad = cad.GetCenterSelected();
            if (cGerber != new System.Drawing.Point() && cCad != new System.Drawing.Point())
            {
                int subx = cGerber.X - cCad.X;
                int suby = cGerber.Y - cCad.Y;
                cad.X += subx;
                cad.Y += suby;
                mGerberToolsWindow.ShowAllLayerImb(ActionMode.Draw_Cad);
            }
            else
            {
                MessageBox.Show(string.Format("Please select item on the layers selected..."), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
