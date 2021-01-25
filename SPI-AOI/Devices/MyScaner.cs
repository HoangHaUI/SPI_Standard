using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.IO;
using NLog;
using System.Threading;
using System.Drawing;



namespace SPI_AOI.Devices
{
    class MyScaner
    {
        private static SerialPort mScanPort = null;
        private static string mCMDRead = "DE";
        private static MyScaner mScan = null;
        private static Logger mLog = Heal.LogCtl.GetInstance();
        private static PLCComm mPLCComm = new PLCComm();
        public static MyScaner GetInstance()
        {
            if(mScan == null)
            {
                mScan = new MyScaner();
            }
            return mScan;
        }
        public int Open(string Comport)
        {
            if(mScanPort == null)
            {
                mScanPort = new SerialPort();
            }
            else
            {
                if(mScanPort.IsOpen)
                {
                    mScanPort.Close();
                }
            }
            mScanPort.PortName = Comport;
            try
            {
                mScanPort.Open();
                mScanPort.ReadTimeout = 500;
                return 0;
            }
            catch (Exception ex)
            {
                mLog.Error(ex.Message);
                return -1;
            }
        }
        public int Close()
        {
            if (mScanPort != null)
            {
                if (mScanPort.IsOpen)
                {
                    mScanPort.Close();
                    
                }
            }
            return 0;
        }
        public string ReadCode(Point XYReadCode, bool MoveAxis = true)
        {
            string sn = "NOT FOUND";
            if(mScanPort == null)
            {
                
                return sn;
            }
            if(!mScanPort.IsOpen)
            {
                try
                {
                    mScanPort.Open();
                }
                catch (Exception ex)
                {
                    mLog.Error(ex.Message);
                    return sn;
                }
            }
            for (int i = -1; i < 2; i++)
            {
                bool breakFor = false;
                
                for (int j = -1; j < 2; j++)
                {
                    string data = null;
                    try
                    {
                        data = mScanPort.ReadTo("\r");
                    }
                    catch { }
                    if (!string.IsNullOrEmpty(data))
                    {
                        sn = data;
                        breakFor = true;
                        break;
                    }
                    mScanPort.Write(mCMDRead);
                    if (MoveAxis)
                    {
                        int move = 2000;
                        int x = XYReadCode.X + move * i;
                        int y = XYReadCode.Y + move * j;
                        VI.MoveXYAxis.ReadCodeBot(mPLCComm, new Point(x, y));
                    }
                }
                if (breakFor)
                    break;

                //Thread.Sleep(200);
            }
            return sn;
        }
        public void ReleaseBuffer()
        {
            if(mScanPort != null)
            {
                if(mScanPort.IsOpen)
                {
                    mScanPort.DiscardInBuffer();
                    mScanPort.DiscardOutBuffer();
                }
            }
        }
    }
}
