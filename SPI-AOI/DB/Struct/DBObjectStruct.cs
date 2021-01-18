using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPI_AOI.DB.Struct
{
    public class Results
    {
        public string ID { get; set; }
        public string ModelName { get; set; }
        public DateTime LoadTime { get; set; }
        public string VIResult { get; set; }
        public string RunningMode { get; set; }
        public string SN { get; set; }

    }
}
