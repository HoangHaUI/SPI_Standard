using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Emgu.CV;
using System.Drawing;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;


namespace SPI_AOI
{
    class CalibrateLoader
    {
        private static Properties.Settings mParam = new Properties.Settings();
        private static CalibrateInfo mCalibrate = null;
        public static CalibrateInfo GetIntance()
        {
            if(mCalibrate == null)
            {
                mCalibrate = UpdateInstance();
            }
            return mCalibrate;
        }
        public static CalibrateInfo UpdateInstance()
        {
            return CalibrateLoader.GetCalibrationInfo(mParam.CAMERA_MATRIX_FILE,
                mParam.CAMERA_DISTCOEFFS_FILE,
                mParam.IMAGE_SIZE,
               new System.Drawing.Rectangle(0, 0, mParam.IMAGE_SIZE.Width, mParam.IMAGE_SIZE.Height));
        }
        public static CalibrateInfo GetCalibrationInfo (string MatrixPath, string DistCoeffsPath, Size SizePattern, Rectangle ROI)
        {
            CalibrateInfo info = new CalibrateInfo();
            Rectangle roi = ROI;
            info.CameraMatrix = new Matrix<double>(3, 3);
            info.DistCoeffs = new Matrix<double>(8, 1);
            FileInfo fi = new FileInfo(MatrixPath);
            string[] matrixStr = File.ReadAllLines(fi.FullName);
            fi = new FileInfo(DistCoeffsPath);
            string disStr = File.ReadAllText(fi.FullName);
            for (int i = 0; i < matrixStr.Length; i++)
            {
                string[] val = matrixStr[i].Split(' ');
                for (int j = 0; j < val.Length; j++)
                {
                    info.CameraMatrix[i, j] = Convert.ToDouble(val[j]);
                }

            }
            string[] disval = disStr.Split(' ');
            for (int j = 0; j < disval.Length; j++)
            {
                info.DistCoeffs[j, 0] = Convert.ToDouble(disval[j]);
            }
            info.NewCameraMatrix = CvInvoke.GetOptimalNewCameraMatrix(info.CameraMatrix, info.DistCoeffs, SizePattern, 1, SizePattern, ref roi);
            info.ROI = roi;
            return info;
        }
    }
    class CalibrateInfo
    {
        public Matrix<double> CameraMatrix { get; set; }
        public Matrix<double> DistCoeffs { get; set; }
        public Mat NewCameraMatrix { get; set; }
        public Rectangle ROI { get; set; }
    }
}
