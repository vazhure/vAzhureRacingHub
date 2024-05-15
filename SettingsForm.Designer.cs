
namespace vAzhureRacingHub
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

            foreach (SettingsControl c in settingsControls)
            {
                Controls.Remove(c);
                c.Dispose();
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
            this.btnExit = new System.Windows.Forms.Button();
            this.btnPlugins = new System.Windows.Forms.Button();
            this.btnGames = new System.Windows.Forms.Button();
            this.btnServer = new System.Windows.Forms.Button();
            this.labelTitle = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnExit
            // 
            this.btnExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExit.FlatAppearance.BorderSize = 0;
            this.btnExit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExit.Image = global::vAzhureRacingHub.Properties.Resources.close_window;
            this.btnExit.Location = new System.Drawing.Point(742, 5);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(40, 40);
            this.btnExit.TabIndex = 1;
            this.btnExit.TabStop = false;
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.BtnExit_Click);
            // 
            // btnPlugins
            // 
            this.btnPlugins.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPlugins.FlatAppearance.BorderSize = 0;
            this.btnPlugins.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.btnPlugins.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.btnPlugins.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPlugins.Image = global::vAzhureRacingHub.Properties.Resources.puzzle24;
            this.btnPlugins.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnPlugins.Location = new System.Drawing.Point(12, 48);
            this.btnPlugins.Name = "btnPlugins";
            this.btnPlugins.Size = new System.Drawing.Size(198, 40);
            this.btnPlugins.TabIndex = 2;
            this.btnPlugins.TabStop = false;
            this.btnPlugins.Text = "Plugins";
            this.btnPlugins.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPlugins.UseVisualStyleBackColor = true;
            this.btnPlugins.Click += new System.EventHandler(this.OnButton);
            // 
            // btnGames
            // 
            this.btnGames.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnGames.FlatAppearance.BorderSize = 0;
            this.btnGames.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.btnGames.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.btnGames.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGames.Image = global::vAzhureRacingHub.Properties.Resources.game24px;
            this.btnGames.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnGames.Location = new System.Drawing.Point(12, 94);
            this.btnGames.Name = "btnGames";
            this.btnGames.Size = new System.Drawing.Size(198, 40);
            this.btnGames.TabIndex = 3;
            this.btnGames.TabStop = false;
            this.btnGames.Text = "Games and Apps";
            this.btnGames.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnGames.UseVisualStyleBackColor = true;
            this.btnGames.Click += new System.EventHandler(this.OnButton);
            // 
            // btnServer
            // 
            this.btnServer.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnServer.FlatAppearance.BorderSize = 0;
            this.btnServer.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.btnServer.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(56)))), ((int)(((byte)(56)))));
            this.btnServer.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnServer.Image = global::vAzhureRacingHub.Properties.Resources.web24;
            this.btnServer.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnServer.Location = new System.Drawing.Point(12, 140);
            this.btnServer.Name = "btnServer";
            this.btnServer.Size = new System.Drawing.Size(198, 40);
            this.btnServer.TabIndex = 4;
            this.btnServer.TabStop = false;
            this.btnServer.Text = "Websocket";
            this.btnServer.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnServer.UseVisualStyleBackColor = true;
            this.btnServer.Click += new System.EventHandler(this.OnButton);
            // 
            // labelTitle
            // 
            this.labelTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelTitle.Enabled = false;
            this.labelTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelTitle.Location = new System.Drawing.Point(9, 5);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(727, 40);
            this.labelTitle.TabIndex = 0;
            this.labelTitle.Text = "Settings";
            this.labelTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelTitle.Paint += new System.Windows.Forms.PaintEventHandler(this.LabelTitle_Paint);
            // 
            // SettingsForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(46)))), ((int)(((byte)(46)))));
            this.ClientSize = new System.Drawing.Size(784, 444);
            this.ControlBox = false;
            this.Controls.Add(this.labelTitle);
            this.Controls.Add(this.btnServer);
            this.Controls.Add(this.btnGames);
            this.Controls.Add(this.btnPlugins);
            this.Controls.Add(this.btnExit);
            this.ForeColor = System.Drawing.Color.White;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(800, 460);
            this.Name = "SettingsForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Button btnPlugins;
        private System.Windows.Forms.Button btnGames;
        private System.Windows.Forms.Button btnServer;
        private System.Windows.Forms.Label labelTitle;
    }
}