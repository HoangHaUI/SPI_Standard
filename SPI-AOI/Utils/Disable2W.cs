using System;
using System.Linq;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using System.Diagnostics;


namespace Heal
{
    public class Disable2W
    {
        private static bool mIsRun = true;
        private static dynamic mMain = null;
        private static System.Timers.Timer mTimer = new System.Timers.Timer(500);
        public static bool Enable(dynamic s)
        {
            String thisprocessname = Process.GetCurrentProcess().ProcessName;

            if (Process.GetProcesses().Count(p => p.ProcessName == thisprocessname) > 1)
            {
                Show();
                return false;
            }
            else
            {
                mMain = s;
                Thread ser = new Thread(() => { Server(); });
                ser.Start();
                mTimer.Elapsed += OntimedEvent;
                mTimer.Enabled = true;
                return true;
            }
           
        }
        public static void OntimedEvent(object s , System.Timers.ElapsedEventArgs e)
        {
            try
            {
                mIsRun = mMain.IsHandleCreated;
            }
            catch
            {}
            if(!mIsRun)
            {
                Exit();
            }
        }
        public static void Disable()
        {
            mIsRun = false;
            Exit();
        }
        private static void Show()
        {
            try
            {
                using (TcpClient client = new TcpClient("127.0.0.1", 5721))
                {
                    using (NetworkStream stream = client.GetStream())
                    {
                        stream.WriteTimeout = 1000;
                        stream.ReadTimeout = 3000;
                        byte[] sendData = new byte[] { (byte)'A' };
                        stream.Write(sendData, 0, sendData.Length);
                    }
                }
            }
            catch
            {
            }
        }
        private static void Exit()
        {
            try
            {
                using (TcpClient client = new TcpClient("127.0.0.1", 5721))
                {
                    using (NetworkStream stream = client.GetStream())
                    {
                        stream.WriteTimeout = 1000;
                        stream.ReadTimeout = 3000;
                        byte[] sendData = new byte[] { (byte)'X' };
                        stream.Write(sendData, 0, sendData.Length);
                    }
                }
            }
            catch
            {
            }
        }
        private static void Server()
        {
            TcpListener server = null;
            try
            {
                // Set the TcpListener on port 13000.
                Int32 port = 5721;
                IPAddress localAddr = IPAddress.Parse("0.0.0.0");
                server = new TcpListener(localAddr, port);
                server.Start();
                Byte[] bytes = new Byte[256];
                String data = null;
                while (mIsRun)
                {
                    using (TcpClient client = server.AcceptTcpClient())
                    {
                        data = null;
                        using (NetworkStream stream = client.GetStream())
                        {
                            int i;
                            while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                            {
                                data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                                data = data.ToUpper();
                                if(data == "X")
                                {
                                    mIsRun = false;
                                    break;
                                }
                                else  if(data == "A")
                                {
                                    mMain.Invoke(new Action(() =>
                                    {
                                        mMain.WindowState = FormWindowState.Normal;
                                    }));
                                }
                            }
                        }
                    }
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                server.Stop();
            }
        }
    }
}
