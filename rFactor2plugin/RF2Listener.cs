using rFactor2plugin.rFactor2Data;
using System;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using vAzhureRacingAPI;
using static rFactor2plugin.rFactor2Constants;

namespace rFactor2plugin
{
    class RF2Listener : VAzhureSharedMemoryClient
    {
        readonly byte[] data = new byte[rF2Telemetry.Length()];
        readonly byte[] scData = new byte[rF2Scoring.Length()];
        readonly byte[] dataExt = new byte[rF2Extended.Length()];

        readonly GamePlugin[] gamePlugins = null;
        TelemetryDataSet dataSet = null;

        const int cThreadInterval = 10; // ms

        private int session = -1;
        private double last_lap_started = -1;
        private bool bEmptyTelemetry = false;

        public bool Enabled { get; set; } = true;
        public bool SpeakBestLap { get; set; } = false;

        private readonly DeltaBest lap_delta = null;
        public RF2Listener(GamePlugin[] games, IVAzhureRacingApp app)
        {
            gamePlugins = games;
            dataSet = new TelemetryDataSet(games.FirstOrDefault() as IGamePlugin);
            lap_delta = new DeltaBest();

            lap_delta.OnBestLap += delegate (object sender, EventArgs e)
            {
                lap_delta.SpeakText("Лучший круг", app.ApplicationSoundVolume);
            };
        }

        private bool bTimingSession = false;
        private bool bGameRunning = false;

        public override void UserFunc()
        {
            GamePlugin gamePlugin = gamePlugins.Where(o => o.Running).FirstOrDefault();

            if (gamePlugin != null && Enabled)
            {
                if (gamePlugin is IGamePlugin game)
                {
                    if (dataSet.GamePlugin != game)
                        dataSet = new TelemetryDataSet(game);
                }

                try
                {
                    using (var memoryFile = MemoryMappedFile.OpenExisting(MM_TELEMETRY_FILE_NAME, MemoryMappedFileRights.Read))
                    using (var viewStream = memoryFile.CreateViewStream(0L, data.Length, MemoryMappedFileAccess.Read))
                    using (var scMemoryFile = MemoryMappedFile.OpenExisting(MM_SCORING_FILE_NAME, MemoryMappedFileRights.Read))
                    using (var scViewStream = scMemoryFile.CreateViewStream(0L, scData.Length, MemoryMappedFileAccess.Read))
                    using (var exMemoryFile = MemoryMappedFile.OpenExisting(MM_EXTENDED_FILE_NAME, MemoryMappedFileRights.Read))
                    using (var exViewStream = scMemoryFile.CreateViewStream(0L, dataExt.Length, MemoryMappedFileAccess.Read))
                    {
                        viewStream.ReadAsync(data, 0, data.Length).Wait();
                        scViewStream.ReadAsync(scData, 0, scData.Length).Wait();

                        var rf2data = Marshalizable<rF2Telemetry>.FromBytes(data);
                        var rf2Scoring = Marshalizable<rF2Scoring>.FromBytes(scData);
                        var rf2Ext = Marshalizable<rF2Extended>.FromBytes(dataExt);

                        GetPlayerIndex(rf2Scoring, out int idx);

                        switch (rf2Scoring.mScoringInfo.mGamePhase)
                        {
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                            case 5:
                            case 6:
                            case 7:
                            case 8:
                                dataSet.TelemetryState = TelemetryState.Playing;
                                break;
                            case 9:
                                dataSet.TelemetryState = TelemetryState.Paused;
                                break;
                            default:
                                dataSet.TelemetryState = TelemetryState.Unknown;
                                break;
                        }

                        if (idx >= 0 && rf2Scoring.mScoringInfo.mGamePhase < (byte)rF2GamePhase.PausedOrHeartbeat)
                        {
                            rF2VehicleTelemetry vt = rf2data.mVehicles[idx];
                            rF2VehicleScoring vs = rf2Scoring.mVehicles[idx];

                            string carName = BytesToString(vs.mVehicleName).Trim();
                            int sector = vs.mSector == 0 ? 3 : vs.mSector;

                            double lap_time_ms = rf2Scoring.mScoringInfo.mCurrentET - vs.mLapStartET;

                            AMCarData carData = dataSet.CarData;
                            AMSessionInfo sessionInfo = dataSet.SessionInfo;
                            AMWeatherData weatherData = dataSet.WeatherData;
                            AMMotionData motionData = carData.MotionData;

                            carData.Valid = false;
                            sessionInfo.Valid = false;
                            weatherData.Valid = false;

                            rF2Vec3 oriX = new rF2Vec3() { x = vt.mOri[0].x, y = vt.mOri[0].y, z = vt.mOri[0].z };
                            rF2Vec3 oriY = new rF2Vec3() { x = vt.mOri[1].x, y = vt.mOri[1].y, z = vt.mOri[1].z };
                            rF2Vec3 oriZ = new rF2Vec3() { x = vt.mOri[2].x, y = vt.mOri[2].y, z = vt.mOri[2].z };

                            motionData.Yaw =  (float)Math.Atan2(oriX.z, oriZ.z) / 3.1415f;
                            motionData.Pitch = (float)Math.Atan2(-oriY.z, Math.Sqrt(oriX.z * oriX.z + oriZ.z * oriZ.z)) / 3.1415f;
                            motionData.Roll = (float)Math.Atan2(oriY.x, Math.Sqrt(oriX.x * oriX.x + oriZ.x * oriZ.x)) / 3.1415f;

                            motionData.Sway = vt.Sway / (float)Math.PI;
                            motionData.Surge = -vt.Surge / (float)Math.PI;
                            motionData.Heave = vt.Heave / (float)Math.PI;
                            motionData.Position = vt.mPos;
                            motionData.LocalVelocity = vt.mLocalVel;
                            motionData.LocalRotAcceleration = vt.mLocalRotAccel;
                            motionData.LocalRot = vt.mLocalRot;

                            /// Переделать времена круга на общую статистику по пилотам, а актуальные данные перенести в carData
                            sessionInfo.TrackName = BytesToString(vt.mTrackName);
                            carData.Lap = sessionInfo.CurrentLapNumber = vs.mTotalLaps + 1;
                            sessionInfo.BestLapTime = (int)(vs.mBestLapTime * 1000);
                            sessionInfo.Sector1BestTime = (int)(vs.mBestLapSector1 * 1000);
                            sessionInfo.Sector2BestTime = (int)((vs.mBestLapSector2 - vs.mBestLapSector1) * 1000);
                            sessionInfo.Sector3BestTime = (int)((vs.mBestLapTime - vs.mBestLapSector2) * 1000);
                            sessionInfo.CurrentSector1Time = sector == 1 ? (int)(lap_time_ms * 1000) : (int)(vs.mCurSector1 * 1000);
                            sessionInfo.CurrentSector2Time = sector == 2 ? (int)((lap_time_ms - vs.mCurSector1) * 1000) : vs.mCurSector1 > 0 ?
                                (int)((vs.mCurSector2 - vs.mCurSector1) * 1000) : (int)((vs.mLastSector2 - vs.mLastSector1) * 1000);
                            sessionInfo.CurrentSector3Time = sector == 3 ? (int)((lap_time_ms - vs.mCurSector2) * 1000) : vs.mCurSector2 > 0 ?
                                (int)((vs.mLastLapTime - vs.mCurSector2) * 1000) : (int)((vs.mLastLapTime - vs.mLastSector2) * 1000);
                            sessionInfo.LastLapTime = vs.mLastLapTime > 0 ? (int)(vs.mLastLapTime * 1000) : -1;
                            sessionInfo.LastSector1Time = vs.mLastSector1 > 0 ? (int)(vs.mLastSector1 * 1000) : -1;
                            sessionInfo.LastSector2Time = vs.mLastSector2 > 0 ? (int)((vs.mLastSector2 - vs.mLastSector1) * 1000) : -1;
                            sessionInfo.LastSector3Time = vs.mLastLapTime > 0 ? (int)((vs.mLastLapTime - vs.mLastSector2) * 1000) : -1;
                            sessionInfo.DriversCount = rf2data.mNumVehicles;
                            carData.Position = sessionInfo.CurrentPosition = vs.mPlace;

                            int maxLaps = rf2Scoring.mScoringInfo.mMaxLaps > short.MaxValue ? -1 : rf2Scoring.mScoringInfo.mMaxLaps;

                            if (rf2Scoring.mScoringInfo.mGamePhase == 0)
                            {
                                bTimingSession = maxLaps <= 0;
                            }

                            if (rf2Scoring.mScoringInfo.mGamePhase >= 8)
                            {
                                sessionInfo.RemainingTime = 0;
                                sessionInfo.RemainingLaps = 0;
                                sessionInfo.CurrentLapTime = 0;
                            }
                            else
                            {
                                sessionInfo.CurrentLapTime = (int)(lap_time_ms * 1000);
                                sessionInfo.RemainingTime = rf2Scoring.mScoringInfo.mCurrentET < 0 ? 0 :
                                    (int)((rf2Scoring.mScoringInfo.mEndET - rf2Scoring.mScoringInfo.mCurrentET) * 1000);

                                if (!bTimingSession)
                                {
                                    sessionInfo.RemainingLaps = Math2.Clamp(maxLaps - vs.mTotalLaps, -1, short.MaxValue);
                                }
                                else
                                    sessionInfo.RemainingLaps = -1;
                            }

                            sessionInfo.Sector = sector;
                            sessionInfo.FinishStatus = vs.mFinishStatus == 1 ? "Finished" : vs.mFinishStatus == 2 ? "DNF" : vs.mFinishStatus == 3 ? "DQ" : "";
                            sessionInfo.PitSpeedLimit = rf2Ext.mCurrentPitSpeedLimit * 3.6;

                            switch (rf2Scoring.mScoringInfo.mSession)
                            {
                                case 0:
                                    sessionInfo.SessionState = "Test Day"; break;
                                case 1:
                                case 2:
                                case 3:
                                case 4:
                                    sessionInfo.SessionState = "Practice"; break;
                                case 5:
                                case 6:
                                case 7:
                                case 8:
                                    sessionInfo.SessionState = "Qualification"; break;
                                case 9:
                                    sessionInfo.SessionState = "Warmup"; break;
                                case 10:
                                case 11:
                                case 12:
                                case 13:
                                    sessionInfo.SessionState = "Race"; break;
                            }

                            carData.CarClass = BytesToString(vs.mVehicleClass).Trim();
                            ExtractCarNumber(ref carName, out string strName, out string strNumber);
                            if (carName != "")
                                carData.CarName = strName.Trim();
                            else
                                carData.CarName = carData.CarClass;
                            carData.CarNumber = strNumber.Trim();
                            carData.DriverName = carData.DriverName = BytesToString(vs.mDriverName).Trim();

                            // Dash
                            carData.FuelLevel = (float)vt.mFuel;
                            carData.FuelCapacity = (float)vt.mFuelCapacity;
                            carData.Gear = (short)(vt.mGear + 1);
                            carData.RPM = vt.mEngineRPM;
                            carData.ShiftUpRPM = carData.MaxRPM = (uint)vt.mEngineMaxRPM;
                            carData.BrakeBias = 100f - (float)vt.mRearBrakeBias * 100f;
                            carData.Speed = vt.Speed;
                            carData.WaterTemp = vt.mEngineWaterTemp;
                            carData.OilTemp = vt.mEngineOilTemp;
                            carData.AIControlled = false;
                            carData.IgnitionStarter = vt.mIgnitionStarter;
                            carData.InPits = vs.mInPits > 0;
                            carData.PitState = vs.mPitState;
                            carData.PitDistance = vs.mPitState == 3 ? vs.mPitLapDist : -1;

                            carData.Steering = 0.5 * vt.mFilteredSteering * vt.mPhysicalSteeringWheelRange * Math.PI / 180.0;
                            carData.Throttle = (float)vt.mUnfilteredThrottle;
                            carData.Brake = (float)vt.mUnfilteredBrake;
                            carData.Clutch = (float)vt.mUnfilteredClutch;

                            carData.Flags = vs.mFlag == 6 ? TelemetryFlags.FlagBlue : TelemetryFlags.FlagNone;

                            for (int i = 0; i < 3; ++i)
                            {
                                if (vt.mCurrentSector == i && rf2Scoring.mScoringInfo.mSectorFlag[i] == (int)rF2YellowFlagState.Pending)
                                    carData.Flags |= TelemetryFlags.FlagYellow;
                            }

                            switch (rf2Scoring.mScoringInfo.mGamePhase)
                            {
                                case 5:
                                    if (carData.Flags == TelemetryFlags.FlagNone)
                                    {
                                        sessionInfo.Flag = "Green";
                                        carData.Flags = TelemetryFlags.FlagGreen;
                                    }
                                    break;
                                case 6:
                                    sessionInfo.Flag = "Yellow";
                                    carData.Flags = TelemetryFlags.FlagYellow;
                                    break;
                                case 8:
                                    sessionInfo.Flag = "Chequered";
                                    carData.Flags = TelemetryFlags.FlagChequered;
                                    break;
                                default:
                                    {
                                        if (vs.mFlag == 6)
                                            sessionInfo.Flag = "Blue";

                                        if (carData.Flags == TelemetryFlags.FlagYellow)
                                            sessionInfo.Flag = "Yellow";
                                    }
                                    break;
                            }

                            carData.Tires = new AMTireData[]
                            {
                                new AMTireData
                                {
                                    BrakeTemperature = vt.mWheels[0].mBrakeTemp- 273.15,
                                    Pressure = vt.mWheels[0].mPressure,
                                    Wear = 1.0f - (float)vt.mWheels[0].mWear,
                                    Temperature = new double []
                                    {
                                        vt.mWheels[0].mTemperature[0] - 273.15,
                                        vt.mWheels[0].mTemperature[1] - 273.15,
                                        vt.mWheels[0].mTemperature[2] - 273.15,
                                    },
                                    Compound = BytesToString(vt. mFrontTireCompoundName),
                                    Detached = vt.mWheels[0].mDetached > 0
                                },
                                new AMTireData
                                {
                                    BrakeTemperature = vt.mWheels[1].mBrakeTemp- 273.15,
                                    Pressure = vt.mWheels[1].mPressure,
                                    Wear = 1.0f - (float)vt.mWheels[1].mWear,
                                    Temperature = new double []
                                    {
                                        vt.mWheels[1].mTemperature[0] - 273.15,
                                        vt.mWheels[1].mTemperature[1] - 273.15,
                                        vt.mWheels[1].mTemperature[2] - 273.15,
                                    },
                                    Compound = BytesToString(vt. mFrontTireCompoundName),
                                    Detached = vt.mWheels[1].mDetached > 0
                                },
                                new AMTireData
                                {
                                    BrakeTemperature = vt.mWheels[2].mBrakeTemp- 273.15,
                                    Pressure = vt.mWheels[2].mPressure,
                                    Wear = 1.0f - (float)vt.mWheels[2].mWear,
                                    Temperature = new double []
                                    {
                                        vt.mWheels[2].mTemperature[0] - 273.15,
                                        vt.mWheels[2].mTemperature[1] - 273.15,
                                        vt.mWheels[2].mTemperature[2] - 273.15,
                                    },
                                    Compound = BytesToString(vt. mRearTireCompoundName),
                                    Detached = vt.mWheels[2].mDetached > 0
                                },
                                new AMTireData
                                {
                                    BrakeTemperature = vt.mWheels[3].mBrakeTemp- 273.15,
                                    Pressure = vt.mWheels[3].mPressure,
                                    Wear = 1.0f - vt.mWheels[3].mWear,
                                    Temperature = new double []
                                    {
                                        vt.mWheels[3].mTemperature[0] - 273.15,
                                        vt.mWheels[3].mTemperature[1] - 273.15,
                                        vt.mWheels[3].mTemperature[2] - 273.15,
                                    },
                                    Compound = BytesToString(vt. mRearTireCompoundName),
                                    Detached = vt.mWheels[3].mDetached > 0
                                },
                            };

                            sessionInfo.TrackLength = rf2Scoring.mScoringInfo.mLapDist;
                            sessionInfo.TotalLapsCount = rf2Scoring.mScoringInfo.mMaxLaps > short.MaxValue ? 0 : rf2Scoring.mScoringInfo.mMaxLaps;

                            // current distance around track
                            int distance = (int)vs.mLapDist;

                            carData.Distance = vs.mLapDist;

                            if (distance >= 0)
                            {
                                if (vs.mLapStartET != last_lap_started)
                                {
                                    last_lap_started = vs.mLapStartET;
                                    if (vs.mTotalLaps > 0)
                                        lap_delta.EndLap(vs.mLastLapTime > 0 ? (int)(vs.mLastLapTime * 1000) : 0);

                                    string trackName = BytesToString(vt.mTrackName);
                                    lap_delta.BeginLap((int)rf2Scoring.mScoringInfo.mLapDist, trackName, carName);

                                    if (vs.mTotalLaps == 0 || session != rf2Scoring.mScoringInfo.mSession)
                                        lap_delta.Reset();

                                    session = rf2Scoring.mScoringInfo.mSession;
                                }

                                int ct = (int)((rf2Scoring.mScoringInfo.mCurrentET - vs.mLapStartET) * 1000);
                                lap_delta[distance] = ct;

                                int bst = rf2Scoring.mScoringInfo.mGamePhase <= 8 ? lap_delta[distance] : 0;
                                sessionInfo.CurrentDelta = (bst > 0 && bst != int.MaxValue) ? ct - bst : 0;
                            }
                            else
                                sessionInfo.CurrentDelta = 0;

                            weatherData.AmbientTemp = (float)rf2Scoring.mScoringInfo.mAmbientTemp;
                            weatherData.TrackTemp = (float)rf2Scoring.mScoringInfo.mTrackTemp;

                            carData.Electronics = vt.mHeadlights > 0 ? CarElectronics.Headlight : CarElectronics.None;
                            carData.Electronics |= vt.mSpeedLimiter > 0 ? CarElectronics.Limiter : CarElectronics.None;
                            carData.Electronics |= vt.mIgnitionStarter > 0 ? CarElectronics.Ignition : CarElectronics.None;
                            carData.Electronics |= vt.mRearFlapLegalStatus == 2 ? CarElectronics.DRS_EN : CarElectronics.None;
                            carData.Electronics |= vt.mRearFlapActivated != 0 ? CarElectronics.DRS : CarElectronics.None;

                            carData.Valid = true;
                            sessionInfo.Valid = true;
                            weatherData.Valid = true;

                            bEmptyTelemetry = false;
                            gamePlugin.NotifyTelemetry(dataSet);
                            Thread.Sleep(cThreadInterval);
                        }
                        else
                        {
                            if (!bEmptyTelemetry)
                            {
                                bEmptyTelemetry = true;
                                dataSet = new TelemetryDataSet(gamePlugin as IGamePlugin);
                                gamePlugin.NotifyTelemetry(dataSet);
                                Thread.Sleep(cThreadInterval * 10); // Not too fast.. Processor high load
                            }
                        }
                    }
                }
                catch { Thread.Sleep(cThreadInterval * 10); }
            }
            else
            {
                Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// Возвращает номер автомобиля
        /// </summary>
        /// <param name="carName"></param>
        /// <returns></returns>
        private bool ExtractCarNumber(ref string carName, out string name, out string number)
        {
            try
            {
                var groups = new Regex("(.*)#(\\d*)(.*)").Match(carName);
                carName = groups.Groups[1].Value.Replace("|", "");

                name = groups.Groups.Count > 1 ? groups.Groups[1].Value : carName;
                number = groups.Groups.Count > 2 ? groups.Groups[2].Value : "";
                return true;
            }
            catch
            {
                name = carName;
                number = "";
            }

            return false;
        }

        /// <summary>
        /// Convert null-terminated byte array to string 
        /// </summary>
        /// <param name="data">sting bytes</param>
        /// <returns></returns>
        private string BytesToString(byte[] data)
        {
            StringBuilder sb = new StringBuilder();

            foreach (byte b in data)
                if (b == 0)
                    break;
                else
                    sb.Append((char)b);

            return sb.ToString();
        }

        /// <summary>
        /// Предыдущий найденный индекс игрока
        /// </summary>
        private int m_oldIdx = -1;

        /// <summary>
        /// Возвращает индекс игрока
        /// </summary>
        /// <param name="scoring"></param>
        /// <param name="idx"></param>
        private void GetPlayerIndex(rF2Scoring scoring, out int idx)
        {
            // Пробуем использовать последний найденный индекс
            if (m_oldIdx > -1 && m_oldIdx < scoring.mScoringInfo.mNumVehicles && scoring.mVehicles[m_oldIdx].mIsPlayer == 1)
            {
                idx = m_oldIdx;
                return;
            }

            if (scoring.mScoringInfo.mNumVehicles == 1)
            {
                m_oldIdx = idx = 0;
                return;
            }
            else
            {
                for (int t = 0; t < scoring.mScoringInfo.mNumVehicles; t++)
                    if (scoring.mVehicles[t].mIsPlayer == 1)
                    {
                        idx = t;
                        return;
                    }
            }
            m_oldIdx = idx = -1;
        }
    }
}