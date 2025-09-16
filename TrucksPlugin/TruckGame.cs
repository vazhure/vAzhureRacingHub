using System;
using System.Drawing;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using vAzhureRacingAPI;

namespace TrucksPlugin
{
    public class TruckGame : VAzhureSharedMemoryClient, IGamePlugin, IDisposable
    {
        public enum GameID : uint { ETS2 = 227300U, ATS = 270880U };

        readonly GameID m_game;
        readonly ProcessMonitor monitor;
        private readonly byte[] viewData = new byte[Marshal.SizeOf(typeof(ETS2_minimal))];
        readonly TelemetryDataSet telemetryDataSet;

        public TruckGame(GameID gameID)
        {
            m_game = gameID;

            LoadSettings();

            telemetryDataSet = new TelemetryDataSet(this);

            monitor = new ProcessMonitor(ExecutableProcessName, 1000);
            monitor.OnProcessRunningStateChanged += delegate (object o, bool bRunning)
            {
                bIsRunning = bRunning;
                if (bRunning)
                    StartThread();
                else
                {
                    StopTrhead();
                    telemetryDataSet.LoadDefaults();
                    OnTelemetry?.Invoke(this, new TelemetryUpdatedEventArgs(telemetryDataSet));
                }

                OnGameStateChanged?.Invoke(this, new EventArgs());
            };
            monitor.Start();

            if (Utils.IsProcessRunning(ExecutableProcessName))
                StartThread();
        }

        public string Name => Enum.GetName(typeof(GameID), m_game);

        public uint SteamGameID => (uint)m_game;

        public string[] ExecutableProcessName
        {
            get
            {
                switch (m_game)
                {
                    default:
                    case GameID.ETS2: return new string[] { "eurotrucks2" };
                    case GameID.ATS: return new string[] { "amtrucks" };
                }
            }
        }

        private GameSettings settings = new GameSettings();

        private void LoadSettings()
        {
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), $"{Name}.json");
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

        public string UserIconPath { get => settings.ExecutableIcon; set => settings.ExecutableIcon = value; }
        public string UserExecutablePath { get => settings.ExecutableLink; set => settings.ExecutableLink = value; }

        bool bIsRunning = false;

        public bool IsRunning => bIsRunning;

        public event EventHandler<TelemetryUpdatedEventArgs> OnTelemetry;
        public event EventHandler OnGameStateChanged;
        public event EventHandler OnGameIconChanged;

        public Icon GetIcon()
        {
            switch (m_game)
            {
                default:
                case GameID.ETS2: return Properties.Resources.eurotrucks2;
                case GameID.ATS: return Properties.Resources.amtrucks;
            }
        }

        readonly string assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public void ShowSettings(IVAzhureRacingApp app)
        {
            {
                try
                {
                    string args = string.Format($"/root, \"{assemblyPath}\"");
                    System.Diagnostics.Process.Start("explorer.exe", args);
                }
                catch { }
            }
        }

        public void Start(IVAzhureRacingApp app)
        {
            if (settings.ExecutableLink != string.Empty)
                Utils.ExecuteCmd(settings.ExecutableLink);
            else
            {
                if (!Utils.RunSteamGame(SteamGameID))
                {
                    app.SetStatusText($"Steam Service is not running. Run {Name} manually!");
                }
            }
        }

        private const string DefaultSharedMemoryMap = @"Local\ETS2";
        public override void UserFunc()
        {
            try
            {
                using (var mappedFile = MemoryMappedFile.OpenExisting(DefaultSharedMemoryMap, MemoryMappedFileRights.ReadWrite))
                using (var reader = mappedFile.CreateViewStream(0L, viewData.Length, MemoryMappedFileAccess.ReadWrite))
                {
                    reader.ReadAsync(viewData, 0, viewData.Length);

                    var data = Marshalizable<ETS2_minimal>.FromBytes(viewData);

                    if (data.running)
                    {
                        var car = telemetryDataSet.CarData;
                        var motion = telemetryDataSet.CarData.MotionData;
                        var session = telemetryDataSet.SessionInfo;
                        var userData = telemetryDataSet.UserData;

                        car.Gear = (short)(data.displayedGear < 0 ? 0 : data.displayedGear + 1);
                        car.Speed = Math.Abs(data.speedometer_speed * 3.6);
                        car.RPM = data.rpm;
                        car.MaxRPM = data.rpmMax;
                        car.Throttle = data.throttle;
                        car.Steering = data.steering;
                        car.Brake = data.brake;
                        car.Clutch = data.clutch;
                        car.FuelLevel = data.fuel;
                        car.FuelCapacity = data.fuelCapacity;
                        car.FuelConsumptionPerLap = data.truckFuelConsumptionAverageLiters;

                        userData["int.gear"] = data.gear;
                        userData["uint.gameTime"] = data.gameTime;
                        userData["uint.restStop"] = data.restStop;
                        userData["uint.deliveryTime"] = data.deliveryTime;
                        userData["uint.navigationTime"] = data.navigationTime;
                        userData["string.destinationCity"] = data.destinationCity;
                        userData["string.sourceCity"] = data.sourceCity;
                        userData["string.cargo"] = data.cargo;
                        userData["float.truckFuelConsumptionAverageLiters"] = data.truckFuelConsumptionAverageLiters;
                        userData["bool.truckElectricEnabled"] = data.truckElectricEnabled;
                        userData["bool.truckEngineEnabled"] = data.truckEngineEnabled;
                        userData["bool.parkingLights"] = data.parkingLights;
                        userData["bool.lowBeamLight"] = data.lowBeamLight;
                        userData["bool.hiBeamLight"] = data.hiBeamLight;
                        userData["bool.truckBrakeParking"] = data.truckBrakeParking;
                        userData["float.truckFuelRangeKm"] = data.truckFuelRangeKm;
                        userData["float.adBlueFuelCapacity"] = data.adBlueFuelCapacity;
                        userData["float.truckAdblueFuelLevelLiters"] = data.truckAdblueFuelLevelLiters;
                        userData["float.truckBatteryVoltage"] = data.truckBatteryVoltage;
                        userData["float.truckOdometerKM"] = data.truckOdometerKM;
                        userData["float.truckNavigationDistanceMeters"] = data.truckNavigationDistanceMeters;
                        userData["float.truckCruise_controlSpeedMS"] = data.truckCruise_controlSpeedMS;

                        session.PitSpeedLimit = data.truckNavigationSpeedLimitMS * 3.6;
                        session.TrackName = $"{data.sourceCity} - {data.destinationCity}";
                        session.TrackConfig = data.cargo;
                        session.RemainingTime = (int)((data.deliveryTime - data.gameTime) % 1440) * 1000; // limited by 24 hours

                        session.SessionState = "Race";
                        session.CurrentLapTime = (int)(data.gameTime % 1440) * 1000; // limited by 24 hours
                        session.Flag = "Green";
                        motion.Position = data.ws_truck_placement.position;

                        motion.Pitch = data.ws_truck_placement.orientation.pitch / 0.5f; // [-180, 180]
                        motion.Roll = -data.ws_truck_placement.orientation.roll / 0.5f; // [-180, 180]
                        motion.Yaw = data.ws_truck_placement.orientation.heading * 2f - 1f;

                        car.Tires[0].Pressure = 175;
                        car.Tires[1].Pressure = 175;
                        car.Tires[2].Pressure = 175;
                        car.Tires[3].Pressure = 175;

                        car.OilPressure = data.truckOilPressure;
                        car.OilTemp = data.truckOilTemperature;
                        car.WaterTemp = data.truckWaterTemperature;

                        motion.Surge = -data.linear_acceleration.z / 9.81f;
                        motion.Sway = -data.linear_acceleration.x / 9.81f;
                        motion.Heave = data.linear_acceleration.y / 9.81f;

                        motion.LocalVelocity = data.linear_velocity;

                        ///Console.WriteLine($"Surge {motion.Surge:N4}, Sway {motion.Sway:N4}, Heave {motion.Heave:N4}, Pitch {motion.Pitch:N4}, Roll {motion.Roll:N4}, Yaw {motion.Yaw:N4}");

                        if (data.lowBeamLight)
                            car.Electronics |= CarElectronics.Headlight;
                        else
                            car.Electronics &= ~CarElectronics.Headlight;

                        if (data.truckWipers)
                            car.Electronics |= CarElectronics.WipersOn;
                        else
                            car.Electronics &= ~CarElectronics.WipersOn;

                        if (data.truckEngineEnabled)
                            car.Electronics |= CarElectronics.Ignition;
                        else
                            car.Electronics &= ~CarElectronics.Ignition;

                        if (data.truckBrakeParking)
                            car.Electronics |= CarElectronics.Handbrake;
                        else
                            car.Electronics &= ~CarElectronics.Handbrake;

                        car.DirectionsLight = (data.lblinker ? DirectionsLight.Left : DirectionsLight.None) |
                            (data.rblinker ? DirectionsLight.Right : DirectionsLight.None) |
                            (data.truckHazardWarning ? DirectionsLight.Booth : DirectionsLight.None);

                        OnTelemetry?.Invoke(this, new TelemetryUpdatedEventArgs(telemetryDataSet));
                    }
                }
            }
            catch { }
            Thread.Sleep(10);
        }

        public void Dispose()
        {
            StopTrhead();
            monitor.Stop();
            SaveSettings();
        }

        public class GameSettings
        {
            public string ExecutableLink { get; set; } = string.Empty;
            public string ExecutableIcon { get; set; } = string.Empty;
            public string ProcessName { get; set; } = string.Empty;
        }
    }
}