using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System.Drawing;
using SPI_AOI.Utils;

namespace SPI_AOI.VI
{
    class Predictor
    {
        public static Image<Gray, byte> ReleaseNoise(Image<Gray, byte> image)
        {
            using (Mat k = CvInvoke.GetStructuringElement(Emgu.CV.CvEnum.ElementShape.Rectangle, new System.Drawing.Size(3, 3), new System.Drawing.Point(-1, -1)))
            {
                CvInvoke.MorphologyEx(image, image, Emgu.CV.CvEnum.MorphOp.Close, k, new System.Drawing.Point(-1, -1), 1, Emgu.CV.CvEnum.BorderType.Default, new MCvScalar());
                CvInvoke.MorphologyEx(image, image, Emgu.CV.CvEnum.MorphOp.Open, k, new System.Drawing.Point(-1, -1), 1, Emgu.CV.CvEnum.BorderType.Default, new MCvScalar());
            }
            return image;
        }
        public static PadSegmentInfo[] GetPadSegmentInfo(Image<Gray, byte> image, Rectangle ROI)
        {
            List<PadSegmentInfo> padinfo = new List<PadSegmentInfo>();
            using (VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint())
            {
                CvInvoke.FindContours(image, contours, null, Emgu.CV.CvEnum.RetrType.External, Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxSimple);
                for (int i = 0; i < contours.Size; i++)
                {
                    Moments mm = CvInvoke.Moments(contours[i]);
                    if (mm.M00 == 0)
                        continue;
                    Point ctCnt = new Point(Convert.ToInt32(mm.M10 / mm.M00), Convert.ToInt32(mm.M01 / mm.M00));
                    ctCnt.X += ROI.X;
                    ctCnt.Y += ROI.Y;
                    Rectangle bound = CvInvoke.BoundingRectangle(contours[i]);
                    bound.X += ROI.X;
                    bound.Y += ROI.Y;
                    double s = CvInvoke.ContourArea(contours[i]);
                    PadSegmentInfo pad = new PadSegmentInfo();
                    pad.Bouding = bound;
                    pad.Area = s;
                    pad.Center = ctCnt;
                    padinfo.Add(pad);
                }
            }
            return padinfo.ToArray();
        }
        public static PadErrorDetail[] ComparePad(Models.Model model, PadSegmentInfo[] padSegment)
        {
            List<PadErrorDetail> padError = new List<PadErrorDetail>();
            double umPPixel = 25.4 / model.DPI * 1000;
            for (int i = 0; i < model.Gerber.PadItems.Count; i++)
            {
                Models.PadItem padItem = model.Gerber.PadItems[i];
                if (!padItem.Enable || model.Gerber.MarkPoint.PadMark.Contains(i))
                {
                    continue;
                }
                double sOverLapMax = 0;
                int id = -1;
                for (int j = 0; j < padSegment.Length; j++)
                {
                    if (padItem.Bouding.IntersectsWith(padSegment[j].Bouding))
                    {
                        if (sOverLapMax < padSegment[j].Area)
                        {
                            sOverLapMax = padSegment[j].Area;
                            id = j;
                        }
                    }
                }
                if(id >= 0)
                {
                    double sPad = CvInvoke.ContourArea(padItem.Contour);
                    double scaleArea = padSegment[id].Area * 100 / sPad;

                    Rectangle b1 = padItem.Bouding;
                    Rectangle b2 = padSegment[id].Bouding;

                    double shiftx = Math.Min(Math.Abs(b1.X - b2.X), Math.Abs((b1.X + b1.Width) - (b2.X + b2.Width)));
                    double shifty = Math.Min(Math.Abs(b1.Y - b2.Y), Math.Abs((b1.Y + b1.Height) - (b2.Y + b2.Height)));
                    bool insert = false;
                    if(scaleArea > padItem.AreaThresh.UM_USL ||scaleArea < padItem.AreaThresh.PERCENT_LSL)
                    {
                        insert = true;
                    }
                    if(shiftx * umPPixel > padItem.ShiftXThresh.UM_USL)
                    {
                        insert = true;
                    }
                    if(shifty * umPPixel > padItem.ShiftXThresh.UM_USL)
                    {
                        insert = true;
                    }
                    if (insert)
                    {
                        PadErrorDetail padEr = new PadErrorDetail();
                        padEr.Area = scaleArea;
                        padEr.ShiftX = Math.Round(shiftx * umPPixel, 2);
                        padEr.ShiftY = Math.Round(shifty * umPPixel, 2);
                        padEr.Pad = padItem;
                        if(padItem.FOVs.Count > 0)
                        {
                            padEr.FOVNo = padItem.FOVs[0];
                        }
                        padEr.ROI = Rectangle.Inflate(b1, 10, 10);
                        // add std
                        padEr.AreaStdHight = padItem.AreaThresh.UM_USL;
                        padEr.AreaStdLow = padItem.AreaThresh.PERCENT_LSL;
                        padEr.ShiftXStduM = padItem.ShiftXThresh.UM_USL;
                        padEr.ShiftYStduM = padItem.ShiftYThresh.UM_USL;
                        padEr.ShiftXStdArea = padItem.ShiftXThresh.PERCENT_LSL;
                        padEr.ShiftYStdArea = padItem.ShiftYThresh.PERCENT_LSL;
                        padError.Add(padEr);
                    }
                }
            }
            return padError.ToArray();
        }
        public static PadErrorDetail[] GetImagePadError(Image<Bgr, byte> image, PadErrorDetail[] PadError, Rectangle ROI)
        {
            for (int i = 0; i < PadError.Length; i++)
            {
                Rectangle bound = PadError[i].ROI;
                bound.X -= ROI.X;
                bound.Y -= ROI.Y;
                bound.X = bound.X < 0 ? 0 : bound.X;
                bound.Y = bound.Y < 0 ? 0 : bound.Y;
                image.ROI = bound;
                PadError[i].PadImage = image.Copy();
                image.ROI = Rectangle.Empty;
                CvInvoke.Rectangle(image, bound, new MCvScalar(0, 255, 255), 3);
            }
            CvInvoke.Imwrite("out.png", image);
            return PadError;
        }
    }
   
}
