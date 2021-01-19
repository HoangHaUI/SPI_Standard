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
using SPI_AOI.DB.Struct;
using System.IO;


namespace SPI_AOI.Views.Histories
{
    /// <summary>
    /// Interaction logic for HistoryWindow.xaml
    /// </summary>
    public partial class HistoryWindow : Window
    {
        List<DB.Struct.ResultsObject> mListResult = new List<DB.Struct.ResultsObject>();
        List<PadErrorObject> mListPadError = new List<PadErrorObject>();
        List<ImageSavedObject> mListImageSaved = new List<ImageSavedObject>();
        DB.Result mMyDatabase = new DB.Result();
        DB.Query mDBQuery = new DB.Query();
        bool mIsFinding = false;
        public HistoryWindow()
        {
            InitializeComponent();
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            string[] modelNames = mMyDatabase.GetModelName();
            for (int i = 0; i < modelNames.Length; i++)
            {
                cbModelName.Items.Add(modelNames[i]);
            }
            dgvPanelResult.ItemsSource = mListResult;
            dgvPanelResult.Items.Refresh();
            dgvDefects.ItemsSource = mListPadError;
            dgvDefects.Items.Refresh();
        }

        private void cbFind_Click(object sender, RoutedEventArgs e)
        {
            mIsFinding = true;
            string modelName = "*";
            string sn = "*";
            DateTime startTime = new DateTime(2021, 1, 1);
            DateTime endTime = DateTime.Now;
            if(cbModelName.SelectedIndex > -1)
            {
                modelName = cbModelName.SelectedItem.ToString();
            }
            if(!string.IsNullOrEmpty(txtSN.Text ))
            {
                sn = txtSN.Text;
            }
            if(dateStartTime.SelectedDate != null)
            {
                startTime = dateStartTime.SelectedDate.Value;
            }
            if (dateEndTime.SelectedDate != null)
            {
                endTime = dateEndTime.SelectedDate.Value;
            }
            var result = mDBQuery.GetResult(modelName, sn, startTime, endTime);
            mListResult.Clear();
            mListResult.AddRange(result);
            dgvPanelResult.Items.Refresh();
            mIsFinding = false;
        }

        private void dgvPanelResult_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (mIsFinding)
                return;
            
            DataGrid dgv = sender as DataGrid;
            int i = dgv.SelectedIndex;
            if(i < mListResult .Count)
            {
                mIsFinding = true;
                string ID = mListResult[i].ID;
                mListImageSaved.Clear();
                mListImageSaved.AddRange(mDBQuery.GetImageSavedDetails(ID));
                mListPadError.Clear();
                mListPadError.AddRange(mDBQuery.GetPadErrorDetails(ID));
                dgvDefects.Items.Refresh();
                mIsFinding = false;
            }
        }

        private void dgvDefects_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (mIsFinding)
                return;

            DataGrid dgv = sender as DataGrid;
            int id = dgv.SelectedIndex;

            if (id < mListPadError.Count)
            {
                var item = mListPadError[id];
                lbErrorType.Content = item.Type;
                lbPadErrorID.Content = item.PadID;
                lbPadErrorComponent.Content = item.Component;
                lbPadErrorArea.Content = string.Format("{0} | {1} ~ {2}", Math.Round(item.AreaMeasure, 3), Math.Round(item.AreaLow, 3), Math.Round(item.AreaHight, 3));
                lbPadErrorShiftX.Content = string.Format("{0} | {1} ~ {2}", Math.Round(item.ShiftXMeasure, 3), 0, Math.Round(item.ShiftXHight, 3));
                lbPadErrorShiftY.Content = string.Format("{0} | {1} ~ {2}", Math.Round(item.ShiftYMeasure, 3), 0, Math.Round(item.ShiftYHight, 3));
                int fov = item.FovID;
                string imagePath = "";
                for (int i = 0; i < mListImageSaved.Count; i++)
                {
                   if( mListImageSaved[i].FovID == fov)
                    {
                        imagePath = mListImageSaved[i].ImagePath;
                        break;
                    }
                }
                if(File.Exists(imagePath))
                {
                    using (Image<Bgr, byte> image = new Image<Bgr, byte>(imagePath))
                    {
                        var loc = item.ROIOnFov;
                        loc.Inflate(10, 10);
                        
                        CvInvoke.Rectangle(image, loc, new MCvScalar(0, 255, 255), 2);
                        BitmapSource bms = Utils.Convertor.Bitmap2BitmapSource(image.Bitmap);
                        imageFOV.Source = bms;
                    } 
                }
            }
        }
    }
}
