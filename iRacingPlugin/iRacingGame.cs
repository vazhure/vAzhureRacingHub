using HerboldRacing;
using System;
using System.Drawing;
using vAzhureRacingAPI;
using static HerboldRacing.IRacingSdkEnum;

namespace iRacingPlugin
{
    class IRacingGame : IGamePlugin
    {
        public string Name => "iRacing";

        public uint SteamGameID => 266410U;

        public string[] ExecutableProcessName => new string[] { "iRacingSim64DX11", "iRacingSim64DX11" };

        public bool IsRunning
        {
            get
            {
                return Utils.IsProcessRunning(ExecutableProcessName);
            }
        }

        string sUserIconPath = "";
        string sUserExecutablePath = "";

        public string UserIconPath
        {
            get
            {
                return sUserIconPath;
            }
            set
            {
                if (sUserIconPath != value)
                {
                    sUserIconPath = value;
                    // TODO: Загрузка иконки
                    OnGameIconChanged?.Invoke(this, new EventArgs());
                }
            }
        }
        public string UserExecutablePath { get => sUserExecutablePath; set => sUserExecutablePath = value; }

        TelemetryDataSet dataSet = null;

        private IRSDKSharper wrapper;

        public void Initialize(IVAzhureRacingApp _)
        {
            dataSet = new TelemetryDataSet(this);

            try
            {
                wrapper = new IRSDKSharper
                {
                    UpdateInterval = 1
                };

                wrapper.OnException += OnException;
                wrapper.OnConnected += OnConnected;
                wrapper.OnDisconnected += OnDisconnected;
                wrapper.OnSessionInfo += OnSessionInfo;
                wrapper.OnTelemetryData += OnTelemetryData;
                wrapper.OnStopped += OnStopped;

                wrapper.Start();
            }
            catch { }
        }

        private void OnException(Exception exception)
        {
            //MessageBox.Show(exception.Message);
        }

        private void OnConnected()
        {
            dataSet = new TelemetryDataSet(this);
            carIdx = 0;
            OnGameStateChanged?.Invoke(this, new EventArgs());
        }

        private void OnDisconnected()
        {
            OnGameStateChanged?.Invoke(this, new EventArgs());
        }

        private int carIdx = 0;

        private void OnSessionInfo()
        {
            dataSet.SessionInfo.TrackName = wrapper.Data.SessionInfo.WeekendInfo.TrackName;
            dataSet.SessionInfo.TrackConfig = wrapper.Data.SessionInfo.WeekendInfo.TrackConfigName;

            if (double.TryParse(wrapper.Data.SessionInfo.WeekendInfo.TrackPitSpeedLimit, out double speedLimit))
                dataSet.SessionInfo.PitSpeedLimit = speedLimit; // kph
            if (double.TryParse(wrapper.Data.SessionInfo.WeekendInfo.TrackLength, out double length))
                dataSet.SessionInfo.TrackLength = length * 1000f;

            //int sessionNum = wrapper.Data.GetInt("SessionNum");

            carIdx = wrapper.Data.SessionInfo.DriverInfo.DriverCarIdx;
            dataSet.CarData.CarName = wrapper.Data.SessionInfo.DriverInfo.Drivers[carIdx].CarScreenName;
            dataSet.CarData.CarClass = wrapper.Data.SessionInfo.DriverInfo.Drivers[carIdx].CarClassShortName;
            dataSet.CarData.DriverName = wrapper.Data.SessionInfo.DriverInfo.Drivers[carIdx].DivisionName;
            dataSet.CarData.CarNumber = wrapper.Data.SessionInfo.DriverInfo.Drivers[carIdx].CarNumber;
            dataSet.CarData.CarClass = wrapper.Data.SessionInfo.DriverInfo.Drivers[carIdx].CarClassShortName;
            dataSet.CarData.MaxRPM = wrapper.Data.SessionInfo.DriverInfo.DriverCarRedLine;
            dataSet.SessionInfo.DriversCount = wrapper.Data.SessionInfo.DriverInfo.PaceCarIdx;
            //dataSet.SessionInfo.TotalLapsCount = wrapper.Data.SessionInfo.SessionInfo.Sessions[]
        }

        private void OnTelemetryData()
        {
            AMCarData carData = dataSet.CarData;
            AMSessionInfo sessionInfo = dataSet.SessionInfo;
            AMWeatherData weatherData = dataSet.WeatherData;
            AMMotionData motionData = carData.MotionData;

            motionData.Pitch = -wrapper.Data.GetFloat("Pitch") / (float)Math.PI;
            motionData.Roll = wrapper.Data.GetFloat("Roll") / (float)Math.PI;
            motionData.Yaw = wrapper.Data.GetFloat("Yaw") / (float)Math.PI;
            motionData.Surge = wrapper.Data.GetFloat("LongAccel") / 30f;
            motionData.Heave = wrapper.Data.GetFloat("VertAccel") / 30f;
            motionData.Sway = wrapper.Data.GetFloat("LatAccel") / 30f;

            EngineWarnings ew = (EngineWarnings)wrapper.Data.GetBitField("EngineWarnings");

            carData.Electronics = ew.HasFlag(EngineWarnings.PitSpeedLimiter) ? CarElectronics.Limiter : CarElectronics.None;

            carData.Steering = -wrapper.Data.GetFloat("SteeringWheelAngle");
            carData.Throttle = wrapper.Data.GetFloat("Throttle");
            carData.Clutch = wrapper.Data.GetFloat("Clutch");
            carData.Brake = wrapper.Data.GetFloat("Brake");
            carData.Speed = wrapper.Data.GetFloat("Speed") * 3.6f;
            carData.RPM = wrapper.Data.GetFloat("RPM");
            carData.Gear = (short)(wrapper.Data.GetInt("Gear") + 1);
            carData.FuelLevel = wrapper.Data.GetFloat("FuelLevel");
            sessionInfo.CurrentLapNumber = carData.Lap = wrapper.Data.GetInt("Lap");
            carData.Distance = wrapper.Data.GetFloat("LapDist");
            carData.OilTemp = wrapper.Data.GetFloat("OilTemp");
            carData.OilPressure = wrapper.Data.GetFloat("OilPress");
            carData.WaterTemp = wrapper.Data.GetFloat("WaterTemp");
            carData.WaterPressure = wrapper.Data.GetFloat("WaterLevel");
            weatherData.AmbientTemp = wrapper.Data.GetFloat("AirTemp");
            weatherData.TrackTemp = wrapper.Data.GetFloat("TrackTemp");
            carData.InPits = ((TrkLoc)wrapper.Data.GetBitField("CarIdxTrackSurface", carIdx)).HasFlag(TrkLoc.InPitStall);
            sessionInfo.CurrentPosition = carData.Position = wrapper.Data.GetInt("CarIdxClassPosition", carIdx);

            carData.Tires = new AMTireData[]
            {
                new AMTireData
                {
                    BrakeTemperature = 0,
                    Pressure = wrapper.Data.GetFloat("LFcoldPressure") , // kPa
                    Wear = 1.0f - wrapper.Data.GetFloat("LFwearM"),
                    Temperature = new double []
                    {
                        wrapper.Data.GetFloat("LFtempCL"),
                        wrapper.Data.GetFloat("LFtempCM"),
                        wrapper.Data.GetFloat("LFtempCR"),
                    },
                    Compound = ""
                },
                new AMTireData
                {
                    BrakeTemperature = 0,
                    Pressure = wrapper.Data.GetFloat("RFcoldPressure"), // kPa
                    Wear = 1.0f - wrapper.Data.GetFloat("RFwearM"),
                    Temperature = new double []
                    {
                        wrapper.Data.GetFloat("RFtempCL"),
                        wrapper.Data.GetFloat("RFtempCM"),
                        wrapper.Data.GetFloat("RFtempCR"),
                    },
                    Compound = ""
                },
                new AMTireData
                {
                    BrakeTemperature = 0,
                    Pressure = wrapper.Data.GetFloat("LRcoldPressure"),// kPa
                    Wear = 1.0f - wrapper.Data.GetFloat("LRwearM"),
                    Temperature = new double []
                    {
                        wrapper.Data.GetFloat("LRtempCL"),
                        wrapper.Data.GetFloat("LRtempCM"),
                        wrapper.Data.GetFloat("LRtempCR"),
                    },
                    Compound = ""
                },
                new AMTireData
                {
                    BrakeTemperature = 0,
                    Pressure = wrapper.Data.GetFloat("RRcoldPressure"), // kPa
                    Wear = 1.0f - wrapper.Data.GetFloat("RRwearM"),
                    Temperature = new double []
                    {
                        wrapper.Data.GetFloat("RRtempCL"),
                        wrapper.Data.GetFloat("RRtempCM"),
                        wrapper.Data.GetFloat("RRtempCR"),
                    },
                    Compound = ""
                },
            };

            sessionInfo.CurrentLapNumber = wrapper.Data.GetInt("CarIdxLap", carIdx);
            sessionInfo.CurrentLapTime = (int)(wrapper.Data.GetFloat("LapCurrentLapTime") * 1000f);
            sessionInfo.BestLapTime = (int)(wrapper.Data.GetFloat("LapBestLapTime") * 1000f);
            sessionInfo.LastLapTime = (int)(wrapper.Data.GetFloat("LapLastLapTime") * 1000f);
            sessionInfo.CurrentDelta = (int)(wrapper.Data.GetFloat("LapDeltaToSessionBestLap") * 1000f);
            sessionInfo.RemainingTime = (int)(wrapper.Data.GetDouble("SessionTimeRemain") * 1000.0);
            sessionInfo.RemainingLaps = wrapper.Data.GetInt("SessionLapsRemain");
            //sessionInfo.TotalLapsCount = wrapper.Data.GetInt("SessionLaps");

            // Car Depending parameters

            try
            {
                carData.BrakeBias = wrapper.Data.GetFloat("dcBrakeBias");
            }
            catch { carData.BrakeBias = 50; }

            try
            {
                carData.AbsLevel = (short)wrapper.Data.GetFloat("dcABS");
            }
            catch { carData.AbsLevel = 0; }

            try
            {
                carData.TcLevel = (short)wrapper.Data.GetFloat("dcTractionControl");
            }
            catch { carData.TcLevel = 0; }

            try
            {
                carData.TcLevel2 = (short)wrapper.Data.GetFloat("dcTractionControl2");
            }
            catch { carData.TcLevel2 = 0; }

            try
            {
                carData.EngineMap = (short)wrapper.Data.GetFloat("dcFuelMixture");
            }
            catch { carData.EngineMap = 0; }

            try
            {
                carData.Electronics |= wrapper.Data.GetBool("dcTractionControlToggle") ? CarElectronics.TCS : CarElectronics.None;
            }
            catch { }

            carData.IgnitionStarter = (short)(ew.HasFlag(EngineWarnings.EngineStalled) ? 0 : ew.HasFlag(EngineWarnings.FuelPressureWarning)? 1 : 2);

            Flags sessionFlags = (Flags)wrapper.Data.GetBitField("SessionFlags");

            carData.Flags = (sessionFlags.HasFlag(Flags.Black) ? TelemetryFlags.FlagBlack : TelemetryFlags.FlagNone) |
                (sessionFlags.HasFlag(Flags.Blue) ? TelemetryFlags.FlagBlue : TelemetryFlags.FlagNone) |
                (sessionFlags.HasFlag(Flags.StartGo) ? TelemetryFlags.FlagGreen : TelemetryFlags.FlagNone) |
                (sessionFlags.HasFlag(Flags.Checkered) ? TelemetryFlags.FlagChequered : TelemetryFlags.FlagNone) |
                (sessionFlags.HasFlag(Flags.White) ? TelemetryFlags.FlagWhite : TelemetryFlags.FlagNone) |
                (sessionFlags.HasFlag(Flags.YellowWaving) ? TelemetryFlags.FlagYellow : TelemetryFlags.FlagNone) |
                (sessionFlags.HasFlag(Flags.Yellow) ? TelemetryFlags.FlagYellow : TelemetryFlags.FlagNone);

            sessionInfo.Sector1BestTime = -1;
            sessionInfo.Sector2BestTime = -1;
            sessionInfo.CurrentSector1Time = -1;
            sessionInfo.CurrentSector2Time = -1;
            sessionInfo.LastSector1Time = -1;
            sessionInfo.LastSector2Time = -1;
            sessionInfo.Flag = carData.Flags.HasFlag(TelemetryFlags.FlagBlack) ? "Black" :
                               carData.Flags.HasFlag(TelemetryFlags.FlagBlue) ? "Blue" :
                               carData.Flags.HasFlag(TelemetryFlags.FlagChequered) ? "Chequered" :
                               carData.Flags.HasFlag(TelemetryFlags.FlagYellow) ? "Yellow" :
                               carData.Flags.HasFlag(TelemetryFlags.FlagWhite) ? "White" :
                               carData.Flags.HasFlag(TelemetryFlags.FlagGreen) ? "Green" :
                               carData.Flags.HasFlag(TelemetryFlags.FlagSlowDown) ? "SlowDown" :
                               carData.Flags.HasFlag(TelemetryFlags.FlagStopAndGo) ? "StopAndGo" :
                               carData.Flags.HasFlag(TelemetryFlags.FlagPenalty) ? "Penalty" : "";

            SessionState sessionState = (SessionState)wrapper.Data.GetBitField("SessionState");

            sessionInfo.SessionState = sessionState.HasFlag(SessionState.ParadeLaps) ? "Formation Lap" :
                                sessionState.HasFlag(SessionState.Warmup) ? "Warmup" :
                                sessionState.HasFlag(SessionState.Racing) ? "Race" :
                                sessionState.HasFlag(SessionState.CoolDown) ? "Cooldown Lap" :
                                sessionState.HasFlag(SessionState.Checkered) ? "Finish" : "";

            sessionInfo.FinishStatus = sessionState.HasFlag(SessionState.Checkered) ? "FINISHED" : "";
            sessionInfo.CurrentSector3Time = -1;
            sessionInfo.LastSector3Time = -1;
            sessionInfo.Sector = -1;
            sessionInfo.Sector3BestTime = -1;

            OnTelemetry?.Invoke(this, new TelemetryUpdatedEventArgs(dataSet));
        }

        private void OnStopped()
        {
            OnGameStateChanged?.Invoke(this, new EventArgs());
        }

        public void Quit(IVAzhureRacingApp _)
        {
            try
            {
                wrapper?.Stop();
            }
            catch { }
        }

        public event EventHandler<TelemetryUpdatedEventArgs> OnTelemetry;
        public event EventHandler OnGameStateChanged;
        public event EventHandler OnGameIconChanged;

        readonly Icon gameIcon = Properties.Resources.iRacing;

        public Icon GetIcon()
        {
            // TODO:
            return gameIcon;
        }

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
    }
}