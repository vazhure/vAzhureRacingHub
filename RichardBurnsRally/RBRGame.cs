using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using vAzhureRacingAPI;

namespace RichardBurnsRally
{
    public class RBRGame : IGamePlugin
    {
        public string Name => "Richard Burns Rally";

        public uint SteamGameID => 0;

        public string[] ExecutableProcessName => new string[] { "RichardBurnsRally_SSE" };

        readonly Icon gameIcon = Properties.Resources.RichardBurnsRally;
        string sCustomGameIcon = "";
        string sExecutablePath = @"C:\Richard Burns Rally\RichardBurnsRally_SSE.exe";

        public string UserIconPath { get => sCustomGameIcon; set => sCustomGameIcon = value; }
        public string UserExecutablePath { get => sExecutablePath; set => sExecutablePath = value; }

        private bool bIsRunning = false;
        public bool IsRunning => bIsRunning;

        public object MessageBox { get; private set; }

        public event EventHandler<TelemetryUpdatedEventArgs> OnTelemetry;
        public event EventHandler OnGameStateChanged;
        public event EventHandler OnGameIconChanged;

        readonly ProcessMonitor monitor;

        TelemetryDataSet dataSet = null;
        readonly VAzhureUDPClient uDPClient = new VAzhureUDPClient();
        readonly int udpPort = 6776;

        // TODO: Settings class

        public RBRGame()
        {
            uDPClient.OnDataReceivedEvent += UDPClient_OnDataReceivedEvent;
            uDPClient.OnTimeout += UDPClient_OnTimeout;

            monitor = new ProcessMonitor(ExecutableProcessName);
            monitor.OnProcessRunningStateChanged += delegate (object o, bool bRunning)
            {
                bIsRunning = bRunning;
                if (bRunning)
                {
                    dataSet = new TelemetryDataSet(this);
                    uDPClient.Run(udpPort, 5000);
                }
                else
                {
                    uDPClient.Stop();
                }
                OnGameStateChanged?.Invoke(this, new EventArgs());
            };
            monitor.Start();
        }

        private void UDPClient_OnTimeout(object sender, EventArgs e)
        {
            dataSet = new TelemetryDataSet(this);
            OnTelemetry?.Invoke(this, new TelemetryUpdatedEventArgs(dataSet));
        }

        double rmpMax = 0;

        private void UDPClient_OnDataReceivedEvent(object sender, byte[] bytes)
        {
            if (bytes.Length == Marshal.SizeOf(typeof(TelemetryData)))
            {
                TelemetryData data = Marshalizable<TelemetryData>.FromBytes(bytes);

                AMCarData carData = dataSet.CarData;
                AMSessionInfo sessionInfo = dataSet.SessionInfo;
                AMWeatherData aMWeatherData = dataSet.WeatherData;
                AMMotionData aMMotionData = dataSet.CarData.MotionData;

                carData.Gear = (short)data.control.gear;
                carData.Brake = data.control.brake;
                carData.Clutch = data.control.clutch;
                carData.Throttle = data.control.throttle;
                carData.Steering = data.control.steering;
                carData.Speed = data.car.speed;
                carData.Distance = data.stage.progress;

                aMMotionData.Pitch = data.car.pitch / 180f;
                aMMotionData.Roll = data.car.roll / 180f;
                aMMotionData.Yaw = data.car.yaw / 180f;
                aMMotionData.Sway = -data.car.accelerations.sway / 9.81f;
                aMMotionData.Surge = data.car.accelerations.surge / 9.81f;
                aMMotionData.Heave = data.car.accelerations.heave / 9.81f;
                aMMotionData.Position = new double[] { data.car.positionX, data.car.positionY, data.car.positionZ };

                carData.RPM = Math.Abs(data.car.engine.rpm);
                rmpMax = Math.Max(carData.RPM, rmpMax);
                carData.MaxRPM = rmpMax;
                carData.ShiftUpRPM = rmpMax * 0.9;
                carData.WaterTemp = data.car.engine.engineCoolantTemperature;
                carData.OilTemp = data.car.engine.engineTemperature;

                sessionInfo.SessionState = "Race";
                sessionInfo.Flag = "Green";
                sessionInfo.TotalLapsCount = 1;
                sessionInfo.CurrentLapTime = (int)(data.stage.raceTime * 1000f);

                OnTelemetry?.Invoke(this, new TelemetryUpdatedEventArgs(dataSet));
            }
        }

        public void OnQuit()
        {
            dataSet = new TelemetryDataSet(this);
            OnTelemetry?.Invoke(this, new TelemetryUpdatedEventArgs(dataSet));
            monitor?.Stop();
        }

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
            if (File.Exists(sExecutablePath))
                Utils.ExecuteCmd(sExecutablePath);
            else
                ShowSettings(app);
        }
    }
}