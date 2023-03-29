
namespace Nextion35Dash
{
    partial class LedsEditor
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
            this.lblVehicle = new System.Windows.Forms.Label();
            this.lblGame = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.ledControl3 = new vAzhureRacingAPI.LedControl();
            this.ledControl2 = new vAzhureRacingAPI.LedControl();
            this.ledControl6 = new vAzhureRacingAPI.LedControl();
            this.ledControl1 = new vAzhureRacingAPI.LedControl();
            this.ledControl4 = new vAzhureRacingAPI.LedControl();
            this.ledControl5 = new vAzhureRacingAPI.LedControl();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.ledControl7 = new vAzhureRacingAPI.LedControl();
            this.ledControl8 = new vAzhureRacingAPI.LedControl();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.btnLoadProfile = new System.Windows.Forms.Button();
            this.btnApply = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.flowLayoutPanel = new Nextion35Dash.FlowLayoutPanel2();
            this.ledGroup1 = new Nextion35Dash.LedGroup();
            this.flowLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblVehicle
            // 
            this.lblVehicle.AutoSize = true;
            this.lblVehicle.Location = new System.Drawing.Point(146, 29);
            this.lblVehicle.Name = "lblVehicle";
            this.lblVehicle.Size = new System.Drawing.Size(110, 17);
            this.lblVehicle.TabIndex = 3;
            this.lblVehicle.Text = "Ferrari 488 GT3";
            // 
            // lblGame
            // 
            this.lblGame.AutoSize = true;
            this.lblGame.Location = new System.Drawing.Point(146, 9);
            this.lblGame.Name = "lblGame";
            this.lblGame.Size = new System.Drawing.Size(65, 17);
            this.lblGame.TabIndex = 1;
            this.lblGame.Text = "rFactor 2";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 29);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(98, 17);
            this.label3.TabIndex = 2;
            this.label3.Text = "Имя профиля";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(39, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "Игра";
            // 
            // ledControl3
            // 
            this.ledControl3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ledControl3.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ledControl3.LedColor = System.Drawing.Color.Orange;
            this.ledControl3.Location = new System.Drawing.Point(747, 66);
            this.ledControl3.Name = "ledControl3";
            this.ledControl3.Size = new System.Drawing.Size(32, 32);
            this.ledControl3.TabIndex = 10;
            this.ledControl3.TabStop = false;
            this.toolTip1.SetToolTip(this.ledControl3, "Оранжевый");
            // 
            // ledControl2
            // 
            this.ledControl2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ledControl2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ledControl2.LedColor = System.Drawing.Color.Yellow;
            this.ledControl2.Location = new System.Drawing.Point(709, 66);
            this.ledControl2.Name = "ledControl2";
            this.ledControl2.Size = new System.Drawing.Size(32, 32);
            this.ledControl2.TabIndex = 9;
            this.ledControl2.TabStop = false;
            this.toolTip1.SetToolTip(this.ledControl2, "Жёлтый");
            // 
            // ledControl6
            // 
            this.ledControl6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ledControl6.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ledControl6.LedColor = System.Drawing.Color.Blue;
            this.ledControl6.Location = new System.Drawing.Point(671, 104);
            this.ledControl6.Name = "ledControl6";
            this.ledControl6.Size = new System.Drawing.Size(32, 32);
            this.ledControl6.TabIndex = 12;
            this.ledControl6.TabStop = false;
            this.toolTip1.SetToolTip(this.ledControl6, "Синий");
            // 
            // ledControl1
            // 
            this.ledControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ledControl1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ledControl1.LedColor = System.Drawing.Color.Lime;
            this.ledControl1.Location = new System.Drawing.Point(671, 66);
            this.ledControl1.Name = "ledControl1";
            this.ledControl1.Size = new System.Drawing.Size(32, 32);
            this.ledControl1.TabIndex = 8;
            this.ledControl1.TabStop = false;
            this.toolTip1.SetToolTip(this.ledControl1, "Зелёный");
            // 
            // ledControl4
            // 
            this.ledControl4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ledControl4.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ledControl4.LedColor = System.Drawing.Color.Red;
            this.ledControl4.Location = new System.Drawing.Point(633, 104);
            this.ledControl4.Name = "ledControl4";
            this.ledControl4.Size = new System.Drawing.Size(32, 32);
            this.ledControl4.TabIndex = 11;
            this.ledControl4.TabStop = false;
            this.toolTip1.SetToolTip(this.ledControl4, "Красный");
            // 
            // ledControl5
            // 
            this.ledControl5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ledControl5.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ledControl5.LedColor = System.Drawing.Color.Black;
            this.ledControl5.Location = new System.Drawing.Point(633, 66);
            this.ledControl5.Name = "ledControl5";
            this.ledControl5.Size = new System.Drawing.Size(32, 32);
            this.ledControl5.TabIndex = 7;
            this.ledControl5.TabStop = false;
            this.toolTip1.SetToolTip(this.ledControl5, "Выключен");
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Location = new System.Drawing.Point(704, 439);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(116, 31);
            this.btnCancel.TabIndex = 17;
            this.btnCancel.Text = "Отмена";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOK.Location = new System.Drawing.Point(582, 439);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(116, 31);
            this.btnOK.TabIndex = 16;
            this.btnOK.Text = "Сохранить";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.BtnOK_Click);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(582, 29);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 17);
            this.label2.TabIndex = 6;
            this.label2.Text = "Палитра";
            // 
            // ledControl7
            // 
            this.ledControl7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ledControl7.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ledControl7.LedColor = System.Drawing.Color.White;
            this.ledControl7.Location = new System.Drawing.Point(709, 104);
            this.ledControl7.Name = "ledControl7";
            this.ledControl7.Size = new System.Drawing.Size(32, 32);
            this.ledControl7.TabIndex = 13;
            this.ledControl7.TabStop = false;
            this.toolTip1.SetToolTip(this.ledControl7, "Белый");
            // 
            // ledControl8
            // 
            this.ledControl8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ledControl8.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ledControl8.LedColor = System.Drawing.Color.Violet;
            this.ledControl8.Location = new System.Drawing.Point(747, 104);
            this.ledControl8.Name = "ledControl8";
            this.ledControl8.Size = new System.Drawing.Size(32, 32);
            this.ledControl8.TabIndex = 14;
            this.ledControl8.TabStop = false;
            this.toolTip1.SetToolTip(this.ledControl8, "Фиолетовый");
            // 
            // btnLoadProfile
            // 
            this.btnLoadProfile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLoadProfile.FlatAppearance.BorderSize = 0;
            this.btnLoadProfile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLoadProfile.Image = global::Nextion35Dash.Properties.Resources.lib24;
            this.btnLoadProfile.Location = new System.Drawing.Point(794, 12);
            this.btnLoadProfile.Name = "btnLoadProfile";
            this.btnLoadProfile.Size = new System.Drawing.Size(26, 26);
            this.btnLoadProfile.TabIndex = 4;
            this.btnLoadProfile.TabStop = false;
            this.toolTip1.SetToolTip(this.btnLoadProfile, "Загрузить из готовых...");
            this.btnLoadProfile.UseVisualStyleBackColor = true;
            // 
            // btnApply
            // 
            this.btnApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnApply.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnApply.Location = new System.Drawing.Point(582, 402);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(238, 31);
            this.btnApply.TabIndex = 15;
            this.btnApply.Text = "Применить";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.BtnApply_Click);
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label4.Location = new System.Drawing.Point(582, 156);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(78, 17);
            this.label4.TabIndex = 6;
            this.label4.Text = "Подсказка";
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label5.Location = new System.Drawing.Point(582, 178);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(238, 200);
            this.label5.TabIndex = 6;
            this.label5.Text = "Переместите цвет из палитры на светодиод.\r\nДля окрашивания нескольких светодиодов" +
    " удерживайте клавишу Ctrl во время перетаскивания\r\n";
            // 
            // flowLayoutPanel
            // 
            this.flowLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel.AutoScroll = true;
            this.flowLayoutPanel.Controls.Add(this.ledGroup1);
            this.flowLayoutPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel.Location = new System.Drawing.Point(15, 66);
            this.flowLayoutPanel.Name = "flowLayoutPanel";
            this.flowLayoutPanel.Size = new System.Drawing.Size(561, 404);
            this.flowLayoutPanel.TabIndex = 18;
            // 
            // ledGroup1
            // 
            this.ledGroup1.AllowDrop = true;
            this.ledGroup1.AutoSize = true;
            this.ledGroup1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ledGroup1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(46)))), ((int)(((byte)(46)))));
            this.ledGroup1.Blink = false;
            this.flowLayoutPanel.SetFlowBreak(this.ledGroup1, true);
            this.ledGroup1.ForeColor = System.Drawing.Color.White;
            this.ledGroup1.Location = new System.Drawing.Point(3, 3);
            this.ledGroup1.Name = "ledGroup1";
            this.ledGroup1.Rev = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.ledGroup1.Size = new System.Drawing.Size(545, 30);
            this.ledGroup1.TabIndex = 0;
            // 
            // LedsEditor
            // 
            this.AllowDrop = true;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(46)))), ((int)(((byte)(46)))));
            this.ClientSize = new System.Drawing.Size(832, 482);
            this.Controls.Add(this.flowLayoutPanel);
            this.Controls.Add(this.btnLoadProfile);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.ledControl3);
            this.Controls.Add(this.ledControl2);
            this.Controls.Add(this.ledControl8);
            this.Controls.Add(this.ledControl7);
            this.Controls.Add(this.ledControl6);
            this.Controls.Add(this.ledControl1);
            this.Controls.Add(this.ledControl4);
            this.Controls.Add(this.ledControl5);
            this.Controls.Add(this.lblVehicle);
            this.Controls.Add(this.lblGame);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.ForeColor = System.Drawing.Color.White;
            this.MinimumSize = new System.Drawing.Size(850, 500);
            this.Name = "LedsEditor";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "LedsEditor";
            this.TopMost = true;
            this.flowLayoutPanel.ResumeLayout(false);
            this.flowLayoutPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblGame;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblVehicle;
        private vAzhureRacingAPI.LedControl ledControl5;
        private vAzhureRacingAPI.LedControl ledControl1;
        private vAzhureRacingAPI.LedControl ledControl2;
        private vAzhureRacingAPI.LedControl ledControl3;
        private vAzhureRacingAPI.LedControl ledControl4;
        private vAzhureRacingAPI.LedControl ledControl6;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label label2;
        private vAzhureRacingAPI.LedControl ledControl7;
        private vAzhureRacingAPI.LedControl ledControl8;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Button btnLoadProfile;
        private FlowLayoutPanel2 flowLayoutPanel;
        private LedGroup ledGroup1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
    }
}