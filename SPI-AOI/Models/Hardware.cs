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
        public int LightIntensity { get; set; }
        public double Gain { get; set; }
        public  double Gamma { get; set; }
        public double ExposureTime { get; set; }
        public Point MarkPosition { get; set; }
        public Point ReadCodePosition { get; set; }
        public Hardware()
        {
            this.LightIntensity = 255;
            this.Gain = 0;
            this.Gamma = 1;
            this.ExposureTime = 10000;
            MarkPosition = new Point();
        }
    }
}
