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
                    pad.Contours = contours[i].ToArray();
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
                Rectangle boundPadRef = padItem.Bouding;
                double sPadRef = CvInvoke.ContourArea(padItem.Contour);
                List<int> idPadSegOverlap = new List<int>();
                for (int j = 0; j < padSegment.Length; j++)
                {
                    
                    if(boundPadRef.IntersectsWith(padSegment[j].Bouding))
                    {
                        idPadSegOverlap.Add(j);
                        break;
                    }
                }
                PadErrorDetail padEr = new PadErrorDetail();
                double scaleArea = 0;
                double shiftx = 0;
                double shifty = 0;
                padEr.AreaStdHight = padItem.AreaThresh.UM_USL;
                padEr.AreaStdLow = padItem.AreaThresh.PERCENT_LSL;
                padEr.ShiftXStduM = padItem.ShiftXThresh.UM_USL;
                padEr.ShiftYStduM = padItem.ShiftYThresh.UM_USL;
                padEr.ShiftXStdArea = padItem.ShiftXThresh.PERCENT_LSL;
                padEr.ShiftYStdArea = padItem.ShiftYThresh.PERCENT_LSL;
                padEr.ROI = Rectangle.Inflate(boundPadRef, 10, 10);
                padEr.Pad = padItem;
                
                if(idPadSegOverlap.Count > 0)
                {
                    double areaAllPadSeg = 0;
                    Rectangle boundAllPadSeg = new Rectangle();
                    for (int j = 0; j < idPadSegOverlap.Count; j++)
                    {
                        PadSegmentInfo padSeg = padSegment[idPadSegOverlap[j]];
                        areaAllPadSeg += padSeg.Area;
                        if(j == 0)
                        {
                            boundAllPadSeg = padSeg.Bouding;
                            continue;
                        }
                        if (padSeg.Bouding.X < boundAllPadSeg.X)
                            boundAllPadSeg.X = padSeg.Bouding.X;
                        if (padSeg.Bouding.Y < boundAllPadSeg.Y)
                            boundAllPadSeg.Y = padSeg.Bouding.Y;
                        if (padSeg.Bouding.X + padSeg.Bouding.Width > boundAllPadSeg.X + boundAllPadSeg.Width)
                            boundAllPadSeg.Width = padSeg.Bouding.X + padSeg.Bouding.Width - boundAllPadSeg.X;
                        if (padSeg.Bouding.Y + padSeg.Bouding.Height > boundAllPadSeg.Y + boundAllPadSeg.Height)
                            boundAllPadSeg.Height = padSeg.Bouding.Y + padSeg.Bouding.Height - boundAllPadSeg.Y;
                    }
                    padEr.Center = new Point( boundAllPadSeg.X + boundAllPadSeg.Width/2, boundAllPadSeg.Y + boundAllPadSeg.Height/ 2);
                    scaleArea = areaAllPadSeg * 100 / sPadRef;

                    shiftx = (Math.Max(Math.Abs(boundPadRef.X - boundAllPadSeg.X), Math.Abs((boundPadRef.X + boundPadRef.Width) - (boundAllPadSeg.X + boundAllPadSeg.Width))) * umPPixel) % 130;
                    shifty = (Math.Max(Math.Abs(boundPadRef.Y - boundAllPadSeg.Y), Math.Abs((boundPadRef.Y + boundPadRef.Height) - (boundAllPadSeg.Y + boundAllPadSeg.Height))) * umPPixel) % 130;
                    
                    bool insert = false;
                    double deviation = (100 - (sPadRef / umPPixel)) / 2;
                    deviation = deviation < 0 ? 0 : deviation;
                    if (scaleArea > padItem.AreaThresh.UM_USL + deviation || scaleArea < padItem.AreaThresh.PERCENT_LSL - deviation  )
                    {
                        if(sPadRef > 100 || (sPadRef < 100 && scaleArea < 5))
                        {
                            insert = true;
                        }
                    }
                    if (shiftx > padItem.ShiftXThresh.UM_USL)
                    {
                        insert = true;
                    }
                    if (shifty > padItem.ShiftXThresh.UM_USL)
                    {
                        insert = true;
                    }
                    if (insert)
                    {
                        
                        padEr.Area = scaleArea;
                        padEr.ShiftX = shiftx;
                        padEr.ShiftY = shifty;
                        padError.Add(padEr);
                    }
                    else
                    {
                        // pad pass
                        continue;
                    }
                }
                else
                {
                    // not found solder paste
                    padError.Add(padEr);
                }
                
            }
            return padError.ToArray();
        }
        public static PadErrorDetail[] GetImagePadError(Image<Bgr, byte> image, PadErrorDetail[] PadError, Rectangle ROI, int limit)
        {
            int lm = PadError.Length > limit ? limit : PadError.Length;
            for (int i = 0; i < lm; i++)
            {
                Rectangle bound = PadError[i].ROI;
                bound.X -= ROI.X;
                bound.Y -= ROI.Y;
                bound.X = bound.X < 0 ? 0 : bound.X;
                bound.Y = bound.Y < 0 ? 0 : bound.Y;
                image.ROI = bound;
                PadError[i].PadImage = image.Copy();
                image.ROI = Rectangle.Empty;
            }
            return PadError;
        }
    }
   
}
