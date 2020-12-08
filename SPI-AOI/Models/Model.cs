using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Util;

namespace SPI_AOI.Models
{
    public class Model
    {
        public string ID { get; set; }
        public string Name { get;set; }
        public string Owner { get; set; }
        public DateTime CreateTime { get; set; }
        public float DPI { get; set; }
        public bool ClearOutROI { get; set; }
        public Size FOV { get; set; }
        public GerberFile Gerber { get; set; }
        public List<CadFile> Cad { get; set; }
        public Image<Gray, byte> ImgGerberProcessed { get; set; }
        public Image<Bgr, byte> ImgGerberProcessedBgr { get; set; }
        public List<PadItem> SelectPads { get; set; }
        public bool ShowLinkLine { get; set; }
        public bool ShowComponentCenter { get; set; }
        public bool ShowComponentName { get; set; }
        public static Model GetNewModel(string ModelName, string Owner, string GerberPath, float DPI, Size FOV)
        {
            Model model = new Model();
            model.ID = Guid.NewGuid().ToString().ToUpper();
            model.Name = ModelName;
            model.Owner = Owner;
            
            model.CreateTime = DateTime.Now;
            model.ShowLinkLine = true;
            model.ShowComponentCenter = true;
            model.ShowComponentName = true;
            model.DPI = DPI;
            model.FOV = FOV;
            model.GetNewGerber(GerberPath);
            model.Cad = new List<CadFile>();
            return model;
        }
        public int GetNewGerber(string Path)
        {
            GerberFile gerber = GerberFile.GetNewGerber(this.ID, Path, this.DPI, this.FOV);
            this.Gerber = gerber;
            if(gerber == null)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }
        public int GerNewCad(string Path)
        {
            int gerberW = 0;
            int gerberH = 0;
            if(this.Gerber is GerberFile)
            {
                gerberW = this.Gerber.OrgGerberImage.Width;
                gerberH = this.Gerber.OrgGerberImage.Height;
            }
            CadFile cad = CadFile.GetNewCadFile(this.ID, Path, this.DPI, gerberW, gerberH);
            if (cad == null)
            {
                return -1;
            }
            else
            {
                this.Cad.Add(cad);
                return 0;
            }
            
        }
        public void RemoveCadByName(string Name)
        {
            for (int i = 0; i < this.Cad.Count; i++)
            {
                if(this.Cad[i].FileName == Name)
                {
                    this.Cad.Remove(this.Cad[i]);
                }
            }
        }
        public void RemoveCadByID(int Index)
        {
            if(Index < this.Cad.Count)
            {
                this.Cad.RemoveAt(Index);
            }
        }
        public void RotateGerber(double Angle)
        {
            double angle = this.Gerber.Angle;
            angle = (angle + Angle) % 360;
            this.Gerber.SetAngle(angle, this.FOV);
        }
        public void SetROI(Rectangle ROI)
        {
            this.Gerber.SetROI(ROI, this.FOV);
        }
        public List<object> GetListLayerInRect(System.Drawing.Rectangle Rect)
        {
            List<object> listObj = new List<object>();
            if (Rect != Rectangle.Empty)
            {
                //gerber layer
                using (VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint())
                {
                    CvInvoke.FindContours(this.Gerber.ProcessingGerberImage, contours, null, Emgu.CV.CvEnum.RetrType.External, Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxSimple);
                    for (int i = 0; i < contours.Size; i++)
                    {
                        Rectangle bound = CvInvoke.BoundingRectangle(contours[i]);
                        if (Rect.Contains(bound))
                        {
                            listObj.Add(this.Gerber);
                            break;
                        }
                    }
                }
                // cad layer
                foreach (var item in this.Cad)
                {
                    for (int i = 0; i < item.CadItems.Count; i++)
                    {
                        Point ct = Point.Round(item.CadItems[i].Center);
                        ct.X += item.X;
                        ct.Y += item.Y;
                        Point newCtRotate = ImageProcessingUtils.PointRotation(ct, new Point((int)item.CenterRotation.X + item.X, (int)item.CenterRotation.Y + item.Y), item.Angle * Math.PI / 180.0);
                        Rectangle bound = new Rectangle(newCtRotate.X, newCtRotate.Y, 1, 1);
                        if (Rect.Contains(bound))
                        {
                            listObj.Add(item);
                            break;
                        }
                    }
                }
            }
            return listObj;
        }
    }
}
