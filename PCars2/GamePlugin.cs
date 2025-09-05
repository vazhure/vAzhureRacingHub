using System;
using System.Drawing;
using System.Threading;
using vAzhureRacingAPI;

namespace PCars2
{
    public class GamePlugin : IGamePlugin
    {
        public enum GameID { AMS2, PC2, PC3 };

        readonly GameID m_gameID = GameID.AMS2;
        private readonly ProcessMonitor monitor;

        public GamePlugin(GameID gameID)
        {
            TelemetryData = new TelemetryDataSet(this);
            m_gameID = gameID;

            monitor = new ProcessMonitor(ExecutableProcessName);

            monitor.OnProcessRunningStateChanged += (object o, bool bRunning) =>
            {
                _bRunning = bRunning;

                try
                {
                    if (!bRunning)
                    {
                        TelemetryData.LoadDefaults();
                        OnTelemetry?.Invoke(this, new TelemetryUpdatedEventArgs(TelemetryData));
                    }

                    OnGameStateChanged?.Invoke(this, new EventArgs());
                }
                catch { }
            };

            monitor.Start();
        }
        ~GamePlugin()
        {
            monitor.Stop();
        }

        public string Name
        {
            get
            {
                switch (m_gameID)
                {
                    default:
                    case GameID.AMS2: return "Automobilista 2";
                    case GameID.PC2: return "ProjectCARS 2";
                    case GameID.PC3: return "ProjectCARS 3";
                }
            }
        }

        public uint SteamGameID
        {
            get
            {
                switch (m_gameID)
                {
                    default:
                    case GameID.AMS2: return 1066890U;
                    case GameID.PC2: return 378860U;
                    case GameID.PC3: return 958400U;
                }
            }
        }

        public string[] ExecutableProcessName
        {
            get
            {
                switch (m_gameID)
                {
                    default:
                    case GameID.AMS2: return new string[] { "AMS2AVX", "AMS2" };
                    case GameID.PC2: return new string[] { "pCARS2AVX", "pcars2" } ;
                    case GameID.PC3: return new string[] { "pCARS3AVX", "pcars3" };
                }
            }
        }

        string sUserIconPath = "";
        string sUserExecutablePath = "";
        private bool _bRunning;

        public string UserIconPath
        {
            get
            {
                return sUserIconPath;
            }
            set
            {
                if (sUserIconPath != value)
                {
                    sUserIconPath = value;
                    // TODO: Загрузка иконки
                    OnGameIconChanged?.Invoke(this, new EventArgs());
                }
            }
        }
        public string UserExecutablePath { get => sUserExecutablePath; set => sUserExecutablePath = value; }

        public bool IsRunning => _bRunning;

        public event EventHandler<TelemetryUpdatedEventArgs> OnTelemetry;
        public event EventHandler OnGameStateChanged;
        public event EventHandler OnGameIconChanged;

        public Icon GetIcon()
        {
            switch (m_gameID)
            {
                default:
                case GameID.AMS2: return Properties.Resources.AMS2;
                case GameID.PC2: return Properties.Resources.pCARS2;
                case GameID.PC3: return Properties.Resources.pCARS3;
            }
        }

        public void ShowSettings(IVAzhureRacingApp app)
        {
            // TODO:
        }

        public void Start(IVAzhureRacingApp app)
        {
            if (!Utils.RunSteamGame(SteamGameID))
            {
                app.SetStatusText($"Steam Service is not running. Run {Name} manually!");
            }
        }

        public GamePlugin() => TelemetryData = new TelemetryDataSet(this);

        public TelemetryDataSet TelemetryData { get; private set; }

        public void NotifyTelemetry(TelemetryDataSet data)
        {
            OnTelemetry?.Invoke(this, new TelemetryUpdatedEventArgs(data));
        }

        public void NotifyGameState()
        {
            OnGameStateChanged?.Invoke(this, new EventArgs());
        }
    }
}
