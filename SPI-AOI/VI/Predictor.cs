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
                //CvInvoke.MorphologyEx(image, image, Emgu.CV.CvEnum.MorphOp.Close, k, new System.Drawing.Point(-1, -1), 1, Emgu.CV.CvEnum.BorderType.Default, new MCvScalar());
                CvInvoke.MorphologyEx(image, image, Emgu.CV.CvEnum.MorphOp.Open, k, new System.Drawing.Point(-1, -1), 1, Emgu.CV.CvEnum.BorderType.Default, new MCvScalar());
            }
            return image;
        }
        public static PadSegmentInfo[] GetPadSegmentInfo(Image<Gray, byte> image, Rectangle ROI, int FovID, string ImageCapturePath, string ImageSegmentPath)
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
                    
                    Rectangle bound = CvInvoke.BoundingRectangle(contours[i]);
                    
                    double s = ImageProcessingUtils.ContourArea(contours[i]);
                    PadSegmentInfo pad = new PadSegmentInfo();
                    pad.FOVID = FovID;
                    pad.ImageCapturePath = ImageCapturePath;
                    pad.ImageSegmentPath = ImageSegmentPath;
                    pad.CenterOnFOV = new Point(ctCnt.X, ctCnt.Y);
                    pad.ContoursOnFOV = contours[i].ToArray();
                    pad.Contours = new Point[pad.ContoursOnFOV.Length];
                    for (int j = 0; j < pad.ContoursOnFOV.Length; j++)
                    {
                        pad.Contours[j] = new Point(pad.ContoursOnFOV[j].X + ROI.X, pad.ContoursOnFOV[j].Y + ROI.Y);
                    }
                    bound.X += ROI.X;
                    bound.Y += ROI.Y;
                    ctCnt.X += ROI.X;
                    ctCnt.Y += ROI.Y;
                    pad.Bouding = bound;
                    pad.Center = ctCnt;
                    pad.Area = s;
                    padinfo.Add(pad);
                }
            }
            return padinfo.ToArray();
        }
        public static PadErrorDetail[] ComparePad(Models.Model model, PadSegmentInfo[] padSegment, int FovID)
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
                if(padItem.FOVs.Count > 0)
                {
                    if(padItem.FOVs[0] == FovID)
                    {
                        PadErrorDetail padEr = CheckPad(padItem, padSegment, umPPixel, false);
                        if (padEr == null)
                            continue;
                        else
                        {
                            padError.Add(padEr);
                        }
                    }
                }
            }
            return padError.ToArray();
        }
        private static bool IntersectsContour(Point[] Cnt1, Point[] Cnt2, Rectangle BoudingCnt1)
        {
            bool intersect = false;
            bool canIntersect = false;
            Point[] cntPTranform1 = new Point[Cnt1.Length];
            Point[] cntPTranform2 = new Point[Cnt2.Length];
            for (int i = 0; i < Cnt1.Length; i++)
            {
                cntPTranform1[i] = new Point(Cnt1[i].X - BoudingCnt1.X, Cnt1[i].Y - BoudingCnt1.Y);
            }
            for (int i = 0; i < Cnt2.Length; i++)
            {
                cntPTranform2[i] = new Point(Cnt2[i].X - BoudingCnt1.X, Cnt2[i].Y - BoudingCnt1.Y);
                if ((cntPTranform2[i].X > 0 && cntPTranform2[i].X < BoudingCnt1.Width) ||
                    (cntPTranform2[i].Y > 0 && cntPTranform2[i].Y < BoudingCnt1.Height))
                    canIntersect = true;
            }
            if(canIntersect)
            {
                using (VectorOfPoint cnt1 = new VectorOfPoint(cntPTranform1))
                using (VectorOfPoint cnt2 = new VectorOfPoint(cntPTranform2))
                using (VectorOfVectorOfPoint contour1 = new VectorOfVectorOfPoint())
                using (VectorOfVectorOfPoint contour2 = new VectorOfVectorOfPoint())
                using (Image<Gray, byte> image1 = new Image<Gray, byte>(BoudingCnt1.Size))
                using (Image<Gray, byte> image2 = new Image<Gray, byte>(BoudingCnt1.Size))
                using (Image<Gray, byte> imageAnd = new Image<Gray, byte>(BoudingCnt1.Size))
                {
                    contour1.Push(cnt1);
                    contour2.Push(cnt2);
                    CvInvoke.DrawContours(image1, contour1, -1, new MCvScalar(255), -1);
                    CvInvoke.DrawContours(image2, contour2, -1, new MCvScalar(255), -1);
                    CvInvoke.BitwiseAnd(image1, image2, imageAnd);
                    int count = CvInvoke.CountNonZero(imageAnd);
                    if (count > 0)
                    {
                        intersect = true;
                    }
                    //CvInvoke.Imshow("image1", image1);
                    //CvInvoke.Imshow("image2", image2);
                    //CvInvoke.Imshow("imageAnd", imageAnd);
                    //CvInvoke.WaitKey(0);
                    //Console.WriteLine("go");
                }
            }
            return intersect;
        }
        private static PadErrorDetail CheckPad(Models.PadItem padItem,PadSegmentInfo[] padSegment,double umPPixel, bool Inflate =false)
        {
            Rectangle boundPadRef = padItem.BoudingAdjust;
            if(Inflate)
                boundPadRef.Inflate(3, 3);
            double sPadRef = padItem.Area;
            List<int> idPadSegOverlap = new List<int>();
            for (int j = 0; j < padSegment.Length; j++)
            {

                //if (boundPadRef.IntersectsWith(padSegment[j].Bouding))
                //{
                //    idPadSegOverlap.Add(j);
                //}
                if (IntersectsContour(padItem.ContourAdjust, padSegment[j].Contours, boundPadRef))
                {
                    idPadSegOverlap.Add(j);
                }
            }
            boundPadRef = padItem.Bouding;
            PadErrorDetail padEr = new PadErrorDetail();
            double scaleArea = 0;
            double scaleAreaAddperimeter = 0;
            double shiftxVal = 0;
            double shiftyVal = 0;
            int inflate = 40;
            padEr.AreaStdHight = padItem.AreaThresh.UM_USL;
            padEr.AreaStdLow = padItem.AreaThresh.PERCENT_LSL;
            padEr.ShiftXStduM = padItem.ShiftXThresh.UM_USL;
            padEr.ShiftYStduM = padItem.ShiftYThresh.UM_USL;
            padEr.ShiftXStdArea = padItem.ShiftXThresh.PERCENT_LSL;
            padEr.ShiftYStdArea = padItem.ShiftYThresh.PERCENT_LSL;
            padEr.ROIOnGerber = Rectangle.Inflate(boundPadRef, inflate, inflate);
            padEr.Pad = padItem;
            if(padItem.FOVs .Count > 0)
                padEr.FOVNo = padItem.FOVs[0];
            if (idPadSegOverlap.Count > 0)
            {
                double areaAllPadSeg = 0;
                double perimeter = 0;
                Rectangle boundAllPadSeg = new Rectangle();
                for (int j = 0; j < idPadSegOverlap.Count; j++)
                {
                    PadSegmentInfo padSeg = padSegment[idPadSegOverlap[j]];
                    areaAllPadSeg += padSeg.Area;
                    using (VectorOfPoint cnt = new VectorOfPoint(padSeg.Contours))
                    {
                        perimeter += CvInvoke.ArcLength(cnt, true) / 2;
                    }
                    if (j == 0)
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
                padEr.ROIOnImage = new Rectangle(boundAllPadSeg.X - (padSegment[idPadSegOverlap[0]].Center.X - padSegment[idPadSegOverlap[0]].CenterOnFOV.X),
                    boundAllPadSeg.Y - (padSegment[idPadSegOverlap[0]].Center.Y - padSegment[idPadSegOverlap[0]].CenterOnFOV.Y),
                    boundAllPadSeg.Width,
                    boundAllPadSeg.Height
                    );
                padEr.ROIOnImage.Inflate(inflate, inflate);
                padEr.Center = new Point(boundAllPadSeg.X + boundAllPadSeg.Width / 2, boundAllPadSeg.Y + boundAllPadSeg.Height / 2);
                scaleArea = areaAllPadSeg * 100 / sPadRef;
                scaleAreaAddperimeter = (areaAllPadSeg + perimeter) * 100 / sPadRef;
                shiftxVal = (Math.Max(Math.Abs(boundPadRef.X - boundAllPadSeg.X), Math.Abs((boundPadRef.X + boundPadRef.Width) - (boundAllPadSeg.X + boundAllPadSeg.Width))) * umPPixel);
                shiftyVal = (Math.Max(Math.Abs(boundPadRef.Y - boundAllPadSeg.Y), Math.Abs((boundPadRef.Y + boundPadRef.Height) - (boundAllPadSeg.Y + boundAllPadSeg.Height))) * umPPixel);

                bool onedotfiveArea = false;
                bool hightArea = false;
                bool lowArea = false;
                bool shiftX = false;
                bool shiftY = false;
                double deviation = (100 - (sPadRef / umPPixel)) / 2;
                deviation = deviation < 0 ? 0 : deviation;
                if (scaleAreaAddperimeter < padItem.AreaThresh.PERCENT_LSL - deviation)
                {
                    lowArea = true;
                }
                if(scaleArea >= 150)
                {
                    onedotfiveArea = true;
                }
                if (scaleArea > padItem.AreaThresh.UM_USL)
                {
                    hightArea = true;
                }
                if (shiftxVal > padItem.ShiftXThresh.UM_USL + 3 * umPPixel)
                {
                    shiftX = true;
                }
                if (shiftyVal > padItem.ShiftXThresh.UM_USL + 3 * umPPixel)
                {
                    shiftY = true;
                }
                if (onedotfiveArea || hightArea || lowArea || shiftX || shiftY)
                {
                    if (onedotfiveArea && (shiftX || shiftY))
                    {
                        padEr.ErrorType = VI.ErrorType.Bridge;
                    }
                    else if (hightArea)
                    {
                        padEr.ErrorType = ErrorType.OverArea;
                    }
                    else if (lowArea)
                    {
                        padEr.ErrorType = ErrorType.Insufficient;
                    }
                    else if (shiftX)
                    {
                        padEr.ErrorType = ErrorType.ShiftX;
                    }
                    else if (shiftY)
                    {
                        padEr.ErrorType = ErrorType.ShiftY;
                    }
                    padEr.Area = scaleArea;
                    padEr.ShiftX = shiftxVal;
                    padEr.ShiftY = shiftyVal;
                    return padEr;
                }
                else
                {
                    // pad pass
                    return null;
                }
            }
            else
            {
                // not found solder paste
                padEr.ErrorType = ErrorType.Missing;
                return padEr;
            }
        }
        public static PadErrorDetail[] GetImagePadError(Image<Bgr, byte> imageSrc, PadErrorDetail[] PadError, Rectangle ROI, int limit)
        {
            int lm = PadError.Length > limit ? limit : PadError.Length;
            using (Image<Bgr, byte> image = imageSrc.Copy())
            {
                for (int i = 0; i < lm; i++)
                {
                    Rectangle bound = PadError[i].ROIOnGerber;
                    bound.X -= ROI.X;
                    bound.Y -= ROI.Y;
                    bound.X = bound.X < 0 ? 0 : bound.X;
                    bound.Y = bound.Y < 0 ? 0 : bound.Y;
                    image.ROI = bound;
                    PadError[i].PadImage = image.Copy();
                    image.ROI = Rectangle.Empty;
                }
            }
            return PadError;
        }
    }
   
}
