using System;
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
        public static ServiceResults Sendfile(string url, string[] files, NameValueCollection formFields = null, bool DontSendIfNotVI = true)
        {
            string resultPath = "Results";
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
            bool vi = false;
            foreach (string item in keys)
            {
                if (item.Contains("VI_"))
                {
                    string value = formFields.Get(item);
                    if (value.ToUpper() == "TRUE")
                    {
                        vi = true;
                    }
                }
            }
            try
            {
                if (!vi && DontSendIfNotVI)
                {
                    using (Image<Gray, byte> ImgSrc = new Image<Gray, byte>(files[0]))
                    {
                        result = new ServiceResults();
                        result.ImgMask = new Image<Gray, byte>(ImgSrc.Size);
                        result.QRCode = "NOT FOUND";
                        return result;
                    }
                }

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
                using (var response = request.GetResponse())
                {
                    Stream stream2 = response.GetResponseStream();
                    StreamReader reader2 = new StreamReader(stream2);
                    string data = reader2.ReadToEnd();
                    dynamic jsonObj = JsonConvert.DeserializeObject(data);
                    string status = jsonObj["status"];
                    if(status.ToUpper() == "OK")
                    {
                        string imageStr = jsonObj["image"];
                        string sn = jsonObj["sn"];
                        byte[] datamask = Convert.FromBase64String(imageStr);
                        string name = pathSave + string.Format("/mask_FOV{0}.png", sttFOV) ;
                        File.WriteAllBytes(name, datamask);
                        result = new ServiceResults();
                        result.ImgMask = new Image<Gray, byte>(name);
                        result.QRCode = sn;
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

    }
    class ServiceResults
    {
        public Image<Gray, byte> ImgMask { get; set; }
        public string QRCode { get; set; }
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
