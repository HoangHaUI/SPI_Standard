using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

namespace SPI_AOI.Devices
{
    class PLCComm : MyPLC
    {
        public bool SetXYTop(int X, int Y)
        {
            int setX = 0;
            int setY = 0;
            // set x
            for (int m = 0; m < 5; m++)
            {
                setX = Set_X_Top(X);
                if (setX == X)
                    break;
                Thread.Sleep(3);
            }
            if (setX != X)
            {
                return false;
            }
            for (int m = 0; m < 5; m++)
            {
                setY = Set_Y_Top(Y);
                if (setY == Y)
                    break;
                Thread.Sleep(3);
            }
            if (setY != Y)
            {
                return false;
            }
            return true;
        }
        public bool GoFinishTop()
        {
            int val = 0;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            while (true)
            {
                val = Get_Go_Coordinates_Finish_Top();
                if (val == 1 || sw.ElapsedMilliseconds == 5000)
                    break;
                Thread.Sleep(3);
            }
            if (val != 1)
            {
                return false;
            }
            return true;
        }
    }
}
