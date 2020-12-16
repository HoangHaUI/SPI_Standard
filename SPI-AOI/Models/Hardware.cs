using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace SPI_AOI.Models
{
    public class Hardware
    {
        public int[] LightIntensity { get; set; }
        public double Gain { get; set; }
        public double ExposureTime { get; set; }
        public Point MarkPosition { get; set; }
        public List<ReadCodePosition> ReadCodePosition { get; set; }
        public double Conveyor { get; set; }
        public Hardware()
        {
            this.LightIntensity = new int[4] { 127,127,127,127};
            this.Gain = 0;
            this.ExposureTime = 3000;
            this.MarkPosition = new Point();
            this.ReadCodePosition = new List<ReadCodePosition>();
            this.Conveyor = 0;
        }
    }
    public class ReadCodePosition
    {
        public Point Origin { get; set; }
        public string Surface { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
    }
}
