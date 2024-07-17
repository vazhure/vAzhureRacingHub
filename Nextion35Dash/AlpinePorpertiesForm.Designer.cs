namespace Nextion35Dash
{
    partial class AlpinePorpertiesForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AlpinePorpertiesForm));
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("BMW M8 GT3 2019");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Assetta Corsa Competizione", new System.Windows.Forms.TreeNode[] {
            treeNode1});
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("Lada VFTS");
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("Mercedes Benz AMG GT3");
            System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("rFactor 2", new System.Windows.Forms.TreeNode[] {
            treeNode3,
            treeNode4});
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.comboComPort = new System.Windows.Forms.ComboBox();
            this.comboBaudRate = new System.Windows.Forms.ComboBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.toolTips = new System.Windows.Forms.ToolTip(this.components);
            this.sliderLedBrightness = new vAzhureRacingAPI.VAzhureSliderControl();
            this.lblLedBrightness = new System.Windows.Forms.Label();
            this.chkEnableLeds = new vAzhureRacingAPI.VAzhureSwitchButton();
            this.chkEnabled = new vAzhureRacingAPI.VAzhureSwitchButton();
            this.btnFirmware = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.comboPresets = new System.Windows.Forms.ComboBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label4 = new System.Windows.Forms.Label();
            this.comboPage = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.comboPressureUnits = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.comboSpeedUnits = new System.Windows.Forms.ComboBox();
            this.treeProfiles = new Nextion35Dash.TreeViewExt();
            this.chkMainDevice = new vAzhureRacingAPI.VAzhureSwitchButton();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 337);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "COM Port";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 367);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Baud rate";
            this.toolTips.SetToolTip(this.label2, "Выберите скорость соединения");
            // 
            // comboComPort
            // 
            this.comboComPort.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(46)))), ((int)(((byte)(46)))));
            this.comboComPort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboComPort.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.comboComPort.ForeColor = System.Drawing.Color.White;
            this.comboComPort.FormattingEnabled = true;
            this.comboComPort.Location = new System.Drawing.Point(261, 333);
            this.comboComPort.Name = "comboComPort";
            this.comboComPort.Size = new System.Drawing.Size(128, 21);
            this.comboComPort.TabIndex = 1;
            this.toolTips.SetToolTip(this.comboComPort, "Укажите номер порта для соединения");
            // 
            // comboBaudRate
            // 
            this.comboBaudRate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(46)))), ((int)(((byte)(46)))));
            this.comboBaudRate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBaudRate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.comboBaudRate.ForeColor = System.Drawing.Color.White;
            this.comboBaudRate.FormattingEnabled = true;
            this.comboBaudRate.Location = new System.Drawing.Point(261, 363);
            this.comboBaudRate.Name = "comboBaudRate";
            this.comboBaudRate.Size = new System.Drawing.Size(128, 21);
            this.comboBaudRate.TabIndex = 3;
            this.toolTips.SetToolTip(this.comboBaudRate, "Выберите скорость соединения\nРекомендуемая скорость 57600");
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOK.Location = new System.Drawing.Point(566, 508);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(95, 31);
            this.btnOK.TabIndex = 17;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.BtnOK_Click);
            // 
            // toolTips
            // 
            this.toolTips.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(42)))), ((int)(((byte)(42)))));
            this.toolTips.ForeColor = System.Drawing.Color.White;
            this.toolTips.ShowAlways = true;
            this.toolTips.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.toolTips.ToolTipTitle = "Настройки Nextion";
            this.toolTips.UseAnimation = false;
            this.toolTips.UseFading = false;
            // 
            // sliderLedBrightness
            // 
            this.sliderLedBrightness.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.sliderLedBrightness.BigStep = 10;
            this.sliderLedBrightness.Location = new System.Drawing.Point(552, 54);
            this.sliderLedBrightness.Maximum = 20;
            this.sliderLedBrightness.Minimum = 0;
            this.sliderLedBrightness.MinimumSize = new System.Drawing.Size(16, 16);
            this.sliderLedBrightness.Name = "sliderLedBrightness";
            this.sliderLedBrightness.Padding = new System.Windows.Forms.Padding(1);
            this.sliderLedBrightness.Size = new System.Drawing.Size(177, 29);
            this.sliderLedBrightness.SliderColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(153)))), ((int)(((byte)(252)))));
            this.sliderLedBrightness.SmallStep = 5;
            this.sliderLedBrightness.Steps = 10;
            this.sliderLedBrightness.TabIndex = 7;
            this.toolTips.SetToolTip(this.sliderLedBrightness, "Leds Brightness");
            this.sliderLedBrightness.Value = 10;
            this.sliderLedBrightness.Vertical = false;
            this.sliderLedBrightness.OnValueChanged += new System.EventHandler(this.SliderLedBrightness_OnValueChanged);
            this.sliderLedBrightness.MouseUp += new System.Windows.Forms.MouseEventHandler(this.SliderLedBrightness_MouseUp);
            // 
            // lblLedBrightness
            // 
            this.lblLedBrightness.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblLedBrightness.Location = new System.Drawing.Point(735, 60);
            this.lblLedBrightness.Name = "lblLedBrightness";
            this.lblLedBrightness.Size = new System.Drawing.Size(50, 17);
            this.lblLedBrightness.TabIndex = 8;
            this.lblLedBrightness.Text = "0";
            this.lblLedBrightness.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.toolTips.SetToolTip(this.lblLedBrightness, "Яркость светодиодов");
            // 
            // chkEnableLeds
            // 
            this.chkEnableLeds.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(46)))), ((int)(((byte)(46)))));
            this.chkEnableLeds.Checked = true;
            this.chkEnableLeds.Location = new System.Drawing.Point(405, 51);
            this.chkEnableLeds.MaximumSize = new System.Drawing.Size(63, 32);
            this.chkEnableLeds.MinimumSize = new System.Drawing.Size(63, 32);
            this.chkEnableLeds.Name = "chkEnableLeds";
            this.chkEnableLeds.Size = new System.Drawing.Size(63, 32);
            this.chkEnableLeds.StateOff = ((System.Drawing.Image)(resources.GetObject("chkEnableLeds.StateOff")));
            this.chkEnableLeds.StateOn = ((System.Drawing.Image)(resources.GetObject("chkEnableLeds.StateOn")));
            this.chkEnableLeds.SwitchText = "Leds";
            this.chkEnableLeds.TabIndex = 6;
            this.toolTips.SetToolTip(this.chkEnableLeds, "Включение или отключение светодиодной полосы");
            this.chkEnableLeds.OnSwitch += new System.EventHandler(this.ChkEnableLeds_OnSwitch);
            // 
            // chkEnabled
            // 
            this.chkEnabled.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(46)))), ((int)(((byte)(46)))));
            this.chkEnabled.Checked = true;
            this.chkEnabled.Location = new System.Drawing.Point(405, 13);
            this.chkEnabled.MaximumSize = new System.Drawing.Size(80, 32);
            this.chkEnabled.MinimumSize = new System.Drawing.Size(80, 32);
            this.chkEnabled.Name = "chkEnabled";
            this.chkEnabled.Size = new System.Drawing.Size(80, 32);
            this.chkEnabled.StateOff = ((System.Drawing.Image)(resources.GetObject("chkEnabled.StateOff")));
            this.chkEnabled.StateOn = ((System.Drawing.Image)(resources.GetObject("chkEnabled.StateOn")));
            this.chkEnabled.SwitchText = "Enabled";
            this.chkEnabled.TabIndex = 5;
            this.toolTips.SetToolTip(this.chkEnabled, "Включение или отключение передачи данных на устройство");
            this.chkEnabled.OnSwitch += new System.EventHandler(this.ChkEnabled_OnSwitch);
            // 
            // btnFirmware
            // 
            this.btnFirmware.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnFirmware.FlatAppearance.BorderSize = 0;
            this.btnFirmware.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFirmware.Image = global::Nextion35Dash.Properties.Resources.download32;
            this.btnFirmware.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnFirmware.Location = new System.Drawing.Point(7, 508);
            this.btnFirmware.Name = "btnFirmware";
            this.btnFirmware.Size = new System.Drawing.Size(382, 31);
            this.btnFirmware.TabIndex = 4;
            this.btnFirmware.Text = "Firmware";
            this.btnFirmware.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.toolTips.SetToolTip(this.btnFirmware, "Открыть папку с прошивкой дисплея");
            this.btnFirmware.UseVisualStyleBackColor = true;
            this.btnFirmware.Click += new System.EventHandler(this.BtnFirmware_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDelete.FlatAppearance.BorderSize = 0;
            this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDelete.Image = global::Nextion35Dash.Properties.Resources.trash16;
            this.btnDelete.Location = new System.Drawing.Point(757, 189);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(26, 24);
            this.btnDelete.TabIndex = 15;
            this.toolTips.SetToolTip(this.btnDelete, "Delete Preset");
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.BtnDelete_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEdit.FlatAppearance.BorderSize = 0;
            this.btnEdit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEdit.Image = global::Nextion35Dash.Properties.Resources.pen16;
            this.btnEdit.Location = new System.Drawing.Point(725, 189);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(26, 24);
            this.btnEdit.TabIndex = 14;
            this.toolTips.SetToolTip(this.btnEdit, "Edit Preset");
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.BtnEdit_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAdd.FlatAppearance.BorderSize = 0;
            this.btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAdd.Image = global::Nextion35Dash.Properties.Resources.add16;
            this.btnAdd.Location = new System.Drawing.Point(693, 189);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(26, 24);
            this.btnAdd.TabIndex = 13;
            this.toolTips.SetToolTip(this.btnAdd, "Add Preset");
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.BtnAdd_Click);
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Location = new System.Drawing.Point(667, 508);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(116, 31);
            this.button1.TabIndex = 18;
            this.button1.Text = "Cancel";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.ButtonCancel_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(402, 192);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(37, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "Preset";
            // 
            // comboPresets
            // 
            this.comboPresets.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboPresets.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboPresets.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.comboPresets.FormattingEnabled = true;
            this.comboPresets.Items.AddRange(new object[] {
            "Авто",
            "Вручную"});
            this.comboPresets.Location = new System.Drawing.Point(478, 189);
            this.comboPresets.Name = "comboPresets";
            this.comboPresets.Size = new System.Drawing.Size(209, 21);
            this.comboPresets.TabIndex = 12;
            this.comboPresets.SelectedIndexChanged += new System.EventHandler(this.ComboPresets_SelectedIndexChanged);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Enabled = false;
            this.pictureBox1.Image = global::Nextion35Dash.Properties.Resources.AlpineDash35;
            this.pictureBox1.Location = new System.Drawing.Point(19, 13);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(370, 307);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(402, 96);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(60, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Dash Page";
            // 
            // comboPage
            // 
            this.comboPage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboPage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboPage.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.comboPage.FormattingEnabled = true;
            this.comboPage.Items.AddRange(new object[] {
            "page0",
            "page1",
            "page2"});
            this.comboPage.Location = new System.Drawing.Point(566, 93);
            this.comboPage.Name = "comboPage";
            this.comboPage.Size = new System.Drawing.Size(217, 21);
            this.comboPage.TabIndex = 10;
            this.comboPage.SelectedIndexChanged += new System.EventHandler(this.ComboPage_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(402, 131);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(75, 13);
            this.label5.TabIndex = 2;
            this.label5.Text = "Pressure Units";
            // 
            // comboPressureUnits
            // 
            this.comboPressureUnits.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboPressureUnits.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(46)))), ((int)(((byte)(46)))));
            this.comboPressureUnits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboPressureUnits.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.comboPressureUnits.ForeColor = System.Drawing.Color.White;
            this.comboPressureUnits.FormattingEnabled = true;
            this.comboPressureUnits.Items.AddRange(new object[] {
            "kPa",
            "PSI"});
            this.comboPressureUnits.Location = new System.Drawing.Point(566, 127);
            this.comboPressureUnits.Name = "comboPressureUnits";
            this.comboPressureUnits.Size = new System.Drawing.Size(217, 21);
            this.comboPressureUnits.TabIndex = 3;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(402, 161);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(65, 13);
            this.label6.TabIndex = 2;
            this.label6.Text = "Speed Units";
            // 
            // comboSpeedUnits
            // 
            this.comboSpeedUnits.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboSpeedUnits.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(46)))), ((int)(((byte)(46)))));
            this.comboSpeedUnits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboSpeedUnits.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.comboSpeedUnits.ForeColor = System.Drawing.Color.White;
            this.comboSpeedUnits.FormattingEnabled = true;
            this.comboSpeedUnits.Items.AddRange(new object[] {
            "km/h",
            "mph"});
            this.comboSpeedUnits.Location = new System.Drawing.Point(566, 157);
            this.comboSpeedUnits.Name = "comboSpeedUnits";
            this.comboSpeedUnits.Size = new System.Drawing.Size(217, 21);
            this.comboSpeedUnits.TabIndex = 3;
            // 
            // treeProfiles
            // 
            this.treeProfiles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeProfiles.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(46)))), ((int)(((byte)(46)))));
            this.treeProfiles.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.treeProfiles.ForeColor = System.Drawing.Color.White;
            this.treeProfiles.FullRowSelect = true;
            this.treeProfiles.HideSelection = false;
            this.treeProfiles.LineColor = System.Drawing.Color.White;
            this.treeProfiles.Location = new System.Drawing.Point(405, 219);
            this.treeProfiles.Name = "treeProfiles";
            treeNode1.Name = "Node4";
            treeNode1.Text = "BMW M8 GT3 2019";
            treeNode2.Name = "Node3";
            treeNode2.Text = "Assetta Corsa Competizione";
            treeNode3.Name = "Node2";
            treeNode3.Text = "Lada VFTS";
            treeNode4.Name = "Node1";
            treeNode4.Text = "Mercedes Benz AMG GT3";
            treeNode5.Name = "Node0";
            treeNode5.Text = "rFactor 2";
            this.treeProfiles.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode2,
            treeNode5});
            this.treeProfiles.Size = new System.Drawing.Size(380, 283);
            this.treeProfiles.TabIndex = 16;
            this.treeProfiles.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.TreeProfiles_AfterSelect);
            this.treeProfiles.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.TreeProfiles_NodeMouseDoubleClick);
            this.treeProfiles.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TreeProfiles_KeyDown);
            this.treeProfiles.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TreeProfiles_KeyUp);
            // 
            // chkMainDevice
            // 
            this.chkMainDevice.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(46)))), ((int)(((byte)(46)))));
            this.chkMainDevice.Checked = false;
            this.chkMainDevice.Location = new System.Drawing.Point(19, 393);
            this.chkMainDevice.MaximumSize = new System.Drawing.Size(115, 32);
            this.chkMainDevice.MinimumSize = new System.Drawing.Size(115, 32);
            this.chkMainDevice.Name = "chkMainDevice";
            this.chkMainDevice.Size = new System.Drawing.Size(115, 32);
            this.chkMainDevice.StateOff = ((System.Drawing.Image)(resources.GetObject("chkMainDevice.StateOff")));
            this.chkMainDevice.StateOn = ((System.Drawing.Image)(resources.GetObject("chkMainDevice.StateOn")));
            this.chkMainDevice.SwitchText = "Primary Device";
            this.chkMainDevice.TabIndex = 5;
            this.chkMainDevice.OnSwitch += new System.EventHandler(this.ChkMainDevice_OnSwitch);
            // 
            // AlpinePorpertiesForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(46)))), ((int)(((byte)(46)))));
            this.ClientSize = new System.Drawing.Size(794, 542);
            this.ControlBox = false;
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnEdit);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.treeProfiles);
            this.Controls.Add(this.comboPage);
            this.Controls.Add(this.comboPresets);
            this.Controls.Add(this.lblLedBrightness);
            this.Controls.Add(this.sliderLedBrightness);
            this.Controls.Add(this.chkEnableLeds);
            this.Controls.Add(this.chkMainDevice);
            this.Controls.Add(this.chkEnabled);
            this.Controls.Add(this.btnFirmware);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.comboSpeedUnits);
            this.Controls.Add(this.comboPressureUnits);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.comboBaudRate);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.comboComPort);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox1);
            this.ForeColor = System.Drawing.Color.White;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(810, 470);
            this.Name = "AlpinePorpertiesForm";
            this.Padding = new System.Windows.Forms.Padding(4);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "AlpinePorpertiesForm";
            this.Load += new System.EventHandler(this.NextionPropertiesForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboComPort;
        private System.Windows.Forms.ComboBox comboBaudRate;
        private System.Windows.Forms.Button btnOK;
        private vAzhureRacingAPI.VAzhureSwitchButton chkEnabled;
        private vAzhureRacingAPI.VAzhureSwitchButton chkEnableLeds;
        private System.Windows.Forms.Button btnFirmware;
        private System.Windows.Forms.ToolTip toolTips;
        private vAzhureRacingAPI.VAzhureSliderControl sliderLedBrightness;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label lblLedBrightness;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comboPresets;
        private TreeViewExt treeProfiles;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox comboPage;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox comboPressureUnits;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox comboSpeedUnits;
        private vAzhureRacingAPI.VAzhureSwitchButton chkMainDevice;
    }
}