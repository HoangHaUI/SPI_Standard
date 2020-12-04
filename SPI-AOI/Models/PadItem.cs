using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;


namespace SPI_AOI.Models
{
    class PadItem
    {
        public string ID { get; set; }
        public Rectangle Bouding { get; set; }
        public double ThreshWarning { get; set; }
        public double ThreshError { get; set; }
        public CadItem CadItem { get; set; }
    
    }
}
