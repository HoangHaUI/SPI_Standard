using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Util;

namespace SPI_AOI
{
    class ImageProcessingUtils
    {
        public static Point PointRotation(Point RotatePoint, Point Center, double Angle)
        {
            int x = RotatePoint.X - Center.X;
            int y = RotatePoint.Y - Center.Y;
            int x1 = (int)Math.Round(x * Math.Cos(Angle) - y * Math.Sin(Angle));
            int y1 = (int)Math.Round(x * Math.Sin(Angle) + y * Math.Cos(Angle));
            RotatePoint.X = x1 + Center.X;
            RotatePoint.Y = y1 + Center.Y;
            return RotatePoint;
        }
        public static Image<Bgr, byte> ImageRotation(Image<Bgr, byte> scr, Point Center, double Angle)
        {
            Angle = Angle * 180.0 / Math.PI;
            using (Mat rotMatrix = new Mat())
            {
                CvInvoke.GetRotationMatrix2D(Center, -Angle, 1, rotMatrix);
                CvInvoke.WarpAffine(scr, scr, rotMatrix, scr.Size);
            }
            return scr;
        }
        public static Image<Gray, byte> ImageRotation(Image<Gray, byte> scr, Point Center, double Angle)
        {
            Angle = Angle * 180.0 / Math.PI;
            using (Mat rotMatrix = new Mat())
            {
                CvInvoke.GetRotationMatrix2D(Center, -Angle, 1, rotMatrix);
                CvInvoke.WarpAffine(scr, scr, rotMatrix, scr.Size);
            }
            return scr;
        }
    }
}
