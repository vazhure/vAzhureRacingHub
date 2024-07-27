using System;
using System.Linq;
using System.Windows.Forms;

namespace vAzhureRacingHub
{
    public partial class WebServerSettingsControl : SettingsControl
    {
        public WebServerSettingsControl()
        {
            InitializeComponent();
        }

        WebServer webServer;
        AppWindow app = null;

        public override void Initialize(AppWindow appWindow)
        {
            base.Initialize(appWindow);
            app = appWindow;
            webServer = appWindow.WebServer;
            btnEnabled.Checked = webServer.Enabled;
            labelPath.Text = webServer.ServerPath;
            webServer.OnClientConnected += WebServer_OnClientConnected;
            webServer.OnClientDisconnected += WebServer_OnClientDisconnected;
            listClients.Items.Clear();
            if (webServer.Clients.Length > 0)
                listClients.Items.AddRange(webServer.Clients);

            if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                lblIP.Text = $"IP: {GetLocalIPAddress()}";
            }
        }

        public static string GetLocalIPAddress()
        {
            var host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
            string result = "127.0.0.1";
            
            foreach (var ip in host.AddressList.Where(p => p.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork))
            {
                result += $", {ip.ToString()}";
            }

            return result;
        }

        private void WebServer_OnClientDisconnected(object sender, WSClient e)
        {
            Invoke((Action)delegate
            {
                listClients.Items.Remove(e.IP);
                listClients.Invalidate();
            });
        }
        
        private void WebServer_OnClientConnected(object sender, WSClient e)
        {
            Invoke((Action)delegate
            {
                listClients.Items.Add(e.IP);
                listClients.Invalidate();
            });
        }

        private void BtnEnabled_OnSwitch(object sender, EventArgs e)
        {
            webServer.Enabled = btnEnabled.Checked;

            if (webServer.Enabled && !webServer.IsRunning)
                webServer.Start();
            else
                if (!webServer.Enabled)
                webServer.Stop();
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

        private void BtnSelectFolder_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog()
            {
                SelectedPath = webServer.ServerPath,
                Description = "Выберите папку сервера"
            })
            {
                if (folderBrowserDialog.ShowDialog(ParentForm) == DialogResult.OK)
                {
                    if (webServer.ServerPath != folderBrowserDialog.SelectedPath)
                    {
                        webServer.ServerPath = labelPath.Text = folderBrowserDialog.SelectedPath;
                        if (MessageBox.Show(this, "Перезапустить приложение?", Properties.Resources.AppTitle, MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            webServer.StoreSettings();
                            app.Restart();
                        }
                    }
                }
            }
        }

        private void NumPort_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                webServer.Port = (int)numPort.Value;
            }catch
            {
                numPort.Value = webServer.Port;
            }
        }
    }
}
