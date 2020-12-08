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
    /// Interaction logic for AvailabilityLayerWindow.xaml
    /// </summary>
    public partial class AvailabilityLayerWindow : Window
    {
        List<object> mListLayers = new List<object>();
        public int ItemSelected { get; set; }
        public AvailabilityLayerWindow(List<object> Layers)
        {
            ItemSelected = -1;
            mListLayers = Layers;
            InitializeComponent();
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            cbLayer.ItemsSource = mListLayers;
            cbLayer.Items.Refresh();
            if (cbLayer.Items.Count > 0)
            {
                cbLayer.SelectedIndex = 0;
            }
        }

        private void btSelect_Click(object sender, RoutedEventArgs e)
        {
            ItemSelected = cbLayer.SelectedIndex;
            this.Close();
        }

        private void cbLayer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(cbLayer.SelectedIndex > -1)
            {
                btSelect.IsEnabled = true;
            }
            else
            {
                btSelect.IsEnabled = false;
            }
        }
    }
}
