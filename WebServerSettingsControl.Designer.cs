
namespace vAzhureRacingHub
{
    partial class WebServerSettingsControl
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
            webServer.OnClientConnected -= WebServer_OnClientConnected;
            webServer.OnClientDisconnected -= WebServer_OnClientDisconnected;

            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WebServerSettingsControl));
            this.btnSelectFolder = new System.Windows.Forms.Button();
            this.labelPath = new System.Windows.Forms.Label();
            this.labelFolder = new System.Windows.Forms.Label();
            this.btnEnabled = new vAzhureRacingAPI.VAzhureSwitchButton();
            this.listClients = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.numPort = new System.Windows.Forms.NumericUpDown();
            this.lblIP = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numPort)).BeginInit();
            this.SuspendLayout();
            // 
            // btnSelectFolder
            // 
            this.btnSelectFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelectFolder.FlatAppearance.BorderSize = 0;
            this.btnSelectFolder.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSelectFolder.Location = new System.Drawing.Point(443, 0);
            this.btnSelectFolder.Name = "btnSelectFolder";
            this.btnSelectFolder.Size = new System.Drawing.Size(62, 23);
            this.btnSelectFolder.TabIndex = 5;
            this.btnSelectFolder.Text = "..";
            this.btnSelectFolder.UseVisualStyleBackColor = true;
            this.btnSelectFolder.Click += new System.EventHandler(this.BtnSelectFolder_Click);
            // 
            // labelPath
            // 
            this.labelPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelPath.Cursor = System.Windows.Forms.Cursors.Hand;
            this.labelPath.Location = new System.Drawing.Point(175, 0);
            this.labelPath.Name = "labelPath";
            this.labelPath.Size = new System.Drawing.Size(262, 23);
            this.labelPath.TabIndex = 3;
            this.labelPath.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelPath.Click += new System.EventHandler(this.LabelPath_Click);
            // 
            // labelFolder
            // 
            this.labelFolder.Location = new System.Drawing.Point(3, 0);
            this.labelFolder.Name = "labelFolder";
            this.labelFolder.Size = new System.Drawing.Size(166, 23);
            this.labelFolder.TabIndex = 4;
            this.labelFolder.Text = "Server folder";
            this.labelFolder.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnEnabled
            // 
            this.btnEnabled.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.btnEnabled.Checked = false;
            this.btnEnabled.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnEnabled.Location = new System.Drawing.Point(16, 31);
            this.btnEnabled.MaximumSize = new System.Drawing.Size(74, 32);
            this.btnEnabled.MinimumSize = new System.Drawing.Size(74, 32);
            this.btnEnabled.Name = "btnEnabled";
            this.btnEnabled.Size = new System.Drawing.Size(74, 32);
            this.btnEnabled.StateOff = ((System.Drawing.Image)(resources.GetObject("btnEnabled.StateOff")));
            this.btnEnabled.StateOn = ((System.Drawing.Image)(resources.GetObject("btnEnabled.StateOn")));
            this.btnEnabled.SwitchText = "Enable";
            this.btnEnabled.TabIndex = 8;
            this.btnEnabled.OnSwitch += new System.EventHandler(this.BtnEnabled_OnSwitch);
            // 
            // listClients
            // 
            this.listClients.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listClients.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(46)))), ((int)(((byte)(46)))));
            this.listClients.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listClients.ForeColor = System.Drawing.Color.White;
            this.listClients.IntegralHeight = false;
            this.listClients.Items.AddRange(new object[] {
            "192.168.1.17",
            "192.168.1.2",
            "192.168.1.5"});
            this.listClients.Location = new System.Drawing.Point(16, 92);
            this.listClients.Name = "listClients";
            this.listClients.Size = new System.Drawing.Size(474, 174);
            this.listClients.TabIndex = 9;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(3, 66);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(166, 23);
            this.label1.TabIndex = 4;
            this.label1.Text = "Clients";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(163, 36);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 23);
            this.label2.TabIndex = 4;
            this.label2.Text = "Port";
            // 
            // numPort
            // 
            this.numPort.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.numPort.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.numPort.ForeColor = System.Drawing.Color.White;
            this.numPort.Location = new System.Drawing.Point(236, 38);
            this.numPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numPort.Minimum = new decimal(new int[] {
            1024,
            0,
            0,
            0});
            this.numPort.Name = "numPort";
            this.numPort.Size = new System.Drawing.Size(89, 16);
            this.numPort.TabIndex = 10;
            this.numPort.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numPort.Value = new decimal(new int[] {
            8080,
            0,
            0,
            0});
            this.numPort.ValueChanged += new System.EventHandler(this.NumPort_ValueChanged);
            // 
            // lblIP
            // 
            this.lblIP.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblIP.AutoSize = true;
            this.lblIP.Location = new System.Drawing.Point(3, 273);
            this.lblIP.Name = "lblIP";
            this.lblIP.Size = new System.Drawing.Size(20, 13);
            this.lblIP.TabIndex = 4;
            this.lblIP.Text = "IP:";
            // 
            // WebServerSettingsControl
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.Controls.Add(this.numPort);
            this.Controls.Add(this.listClients);
            this.Controls.Add(this.btnEnabled);
            this.Controls.Add(this.btnSelectFolder);
            this.Controls.Add(this.labelPath);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblIP);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.labelFolder);
            this.ForeColor = System.Drawing.Color.White;
            this.Name = "WebServerSettingsControl";
            this.Size = new System.Drawing.Size(508, 291);
            ((System.ComponentModel.ISupportInitialize)(this.numPort)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSelectFolder;
        private System.Windows.Forms.Label labelPath;
        private System.Windows.Forms.Label labelFolder;
        private vAzhureRacingAPI.VAzhureSwitchButton btnEnabled;
        private System.Windows.Forms.ListBox listClients;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numPort;
        private System.Windows.Forms.Label lblIP;
    }
}
