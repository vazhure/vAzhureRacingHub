using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using vAzhureRacingAPI;

namespace vAzhureRacingHub
{
    public partial class PluginsSettingsControl : SettingsControl
    {
        AppWindow app = null;

        public PluginsSettingsControl()
        {
            InitializeComponent();
        }

        public override void Initialize(AppWindow appWindow)
        {
            base.Initialize(appWindow);
            app = appWindow;

            SuspendLayout();
            int y = labelFolder.Bottom + 2;
            int x = 32;

            labelPath.Text = appWindow.Settings.PluginsFolder;
            labelPath.AutoEllipsis = true;

            foreach (ICustomPlugin plugin in appWindow.Plugins.OrderBy(o => o.Name).ToList())
            {
                VAzhureSwitchButton btn = new VAzhureSwitchButton
                {
                    SwitchText = plugin.Name,
                    Checked = !appWindow.Settings.DisabledPlugins.Contains(plugin.Name)
                };

                btn.OnSwitch += delegate
                {
                    if (!btn.Checked)
                    {
                        if (!appWindow.Settings.DisabledPlugins.Contains(plugin.Name))
                            appWindow.Settings.DisabledPlugins.Add(plugin.Name);
                    }
                    else
                    {
                        if (appWindow.Settings.DisabledPlugins.Contains(plugin.Name))
                            appWindow.Settings.DisabledPlugins.Remove(plugin.Name);
                    }
                };

                Controls.Add(btn);
                btn.Location = new Point(x, y);
                btn.Cursor = Cursors.Hand;
                toolTips.SetToolTip(btn, plugin.Description);
                y += btn.Height + 2;
            }

            ResumeLayout(false);
        }

        private void BtnSelectFolder_Click(object sender, EventArgs e)
        {
            if (app == null)
                return;

            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog()
            {
                SelectedPath = app.Settings.PluginsFolder,
                Description = "Выберите папку плагинов"
            })
            {
                if (folderBrowserDialog.ShowDialog(ParentForm) == DialogResult.OK)
                {
                    if (app.Settings.PluginsFolder != folderBrowserDialog.SelectedPath)
                    {
                        app.Settings.PluginsFolder = labelPath.Text = folderBrowserDialog.SelectedPath;
                        if (MessageBox.Show(this, "Перезапустить приложение?", Properties.Resources.AppTitle, MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            app.Restart();
                        }
                    }
                }
            }
        }

        private void LabelPath_Click(object sender, EventArgs e)
        {
            try
            {
                string args = string.Format($"/root, \"{labelPath.Text}\"");
                System.Diagnostics.Process.Start("explorer.exe", args);
            }
            catch { }
        }

        private void LabelPath_Paint(object sender, PaintEventArgs e)
        {
            Label lbl = sender as Label;
            e.Graphics.Clear(lbl.BackColor);
            using (StringFormat sf = new StringFormat() { FormatFlags = StringFormatFlags.LineLimit | StringFormatFlags.NoWrap | StringFormatFlags.NoClip,
                Trimming = StringTrimming.EllipsisPath, LineAlignment = StringAlignment.Center})
            using (SolidBrush brush = new SolidBrush(lbl.ForeColor))
            {
                e.Graphics.DrawString(lbl.Text, lbl.Font, brush, lbl.ClientRectangle, sf);
            }
        }
    }
}
