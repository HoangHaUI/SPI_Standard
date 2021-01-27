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
    /// Interaction logic for SaveAsWindow.xaml
    /// </summary>
    public partial class SaveAsWindow : Window
    {
        public string ModelName { get; set; }
        public SaveAsWindow(string modelName)
        {
            InitializeComponent();
            string[] listModel = Model.GetModelNames();
            int count = 1;
            string newModelName = modelName;
            while (true)
            {
                newModelName = modelName + "_" + count.ToString();
                if(!listModel.Contains(newModelName))
                {
                    break;
                }
                else
                {
                    count++;
                }
            }
            txtModelName.Text = newModelName;
        }

        private void btSave_Click(object sender, RoutedEventArgs e)
        {
            string[] listModel = Model.GetModelNames();
            string modelSaveAs = txtModelName.Text;
            if(!string.IsNullOrEmpty(modelSaveAs))
            {
                if (listModel.Contains(modelSaveAs))
                {
                    MessageBox.Show("Model " + modelSaveAs + " is existed!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                else
                {
                    ModelName = modelSaveAs;
                    this.Close();
                }
            }
            else
            {
                MessageBox.Show("Model name cant empty!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        }
    }
}
