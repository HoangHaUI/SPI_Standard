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
        public static Image<Bgr, byte> ImageTransformation(Image<Bgr, byte> scr, int X, int Y)
        {
              float[,] translationArray = { { 1, 0, X }, { 0, 1, Y } };
            using (Matrix<float> translationMatrix = new Matrix<float>(translationArray))
            {
                CvInvoke.WarpAffine(scr, scr, translationMatrix, scr.Size);
            }
            return scr;
        }
        public static Image<Gray, byte> ImageTransformation(Image<Gray, byte> scr, int X, int Y)
        {
            float[,] translationArray = { { 1, 0, X }, { 0, 1, Y } };
            using (Matrix<float> translationMatrix = new Matrix<float>(translationArray))
            {
                CvInvoke.WarpAffine(scr, scr, translationMatrix, scr.Size);
            }
            return scr;
        }
        public static int GetCircleByThreePoint(Point P1, Point P2, Point P3, ref PointF Center, ref double Radius)
        {
            // phuong trinh duong thang p1, p2
            float dt12a = (P1.X - P2.X) == 0 ? 0 : (P1.Y - P2.Y) / (P1.X - P2.X);
            float dt12b = P2.Y - (dt12a * P2.X);
            // phuon trinh duong thang p2,p3
            float dt23a = (P3.X - P2.X) == 0 ? 0 : (P3.Y - P2.Y) / (P3.X - P2.X);
            float dt23b = P2.Y - (dt12a * P2.X);
            //tim trung diem
            PointF mid12 = new PointF((P1.X + P2.X) / 2, (P1.Y + P2.Y) / 2);
            PointF mid23 = new PointF((P3.X + P2.X) / 2, (P3.Y + P2.Y) / 2);
            //tim duong thang vuong goc di qua trung dien
            dt12a = dt12a == 0 ? 0 : -1 / dt12a;
            dt12b = mid12.Y - dt12a * mid12.X;
            dt23a = dt23a == 0 ? 0 : -1 / dt23a;
            dt23b = mid23.Y - dt23a * mid23.X;
            // tinh tam va ban kinh
            Center.X = (dt12a - dt23a) == 0 ? 0 : ((dt23b - dt12b) / (dt12a - dt23a));
            Center.Y = (dt23a * Center.X + dt23b);
            Radius = (float)Math.Sqrt(Math.Pow((P1.X - Center.X), 2) + Math.Pow((P1.Y - Center.Y), 2));
            return 0;
        }
        public static double DistanceTwoPoint(Point P1, Point P2)
        {
            return Math.Sqrt(Math.Pow(P2.X - P1.X, 2) + Math.Pow(P2.Y - P1.Y, 2));
        }
        public static double DistanceSquaredAngle(Point P1, Point P2)
        {
            System.Windows.Vector v1 = new System.Windows.Vector(P2.X - P1.X, P2.Y - P1.Y);
            System.Windows.Vector vx = new System.Windows.Vector(1, 0);
            System.Windows.Vector vy = new System.Windows.Vector(0, 1);
            double angle = System.Windows.Vector.AngleBetween(v1, vx);
            if (Math.Abs(angle % 90) > 45)
            {
                angle = System.Windows.Vector.AngleBetween(v1, vy);
            }
            angle = Math.Abs(angle % 90);
            double leght = DistanceTwoPoint(P1, P2);
            double dist = Math.Cos(angle * Math.PI / 180.0) * leght;
            return dist;
        }
        
    }
}
