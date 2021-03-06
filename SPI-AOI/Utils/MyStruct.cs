﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System.Drawing;


namespace SPI_AOI.Utils
{
    class MachineAlarmResult
    {
        public string ErCode { get; set; }
        public string ErMessages { get; set; }
        public string ErSolution { get; set; }
        public bool Success { get; set; }
    }
    class SummaryInfo
    {
        public string Field { get; set; }
        public long Count { get; set; }
        public int PPM { get; set; }
        public double Rate { get; set; }
    }
    class FOVDisplayInfo
    {
        public double Witdh { get; set; }
        public double Height { get; set; }
        public System.Windows.Point[] StartPoint { get; set; }
    }
    public class MarkAdjust
    {
        public double Angle { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public ActionStatus Status { get; set; }
    }
    class PadErrorDetail
    {
        public string PanelID { get; set; }
        public DateTime LoadTime { get; set; }
        public string SN { get; set; }
        public string ModelName { get; set; }
        public string ImageCapturePath { get; set; }
        public string ImageSegmentPath { get; set; }
        public string ErrorType { get; set; }
        public string MachineResult { get; set; }
        public string ConfirmResult { get; set; }
        public string Component { get; set; }
        public Models.PadItem Pad { get; set; }
        public Rectangle ROIOnImage { get; set; }
        public Rectangle ROIOnGerber { get; set; }
        public Image<Bgr, byte> PadImage { get; set; }
        public Point[] CntPadReference { get; set; }
        public List<Point[]> ContoursPadSegment { get; set; }
        public Point Center { get; set; }
        public int FOVNo { get; set; }
        public double Area { get; set; }
        public double ShiftX { get; set; }
        public double ShiftY { get; set; }
        public double AreaStdHight { get; set; }
        public double AreaStdLow { get; set; }
        public double ShiftXStduM { get; set; }
        public double ShiftXStdArea { get; set; }
        public double ShiftYStduM { get; set; }
        public double ShiftYStdArea { get; set; }
        public void Dispose()
        {
            if(this.PadImage != null)
            {
                this.PadImage.Dispose();
                this.PadImage = null;
            }
        }
    }
    class PadSegmentInfo
    {
        public int FOVID { get; set; }
        public string ImageCapturePath { get; set; }
        public string ImageSegmentPath { get; set; }
        public Rectangle Bouding { get; set; }
        public double Area { get; set; }
        public  Point Center { get; set; }
        public Point[] Contours { get; set; }
        public Point CenterOnFOV { get; set; }
        public Point[] ContoursOnFOV { get; set; }
    }
    public class PadAdjustResult
    {
        public int X { get; set; }
        public int Y { get; set; }
    }
}
