
namespace MotionPlatform3
{
    partial class GameTuneDlg
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
            this.label14 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.labelHeave = new System.Windows.Forms.Label();
            this.labelRoll = new System.Windows.Forms.Label();
            this.labelPitch = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.numPitchMax = new System.Windows.Forms.NumericUpDown();
            this.numPitchOffset = new System.Windows.Forms.NumericUpDown();
            this.numRollMax = new System.Windows.Forms.NumericUpDown();
            this.numRollOffset = new System.Windows.Forms.NumericUpDown();
            this.numHeaveMax = new System.Windows.Forms.NumericUpDown();
            this.numHeaveOffset = new System.Windows.Forms.NumericUpDown();
            this.numSurgeMax = new System.Windows.Forms.NumericUpDown();
            this.numSurgeOffset = new System.Windows.Forms.NumericUpDown();
            this.numSwayMax = new System.Windows.Forms.NumericUpDown();
            this.numSwayOffset = new System.Windows.Forms.NumericUpDown();
            this.numYawMax = new System.Windows.Forms.NumericUpDown();
            this.numYawOffset = new System.Windows.Forms.NumericUpDown();
            this.btnTune = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.btnApply = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numPitchMax)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPitchOffset)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRollMax)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRollOffset)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numHeaveMax)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numHeaveOffset)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSurgeMax)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSurgeOffset)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSwayMax)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSwayOffset)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numYawMax)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numYawOffset)).BeginInit();
            this.SuspendLayout();
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(12, 181);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(33, 13);
            this.label14.TabIndex = 42;
            this.label14.Text = "Sway";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(12, 146);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(35, 13);
            this.label12.TabIndex = 41;
            this.label12.Text = "Surge";
            // 
            // labelHeave
            // 
            this.labelHeave.AutoSize = true;
            this.labelHeave.Location = new System.Drawing.Point(12, 111);
            this.labelHeave.Name = "labelHeave";
            this.labelHeave.Size = new System.Drawing.Size(39, 13);
            this.labelHeave.TabIndex = 40;
            this.labelHeave.Text = "Heave";
            // 
            // labelRoll
            // 
            this.labelRoll.AutoSize = true;
            this.labelRoll.Location = new System.Drawing.Point(12, 76);
            this.labelRoll.Name = "labelRoll";
            this.labelRoll.Size = new System.Drawing.Size(25, 13);
            this.labelRoll.TabIndex = 39;
            this.labelRoll.Text = "Roll";
            // 
            // labelPitch
            // 
            this.labelPitch.AutoSize = true;
            this.labelPitch.Location = new System.Drawing.Point(12, 41);
            this.labelPitch.Name = "labelPitch";
            this.labelPitch.Size = new System.Drawing.Size(31, 13);
            this.labelPitch.TabIndex = 38;
            this.labelPitch.Text = "Pitch";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 216);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(28, 13);
            this.label6.TabIndex = 43;
            this.label6.Text = "Yaw";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(80, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(27, 13);
            this.label1.TabIndex = 38;
            this.label1.Text = "Max";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(193, 14);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 38;
            this.label2.Text = "Offset";
            // 
            // numPitchMax
            // 
            this.numPitchMax.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.numPitchMax.DecimalPlaces = 2;
            this.numPitchMax.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numPitchMax.Location = new System.Drawing.Point(83, 42);
            this.numPitchMax.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numPitchMax.Name = "numPitchMax";
            this.numPitchMax.Size = new System.Drawing.Size(89, 16);
            this.numPitchMax.TabIndex = 44;
            this.numPitchMax.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // numPitchOffset
            // 
            this.numPitchOffset.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.numPitchOffset.DecimalPlaces = 2;
            this.numPitchOffset.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numPitchOffset.Location = new System.Drawing.Point(196, 42);
            this.numPitchOffset.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numPitchOffset.Minimum = new decimal(new int[] {
            10000000,
            0,
            0,
            -2147483648});
            this.numPitchOffset.Name = "numPitchOffset";
            this.numPitchOffset.Size = new System.Drawing.Size(89, 16);
            this.numPitchOffset.TabIndex = 44;
            this.numPitchOffset.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // numRollMax
            // 
            this.numRollMax.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.numRollMax.DecimalPlaces = 2;
            this.numRollMax.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numRollMax.Location = new System.Drawing.Point(83, 77);
            this.numRollMax.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numRollMax.Name = "numRollMax";
            this.numRollMax.Size = new System.Drawing.Size(89, 16);
            this.numRollMax.TabIndex = 44;
            this.numRollMax.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // numRollOffset
            // 
            this.numRollOffset.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.numRollOffset.DecimalPlaces = 2;
            this.numRollOffset.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numRollOffset.Location = new System.Drawing.Point(196, 77);
            this.numRollOffset.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numRollOffset.Minimum = new decimal(new int[] {
            10000000,
            0,
            0,
            -2147483648});
            this.numRollOffset.Name = "numRollOffset";
            this.numRollOffset.Size = new System.Drawing.Size(89, 16);
            this.numRollOffset.TabIndex = 44;
            this.numRollOffset.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // numHeaveMax
            // 
            this.numHeaveMax.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.numHeaveMax.DecimalPlaces = 2;
            this.numHeaveMax.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numHeaveMax.Location = new System.Drawing.Point(83, 112);
            this.numHeaveMax.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numHeaveMax.Name = "numHeaveMax";
            this.numHeaveMax.Size = new System.Drawing.Size(89, 16);
            this.numHeaveMax.TabIndex = 44;
            this.numHeaveMax.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // numHeaveOffset
            // 
            this.numHeaveOffset.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.numHeaveOffset.DecimalPlaces = 2;
            this.numHeaveOffset.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numHeaveOffset.Location = new System.Drawing.Point(196, 112);
            this.numHeaveOffset.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numHeaveOffset.Minimum = new decimal(new int[] {
            10000000,
            0,
            0,
            -2147483648});
            this.numHeaveOffset.Name = "numHeaveOffset";
            this.numHeaveOffset.Size = new System.Drawing.Size(89, 16);
            this.numHeaveOffset.TabIndex = 44;
            this.numHeaveOffset.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // numSurgeMax
            // 
            this.numSurgeMax.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.numSurgeMax.DecimalPlaces = 2;
            this.numSurgeMax.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numSurgeMax.Location = new System.Drawing.Point(83, 147);
            this.numSurgeMax.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numSurgeMax.Name = "numSurgeMax";
            this.numSurgeMax.Size = new System.Drawing.Size(89, 16);
            this.numSurgeMax.TabIndex = 44;
            this.numSurgeMax.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // numSurgeOffset
            // 
            this.numSurgeOffset.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.numSurgeOffset.DecimalPlaces = 2;
            this.numSurgeOffset.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numSurgeOffset.Location = new System.Drawing.Point(196, 147);
            this.numSurgeOffset.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numSurgeOffset.Minimum = new decimal(new int[] {
            10000000,
            0,
            0,
            -2147483648});
            this.numSurgeOffset.Name = "numSurgeOffset";
            this.numSurgeOffset.Size = new System.Drawing.Size(89, 16);
            this.numSurgeOffset.TabIndex = 44;
            this.numSurgeOffset.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // numSwayMax
            // 
            this.numSwayMax.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.numSwayMax.DecimalPlaces = 2;
            this.numSwayMax.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numSwayMax.Location = new System.Drawing.Point(83, 182);
            this.numSwayMax.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numSwayMax.Name = "numSwayMax";
            this.numSwayMax.Size = new System.Drawing.Size(89, 16);
            this.numSwayMax.TabIndex = 44;
            this.numSwayMax.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // numSwayOffset
            // 
            this.numSwayOffset.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.numSwayOffset.DecimalPlaces = 2;
            this.numSwayOffset.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numSwayOffset.Location = new System.Drawing.Point(196, 182);
            this.numSwayOffset.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numSwayOffset.Minimum = new decimal(new int[] {
            10000000,
            0,
            0,
            -2147483648});
            this.numSwayOffset.Name = "numSwayOffset";
            this.numSwayOffset.Size = new System.Drawing.Size(89, 16);
            this.numSwayOffset.TabIndex = 44;
            this.numSwayOffset.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // numYawMax
            // 
            this.numYawMax.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.numYawMax.DecimalPlaces = 2;
            this.numYawMax.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numYawMax.Location = new System.Drawing.Point(83, 217);
            this.numYawMax.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numYawMax.Name = "numYawMax";
            this.numYawMax.Size = new System.Drawing.Size(89, 16);
            this.numYawMax.TabIndex = 44;
            this.numYawMax.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // numYawOffset
            // 
            this.numYawOffset.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.numYawOffset.DecimalPlaces = 2;
            this.numYawOffset.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numYawOffset.Location = new System.Drawing.Point(196, 217);
            this.numYawOffset.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.numYawOffset.Minimum = new decimal(new int[] {
            10000000,
            0,
            0,
            -2147483648});
            this.numYawOffset.Name = "numYawOffset";
            this.numYawOffset.Size = new System.Drawing.Size(89, 16);
            this.numYawOffset.TabIndex = 44;
            this.numYawOffset.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // btnTune
            // 
            this.btnTune.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTune.Location = new System.Drawing.Point(316, 34);
            this.btnTune.Name = "btnTune";
            this.btnTune.Size = new System.Drawing.Size(92, 27);
            this.btnTune.TabIndex = 46;
            this.btnTune.Tag = "Pitch";
            this.btnTune.Text = "Defaults";
            this.btnTune.UseVisualStyleBackColor = true;
            this.btnTune.Click += new System.EventHandler(this.Button6_Click);
            // 
            // button1
            // 
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Location = new System.Drawing.Point(316, 69);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(92, 27);
            this.button1.TabIndex = 46;
            this.button1.Tag = "Roll";
            this.button1.Text = "Defaults";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.Button6_Click);
            // 
            // button2
            // 
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.Location = new System.Drawing.Point(316, 104);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(92, 27);
            this.button2.TabIndex = 46;
            this.button2.Tag = "Heave";
            this.button2.Text = "Defaults";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.Button6_Click);
            // 
            // button3
            // 
            this.button3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button3.Location = new System.Drawing.Point(316, 139);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(92, 27);
            this.button3.TabIndex = 46;
            this.button3.Tag = "Surge";
            this.button3.Text = "Defaults";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.Button6_Click);
            // 
            // button4
            // 
            this.button4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button4.Location = new System.Drawing.Point(316, 174);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(92, 27);
            this.button4.TabIndex = 46;
            this.button4.Tag = "Sway";
            this.button4.Text = "Defaults";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.Button6_Click);
            // 
            // button5
            // 
            this.button5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button5.Location = new System.Drawing.Point(316, 209);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(92, 27);
            this.button5.TabIndex = 46;
            this.button5.Tag = "Yaw";
            this.button5.Text = "Defaults";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.Button6_Click);
            // 
            // button6
            // 
            this.button6.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button6.Location = new System.Drawing.Point(316, 242);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(92, 27);
            this.button6.TabIndex = 46;
            this.button6.Tag = "All";
            this.button6.Text = "Defaults All";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.Button6_Click);
            // 
            // btnApply
            // 
            this.btnApply.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnApply.Location = new System.Drawing.Point(218, 288);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(92, 27);
            this.btnApply.TabIndex = 46;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.BtnApply_Click);
            // 
            // btnClose
            // 
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Location = new System.Drawing.Point(316, 288);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(92, 27);
            this.btnClose.TabIndex = 46;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.BtnClose_Click);
            // 
            // GameTuneDlg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(46)))), ((int)(((byte)(46)))));
            this.ClientSize = new System.Drawing.Size(432, 329);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnTune);
            this.Controls.Add(this.numYawOffset);
            this.Controls.Add(this.numSwayOffset);
            this.Controls.Add(this.numSurgeOffset);
            this.Controls.Add(this.numHeaveOffset);
            this.Controls.Add(this.numRollOffset);
            this.Controls.Add(this.numPitchOffset);
            this.Controls.Add(this.numYawMax);
            this.Controls.Add(this.numSwayMax);
            this.Controls.Add(this.numSurgeMax);
            this.Controls.Add(this.numHeaveMax);
            this.Controls.Add(this.numRollMax);
            this.Controls.Add(this.numPitchMax);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.labelHeave);
            this.Controls.Add(this.labelRoll);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.labelPitch);
            this.Controls.Add(this.label6);
            this.ForeColor = System.Drawing.Color.White;
            this.Name = "GameTuneDlg";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "GameTuneDlg";
            ((System.ComponentModel.ISupportInitialize)(this.numPitchMax)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPitchOffset)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRollMax)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRollOffset)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numHeaveMax)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numHeaveOffset)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSurgeMax)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSurgeOffset)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSwayMax)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSwayOffset)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numYawMax)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numYawOffset)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label labelHeave;
        private System.Windows.Forms.Label labelRoll;
        private System.Windows.Forms.Label labelPitch;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numPitchMax;
        private System.Windows.Forms.NumericUpDown numPitchOffset;
        private System.Windows.Forms.NumericUpDown numRollMax;
        private System.Windows.Forms.NumericUpDown numRollOffset;
        private System.Windows.Forms.NumericUpDown numHeaveMax;
        private System.Windows.Forms.NumericUpDown numHeaveOffset;
        private System.Windows.Forms.NumericUpDown numSurgeMax;
        private System.Windows.Forms.NumericUpDown numSurgeOffset;
        private System.Windows.Forms.NumericUpDown numSwayMax;
        private System.Windows.Forms.NumericUpDown numSwayOffset;
        private System.Windows.Forms.NumericUpDown numYawMax;
        private System.Windows.Forms.NumericUpDown numYawOffset;
        private System.Windows.Forms.Button btnTune;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Button btnClose;
    }
}