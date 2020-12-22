using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using NLog;
using System.Threading;
using System.Runtime.InteropServices;


namespace Heal
{
    
    class DKZ224V4ACCom
    {
        private Logger mLog = Heal.LogCtl.GetInstance();
        public SerialPort Serial { get; set; }
        public DKZ224V4ACCom(string PortName)
        {
            Serial = new SerialPort();
            Serial.PortName = PortName;
            Serial.BaudRate = 19200;
        }
        public int Open()
        {
            if (!Serial.IsOpen)
            {
                try
                {
                    Serial.WriteTimeout = 2000;
                    Serial.ReadTimeout = 2000;
                    Serial.Open();

                }
                catch
                {
                    return -1;
                }
                return 0;
            }
            return 0;
        }
        public int Close()
        {
            if (Serial.IsOpen)
            {
                try
                {
                    Serial.Close();
                }
                catch
                {
                    return -1;
                }
                return 0;
            }
            return 0;
        }
        public int ActiveOne(int Channel, int Mode)
        {
            byte[] data = new byte[] { 0xab, 0xba, 0x03, 0x32, 0x00, 0x00 };
            data[4] = Convert.ToByte(Channel - 1);
            data[5] = Convert.ToByte(Mode);
            bool succ = false;
            try
            {
                Serial.Write(data, 0, data.Length);
                Thread.Sleep(10);
                int response = Serial.ReadByte();
                if (response == 0x4f || response == 0x6f)
                    succ = true;
                Serial.DiscardInBuffer();
                Serial.DiscardOutBuffer();
            }
            catch
            {
                return -1;
            }
            if (succ)
                return 0;
            else
                return -2;
        }
        public int SetOne(int Channel, int Value)
        {
            byte[] data = new byte[] { 0xab, 0xba, 0x03, 0x31, 0x00, 0x00 };
            data[4] = Convert.ToByte(Channel - 1);
            data[5] = Convert.ToByte(Value);
            bool succ = false;
            try
            {
                Serial.Write(data, 0, data.Length);
                Thread.Sleep(10);
                int response = Serial.ReadByte();
                if (response == 0x4f || response == 0x6f)
                    succ = true;
                Serial.DiscardInBuffer();
                Serial.DiscardOutBuffer();
            }
            catch (Exception ex)
            {
                mLog.Error(ex.Message);
                return -1;
            }
            if (succ)
                return 0;
            else
                return -2;
        }
        public int SetFour(int Value1, int Value2, int Value3, int Value4)
        {
            byte[] data = new byte[] { 0xab, 0xba, 0x05, 0x33, 0x00, 0x00, 0x00, 0x00 };
            data[4] = Convert.ToByte(Value1);
            data[5] = Convert.ToByte(Value2);
            data[6] = Convert.ToByte(Value3);
            data[7] = Convert.ToByte(Value4);
            bool succ = false;
            try
            {
                Serial.Write(data, 0, data.Length);
                Thread.Sleep(10);
                int response = Serial.ReadByte();
                if (response == 0x4f || response == 0x6f)
                    succ = true;
                Serial.DiscardInBuffer();
                Serial.DiscardOutBuffer();
            }
            catch
            {
                return -1;
            }
            if (succ)
                return 0;
            else
                return -2;
        }
        public int ActiveFour(int Mode1, int Mode2, int Mode3, int Mode4)
        {
            byte[] data = new byte[] { 0xab, 0xba, 0x05, 0x34, 0x00, 0x00, 0x00, 0x00 };
            data[4] = Convert.ToByte(Mode1);
            data[5] = Convert.ToByte(Mode2);
            data[6] = Convert.ToByte(Mode3);
            data[7] = Convert.ToByte(Mode4);
            bool succ = false;
            try
            {
                Serial.Write(data, 0, data.Length);
                Thread.Sleep(10);
                int response = Serial.ReadByte();
                if (response == 0x4f || response == 0x6f)
                    succ = true;
                Serial.DiscardInBuffer();
                Serial.DiscardOutBuffer();
            }
            catch
            {
                return -1;
            }
            if (succ)
                return 0;
            else
                return -2;
        }
        public int[] GetStatus()
        {
            byte[] data = new byte[] { 0xab, 0xba, 0x02, 0x02, 0x0d };
            bool succ = false;
            int[] result = new int[8];
            try
            {
                Serial.Write(data, 0, data.Length);
                Thread.Sleep(10);
                byte[] response = new byte[12];
                Serial.Read(response, 0, response.Length);
                for (int i = 0; i < 8; i++)
                {
                    result[i] = Convert.ToInt32(response[i + 4]);
                }
                succ = true;
            }
            catch
            {
                return null;
            }
            if (succ)
                return result;
            else
                return null;
        }

    }
    class DCPS1A8Com
    {
        private SerialPort mSerial;
        // Brightness adjustment
        private int mMinVal = 0;
        private int mMaxVal = 999;
        public DCPS1A8Com(string Comport, int Baudrate = 9600, int MinVal = 0, int MaxVal = 255)
        {
            mMinVal = MinVal;
            mMaxVal = MaxVal;
            mSerial = new SerialPort(Comport, Baudrate);
            mSerial.ReadTimeout = 5000;
            mSerial.WriteTimeout = 1000;
        }
        public int Open()
        {
            if (!mSerial.IsOpen)
            {
                try
                {
                    mSerial.Open();
                }
                catch
                {
                    return -1;
                }
            }
            return 0;
        }
        public int Close()
        {
            if (mSerial.IsOpen)
            {
                try
                {
                    mSerial.Close();
                }
                catch
                {
                    return -1;
                }
            }
            return 0;
        }
        public int SetBrightness(int Channel1 = 0xFFFFFFF, int Channel2 = 0xFFFFFFF, int Channel3 = 0xFFFFFFF, int Channel4 = 0xFFFFFFF,
                                  int Channel5 = 0xFFFFFFF, int Channel6 = 0xFFFFFFF, int Channel7 = 0xFFFFFFF, int Channel8 = 0xFFFFFFF)
        {
            int errorCode = 0;
            int[] brightness = new int[] {Channel1, Channel2, Channel3, Channel4, Channel5, Channel6, Channel7, Channel8};
            string cmd = string.Empty;
            int count = 0;
            for (int i = 0; i < brightness.Length; i++)
            {
                if(mMinVal <= brightness[i] && brightness[i] <= mMaxVal)
                {
                    if (cmd != string.Empty)
                        cmd += ",";
                    cmd += "LP" + Convert.ToString(i + 1) + "," + Convert.ToString(brightness[i]);
                    count++;
                }
            }
            if (cmd == string.Empty)
                return errorCode;
            cmd += ">";
            try
            {
                if(mSerial.IsOpen)
                {
                    mSerial.DiscardInBuffer();
                    mSerial.DiscardOutBuffer();
                    mSerial.Write(cmd);
                    for (int i = 0; i < count; i++)
                    {
                        Thread.Sleep(10);
                        string strRecevied = mSerial.ReadExisting();
                        if(strRecevied.ToUpper().Contains("E"))
                        {
                            errorCode = -1;
                        }
                    }
                    
                }
                else
                {
                    errorCode = -2;
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return errorCode;
        }
        public void SetWorkingMode(int Channel1 = 0xFFFFFFF, int Channel2 = 0xFFFFFFF, int Channel3 = 0xFFFFFFF, int Channel4 = 0xFFFFFFF,
                                  int Channel5 = 0xFFFFFFF, int Channel6 = 0xFFFFFFF, int Channel7 = 0xFFFFFFF, int Channel8 = 0xFFFFFFF)
        {
            int[] wm = new int[] { Channel1, Channel2, Channel3, Channel4, Channel5, Channel6, Channel7, Channel8 };
            string cmd = string.Empty;
            for (int i = 0; i < wm.Length; i++)
            {
                if (wm[i] == 0 || wm[i] == 1)
                {
                    if (cmd != string.Empty)
                        cmd += ",";
                    cmd += "WM" + Convert.ToString(i + 1) + "," + Convert.ToString(wm[i]);
                }
            }
            if (cmd == string.Empty)
                return;
            cmd += ">";
            try
            {
                mSerial.Write(cmd);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public void SetOnOff(int Channel1 = 0xFFFFFFF, int Channel2 = 0xFFFFFFF, int Channel3 = 0xFFFFFFF, int Channel4 = 0xFFFFFFF,
                                  int Channel5 = 0xFFFFFFF, int Channel6 = 0xFFFFFFF, int Channel7 = 0xFFFFFFF, int Channel8 = 0xFFFFFFF)
        {
            // open setting a luminance channels
            int[] luminance = new int[] { Channel1, Channel2, Channel3, Channel4, Channel5, Channel6, Channel7, Channel8 };
            string cmd = string.Empty;
            for (int i = 0; i < luminance.Length; i++)
            {
                if (luminance[i] == 0 || luminance[i] == 1)
                {
                    if (cmd != string.Empty)
                        cmd += ",";
                    cmd += "FC" + Convert.ToString(i + 1) + "," + Convert.ToString(luminance[i]);
                }
            }
            if (cmd == string.Empty)
                return;
            cmd += ">";
            try
            {
                if(mSerial.IsOpen)
                    mSerial.Write(cmd);
                else
                {

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public int[] GetBrightness()
        {
            // open setting a luminance channels
            int[] currentVal = new int[8];
            string cmd = "RS1,1,RS2,1,RS3,1,RS4,1,RS5,1,RS6,1,RS7,1,RS8,1>";
            try
            {
                mSerial.Write(cmd);
                System.Threading.Thread.Sleep(200);
                string str_recevied = mSerial.ReadExisting();
                string[] strSplit = str_recevied.Split(new char[] { ',', '>'});
                for (int i = 0; i < 8; i++)
                {
                    currentVal[i] = Convert.ToInt32(strSplit[i * 2 + 1]);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return currentVal;
        }
    }
}
