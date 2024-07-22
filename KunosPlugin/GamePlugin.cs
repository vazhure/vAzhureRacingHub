using System;
using System.Drawing;
using vAzhureRacingAPI;

namespace KunosPlugin
{
    internal class GamePlugin : IGamePlugin
    {
        public enum GameID { AC, ACC };
        public readonly GameID gameID = GameID.ACC;

        public TelemetryDataSet DataSet { get; private set; }

        public GamePlugin(GameID id)
        {
            gameID = id;
            DataSet = new TelemetryDataSet(this);
        }

        public void NotifyTelemetry()
        {
            OnTelemetry?.Invoke(this, new TelemetryUpdatedEventArgs(DataSet));
        }

        public string Name => gameID == GameID.AC ? "Assetto Corsa" : "Assetto Corsa Competizione";

        public uint SteamGameID => gameID == GameID.AC ? 244210U : 805550U;

        public string[] ExecutableProcessName => gameID == GameID.AC ? new string[] { "acs", "acs_x86.exe" } : new string[] { "acc", "AC2-Win64-Shipping" };

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

        public bool Enabled { get; private set; } = true;

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

        public void NotifyGameState()
        {
            OnGameStateChanged?.Invoke(this, new EventArgs());
        }

        public Icon GetIcon()
        {
            return gameID == GameID.AC ? Properties.Resources.AssettoCorsa : Properties.Resources.acc;
        }

        public void ShowSettings(IVAzhureRacingApp app)
        {
            // TODO
        }

        public void Start(IVAzhureRacingApp app)
        {
            if (!Utils.RunSteamGame(SteamGameID))
            {
                app.SetStatusText($"Steam Service is not running. Run {Name} manually!");
            }
        }
    }
}
