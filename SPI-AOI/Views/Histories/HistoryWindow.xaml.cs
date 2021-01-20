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
using System.Runtime.InteropServices;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using SPI_AOI.DB.Struct;
using System.IO;
using Excel = Microsoft.Office.Interop.Excel;


namespace SPI_AOI.Views.Histories
{
    /// <summary>
    /// Interaction logic for HistoryWindow.xaml
    /// </summary>
    public partial class HistoryWindow : System.Windows.Window
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
                   if( mListImageSaved[i].FovID == fov + 1)
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

        private void btExportExcel_Click(object sender, RoutedEventArgs e)
        {
            if(mListResult.Count > 0)
            {
                System.Windows.Forms.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog();
                sfd.DefaultExt = "xls";
                sfd.Filter = "Excel file | *.xls";
                sfd.FileName = "SPI_" + DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss") + ".xls";
                if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();

                    if (xlApp == null)
                    {
                        MessageBox.Show("Excel is not properly installed!!");
                        return;
                    }
                    Microsoft.Office.Interop.Excel.Workbook xlWorkBook;
                    Microsoft.Office.Interop.Excel.Worksheet xlWorkSheet;
                    object misValue = System.Reflection.Missing.Value;
                    xlWorkBook = xlApp.Workbooks.Add(misValue);
                    xlWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
                    
                    xlWorkSheet.Cells[1, 1] = "Model Name";
                    xlWorkSheet.Cells[1, 2] = "SN";
                    xlWorkSheet.Cells[1, 3] = "Load Time";
                    xlWorkSheet.Cells[1, 4] = "Machine VI Result";
                    xlWorkSheet.Cells[1, 5] = "Confirm VI Result";
                    xlWorkSheet.Cells[1, 6] = "Component Fail";
                    xlWorkSheet.Cells[1, 7] = "Pad ID Fail";
                    xlWorkSheet.Cells[1, 8] = "Fail Type";
                    int row = 2;
                    for (int ii = 0; ii < mListResult.Count; ii++)
                    {
                        var result = mListResult[ii];
                        if (result.SN.Contains("NOT FOUND"))
                            continue;
                        string ID = result.ID;
                        var listError = mDBQuery.GetPadErrorDetails(ID);
                        xlWorkSheet.Cells[row, 1] = result.ModelName;
                        xlWorkSheet.Cells[row, 2] = result.SN;
                        xlWorkSheet.Cells[row, 3] = result.LoadTime.ToString("yyyy-MM-dd HH:mm:ss");
                        xlWorkSheet.Cells[row, 4] = result.MachineResult;
                        xlWorkSheet.Cells[row, 5] = result.ConfirmResult;
                        if (listError.Length > 0)
                        {
                            string componentFail = "";
                            string padIDFail = "";
                            string failType = "";
                            for (int i = 0; i < listError.Length; i++)
                            {
                                componentFail += listError[i].Component + ", ";
                                padIDFail += listError[i].PadID + ", ";
                                failType += listError[i].Type + ", ";
                            }
                            xlWorkSheet.Cells[row, 6] = componentFail;
                            xlWorkSheet.Cells[row, 7] = padIDFail;
                            xlWorkSheet.Cells[row, 8] = failType;
                        }
                        row++;
                    }
                    xlWorkBook.SaveAs(sfd.FileName, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                    xlWorkBook.Close(true, misValue, misValue);
                    xlApp.Quit();
                    Marshal.ReleaseComObject(xlWorkSheet);
                    Marshal.ReleaseComObject(xlWorkBook);
                    Marshal.ReleaseComObject(xlApp);
                    MessageBox.Show("Excel file created , you can find the file " + sfd.FileName, "INFO", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                
            }
            else
            {
                MessageBox.Show("Content is empty!", "INFO", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
