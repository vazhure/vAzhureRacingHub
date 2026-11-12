// LibreCorsaPlugin — плагин телеметрии LibreCorsa для vAzhureRacingHub.
//
// Архитектура:
//   Браузер (игра) ──WebSocket──► Bridge (npm run ffb, ws://127.0.0.1:18500)
//   Bridge ──UDP JSON 60 Гц──► этот плагин (слушает на настраиваемом порту)
//
// Bridge пересылает телеметрию на порт, указанный игрой в WebSocket-сообщении
// { t: "tel", port: <port>, d: <data> }. Плагин слушает на этом порту.
//
// Настройка:
//   1. git clone https://github.com/KhazP/librecorsa && npm install
//   2. npm run ffb (если игра на librecorsa.org — задать FFB_ALLOW_ORIGIN)
//   3. В игре указать порт телеметрии = порт плагина (по умолч. 20790)
//   4. Запустить vAzhureRacingHub — плагин слушает UDP

using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using vAzhureRacingAPI;
using System.Text.Json;

namespace LibreCorsaPlugin
{
    public class Plugin : ICustomPlugin
    {
        public string Name => "LibreCorsa";
        public string Description => "LibreCorsa browser racing sim telemetry (UDP JSON via FFB bridge)";
        public ulong Version => 1UL;

        private readonly LibreCorsaGame _game = new LibreCorsaGame();

        public bool Initialize(IVAzhureRacingApp app)
        {
            _game.SetApp(app);
            app.RegisterGame(_game);
            return true;
        }

        public bool CanClose(IVAzhureRacingApp app) => true;

        public void Quit(IVAzhureRacingApp app) => _game.OnQuit();
    }

    public class LibreCorsaGame : IGamePlugin
    {
        public string Name => "LibreCorsa";
        public uint SteamGameID => 0U; // браузерная игра, не в Steam
        public string[] ExecutableProcessName => new[] { "librecorsa" };

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
        private LibreCorsaSettings _settings = new LibreCorsaSettings();
        private UdpClient _udp;
        private System.Timers.Timer _idleTimer;
        private readonly object _dataLock = new object();
        private TelemetryDataSet _data;
        private DateTime _lastPacket = DateTime.MinValue;
        private int _packetCount = 0;

        private IVAzhureRacingApp _app;
        internal void SetApp(IVAzhureRacingApp app) => _app = app;

        public LibreCorsaGame()
        {
            LoadSettings();

            // LibreCorsa — браузерная игра; ProcessMonitor не найдёт процесс.
            // Вместо этого слушаем UDP напрямую — если пакеты идут, значит игра активна.
            // Запускаем слушатель сразу при создании плагина.
            StartUdp();

            _idleTimer = new System.Timers.Timer { Interval = 2000, AutoReset = true };
            _idleTimer.Elapsed += (s, e) =>
            {
                if (_lastPacket == DateTime.MinValue)
                {
                    if (_udp != null && _packetCount == 0)
                        _app?.SetStatusText($"LibreCorsa: waiting for data on :{_settings.ListenPort}...");
                    return;
                }
                if ((DateTime.Now - _lastPacket).TotalSeconds > 3)
                {
                    if (_isRunning)
                    {
                        _isRunning = false;
                        _app?.SetStatusText($"LibreCorsa: stream stopped ({_packetCount} packets total)");
                        OnGameStateChanged?.Invoke(this, EventArgs.Empty);
                    }
                }
            };
            _idleTimer.Start();
        }

        public void ShowSettings(IVAzhureRacingApp app)
        {
            using (var dlg = new LibreCorsaSettingsForm(_settings))
            {
                if (dlg.ShowDialog(app.MainForm) == DialogResult.OK)
                {
                    SaveSettings();
                    if (_isRunning || _udp != null)
                    {
                        StopUdp();
                        StartUdp();
                    }
                }
            }
        }

        public void Start(IVAzhureRacingApp app)
        {
            // LibreCorsa — браузерная игра; "запуск" = открыть URL
            if (!string.IsNullOrEmpty(_settings.GameUrl))
            {
                try { System.Diagnostics.Process.Start(_settings.GameUrl); }
                catch { app.SetStatusText($"Open {_settings.GameUrl} manually"); }
            }
            else
            {
                app.SetStatusText("Open https://librecorsa.org in your browser. Start bridge: npm run ffb");
            }
        }

        // --- UDP слушатель ---

        private void StartUdp()
        {
            StopUdp();
            try
            {
                _udp = new UdpClient(_settings.ListenPort);
                _app?.SetStatusText($"LibreCorsa: listening UDP :{_settings.ListenPort}");
                _udp.BeginReceive(OnUdpReceive, null);
            }
            catch (Exception ex)
            {
                _app?.SetStatusText($"LibreCorsa: port {_settings.ListenPort} BUSY — {ex.Message}");
            }
        }

        private void StopUdp()
        {
            try { _udp?.Close(); } catch { }
            _udp = null;
        }

        private void OnUdpReceive(IAsyncResult ar)
        {
            if (_udp == null) return;
            try
            {
                IPEndPoint ep = null;
                byte[] bytes = _udp.EndReceive(ar, ref ep);
                _udp.BeginReceive(OnUdpReceive, null);
                ProcessPacket(bytes);
            }
            catch (ObjectDisposedException) { return; }
            catch (SocketException) { try { _udp?.BeginReceive(OnUdpReceive, null); } catch { } }
            catch { try { _udp?.BeginReceive(OnUdpReceive, null); } catch { } }
        }

        private static readonly JsonSerializerOptions JsonOpts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        private void ProcessPacket(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0) return;
            LibreCorsaTelemetryPacket pkt;
            try
            {
                string json = Encoding.UTF8.GetString(bytes);
                pkt = JsonSerializer.Deserialize<LibreCorsaTelemetryPacket>(json, JsonOpts);
            }
            catch (Exception ex)
            {
                if (_packetCount < 3)
                    _app?.SetStatusText($"LibreCorsa: JSON parse error — {ex.Message}");
                return;
            }
            if (pkt == null) return;

            _packetCount++;
            _lastPacket = DateTime.Now;

            if (_packetCount == 1)
                _app?.SetStatusText($"LibreCorsa: 1st packet! {pkt.SpeedKmh:F0} km/h, {pkt.Rpm:F0} RPM, G{pkt.Gear}");
            else if (_packetCount % 120 == 0)
                _app?.SetStatusText($"LibreCorsa: {_packetCount} pkts, {pkt.SpeedKmh:F0} km/h, {pkt.Rpm:F0} RPM");

            if (!_isRunning)
            {
                _isRunning = true;
                OnGameStateChanged?.Invoke(this, EventArgs.Empty);
            }

            ApplyToTelemetry(pkt);
        }

        // --- Маппинг в TelemetryDataSet ---

        private void ApplyToTelemetry(LibreCorsaTelemetryPacket p)
        {
            lock (_dataLock)
            {
                _data = _data ?? new TelemetryDataSet(this);
                AMCarData car = _data.CarData;
                AMSessionInfo ses = _data.SessionInfo;
                AMMotionData motion = car.MotionData;

                _data.TelemetryState = (p.SpeedKmh > 0.1 || p.Rpm > 100)
                    ? TelemetryState.Playing : TelemetryState.Paused;

                // Двигатель / коробка
                car.Speed = p.SpeedKmh;                    // уже км/ч
                car.RPM = p.Rpm;
                car.MaxRPM = p.MaxRpm;
                car.ShiftUpRPM = 0;                         // LibreCorsa не сообщает optimal shift RPM
                car.Gear = (short)(p.Gear + 1);
                car.Steering = p.Steer * 450;               // -1..1 → градусы (приблизительно, ±450°)
                car.Throttle = Clamp01(p.Throttle);
                car.Brake = Clamp01(p.Brake);
                car.Clutch = Clamp01(p.Clutch);

                // Топливо: кг → литры (плотность бензина ~0.74 кг/л)
                car.FuelLevel = p.FuelKg / 0.74;
                car.FuelCapacity = -1;                      // не сообщается

                // Электроника
                CarElectronics el = CarElectronics.None;
                if (p.Abs) el |= CarElectronics.ABS;
                if (p.Tc) el |= CarElectronics.TCS;
                car.Electronics = el;
                car.AIControlled = false;

                car.Position = p.Position;
                car.Lap = p.Lap;
                car.Distance = p.TrackPos;                  // 0..1 (нормализованная позиция на круге)

                // Шины [FL, FR, RL, RR]
                car.Tires = new[]
                {
                    ToTire(p, 0), ToTire(p, 1), ToTire(p, 2), ToTire(p, 3),
                };

                motion.Sway = (float)(-p.GLat) / 9.81f;
                motion.Surge = (float)(p.GLon) / 9.81f;
                motion.Heave = (float)(-(p.GVert - 1f)) / 9.81f;
                motion.LocalAcceleration = new[] { motion.Sway, motion.Heave, motion.Surge };

                motion.Pitch = (float)(p.Pitch / Math.PI);
                motion.Roll = (float)(p.Roll / Math.PI);
                motion.LocalRot = new[] { (float)p.YawRate, 0f, 0f };
                motion.ABSVibration = p.Abs ? 1f : 0f;

                // Сессия
                ses.Valid = true;
                ses.CurrentLapNumber = p.Lap;
                ses.CurrentLapTime = (int)p.LapTimeMs;       // уже ms
                ses.LastSector1Time = 0;                     // не сообщается
                ses.BestLapTime = (int)p.BestLapMs;          // 0 = нет
                ses.CurrentPosition = p.Position;
                ses.TrackName = p.Track ?? "";
                ses.SessionState = "";
                ses.Flag = p.OffTrack ? "White" : "Green";

                // UserData — «сырые» поля для дашбордов
                var ud = _data.UserData;
                ud["LibreCorsa.Car"] = p.Car ?? "";
                ud["LibreCorsa.Track"] = p.Track ?? "";
                ud["LibreCorsa.Timestamp"] = p.T;
                ud["LibreCorsa.ErsPct"] = p.ErsPct;
                ud["LibreCorsa.Boost"] = p.Boost;
                ud["LibreCorsa.Drs"] = p.Drs;
                ud["LibreCorsa.AeroMode"] = p.AeroMode ?? "";
                ud["LibreCorsa.FuelKg"] = p.FuelKg;
                ud["LibreCorsa.TrackPos"] = p.TrackPos;
                ud["LibreCorsa.Kerb"] = p.Kerb;
                ud["LibreCorsa.OffTrack"] = p.OffTrack;
                ud["LibreCorsa.Ffb"] = p.Ffb;
                ud["LibreCorsa.OversteerPct"] = p.OversteerPct;
                ud["LibreCorsa.UndersteerPct"] = p.UndersteerPct;
                if (p.WheelSlip != null) ud["LibreCorsa.WheelSlip"] = string.Join(",", p.WheelSlip);
                if (p.SuspTravelMm != null) ud["LibreCorsa.SuspTravelMm"] = string.Join(",", p.SuspTravelMm);
                if (p.Surface != null) ud["LibreCorsa.Surface"] = string.Join(",", p.Surface);

                OnTelemetry?.Invoke(this, new TelemetryUpdatedEventArgs(_data));
            }
        }

        private static AMTireData ToTire(LibreCorsaTelemetryPacket p, int i)
        {
            return new AMTireData
            {
                Pressure = Get(p.TyrePsi, i) * 6.89476,       // PSI → кПа
                Wear = Clamp01(Get(p.TyreWearPct, i) / 100.0),// 0..100% → 0..1
                Temperature = new[]
                {
                    Get(p.TyreTempC, i), Get(p.TyreTempC, i), Get(p.TyreTempC, i),
                },
                BrakeTemperature = Get(p.BrakeTempC, i),
                Detached = false,
                Compound = "",
            };
        }

        private static double Get(double[] arr, int i) => (arr != null && i < arr.Length) ? arr[i] : 0;

        private static double Clamp01(double v) => v < 0 ? 0 : (v > 1 ? 1 : v);

        internal void OnQuit()
        {
            try { _monitor?.Stop(); } catch { }
            try { _idleTimer?.Stop(); } catch { }
            StopUdp();
            SaveSettings();
        }

        // --- Настройки ---

        private void LoadSettings()
        {
            string path = SettingsPath;
            if (File.Exists(path))
            {
                try { _settings = ObjectSerializeHelper.DeserializeJson<LibreCorsaSettings>(File.ReadAllText(path)); }
                catch { }
            }
        }

        private void SaveSettings()
        {
            try
            {
                string path = SettingsPath;
                string dir = Path.GetDirectoryName(path);
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                File.WriteAllText(path, _settings.GetJson());
            }
            catch { }
        }

        Icon IGamePlugin.GetIcon()
        {
            return Properties.Resources.lc_logo;
        }

        private string SettingsPath => Path.Combine(
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? ".",
            "LibreCorsa.json");
    }

    public class LibreCorsaSettings
    {
        public int ListenPort { get; set; } = 20790;
        public string GameUrl { get; set; } = "https://librecorsa.org";
    }

    // Простая форма настроек
    public class LibreCorsaSettingsForm : Form
    {
        private readonly LibreCorsaSettings _settings;
        private readonly TextBox _txtPort;
        private readonly TextBox _txtUrl;

        public LibreCorsaSettingsForm(LibreCorsaSettings settings)
        {
            _settings = settings;
            Text = "LibreCorsa — настройки";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(380, 160);

            int y = 12;
            var l1 = new Label { Text = "UDP порт (как в игре):", Left = 12, Top = y + 3, AutoSize = true };
            _txtPort = new TextBox { Left = 200, Top = y, Width = 160, Text = _settings.ListenPort.ToString() };
            Controls.Add(l1); Controls.Add(_txtPort);
            y += 30;

            var l2 = new Label { Text = "URL игры:", Left = 12, Top = y + 3, AutoSize = true };
            _txtUrl = new TextBox { Left = 200, Top = y, Width = 160, Text = _settings.GameUrl };
            Controls.Add(l2); Controls.Add(_txtUrl);
            y += 34;

            var ok = new Button { Text = "OK", DialogResult = DialogResult.OK, Left = 210, Top = y, Width = 75, Height = 26 };
            var cancel = new Button { Text = "Отмена", DialogResult = DialogResult.Cancel, Left = 295, Top = y, Width = 75, Height = 26 };
            Controls.Add(ok); Controls.Add(cancel);
            AcceptButton = ok; CancelButton = cancel;

            FormClosing += (s, e) =>
            {
                if (DialogResult == DialogResult.OK)
                {
                    if (int.TryParse(_txtPort.Text, out int port)) _settings.ListenPort = port;
                    _settings.GameUrl = _txtUrl.Text ?? "";
                }
            };
        }
    }
}
