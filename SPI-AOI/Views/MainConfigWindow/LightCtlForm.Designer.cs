namespace SPI_AOI.Views.MainConfigWindow
{
    partial class LightCtlForm
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btClose = new System.Windows.Forms.Button();
            this.btOpen = new System.Windows.Forms.Button();
            this.cbCom = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.grCH4 = new System.Windows.Forms.GroupBox();
            this.nIntensityCH4 = new System.Windows.Forms.NumericUpDown();
            this.btOffCH4 = new System.Windows.Forms.Button();
            this.btOnCH4 = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.grCH3 = new System.Windows.Forms.GroupBox();
            this.nIntensityCH3 = new System.Windows.Forms.NumericUpDown();
            this.btOffCH3 = new System.Windows.Forms.Button();
            this.btOnCH3 = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.grCH2 = new System.Windows.Forms.GroupBox();
            this.nIntensityCH2 = new System.Windows.Forms.NumericUpDown();
            this.btOffCH2 = new System.Windows.Forms.Button();
            this.btOnCH2 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.grCH1 = new System.Windows.Forms.GroupBox();
            this.nIntensityCH1 = new System.Windows.Forms.NumericUpDown();
            this.btOffCH1 = new System.Windows.Forms.Button();
            this.btOnCH1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.grCH4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nIntensityCH4)).BeginInit();
            this.grCH3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nIntensityCH3)).BeginInit();
            this.grCH2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nIntensityCH2)).BeginInit();
            this.grCH1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nIntensityCH1)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btClose);
            this.groupBox1.Controls.Add(this.btOpen);
            this.groupBox1.Controls.Add(this.cbCom);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.grCH4);
            this.groupBox1.Controls.Add(this.grCH3);
            this.groupBox1.Controls.Add(this.grCH2);
            this.groupBox1.Controls.Add(this.grCH1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(290, 271);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // btClose
            // 
            this.btClose.Location = new System.Drawing.Point(225, 19);
            this.btClose.Name = "btClose";
            this.btClose.Size = new System.Drawing.Size(54, 23);
            this.btClose.TabIndex = 9;
            this.btClose.Text = "Close";
            this.btClose.UseVisualStyleBackColor = true;
            this.btClose.Click += new System.EventHandler(this.btClose_Click);
            // 
            // btOpen
            // 
            this.btOpen.Location = new System.Drawing.Point(161, 19);
            this.btOpen.Name = "btOpen";
            this.btOpen.Size = new System.Drawing.Size(54, 23);
            this.btOpen.TabIndex = 8;
            this.btOpen.Text = "Open";
            this.btOpen.UseVisualStyleBackColor = true;
            this.btOpen.Click += new System.EventHandler(this.btOpen_Click);
            // 
            // cbCom
            // 
            this.cbCom.FormattingEnabled = true;
            this.cbCom.Location = new System.Drawing.Point(50, 19);
            this.cbCom.Name = "cbCom";
            this.cbCom.Size = new System.Drawing.Size(88, 21);
            this.cbCom.TabIndex = 7;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(13, 22);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(31, 13);
            this.label5.TabIndex = 6;
            this.label5.Text = "Com:";
            // 
            // grCH4
            // 
            this.grCH4.Controls.Add(this.nIntensityCH4);
            this.grCH4.Controls.Add(this.btOffCH4);
            this.grCH4.Controls.Add(this.btOnCH4);
            this.grCH4.Controls.Add(this.label4);
            this.grCH4.Location = new System.Drawing.Point(147, 164);
            this.grCH4.Name = "grCH4";
            this.grCH4.Size = new System.Drawing.Size(135, 100);
            this.grCH4.TabIndex = 5;
            this.grCH4.TabStop = false;
            this.grCH4.Text = "CH4";
            // 
            // nIntensityCH4
            // 
            this.nIntensityCH4.Location = new System.Drawing.Point(72, 66);
            this.nIntensityCH4.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nIntensityCH4.Name = "nIntensityCH4";
            this.nIntensityCH4.Size = new System.Drawing.Size(49, 20);
            this.nIntensityCH4.TabIndex = 1;
            this.nIntensityCH4.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nIntensityCH4.ValueChanged += new System.EventHandler(this.nIntensityCH4_ValueChanged);
            // 
            // btOffCH4
            // 
            this.btOffCH4.Location = new System.Drawing.Point(71, 31);
            this.btOffCH4.Name = "btOffCH4";
            this.btOffCH4.Size = new System.Drawing.Size(50, 23);
            this.btOffCH4.TabIndex = 1;
            this.btOffCH4.Text = "Off";
            this.btOffCH4.UseVisualStyleBackColor = true;
            this.btOffCH4.Click += new System.EventHandler(this.btOffCH4_Click);
            // 
            // btOnCH4
            // 
            this.btOnCH4.Location = new System.Drawing.Point(15, 31);
            this.btOnCH4.Name = "btOnCH4";
            this.btOnCH4.Size = new System.Drawing.Size(50, 23);
            this.btOnCH4.TabIndex = 2;
            this.btOnCH4.Text = "On";
            this.btOnCH4.UseVisualStyleBackColor = true;
            this.btOnCH4.Click += new System.EventHandler(this.btOnCH4_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 68);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(49, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Intensity:";
            // 
            // grCH3
            // 
            this.grCH3.Controls.Add(this.nIntensityCH3);
            this.grCH3.Controls.Add(this.btOffCH3);
            this.grCH3.Controls.Add(this.btOnCH3);
            this.grCH3.Controls.Add(this.label3);
            this.grCH3.Location = new System.Drawing.Point(6, 164);
            this.grCH3.Name = "grCH3";
            this.grCH3.Size = new System.Drawing.Size(135, 100);
            this.grCH3.TabIndex = 4;
            this.grCH3.TabStop = false;
            this.grCH3.Text = "CH3";
            // 
            // nIntensityCH3
            // 
            this.nIntensityCH3.Location = new System.Drawing.Point(72, 66);
            this.nIntensityCH3.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nIntensityCH3.Name = "nIntensityCH3";
            this.nIntensityCH3.Size = new System.Drawing.Size(49, 20);
            this.nIntensityCH3.TabIndex = 1;
            this.nIntensityCH3.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nIntensityCH3.ValueChanged += new System.EventHandler(this.nIntensityCH3_ValueChanged);
            // 
            // btOffCH3
            // 
            this.btOffCH3.Location = new System.Drawing.Point(71, 31);
            this.btOffCH3.Name = "btOffCH3";
            this.btOffCH3.Size = new System.Drawing.Size(50, 23);
            this.btOffCH3.TabIndex = 1;
            this.btOffCH3.Text = "Off";
            this.btOffCH3.UseVisualStyleBackColor = true;
            this.btOffCH3.Click += new System.EventHandler(this.btOffCH3_Click);
            // 
            // btOnCH3
            // 
            this.btOnCH3.Location = new System.Drawing.Point(15, 31);
            this.btOnCH3.Name = "btOnCH3";
            this.btOnCH3.Size = new System.Drawing.Size(50, 23);
            this.btOnCH3.TabIndex = 2;
            this.btOnCH3.Text = "On";
            this.btOnCH3.UseVisualStyleBackColor = true;
            this.btOnCH3.Click += new System.EventHandler(this.btOnCH3_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 68);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(49, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Intensity:";
            // 
            // grCH2
            // 
            this.grCH2.Controls.Add(this.nIntensityCH2);
            this.grCH2.Controls.Add(this.btOffCH2);
            this.grCH2.Controls.Add(this.btOnCH2);
            this.grCH2.Controls.Add(this.label2);
            this.grCH2.Location = new System.Drawing.Point(147, 58);
            this.grCH2.Name = "grCH2";
            this.grCH2.Size = new System.Drawing.Size(135, 100);
            this.grCH2.TabIndex = 3;
            this.grCH2.TabStop = false;
            this.grCH2.Text = "CH2";
            // 
            // nIntensityCH2
            // 
            this.nIntensityCH2.Location = new System.Drawing.Point(72, 66);
            this.nIntensityCH2.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nIntensityCH2.Name = "nIntensityCH2";
            this.nIntensityCH2.Size = new System.Drawing.Size(49, 20);
            this.nIntensityCH2.TabIndex = 1;
            this.nIntensityCH2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nIntensityCH2.ValueChanged += new System.EventHandler(this.nIntensityCH2_ValueChanged);
            // 
            // btOffCH2
            // 
            this.btOffCH2.Location = new System.Drawing.Point(71, 31);
            this.btOffCH2.Name = "btOffCH2";
            this.btOffCH2.Size = new System.Drawing.Size(50, 23);
            this.btOffCH2.TabIndex = 1;
            this.btOffCH2.Text = "Off";
            this.btOffCH2.UseVisualStyleBackColor = true;
            this.btOffCH2.Click += new System.EventHandler(this.btOffCH2_Click);
            // 
            // btOnCH2
            // 
            this.btOnCH2.Location = new System.Drawing.Point(15, 31);
            this.btOnCH2.Name = "btOnCH2";
            this.btOnCH2.Size = new System.Drawing.Size(50, 23);
            this.btOnCH2.TabIndex = 2;
            this.btOnCH2.Text = "On";
            this.btOnCH2.UseVisualStyleBackColor = true;
            this.btOnCH2.Click += new System.EventHandler(this.btOnCH2_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 68);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(49, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Intensity:";
            // 
            // grCH1
            // 
            this.grCH1.Controls.Add(this.nIntensityCH1);
            this.grCH1.Controls.Add(this.btOffCH1);
            this.grCH1.Controls.Add(this.btOnCH1);
            this.grCH1.Controls.Add(this.label1);
            this.grCH1.Location = new System.Drawing.Point(6, 58);
            this.grCH1.Name = "grCH1";
            this.grCH1.Size = new System.Drawing.Size(135, 100);
            this.grCH1.TabIndex = 0;
            this.grCH1.TabStop = false;
            this.grCH1.Text = "CH1";
            // 
            // nIntensityCH1
            // 
            this.nIntensityCH1.Location = new System.Drawing.Point(72, 66);
            this.nIntensityCH1.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nIntensityCH1.Name = "nIntensityCH1";
            this.nIntensityCH1.Size = new System.Drawing.Size(49, 20);
            this.nIntensityCH1.TabIndex = 1;
            this.nIntensityCH1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nIntensityCH1.ValueChanged += new System.EventHandler(this.nIntensityCH1_ValueChanged);
            // 
            // btOffCH1
            // 
            this.btOffCH1.Location = new System.Drawing.Point(71, 31);
            this.btOffCH1.Name = "btOffCH1";
            this.btOffCH1.Size = new System.Drawing.Size(50, 23);
            this.btOffCH1.TabIndex = 1;
            this.btOffCH1.Text = "Off";
            this.btOffCH1.UseVisualStyleBackColor = true;
            this.btOffCH1.Click += new System.EventHandler(this.btOffCH1_Click);
            // 
            // btOnCH1
            // 
            this.btOnCH1.Location = new System.Drawing.Point(15, 31);
            this.btOnCH1.Name = "btOnCH1";
            this.btOnCH1.Size = new System.Drawing.Size(50, 23);
            this.btOnCH1.TabIndex = 2;
            this.btOnCH1.Text = "On";
            this.btOnCH1.UseVisualStyleBackColor = true;
            this.btOnCH1.Click += new System.EventHandler(this.btOnCH1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 68);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Intensity:";
            // 
            // LightCtlForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(314, 294);
            this.Controls.Add(this.groupBox1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LightCtlForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "LightCtlForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LightCtlForm_FormClosing);
            this.Load += new System.EventHandler(this.LightCtlForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.grCH4.ResumeLayout(false);
            this.grCH4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nIntensityCH4)).EndInit();
            this.grCH3.ResumeLayout(false);
            this.grCH3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nIntensityCH3)).EndInit();
            this.grCH2.ResumeLayout(false);
            this.grCH2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nIntensityCH2)).EndInit();
            this.grCH1.ResumeLayout(false);
            this.grCH1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nIntensityCH1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox grCH1;
        private System.Windows.Forms.Button btOnCH1;
        private System.Windows.Forms.Button btOffCH1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown nIntensityCH1;
        private System.Windows.Forms.GroupBox grCH4;
        private System.Windows.Forms.NumericUpDown nIntensityCH4;
        private System.Windows.Forms.Button btOffCH4;
        private System.Windows.Forms.Button btOnCH4;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox grCH3;
        private System.Windows.Forms.NumericUpDown nIntensityCH3;
        private System.Windows.Forms.Button btOffCH3;
        private System.Windows.Forms.Button btOnCH3;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox grCH2;
        private System.Windows.Forms.NumericUpDown nIntensityCH2;
        private System.Windows.Forms.Button btOffCH2;
        private System.Windows.Forms.Button btOnCH2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btClose;
        private System.Windows.Forms.Button btOpen;
        private System.Windows.Forms.ComboBox cbCom;
        private System.Windows.Forms.Label label5;
    }
}