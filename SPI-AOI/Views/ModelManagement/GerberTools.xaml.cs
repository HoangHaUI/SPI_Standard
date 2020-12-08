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
using Emgu.CV;
using Emgu.CV.Structure;
using System.ComponentModel;
using System.Threading;

namespace SPI_AOI.Views.ModelManagement 
{
    /// <summary>
    /// Interaction logic for GerberTools.xaml
    /// </summary>
    public partial class GerberTools : Window
    {
        // variable user status
        bool mMousePress = false;
        bool mIsDrawing = false;
        bool mIsDrawROI = false;
        bool mIsDrawItems = false;
        System.Drawing.Rectangle mSelectRecangle = System.Drawing.Rectangle.Empty;
        System.Drawing.Point StartPoint = new System.Drawing.Point();
        public Model mModel = new Model();
        private List<object> mImportedFiles = new List<object>();
        public GerberTools()
        {
            InitializeComponent();

        }
        private void Window_Initialized(object sender, EventArgs e)
        {
            listImportedFile.ItemsSource = mImportedFiles;
            mModel = Model.GetNewModel("HAHA", "thieu", null, 500, new System.Drawing.Size(600, 400));
            UpdateUIModel();
        }
        private void UpdateUIModel()
        {
            if (mModel != null)
            {
                chbShowLinkLine.IsChecked = mModel.ShowLinkLine;
                chbShowComponentCenter.IsChecked = mModel.ShowComponentCenter;
                chbShowComponentName.IsChecked = mModel.ShowComponentName;
            }
        }
        private void UpdateListImportedFile()
        {
            mImportedFiles.Clear();
            if (mModel.Gerber is GerberFile)
            {

                mImportedFiles.Add(mModel.Gerber);
            }
            for (int i = 0; i < mModel.Cad.Count; i++)
            {
                mImportedFiles.Add(mModel.Cad[i]);
            }
            this.Dispatcher.Invoke(() =>
            {
                listImportedFile.Items.Refresh();
            });
        }
        private void imBox_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (mIsDrawing)
            {
                mMousePress = true;
                mSelectRecangle = System.Drawing.Rectangle.Empty;
                StartPoint.X = Convert.ToInt32(e.Location.X / imBox.ZoomScale + imBox.HorizontalScrollBar.Value);
                StartPoint.Y = Convert.ToInt32(e.Location.Y / imBox.ZoomScale + imBox.VerticalScrollBar.Value);
            }
        }

        private void imBox_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            mMousePress = false;
            mIsDrawing = false;
            if (mSelectRecangle != System.Drawing.Rectangle.Empty)
            {
                if (mIsDrawROI)
                {
                    mModel.SetROI(mSelectRecangle);
                    ShowAllLayerImb(ActionMode.Render);
                    mIsDrawROI = false;
                }
                if(mIsDrawItems)
                {
                    DrawItems(mSelectRecangle);
                    mIsDrawItems = false;
                }
            }
            Cursor = Cursors.Arrow;
            mSelectRecangle = System.Drawing.Rectangle.Empty;
            imBox.Refresh();
        }
        private void DrawItems(System.Drawing.Rectangle Rect)
        {
            List<object> availabilityLayers = mModel.GetListLayerInRect(Rect);
            int id = -1;
            if (availabilityLayers.Count != 1)
            {
                AvailabilityLayerWindow avaiWD = new AvailabilityLayerWindow(availabilityLayers);
                avaiWD.ShowDialog();
                id = avaiWD.ItemSelected;
            }
            else
            {
                id = 0;
            }
            if(id> -1)
            {
                object itemsSelected = availabilityLayers[id];
                if (itemsSelected is GerberFile)
                {
                    ((GerberFile)itemsSelected).SelectPad = mSelectRecangle;
                }
                else if (itemsSelected is CadFile)
                {
                    ((CadFile)itemsSelected).SelectCenter = mSelectRecangle;
                }
                ShowAllLayerImb(ActionMode.Select_Pad);
            }
        }
        private void imBox_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (mMousePress && mIsDrawing)
            {
                System.Drawing.Point endPoint = new System.Drawing.Point();

                endPoint.X = Convert.ToInt32(e.Location.X / imBox.ZoomScale + imBox.HorizontalScrollBar.Value);
                endPoint.Y = Convert.ToInt32(e.Location.Y / imBox.ZoomScale + imBox.VerticalScrollBar.Value);
                int x = Math.Min(StartPoint.X, endPoint.X);
                int y = Math.Min(StartPoint.Y, endPoint.Y);
                int w = Math.Abs(StartPoint.X - endPoint.X);
                int h = Math.Abs(StartPoint.Y - endPoint.Y);
                mSelectRecangle = new System.Drawing.Rectangle(x, y, w, h);
                imBox.Refresh();
            }
        }

        private void imBox_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            if (mSelectRecangle != System.Drawing.Rectangle.Empty)
            {
                e.Graphics.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.Orange, 2), mSelectRecangle);
            }
        }
        private void imBox_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (imBox.Image != null)
            {
                if (e.Delta > 0)
                {
                    imBox.SetZoomScale(1.2 * imBox.ZoomScale, e.Location);
                }
                else
                {
                    imBox.SetZoomScale(0.8 * imBox.ZoomScale, e.Location);
                }
            }

        }
        private void btImportCad_Click(object sender, RoutedEventArgs e)
        {
            if (mModel.Gerber is GerberFile)
            {
                System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
                if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    int r = mModel.GerNewCad(ofd.FileName);
                    if(r == 0)
                    {
                        UpdateListImportedFile();
                        ShowAllLayerImb(ActionMode.Draw_Cad);
                    }
                    else
                    {
                        MessageBox.Show("Input string was not in a correct format!", "ERROR", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                
            }
            else
            {
                MessageBox.Show("Please select gerber file before select cad files!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }

        }
        private void btImportGerber_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.Filter = "Gerber file (*.gbr;*.gbx) | *.gbr;*.gbx; | All files (*.*)|*.*";
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                WaitingForm wait = new WaitingForm("Rendering...");
                Thread st = new Thread(() =>
                {
                    mModel.GetNewGerber(ofd.FileName);
                    UpdateListImportedFile();
                    ShowAllLayerImb( ActionMode.Render);
                    wait.KillMe = true;
                });
                st.Start();
                wait.ShowDialog();
            }
        }
        public void ShowAllLayerImb(ActionMode mode)
        {
            imBox.Invoke(new Action(() =>
            {
                var x = imBox.Image;
                Image<Bgr, byte> imgLayers = ShowModel.GetLayoutImage(mModel, mode);
                imBox.Image = imgLayers;
                if (x != null)
                {
                    x.Dispose();
                    x = null;
                }
            }));
        }
        private void Border_MouseUp(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Forms.ColorDialog cld = new System.Windows.Forms.ColorDialog();
            if (cld.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                object selectItem = listImportedFile.SelectedItem;
                if (selectItem is GerberFile)
                {
                    ((GerberFile)selectItem).Color = cld.Color;

                    ShowAllLayerImb( ActionMode.Render);
                }
                else if (selectItem is CadFile)
                {
                    ((CadFile)selectItem).Color = cld.Color;

                    ShowAllLayerImb( ActionMode.Draw_Cad);
                }
                UpdateListImportedFile();
            }
        }
        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            object selectItem = listImportedFile.SelectedItem;
            if (selectItem is GerberFile)
            {
                ((GerberFile)selectItem).Visible = (sender as CheckBox).IsChecked.Value;
            }
            else if (selectItem is CadFile)
            {
                ((CadFile)selectItem).Visible = (sender as CheckBox).IsChecked.Value;
            }
            UpdateListImportedFile();
            ShowAllLayerImb( ActionMode.Draw_Cad);
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox_Unchecked(sender, e);
        }

        
        private void btSetROI_Click(object sender, RoutedEventArgs e)
        {
            if (imBox.Image != null)
            {
                mIsDrawing = true;
                mIsDrawROI = true;
                Cursor = Cursors.Cross;
            }
        }

        private void btAdjustXY_Click(object sender, RoutedEventArgs e)
        {
            if (mModel.Gerber is GerberFile || mModel.Cad.Count > 0)
            {
                List<GerberFile> listGerbers = new List<GerberFile>();
                List<CadFile> listCads = new List<CadFile>();
                listGerbers.Add(mModel.Gerber);
                foreach (CadFile item in mModel.Cad)
                {
                    listCads.Add(item);
                }
                AutoAdjustWindow adjustWD = new AutoAdjustWindow(listGerbers, listCads, this);
                adjustWD.ShowDialog();
            }
            else
            {
                MessageBox.Show(string.Format("Please insert gerber file..."), "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void menuCtxDelete_Click(object sender, RoutedEventArgs e)
        {
            object item = listImportedFile.SelectedItem;
            if(item != null)
            {
                string name = string.Empty;
                if (item is CadFile)
                {
                    name = ((CadFile)item).FileName;
                }
                else if (item is GerberFile)
                {
                    name = ((GerberFile)item).FileName;
                }
                var r = MessageBox.Show(string.Format("Are you want to delete {0} file?", name), "Question", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                if(r == MessageBoxResult.Yes)
                {
                    if (item is CadFile)
                    {
                        CadFile cad = ((CadFile)item);
                        mModel.Cad.Remove(cad);
                        ShowAllLayerImb( ActionMode.Draw_Cad);
                    }
                    else if (item is GerberFile)
                    {
                        GerberFile gerber = ((GerberFile)item);
                        mModel.Gerber.Dispose();
                        mModel.Gerber = null;
                        ShowAllLayerImb(ActionMode.Render);
                    }
                    UpdateListImportedFile();
                }
            }
        }

        private void btSelectPad_Click(object sender, RoutedEventArgs e)
        {
            if (mModel.Gerber is GerberFile || mModel.Cad.Count > 0)
            {
                if (imBox.Image != null)
                {
                    mIsDrawing = true;
                    mIsDrawItems = true;
                    Cursor = Cursors.Cross;
                }
            }
            else
            {
                MessageBox.Show(string.Format("Please insert gerber file..."), "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btSelectCenter_Click(object sender, RoutedEventArgs e)
        {
            if (imBox.Image != null)
            {
                mIsDrawing = true;
                mIsDrawItems = true;
                Cursor = Cursors.Cross;
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Escape)
            {
                mIsDrawing = false;
                mIsDrawROI = false;
                mIsDrawItems = false;
                Cursor = Cursors.Arrow;
            }
        }
        private void btRotation_Click(object sender, RoutedEventArgs e)
        {
            if(mModel.Gerber is GerberFile || mModel.Cad.Count > 0)
            {
                RotationWindow rotateWindow = new RotationWindow(this);
                rotateWindow.ShowDialog();
            }
        }
        private void chbShowLinkLine_Click(object sender, RoutedEventArgs e)
        {
            if (mModel != null)
            {
                mModel.ShowLinkLine = (sender as MenuItem).IsChecked;
                ShowAllLayerImb(ActionMode.Draw_Cad);
            }
        }

        private void chbShowComponentCenter_Click(object sender, RoutedEventArgs e)
        {
            if (mModel != null)
            {
                mModel.ShowComponentCenter = (sender as MenuItem).IsChecked;
                ShowAllLayerImb(ActionMode.Draw_Cad);
            }
        }

        private void chbShowComponentName_Click(object sender, RoutedEventArgs e)
        {
            if (mModel != null)
            {
                mModel.ShowComponentName = (sender as MenuItem).IsChecked;
                ShowAllLayerImb(ActionMode.Draw_Cad);
            }
        }

        private void menuCtxCopy_Click(object sender, RoutedEventArgs e)
        {
            object item = listImportedFile.SelectedItem;
            if (item != null)
            {
                if (item is GerberFile)
                {
                    MessageBox.Show(string.Format("Only support Cad file!"), "Question", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                CadFile cad = ((CadFile)item).Clone();
                mModel.Cad.Add(cad);
                ShowAllLayerImb(ActionMode.Draw_Cad);
                UpdateListImportedFile();
            }
        }
    }
}
