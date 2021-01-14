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
        public string GerberID { get; set; }
        public bool Enable { get; set; }
        public int NoID { get; set; }
        public double Area { get; set; }
        public Point[] Contour { get; set; }
        public Rectangle  Bouding { get; set; }
        public Point[] ContourAdjust { get; set; }
        public Rectangle BoudingAdjust { get; set; }
        public Point Center { get; set; }
        public StandardThreshold AreaThresh { get; set; }
        public StandardThreshold ShiftXThresh { get; set; }
        public StandardThreshold ShiftYThresh { get; set; }
        public int CadItemIndex { get; set; }
        public string CadFileID { get; set; }
        public List<int> FOVs { get; set; }
        public static List<PadItem> GetPads(string GerberID, Image<Gray, byte> ImgGerber, Rectangle ROI)
        {
            List<PadItem> padItems = new List<PadItem>();
            ImgGerber.ROI = ROI;
            using (VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint())
            {
                CvInvoke.FindContours(ImgGerber, contours, null, Emgu.CV.CvEnum.RetrType.External, Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxSimple);
                for (int i = 0; i < contours.Size; i++)
                {
                    Moments mm = CvInvoke.Moments(contours[i]);
                    if (mm.M00 == 0)
                        continue;
                    Point ctCnt = new Point(Convert.ToInt32(mm.M10 / mm.M00), Convert.ToInt32(mm.M01 / mm.M00));
                    Rectangle bound = CvInvoke.BoundingRectangle(contours[i]);
                    double area = CvInvoke.ContourArea(contours[i]);
                    PadItem pad = new PadItem();
                    pad.GerberID = GerberID;
                    bound.X += ROI.X;
                    bound.Y += ROI.Y;
                    ctCnt.X += ROI.X;
                    ctCnt.Y += ROI.Y;
                    pad.Center = ctCnt;
                    pad.Bouding = bound;
                    pad.BoudingAdjust = bound;
                    Point[] cntPoint = contours[i].ToArray();
                    for (int k = 0; k < cntPoint.Length; k++)
                    {
                        cntPoint[k].X += ROI.X;
                        cntPoint[k].Y += ROI.Y;
                    }
                    pad.Contour = cntPoint;
                    pad.ContourAdjust = new Point[cntPoint.Length];
                    for (int j = 0; j < cntPoint.Length; j++)
                    {
                        pad.ContourAdjust[j] = new Point(cntPoint[j].X, cntPoint[j].Y);
                    }
                    pad.Area = area;
                    pad.AreaThresh = new StandardThreshold(260, 60);
                    pad.ShiftXThresh = new StandardThreshold(370, 40);
                    pad.ShiftYThresh = new StandardThreshold(370, 40);
                    pad.FOVs = new List<int>();
                    pad.CadFileID = string.Empty;
                    pad.CadItemIndex = -1;
                    pad.NoID = i;
                    pad.Enable = true;
                    padItems.Add(pad);
                }
            }
            ImgGerber.ROI = Rectangle.Empty;
            return padItems;
        }
        public void Dispose()
        {
            this.FOVs.Clear();
        }
    }
    public class StandardThreshold
    {
        public double UM_USL { get; set; }
        public double PERCENT_LSL { get; set; }
        public StandardThreshold() { }
        public StandardThreshold(double UM_USL, double PERCENT_LSL)
        {
            this.UM_USL = UM_USL;
            this.PERCENT_LSL = PERCENT_LSL;
        }
    }
    
}
