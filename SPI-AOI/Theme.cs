using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SPI_AOI
{
    class Theme
    {
        public Brush BackGround { get
            {
                return Brushes.DimGray;
            }
        }
        public Brush ForeGround
        {
            get
            {
                return Brushes.White;
            }
        }
    }
}
