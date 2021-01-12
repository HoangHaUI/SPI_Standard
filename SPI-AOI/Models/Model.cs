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
using Newtonsoft.Json;

namespace SPI_AOI.Models
{
    public class Model
    {
        public string ID { get; set; }
        public string Name { get;set; }
        public string Owner { get; set; }
        public DateTime CreateTime { get; set; }
        public float DPI { get; set; }
        public Size FOV { get; set; }
        public double PulseXPerPixel { get; set; }
        public double PulseYPerPixel { get; set; }
        public double AngleAxisCamera { get; set; }
        public double AngleXYAxis { get; set; }
        public double PCBThickness { get; set; }
        public GerberFile Gerber { get; set; }
        public List<CadFile> Cad { get; set; }
        public Image<Bgr, byte> ImgGerberProcessedBgr { get; set; }
        public List<PadItem> SelectPads { get; set; }
        public bool ShowLinkLine { get; set; }
        public bool ShowComponentCenter { get; set; }
        public bool ShowComponentName { get; set; }
        public bool ShowOnlyInROI { get; set; }
        public bool HightLineLinkedPad { get; set; }
        public Hardware HardwareSettings { get; set; }

        private static string ModelPath = "Models";
        public static Model GetNewModel(
            string ModelName, string Owner, string GerberPath, 
            float DPI, Size FOV, double PXPP, double PYPP, 
            double AngleXAxisCamera, double AngleXYAxis, double PCBThickness)
        {
            Model model = new Model();
            model.ID = Utils.GetNewID();
            model.Name = ModelName;
            model.Owner = Owner;
            model.CreateTime = DateTime.Now;
            model.ShowLinkLine = true;
            model.ShowComponentCenter = true;
            model.ShowComponentName = true;
            model.ShowOnlyInROI = true;
            model.HightLineLinkedPad = true;
            model.DPI = DPI;
            model.PulseXPerPixel = PXPP;
            model.PulseYPerPixel = PYPP;
            model.AngleAxisCamera = AngleXAxisCamera;
            model.AngleXYAxis = AngleXYAxis;
            model.PCBThickness = PCBThickness;
            model.FOV = FOV;
            model.GetNewGerber(GerberPath);
            model.Cad = new List<CadFile>();
            model.HardwareSettings = new Hardware();
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
        public Rectangle GetRectangleComponent(PadItem pad)
        {
            int Top = (int) Math.Pow(2, 30);
            int Left = (int)Math.Pow(2, 30);
            int Bot = 0;
            int Right = 0;
            string ComponentName = this.GetComponentName(pad);
            if (ComponentName == "UNDEFINE")
                return pad.Bouding;
            for (int i = 0; i < this.Gerber.PadItems.Count; i++)
            {
                PadItem item = this.Gerber.PadItems[i];
                if(item.CadFileID == pad.CadFileID && item.CadItemIndex == pad.CadItemIndex)
                {
                    Point[] p = item.Contour.ToArray();
                    for (int j = 0; j < p.Length; j++)
                    {
                        Top = p[j].Y < Top ? p[j].Y : Top;
                        Left = p[j].X < Left ? p[j].X : Left;
                        Bot = p[j].Y > Bot ? p[j].Y : Bot;
                        Right = p[j].X > Right ? p[j].X : Right;
                    }
                }
            }
            if(Top != (int)Math.Pow(2, 30) && Left != (int)Math.Pow(2, 30) && Bot != 0 && Right != 0)
            {
                return new Rectangle(Left, Top, Right - Left, Bot - Top);
            }
            else
            {
                return Rectangle.Empty;
            }
        }
        public string GetComponentName(PadItem pad)
        {
            for (int c = 0; c < this.Cad.Count; c++)
            {
                if (this.Cad[c].CadFileID == pad.CadFileID)
                {
                    return this.Cad[c].CadItems[pad.CadItemIndex].Name;
                }
            }
            return null;
        }
        public void UpdateAfterEditGerber()
        {
            this.Gerber.UpdateFOV(this.FOV);
            this.Gerber.LinkPadWidthFov(this.FOV);
        }
        public void UpdateFOV()
        {
            this.Gerber.UpdateFOV(this.FOV);
            this.Gerber.LinkPadWidthFov(this.FOV);
        }
        public int GetNewCad(string Path)
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
        public Image<Bgr, byte> GetDiagramImage()
        {
            return this.Gerber.GetDiagramImage();
        }
        public Point[] GetAnchorsDiagram()
        {
            Point[] anchors = new Point[ this.Gerber.FOVs.Count];
            for (int i = 0; i < anchors.Length; i++)
            {
                anchors[i] = new Point(this.Gerber.FOVs[i].Anchor.X - this.Gerber.ROI.X, this.Gerber.FOVs[i].Anchor.Y - this.Gerber.ROI.Y);
            }
            return anchors;
        }
        private Point GetPLCPoint(Point PlcRefPoint, Point RefPoint, Point TranfPoint)
        {
            Point point = new Point();
            Point ctMark1 = RefPoint;
            Point ctMark2 = TranfPoint;

            double subx = (ctMark2.X - ctMark1.X);
            double suby = (ctMark2.Y - ctMark1.Y);

            double subxPulse = subx * this.PulseXPerPixel;
            double subyPulse = suby * this.PulseYPerPixel;
            double devPulseX = Math.Sin(this.AngleXYAxis * Math.PI / 180.0) * subxPulse;
            double devPulseY = Math.Sin(-this.AngleXYAxis * Math.PI / 180.0) * subyPulse;

            int disX = Convert.ToInt32(subxPulse + devPulseX);
            int disY = Convert.ToInt32(subyPulse + devPulseY);
            point = new Point(PlcRefPoint.X + disX, PlcRefPoint.Y + disY);
            return point;
        }
        public Point[] GetPLCMarkPosition()
        {
            Point[] mark = new Point[2];
            Point realMark1 = this.HardwareSettings.MarkPosition;
            if (this.Gerber.MarkPoint.PadMark.Count == 2)
            {
                mark[0] = new Point(realMark1.X, realMark1.Y);
                PadItem padMark1 = this.Gerber.PadItems[this.Gerber.MarkPoint.PadMark[0]];
                PadItem padMark2 = this.Gerber.PadItems[this.Gerber.MarkPoint.PadMark[1]];
                Point ctMark1 = padMark1.Center;
                Point ctMark2 = padMark2.Center;
                mark[1] = GetPLCPoint(realMark1, ctMark1, ctMark2);
            }
            return mark;
        }
        public Point[] GetPulseXYFOVs()
        {
            Point[] position = new Point[this.Gerber.FOVs.Count];
            Point realMark1 = this.HardwareSettings.MarkPosition;
            for (int i = 0; i < this.Gerber.FOVs.Count; i++)
            {
                PadItem padMark1 = this.Gerber.PadItems[this.Gerber.MarkPoint.PadMark[0]];
                Point ctMark1 = padMark1.Center;
                Point ctMark2 = this.Gerber.FOVs[i].Anchor;

                position[i] = GetPLCPoint(realMark1, ctMark1, ctMark2);
            }
            return position;
        }
        public Point[] GetAnchorsFOV(bool setROI = true)
        {
            Point[] fovs = new Point[this.Gerber.FOVs.Count];
            for (int i = 0; i < fovs.Length; i++)
            {
                Point anchor = this.Gerber.FOVs[i].Anchor;
                if (setROI)
                {
                    Rectangle ROI = this.Gerber.ROI;
                    fovs[i] = new Point(anchor.X - ROI.X, anchor.Y - ROI.Y);
                }
                else
                {
                    fovs[i] = new Point(anchor.X, anchor.Y);
                }
            }
            return fovs;
        }
        public Rectangle GetRectROIMark()
        {
            int searchW = Convert.ToInt32(this.Gerber.MarkPoint.SearchX * this.DPI / 25.4);
            int searchH = Convert.ToInt32(this.Gerber.MarkPoint.SearchY * this.DPI / 25.4);
            Rectangle rectSearch = new System.Drawing.Rectangle(2448 / 2 - searchW / 2, 2048 / 2 - searchH / 2, searchW, searchH);
            return rectSearch;
        }
        public string SaveModel()
        {
            string modelPath = ModelPath  + "/" + this.Name + ".json";
            var mgGerberProcessedBgr = this.ImgGerberProcessedBgr;
            this.ImgGerberProcessedBgr = null;
            var orgGerberImage = this.Gerber.OrgGerberImage;
            this.Gerber.OrgGerberImage = null;
            var processingGerberImage = this.Gerber.ProcessingGerberImage;
            this.Gerber.ProcessingGerberImage = null;
            try
            {
                string json = JsonConvert.SerializeObject(this);
                File.WriteAllText(modelPath, json);
            }
            catch
            {
                return null;
            }
            finally
            {
                this.ImgGerberProcessedBgr = mgGerberProcessedBgr;
                this.Gerber.OrgGerberImage = orgGerberImage;
                this.Gerber.ProcessingGerberImage = processingGerberImage;
            }
            return new FileInfo(modelPath).FullName;
        }
        public static Model LoadModelByPath(string Path)
        {
            Model model = null;
            try
            {
                string s = File.ReadAllText(Path);
                model = JsonConvert.DeserializeObject<Model>(s);
            }
            catch
            {
                return null;
            }
            if(model != null)
            {
                if (model.Gerber != null)
                {
                    model.Gerber.LoadGerber(model.DPI, model.FOV);
                }
            }
            return model;
        }
        public static Model LoadModelByName(string ModelName)
        {
            string Path = GetPathModelByName(ModelName);
            return LoadModelByPath(Path);
        }
        public static string[] GetModelNames()
        {
            string[] mListModelNames = Directory.GetFiles(ModelPath, "*.json");
            for (int i = 0; i < mListModelNames.Length; i++)
            {
                FileInfo fi = new FileInfo(mListModelNames[i]);
                mListModelNames[i] = fi.Name.Replace(".json", "");
            }
            return mListModelNames;
        }
        private static string GetPathModelByName(string modelName)
        {
            string[] mListModelNames = Directory.GetFiles(ModelPath, "*.json");
            for (int i = 0; i < mListModelNames.Length; i++)
            {
                FileInfo fi = new FileInfo(mListModelNames[i]);
                if (fi.Name.Contains(modelName))
                {
                    return fi.FullName;
                }
            }
            return null;
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
            ClearLinkPad();
            double angle = this.Gerber.Angle;
            angle = (angle + Angle) % 360;
            this.Gerber.SetAngle(angle, this.FOV);
            
        }
        public Image<Bgr, byte> GetFOVImage(int IDFOV = -1)
        {
            return this.Gerber.GetFOVDiagram(this.FOV, IDFOV);
        }
        public void SetROI(Rectangle ROI)
        {
            ClearLinkPad();
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
        public void DeteleLinkPad(PadItem Pad)
        {
            if (Pad.CadItemIndex < 0)
                return;
            int idPad = this.Gerber.PadItems.IndexOf(Pad);
            for (int i = 0; i < this.Cad.Count; i++)
            {
                if(this.Cad[i].CadFileID == Pad.CadFileID)
                {
                    CadFile cad = this.Cad[i];
                    cad.CadItems[Pad.CadItemIndex].PadsIndex.Remove(idPad);
                    break;
                }
            }
            Pad.CadItemIndex = -1;
            Pad.CadFileID = string.Empty;
        }
        public List<PadItem> GetPadsInRect(Rectangle Rect, bool Linked = false)
        {
            List<PadItem> pads = new List<PadItem>();
            Thread t1 = new Thread(() => {
                List<PadItem> padT1 = GetPadsInRect(Rect, 0, 1* this.Gerber.PadItems.Count / 3, Linked);
                lock(pads)
                {
                    pads.AddRange(padT1);
                }
            });
            Thread t2 = new Thread(() => {
                List<PadItem> padT2 = GetPadsInRect(Rect, 1 * this.Gerber.PadItems.Count / 3, 2*this.Gerber.PadItems.Count / 3, Linked);
                lock (pads)
                {
                    pads.AddRange(padT2);
                }
            });
            Thread t3 = new Thread(() => {
                List<PadItem> padT3 = GetPadsInRect(Rect, 2 * this.Gerber.PadItems.Count / 3, this.Gerber.PadItems.Count, Linked);
                lock (pads)
                {
                    pads.AddRange(padT3);
                }
            });
            t1.Start();
            t2.Start();
            t3.Start();
            t1.Join();
            t2.Join();
            t3.Join();
            return pads;
        }
        private List<PadItem> GetPadsInRect(Rectangle Rect, int Start, int Stop, bool Linked = false)
        {
            List<PadItem> pads = new List<PadItem>();
            for (int i = Start; i < Stop; i++)
            {
                var item = this.Gerber.PadItems[i];
                if (Linked)
                {
                    if (item.CadItemIndex < 0)
                    {
                        continue;
                    }
                }
                else
                {
                    if (item.CadItemIndex >= 0)
                    {
                        continue;
                    }
                }
                Rectangle bound = new Rectangle(item.Center.X, item.Center.Y, 1, 1);
                if (Rect.Contains(bound))
                {
                    pads.Add(item);
                }
            }
            return pads;
        }
        public List<Tuple<CadFile, int>> GetSuggestCadItemName(Rectangle Rect, bool GetAllComponent = false)
        {
            List<Tuple<CadFile, int>> suggest = new List<Tuple<CadFile, int>>();
            if (Rect != Rectangle.Empty)
            {
                Thread t1 = new Thread(() => { 
                  // find pads in rect
                    for (int i = 0; i < this.Gerber.PadItems.Count; i++)
                    {
                        var item = this.Gerber.PadItems[i];
                        if(item.CadItemIndex >=0)
                        {
                            Rectangle bound = new Rectangle(item.Center.X, item.Center.Y, 1, 1);
                            if (Rect.Contains(bound))
                            {
                                for (int j = 0; j < this.Cad.Count; j++)
                                {
                                    if (this.Cad[j].CadFileID == item.CadFileID)
                                    {
                                        Tuple<CadFile, int> tlCad = new Tuple<CadFile, int>(this.Cad[j], item.CadItemIndex);
                                        lock (suggest)
                                        {
                                            if (!suggest.Contains(tlCad))
                                            {
                                                suggest.Add(tlCad);
                                            }
                                        }
                                        
                                    }
                                }
                            }
                        }
                    
                    }
                });
                // cad layer
                Thread t2 = new Thread(()=>{
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
                                Tuple<CadFile, int> tlCad = new Tuple<CadFile, int>(item, i);
                                lock (suggest)
                                {
                                    if (!suggest.Contains(tlCad) || i == item.CadItems.Count - 1)
                                    {
                                        suggest.Add(tlCad);
                                    }
                                }
                            }
                        }
                    }
                    foreach (var item in this.Cad)
                    {
                        for (int i = item.CadItems.Count - 1; i >= 0; i--)
                        {
                            Tuple<CadFile, int> tlCad = new Tuple<CadFile, int>(item, i);
                            if (!suggest.Contains(tlCad))
                            {
                                suggest.Add(tlCad);
                                if (!GetAllComponent)
                                {
                                    break;
                                }
                            }
                        }
                    }
                });
                t1.Start();
                t2.Start();
                t1.Join();
                t2.Join();
            }
            return suggest;
        }
        
        public void AutoLinkPad(CadFile Cad, int Mode, int Width = 800, int Height = 800)
        {
            
            List<Tuple<Point, int>> padsList = new List<Tuple<Point, int>>();
            for (int i = 0; i < this.Gerber.PadItems.Count; i++)
            {
                padsList.Add(new Tuple<Point, int>(this.Gerber.PadItems[i].Center, i));
            }
            Point cadCenterRotate = Cad.CenterRotation;
            List<Tuple<CadItem, int>> cadItemNotLink = new List<Tuple<CadItem, int>>();
            int cadX = Cad.X;
            int cadY = Cad.Y;
            double cadAngle = Cad.Angle;
            // filter Resistor and Capacitor component
            for (int t = 0; t < Cad.CadItems.Count; t++)
            {
                var item = Cad.CadItems[t];
                string name = Convert.ToString(item.Name[0]);
                string nextName = Convert.ToString(item.Name[1]);
                Point cadCenter = Point.Round(item.Center);
                Point cadCenterRotated = CadItem.GetCenterRotated(cadCenter, cadCenterRotate, cadX, cadY, cadAngle);
                var sorted = padsList.OrderBy(i => ImageProcessingUtils.DistanceTwoPoint(i.Item1, cadCenterRotated));
                if ((name.ToUpper() == "R" || name.ToUpper() == "C") && "0123456789".Contains(nextName))
                {
                    item.PadsIndex.Add(sorted.ElementAt(0).Item2);
                    item.PadsIndex.Add(sorted.ElementAt(1).Item2);
                    this.Gerber.PadItems[sorted.ElementAt(0).Item2].CadFileID = Cad.CadFileID;
                    this.Gerber.PadItems[sorted.ElementAt(0).Item2].CadItemIndex = t;
                    this.Gerber.PadItems[sorted.ElementAt(1).Item2].CadFileID = Cad.CadFileID;
                    this.Gerber.PadItems[sorted.ElementAt(1).Item2].CadItemIndex = t;
                }
                else
                {
                    if (name.ToUpper() != "S")
                    {
                        cadItemNotLink.Add(new Tuple<CadItem, int>(item, t));
                    }
                }
            }
            // reset pad list
            padsList.Clear();
            for (int i = 0; i < this.Gerber.PadItems.Count; i++)
            {
                if (string.IsNullOrEmpty(this.Gerber.PadItems[i].CadFileID))
                {
                    padsList.Add(new Tuple<Point, int>(this.Gerber.PadItems[i].Center, i));
                }
            }
            if (Mode < 1)
                return;
            // filter only has two pad
            foreach (var itemtl in cadItemNotLink)
            {
                var item = itemtl.Item1;
                string name = Convert.ToString(item.Name[0]);
                string nextName = Convert.ToString(item.Name[1]);
                Point cadCenter = Point.Round(item.Center);
                Point cadCenterRotated = CadItem.GetCenterRotated(cadCenter, cadCenterRotate, cadX, cadY, cadAngle);
                var sorted = padsList.OrderBy(i => ImageProcessingUtils.DistanceTwoPoint(i.Item1, cadCenterRotated));
                Tuple<Point, int>[] arSorted = sorted.ToArray();
                List<int> idGot = new List<int>();
                int limit = arSorted.Length > 10 ? 10 : arSorted.Length;
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
                        if (Math.Abs(ctP12.X - cadCenterRotated.X) < 0.01 * this.DPI || Math.Abs(ctP12.Y - cadCenterRotated.Y) < 0.01 * this.DPI)
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
                        this.Gerber.PadItems[idGot[i]].CadFileID = Cad.CadFileID;
                        this.Gerber.PadItems[idGot[i]].CadItemIndex = itemtl.Item2;
                        item.PadsIndex.Add(idGot[i]);
                    }
                }
            }
            // reset pads and cad list
            padsList.Clear();
            cadItemNotLink.Clear();
            if (Mode < 2)
                return;
            //get all pad
            for (int i = 0; i < this.Gerber.PadItems.Count; i++)
            {
                padsList.Add(new Tuple<Point, int>(this.Gerber.PadItems[i].Center, i));
            }
            for (int i = 0; i < Cad.CadItems.Count; i++)
            {
                if (Cad.CadItems[i].PadsIndex.Count == 0 && Cad.CadItems[i].Name[0].ToString().ToUpper() != "S")
                {
                    cadItemNotLink.Add(new Tuple<CadItem, int>(Cad.CadItems[i], i));
                }
            }
            foreach (var itemtl in cadItemNotLink)
            {
                var item = itemtl.Item1;
                string name = Convert.ToString(item.Name[0]);
                string nextName = Convert.ToString(item.Name[1]);
                Point cadCenter = Point.Round(item.Center);
                Point cadCenterRotated = CadItem.GetCenterRotated(cadCenter, cadCenterRotate, cadX, cadY, cadAngle);
                double angle = Math.Abs(item.Angle % 90);
                angle = angle > 45 ? 90 - angle : angle;
                var sorted = padsList.OrderBy(i => ImageProcessingUtils.DistanceTwoPoint(i.Item1, cadCenterRotated) * Math.Cos(angle * Math.PI / 180.0));
                
                Tuple<Point, int>[] arSorted = sorted.ToArray();
                int limit = arSorted.Length > 1000 ? 1000 : arSorted.Length;
                List<int> idGot = new List<int>();
                //double deviationDist = Math.Max(d1, d2) > 0.05 * this.DPI ? 0.1 * Math.Min(d1, d2) : 0.03 * this.DPI;
                for (int i = 0; i < limit - 1; i++)
                {
                    if (!string.IsNullOrEmpty(this.Gerber.PadItems[arSorted[i].Item2].CadFileID))
                    {
                        break;
                    }
                    double d1 = ImageProcessingUtils.DistanceTwoPoint(arSorted[i].Item1, cadCenterRotated);
                    if(d1 > this.DPI)
                    {
                        break;
                    }
                    idGot.Add(arSorted[i].Item2);
                }
                for (int i = 0; i < idGot.Count; i++)
                {
                    this.Gerber.PadItems[idGot[i]].CadFileID = Cad.CadFileID;
                    this.Gerber.PadItems[idGot[i]].CadItemIndex = itemtl.Item2;
                    item.PadsIndex.Add(idGot[i]);
                }
            }
        }

        public List<object> GetListLayerInRect(System.Drawing.Rectangle Rect)
        {
            List<object> listObj = new List<object>();
            if (Rect != Rectangle.Empty)
            {
                //gerber layer
                var a = GetPadsInRect(Rect, true);
                var b = GetPadsInRect(Rect, false);
                if(a.Count + b.Count > 0)
                {
                    listObj.Add(this.Gerber);
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
        public void Dispose()
        {
            this.Gerber.Dispose();
        }
    }
}
