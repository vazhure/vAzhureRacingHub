using System.IO.MemoryMappedFiles;
using System.Threading;
using vAzhureRacingAPI;
using PC2SharedMemory;
using System;
using System.Linq;

namespace PCars2
{
    public class PC2AMS2Listener : VAzhureSharedMemoryClient
    {
        readonly GamePlugin[] games = new GamePlugin[] { };
        public PC2AMS2Listener(GamePlugin[] gamePlugins)
        {
            games = gamePlugins;
        }

        /// <summary>
        /// TODO: settings.json with Steam player name property
        /// </summary>
        public string PlayerName { get; set; } = "V.AZHURE";

        const string PC2sharedMemoryFile = @"$pcars2$";

        public bool Enabled { get; set; } = true;

        const int cThreadInterval = 10; // ms

        private readonly byte[] data = new byte[PC2SharedMemoryStruct.Length()];

        private GamePlugin m_RunningGame = null;

        long m_packetID = -1;

        private static int lastGuessAtPlayerIndex = -1;

        public static int GetPlayerIndex(PC2SharedMemoryStruct pCars2APIStruct, string playerName = "")
        {
            if (playerName != "")
            {
                int idx = 0;
                foreach (ParticipantInfo pi in pCars2APIStruct.mParticipantInfo)
                {
                    if (pi.mName == playerName)
                        return idx;
                    idx++;
                }
            }

            if (pCars2APIStruct.mGameState == GameState.INPLAYING)
            {
                lastGuessAtPlayerIndex = pCars2APIStruct.mViewedParticipantIndex;
            }

            if (lastGuessAtPlayerIndex == -1)
            {
                return pCars2APIStruct.mViewedParticipantIndex;
            }

            return lastGuessAtPlayerIndex;
        }

        public bool EnableMotionOnReplay { get; set; } = false;

        private bool bPaused = false;

        public override void UserFunc()
        {
            GamePlugin runningGame = games.Where(g => g.IsRunning).FirstOrDefault();

            if (Enabled && runningGame != null)
            {
                if (m_RunningGame != runningGame)
                {
                    m_RunningGame = runningGame;
                    runningGame.NotifyGameState();
                }

                TelemetryDataSet dataSet = runningGame.TelemetryData;

                AMCarData carData = dataSet.CarData;
                AMSessionInfo sessionInfo = dataSet.SessionInfo;
                AMWeatherData weatherData = dataSet.WeatherData;
                AMMotionData motionData = carData.MotionData;

                try
                {
                    using (var memoryFile = MemoryMappedFile.OpenExisting(PC2sharedMemoryFile, MemoryMappedFileRights.Read))
                    using (var viewStream = memoryFile.CreateViewStream(0L, data.Length, MemoryMappedFileAccess.Read))
                    {
                        viewStream.ReadAsync(data, 0, data.Length).Wait();
                        var pageFileContent = Marshalizable<PC2SharedMemoryStruct>.FromBytes(data);

                        //pageFileContent.mVersion

                        switch (pageFileContent.mGameState)
                        {
                            default:
                            case GameState.EXITED:
                                dataSet.TelemetryState = TelemetryState.Menu;
                                break;
                            case GameState.INREPLAY:
                            case GameState.FRONT_END_REPLAY:
                                dataSet.TelemetryState = TelemetryState.Replay;
                                break;
                            case GameState.INPLAYING:
                                dataSet.TelemetryState = TelemetryState.Playing;
                                break;
                            case GameState.INPAUSED:
                                dataSet.TelemetryState = TelemetryState.Paused;
                                break;
                        }

                        if (pageFileContent.mSequenceNumber != m_packetID)
                        {
                            m_packetID = pageFileContent.mSequenceNumber;

                            if (pageFileContent.mGameState == GameState.INPLAYING)
                            {
                                int playerIdx = GetPlayerIndex(pageFileContent, PlayerName);
                                //System.Diagnostics.Debug.WriteLine($"playerIdx = {playerIdx}");
                                ParticipantInfo pi = pageFileContent.mParticipantInfo[playerIdx];

                                // Dash
                                sessionInfo.CurrentLapNumber = carData.Lap = (int)pi.mCurrentLap;
                                sessionInfo.CurrentPosition = carData.Position = (int)pi.mRacePosition;
                                carData.Speed = pageFileContent.mSpeed * 3.6f; // meters per second -> km per hour
                                carData.RPM = (uint)pageFileContent.mRpm;
                                carData.MaxRPM = (uint)pageFileContent.mMaxRPM;
                                carData.Gear = (short)(pageFileContent.mGear + 1); // R is -1
                                carData.FuelLevel = pageFileContent.mFuelCapacity * pageFileContent.mFuelLevel;
                                carData.BrakeBias = pageFileContent.mBrakeBias < 0 ? 50f : (1 - pageFileContent.mBrakeBias) * 100f;
                                carData.Distance = pi.mCurrentLapDistance;

                                weatherData.AmbientTemp = pageFileContent.mAmbientTemperature;
                                weatherData.TrackTemp = pageFileContent.mTrackTemperature;
                                weatherData.Raining = pageFileContent.mRainDensity;
                                weatherData.WindSpeed = new double[]{ pageFileContent.mWindDirectionX * pageFileContent.mWindSpeed,
                                    pageFileContent.mWindDirectionY * pageFileContent.mWindSpeed, 0};

                                switch (pageFileContent.mHighestFlagColour)
                                {
                                    default:
                                        {
                                            switch (pageFileContent.mRaceState)
                                            {
                                                case RaceState.RACING: 
                                                    carData.Flags = TelemetryFlags.FlagGreen; 
                                                    break;
                                                case RaceState.FINISHED:
                                                    carData.Flags = TelemetryFlags.FlagChequered;
                                                    break;
                                                default: 
                                                    carData.Flags = TelemetryFlags.FlagNone;
                                                    break;
                                            }
                                        }
                                        break;
                                    case FlagColour.BLACK:
                                        carData.Flags = TelemetryFlags.FlagBlack;
                                        break;
                                    case FlagColour.BLUE:
                                        carData.Flags = TelemetryFlags.FlagBlue;
                                        break;
                                    case FlagColour.WHITE_FINAL_LAP:
                                        carData.Flags = TelemetryFlags.FlagWhite;
                                        break;
                                    case FlagColour.YELLOW:
                                        carData.Flags = TelemetryFlags.FlagYellow;
                                        break;
                                    case FlagColour.GREEN:
                                        carData.Flags = TelemetryFlags.FlagGreen;
                                        break;
                                    case FlagColour.RED:
                                        carData.Flags = TelemetryFlags.FlagPenalty;
                                        break;
                                    case FlagColour.DOUBLE_YELLOW:
                                        carData.Flags = TelemetryFlags.FlagYellow;
                                        break;
                                    case FlagColour.CHEQUERED:
                                        carData.Flags = TelemetryFlags.FlagChequered;
                                        break;
                                }

                                bool bTimedLap = pageFileContent.mSessionState != SessionState.INVALID && pageFileContent.mSessionState != SessionState.FORMATION_LAP;

                                carData.Brake = pageFileContent.mUnfilteredBrake;
                                carData.Clutch = pageFileContent.mUnfilteredClutch;
                                carData.Throttle = pageFileContent.mUnfilteredThrottle;
                                carData.Steering = pageFileContent.mUnfilteredSteering * Math.PI * 2.0;

                                carData.CarClass = pageFileContent.mCarClassName; 
                                carData.FuelConsumptionPerLap = 0;
                                carData.DriverName = pi.mName;
                                carData.CarName = pageFileContent.mCarName;
                                carData.CarNumber = "";

                                carData.Tires = new AMTireData[]
                                {
                                    new AMTireData
                                    {
                                        Wear = pageFileContent.mTyreWear[0],
                                        BrakeTemperature = pageFileContent.mBrakeTempCelsius[0],
                                        Pressure = pageFileContent.mAirPressure[0],
                                        Temperature = new double []
                                        {
                                            pageFileContent.mTyreTemp[0],
                                            pageFileContent.mTyreTemp[0],
                                            pageFileContent.mTyreTemp[0],
                                        },
                                        Compound = pageFileContent.mTyreCompound[0].text
                                    },
                                    new AMTireData
                                    {
                                        Wear = pageFileContent.mTyreWear[1],
                                        BrakeTemperature = pageFileContent.mBrakeTempCelsius[1],
                                        Pressure = pageFileContent.mAirPressure[1],
                                        Temperature = new double []
                                        {
                                            pageFileContent.mTyreTemp[1],
                                            pageFileContent.mTyreTemp[1],
                                            pageFileContent.mTyreTemp[1],
                                        },
                                        Compound = pageFileContent.mTyreCompound[1].text
                                    },
                                    new AMTireData
                                    {
                                        Wear = pageFileContent.mTyreWear[2],
                                        BrakeTemperature = pageFileContent.mBrakeTempCelsius[2],
                                        Pressure = pageFileContent.mAirPressure[2],
                                        Temperature = new double []
                                        {
                                            pageFileContent.mTyreTemp[2],
                                            pageFileContent.mTyreTemp[2],
                                            pageFileContent.mTyreTemp[2],
                                        },
                                        Compound = pageFileContent.mTyreCompound[2].text
                                    },
                                     new AMTireData
                                    {
                                        Wear = pageFileContent.mTyreWear[3],
                                        BrakeTemperature = pageFileContent.mBrakeTempCelsius[3],
                                        Pressure = pageFileContent.mAirPressure[3],
                                        Temperature = new double []
                                        {
                                            pageFileContent.mTyreTemp[3],
                                            pageFileContent.mTyreTemp[3],
                                            pageFileContent.mTyreTemp[3],
                                        },
                                        Compound = pageFileContent.mTyreCompound[3].text
                                    },
                                };

                                carData.InPits = pageFileContent.mPitMode != PitMode.NONE;

                                carData.IgnitionStarter = (short)(pageFileContent.mCarFlags.HasFlag(CarFlags.ENGINE_ACTIVE) ? 2 : pageFileContent.mCarFlags.HasFlag(CarFlags.ENGINE_WARNING) ? 1 : 0);

                                carData.OilTemp = pageFileContent.mOilTempCelsius;
                                carData.OilPressure = pageFileContent.mOilPressureKPa;
                                carData.WaterTemp = pageFileContent.mWaterTempCelsius;
                                carData.WaterPressure = pageFileContent.mWaterPressureKPa;

                                carData.Electronics = (pageFileContent.mCarFlags.HasFlag(CarFlags.HEADLIGHT) ? CarElectronics.Headlight : CarElectronics.None) |
                                    (pageFileContent.mCarFlags.HasFlag(CarFlags.ABS) ? CarElectronics.ABS : CarElectronics.None) |
                                    (pageFileContent.mCarFlags.HasFlag(CarFlags.HANDBRAKE) ? CarElectronics.Handbrake : CarElectronics.None) |
                                    (pageFileContent.mCarFlags.HasFlag(CarFlags.SPEED_LIMITER) ? CarElectronics.Limiter : CarElectronics.None);

                                // Motion
                                if (pageFileContent.mGameState == GameState.INPLAYING || ((pageFileContent.mGameState == GameState.INREPLAY ||
                                pageFileContent.mGameState == GameState.FRONT_END_REPLAY) && EnableMotionOnReplay))
                                {
                                    motionData.LocalVelocity = pageFileContent.mLocalVelocity;
                                    motionData.LocalRot = pageFileContent.mOrientation;

                                    motionData.Pitch = pageFileContent.mOrientation[0] / (float)Math.PI;
                                    motionData.Yaw = pageFileContent.mOrientation[1] / (float)Math.PI;
                                    motionData.Roll = pageFileContent.mOrientation[2] / (float)Math.PI;

                                    motionData.Surge = -pageFileContent.Surge / (float)Math.PI;
                                    motionData.Heave = pageFileContent.Heave / (float)Math.PI;
                                    motionData.Sway = pageFileContent.Sway / (float)Math.PI;
                                }
                                else
                                {
                                    motionData = new AMMotionData();
                                }

                                sessionInfo.TotalLapsCount = (int)pageFileContent.mLapsInEvent;
                                sessionInfo.DriversCount = pageFileContent.mNumParticipants;
                                sessionInfo.CurrentLapTime = (int)(pageFileContent.mCurrentTime * 1000.0f); // seconds to milliseconds, -1 - undefined
                                sessionInfo.LastLapTime = (int)(pageFileContent.mLastLapTime * 1000.0f);
                                sessionInfo.BestLapTime = (int)(pageFileContent.mBestLapTime * 1000.0f);
                                sessionInfo.RemainingTime = (int)(pageFileContent.mEventTimeRemaining * 1000.0f);
                                sessionInfo.RemainingLaps = (int)pageFileContent.mLapsInEvent - (int)pi.mCurrentLap - 1;
                                sessionInfo.TrackLength = pageFileContent.mTrackLength;
                                sessionInfo.DriversCount = pageFileContent.mNumParticipants;
                                sessionInfo.TrackName = pageFileContent.mTrackLocation;
                                sessionInfo.TrackConfig = pageFileContent.mTrackVariation;
                                sessionInfo.Sector = pi.mCurrentSector + 1;
                                sessionInfo.SessionState = pageFileContent.mSessionState == SessionState.FORMATION_LAP ? "Formation Lap" :
                                pageFileContent.mSessionState == SessionState.PRACTICE ? "Practice" :
                                pageFileContent.mSessionState == SessionState.QUALIFY ? "Qualifying" :
                                pageFileContent.mSessionState == SessionState.RACE ? "Race" :
                                pageFileContent.mSessionState == SessionState.TIME_ATTACK ? "Time Attack" :
                                pageFileContent.mSessionState == SessionState.TEST ? "Test day" : "";

                                sessionInfo.Flag = carData.Flags == TelemetryFlags.FlagBlack ? "Black" :
                                carData.Flags == TelemetryFlags.FlagBlue ? "Blue" :
                                carData.Flags == TelemetryFlags.FlagChequered ? "Chequered" :
                                carData.Flags == TelemetryFlags.FlagYellow ? "Yellow" :
                                carData.Flags == TelemetryFlags.FlagWhite ? "White" :
                                carData.Flags == TelemetryFlags.FlagGreen ? "Green" :
                                carData.Flags == TelemetryFlags.FlagSlowDown ? "SlowDown" :
                                carData.Flags == TelemetryFlags.FlagStopAndGo ? "StopAndGo" :
                                carData.Flags == TelemetryFlags.FlagPenalty ? "Penalty" : "";

                                sessionInfo.CurrentDelta = 0;// (int)(pageFileContent.mSplitTime * 1000.0f);
                                sessionInfo.LastSector1Time = (int)(pageFileContent.mLastLapTimes[0] * 1000.0f);
                                sessionInfo.LastSector2Time = (int)(pageFileContent.mLastLapTimes[1] * 1000.0f);
                                sessionInfo.LastSector3Time = (int)(pageFileContent.mLastLapTimes[2] * 1000.0f);
                                sessionInfo.Sector1BestTime = (int)(pageFileContent.mFastestSector1Time * 1000.0f);
                                sessionInfo.Sector2BestTime = (int)(pageFileContent.mFastestSector2Time * 1000.0f);
                                sessionInfo.Sector3BestTime = (int)(pageFileContent.mFastestSector3Time * 1000.0f);
                                sessionInfo.CurrentSector1Time = (int)(pageFileContent.mCurrentSector1Time * 1000.0f);
                                sessionInfo.CurrentSector2Time = (int)(pageFileContent.mCurrentSector2Time * 1000.0f);
                                sessionInfo.CurrentSector3Time = (int)(pageFileContent.mCurrentSector3Time * 1000.0f);
                                sessionInfo.FinishStatus = pageFileContent.mRaceStates[playerIdx] == RaceState.DISQUALIFIED ? "DQ" :
                                pageFileContent.mRaceStates[playerIdx] == RaceState.DNF ? "DNF" :
                                pageFileContent.mRaceStates[playerIdx] == RaceState.NOT_STARTED ? "DNS" :
                                pageFileContent.mRaceStates[playerIdx] == RaceState.RACING ? "Racing" :
                                pageFileContent.mRaceStates[playerIdx] == RaceState.RETIRED ? "Retired" :
                                pageFileContent.mRaceStates[playerIdx] == RaceState.FINISHED ? "Finished" : "";

                                sessionInfo.Valid = true;
                                carData.Valid = true;
                                weatherData.Valid = true;
                                bPaused = false;

                                runningGame?.NotifyTelemetry(dataSet);
                            }
                            else
                            {
                                if (!bPaused)
                                {
                                    dataSet.CarData.MotionData = new AMMotionData();
                                    runningGame?.NotifyTelemetry(dataSet);
                                    bPaused = true;
                                }
                            }
                        }
                    }
                }
                catch
                {
                    if (!bPaused)
                    {
                        dataSet.LoadDefaults();
                        runningGame?.NotifyTelemetry(dataSet);
                        bPaused = true;
                    }
                }

                Thread.Sleep(cThreadInterval);
            }
            else
            {
                if (m_RunningGame != null)
                {
                    m_RunningGame.NotifyGameState();
                    m_RunningGame = null;
                }

                Thread.Sleep(1000);
            }
        }
    }
}