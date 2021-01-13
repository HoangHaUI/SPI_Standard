using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using NLog;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System.Threading;

namespace SPI_AOI.Models
{
    public class GerberFile
    {
        public static Logger mLog = Heal.LogCtl.GetInstance();
        public string ModelID { get; set; }
        public string GerberID { get; set; }
        public Color Color { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public byte[] FileData { get; set; }
        public Image<Gray, byte> OrgGerberImage { get; set; }
        public Image<Gray, byte> ProcessingGerberImage { get; set; }
        
        public bool Visible { get; set; }
        public double Angle { get; set; }
        public SPI_AOI.Utils.StartPoint StartPoint { get; set; }
        public Rectangle ROI { get; set; }
        public Rectangle SelectPad { get; set; }
        public Mark MarkPoint { get; set; }
        public List<Fov> FOVs { get; set; }
        public List<PadItem> PadItems { get; set; }
        public static GerberFile GetNewGerber(string ID, string Path, float DPI, Size FOV)
        {
            if(Path == null)
            {
                return null;
            }
            GerberFile gerber = new GerberFile();
            FileInfo fi = new FileInfo(Path);
            SPI_AOI.Utils.GerberRenderResult renderResults = SPI_AOI.Utils.GerberUtils.Render(fi.FullName, DPI, Color.White, Color.Black);
            if(renderResults.Status == SPI_AOI.Utils.ActionStatus.Fail)
            {
                return null;
            }
            gerber.ModelID = ID;
            gerber.GerberID = Utils.GetNewID();
            gerber.FileName = fi.Name;
            gerber.FilePath = fi.FullName;
            gerber.Color = Color.FromArgb(255, 0, 0);
            gerber.Visible = true;
            gerber.Angle = 0;
            gerber.OrgGerberImage = renderResults.GerberImage;
            gerber.ProcessingGerberImage = gerber.OrgGerberImage.Copy();
            gerber.FileData = File.ReadAllBytes(fi.FullName);
            gerber.ROI = new Rectangle();
            gerber.SelectPad = Rectangle.Empty;
            gerber.MarkPoint = new Mark(gerber.GerberID);
            gerber.ResetMark();
            gerber.StartPoint = SPI_AOI.Utils.StartPoint.TOP_LEFT;
            gerber.UpdatePadItems();
            gerber.UpdateFOV(FOV);
            gerber.LinkPadWidthFov(FOV);
            return gerber;
            
        }
        public void LoadGerber(float DPI, Size FOV)
        {
            FileInfo fi = new FileInfo(this.FilePath);
            if (!Directory.Exists("TempPath"))

            {
                Directory.CreateDirectory("TempPath");
            }
            File.WriteAllBytes("TempPath/" + this.FileName, this.FileData);
            SPI_AOI.Utils.GerberRenderResult renderResults = SPI_AOI.Utils.GerberUtils.Render("TempPath/" + this.FileName, DPI, Color.White, Color.Black);
            File.Delete("TempPath/" + this.FileName);
            if (renderResults.Status == SPI_AOI.Utils.ActionStatus.Fail)
            {
                return;
            }
            this.OrgGerberImage = renderResults.GerberImage;
            SetAngle(this.Angle, FOV, false);
        }
        public void SetROI(Rectangle ROI, Size FOV)
        {
            this.ROI = ROI;
            this.UpdatePadItems();
            this.ResetMark();
        }
        public void SetStartPoint(SPI_AOI.Utils.StartPoint StartPoint, Size FOV)
        {
            this.StartPoint = StartPoint;
            this.ResetMark();
        }
        public void SetAngle(double Angle, Size FOV, bool Reload = true)
        {
            this.Angle = Angle;
            if(this.ProcessingGerberImage != null)
            {
                this.ProcessingGerberImage.Dispose();
                this.ProcessingGerberImage = null;
            }
            this.ProcessingGerberImage = ImageProcessingUtils.ImageRotation(this.OrgGerberImage.Copy(), new Point(this.OrgGerberImage.Width / 2, this.OrgGerberImage.Height / 2), this.Angle * Math.PI / 180.0);
            if(Reload)
            {
                this.UpdatePadItems();
                this.ResetMark();
            }
            
        }
        public void ResetMark()
        {
            this.MarkPoint.PadMark[0] = -1;
            this.MarkPoint.PadMark[0] = -1;
        }
        public List<PadItem> GetPadSelected()
        {
            List<PadItem> padSelected = new List<PadItem>();
            if (this.SelectPad != Rectangle.Empty)
            {
                for (int i = 0; i < this.PadItems.Count; i++)
                {
                    Rectangle bound = new Rectangle(this.PadItems[i].Center.X, this.PadItems[i].Center.Y, 1, 1);
                    if (this.SelectPad.Contains(bound))
                    {
                        padSelected.Add(this.PadItems[i]);
                    }
                }
            }
            return padSelected;
        }
        public Image<Bgr, byte> GetDiagramImage()
        {
            this.ProcessingGerberImage.ROI = this.ROI;
            Image<Bgr, byte> Img = new Image<Bgr, byte>(this.ProcessingGerberImage.Size);
            using (Image<Gray, byte> imgTemp = this.ProcessingGerberImage.Copy())
            using (Image<Bgr, byte> imgAdd = new Image<Bgr, byte>(Img.Size.Width, Img.Size.Height, new Bgr(0, 50, 0)))
            {
                CvInvoke.BitwiseNot(imgTemp, imgTemp);
                CvInvoke.CvtColor(this.ProcessingGerberImage, Img, Emgu.CV.CvEnum.ColorConversion.Gray2Bgr);
                CvInvoke.Add(Img, imgAdd, Img, mask: imgTemp);
            }
            return Img;
        }
        public Point GetCenterPadsSelected()
        {
            if (this.SelectPad == Rectangle.Empty)
            {
                return new Point();
            }
            List<PadItem> padSelected = GetPadSelected();
            List<Point> centerEachPad = new List<Point>();
            for (int i = 0; i < padSelected.Count; i++)
            {
                centerEachPad.Add(padSelected[i].Center);
            }
            if (centerEachPad.Count > 0)
            {
                long x = 0;
                long y = 0;
                for (int i = 0; i < centerEachPad.Count; i++)
                {
                    x += centerEachPad[i].X;
                    y += centerEachPad[i].Y;
                }
                x = x / centerEachPad.Count;
                y = y / centerEachPad.Count;
                return new Point(Convert.ToInt32(x), Convert.ToInt32(y));
            }
            else
            {
                return new Point();
            }
        }
        public void UpdateFOV(Size FOV)
        {
            using (Image<Gray, byte> searchFOVImage = new Image<Gray, byte>(ProcessingGerberImage.Size))
            using (VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint())
            {
                for (int i = 0; i < this.PadItems.Count; i++)
                {
                    if(!this.MarkPoint.PadMark.Contains(i) && this.PadItems[i].Enable)
                    {
                        using (VectorOfPoint cnt = new VectorOfPoint(this.PadItems[i].Contour))
                        {
                            contours.Push(cnt);
                        }
                    }
                }
                CvInvoke.DrawContours(searchFOVImage, contours, -1, new MCvScalar(255), -1);
                this.FOVs = Fov.GetFov(this.GerberID, searchFOVImage, this.ROI, FOV, this.StartPoint);
            }
        }
        public Image<Bgr, byte> GetFOVDiagram(Size FOV, int IndexHightLight)
        {
            List<Point> anchors = new List<Point>();
            for (int i = 0; i < this.FOVs.Count; i++)
            {
                anchors.Add(this.FOVs[i].Anchor);
            }
            Image<Bgr, byte> img =  Fov.GetFOVDiagram(this.ProcessingGerberImage, anchors.ToArray(), FOV, IndexHightLight);
            img.ROI = this.ROI;
            return img;
        }
        public void UpdatePadItems()
        {
            if(this.PadItems != null)
            {
                for (int i = 0; i < this.PadItems.Count; i++)
                {
                    this.PadItems[i].Dispose();
                }
            }
            this.PadItems = PadItem.GetPads(this.GerberID, this.ProcessingGerberImage, this.ROI);
            
        }
        public void LinkPadWidthFov(Size FOV)
        {
            for (int i = 0; i < this.PadItems.Count; i++)
            {
                this.PadItems[i].FOVs.Clear();
                Rectangle padBound = PadItems[i].Bouding;
                for (int j = 0; j < this.FOVs.Count; j++)
                {
                    Rectangle fov = SPI_AOI.Utils.FOVOptimize.GetRectangleByAnchor(this.FOVs[j].Anchor, FOV);
                    if(fov.Contains(padBound))
                    {
                        this.PadItems[i].FOVs.Add(j);
                        this.FOVs[j].PadItems.Add(i);
                        break;
                    }
                }
            }
            List<PadItem> padsNotLink = new List<PadItem>();
            for (int i = 0; i < this.PadItems.Count; i++)
            {
                if(this.PadItems[i].FOVs.Count == 0 && this.PadItems[i].Enable && !this.MarkPoint.PadMark.Contains(i))
                    padsNotLink.Add(this.PadItems[i]);
            }
            Console.WriteLine("has {0} pad not link", padsNotLink.Count);
        }
        public void ClearLinkCadItem()
        {
            for (int i = 0; i < this.PadItems.Count ; i++)
            {
                this.PadItems[i].CadFileID = string.Empty;
                this.PadItems[i].CadItemIndex = -1;
            }
        }
        public void Dispose()
        {
            if (this.ProcessingGerberImage != null)
            {
                this.ProcessingGerberImage.Dispose();
                this.ProcessingGerberImage = null;
            }
            if (this.OrgGerberImage != null)
            {
                this.OrgGerberImage.Dispose();
                this.OrgGerberImage = null;
            }
            FOVs.Clear();
            PadItems.Clear();
        }
    }
}
