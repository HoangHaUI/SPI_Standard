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
            this.label3 = new System.Windows.Forms.Label();
            this.txtCameraMatrix = new System.Windows.Forms.TextBox();
            this.btBrowserCameraMatrix = new System.Windows.Forms.Button();
            this.btBrowserCameraDistcoeffs = new System.Windows.Forms.Button();
            this.txtCameraDistcoeffs = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.nFOVW = new System.Windows.Forms.NumericUpDown();
            this.nFOVH = new System.Windows.Forms.NumericUpDown();
            this.btSelectSavePath = new System.Windows.Forms.Button();
            this.txtSavePath = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.nSaveDays = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.nFOVW)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nFOVH)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nSaveDays)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Light Port:";
            // 
            // cbLightPort
            // 
            this.cbLightPort.FormattingEnabled = true;
            this.cbLightPort.Location = new System.Drawing.Point(146, 26);
            this.cbLightPort.Name = "cbLightPort";
            this.cbLightPort.Size = new System.Drawing.Size(79, 21);
            this.cbLightPort.TabIndex = 1;
            this.cbLightPort.SelectedIndexChanged += new System.EventHandler(this.cbLightPort_SelectedIndexChanged);
            // 
            // cbScanner
            // 
            this.cbScanner.FormattingEnabled = true;
            this.cbScanner.Location = new System.Drawing.Point(146, 53);
            this.cbScanner.Name = "cbScanner";
            this.cbScanner.Size = new System.Drawing.Size(79, 21);
            this.cbScanner.TabIndex = 3;
            this.cbScanner.SelectedIndexChanged += new System.EventHandler(this.cbScanner_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(21, 56);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Scanner Port:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(291, 26);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(92, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Camera matrix file:";
            // 
            // txtCameraMatrix
            // 
            this.txtCameraMatrix.Location = new System.Drawing.Point(294, 42);
            this.txtCameraMatrix.Name = "txtCameraMatrix";
            this.txtCameraMatrix.Size = new System.Drawing.Size(164, 20);
            this.txtCameraMatrix.TabIndex = 5;
            // 
            // btBrowserCameraMatrix
            // 
            this.btBrowserCameraMatrix.Location = new System.Drawing.Point(464, 40);
            this.btBrowserCameraMatrix.Name = "btBrowserCameraMatrix";
            this.btBrowserCameraMatrix.Size = new System.Drawing.Size(31, 23);
            this.btBrowserCameraMatrix.TabIndex = 6;
            this.btBrowserCameraMatrix.Text = "...";
            this.btBrowserCameraMatrix.UseVisualStyleBackColor = true;
            this.btBrowserCameraMatrix.Click += new System.EventHandler(this.btBrowserCameraMatrix_Click);
            // 
            // btBrowserCameraDistcoeffs
            // 
            this.btBrowserCameraDistcoeffs.Location = new System.Drawing.Point(464, 89);
            this.btBrowserCameraDistcoeffs.Name = "btBrowserCameraDistcoeffs";
            this.btBrowserCameraDistcoeffs.Size = new System.Drawing.Size(31, 23);
            this.btBrowserCameraDistcoeffs.TabIndex = 9;
            this.btBrowserCameraDistcoeffs.Text = "...";
            this.btBrowserCameraDistcoeffs.UseVisualStyleBackColor = true;
            this.btBrowserCameraDistcoeffs.Click += new System.EventHandler(this.btBrowserCameraDistcoeffs_Click);
            // 
            // txtCameraDistcoeffs
            // 
            this.txtCameraDistcoeffs.Location = new System.Drawing.Point(294, 91);
            this.txtCameraDistcoeffs.Name = "txtCameraDistcoeffs";
            this.txtCameraDistcoeffs.Size = new System.Drawing.Size(164, 20);
            this.txtCameraDistcoeffs.TabIndex = 8;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(291, 75);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(110, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Camera distcoeffs file:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(21, 94);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(62, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "Image Size:";
            // 
            // nFOVW
            // 
            this.nFOVW.Location = new System.Drawing.Point(99, 92);
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
            this.nFOVH.Location = new System.Drawing.Point(165, 92);
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
            this.btSelectSavePath.Location = new System.Drawing.Point(194, 148);
            this.btSelectSavePath.Name = "btSelectSavePath";
            this.btSelectSavePath.Size = new System.Drawing.Size(31, 23);
            this.btSelectSavePath.TabIndex = 15;
            this.btSelectSavePath.Text = "...";
            this.btSelectSavePath.UseVisualStyleBackColor = true;
            this.btSelectSavePath.Click += new System.EventHandler(this.button1_Click);
            // 
            // txtSavePath
            // 
            this.txtSavePath.Location = new System.Drawing.Point(24, 150);
            this.txtSavePath.Name = "txtSavePath";
            this.txtSavePath.Size = new System.Drawing.Size(164, 20);
            this.txtSavePath.TabIndex = 14;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(21, 134);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(92, 13);
            this.label6.TabIndex = 13;
            this.label6.Text = "Save Image Path:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(291, 153);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(79, 13);
            this.label7.TabIndex = 16;
            this.label7.Text = "Save for hours:";
            // 
            // nSaveDays
            // 
            this.nSaveDays.Location = new System.Drawing.Point(398, 151);
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
            // IOConfigForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(526, 197);
            this.Controls.Add(this.nSaveDays);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.btSelectSavePath);
            this.Controls.Add(this.txtSavePath);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.nFOVH);
            this.Controls.Add(this.nFOVW);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.btBrowserCameraDistcoeffs);
            this.Controls.Add(this.txtCameraDistcoeffs);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btBrowserCameraMatrix);
            this.Controls.Add(this.txtCameraMatrix);
            this.Controls.Add(this.label3);
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
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbLightPort;
        private System.Windows.Forms.ComboBox cbScanner;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtCameraMatrix;
        private System.Windows.Forms.Button btBrowserCameraMatrix;
        private System.Windows.Forms.Button btBrowserCameraDistcoeffs;
        private System.Windows.Forms.TextBox txtCameraDistcoeffs;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown nFOVW;
        private System.Windows.Forms.NumericUpDown nFOVH;
        private System.Windows.Forms.Button btSelectSavePath;
        private System.Windows.Forms.TextBox txtSavePath;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown nSaveDays;
    }
}