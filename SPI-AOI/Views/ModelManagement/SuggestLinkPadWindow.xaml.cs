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
    public partial class SuggestLinkPadWindow : Window
    {
        List<SuggestCadItem> mListSuggest = new List<SuggestCadItem>();
        public int ItemSelected { get; set; }
        public SuggestLinkPadWindow(List<Tuple<CadFile, int>> Layers)
        {
            ItemSelected = -1;
            for (int i = 0; i < Layers.Count; i++)
            {
                System.Drawing.Color color = Layers[i].Item1.Color;
                string name = Layers[i].Item1.CadItems[Layers[i].Item2].Name;
                mListSuggest.Add(new SuggestCadItem(color, name));
            }
            InitializeComponent();
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            cbLayer.ItemsSource = mListSuggest;
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
    public class SuggestCadItem
    {
        public System.Drawing.Color Color { get; set; }
        public string Name { get; set; }
        public SuggestCadItem(System.Drawing.Color Color, string Name)
        {
            this.Color = Color;
            this.Name = Name;
        }
        public SuggestCadItem() { }
    }
}
