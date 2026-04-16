using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using vAzhureRacingAPI;

namespace MudRunner
{
    /// <summary>
    /// MudRunner game plugin for vAzhureRacingAPI framework.
    /// 
    /// This plugin reads telemetry data from MudRunner via a shared file.
    /// The game exports telemetry through a Lua mod script that writes
    /// structured data to a local file:
    /// 
    ///   Lua (in-game) → telemetry_out.txt (game directory)
    ///     → TelemetryFileReader → MudRunnerGame → vAzhureRacingAPI
    /// 
    /// TRANSPORT: File-based polling
    /// 
    /// SSD WEAR: ~300 bytes x 60 Hz = 18 KB/sec.
    ///   Same file overwritten each frame. Negligible for modern SSDs.
    /// </summary>
    public class MudRunnerGame : IGamePlugin
    {
        public string Name => "MudRunner";

        public uint SteamGameID => 675010U; // MudRunner Steam App ID

        public string[] ExecutableProcessName => new[] { "MudRunner" };

        private string sUserIcon = string.Empty;
        private string sExecutablePath = string.Empty;

        public string UserIconPath { get => sUserIcon; set => sUserIcon = value; }
        public string UserExecutablePath { get => sExecutablePath; set => sExecutablePath = value; }

        public bool IsRunning => Utils.IsProcessRunning(ExecutableProcessName);

        public event EventHandler<TelemetryUpdatedEventArgs> OnTelemetry;
        public event EventHandler OnGameStateChanged;
        public event EventHandler OnGameIconChanged;

        /// <summary>
        /// Get the game icon.
        /// </summary>
        public Icon GetIcon()
        {
            try
            {
                return Properties.Resources.MudRunner;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Show settings dialog.
        /// Opens a file dialog to locate MudRunner.exe, extracts the game
        /// directory, and saves it for the TelemetryFileReader.
        /// </summary>
        public void ShowSettings(IVAzhureRacingApp app)
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Title = "Select MudRunner executable (MudRunner.exe)";
                dialog.Filter = "MudRunner executable|MudRunner.exe|Executable files|*.exe|All files|*.*";
                dialog.FileName = "MudRunner.exe";

                // Pre-select the current game directory if known
                if (!string.IsNullOrEmpty(GameDirectory) && Directory.Exists(GameDirectory))
                {
                    dialog.InitialDirectory = GameDirectory;
                }

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedPath = dialog.FileName;

                    // Validate: must be named MudRunner.exe
                    if (!string.Equals(Path.GetFileName(selectedPath), "MudRunner.exe",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        MessageBox.Show(
                            "Please select MudRunner.exe, not " + Path.GetFileName(selectedPath),
                            "MudRunner Plugin",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                        return;
                    }

                    // Extract game directory
                    string gameDir = Path.GetDirectoryName(selectedPath);
                    if (string.IsNullOrEmpty(gameDir))
                    {
                        MessageBox.Show(
                            "Could not determine game directory from selected path.",
                            "MudRunner Plugin",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        return;
                    }

                    // Update settings
                    GameDirectory = gameDir;
                    settings.GameDirectory = GameDirectory;

                    // Restart file reader with new path
                    ApplyGameDirectory();

                    // Save settings to disk
                    SaveSettings();

                    app.SetStatusText($"MudRunner game folder: {GameDirectory}");
                    Console.WriteLine($"[MudRunner] Settings updated — game directory: {GameDirectory}");
                }
            }
        }

        /// <summary>
        /// Start the game via Steam.
        /// </summary>
        public void Start(IVAzhureRacingApp app)
        {
            if (GameDirectory == string.Empty)
            {
                if (MessageBox.Show("Game directory is not defined. Proceed?", app.MainForm.Text, buttons: MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    ShowSettings(app);
                }
            }

            if (!Utils.RunSteamGame(SteamGameID))
            {
                app.SetStatusText($"Steam is not running. Start {Name} manually!");
            }
        }

        // ============================================================
        //  Private fields
        // ============================================================

        private readonly ProcessMonitor monitor;
        private readonly TelemetryDataSet dataSet;
        private readonly TelemetryFileReader fileReader;

        private MudRunnerTelemetry.TelemetryPacket currentPacket;
        private ushort sequenceCounter;
        private DateTime lastUpdate;

        /// <summary>
        /// GameSettings instance for persistent configuration.
        /// </summary>
        private GameSettings settings;

        /// <summary>
        /// Full path to the game directory containing MudRunner.exe.
        /// The telemetry file is expected at: GameDirectory\TelemetryFileName
        /// </summary>
        public string GameDirectory { get; set; } = "";


        /// <summary>
        /// Name of the telemetry file (relative to game directory).
        /// Lua must use: io.open("telemetry_out.txt", "w")
        /// </summary>
        public string TelemetryFileName { get; set; } = "telemetry_out.txt";

        /// <summary>
        /// Timeout in seconds before declaring "no data" (default: 2s).
        /// </summary>
        public int NoDataTimeoutSeconds { get; set; } = 2;

        // ============================================================
        //  Constructor / Destructor
        // ============================================================

        public MudRunnerGame()
        {
            dataSet = new TelemetryDataSet(this);
            sequenceCounter = 0;
            currentPacket = new MudRunnerTelemetry.TelemetryPacket() { bValid = false };
            lastUpdate = DateTime.MinValue;

            // Load saved settings (GameDirectory, etc.)
            LoadSettings();

            fileReader = new TelemetryFileReader(TelemetryFileName)
            {
                FullPath = Path.Combine(settings.GameDirectory, TelemetryFileName)
            };
            fileReader.OnTelemetryPacket += OnNewTelemetryData;
            fileReader.OnConnectionStateChanged += OnFileConnectionChanged;

            // Set up process monitor
            monitor = new ProcessMonitor(ExecutableProcessName);
            monitor.OnProcessRunningStateChanged += OnProcessStateChanged;
            monitor.Start();
        }

        ~MudRunnerGame()
        {
            monitor.Stop();
            fileReader?.Stop();
            fileReader?.Dispose();
        }

        /// <summary>
        /// Save settings to a JSON file next to the plugin DLL.
        /// File: MudRunner.json
        /// </summary>
        private void SaveSettings()
        {
            string path = Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                $"{Name}.json");

            try
            {
                string jsonNew = settings.GetJson();
                string jsonOld = "";

                if (File.Exists(path))
                {
                    try { jsonOld = File.ReadAllText(path); }
                    catch { }
                }

                // Only write if changed
                if (jsonOld != jsonNew)
                {
                    File.WriteAllText(path, jsonNew);
                    Console.WriteLine($"[MudRunner] Settings saved to {path}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MudRunner] Failed to save settings: {ex.Message}");
            }
        }

        /// <summary>
        /// Load settings from a JSON file next to the plugin DLL.
        /// File: MudRunner.json
        /// </summary>
        private void LoadSettings()
        {
            string path = Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                $"{Name}.json");

            try
            {
                if (File.Exists(path))
                {
                    string json = File.ReadAllText(path);
                    settings = ObjectSerializeHelper.DeserializeJson<GameSettings>(json);

                    if (settings != null)
                    {
                        // Restore game directory
                        string savedDir = settings.GameDirectory;
                        if (!string.IsNullOrEmpty(savedDir))
                        {
                            GameDirectory = savedDir;
                            Console.WriteLine($"[MudRunner] Settings loaded — game directory: {GameDirectory}");
                        }

                        // Restore custom telemetry file name (optional)
                        string savedFile = settings.TelemetryFileName;
                        if (!string.IsNullOrEmpty(savedFile))
                        {
                            TelemetryFileName = savedFile;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MudRunner] Failed to load settings: {ex.Message}");
            }

            // Ensure settings object exists even if file was missing
            if (settings == null)
            {
                settings = new GameSettings();
            }
        }

        /// <summary>
        /// Apply the current GameDirectory to the file reader.
        /// Stops the reader, updates the path, and restarts it.
        /// </summary>
        private void ApplyGameDirectory()
        {
            if (fileReader == null) return;

            bool wasRunning = fileReader.IsRunning;
            if (wasRunning)
            {
                fileReader.Stop();
            }

            fileReader.SetGameDirectory(GameDirectory);

            if (wasRunning)
            {
                fileReader.Start();
            }
        }

        // ============================================================
        //  Event Handlers
        // ============================================================

        private void OnProcessStateChanged(object _, bool isRunning)
        {
            if (isRunning)
            {
                // Game started — try to update the file reader's path
                // Auto-detect will find the game directory on next poll
                fileReader.Start();
            }
            else
            {
                fileReader?.Stop();
                // Game stopped — reset data, but keep file reader alive.
                dataSet.LoadDefaults();
                currentPacket = new MudRunnerTelemetry.TelemetryPacket() { bValid = false };

                OnTelemetry?.Invoke(this, new TelemetryUpdatedEventArgs(dataSet));
            }

            OnGameStateChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnFileConnectionChanged(object sender, bool connected)
        {
            if (connected)
            {
                Console.WriteLine("[MudRunner] Telemetry file detected — receiving data.");
            }
            else
            {
                Console.WriteLine("[MudRunner] Telemetry file stopped updating.");
            }
        }

        private void OnNewTelemetryData(object sender, MudRunnerTelemetry.TelemetryPacket packet)
        {
            sequenceCounter++;
            packet.Sequence = sequenceCounter;

            currentPacket = packet;
            lastUpdate = DateTime.Now;

            // Map telemetry data to vAzhureRacingAPI format
            MapToDataSet(packet);

            // Raise telemetry update event
            OnTelemetry?.Invoke(this, new TelemetryUpdatedEventArgs(dataSet));
        }

        // ============================================================
        //  Data Mapping: MudRunner Telemetry → vAzhureRacingAPI
        // ============================================================

        private void MapToDataSet(MudRunnerTelemetry.TelemetryPacket packet)
        {
            AMCarData carData = dataSet.CarData;

            // === Driver Input ===
            carData.Steering = packet.SteeringAngle;
            carData.Throttle = packet.Throttle;
            carData.Brake = packet.Brake;
            carData.Clutch = packet.Clutch;

            // === Gear ===
            // MudRunner: 0=Neutral, -1=Reverse, 1+=Forward gears
            carData.Gear = (short)packet.Gear;

            // === Speed ===
            carData.Speed = packet.SpeedKmh;

            // === RPM (estimated from engine tension) ===
            carData.RPM = packet.RPM;
            carData.MaxRPM = packet.MaxRPM;

            // === Engine ===
            carData.Electronics = CarElectronics.None;
            if (packet.EngineRunning != 0)
                carData.Electronics |= CarElectronics.Ignition;
            if (packet.Headlights != 0)
                carData.Electronics |= CarElectronics.Headlight;

            // === Directions Light (not available in MudRunner) ===
            carData.DirectionsLight = DirectionsLight.None;

            // === Motion Data (for motion platforms) ===
            carData.MotionData = new AMMotionData()
            {
                // Linear accelerations (m/s^2)
                Surge = packet.SmoothSurge / 98.1f,  // Forward acceleration
                Sway = packet.SmoothSway / 98.1f,    // Lateral acceleration
                Heave = packet.SmoothHeave / 98.1f,  // Vertical acceleration

                Pitch = packet.Pitch / 180.0f,
                Roll = packet.Roll / 180.0f,
                Yaw = packet.Yaw / 180.0f,
            };

#if DEBUG
            Console.WriteLine($"Pitch: {carData.MotionData.Pitch:0.000}; Roll: {carData.MotionData.Roll:0.000}; Yaw: {carData.MotionData.Yaw:0.000}");
#endif
            // === Fuel ===
            // MudRunner does NOT expose fuel to Lua API.
            carData.FuelLevel = 80f;

            // === Weather Data ===
            dataSet.WeatherData.Raining = Math.Min(packet.MaxWaterDepth * 10f, 100f);
            dataSet.WeatherData.TrackTemp = 20f;

            // === Additional custom data ===
            //dataSet.CarData.TireTempFL = packet.DamageFactor * 100f; // Repurpose as damage %
        }

        // ============================================================
        //  Utility Methods
        // ============================================================

        /// <summary>
        /// Check if telemetry data is being received.
        /// </summary>
        public bool IsTelemetryActive
        {
            get
            {
                if (lastUpdate == DateTime.MinValue)
                    return false;
                return (DateTime.Now - lastUpdate).TotalSeconds < NoDataTimeoutSeconds;
            }
        }

        /// <summary>
        /// Get the current telemetry packet (raw).
        /// </summary>
        public MudRunnerTelemetry.TelemetryPacket CurrentPacket => currentPacket;
    }

    internal class GameSettings
    {
        public string GameDirectory { get; set; } = string.Empty;
        public string TelemetryFileName { get; set; } = "telemetry_out.txt";
    }
}
