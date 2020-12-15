namespace Heal.UI
{
    partial class MachineIssueForm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.grDetails = new System.Windows.Forms.GroupBox();
            this.txtSolution = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtMessage = new System.Windows.Forms.TextBox();
            this.lbMessage = new System.Windows.Forms.Label();
            this.txtBit = new System.Windows.Forms.TextBox();
            this.lbBit = new System.Windows.Forms.Label();
            this.btDelete = new System.Windows.Forms.Button();
            this.btModify = new System.Windows.Forms.Button();
            this.btAdd = new System.Windows.Forms.Button();
            this.grErrorsList = new System.Windows.Forms.GroupBox();
            this.dgvErrors = new System.Windows.Forms.DataGridView();
            this.Bit = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Message = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Solution = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.grDetails.SuspendLayout();
            this.grErrorsList.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvErrors)).BeginInit();
            this.SuspendLayout();
            // 
            // grDetails
            // 
            this.grDetails.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grDetails.Controls.Add(this.txtSolution);
            this.grDetails.Controls.Add(this.label1);
            this.grDetails.Controls.Add(this.txtMessage);
            this.grDetails.Controls.Add(this.lbMessage);
            this.grDetails.Controls.Add(this.txtBit);
            this.grDetails.Controls.Add(this.lbBit);
            this.grDetails.Controls.Add(this.btDelete);
            this.grDetails.Controls.Add(this.btModify);
            this.grDetails.Controls.Add(this.btAdd);
            this.grDetails.Location = new System.Drawing.Point(12, 12);
            this.grDetails.Name = "grDetails";
            this.grDetails.Size = new System.Drawing.Size(685, 188);
            this.grDetails.TabIndex = 0;
            this.grDetails.TabStop = false;
            this.grDetails.Text = "Details";
            // 
            // txtSolution
            // 
            this.txtSolution.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSolution.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSolution.Location = new System.Drawing.Point(77, 95);
            this.txtSolution.Multiline = true;
            this.txtSolution.Name = "txtSolution";
            this.txtSolution.Size = new System.Drawing.Size(578, 87);
            this.txtSolution.TabIndex = 8;
            this.txtSolution.TabStop = false;
            this.txtSolution.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 98);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Solution:";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // txtMessage
            // 
            this.txtMessage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtMessage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtMessage.Location = new System.Drawing.Point(77, 57);
            this.txtMessage.Multiline = true;
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.Size = new System.Drawing.Size(578, 32);
            this.txtMessage.TabIndex = 6;
            this.txtMessage.TabStop = false;
            // 
            // lbMessage
            // 
            this.lbMessage.AutoSize = true;
            this.lbMessage.Location = new System.Drawing.Point(18, 60);
            this.lbMessage.Name = "lbMessage";
            this.lbMessage.Size = new System.Drawing.Size(53, 13);
            this.lbMessage.TabIndex = 5;
            this.lbMessage.Text = "Message:";
            // 
            // txtBit
            // 
            this.txtBit.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBit.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtBit.Location = new System.Drawing.Point(77, 25);
            this.txtBit.Name = "txtBit";
            this.txtBit.Size = new System.Drawing.Size(236, 20);
            this.txtBit.TabIndex = 4;
            this.txtBit.TabStop = false;
            this.txtBit.TextChanged += new System.EventHandler(this.txtBit_TextChanged);
            // 
            // lbBit
            // 
            this.lbBit.AutoSize = true;
            this.lbBit.Location = new System.Drawing.Point(39, 28);
            this.lbBit.Name = "lbBit";
            this.lbBit.Size = new System.Drawing.Size(22, 13);
            this.lbBit.TabIndex = 3;
            this.lbBit.Text = "Bit:";
            // 
            // btDelete
            // 
            this.btDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btDelete.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btDelete.ForeColor = System.Drawing.Color.Red;
            this.btDelete.Location = new System.Drawing.Point(580, 28);
            this.btDelete.Name = "btDelete";
            this.btDelete.Size = new System.Drawing.Size(75, 23);
            this.btDelete.TabIndex = 2;
            this.btDelete.TabStop = false;
            this.btDelete.Text = "Delete";
            this.btDelete.UseVisualStyleBackColor = true;
            this.btDelete.Click += new System.EventHandler(this.btDelete_Click);
            // 
            // btModify
            // 
            this.btModify.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btModify.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btModify.ForeColor = System.Drawing.Color.Blue;
            this.btModify.Location = new System.Drawing.Point(484, 28);
            this.btModify.Name = "btModify";
            this.btModify.Size = new System.Drawing.Size(75, 23);
            this.btModify.TabIndex = 1;
            this.btModify.TabStop = false;
            this.btModify.Text = "Update";
            this.btModify.UseVisualStyleBackColor = true;
            this.btModify.Click += new System.EventHandler(this.btModify_Click);
            // 
            // btAdd
            // 
            this.btAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btAdd.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btAdd.ForeColor = System.Drawing.Color.Green;
            this.btAdd.Location = new System.Drawing.Point(387, 28);
            this.btAdd.Name = "btAdd";
            this.btAdd.Size = new System.Drawing.Size(75, 23);
            this.btAdd.TabIndex = 0;
            this.btAdd.TabStop = false;
            this.btAdd.Text = "Add";
            this.btAdd.UseVisualStyleBackColor = true;
            this.btAdd.Click += new System.EventHandler(this.btAdd_Click);
            // 
            // grErrorsList
            // 
            this.grErrorsList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grErrorsList.Controls.Add(this.dgvErrors);
            this.grErrorsList.Location = new System.Drawing.Point(12, 206);
            this.grErrorsList.Name = "grErrorsList";
            this.grErrorsList.Size = new System.Drawing.Size(685, 233);
            this.grErrorsList.TabIndex = 1;
            this.grErrorsList.TabStop = false;
            this.grErrorsList.Text = "Errors list";
            // 
            // dgvErrors
            // 
            this.dgvErrors.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvErrors.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvErrors.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Bit,
            this.Message,
            this.Solution});
            this.dgvErrors.Cursor = System.Windows.Forms.Cursors.Hand;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvErrors.DefaultCellStyle = dataGridViewCellStyle1;
            this.dgvErrors.Location = new System.Drawing.Point(6, 19);
            this.dgvErrors.Name = "dgvErrors";
            this.dgvErrors.ReadOnly = true;
            this.dgvErrors.Size = new System.Drawing.Size(673, 208);
            this.dgvErrors.TabIndex = 0;
            this.dgvErrors.TabStop = false;
            this.dgvErrors.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvErrors_CellClick);
            // 
            // Bit
            // 
            this.Bit.FillWeight = 20.30457F;
            this.Bit.HeaderText = "Bit";
            this.Bit.Name = "Bit";
            this.Bit.ReadOnly = true;
            // 
            // Message
            // 
            this.Message.FillWeight = 179.6954F;
            this.Message.HeaderText = "Message";
            this.Message.Name = "Message";
            this.Message.ReadOnly = true;
            // 
            // Solution
            // 
            this.Solution.FillWeight = 150F;
            this.Solution.HeaderText = "Solution";
            this.Solution.Name = "Solution";
            this.Solution.ReadOnly = true;
            // 
            // MachineIssueForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(709, 451);
            this.Controls.Add(this.grErrorsList);
            this.Controls.Add(this.grDetails);
            this.MinimumSize = new System.Drawing.Size(620, 440);
            this.Name = "MachineIssueForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Machine issue form";
            this.Load += new System.EventHandler(this.Errors_Load);
            this.grDetails.ResumeLayout(false);
            this.grDetails.PerformLayout();
            this.grErrorsList.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvErrors)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grDetails;
        private System.Windows.Forms.GroupBox grErrorsList;
        private System.Windows.Forms.DataGridView dgvErrors;
        private System.Windows.Forms.Button btDelete;
        private System.Windows.Forms.Button btModify;
        private System.Windows.Forms.Button btAdd;
        private System.Windows.Forms.TextBox txtMessage;
        private System.Windows.Forms.Label lbMessage;
        private System.Windows.Forms.TextBox txtBit;
        private System.Windows.Forms.Label lbBit;
        private System.Windows.Forms.DataGridViewTextBoxColumn Bit;
        private System.Windows.Forms.DataGridViewTextBoxColumn Message;
        private System.Windows.Forms.DataGridViewTextBoxColumn Solution;
        private System.Windows.Forms.TextBox txtSolution;
        private System.Windows.Forms.Label label1;
    }
}