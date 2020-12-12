using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Heal;

namespace SPI_AOI.Devices
{
    class MyPLC
    {
        private SLMP mSLMP = new SLMP(Properties.Settings.Default.PLC_IP, Properties.Settings.Default.PLC_PORT);
        private Properties.Settings mParam = Properties.Settings.Default;
        public int  Ping()
        {
            return mSLMP.GetPing();
        }
        public int Set_Go_Top()
        {
            return mSLMP.SetDevice(mParam.PLC_BIT_GO_TOP, 1);
        }
        public int Reset_Go_Top()
        {
            return mSLMP.SetDevice(mParam.PLC_BIT_GO_TOP, 0);
        }
        public int Set_Go_Bot()
        {
            return mSLMP.SetDevice(mParam.PLC_BIT_GO_BOT, 1);
        }
        public int Reset_Go_Bot()
        {
            return mSLMP.SetDevice(mParam.PLC_BIT_GO_BOT, 0);
        }
        public int Set_Go_Left()
        {
            return mSLMP.SetDevice(mParam.PLC_BIT_GO_LEFT, 1);
        }
        public int Reset_Go_Left()
        {
            return mSLMP.SetDevice(mParam.PLC_BIT_GO_LEFT, 0);
        }
        public int Set_Go_Right()
        {
            return mSLMP.SetDevice(mParam.PLC_BIT_GO_RIGHT, 1);
        }
        public int Reset_Go_Right()
        {
            return mSLMP.SetDevice(mParam.PLC_BIT_GO_RIGHT, 0);
        }
        public int Set_Go_Home()
        {
            return mSLMP.SetDevice(mParam.PLC_BIT_GO_HOME, 0);
        }
        public int Set_Speed(int speed)
        {
            return mSLMP.SetDevice2(mParam.PLC_REG_SPEED, speed);
        }
        public int Get_Speed()
        {
            return mSLMP.GetDevice2(mParam.PLC_REG_SPEED);
        }
        public int Set_Setup_Model()
        {
            return mSLMP.SetDevice(mParam.PLC_BIT_SETUP_MODEL, 1);
        }
        public int Reset_Setup_Model()
        {
            return mSLMP.SetDevice(mParam.PLC_BIT_SETUP_MODEL, 0);
        }
        public int Set_X_Top(int value)
        {
            return mSLMP.SetDevice2(mParam.PLC_REG_X_TOP, value);
        }
        public int Set_Y_Top(int value)
        {
            return mSLMP.SetDevice2(mParam.PLC_REG_Y_TOP, value);
        }
        public int Set_X_Bot(int value)
        {
            return mSLMP.SetDevice2(mParam.PLC_REG_X_BOT, value);
        }
        public int Set_Y_Bot(int value)
        {
            return mSLMP.SetDevice2(mParam.PLC_REG_Y_BOT, value);
        }
        public int Get_Error_Machine()
        {
            return mSLMP.GetDevice(mParam.PLC_BIT_ERROR_MACHINE);
        }
        public int Set_Confirm_Error_Machine()
        {
            return mSLMP.GetDevice(mParam.PLC_BIT_ERROR_MACHINE_CONFIRM);
        }
        public int Get_Has_Product()
        {
            return mSLMP.GetDevice(mParam.PLC_BIT_HAS_PRODUCT);
        }
        public int Set_Write_Coordinates_Finish()
        {
            return mSLMP.SetDevice(mParam.PLC_BIT_WIRTE_COORDINATES_FINISH, 1);
        }
        public int Get_Go_Coordinates_Finish()
        {
            return mSLMP.GetDevice(mParam.PLC_BIT_GO_COORDINATES_FINISH);
        }
        public int Set_Capture_Finish()
        {
            return mSLMP.SetDevice(mParam.PLC_BIT_CAPTURE_FINISH, 1);
        }
        public int Set_Pass()
        {
            return mSLMP.SetDevice(mParam.PLC_BIT_PRODUCT_PASS, 1);
        }
        public int Set_Fail()
        {
            return mSLMP.SetDevice(mParam.PLC_BIT_PRODUCT_FAIL, 1);
        }
        public int Set_Scan_QRCode_Fail()
        {
            return mSLMP.SetDevice(mParam.PLC_BIT_READ_QRCODE_FAIL, 1);
        }
        public int Set_Capture_Fail()
        {
            return mSLMP.SetDevice(mParam.PLC_BIT_CAPTURE_FAIL, 1);
        }
        public int Get_Start_Stop_Status()
        {
            return mSLMP.GetDevice(mParam.PLC_BIT_MACHINE_STATUS);
        }
    }
}
