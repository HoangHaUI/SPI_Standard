using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.IO;
using NLog;
using System.Threading;



namespace SPI_AOI.Devices
{
    class MyScaner
    {
        private static SerialPort mScanPort = null;
        private static string mCMDRead = "DE\n";
        private static string mCMDEnd = "E\n";
        private static MyScaner mScan = null;
        private static Logger mLog = Heal.LogCtl.GetInstance();
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
                mScanPort.ReadTimeout = 5000;
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
        public string ReadCode()
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
            try
            {
                for (int i = 0; i < 3; i++)
                {
                    string data = mScanPort.ReadTo("\r");
                    if (!string.IsNullOrEmpty(data))
                    {
                        sn = data;
                        break;
                    }
                    mScanPort.Write(mCMDRead);
                }
             }
            catch (Exception ex)
            {
                mLog.Error(ex.Message);
                return sn;
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
