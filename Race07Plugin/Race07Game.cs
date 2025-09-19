using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using vAzhureRacingAPI;

namespace Race07Plugin
{
    public class Race07Game : VAzhureSharedMemoryClient, IGamePlugin
    {
        private readonly bool m_gtr = false;

        private Settings settings = new Settings();
        private readonly ProcessMonitor monitor;
        private bool bIsRunning = false;

        private readonly byte[] viewData;
        readonly TelemetryDataSet telemetry;

        public Race07Game(bool gtr)
        {
            m_gtr = gtr;
            telemetry = new TelemetryDataSet(this);

            LoadSettings();

            viewData = new byte[viewSize];

            monitor = new ProcessMonitor(ExecutableProcessName, 1000);
            monitor.OnProcessRunningStateChanged += delegate (object o, bool bRunning)
            {
                bIsRunning = bRunning;
                if (bRunning)
                    StartThread();
                else
                    StopTrhead();

                OnGameStateChanged?.Invoke(this, new EventArgs());
            };
            monitor.Start();
        }

        private void LoadSettings()
        {
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), $"{Name}.json");
            if (File.Exists(path))
            {
                try
                {
                    string json = File.ReadAllText(path);

                    settings = ObjectSerializeHelper.DeserializeJson<Settings>(json);
                }
                catch { }
            }
        }

        readonly int viewSize = Marshal.SizeOf(typeof(DataStruct));

        public override void UserFunc()
        {
            try
            {
                using (var mappedFile = MemoryMappedFile.OpenExisting(m_gtr ? "$gtr2$" : "$Race$", MemoryMappedFileRights.ReadWrite))
                using (var reader = mappedFile.CreateViewStream(0L, viewData.Length, MemoryMappedFileAccess.ReadWrite))
                {
                    reader.ReadAsync(viewData, 0, viewData.Length).Wait();

                    var data = Marshalizable<DataStruct>.FromBytes(viewData);
                    var carData = telemetry.CarData;
                    var motionData = carData.MotionData;
                    var session = telemetry.SessionInfo;

                    int time = (int)(data.lapTimeCurrent * 1000f);
                    carData.Flags = (time > 0 && session.CurrentLapTime != time) ? TelemetryFlags.FlagGreen : data.position > 0 ? TelemetryFlags.FlagChequered : TelemetryFlags.FlagNone;

                    carData.Gear = (short)(data.gear == -2 ? 1 : (data.gear + 1));
                    carData.Speed = data.carSpeed * 3.6f;
                    carData.Position = data.position;
                    carData.Lap = data.completedLaps + 1;
                    carData.MaxRPM = data.maxEngineRPS > 0 ? data.maxEngineRPS * (60.0 / (2.0 * (float)Math.PI)) : 0;
                    carData.RPM = data.rpm > 0 ? data.rpm * 10.0 : 0;
                    carData.OilTemp = data.engineOilTemp;
                    carData.OilPressure = data.engineOilPressure;
                    carData.WaterTemp = data.engineWaterTemp;
                    carData.FuelCapacity = data.fuelCapacityLiters;
                    carData.FuelLevel = data.fuel;

                    carData.Tires[0].Temperature = new double[] { data.tirefrontleft[0], data.tirefrontleft[1], data.tirefrontleft[2] };
                    carData.Tires[1].Temperature = new double[] { data.tirefrontright[0], data.tirefrontright[1], data.tirefrontright[2] };
                    carData.Tires[2].Temperature = new double[] { data.tirerearleft[0], data.tirerearleft[1], data.tirerearleft[2] };
                    carData.Tires[3].Temperature = new double[] { data.tirerearright[0], data.tirerearright[1], data.tirerearright[2] };

                    carData.Tires[0].Pressure = 140;
                    carData.Tires[1].Pressure = 140;
                    carData.Tires[2].Pressure = 140;
                    carData.Tires[3].Pressure = 140;

                    // GTR 2 Pitch/Roll/Yaw values are incorrect
                    motionData.Pitch = m_gtr ? 0 : (float)(-data.pitch / Math.PI);
                    motionData.Roll = m_gtr ? 0 : (float)(data.roll / Math.PI);
                    motionData.Yaw = m_gtr ? 0 : (float)(data.yaw / Math.PI);

                    motionData.Position = new double[] { data.carCGLoc[0], data.carCGLoc[1], data.carCGLoc[2] };

                    motionData.Surge = -data.longintitudinal / 30f;
                    motionData.Sway = data.lateral / 30f;
                    motionData.Heave = data.vertical / 30f;

                    session.CurrentPosition = carData.Position;
                    session.SessionState = data.numCars == 1 ? "Practice" : data.numCars > 0 ? "Race" : "";
                    session.DriversCount = data.numCars;
                    session.TotalLapsCount = data.numberOfLaps;
                    session.RemainingLaps = data.numberOfLaps ==-1 ? -1 : data.numberOfLaps - data.completedLaps;
                    session.CurrentLapNumber = data.completedLaps == -1 ? -1 : data.completedLaps + 1;
                    session.CurrentLapTime = time < 0 ? -1 : time;
                    session.LastLapTime = data.lapTimePrevious < 0 ? -1 : (int)(data.lapTimePrevious * 1000f);
                    session.BestLapTime = data.lapTimeBest < 0 ? -1 : (int)(data.lapTimeBest * 1000f);

                    switch (carData.Flags)
                    {
                        default:
                        case TelemetryFlags.FlagNone: session.Flag = ""; break;
                        case TelemetryFlags.FlagGreen: session.Flag = "Green"; break;
                        case TelemetryFlags.FlagChequered: session.Flag = "Chequered"; break;
                    }

                    OnTelemetry?.Invoke(this, new TelemetryUpdatedEventArgs(telemetry));
                }
            }
            catch { }

            Thread.Sleep(10); // 100 FPS
        }

        public string Name => m_gtr ? "GTR 2" : "Race 07";

        public uint SteamGameID => m_gtr ? 8790u : 8600u;

        public string[] ExecutableProcessName => m_gtr ? new string[] { "gtr2" } : new string[] { "Race_Steam" };

        public string UserIconPath { get => settings.GameIcon; set => settings.GameIcon = value; }
        public string UserExecutablePath { get => settings.ExecutabeLink; set => settings.ExecutabeLink = value; }

        public bool IsRunning => bIsRunning;

        public event EventHandler<TelemetryUpdatedEventArgs> OnTelemetry;
        public event EventHandler OnGameStateChanged;
        public event EventHandler OnGameIconChanged;

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

        public System.Drawing.Icon GetIcon()
        {
            return m_gtr ? Properties.Resources.GTR2 : Properties.Resources.Race07;
        }

        public void Quit()
        {
            StopTrhead();
            SaveSettings();
        }

        private void SaveSettings()
        {
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), $"{Name}.json");
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

        public class Settings
        {
            public string ProcessName { get; set; } = "";
            public string ExecutabeLink { get; set; } = "";
            public string GameIcon { get; set; } = "";
        }
    }
}