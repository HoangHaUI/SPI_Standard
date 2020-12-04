using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace SPI_AOI.Models
{
    public class ImportedFile
    {
        public Color Color { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public bool Visible { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int R { get; set; }
        public static int W { get; set; }
        public static int H { get; set; }
        public ImportedFile(string Path, bool Visible)
        {

            this.FilePath = Path;
            this.FileName = System.IO.Path.GetFileName(Path);
            //var extension = System.IO.Path.GetExtension(Path);
            this.Visible = Visible;
            
        }



    }
}
