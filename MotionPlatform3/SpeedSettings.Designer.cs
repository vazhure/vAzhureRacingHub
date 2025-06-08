
namespace MotionPlatform3
{
    partial class SpeedSettings
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
            this.btnApply = new System.Windows.Forms.Button();
            this.numAcceleration = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.numSlowSpeed = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.numMinSpeed = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.numMaxSpeed = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.numAcceleration)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSlowSpeed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMinSpeed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxSpeed)).BeginInit();
            this.SuspendLayout();
            // 
            // btnApply
            // 
            this.btnApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnApply.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnApply.Location = new System.Drawing.Point(205, 121);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(147, 44);
            this.btnApply.TabIndex = 36;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.BtnApply_Click);
            // 
            // numAcceleration
            // 
            this.numAcceleration.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numAcceleration.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numAcceleration.Location = new System.Drawing.Point(205, 7);
            this.numAcceleration.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numAcceleration.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numAcceleration.Name = "numAcceleration";
            this.numAcceleration.Size = new System.Drawing.Size(89, 20);
            this.numAcceleration.TabIndex = 47;
            this.numAcceleration.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numAcceleration.Value = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.numAcceleration.ValueChanged += new System.EventHandler(this.OnValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 13);
            this.label1.TabIndex = 46;
            this.label1.Text = "Acceleration";
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Location = new System.Drawing.Point(358, 121);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(147, 44);
            this.btnClose.TabIndex = 48;
            this.btnClose.Text = "CLOSE";
            this.btnClose.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 35);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(106, 13);
            this.label2.TabIndex = 46;
            this.label2.Text = "Slow speed, mm/sec";
            // 
            // numSlowSpeed
            // 
            this.numSlowSpeed.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numSlowSpeed.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numSlowSpeed.Location = new System.Drawing.Point(205, 33);
            this.numSlowSpeed.Maximum = new decimal(new int[] {
            300,
            0,
            0,
            0});
            this.numSlowSpeed.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numSlowSpeed.Name = "numSlowSpeed";
            this.numSlowSpeed.Size = new System.Drawing.Size(89, 20);
            this.numSlowSpeed.TabIndex = 47;
            this.numSlowSpeed.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numSlowSpeed.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.numSlowSpeed.ValueChanged += new System.EventHandler(this.OnValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 61);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(100, 13);
            this.label3.TabIndex = 46;
            this.label3.Text = "Min speed, mm/sec";
            // 
            // numMinSpeed
            // 
            this.numMinSpeed.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numMinSpeed.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numMinSpeed.Location = new System.Drawing.Point(205, 59);
            this.numMinSpeed.Maximum = new decimal(new int[] {
            300,
            0,
            0,
            0});
            this.numMinSpeed.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numMinSpeed.Name = "numMinSpeed";
            this.numMinSpeed.Size = new System.Drawing.Size(89, 20);
            this.numMinSpeed.TabIndex = 47;
            this.numMinSpeed.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numMinSpeed.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.numMinSpeed.ValueChanged += new System.EventHandler(this.OnValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 87);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(103, 13);
            this.label4.TabIndex = 46;
            this.label4.Text = "Max speed, mm/sec";
            // 
            // numMaxSpeed
            // 
            this.numMaxSpeed.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numMaxSpeed.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numMaxSpeed.Location = new System.Drawing.Point(205, 85);
            this.numMaxSpeed.Maximum = new decimal(new int[] {
            300,
            0,
            0,
            0});
            this.numMaxSpeed.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numMaxSpeed.Name = "numMaxSpeed";
            this.numMaxSpeed.Size = new System.Drawing.Size(89, 20);
            this.numMaxSpeed.TabIndex = 47;
            this.numMaxSpeed.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numMaxSpeed.Value = new decimal(new int[] {
            120,
            0,
            0,
            0});
            this.numMaxSpeed.ValueChanged += new System.EventHandler(this.OnValueChanged);
            // 
            // SpeedSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(46)))), ((int)(((byte)(46)))));
            this.ClientSize = new System.Drawing.Size(517, 177);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.numMaxSpeed);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.numMinSpeed);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.numSlowSpeed);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.numAcceleration);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnApply);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SpeedSettings";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Advanced Speed Settings";
            ((System.ComponentModel.ISupportInitialize)(this.numAcceleration)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSlowSpeed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMinSpeed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxSpeed)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.NumericUpDown numAcceleration;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numSlowSpeed;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numMinSpeed;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown numMaxSpeed;
    }
}