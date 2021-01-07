using System;
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
    class SummaryInfo
    {
        public string Field { get; set; }
        public int Count { get; set; }
        public int PPM { get; set; }
    }
    class ImageCaptureInfo
    {
        public int ID { get; set; }
        public Image<Bgr, byte> ImageSource { get; set; }
        public Image<Gray, byte> ImageSegment { get; set; }
        public Size ROI { get; set; }
        public Size FOV { get; set; }
        public double Angle { get; set; }
    }
    class MarkCaptureInfo
    {
        public int ID { get; set; }
        public Image<Bgr, byte> Image { get; set; }
        public Size ROI { get; set; }
        public Point MarkCenter { get; set; }
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
}
