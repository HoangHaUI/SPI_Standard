﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using NLog;

namespace SPI_AOI.Models
{
    public class CadFile
    {
        private static NLog.Logger mLog = Heal.LogCtl.GetInstance();
        public string ModelID { get; set; }
        public string CadFileID { get; set; }
        public Color Color { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string[] CadFileData { get; set; }
        public bool Visible { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public double Angle { get; set; }
        public Point CenterRotation { get; set; }
        public List<CadItem> CadItems { get; set; }
        public List<CadItem> CadItemSelected { get; set; }
        public static CadFile GetNewCadFile(string ID, string Path, double DPI, int GerberWidth = 0, int GerberHeight= 0)
        {
            FileInfo fi = new FileInfo(Path);
            CadFile cad = new CadFile();
            cad.ModelID = ID;
            cad.CadFileID = Utils.GetNewID();
            cad.Color = Color.FromArgb(0, 255, 0);
            cad.FileName = fi.Name;
            cad.FilePath = fi.FullName;
            cad.Visible = true;
            cad.Angle = 0;
            cad.CadItems = new List<CadItem>();
            cad.CadItemSelected = new List<CadItem>();
            string[] content = File.ReadAllLines(fi.FullName);
            double _XMin = 999999;
            double _YMin = 999999;
            double _XMax = -999999;
            double _YMax = -999999;
            foreach (string item in content)
            {
                string line = item.Replace('\t', ' ');
                string[] arrall = line.Split(' ');
                List<string> arr = new List<string>();
                foreach (var str in arrall)
                {
                    if(str != "")
                    {
                        arr.Add(str);
                    }
                }
                CadItem cadItem = new CadItem();
                try
                {
                    cadItem.CadFileID = cad.CadFileID;
                    cadItem.Name = arr[0];
                    cadItem.Center = new PointF((float)(-Convert.ToDouble(arr[1]) * DPI / 25.4), (float)(Convert.ToDouble(arr[2]) * DPI / 25.4));
                    cadItem.Angle = Convert.ToDouble(arr[3]);
                    cadItem.Code = arr[4];
                    cadItem.PadsIndex = new List<int>();
                    cad.CadItems.Add(cadItem);
                    if(cadItem.Center.X < _XMin )
                    {
                        _XMin = cadItem.Center.X;
                    }
                    if (cadItem.Center.Y < _YMin)
                    {
                        _YMin = cadItem.Center.Y;
                    }
                    if (cadItem.Center.X > _XMax)
                    {
                        _XMax = cadItem.Center.X;
                    }
                    if (cadItem.Center.Y > _YMax)
                    {
                        _YMax = cadItem.Center.Y;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(item);
                    mLog.Error(ex.Message);
                    return null;
                }
            }
            // add undefine caditem
            CadItem cadItemUndefine = new CadItem();
            cadItemUndefine.CadFileID = cad.CadFileID;
            cadItemUndefine.Name = "UNDEFINE";
            cadItemUndefine.Center = new PointF(-9999, -9999);
            cadItemUndefine.Angle = 0;
            cadItemUndefine.Code = "UNDEFINE";
            cadItemUndefine.PadsIndex = new List<int>();
            cad.CadItems.Add(cadItemUndefine);
            // calculate rotate point
            for (int i = 0; i < cad.CadItems.Count; i++)
            {
                PointF ct = cad.CadItems[i].Center;
                cad.CadItems[i].Center = new PointF((float)(ct.X - _XMin), ct.Y);
            }
            _YMax -= _XMin;
            _XMax -= _XMin;
            _YMin -= _XMin;
            _XMin = 0; 
            cad.CenterRotation = new Point((int)(_XMax - _XMin) / 2, (int)(_YMax - _YMin) / 2);
            cad.X = GerberWidth / 2 - cad.CenterRotation.X;
            cad.Y = GerberHeight / 2 - cad.CenterRotation.Y;
            cad.CadFileData = content;
            return cad;
        }
        public Point GetCenterOfCenterSelected()
        {
            Point centerOfCenter = new Point();
            if (this.CadItemSelected.Count > 0)
            {
                
                long x = 0;
                long y = 0;
                for (int i = 0; i < this.CadItemSelected.Count; i++)
                {
                    Point ct = Point.Round(CadItemSelected[i].Center);
                    ct.X += this.X;
                    ct.Y += this.Y;
                    Point newCtRotate = ImageProcessingUtils.PointRotation(ct, new Point((int)this.CenterRotation.X + this.X, (int)this.CenterRotation.Y + this.Y), this.Angle * Math.PI / 180.0);
                    x += newCtRotate.X;
                    y += newCtRotate.Y;
                }
                x = x / this.CadItemSelected.Count;
                y = y / this.CadItemSelected.Count;
                centerOfCenter = new Point(Convert.ToInt32(x), Convert.ToInt32(y));
            }
            return centerOfCenter;
        }
        public int AddSelectCenter(Rectangle SelectRect)
        {
            List<CadItem> centerSelected = ItemInRect(SelectRect);
            for (int i = 0; i < centerSelected.Count; i++)
            {
                if(!this.CadItemSelected.Contains(centerSelected[i]))
                {
                    this.CadItemSelected.Add(centerSelected[i]);
                }
            }
            return centerSelected.Count;
        }
        public int RemoveSelectCenter(Rectangle SelectRect)
        {
            List<CadItem> centerSelected = ItemInRect(SelectRect);
            for (int i = 0; i < centerSelected.Count; i++)
            {
                if (this.CadItemSelected.Contains(centerSelected[i]))
                {
                    this.CadItemSelected.Remove(centerSelected[i]);
                }
            }
            return centerSelected.Count;
        }
        public void ClearSelectCenter()
        {
            this.CadItemSelected.Clear();
        }
        public List<CadItem> ItemInRect(Rectangle SelectRect)
        {
            List<CadItem> centerSelected = new List<CadItem>();
            if(SelectRect != Rectangle.Empty)
            {
                for (int i = 0; i < CadItems.Count; i++)
                {
                    Point ct = Point.Round(CadItems[i].Center);
                    ct.X += this.X;
                    ct.Y += this.Y;
                    Point newCtRotate = ImageProcessingUtils.PointRotation(ct, new Point((int)this.CenterRotation.X + this.X, (int)this.CenterRotation.Y + this.Y), this.Angle * Math.PI / 180.0);
                    if (SelectRect.Contains(newCtRotate))
                    {
                        centerSelected.Add(CadItems[i]);
                    }
                }
            }
            return centerSelected;
        }
        public CadFile Copy()
        {
            CadFile cad = new CadFile();
            cad.ModelID = this.ModelID;
            cad.CadFileID = Utils.GetNewID();
            cad.FileName = this.FileName;
            cad.FilePath = this.FilePath;
            cad.Angle = this.Angle;
            cad.Color = this.Color;
            cad.CenterRotation = new Point(this.CenterRotation.X, CenterRotation.Y);
            cad.Visible = true;
            cad.X = this.X;
            cad.Y = this.Y;
            CadFileData = this.CadFileData;
            cad.CadItems = new List<CadItem>();
            for (int i = 0; i < this.CadItems.Count; i++)
            {
                cad.CadItems.Add(this.CadItems[i].Copy(cad.CadFileID));
            }
            cad.CadItemSelected = new List<CadItem>();
            return cad;
        }
        public void ClearLinkPadItem()
        {
            for (int i = 0; i < this.CadItems.Count; i++)
            {
                this.CadItems[i].PadsIndex = new List<int>();
            }
        }
        public void FlipX()
        {
            if(this.CadItems.Count > 0)
            {
                float min = 65535;
                float max = 0;
                for (int i = 0; i < this.CadItems.Count; i++)
                {
                    if(this.CadItems[i].Name != "UNDEFINE")
                    {
                        min = min > this.CadItems[i].Center.X ? this.CadItems[i].Center.X : min;
                        max = max < this.CadItems[i].Center.X ? this.CadItems[i].Center.X : max;
                    }
                }
                float valFlip = (min + max) / 2;
                for (int i = 0; i < this.CadItems.Count; i++)
                {
                    PointF ct = this.CadItems[i].Center;
                    this.CadItems[i].Center = new PointF(ct.X + 2 * (valFlip - ct.X), ct.Y);
                }
            }
        }
        public void FlipY()
        {
            if (this.CadItems.Count > 0)
            {
                float min = 65535;
                float max = 0;
                for (int i = 0; i < this.CadItems.Count; i++)
                {
                    if (this.CadItems[i].Name != "UNDEFINE")
                    {
                        min = min > this.CadItems[i].Center.Y ? this.CadItems[i].Center.Y : min;
                        max = max < this.CadItems[i].Center.Y ? this.CadItems[i].Center.Y : max;
                    }
                }
                float valFlip = (min + max) / 2;
                for (int i = 0; i < this.CadItems.Count; i++)
                {
                    PointF ct = this.CadItems[i].Center;
                    this.CadItems[i].Center = new PointF(ct.X , ct.Y + 2 * (valFlip - ct.Y));
                }
            }
        }
    }
}