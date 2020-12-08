using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Collections;
using System.Windows.Controls;
using SPI_AOI.Models;

namespace SPI_AOI.Views.ModelManagement
{
    /// <summary>
    /// Interaction logic for XYMoving.xaml
    /// </summary>
    public partial class XYMovingWindow : Window
    {
        GerberTools mGerberToolsForm = null;
        List<CadFile> mListCadFiles = new List<CadFile>();
        public XYMovingWindow(GerberTools GerberToolsForm )
        {
            mGerberToolsForm = GerberToolsForm;
            InitializeComponent();
        }
        private void Window_Initialized(object sender, EventArgs e)
        {
            cbLayer.ItemsSource = mListCadFiles;
            UpdateListImportedFile();
        }
        private void cbLayer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        private void UpdateListImportedFile()
        {
            mListCadFiles.Clear();
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
    }
}
