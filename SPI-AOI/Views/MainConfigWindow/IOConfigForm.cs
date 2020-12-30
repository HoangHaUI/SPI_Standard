using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.IO.Ports;

namespace SPI_AOI.Views.MainConfigWindow
{
    public partial class IOConfigForm : Form
    {
        Properties.Settings mParam = Properties.Settings.Default;
        bool mLoaded = false;
        public IOConfigForm()
        {
            InitializeComponent();
        }
        private void LoadUI()
        {
            string[] ports = SerialPort.GetPortNames();
            cbLightPort.Items.Clear();
            cbScanner.Items.Clear();
            cbLightPort.Items.AddRange(ports);
            cbScanner.Items.AddRange(ports);
            if(ports.Contains(mParam.LIGHT_COM))
            {
                cbLightPort.SelectedItem = mParam.LIGHT_COM;
            }
            if (ports.Contains(mParam.SCANER_COM))
            {
                cbScanner.SelectedItem = mParam.SCANER_COM;
            }
            txtCameraMatrix.Text = mParam.CAMERA_MATRIX_FILE;
            txtCameraDistcoeffs.Text = mParam.CAMERA_DISTCOEFFS_FILE;
            nFOVW.Value = mParam.IMAGE_SIZE.Width;
            nFOVH.Value = mParam.IMAGE_SIZE.Height;
            nSaveDays.Value = mParam.SAVE_IMAGE_HOURS;
            txtSavePath.Text = mParam.SAVE_IMAGE_PATH;
            mLoaded = true;
        }
        private void IOConfigForm_Load(object sender, EventArgs e)
        {
            LoadUI();
        }

        private void btBrowserCameraMatrix_Click(object sender, EventArgs e)
        {
            if (!mLoaded)
                return;
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                if(ofd.ShowDialog() == DialogResult.OK)
                {
                    mParam.CAMERA_MATRIX_FILE = ofd.FileName;
                }
            }
            mParam.Save();
        }

        private void btBrowserCameraDistcoeffs_Click(object sender, EventArgs e)
        {
            if (!mLoaded)
                return;
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    mParam.CAMERA_DISTCOEFFS_FILE = ofd.FileName;
                }
            }
            mParam.Save();
        }

        private void cbLightPort_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!mLoaded)
                return;
            ComboBox cb = sender as ComboBox;
            if(cb.SelectedIndex >= 0)
            {
                mParam.LIGHT_COM = cb.SelectedItem.ToString();
            }
            mParam.Save();
        }

        private void cbScanner_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!mLoaded)
                return;
            ComboBox cb = sender as ComboBox;
            if (cb.SelectedIndex >= 0)
            {
                mParam.SCANER_COM = cb.SelectedItem.ToString();
            }
            mParam.Save();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!mLoaded)
                return;

            using (FolderBrowserDialog ofd = new FolderBrowserDialog())
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    mParam.SAVE_IMAGE_PATH = ofd.SelectedPath;
                }
            }
            mParam.Save();
        }

        private void nSaveDays_ValueChanged(object sender, EventArgs e)
        {
            if (!mLoaded)
                return;
            NumericUpDown n = sender as NumericUpDown;
            mParam.SAVE_IMAGE_HOURS = Convert.ToInt32(n.Value);
            mParam.Save();
        }

        private void nFOVW_ValueChanged(object sender, EventArgs e)
        {
            if (!mLoaded)
                return;
            NumericUpDown n = sender as NumericUpDown;
            mParam.IMAGE_SIZE = new Size( Convert.ToInt32(n.Value), mParam.IMAGE_SIZE.Height);
            mParam.Save();
        }

        private void nFOVH_ValueChanged(object sender, EventArgs e)
        {
            if (!mLoaded)
                return;
            NumericUpDown n = sender as NumericUpDown;
            mParam.IMAGE_SIZE = new Size(mParam.IMAGE_SIZE.Width, Convert.ToInt32(n.Value));
            mParam.Save();
        }
    }
}
