// RennsportPlugin

using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using vAzhureRacingAPI;

namespace RennsportPlugin
{
    /// <summary>
    /// Точка входа плагина
    /// </summary>
    public class Plugin : ICustomPlugin
    {
        public string Name => "Rennsport";
        public string Description => "Rennsport telemetry plugin";
        public ulong Version => 1UL;

        private readonly RennsportGame _game = new RennsportGame();

        public bool Initialize(IVAzhureRacingApp app)
        {
            app.RegisterGame(_game);
            return true;
        }

        public bool CanClose(IVAzhureRacingApp app) => true;

        public void Quit(IVAzhureRacingApp app) => _game.OnQuit();
    }

    /// <summary>
    /// 
    /// </summary>
    public class RennsportGame : IGamePlugin
    {
        public string Name => "Rennsport";

        // Steam App ID Rennsport
        public uint SteamGameID => 2077750;

        // Имена процессов игры (для ProcessMonitor).
        public string[] ExecutableProcessName => new[] { "Rennsport", "Rennsport-Win64-Shipping" };

        private string _iconPath = "";
        public string UserIconPath { get => _iconPath; set => _iconPath = value; }

        private string _exePath = "";
        public string UserExecutablePath { get => _exePath; set => _exePath = value; }

        private bool _isRunning;
        public bool IsRunning => _isRunning;

        public event EventHandler<TelemetryUpdatedEventArgs> OnTelemetry;
        public event EventHandler OnGameStateChanged;
        public event EventHandler OnGameIconChanged;

        private readonly ProcessMonitor _monitor;
        private RennsportGameSettings _settings = new RennsportGameSettings();

        private readonly RennsportMmfProvider _mmfProvider;

        private readonly object _dataLock = new object();

        // Последнее разобранное состояние телеметрии.
        private readonly TelemetryDataSet _data;

        private readonly System.Timers.Timer _idleTimer;

        public RennsportGame()
        {
            LoadSettings();

            _mmfProvider = new RennsportMmfProvider(this);
            _mmfProvider.OnTelemetry += (o, e) => { OnTelemetry(o, e); };

            _data = new TelemetryDataSet(this);

            _monitor = new ProcessMonitor(ExecutableProcessName, 1000);
            _monitor.OnProcessRunningStateChanged += (o, bRunning) =>
            {
                _isRunning = bRunning;
                if (bRunning) StartMMF();
                else StopMMF();
                OnGameStateChanged?.Invoke(this, EventArgs.Empty);
            };

            _monitor.Start();

            // Если пакеты не приходят > 2 с — считаем, что игра в меню / на паузе.
            _idleTimer = new System.Timers.Timer { Interval = 1000, AutoReset = true };
            _idleTimer.Elapsed += (s, e) =>
            {
                if (!_isRunning) return;
                var last = _mmfProvider._lastTelemetryTime;
                if (last == DateTime.MinValue) return;
                if ((DateTime.Now - last).TotalSeconds > 2)
                {
                    lock (_dataLock)
                    {
                        if (_data != null)
                            _data.TelemetryState = TelemetryState.Menu;
                    }
                }
            };
            _idleTimer.Start();
        }

        private void StopMMF()
        {
            _mmfProvider.Stop();
        }

        private void StartMMF()
        {
            _mmfProvider.Start();
        }

        public Icon GetIcon()
        {
            return Properties.Resources.Rennsport;
        }

        public void ShowSettings(IVAzhureRacingApp app)
        {
            /// TODO:
        }

        public void Start(IVAzhureRacingApp app)
        {
            if (!string.IsNullOrEmpty(_settings.ExecutablePath) && File.Exists(_settings.ExecutablePath))
                Utils.ExecuteCmd(_settings.ExecutablePath);
            else if (SteamGameID != 0U)
            {
                if (!Utils.RunSteamGame(SteamGameID))
                    app.SetStatusText($"Steam Service is not running. Run {Name} manually!");
            }
            else
            {
                app.SetStatusText($"Launch {Name} manually. Steam App ID is not set in the plugin yet.");
            }
        }

        internal void OnQuit()
        {
            try { _monitor?.Stop(); } catch { }
            StopMMF();
            try { _idleTimer?.Stop(); } catch { }
            SaveSettings();
        }


        private void LoadSettings()
        {
            string path = SettingsPath;
            if (File.Exists(path))
            {
                try { _settings = ObjectSerializeHelper.DeserializeJson<RennsportGameSettings>(File.ReadAllText(path)); }
                catch { }
            }
            if (!string.IsNullOrEmpty(_settings.ExecutablePath))
                _exePath = _settings.ExecutablePath;
        }

        private void SaveSettings()
        {
            _settings.ExecutablePath = _exePath;
            try
            {
                string path = SettingsPath;
                string dir = Path.GetDirectoryName(path);
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                File.WriteAllText(path, _settings.GetJson());
            }
            catch { }
        }

        private string SettingsPath => Path.Combine(
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? ".",
            "Rennsport.json");
    }

    /// <summary>Настройки плагина Rennsport (сериализуются в Rennsport.json рядом с DLL).</summary>
    public class RennsportGameSettings
    {
        /// <summary>
        /// Если true — телеметрия читается из shared memory (MMF) Rennsport,
        /// а не из UDP. MMF — предпочтительный путь для новой игры с включённой
        /// опцией "Write physics telemetry to MMF": не нужна регистрация,
        /// keep-alive, не грузится сеть. Структура получена из IL-декомпиляции
        /// ридера SimHub. По умолчанию true. Переключите в false, чтобы
        /// использовать UDP-путь (для старых версий игры без MMF).
        /// </summary>
        public bool UseSharedMemory { get; set; } = true;

        /// <summary>Локальный порт, на котором плагин слушает пакеты от игры.</summary>
        public int ListenPort { get; set; } = 5590;

        /// <summary>Адрес UDP API Rennsport.</summary>
        public string GameHost { get; set; } = "127.0.0.1";

        /// <summary>Порт UDP API Rennsport (как в BroadcastingSocket.json).</summary>
        public int GamePort { get; set; } = 559;

        /// <summary>Пароль доступа (accessPassword из BroadcastingSocket.json). Обычно пустой.</summary>
        public string AccessPassword { get; set; } = "";

        /// <summary>Путь к исполняемому файлу игры (необязательно).</summary>
        public string ExecutablePath { get; set; } = "";
    }
}
