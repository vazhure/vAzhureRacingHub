
namespace Nextion35Dash
{
    partial class LedGroup
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.chkBlink = new System.Windows.Forms.CheckBox();
            this.numRev = new System.Windows.Forms.NumericUpDown();
            this.ledControl8 = new vAzhureRacingAPI.LedControl();
            this.ledControl7 = new vAzhureRacingAPI.LedControl();
            this.ledControl6 = new vAzhureRacingAPI.LedControl();
            this.ledControl5 = new vAzhureRacingAPI.LedControl();
            this.ledControl4 = new vAzhureRacingAPI.LedControl();
            this.ledControl3 = new vAzhureRacingAPI.LedControl();
            this.ledControl2 = new vAzhureRacingAPI.LedControl();
            this.ledControl1 = new vAzhureRacingAPI.LedControl();
            this.buttonAdd = new System.Windows.Forms.Button();
            this.buttonMinus = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.numRev)).BeginInit();
            this.SuspendLayout();
            // 
            // chkBlink
            // 
            this.chkBlink.AutoSize = true;
            this.chkBlink.Location = new System.Drawing.Point(397, 4);
            this.chkBlink.Name = "chkBlink";
            this.chkBlink.Size = new System.Drawing.Size(86, 21);
            this.chkBlink.TabIndex = 14;
            this.chkBlink.Text = "Мигание";
            this.toolTip1.SetToolTip(this.chkBlink, "Мигание");
            this.chkBlink.UseVisualStyleBackColor = true;
            // 
            // numRev
            // 
            this.numRev.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(46)))), ((int)(((byte)(46)))));
            this.numRev.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numRev.ForeColor = System.Drawing.Color.White;
            this.numRev.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numRev.Location = new System.Drawing.Point(21, 3);
            this.numRev.Maximum = new decimal(new int[] {
            20000,
            0,
            0,
            0});
            this.numRev.Name = "numRev";
            this.numRev.Size = new System.Drawing.Size(107, 22);
            this.numRev.TabIndex = 5;
            this.numRev.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.toolTip1.SetToolTip(this.numRev, "Обороты (Об./мин.)");
            // 
            // ledControl8
            // 
            this.ledControl8.AllowDrop = true;
            this.ledControl8.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ledControl8.LedColor = System.Drawing.Color.Black;
            this.ledControl8.Location = new System.Drawing.Point(355, 3);
            this.ledControl8.Name = "ledControl8";
            this.ledControl8.Size = new System.Drawing.Size(24, 24);
            this.ledControl8.TabIndex = 6;
            this.toolTip1.SetToolTip(this.ledControl8, "Светодиод 8");
            // 
            // ledControl7
            // 
            this.ledControl7.AllowDrop = true;
            this.ledControl7.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ledControl7.LedColor = System.Drawing.Color.Black;
            this.ledControl7.Location = new System.Drawing.Point(325, 3);
            this.ledControl7.Name = "ledControl7";
            this.ledControl7.Size = new System.Drawing.Size(24, 24);
            this.ledControl7.TabIndex = 7;
            this.toolTip1.SetToolTip(this.ledControl7, "Светодиод 7");
            // 
            // ledControl6
            // 
            this.ledControl6.AllowDrop = true;
            this.ledControl6.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ledControl6.LedColor = System.Drawing.Color.Black;
            this.ledControl6.Location = new System.Drawing.Point(295, 3);
            this.ledControl6.Name = "ledControl6";
            this.ledControl6.Size = new System.Drawing.Size(24, 24);
            this.ledControl6.TabIndex = 8;
            this.toolTip1.SetToolTip(this.ledControl6, "Светодиод 6");
            // 
            // ledControl5
            // 
            this.ledControl5.AllowDrop = true;
            this.ledControl5.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ledControl5.LedColor = System.Drawing.Color.Black;
            this.ledControl5.Location = new System.Drawing.Point(265, 3);
            this.ledControl5.Name = "ledControl5";
            this.ledControl5.Size = new System.Drawing.Size(24, 24);
            this.ledControl5.TabIndex = 9;
            this.toolTip1.SetToolTip(this.ledControl5, "Светодиод 5");
            // 
            // ledControl4
            // 
            this.ledControl4.AllowDrop = true;
            this.ledControl4.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ledControl4.LedColor = System.Drawing.Color.Black;
            this.ledControl4.Location = new System.Drawing.Point(235, 3);
            this.ledControl4.Name = "ledControl4";
            this.ledControl4.Size = new System.Drawing.Size(24, 24);
            this.ledControl4.TabIndex = 10;
            this.toolTip1.SetToolTip(this.ledControl4, "Светодиод 4");
            // 
            // ledControl3
            // 
            this.ledControl3.AllowDrop = true;
            this.ledControl3.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ledControl3.LedColor = System.Drawing.Color.Black;
            this.ledControl3.Location = new System.Drawing.Point(205, 3);
            this.ledControl3.Name = "ledControl3";
            this.ledControl3.Size = new System.Drawing.Size(24, 24);
            this.ledControl3.TabIndex = 11;
            this.toolTip1.SetToolTip(this.ledControl3, "Светодиод 3");
            // 
            // ledControl2
            // 
            this.ledControl2.AllowDrop = true;
            this.ledControl2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ledControl2.LedColor = System.Drawing.Color.Black;
            this.ledControl2.Location = new System.Drawing.Point(175, 3);
            this.ledControl2.Name = "ledControl2";
            this.ledControl2.Size = new System.Drawing.Size(24, 24);
            this.ledControl2.TabIndex = 12;
            this.toolTip1.SetToolTip(this.ledControl2, "Светодиод 2");
            // 
            // ledControl1
            // 
            this.ledControl1.AllowDrop = true;
            this.ledControl1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ledControl1.LedColor = System.Drawing.Color.Black;
            this.ledControl1.Location = new System.Drawing.Point(145, 3);
            this.ledControl1.Name = "ledControl1";
            this.ledControl1.Size = new System.Drawing.Size(24, 24);
            this.ledControl1.TabIndex = 13;
            this.toolTip1.SetToolTip(this.ledControl1, "Светодиод 1");
            // 
            // buttonAdd
            // 
            this.buttonAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonAdd.Location = new System.Drawing.Point(487, 4);
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.Size = new System.Drawing.Size(26, 23);
            this.buttonAdd.TabIndex = 15;
            this.buttonAdd.Text = "+";
            this.toolTip1.SetToolTip(this.buttonAdd, "Добавить");
            this.buttonAdd.UseVisualStyleBackColor = true;
            this.buttonAdd.Click += new System.EventHandler(this.OnPlus);
            // 
            // buttonMinus
            // 
            this.buttonMinus.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonMinus.Location = new System.Drawing.Point(516, 4);
            this.buttonMinus.Name = "buttonMinus";
            this.buttonMinus.Size = new System.Drawing.Size(26, 23);
            this.buttonMinus.TabIndex = 15;
            this.buttonMinus.Text = "-";
            this.toolTip1.SetToolTip(this.buttonMinus, "Удалить");
            this.buttonMinus.UseVisualStyleBackColor = true;
            this.buttonMinus.Click += new System.EventHandler(this.OnMinus);
            // 
            // LedGroup
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(46)))), ((int)(((byte)(46)))));
            this.Controls.Add(this.buttonMinus);
            this.Controls.Add(this.buttonAdd);
            this.Controls.Add(this.chkBlink);
            this.Controls.Add(this.ledControl8);
            this.Controls.Add(this.ledControl7);
            this.Controls.Add(this.ledControl6);
            this.Controls.Add(this.ledControl5);
            this.Controls.Add(this.ledControl4);
            this.Controls.Add(this.ledControl3);
            this.Controls.Add(this.ledControl2);
            this.Controls.Add(this.ledControl1);
            this.Controls.Add(this.numRev);
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.Color.White;
            this.Name = "LedGroup";
            this.Size = new System.Drawing.Size(545, 30);
            ((System.ComponentModel.ISupportInitialize)(this.numRev)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkBlink;
        private vAzhureRacingAPI.LedControl ledControl8;
        private vAzhureRacingAPI.LedControl ledControl7;
        private vAzhureRacingAPI.LedControl ledControl6;
        private vAzhureRacingAPI.LedControl ledControl5;
        private vAzhureRacingAPI.LedControl ledControl4;
        private vAzhureRacingAPI.LedControl ledControl3;
        private vAzhureRacingAPI.LedControl ledControl2;
        private vAzhureRacingAPI.LedControl ledControl1;
        private System.Windows.Forms.NumericUpDown numRev;
        private System.Windows.Forms.Button buttonAdd;
        private System.Windows.Forms.Button buttonMinus;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}
