using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPI_AOI.Utils
{
    enum LabelStatus
    {
        PASS,
        FAIL,
        GOOD,
        OK,
        CLOSED,
        OPEN,
        RUNNING,
        CONTROL_RUN,
        IDLE,
        READY,
        WAITTING,
        PROCESSING,
        ERROR,
        TEST,

    }
    enum LabelMode
    {
        PLC,
        DOOR,
        RUNNING_MODE,
        MACHINE_STATUS,
        PRODUCT_STATUS
    }
    public enum AutoLinkMode
    {
        RnC,
        TwoPad,
        All,
        NotLink
    }
}
