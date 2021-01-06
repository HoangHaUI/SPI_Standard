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
using System.IO;

namespace SPI_AOI.Views.ModelManagement
{
    /// <summary>
    /// Interaction logic for DashBoard.xaml
    /// </summary>
    public partial class DashBoard : Window
    {
        Model mModel = null;
        public DashBoard()
        {
            InitializeComponent();
        }
        private void Window_Initialized(object sender, EventArgs e)
        {
            
            ResetDetails();
            btReload_Click(null, null);
            cbModelsName_SelectionChanged(null, null);
        }
        private void LoadModelsName()
        {
            string selected = Convert.ToString(cbModelsName.SelectedItem);
            cbModelsName.Items.Clear();
            string[] modelNames = Model.GetModelNames();
            if(modelNames != null)
            {
                for (int i = 0; i < modelNames.Length; i++)
                {
                    cbModelsName.Items.Add(modelNames[i]);
                }
            }
            if (modelNames.Contains(selected))
            {
                cbModelsName.SelectedItem = selected;
            }
        }
        private void cbModelsName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (mModel != null)
            {
                mModel.Dispose();
                mModel = null;
            }
            if (cbModelsName.SelectedIndex > -1)
            {
                string modelName = cbModelsName.SelectedItem.ToString();
                int id = cbModelsName.SelectedIndex;
                mModel = Model.LoadModelByName(cbModelsName.SelectedItem.ToString());
                if (mModel != null)
                {
                    // insert model to config
                    gridConfig.IsEnabled = true;
                    LoadDetails();
                }
                else
                {
                    MessageBox.Show(string.Format("Cant load '{0}' model...", modelName), "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                    gridConfig.IsEnabled = false;
                }
            }
            else
            {
                gridConfig.IsEnabled = false;
                ResetDetails();
            }
        }

        private void btReload_Click(object sender, RoutedEventArgs e)
        {
            LoadModelsName();
        }

        private void btAddModel_Click(object sender, RoutedEventArgs e)
        {
            NewModel form = new NewModel();
            form.ShowDialog();
            btReload_Click(null, null);
        }

        private void btSaveModel_Click(object sender, RoutedEventArgs e)
        {
            if(mModel != null)
            {
                string modelName = mModel.Name;
                mModel.SaveModel();
                MessageBox.Show(string.Format("Save {0} model successfully!", modelName), "SAVE MODEL", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            
        }

        private void btRemoveModel_Click(object sender, RoutedEventArgs e)
        {
            if (mModel != null)
            {
                string modelName = mModel.Name;
                string[] models = Directory.GetFiles("Models/");
                for (int i = 0; i < models.Length; i++)
                {
                    if (models[i].Contains(modelName))
                    {
                        var a = MessageBox.Show(string.Format("Are you want to detele {0} model ?", modelName), "DELETE MODEL", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                        if(a == MessageBoxResult.Yes)
                        {
                            File.Delete(models[i]);
                            MessageBox.Show(string.Format("Delete {0} model successfully!", modelName), "DELETE MODEL", MessageBoxButton.OK, MessageBoxImage.Information);
                            btReload_Click(null, null);
                        }
                        return;
                    }
                }
                MessageBox.Show(string.Format("Not found {0} model!", modelName), "DELETE MODELL", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btImportModel_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btExportModel_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btGerberSettings_Click(object sender, RoutedEventArgs e)
        {
            GerberTools gerberTools = new GerberTools(mModel);
            gerberTools.ShowDialog();
            mModel.UpdateAfterEditGerber();
            LoadDetails();

        }

        private void btFOVsSettings_Click(object sender, RoutedEventArgs e)
        {
            FOVCaptureWindow fovWD = new FOVCaptureWindow(mModel);
            fovWD.Show();
            //mModel.UpdateAfterEditGerber();
            LoadDetails();
        }

        private void btHardwareSettings_Click(object sender, RoutedEventArgs e)
        {
            HardwareWindow hwWD = new HardwareWindow(mModel);
            hwWD.Show();
            //mModel.UpdateAfterEditGerber();
            LoadDetails();
        }
        private void LoadDetails()
        {
            lbModelName.Content = mModel.Name;
            lbTimeCreate.Content = mModel.CreateTime.ToString("HH:mm:ss   dd/MM/yyyy");
            lbNumFOVs.Content = mModel.Gerber.FOVs.Count.ToString() + " FOVs";
            lbOwner.Content = mModel.Owner;
            lbGerberFile.Content = mModel.Gerber.FileName;
            btSaveModel.IsEnabled = true;
            btRemoveModel.IsEnabled = true;
            btExportModel.IsEnabled = true;
            btSaveModel.Opacity = 1;
            btRemoveModel.Opacity = 1;
            btExportModel.Opacity = 1;
        }
        private void ResetDetails()
        {
            lbModelName.Content = "-----";
            lbTimeCreate.Content = "-----";
            lbNumFOVs.Content = "-----";
            lbOwner.Content = "-----";
            lbGerberFile.Content = "-----";
            btSaveModel.IsEnabled = false;
            btRemoveModel.IsEnabled = false;
            btExportModel.IsEnabled = false;
            btSaveModel.Opacity = 0.5;
            btRemoveModel.Opacity = 0.5;
            btExportModel.Opacity = 0.5;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (mModel != null)
            {
                mModel.Dispose();
                mModel = null;
            }
        }

        private void btAutoAdjustFOV_Click(object sender, RoutedEventArgs e)
        {
            AutoAdjustFOVWindow adjustWindow = new AutoAdjustFOVWindow(mModel);
            adjustWindow.ShowDialog();
            LoadDetails();
        }
    }
}
