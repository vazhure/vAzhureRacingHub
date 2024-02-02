using iRacingSdkWrapper;
using iRacingSdkWrapper.Bitfields;
using System;
using System.Drawing;
using System.Globalization;
using vAzhureRacingAPI;

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

        private SdkWrapper wrapper;

        public void Initialize(IVAzhureRacingApp _)
        {
            dataSet = new TelemetryDataSet(this);

            try
            {
                wrapper = new SdkWrapper
                {
                    EventRaiseType = SdkWrapper.EventRaiseTypes.CurrentThread,
                    TelemetryUpdateFrequency = 60
                };

                wrapper.TelemetryUpdated += Wrapper_TelemetryUpdated;
                wrapper.Connected += Wrapper_Connected;
                wrapper.Disconnected += Wrapper_Disconnected;
                wrapper.SessionInfoUpdated += Wrapper_SessionInfoUpdated;

                wrapper.Start();
            }
            catch { }
        }

        private void Wrapper_SessionInfoUpdated(object sender, SdkWrapper.SessionInfoUpdatedEventArgs e)
        {
            ParseCarData(e.SessionInfo, sessionData);
        }

        private void Wrapper_Disconnected(object sender, EventArgs e)
        {
            OnGameStateChanged?.Invoke(this, new EventArgs());
        }

        private void Wrapper_Connected(object sender, EventArgs e)
        {
            OnGameStateChanged?.Invoke(this, new EventArgs());
        }

        private void Wrapper_TelemetryUpdated(object sender, SdkWrapper.TelemetryUpdatedEventArgs e)
        {
            TelemetryInfo ti = e.TelemetryInfo;

            AMCarData carData = dataSet.CarData;
            AMSessionInfo sessionInfo = dataSet.SessionInfo;
            AMWeatherData weatherData = dataSet.WeatherData;
            AMMotionData motionData = carData.MotionData;

            sessionData.currentSessionNum = ti.SessionNum.Value;

            motionData.Pitch = ti.Pitch.Value;
            motionData.Roll = ti.Roll.Value;
            motionData.Yaw = ti.Yaw.Value;
            motionData.Surge = ti.LongAccel.Value / 9.81f;
            motionData.Heave = ti.VertAccel.Value / 9.81f;
            motionData.Sway = ti.LatAccel.Value / 9.81f;
            //motionData.LocalAcceleration = new float[] { ti.LatAccel.Value, ti.VertAccel.Value, ti.LongAccel.Value };

            int idx = ti.PlayerCarIdx.Value;

            carData.DriverName = sessionData.driverName;
            carData.CarClass = sessionData.carClassName;
            carData.CarName = sessionData.carName;
            carData.CarNumber = sessionData.carNumber;

            carData.Electronics = ti.EngineWarnings.Value.Contains(EngineWarnings.PitSpeedLimiter) ? CarElectronics.Limiter : CarElectronics.None;

            carData.Electronics |= ti.DrsStatus.Value > 0 ? CarElectronics.DRS : CarElectronics.None;
            carData.Steering = -ti.SteeringWheelAngle.Value;
            carData.Throttle = ti.Throttle.Value;
            carData.Clutch = 1.0f - ti.Clutch.Value;
            carData.Brake = ti.Brake.Value;
            carData.Speed = ti.Speed.Value * 3.6f;
            carData.RPM = (uint)ti.RPM.Value;
            carData.MaxRPM = sessionData.rpmMax;
            carData.Gear = (short)(ti.Gear.Value + 1);
            carData.FuelLevel = ti.FuelLevel.Value;
            sessionInfo.CurrentLapNumber = carData.Lap = ti.Lap.Value;
            sessionInfo.CurrentPosition = carData.Position = ti.CarIdxPosition.Value[idx];
            carData.Distance = ti.LapDist.Value;
            carData.OilTemp = ti.OilTemp.Value;
            carData.OilPressure = ti.OilPress.Value;
            carData.WaterTemp = ti.WaterTemp.Value;
            carData.WaterPressure = 0;

            weatherData.AmbientTemp = ti.AirTemp.Value;
            weatherData.TrackTemp = ti.TrackTemp.Value;

            SessionStates sessionState = ti.SessionState.Value;

            carData.Tires = new AMTireData[]
            {
                new AMTireData
                {
                    BrakeTemperature = 0,
                    Pressure = new TelemetryValue<float>(wrapper.Sdk, "LFcoldPressure").Value,
                    Wear = 1.0f - (float)(new TelemetryValue<float>(wrapper.Sdk, "LFwearM").Value),
                    Temperature = new double []
                    {
                        new TelemetryValue<float>(wrapper.Sdk, "LFtempCL").Value,
                        new TelemetryValue<float>(wrapper.Sdk, "LFtempCM").Value,
                        new TelemetryValue<float>(wrapper.Sdk, "LFtempCR").Value,
                    },
                    Compound = ""
                },
                new AMTireData
                {
                    BrakeTemperature = 0,
                    Pressure = new TelemetryValue<float>(wrapper.Sdk, "RFcoldPressure").Value,
                    Wear = 1.0f - (float)(new TelemetryValue<float>(wrapper.Sdk, "RFwearM").Value),
                    Temperature = new double []
                    {
                        new TelemetryValue<float>(wrapper.Sdk, "RFtempCL").Value,
                        new TelemetryValue<float>(wrapper.Sdk, "RFtempCM").Value,
                        new TelemetryValue<float>(wrapper.Sdk, "RFtempCR").Value,
                    },
                    Compound = ""
                },
                new AMTireData
                {
                    BrakeTemperature = 0,
                    Pressure = new TelemetryValue<float>(wrapper.Sdk, "LRcoldPressure").Value,
                    Wear = 1.0f - (float)(new TelemetryValue<float>(wrapper.Sdk, "LRwearM").Value),
                    Temperature = new double []
                    {
                        new TelemetryValue<float>(wrapper.Sdk, "LRtempCL").Value,
                        new TelemetryValue<float>(wrapper.Sdk, "LRtempCM").Value,
                        new TelemetryValue<float>(wrapper.Sdk, "LRtempCR").Value,
                    },
                    Compound = ""
                },
                new AMTireData
                {
                    BrakeTemperature = 0,
                    Pressure = new TelemetryValue<float>(wrapper.Sdk, "RRcoldPressure").Value,
                    Wear = 1.0f - (float)(new TelemetryValue<float>(wrapper.Sdk, "RRwearM").Value),
                    Temperature = new double []
                    {
                        new TelemetryValue<float>(wrapper.Sdk, "RRtempCL").Value,
                        new TelemetryValue<float>(wrapper.Sdk, "RRtempCM").Value,
                        new TelemetryValue<float>(wrapper.Sdk, "RRtempCR").Value,
                    },
                    Compound = ""
                },
            };

            TelemetryValue<int> carIdx = new TelemetryValue<int>(wrapper.Sdk, "PlayerCarIdx");
            TelemetryValue<float> shiftRPM = new TelemetryValue<float>(wrapper.Sdk, "ShiftGrindRPM");

            TelemetryValue<float> curLapTime = new TelemetryValue<float>(wrapper.Sdk, "LapCurrentLapTime");
            sessionInfo.CurrentLapTime = (int)(curLapTime.Value * 1000);

            TelemetryValue<float> bestLapTime = new TelemetryValue<float>(wrapper.Sdk, "LapBestLapTime");
            sessionInfo.BestLapTime = (int)(bestLapTime.Value * 1000);

            TelemetryValue<float> lastLapTime = new TelemetryValue<float>(wrapper.Sdk, "LapLastLapTime");
            sessionInfo.LastLapTime = (int)(lastLapTime.Value * 1000);

            TelemetryValue<float> diffTime = new TelemetryValue<float>(wrapper.Sdk, "LapDeltaToSessionBestLap");
            sessionInfo.CurrentDelta = (int)(diffTime.Value * 1000);

            TelemetryValue<int> lapsRemain = new TelemetryValue<int>(wrapper.Sdk, "SessionLapsRemainEx");
            sessionInfo.RemainingLaps = lapsRemain.Value == short.MaxValue ? 0 : lapsRemain.Value;

            sessionInfo.PitSpeedLimit = sessionData.trackSpeedLimit;

            // Car Depending parameters

            try
            {
                TelemetryValue<float> dcBrakeBias = new TelemetryValue<float>(wrapper.Sdk, "dcBrakeBias");
                carData.BrakeBias = dcBrakeBias.Value;
            }
            catch { carData.BrakeBias = 50; }

            try
            {
                TelemetryValue<float> dcABS = new TelemetryValue<float>(wrapper.Sdk, "dcABS");
                carData.AbsLevel = (short)dcABS.Value;
            }
            catch { carData.AbsLevel = 0; }

            try
            {
                TelemetryValue<float> dcTractionControl = new TelemetryValue<float>(wrapper.Sdk, "dcTractionControl");
                carData.TcLevel = (short)dcTractionControl.Value;
            }
            catch { carData.TcLevel = 0; }

            try
            {
                TelemetryValue<bool> dcTractionControlToggle = new TelemetryValue<bool>(wrapper.Sdk, "dcTractionControlToggle");
                carData.Electronics |= dcTractionControlToggle.Value ? CarElectronics.TCS : CarElectronics.None;
            }
            catch { }

            carData.Flags = (ti.SessionFlags.Value.Contains(SessionFlags.Black) ? TelemetryFlags.FlagBlack : TelemetryFlags.FlagNone) |
                (ti.SessionFlags.Value.Contains(SessionFlags.Blue) ? TelemetryFlags.FlagBlue : TelemetryFlags.FlagNone) |
                (ti.SessionFlags.Value.Contains(SessionFlags.StartGo) ? TelemetryFlags.FlagGreen : TelemetryFlags.FlagNone) |
                (ti.SessionFlags.Value.Contains(SessionFlags.Checkered) ? TelemetryFlags.FlagChequered : TelemetryFlags.FlagNone) |
                (ti.SessionFlags.Value.Contains(SessionFlags.White) ? TelemetryFlags.FlagWhite : TelemetryFlags.FlagNone) |
                (ti.SessionFlags.Value.Contains(SessionFlags.YellowWaving) ? TelemetryFlags.FlagYellow : TelemetryFlags.FlagNone) |
                (ti.SessionFlags.Value.Contains(SessionFlags.Yellow) ? TelemetryFlags.FlagYellow : TelemetryFlags.FlagNone);

            sessionInfo.RemainingTime = (int)(ti.SessionTimeRemain.Value * 1000);
            sessionInfo.TrackLength = sessionData.trackLength;
            sessionInfo.TrackName = sessionData.trackName;
            sessionInfo.TrackConfig = sessionData.trackConfigName;
            sessionInfo.CurrentPosition = ti.CarIdxPosition.Value[idx];
            sessionInfo.CurrentLapNumber = ti.Lap.Value;
            sessionInfo.CurrentLapTime = (int)(curLapTime.Value * 1000);
            sessionInfo.LastLapTime = (int)(lastLapTime.Value * 1000);
            sessionInfo.BestLapTime = (int)(bestLapTime.Value * 1000);
            sessionInfo.CurrentDelta = (int)(diffTime.Value * 1000);
            sessionInfo.Sector1BestTime = -1;
            sessionInfo.Sector2BestTime = -1;
            sessionInfo.CurrentSector1Time = -1;
            sessionInfo.CurrentSector2Time = -1;
            sessionInfo.LastSector1Time = -1;
            sessionInfo.LastSector2Time = -1;
            sessionInfo.Flag = carData.Flags == TelemetryFlags.FlagBlack ? "Black" :
                               carData.Flags == TelemetryFlags.FlagBlue ? "Blue" :
                               carData.Flags == TelemetryFlags.FlagChequered ? "Chequered" :
                               carData.Flags == TelemetryFlags.FlagYellow ? "Yellow" :
                               carData.Flags == TelemetryFlags.FlagWhite ? "White" :
                               carData.Flags == TelemetryFlags.FlagGreen ? "Green" :
                               carData.Flags == TelemetryFlags.FlagSlowDown ? "SlowDown" :
                               carData.Flags == TelemetryFlags.FlagStopAndGo ? "StopAndGo" :
                               carData.Flags == TelemetryFlags.FlagPenalty ? "Penalty" : "";
            sessionInfo.TotalLapsCount = lapsRemain.Value;
            sessionInfo.DriversCount = sessionData.numDrivers;
            sessionInfo.SessionState = sessionState == SessionStates.ParadeLaps ? "Formation Lap" :
                                sessionState == SessionStates.Warmup ? "Warmup" :
                                sessionState == SessionStates.Racing ? "Race" :
                                sessionState == SessionStates.CoolDown ? "Cooldown Lap" :
                                sessionState == SessionStates.Checkered ? "Finish" : "";
            sessionInfo.FinishStatus = sessionState == SessionStates.Checkered ? "FINISHED" : "";
            sessionInfo.CurrentSector3Time = -1;
            sessionInfo.LastSector3Time = -1;
            sessionInfo.Sector = -1;
            sessionInfo.Sector3BestTime = -1;

            OnTelemetry?.Invoke(this, new TelemetryUpdatedEventArgs(dataSet));
        }

        public void Quit(IVAzhureRacingApp _)
        {
            try
            {
                wrapper.TelemetryUpdated -= Wrapper_TelemetryUpdated;
                wrapper.Connected -= Wrapper_Connected;
                wrapper.Disconnected -= Wrapper_Disconnected;
                wrapper.SessionInfoUpdated -= Wrapper_SessionInfoUpdated;
                wrapper.Stop();
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
                app.SetStatusText($"Ошибка запуска игры {Name}!");
            }
        }

        public class SessionData
        {
            public int currentSessionNum = -1;
            public int numDrivers = 0;
            public double trackLength = 0;
            public float trackSpeedLimit = 50;
            public uint rpmMax = 0;
            public string driverName = "";
            public string trackName = "";
            public string trackConfigName = "";
            public string carNumber = "";
            public string carName = "";
            public string carClassName = "";
            public string sessionType = "";
        }

        readonly SessionData sessionData = new SessionData();

        private void ParseCarData(SessionInfo sessionInfo, SessionData data)
        {
            try
            {
                sessionInfo["WeekendInfo"]["TrackDisplayName"].TryGetValue(out data.trackName);
                sessionInfo["WeekendInfo"]["TrackConfigName"].TryGetValue(out data.trackConfigName);
                sessionInfo["WeekendInfo"]["TrackPitSpeedLimit"].TryGetValue(out string speedLimit);

                if (sessionInfo["WeekendInfo"]["TrackLength"].TryGetValue(out string tracklen))
                {
                    try
                    {
                        data.trackLength = float.Parse(tracklen, CultureInfo.InvariantCulture) * 1000f;
                    }
                    catch
                    {
                        data.trackLength = 0;
                    }
                }

                if (sessionInfo["WeekendInfo"]["SessionID"].TryGetValue(out string sessionID))
                {
                    if (int.TryParse(sessionID, out data.currentSessionNum))
                    {
                        YamlQuery session = sessionInfo["WeekendInfo"]["Sessions"]["SessionNum", data.currentSessionNum];
                        session["SessionType"].TryGetValue(out data.sessionType);
                    }
                }

                if (sessionInfo["DriverInfo"]["DriverCarIdx"].TryGetValue(out string carIdx))
                {
                    if (int.TryParse(carIdx, out int idx))
                    {
                        YamlQuery driver = sessionInfo["DriverInfo"]["Drivers"]["CarIdx", idx];

                        if (driver["UserName"].TryGetValue(out string name))
                            data.driverName = name;
                        if (driver["CarNumber"].TryGetValue(out string number))
                            data.carNumber = number;
                        if (driver["CarClassShortName"].TryGetValue(out string carclass))
                            data.carClassName = carclass;
                        if (driver["CarScreenName"].TryGetValue(out string carname))
                            data.carName = carname;
                    }
                }

                YamlQuery query = sessionInfo["DriverInfo"];
                data.rpmMax = (uint)float.Parse(query["DriverCarRedLine"].GetValue("0"), CultureInfo.InvariantCulture);
                data.numDrivers =  int.Parse(query["PaceCarIdx"].GetValue("1"), CultureInfo.InvariantCulture);

                float.TryParse(speedLimit, out data.trackSpeedLimit);
            }
            catch { }
        }
    }
}