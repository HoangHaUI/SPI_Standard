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
    /// Interaction logic for NewModel.xaml
    /// </summary>
    public partial class NewModel : Window
    {
        Model mModel;
        Properties.Settings mParam = Properties.Settings.Default;
        public NewModel()
        {
            InitializeComponent();
        }
        private void Window_Initialized(object sender, EventArgs e)
        {
            EnableBtAdd();
        }
        private void btBrowser_Click(object sender, RoutedEventArgs e)
        {
            using (System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog())
            {
                ofd.Title = "[AUTO STENCIL INSPECTION] Select gerber file";
                ofd.Filter = "Gerber file | *.gbr;*.gbx";
                if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    txtGerberPath.Text = ofd.FileName;
                }
            }
        }

        private void txtModelName_TextChanged(object sender, TextChangedEventArgs e)
        {
            EnableBtAdd();
        }

        private void txtGerberPath_TextChanged(object sender, TextChangedEventArgs e)
        {
            EnableBtAdd();
        }

        private void btAdd_Click(object sender, RoutedEventArgs e)
        {
            string modelName = txtModelName.Text;
            string gerberPath = txtGerberPath.Text;
            float dpi = mParam.DPI;
            System.Drawing.Size fov = mParam.FOV;
            mModel = Model.GetNewModel(modelName, "Admin", gerberPath, dpi, fov);
            if (mModel == null)
            {
                MessageBox.Show("Gerber file incorrect!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else
            {
                mModel.SaveModel("Models/" + modelName + ".json");
                mModel.Dispose();
                this.Close();
            }
        }

        private void btCancel_Click(object sender, RoutedEventArgs e)
        {
            mModel.Dispose();
            Close();
        }

        
        private void EnableBtAdd()
        {
            btAdd.IsEnabled = txtModelName.Text != null && txtModelName.Text != string.Empty && txtModelName.Text != "" &&
                txtGerberPath.Text != null && txtGerberPath.Text != string.Empty && txtGerberPath.Text != "[Import File]";
        }
    }
}
