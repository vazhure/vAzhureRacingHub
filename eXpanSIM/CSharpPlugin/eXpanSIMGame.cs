using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using vAzhureRacingAPI;

namespace eXpanSIM
{
    /// <summary>
    /// vAzhureRacingHub plugin for eXpanSIM universal vehicle simulator.
    /// Requires C++ bridge plugin (eXpanSIM_TelemetryBridge.dll) installed in eXpanSIM Plugins folder.
    /// </summary>
    public class eXpanSIMGame : IGamePlugin
    {
        public string Name => "eXpanSIM";

        /// <summary>
        /// Steam AppID for eXpanSIM (closed beta/playtest).
        /// </summary>
        public uint SteamGameID => 1743860U;

        /// <summary>
        /// eXpanSIM process name.
        /// </summary>
        public string[] ExecutableProcessName => new[] { "eXpanSIM" };

        private string strUserIcon = "";
        private string strExecutablePath = "";

        public string UserIconPath { get => strUserIcon; set => strUserIcon = value; }
        public string UserExecutablePath { get => strExecutablePath; set => strExecutablePath = value; }

        public bool IsRunning => Utils.IsProcessRunning(ExecutableProcessName);

        public event EventHandler<TelemetryUpdatedEventArgs> OnTelemetry;
        public event EventHandler OnGameStateChanged;
        public event EventHandler OnGameIconChanged;

        private readonly ProcessMonitor monitor;
        private readonly eXpanSIMListener listener;
        private readonly TelemetryDataSet dataSet;

        public eXpanSIMGame()
        {
            dataSet = new TelemetryDataSet(this);

            listener = new eXpanSIMListener(this);
            listener.OnThreadError += (sender, e) =>
            {
                listener.StopTrhead();
                Thread.Sleep(1000);
                listener.StartThread();
            };

            monitor = new ProcessMonitor(ExecutableProcessName);
            monitor.OnProcessRunningStateChanged += (process, bRunning) =>
            {
                if (bRunning)
                {
                    listener.StartThread();
                }
                else
                {
                    dataSet.LoadDefaults();
                    OnTelemetry?.Invoke(this, new TelemetryUpdatedEventArgs(dataSet));
                    listener.StopTrhead();
                }
                OnGameStateChanged?.Invoke(this, EventArgs.Empty);
            };
            monitor.Start();
        }

        public Icon GetIcon()
        {
            return Properties.Resources.eXpanSIM;
        }

        internal void NotifyTelemetry(TelemetryDataSet tds)
        {
            OnTelemetry?.Invoke(this, new TelemetryUpdatedEventArgs(tds));
        }

        /// <summary>
        /// Installs C++ bridge plugin to Documents\eXpanSIM\Plugins\
        /// </summary>
        public void ShowSettings(IVAzhureRacingApp app)
        {
            try
            {
                if (MessageBox.Show(
                        $"Install eXpanSIM telemetry bridge plugin?{Environment.NewLine}{Environment.NewLine}" +
                        $"This will copy eXpanSIM_TelemetryBridge.dll and eXpanSIM_TelemetryBridge.json to:{Environment.NewLine}" +
                        $"%USERPROFILE%\\Documents\\eXpanSIM\\Plugins\\",
                        "eXpanSIM Setup",
                        MessageBoxButtons.YesNoCancel,
                        MessageBoxIcon.Question,
                        MessageBoxDefaultButton.Button1) != DialogResult.Yes)
                {
                    return;
                }

                string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string pluginsPath = Path.Combine(documentsPath, "eXpanSIM", "Plugins");

                if (!Directory.Exists(pluginsPath))
                    Directory.CreateDirectory(pluginsPath);

                string pluginDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string[] filesToCopy = { "eXpanSIM_TelemetryBridge.dll", "eXpanSIM_TelemetryBridge.json" };

                foreach (string file in filesToCopy)
                {
                    string source = Path.Combine(pluginDir, file);
                    string dest = Path.Combine(pluginsPath, file);

                    if (!File.Exists(source))
                    {
                        app.SetStatusText($"eXpanSIM: {file} not found in plugin folder ({pluginDir})");
                        return;
                    }

                    File.Copy(source, dest, overwrite: true);
                }

                app.SetStatusText($"eXpanSIM: Telemetry bridge installed to {pluginsPath}");
            }
            catch (UnauthorizedAccessException)
            {
                app.SetStatusText("eXpanSIM: Access denied.");
            }
            catch (Exception ex)
            {
                app.SetStatusText($"eXpanSIM: {ex.Message}");
            }
        }

        public void Start(IVAzhureRacingApp app)
        {
            if (!string.IsNullOrEmpty(UserExecutablePath) && File.Exists(UserExecutablePath))
            {
                if (Utils.ExecuteCmd(UserExecutablePath))
                    return;
            }

            if (!Utils.RunSteamGame(SteamGameID))
            {
                app.SetStatusText($"Steam Service is not running. Run {Name} manually!");
            }
        }

        public void Dispose()
        {
            monitor?.Stop();
            listener?.StopTrhead();
        }
    }
}
