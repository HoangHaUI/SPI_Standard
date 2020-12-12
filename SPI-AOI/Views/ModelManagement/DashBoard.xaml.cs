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
        string ModelPath = "Models";
        string[] mListPathModel = new string[0];
        public DashBoard()
        {
            InitializeComponent();
        }
        private void Window_Initialized(object sender, EventArgs e)
        {
            if(!Directory.Exists(ModelPath))
            {
                Directory.CreateDirectory(ModelPath);
            }
            ResetDetails();
            btReload_Click(null, null);
            cbModelsName_SelectionChanged(null, null);
        }
        private void LoadModelsName()
        {
            string selected = Convert.ToString(cbModelsName.SelectedItem);
            cbModelsName.Items.Clear();
            mListPathModel = Directory.GetFiles(ModelPath, "*.json");
            for (int i = 0; i < mListPathModel.Length; i++)
            {
                FileInfo fi = new FileInfo(mListPathModel[i]);
                
                cbModelsName.Items.Add(fi.Name.Replace(".json", ""));
            }
            if (mListPathModel.Contains(selected))
            {
                cbModelsName.SelectedItem = selected;
            }
        }
        private void cbModelsName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbModelsName.SelectedIndex > -1)
            {
                string modelName = cbModelsName.SelectedItem.ToString();
                int id = cbModelsName.SelectedIndex;
                if (mModel != null)
                {
                    mModel.Dispose();
                    mModel = null;
                }
                mModel = Model.LoadModel(mListPathModel[id]);
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
            string modelName = mModel.Name;
            mModel.SaveModel("Models/" + modelName + ".json");
            MessageBox.Show(string.Format("Save {0} model successfully!", modelName), "SAVE MODEL", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btRemoveModel_Click(object sender, RoutedEventArgs e)
        {

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
    }
}
