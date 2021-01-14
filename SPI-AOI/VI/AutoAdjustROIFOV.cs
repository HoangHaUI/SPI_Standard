using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System.IO;
using System.Drawing;


namespace SPI_AOI.VI
{
    class AutoAdjustROIFOV
    {
        public static List<Utils.PadAdjustResult> AdjustPad(Image<Bgr, byte> ImgCap,List<Rectangle> BoundPad, Hsv Upper, Hsv Lower, Rectangle CapROI)
        {
            ImgCap.ROI = CapROI;
            List<Utils.PadAdjustResult> adjustResult = new List<Utils.PadAdjustResult>();
            using (Image<Hsv, byte> ImgHsv = new Image<Hsv, byte>(ImgCap.Size))
            {
                CvInvoke.CvtColor(ImgCap, ImgHsv, Emgu.CV.CvEnum.ColorConversion.Bgr2Hsv);
                using (Image<Gray, byte> imgSegment = ImgHsv.InRange(Upper, Lower))
                using (VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint())
                {
                    CvInvoke.FindContours(imgSegment, contours, null, Emgu.CV.CvEnum.RetrType.External, Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxSimple);
                    Rectangle[] listBoundPadSeg = new Rectangle[contours.Size];
                    for (int i = 0; i < contours.Size; i++)
                    {
                        listBoundPadSeg[i] = CvInvoke.BoundingRectangle(contours[i]);
                    }
                    for (int i = 0; i < BoundPad.Count; i++)
                    {
                        Rectangle boundPadRef = BoundPad[i];
                        Rectangle boundPadSeg = Rectangle.Empty;
                        Utils.PadAdjustResult padAdjustResult = new Utils.PadAdjustResult();

                        foreach (var item in listBoundPadSeg)
                        {
                            if (BoundPad[i].IntersectsWith(item))
                            {
                                if (boundPadSeg == Rectangle.Empty)
                                {
                                    boundPadSeg = item;
                                }
                                else
                                {
                                    if (boundPadSeg.X > item.X)
                                        boundPadSeg.X = item.X;
                                    if (boundPadSeg.Y > item.Y)
                                        boundPadSeg.Y = item.Y;
                                    if (boundPadSeg.X + boundPadSeg.Width < item.X + item.Width)
                                    {
                                        boundPadSeg.Width = item.X + item.Width - boundPadSeg.X;
                                    }
                                    if (boundPadSeg.Y + boundPadSeg.Height < item.Y + item.Height)
                                    {
                                        boundPadSeg.Height = item.Y + item.Height - boundPadSeg.Y;
                                    }
                                }
                            }
                        }
                        if(boundPadSeg != Rectangle.Empty)
                        {
                            Point ctSeg = new Point(boundPadSeg.Width / 2 + boundPadSeg.X, boundPadSeg.Height / 2 + boundPadSeg.Y);
                            Point ctRef = new Point(boundPadRef.Width / 2 + boundPadRef.X, boundPadRef.Height / 2 + boundPadRef.Y);
                            int subx = ctRef.X - ctSeg.X;
                            int suby = ctRef.Y - ctSeg.Y;
                            if (Math.Abs(subx) <= 5)
                                padAdjustResult.X = -subx;
                            if(Math.Abs(suby) <= 5)
                            {
                                padAdjustResult.Y = -suby;
                            }
                        }
                        adjustResult.Add(padAdjustResult);
                    }
                }
            }
            ImgCap.ROI = Rectangle.Empty;
            return adjustResult;
        }
        public static Rectangle Adjust(Image<Bgr, byte> ImgCap, Image<Gray, byte> ImgGerber, Hsv Upper, Hsv Lower, Size FOV, Rectangle CapROI)
        {
            Rectangle ROIAdjust = new Rectangle(CapROI.X, CapROI.Y, CapROI.Width, CapROI.Height);
            using (Image<Hsv, byte> ImgHsv = new Image<Hsv, byte>(ImgCap.Size))
            {
                CvInvoke.CvtColor(ImgCap, ImgHsv, Emgu.CV.CvEnum.ColorConversion.Bgr2Hsv);
                using (Image<Gray, byte> imgSegment = ImgHsv.InRange(Upper, Lower))
                using(VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint())
                {
                    CvInvoke.FindContours(ImgGerber, contours, null, Emgu.CV.CvEnum.RetrType.External, Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxSimple);
                    int[] idCheck = GetListContoursAdjust(contours, FOV, 10, 3);
                    int[] xad = new int[idCheck.Length];
                    int[] yad = new int[idCheck.Length];
                    int extend = 20;
                    for (int i = 0; i < idCheck.Length; i++)
                    {
                        Rectangle ROI = CvInvoke.BoundingRectangle(contours[idCheck[i]]);
                        ROI.X -= extend;
                        ROI.Y -= extend;
                        ROI.Width += 2 * extend;
                        ROI.Height += 2 * extend;
                        ImgGerber.ROI = ROI;
                        imgSegment.ROI = ROI;
                        int[] adjust = GetAdjust(imgSegment, ImgGerber, 50, 10);
                        xad[i] = adjust[0];
                        yad[i] = adjust[1];
                        ImgGerber.ROI = Rectangle.Empty;
                        imgSegment.ROI = Rectangle.Empty;
                    }
                    int x = Convert.ToInt32(Sum(xad) / idCheck.Length);
                    int y = Convert.ToInt32(Sum(yad) / idCheck.Length);
                    ROIAdjust.X -= x;
                    ROIAdjust.Y -= y;
                }
            }
            return ROIAdjust;
        }
        private static int[] GetListContoursAdjust(VectorOfVectorOfPoint contours, Size FOV, int NumGet = 3, double MaxArea = 3)
        {
            Tuple<double, int>[] cntInfo = new Tuple<double, int>[contours.Size];
            for (int i = 0; i < contours.Size; i++)
            {
                double s = CvInvoke.ContourArea(contours[i]);
                cntInfo[i] = new Tuple<double, int>(s, i);
            }
            Tuple<double, int>[] sorted = cntInfo.OrderBy(item => item.Item1).ToArray();
            List<int> id = new List<int>();
            for (int i = sorted.Length - 1; i >=0; i--)
            {
                if(sorted[i].Item1 * 100 / (FOV.Width * FOV.Height) < MaxArea)
                {
                    if (id.Count <= NumGet)
                    {
                        id.Add(sorted[i].Item2);
                    }
                    else
                        break;

                }
            }
            return id.ToArray();
        }
        private static double Sum (int [] values)
        {
            double sum = 0;
            for (int i = 0; i < values.Length; i++)
            {
                sum += values[i];
            }
            return sum;
        }
        private static int[] GetAdjust(Image<Gray, byte> ImgCap, Image<Gray, byte> ImgGerber, int RangeX, int RangeY)
        {
            int x_ok = 0;
            int y_ok = 0;
            double diff = 2448 * 2018;
            for (int x = -RangeX; x < RangeX; x++)
            {
                for (int y = -RangeY; y < RangeY; y++)
                {
                    using (Image<Gray, byte> imgTransform = ImageProcessingUtils.ImageTransformation(ImgCap.Copy(), x, y))
                    {
                        CvInvoke.AbsDiff(imgTransform, ImgGerber, imgTransform);
                        
                        int count = CvInvoke.CountNonZero(imgTransform);
                        if(count < diff)
                        {
                            diff = count;
                            x_ok = x;
                            y_ok = y;
                        }
                    }
                }
            }
            return new int[] { x_ok, y_ok };
        }
    }
}
