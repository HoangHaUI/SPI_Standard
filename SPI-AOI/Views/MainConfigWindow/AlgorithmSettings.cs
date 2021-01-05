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
    public partial class AlgorithmSettings : Form
    {
        bool mLoaded = false;
        Properties.Settings mParam = Properties.Settings.Default;
        public AlgorithmSettings()
        {
            InitializeComponent();
        }

        private void AlgorithmSettings_Load(object sender, EventArgs e)
        {
            LoadUI();
        }
        private void LoadUI()
        {
            switch (mParam.RUNNING_MODE)
            {
                case 0:
                    rbControlRun.Checked = true;
                    break;
                case 1:
                    rbTesting.Checked = true;
                    break;
                case 2:
                    rbByPass.Checked = true;
                    break;
                default:
                    break;
            }
            switch (mParam.LIGHT_MODE)
            {
                case 0:
                    rbLightStrobeMode.Checked = true;
                    break;
                case 1:
                    rbLightConstantMode.Checked = true;
                    break;
                default:
                    break;
            }
            nFOVW.Value = mParam.FOV.Width;
            nFOVH.Value = mParam.FOV.Height;
            nPulsePerPixelX.Value = Convert.ToDecimal(mParam.PULSE_X_PER_PIXEL_DEFAULT);
            nPulsePerPixelY.Value = Convert.ToDecimal(mParam.PULSE_Y_PER_PIXEL_DEFAULT);
            nPulseScaleX.Value = Convert.ToDecimal(mParam.SCALE_PULSE_X);
            nPulseScaleY.Value = Convert.ToDecimal(mParam.SCALE_PULSE_Y);
            nAngleCameraX.Value = Convert.ToDecimal(mParam.CAMERA_X_AXIS_ANGLE);
            nAngleXY.Value = Convert.ToDecimal(mParam.XY_AXIS_ANGLE);
            nDPIDefault.Value = Convert.ToDecimal(mParam.DPI_DEFAULT);
            nDPIScale.Value = Convert.ToDecimal(mParam.DPI_SCALE);
            nThicknessDefault.Value = Convert.ToDecimal(mParam.THICKNESS_DEFAULT);
            nExposureTime.Value = Convert.ToDecimal(mParam.CAMERA_SETUP_EXPOSURE_TIME);
            nGain.Value = Convert.ToDecimal(mParam.CAMERA_GAIN);
            nLightCH1.Value = Convert.ToDecimal(mParam.LIGHT_SETUP_DEFAULT_INTENSITY_CH1);
            nLightCH2.Value = Convert.ToDecimal(mParam.LIGHT_SETUP_DEFAULT_INTENSITY_CH2);
            nLightCH3.Value = Convert.ToDecimal(mParam.LIGHT_SETUP_DEFAULT_INTENSITY_CH3);
            nLightCH4.Value = Convert.ToDecimal(mParam.LIGHT_SETUP_DEFAULT_INTENSITY_CH4);
            mLoaded = true;
        }

        private void rbControlRun_CheckedChanged(object sender, EventArgs e)
        {
            if (!mLoaded)
                return;
            RadioButton rb = sender as RadioButton;
            if(rb.Checked)
                mParam.RUNNING_MODE = 0;
            mParam.Save();
        }

        private void rbByPass_CheckedChanged(object sender, EventArgs e)
        {
            if (!mLoaded)
                return;
            RadioButton rb = sender as RadioButton;
            if (rb.Checked)
                mParam.RUNNING_MODE = 2;
            mParam.Save();
        }

        private void rbTesting_CheckedChanged(object sender, EventArgs e)
        {
            if (!mLoaded)
                return;
            RadioButton rb = sender as RadioButton;
            if (rb.Checked)
                mParam.RUNNING_MODE = 1;
            mParam.Save();
        }

        private void rbLightStrobeMode_CheckedChanged(object sender, EventArgs e)
        {
            if (!mLoaded)
                return;
            RadioButton rb = sender as RadioButton;
            if (rb.Checked)
                mParam.LIGHT_MODE = 0;
            mParam.Save();
        }

        private void rbLightConstantMode_CheckedChanged(object sender, EventArgs e)
        {
            if (!mLoaded)
                return;
            RadioButton rb = sender as RadioButton;
            if (rb.Checked)
                mParam.LIGHT_MODE = 1;
            mParam.Save();
        }

        private void nPulsePerPixelX_ValueChanged(object sender, EventArgs e)
        {
            if (!mLoaded)
                return;
            NumericUpDown numeric = sender as NumericUpDown;
            mParam.PULSE_X_PER_PIXEL_DEFAULT = Convert.ToDouble(numeric.Value);
            mParam.Save();
        }

        private void nPulsePerPixelY_ValueChanged(object sender, EventArgs e)
        {
            if (!mLoaded)
                return;
            NumericUpDown numeric = sender as NumericUpDown;
            mParam.PULSE_Y_PER_PIXEL_DEFAULT = Convert.ToDouble(numeric.Value);
            mParam.Save();
        }

        private void nPulseScaleX_ValueChanged(object sender, EventArgs e)
        {
            if (!mLoaded)
                return;
            NumericUpDown numeric = sender as NumericUpDown;
            mParam.SCALE_PULSE_X = Convert.ToDouble(numeric.Value);
            mParam.Save();
        }

        private void nPulseScaleY_ValueChanged(object sender, EventArgs e)
        {
            if (!mLoaded)
                return;
            NumericUpDown numeric = sender as NumericUpDown;
            mParam.SCALE_PULSE_Y = Convert.ToDouble(numeric.Value);
            mParam.Save();
        }

        private void nAngleCameraX_ValueChanged(object sender, EventArgs e)
        {
            if (!mLoaded)
                return;
            NumericUpDown numeric = sender as NumericUpDown;
            mParam.CAMERA_X_AXIS_ANGLE = Convert.ToDouble(numeric.Value);
            mParam.Save();
        }

        private void nAngleXY_ValueChanged(object sender, EventArgs e)
        {
            if (!mLoaded)
                return;
            NumericUpDown numeric = sender as NumericUpDown;
            mParam.XY_AXIS_ANGLE = Convert.ToDouble(numeric.Value);
            mParam.Save();
        }

        private void nDPIDefault_ValueChanged(object sender, EventArgs e)
        {
            if (!mLoaded)
                return;
            NumericUpDown numeric = sender as NumericUpDown;
            mParam.DPI_DEFAULT = (float)Convert.ToDouble(numeric.Value);
            mParam.Save();
        }

        private void nDPIScale_ValueChanged(object sender, EventArgs e)
        {
            if (!mLoaded)
                return;
            NumericUpDown numeric = sender as NumericUpDown;
            mParam.DPI_SCALE = Convert.ToDouble(numeric.Value);
            mParam.Save();
        }

        private void nThicknessDefault_ValueChanged(object sender, EventArgs e)
        {
            if (!mLoaded)
                return;
            NumericUpDown numeric = sender as NumericUpDown;
            mParam.THICKNESS_DEFAULT = Convert.ToDouble(numeric.Value);
            mParam.Save();
        }

        private void nExposureTime_ValueChanged(object sender, EventArgs e)
        {
            if (!mLoaded)
                return;
            NumericUpDown numeric = sender as NumericUpDown;
            mParam.CAMERA_SETUP_EXPOSURE_TIME = Convert.ToInt32(numeric.Value);
            mParam.Save();
        }

        private void nGain_ValueChanged(object sender, EventArgs e)
        {
            if (!mLoaded)
                return;
            NumericUpDown numeric = sender as NumericUpDown;
            mParam.CAMERA_GAIN = Convert.ToDouble(numeric.Value);
            mParam.Save();
        }

        private void nLightCH1_ValueChanged(object sender, EventArgs e)
        {
            if (!mLoaded)
                return;
            NumericUpDown numeric = sender as NumericUpDown;
            mParam.LIGHT_SETUP_DEFAULT_INTENSITY_CH1 = Convert.ToInt32(numeric.Value);
            mParam.Save();
        }

        private void nLightCH2_ValueChanged(object sender, EventArgs e)
        {
            if (!mLoaded)
                return;
            NumericUpDown numeric = sender as NumericUpDown;
            mParam.LIGHT_SETUP_DEFAULT_INTENSITY_CH2 = Convert.ToInt32(numeric.Value);
            mParam.Save();
        }

        private void nLightCH3_ValueChanged(object sender, EventArgs e)
        {
            if (!mLoaded)
                return;
            NumericUpDown numeric = sender as NumericUpDown;
            mParam.LIGHT_SETUP_DEFAULT_INTENSITY_CH3 = Convert.ToInt32(numeric.Value);
            mParam.Save();
        }
        private void nLightCH4_ValueChanged(object sender, EventArgs e)
        {
            if (!mLoaded)
                return;
            NumericUpDown numeric = sender as NumericUpDown;
            mParam.LIGHT_SETUP_DEFAULT_INTENSITY_CH4 = Convert.ToInt32(numeric.Value);
            mParam.Save();
        }
        private void nFOVW_ValueChanged(object sender, EventArgs e)
        {
            if (!mLoaded)
                return;
            NumericUpDown numeric = sender as NumericUpDown;
            mParam.FOV = new Size(Convert.ToInt32(numeric.Value), mParam.FOV.Height);
            mParam.Save();
        }
        private void nFOVH_ValueChanged(object sender, EventArgs e)
        {
            if (!mLoaded)
                return;
            NumericUpDown numeric = sender as NumericUpDown;
            mParam.FOV = new Size(mParam.FOV.Width, Convert.ToInt32(numeric.Value));
            mParam.Save();
        }
    }
}
