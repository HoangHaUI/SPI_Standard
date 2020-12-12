using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IOT;

namespace SPI_AOI.Devices
{
    class MyCamera
    {
        private static HikCamera mCamera = null;
        private static string[] mListCameraName = null;
        public static string[] GetCameraNames()
        {
            mListCameraName = HikCamera.GetCameraNames();
            return mListCameraName;
        }
        public static HikCamera GetInstance()
        {
            GetCameraNames();
            if (mCamera == null)
            {
                if(mListCameraName != null)
                {
                    if(mListCameraName.Length > 0)
                    {
                        mCamera = new HikCamera(0);
                    }
                }
            }
            return mCamera;
        }
    }
}
