using System;
using System.Drawing;
using vAzhureRacingAPI;

namespace KunosPlugin
{
    internal class GamePlugin : IGamePlugin
    {
        public enum GameID { AC, ACC, ACEVO };
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

        public string Name {
            get 
            { 
                switch (gameID)
                {
                    default:
                    case GameID.AC: return "Assetto Corsa";
                    case GameID.ACEVO: return "Assetto Corsa EVO";
                    case GameID.ACC: return "Assetto Corsa Competizione";
                }
           }
        }

        //
        public uint SteamGameID
        {
            get
            {
                switch (gameID)
                {
                    default:
                    case GameID.AC: return 244210U;
                    case GameID.ACEVO: return 3058630U;
                    case GameID.ACC: return 805550U;
                }
            }
        }

        public string[] ExecutableProcessName
        {
            get
            {
                switch (gameID)
                {
                    default:
                    case GameID.AC: return new string[] { "acs", "acs_x86" };
                    case GameID.ACEVO: return new string[] { "AssettoCorsaEVO" };
                    case GameID.ACC: return new string[] { "acc", "AC2-Win64-Shipping"};
                }
            }
        }

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
            switch (gameID)
            {
                default:
                case GameID.AC: return Properties.Resources.AssettoCorsa;
                case GameID.ACEVO: return Properties.Resources.evo;
                case GameID.ACC: return Properties.Resources.acc;
            }
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
