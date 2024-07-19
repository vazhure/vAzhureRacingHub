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
        private readonly byte[] viewData = new byte[Marshal.SizeOf(typeof(ETS2_Struct))];
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
            Utils.RunSteamGame((uint)m_game);
        }

        private const string DefaultSharedMemoryMap = "Local\\Ets2TelemetryServer";

        public override void UserFunc()
        {
            try
            {
                using (var mappedFile = MemoryMappedFile.OpenExisting(DefaultSharedMemoryMap, MemoryMappedFileRights.ReadWrite))
                using (var reader = mappedFile.CreateViewStream(0L, viewData.Length, MemoryMappedFileAccess.ReadWrite))
                {
                    reader.ReadAsync(viewData, 0, viewData.Length);

                    var data = Marshalizable<ETS2_Struct>.FromBytes(viewData);
                    var car = telemetryDataSet.CarData;
                    var motion = telemetryDataSet.CarData.MotionData;
                    var session = telemetryDataSet.SessionInfo;

                    car.Gear = (short)(data.displayedGear < 0 ? 0 : data.displayedGear + 1);
                    car.Speed = Math.Abs(data.speed * 3.6);
                    car.RPM = data.engineRpm;
                    car.MaxRPM = data.engineRpmMax;
                    car.Throttle = data.userThrottle;
                    car.Steering = data.userSteer;
                    car.Brake = data.userBrake;
                    car.Clutch = data.userClutch;

                    motion.Position = new double[] { data.coordinateX, data.coordinateY, data.coordinateZ };

                    // TODO: 
                    motion.Pitch = 0; //-data.rotationZ / (float)Math.PI;
                    motion.Roll = 0; //-data.rotationX / (float)Math.PI;

                    motion.Surge = -data.accelerationZ / 9.81f;
                    motion.Sway = -data.accelerationX / 9.81f;
                    motion.Heave = data.accelerationY / 9.81f;

                    // Not sending data?
                    if (data.lightsBeamLow || data.lightsBeamHigh)
                        car.Electronics |= CarElectronics.Headlight;
                    else
                        car.Electronics &= ~CarElectronics.Headlight;

                    if (data.wipers)
                        car.Electronics |= CarElectronics.WipersOn;
                    else
                        car.Electronics &= ~CarElectronics.WipersOn;

                    car.DirectionsLight = (data.lightsParking ? DirectionsLight.Booth : DirectionsLight.None) |
                        (data.blinkerLeftActive ? DirectionsLight.Left : DirectionsLight.None) |
                        (data.blinkerRightActive ? DirectionsLight.Right : DirectionsLight.None);

                    OnTelemetry?.Invoke(this, new TelemetryUpdatedEventArgs(telemetryDataSet));
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