using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using Heal;
using NLog;
using System.Threading;

namespace SPI_AOI.Devices
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
}
