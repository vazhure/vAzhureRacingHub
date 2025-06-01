
namespace MotionPlatform3
{
    partial class ManualControlForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ManualControlForm));
            this.lblHeave = new System.Windows.Forms.Label();
            this.lblRoll = new System.Windows.Forms.Label();
            this.lblPitch = new System.Windows.Forms.Label();
            this.labelHeave = new System.Windows.Forms.Label();
            this.labelRoll = new System.Windows.Forms.Label();
            this.labelPitch = new System.Windows.Forms.Label();
            this.sliderHeave = new vAzhureRacingAPI.VAzhureSliderControl();
            this.sliderRoll = new vAzhureRacingAPI.VAzhureSliderControl();
            this.sliderPitch = new vAzhureRacingAPI.VAzhureSliderControl();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnResetPitch = new System.Windows.Forms.Button();
            this.btnResetRoll = new System.Windows.Forms.Button();
            this.btnResetHeave = new System.Windows.Forms.Button();
            this.chkReducedSpeed = new vAzhureRacingAPI.VAzhureSwitchButton();
            this.SuspendLayout();
            // 
            // lblHeave
            // 
            this.lblHeave.AutoSize = true;
            this.lblHeave.Location = new System.Drawing.Point(511, 104);
            this.lblHeave.Name = "lblHeave";
            this.lblHeave.Size = new System.Drawing.Size(32, 13);
            this.lblHeave.TabIndex = 34;
            this.lblHeave.Text = "0 mm";
            // 
            // lblRoll
            // 
            this.lblRoll.AutoSize = true;
            this.lblRoll.Location = new System.Drawing.Point(511, 69);
            this.lblRoll.Name = "lblRoll";
            this.lblRoll.Size = new System.Drawing.Size(17, 13);
            this.lblRoll.TabIndex = 31;
            this.lblRoll.Text = "0°";
            // 
            // lblPitch
            // 
            this.lblPitch.AutoSize = true;
            this.lblPitch.Location = new System.Drawing.Point(511, 34);
            this.lblPitch.Name = "lblPitch";
            this.lblPitch.Size = new System.Drawing.Size(17, 13);
            this.lblPitch.TabIndex = 28;
            this.lblPitch.Text = "0°";
            // 
            // labelHeave
            // 
            this.labelHeave.AutoSize = true;
            this.labelHeave.Location = new System.Drawing.Point(15, 104);
            this.labelHeave.Name = "labelHeave";
            this.labelHeave.Size = new System.Drawing.Size(39, 13);
            this.labelHeave.TabIndex = 32;
            this.labelHeave.Text = "Heave";
            // 
            // labelRoll
            // 
            this.labelRoll.AutoSize = true;
            this.labelRoll.Location = new System.Drawing.Point(15, 69);
            this.labelRoll.Name = "labelRoll";
            this.labelRoll.Size = new System.Drawing.Size(25, 13);
            this.labelRoll.TabIndex = 29;
            this.labelRoll.Text = "Roll";
            // 
            // labelPitch
            // 
            this.labelPitch.AutoSize = true;
            this.labelPitch.Location = new System.Drawing.Point(15, 34);
            this.labelPitch.Name = "labelPitch";
            this.labelPitch.Size = new System.Drawing.Size(31, 13);
            this.labelPitch.TabIndex = 26;
            this.labelPitch.Text = "Pitch";
            // 
            // sliderHeave
            // 
            this.sliderHeave.BigStep = 10;
            this.sliderHeave.Location = new System.Drawing.Point(86, 96);
            this.sliderHeave.Maximum = 10;
            this.sliderHeave.Minimum = -10;
            this.sliderHeave.MinimumSize = new System.Drawing.Size(16, 16);
            this.sliderHeave.Name = "sliderHeave";
            this.sliderHeave.Padding = new System.Windows.Forms.Padding(1);
            this.sliderHeave.Size = new System.Drawing.Size(419, 29);
            this.sliderHeave.SliderColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(153)))), ((int)(((byte)(252)))));
            this.sliderHeave.SmallStep = 5;
            this.sliderHeave.Steps = 10;
            this.sliderHeave.TabIndex = 33;
            this.sliderHeave.Tag = "Heave";
            this.sliderHeave.Value = 0;
            this.sliderHeave.Vertical = false;
            this.sliderHeave.OnValueChanged += new System.EventHandler(this.OnSlider);
            // 
            // sliderRoll
            // 
            this.sliderRoll.BigStep = 10;
            this.sliderRoll.Location = new System.Drawing.Point(86, 61);
            this.sliderRoll.Maximum = 10;
            this.sliderRoll.Minimum = -10;
            this.sliderRoll.MinimumSize = new System.Drawing.Size(16, 16);
            this.sliderRoll.Name = "sliderRoll";
            this.sliderRoll.Padding = new System.Windows.Forms.Padding(1);
            this.sliderRoll.Size = new System.Drawing.Size(419, 29);
            this.sliderRoll.SliderColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(153)))), ((int)(((byte)(252)))));
            this.sliderRoll.SmallStep = 5;
            this.sliderRoll.Steps = 10;
            this.sliderRoll.TabIndex = 30;
            this.sliderRoll.Tag = "Roll";
            this.sliderRoll.Value = 0;
            this.sliderRoll.Vertical = false;
            this.sliderRoll.OnValueChanged += new System.EventHandler(this.OnSlider);
            // 
            // sliderPitch
            // 
            this.sliderPitch.BigStep = 10;
            this.sliderPitch.Location = new System.Drawing.Point(86, 26);
            this.sliderPitch.Maximum = 10;
            this.sliderPitch.Minimum = -10;
            this.sliderPitch.MinimumSize = new System.Drawing.Size(16, 16);
            this.sliderPitch.Name = "sliderPitch";
            this.sliderPitch.Padding = new System.Windows.Forms.Padding(1);
            this.sliderPitch.Size = new System.Drawing.Size(419, 29);
            this.sliderPitch.SliderColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(153)))), ((int)(((byte)(252)))));
            this.sliderPitch.SmallStep = 5;
            this.sliderPitch.Steps = 10;
            this.sliderPitch.TabIndex = 27;
            this.sliderPitch.Tag = "Pitch";
            this.sliderPitch.Value = 0;
            this.sliderPitch.Vertical = false;
            this.sliderPitch.OnValueChanged += new System.EventHandler(this.OnSlider);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Location = new System.Drawing.Point(465, 150);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(147, 44);
            this.btnClose.TabIndex = 35;
            this.btnClose.Text = "CLOSE";
            this.btnClose.UseVisualStyleBackColor = true;
            // 
            // btnResetPitch
            // 
            this.btnResetPitch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnResetPitch.FlatAppearance.BorderSize = 0;
            this.btnResetPitch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnResetPitch.Location = new System.Drawing.Point(549, 27);
            this.btnResetPitch.Name = "btnResetPitch";
            this.btnResetPitch.Size = new System.Drawing.Size(63, 27);
            this.btnResetPitch.TabIndex = 36;
            this.btnResetPitch.Tag = "Pitch";
            this.btnResetPitch.Text = "Reset";
            this.btnResetPitch.UseVisualStyleBackColor = true;
            this.btnResetPitch.Click += new System.EventHandler(this.OnReset);
            // 
            // btnResetRoll
            // 
            this.btnResetRoll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnResetRoll.FlatAppearance.BorderSize = 0;
            this.btnResetRoll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnResetRoll.Location = new System.Drawing.Point(549, 62);
            this.btnResetRoll.Name = "btnResetRoll";
            this.btnResetRoll.Size = new System.Drawing.Size(63, 27);
            this.btnResetRoll.TabIndex = 36;
            this.btnResetRoll.Tag = "Roll";
            this.btnResetRoll.Text = "Reset";
            this.btnResetRoll.UseVisualStyleBackColor = true;
            this.btnResetRoll.Click += new System.EventHandler(this.OnReset);
            // 
            // btnResetHeave
            // 
            this.btnResetHeave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnResetHeave.FlatAppearance.BorderSize = 0;
            this.btnResetHeave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnResetHeave.Location = new System.Drawing.Point(549, 98);
            this.btnResetHeave.Name = "btnResetHeave";
            this.btnResetHeave.Size = new System.Drawing.Size(63, 27);
            this.btnResetHeave.TabIndex = 36;
            this.btnResetHeave.Tag = "Heave";
            this.btnResetHeave.Text = "Reset";
            this.btnResetHeave.UseVisualStyleBackColor = true;
            this.btnResetHeave.Click += new System.EventHandler(this.OnReset);
            // 
            // chkReducedSpeed
            // 
            this.chkReducedSpeed.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(46)))), ((int)(((byte)(46)))));
            this.chkReducedSpeed.Checked = false;
            this.chkReducedSpeed.Location = new System.Drawing.Point(18, 131);
            this.chkReducedSpeed.MaximumSize = new System.Drawing.Size(111, 32);
            this.chkReducedSpeed.MinimumSize = new System.Drawing.Size(111, 32);
            this.chkReducedSpeed.Name = "chkReducedSpeed";
            this.chkReducedSpeed.Size = new System.Drawing.Size(111, 32);
            this.chkReducedSpeed.StateOff = ((System.Drawing.Image)(resources.GetObject("chkReducedSpeed.StateOff")));
            this.chkReducedSpeed.StateOn = ((System.Drawing.Image)(resources.GetObject("chkReducedSpeed.StateOn")));
            this.chkReducedSpeed.SwitchText = "Reduce speed";
            this.chkReducedSpeed.TabIndex = 37;
            this.chkReducedSpeed.OnSwitch += new System.EventHandler(this.ChkReducedSpeed_OnSwitch);
            // 
            // ManualControlForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(46)))), ((int)(((byte)(46)))));
            this.ClientSize = new System.Drawing.Size(624, 206);
            this.Controls.Add(this.chkReducedSpeed);
            this.Controls.Add(this.btnResetHeave);
            this.Controls.Add(this.btnResetRoll);
            this.Controls.Add(this.btnResetPitch);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.lblHeave);
            this.Controls.Add(this.lblRoll);
            this.Controls.Add(this.lblPitch);
            this.Controls.Add(this.labelHeave);
            this.Controls.Add(this.labelRoll);
            this.Controls.Add(this.labelPitch);
            this.Controls.Add(this.sliderHeave);
            this.Controls.Add(this.sliderRoll);
            this.Controls.Add(this.sliderPitch);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(640, 245);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(640, 245);
            this.Name = "ManualControlForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Manual control";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblHeave;
        private System.Windows.Forms.Label lblRoll;
        private System.Windows.Forms.Label lblPitch;
        private System.Windows.Forms.Label labelHeave;
        private System.Windows.Forms.Label labelRoll;
        private System.Windows.Forms.Label labelPitch;
        private vAzhureRacingAPI.VAzhureSliderControl sliderHeave;
        private vAzhureRacingAPI.VAzhureSliderControl sliderRoll;
        private vAzhureRacingAPI.VAzhureSliderControl sliderPitch;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnResetPitch;
        private System.Windows.Forms.Button btnResetRoll;
        private System.Windows.Forms.Button btnResetHeave;
        private vAzhureRacingAPI.VAzhureSwitchButton chkReducedSpeed;
    }
}