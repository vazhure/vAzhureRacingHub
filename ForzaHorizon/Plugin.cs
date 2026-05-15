using System;
using vAzhureRacingAPI;
using ForzaMotorsport;
using System.IO;
using System.Reflection;
using System.Collections.Generic;

namespace ForzaHorizon
{
    public class Plugin : ICustomPlugin
    {
        public string Name => "Forza Horizon";

        public string Description => "Forza Horizon series plugin";

        public ulong Version => 2UL;

        public bool CanClose(IVAzhureRacingApp app)
        {
            return true;
        }

        readonly ForayHorizonGame fh4 = new ForayHorizonGame(ForayHorizonGame.FHVersion.FH4);
        readonly ForayHorizonGame fh5 = new ForayHorizonGame(ForayHorizonGame.FHVersion.FH5);
        readonly ForayHorizonGame fh6 = new ForayHorizonGame(ForayHorizonGame.FHVersion.FH6);
        readonly ForayHorizonGame fMS = new ForayHorizonGame(ForayHorizonGame.FHVersion.ForzaMotorsport);

        public bool Initialize(IVAzhureRacingApp app)
        {
            app.RegisterGame(fh4);
            app.RegisterGame(fh5);
            app.RegisterGame(fh6);
            app.RegisterGame(fMS);
            return true;
        }

        public void Quit(IVAzhureRacingApp app)
        {
            fh4.OnQuit();
            fh5.OnQuit();
            fh6.OnQuit();
            fMS.OnQuit();
        }
    }

    public class ForayHorizonGame : VAzhureUDPClient, IGamePlugin
    {
        public enum FHVersion { FH4, FH5, FH6, ForzaMotorsport }

        // Static port tracking to prevent ConsoleMode conflicts
        // Only one instance can bind to a given port at a time
        private static readonly HashSet<int> s_boundPorts = new HashSet<int>();
        private static readonly object s_portLock = new object();

        public string Name
        {
            get
            {
                switch (m_version)
                {
                    default:
                    case FHVersion.FH4: return "Forza Horizon 4";
                    case FHVersion.FH5: return "Forza Horizon 5";
                    case FHVersion.FH6: return "Forza Horizon 6";
                    case FHVersion.ForzaMotorsport: return "Forza Motorsport 2023";
                }
            }
        }

        public uint SteamGameID
        {
            get
            {
                switch (m_version)
                {
                    case FHVersion.FH5: return 1551360U;
                    case FHVersion.FH6: return 2483190U;
                    case FHVersion.ForzaMotorsport: return 2440510U;
                    default:
                    case FHVersion.FH4: return 1293830U;
                }
            }
        }

        public string[] ExecutableProcessName
        {
            get
            {
                switch (m_version)
                {
                    default:
                    case FHVersion.FH4: return new string[] { "ForzaHorizon4" };
                    case FHVersion.FH5: return new string[] { "ForzaHorizon5" };
                    case FHVersion.FH6: return new string[] { "ForzaHorizon6" };
                    case FHVersion.ForzaMotorsport: return new string[] { "forza_gaming.desktop.x64_release_final" };
                }
            }
        }

        private string sIconPath = "";
        private string sGamePath = "";
        public string UserIconPath { get => sIconPath; set => sIconPath = value; }
        public string UserExecutablePath { get => sGamePath; set => sGamePath = value; }

        bool bIsRunning = false;
        public bool IsRunning => bIsRunning;

        public event EventHandler<TelemetryUpdatedEventArgs> OnTelemetry;
        public event EventHandler OnGameStateChanged;
        public event EventHandler OnGameIconChanged;

        public System.Drawing.Icon GetIcon()
        {
            switch (m_version)
            {
                default:
                case FHVersion.FH4: return Properties.Resources.ForzaHorizon4;
                case FHVersion.FH5: return Properties.Resources.ForzaHorizon5;
                case FHVersion.FH6: return Properties.Resources.ForzaHorizon6;
                case FHVersion.ForzaMotorsport: return Properties.Resources.ForzaMotosport;
            }
        }

        public void ShowSettings(IVAzhureRacingApp app)
        {

        }

        public void Start(IVAzhureRacingApp app)
        {
            if (settings.ConsoleMode)
            {
                app.SetStatusText("Console Mode: start the game on your console and configure Data Out to this PC's IP.");
                return;
            }

            if (settings.ExecutablePath != string.Empty)
                Utils.ExecuteCmd(settings.ExecutablePath);
            else
            {
                if (!Utils.RunSteamGame(SteamGameID))
                {
                    app.SetStatusText($"Steam Service is not running. Run {Name} manually!");
                }
            }
        }

        private readonly ProcessMonitor monitor;
        private GameSettings settings = new GameSettings();
        private bool m_bOwnsPort = false;

        private FHVersion m_version = FHVersion.FH4;
        public ForayHorizonGame(FHVersion version)
        {
            m_version = version;
            LoadSettings();

            if (settings.ConsoleMode)
            {
                // Console mode: only one instance per port to avoid conflicts
                lock (s_portLock)
                {
                    if (!s_boundPorts.Contains(settings.Port))
                    {
                        s_boundPorts.Add(settings.Port);
                        m_bOwnsPort = true;
                    }
                }
                if (m_bOwnsPort)
                {
                    // Listen immediately, auto-detect FH8 vs FHDash by packet size
                    Run(settings.Port);
                }
            }
            else
            {
                // PC mode: wait for the local game process
                monitor = new ProcessMonitor(ExecutableProcessName, 1000);
                monitor.OnProcessRunningStateChanged += delegate (object o, bool bRunning)
                {
                    bIsRunning = bRunning;
                    if (bRunning)
                        Run(settings.Port);
                    else
                        Stop();

                    OnGameStateChanged?.Invoke(this, new EventArgs());
                };
                monitor.Start();
            }
        }

        private void LoadSettings()
        {
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), $"{m_version.ToString()}.json");
            if (File.Exists(path))
            {
                try
                {
                    string json = File.ReadAllText(path);

                    settings = ObjectSerializeHelper.DeserializeJson<GameSettings>(json);
                }
                catch { }
            }
        }

        TelemetryDataSet mCarData = null;

        public override void OnDataReceived(ref byte[] bytes)
        {
            bool bHasData = false;

            mCarData = mCarData ?? new TelemetryDataSet(this);

            if (settings.ConsoleMode)
            {
                // ConsoleMode: auto-detect format by packet size
                // FH8 (Forza Motorsport 2023) is larger than FHDash (FH4/FH5/FH6)
                if (bytes.Length == FH8.nSize)
                    bHasData = ProcessFH8Data(bytes);
                else if (bytes.Length == FHDash.nSize)
                    bHasData = ProcessFHDashData(bytes);
            }
            else
            {
                // PC mode: use m_version (ProcessMonitor already ensures correct game)
                if (m_version == FHVersion.ForzaMotorsport)
                    bHasData = ProcessFH8Data(bytes);
                else if (bytes.Length >= FHDash.nSize)
                    bHasData = ProcessFHDashData(bytes);
            }

            if (bHasData)
                OnTelemetry?.Invoke(this, new TelemetryUpdatedEventArgs(mCarData));
        }

        /// <summary>
        /// Process Forza Motorsport 2023 (FH8) telemetry packet
        /// </summary>
        private bool ProcessFH8Data(byte[] bytes)
        {
            try
            {
                FH8 dash = Marshalizable<FH8>.FromBytes(bytes);
                AMCarData cardata = mCarData.CarData;
                AMMotionData motionData = mCarData.CarData.MotionData;
                AMSessionInfo session = mCarData.SessionInfo;

                cardata.RPM = dash.CurrentEngineRpm;
                cardata.MaxRPM = dash.EngineMaxRpm;
                cardata.Brake = dash.Brake / 255.0f;
                cardata.Throttle = dash.Accel / 255.0f;
                cardata.Clutch = dash.Clutch / 255.0f;
                cardata.Gear = dash.Gear == 0 ? (short)0 : dash.Gear < 10 ? (short)(dash.Gear + 1) : (short)1;
                cardata.Speed = dash.Speed * 3.6f;
                cardata.Steering = dash.Steer / 127.0f;
                cardata.Position = dash.RacePosition;
                cardata.Lap = dash.LapNumber + 1;
                cardata.FuelLevel = dash.Fuel * 100.0f; // Normalized 0.0-1.0 -> percent

                // No motion in menu
                motionData.Yaw = dash.IsRaceOn > 0 ? (float)(dash.Yaw / Math.PI) : 0;
                motionData.Pitch = dash.IsRaceOn > 0 ? (float)(-dash.Pitch / Math.PI) : 0;
                motionData.Roll = dash.IsRaceOn > 0 ? (float)(-dash.Roll / Math.PI) : 0;
                motionData.Surge = dash.IsRaceOn > 0 ? dash.AccelerationZ / 30f : 0;
                motionData.Sway = dash.IsRaceOn > 0 ? -dash.AccelerationX / 30f : 0;
                motionData.Heave = dash.IsRaceOn > 0 ? dash.AccelerationY / 30f : 0;

                session.Flag = dash.IsRaceOn > 0 ? "Green" : "";
                session.SessionState = dash.IsRaceOn > 0 ? "Race" : "";
                session.BestLapTime = (int)(dash.BestLap * 1000f);
                session.CurrentLapTime = (int)(dash.CurrentLap * 1000f);
                session.CurrentLapNumber = dash.LapNumber + 1;
                session.CurrentPosition = dash.RacePosition;

                if (CarList.CarsFH8.TryGetValue(dash.CarOrdinal, out string carname))
                    cardata.CarName = carname;
                else
                    cardata.CarName = "Unknown model";

                if (Tracks.TrackList.TryGetValue(dash.TrackOrdinal, out string trackname))
                    session.TrackName = trackname;
                else
                    session.TrackName = "Unknown track";

                if (Tracks.TrackLength.TryGetValue(dash.TrackOrdinal, out float tracklen))
                    session.TrackLength = tracklen * 1000f;
                else
                    session.TrackLength = 1;

                cardata.Distance = dash.DistanceTraveled;

                cardata.Tires = new AMTireData[]
                {
                    new AMTireData()
                    {
                        Pressure = 140,
                        Temperature = new double[]{ dash.TireTempFrontLeft, dash.TireTempFrontLeft,dash.TireTempFrontLeft,dash.TireTempFrontLeft },
                        BrakeTemperature = 100,
                        Detached = false,
                        Wear = dash.TireWearFrontLeft,
                    },
                    new AMTireData()
                    {
                        Pressure = 140,
                        Temperature = new double[]{ dash.TireTempFrontRight, dash.TireTempFrontRight, dash.TireTempFrontRight, dash.TireTempFrontRight },
                        BrakeTemperature = 100,
                        Detached = false,
                        Wear = dash.TireWearFrontRight,
                    },
                    new AMTireData()
                    {
                        Pressure = 140,
                        Temperature = new double[]{ dash.TireTempRearLeft, dash.TireTempRearLeft, dash.TireTempRearLeft, dash.TireTempRearLeft },
                        BrakeTemperature = 100,
                        Detached = false,
                        Wear = dash.TireWearRearLeft,
                    },
                    new AMTireData()
                    {
                        Pressure = 140,
                        Temperature = new double[]{ dash.TireTempRearRight, dash.TireTempRearRight, dash.TireTempRearRight, dash.TireTempRearRight },
                        BrakeTemperature = 100,
                        Detached = false,
                        Wear = dash.TireWearRearRight,
                    },
                };

                return true;
            }
            catch { }
            return false;
        }

        /// <summary>
        /// Process Forza Horizon (FH4/FH5/FH6) telemetry packet
        /// </summary>
        private bool ProcessFHDashData(byte[] bytes)
        {
            try
            {
                FHDash dash = Marshalizable<FHDash>.FromBytes(bytes);
                AMCarData cardata = mCarData.CarData;
                AMMotionData motionData = mCarData.CarData.MotionData;
                AMSessionInfo session = mCarData.SessionInfo;

                cardata.RPM = dash.CurrentEngineRpm;
                cardata.MaxRPM = dash.EngineMaxRpm;
                cardata.Brake = dash.Brake / 255.0f;
                cardata.Throttle = dash.Accel / 255.0f;
                cardata.Clutch = dash.Clutch / 255.0f;
                cardata.Gear = dash.Gear == 0 ? (short)0 : dash.Gear < 10 ?  (short)(dash.Gear + 1) : (short)1;
                cardata.Speed = dash.Speed * 3.6f;
                cardata.Steering = dash.Steer / 127.0f;
                cardata.Position = dash.RacePosition;
                cardata.Lap = dash.LapNumber;
                cardata.FuelLevel = dash.Fuel * 100.0f; // Normalized 0.0-1.0 -> percent

                // No motion in menu
                motionData.Yaw = dash.IsRaceOn > 0 ? (float)(dash.Yaw / Math.PI) : 0;
                motionData.Pitch = dash.IsRaceOn > 0 ? (float)(-dash.Pitch / Math.PI) : 0;
                motionData.Roll = dash.IsRaceOn > 0 ? (float)(-dash.Roll / Math.PI) : 0;
                motionData.Surge = dash.IsRaceOn > 0 ? dash.AccelerationZ / 30f : 0;
                motionData.Sway = dash.IsRaceOn > 0 ? -dash.AccelerationX / 30f : 0;
                motionData.Heave = dash.IsRaceOn > 0 ? dash.AccelerationY / 30f : 0;

                session.Flag = dash.IsRaceOn > 0 ? "Green" : "";
                session.SessionState = dash.IsRaceOn > 0 ? "Race" : "";
                session.BestLapTime = (int)(dash.BestLapTime * 1000f);
                session.CurrentLapTime = (int)(dash.CurrentLapTime * 1000f);
                session.CurrentLapNumber = dash.LapNumber;
                session.CurrentPosition = dash.RacePosition;

                // FH4/FH5 share car list; FH6 will use same ordinals initially
                if (CarList.Cars.TryGetValue(dash.CarOrdinal, out string carname))
                    cardata.CarName = carname;
                else
                    cardata.CarName = "Unknown model";

                cardata.Tires = new AMTireData[]
                {
                    new AMTireData()
                    {
                        Pressure = 140,
                        Temperature = new double[]{ dash.TireTempFrontLeft, dash.TireTempFrontLeft,dash.TireTempFrontLeft,dash.TireTempFrontLeft },
                        BrakeTemperature = 100,
                        Detached = false
                    },
                    new AMTireData()
                    {
                        Pressure = 140,
                        Temperature = new double[]{ dash.TireTempFrontRight, dash.TireTempFrontRight, dash.TireTempFrontRight, dash.TireTempFrontRight },
                        BrakeTemperature = 100,
                        Detached = false
                    },
                    new AMTireData()
                    {
                        Pressure = 140,
                        Temperature = new double[]{ dash.TireTempRearLeft, dash.TireTempRearLeft, dash.TireTempRearLeft, dash.TireTempRearLeft },
                        BrakeTemperature = 100,
                        Detached = false
                    },
                    new AMTireData()
                    {
                        Pressure = 140,
                        Temperature = new double[]{ dash.TireTempRearRight, dash.TireTempRearRight, dash.TireTempRearRight, dash.TireTempRearRight },
                        BrakeTemperature = 100,
                        Detached = false
                    },
                };

                return true;
            }
            catch { }
            return false;
        }

        internal void OnQuit()
        {
            monitor?.Stop();
            Stop();
            if (m_bOwnsPort)
            {
                lock (s_portLock)
                    s_boundPorts.Remove(settings.Port);
            }
            SaveSettings();
        }

        private void SaveSettings()
        {
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), $"{m_version.ToString()}.json");
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

    public class GameSettings
    {
        public int Port { get; set; } = 5200;
        public string ExecutablePath { get; set; } = string.Empty;
        public bool ConsoleMode { get; set; } = false;
    }
}