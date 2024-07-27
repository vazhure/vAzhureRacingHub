using System;
using System.Drawing;
using System.IO.MemoryMappedFiles;
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
                    case GameID.ETS2: return new string[] { "eurotrucks2" }; break;
                    case GameID.ATS: return new string[] { "amtrucks" }; break;
                }
            }
        }

        string m_iconPath = "";
        string m_exePath = "";

        public string UserIconPath { get => m_iconPath; set => m_iconPath = value; }
        public string UserExecutablePath { get => m_exePath; set => m_exePath = value; }

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
                case GameID.ETS2: return Properties.Resources.eurotrucks2; break;
                case GameID.ATS: return Properties.Resources.amtrucks; break;
            }
        }

        public void ShowSettings(IVAzhureRacingApp app)
        {

        }

        public void Start(IVAzhureRacingApp app)
        {
            if (!Utils.RunSteamGame(SteamGameID))
            {
                app.SetStatusText($"Steam Service is not running. Run {Name} manually!");
            }
        }

        private const string DefaultSharedMemoryMap = "Local\\ETS2";
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

                        session.SessionState = "Race";
                        session.Flag = "Green";
                        motion.Position = data.ws_truck_placement.position;

                        motion.Pitch = -data.ws_truck_placement.orientation.pitch / 0.25f;
                        motion.Roll = -data.ws_truck_placement.orientation.roll / 0.5f;
                        motion.Yaw = data.ws_truck_placement.orientation.heading * 2f - 1f;

                        car.Tires[0].Pressure = 175;
                        car.Tires[1].Pressure = 175;
                        car.Tires[2].Pressure = 175;
                        car.Tires[3].Pressure = 175;

                        motion.Surge = -data.linear_acceleration.z / 9.81f;
                        motion.Sway = -data.linear_acceleration.x / 9.81f;
                        motion.Heave = data.linear_acceleration.y / 9.81f;

                        motion.LocalVelocity = data.linear_velocity;

                        ///Console.WriteLine($"Surge {motion.Surge:N4}, Sway {motion.Sway:N4}, Heave {motion.Heave:N4}, Pitch {motion.Pitch:N4}, Roll {motion.Roll:N4}, Yaw {motion.Yaw:N4}");

                        if (data.lowBeamLight || data.hiBeamLight)
                            car.Electronics |= CarElectronics.Headlight;
                        else
                            car.Electronics &= ~CarElectronics.Headlight;

                        car.DirectionsLight = (data.lblinker ? DirectionsLight.Left : DirectionsLight.None) |
                            (data.rblinker ? DirectionsLight.Right : DirectionsLight.None);

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
        }
    }
}