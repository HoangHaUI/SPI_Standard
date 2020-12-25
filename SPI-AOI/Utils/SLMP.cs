using System;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Threading;
using NLog;
namespace Heal
{
    class SLMP
    {
        System.Text.Encoding utf8 = System.Text.Encoding.UTF8;
        private int _port;
        private string _ip;
        private string _SubHeader;
        private string _Network;
        private string _Station;
        private string _Moduleio;
        private string _Multidrop;
        private string _reserved;
        private string _write;
        private string _read;
        private string _bit;
        private string _word;
        private string _Listsp = "MYZDLFVBSW";
        private static Object synLock = new Object();
        private static Logger mLog = Heal.LogCtl.GetInstance();

        public SLMP(string ip, int port)
        {
            this._ip = ip;
            this._port = port;
            this._SubHeader = "5000";
            this._Network = "00";
            this._Station = "FF";
            this._Moduleio = "03FF";
            this._Multidrop = "00";
            this._reserved = "0010";
            this._write = "1401";
            this._read = "0401";
            this._bit = "0001";
            this._word = "0000";
        }
        public int GetPing(string Ip = null)
        {
            Ping p = new Ping();
            PingReply r;
            
            string ip = null;
            if (Ip != null)
            {
                ip = Ip;
            }
            else
            {
                ip = _ip;
            }
            try
            {
                r = p.Send(ip);
            }
            catch
            {
                return -1;
            }
            if (r.Status == IPStatus.Success)
            {
                return 0;
            }
            else
            {
                return -1;
            }
        }
        private string SendCommand(string command)
        {
            lock (synLock)
            {
                try
                {
                    Thread.Sleep(10);
                    TcpClient client = new TcpClient();
                    if (client.ConnectAsync(_ip, _port).Wait(500))
                    {
                        client.SendTimeout = 500;
                        client.ReceiveTimeout = 500;
                        NetworkStream stream = client.GetStream();
                        byte[] commandBytes = System.Text.Encoding.UTF8.GetBytes(command);
                        stream.Write(commandBytes, 0, commandBytes.Length);
                        byte[] buff = new byte[1024];
                        stream.Read(buff, 0, 1024);
                        stream.Close();
                        client.Close();
                        string result = System.Text.Encoding.UTF8.GetString(buff);
                        int index = 0;
                        while (result.Substring(index, 1) != "\0")
                        {
                            index++;
                        }
                        result = result.Substring(0, index);
                        return result;
                    }
                    else
                        return "-1";

                }
                catch (Exception ex)
                {
                    mLog.Error(ex.Message);
                    return "-1";
                }
            }
        }
        public int SetDevice2(string Device, int value)
        {
            uint val = (uint)value;
            
            int valueL = Convert.ToUInt16((int) val & 0xffff);
            int valueH = Convert.ToUInt16((int) (val >> 16 & 0xffff));
            int i = 0;
            for (; i < Device.Length; i++)
            {
                if(Device.ToCharArray()[i] >= 0x30 && Device.ToCharArray()[i] <= 0x39)
                {
                    break;
                }
            }
            string ext = Device.Substring(0, i);
            int currentIndex = Convert.ToInt32(Device.Substring(i, Device.Length - i));
            int nextIndex = currentIndex + 1;
            string DeviceH = ext + nextIndex.ToString();
            string DeviceL = ext + currentIndex.ToString();
            SLMPResult sta = SetDevice(DeviceH, valueH);
            if (sta.Status == SLMPStatus.SUCCESSFULLY)
            {
                sta = SetDevice(DeviceL, valueL);
                if (sta.Status == SLMPStatus.SUCCESSFULLY)
                {
                    return 0;
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                return -1;
            }
        }
        public int GetDevice2(string Device)
        {
            int value = -1;
            int i = 0;
            for (; i < Device.Length; i++)
            {
                if (Device.ToCharArray()[i] >= 0x30 && Device.ToCharArray()[i] <= 0x39)
                {
                    break;
                }
            }
            string ext = Device.Substring(0, i);
            int currentIndex = Convert.ToInt32(Device.Substring(i, Device.Length - i));
            int nextIndex = currentIndex + 1;
            string DeviceH = ext + nextIndex.ToString();
            string DeviceL = ext + currentIndex.ToString();
            int valueL = 0;
            int valueH = 0;
            SLMPResult staH = GetDevice(DeviceH);
            SLMPResult staL = GetDevice(DeviceL);
            if(staH.Status == SLMPStatus.SUCCESSFULLY && staL.Status == SLMPStatus.SUCCESSFULLY)
            {
                valueH = staH.Value;
                valueL = staL.Value;
                valueH = valueH << 16;
                value = valueH | valueL;
            }
            return value;
        }
        public SLMPResult SetDevice(string device, int value)
        {
            SLMPResult slmpResult = new SLMPResult();
            slmpResult.Status = SLMPStatus.FAIL;
            slmpResult.Value = 0;
            string acommand = string.Empty;
            string ccommand = string.Empty;
            string bcommand = string.Empty;
            if (_Listsp.Contains(device.Substring(0, 1).ToUpper()))
            {
                acommand = _SubHeader + _Network + _Station + _Moduleio + _Multidrop;
                ccommand = "D000" + _Network + _Station + _Moduleio + _Multidrop;
                int length = 24;
                if ((device.Substring(0, 1).ToUpper() == "D") || (device.Substring(0, 1).ToUpper() == "W"))
                {
                    if ((value < 655535) && (value >= 0))
                    {
                        length += 4;
                        bcommand = acommand + length.ToString("X4") + _reserved + _write
                            + _word + device.Substring(0, 1).ToUpper() + (char)0x2A
                            + Convert.ToInt32(device.Substring(1).ToUpper()).ToString("D6")
                            + "0001" + value.ToString("X4");
                    }
                    else
                        return slmpResult;

                }
                else if (device.Substring(0, 2).ToUpper() == "SD")
                {
                    length += 4;
                    bcommand = acommand + length.ToString("X4") + _reserved + _write
                        + _word + device.Substring(0, 2).ToUpper()
                        + Convert.ToInt32(device.Substring(1).ToUpper()).ToString("D6")
                        + "0001" + value.ToString("X4");
                }
                else
                {
                    if ((value == 1) || (value == 0))
                    {
                        length += 1;
                        bcommand = acommand + length.ToString("X4") + _reserved + _write
                            + _bit + device.Substring(0, 1).ToUpper() + (char)0x2A
                            + Convert.ToInt32(device.Substring(1).ToUpper()).ToString("D6")
                            + "0001" + value.ToString("X1");
                    }
                    else
                        return slmpResult;
                }

                try
                {
                    string result;

                    result = SendCommand(bcommand);

                    if (result == "-1")
                        return slmpResult;
                    if (Convert.ToInt32(result.Substring(ccommand.Length + 4)) == 0)
                    {
                        slmpResult.Status = SLMPStatus.SUCCESSFULLY;
                        slmpResult.Value = 0;
                    }
                    else
                        return slmpResult;
                }
                catch (Exception ex)
                {
                    mLog.Error(ex.Message);
                    return slmpResult;
                }
            }
            return slmpResult;
        }

        public SLMPResult GetDevice(string device)
        {
            SLMPResult slmpResult = new SLMPResult();
            slmpResult.Status = SLMPStatus.FAIL;
            slmpResult.Value = 0;
            string acommand = string.Empty;
            string ccommand = string.Empty;
            string bcommand = string.Empty;
            if (_Listsp.Contains(device.Substring(0, 1).ToUpper()))
            {
                acommand = _SubHeader + _Network + _Station + _Moduleio + _Multidrop;
                ccommand = "D000" + _Network + _Station + _Moduleio + _Multidrop;
                int length = 24;
                if ((device.Substring(0, 1).ToUpper() == "D") || (device.Substring(0, 1).ToUpper() == "W"))
                {
                    bcommand = acommand + length.ToString("X4") + _reserved +
                        _read + _word + device.Substring(0, 1).ToUpper() + (char)0x2A
                        + Convert.ToInt32(device.Substring(1).ToUpper()).ToString("D6") + "0001";

                }
                else if ((device.Substring(0, 2).ToUpper() == "SD"))
                {

                    bcommand = acommand + length.ToString("X4") + _reserved +
                         _read + _word + device.Substring(0, 2).ToUpper()
                         + Convert.ToInt32(device.Substring(2).ToUpper()).ToString("D6") + "0001";
                }
                else
                {


                    bcommand = acommand + length.ToString("X4") + _reserved +
                       _read + _bit + device.Substring(0, 1).ToUpper() + (char)0x2A
                       + Convert.ToInt32(device.Substring(1).ToUpper()).ToString("D6") + "0001";
                }
                try
                {
                    int i = 0;
                    string result;
                    do
                    {
                        result = SendCommand(bcommand);
                        i++;
                    } while ((result == "-1") && (i < 3));

                    if (result == "-1")
                    {
                        slmpResult.Value = 0;
                        slmpResult.Status = SLMPStatus.TIME_OUT;
                    }
                    if (Convert.ToInt32(result.Substring(ccommand.Length + 4, 4), 16) == 0)
                    {
                        slmpResult.Value = Convert.ToInt32(result.Substring(ccommand.Length + 8), 16);
                        slmpResult.Status = SLMPStatus.SUCCESSFULLY;
                    }

                }
                catch (Exception ex)
                {
                    mLog.Error(ex.Message);
                    slmpResult.Value = 0;
                    slmpResult.Status = SLMPStatus.FAIL;
                }

            }

            return slmpResult;
        }

    }


    public class SLMPResult
    {
        public SLMPStatus Status { get; set; }
        public int Value { get; set; }
    }
    public enum SLMPStatus
    {
        SUCCESSFULLY,
        FAIL,
        TIME_OUT
    }

}
