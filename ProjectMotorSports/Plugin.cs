using System;
using System.Drawing;
using vAzhureRacingAPI;

namespace ProjectMotorSports
{
    public class Plugin : ICustomPlugin
    {
        private readonly Game game = new Game();

        public string Name => "Project Motor Racing Plugin";

        public string Description => "Project Motor Racing Plugin";

        public ulong Version => 1UL;

        public bool CanClose(IVAzhureRacingApp app)
        {
            return true;
        }

        public bool Initialize(IVAzhureRacingApp app)
        {
            try
            {
                app.RegisterGame(game);
            }
            catch
            {
                return false;
            }
            
            return true;
        }

        public void Quit(IVAzhureRacingApp app)
        {
            game.Stop();
        }
    }

    public class Game : IGamePlugin
    {
        public string Name => "Project Motor Racing";

        public uint SteamGameID => 299970U;

        public string[] ExecutableProcessName => new string[] { "ProjectMotorRacing" };

        string sUserIconPath = "";
        string sUserExecutablePath = "";

        public string UserIconPath { get => sUserIconPath; set => sUserIconPath = value; }
        public string UserExecutablePath { get => sUserExecutablePath; set => sUserExecutablePath = value; }

        public bool IsRunning => _bRunning;

        public event EventHandler<TelemetryUpdatedEventArgs> OnTelemetry;
        public event EventHandler OnGameStateChanged;
        public event EventHandler OnGameIconChanged;

        private bool _bRunning = false;

        public TelemetryDataSet TelemetryData { get; }

        private readonly ProcessMonitor monitor;
        private readonly UDPThread udpThread = null;

        public Game()
        {
            TelemetryData = new TelemetryDataSet(this);
            monitor = new ProcessMonitor(ExecutableProcessName);
            udpThread = new UDPThread(new string[] { });

            udpThread.OnRaceInfo += (_, raceinfo) =>
            {
                AMSessionInfo sessionInfo = TelemetryData.SessionInfo;

                sessionInfo.DriversCount = raceinfo.m_numParticipants;
                sessionInfo.TrackConfig = raceinfo.m_layout;
                sessionInfo.TrackName = raceinfo.m_track;
                sessionInfo.TotalLapsCount = raceinfo.m_isLaps ? (int)raceinfo.m_duration : -1;
                sessionInfo.RemainingTime = raceinfo.m_isLaps ?  -1 : (int)raceinfo.m_duration;
                sessionInfo.TrackLength = raceinfo.m_layoutLength;

                AMWeatherData weatherData = TelemetryData.WeatherData;
                weatherData.AmbientTemp = raceinfo.m_ambientTemperature;
                weatherData.TrackTemp = raceinfo.m_trackTemperature;

                switch (raceinfo.m_state)
                {
                    case UDPRaceSessionState.Active:
                        {
                            sessionInfo.SessionState = "Racing";
                        }break;
                    case UDPRaceSessionState.Complete:
                        {
                            sessionInfo.SessionState = "Finish";
                        }
                        break;
                    default:
                        {
                            sessionInfo.SessionState = "";
                        }
                        break;
                }

                OnTelemetry?.Invoke(this, new TelemetryUpdatedEventArgs(TelemetryData));
            };

            udpThread.OnParticipantRaceState += (_, participant) =>
            {
                AMSessionInfo sessionInfo = TelemetryData.SessionInfo;
                AMCarData carData = TelemetryData.CarData;

                sessionInfo.CurrentPosition = participant.m_racePos;
                sessionInfo.CurrentLapNumber = participant.m_currentLap;
                sessionInfo.Sector = participant.m_currentSector;
                sessionInfo.CurrentLapTime = (int)participant.m_currentLapTime;
                sessionInfo.CurrentLapTime = (int)participant.m_bestLapTime;
                sessionInfo.CurrentLapNumber = participant.m_currentLap;

                sessionInfo.CurrentSector1Time = (int)participant.m_sectorTimes[0];
                sessionInfo.CurrentSector2Time = (int)participant.m_sectorTimes[1];
                sessionInfo.CurrentSector3Time = (int)participant.m_sectorTimes[2];
                sessionInfo.Sector1BestTime = (int)participant.m_bestSectorTimes[0];
                sessionInfo.Sector2BestTime = (int)participant.m_bestSectorTimes[1];
                sessionInfo.Sector3BestTime = (int)participant.m_bestSectorTimes[2];

                carData.InPits = participant.m_inPits;
                carData.CarName = participant.m_vehicleName;
                carData.DriverName = participant.m_driverName;
                carData.CarNumber = participant.m_vehicleId.ToString();

                OnTelemetry?.Invoke(this, new TelemetryUpdatedEventArgs(TelemetryData));
            };

            udpThread.OnVehicleTelemetry += (_, vechicle) =>
            {
                AMCarData carData = TelemetryData.CarData;
                AMMotionData motionData = TelemetryData.CarData.MotionData;

                int gear = vechicle.m_input.m_gear;

                carData.Steering = vechicle.m_input.m_steering;
                carData.Throttle = vechicle.m_input.m_accelerator;
                carData.Brake = vechicle.m_input.m_brake;
                carData.Clutch = vechicle.m_input.m_clutch;
                carData.Gear = (short)gear;
                carData.BrakeBias = vechicle.m_setup.m_brakeBias;
                carData.Speed = vechicle.m_chassis.m_forwardSpeed;

                try
                {
                    carData.ShiftUpRPM = vechicle.m_drivetrain.m_gears[gear].m_upshiftRPM;
                }
                catch { }

                carData.RPM = Math.Abs(vechicle.m_drivetrain.m_engineRPM);
                carData.MaxRPM = vechicle.m_constant.m_engineMaxRPM;
                carData.FuelCapacity = vechicle.m_constant.m_fuelCapacity;
                carData.FuelLevel = vechicle.m_drivetrain.m_fuelRemaining;
                carData.FuelConsumptionPerLap = vechicle.m_drivetrain.m_fuelUseRate;
                carData.Electronics = vechicle.m_drivetrain.m_speedLimiterActive ? CarElectronics.Limiter : CarElectronics.None;
                carData.Electronics|= vechicle.m_drivetrain.m_engineRunning ? CarElectronics.None : CarElectronics.Ignition;

                vechicle.m_chassis.m_quat.GetYawPitchRoll(out float yaw, out float pitch, out float roll);

                motionData.Pitch = 0.5f * pitch / (float)Math.PI;
                motionData.Yaw = yaw / (float)Math.PI;
                motionData.Roll = 0.5f * roll / (float)Math.PI;

                motionData.Sway =  vechicle.m_chassis.m_accelerationLS.x / 30f;
                motionData.Heave = vechicle.m_chassis.m_accelerationLS.y / 30f;
                motionData.Surge = vechicle.m_chassis.m_accelerationLS.z / 30f;

                OnTelemetry?.Invoke(this, new TelemetryUpdatedEventArgs(TelemetryData)); 
            };

            monitor.OnProcessRunningStateChanged += (_, bRunning) =>
            {
                _bRunning = bRunning;

                if (bRunning)
                {
                    TelemetryData?.LoadDefaults();
                    udpThread?.StopThread();
                    udpThread.StartThread();
                }
                else
                {
                    udpThread?.StopThread();
                    TelemetryData?.LoadDefaults();
                    OnTelemetry?.Invoke(this, new TelemetryUpdatedEventArgs(TelemetryData));
                }

                OnGameStateChanged?.Invoke(this, new EventArgs());
            };

            monitor.Start();
        }

        public Icon GetIcon()
        {
            return Properties.Resources.pmr;
        }

        public void ShowSettings(IVAzhureRacingApp app)
        {
            // TODO:
        }

        public void Start(IVAzhureRacingApp app)
        {
            if (!Utils.RunSteamGame(SteamGameID))
            {
                app.SetStatusText($"Steam Service is not running. Run {Name} manually!");
            }
        }

        internal void Stop()
        {
            udpThread?.StopThread();
            udpThread?.Dispose();
        }
    }
}