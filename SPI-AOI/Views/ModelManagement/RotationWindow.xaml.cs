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
    /// Interaction logic for RotationWindow.xaml
    /// </summary>
    public partial class RotationWindow : Window
    {
        GerberTools mGerberToolsForm = null;
        List<object> mListCadFiles = new List<object>();
        public RotationWindow(GerberTools GerberToolsForm)
        {
            mGerberToolsForm = GerberToolsForm;
            InitializeComponent();
        }
        private void UpdateListImportedFile()
        {
            mListCadFiles.Clear();
            if (mGerberToolsForm.mModel.Gerber is GerberFile)
            {

                mListCadFiles.Add(mGerberToolsForm.mModel.Gerber);
            }
            for (int i = 0; i < mGerberToolsForm.mModel.Cad.Count; i++)
            {
                mListCadFiles.Add(mGerberToolsForm.mModel.Cad[i]);
            }
            this.Dispatcher.Invoke(() =>
            {
                cbLayer.SelectedIndex = -1;
                cbLayer.Items.Refresh();
            });
        }
        private void btRefresh_Click(object sender, RoutedEventArgs e)
        {
            UpdateListImportedFile();
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            cbLayer.ItemsSource = mListCadFiles;
            UpdateListImportedFile();
            cbLayer_SelectionChanged(null, null);
        }

        private void cbLayer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(cbLayer.SelectedIndex > -1)
            {
                btRotate90.IsEnabled = true;
                btRotate_90.IsEnabled = true;
                
                object item = cbLayer.SelectedItem;
                if (item is CadFile)
                {
                    txtX.IsEnabled = true;
                    txtY.IsEnabled = true;
                    btSetY.IsEnabled = true;
                    btSetX.IsEnabled = true;
                    txtX.Text = ((CadFile)item).X.ToString();
                    txtY.Text = ((CadFile)item).Y.ToString();
                    
                }
            }
            else
            {
                btRotate90.IsEnabled = false;
                btRotate_90.IsEnabled = false;
                txtX.IsEnabled = false;
                txtY.IsEnabled = false;
                btSetY.IsEnabled = false;
                btSetX.IsEnabled = false;
            }
            
        }
        
        private void RotateAngle(double angle)
        {
            object item = cbLayer.SelectedItem;
            if(item is GerberFile)
            {
                mGerberToolsForm.mModel.RotateGerber(angle);
                mGerberToolsForm.ShowAllLayerImb(ActionMode.Render);
            }
            else if(item is CadFile)
            {
                ((CadFile)item).Angle += angle;
                ((CadFile)item).Angle = ((CadFile)item).Angle % 360;
                mGerberToolsForm.ShowAllLayerImb(ActionMode.Draw_Cad);
            }
        }
        private void btRotate_90_Click(object sender, RoutedEventArgs e)
        {
            RotateAngle(-90);
        }

        private void btRotate90_Click(object sender, RoutedEventArgs e)
        {
            RotateAngle(90);
        }

        private void btSetX_Click(object sender, RoutedEventArgs e)
        {
            int val = 0;
            try
            {
                val = Convert.ToInt32(Convert.ToDouble(txtX.Text));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            object item = cbLayer.SelectedItem;
            if (item is CadFile)
            {
                CadFile cad = (CadFile)item;
                cad.X = val;
                mGerberToolsForm.ShowAllLayerImb(ActionMode.Draw_Cad);
            }
        }

        private void btSetY_Click(object sender, RoutedEventArgs e)
        {
            int val = 0;
            try
            {
                val = Convert.ToInt32(Convert.ToDouble(txtY.Text));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            object item = cbLayer.SelectedItem;
            if (item is CadFile)
            {
                CadFile cad = (CadFile)item;
                cad.Y = val;
                mGerberToolsForm.ShowAllLayerImb(ActionMode.Draw_Cad);
            }
        }
    }
}
