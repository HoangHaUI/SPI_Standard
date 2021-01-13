using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Heal;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System.Collections;
using System.Threading;

namespace SPI_AOI.Utils
{
    public class FOVOptimize
    {
        public static Point[] GetAnchorsFOV(Image<Gray, byte> ImgGerber, Rectangle ROI, Size FOV, StartPoint StartPoint = StartPoint.TOP_LEFT)
        {
            ImgGerber.ROI = ROI;
            Point[] Anchors = null;
            Size fovTruncate = new Size(FOV.Width - 64, FOV.Height - 64);
            using (Image<Gray, byte> img = ImgGerber.Copy())
            {
                ExtremePoints extreme = FindExtremeImage(img);
                Point[][] AnchorsArr = GetAnchorsX(img, extreme, fovTruncate, FOV);
                Anchors = Sort(AnchorsArr, StartPoint);
            }
            if (ROI != null)
            {
                for (int i = 0; i < Anchors.Length; i++)
                {
                    Anchors[i].X += ROI.X;
                    Anchors[i].Y += ROI.Y;
                }
            }
            ImgGerber.ROI = Rectangle.Empty;
            return Anchors;
        }
        public static Rectangle GetRectangleByAnchor(Point Anchor, Size FOV)
        {
            int x = Anchor.X - FOV.Width / 2;
            int y = Anchor.Y - FOV.Height / 2;
            int w = FOV.Width;
            int h = FOV.Height;
            return new Rectangle(x, y, w, h);
        }

        public static Image<Bgr, byte> GetDiagram(Image<Gray, byte> ImgGerber, Point[] Anchors, Size FOV)
        {
            Image<Bgr, byte> imgbgr = ImgGerber.Convert<Bgr, byte>();
            for (int i = 0; i < Anchors.Length; i++)
            {
                MCvScalar[] color = new MCvScalar[] {
                    new MCvScalar(255,0,0),
                    new MCvScalar(255,255,0),
                    new MCvScalar(0,255,0),
                    new MCvScalar(0,255,255),
                    new MCvScalar(255,0,255)
                };
                Rectangle fov = new Rectangle(Anchors[i].X - FOV.Width / 2, Anchors[i].Y - FOV.Height / 2, FOV.Width, FOV.Height);
                CvInvoke.Rectangle(imgbgr, fov, color[i % 5], 2);
                CvInvoke.PutText(imgbgr, (i + 1).ToString(), Anchors[i], Emgu.CV.CvEnum.FontFace.HersheyComplex, 3, new MCvScalar(0, 255, 0), 1);
                if (i > 0)
                {
                    CvInvoke.Line(imgbgr, Anchors[i - 1], Anchors[i], new MCvScalar(0, 0, 255), 5);
                }
            }
            return imgbgr;
        }
        public static Image<Bgr, byte> GetDiagramHightLight(Image<Gray, byte> ImgGerber, Point[] Anchors, Size FOV, int IndexHightLight)
        {
            Image<Bgr, byte> imgbgr = ImgGerber.Convert<Bgr, byte>();
            Rectangle highLight = Rectangle.Empty;
            for (int i = 0; i < Anchors.Length; i++)
            {
                MCvScalar colorHightLight = new MCvScalar(0, 255, 255);
                MCvScalar colorNormal = new MCvScalar(255, 0, 0);
                Rectangle fov = new Rectangle(Anchors[i].X - FOV.Width / 2, Anchors[i].Y - FOV.Height / 2, FOV.Width, FOV.Height);
                if (i == IndexHightLight)
                {
                    highLight = fov;
                }
                else
                {
                    CvInvoke.Rectangle(imgbgr, fov, new MCvScalar(255, 0, 0), 5);
                }
                CvInvoke.PutText(imgbgr, (i).ToString(), Anchors[i], Emgu.CV.CvEnum.FontFace.HersheyComplex, 3, new MCvScalar(0, 255, 0), 3);
                if (i > 0)
                {
                    CvInvoke.Line(imgbgr, Anchors[i - 1], Anchors[i], new MCvScalar(0, 0, 255), 5);
                }
            }
            if (highLight != Rectangle.Empty)
            {
                CvInvoke.Rectangle(imgbgr, highLight, new MCvScalar(0, 255, 255), 12);
            }
            return imgbgr;
        }
        private static Point[] Sort(Point[][] Anchors, StartPoint StartPoint)
        {
            ArrayList anchorsSorted = new ArrayList();
            /*  x = true ====>   , x = false <=====
                y = true ||         y = false /||\
                         ||                    ||
                        \||/                   ||
            */
            bool directX = true;
            bool directY = true;
            switch (StartPoint)
            {
                case StartPoint.TOP_LEFT:
                    directX = true;
                    directY = true;
                    break;
                case StartPoint.BOT_LEFT:
                    directX = true;
                    directY = false;
                    break;
                case StartPoint.TOP_RIGHT:
                    directX = false;
                    directY = true;
                    break;
                case StartPoint.BOT_RIGHT:
                    directX = false;
                    directY = false;
                    break;
                default:
                    break;
            }
            for (int y = 0; y < Anchors.Length; y++)
            {
                int realValY = directY ? y : Anchors.Length - y - 1;
                for (int x = 0; x < Anchors[realValY].Length; x++)
                {
                    int realValX = directX ? x : Anchors[realValY].Length - x - 1;
                    anchorsSorted.Add(Anchors[realValY][realValX]);
                }
                if (Anchors[realValY].Length > 0)
                {
                    directX = !directX;
                }
            }
            Point[] anchors = (Point[])anchorsSorted.ToArray(typeof(Point));
            return anchors;
        }
        private static void Optimization(Point[] Anchors)
        {
            Point startPoint = Anchors[0];
            ArrayList Points = new ArrayList();
            double initDistan = 0;
            for (int i = 1; i < Anchors.Length; i++)
            {
                initDistan += Distance(Anchors[0], Anchors[1]);
                Points.Add(Anchors[i]);
            }
            Console.WriteLine("Init dist is {0}", initDistan);
            RecursionBacktracking(startPoint, Points, 0, initDistan);
        }

        private static void RecursionBacktracking(Point CurrentPoint, ArrayList Points, double CurrentDist, double limitDist)
        {
            Thread.Sleep(10);
            Console.WriteLine(CurrentDist);
            if (Points.Count > 0)
            {
                Point[] Anchors = (Point[])Points.ToArray(typeof(Point));
                for (int i = 0; i < Anchors.Length; i++)
                {
                    double currentDist = CurrentDist;
                    currentDist += Distance(CurrentPoint, Anchors[i]);
                    if (currentDist <= limitDist)
                    {
                        ArrayList cpPoints = (ArrayList)Points.Clone();
                        cpPoints.RemoveAt(i);
                        Point crPoint = Anchors[i];
                        RecursionBacktracking(CurrentPoint, cpPoints, currentDist, limitDist);

                    }
                }
            }
            else
            {
                Console.WriteLine("Init dist is {0}", CurrentDist);
            }


        }
        private static double Distance(Point A, Point B)
        {
            return Math.Sqrt(Math.Pow(A.X - B.X, 2) + Math.Pow(A.Y - B.Y, 2));
        }
        private static Point[][] GetAnchorsX(Image<Gray, byte> Img, ExtremePoints Extreme, Size FOVTruncate, Size FOVNormal)
        {
            ArrayList listAnchors = new ArrayList();
            int stx = Extreme.Left.X, sty = Extreme.Top.Y;
            int width = Extreme.Right.X - Extreme.Left.X;
            int height = Extreme.Bot.Y - Extreme.Top.Y;
            int step = height % FOVTruncate.Height == 0 ? height / FOVTruncate.Height : height / FOVTruncate.Height + 1;
            using (Image<Gray, byte> img = Img.Copy())
            {
                for (int i = 0; i < step; i++)
                {
                    ArrayList anchors = new ArrayList();
                    int x = stx;
                    int y = i * FOVTruncate.Height + sty;
                    int w = width;
                    int h = FOVTruncate.Height;
                    Rectangle ROINormal = new Rectangle(x, y, w, h);
                    Rectangle ROIExtened = new Rectangle(x, y, w, h * 2);
                    if (y + h > sty + height)
                    {
                        ROINormal = new Rectangle(x, y, w, sty + height - y);
                    }
                    if (y + 2 * h > sty + height)
                    {
                        ROIExtened = new Rectangle(x, y, w, sty + height - y);
                    }
                    img.ROI = ROINormal;
                    using (Image<Gray, byte> tempImg = img.Copy())
                    {
                        while (true)
                        {
                            int count = CvInvoke.CountNonZero(tempImg);
                            if (count == 0)
                            {
                                break;
                            }
                            Rectangle fov = Rectangle.Empty;
                            Point anchor = new Point();
                            ExtremePoints extROI = FindExtremeImage(tempImg);
                            Rectangle tempRect = new Rectangle(extROI.Left.X, 0, FOVTruncate.Width, FOVTruncate.Height);
                            if (tempRect.X + tempRect.Width > width)
                            {
                                tempRect = new Rectangle(tempRect.X, tempRect.Y, width - tempRect.X, tempRect.Height);
                            }
                            if (tempRect.Y + tempRect.Height > height)
                            {
                                tempRect = new Rectangle(tempRect.X, tempRect.Y, tempRect.Width, height - tempRect.Y);
                            }
                            tempImg.ROI = tempRect;
                            extROI = FindExtremeImage(tempImg);
                            tempImg.ROI = Rectangle.Empty;
                            tempRect = new Rectangle(tempRect.X, extROI.Top.Y, tempRect.Width, tempRect.Height);
                            img.ROI = Rectangle.Empty;
                            img.ROI = ROIExtened;

                            if (tempRect.X + tempRect.Width > img.Width)
                            {
                                tempRect = new Rectangle(tempRect.X, tempRect.Y, img.Width - tempRect.X, tempRect.Height);
                            }
                            if (tempRect.Y + tempRect.Height > img.Height)
                            {
                                tempRect = new Rectangle(tempRect.X, tempRect.Y, tempRect.Width, img.Height - tempRect.Y);
                            }
                            using (Image<Gray, byte> tempimgfindAnchor = img.Copy())
                            {
                                tempimgfindAnchor.ROI = tempRect;
                                extROI = FindExtremeImage(tempimgfindAnchor);
                                int top = extROI.Top.Y;
                                int bot = extROI.Bot.Y;
                                int left = extROI.Left.X;
                                int right = extROI.Right.X;
                                anchor = new Point(left + (right - left + 1) / 2, top + (bot - top + 1) / 2);
                                anchor.X += tempRect.X;
                                anchor.Y += tempRect.Y;
                                fov = new Rectangle(anchor.X - FOVTruncate.Width / 2, anchor.Y - FOVTruncate.Height / 2, FOVTruncate.Width, FOVTruncate.Height);
                            }
                            CvInvoke.Rectangle(img, fov, new MCvScalar(0), -1);
                            CvInvoke.Rectangle(tempImg, fov, new MCvScalar(0), -1);
                            //fov = new Rectangle(fov.X + x, fov.Y + y, fov.Width, fov.Height);
                            // add condition

                            anchor.X += x;
                            anchor.Y += y;
                            if (anchor.X + FOVNormal.Width / 2 > Img.Width - 1)
                            {
                                anchor.X -= (anchor.X + FOVNormal.Width / 2) - (Img.Width - 1);
                            }
                            if (anchor.Y + FOVNormal.Height / 2 > Img.Height - 1)
                            {
                                anchor.Y -= (anchor.Y + FOVNormal.Height / 2) - (Img.Height - 1);
                            }
                            if (anchor.X - FOVNormal.Width / 2 < 0)
                            {
                                anchor.X += FOVNormal.Width / 2 - anchor.X;
                            }
                            if (anchor.Y - FOVNormal.Height / 2 < 0)
                            {
                                anchor.Y += FOVNormal.Height / 2 - anchor.Y;
                            }
                            anchors.Add(anchor);
                        }
                    }
                    Point[] fovs = (Point[])anchors.ToArray(typeof(Point));
                    listAnchors.Add(fovs);
                }
            }
            Point[][] bestAnchors = (Point[][])listAnchors.ToArray(typeof(Point[]));
            return bestAnchors;
        }
        private static ExtremePoints FindExtremeImage(Image<Gray, byte> ImgInput)
        {
            ExtremePoints points = new ExtremePoints();
            points.Top = new Point(0, (int)Math.Pow(2, 16));
            points.Bot = new Point(0, 0);
            points.Left = new Point((int)Math.Pow(2, 16), 0);
            points.Right = new Point(0, 0);
            using (VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint())
            {
                CvInvoke.FindContours(ImgInput, contours, null, Emgu.CV.CvEnum.RetrType.External, Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxSimple);
                for (int i = 0; i < contours.Size; i++)
                {
                    Point[] p = contours[i].ToArray();
                    var sortx = p.OrderBy(x => x.X);
                    var sorty = p.OrderBy(y => y.Y);
                    points.Top = sorty.ElementAt(0).Y < points.Top.Y ? sorty.ElementAt(0) : points.Top;
                    points.Bot = sorty.ElementAt(p.Length - 1).Y > points.Bot.Y ? sorty.ElementAt(p.Length - 1) : points.Bot;
                    points.Left = sortx.ElementAt(0).X < points.Left.X ? sortx.ElementAt(0) : points.Left;
                    points.Right = sortx.ElementAt(p.Length - 1).X > points.Right.X ? sortx.ElementAt(p.Length - 1) : points.Right;
                }
            }
            return points;
        }
    }
    class ExtremePoints
    {
        public Point Top { get; set; }
        public Point Left { get; set; }
        public Point Right { get; set; }
        public Point Bot { get; set; }
    }
    public enum StartPoint
    {
        TOP_LEFT,
        TOP_RIGHT,
        BOT_LEFT,
        BOT_RIGHT
    }
}