using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SPI_AOI.Utils
{
    class MyState
    {
        public static string GetStringStatus(Utils.LabelStatus mode)
        {
            switch (mode)
            {
                case Utils.LabelStatus.PASS:
                    return "PASS";
                case Utils.LabelStatus.FAIL:
                    return "FAIL";
                case Utils.LabelStatus.GOOD:
                    return "GOOD";
                case Utils.LabelStatus.OK:
                    return "OK";
                case Utils.LabelStatus.CLOSED:
                    return "CLOSED";
                case Utils.LabelStatus.OPEN:
                    return "OPEN";
                case Utils.LabelStatus.RUNNING:
                    return "RUNNING";
                case Utils.LabelStatus.CONTROL_RUN:
                    return "CONTROL RUN";
                case Utils.LabelStatus.IDLE:
                    return "IDLE";
                case Utils.LabelStatus.READY:
                    return "READY";
                case Utils.LabelStatus.BY_PASS:
                    return "BYPASS";
                case Utils.LabelStatus.WARNING:
                    return "WARNING";
                case Utils.LabelStatus.WAITTING:
                    return "WAITTING";
                case Utils.LabelStatus.PROCESSING:
                    return "PROCESSING";
                case Utils.LabelStatus.ERROR:
                    return "ERROR";
                case Utils.LabelStatus.TEST:
                    return "TESTING";
                case Utils.LabelStatus.STOP:
                    return "STOPPED";
                default:
                    return "NOT DEFINE";
            }
        }
        public static SolidColorBrush GetColorStatus(Utils.LabelStatus mode)
        {
            switch (mode)
            {
                case Utils.LabelStatus.PASS:
                    return Brushes.Green;
                case Utils.LabelStatus.FAIL:
                    return Brushes.Red;
                case Utils.LabelStatus.GOOD:
                    return Brushes.Green;
                case Utils.LabelStatus.OK:
                    return Brushes.Green;
                case Utils.LabelStatus.CLOSED:
                    return Brushes.Green;
                case Utils.LabelStatus.OPEN:
                    return Brushes.Orange;
                case Utils.LabelStatus.RUNNING:
                    return Brushes.YellowGreen;
                case Utils.LabelStatus.CONTROL_RUN:
                    return Brushes.Green;
                case Utils.LabelStatus.IDLE:
                    return Brushes.Gray;
                case Utils.LabelStatus.READY:
                    return Brushes.DeepSkyBlue;
                case Utils.LabelStatus.WAITTING:
                    return Brushes.Orange;
                case Utils.LabelStatus.PROCESSING:
                    return Brushes.Orange;
                case Utils.LabelStatus.ERROR:
                    return Brushes.Red;
                case Utils.LabelStatus.TEST:
                    return Brushes.Orange;
                case Utils.LabelStatus.BY_PASS:
                    return Brushes.Green;
                case Utils.LabelStatus.WARNING:
                    return Brushes.Orange;
                case Utils.LabelStatus.STOP:
                    return Brushes.Red;
                default:
                    return Brushes.Green;
            }
        }
    }
}
