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
    /// Interaction logic for GerberTools.xaml
    /// </summary>
    public partial class GerberTools : Window
    {
        Theme mTheme = new Theme();
        public GerberTools()
        {
            InitializeComponent();
            List<CadFile> m = new List<CadFile>();
            CadFile cad = new CadFile();
            cad.Color = System.Drawing.Color.Red;
            m.Add(cad);
            listImportedFile.ItemsSource = m;

        }

        private void imBox_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {

        }

        private void imBox_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {

        }

        private void imBox_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {

        }

        private void imBox_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {

        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Forms.ColorDialog cld = new System.Windows.Forms.ColorDialog();
            if(cld.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                (sender as Border).Background = new SolidColorBrush(Color.FromRgb(cld.Color.R, cld.Color.G, cld.Color.B));
            }
            
        }
    }
}
