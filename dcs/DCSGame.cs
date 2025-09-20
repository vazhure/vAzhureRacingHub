using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using vAzhureRacingAPI;
using static DCS.DCSGame;

namespace DCS
{
    public class DCSGame : VAzhureUDPClient, IGamePlugin, IDisposable
    {
        public string Name => "DCS World";

        public uint SteamGameID => 223750U;

        public string[] ExecutableProcessName => new[] { "DCS" };

        private string strUserIcon = "";

        public string UserIconPath { get => strUserIcon; set => strUserIcon = value; }
        public string UserExecutablePath { get => settings.ExecutablePath; set => settings.ExecutablePath = value; }

        public bool IsRunning => Utils.IsProcessRunning(ExecutableProcessName);

        public event EventHandler<TelemetryUpdatedEventArgs> OnTelemetry;
        public event EventHandler OnGameStateChanged;
        public event EventHandler OnGameIconChanged;

        private readonly GameSettings settings;

        private readonly ProcessMonitor monitor;
        private readonly TelemetryDataSet dataSet;

        public DCSGame()
        {
            settings = new GameSettings();

            dataSet = new TelemetryDataSet(this);

            try
            {
                string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), $"{Name}.json");
                if (File.Exists(path))
                {
                    string json = File.ReadAllText(path);

                    settings = ObjectSerializeHelper.DeserializeJson<GameSettings>(json);
                }
            }
            catch { }

            monitor = new ProcessMonitor(ExecutableProcessName);
            monitor.OnProcessRunningStateChanged += (process, bRunning) =>
            {
                if (bRunning)
                {
                    Run(settings.Port);
                }
                else
                {
                    dataSet.LoadDefaults();
                    OnTelemetry?.Invoke(this, new TelemetryUpdatedEventArgs(dataSet));
                    Stop();
                }
                OnGameStateChanged?.Invoke(this, EventArgs.Empty);
            };
            monitor.Start();
        }

        readonly string assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public System.Drawing.Icon GetIcon()
        {
            return Properties.Resources.DCS_2;
        }

        public void ShowSettings(IVAzhureRacingApp app)
        {
            if (MessageBox.Show("Patch DCS World?", "Patch", buttons: MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
            {
                ///Saved Games\DCS\Scripts\Hooks
                string dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Saved Games", "DCS", "Scripts", "Hooks");
                string filePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "SHTelemetry.lua");
                string destPath = Path.Combine(dir, "SHTelemetry.lua");

                if (File.Exists(destPath))
                {
                    app.SetStatusText($"{Name} already patched!");
                    return;
                }

                try
                {
                    if (!Directory.Exists(dir))
                        Directory.CreateDirectory(dir);

                    File.Copy(filePath, destPath);
                    app.SetStatusText($"{Name} patched successfully!");
                }
                catch
                {
                    app.SetStatusText($"Failed to patch {Name}");
                }
            }
        }

        public void Start(IVAzhureRacingApp app)
        {
            {
                if (settings.ExecutablePath != string.Empty)
                {
                    if (Utils.ExecuteCmd(settings.ExecutablePath))
                        return;
                }

                if (!Utils.RunSteamGame(SteamGameID))
                {
                    app.SetStatusText($"Steam Service is not running. Run {Name} manually!");
                }
            }
        }

#if DEBUG
    private readonly List<Telemetry> telemetry_cache = new List<Telemetry>(10000000);
#endif

        public override void OnDataReceived(ref byte[] bytes)
        {
            try
            {
                string message = Encoding.UTF8.GetString(bytes);
                Telemetry telemetry = ObjectSerializeHelper.DeserializeJson<Telemetry>(message);

#if DEBUG
                    if (telemetry_cache.Count < telemetry_cache.Capacity)
                        telemetry_cache.Add(telemetry);
#endif

                if (telemetry != null)
                {
                    if (telemetry.missionStartTime > 0)
                    {
                        var motion = dataSet.CarData.MotionData = new AMMotionData()
                        {
                            Roll = (float)(telemetry.aDIBank / Math.PI),
                            Pitch = (float)(telemetry.aDIPitch / Math.PI),
                            Yaw = (float)(telemetry.aDIYaw / Math.PI),
                            Surge = telemetry.accelerationUnits.x,
                            Sway = -telemetry.accelerationUnits.z,
                            Heave = -(telemetry.accelerationUnits.y - 1f),
                        };
                    }
                    else
                        dataSet.CarData.MotionData = new AMMotionData();

                    OnTelemetry?.Invoke(this, new TelemetryUpdatedEventArgs(dataSet));
                }
            }
            catch { }
        }

        public void Dispose()
        {
#if DEBUG
            if (telemetry_cache.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("aDIPitch;aDIBank;aDIYaw;accelerationUnits.x;accelerationUnits.y;accelerationUnits.z;");
                foreach (var tele in telemetry_cache)
                    sb.AppendLine($"{tele.aDIPitch};{tele.aDIBank};{tele.aDIYaw};{tele.accelerationUnits.x};{tele.accelerationUnits.y};{tele.accelerationUnits.z};");

                File.WriteAllText(Path.Combine(assemblyPath, "dcs_telemetry.csv"), sb.ToString());
            }
#endif

            monitor?.Stop();
            try
            {
                string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), $"{Name}.json");

                string json = ObjectSerializeHelper.GetJson(settings);
                File.WriteAllText(path, json);
            }
            catch { }
        }
    }

    public class GameSettings
    {
        public int Port { get; set; } = 10025;
        public string IP { get; set; } = "127.0.0.1";
        public string ExecutablePath = "";
    }

    public class Vec3
    {
        public float x { get; set; } = 0;
        public float y { get; set; } = 0;
        public float z { get; set; } = 0;
    }

    public class Telemetry
    {
        public float aDIPitch { get; set; } = 0;
        public float aDIBank { get; set; } = 0;
        public float aDIYaw { get; set; } = 0;
        public Vec3 accelerationUnits { get; set; } = new Vec3();
        public Vec3 angularVelocity { get; set; } = new Vec3();
        public Vec3 vectorVelocity { get; set; } = new Vec3();
        public float verticalVelocity { get; set; } = 0;
        public int missionStartTime { get; set; } = 0;
    }
}
