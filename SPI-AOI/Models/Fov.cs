using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace SPI_AOI.Models
{
    class Fov
    {
        public string ID { get; set; }
        public int NO { get; set; }
        public Point Anchor { get; set; }
        List<PadItem> PadItems { get; set; }

    }
}
