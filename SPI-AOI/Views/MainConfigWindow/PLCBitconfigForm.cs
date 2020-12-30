using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SPI_AOI.Views.MainConfigWindow
{
    public partial class PLCBitconfigForm : Form
    {
        Properties.Settings mParam = Properties.Settings.Default;
        bool mLoaded = false;
        public PLCBitconfigForm()
        {
            InitializeComponent();
        }

        private void PLCBitconfigForm_Load(object sender, EventArgs e)
        {
            LoadGeneral();
            LoadTopAxis();
            LoadBotAxis();
            LoadConveyorAxis();
            mLoaded = true;
        }
        private void LoadGeneral()
        {
            txtIP.Text = mParam.PLC_IP;
            nPort.Value = mParam.PLC_PORT;
            txtBitHasProductTop.Text = mParam.PLC_BIT_HAS_PRODUCT_TOP;
            txtBitDoorStatus.Text = mParam.PLC_BIT_DOOR_STATUS;
            txtBitHasProductBot.Text = mParam.PLC_BIT_HAS_PRODUCT_BOT;
            txtBitErrorMachine.Text = mParam.PLC_BIT_ERROR_MACHINE;
            txtBitReadCodeFail.Text = mParam.PLC_BIT_READ_QRCODE_FAIL;
            txtBitCaptureFail.Text = mParam.PLC_BIT_CAPTURE_FAIL;
            txtBitPass.Text = mParam.PLC_BIT_PRODUCT_PASS;
            txtBitFail.Text = mParam.PLC_BIT_PRODUCT_FAIL;
        }
        private void LoadTopAxis()
        {
            txtRegXTop.Text = mParam.PLC_REG_X_TOP;
            txtRegYTop.Text = mParam.PLC_REG_Y_TOP;
            txtBitUpTop.Text = mParam.PLC_BIT_GO_UP_TOP;
            txtBitDownTop.Text = mParam.PLC_BIT_GO_DOWN_TOP;
            txtBitLeftTop.Text = mParam.PLC_BIT_GO_LEFT_TOP;
            txtBitRightTop.Text = mParam.PLC_BIT_GO_RIGHT_TOP;
            txtBitWriteFinishTop.Text = mParam.PLC_BIT_WRITE_COORDINATES_FINISH_TOP;
            txtBitGoFinishTop.Text = mParam.PLC_BIT_GO_COORDINATES_FINISH_TOP;
            txtBitWriteFinishSetupTop.Text = mParam.PLC_BIT_WRITE_COORDINATES_FINISH_SETUP_TOP;
            txtBitGoFinishSetupTop.Text = mParam.PLC_BIT_GO_COORDINATES_FINISH_SETUP_TOP;
            txtRegReadXTop.Text = mParam.PLC_REG_X_TOP_READ;
            txtRegReadYTop.Text = mParam.PLC_REG_Y_TOP_READ;
            txtRegSpeedXTop.Text = mParam.PLC_REG_SPEED_RUN_X_TOP;
            txtRegSpeedYTop.Text = mParam.PLC_REG_SPEED_RUN_Y_TOP;
            txtSpeedRunXTop.Text = mParam.RUN_X_TOP_SPEED.ToString();
            txtSpeedRunYTop.Text = mParam.RUN_Y_TOP_SPEED.ToString();
        }
        private void LoadBotAxis()
        {
            txtRegXBot.Text = mParam.PLC_REG_X_BOT;
            txtRegYBot.Text = mParam.PLC_REG_Y_BOT;
            txtBitUpBot.Text = mParam.PLC_BIT_GO_UP_BOT;
            txtBitDownBot.Text = mParam.PLC_BIT_GO_DOWN_BOT;
            txtBitLeftBot.Text = mParam.PLC_BIT_GO_LEFT_BOT;
            txtBitRightBot.Text = mParam.PLC_BIT_GO_RIGHT_BOT;
            txtBitWriteFinishBot.Text = mParam.PLC_BIT_WRITE_COORDINATES_FINISH_BOT;
            txtBitGoFinishBot.Text = mParam.PLC_BIT_GO_COORDINATES_FINISH_BOT;
            txtBitWriteFinishSetupBot.Text = mParam.PLC_BIT_WRITE_COORDINATES_FINISH_SETUP_BOT;
            txtBitGoFinishSetupBot.Text = mParam.PLC_BIT_GO_COORDINATES_FINISH_SETUP_BOT;
            txtRegReadXBot.Text = mParam.PLC_REG_X_BOT_READ;
            txtRegReadYBot.Text = mParam.PLC_REG_Y_BOT_READ;
            txtRegSpeedXBot.Text = mParam.PLC_REG_SPEED_RUN_X_BOT;
            txtRegSpeedYBot.Text = mParam.PLC_REG_SPEED_RUN_Y_BOT;
            txtSpeedRunXBot.Text = mParam.RUN_X_BOT_SPEED.ToString();
            txtSpeedRunYBot.Text = mParam.RUN_Y_BOT_SPEED.ToString();
        }
        private void LoadConveyorAxis()
        {
            txtRegConveyor.Text = mParam.PLC_REG_CONVEYOR;
            txtBitUpConveyor.Text = mParam.PLC_BIT_GO_UP_CONVEYOR;
            txtBitDownConveyor.Text = mParam.PLC_BIT_GO_DOWN_CONVEYOR;
            txtBitWriteFinishConveyor.Text = mParam.PLC_BIT_WRITE_COORDINATES_FINISH_CONVEYOR;
            txtBitGoFinishConveyor.Text = mParam.PLC_BIT_GO_COORDINATES_FINISH_CONVEYOR;
            txtBitWriteFinishSetupConveyor.Text = mParam.PLC_BIT_WRITE_COORDINATES_FINISH_SETUP_CONVEYOR;
            txtBitGoFinishSetupConveyor.Text = mParam.PLC_BIT_GO_COORDINATES_FINISH_SETUP_CONVEYOR;
        }

        private void txtIP_TextChanged(object sender, EventArgs e)
        {
            if (!mLoaded)
                return;
            TextBox txt = sender as TextBox;
            mParam.PLC_IP = txt.Text;
            mParam.Save();
        }

        private void nPort_ValueChanged(object sender, EventArgs e)
        {
            if (!mLoaded)
                return;
            NumericUpDown num = sender as NumericUpDown;
            mParam.PLC_PORT = Convert.ToInt32(num.Value);
            mParam.Save();
        }

        private void txtBitHasProduct_TextChanged(object sender, EventArgs e)
        {
            if (!mLoaded)
                return;
            TextBox txt = sender as TextBox;
            mParam.PLC_BIT_HAS_PRODUCT_TOP = txt.Text;
            mParam.Save();
        }
        private void txtBitHasProductBot_TextChanged(object sender, EventArgs e)
        {
            if (!mLoaded)
                return;
            TextBox txt = sender as TextBox;
            mParam.PLC_BIT_HAS_PRODUCT_BOT = txt.Text;
            mParam.Save();
        }
        private void txtBitDoorStatus_TextChanged(object sender, EventArgs e)
        {
            if (!mLoaded)
                return;
            TextBox txt = sender as TextBox;
            mParam.PLC_BIT_DOOR_STATUS = txt.Text;
            mParam.Save();
        }
        

        private void txtBitErrorMachine_TextChanged(object sender, EventArgs e)
        {
            if (!mLoaded)
                return;
            TextBox txt = sender as TextBox;
            mParam.PLC_BIT_ERROR_MACHINE = txt.Text;
            mParam.Save();
        }

        private void txtBitReadCodeFail_TextChanged(object sender, EventArgs e)
        {
            if (!mLoaded)
                return;
            TextBox txt = sender as TextBox;
            mParam.PLC_BIT_READ_QRCODE_FAIL = txt.Text;
            mParam.Save();
        }

        private void txtBitCaptureFail_TextChanged(object sender, EventArgs e)
        {
            if (!mLoaded)
                return;
            TextBox txt = sender as TextBox;
            mParam.PLC_BIT_CAPTURE_FAIL = txt.Text;
            mParam.Save();
        }

        private void txtBitPass_TextChanged(object sender, EventArgs e)
        {
            if (!mLoaded)
                return;
            TextBox txt = sender as TextBox;
            mParam.PLC_BIT_PRODUCT_PASS = txt.Text;
            mParam.Save();
        }

        private void txtBitFail_TextChanged(object sender, EventArgs e)
        {
            if (!mLoaded)
                return;
            TextBox txt = sender as TextBox;
            mParam.PLC_BIT_PRODUCT_FAIL = txt.Text;
            mParam.Save();
        }

        private void txtRegXTop_TextChanged(object sender, EventArgs e)
        {
            if (!mLoaded)
                return;
            TextBox txt = sender as TextBox;
            mParam.PLC_REG_X_TOP = txt.Text;
            mParam.Save();
        }

        private void txtRegYTop_TextChanged(object sender, EventArgs e)
        {
            if (!mLoaded)
                return;
            TextBox txt = sender as TextBox;
            mParam.PLC_REG_Y_TOP = txt.Text;
            mParam.Save();
        }

        private void txtBitUpTop_TextChanged(object sender, EventArgs e)
        {
            if (!mLoaded)
                return;
            TextBox txt = sender as TextBox;
            mParam.PLC_BIT_GO_UP_TOP = txt.Text;
            mParam.Save();
        }

        private void txtBitDownTop_TextChanged(object sender, EventArgs e)
        {
            if (!mLoaded)
                return;
            TextBox txt = sender as TextBox;
            mParam.PLC_BIT_GO_DOWN_TOP = txt.Text;
            mParam.Save();
        }

        private void txtBitLeftTop_TextChanged(object sender, EventArgs e)
        {
            if (!mLoaded)
                return;
            TextBox txt = sender as TextBox;
            mParam.PLC_BIT_GO_LEFT_TOP = txt.Text;
            mParam.Save();
        }

        private void txtBitRightTop_TextChanged(object sender, EventArgs e)
        {
            if (!mLoaded)
                return;
            TextBox txt = sender as TextBox;
            mParam.PLC_BIT_GO_RIGHT_TOP = txt.Text;
            mParam.Save();
        }

        private void txtBitWriteFinishTop_TextChanged(object sender, EventArgs e)
        {
            if (!mLoaded)
                return;
            TextBox txt = sender as TextBox;
            mParam.PLC_BIT_WRITE_COORDINATES_FINISH_TOP = txt.Text;
            mParam.Save();
        }

        private void txtBitGoFinishTop_TextChanged(object sender, EventArgs e)
        {
            if (!mLoaded)
                return;
            TextBox txt = sender as TextBox;
            mParam.PLC_BIT_GO_COORDINATES_FINISH_TOP = txt.Text;
            mParam.Save();
        }

        private void txtRegXBot_TextChanged(object sender, EventArgs e)
        {
            if (!mLoaded)
                return;
            TextBox txt = sender as TextBox;
            mParam.PLC_REG_X_BOT = txt.Text;
            mParam.Save();
        }

        private void txtRegYBot_TextChanged(object sender, EventArgs e)
        {
            if (!mLoaded)
                return;
            TextBox txt = sender as TextBox;
            mParam.PLC_REG_Y_BOT = txt.Text;
            mParam.Save();
        }

        private void txtBitUpBot_TextChanged(object sender, EventArgs e)
        {
            if (!mLoaded)
                return;
            TextBox txt = sender as TextBox;
            mParam.PLC_BIT_GO_UP_BOT = txt.Text;
            mParam.Save();
        }

        private void txtBitDownBot_TextChanged(object sender, EventArgs e)
        {
            if (!mLoaded)
                return;
            TextBox txt = sender as TextBox;
            mParam.PLC_BIT_GO_DOWN_BOT = txt.Text;
            mParam.Save();
        }

        private void txtBitLeftBot_TextChanged(object sender, EventArgs e)
        {
            if (!mLoaded)
                return;
            TextBox txt = sender as TextBox;
            mParam.PLC_BIT_GO_LEFT_BOT = txt.Text;
            mParam.Save();
        }

        private void txtBitRightBot_TextChanged(object sender, EventArgs e)
        {
            if (!mLoaded)
                return;
            TextBox txt = sender as TextBox;
            mParam.PLC_BIT_GO_RIGHT_BOT = txt.Text;
            mParam.Save();
        }

        private void txtBitWriteFinishBot_TextChanged(object sender, EventArgs e)
        {
            if (!mLoaded)
                return;
            TextBox txt = sender as TextBox;
            mParam.PLC_BIT_WRITE_COORDINATES_FINISH_BOT = txt.Text;
            mParam.Save();
        }

        private void txtBitGoFinishBot_TextChanged(object sender, EventArgs e)
        {
            if (!mLoaded)
                return;
            TextBox txt = sender as TextBox;
            mParam.PLC_BIT_GO_COORDINATES_FINISH_BOT = txt.Text;
            mParam.Save();
        }

        private void txtRegConveyor_TextChanged(object sender, EventArgs e)
        {
            if (!mLoaded)
                return;
            TextBox txt = sender as TextBox;
            mParam.PLC_REG_CONVEYOR = txt.Text;
            mParam.Save();
        }

        private void txtBitUpConveyor_TextChanged(object sender, EventArgs e)
        {
            if (!mLoaded)
                return;
            TextBox txt = sender as TextBox;
            mParam.PLC_BIT_GO_UP_CONVEYOR = txt.Text;
            mParam.Save();
        }

        private void txtBitDownConveyor_TextChanged(object sender, EventArgs e)
        {
            if (!mLoaded)
                return;
            TextBox txt = sender as TextBox;
            mParam.PLC_BIT_GO_DOWN_CONVEYOR = txt.Text;
            mParam.Save();
        }

        private void txtBitWriteFinishConveyor_TextChanged(object sender, EventArgs e)
        {
            if (!mLoaded)
                return;
            TextBox txt = sender as TextBox;
            mParam.PLC_BIT_WRITE_COORDINATES_FINISH_CONVEYOR = txt.Text;
            mParam.Save();
        }

        private void txtBitGoFinishConveyor_TextChanged(object sender, EventArgs e)
        {
            if (!mLoaded)
                return;
            TextBox txt = sender as TextBox;
            mParam.PLC_BIT_GO_COORDINATES_FINISH_CONVEYOR = txt.Text;
            mParam.Save();
        }

        private void txtBitWriteFinishSetupTop_TextChanged(object sender, EventArgs e)
        {
            if (!mLoaded)
                return;
            TextBox txt = sender as TextBox;
            mParam.PLC_BIT_WRITE_COORDINATES_FINISH_SETUP_TOP = txt.Text;
            mParam.Save();
        }

        private void txtBitGoFinishSetupTop_TextChanged(object sender, EventArgs e)
        {
            if (!mLoaded)
                return;
            TextBox txt = sender as TextBox;
            mParam.PLC_BIT_GO_COORDINATES_FINISH_SETUP_TOP = txt.Text;
            mParam.Save();
        }

        private void txtBitWriteFinishSetupBot_TextChanged(object sender, EventArgs e)
        {
            if (!mLoaded)
                return;
            TextBox txt = sender as TextBox;
            mParam.PLC_BIT_WRITE_COORDINATES_FINISH_SETUP_BOT = txt.Text;
            mParam.Save();
        }

        private void txtBitGoFinishSetupBot_TextChanged(object sender, EventArgs e)
        {
            if (!mLoaded)
                return;
            TextBox txt = sender as TextBox;
            mParam.PLC_BIT_GO_COORDINATES_FINISH_SETUP_BOT = txt.Text;
            mParam.Save();
        }

        private void txtBitWriteFinishSetupConveyor_TextChanged(object sender, EventArgs e)
        {
            if (!mLoaded)
                return;
            TextBox txt = sender as TextBox;
            mParam.PLC_BIT_WRITE_COORDINATES_FINISH_SETUP_CONVEYOR = txt.Text;
            mParam.Save();
        }

        private void txtBitGoFinishSetupConveyor_TextChanged(object sender, EventArgs e)
        {
            if (!mLoaded)
                return;
            TextBox txt = sender as TextBox;
            mParam.PLC_BIT_GO_COORDINATES_FINISH_SETUP_CONVEYOR = txt.Text;
            mParam.Save();
        }
    }
}
