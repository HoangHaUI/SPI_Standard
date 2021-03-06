﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Drawing;
using System.IO;
using System.Collections.Specialized;
using NLog;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SPI_AOI.VI
{
    class ServiceComm
    {
        private static Logger mLog = Heal.LogCtl.GetInstance();
        private static Properties.Settings mParam = Properties.Settings.Default;
        public static ServiceResults SegmentFOV(string url, string[] files, int NoFOV, bool Debug)
        {
            int id = NoFOV;
            NameValueCollection data = new NameValueCollection();
            data.Add("Type", "Segment");
            data.Add("FOV", (id + 1).ToString());
            data.Add("Debug", Convert.ToString(Debug));
            return  VI.ServiceComm.Sendfile(url, files, data);
        }
        public static ServiceResults Decode(string url, string[] files, bool Debug)
        {
            NameValueCollection data = new NameValueCollection();
            data.Add("Type", "Decode");
            data.Add("FOV", "0");
            data.Add("Debug", Convert.ToString(Debug));
            return VI.ServiceComm.Sendfile(url, files, data);
        }
        public static ServiceResults Sendfile(string url, string[] files, NameValueCollection formFields = null)
        {
            string resultPath = "ServiceResults";
            string sttFOV = formFields.Get("FOV");
            if (!Directory.Exists(resultPath))
            {
                Directory.CreateDirectory(resultPath);
            }
            string pathSave = resultPath + "/" + formFields.Get("Type");
            if (!Directory.Exists(pathSave))
            {
                Directory.CreateDirectory(pathSave);
            }
            ServiceResults result = null;
            string[] keys = formFields.AllKeys;
            try
            {
                string boundary = "----------------------------" + DateTime.Now.Ticks.ToString("x");

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.ContentType = "multipart/form-data; boundary=" + boundary;
                request.Method = "POST";
                request.KeepAlive = true;

                Stream memStream = new System.IO.MemoryStream();

                var boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" +
                                                                        boundary + "\r\n");
                var endBoundaryBytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" +
                                                                            boundary + "--");


                string formdataTemplate = "\r\n--" + boundary +
                                            "\r\nContent-Disposition: form-data; name=\"{0}\";\r\n\r\n{1}";

                if (formFields != null)
                {
                    foreach (string key in formFields.Keys)
                    {
                        string formitem = string.Format(formdataTemplate, key, formFields[key]);
                        byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
                        memStream.Write(formitembytes, 0, formitembytes.Length);
                    }
                }

                string headerTemplate =
                    "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\n" +
                    "Content-Type: application/octet-stream\r\n\r\n";

                for (int i = 0; i < files.Length; i++)
                {
                    memStream.Write(boundarybytes, 0, boundarybytes.Length);
                    var header = string.Format(headerTemplate, "file", files[i]);
                    var headerbytes = System.Text.Encoding.UTF8.GetBytes(header);

                    memStream.Write(headerbytes, 0, headerbytes.Length);

                    using (var fileStream = new FileStream(files[i], FileMode.Open, FileAccess.Read))
                    {
                        var buffer = new byte[1024];
                        var bytesRead = 0;
                        while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                        {
                            memStream.Write(buffer, 0, bytesRead);
                        }
                    }
                }

                memStream.Write(endBoundaryBytes, 0, endBoundaryBytes.Length);
                request.ContentLength = memStream.Length;

                using (Stream requestStream = request.GetRequestStream())
                {
                    memStream.Position = 0;
                    byte[] tempBuffer = new byte[memStream.Length];
                    memStream.Read(tempBuffer, 0, tempBuffer.Length);
                    memStream.Close();
                    requestStream.Write(tempBuffer, 0, tempBuffer.Length);
                }
                request.Timeout = 3000;
                using (var response = request.GetResponse())
                {
                    Stream stream2 = response.GetResponseStream();
                    StreamReader reader2 = new StreamReader(stream2);
                    string data = reader2.ReadToEnd();
                    dynamic jsonObj = JsonConvert.DeserializeObject(data);
                    string status = jsonObj["status"];
                    if(status.ToUpper() == "OK")
                    {
                        if(formFields.Get("Type") == "Segment")
                        {
                            string imageStr = jsonObj["image"];
                            byte[] datamask = Convert.FromBase64String(imageStr);
                            string name = pathSave + string.Format("/mask_FOV{0}.png", sttFOV);
                            File.WriteAllBytes(name, datamask);
                            result = new ServiceResults();
                            result.ImgMask = new Image<Gray, byte>(name);
                        }
                        else if(formFields.Get("Type") == "Decode")
                        {
                            string code = jsonObj["sn"];
                            result = new ServiceResults();
                            result.SN = code;
                        }
                        else if (formFields.Get("Type") == "Test")
                        {
                            result = new ServiceResults();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                mLog.Error(ex.Message);
                return null;
            }
            return result;
        }
        public static byte[] ConvertToBytes(Image<Gray, byte> Img)
        {
            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(Img.Bitmap, typeof(byte[]));
        }
        public static Image<Gray, byte> ConvertToImage(Size size, byte[] data)
        {
            Image<Gray, byte> depthImage = new Image<Gray, byte>(size);
            depthImage.Bytes = data;
            return depthImage;
        }
        public static int TestService(string url)
        {
            NameValueCollection data = new NameValueCollection();
            data.Add("Type", "Test");
            data.Add("FOV", "0");
            data.Add("Debug", Convert.ToString(true));
            var result = VI.ServiceComm.Sendfile(url, new string[0], data);
            if(result != null)
            {
                return 0;
            }
            else
            {
                return -1;
            }
        }
        public static void StartService()
        {
            try
            {
                System.Diagnostics.Process p = new System.Diagnostics.Process();
                System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo();
                info.FileName = "cmd.exe";
                info.RedirectStandardInput = true;
                info.UseShellExecute = false;
                p.StartInfo = info;
                p.Start();
                using (StreamWriter sw = p.StandardInput)
                {
                    if (sw.BaseStream.CanWrite)
                    {
                        sw.WriteLine("cd Pulish");
                        sw.WriteLine("services.vbs");
                    }
                }
            }
            catch (Exception ex)
            {
                mLog.Error(ex.Message);
            }
        }

    }
    class ServiceResults
    {
        public Image<Gray, byte> ImgMask { get; set; }
        public string SN { get; set; }
        public void Dispose()
        {
            if (ImgMask != null)
            {
                ImgMask.Dispose();
                ImgMask = null;
            }
        }
    }
}
