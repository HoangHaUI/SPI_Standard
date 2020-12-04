using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Drawing;

namespace SPI_AOI.Models
{
    class CadItem
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public double Angle { get; set; }
        public Point Center { get; set; }
        public string Code { get; set; }
        public List<PadItem> Pads { get; set; }
    }
}
