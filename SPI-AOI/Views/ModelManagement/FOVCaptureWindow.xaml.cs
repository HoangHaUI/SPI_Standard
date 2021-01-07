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
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using SPI_AOI.Models;


namespace SPI_AOI.Views.ModelManagement
{
    /// <summary>
    /// Interaction logic for FOVCaptureWindow.xaml
    /// </summary>
    public partial class FOVCaptureWindow : Window
    {
        Model mModel = null;
        public FOVCaptureWindow(Model model)
        {
            mModel = model;
            InitializeComponent();
            
        }
        private void Window_Initialized(object sender, EventArgs e)
        {
            LoadUI();
        }
        private void LoadUI()
        {
            switch (mModel.Gerber.StartPoint)
            {
                case Utils.StartPoint.TOP_LEFT:
                    rbTopLeft.IsChecked = true;
                    break;
                case Utils.StartPoint.BOT_LEFT:
                    rbBotLeft.IsChecked = true;
                    break;
                case Utils.StartPoint.TOP_RIGHT:
                    rbTopRight.IsChecked = true;
                    break;
                case Utils.StartPoint.BOT_RIGHT:
                    rbBitRight.IsChecked = true;
                    break;
                default:
                    break;
            }
            lbNumFovs.Content = mModel.Gerber.FOVs.Count.ToString();
            LoadDiagram(-1);
            RenderAnchorDetails();
        }
        private void LoadDiagram(int Hightlight = -1)
        {

            using (Image<Bgr, byte> img = mModel.GetFOVImage(Hightlight))
            {
                BitmapSource bms = SPI_AOI.Utils.Convertor.Bitmap2BitmapSource(img.ToBitmap());
                imbView.Source = bms;
            }
        }
        private void RenderAnchorDetails()
        {
            dgwAnchors.ItemsSource = mModel.Gerber.FOVs;
            dgwAnchors.Items.Refresh();
        }
        private void rbBitRight_Checked(object sender, RoutedEventArgs e)
        {
            // bot right
            if ((sender as RadioButton).IsChecked == true)
            {
                mModel.Gerber.StartPoint = Utils.StartPoint.BOT_RIGHT;
            }
            mModel.UpdateFOV();
            LoadDiagram(0);
            RenderAnchorDetails();
        }

        private void rbBotLeft_Checked(object sender, RoutedEventArgs e)
        {
            if ((sender as RadioButton).IsChecked == true)
            {
                mModel.Gerber.StartPoint = Utils.StartPoint.BOT_LEFT;
            }
            mModel.UpdateFOV();
            LoadDiagram(0);
            RenderAnchorDetails();
        }

        private void rbTopRight_Checked(object sender, RoutedEventArgs e)
        {
            if ((sender as RadioButton).IsChecked == true)
            {
                mModel.Gerber.StartPoint = Utils.StartPoint.TOP_RIGHT;
            }
            mModel.UpdateFOV();
            LoadDiagram(0);
            RenderAnchorDetails();
        }

        private void dgwAnchors_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void dgwAnchors_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            var item = dgwAnchors.CurrentCell.Item;

            if (item != CollectionView.NewItemPlaceholder && item != DependencyProperty.UnsetValue)
            {
                LoadDiagram(dgwAnchors.SelectedIndex);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        

        private void rbTopLeft_Checked(object sender, RoutedEventArgs e)
        {
            if ((sender as RadioButton).IsChecked == true)
            {
                mModel.Gerber.StartPoint = Utils.StartPoint.TOP_LEFT;
            }
            mModel.UpdateFOV();
            LoadDiagram(0);
            RenderAnchorDetails();
        }

        private void Border_MouseWheel(object sender, MouseWheelEventArgs e)
        {
           
        }
    }
}
