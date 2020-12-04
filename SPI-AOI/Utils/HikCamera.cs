using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using MvCamCtrl.NET;
using System.Xml;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
namespace IOT
{ 
    /// <summary>
    ///  <para> HiKVision Camera API written by Heal (©2019 FII - IoT Team - 08/05/2020) </para>
    ///  <para> Update the OOP for this class </para>
    /// </summary>
    public class HikCamera
    {
        private MvCamCtrl.NET.MyCamera ptrCamera;
        private UInt32 m_nBufSizeForDriver = 3072 * 2048 * 3;
        private byte[] m_pBufForDriver = new byte[3072 * 2048 * 3];
        private UInt32 m_nBufSizeForSaveImage = 3072 * 2048 * 3 * 3 + 2048;
        private byte[] m_pBufForSaveImage = new byte[3072 * 2048 * 3 * 3 + 2048];
        private static InfoDevice[] mDeviceInfoList;
        private bool isGrabbing = false;
        private bool isRecording = false;
        private bool isOpen = false;
        private MvCamCtrl.NET.MyCamera.MV_CC_RECORD_PARAM mRecoder = new MvCamCtrl.NET.MyCamera.MV_CC_RECORD_PARAM();
        private string mCameraName { get; set; }
        public uint mImageOnBuffer { get; set; }
        private Size frameInfo = new Size();
        private static string[] ListCameraNames { get; set; }
        /// <summary>
        /// <para>en: Returns singleton object for SDK</para>
        /// <para>vi: Trả về đối tượng singleton cho SDK</para>
        /// </summary>
        #region "status camera"
        /// <summary>
        /// True if camera is openning, else false
        /// </summary>
        public bool IsOpen
        {
            get { return isOpen; }
        }
        /// <summary>
        /// True if camera is recording, else false
        /// </summary>
        public bool IsRecord
        {
            get { return IsRecord; }
        }
        /// <summary>
        /// True if camera is grabbing, else false
        /// </summary>
        public bool IsGrab
        {
            get { return isGrabbing; }
        }
        public Size ImageSize
        {
            get { return frameInfo; }
        }
        #endregion
        #region "Error list"
        public static string GetErrMessageByErrCode(int ErrorCode)
        {
            string errorMsg = string.Empty; ;
            switch (ErrorCode)
            {
                case 0:
                    errorMsg = "No Error!";
                    break;
                case -1:
                    errorMsg = "Fail to get the permission! The device is in use (0x80000203).";
                    break;
                case -2:
                    errorMsg = "List device is zero!";
                    break;
                case -3:
                    errorMsg = "List device is Null!";
                    break;
                case -50:
                    errorMsg = "Could not load configuration file!";
                    break;
                case -56:
                    errorMsg = "Could not set parameter!";
                    break;
                default:
                    errorMsg = "Not found error!";
                    break;
            }
            return errorMsg;
        }
        #endregion
        #region "List device and choose device"
        /// <summary>
        /// <para>en: Returns a string array that names all available HIK vision cameras on the current computer if available, null if none </para>
        /// <para>vi: Trả về 1 mảng string tên tất cả các camera HIK vision khả dụng trên máy tính hiện tại nếu có, null nếu không có</para>
        /// </summary>
        /// <returns>null</returns>
        public static string[] GetCameraNames()
        {
            uint nTLayerType = MvCamCtrl.NET.MyCamera.MV_GIGE_DEVICE | MvCamCtrl.NET.MyCamera.MV_USB_DEVICE;
            MvCamCtrl.NET.MyCamera.MV_CC_DEVICE_INFO_LIST stDeviceList = new MvCamCtrl.NET.MyCamera.MV_CC_DEVICE_INFO_LIST();
            
            int nRet = MvCamCtrl.NET.MyCamera.MV_CC_EnumDevices_NET(nTLayerType, ref stDeviceList);
            if (stDeviceList.nDeviceNum == 0)
            {
                ListCameraNames = null;
            }
            else
            {
                mDeviceInfoList = new InfoDevice[stDeviceList.nDeviceNum];
                ListCameraNames = new string[stDeviceList.nDeviceNum];
                for (int i=0;i< stDeviceList.nDeviceNum;i++)
                {
                    mDeviceInfoList[i].DeviceInfo = (MvCamCtrl.NET.MyCamera.MV_CC_DEVICE_INFO)Marshal.PtrToStructure(stDeviceList.pDeviceInfo[i], typeof(MvCamCtrl.NET.MyCamera.MV_CC_DEVICE_INFO));
                    if(mDeviceInfoList[i].DeviceInfo.nTLayerType == MvCamCtrl.NET.MyCamera.MV_GIGE_DEVICE)
                    {
                        IntPtr buffer = Marshal.UnsafeAddrOfPinnedArrayElement(mDeviceInfoList[i].DeviceInfo.SpecialInfo.stGigEInfo, 0);
                        MvCamCtrl.NET.MyCamera.MV_GIGE_DEVICE_INFO gigeInfo = (MvCamCtrl.NET.MyCamera.MV_GIGE_DEVICE_INFO)Marshal.PtrToStructure(buffer, typeof(MvCamCtrl.NET.MyCamera.MV_GIGE_DEVICE_INFO));
                        if (gigeInfo.chUserDefinedName != "")
                        {
                            ListCameraNames[i] = mDeviceInfoList[i].Name = ("GigE: " + gigeInfo.chUserDefinedName + " (" + gigeInfo.chSerialNumber + ")");
                        }
                        else
                        {
                            ListCameraNames[i] = mDeviceInfoList[i].Name = ("GigE: " + gigeInfo.chManufacturerName + " " + gigeInfo.chModelName + " (" + gigeInfo.chSerialNumber + ")");
                        }
                    }
                    else if (mDeviceInfoList[i].DeviceInfo.nTLayerType == MvCamCtrl.NET.MyCamera.MV_USB_DEVICE)
                    {
                        IntPtr buffer = Marshal.UnsafeAddrOfPinnedArrayElement(mDeviceInfoList[i].DeviceInfo.SpecialInfo.stUsb3VInfo, 0);
                        MvCamCtrl.NET.MyCamera.MV_USB3_DEVICE_INFO usbInfo = (MvCamCtrl.NET.MyCamera.MV_USB3_DEVICE_INFO)Marshal.PtrToStructure(buffer, typeof(MvCamCtrl.NET.MyCamera.MV_USB3_DEVICE_INFO));
                        if (usbInfo.chUserDefinedName != "")
                        {
                            ListCameraNames[i] = mDeviceInfoList[i].Name = ("USB3: " + usbInfo.chUserDefinedName + " (" + usbInfo.chSerialNumber + ")");
                        }
                        else
                        {
                            ListCameraNames[i] = mDeviceInfoList[i].Name = ("USB3: " + usbInfo.chManufacturerName + " " + usbInfo.chModelName + " (" + usbInfo.chSerialNumber + ")");
                        }
                    }
                }
            }
            return ListCameraNames;
        }
        private bool GetMacAddressbyName(string name,ref MvCamCtrl.NET.MyCamera.MV_CC_DEVICE_INFO deviceInfo)
        {
            bool response = false;
            foreach (InfoDevice item in mDeviceInfoList)
            {
                if (name == item.Name)
                {
                    deviceInfo = item.DeviceInfo;
                    response = true;
                }
            }
            return response;
        }
        #endregion
        /// <summary>
        /// <para>en: Open device (connect to device). Exclusive permission, for other Apps, only reading from CCP register is allowed.</para>
        /// <para>vi: Mở thiết bị (kết nối với thiết bị). Quyền độc quyền, đối với các Ứng dụng khác, chỉ đọc từ đăng ký của ĐCSTQ.</para>
        /// </summary>
        /// <param name="CameraName"></param>
        /// <param name="ImageOnBuffer"></param>
        /// <returns></returns>
        /// 
        public HikCamera(string CameraName, uint ImageOnBuffer = 1)
        {
            mCameraName = CameraName;
            mImageOnBuffer = ImageOnBuffer;
        }
        public HikCamera(int CameraIndex, uint ImageOnBuffer = 1)
        {
            if (ListCameraNames != null)
            {
                if (ListCameraNames.Length > 0 && CameraIndex < ListCameraNames.Length)
                {
                    mCameraName = ListCameraNames[CameraIndex];
                    mImageOnBuffer = ImageOnBuffer;
                }
            }
        }
        public int Open()
        {
            if (isOpen)
            {
                return 0;
            }
            if (ptrCamera == null)
                ptrCamera = new MvCamCtrl.NET.MyCamera();
            
            if (ListCameraNames != null)
            {
                if (ListCameraNames.Length > 0)
                {
                    return CreateCameraDevice(mCameraName, mImageOnBuffer);
                }
                else
                {
                    return -2;
                }
            }
            else
                return -3;
        }
        ///// <summary>
        ///// <para>en: Open device (connect to device). Exclusive permission, for other Apps, only reading from CCP register is allowed.</para>
        ///// <para>vi: Mở thiết bị (kết nối với thiết bị). Quyền độc quyền, đối với các Ứng dụng khác, chỉ đọc từ đăng ký của ĐCSTQ.</para>
        ///// </summary>
        ///// <param name="CameraIndex"></param>
        ///// <param name="ImageOnBuffer"></param>
        ///// <returns></returns>
        //public int Open(int CameraIndex, uint ImageOnBuffer = 1)
        //{
        //    if (ptrCamera == null)
        //        ptrCamera = new MvCamCtrl.NET.MyCamera();
        //    if (ListCameraNames != null)
        //    {
        //        if (ListCameraNames.Length > 0 && CameraIndex < ListCameraNames.Length)
        //        {
        //            return CreateCameraDevice(ListCameraNames[CameraIndex], ImageOnBuffer);
        //        }
        //        else
        //        {
        //            return -2;
        //        }
        //    }
        //    else
        //        return -3;
        //}
        /// <summary>
        /// nothing!
        /// </summary>
        /// <param name="CameraName"></param>
        /// <param name="ImageOnBuffer"></param>
        /// <returns></returns>
        private int CreateCameraDevice(string CameraName, uint ImageOnBuffer)
        {
            MvCamCtrl.NET.MyCamera.MV_CC_DEVICE_INFO stDevInfo = new MvCamCtrl.NET.MyCamera.MV_CC_DEVICE_INFO();
            bool resultGetNameCamera = GetMacAddressbyName(CameraName, ref stDevInfo);
            if (resultGetNameCamera)
            {
                int nRet = ptrCamera.MV_CC_CreateDevice_NET(ref stDevInfo);
                if (nRet != MvCamCtrl.NET.MyCamera.MV_OK)
                    return nRet;
                bool nAsscessMode = MvCamCtrl.NET.MyCamera.MV_CC_IsDeviceAccessible_NET(ref stDevInfo, MvCamCtrl.NET.MyCamera.MV_ACCESS_Exclusive);
                if (nAsscessMode == false)
                {
                    nRet = ptrCamera.MV_CC_DestroyDevice_NET();
                    return -1;
                }
                else
                {
                    nRet = ptrCamera.MV_CC_OpenDevice_NET(MvCamCtrl.NET.MyCamera.MV_ACCESS_Exclusive, 0);
                    if (nRet != MvCamCtrl.NET.MyCamera.MV_OK)
                    {
                        nRet = ptrCamera.MV_CC_DestroyDevice_NET();
                        return nRet;
                    }
                    else
                    {
                        //Detection network optimal package size(It only works for the GigE camera)
                        int nPacketSize = ptrCamera.MV_CC_GetOptimalPacketSize_NET();
                        ptrCamera.MV_CC_SetImageNodeNum_NET(ImageOnBuffer);
                        ptrCamera.MV_CC_SetIntValue_NET("GevSCPSPacketSize", (uint)nPacketSize);
                        ptrCamera.MV_CC_SetIntValue_NET("AcquisitionMode", 2);
                        MyCamera.MVCC_INTVALUE val = new MyCamera.MVCC_INTVALUE();
                        ptrCamera.MV_CC_GetIntValue_NET("Width", ref val);
                        frameInfo = new Size((int)val.nCurValue, 0);
                        ptrCamera.MV_CC_GetIntValue_NET("Height", ref val);
                        frameInfo = new Size(frameInfo.Width, (int)val.nCurValue);
                        isOpen = true;
                        return 0;
                    }

                }
            }
            else
            {
                return -1;
            }
        }
        /// <summary>
        /// <para>en: Close device. If succeeded, return 0; if failed, return Error Code.</para>
        /// <para>vi: Đóng thiết bị. Nếu thành công, trả về 0; nếu thất bại, trả về Mã lỗi.</para>
        /// </summary>
        /// <returns></returns>
        public int Close()
        {
            int nRet = 0;
            if (ptrCamera != null)
            {
                if (isRecording)
                {
                    nRet = StopRecording();
                    if (MvCamCtrl.NET.MyCamera.MV_OK != nRet)
                    {
                        return nRet;
                    }
                }
                if (isGrabbing)
                {
                    nRet = StopGrabbing();
                    if (MvCamCtrl.NET.MyCamera.MV_OK != nRet)
                    {
                        return nRet;
                    }
                }
                nRet = ptrCamera.MV_CC_CloseDevice_NET();
                if (MvCamCtrl.NET.MyCamera.MV_OK != nRet)
                {
                    return nRet;
                }

                nRet = ptrCamera.MV_CC_DestroyDevice_NET();
                if (MvCamCtrl.NET.MyCamera.MV_OK != nRet)
                {
                    return nRet;
                }
            }
            isOpen = false;
            return nRet;
        }
        /// <summary>
        /// <para>en: Display captured picture.If succeeded, return 0; if failed, return Error Code</para>
        /// <para>vn: Hiển thị ảnh đã chụp. Nếu thành công, trả về 0; nếu thất bại, trả về mã lỗi</para>
        /// </summary>
        /// <param name="ptr">Window handle</param>
        /// <returns></returns>
        public int ShowDisplay(IntPtr ptr)
        {
            if (ptrCamera != null && isGrabbing == true)
            {
                int nRet = ptrCamera.MV_CC_Display_NET(ptr);
                return nRet;
            }
            else
                // camera is not open or grabbing is not start
                return -1;
        }
        /// <summary>
        /// <para>en: Get one picture frame, supports getting chunk information and setting timeout. If succeeded then return Bitmap image; if failed then return null</para>
        /// <para>vi: Nhận một khung hình, hỗ trợ lấy thông tin chunk và đặt thời gian chờ. Nếu thành công thì trả về hình ảnh Bitmap; nếu thất bại thì trả về null</para>
        /// </summary>
        /// <param name="MilsecTimeout">Waiting timeout, int: millisecond</param>
        /// <returns></returns>
        public Bitmap GetOneBitmap(int MilsecTimeout)
        {
            int nRet;
            UInt32 nPayloadSize = 0;
            MyCamera.MVCC_INTVALUE stParam = new MyCamera.MVCC_INTVALUE();
            nRet = ptrCamera.MV_CC_GetIntValue_NET("PayloadSize", ref stParam);
            if (MyCamera.MV_OK != nRet)
            {
                return null;
            }
            nPayloadSize = stParam.nCurValue;
            if (nPayloadSize > m_nBufSizeForDriver)
            {
                m_nBufSizeForDriver = nPayloadSize;
                m_pBufForDriver = new byte[m_nBufSizeForDriver];

                // ch:同时对保存图像的缓存做大小判断处理 | en:Determine the buffer size to save image
                // ch:BMP图片大小：width * height * 3 + 2048(预留BMP头大小) | en:BMP image size: width * height * 3 + 2048 (Reserved for BMP header)
                m_nBufSizeForSaveImage = m_nBufSizeForDriver * 3 + 2048;
                m_pBufForSaveImage = new byte[m_nBufSizeForSaveImage];
            }

            IntPtr pData = Marshal.UnsafeAddrOfPinnedArrayElement(m_pBufForDriver, 0);
            MyCamera.MV_FRAME_OUT_INFO_EX stFrameInfo = new MyCamera.MV_FRAME_OUT_INFO_EX();
            // ch:超时获取一帧，超时时间为1秒 | en:Get one frame timeout, timeout is 1 sec
            nRet = ptrCamera.MV_CC_GetOneFrameTimeout_NET(pData, m_nBufSizeForDriver, ref stFrameInfo, 1000);
            if (MyCamera.MV_OK != nRet)
            {
                return null;
            }

            MyCamera.MvGvspPixelType enDstPixelType;
            if (IsMonoData(stFrameInfo.enPixelType))
            {
                enDstPixelType = MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono8;
            }
            else if (IsColorData(stFrameInfo.enPixelType))
            {
                enDstPixelType = MyCamera.MvGvspPixelType.PixelType_Gvsp_RGB8_Packed;
            }
            else
            {
                return null;
            }
            IntPtr pImage = Marshal.UnsafeAddrOfPinnedArrayElement(m_pBufForSaveImage, 0);
            MyCamera.MV_PIXEL_CONVERT_PARAM stConverPixelParam = new MyCamera.MV_PIXEL_CONVERT_PARAM();
            stConverPixelParam.nWidth = stFrameInfo.nWidth;
            stConverPixelParam.nHeight = stFrameInfo.nHeight;
            stConverPixelParam.pSrcData = pData;
            stConverPixelParam.nSrcDataLen = stFrameInfo.nFrameLen;
            stConverPixelParam.enSrcPixelType = stFrameInfo.enPixelType;
            stConverPixelParam.enDstPixelType = enDstPixelType;
            stConverPixelParam.pDstBuffer = pImage;
            stConverPixelParam.nDstBufferSize = m_nBufSizeForSaveImage;
            nRet = ptrCamera.MV_CC_ConvertPixelType_NET(ref stConverPixelParam);
            if (MyCamera.MV_OK != nRet)
            {
                return null;
            }

            if (enDstPixelType == MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono8)
            {
                //************************Mono8 转 Bitmap*******************************
                Bitmap bmp = new Bitmap(stFrameInfo.nWidth, stFrameInfo.nHeight, stFrameInfo.nWidth * 1, PixelFormat.Format8bppIndexed, pImage);

                ColorPalette cp = bmp.Palette;
                // init palette
                for (int i = 0; i < 256; i++)
                {
                    cp.Entries[i] = Color.FromArgb(i, i, i);
                }
                // set palette back
                bmp.Palette = cp;
                return bmp;
            }
            else
            {
                //*********************RGB8 转 Bitmap**************************
                for (int i = 0; i < stFrameInfo.nHeight; i++)
                {
                    for (int j = 0; j < stFrameInfo.nWidth; j++)
                    {
                        byte chRed = m_pBufForSaveImage[i * stFrameInfo.nWidth * 3 + j * 3];
                        m_pBufForSaveImage[i * stFrameInfo.nWidth * 3 + j * 3] = m_pBufForSaveImage[i * stFrameInfo.nWidth * 3 + j * 3 + 2];
                        m_pBufForSaveImage[i * stFrameInfo.nWidth * 3 + j * 3 + 2] = chRed;
                    }
                }
                try
                {
                    Bitmap bmp = new Bitmap(stFrameInfo.nWidth, stFrameInfo.nHeight, stFrameInfo.nWidth * 3, PixelFormat.Format24bppRgb, pImage);
                    return bmp;
                }
                catch
                {
                }

            }
            return null;
        }
        private void ReceiveImageWorkThread(object obj, uint PayloadSize)
        {
            int nRet = MvCamCtrl.NET.MyCamera.MV_OK;
            MvCamCtrl.NET.MyCamera device = obj as MvCamCtrl.NET.MyCamera;
            MvCamCtrl.NET.MyCamera.MV_FRAME_OUT_INFO_EX stImageInfo = new MvCamCtrl.NET.MyCamera.MV_FRAME_OUT_INFO_EX();
            IntPtr pData = Marshal.AllocHGlobal((int)PayloadSize);
            if (pData == IntPtr.Zero)
            {
                return;
            }
            uint nDataSize = PayloadSize;
            MvCamCtrl.NET.MyCamera.MV_CC_INPUT_FRAME_INFO stInputFrameInfo = new MvCamCtrl.NET.MyCamera.MV_CC_INPUT_FRAME_INFO();
            while (true)
            {
                nRet = device.MV_CC_GetOneFrameTimeout_NET(pData, nDataSize, ref stImageInfo, 1000);
                if (nRet == MvCamCtrl.NET.MyCamera.MV_OK)
                {
                    stInputFrameInfo.pData = pData;
                    stInputFrameInfo.nDataLen = stImageInfo.nFrameLen;
                    nRet = device.MV_CC_InputOneFrame_NET(ref stInputFrameInfo);
                    if (MvCamCtrl.NET.MyCamera.MV_OK != nRet)
                    {
                        Console.WriteLine("Input one frame failed: nRet {0:x8}", nRet);
                    }
                }
                if (!isRecording)
                {
                    Marshal.FreeHGlobal(pData);
                    break;
                }
                
            }
        }
        /// <summary>
        /// <para>en: Start recording. Return 0 on success, and return Error Code on failure.</para>
        /// <para>vi: Bắt đầu thu. Trả về 0 khi thành công và trả về Mã lỗi khi thất bại.</para>
        /// </summary>
        /// <param name="VideoPath">
        /// <para>en: Video path with avi format</para>
        /// <para>vi: Đường dẫn video với định dạng avi</para>
        /// </param>
        /// <returns></returns>
        public int StartRecording(string VideoPath)
        {
            FileInfo fileInfo = new FileInfo(VideoPath);
            string url = fileInfo.FullName;
            if(Directory.Exists(fileInfo.DirectoryName))
            {
                Directory.CreateDirectory(fileInfo.DirectoryName);
            }
            int nRet = -1;
            if (ptrCamera != null && isRecording == false)
            {
                //turn off trigger mode
                nRet = ptrCamera.MV_CC_SetEnumValue_NET("TriggerMode", 0);
                if (MvCamCtrl.NET.MyCamera.MV_OK != nRet)
                {
                    return nRet;
                }
                //Get packet size
                MvCamCtrl.NET.MyCamera.MVCC_INTVALUE stParam = new MvCamCtrl.NET.MyCamera.MVCC_INTVALUE();
                nRet = ptrCamera.MV_CC_GetIntValue_NET("PayloadSize", ref stParam);
                uint PayloadSize = stParam.nCurValue;
                if (MvCamCtrl.NET.MyCamera.MV_OK != nRet)
                {
                    return nRet;
                }
                nRet = ptrCamera.MV_CC_GetIntValue_NET("Width", ref stParam);
                if (MvCamCtrl.NET.MyCamera.MV_OK != nRet)
                {
                    return nRet;
                }
                mRecoder.nWidth = (ushort)stParam.nCurValue;
                nRet = ptrCamera.MV_CC_GetIntValue_NET("Height", ref stParam);
                if (MvCamCtrl.NET.MyCamera.MV_OK != nRet)
                {
                    return nRet;
                }
                mRecoder.nHeight = (ushort)stParam.nCurValue;
                MvCamCtrl.NET.MyCamera.MVCC_ENUMVALUE stEnumValue = new MvCamCtrl.NET.MyCamera.MVCC_ENUMVALUE();
                nRet = ptrCamera.MV_CC_GetEnumValue_NET("PixelFormat", ref stEnumValue);
                if (MvCamCtrl.NET.MyCamera.MV_OK != nRet)
                {
                    return nRet;
                }
                mRecoder.enPixelType = (MvCamCtrl.NET.MyCamera.MvGvspPixelType)stEnumValue.nCurValue;
                MvCamCtrl.NET.MyCamera.MVCC_FLOATVALUE stFloatValue = new MvCamCtrl.NET.MyCamera.MVCC_FLOATVALUE();
                nRet = ptrCamera.MV_CC_GetFloatValue_NET("AcquisitionFrameRate", ref stFloatValue);
                if (MvCamCtrl.NET.MyCamera.MV_OK != nRet)
                {
                    return nRet;
                }
                mRecoder.fFrameRate = stFloatValue.fCurValue > 28 ? 28 : stFloatValue.fCurValue;
                mRecoder.nBitRate = 1000;
                mRecoder.enRecordFmtType = MvCamCtrl.NET.MyCamera.MV_RECORD_FORMAT_TYPE.MV_FormatType_AVI;

                mRecoder.strFilePath = url;


                nRet = ptrCamera.MV_CC_StartGrabbing_NET();
                if (MvCamCtrl.NET.MyCamera.MV_OK != nRet)
                {
                    return nRet;
                }

                nRet = ptrCamera.MV_CC_StartRecord_NET(ref mRecoder);
                if(nRet == MvCamCtrl.NET.MyCamera.MV_OK)
                {
                    isRecording = true;
                }
                Thread hReceiveImageThreadHandle = new Thread(() => ReceiveImageWorkThread(ptrCamera, PayloadSize));
                hReceiveImageThreadHandle.Start();
            }
            return nRet;
        }
        /// <summary>
        /// <para>en: Stop recording. Return 0 on success, and return Error Code on failure.</para>
        /// <para>vi: Dừng thu. Trả về 0 khi thành công và trả về Mã lỗi khi thất bại.</para>
        /// </summary>
        /// <returns></returns>
        public int StopRecording()
        {
            int nRet = -1;
            if (ptrCamera != null && isRecording == true)
            {
                nRet = ptrCamera.MV_CC_StopGrabbing_NET();
                if (nRet == MvCamCtrl.NET.MyCamera.MV_OK)
                {
                    isRecording = false;
                }
                nRet = ptrCamera.MV_CC_StopRecord_NET();
            }
            return nRet;
        }
        /// <summary>
        /// <para>en: Start the image acquisition.If succeeded, return 0; if failed, return error Code.</para>
        /// <para>vi: Bắt đầu thu thập hình ảnh. Nếu thành công, trả về 0; nếu thất bại, trả về mã lỗi.</para>
        /// </summary>
        /// <returns></returns>
        public int StartGrabbing()
        {
            int nRet = -1;
            if (isGrabbing == true)
            {
                return 0;
            }
            if (ptrCamera!= null && isGrabbing == false)
            {
                nRet = ptrCamera.MV_CC_StartGrabbing_NET();
                if (nRet == MvCamCtrl.NET.MyCamera.MV_OK)
                    isGrabbing = true;
            }
            return nRet;
        }
        /// <summary>
        /// <para>en: Stop the image acquisition.If succeeded, return 0; if failed, return error Code.</para>
        /// <para>vi: Dừng thu thập hình ảnh. Nếu thành công, trả về 0; nếu thất bại, trả về mã lỗi.</para>
        /// </summary>
        /// <returns></returns>
        public int StopGrabbing()
        {
            int nRet = -1;
            if (ptrCamera != null && isGrabbing == true)
            {
                nRet = ptrCamera.MV_CC_StopGrabbing_NET();
                if (nRet == MvCamCtrl.NET.MyCamera.MV_OK)
                    isGrabbing = false;
            }
            return nRet;
        }
        /// <summary>
        /// <para>en: Import the local features to the camera. </para>
        /// <para>vi: Nhập các tính năng cục bộ vào máy ảnh.</para>
        /// </summary>
        /// <param name="pFileName"></param>
        /// <returns></returns>

        public int FeatureLoad(string pFileName)
        {
            XmlDocument fileDocXml = new XmlDocument();
            try
            {
                fileDocXml.Load(pFileName);
            }
            catch (Exception)
            {
                return -50;
            }
            XmlNodeList fileNodeList = fileDocXml.GetElementsByTagName("Feature");
            foreach (XmlNode item in fileNodeList)
            {
                KeyType type = KeyType.None;
                switch(item["Type"].InnerText)
                {
                    case "Boolean":
                        { type = KeyType.Boolean;break; }
                    case "Integer":
                        { type = KeyType.Integer; break; }
                    case "Enumeration":
                        { type = KeyType.Enumeration; break; }
                    case "Float":
                        { type = KeyType.Float; break; }
                    case "String":
                        { type = KeyType.String; break; }
                }
                if (type == KeyType.None)
                    continue;
                SetValue(item["Name"].InnerText,item["Value"].InnerText, type);
            }
            return 0;
        }
        /// <summary>
        /// <para>en: Set the value for camera node.</para>
        /// <para>vi: Đặt giá trị cho nút camera.</para>
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        public int SetParameter(KeyName Key, object Value)
        {
            string keyStr = GetStringKey(Key);
            KeyType keyType = GetKeyType(keyStr);
            return SetValue(keyStr, Value, keyType);
        }
        /// <summary>
        /// <para>en: Get the value of camera node.</para>
        /// <para>vi: Lấy giá trị của nút camera.</para>
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        public int GetParameter(KeyName Key, ref object Value)
        {
            string keyStr = GetStringKey(Key);
            KeyType keyType = GetKeyType(keyStr);
            return GetValue(keyStr, ref Value, keyType);
        }
        /// <summary>
        /// <para>en: Get the value of camera node.</para>
        /// <para>vi: Lấy giá trị của nút camera.</para>
        /// </summary>
        /// <typeparam name="H">type is parameter</typeparam>
        /// <param name="Key"></param>
        /// <param name="value"></param>
        /// <param name="DataType"></param>
        /// <returns></returns>
        private int GetValue<H>(string Key, ref H value, KeyType DataType)
        {
            int nRet = MvCamCtrl.NET.MyCamera.MV_OK;
            try
            {
                switch (DataType)
                {
                    case KeyType.Boolean:
                        {
                            bool t = true;
                            nRet = ptrCamera.MV_CC_GetBoolValue_NET(Key, ref t);
                            value = (H)Convert.ChangeType(t, typeof(H));
                            break;
                        }
                    case KeyType.Integer:
                        {
                            MvCamCtrl.NET.MyCamera.MVCC_INTVALUE_EX t = new MvCamCtrl.NET.MyCamera.MVCC_INTVALUE_EX();
                            nRet = ptrCamera.MV_CC_GetIntValueEx_NET(Key, ref t);
                            value = (H)Convert.ChangeType(t.nCurValue, typeof(H));
                            break;
                        }
                    case KeyType.Enumeration:
                        {
                            MvCamCtrl.NET.MyCamera.MVCC_ENUMVALUE t = new MvCamCtrl.NET.MyCamera.MVCC_ENUMVALUE();
                            nRet = ptrCamera.MV_CC_GetEnumValue_NET(Key, ref t);
                            value = (H)Convert.ChangeType(t.nCurValue, typeof(H));
                            break;
                        }
                    case KeyType.Float:
                        {
                            MvCamCtrl.NET.MyCamera.MVCC_FLOATVALUE t = new MvCamCtrl.NET.MyCamera.MVCC_FLOATVALUE();
                            nRet = ptrCamera.MV_CC_GetFloatValue_NET(Key, ref t);
                            value = (H)Convert.ChangeType(t.fCurValue, typeof(H));
                            break;
                        }
                    case KeyType.String:
                        {
                            MvCamCtrl.NET.MyCamera.MVCC_STRINGVALUE t = new MvCamCtrl.NET.MyCamera.MVCC_STRINGVALUE();
                            nRet = ptrCamera.MV_CC_GetStringValue_NET(Key, ref t);
                            value = (H)Convert.ChangeType(t.chCurValue, typeof(H));
                            break;
                        }
                }
                return nRet;
            }
            catch
            {
                return -1000;
            }
            
        }
        /// <summary>
        /// <para>en: Set the value for camera node.</para>
        /// <para>vi: Đặt giá trị cho nút camera.</para>
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="value"></param>
        /// <param name="DataType"></param>
        /// <returns></returns>
        private int SetValue(string Key, object value, KeyType DataType)
        {
            int nRet = MvCamCtrl.NET.MyCamera.MV_OK;
            try
            {
                switch (DataType)
                {
                    case KeyType.Boolean:
                        {
                            bool v = (Convert.ToInt16(value) == 1) ? true : false;
                            nRet = ptrCamera.MV_CC_SetBoolValue_NET(Key, v);
                            break;
                        }
                    case KeyType.Integer:
                        {
                            nRet = ptrCamera.MV_CC_SetIntValueEx_NET(Key, Convert.ToInt64(value));
                            break;
                        }
                    case KeyType.Enumeration:
                        {
                            nRet = ptrCamera.MV_CC_SetEnumValue_NET(Key, Convert.ToUInt32(value));
                            break;
                        }
                    case KeyType.Float:
                        {
                            float v = (float)Convert.ToDecimal(value);
                            nRet = ptrCamera.MV_CC_SetFloatValue_NET(Key, v);
                            break;
                        }
                    case KeyType.Command:
                        {
                            nRet = ptrCamera.MV_CC_SetCommandValue_NET(Key);
                            break;
                        }
                    case KeyType.String:
                        {
                            nRet = ptrCamera.MV_CC_SetStringValue_NET(Key, Convert.ToString(value));
                            break;
                        }
                }
                return nRet;
            }
            catch
            {
                return -56;
            }
            
        }
        private KeyType GetKeyType(string KeyName)
        {
            string[] IEnumeration = new string[] {"DeviceType","DeviceScanType","DeviceConnectionStatus","DeviceLinkHeartbeatMode",
                "DeviceStreamChannelType","DeviceStreamChannelEndianness","DeviceCharacterSet","DeviceTemperatureSelector",
                "RegionSelector","RegionDestination","PixelFormat","PixelSize","ImageCompressionMode","TestPatternGeneratorSelector",
                "TestPattern","BinningSelector","BinningHorizontal","BinningVertical","DecimationHorizontal","DecimationVertical",
                "Deinterlacing","FrameSpecInfoSelector","AcquisitionMode","TriggerSelector","TriggerMode","TriggerSource",
                "TriggerActivation","SensorShutterMode","ExposureMode","ExposureAuto","GainShutPrior","LineSelector","LineMode",
                "LineSource","CounterSelector","CounterEventSource","CounterResetSource","GainAuto","BlackLevelAuto","BalanceWhiteAuto",
                "BalanceRatioSelector","GammaSelector","SharpnessAuto","HueAuto","SaturationAuto","DigitalNoiseReductionMode",
                "AutoFunctionAOISelector","LUTSelector","EncoderSelector","EncoderSourceA","EncoderSourceB","EncoderTriggerMode",
                "EncoderCounterMode","InputSource","SignalAlignment","ShadingSelector","UserSetSelector","UserSetDefault",
                "GevDeviceModeCharacterSet","GevSupportedOptionSelector","GevCCP"};

            string[] IBoolean = new string[] {"ReverseX","ReverseY","ReverseScanDirection","FrameSpecInfo","AcquisitionFrameRateEnable",
                "AcquisitionLineRateEnable","TriggerCacheEnable","FrameTimeoutEnable","HDREnable","LineInverter","LineTermination",
                "LineStatus","StrobeEnable","ADCGainEnable","DigitalShiftEnable","BlackLevelEnable","GammaEnable","SharpnessEnable",
                "HueEnable","SaturationEnable","AutoFunctionAOIUsageIntensity","AutoFunctionAOIUsageWhiteBalance","LUTEnable",
                "NUCEnable","FPNCEnable","PRNUCEnable","GevDeviceModeIsBigEndian","GevSupportedOption","GevCurrentIPConfigurationLLA",
                "GevCurrentIPConfigurationDHCP","GevCurrentIPConfigurationPersistentIP","GevPAUSEFrameReception","GevGVCPHeartbeatDisable",
                "GevSCPSFireTestPacket","GevSCPSDoNotFragment","GevSCPSBigEndian","PacketUnorderSupport"};

            string[] ICommand = new string[] {"DeviceReset","FindMe","AcquisitionStart","AcquisitionStop","TriggerSoftware","CounterReset",
                "EncoderCounterReset","EncoderReverseCounterReset","ActivateShading","UserSetLoad","UserSetSave","GevTimestampControlLatch",
                "GevTimestampControlReset","GevTimestampControlLatchReset"};

            string[] IFloat = new string[] {"DeviceTemperature","AcquisitionFrameRate","ResultingFrameRate","TriggerDelay","ExposureTime",
                "HDRGain","Gain","AutoGainLowerLimit","AutoGainUpperLimit","DigitalShift","Gamma"};

            string[] IString = new string[] {
                "DeviceVendorName","DeviceModelName","DeviceManufacturerInfo","DeviceVersion","DeviceFirmwareVersion","DeviceSerialNumber","DeviceID",
                "DeviceUserID"
                };
            string[] IInteger = new string[] {"DeviceUptime","BoardDeviceType","DeviceConnectionSelector","DeviceConnectionSpeed",
                "DeviceLinkSelector","DeviceLinkSpeed","DeviceLinkConnectionCount","DeviceLinkHeartbeatTimeout","DeviceStreamChannelCount",
                "DeviceStreamChannelSelector","DeviceStreamChannelLink","DeviceStreamChannelPacketSize","DeviceEventChannelCount",
                "DeviceMaxThroughput","WidthMax","HeightMax","Width","Height","OffsetX","OffsetY","ImageCompressionQuality",
                "AcquisitionBurstFrameCount","AcquisitionLineRate","ResultingLineRate","AutoExposureTimeLowerLimit",
                "AutoExposureTimeUpperLimit","FrameTimeoutTime","HDRSelector","HDRShuter","LineStatusAll","LineDebouncerTime",
                "StrobeLineDuration","StrobeLineDelay","StrobeLinePreDelay","CounterValue","CounterCurrentValue","Brightness",
                "BlackLevel","BalanceRatio","Sharpness","Hue","Saturation","NoiseReduction","AirspaceNoiseReduction",
                "TemporalNoiseReduction","AutoFunctionAOIWidth","AutoFunctionAOIHeight","AutoFunctionAOIOffsetX","AutoFunctionAOIOffsetY",
                "LUTIndex","LUTValue","EncoderCounter","EncoderCounterMax","EncoderMaxReverseCounter","PreDivider","Multiplier",
                "PostDivider","UserSetCurrent","PayloadSize","GevVersionMajor","GevVersionMinor","GevInterfaceSelector","GevMACAddress",
                "GevCurrentIPAddress","GevCurrentSubnetMask","GevCurrentDefaultGateway","GevNumberOfInterfaces","GevPersistentIPAddress",
                "GevPersistentSubnetMask","GevPersistentDefaultGateway","GevLinkSpeed","GevMessageChannelCount","GevStreamChannelCount",
                "GevHeartbeatTimeout","GevTimestampTickFrequency","GevTimestampValue","GevStreamChannelSelector","GevSCPInterfaceIndex",
                "GevSCPHostPort","GevSCPDirection","GevSCPSPacketSize","GevSCPD","GevSCDA","GevSCSP","TLParamsLocked"};


            if(IEnumeration.Contains(KeyName))
            {
                return KeyType.Enumeration;
            }
            else if (IBoolean.Contains(KeyName))
            {
                return KeyType.Boolean;
            }
            else if (ICommand.Contains(KeyName))
            {
                return KeyType.Command;
            }
            else if (IFloat.Contains(KeyName))
            {
                return KeyType.Float;
            }
            else if (IString.Contains(KeyName))
            {
                return KeyType.String;
            }
            else if (IInteger.Contains(KeyName))
            {
                return KeyType.Integer;
            }
            else
            {
                return KeyType.None;
            }
        }
        private string GetStringKey(KeyName Key)
        {
            return Enum.GetName(typeof(KeyName), Key).Trim();
        }
        private Boolean IsMonoData(MvCamCtrl.NET.MyCamera.MvGvspPixelType enGvspPixelType)
        {
            switch (enGvspPixelType)
            {
                case MvCamCtrl.NET.MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono8:
                case MvCamCtrl.NET.MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono10:
                case MvCamCtrl.NET.MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono10_Packed:
                case MvCamCtrl.NET.MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono12:
                case MvCamCtrl.NET.MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono12_Packed:
                    return true;

                default:
                    return false;
            }
        }
        private Boolean IsColorData(MvCamCtrl.NET.MyCamera.MvGvspPixelType enGvspPixelType)
        {
            switch (enGvspPixelType)
            {
                case MvCamCtrl.NET.MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGR8:
                case MvCamCtrl.NET.MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerRG8:
                case MvCamCtrl.NET.MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGB8:
                case MvCamCtrl.NET.MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerBG8:
                case MvCamCtrl.NET.MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGR10:
                case MvCamCtrl.NET.MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerRG10:
                case MvCamCtrl.NET.MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGB10:
                case MvCamCtrl.NET.MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerBG10:
                case MvCamCtrl.NET.MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGR12:
                case MvCamCtrl.NET.MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerRG12:
                case MvCamCtrl.NET.MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGB12:
                case MvCamCtrl.NET.MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerBG12:
                case MvCamCtrl.NET.MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGR10_Packed:
                case MvCamCtrl.NET.MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerRG10_Packed:
                case MvCamCtrl.NET.MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGB10_Packed:
                case MvCamCtrl.NET.MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerBG10_Packed:
                case MvCamCtrl.NET.MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGR12_Packed:
                case MvCamCtrl.NET.MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerRG12_Packed:
                case MvCamCtrl.NET.MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGB12_Packed:
                case MvCamCtrl.NET.MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerBG12_Packed:
                case MvCamCtrl.NET.MyCamera.MvGvspPixelType.PixelType_Gvsp_RGB8_Packed:
                case MvCamCtrl.NET.MyCamera.MvGvspPixelType.PixelType_Gvsp_YUV422_Packed:
                case MvCamCtrl.NET.MyCamera.MvGvspPixelType.PixelType_Gvsp_YUV422_YUYV_Packed:
                case MvCamCtrl.NET.MyCamera.MvGvspPixelType.PixelType_Gvsp_YCBCR411_8_CBYYCRYY:
                    return true;

                default:
                    return false;
            }
        }
    }
    struct InfoDevice
    {
        public string Name { get; set; }
        public MvCamCtrl.NET.MyCamera.MV_CC_DEVICE_INFO DeviceInfo { get; set; }

    }
    /// <summary>
    /// 
    /// </summary>
    public enum KeyType
    {
        /// <summary>
        /// 
        /// </summary>
        None,
        /// <summary>
        /// 
        /// </summary>
        Boolean,
        /// <summary>
        /// 
        /// </summary>
        Integer,
        /// <summary>
        /// 
        /// </summary>
        Enumeration,
        /// <summary>
        /// 
        /// </summary>
        Float,
        /// <summary>
        /// 
        /// </summary>
        Command,
        /// <summary>
        /// 
        /// </summary>
        String
    }
    enum TriggerSource
    {
        Line0 = 0,
        Line1 = 1,
        Line2 = 2,
        Line3 = 3,
        Counter0 = 4,
        Software = 7,
        FrequencyConverter = 8,
    }
    /// <summary>
    /// 
    /// </summary>
    public enum KeyName
    {
        /// <summary>
        /// 
        /// </summary>
        DeviceType,
        /// <summary>
        /// 
        /// </summary>
        DeviceScanType,
        /// <summary>
        /// 
        /// </summary>
        DeviceVendorName,
        /// <summary>
        /// 
        /// </summary>
        DeviceModelName,
        /// <summary>
        /// 
        /// </summary>
        DeviceManufacturerInfo,
        /// <summary>
        /// 
        /// </summary>
        DeviceVersion,
        /// <summary>
        /// 
        /// </summary>
        DeviceFirmwareVersion,
        /// <summary>
        /// 
        /// </summary>
        DeviceSerialNumber,
        /// <summary>
        /// 
        /// </summary>
        DeviceID,
        /// <summary>
        /// 
        /// </summary>
        DeviceUserID,
        /// <summary>
        /// 
        /// </summary>
        DeviceUptime,
        /// <summary>
        /// 
        /// </summary>
        BoardDeviceType,
        /// <summary>
        /// 
        /// </summary>
        DeviceConnectionSelector,
        /// <summary>
        /// 
        /// </summary>
        DeviceConnectionSpeed,
        /// <summary>
        /// 
        /// </summary>
        DeviceConnectionStatus,
        /// <summary>
        /// 
        /// </summary>
        DeviceLinkSelector,
        /// <summary>
        /// 
        /// </summary>
        DeviceLinkSpeed,
        /// <summary>
        /// 
        /// </summary>
        DeviceLinkConnectionCount,
        /// <summary>
        /// 
        /// </summary>
        DeviceLinkHeartbeatMode,
        /// <summary>
        /// 
        /// </summary>
        DeviceLinkHeartbeatTimeout,
        /// <summary>
        /// 
        /// </summary>
        DeviceStreamChannelCount,
        /// <summary>
        /// 
        /// </summary>
        DeviceStreamChannelSelector,
        /// <summary>
        /// 
        /// </summary>
        DeviceStreamChannelType,
        /// <summary>
        /// 
        /// </summary>
        DeviceStreamChannelLink,
        /// <summary>
        /// 
        /// </summary>
        DeviceStreamChannelEndianness,
        /// <summary>
        /// 
        /// </summary>
        DeviceStreamChannelPacketSize,
        /// <summary>
        /// 
        /// </summary>
        DeviceEventChannelCount,
        /// <summary>
        /// 
        /// </summary>
        DeviceCharacterSet,
        /// <summary>
        /// 
        /// </summary>
        DeviceReset,
        /// <summary>
        /// 
        /// </summary>
        DeviceTemperatureSelector,
        /// <summary>
        /// 
        /// </summary>
        DeviceTemperature,
        /// <summary>
        /// 
        /// </summary>
        FindMe,
        /// <summary>
        /// 
        /// </summary>
        DeviceMaxThroughput,
        /// <summary>
        /// 
        /// </summary>
        WidthMax,
        /// <summary>
        /// 
        /// </summary>
        HeightMax,
        /// <summary>
        /// 
        /// </summary>
        RegionSelector,
        /// <summary>
        /// 
        /// </summary>
        RegionDestination,
        /// <summary>
        /// 
        /// </summary>
        Width,
        /// <summary>
        /// 
        /// </summary>
        Height,
        /// <summary>
        /// 
        /// </summary>
        OffsetX,
        /// <summary>
        /// 
        /// </summary>
        OffsetY,
        /// <summary>
        /// 
        /// </summary>
        ReverseX,
        /// <summary>
        /// 
        /// </summary>
        /// <summary>
        /// 
        /// </summary>
        ReverseY,
        /// <summary>
        /// 
        /// </summary>
        ReverseScanDirection,
        /// <summary>
        /// 
        /// </summary>
        PixelFormat,
        /// <summary>
        /// 
        /// </summary>
        PixelSize,
        /// <summary>
        /// 
        /// </summary>
        ImageCompressionMode,
        /// <summary>
        /// 
        /// </summary>
        ImageCompressionQuality,
        /// <summary>
        /// 
        /// </summary>
        TestPatternGeneratorSelector,
        /// <summary>
        /// 
        /// </summary>
        TestPattern,
        /// <summary>
        /// 
        /// </summary>
        BinningSelector,
        /// <summary>
        /// 
        /// </summary>
        BinningHorizontal,
        /// <summary>
        /// 
        /// </summary>
        BinningVertical,
        /// <summary>
        /// 
        /// </summary>
        DecimationHorizontal,
        /// <summary>
        /// 
        /// </summary>
        DecimationVertical,
        /// <summary>
        /// 
        /// </summary>
        Deinterlacing,
        /// <summary>
        /// 
        /// </summary>
        FrameSpecInfoSelector,
        /// <summary>
        /// 
        /// </summary>
        FrameSpecInfo,
        /// <summary>
        /// 
        /// </summary>
        AcquisitionMode,
        /// <summary>
        /// 
        /// </summary>
        AcquisitionStart,
        /// <summary>
        /// 
        /// </summary>
        AcquisitionStop,
        /// <summary>
        /// 
        /// </summary>
        AcquisitionBurstFrameCount,
        /// <summary>
        /// 
        /// </summary>
        AcquisitionFrameRate,
        /// <summary>
        /// 
        /// </summary>
        AcquisitionFrameRateEnable,
        /// <summary>
        /// 
        /// </summary>
        AcquisitionLineRate,
        /// <summary>
        /// 
        /// </summary>
        AcquisitionLineRateEnable,
        /// <summary>
        /// 
        /// </summary>
        ResultingLineRate,
        /// <summary>
        /// 
        /// </summary>
        ResultingFrameRate,
        /// <summary>
        /// 
        /// </summary>
        TriggerSelector,
        /// <summary>
        /// 
        /// </summary>
        TriggerMode,
        /// <summary>
        /// 
        /// </summary>
        TriggerSoftware,
        /// <summary>
        /// 
        /// </summary>
        TriggerSource,
        /// <summary>
        /// 
        /// </summary>
        TriggerActivation,
        /// <summary>
        /// 
        /// </summary>
        /// <summary>
        /// 
        /// </summary>
        TriggerDelay,
        /// <summary>
        /// 
        /// </summary>
        TriggerCacheEnable,
        /// <summary>
        /// 
        /// </summary>
        SensorShutterMode,
        /// <summary>
        /// 
        /// </summary>
        ExposureMode,
        /// <summary>
        /// 
        /// </summary>
        ExposureTime,
        /// <summary>
        /// 
        /// </summary>
        ExposureAuto,
        /// <summary>
        /// 
        /// </summary>
        AutoExposureTimeLowerLimit,
        /// <summary>
        /// 
        /// </summary>
        AutoExposureTimeUpperLimit,
        /// <summary>
        /// 
        /// </summary>
        GainShutPrior,
        /// <summary>
        /// 
        /// </summary>
        FrameTimeoutEnable,
        /// <summary>
        /// 
        /// </summary>
        FrameTimeoutTime,
        /// <summary>
        /// 
        /// </summary>
        HDREnable,
        /// <summary>
        /// 
        /// </summary>
        HDRSelector,
        /// <summary>
        /// 
        /// </summary>
        HDRShuter,
        /// <summary>
        /// 
        /// </summary>
        HDRGain,
        /// <summary>
        /// 
        /// </summary>
        LineSelector,
        /// <summary>
        /// 
        /// </summary>
        LineMode,
        /// <summary>
        /// 
        /// </summary>
        LineInverter,
        /// <summary>
        /// 
        /// </summary>
        LineTermination,
        /// <summary>
        /// 
        /// </summary>
        LineStatus,
        /// <summary>
        /// 
        /// </summary>
        LineStatusAll,
        /// <summary>
        /// 
        /// </summary>
        LineSource,
        /// <summary>
        /// 
        /// </summary>
        StrobeEnable,
        /// <summary>
        /// 
        /// </summary>
        LineDebouncerTime,
        /// <summary>
        /// 
        /// </summary>
        StrobeLineDuration,
        /// <summary>
        /// 
        /// </summary>
        StrobeLineDelay,
        /// <summary>
        /// 
        /// </summary>
        StrobeLinePreDelay,
        /// <summary>
        /// 
        /// </summary>
        CounterSelector,
        /// <summary>
        /// 
        /// </summary>
        CounterEventSource,
        /// <summary>
        /// 
        /// </summary>
        CounterResetSource,
        /// <summary>
        /// 
        /// </summary>
        CounterReset,
        /// <summary>
        /// 
        /// </summary>
        CounterValue,
        /// <summary>
        /// 
        /// </summary>
        CounterCurrentValue,
        /// <summary>
        /// 
        /// </summary>
        Gain,
        /// <summary>
        /// 
        /// </summary>
        GainAuto,
        /// <summary>
        /// 
        /// </summary>
        AutoGainLowerLimit,
        /// <summary>
        /// 
        /// </summary>
        AutoGainUpperLimit,
        /// <summary>
        /// 
        /// </summary>
        ADCGainEnable,
        /// <summary>
        /// 
        /// </summary>
        DigitalShift,
        /// <summary>
        /// 
        /// </summary>
        DigitalShiftEnable,
        /// <summary>
        /// 
        /// </summary>
        Brightness,
        /// <summary>
        /// 
        /// </summary>
        BlackLevel,
        /// <summary>
        /// 
        /// </summary>
        BlackLevelEnable,
        /// <summary>
        /// 
        /// </summary>
        /// <summary>
        /// 
        /// </summary>
        BlackLevelAuto,
        /// <summary>
        /// 
        /// </summary>
        BalanceWhiteAuto,
        /// <summary>
        /// 
        /// </summary>
        BalanceRatioSelector,
        /// <summary>
        /// 
        /// </summary>
        BalanceRatio,
        /// <summary>
        /// 
        /// </summary>
        Gamma,
        /// <summary>
        /// 
        /// </summary>
        GammaSelector,
        /// <summary>
        /// 
        /// </summary>
        GammaEnable,
        /// <summary>
        /// 
        /// </summary>
        Sharpness,
        /// <summary>
        /// 
        /// </summary>
        SharpnessEnable,
        /// <summary>
        /// 
        /// </summary>
        SharpnessAuto,
        /// <summary>
        /// 
        /// </summary>
        Hue,
        /// <summary>
        /// 
        /// </summary>
        HueEnable,
        /// <summary>
        /// 
        /// </summary>
        HueAuto,
        /// <summary>
        /// 
        /// </summary>
        Saturation,
        /// <summary>
        /// 
        /// </summary>
        SaturationEnable,
        /// <summary>
        /// 
        /// </summary>
        SaturationAuto,
        /// <summary>
        /// 
        /// </summary>
        DigitalNoiseReductionMode,
        /// <summary>
        /// 
        /// </summary>
        NoiseReduction,
        /// <summary>
        /// 
        /// </summary>
        AirspaceNoiseReduction,
        /// <summary>
        /// 
        /// </summary>
        TemporalNoiseReduction,
        /// <summary>
        /// 
        /// </summary>
        AutoFunctionAOISelector,
        /// <summary>
        /// 
        /// </summary>
        AutoFunctionAOIWidth,
        /// <summary>
        /// 
        /// </summary>
        AutoFunctionAOIHeight,
        /// <summary>
        /// 
        /// </summary>
        AutoFunctionAOIOffsetX,
        /// <summary>
        /// 
        /// </summary>
        AutoFunctionAOIOffsetY,
        /// <summary>
        /// 
        /// </summary>
        AutoFunctionAOIUsageIntensity,
        /// <summary>
        /// 
        /// </summary>
        AutoFunctionAOIUsageWhiteBalance,
        /// <summary>
        /// 
        /// </summary>
        LUTSelector,
        /// <summary>
        /// 
        /// </summary>
        LUTEnable,
        /// <summary>
        /// 
        /// </summary>
        LUTIndex,
        /// <summary>
        /// 
        /// </summary>
        LUTValue,
        /// <summary>
        /// 
        /// </summary>
        LUTValueAll,
        /// <summary>
        /// 
        /// </summary>
        EncoderSelector,
        /// <summary>
        /// 
        /// </summary>
        EncoderSourceA,
        /// <summary>
        /// 
        /// </summary>
        EncoderSourceB,
        /// <summary>
        /// 
        /// </summary>
        EncoderTriggerMode,
        /// <summary>
        /// 
        /// </summary>
        EncoderCounterMode,
        /// <summary>
        /// 
        /// </summary>
        EncoderCounter,
        /// <summary>
        /// 
        /// </summary>
        EncoderCounterMax,
        /// <summary>
        /// 
        /// </summary>
        EncoderCounterReset,
        /// <summary>
        /// 
        /// </summary>
        EncoderMaxReverseCounter,
        /// <summary>
        /// 
        /// </summary>
        EncoderReverseCounterReset,
        /// <summary>
        /// 
        /// </summary>
        InputSource,
        /// <summary>
        /// 
        /// </summary>
        SignalAlignment,
        /// <summary>
        /// 
        /// </summary>
        PreDivider,
        /// <summary>
        /// 
        /// </summary>
        Multiplier,
        /// <summary>
        /// 
        /// </summary>
        PostDivider,
        /// <summary>
        /// 
        /// </summary>
        ShadingSelector,
        /// <summary>
        /// 
        /// </summary>
        ActivateShading,
        /// <summary>
        /// 
        /// </summary>
        NUCEnable,
        /// <summary>
        /// 
        /// </summary>
        FPNCEnable,
        /// <summary>
        /// 
        /// </summary>
        PRNUCEnable,
        /// <summary>
        /// 
        /// </summary>
        UserSetCurrent,
        /// <summary>
        /// 
        /// </summary>
        UserSetSelector,
        /// <summary>
        /// 
        /// </summary>
        UserSetLoad,
        /// <summary>
        /// 
        /// </summary>
        UserSetSave,
        /// <summary>
        /// 
        /// </summary>
        UserSetDefault,
        /// <summary>
        /// 
        /// </summary>
        PayloadSize,
        /// <summary>
        /// 
        /// </summary>
        GevVersionMajor,
        /// <summary>
        /// 
        /// </summary>
        GevVersionMinor,
        /// <summary>
        /// 
        /// </summary>
        GevDeviceModeIsBigEndian,
        /// <summary>
        /// 
        /// </summary>
        GevDeviceModeCharacterSet,
        /// <summary>
        /// 
        /// </summary>
        GevInterfaceSelector,
        /// <summary>
        /// 
        /// </summary>
        GevMACAddress,
        /// <summary>
        /// 
        /// </summary>
        GevSupportedOptionSelector,
        /// <summary>
        /// 
        /// </summary>
        GevSupportedOption,
        /// <summary>
        /// 
        /// </summary>
        GevCurrentIPConfigurationLLA,
        /// <summary>
        /// 
        /// </summary>
        GevCurrentIPConfigurationDHCP,
        /// <summary>
        /// 
        /// </summary>
        GevCurrentIPConfigurationPersistentIP,
        /// <summary>
        /// 
        /// </summary>
        GevPAUSEFrameReception,
        /// <summary>
        /// 
        /// </summary>
        GevCurrentIPAddress,
        /// <summary>
        /// 
        /// </summary>
        GevCurrentSubnetMask,
        /// <summary>
        /// 
        /// </summary>
        GevCurrentDefaultGateway,
        /// <summary>
        /// 
        /// </summary>
        GevFirstURL,
        /// <summary>
        /// 
        /// </summary>
        GevSecondURL,
        /// <summary>
        /// 
        /// </summary>
        GevNumberOfInterfaces,
        /// <summary>
        /// 
        /// </summary>
        GevPersistentIPAddress,
        /// <summary>
        /// 
        /// </summary>
        GevPersistentSubnetMask,
        /// <summary>
        /// 
        /// </summary>
        GevPersistentDefaultGateway,
        /// <summary>
        /// 
        /// </summary>
        GevLinkSpeed,
        /// <summary>
        /// 
        /// </summary>
        GevMessageChannelCount,
        /// <summary>
        /// 
        /// </summary>
        GevStreamChannelCount,
        /// <summary>
        /// 
        /// </summary>
        GevHeartbeatTimeout,
        /// <summary>
        /// 
        /// </summary>
        GevGVCPHeartbeatDisable,
        /// <summary>
        /// 
        /// </summary>
        GevTimestampTickFrequency,
        /// <summary>
        /// 
        /// </summary>
        GevTimestampControlLatch,
        /// <summary>
        /// 
        /// </summary>
        GevTimestampControlReset,
        /// <summary>
        /// 
        /// </summary>
        GevTimestampControlLatchReset,
        /// <summary>
        /// 
        /// </summary>
        GevTimestampValue,
        /// <summary>
        /// 
        /// </summary>
        GevCCP,
        /// <summary>
        /// 
        /// </summary>
        GevStreamChannelSelector,
        /// <summary>
        /// 
        /// </summary>
        GevSCPInterfaceIndex,
        /// <summary>
        /// 
        /// </summary>
        GevSCPHostPort,
        /// <summary>
        /// 
        /// </summary>
        GevSCPDirection,
        /// <summary>
        /// 
        /// </summary>
        GevSCPSFireTestPacket,
        /// <summary>
        /// 
        /// </summary>
        GevSCPSDoNotFragment,
        /// <summary>
        /// 
        /// </summary>
        GevSCPSBigEndian,
        /// <summary>
        /// 
        /// </summary>
        PacketUnorderSupport,
        /// <summary>
        /// 
        /// </summary>
        GevSCPSPacketSize,
        /// <summary>
        /// 
        /// </summary>
        GevSCPD,
        /// <summary>
        /// 
        /// </summary>
        GevSCDA,
        /// <summary>
        /// 
        /// </summary>
        GevSCSP,
        /// <summary>
        /// 
        /// </summary>
        TLParamsLocked
    }
}
