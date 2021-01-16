using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using ZXing;
using ZXing.Common;
using System.Drawing;

namespace SPI_AOI.VI
{
    class Decoder
    {
        public static string Decode(Image<Gray, byte> image)
        {
            return GetCode(image);
        }
        private static string GetCode(Image<Gray, byte> image)
        {
            string code = null;
            for (int i = -3; i < 15; i++)
            {
                using (Image<Gray, byte> imgCode = new Image<Gray, byte>(image.Size))
                {
                    double alpha = i * 0.1 + 1;
                    CvInvoke.ConvertScaleAbs(image, imgCode, alpha, 0);
                    CvInvoke.Threshold(imgCode, imgCode, 0, 255, Emgu.CV.CvEnum.ThresholdType.Otsu);
                    CvInvoke.Imshow("", imgCode);
                    CvInvoke.WaitKey(0);
                    code = ZxingDecode(imgCode.Bitmap);
                    if(code != null)
                    { break; }
                }
            }
            return code;
        }
        private static string ZxingDecode(Bitmap image)
        {
            string code = null;
            using (image)
            {
                LuminanceSource source = new BitmapLuminanceSource(image);
                BinaryBitmap bitmap = new BinaryBitmap(new HybridBinarizer(source));
                Result result = new MultiFormatReader().decode(bitmap);
                if (result != null)
                {
                    code = result.Text;
                }
            }
            return code;
        }
    }
}
