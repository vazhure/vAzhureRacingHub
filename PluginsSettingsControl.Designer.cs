
namespace vAzhureRacingHub
{
    partial class PluginsSettingsControl
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
            this.labelFolder = new System.Windows.Forms.Label();
            this.btnSelectFolder = new System.Windows.Forms.Button();
            this.labelPath = new System.Windows.Forms.Label();
            this.toolTips = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // labelFolder
            // 
            this.labelFolder.Location = new System.Drawing.Point(3, 0);
            this.labelFolder.Name = "labelFolder";
            this.labelFolder.Size = new System.Drawing.Size(166, 23);
            this.labelFolder.TabIndex = 0;
            this.labelFolder.Text = "Plugins folder";
            this.labelFolder.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnSelectFolder
            // 
            this.btnSelectFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelectFolder.FlatAppearance.BorderSize = 0;
            this.btnSelectFolder.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSelectFolder.Location = new System.Drawing.Point(401, 0);
            this.btnSelectFolder.Name = "btnSelectFolder";
            this.btnSelectFolder.Size = new System.Drawing.Size(62, 23);
            this.btnSelectFolder.TabIndex = 2;
            this.btnSelectFolder.Text = "..";
            this.toolTips.SetToolTip(this.btnSelectFolder, "Click to pick new folder");
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
            this.labelPath.Size = new System.Drawing.Size(217, 23);
            this.labelPath.TabIndex = 0;
            this.labelPath.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelPath.Click += new System.EventHandler(this.LabelPath_Click);
            this.labelPath.Paint += new System.Windows.Forms.PaintEventHandler(this.LabelPath_Paint);
            // 
            // PluginsSettingsControl
            // 
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(46)))), ((int)(((byte)(46)))));
            this.Controls.Add(this.btnSelectFolder);
            this.Controls.Add(this.labelPath);
            this.Controls.Add(this.labelFolder);
            this.ForeColor = System.Drawing.Color.White;
            this.Name = "PluginsSettingsControl";
            this.Size = new System.Drawing.Size(466, 266);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labelFolder;
        private System.Windows.Forms.Button btnSelectFolder;
        private System.Windows.Forms.Label labelPath;
        private System.Windows.Forms.ToolTip toolTips;
    }
}
