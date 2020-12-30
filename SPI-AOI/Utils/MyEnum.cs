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
        BY_PASS,
        WARNING,
        STOP
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
    public enum RunMode
    {
        START,
        STOP
    }
    public enum RunningMode
    {
        CONTROL_RUN,
        TESTING,
        BY_PASS
    }
    public enum VIStatus
    {
        PASS,
        FAIL,
        GOOD,
        BAD,
        NONE,

    }
}
