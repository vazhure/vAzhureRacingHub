﻿
namespace MotionPlatform3
{
    partial class SettingsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.lblRearRightState = new System.Windows.Forms.Label();
            this.lblFrontState = new System.Windows.Forms.Label();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.lblRearLeftState = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblPosRL = new System.Windows.Forms.Label();
            this.lblPosFront = new System.Windows.Forms.Label();
            this.lblPosRR = new System.Windows.Forms.Label();
            this.lblTargetRL = new System.Windows.Forms.Label();
            this.lblTargetFront = new System.Windows.Forms.Label();
            this.lblTargetRR = new System.Windows.Forms.Label();
            this.btnHome = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnPark = new System.Windows.Forms.Button();
            this.sliderSpeed = new vAzhureRacingAPI.VAzhureSliderControl();
            this.chkEnabled = new vAzhureRacingAPI.VAzhureSwitchButton();
            this.label4 = new System.Windows.Forms.Label();
            this.sliderPitch = new vAzhureRacingAPI.VAzhureSliderControl();
            this.label5 = new System.Windows.Forms.Label();
            this.lblSpeed = new System.Windows.Forms.Label();
            this.lblPitch = new System.Windows.Forms.Label();
            this.sliderRoll = new vAzhureRacingAPI.VAzhureSliderControl();
            this.label8 = new System.Windows.Forms.Label();
            this.lblRoll = new System.Windows.Forms.Label();
            this.sliderHeave = new vAzhureRacingAPI.VAzhureSliderControl();
            this.label10 = new System.Windows.Forms.Label();
            this.lblHeave = new System.Windows.Forms.Label();
            this.sliderSurge = new vAzhureRacingAPI.VAzhureSliderControl();
            this.label12 = new System.Windows.Forms.Label();
            this.lblSurge = new System.Windows.Forms.Label();
            this.sliderSway = new vAzhureRacingAPI.VAzhureSliderControl();
            this.label14 = new System.Windows.Forms.Label();
            this.lblSway = new System.Windows.Forms.Label();
            this.chkInvertPitch = new vAzhureRacingAPI.VAzhureSwitchButton();
            this.chkInvertRoll = new vAzhureRacingAPI.VAzhureSwitchButton();
            this.chkInvertHeave = new vAzhureRacingAPI.VAzhureSwitchButton();
            this.chkInvertSurge = new vAzhureRacingAPI.VAzhureSwitchButton();
            this.chkInvertSway = new vAzhureRacingAPI.VAzhureSwitchButton();
            this.sliderOveralEffects = new vAzhureRacingAPI.VAzhureSliderControl();
            this.label16 = new System.Windows.Forms.Label();
            this.lblOveralEffects = new System.Windows.Forms.Label();
            this.btnApply = new System.Windows.Forms.Button();
            this.chkCollect = new vAzhureRacingAPI.VAzhureSwitchButton();
            this.btnAlarmReset = new System.Windows.Forms.Button();
            this.sliderSmooth = new vAzhureRacingAPI.VAzhureSliderControl();
            this.label6 = new System.Windows.Forms.Label();
            this.lblSmooth = new System.Windows.Forms.Label();
            this.comboComPort = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.lblGame = new System.Windows.Forms.Label();
            this.btnResetData = new System.Windows.Forms.Button();
            this.sliderLinearity = new vAzhureRacingAPI.VAzhureSliderControl();
            this.label9 = new System.Windows.Forms.Label();
            this.lblLinearity = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Image = global::MotionPlatform3.Properties.Resources._3dof;
            this.pictureBox1.Location = new System.Drawing.Point(3, 43);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(144, 301);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33028F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33486F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33486F));
            this.tableLayoutPanel1.Controls.Add(this.lblRearRightState, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblFrontState, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.pictureBox1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.pictureBox2, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.pictureBox3, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.lblRearLeftState, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.label2, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.label3, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.lblPosRL, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.lblPosFront, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.lblPosRR, 2, 3);
            this.tableLayoutPanel1.Controls.Add(this.lblTargetRL, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.lblTargetFront, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.lblTargetRR, 2, 4);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 71);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(453, 417);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // lblRearRightState
            // 
            this.lblRearRightState.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblRearRightState.Location = new System.Drawing.Point(304, 0);
            this.lblRearRightState.Name = "lblRearRightState";
            this.lblRearRightState.Size = new System.Drawing.Size(146, 40);
            this.lblRearRightState.TabIndex = 2;
            this.lblRearRightState.Text = "Unknown state";
            this.lblRearRightState.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblFrontState
            // 
            this.lblFrontState.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblFrontState.Location = new System.Drawing.Point(153, 0);
            this.lblFrontState.Name = "lblFrontState";
            this.lblFrontState.Size = new System.Drawing.Size(145, 40);
            this.lblFrontState.TabIndex = 1;
            this.lblFrontState.Text = "Unknown state";
            this.lblFrontState.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox2.Image = global::MotionPlatform3.Properties.Resources._3dof;
            this.pictureBox2.Location = new System.Drawing.Point(153, 43);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(145, 301);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox2.TabIndex = 0;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox3
            // 
            this.pictureBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox3.Image = global::MotionPlatform3.Properties.Resources._3dof;
            this.pictureBox3.Location = new System.Drawing.Point(304, 43);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(146, 301);
            this.pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox3.TabIndex = 0;
            this.pictureBox3.TabStop = false;
            // 
            // lblRearLeftState
            // 
            this.lblRearLeftState.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblRearLeftState.Location = new System.Drawing.Point(3, 0);
            this.lblRearLeftState.Name = "lblRearLeftState";
            this.lblRearLeftState.Size = new System.Drawing.Size(144, 40);
            this.lblRearLeftState.TabIndex = 0;
            this.lblRearLeftState.Text = "Unknown state";
            this.lblRearLeftState.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(3, 347);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(144, 30);
            this.label1.TabIndex = 3;
            this.label1.Text = "Rear Left";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Location = new System.Drawing.Point(153, 347);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(145, 30);
            this.label2.TabIndex = 4;
            this.label2.Text = "Front";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Location = new System.Drawing.Point(304, 347);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(146, 30);
            this.label3.TabIndex = 5;
            this.label3.Text = "Rear Right";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblPosRL
            // 
            this.lblPosRL.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblPosRL.Location = new System.Drawing.Point(3, 377);
            this.lblPosRL.Name = "lblPosRL";
            this.lblPosRL.Size = new System.Drawing.Size(144, 20);
            this.lblPosRL.TabIndex = 3;
            this.lblPosRL.Text = "-";
            this.lblPosRL.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblPosFront
            // 
            this.lblPosFront.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblPosFront.Location = new System.Drawing.Point(153, 377);
            this.lblPosFront.Name = "lblPosFront";
            this.lblPosFront.Size = new System.Drawing.Size(145, 20);
            this.lblPosFront.TabIndex = 3;
            this.lblPosFront.Text = "-";
            this.lblPosFront.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblPosRR
            // 
            this.lblPosRR.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblPosRR.Location = new System.Drawing.Point(304, 377);
            this.lblPosRR.Name = "lblPosRR";
            this.lblPosRR.Size = new System.Drawing.Size(146, 20);
            this.lblPosRR.TabIndex = 3;
            this.lblPosRR.Text = "-";
            this.lblPosRR.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblTargetRL
            // 
            this.lblTargetRL.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTargetRL.Location = new System.Drawing.Point(3, 397);
            this.lblTargetRL.Name = "lblTargetRL";
            this.lblTargetRL.Size = new System.Drawing.Size(144, 20);
            this.lblTargetRL.TabIndex = 3;
            this.lblTargetRL.Text = "-";
            this.lblTargetRL.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblTargetFront
            // 
            this.lblTargetFront.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTargetFront.Location = new System.Drawing.Point(153, 397);
            this.lblTargetFront.Name = "lblTargetFront";
            this.lblTargetFront.Size = new System.Drawing.Size(145, 20);
            this.lblTargetFront.TabIndex = 3;
            this.lblTargetFront.Text = "-";
            this.lblTargetFront.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblTargetRR
            // 
            this.lblTargetRR.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTargetRR.Location = new System.Drawing.Point(304, 397);
            this.lblTargetRR.Name = "lblTargetRR";
            this.lblTargetRR.Size = new System.Drawing.Size(146, 20);
            this.lblTargetRR.TabIndex = 3;
            this.lblTargetRR.Text = "-";
            this.lblTargetRR.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnHome
            // 
            this.btnHome.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnHome.Location = new System.Drawing.Point(12, 12);
            this.btnHome.Name = "btnHome";
            this.btnHome.Size = new System.Drawing.Size(147, 44);
            this.btnHome.TabIndex = 0;
            this.btnHome.Text = "HOME";
            this.btnHome.UseVisualStyleBackColor = true;
            this.btnHome.Click += new System.EventHandler(this.BtnHome_Click);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Location = new System.Drawing.Point(767, 12);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(147, 44);
            this.btnClose.TabIndex = 2;
            this.btnClose.Text = "CLOSE";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.BtnClose_Click);
            // 
            // btnPark
            // 
            this.btnPark.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPark.Location = new System.Drawing.Point(165, 12);
            this.btnPark.Name = "btnPark";
            this.btnPark.Size = new System.Drawing.Size(147, 44);
            this.btnPark.TabIndex = 1;
            this.btnPark.Text = "PARK";
            this.btnPark.UseVisualStyleBackColor = true;
            this.btnPark.Click += new System.EventHandler(this.ButtonPark_Click);
            // 
            // sliderSpeed
            // 
            this.sliderSpeed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.sliderSpeed.BigStep = 10;
            this.sliderSpeed.Location = new System.Drawing.Point(542, 116);
            this.sliderSpeed.Maximum = 250;
            this.sliderSpeed.Minimum = 27;
            this.sliderSpeed.MinimumSize = new System.Drawing.Size(16, 16);
            this.sliderSpeed.Name = "sliderSpeed";
            this.sliderSpeed.Padding = new System.Windows.Forms.Padding(1);
            this.sliderSpeed.Size = new System.Drawing.Size(161, 29);
            this.sliderSpeed.SliderColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(153)))), ((int)(((byte)(252)))));
            this.sliderSpeed.SmallStep = 5;
            this.sliderSpeed.Steps = 10;
            this.sliderSpeed.TabIndex = 5;
            this.sliderSpeed.Value = 250;
            this.sliderSpeed.Vertical = false;
            this.sliderSpeed.OnValueChanged += new System.EventHandler(this.SliderSpeed_OnValueChanged);
            // 
            // chkEnabled
            // 
            this.chkEnabled.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkEnabled.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(46)))), ((int)(((byte)(46)))));
            this.chkEnabled.Checked = true;
            this.chkEnabled.Location = new System.Drawing.Point(474, 71);
            this.chkEnabled.MaximumSize = new System.Drawing.Size(80, 32);
            this.chkEnabled.MinimumSize = new System.Drawing.Size(80, 32);
            this.chkEnabled.Name = "chkEnabled";
            this.chkEnabled.Size = new System.Drawing.Size(80, 32);
            this.chkEnabled.StateOff = ((System.Drawing.Image)(resources.GetObject("chkEnabled.StateOff")));
            this.chkEnabled.StateOn = ((System.Drawing.Image)(resources.GetObject("chkEnabled.StateOn")));
            this.chkEnabled.SwitchText = "Enabled";
            this.chkEnabled.TabIndex = 3;
            this.chkEnabled.OnSwitch += new System.EventHandler(this.ChkEnabled_OnSwitch);
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(471, 124);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(38, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Speed";
            // 
            // sliderPitch
            // 
            this.sliderPitch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.sliderPitch.BigStep = 10;
            this.sliderPitch.Location = new System.Drawing.Point(542, 186);
            this.sliderPitch.Maximum = 200;
            this.sliderPitch.Minimum = 0;
            this.sliderPitch.MinimumSize = new System.Drawing.Size(16, 16);
            this.sliderPitch.Name = "sliderPitch";
            this.sliderPitch.Padding = new System.Windows.Forms.Padding(1);
            this.sliderPitch.Size = new System.Drawing.Size(161, 29);
            this.sliderPitch.SliderColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(153)))), ((int)(((byte)(252)))));
            this.sliderPitch.SmallStep = 5;
            this.sliderPitch.Steps = 10;
            this.sliderPitch.TabIndex = 8;
            this.sliderPitch.Value = 20;
            this.sliderPitch.Vertical = false;
            this.sliderPitch.OnValueChanged += new System.EventHandler(this.SliderControl_OnValueChanged);
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(471, 194);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(31, 13);
            this.label5.TabIndex = 7;
            this.label5.Text = "Pitch";
            // 
            // lblSpeed
            // 
            this.lblSpeed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSpeed.AutoSize = true;
            this.lblSpeed.Location = new System.Drawing.Point(726, 124);
            this.lblSpeed.Name = "lblSpeed";
            this.lblSpeed.Size = new System.Drawing.Size(33, 13);
            this.lblSpeed.TabIndex = 6;
            this.lblSpeed.Text = "100%";
            // 
            // lblPitch
            // 
            this.lblPitch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPitch.AutoSize = true;
            this.lblPitch.Location = new System.Drawing.Point(726, 194);
            this.lblPitch.Name = "lblPitch";
            this.lblPitch.Size = new System.Drawing.Size(27, 13);
            this.lblPitch.TabIndex = 9;
            this.lblPitch.Text = "20%";
            // 
            // sliderRoll
            // 
            this.sliderRoll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.sliderRoll.BigStep = 10;
            this.sliderRoll.Location = new System.Drawing.Point(542, 221);
            this.sliderRoll.Maximum = 200;
            this.sliderRoll.Minimum = 0;
            this.sliderRoll.MinimumSize = new System.Drawing.Size(16, 16);
            this.sliderRoll.Name = "sliderRoll";
            this.sliderRoll.Padding = new System.Windows.Forms.Padding(1);
            this.sliderRoll.Size = new System.Drawing.Size(161, 29);
            this.sliderRoll.SliderColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(153)))), ((int)(((byte)(252)))));
            this.sliderRoll.SmallStep = 5;
            this.sliderRoll.Steps = 10;
            this.sliderRoll.TabIndex = 8;
            this.sliderRoll.Value = 20;
            this.sliderRoll.Vertical = false;
            this.sliderRoll.OnValueChanged += new System.EventHandler(this.SliderControl_OnValueChanged);
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(471, 229);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(25, 13);
            this.label8.TabIndex = 7;
            this.label8.Text = "Roll";
            // 
            // lblRoll
            // 
            this.lblRoll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblRoll.AutoSize = true;
            this.lblRoll.Location = new System.Drawing.Point(726, 229);
            this.lblRoll.Name = "lblRoll";
            this.lblRoll.Size = new System.Drawing.Size(27, 13);
            this.lblRoll.TabIndex = 9;
            this.lblRoll.Text = "20%";
            // 
            // sliderHeave
            // 
            this.sliderHeave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.sliderHeave.BigStep = 10;
            this.sliderHeave.Location = new System.Drawing.Point(542, 256);
            this.sliderHeave.Maximum = 200;
            this.sliderHeave.Minimum = 0;
            this.sliderHeave.MinimumSize = new System.Drawing.Size(16, 16);
            this.sliderHeave.Name = "sliderHeave";
            this.sliderHeave.Padding = new System.Windows.Forms.Padding(1);
            this.sliderHeave.Size = new System.Drawing.Size(161, 29);
            this.sliderHeave.SliderColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(153)))), ((int)(((byte)(252)))));
            this.sliderHeave.SmallStep = 5;
            this.sliderHeave.Steps = 10;
            this.sliderHeave.TabIndex = 8;
            this.sliderHeave.Value = 20;
            this.sliderHeave.Vertical = false;
            this.sliderHeave.OnValueChanged += new System.EventHandler(this.SliderControl_OnValueChanged);
            // 
            // label10
            // 
            this.label10.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(471, 264);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(39, 13);
            this.label10.TabIndex = 7;
            this.label10.Text = "Heave";
            // 
            // lblHeave
            // 
            this.lblHeave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblHeave.AutoSize = true;
            this.lblHeave.Location = new System.Drawing.Point(726, 264);
            this.lblHeave.Name = "lblHeave";
            this.lblHeave.Size = new System.Drawing.Size(27, 13);
            this.lblHeave.TabIndex = 9;
            this.lblHeave.Text = "20%";
            // 
            // sliderSurge
            // 
            this.sliderSurge.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.sliderSurge.BigStep = 10;
            this.sliderSurge.Location = new System.Drawing.Point(542, 291);
            this.sliderSurge.Maximum = 200;
            this.sliderSurge.Minimum = 0;
            this.sliderSurge.MinimumSize = new System.Drawing.Size(16, 16);
            this.sliderSurge.Name = "sliderSurge";
            this.sliderSurge.Padding = new System.Windows.Forms.Padding(1);
            this.sliderSurge.Size = new System.Drawing.Size(161, 29);
            this.sliderSurge.SliderColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(153)))), ((int)(((byte)(252)))));
            this.sliderSurge.SmallStep = 5;
            this.sliderSurge.Steps = 10;
            this.sliderSurge.TabIndex = 8;
            this.sliderSurge.Value = 20;
            this.sliderSurge.Vertical = false;
            this.sliderSurge.OnValueChanged += new System.EventHandler(this.SliderControl_OnValueChanged);
            // 
            // label12
            // 
            this.label12.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(471, 299);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(35, 13);
            this.label12.TabIndex = 7;
            this.label12.Text = "Surge";
            // 
            // lblSurge
            // 
            this.lblSurge.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSurge.AutoSize = true;
            this.lblSurge.Location = new System.Drawing.Point(726, 299);
            this.lblSurge.Name = "lblSurge";
            this.lblSurge.Size = new System.Drawing.Size(27, 13);
            this.lblSurge.TabIndex = 9;
            this.lblSurge.Text = "20%";
            // 
            // sliderSway
            // 
            this.sliderSway.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.sliderSway.BigStep = 10;
            this.sliderSway.Location = new System.Drawing.Point(542, 326);
            this.sliderSway.Maximum = 200;
            this.sliderSway.Minimum = 0;
            this.sliderSway.MinimumSize = new System.Drawing.Size(16, 16);
            this.sliderSway.Name = "sliderSway";
            this.sliderSway.Padding = new System.Windows.Forms.Padding(1);
            this.sliderSway.Size = new System.Drawing.Size(161, 29);
            this.sliderSway.SliderColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(153)))), ((int)(((byte)(252)))));
            this.sliderSway.SmallStep = 5;
            this.sliderSway.Steps = 10;
            this.sliderSway.TabIndex = 8;
            this.sliderSway.Value = 20;
            this.sliderSway.Vertical = false;
            this.sliderSway.OnValueChanged += new System.EventHandler(this.SliderControl_OnValueChanged);
            // 
            // label14
            // 
            this.label14.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(471, 334);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(33, 13);
            this.label14.TabIndex = 7;
            this.label14.Text = "Sway";
            // 
            // lblSway
            // 
            this.lblSway.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSway.AutoSize = true;
            this.lblSway.Location = new System.Drawing.Point(726, 334);
            this.lblSway.Name = "lblSway";
            this.lblSway.Size = new System.Drawing.Size(27, 13);
            this.lblSway.TabIndex = 9;
            this.lblSway.Text = "20%";
            // 
            // chkInvertPitch
            // 
            this.chkInvertPitch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkInvertPitch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(46)))), ((int)(((byte)(46)))));
            this.chkInvertPitch.Checked = false;
            this.chkInvertPitch.Location = new System.Drawing.Point(766, 184);
            this.chkInvertPitch.MaximumSize = new System.Drawing.Size(66, 32);
            this.chkInvertPitch.MinimumSize = new System.Drawing.Size(66, 32);
            this.chkInvertPitch.Name = "chkInvertPitch";
            this.chkInvertPitch.Size = new System.Drawing.Size(66, 32);
            this.chkInvertPitch.StateOff = ((System.Drawing.Image)(resources.GetObject("chkInvertPitch.StateOff")));
            this.chkInvertPitch.StateOn = ((System.Drawing.Image)(resources.GetObject("chkInvertPitch.StateOn")));
            this.chkInvertPitch.SwitchText = "Invert";
            this.chkInvertPitch.TabIndex = 3;
            // 
            // chkInvertRoll
            // 
            this.chkInvertRoll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkInvertRoll.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(46)))), ((int)(((byte)(46)))));
            this.chkInvertRoll.Checked = false;
            this.chkInvertRoll.Location = new System.Drawing.Point(766, 219);
            this.chkInvertRoll.MaximumSize = new System.Drawing.Size(66, 32);
            this.chkInvertRoll.MinimumSize = new System.Drawing.Size(66, 32);
            this.chkInvertRoll.Name = "chkInvertRoll";
            this.chkInvertRoll.Size = new System.Drawing.Size(66, 32);
            this.chkInvertRoll.StateOff = ((System.Drawing.Image)(resources.GetObject("chkInvertRoll.StateOff")));
            this.chkInvertRoll.StateOn = ((System.Drawing.Image)(resources.GetObject("chkInvertRoll.StateOn")));
            this.chkInvertRoll.SwitchText = "Invert";
            this.chkInvertRoll.TabIndex = 3;
            // 
            // chkInvertHeave
            // 
            this.chkInvertHeave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkInvertHeave.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(46)))), ((int)(((byte)(46)))));
            this.chkInvertHeave.Checked = false;
            this.chkInvertHeave.Location = new System.Drawing.Point(766, 254);
            this.chkInvertHeave.MaximumSize = new System.Drawing.Size(66, 32);
            this.chkInvertHeave.MinimumSize = new System.Drawing.Size(66, 32);
            this.chkInvertHeave.Name = "chkInvertHeave";
            this.chkInvertHeave.Size = new System.Drawing.Size(66, 32);
            this.chkInvertHeave.StateOff = ((System.Drawing.Image)(resources.GetObject("chkInvertHeave.StateOff")));
            this.chkInvertHeave.StateOn = ((System.Drawing.Image)(resources.GetObject("chkInvertHeave.StateOn")));
            this.chkInvertHeave.SwitchText = "Invert";
            this.chkInvertHeave.TabIndex = 3;
            // 
            // chkInvertSurge
            // 
            this.chkInvertSurge.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkInvertSurge.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(46)))), ((int)(((byte)(46)))));
            this.chkInvertSurge.Checked = false;
            this.chkInvertSurge.Location = new System.Drawing.Point(766, 289);
            this.chkInvertSurge.MaximumSize = new System.Drawing.Size(66, 32);
            this.chkInvertSurge.MinimumSize = new System.Drawing.Size(66, 32);
            this.chkInvertSurge.Name = "chkInvertSurge";
            this.chkInvertSurge.Size = new System.Drawing.Size(66, 32);
            this.chkInvertSurge.StateOff = ((System.Drawing.Image)(resources.GetObject("chkInvertSurge.StateOff")));
            this.chkInvertSurge.StateOn = ((System.Drawing.Image)(resources.GetObject("chkInvertSurge.StateOn")));
            this.chkInvertSurge.SwitchText = "Invert";
            this.chkInvertSurge.TabIndex = 3;
            // 
            // chkInvertSway
            // 
            this.chkInvertSway.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkInvertSway.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(46)))), ((int)(((byte)(46)))));
            this.chkInvertSway.Checked = false;
            this.chkInvertSway.Location = new System.Drawing.Point(766, 324);
            this.chkInvertSway.MaximumSize = new System.Drawing.Size(66, 32);
            this.chkInvertSway.MinimumSize = new System.Drawing.Size(66, 32);
            this.chkInvertSway.Name = "chkInvertSway";
            this.chkInvertSway.Size = new System.Drawing.Size(66, 32);
            this.chkInvertSway.StateOff = ((System.Drawing.Image)(resources.GetObject("chkInvertSway.StateOff")));
            this.chkInvertSway.StateOn = ((System.Drawing.Image)(resources.GetObject("chkInvertSway.StateOn")));
            this.chkInvertSway.SwitchText = "Invert";
            this.chkInvertSway.TabIndex = 3;
            // 
            // sliderOveralEffects
            // 
            this.sliderOveralEffects.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.sliderOveralEffects.BigStep = 10;
            this.sliderOveralEffects.Location = new System.Drawing.Point(542, 151);
            this.sliderOveralEffects.Maximum = 200;
            this.sliderOveralEffects.Minimum = 0;
            this.sliderOveralEffects.MinimumSize = new System.Drawing.Size(16, 16);
            this.sliderOveralEffects.Name = "sliderOveralEffects";
            this.sliderOveralEffects.Padding = new System.Windows.Forms.Padding(1);
            this.sliderOveralEffects.Size = new System.Drawing.Size(161, 29);
            this.sliderOveralEffects.SliderColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(153)))), ((int)(((byte)(252)))));
            this.sliderOveralEffects.SmallStep = 5;
            this.sliderOveralEffects.Steps = 10;
            this.sliderOveralEffects.TabIndex = 8;
            this.sliderOveralEffects.Value = 100;
            this.sliderOveralEffects.Vertical = false;
            this.sliderOveralEffects.OnValueChanged += new System.EventHandler(this.SliderControl_OnValueChanged);
            // 
            // label16
            // 
            this.label16.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(471, 159);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(38, 13);
            this.label16.TabIndex = 7;
            this.label16.Text = "Overal";
            // 
            // lblOveralEffects
            // 
            this.lblOveralEffects.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblOveralEffects.AutoSize = true;
            this.lblOveralEffects.Location = new System.Drawing.Point(726, 159);
            this.lblOveralEffects.Name = "lblOveralEffects";
            this.lblOveralEffects.Size = new System.Drawing.Size(33, 13);
            this.lblOveralEffects.TabIndex = 9;
            this.lblOveralEffects.Text = "100%";
            // 
            // btnApply
            // 
            this.btnApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnApply.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnApply.Location = new System.Drawing.Point(767, 441);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(147, 44);
            this.btnApply.TabIndex = 2;
            this.btnApply.Text = "APPLY";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.ButtonApply_Click);
            // 
            // chkCollect
            // 
            this.chkCollect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkCollect.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(46)))), ((int)(((byte)(46)))));
            this.chkCollect.Checked = true;
            this.chkCollect.Location = new System.Drawing.Point(474, 455);
            this.chkCollect.MaximumSize = new System.Drawing.Size(98, 32);
            this.chkCollect.MinimumSize = new System.Drawing.Size(98, 32);
            this.chkCollect.Name = "chkCollect";
            this.chkCollect.Size = new System.Drawing.Size(98, 32);
            this.chkCollect.StateOff = ((System.Drawing.Image)(resources.GetObject("chkCollect.StateOff")));
            this.chkCollect.StateOn = ((System.Drawing.Image)(resources.GetObject("chkCollect.StateOn")));
            this.chkCollect.SwitchText = "Collect data";
            this.chkCollect.TabIndex = 3;
            this.chkCollect.OnSwitch += new System.EventHandler(this.ChkCollect_OnSwitch);
            // 
            // btnAlarmReset
            // 
            this.btnAlarmReset.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAlarmReset.Location = new System.Drawing.Point(318, 12);
            this.btnAlarmReset.Name = "btnAlarmReset";
            this.btnAlarmReset.Size = new System.Drawing.Size(147, 44);
            this.btnAlarmReset.TabIndex = 1;
            this.btnAlarmReset.Text = "RESET";
            this.btnAlarmReset.UseVisualStyleBackColor = true;
            this.btnAlarmReset.Click += new System.EventHandler(this.BtnAlarmReset_Click);
            // 
            // sliderSmooth
            // 
            this.sliderSmooth.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.sliderSmooth.BigStep = 10;
            this.sliderSmooth.Location = new System.Drawing.Point(542, 361);
            this.sliderSmooth.Maximum = 100;
            this.sliderSmooth.Minimum = 0;
            this.sliderSmooth.MinimumSize = new System.Drawing.Size(16, 16);
            this.sliderSmooth.Name = "sliderSmooth";
            this.sliderSmooth.Padding = new System.Windows.Forms.Padding(1);
            this.sliderSmooth.Size = new System.Drawing.Size(161, 29);
            this.sliderSmooth.SliderColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(153)))), ((int)(((byte)(252)))));
            this.sliderSmooth.SmallStep = 5;
            this.sliderSmooth.Steps = 10;
            this.sliderSmooth.TabIndex = 5;
            this.sliderSmooth.Value = 90;
            this.sliderSmooth.Vertical = false;
            this.sliderSmooth.OnValueChanged += new System.EventHandler(this.SliderControl_OnValueChanged);
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(471, 369);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(43, 13);
            this.label6.TabIndex = 4;
            this.label6.Text = "Smooth";
            // 
            // lblSmooth
            // 
            this.lblSmooth.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSmooth.AutoSize = true;
            this.lblSmooth.Location = new System.Drawing.Point(726, 369);
            this.lblSmooth.Name = "lblSmooth";
            this.lblSmooth.Size = new System.Drawing.Size(33, 13);
            this.lblSmooth.TabIndex = 6;
            this.lblSmooth.Text = "100%";
            // 
            // comboComPort
            // 
            this.comboComPort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.comboComPort.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(46)))), ((int)(((byte)(46)))));
            this.comboComPort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboComPort.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.comboComPort.ForeColor = System.Drawing.Color.White;
            this.comboComPort.FormattingEnabled = true;
            this.comboComPort.Location = new System.Drawing.Point(474, 35);
            this.comboComPort.Name = "comboComPort";
            this.comboComPort.Size = new System.Drawing.Size(128, 21);
            this.comboComPort.TabIndex = 11;
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(471, 12);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(53, 13);
            this.label7.TabIndex = 10;
            this.label7.Text = "COM Port";
            // 
            // lblGame
            // 
            this.lblGame.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblGame.AutoSize = true;
            this.lblGame.Location = new System.Drawing.Point(474, 434);
            this.lblGame.Name = "lblGame";
            this.lblGame.Size = new System.Drawing.Size(68, 13);
            this.lblGame.TabIndex = 12;
            this.lblGame.Text = "Active Game";
            // 
            // btnResetData
            // 
            this.btnResetData.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnResetData.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnResetData.Location = new System.Drawing.Point(594, 458);
            this.btnResetData.Name = "btnResetData";
            this.btnResetData.Size = new System.Drawing.Size(92, 27);
            this.btnResetData.TabIndex = 1;
            this.btnResetData.Text = "RESET";
            this.btnResetData.UseVisualStyleBackColor = true;
            this.btnResetData.Click += new System.EventHandler(this.BtnResetData_Click);
            // 
            // sliderLinearity
            // 
            this.sliderLinearity.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.sliderLinearity.BigStep = 10;
            this.sliderLinearity.Location = new System.Drawing.Point(542, 396);
            this.sliderLinearity.Maximum = 100;
            this.sliderLinearity.Minimum = 0;
            this.sliderLinearity.MinimumSize = new System.Drawing.Size(16, 16);
            this.sliderLinearity.Name = "sliderLinearity";
            this.sliderLinearity.Padding = new System.Windows.Forms.Padding(1);
            this.sliderLinearity.Size = new System.Drawing.Size(161, 29);
            this.sliderLinearity.SliderColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(153)))), ((int)(((byte)(252)))));
            this.sliderLinearity.SmallStep = 5;
            this.sliderLinearity.Steps = 10;
            this.sliderLinearity.TabIndex = 5;
            this.sliderLinearity.Value = 0;
            this.sliderLinearity.Vertical = false;
            this.sliderLinearity.OnValueChanged += new System.EventHandler(this.SliderControl_OnValueChanged);
            // 
            // label9
            // 
            this.label9.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(471, 404);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(46, 13);
            this.label9.TabIndex = 4;
            this.label9.Text = "Linearity";
            // 
            // lblLinearity
            // 
            this.lblLinearity.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblLinearity.AutoSize = true;
            this.lblLinearity.Location = new System.Drawing.Point(726, 404);
            this.lblLinearity.Name = "lblLinearity";
            this.lblLinearity.Size = new System.Drawing.Size(33, 13);
            this.lblLinearity.TabIndex = 6;
            this.lblLinearity.Text = "100%";
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(46)))), ((int)(((byte)(46)))));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.ClientSize = new System.Drawing.Size(924, 494);
            this.ControlBox = false;
            this.Controls.Add(this.lblGame);
            this.Controls.Add(this.comboComPort);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.lblSway);
            this.Controls.Add(this.lblSurge);
            this.Controls.Add(this.lblHeave);
            this.Controls.Add(this.lblRoll);
            this.Controls.Add(this.lblOveralEffects);
            this.Controls.Add(this.lblPitch);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.sliderSway);
            this.Controls.Add(this.sliderSurge);
            this.Controls.Add(this.sliderHeave);
            this.Controls.Add(this.sliderRoll);
            this.Controls.Add(this.sliderOveralEffects);
            this.Controls.Add(this.sliderPitch);
            this.Controls.Add(this.lblLinearity);
            this.Controls.Add(this.lblSmooth);
            this.Controls.Add(this.lblSpeed);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.sliderLinearity);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.sliderSmooth);
            this.Controls.Add(this.sliderSpeed);
            this.Controls.Add(this.chkInvertSway);
            this.Controls.Add(this.chkInvertSurge);
            this.Controls.Add(this.chkInvertHeave);
            this.Controls.Add(this.chkInvertRoll);
            this.Controls.Add(this.chkInvertPitch);
            this.Controls.Add(this.chkCollect);
            this.Controls.Add(this.chkEnabled);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnResetData);
            this.Controls.Add(this.btnAlarmReset);
            this.Controls.Add(this.btnPark);
            this.Controls.Add(this.btnHome);
            this.Controls.Add(this.tableLayoutPanel1);
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.Color.White;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(940, 510);
            this.Name = "SettingsForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "SettingsForm";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label lblRearRightState;
        private System.Windows.Forms.Label lblFrontState;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.Label lblRearLeftState;
        private System.Windows.Forms.Button btnHome;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnPark;
        private vAzhureRacingAPI.VAzhureSliderControl sliderSpeed;
        private vAzhureRacingAPI.VAzhureSwitchButton chkEnabled;
        private System.Windows.Forms.Label label4;
        private vAzhureRacingAPI.VAzhureSliderControl sliderPitch;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblSpeed;
        private System.Windows.Forms.Label lblPitch;
        private vAzhureRacingAPI.VAzhureSliderControl sliderRoll;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label lblRoll;
        private vAzhureRacingAPI.VAzhureSliderControl sliderHeave;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label lblHeave;
        private vAzhureRacingAPI.VAzhureSliderControl sliderSurge;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label lblSurge;
        private vAzhureRacingAPI.VAzhureSliderControl sliderSway;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label lblSway;
        private vAzhureRacingAPI.VAzhureSwitchButton chkInvertPitch;
        private vAzhureRacingAPI.VAzhureSwitchButton chkInvertRoll;
        private vAzhureRacingAPI.VAzhureSwitchButton chkInvertHeave;
        private vAzhureRacingAPI.VAzhureSwitchButton chkInvertSurge;
        private vAzhureRacingAPI.VAzhureSwitchButton chkInvertSway;
        private vAzhureRacingAPI.VAzhureSliderControl sliderOveralEffects;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label lblOveralEffects;
        private System.Windows.Forms.Button btnApply;
        private vAzhureRacingAPI.VAzhureSwitchButton chkCollect;
        private System.Windows.Forms.Button btnAlarmReset;
        private System.Windows.Forms.Label lblPosRL;
        private System.Windows.Forms.Label lblPosFront;
        private System.Windows.Forms.Label lblPosRR;
        private System.Windows.Forms.Label lblTargetRL;
        private System.Windows.Forms.Label lblTargetFront;
        private System.Windows.Forms.Label lblTargetRR;
        private vAzhureRacingAPI.VAzhureSliderControl sliderSmooth;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lblSmooth;
        private System.Windows.Forms.ComboBox comboComPort;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label lblGame;
        private System.Windows.Forms.Button btnResetData;
        private vAzhureRacingAPI.VAzhureSliderControl sliderLinearity;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label lblLinearity;
    }
}