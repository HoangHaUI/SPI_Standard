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
                mScanPort.ReadTimeout = 1000;
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
        public string ReadCode(Point XYReadCode)
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
            for (int i = 0; i < 3; i++)
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
                    break;
                }
                mScanPort.Write(mCMDRead);
                int move = 5000;
                int x = XYReadCode.X > move ? XYReadCode.X - move : XYReadCode.X + move;
                int y = XYReadCode.Y > move ? XYReadCode.Y - move : XYReadCode.Y + move;
                VI.MoveXYAxis.ReadCodeBot(mPLCComm, new Point(x, y));
                VI.MoveXYAxis.ReadCodeBot(mPLCComm, XYReadCode);
                //Thread.Sleep(200);
            }
            return sn;
        }
        public void ReleaseBuffer()
        {
            mScanPort.DiscardInBuffer();
            mScanPort.DiscardOutBuffer();
        }
    }
}
