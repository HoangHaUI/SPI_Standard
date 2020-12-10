using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Drawing;

namespace SPI_AOI.Models
{
    public class CadItem
    {
        public string ID { get; set; }
        public int NoCadFile { get; set; }
        public string Name { get; set; }
        public double Angle { get; set; }
        public PointF Center { get; set; }
        public string Code { get; set; }
        public List<PadItem> Pads { get; set; }
        public CadItem Copy()
        {
            CadItem cadItem = new CadItem();
            cadItem.ID = this.ID;
            cadItem.Name = this.Name;
            cadItem.Angle = this.Angle;
            cadItem.Center = new PointF(this.Center.X, this.Center.Y);
            cadItem.Code = Code;
            cadItem.Pads = new List<PadItem>();
            return cadItem;
        }
        public static Point GetCenterRotated(Point Center, Point CenterRotate, int X, int Y, double Angle)
        {
            Center.X += X;
            Center.Y += Y;
            CenterRotate.X += X;
            CenterRotate.Y += Y;
            Point cadCenterRotated = ImageProcessingUtils.PointRotation(Center, CenterRotate, Angle * Math.PI / 180.0);
            return cadCenterRotated;
        }
    }
}
