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
    public class PadItem
    {
        public string ID { get; set; }
        public Rectangle Bouding { get; set; }
        public Thresh Insufficient { get; set; }
        public Thresh Excess { get; set; }
        public Thresh Position { get; set; }
        public Thresh Bridge { get; set; }
        public CadItem CadItem { get; set; }
        public List<Fov> FOVs { get; set; }
        public static List<PadItem> GetPads(string ID, Image<Gray, byte> ImgGerber, Rectangle ROI)
        {
            List<PadItem> padItems = new List<PadItem>();
            ImgGerber.ROI = ROI;
            using (VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint())
            {
                CvInvoke.FindContours(ImgGerber, contours, null, Emgu.CV.CvEnum.RetrType.External, Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxSimple);
                for (int i = 0; i < contours.Size; i++)
                {
                    
                    PadItem pad = new PadItem();
                    pad.ID = ID;
                    Rectangle bound = CvInvoke.BoundingRectangle(contours[i]);
                    bound.X += ROI.X;
                    bound.Y += ROI.Y;
                    pad.Bouding = bound;
                    pad.Bridge = new Thresh(5,10);
                    pad.Excess = new Thresh(5, 10);
                    pad.Insufficient = new Thresh(5, 10);
                    pad.Position = new Thresh(5, 10);
                    pad.FOVs = new List<Fov>();
                    padItems.Add(pad);
                }
            }
            ImgGerber.ROI = Rectangle.Empty;
            return padItems;
        }
    }
    public class Thresh
    {
        public double Warning { get; set; }
        public double Error { get; set; }
        public Thresh() { }
        public Thresh(double Warning, double Error)
        {
            this.Warning = Warning;
            this.Error = Error;
        }
    }
    
}
