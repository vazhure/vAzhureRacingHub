
namespace vAzhureRacingHub
{
    partial class AppWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AppWindow));
            this.btnMinimize = new System.Windows.Forms.Button();
            this.btnExpand = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.toolTips = new System.Windows.Forms.ToolTip(this.components);
            this.btnSound = new vAzhureRacingAPI.VAzhureSwitchButton();
            this.volumeControl = new vAzhureRacingAPI.VAzhureVolumeControl();
            this.btnAbout = new System.Windows.Forms.Button();
            this.btnSettings = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.titlesGames = new vAzhureRacingAPI.VAzhureTilesControl();
            this.titlesDevices = new vAzhureRacingAPI.VAzhureTilesControl();
            this.SuspendLayout();
            // 
            // btnMinimize
            // 
            this.btnMinimize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMinimize.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnMinimize.FlatAppearance.BorderSize = 0;
            this.btnMinimize.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMinimize.Image = global::vAzhureRacingHub.Properties.Resources.collapse_window;
            this.btnMinimize.Location = new System.Drawing.Point(638, 6);
            this.btnMinimize.Name = "btnMinimize";
            this.btnMinimize.Size = new System.Drawing.Size(40, 40);
            this.btnMinimize.TabIndex = 3;
            this.toolTips.SetToolTip(this.btnMinimize, "Collapse");
            this.btnMinimize.UseVisualStyleBackColor = true;
            this.btnMinimize.Click += new System.EventHandler(this.BtnMinimize_Click);
            // 
            // btnExpand
            // 
            this.btnExpand.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExpand.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnExpand.FlatAppearance.BorderSize = 0;
            this.btnExpand.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExpand.Image = global::vAzhureRacingHub.Properties.Resources.expand_window;
            this.btnExpand.Location = new System.Drawing.Point(684, 6);
            this.btnExpand.Name = "btnExpand";
            this.btnExpand.Size = new System.Drawing.Size(40, 40);
            this.btnExpand.TabIndex = 4;
            this.toolTips.SetToolTip(this.btnExpand, "Maximize");
            this.btnExpand.UseVisualStyleBackColor = true;
            this.btnExpand.Click += new System.EventHandler(this.BtnExpand_Click);
            // 
            // btnExit
            // 
            this.btnExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExit.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnExit.FlatAppearance.BorderSize = 0;
            this.btnExit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExit.Image = global::vAzhureRacingHub.Properties.Resources.close_window;
            this.btnExit.Location = new System.Drawing.Point(730, 6);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(40, 40);
            this.btnExit.TabIndex = 5;
            this.toolTips.SetToolTip(this.btnExit, "Close application");
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.BtnExit_Click);
            // 
            // toolTips
            // 
            this.toolTips.AutoPopDelay = 5000;
            this.toolTips.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(46)))), ((int)(((byte)(46)))));
            this.toolTips.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.toolTips.InitialDelay = 250;
            this.toolTips.ReshowDelay = 100;
            this.toolTips.ShowAlways = true;
            this.toolTips.ToolTipTitle = "vAzhure Racing Hub";
            this.toolTips.UseAnimation = false;
            this.toolTips.UseFading = false;
            // 
            // btnSound
            // 
            this.btnSound.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSound.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(46)))), ((int)(((byte)(46)))));
            this.btnSound.Checked = true;
            this.btnSound.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSound.Location = new System.Drawing.Point(648, 412);
            this.btnSound.MaximumSize = new System.Drawing.Size(27, 24);
            this.btnSound.MinimumSize = new System.Drawing.Size(27, 24);
            this.btnSound.Name = "btnSound";
            this.btnSound.Size = new System.Drawing.Size(35, 32);
            this.btnSound.StateOff = ((System.Drawing.Image)(resources.GetObject("btnSound.StateOff")));
            this.btnSound.StateOn = ((System.Drawing.Image)(resources.GetObject("btnSound.StateOn")));
            this.btnSound.SwitchText = "";
            this.btnSound.TabIndex = 6;
            this.toolTips.SetToolTip(this.btnSound, "Mute / Unmute");
            this.btnSound.OnSwitch += new System.EventHandler(this.BtnSound_OnSwitch);
            // 
            // volumeControl
            // 
            this.volumeControl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.volumeControl.Location = new System.Drawing.Point(684, 414);
            this.volumeControl.Maximum = 100;
            this.volumeControl.Minimum = 0;
            this.volumeControl.Name = "volumeControl";
            this.volumeControl.Padding = new System.Windows.Forms.Padding(2);
            this.volumeControl.Size = new System.Drawing.Size(89, 24);
            this.volumeControl.Steps = 10;
            this.volumeControl.TabIndex = 7;
            this.toolTips.SetToolTip(this.volumeControl, "Sound level");
            this.volumeControl.Value = 50;
            this.volumeControl.OnValueChanged += new System.EventHandler(this.VolumeControl_OnValueChanged);
            // 
            // btnAbout
            // 
            this.btnAbout.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAbout.FlatAppearance.BorderSize = 0;
            this.btnAbout.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAbout.Image = global::vAzhureRacingHub.Properties.Resources.info32px;
            this.btnAbout.Location = new System.Drawing.Point(12, 6);
            this.btnAbout.Name = "btnAbout";
            this.btnAbout.Size = new System.Drawing.Size(40, 40);
            this.btnAbout.TabIndex = 0;
            this.btnAbout.TabStop = false;
            this.toolTips.SetToolTip(this.btnAbout, "About...");
            this.btnAbout.UseVisualStyleBackColor = true;
            this.btnAbout.Click += new System.EventHandler(this.BtnAbout_Click);
            // 
            // btnSettings
            // 
            this.btnSettings.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSettings.FlatAppearance.BorderSize = 0;
            this.btnSettings.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSettings.Image = global::vAzhureRacingHub.Properties.Resources.list32;
            this.btnSettings.Location = new System.Drawing.Point(58, 6);
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Size = new System.Drawing.Size(40, 40);
            this.btnSettings.TabIndex = 0;
            this.btnSettings.TabStop = false;
            this.toolTips.SetToolTip(this.btnSettings, "Settings...");
            this.btnSettings.UseVisualStyleBackColor = true;
            this.btnSettings.Click += new System.EventHandler(this.BtnSettings_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblStatus.Location = new System.Drawing.Point(6, 415);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(636, 29);
            this.lblStatus.TabIndex = 8;
            this.lblStatus.Text = "Waiting...";
            // 
            // titlesGames
            // 
            this.titlesGames.ActiveTitle = null;
            this.titlesGames.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.titlesGames.BackColor = System.Drawing.Color.Transparent;
            this.titlesGames.HoverTitle = null;
            this.titlesGames.Location = new System.Drawing.Point(12, 52);
            this.titlesGames.Name = "titlesGames";
            this.titlesGames.Padding = new System.Windows.Forms.Padding(10);
            this.titlesGames.ShowTitle = true;
            this.titlesGames.Size = new System.Drawing.Size(758, 222);
            this.titlesGames.TabIndex = 9;
            this.titlesGames.Title = "Games and applications";
            this.titlesGames.TitleHeight = 64;
            this.titlesGames.TitleSpace = 8;
            this.titlesGames.TitleWidth = 64;
            // 
            // titlesDevices
            // 
            this.titlesDevices.ActiveTitle = null;
            this.titlesDevices.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.titlesDevices.BackColor = System.Drawing.Color.Transparent;
            this.titlesDevices.HoverTitle = null;
            this.titlesDevices.Location = new System.Drawing.Point(12, 280);
            this.titlesDevices.Name = "titlesDevices";
            this.titlesDevices.Padding = new System.Windows.Forms.Padding(10);
            this.titlesDevices.ShowTitle = true;
            this.titlesDevices.Size = new System.Drawing.Size(758, 126);
            this.titlesDevices.TabIndex = 9;
            this.titlesDevices.Title = "Devices and Plugins";
            this.titlesDevices.TitleHeight = 64;
            this.titlesDevices.TitleSpace = 8;
            this.titlesDevices.TitleWidth = 64;
            // 
            // AppWindow
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(46)))), ((int)(((byte)(46)))));
            this.ClientSize = new System.Drawing.Size(786, 446);
            this.ControlBox = false;
            this.Controls.Add(this.titlesDevices);
            this.Controls.Add(this.titlesGames);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.volumeControl);
            this.Controls.Add(this.btnSound);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnExpand);
            this.Controls.Add(this.btnSettings);
            this.Controls.Add(this.btnAbout);
            this.Controls.Add(this.btnMinimize);
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.Color.White;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(800, 460);
            this.Name = "AppWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnMinimize;
        private System.Windows.Forms.Button btnExpand;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.ToolTip toolTips;
        private vAzhureRacingAPI.VAzhureSwitchButton btnSound;
        private vAzhureRacingAPI.VAzhureVolumeControl volumeControl;
        private System.Windows.Forms.Label lblStatus;
        private vAzhureRacingAPI.VAzhureTilesControl titlesGames;
        private vAzhureRacingAPI.VAzhureTilesControl titlesDevices;
        private System.Windows.Forms.Button btnAbout;
        private System.Windows.Forms.Button btnSettings;
    }
}