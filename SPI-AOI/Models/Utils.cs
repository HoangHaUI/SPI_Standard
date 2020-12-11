using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPI_AOI.Models
{
    public class Utils
    {
        public static string GetNewID()
        {
            return Guid.NewGuid().ToString().ToUpper();
        }
    }
}
