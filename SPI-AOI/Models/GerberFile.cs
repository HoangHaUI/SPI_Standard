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
        public string ID { get; set; }//
        public Color Color { get; set; }//
        public string FileName { get; set; }//
        public string FilePath { get; set; }//
        public byte[] FileData { get; set; }//
        public Image<Gray, byte> OrgGerberImage { get; set; }
        public Image<Gray, byte> ProcessingGerberImage { get; set; }
        public bool Visible { get; set; }//
        public double Angle { get; set; }//
        public SPI_AOI.Utils.StartPoint StartPoint { get; set; }//
        public Rectangle ROI { get; set; }//
        public Rectangle SelectPad { get; set; }//
        public List<Rectangle> RemoveROI { get; set; }
        public List<Mark> MarkPoint { get; set; }
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
            gerber.ID = ID;
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
            gerber.RemoveROI = new List<Rectangle>();
            gerber.MarkPoint = new List<Mark>();
            gerber.StartPoint = SPI_AOI.Utils.StartPoint.TOP_LEFT;
            gerber.UpdatePadItems();
            gerber.UpdateFOV(FOV);
            gerber.LinkPadWidthFov(FOV);
            return gerber;
            
        }
        public void SetROI(Rectangle ROI, Size FOV)
        {
            this.ROI = ROI;
            this.UpdatePadItems();
            
            //this.UpdateFOV(FOV);
            //this.LinkPadWidthFov(FOV);
        }
        public void SetStartPoint(SPI_AOI.Utils.StartPoint StartPoint, Size FOV)
        {
            this.StartPoint = StartPoint;
            //this.UpdateFOV(FOV);
        }
        public void SetAngle(double Angle, Size FOV)
        {
            this.Angle = Angle;
            if(this.ProcessingGerberImage != null)
            {
                this.ProcessingGerberImage.Dispose();
                this.ProcessingGerberImage = null;
            }
            this.ProcessingGerberImage = ImageProcessingUtils.ImageRotation(this.OrgGerberImage.Copy(), new Point(this.OrgGerberImage.Width / 2, this.OrgGerberImage.Height / 2), this.Angle * Math.PI / 180.0);
            this.UpdatePadItems();
            //this.UpdateFOV(FOV);
            //this.LinkPadWidthFov(FOV);
        }
        public Point GetCenterPadsSelected()
        {
            if(this.SelectPad == Rectangle.Empty)
            {
                return new Point();
            }
            List<Point> centerEachPad = new List<Point>();
            for (int i = 0; i < this.PadItems.Count; i++)
            {
                Rectangle bound = this.PadItems[i].Bouding;// new Rectangle(this.PadItems[i].Center.X, this.PadItems[i].Center.Y, 1,1);
                if (this.SelectPad.Contains(bound))
                {
                    centerEachPad.Add(new Point(bound.X + bound.Width / 2, bound.Y + bound.Height / 2));
                }
            }
            if(centerEachPad .Count > 0)
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
            this.FOVs = Fov.GetFov(this.ID, this.ProcessingGerberImage, this.ROI, FOV, this.StartPoint);
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
            this.PadItems = PadItem.GetPads(this.ID, this.ProcessingGerberImage, this.ROI);
            
        }
        public void LinkPadWidthFov(Size FOV)
        {
            for (int i = 0; i < this.PadItems.Count; i++)
            {
                Rectangle padBound = new Rectangle(this.PadItems[i].Center.X, this.PadItems[i].Center.Y, 1, 1);
                for (int j = 0; j < this.FOVs.Count; j++)
                {
                    Rectangle fov = SPI_AOI.Utils.FOVOptimize.GetRectangleByAnchor(this.FOVs[j].Anchor, FOV);
                    if(fov.Contains(padBound))
                    {
                        this.PadItems[i].FOVs.Add(this.FOVs[j]);
                        break;
                    }
                }
            }
        }
        public void ClearLinkCadItem()
        {
            for (int i = 0; i < this.PadItems.Count ; i++)
            {
                this.PadItems[i].CadItem = null;
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
            RemoveROI.Clear();
            MarkPoint.Clear();
            FOVs.Clear();
            PadItems.Clear();
        }
    }
}
