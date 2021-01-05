using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;

namespace SPI_AOI.Views.MainConfigWindow
{
    public partial class LightCtlForm : Form
    {
        Properties.Settings mParam = Properties.Settings.Default;
        Devices.DKZ224V4ACCom mLight = null;
        bool mIsOpen = false;
        public LightCtlForm()
        {
            InitializeComponent();
        }

        private void LightCtlForm_Load(object sender, EventArgs e)
        {
            string[] ports = SerialPort.GetPortNames();
            cbCom.Items.AddRange(ports);
            if (ports.Contains(mParam.LIGHT_COM))
            {
                cbCom.SelectedItem = mParam.LIGHT_COM;
            }
            ActiveUI();
        }

        private void btOpen_Click(object sender, EventArgs e)
        {
            if(cbCom.SelectedIndex >= 0)
            {
                mLight = new Devices.DKZ224V4ACCom(cbCom.SelectedItem.ToString());
                if (mLight.Open() == 0)
                {
                    mIsOpen = true;
                    ActiveUI();
                    int[] values = mLight.GetStatus();
                    if(values != null)
                    {
                        nIntensityCH1.Value = values[0];
                        nIntensityCH2.Value = values[1];
                        nIntensityCH3.Value = values[2];
                        nIntensityCH4.Value = values[3];
                    }
                }
            }
           
        }
        private void ActiveUI()
        {
            grCH1.Enabled = mIsOpen;
            grCH2.Enabled = mIsOpen;
            grCH3.Enabled = mIsOpen;
            grCH4.Enabled = mIsOpen;
        }

        private void btClose_Click(object sender, EventArgs e)
        {
            if (mLight != null)
            {
                if (mLight.Close() == 0)
                {
                    mIsOpen = false;
                }
                else
                {
                    MessageBox.Show("cant close Lightsource");
                }
                ActiveUI();
            }
        }

        private void btOnCH1_Click(object sender, EventArgs e)
        {
            mLight.ActiveOne(1, 1);
        }

        private void btOnCH2_Click(object sender, EventArgs e)
        {
            mLight.ActiveOne(2, 1);
        }

        private void btOnCH3_Click(object sender, EventArgs e)
        {
            mLight.ActiveOne(3, 1);
        }

        private void btOnCH4_Click(object sender, EventArgs e)
        {
            mLight.ActiveOne(4, 1);
        }

        private void btOffCH1_Click(object sender, EventArgs e)
        {
            mLight.ActiveOne(1, 0);
        }

        private void btOffCH2_Click(object sender, EventArgs e)
        {
            mLight.ActiveOne(2, 0);
        }

        private void btOffCH3_Click(object sender, EventArgs e)
        {
            mLight.ActiveOne(3, 0);
        }

        private void btOffCH4_Click(object sender, EventArgs e)
        {
            mLight.ActiveOne(4, 0);
        }

        private void nIntensityCH1_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDown num = sender as NumericUpDown;
            int value = (int)num.Value;
            mLight.SetOne(1, value);
        }

        private void nIntensityCH2_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDown num = sender as NumericUpDown;
            int value = (int)num.Value;
            mLight.SetOne(2, value);
        }

        private void nIntensityCH3_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDown num = sender as NumericUpDown;
            int value = (int)num.Value;
            mLight.SetOne(3, value);
        }

        private void nIntensityCH4_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDown num = sender as NumericUpDown;
            int value = (int)num.Value;
            mLight.SetOne(4, value);
        }

        private void LightCtlForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            btClose_Click(null, null);
        }
    }
}
