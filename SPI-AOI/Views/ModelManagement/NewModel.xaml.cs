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
            string user = mParam.USER_LOGIN;
            string modelName = txtModelName.Text;
            string gerberPath = txtGerberPath.Text;
            double pcbThickness = 0;
            try
            {
                pcbThickness = Convert.ToDouble(txtThickness.Text);
            }
            catch
            {
                MessageBox.Show("PCB Thickness incorrect!", 
                    "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            double dev = mParam.THICKNESS_DEFAULT - pcbThickness ;
            double pulseXPerPixel = dev * mParam.SCALE_PULSE_X + mParam.PULSE_X_PER_PIXEL_DEFAULT;
            double pulseYPerPixel = dev * mParam.SCALE_PULSE_Y + mParam.PULSE_Y_PER_PIXEL_DEFAULT;
            double angleXAxisCamera = mParam.CAMERA_X_AXIS_ANGLE;
            double angleXYAxis = mParam.XY_AXIS_ANGLE;
            float dpi = (float)(mParam.DPI_DEFAULT + (mParam.DPI_SCALE * -dev));
            System.Drawing.Size fov = mParam.FOV;
            mModel = Model.GetNewModel(modelName, user, gerberPath, dpi, fov, pulseXPerPixel, pulseYPerPixel, angleXAxisCamera, angleXYAxis, pcbThickness);
            if (mModel == null)
            {
                MessageBox.Show("Gerber file incorrect!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else
            {
                mModel.SaveModel();
                mModel.Dispose();
                MessageBox.Show("Add model successfully!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
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
            btAdd.IsEnabled =   !string.IsNullOrEmpty(txtModelName.Text) &&
                                !string.IsNullOrEmpty(txtGerberPath.Text) &&
                                !string.IsNullOrEmpty(txtThickness.Text);
        }
    }
}
