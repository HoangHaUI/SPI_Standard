namespace SPI_AOI.Views.MainConfigWindow
{
    partial class PLCMonitor
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PLCMonitor));
            this.btPing = new System.Windows.Forms.Button();
            this.pnMain = new System.Windows.Forms.Panel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btHomeAll = new System.Windows.Forms.Button();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.btSetSpeedConveyor = new System.Windows.Forms.Button();
            this.txtSpeedConveyor = new System.Windows.Forms.TextBox();
            this.btHomeConveyor = new System.Windows.Forms.Button();
            this.label11 = new System.Windows.Forms.Label();
            this.txtY_Conveyor = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.btGoConveyor = new System.Windows.Forms.Button();
            this.btLoadPanel = new System.Windows.Forms.Button();
            this.btGoBot_ConveyorAxis = new System.Windows.Forms.Button();
            this.btUnloadPanel = new System.Windows.Forms.Button();
            this.btGoTop_ConveyorAxis = new System.Windows.Forms.Button();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.btSetSpeedBot = new System.Windows.Forms.Button();
            this.txtSpeedBot = new System.Windows.Forms.TextBox();
            this.btHomeScan = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.txtY_Bot = new System.Windows.Forms.TextBox();
            this.txtX_Bot = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.btGoBot = new System.Windows.Forms.Button();
            this.btGoRight_BotAxis = new System.Windows.Forms.Button();
            this.btGoBot_BotAxis = new System.Windows.Forms.Button();
            this.btGoLeft_BotAxis = new System.Windows.Forms.Button();
            this.btGoTop_BotAxis = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.btSetSpeedTop = new System.Windows.Forms.Button();
            this.txtSpeedTop = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btHomeTop = new System.Windows.Forms.Button();
            this.txtY_Top = new System.Windows.Forms.TextBox();
            this.txtX_Top = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.btGoTop = new System.Windows.Forms.Button();
            this.btGoRight_TopAxis = new System.Windows.Forms.Button();
            this.btGoBot_TopAxis = new System.Windows.Forms.Button();
            this.btGoLeft_TopAxis = new System.Windows.Forms.Button();
            this.btGoTop_TopAxis = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btSet = new System.Windows.Forms.Button();
            this.txtValueSet = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtDeviceSet = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.btGet = new System.Windows.Forms.Button();
            this.txtValueGet = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtDeviceGet = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.pnMain.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // btPing
            // 
            this.btPing.Location = new System.Drawing.Point(333, 19);
            this.btPing.Name = "btPing";
            this.btPing.Size = new System.Drawing.Size(58, 59);
            this.btPing.TabIndex = 4;
            this.btPing.Text = "Ping";
            this.btPing.UseVisualStyleBackColor = true;
            this.btPing.Click += new System.EventHandler(this.btPing_Click);
            // 
            // pnMain
            // 
            this.pnMain.Controls.Add(this.groupBox2);
            this.pnMain.Controls.Add(this.groupBox6);
            this.pnMain.Controls.Add(this.groupBox5);
            this.pnMain.Controls.Add(this.groupBox4);
            this.pnMain.Controls.Add(this.groupBox3);
            this.pnMain.Location = new System.Drawing.Point(9, 12);
            this.pnMain.Name = "pnMain";
            this.pnMain.Size = new System.Drawing.Size(555, 369);
            this.pnMain.TabIndex = 6;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btHomeAll);
            this.groupBox2.Location = new System.Drawing.Point(3, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(133, 93);
            this.groupBox2.TabIndex = 12;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Home All";
            // 
            // btHomeAll
            // 
            this.btHomeAll.BackColor = System.Drawing.Color.White;
            this.btHomeAll.Image = ((System.Drawing.Image)(resources.GetObject("btHomeAll.Image")));
            this.btHomeAll.Location = new System.Drawing.Point(13, 23);
            this.btHomeAll.Name = "btHomeAll";
            this.btHomeAll.Size = new System.Drawing.Size(106, 60);
            this.btHomeAll.TabIndex = 10;
            this.toolTip.SetToolTip(this.btHomeAll, "Home ALL");
            this.btHomeAll.UseVisualStyleBackColor = false;
            this.btHomeAll.Click += new System.EventHandler(this.btHomeAll_Click);
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.btSetSpeedConveyor);
            this.groupBox6.Controls.Add(this.txtSpeedConveyor);
            this.groupBox6.Controls.Add(this.btHomeConveyor);
            this.groupBox6.Controls.Add(this.label11);
            this.groupBox6.Controls.Add(this.txtY_Conveyor);
            this.groupBox6.Controls.Add(this.label12);
            this.groupBox6.Controls.Add(this.btGoConveyor);
            this.groupBox6.Controls.Add(this.btLoadPanel);
            this.groupBox6.Controls.Add(this.btGoBot_ConveyorAxis);
            this.groupBox6.Controls.Add(this.btUnloadPanel);
            this.groupBox6.Controls.Add(this.btGoTop_ConveyorAxis);
            this.groupBox6.Location = new System.Drawing.Point(373, 102);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(179, 264);
            this.groupBox6.TabIndex = 11;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Conveyor Moves";
            // 
            // btSetSpeedConveyor
            // 
            this.btSetSpeedConveyor.Location = new System.Drawing.Point(125, 226);
            this.btSetSpeedConveyor.Name = "btSetSpeedConveyor";
            this.btSetSpeedConveyor.Size = new System.Drawing.Size(35, 23);
            this.btSetSpeedConveyor.TabIndex = 18;
            this.btSetSpeedConveyor.Text = "Set";
            this.btSetSpeedConveyor.UseVisualStyleBackColor = true;
            this.btSetSpeedConveyor.Click += new System.EventHandler(this.btSetSpeedConveyor_Click);
            // 
            // txtSpeedConveyor
            // 
            this.txtSpeedConveyor.Location = new System.Drawing.Point(13, 228);
            this.txtSpeedConveyor.Name = "txtSpeedConveyor";
            this.txtSpeedConveyor.Size = new System.Drawing.Size(106, 20);
            this.txtSpeedConveyor.TabIndex = 17;
            // 
            // btHomeConveyor
            // 
            this.btHomeConveyor.BackColor = System.Drawing.Color.White;
            this.btHomeConveyor.Image = ((System.Drawing.Image)(resources.GetObject("btHomeConveyor.Image")));
            this.btHomeConveyor.Location = new System.Drawing.Point(139, 22);
            this.btHomeConveyor.Name = "btHomeConveyor";
            this.btHomeConveyor.Size = new System.Drawing.Size(30, 30);
            this.btHomeConveyor.TabIndex = 9;
            this.toolTip.SetToolTip(this.btHomeConveyor, "Go Home");
            this.btHomeConveyor.UseVisualStyleBackColor = false;
            this.btHomeConveyor.Click += new System.EventHandler(this.btHomeConveyor_Click);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(10, 212);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(41, 13);
            this.label11.TabIndex = 16;
            this.label11.Text = "Speed:";
            // 
            // txtY_Conveyor
            // 
            this.txtY_Conveyor.Location = new System.Drawing.Point(33, 151);
            this.txtY_Conveyor.Name = "txtY_Conveyor";
            this.txtY_Conveyor.Size = new System.Drawing.Size(86, 20);
            this.txtY_Conveyor.TabIndex = 7;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(10, 154);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(17, 13);
            this.label12.TabIndex = 5;
            this.label12.Text = "Y:";
            // 
            // btGoConveyor
            // 
            this.btGoConveyor.Location = new System.Drawing.Point(125, 151);
            this.btGoConveyor.Name = "btGoConveyor";
            this.btGoConveyor.Size = new System.Drawing.Size(35, 20);
            this.btGoConveyor.TabIndex = 4;
            this.btGoConveyor.Text = "Go";
            this.btGoConveyor.UseVisualStyleBackColor = true;
            this.btGoConveyor.Click += new System.EventHandler(this.btGoConveyor_Click);
            // 
            // btLoadPanel
            // 
            this.btLoadPanel.BackColor = System.Drawing.Color.Transparent;
            this.btLoadPanel.Location = new System.Drawing.Point(77, 67);
            this.btLoadPanel.Name = "btLoadPanel";
            this.btLoadPanel.Size = new System.Drawing.Size(83, 30);
            this.btLoadPanel.TabIndex = 3;
            this.btLoadPanel.Text = "Load Panel";
            this.toolTip.SetToolTip(this.btLoadPanel, "Load Panel");
            this.btLoadPanel.UseVisualStyleBackColor = false;
            this.btLoadPanel.Click += new System.EventHandler(this.btLoadPanel_Click);
            // 
            // btGoBot_ConveyorAxis
            // 
            this.btGoBot_ConveyorAxis.BackColor = System.Drawing.Color.White;
            this.btGoBot_ConveyorAxis.Image = ((System.Drawing.Image)(resources.GetObject("btGoBot_ConveyorAxis.Image")));
            this.btGoBot_ConveyorAxis.Location = new System.Drawing.Point(27, 101);
            this.btGoBot_ConveyorAxis.Name = "btGoBot_ConveyorAxis";
            this.btGoBot_ConveyorAxis.Size = new System.Drawing.Size(30, 30);
            this.btGoBot_ConveyorAxis.TabIndex = 2;
            this.toolTip.SetToolTip(this.btGoBot_ConveyorAxis, "Down");
            this.btGoBot_ConveyorAxis.UseVisualStyleBackColor = false;
            this.btGoBot_ConveyorAxis.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btGoBot_ConveyorAxis_MouseDown);
            this.btGoBot_ConveyorAxis.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btGoBot_ConveyorAxis_MouseUp);
            // 
            // btUnloadPanel
            // 
            this.btUnloadPanel.BackColor = System.Drawing.Color.Transparent;
            this.btUnloadPanel.Location = new System.Drawing.Point(77, 103);
            this.btUnloadPanel.Name = "btUnloadPanel";
            this.btUnloadPanel.Size = new System.Drawing.Size(83, 30);
            this.btUnloadPanel.TabIndex = 1;
            this.btUnloadPanel.Text = "Unload Panel";
            this.toolTip.SetToolTip(this.btUnloadPanel, "Unload Panel");
            this.btUnloadPanel.UseVisualStyleBackColor = false;
            this.btUnloadPanel.Click += new System.EventHandler(this.btUnloadPanel_Click);
            // 
            // btGoTop_ConveyorAxis
            // 
            this.btGoTop_ConveyorAxis.BackColor = System.Drawing.Color.White;
            this.btGoTop_ConveyorAxis.Image = ((System.Drawing.Image)(resources.GetObject("btGoTop_ConveyorAxis.Image")));
            this.btGoTop_ConveyorAxis.Location = new System.Drawing.Point(27, 34);
            this.btGoTop_ConveyorAxis.Name = "btGoTop_ConveyorAxis";
            this.btGoTop_ConveyorAxis.Size = new System.Drawing.Size(30, 30);
            this.btGoTop_ConveyorAxis.TabIndex = 0;
            this.toolTip.SetToolTip(this.btGoTop_ConveyorAxis, "Up");
            this.btGoTop_ConveyorAxis.UseVisualStyleBackColor = false;
            this.btGoTop_ConveyorAxis.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btGoTop_ConveyorAxis_MouseDown);
            this.btGoTop_ConveyorAxis.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btGoTop_ConveyorAxis_MouseUp);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.btSetSpeedBot);
            this.groupBox5.Controls.Add(this.txtSpeedBot);
            this.groupBox5.Controls.Add(this.btHomeScan);
            this.groupBox5.Controls.Add(this.label2);
            this.groupBox5.Controls.Add(this.txtY_Bot);
            this.groupBox5.Controls.Add(this.txtX_Bot);
            this.groupBox5.Controls.Add(this.label9);
            this.groupBox5.Controls.Add(this.label10);
            this.groupBox5.Controls.Add(this.btGoBot);
            this.groupBox5.Controls.Add(this.btGoRight_BotAxis);
            this.groupBox5.Controls.Add(this.btGoBot_BotAxis);
            this.groupBox5.Controls.Add(this.btGoLeft_BotAxis);
            this.groupBox5.Controls.Add(this.btGoTop_BotAxis);
            this.groupBox5.Location = new System.Drawing.Point(188, 102);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(179, 264);
            this.groupBox5.TabIndex = 10;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Scan - Bot Axis Moves";
            // 
            // btSetSpeedBot
            // 
            this.btSetSpeedBot.Location = new System.Drawing.Point(125, 226);
            this.btSetSpeedBot.Name = "btSetSpeedBot";
            this.btSetSpeedBot.Size = new System.Drawing.Size(35, 23);
            this.btSetSpeedBot.TabIndex = 15;
            this.btSetSpeedBot.Text = "Set";
            this.btSetSpeedBot.UseVisualStyleBackColor = true;
            this.btSetSpeedBot.Click += new System.EventHandler(this.btSetSpeedBot_Click);
            // 
            // txtSpeedBot
            // 
            this.txtSpeedBot.Location = new System.Drawing.Point(13, 228);
            this.txtSpeedBot.Name = "txtSpeedBot";
            this.txtSpeedBot.Size = new System.Drawing.Size(106, 20);
            this.txtSpeedBot.TabIndex = 14;
            // 
            // btHomeScan
            // 
            this.btHomeScan.BackColor = System.Drawing.Color.White;
            this.btHomeScan.Image = ((System.Drawing.Image)(resources.GetObject("btHomeScan.Image")));
            this.btHomeScan.Location = new System.Drawing.Point(139, 22);
            this.btHomeScan.Name = "btHomeScan";
            this.btHomeScan.Size = new System.Drawing.Size(30, 30);
            this.btHomeScan.TabIndex = 9;
            this.toolTip.SetToolTip(this.btHomeScan, "Go Home");
            this.btHomeScan.UseVisualStyleBackColor = false;
            this.btHomeScan.Click += new System.EventHandler(this.btHomeScan_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 212);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 13);
            this.label2.TabIndex = 13;
            this.label2.Text = "Speed:";
            // 
            // txtY_Bot
            // 
            this.txtY_Bot.Location = new System.Drawing.Point(33, 178);
            this.txtY_Bot.Name = "txtY_Bot";
            this.txtY_Bot.Size = new System.Drawing.Size(86, 20);
            this.txtY_Bot.TabIndex = 8;
            // 
            // txtX_Bot
            // 
            this.txtX_Bot.Location = new System.Drawing.Point(33, 151);
            this.txtX_Bot.Name = "txtX_Bot";
            this.txtX_Bot.Size = new System.Drawing.Size(86, 20);
            this.txtX_Bot.TabIndex = 7;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(10, 181);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(17, 13);
            this.label9.TabIndex = 6;
            this.label9.Text = "Y:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(10, 154);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(17, 13);
            this.label10.TabIndex = 5;
            this.label10.Text = "X:";
            // 
            // btGoBot
            // 
            this.btGoBot.Location = new System.Drawing.Point(125, 151);
            this.btGoBot.Name = "btGoBot";
            this.btGoBot.Size = new System.Drawing.Size(35, 47);
            this.btGoBot.TabIndex = 4;
            this.btGoBot.Text = "Go";
            this.btGoBot.UseVisualStyleBackColor = true;
            this.btGoBot.Click += new System.EventHandler(this.btGoBot_Click);
            // 
            // btGoRight_BotAxis
            // 
            this.btGoRight_BotAxis.BackColor = System.Drawing.Color.White;
            this.btGoRight_BotAxis.Image = ((System.Drawing.Image)(resources.GetObject("btGoRight_BotAxis.Image")));
            this.btGoRight_BotAxis.Location = new System.Drawing.Point(103, 67);
            this.btGoRight_BotAxis.Name = "btGoRight_BotAxis";
            this.btGoRight_BotAxis.Size = new System.Drawing.Size(30, 30);
            this.btGoRight_BotAxis.TabIndex = 3;
            this.toolTip.SetToolTip(this.btGoRight_BotAxis, "Right");
            this.btGoRight_BotAxis.UseVisualStyleBackColor = false;
            this.btGoRight_BotAxis.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btGoRight_BotAxis_MouseDown);
            this.btGoRight_BotAxis.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btGoRight_BotAxis_MouseUp);
            // 
            // btGoBot_BotAxis
            // 
            this.btGoBot_BotAxis.BackColor = System.Drawing.Color.White;
            this.btGoBot_BotAxis.Image = ((System.Drawing.Image)(resources.GetObject("btGoBot_BotAxis.Image")));
            this.btGoBot_BotAxis.Location = new System.Drawing.Point(68, 101);
            this.btGoBot_BotAxis.Name = "btGoBot_BotAxis";
            this.btGoBot_BotAxis.Size = new System.Drawing.Size(30, 30);
            this.btGoBot_BotAxis.TabIndex = 2;
            this.toolTip.SetToolTip(this.btGoBot_BotAxis, "Down");
            this.btGoBot_BotAxis.UseVisualStyleBackColor = false;
            this.btGoBot_BotAxis.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btGoBot_BotAxis_MouseDown);
            this.btGoBot_BotAxis.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btGoBot_BotAxis_MouseUp);
            // 
            // btGoLeft_BotAxis
            // 
            this.btGoLeft_BotAxis.BackColor = System.Drawing.Color.White;
            this.btGoLeft_BotAxis.Image = ((System.Drawing.Image)(resources.GetObject("btGoLeft_BotAxis.Image")));
            this.btGoLeft_BotAxis.Location = new System.Drawing.Point(34, 67);
            this.btGoLeft_BotAxis.Name = "btGoLeft_BotAxis";
            this.btGoLeft_BotAxis.Size = new System.Drawing.Size(30, 30);
            this.btGoLeft_BotAxis.TabIndex = 1;
            this.toolTip.SetToolTip(this.btGoLeft_BotAxis, "Left");
            this.btGoLeft_BotAxis.UseVisualStyleBackColor = false;
            this.btGoLeft_BotAxis.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btGoLeft_BotAxis_MouseDown);
            this.btGoLeft_BotAxis.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btGoLeft_BotAxis_MouseUp);
            // 
            // btGoTop_BotAxis
            // 
            this.btGoTop_BotAxis.BackColor = System.Drawing.Color.White;
            this.btGoTop_BotAxis.Image = ((System.Drawing.Image)(resources.GetObject("btGoTop_BotAxis.Image")));
            this.btGoTop_BotAxis.Location = new System.Drawing.Point(68, 34);
            this.btGoTop_BotAxis.Name = "btGoTop_BotAxis";
            this.btGoTop_BotAxis.Size = new System.Drawing.Size(30, 30);
            this.btGoTop_BotAxis.TabIndex = 0;
            this.toolTip.SetToolTip(this.btGoTop_BotAxis, "Up");
            this.btGoTop_BotAxis.UseVisualStyleBackColor = false;
            this.btGoTop_BotAxis.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btGoTop_BotAxis_MouseDown);
            this.btGoTop_BotAxis.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btGoTop_BotAxis_MouseUp);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.btSetSpeedTop);
            this.groupBox4.Controls.Add(this.txtSpeedTop);
            this.groupBox4.Controls.Add(this.label1);
            this.groupBox4.Controls.Add(this.btHomeTop);
            this.groupBox4.Controls.Add(this.txtY_Top);
            this.groupBox4.Controls.Add(this.txtX_Top);
            this.groupBox4.Controls.Add(this.label8);
            this.groupBox4.Controls.Add(this.label7);
            this.groupBox4.Controls.Add(this.btGoTop);
            this.groupBox4.Controls.Add(this.btGoRight_TopAxis);
            this.groupBox4.Controls.Add(this.btGoBot_TopAxis);
            this.groupBox4.Controls.Add(this.btGoLeft_TopAxis);
            this.groupBox4.Controls.Add(this.btGoTop_TopAxis);
            this.groupBox4.Location = new System.Drawing.Point(3, 102);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(179, 264);
            this.groupBox4.TabIndex = 2;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Camera  - Top Axis Moves";
            // 
            // btSetSpeedTop
            // 
            this.btSetSpeedTop.Location = new System.Drawing.Point(125, 226);
            this.btSetSpeedTop.Name = "btSetSpeedTop";
            this.btSetSpeedTop.Size = new System.Drawing.Size(35, 23);
            this.btSetSpeedTop.TabIndex = 12;
            this.btSetSpeedTop.Text = "Set";
            this.btSetSpeedTop.UseVisualStyleBackColor = true;
            this.btSetSpeedTop.Click += new System.EventHandler(this.btSetSpeedTop_Click);
            // 
            // txtSpeedTop
            // 
            this.txtSpeedTop.Location = new System.Drawing.Point(13, 228);
            this.txtSpeedTop.Name = "txtSpeedTop";
            this.txtSpeedTop.Size = new System.Drawing.Size(106, 20);
            this.txtSpeedTop.TabIndex = 11;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 212);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Speed:";
            // 
            // btHomeTop
            // 
            this.btHomeTop.BackColor = System.Drawing.Color.White;
            this.btHomeTop.Image = ((System.Drawing.Image)(resources.GetObject("btHomeTop.Image")));
            this.btHomeTop.Location = new System.Drawing.Point(139, 22);
            this.btHomeTop.Name = "btHomeTop";
            this.btHomeTop.Size = new System.Drawing.Size(30, 30);
            this.btHomeTop.TabIndex = 9;
            this.toolTip.SetToolTip(this.btHomeTop, "Go Home");
            this.btHomeTop.UseVisualStyleBackColor = false;
            this.btHomeTop.Click += new System.EventHandler(this.btHomeTop_Click);
            // 
            // txtY_Top
            // 
            this.txtY_Top.Location = new System.Drawing.Point(33, 178);
            this.txtY_Top.Name = "txtY_Top";
            this.txtY_Top.Size = new System.Drawing.Size(86, 20);
            this.txtY_Top.TabIndex = 8;
            // 
            // txtX_Top
            // 
            this.txtX_Top.Location = new System.Drawing.Point(33, 151);
            this.txtX_Top.Name = "txtX_Top";
            this.txtX_Top.Size = new System.Drawing.Size(86, 20);
            this.txtX_Top.TabIndex = 7;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(10, 181);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(17, 13);
            this.label8.TabIndex = 6;
            this.label8.Text = "Y:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(10, 154);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(17, 13);
            this.label7.TabIndex = 5;
            this.label7.Text = "X:";
            // 
            // btGoTop
            // 
            this.btGoTop.Location = new System.Drawing.Point(125, 151);
            this.btGoTop.Name = "btGoTop";
            this.btGoTop.Size = new System.Drawing.Size(35, 47);
            this.btGoTop.TabIndex = 4;
            this.btGoTop.Text = "Go";
            this.btGoTop.UseVisualStyleBackColor = true;
            this.btGoTop.Click += new System.EventHandler(this.btGoTop_Click);
            // 
            // btGoRight_TopAxis
            // 
            this.btGoRight_TopAxis.BackColor = System.Drawing.Color.White;
            this.btGoRight_TopAxis.Image = ((System.Drawing.Image)(resources.GetObject("btGoRight_TopAxis.Image")));
            this.btGoRight_TopAxis.Location = new System.Drawing.Point(103, 67);
            this.btGoRight_TopAxis.Name = "btGoRight_TopAxis";
            this.btGoRight_TopAxis.Size = new System.Drawing.Size(30, 30);
            this.btGoRight_TopAxis.TabIndex = 3;
            this.toolTip.SetToolTip(this.btGoRight_TopAxis, "Right");
            this.btGoRight_TopAxis.UseVisualStyleBackColor = false;
            this.btGoRight_TopAxis.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btGoRight_TopAxis_MouseDown);
            this.btGoRight_TopAxis.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btGoRight_TopAxis_MouseUp);
            // 
            // btGoBot_TopAxis
            // 
            this.btGoBot_TopAxis.BackColor = System.Drawing.Color.White;
            this.btGoBot_TopAxis.Image = ((System.Drawing.Image)(resources.GetObject("btGoBot_TopAxis.Image")));
            this.btGoBot_TopAxis.Location = new System.Drawing.Point(68, 101);
            this.btGoBot_TopAxis.Name = "btGoBot_TopAxis";
            this.btGoBot_TopAxis.Size = new System.Drawing.Size(30, 30);
            this.btGoBot_TopAxis.TabIndex = 2;
            this.toolTip.SetToolTip(this.btGoBot_TopAxis, "Down");
            this.btGoBot_TopAxis.UseVisualStyleBackColor = false;
            this.btGoBot_TopAxis.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btGoBot_TopAxis_MouseDown);
            this.btGoBot_TopAxis.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btGoBot_TopAxis_MouseUp);
            // 
            // btGoLeft_TopAxis
            // 
            this.btGoLeft_TopAxis.BackColor = System.Drawing.Color.White;
            this.btGoLeft_TopAxis.Image = ((System.Drawing.Image)(resources.GetObject("btGoLeft_TopAxis.Image")));
            this.btGoLeft_TopAxis.Location = new System.Drawing.Point(34, 67);
            this.btGoLeft_TopAxis.Name = "btGoLeft_TopAxis";
            this.btGoLeft_TopAxis.Size = new System.Drawing.Size(30, 30);
            this.btGoLeft_TopAxis.TabIndex = 1;
            this.toolTip.SetToolTip(this.btGoLeft_TopAxis, "Left");
            this.btGoLeft_TopAxis.UseVisualStyleBackColor = false;
            this.btGoLeft_TopAxis.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btGoLeft_TopAxis_MouseDown);
            this.btGoLeft_TopAxis.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btGoLeft_TopAxis_MouseUp);
            // 
            // btGoTop_TopAxis
            // 
            this.btGoTop_TopAxis.BackColor = System.Drawing.Color.White;
            this.btGoTop_TopAxis.Image = ((System.Drawing.Image)(resources.GetObject("btGoTop_TopAxis.Image")));
            this.btGoTop_TopAxis.Location = new System.Drawing.Point(68, 34);
            this.btGoTop_TopAxis.Name = "btGoTop_TopAxis";
            this.btGoTop_TopAxis.Size = new System.Drawing.Size(30, 30);
            this.btGoTop_TopAxis.TabIndex = 0;
            this.toolTip.SetToolTip(this.btGoTop_TopAxis, "Up");
            this.btGoTop_TopAxis.UseVisualStyleBackColor = false;
            this.btGoTop_TopAxis.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btGoTop_TopAxis_MouseDown);
            this.btGoTop_TopAxis.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btGoTop_TopAxis_MouseUp);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.btSet);
            this.groupBox3.Controls.Add(this.btPing);
            this.groupBox3.Controls.Add(this.txtValueSet);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.txtDeviceSet);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.btGet);
            this.groupBox3.Controls.Add(this.txtValueGet);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.txtDeviceGet);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Location = new System.Drawing.Point(142, 3);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(410, 93);
            this.groupBox3.TabIndex = 1;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Monitor";
            // 
            // btSet
            // 
            this.btSet.Location = new System.Drawing.Point(270, 55);
            this.btSet.Name = "btSet";
            this.btSet.Size = new System.Drawing.Size(34, 23);
            this.btSet.TabIndex = 9;
            this.btSet.Text = "Set";
            this.btSet.UseVisualStyleBackColor = true;
            this.btSet.Click += new System.EventHandler(this.btSet_Click);
            // 
            // txtValueSet
            // 
            this.txtValueSet.Location = new System.Drawing.Point(190, 58);
            this.txtValueSet.Name = "txtValueSet";
            this.txtValueSet.Size = new System.Drawing.Size(67, 20);
            this.txtValueSet.TabIndex = 8;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(147, 61);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(37, 13);
            this.label5.TabIndex = 7;
            this.label5.Text = "Value:";
            // 
            // txtDeviceSet
            // 
            this.txtDeviceSet.Location = new System.Drawing.Point(66, 58);
            this.txtDeviceSet.Name = "txtDeviceSet";
            this.txtDeviceSet.Size = new System.Drawing.Size(67, 20);
            this.txtDeviceSet.TabIndex = 6;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(16, 61);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(44, 13);
            this.label6.TabIndex = 5;
            this.label6.Text = "Device:";
            // 
            // btGet
            // 
            this.btGet.Location = new System.Drawing.Point(139, 19);
            this.btGet.Name = "btGet";
            this.btGet.Size = new System.Drawing.Size(34, 23);
            this.btGet.TabIndex = 4;
            this.btGet.Text = "Get";
            this.btGet.UseVisualStyleBackColor = true;
            this.btGet.Click += new System.EventHandler(this.btGet_Click);
            // 
            // txtValueGet
            // 
            this.txtValueGet.Location = new System.Drawing.Point(237, 20);
            this.txtValueGet.Name = "txtValueGet";
            this.txtValueGet.Size = new System.Drawing.Size(67, 20);
            this.txtValueGet.TabIndex = 3;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(187, 23);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(37, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "Value:";
            // 
            // txtDeviceGet
            // 
            this.txtDeviceGet.Location = new System.Drawing.Point(66, 20);
            this.txtDeviceGet.Name = "txtDeviceGet";
            this.txtDeviceGet.Size = new System.Drawing.Size(67, 20);
            this.txtDeviceGet.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 23);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(44, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Device:";
            // 
            // PLCMonitor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(573, 391);
            this.Controls.Add(this.pnMain);
            this.MaximizeBox = false;
            this.Name = "PLCMonitor";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "PLC Monitor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PLCMonitor_FormClosing);
            this.Load += new System.EventHandler(this.PLCMonitor_Load);
            this.pnMain.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btPing;
        private System.Windows.Forms.Panel pnMain;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btSet;
        private System.Windows.Forms.TextBox txtValueSet;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtDeviceSet;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btGet;
        private System.Windows.Forms.TextBox txtValueGet;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtDeviceGet;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button btGoRight_TopAxis;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Button btGoBot_TopAxis;
        private System.Windows.Forms.Button btGoLeft_TopAxis;
        private System.Windows.Forms.Button btGoTop_TopAxis;
        private System.Windows.Forms.TextBox txtY_Top;
        private System.Windows.Forms.TextBox txtX_Top;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btGoTop;
        private System.Windows.Forms.Button btHomeTop;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Button btHomeScan;
        private System.Windows.Forms.TextBox txtY_Bot;
        private System.Windows.Forms.TextBox txtX_Bot;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button btGoBot;
        private System.Windows.Forms.Button btGoRight_BotAxis;
        private System.Windows.Forms.Button btGoBot_BotAxis;
        private System.Windows.Forms.Button btGoLeft_BotAxis;
        private System.Windows.Forms.Button btGoTop_BotAxis;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.Button btHomeConveyor;
        private System.Windows.Forms.TextBox txtY_Conveyor;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Button btGoConveyor;
        private System.Windows.Forms.Button btGoBot_ConveyorAxis;
        private System.Windows.Forms.Button btUnloadPanel;
        private System.Windows.Forms.Button btGoTop_ConveyorAxis;
        private System.Windows.Forms.Button btLoadPanel;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btHomeAll;
        private System.Windows.Forms.Button btSetSpeedConveyor;
        private System.Windows.Forms.TextBox txtSpeedConveyor;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Button btSetSpeedBot;
        private System.Windows.Forms.TextBox txtSpeedBot;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btSetSpeedTop;
        private System.Windows.Forms.TextBox txtSpeedTop;
        private System.Windows.Forms.Label label1;
    }
}