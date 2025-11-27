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

        public string[] ExecutableProcessName => new string[] { "ProjectMotorRacingGame" };

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

        int m_PlayerIdx = 0;

        public Game()
        {
            TelemetryData = new TelemetryDataSet(this);
            monitor = new ProcessMonitor(ExecutableProcessName);
            udpThread = new UDPThread();

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
                            sessionInfo.SessionState = raceinfo.m_session.ToUpper();
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
                if (participant.m_isPlayer)
                {
                    m_PlayerIdx = participant.m_vehicleId;
                    AMSessionInfo sessionInfo = TelemetryData.SessionInfo;
                    AMCarData carData = TelemetryData.CarData;

                    sessionInfo.CurrentPosition = participant.m_racePos;
                    sessionInfo.CurrentLapNumber = participant.m_currentLap;
                    sessionInfo.Sector = participant.m_currentSector;
                    sessionInfo.CurrentLapTime = (int)participant.m_currentLapTime;
                    sessionInfo.CurrentLapTime = (int)participant.m_bestLapTime;
                    sessionInfo.CurrentLapNumber = participant.m_currentLap;

                    if (participant.m_sectorTimes.Count == 2)
                    {
                        sessionInfo.CurrentSector1Time = (int)participant.m_sectorTimes[0];
                        sessionInfo.CurrentSector2Time = (int)participant.m_sectorTimes[1];
                        sessionInfo.CurrentSector3Time = (int)participant.m_sectorTimes[2];
                        sessionInfo.Sector1BestTime = (int)participant.m_bestSectorTimes[0];
                        sessionInfo.Sector2BestTime = (int)participant.m_bestSectorTimes[1];
                        sessionInfo.Sector3BestTime = (int)participant.m_bestSectorTimes[2];
                    }

                    carData.InPits = participant.m_inPits;
                    carData.CarName = participant.m_vehicleName;
                    carData.DriverName = participant.m_driverName;
                    carData.CarNumber = participant.m_vehicleId.ToString();
                    carData.Lap = participant.m_currentLap;
                    carData.Position = participant.m_racePos;
                    carData.Distance = participant.m_lapProgress;

                    sessionInfo.Flag = participant.m_dq ? "DQ" : sessionInfo.Flag;

                    carData.Flags = participant.m_dq ? TelemetryFlags.FlagBlack : TelemetryFlags.FlagNone;

                    switch ((UDPFlagType)participant.m_flags)
                    {
                        case UDPFlagType.ChequeredFlag:
                            sessionInfo.Flag = "Chequered";
                            carData.Flags = TelemetryFlags.FlagChequered;
                            break;
                        case UDPFlagType.YellowFlag:
                            sessionInfo.Flag = "Yellow";
                            carData.Flags = TelemetryFlags.FlagYellow;
                            break;
                        case UDPFlagType.BlueFlag:
                            sessionInfo.Flag = "Blue";
                            carData.Flags = TelemetryFlags.FlagBlue;
                            break;
                        case UDPFlagType.WhiteFlag:
                            sessionInfo.Flag = "White";
                            carData.Flags = TelemetryFlags.FlagWhite;
                            break;
                        default:
                            if (participant.m_sessionFinished)
                            {
                                sessionInfo.Flag = "Finished";
                                carData.Flags = TelemetryFlags.FlagChequered;
                            }
                            else
                            {
                                sessionInfo.Flag = "Green";
                                carData.Flags = TelemetryFlags.FlagGreen;
                            }
                            break;
                    }

                    OnTelemetry?.Invoke(this, new TelemetryUpdatedEventArgs(TelemetryData));
                }
            };

            udpThread.OnVehicleTelemetry += (_, vechicle) =>
            {
                if (vechicle.m_vehicleId == m_PlayerIdx)
                {
                    AMCarData carData = TelemetryData.CarData;
                    AMMotionData motionData = TelemetryData.CarData.MotionData;

                    int gear = vechicle.m_input.m_gear;

                    carData.Steering = vechicle.m_input.m_steering;
                    carData.Throttle = vechicle.m_input.m_accelerator;
                    carData.Brake = vechicle.m_input.m_brake;
                    carData.Clutch = vechicle.m_input.m_clutch;
                    carData.Gear = (short)(gear + 1);
                    carData.BrakeBias = vechicle.m_setup.m_brakeBias;
                    carData.Speed = vechicle.m_chassis.m_forwardSpeed * 3.6f;

                    if (vechicle.m_drivetrain.m_engineRevRatio > 0)
                    {
                        carData.MaxRPM = vechicle.m_drivetrain.m_engineRPM / vechicle.m_drivetrain.m_engineRevRatio;
                    }

                    try
                    {
                        carData.ShiftUpRPM = vechicle.m_drivetrain.m_gears[gear].m_upshiftRPM;
                    }
                    catch { }

                    carData.RPM = Math.Abs(vechicle.m_drivetrain.m_engineRPM);
                    carData.FuelCapacity = vechicle.m_constant.m_fuelCapacity;
                    carData.FuelLevel = vechicle.m_drivetrain.m_fuelRemaining;
                    carData.FuelConsumptionPerLap = vechicle.m_drivetrain.m_fuelUseRate;
                    carData.Electronics = vechicle.m_drivetrain.m_speedLimiterActive ? CarElectronics.Limiter : CarElectronics.None;
                    carData.Electronics |= vechicle.m_drivetrain.m_engineRunning ? CarElectronics.None : CarElectronics.Ignition;

                    vechicle.m_chassis.m_quat.GetYawPitchRoll(out float yaw, out float pitch, out float roll);

                    motionData.Pitch = 0.5f * pitch / (float)Math.PI;
                    motionData.Yaw = yaw / (float)Math.PI;
                    motionData.Roll = 0.5f * roll / (float)Math.PI;

                    motionData.Sway = vechicle.m_chassis.m_accelerationLS.x / 30f;
                    motionData.Heave = vechicle.m_chassis.m_accelerationLS.y / 30f;
                    motionData.Surge = vechicle.m_chassis.m_accelerationLS.z / 30f;

                    carData.AbsLevel = vechicle.m_setup.m_absLevel;
                    carData.TcLevel = vechicle.m_setup.m_tcsLevel;

                    OnTelemetry?.Invoke(this, new TelemetryUpdatedEventArgs(TelemetryData));
                }
            };

            monitor.OnProcessRunningStateChanged += (_, bRunning) =>
            {
                _bRunning = bRunning;

                if (bRunning)
                {
                    TelemetryData?.LoadDefaults();
                    udpThread?.Stop();
                    udpThread.Start();
                }
                else
                {
                    udpThread?.Stop();
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
            udpThread?.Dispose();
        }
    }
}