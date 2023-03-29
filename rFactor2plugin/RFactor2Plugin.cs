using System;
using System.Drawing;
using vAzhureRacingAPI;

namespace rFactor2plugin
{
    public class RFactor2Plugin : ICustomPlugin
    {
        readonly RFactor2GamePlugin rFactor2GamePlugin = new RFactor2GamePlugin();
        RF2Listener listener = null;

        public string Name => "rFactor 2";

        public string Description => "Плагин Factor2 для vAzhure Racing Hub\nАвтор: Журавлев Андрей";

        public ulong Version => 1U;

        public bool CanClose(IVAzhureRacingApp app)
        {
            return true;
        }

        public bool Initialize(IVAzhureRacingApp app)
        {
            Console.WriteLine($"Plugin Initialization: {Name}");
            app.RegisterGame(rFactor2GamePlugin);
            listener = new RF2Listener(rFactor2GamePlugin, app);
            listener.StartThread();
            listener.OnThreadError += delegate (object sender, EventArgs e)
            {
                app.SetStatusText($"Ошибка процесса плагина {Name}");
                // Перезапуск процесса
                listener.StopTrhead();
                System.Threading.Thread.Sleep(1000); // Делаем паузу, чтоб не загружать процессор
                listener.StartThread();
            };
            return true;
        }

        public void Quit(IVAzhureRacingApp app)
        {
            listener?.StopTrhead();
        }
    }

    public class RFactor2GamePlugin : IGamePlugin
    {
        public string Name => "rFactor 2";

        public uint SteamGameID => 365960U;

        public string[] ExecutableProcessName => new string[] { rFactor2Constants.RFACTOR2_PROCESS_NAME };

        public event EventHandler<TelemetryUpdatedEventArgs> OnTelemetry;
        public event EventHandler OnGameStateChanged;
        public event EventHandler OnGameIconChanged;

        public RFactor2GamePlugin() => TelemetryData = new TelemetryDataSet(this);

        public TelemetryDataSet TelemetryData { get; private set; }

        public bool IsRunning
        {
            get
            {
                return Utils.IsProcessRunning(ExecutableProcessName);
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

        public void NotifyTelemetry(TelemetryDataSet data)
        {
            OnTelemetry?.Invoke(this, new TelemetryUpdatedEventArgs(data));
        }

        public void NotifyGameState()
        {
            OnGameStateChanged?.Invoke(this, new EventArgs());
        }

        readonly Icon gameIcon = Properties.Resources.rFactor2;

        public Icon GetIcon()
        {
            return gameIcon;
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
    }
}