namespace SPI_AOI.Views.MainConfigWindow
{
    partial class AlgorithmSettings
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.rbControlRun = new System.Windows.Forms.RadioButton();
            this.rbTesting = new System.Windows.Forms.RadioButton();
            this.rbByPass = new System.Windows.Forms.RadioButton();
            this.nFOVW = new System.Windows.Forms.NumericUpDown();
            this.nFOVH = new System.Windows.Forms.NumericUpDown();
            this.nPulsePerPixelX = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.nPulsePerPixelY = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.nAngleCameraX = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.nAngleXY = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.nPulseScaleX = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.nPulseScaleY = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            this.nDPIDefault = new System.Windows.Forms.NumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this.nDPIScale = new System.Windows.Forms.NumericUpDown();
            this.panel2 = new System.Windows.Forms.Panel();
            this.rbLightStrobeMode = new System.Windows.Forms.RadioButton();
            this.label11 = new System.Windows.Forms.Label();
            this.rbLightConstantMode = new System.Windows.Forms.RadioButton();
            this.label12 = new System.Windows.Forms.Label();
            this.nExposureTime = new System.Windows.Forms.NumericUpDown();
            this.label13 = new System.Windows.Forms.Label();
            this.nGain = new System.Windows.Forms.NumericUpDown();
            this.label15 = new System.Windows.Forms.Label();
            this.nLightCH1 = new System.Windows.Forms.NumericUpDown();
            this.nLightCH2 = new System.Windows.Forms.NumericUpDown();
            this.label16 = new System.Windows.Forms.Label();
            this.nLightCH3 = new System.Windows.Forms.NumericUpDown();
            this.label17 = new System.Windows.Forms.Label();
            this.nLightCH4 = new System.Windows.Forms.NumericUpDown();
            this.label18 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.nThicknessDefault = new System.Windows.Forms.NumericUpDown();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nFOVW)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nFOVH)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nPulsePerPixelX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nPulsePerPixelY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nAngleCameraX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nAngleXY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nPulseScaleX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nPulseScaleY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nDPIDefault)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nDPIScale)).BeginInit();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nExposureTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nGain)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nLightCH1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nLightCH2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nLightCH3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nLightCH4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nThicknessDefault)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(147, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(31, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "FOV:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(80, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Running Mode:";
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.rbByPass);
            this.panel1.Controls.Add(this.rbTesting);
            this.panel1.Controls.Add(this.rbControlRun);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Location = new System.Drawing.Point(8, 8);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(114, 111);
            this.panel1.TabIndex = 2;
            // 
            // rbControlRun
            // 
            this.rbControlRun.AutoSize = true;
            this.rbControlRun.Location = new System.Drawing.Point(13, 33);
            this.rbControlRun.Name = "rbControlRun";
            this.rbControlRun.Size = new System.Drawing.Size(81, 17);
            this.rbControlRun.TabIndex = 2;
            this.rbControlRun.Text = "Control Run";
            this.rbControlRun.UseVisualStyleBackColor = true;
            this.rbControlRun.CheckedChanged += new System.EventHandler(this.rbControlRun_CheckedChanged);
            // 
            // rbTesting
            // 
            this.rbTesting.AutoSize = true;
            this.rbTesting.Checked = true;
            this.rbTesting.Location = new System.Drawing.Point(13, 56);
            this.rbTesting.Name = "rbTesting";
            this.rbTesting.Size = new System.Drawing.Size(60, 17);
            this.rbTesting.TabIndex = 3;
            this.rbTesting.TabStop = true;
            this.rbTesting.Text = "Testing";
            this.rbTesting.UseVisualStyleBackColor = true;
            this.rbTesting.CheckedChanged += new System.EventHandler(this.rbTesting_CheckedChanged);
            // 
            // rbByPass
            // 
            this.rbByPass.AutoSize = true;
            this.rbByPass.Location = new System.Drawing.Point(13, 79);
            this.rbByPass.Name = "rbByPass";
            this.rbByPass.Size = new System.Drawing.Size(63, 17);
            this.rbByPass.TabIndex = 4;
            this.rbByPass.Text = "By Pass";
            this.rbByPass.UseVisualStyleBackColor = true;
            this.rbByPass.CheckedChanged += new System.EventHandler(this.rbByPass_CheckedChanged);
            // 
            // nFOVW
            // 
            this.nFOVW.Location = new System.Drawing.Point(215, 10);
            this.nFOVW.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.nFOVW.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nFOVW.Name = "nFOVW";
            this.nFOVW.Size = new System.Drawing.Size(69, 20);
            this.nFOVW.TabIndex = 3;
            this.nFOVW.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nFOVW.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nFOVW.ValueChanged += new System.EventHandler(this.nFOVW_ValueChanged);
            // 
            // nFOVH
            // 
            this.nFOVH.Location = new System.Drawing.Point(290, 10);
            this.nFOVH.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.nFOVH.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nFOVH.Name = "nFOVH";
            this.nFOVH.Size = new System.Drawing.Size(69, 20);
            this.nFOVH.TabIndex = 4;
            this.nFOVH.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nFOVH.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nFOVH.ValueChanged += new System.EventHandler(this.nFOVH_ValueChanged);
            // 
            // nPulsePerPixelX
            // 
            this.nPulsePerPixelX.DecimalPlaces = 9;
            this.nPulsePerPixelX.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.nPulsePerPixelX.Location = new System.Drawing.Point(244, 36);
            this.nPulsePerPixelX.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.nPulsePerPixelX.Name = "nPulsePerPixelX";
            this.nPulsePerPixelX.Size = new System.Drawing.Size(115, 20);
            this.nPulsePerPixelX.TabIndex = 5;
            this.nPulsePerPixelX.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nPulsePerPixelX.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nPulsePerPixelX.ValueChanged += new System.EventHandler(this.nPulsePerPixelX_ValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(147, 38);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(90, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Pulse Per Pixel X:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(147, 64);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(90, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Pulse Per Pixel Y:";
            // 
            // nPulsePerPixelY
            // 
            this.nPulsePerPixelY.DecimalPlaces = 9;
            this.nPulsePerPixelY.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.nPulsePerPixelY.Location = new System.Drawing.Point(244, 62);
            this.nPulsePerPixelY.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.nPulsePerPixelY.Name = "nPulsePerPixelY";
            this.nPulsePerPixelY.Size = new System.Drawing.Size(115, 20);
            this.nPulsePerPixelY.TabIndex = 7;
            this.nPulsePerPixelY.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nPulsePerPixelY.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nPulsePerPixelY.ValueChanged += new System.EventHandler(this.nPulsePerPixelY_ValueChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(147, 142);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(92, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "Angle Camera - X:";
            // 
            // nAngleCameraX
            // 
            this.nAngleCameraX.DecimalPlaces = 9;
            this.nAngleCameraX.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.nAngleCameraX.Location = new System.Drawing.Point(244, 140);
            this.nAngleCameraX.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.nAngleCameraX.Name = "nAngleCameraX";
            this.nAngleCameraX.Size = new System.Drawing.Size(115, 20);
            this.nAngleCameraX.TabIndex = 9;
            this.nAngleCameraX.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nAngleCameraX.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nAngleCameraX.ValueChanged += new System.EventHandler(this.nAngleCameraX_ValueChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(147, 168);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(57, 13);
            this.label6.TabIndex = 12;
            this.label6.Text = "Angle X-Y:";
            // 
            // nAngleXY
            // 
            this.nAngleXY.DecimalPlaces = 9;
            this.nAngleXY.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.nAngleXY.Location = new System.Drawing.Point(244, 166);
            this.nAngleXY.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.nAngleXY.Name = "nAngleXY";
            this.nAngleXY.Size = new System.Drawing.Size(115, 20);
            this.nAngleXY.TabIndex = 11;
            this.nAngleXY.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nAngleXY.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nAngleXY.ValueChanged += new System.EventHandler(this.nAngleXY_ValueChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(147, 90);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(76, 13);
            this.label7.TabIndex = 14;
            this.label7.Text = "Pulse X Scale:";
            // 
            // nPulseScaleX
            // 
            this.nPulseScaleX.DecimalPlaces = 9;
            this.nPulseScaleX.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.nPulseScaleX.Location = new System.Drawing.Point(244, 88);
            this.nPulseScaleX.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.nPulseScaleX.Name = "nPulseScaleX";
            this.nPulseScaleX.Size = new System.Drawing.Size(115, 20);
            this.nPulseScaleX.TabIndex = 13;
            this.nPulseScaleX.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nPulseScaleX.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nPulseScaleX.ValueChanged += new System.EventHandler(this.nPulseScaleX_ValueChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(147, 116);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(76, 13);
            this.label8.TabIndex = 16;
            this.label8.Text = "Pulse Y Scale:";
            // 
            // nPulseScaleY
            // 
            this.nPulseScaleY.DecimalPlaces = 9;
            this.nPulseScaleY.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.nPulseScaleY.Location = new System.Drawing.Point(244, 114);
            this.nPulseScaleY.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.nPulseScaleY.Name = "nPulseScaleY";
            this.nPulseScaleY.Size = new System.Drawing.Size(115, 20);
            this.nPulseScaleY.TabIndex = 15;
            this.nPulseScaleY.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nPulseScaleY.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nPulseScaleY.ValueChanged += new System.EventHandler(this.nPulseScaleY_ValueChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(147, 194);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(28, 13);
            this.label9.TabIndex = 18;
            this.label9.Text = "DPI:";
            // 
            // nDPIDefault
            // 
            this.nDPIDefault.DecimalPlaces = 9;
            this.nDPIDefault.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.nDPIDefault.Location = new System.Drawing.Point(244, 192);
            this.nDPIDefault.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.nDPIDefault.Name = "nDPIDefault";
            this.nDPIDefault.Size = new System.Drawing.Size(115, 20);
            this.nDPIDefault.TabIndex = 17;
            this.nDPIDefault.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nDPIDefault.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nDPIDefault.ValueChanged += new System.EventHandler(this.nDPIDefault_ValueChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(147, 220);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(58, 13);
            this.label10.TabIndex = 20;
            this.label10.Text = "DPI Scale:";
            // 
            // nDPIScale
            // 
            this.nDPIScale.DecimalPlaces = 9;
            this.nDPIScale.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.nDPIScale.Location = new System.Drawing.Point(244, 218);
            this.nDPIScale.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.nDPIScale.Name = "nDPIScale";
            this.nDPIScale.Size = new System.Drawing.Size(115, 20);
            this.nDPIScale.TabIndex = 19;
            this.nDPIScale.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nDPIScale.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nDPIScale.ValueChanged += new System.EventHandler(this.nDPIScale_ValueChanged);
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.rbLightConstantMode);
            this.panel2.Controls.Add(this.rbLightStrobeMode);
            this.panel2.Controls.Add(this.label11);
            this.panel2.Location = new System.Drawing.Point(8, 125);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(114, 87);
            this.panel2.TabIndex = 5;
            // 
            // rbLightStrobeMode
            // 
            this.rbLightStrobeMode.AutoSize = true;
            this.rbLightStrobeMode.Checked = true;
            this.rbLightStrobeMode.Location = new System.Drawing.Point(13, 33);
            this.rbLightStrobeMode.Name = "rbLightStrobeMode";
            this.rbLightStrobeMode.Size = new System.Drawing.Size(56, 17);
            this.rbLightStrobeMode.TabIndex = 2;
            this.rbLightStrobeMode.TabStop = true;
            this.rbLightStrobeMode.Text = "Strobe";
            this.rbLightStrobeMode.UseVisualStyleBackColor = true;
            this.rbLightStrobeMode.CheckedChanged += new System.EventHandler(this.rbLightStrobeMode_CheckedChanged);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(14, 9);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(77, 13);
            this.label11.TabIndex = 1;
            this.label11.Text = "Lighting Mode:";
            // 
            // rbLightConstantMode
            // 
            this.rbLightConstantMode.AutoSize = true;
            this.rbLightConstantMode.Location = new System.Drawing.Point(13, 56);
            this.rbLightConstantMode.Name = "rbLightConstantMode";
            this.rbLightConstantMode.Size = new System.Drawing.Size(67, 17);
            this.rbLightConstantMode.TabIndex = 3;
            this.rbLightConstantMode.Text = "Constant";
            this.rbLightConstantMode.UseVisualStyleBackColor = true;
            this.rbLightConstantMode.CheckedChanged += new System.EventHandler(this.rbLightConstantMode_CheckedChanged);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(395, 12);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(80, 13);
            this.label12.TabIndex = 22;
            this.label12.Text = "Exposure Time:";
            // 
            // nExposureTime
            // 
            this.nExposureTime.Location = new System.Drawing.Point(492, 10);
            this.nExposureTime.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.nExposureTime.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nExposureTime.Name = "nExposureTime";
            this.nExposureTime.Size = new System.Drawing.Size(115, 20);
            this.nExposureTime.TabIndex = 21;
            this.nExposureTime.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nExposureTime.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nExposureTime.ValueChanged += new System.EventHandler(this.nExposureTime_ValueChanged);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(395, 38);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(32, 13);
            this.label13.TabIndex = 24;
            this.label13.Text = "Gain:";
            // 
            // nGain
            // 
            this.nGain.Increment = new decimal(new int[] {
            1,
            0,
            0,
            262144});
            this.nGain.Location = new System.Drawing.Point(492, 36);
            this.nGain.Maximum = new decimal(new int[] {
            15,
            0,
            0,
            0});
            this.nGain.Name = "nGain";
            this.nGain.Size = new System.Drawing.Size(115, 20);
            this.nGain.TabIndex = 23;
            this.nGain.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nGain.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nGain.ValueChanged += new System.EventHandler(this.nGain_ValueChanged);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(395, 67);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(57, 13);
            this.label15.TabIndex = 27;
            this.label15.Text = "Light CH1:";
            // 
            // nLightCH1
            // 
            this.nLightCH1.Location = new System.Drawing.Point(538, 65);
            this.nLightCH1.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nLightCH1.Name = "nLightCH1";
            this.nLightCH1.Size = new System.Drawing.Size(69, 20);
            this.nLightCH1.TabIndex = 28;
            this.nLightCH1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nLightCH1.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nLightCH1.ValueChanged += new System.EventHandler(this.nLightCH1_ValueChanged);
            // 
            // nLightCH2
            // 
            this.nLightCH2.Location = new System.Drawing.Point(538, 91);
            this.nLightCH2.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nLightCH2.Name = "nLightCH2";
            this.nLightCH2.Size = new System.Drawing.Size(69, 20);
            this.nLightCH2.TabIndex = 30;
            this.nLightCH2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nLightCH2.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nLightCH2.ValueChanged += new System.EventHandler(this.nLightCH2_ValueChanged);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(395, 93);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(57, 13);
            this.label16.TabIndex = 29;
            this.label16.Text = "Light CH2:";
            // 
            // nLightCH3
            // 
            this.nLightCH3.Location = new System.Drawing.Point(538, 117);
            this.nLightCH3.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nLightCH3.Name = "nLightCH3";
            this.nLightCH3.Size = new System.Drawing.Size(69, 20);
            this.nLightCH3.TabIndex = 32;
            this.nLightCH3.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nLightCH3.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nLightCH3.ValueChanged += new System.EventHandler(this.nLightCH3_ValueChanged);
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(395, 119);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(57, 13);
            this.label17.TabIndex = 31;
            this.label17.Text = "Light CH3:";
            // 
            // nLightCH4
            // 
            this.nLightCH4.Location = new System.Drawing.Point(538, 143);
            this.nLightCH4.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nLightCH4.Name = "nLightCH4";
            this.nLightCH4.Size = new System.Drawing.Size(69, 20);
            this.nLightCH4.TabIndex = 34;
            this.nLightCH4.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nLightCH4.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nLightCH4.ValueChanged += new System.EventHandler(this.nLightCH4_ValueChanged);
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(395, 145);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(57, 13);
            this.label18.TabIndex = 33;
            this.label18.Text = "Light CH4:";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(147, 246);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(59, 13);
            this.label19.TabIndex = 36;
            this.label19.Text = "Thickness:";
            // 
            // nThicknessDefault
            // 
            this.nThicknessDefault.DecimalPlaces = 9;
            this.nThicknessDefault.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.nThicknessDefault.Location = new System.Drawing.Point(244, 244);
            this.nThicknessDefault.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.nThicknessDefault.Name = "nThicknessDefault";
            this.nThicknessDefault.Size = new System.Drawing.Size(115, 20);
            this.nThicknessDefault.TabIndex = 35;
            this.nThicknessDefault.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nThicknessDefault.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nThicknessDefault.ValueChanged += new System.EventHandler(this.nThicknessDefault_ValueChanged);
            // 
            // AlgorithmSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(631, 281);
            this.Controls.Add(this.label19);
            this.Controls.Add(this.nThicknessDefault);
            this.Controls.Add(this.nLightCH4);
            this.Controls.Add(this.label18);
            this.Controls.Add(this.nLightCH3);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.nLightCH2);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.nLightCH1);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.nGain);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.nExposureTime);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.nDPIScale);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.nDPIDefault);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.nPulseScaleY);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.nPulseScaleX);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.nAngleXY);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.nAngleCameraX);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.nPulsePerPixelY);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.nPulsePerPixelX);
            this.Controls.Add(this.nFOVH);
            this.Controls.Add(this.nFOVW);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AlgorithmSettings";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Algorithm Settings";
            this.Load += new System.EventHandler(this.AlgorithmSettings_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nFOVW)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nFOVH)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nPulsePerPixelX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nPulsePerPixelY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nAngleCameraX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nAngleXY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nPulseScaleX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nPulseScaleY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nDPIDefault)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nDPIScale)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nExposureTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nGain)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nLightCH1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nLightCH2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nLightCH3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nLightCH4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nThicknessDefault)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton rbByPass;
        private System.Windows.Forms.RadioButton rbTesting;
        private System.Windows.Forms.RadioButton rbControlRun;
        private System.Windows.Forms.NumericUpDown nFOVW;
        private System.Windows.Forms.NumericUpDown nFOVH;
        private System.Windows.Forms.NumericUpDown nPulsePerPixelX;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown nPulsePerPixelY;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown nAngleCameraX;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown nAngleXY;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown nPulseScaleX;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.NumericUpDown nPulseScaleY;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.NumericUpDown nDPIDefault;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.NumericUpDown nDPIScale;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.RadioButton rbLightStrobeMode;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.RadioButton rbLightConstantMode;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.NumericUpDown nExposureTime;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.NumericUpDown nGain;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.NumericUpDown nLightCH1;
        private System.Windows.Forms.NumericUpDown nLightCH2;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.NumericUpDown nLightCH3;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.NumericUpDown nLightCH4;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.NumericUpDown nThicknessDefault;
    }
}