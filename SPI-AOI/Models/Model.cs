using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System.IO;
using System.Threading;

namespace SPI_AOI.Models
{
    public class Model
    {
        public string ID { get; set; }//
        public string Name { get;set; }//
        public string Owner { get; set; }//
        public DateTime CreateTime { get; set; }//
        public float DPI { get; set; }//
        public Size FOV { get; set; }//
        public GerberFile Gerber { get; set; }//
        public List<CadFile> Cad { get; set; }//
        public Image<Bgr, byte> ImgGerberProcessedBgr { get; set; }
        public List<PadItem> SelectPads { get; set; }
        public bool ShowLinkLine { get; set; }
        public bool ShowComponentCenter { get; set; }
        public bool ShowComponentName { get; set; }
        public bool ShowOnlyInROI { get; set; }
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
            model.ShowOnlyInROI = true;
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

        public void ClearLinkPad()
        {
            for (int i = 0; i < this.Cad.Count; i++)
            {
                this.Cad[i].ClearLinkPadItem();
            }
            this.Gerber.ClearLinkCadItem();
        }
        //public void AutoLinkPad(CadFile Cad, int Width = 800, int Height = 800)
        //{

        //    List<Tuple<Point, int>> padsList = new List<Tuple<Point, int>>();
        //    for (int i = 0; i < this.Gerber.PadItems.Count; i++)
        //    {
        //        padsList.Add(new Tuple<Point, int>(this.Gerber.PadItems[i].Center, i));
        //    }
        //    Point cadCenterRotate = Cad.CenterRotation;
        //    List<CadItem> cadItemNotLink = new List<CadItem>();
        //    int cadX = Cad.X;
        //    int cadY = Cad.Y;
        //    double cadAngle = Cad.Angle;

        //    // filter onle tow pads component
        //    foreach (var item in Cad.CadItems)
        //    {
        //        string name = Convert.ToString(item.Name[0]);
        //        string nextName = Convert.ToString(item.Name[1]);
        //        Point cadCenter = Point.Round(item.Center);
        //        Point cadCenterRotated = CadItem.GetCenterRotated(cadCenter, cadCenterRotate, cadX, cadY, cadAngle);
        //        var sorted = padsList.OrderBy(i => ImageProcessingUtils.DistanceTwoPoint(i.Item1, cadCenterRotated));
        //        Tuple<Point, int>[] arSorted = sorted.ToArray();
        //        Point p1 = arSorted[0].Item1;
        //        Point p2 = arSorted[1].Item1;
        //        Point p3 = arSorted[2].Item1;
        //        Point ctP12 = new Point((p1.X + p2.X) / 2, (p1.Y + p2.Y) / 2);
        //        double d1 = ImageProcessingUtils.DistanceTwoPoint(p1, cadCenterRotated);
        //        double d2 = ImageProcessingUtils.DistanceTwoPoint(p2, cadCenterRotated);
        //        double d3 = ImageProcessingUtils.DistanceTwoPoint(p3, cadCenterRotated);
        //        System.Windows.Vector v1 = new System.Windows.Vector(p1.X - p2.X, p1.Y - p2.Y);
        //        System.Windows.Vector vo = new System.Windows.Vector(1, 0);
        //        double angle = System.Windows.Vector.AngleBetween(v1, vo);
        //        angle = Math.Abs(angle % 45);
        //        double angleRef = Math.Abs(Cad.Angle % 45);
        //        double deviationDist = Math.Max(d1, d2) > 0.05 * this.DPI ? 0.1 * Math.Min(d1, d2) : 0.03 * this.DPI;
        //        if(item.Name == "L2977")
        //        {
        //            Console.WriteLine("");
        //        }
        //        if ((d2 - d1 < deviationDist &&
        //            (d3 - d2) > 2 * (d2 - d1) &&
        //            Math.Abs(angle - angleRef) < 5 &&
        //            Math.Abs(ctP12.X - cadCenterRotated.X) < deviationDist &&
        //            Math.Abs(ctP12.Y - cadCenterRotated.Y) < deviationDist)
        //            || (name.ToUpper() == "R" || name.ToUpper() == "C") && "0123456789".Contains(nextName))
        //        {
        //            item.Pads.Add(this.Gerber.PadItems[sorted.ElementAt(0).Item2]);
        //            item.Pads.Add(this.Gerber.PadItems[sorted.ElementAt(1).Item2]);
        //            this.Gerber.PadItems[sorted.ElementAt(0).Item2].CadItem = item;
        //            this.Gerber.PadItems[sorted.ElementAt(1).Item2].CadItem = item;
        //        }
        //        else
        //        {
        //            if (name.ToUpper() != "S")
        //            {
        //                cadItemNotLink.Add(item);
        //            }
        //        }
        //    }
        //    // reset pad list
        //    padsList.Clear();
        //    for (int i = 0; i < this.Gerber.PadItems.Count; i++)
        //    {
        //        if (this.Gerber.PadItems[i].CadItem == null || this.Gerber.PadItems[i].CadItem == new CadItem())
        //        {
        //            padsList.Add(new Tuple<Point, int>(this.Gerber.PadItems[i].Center, i));
        //        }
        //    }
        //}

        public void AutoLinkPad(CadFile Cad, int Width = 800, int Height = 800)
        {

            List<Tuple<Point, int>> padsList = new List<Tuple<Point, int>>();
            for (int i = 0; i < this.Gerber.PadItems.Count; i++)
            {
                padsList.Add(new Tuple<Point, int>(this.Gerber.PadItems[i].Center, i));
            }
            Point cadCenterRotate = Cad.CenterRotation;
            List<CadItem> cadItemNotLink = new List<CadItem>();
            int cadX = Cad.X;
            int cadY = Cad.Y;
            double cadAngle = Cad.Angle;
            // filter Resistor and Capacitor component
            foreach (var item in Cad.CadItems)
            {
                string name = Convert.ToString(item.Name[0]);
                string nextName = Convert.ToString(item.Name[1]);
                Point cadCenter = Point.Round(item.Center);
                Point cadCenterRotated = CadItem.GetCenterRotated(cadCenter, cadCenterRotate, cadX, cadY, cadAngle);
                var sorted = padsList.OrderBy(i => ImageProcessingUtils.DistanceTwoPoint(i.Item1, cadCenterRotated));
                if ((name.ToUpper() == "R" || name.ToUpper() == "C") && "0123456789".Contains(nextName))
                {
                    item.Pads.Add(this.Gerber.PadItems[sorted.ElementAt(0).Item2]);
                    item.Pads.Add(this.Gerber.PadItems[sorted.ElementAt(1).Item2]);
                    this.Gerber.PadItems[sorted.ElementAt(0).Item2].CadItem = item;
                    this.Gerber.PadItems[sorted.ElementAt(1).Item2].CadItem = item;
                }
                else
                {
                    if (name.ToUpper() != "S")
                    {
                        cadItemNotLink.Add(item);
                    }
                }
            }
            // reset pad list
            padsList.Clear();
            for (int i = 0; i < this.Gerber.PadItems.Count; i++)
            {
                if (this.Gerber.PadItems[i].CadItem == null || this.Gerber.PadItems[i].CadItem == new CadItem())
                {
                    padsList.Add(new Tuple<Point, int>(this.Gerber.PadItems[i].Center, i));
                }
            }

            // filter only has two pad
            List<Tuple<Point, int, CadItem>> mayPads = new List<Tuple<Point, int, CadItem>>();
            foreach (var item in cadItemNotLink)
            {
                string name = Convert.ToString(item.Name[0]);
                string nextName = Convert.ToString(item.Name[1]);
                Point cadCenter = Point.Round(item.Center);
                Point cadCenterRotated = CadItem.GetCenterRotated(cadCenter, cadCenterRotate, cadX, cadY, cadAngle);
                var sorted = padsList.OrderBy(i => ImageProcessingUtils.DistanceTwoPoint(i.Item1, cadCenterRotated));
                Tuple<Point, int>[] arSorted = sorted.ToArray();
                List<int> idGot = new List<int>();
                int limit = arSorted.Length > 10 ? 10 : arSorted.Length;
                //if (item.Name == "D1010")
                //{
                //    Console.WriteLine("Break");
                //}
                double crDist = -1;
                for (int i = 0; i < limit - 1; i++)
                {
                    if (idGot.Contains(arSorted[i].Item2)) continue;
                    if (i == 1 && idGot.Count == 0)
                    {
                        break;
                    }
                    Point p1 = arSorted[i].Item1;
                    double d1 = ImageProcessingUtils.DistanceTwoPoint(p1, cadCenterRotated);
                    if (idGot.Count > 0 && 2 * crDist < d1)
                    {
                        break;
                    }
                    if (d1 > this.DPI / 2)
                    {
                        break;
                    }
                    for (int j = i + 1; j < limit; j++)
                    {
                        Point p2 = arSorted[j].Item1;
                        double d2 = ImageProcessingUtils.DistanceTwoPoint(p2, cadCenterRotated);
                        double deviationDist = Math.Max(d1, d2) > 0.05 * this.DPI ? 0.1 * Math.Min(d1, d2) : 0.03 * this.DPI;
                        if (Math.Abs(d1 - d2) > deviationDist || d2 > this.DPI / 2)
                        {
                            break;
                        }
                        Point ctP12 = new Point((p1.X + p2.X) / 2, (p1.Y + p2.Y) / 2);
                        if (Math.Abs(ctP12.X - cadCenterRotated.X) < 0.01 * this.DPI || Math.Abs(ctP12.Y - cadCenterRotated.Y) < 0.01)
                        {
                            if (crDist != -1 && (Math.Abs(crDist - d1) > 0.5 * crDist || Math.Abs(crDist - d2) > 0.5 * crDist))
                            {
                                break;
                            }
                            crDist = (d1 + d2) / 2;
                            crDist = Math.Min(d1, d2);
                            idGot.Add(arSorted[i].Item2);
                            idGot.Add(arSorted[j].Item2);
                        }
                    }
                }
                if (idGot.Count == 2)
                {
                    for (int i = 0; i < idGot.Count; i++)
                    {
                        this.Gerber.PadItems[idGot[i]].CadItem = item;
                        item.Pads.Add(this.Gerber.PadItems[idGot[i]]);
                    }
                }
            }
            // reset pads and cad list
            padsList.Clear();
            cadItemNotLink.Clear();
            for (int i = 0; i < this.Gerber.PadItems.Count; i++)
            {
                if (this.Gerber.PadItems[i].CadItem == null || this.Gerber.PadItems[i].CadItem == new CadItem())
                {
                    padsList.Add(new Tuple<Point, int>(this.Gerber.PadItems[i].Center, i));
                }
            }
            for (int i = 0; i < Cad.CadItems.Count; i++)
            {
                if (Cad.CadItems[i].Pads.Count == 0)
                {
                    cadItemNotLink.Add(Cad.CadItems[i]);
                }
            }

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
