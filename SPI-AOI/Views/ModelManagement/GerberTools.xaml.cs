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
using System.IO;
using Newtonsoft.Json;
using System.Diagnostics;


namespace SPI_AOI.Views.ModelManagement 
{
    /// <summary>
    /// Interaction logic for GerberTools.xaml
    /// </summary>
    public partial class GerberTools : Window
    {
        // variable user status
        bool mMousePress = false;
        bool mCtrlPress = false;
        //bool mIsDrawROI = false;
        //bool mIsDrawItems = false;
        System.Drawing.Rectangle mSelectRecangle = System.Drawing.Rectangle.Empty;
        System.Drawing.Point StartPoint = new System.Drawing.Point();
        public Model mModel = new Model();
        private List<object> mImportedFiles = new List<object>();
        private List<PadItem> mPads = new List<PadItem>();
        public GerberTools(Model model)
        {
            mModel = model;
            InitializeComponent();
        }
        private void Window_Initialized(object sender, EventArgs e)
        {
            listImportedFile.ItemsSource = mImportedFiles;
            dgwPads.ItemsSource = mPads;
            UpdateUIModel();
            UpdateListImportedFile();
            ShowAllLayerImb(ActionMode.Render, thisThread:true);
        }
        private void UpdateUIModel()
        {
            if (mModel != null)
            {
                chbShowLinkLine.IsChecked = mModel.ShowLinkLine;
                chbShowComponentCenter.IsChecked = mModel.ShowComponentCenter;
                chbShowComponentName.IsChecked = mModel.ShowComponentName;
                chbHighlightPadLinked.IsChecked = mModel.HightLineLinkedPad;
            }
        }
        private void UpdateListImportedFile()
        {
            mImportedFiles.Clear();
            mPads.Clear();
            if (mModel.Gerber is GerberFile)
            {
                mImportedFiles.Add(mModel.Gerber);
                for (int i = 0; i < mModel.Gerber.PadItems.Count; i++)
                {
                    if(mModel.Gerber.PadItems[i].Enable)
                    {
                        mPads.Add(mModel.Gerber.PadItems[i]);
                    }
                    
                }
            }
            for (int i = 0; i < mModel.Cad.Count; i++)
            {
                mImportedFiles.Add(mModel.Cad[i]);
            }
            this.Dispatcher.Invoke(() =>
            {
                listImportedFile.Items.Refresh();
                dgwPads.Items.Refresh();
            });

        }
        private void imBox_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            mMousePress = true;
            mSelectRecangle = System.Drawing.Rectangle.Empty;
            Cursor = Cursors.Cross;
            StartPoint.X = Convert.ToInt32(e.Location.X / imBox.ZoomScale + imBox.HorizontalScrollBar.Value);
            StartPoint.Y = Convert.ToInt32(e.Location.Y / imBox.ZoomScale + imBox.VerticalScrollBar.Value);
        }

        private void imBox_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            mMousePress = false;
            Cursor = Cursors.Arrow;
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
        private void DeletePads(System.Drawing.Rectangle Rect)
        {
            List<PadItem> pads = mModel.GetPadsInRect(Rect, Linked:true);
            foreach (var item in pads)
            {
                mModel.DeteleLinkPad(item);
            }
            ShowAllLayerImb(ActionMode.Update_Color_Gerber);
        }
        private void SetPadName(System.Drawing.Rectangle Rect)
        {
            List<Tuple<CadFile, int>> suggets = mModel.GetSuggestCadItemName(Rect);
            int id = -1;
            if (suggets.Count > 0 )
            {
                SuggestLinkPadWindow avaiWD = new SuggestLinkPadWindow(suggets);
                avaiWD.ShowDialog();
                id = avaiWD.ItemSelected;
            }
            if (id > -1)
            {
                
                List<PadItem> pads = mModel.GetPadsInRect(Rect, Linked:false);
                foreach (var item in pads)
                {
                    CadFile cadFile = suggets[id].Item1;
                    int idCadItem = suggets[id].Item2;
                    CadItem cadItem = cadFile.CadItems[idCadItem];
                    PadItem pad = item;
                    int idPadItem = mModel.Gerber.PadItems.IndexOf(pad);
                    pad.CadFileID = cadFile.CadFileID;
                    pad.CadItemIndex = idCadItem;
                    cadItem.PadsIndex.Add(idPadItem);
                }
                ShowAllLayerImb(ActionMode.Update_Color_Gerber);
            }
        }
        private void imBox_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (mMousePress)
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
                    int r = mModel.GetNewCad(ofd.FileName);
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
        public void ShowAllLayerImb(ActionMode mode, bool thisThread = false)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Image<Bgr, byte> imgLayers = ShowModel.GetLayoutImage(mModel, mode);
            if(!thisThread)
            {
                imBox.Invoke(new Action(() =>
                {
                    var x = imBox.Image;
                    imBox.Image = imgLayers;
                    if (x != null)
                    {
                        x.Dispose();
                        x = null;
                    }
                    imBox.Refresh();
                }));
            }
            else
            {
                var x = imBox.Image;
                imBox.Image = imgLayers;
                if (x != null)
                {
                    x.Dispose();
                    x = null;
                }
                imBox.Refresh();
            }  
            
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
                if(mSelectRecangle != System.Drawing.Rectangle.Empty)
                {
                    if(mSelectRecangle.Width + mSelectRecangle.X < mModel.Gerber.ProcessingGerberImage.Width &&
                       mSelectRecangle.Height + mSelectRecangle.Y < mModel.Gerber.ProcessingGerberImage.Height)
                    {
                        mModel.SetROI(mSelectRecangle);
                        mSelectRecangle = System.Drawing.Rectangle.Empty;
                        ShowAllLayerImb(ActionMode.Render);
                        UpdateListImportedFile();
                    }
                    else
                    {
                        MessageBox.Show(string.Format("ROI selected is incorrect!"), "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    
                }
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
                    if(mSelectRecangle != System.Drawing.Rectangle.Empty)
                    {
                        DrawItems(mSelectRecangle);
                        mSelectRecangle = System.Drawing.Rectangle.Empty;
                        imBox.Refresh();
                    }
                    else
                    {
                        MessageBox.Show(string.Format("Area select is emty!"), "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
            else
            {
                MessageBox.Show(string.Format("Please insert gerber file..."), "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void btRemovePad_Click(object sender, RoutedEventArgs e)
        {
            if (mModel.Gerber is GerberFile)
            {
                List<PadItem> padSelectedLink = mModel.GetPadsInRect(mSelectRecangle, true);
                List<PadItem> padSelectedNotLink = mModel.GetPadsInRect(mSelectRecangle);
                if(padSelectedLink.Count > 0 || padSelectedNotLink .Count > 0)
                {
                    var r = MessageBox.Show(string.Format("Are you want to delete pads ?"), 
                        "Question", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                    if(r == MessageBoxResult.Yes)
                    {
                        foreach (var item in padSelectedLink)
                        {
                            item.Enable = false;
                        }
                        foreach (var item in padSelectedNotLink)
                        {
                            item.Enable = false;
                        }
                        ShowAllLayerImb(ActionMode.Update_Color_Gerber);
                        mSelectRecangle = System.Drawing.Rectangle.Empty;
                        imBox.Refresh();
                        UpdateListImportedFile();
                    }
                }
            }
            else
            {
               
            }
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.LeftCtrl || e.Key == Key.Right)
            {
                mCtrlPress = true;
            }
            if (e.Key == Key.D && mCtrlPress)
            {
                btSetLinkPad_Click(null, null);
            }
            if (e.Key == Key.R && mCtrlPress)
            {
                btSetROI_Click(null, null);
            }
            if (e.Key == Key.B && mCtrlPress)
            {
                btSelectPad_Click(null, null);
            }
            if (e.Key == Key.Delete)
            {
                btDeleteLinkPad_Click(null, null);
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
                ShowAllLayerImb(ActionMode.Render);
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
                CadFile cad = ((CadFile)item).Copy();
                mModel.Cad.Add(cad);
                ShowAllLayerImb(ActionMode.Draw_Cad);
                UpdateListImportedFile();
            }
        }

        private void btLinkPad_Click(object sender, RoutedEventArgs e)
        {
            
            if (mModel.Gerber is GerberFile || mModel.Cad.Count > 0)
            {
                mModel.ClearLinkPad();
                foreach (var item in mModel.Cad)
                {
                    AutoLinkPadWindow autoLinkWD = new AutoLinkPadWindow();
                    autoLinkWD.ShowDialog();
                    if(autoLinkWD.ModeLinkPad != Utils.AutoLinkMode.NotLink)
                    {
                        int mode = 0;
                        if (autoLinkWD.ModeLinkPad == Utils.AutoLinkMode.RnC)
                        {
                            mode = 0;
                        }
                        if (autoLinkWD.ModeLinkPad == Utils.AutoLinkMode.TwoPad)
                        {
                            mode = 1;
                        }
                        if (autoLinkWD.ModeLinkPad == Utils.AutoLinkMode.All)
                        {
                            mode = 2;
                        }
                        Thread pr = new Thread(() => {
                            mModel.AutoLinkPad(item, mode);
                            ShowAllLayerImb(ActionMode.Update_Color_Gerber);
                        });
                        pr.Start();
                    }
                }
            }
            else
            {
                MessageBox.Show(string.Format("Please insert gerber file..."), "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void btSetLinkPad_Click(object sender, RoutedEventArgs e)
        {
            if (mModel.Gerber is GerberFile || mModel.Cad.Count > 0)
            {
                if (imBox.Image != null)
                {
                    if (mSelectRecangle != System.Drawing.Rectangle.Empty)
                    {
                        SetPadName(mSelectRecangle);
                        mSelectRecangle = System.Drawing.Rectangle.Empty;
                        imBox.Refresh();
                        UpdateListImportedFile();
                    }
                    else
                    {
                        MessageBox.Show(string.Format("Area select is emty!"), "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
            else
            {
                MessageBox.Show(string.Format("Please insert gerber file..."), "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void btDeleteLinkPad_Click(object sender, RoutedEventArgs e)
        {
            if (mModel.Gerber is GerberFile || mModel.Cad.Count > 0)
            {
                if (imBox.Image != null)
                {
                    if (mSelectRecangle != System.Drawing.Rectangle.Empty)
                    {
                        DeletePads(mSelectRecangle);
                        mSelectRecangle = System.Drawing.Rectangle.Empty;
                        imBox.Refresh();
                        UpdateListImportedFile();
                    }
                    else
                    {
                        MessageBox.Show(string.Format("Area select is emty!"), "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
            else
            {
                MessageBox.Show(string.Format("Please insert gerber file..."), "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl || e.Key == Key.Right)
            {
                mCtrlPress = false;
            }
        }

        private void chbHighlightPadLinked_Click(object sender, RoutedEventArgs e)
        {
            if (mModel != null)
            {
                mModel.HightLineLinkedPad = (sender as MenuItem).IsChecked;
                ShowAllLayerImb(ActionMode.Render);
            }
        }

        private void btPadSettings_Click(object sender, RoutedEventArgs e)
        {
            if (mModel.Gerber is GerberFile || mModel.Cad.Count > 0)
            {
                List<PadItem> count = mModel.GetPadsInRect(new System.Drawing.Rectangle(0, 0, mModel.Gerber.ProcessingGerberImage.Width, mModel.Gerber.ProcessingGerberImage.Height), Linked: false);
                
                if(count.Count == 0)
                {
                    List<PadItem> listSelectPad = mModel.GetPadsInRect(mSelectRecangle, Linked: true);
                    PadconditionWindow padConditionWD = new PadconditionWindow(mModel.Gerber.PadItems, listSelectPad);
                    padConditionWD.ShowDialog();
                }
                else
                {
                    MessageBox.Show(string.Format("Please link all pad before settings!..."), "Information", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btMark1_Click(object sender, RoutedEventArgs e)
        {
            if (mModel.Gerber is GerberFile || mModel.Cad.Count > 0)
            {
                List<PadItem> count = mModel.GetPadsInRect(new System.Drawing.Rectangle(0, 0, mModel.Gerber.ProcessingGerberImage.Width, mModel.Gerber.ProcessingGerberImage.Height), Linked: false);

                if (count.Count == 0)
                {
                    List<PadItem> listSelectPad = mModel.GetPadsInRect(mSelectRecangle, Linked: true);
                    if(listSelectPad.Count > 1)
                    {
                        MessageBox.Show(string.Format("You are select more than 1 item!..."), "Information", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else if(listSelectPad.Count < 1)
                    {
                        MessageBox.Show(string.Format("Not found mark point!..."), "Information", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else if (listSelectPad[0].NoID == mModel.Gerber.MarkPoint.PadMark[1])
                    {
                        MessageBox.Show(string.Format("This is G2 Mark point!..."), "Information", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        mModel.Gerber.MarkPoint.PadMark[0] = listSelectPad[0].NoID;
                        mSelectRecangle = System.Drawing.Rectangle.Empty;
                        ShowAllLayerImb(ActionMode.Draw_Cad);
                    }
                    
                }
                else
                {
                    MessageBox.Show(string.Format("Please link all pad before settings!..."), "Information", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btMark2_Click(object sender, RoutedEventArgs e)
        {
            if (mModel.Gerber is GerberFile || mModel.Cad.Count > 0)
            {
                List<PadItem> count = mModel.GetPadsInRect(new System.Drawing.Rectangle(0, 0, mModel.Gerber.ProcessingGerberImage.Width, mModel.Gerber.ProcessingGerberImage.Height), Linked: false);

                if (count.Count == 0)
                {
                    List<PadItem> listSelectPad = mModel.GetPadsInRect(mSelectRecangle, Linked: true);
                    if (listSelectPad.Count > 1)
                    {
                        MessageBox.Show(string.Format("You are select more than 1 item!..."), "Information", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else if (listSelectPad.Count < 1)
                    {
                        MessageBox.Show(string.Format("Not found mark point!..."), "Information", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else if(listSelectPad[0].NoID == mModel.Gerber.MarkPoint.PadMark[0])
                    {
                        MessageBox.Show(string.Format("This is G1 Mark point!..."), "Information", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        mModel.Gerber.MarkPoint.PadMark[1] = listSelectPad[0].NoID;
                        mSelectRecangle = System.Drawing.Rectangle.Empty;
                        ShowAllLayerImb(ActionMode.Draw_Cad);
                    }
                }
                else
                {
                    MessageBox.Show(string.Format("Please link all pad before settings!..."), "Information", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if(imBox.Image != null)
            {
                imBox.Image.Dispose();
                imBox.Image = null;
            }
            if(mModel != null)
            {
                if(mModel.ImgGerberProcessedBgr != null)
                {
                    mModel.ImgGerberProcessedBgr.Dispose();
                    mModel.ImgGerberProcessedBgr = null;
                    
                }
            }
        }

        private void btSaveGerber_Click(object sender, RoutedEventArgs e)
        {
            using (System.Windows.Forms.SaveFileDialog sdf = new System.Windows.Forms.SaveFileDialog())
            {
                sdf.DefaultExt = "png";
                sdf.Filter = "Png Image|*.png|JPeg Image|*.jpg|Bitmap Image|*.bmp|Gif Image|*.gif";
                if (sdf.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    try
                    {
                        CvInvoke.Imwrite(sdf.FileName, mModel.Gerber.ProcessingGerberImage);
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }

        }
    }
}
