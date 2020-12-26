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


namespace Heal
{
    class CalibrateLoader
    {
        public static CalibrateInfo GetCalibrationInfo (string MatrixPath, string DistCoeffsPath, Size SizePattern, Rectangle ROI)
        {
            CalibrateInfo info = new CalibrateInfo();
            Rectangle roi = ROI;
            info.CameraMatrix = new Matrix<double>(3, 3);
            info.DistCoeffs = new Matrix<double>(8, 1);
            string[] matrixStr = File.ReadAllLines(MatrixPath);
            string disStr = File.ReadAllText(DistCoeffsPath);
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
