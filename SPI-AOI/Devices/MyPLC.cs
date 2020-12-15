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
        public int Set_Go_Up_Top()
        {
            return mSLMP.SetDevice(mParam.PLC_BIT_GO_UP_TOP, 1);
        }
        public int Reset_Go_Up_Top()
        {
            return mSLMP.SetDevice(mParam.PLC_BIT_GO_UP_TOP, 0);
        }
        public int Set_Go_Down_Top()
        {
            return mSLMP.SetDevice(mParam.PLC_BIT_GO_DOWN_TOP, 1);
        }
        public int Reset_Go_Down_Top()
        {
            return mSLMP.SetDevice(mParam.PLC_BIT_GO_DOWN_TOP, 0);
        }
        public int Set_Go_Left_Top()
        {
            return mSLMP.SetDevice(mParam.PLC_BIT_GO_LEFT_TOP, 1);
        }
        public int Reset_Go_Left_Top()
        {
            return mSLMP.SetDevice(mParam.PLC_BIT_GO_LEFT_TOP, 0);
        }
        public int Set_Go_Right_Top()
        {
            return mSLMP.SetDevice(mParam.PLC_BIT_GO_RIGHT_TOP, 1);
        }
        public int Reset_Go_Right_Top()
        {
            return mSLMP.SetDevice(mParam.PLC_BIT_GO_RIGHT_TOP, 0);
        }
        public int Set_Go_Up_Bot()
        {
            return mSLMP.SetDevice(mParam.PLC_BIT_GO_UP_BOT, 1);
        }
        public int Reset_Go_Up_Bot()
        {
            return mSLMP.SetDevice(mParam.PLC_BIT_GO_UP_BOT, 0);
        }
        public int Set_Go_Down_Bot()
        {
            return mSLMP.SetDevice(mParam.PLC_BIT_GO_DOWN_BOT, 1);
        }
        public int Reset_Go_Down_Bot()
        {
            return mSLMP.SetDevice(mParam.PLC_BIT_GO_DOWN_BOT, 0);
        }
        public int Set_Go_Up_Conveyor()
        {
            return mSLMP.SetDevice(mParam.PLC_BIT_GO_UP_CONVEYOR, 1);
        }
        public int Reset_Go_Up_Conveyor()
        {
            return mSLMP.SetDevice(mParam.PLC_BIT_GO_UP_CONVEYOR, 0);
        }
        public int Set_Go_Down_Conveyor()
        {
            return mSLMP.SetDevice(mParam.PLC_BIT_GO_DOWN_CONVEYOR, 1);
        }
        public int Reset_Go_Down_Conveyor()
        {
            return mSLMP.SetDevice(mParam.PLC_BIT_GO_DOWN_CONVEYOR, 0);
        }
        public int Set_Go_Left_Bot()
        {
            return mSLMP.SetDevice(mParam.PLC_BIT_GO_LEFT_BOT, 1);
        }
        public int Reset_Go_Left_Bot()
        {
            return mSLMP.SetDevice(mParam.PLC_BIT_GO_LEFT_BOT, 0);
        }
        public int Set_Go_Right_Bot()
        {
            return mSLMP.SetDevice(mParam.PLC_BIT_GO_RIGHT_BOT, 1);
        }
        public int Reset_Go_Right_Bot()
        {
            return mSLMP.SetDevice(mParam.PLC_BIT_GO_RIGHT_BOT, 0);
        }
        public int Set_Go_Home()
        {
            return mSLMP.SetDevice(mParam.PLC_BIT_GO_HOME, 0);
        }
        public int Set_Speed_Top(int speed)
        {
            return mSLMP.SetDevice2(mParam.PLC_REG_SPEED_TOP, speed);
        }
        public int Set_Speed_Bot(int speed)
        {
            return mSLMP.SetDevice2(mParam.PLC_REG_SPEED_BOT, speed);
        }
        public int Set_Speed_Conveyor(int speed)
        {
            return mSLMP.SetDevice2(mParam.PLC_REG_SPEED_CONVEYOR, speed);
        }
        public int Get_Speed()
        {
            return mSLMP.GetDevice2(mParam.PLC_REG_SPEED_TOP);
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
        public int Get_X_Top()
        {
            return mSLMP.GetDevice2(mParam.PLC_REG_X_TOP);
        }
        public int Get_Y_Top()
        {
            return mSLMP.GetDevice2(mParam.PLC_REG_Y_TOP);
        }
        public int Set_X_Bot(int value)
        {
            return mSLMP.SetDevice2(mParam.PLC_REG_X_BOT, value);
        }
        public int Set_Y_Bot(int value)
        {
            return mSLMP.SetDevice2(mParam.PLC_REG_Y_BOT, value);
        }
        public int Get_X_Bot()
        {
            return mSLMP.GetDevice2(mParam.PLC_REG_X_BOT);
        }
        public int Get_Y_Bot()
        {
            return mSLMP.GetDevice2(mParam.PLC_REG_Y_BOT);
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
            return mSLMP.SetDevice(mParam.PLC_BIT_WIRTE_COORDINATES_FINISH_TOP, 1);
        }
        public int Get_Go_Coordinates_Finish()
        {
            return mSLMP.GetDevice(mParam.PLC_BIT_GO_COORDINATES_FINISH_TOP);
        }
        public int Set_Capture_Finish()
        {
            return mSLMP.SetDevice(mParam.PLC_BIT_HAS_PRODUCT, 0);
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
