using System;
using System.Drawing;
using System.IO.MemoryMappedFiles;
using System.Threading;
using vAzhureRacingAPI;

namespace BusBound
{
    public class BusGame : IGamePlugin
    {
        public string Name => "Bus Bound";

        public uint SteamGameID => 2095420U;

        public string[] ExecutableProcessName => new [] { "BusGame" };

        private string sUserIcon = string.Empty;
        private string sExecutablePath = string.Empty;

        public string UserIconPath { get => sUserIcon; set => sUserIcon = value; }
        public string UserExecutablePath { get => sExecutablePath; set => sExecutablePath = value; }

        public bool IsRunning => Utils.IsProcessRunning(ExecutableProcessName);

        public event EventHandler<TelemetryUpdatedEventArgs> OnTelemetry;
        public event EventHandler OnGameStateChanged;
        public event EventHandler OnGameIconChanged;

        public Icon GetIcon()
        {
            return Properties.Resources.BusGame;
        }

        public void ShowSettings(IVAzhureRacingApp app)
        {
            // none
        }

        public void Start(IVAzhureRacingApp app)
        {
            if (!Utils.RunSteamGame(SteamGameID))
            {
                app.SetStatusText($"Steam Service is not running. Run {Name} manually!");
            }
        }

        private readonly ProcessMonitor monitor;
        private readonly TelemetryDataSet dataSet;
        private readonly CustomSharedMemClient customSharedMemClient;

        ~BusGame()
        {
            monitor.Stop();
            customSharedMemClient?.StopTrhead();
        }

        public BusGame()
        {
            dataSet = new TelemetryDataSet(this);

            monitor = new ProcessMonitor(ExecutableProcessName);

            customSharedMemClient = new CustomSharedMemClient();

            monitor.OnProcessRunningStateChanged += (process, bRunning) =>
            {
                if (bRunning)
                {
                    customSharedMemClient.StartThread();
                    customSharedMemClient.OnUserFunc += delegate (object sender, EventArgs ea)
                    {
                        ProcessSharedMemory();
                    };
                }
                else
                {
                    customSharedMemClient?.StopTrhead();
                    dataSet.LoadDefaults();
                    OnTelemetry?.Invoke(this, new TelemetryUpdatedEventArgs(dataSet));
                }
                OnGameStateChanged?.Invoke(this, EventArgs.Empty);
            };

            monitor.Start();
        }

        private readonly byte[] dataStatic = new byte[BusGameTelemetry.BusGameTelemetrySize];

        private void ProcessSharedMemory()
        {
            try
            {
                using (var mmf = MemoryMappedFile.OpenExisting(BusGameTelemetry.BUS_BOUND_SHARED_MEMORY_NAME, MemoryMappedFileRights.ReadWrite))
                using (var mmfView = mmf.CreateViewStream(0L, dataStatic.Length, MemoryMappedFileAccess.ReadWrite))
                {
                    mmfView.ReadAsync(dataStatic, 0, dataStatic.Length).Wait();

                    BusGameTelemetry.Game telemetry = Marshalizable<BusGameTelemetry.Game>.FromBytes(dataStatic);

                    if (telemetry.IsGameActive)
                    {
                        AMCarData carData = dataSet.CarData;

                        dataSet.CarData.DriverName = telemetry.PlayerData.Data.Name;
                        //switch (telemetry.Bus.Data.CurrentAutomaticGear)
                        //{
                        //    case BusGameTelemetry.Gear.Neutral: dataSet.CarData.Gear = 1; break;
                        //    case BusGameTelemetry.Gear.Reverse: dataSet.CarData.Gear = 0; break;
                        //    case BusGameTelemetry.Gear.Drive: dataSet.CarData.Gear = 2; break;
                        //}

                        if (telemetry.State == BusGameTelemetry.GameState.Drive)
                        {
                            carData.Gear = (short)(telemetry.Bus.Data.Engine.Data.Gear + 1);
                            carData.RPM = telemetry.Bus.Data.Engine.Data.RPM;
                            carData.MaxRPM = telemetry.Bus.Data.Engine.Data.MaxRPM;
                            carData.Speed = Math.Abs(telemetry.Bus.Data.SpeedCMS * 0.036f);
                            carData.Throttle = telemetry.Bus.Data.Throttle;
                            carData.Brake = telemetry.Bus.Data.Brake;
                            carData.Clutch = 1f;

                            carData.Electronics = CarElectronics.None;
                            dataSet.WeatherData.AmbientTemp = telemetry.Bus.Data.Temperature;

                            if (telemetry.Bus.Data.IsFarLightOn)
                                carData.Electronics |= CarElectronics.Headlight;

                            carData.DirectionsLight = DirectionsLight.None;
                            if (telemetry.Bus.Data.IndicatorLightState == BusGameTelemetry.IndicatorState.Left)
                                carData.DirectionsLight = DirectionsLight.Left;
                            if (telemetry.Bus.Data.IndicatorLightState == BusGameTelemetry.IndicatorState.Right)
                                carData.DirectionsLight = DirectionsLight.Right;

                            if (telemetry.Bus.Data.Engine.Data.Ignition == BusGameTelemetry.IgnitionState.PowerOn || telemetry.Bus.Data.Engine.Data.Ignition == BusGameTelemetry.IgnitionState.AllOn)
                                carData.Electronics |= CarElectronics.Ignition;

                            carData.MotionData = new AMMotionData()
                            {
                                Sway = -(float)telemetry.Bus.Data.LocalAcceleration.y / 1000f,
                                Surge = (float)telemetry.Bus.Data.LocalAcceleration.x / 1000f,
                                Heave = (float)telemetry.Bus.Data.LocalAcceleration.z / 1000f,
                                Yaw = (float)(telemetry.Bus.Data.Rotation.yaw / 180.0f),
                                Pitch = (float)(telemetry.Bus.Data.Rotation.pitch / 180.0f),
                                Roll = (float)(telemetry.Bus.Data.Rotation.roll / 180.0f),
                            };
                        }
                        else
                            carData = new AMCarData();

                        OnTelemetry?.Invoke(this, new TelemetryUpdatedEventArgs(dataSet));
                    }
                }
            }
            catch
            {
                AMMotionData data = new AMMotionData()
                {
                    Pitch = 0,
                    Roll = 0,
                    Yaw = 0,
                };

                dataSet.CarData.MotionData = data;
                OnTelemetry?.Invoke(this, new TelemetryUpdatedEventArgs(dataSet));
            }
        }
    }

    public class CustomSharedMemClient : VAzhureSharedMemoryClient
    {
        public event EventHandler OnUserFunc;
        public override void UserFunc()
        {
            OnUserFunc?.Invoke(this, EventArgs.Empty);
            Thread.Sleep(10);
        }
    }
}
