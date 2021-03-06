﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using SPI_AOI.Devices;
using IOT;
using NLog;
using System.Threading;
using Emgu.CV;
using Emgu.CV.Structure;

namespace SPI_AOI.VI
{
    class MoveXYAxis
    {
        private static Logger mLog = Heal.LogCtl.GetInstance();
        private static CalibrateInfo mCalibImage = CalibrateLoader.GetIntance();
        public static Image<Bgr, byte> CaptureFOV(PLCComm PLC, HikCamera Camera, DKZ224V4ACCom LightCtl, Point Anchor, bool ActiveLight, int TimeSleep = 200, bool setup = false)
        {
            Image<Bgr, byte> img = null;
            bool ret = PLC.SetXYTop(Anchor.X, Anchor.Y);
            if (!ret)
            {
                return null;
            }
            if(setup)
            {
                PLC.Set_Write_Coordinates_Finish_Setup_Top();
                ret = PLC.GoFinishTopSetup();
            }
            else
            {
                PLC.Set_Write_Coordinates_Finish_Top();
                ret = PLC.GoFinishTop();
            }
            if (!ret)
            {
                return null;
            }
            if(setup)
            {
                PLC.Reset_Go_Coordinates_Finish_Setup_Top();
            }
            else
            {

                PLC.Reset_Go_Coordinates_Finish_Top();
            }
            if (ActiveLight)
            {
                LightCtl.ActiveFour(1, 1, 1, 1);
                Thread.Sleep(100);
            }
            Thread.Sleep(TimeSleep);
            Bitmap bm = Camera.GetOneBitmap(1000);
            if (ActiveLight)
            {
                LightCtl.ActiveFour(0, 0, 0, 0);
            }
            if(bm != null)
            {
                using (Image<Bgr, byte> imgDis = new Image<Bgr, byte>(bm))
                {
                    img = new Image<Bgr, byte>(imgDis.Size);
                    CvInvoke.Undistort(imgDis, img, mCalibImage.CameraMatrix, mCalibImage.DistCoeffs, mCalibImage.NewCameraMatrix);
                }
            }
            return img;
        }
        public static int ReadCodeBot(PLCComm PLC, Point Anchor, int TimeSleep = 200)
        {
            bool ret = PLC.SetXYBot(Anchor.X, Anchor.Y);
            if (!ret)
            {
                return -1;
            }
            PLC.Set_Write_Coordinates_Finish_Bot();
            ret = PLC.GoFinishBot();
            if (!ret)
            {
                return -1;
            }
            PLC.Reset_Go_Coordinates_Finish_Bot();
            return 0;
        }
    }
}
