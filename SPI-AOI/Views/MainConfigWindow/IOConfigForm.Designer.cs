namespace SPI_AOI.Views.MainConfigWindow
{
    partial class IOConfigForm
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
            this.cbLightPort = new System.Windows.Forms.ComboBox();
            this.cbScanner = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.nFOVW = new System.Windows.Forms.NumericUpDown();
            this.nFOVH = new System.Windows.Forms.NumericUpDown();
            this.btSelectSavePath = new System.Windows.Forms.Button();
            this.txtSavePath = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.nSaveDays = new System.Windows.Forms.NumericUpDown();
            this.panel2 = new System.Windows.Forms.Panel();
            this.rbLightConstantMode = new System.Windows.Forms.RadioButton();
            this.rbLightStrobeMode = new System.Windows.Forms.RadioButton();
            this.label11 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.rbByPass = new System.Windows.Forms.RadioButton();
            this.rbTesting = new System.Windows.Forms.RadioButton();
            this.rbControlRun = new System.Windows.Forms.RadioButton();
            this.label3 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.rbDisableScan = new System.Windows.Forms.RadioButton();
            this.rbEnableScan = new System.Windows.Forms.RadioButton();
            this.label4 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.nFOVW)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nFOVH)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nSaveDays)).BeginInit();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 152);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Light Port:";
            // 
            // cbLightPort
            // 
            this.cbLightPort.FormattingEnabled = true;
            this.cbLightPort.Location = new System.Drawing.Point(146, 149);
            this.cbLightPort.Name = "cbLightPort";
            this.cbLightPort.Size = new System.Drawing.Size(79, 21);
            this.cbLightPort.TabIndex = 1;
            this.cbLightPort.SelectedIndexChanged += new System.EventHandler(this.cbLightPort_SelectedIndexChanged);
            // 
            // cbScanner
            // 
            this.cbScanner.FormattingEnabled = true;
            this.cbScanner.Location = new System.Drawing.Point(146, 176);
            this.cbScanner.Name = "cbScanner";
            this.cbScanner.Size = new System.Drawing.Size(79, 21);
            this.cbScanner.TabIndex = 3;
            this.cbScanner.SelectedIndexChanged += new System.EventHandler(this.cbScanner_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(21, 179);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Scanner Port:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(21, 217);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(62, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "Image Size:";
            // 
            // nFOVW
            // 
            this.nFOVW.Location = new System.Drawing.Point(99, 215);
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
            this.nFOVW.Size = new System.Drawing.Size(60, 20);
            this.nFOVW.TabIndex = 11;
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
            this.nFOVH.Location = new System.Drawing.Point(165, 215);
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
            this.nFOVH.Size = new System.Drawing.Size(60, 20);
            this.nFOVH.TabIndex = 12;
            this.nFOVH.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nFOVH.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nFOVH.ValueChanged += new System.EventHandler(this.nFOVH_ValueChanged);
            // 
            // btSelectSavePath
            // 
            this.btSelectSavePath.Location = new System.Drawing.Point(451, 163);
            this.btSelectSavePath.Name = "btSelectSavePath";
            this.btSelectSavePath.Size = new System.Drawing.Size(31, 23);
            this.btSelectSavePath.TabIndex = 15;
            this.btSelectSavePath.Text = "...";
            this.btSelectSavePath.UseVisualStyleBackColor = true;
            this.btSelectSavePath.Click += new System.EventHandler(this.button1_Click);
            // 
            // txtSavePath
            // 
            this.txtSavePath.Location = new System.Drawing.Point(281, 165);
            this.txtSavePath.Name = "txtSavePath";
            this.txtSavePath.Size = new System.Drawing.Size(164, 20);
            this.txtSavePath.TabIndex = 14;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(278, 149);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(92, 13);
            this.label6.TabIndex = 13;
            this.label6.Text = "Save Image Path:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(278, 204);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(79, 13);
            this.label7.TabIndex = 16;
            this.label7.Text = "Save for hours:";
            // 
            // nSaveDays
            // 
            this.nSaveDays.Location = new System.Drawing.Point(385, 202);
            this.nSaveDays.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.nSaveDays.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nSaveDays.Name = "nSaveDays";
            this.nSaveDays.Size = new System.Drawing.Size(60, 20);
            this.nSaveDays.TabIndex = 17;
            this.nSaveDays.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nSaveDays.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nSaveDays.ValueChanged += new System.EventHandler(this.nSaveDays_ValueChanged);
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.rbLightConstantMode);
            this.panel2.Controls.Add(this.rbLightStrobeMode);
            this.panel2.Controls.Add(this.label11);
            this.panel2.Location = new System.Drawing.Point(165, 12);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(114, 111);
            this.panel2.TabIndex = 19;
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
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.rbByPass);
            this.panel1.Controls.Add(this.rbTesting);
            this.panel1.Controls.Add(this.rbControlRun);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Location = new System.Drawing.Point(24, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(114, 111);
            this.panel1.TabIndex = 18;
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
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(80, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Running Mode:";
            // 
            // panel3
            // 
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Controls.Add(this.rbDisableScan);
            this.panel3.Controls.Add(this.rbEnableScan);
            this.panel3.Controls.Add(this.label4);
            this.panel3.Location = new System.Drawing.Point(306, 12);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(114, 111);
            this.panel3.TabIndex = 20;
            // 
            // rbDisableScan
            // 
            this.rbDisableScan.AutoSize = true;
            this.rbDisableScan.Location = new System.Drawing.Point(13, 56);
            this.rbDisableScan.Name = "rbDisableScan";
            this.rbDisableScan.Size = new System.Drawing.Size(60, 17);
            this.rbDisableScan.TabIndex = 3;
            this.rbDisableScan.Text = "Disable";
            this.rbDisableScan.UseVisualStyleBackColor = true;
            this.rbDisableScan.CheckedChanged += new System.EventHandler(this.rbDisableScan_CheckedChanged);
            // 
            // rbEnableScan
            // 
            this.rbEnableScan.AutoSize = true;
            this.rbEnableScan.Checked = true;
            this.rbEnableScan.Location = new System.Drawing.Point(13, 33);
            this.rbEnableScan.Name = "rbEnableScan";
            this.rbEnableScan.Size = new System.Drawing.Size(58, 17);
            this.rbEnableScan.TabIndex = 2;
            this.rbEnableScan.TabStop = true;
            this.rbEnableScan.Text = "Enable";
            this.rbEnableScan.UseVisualStyleBackColor = true;
            this.rbEnableScan.CheckedChanged += new System.EventHandler(this.rbEnableScan_CheckedChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(14, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "Scan Mode:";
            // 
            // IOConfigForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(512, 258);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.nSaveDays);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.btSelectSavePath);
            this.Controls.Add(this.txtSavePath);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.nFOVH);
            this.Controls.Add(this.nFOVW);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.cbScanner);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cbLightPort);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "IOConfigForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "IO Config Form";
            this.Load += new System.EventHandler(this.IOConfigForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.nFOVW)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nFOVH)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nSaveDays)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbLightPort;
        private System.Windows.Forms.ComboBox cbScanner;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown nFOVW;
        private System.Windows.Forms.NumericUpDown nFOVH;
        private System.Windows.Forms.Button btSelectSavePath;
        private System.Windows.Forms.TextBox txtSavePath;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown nSaveDays;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.RadioButton rbLightConstantMode;
        private System.Windows.Forms.RadioButton rbLightStrobeMode;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton rbByPass;
        private System.Windows.Forms.RadioButton rbTesting;
        private System.Windows.Forms.RadioButton rbControlRun;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.RadioButton rbDisableScan;
        private System.Windows.Forms.RadioButton rbEnableScan;
        private System.Windows.Forms.Label label4;
    }
}