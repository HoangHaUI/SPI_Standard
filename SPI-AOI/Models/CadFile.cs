using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace SPI_AOI.Models
{
    class CadFile
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public bool Visible { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int R { get; set; }
        public List<CadItem> Items { get; set; }
    }
}
