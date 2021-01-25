using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NLog;
using System.Threading;

namespace SPI_AOI.Views.MainConfigWindow
{
    public partial class PLCMonitor : Form
    {
        Devices.PLCComm mPLCComm = new Devices.PLCComm();
        Properties.Settings mParam = Properties.Settings.Default;
        Logger mLog = Heal.LogCtl.GetInstance();
        public PLCMonitor()
        {
            InitializeComponent();
        }

        private void PLCMonitor_Load(object sender, EventArgs e)
        {
            int ping = mPLCComm.Ping();
            if(ping == 0)
            {
                do
                {
                    int xTop = mPLCComm.Get_X_Top();
                    if (xTop < 0)
                    {
                        break;
                    }
                    int yTop = mPLCComm.Get_Y_Top();
                    if (yTop < 0)
                    {
                        break;
                    }
                    int xBot = mPLCComm.Get_X_Bot();
                    if (xBot < 0)
                    {
                        break;
                    }
                    int yBot = mPLCComm.Get_Y_Bot();
                    if (yBot < 0)
                    {
                        break;
                    }
                    int yConveyor = mPLCComm.Get_Conveyor();
                    if (yConveyor < 0)
                    {
                        break;
                    }
                    int speedTop = mPLCComm.Get_Speed_Setup_Top();
                    if (speedTop < 0)
                    {
                        break;
                    }
                    int speedBot = mPLCComm.Get_Speed_Setup_Bot();
                    if (speedBot < 0)
                    {
                        break;
                    }
                    int speedconveyor = mPLCComm.Get_Speed_Setup_Conveyor();
                    if (speedconveyor < 0)
                    {
                        break;
                    }
                    mPLCComm.Login();
                    txtX_Top.Text = xTop.ToString();
                    txtY_Top.Text = yTop.ToString();
                    txtX_Bot.Text = xBot.ToString();
                    txtX_Bot.Text = yBot.ToString();
                    txtY_Conveyor.Text = yConveyor.ToString();
                    txtSpeedTop.Text = speedTop.ToString();
                    txtSpeedBot.Text = speedBot.ToString();
                    txtSpeedConveyor.Text = speedconveyor.ToString();
                    return;
                }
                while (false);
                
            }
            pnMain.Enabled = false;
            MessageBox.Show("Cant connect to PLC, please check the connection!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void btPing_Click(object sender, EventArgs e)
        {
            int ping = mPLCComm.Ping();
            if(ping == 0)
            {
                MessageBox.Show("Ping sucessfully!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Ping Failed!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void PLCMonitor_FormClosing(object sender, FormClosingEventArgs e)
        {
            mPLCComm.Logout();
        }
        private void btGoTop_TopAxis_MouseDown(object sender, MouseEventArgs e)
        {
            mPLCComm.Login();
            mPLCComm.Set_Go_Up_Top();
        }

        private void btGoTop_TopAxis_MouseUp(object sender, MouseEventArgs e)
        {
            mPLCComm.Login();
            mPLCComm.Reset_Go_Up_Top();
        }

        

        private void btGoLeft_TopAxis_MouseDown(object sender, MouseEventArgs e)
        {
            mPLCComm.Login();
            mPLCComm.Set_Go_Left_Top();
        }

        private void btGoLeft_TopAxis_MouseUp(object sender, MouseEventArgs e)
        {
            mPLCComm.Login();
            mPLCComm.Reset_Go_Left_Top();
        }

        private void btGoBot_TopAxis_MouseDown(object sender, MouseEventArgs e)
        {
            mPLCComm.Login();
            mPLCComm.Set_Go_Down_Top();
        }

        private void btGoBot_TopAxis_MouseUp(object sender, MouseEventArgs e)
        {
            mPLCComm.Login();
            mPLCComm.Reset_Go_Down_Top();
        }

        private void btGoRight_TopAxis_MouseDown(object sender, MouseEventArgs e)
        {
            mPLCComm.Login();
            mPLCComm.Set_Go_Right_Top();
        }

        private void btGoRight_TopAxis_MouseUp(object sender, MouseEventArgs e)
        {
            mPLCComm.Login();
            mPLCComm.Reset_Go_Right_Top();
        }

        private void btGoTop_BotAxis_MouseDown(object sender, MouseEventArgs e)
        {
            mPLCComm.Login();
            mPLCComm.Set_Go_Up_Bot();
        }

        private void btGoTop_BotAxis_MouseUp(object sender, MouseEventArgs e)
        {
            mPLCComm.Login();
            mPLCComm.Reset_Go_Up_Bot();
        }

        private void btGoLeft_BotAxis_MouseDown(object sender, MouseEventArgs e)
        {
            mPLCComm.Login();
            mPLCComm.Set_Go_Left_Bot();
        }

        private void btGoLeft_BotAxis_MouseUp(object sender, MouseEventArgs e)
        {
            mPLCComm.Login();
            mPLCComm.Reset_Go_Left_Bot();
        }

        private void btGoBot_BotAxis_MouseDown(object sender, MouseEventArgs e)
        {
            mPLCComm.Login();
            mPLCComm.Set_Go_Down_Bot();
        }

        private void btGoBot_BotAxis_MouseUp(object sender, MouseEventArgs e)
        {
            mPLCComm.Login();
            mPLCComm.Reset_Go_Down_Bot();
        }

        private void btGoRight_BotAxis_MouseDown(object sender, MouseEventArgs e)
        {
            mPLCComm.Login();
            mPLCComm.Set_Go_Right_Bot();
        }

        private void btGoRight_BotAxis_MouseUp(object sender, MouseEventArgs e)
        {
            mPLCComm.Login();
            mPLCComm.Reset_Go_Right_Bot();
        }

        private void btGoTop_ConveyorAxis_MouseDown(object sender, MouseEventArgs e)
        {
            mPLCComm.Login();
            mPLCComm.Set_Go_Up_Conveyor();
        }

        private void btGoTop_ConveyorAxis_MouseUp(object sender, MouseEventArgs e)
        {
            mPLCComm.Login();
            mPLCComm.Reset_Go_Up_Conveyor();
        }

        private void btGoBot_ConveyorAxis_MouseDown(object sender, MouseEventArgs e)
        {
            mPLCComm.Login();
            mPLCComm.Set_Go_Up_Conveyor();
        }

        private void btGoBot_ConveyorAxis_MouseUp(object sender, MouseEventArgs e)
        {
            mPLCComm.Login();
            mPLCComm.Reset_Go_Up_Conveyor();
        }

        private void btLoadPanel_Click(object sender, EventArgs e)
        {
            mPLCComm.Login();
            mPLCComm.Set_Load_Product();
        }

        private void btUnloadPanel_Click(object sender, EventArgs e)
        {
            mPLCComm.Login();
            mPLCComm.Set_Unload_Product();
        }

        private void btGet_Click(object sender, EventArgs e)
        {
            string device = txtDeviceGet.Text;
            if(string.IsNullOrEmpty(device))
            {
                MessageBox.Show("Please insert device name!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            int value = mPLCComm.GetDevice2(device);
            txtValueGet.Text = value.ToString();
        }

        private void btSet_Click(object sender, EventArgs e)
        {
            string device = txtDeviceSet.Text;
            int value = 0;
            if (string.IsNullOrEmpty(device))
            {
                MessageBox.Show("Please insert device name!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                value = Convert.ToInt32(txtValueSet.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            mPLCComm.SetDevice2(device, value);
        }

        private void btHomeAll_Click(object sender, EventArgs e)
        {
            mPLCComm.Login();
            var r = MessageBox.Show("You want to GO HOME ALL AXIS AND CONVEYOR?", "Infomation", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            if(r == DialogResult.Yes)
            {
                mPLCComm.Set_Go_Home_All();
            }
        }

        private void btHomeTop_Click(object sender, EventArgs e)
        {
            mPLCComm.Login();
            var r = MessageBox.Show("You want to GO HOME TOP Axis?", "Infomation", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            if (r == DialogResult.Yes)
            {
                mPLCComm.Set_Go_Home_Top();
            }
        }

        private void btHomeScan_Click(object sender, EventArgs e)
        {
            mPLCComm.Login();
            var r = MessageBox.Show("You want to GO HOME BOT Axis?", "Infomation", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            if (r == DialogResult.Yes)
            {
                mPLCComm.Set_Go_Home_Bot();
            }
        }

        private void btHomeConveyor_Click(object sender, EventArgs e)
        {
            mPLCComm.Login();
            var r = MessageBox.Show("You want to GO HOME CONVEYOR?", "Infomation", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            if (r == DialogResult.Yes)
            {
                mPLCComm.Set_Go_Home_Conveyor();
            }
        }

        private void btGoTop_Click(object sender, EventArgs e)
        {
            mPLCComm.Login();
            int x = 0;
            int y = 0;
            try
            {
                x = Convert.ToInt32(txtX_Top.Text);
                y = Convert.ToInt32(txtY_Top.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            mPLCComm.SetXYTop(x, y);
            mPLCComm.Set_Write_Coordinates_Finish_Setup_Top();
        }

        private void btGoBot_Click(object sender, EventArgs e)
        {
            mPLCComm.Login();
            int x = 0;
            int y = 0;
            try
            {
                x = Convert.ToInt32(txtX_Bot.Text);
                y = Convert.ToInt32(txtY_Bot.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            mPLCComm.SetXYBot(x, y);
            mPLCComm.Set_Write_Coordinates_Finish_Setup_Bot();
        }

        private void btGoConveyor_Click(object sender, EventArgs e)
        {
            mPLCComm.Login();
            int y = 0;
            try
            {
                y = Convert.ToInt32(txtY_Conveyor.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            mPLCComm.Set_Conveyor(y);
            mPLCComm.Set_Write_Coordinates_Finish_Setup_Conveyor();
        }

        private void btSetSpeedTop_Click(object sender, EventArgs e)
        {
            mPLCComm.Login();
            int speed = 0;
            try
            {
                speed = Convert.ToInt32(txtSpeedTop.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            mPLCComm.Set_Speed_Top(speed);
        }

        private void btSetSpeedBot_Click(object sender, EventArgs e)
        {
            mPLCComm.Login();
            int speed = 0;
            try
            {
                speed = Convert.ToInt32(txtSpeedBot.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            mPLCComm.Set_Speed_Bot(speed);
        }

        private void btSetSpeedConveyor_Click(object sender, EventArgs e)
        {
            mPLCComm.Login();
            int speed = 0;
            try
            {
                speed = Convert.ToInt32(txtSpeedConveyor.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            mPLCComm.Set_Speed_Conveyor(speed);
        }
    }
}
