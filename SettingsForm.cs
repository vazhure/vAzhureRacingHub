using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using vAzhureRacingAPI;

namespace vAzhureRacingHub
{
    public partial class SettingsForm : MovableForm
    {
        readonly AppWindow appWindow;
        readonly Dictionary<Control, SettingsControl> pages = new Dictionary<Control, SettingsControl>();

        readonly Color ActiveBtnColor = Color.FromArgb(56, 56, 56);
        public SettingsForm(AppWindow wnd)
        {
            appWindow = wnd;
            InitializeComponent();
            InitializePages();
        }

        private readonly SettingsControl[] settingsControls = new SettingsControl[] { new PluginsSettingsControl(), new GamesSettingsControl(), new WebServerSettingsControl() };

        private void InitializePages()
        {
            Rectangle rcWindow = new Rectangle(btnPlugins.Right, btnPlugins.Top, ClientRectangle.Width - btnPlugins.Right - Padding.Right,
                ClientRectangle.Height - btnPlugins.Top - Padding.Bottom);

            SuspendLayout();

            foreach (SettingsControl c in settingsControls)
            {
                Controls.Add(c);
                c.Size = rcWindow.Size;
                c.Location = rcWindow.Location;
                c.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
                c.Visible = false;
            }

            ResumeLayout(false);

            pages.Add(btnPlugins, settingsControls[0]);
            pages.Add(btnGames, settingsControls[1]);
            pages.Add(btnServer, settingsControls[2]);

            foreach (SettingsControl c in settingsControls)
                c.Initialize(appWindow);

            ActivatePage(btnPlugins);
        }

        void ActivatePage(Control btn)
        {
            foreach (var kp in pages)
            {
                if (kp.Key == btn)
                {
                    kp.Value.Visible = true;
                    kp.Value.BackColor = kp.Key.BackColor = ActiveBtnColor;
                }
                else
                {
                    kp.Value.Visible = false;
                    kp.Value.BackColor = kp.Key.BackColor = BackColor;
                }
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            pages.Clear();
        }

        private void BtnExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void OnButton(object sender, EventArgs e)
        {
            ActivatePage(sender as Control);
        }

        private void LabelTitle_Paint(object sender, PaintEventArgs e)
        {
            if (sender is Control label)
            {
                e.Graphics.Clear(label.BackColor);
                using (StringFormat sf = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                    e.Graphics.DrawString(label.Text, label.Font, Brushes.White, label.ClientRectangle, sf);
            }
        }
    }
}