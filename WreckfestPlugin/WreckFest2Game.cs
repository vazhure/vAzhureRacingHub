using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using vAzhureRacingAPI;

namespace WreckfestPlugin
{
    public class WreckFest2Game : VAzhureUDPClient, IGamePlugin
    {
        public string Name => "Wreckfest 2";

        public uint SteamGameID => 1203190u;

        public string[] ExecutableProcessName => new []{ "Wreckfest2" };

        string iconPath = "";
        string exePath = "";

        public string UserIconPath { get => iconPath; set => iconPath = value; }
        public string UserExecutablePath { get => exePath; set => exePath = value; }

        bool bRunning = false;
        public bool IsRunning => bRunning;

        public event EventHandler<TelemetryUpdatedEventArgs> OnTelemetry;
        public event EventHandler OnGameStateChanged;
        public event EventHandler OnGameIconChanged;

        private readonly GameSettings settings;

        private readonly ProcessMonitor monitor;
        private readonly TelemetryDataSet telemetryDataSet;

        public WreckFest2Game()
        {
            telemetryDataSet = new TelemetryDataSet(this);
            settings = LoadSettings(Name);
            monitor = new ProcessMonitor(ExecutableProcessName);
            monitor.OnProcessRunningStateChanged += (object o, bool running) =>
            {
                bRunning = running;

                if (running)
                {
                    Run(settings.PortUDP, 5000);
                }
                else
                {
                    telemetryDataSet.LoadDefaults();
                    OnTelemetry?.Invoke(this, new TelemetryUpdatedEventArgs(telemetryDataSet));
                    Stop();
                }

                OnGameStateChanged?.Invoke(this, new EventArgs());
            };
        }

         ~WreckFest2Game()
        {
            SaveSettings(settings, Name);
        }

        public override void OnDataReceived(ref byte[] bytes)
        {
            // TODO:
            OnTelemetry?.Invoke(this, new TelemetryUpdatedEventArgs(telemetryDataSet));
        }

        public Icon GetIcon()
        {
            return Properties.Resources.Wreckfest2;
        }

        public void ShowSettings(IVAzhureRacingApp app)
        {
            //
        }

        public void Start(IVAzhureRacingApp app)
        {
            if (settings.Executable != string.Empty)
            {
                if (Utils.ExecuteCmd(settings.Executable))
                    return;
            }

            if (!Utils.RunSteamGame(SteamGameID))
            {
                app.SetStatusText($"Steam Service is not running. Run {Name} manually!");
            }
        }

        public static GameSettings LoadSettings(string name)
        {
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetCallingAssembly().Location), $"{name}.json");
            if (File.Exists(path))
            {
                try
                {
                    string json = File.ReadAllText(path);

                    return ObjectSerializeHelper.DeserializeJson<GameSettings>(json);
                }
                catch { }
            }
            return new GameSettings();
        }

        public static void SaveSettings(GameSettings settings, string name)
        {
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetCallingAssembly().Location), $"{name}.json");
            string json = "";
            if (File.Exists(path))
            {
                try
                {
                    json = File.ReadAllText(path);
                }
                catch { }
            }
            string jsonNew = settings.GetJson();
            if (json != jsonNew)
            {
                try
                {
                    File.WriteAllText(path, jsonNew);
                }
                catch { }
            }
        }
    }
}