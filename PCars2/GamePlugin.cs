using System;
using System.Drawing;
using vAzhureRacingAPI;

namespace PCars2
{
    public class GamePlugin : IGamePlugin
    {
        public enum GameID { AMS2, PC2};

        readonly GameID m_gameID = GameID.AMS2;
        public GamePlugin(GameID gameID) 
        {
            TelemetryData = new TelemetryDataSet(this);
            m_gameID = gameID;
        }

        public string Name => m_gameID == GameID.AMS2? "Automobilista 2" : "ProjectCARS 2";

        public uint SteamGameID => m_gameID == GameID.AMS2 ? 1066890U : 378860U;

        public string[] ExecutableProcessName => m_gameID == GameID.AMS2 ? new string[] { "AMS2AVX","AMS2" } : new string[] { "pCARS2AVX", "pcars2" };

        string sUserIconPath = "";
        string sUserExecutablePath = "";

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

        public bool IsRunning
        {
            get
            {
                return Utils.IsProcessRunning(ExecutableProcessName);
            }
        }

        public event EventHandler<TelemetryUpdatedEventArgs> OnTelemetry;
        public event EventHandler OnGameStateChanged;
        public event EventHandler OnGameIconChanged;

        public Icon GetIcon()
        {
            return m_gameID == GameID.AMS2 ? Properties.Resources.AMS2 : Properties.Resources.pCARS2;
        }

        public void ShowSettings(IVAzhureRacingApp app)
        {
            // TODO:
        }

        public void Start(IVAzhureRacingApp app)
        {
            if (!Utils.RunSteamGame(SteamGameID))
            {
                app.SetStatusText($"Ошибка запуска игры {Name}!");
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
