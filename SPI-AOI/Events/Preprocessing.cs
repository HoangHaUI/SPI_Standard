using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using SPI_AOI.Models;

namespace SPI_AOI.Events
{
    class Preprocessing
    {
        private static DB.Result mMyDBResult = new DB.Result();
        private static CalibrateInfo mCalibImage = CalibrateLoader.GetIntance();
        private static Properties.Settings mParam = Properties.Settings.Default;
        public static void PreCapture(Model mModel, string ID, string SavePath, int Index, Image<Bgr, byte> Img, Image<Gray, byte> ImgGerber, Rectangle ROI, Rectangle ROIGerber )
        {
            lock(mModel)
            {

            }
            using (Image<Bgr, byte> imgRotated = ImageProcessingUtils.ImageRotation(Img, new Point(Img.Width / 2, Img.Height / 2), -mModel.AngleAxisCamera * Math.PI / 180.0))
            using (Image<Bgr, byte> imgUndis = new Image<Bgr, byte>(imgRotated.Size))
            {
                CvInvoke.Undistort(imgRotated, imgUndis, mCalibImage.CameraMatrix, mCalibImage.DistCoeffs, mCalibImage.NewCameraMatrix);
                imgUndis.ROI = ROI;
                string fileName = string.Format("{0}//Image_{1}_ROI({2}_{3}_{4}_{5})_ROI_GERBER({6}_{7}_{8},{9}).png",
                    SavePath, Index + 1, ROI.X, ROI.Y, ROI.Width, ROI.Height,
                    ROIGerber.X, ROIGerber.Y, ROIGerber.Width, ROIGerber.Height);
                CvInvoke.Imwrite(fileName, imgUndis);
            }
        }
    }
}
