using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using vAzhureRacingAPI;

namespace F1Series
{
    public class F1Game : VAzhureUDPClient, IGamePlugin
    {
        private readonly uint m_version;
        public F1Game(uint version)
        {
            m_version = version;
            try
            {
                LoadSettings();
            }
            catch { }

            monitor = new ProcessMonitor(ExecutableProcessName, 1000);
            monitor.OnProcessRunningStateChanged += delegate (object o, bool bRunning)
            {
                bIsRunning = bRunning;
                if (bRunning)
                    Run(settings.UPDPort);
                else
                    Stop();

                OnGameStateChanged?.Invoke(this, new EventArgs());
            };
            monitor.Start();
        }

        private bool bIsRunning = false;

        private void LoadSettings()
        {
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), $"{Name}.json");
            if (File.Exists(path))
            {
                try
                {
                    string json = File.ReadAllText(path);

                    settings = ObjectSerializeHelper.DeserializeJson<F1GameSettings>(json);
                }
                catch { }
            }
        }

        private void SaveSettings()
        {
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), $"{Name}.json");
            string json = "";
            if (File.Exists(path))
            {
                try
                {
                    json = File.ReadAllText(path);
                }
                catch { }
            }
            string jsonNew = settings.GetJson();
            if (json != jsonNew)
            {
                try
                {
                    File.WriteAllText(path, jsonNew);
                }
                catch { }
            }
        }

        public string Name => $"F1 {m_version}";

        public uint SteamGameID
        {
            get
            {
                switch (m_version)
                {
                    default:
                    case 2018: return 737800U;
                    case 2019: return 928600U;
                    case 2020: return 1080110U;
                    case 2021: return 1134570U;
                    case 2022: return 1692250U;
                    case 2023: return 2108330U;
                    case 2024: return 2488620U;
                    case 2025: return 3059520U;
                }
            }
        }

        TelemetryDataSet mCarData = null;

        public override void OnDataReceived(ref byte[] bytes)
        {
            mCarData = mCarData ?? new TelemetryDataSet(this);

            ushort packetFormat = BitConverter.ToUInt16(bytes, 0);

            switch (packetFormat)
            {
                case 2022: ProcessPacket2022(ref bytes); break;
                case 2023: ProcessPacket2023(ref bytes); break;
                case 2024: ProcessPacket2024(ref bytes); break;
                case 2025: ProcessPacket2025(ref bytes); break;
            }
        }

        private void ProcessPacket2025(ref byte[] bytes)
        {
            F12025.PacketHeader header = Marshalizable<F12025.PacketHeader>.FromBytes(bytes);

            if (header.m_sessionUID != m_prevSessionUID)
            {
                mCarData.LoadDefaults();
                m_prevSessionUID = header.m_sessionUID;
            }

            switch (header.m_packetId)
            {
                case F12025.PacketId.ePacketIdMotion:
                    {
                        F12025.PacketMotionData data = Marshalizable<F12025.PacketMotionData>.FromBytes(bytes);

                        F12025.CarMotionData carMotionData = data.m_carMotionData[header.m_playerCarIndex];

                        AMMotionData motionData = mCarData.CarData.MotionData;

                        motionData.Position = new double[] { carMotionData.m_worldPositionX, carMotionData.m_worldPositionY, carMotionData.m_worldPositionZ };

                        motionData.Pitch = carMotionData.m_pitch / (float)Math.PI;
                        motionData.Roll = carMotionData.m_roll / (float)Math.PI;
                        motionData.Yaw = carMotionData.m_yaw / (float)Math.PI;

                        motionData.Surge = carMotionData.m_gForceLongitudinal / 3f;
                        motionData.Sway = carMotionData.m_gForceLateral / 3f;
                        motionData.Heave = carMotionData.m_gForceVertical / 3f;

                        OnTelemetry?.Invoke(this, new TelemetryUpdatedEventArgs(mCarData));
                    }
                    break;
                case F12025.PacketId.ePacketIdCarStatus:
                    {
                        var data = Marshalizable<F12025.PacketCarStatusData>.FromBytes(bytes);
                        var status = data.m_carStatusData[header.m_playerCarIndex];
                        var sessionInfo = mCarData.SessionInfo;
                        var carData = mCarData.CarData;

                        carData.MaxRPM = status.m_maxRPM;
                        carData.AbsLevel = status.m_antiLockBrakes;
                        carData.TcLevel = status.m_tractionControl;
                        carData.EngineMap = status.m_fuelMix;
                        carData.BrakeBias = status.m_frontBrakeBias;

                        if (status.m_drsAllowed > 0)
                            carData.Electronics |= CarElectronics.DRS_EN;
                        else
                            carData.Electronics &= ~CarElectronics.DRS_EN;

                        if (status.m_pitLimiterStatus > 0)
                            carData.Electronics |= CarElectronics.Limiter;
                        else
                            carData.Electronics &= ~CarElectronics.Limiter;

                        carData.FuelCapacity = status.m_fuelCapacity;
                        carData.FuelLevel = status.m_fuelInTank;
                        carData.FuelEstimatedLaps = status.m_fuelRemainingLaps;

                        carData.Tires = carData.Tires ?? new AMTireData[4];
                        F12022.CompoundID.TryGetValue(status.m_visualTyreCompound, out string compound);
                        carData.Tires[0].Compound = compound;
                        carData.Tires[1].Compound = compound;
                        carData.Tires[2].Compound = compound;
                        carData.Tires[3].Compound = compound;

                        F12024.FiaFlags flags = (F12024.FiaFlags)status.m_vehicleFIAFlags;

                        switch (flags)
                        {
                            default:
                            case F12024.FiaFlags.None:
                            case F12024.FiaFlags.Invalid: carData.Flags = TelemetryFlags.FlagNone; sessionInfo.Flag = ""; break;
                            case F12024.FiaFlags.Blue: carData.Flags = TelemetryFlags.FlagGreen | TelemetryFlags.FlagBlue; sessionInfo.Flag = "Blue"; break;
                            case F12024.FiaFlags.Yellow: carData.Flags = TelemetryFlags.FlagYellow; sessionInfo.Flag = "Yellow"; break;
                            case F12024.FiaFlags.Red: carData.Flags = TelemetryFlags.FlagBlack; sessionInfo.Flag = "Red"; break;
                            case F12024.FiaFlags.Green: carData.Flags = TelemetryFlags.FlagGreen; sessionInfo.Flag = "Green"; break;
                        }

                        carData.Valid = true;
                        OnTelemetry?.Invoke(this, new TelemetryUpdatedEventArgs(mCarData));
                    }
                    break;
                case F12025.PacketId.ePacketIdCarTelemetry:
                    {
                        var data = Marshalizable<F12025.PacketCarTelemetryData>.FromBytes(bytes);
                        var telemetry = data.m_carTelemetryData[header.m_playerCarIndex];
                        var carData = mCarData.CarData;

                        carData.Gear = (short)(telemetry.m_gear + 1);
                        carData.RPM = telemetry.m_engineRPM;
                        carData.Speed = telemetry.m_speed;
                        carData.Throttle = telemetry.m_throttle;
                        carData.Brake = telemetry.m_brake;
                        carData.Clutch = telemetry.m_clutch;
                        carData.Steering = telemetry.m_steer;

                        if (telemetry.m_drs > 0)
                            carData.Electronics |= CarElectronics.DRS;
                        else
                            carData.Electronics &= ~CarElectronics.DRS;

                        carData.Tires = carData.Tires ?? new AMTireData[4];

                        carData.Tires[0].Pressure = telemetry.m_tyresPressure[0] * 6.89476; // PSI -> kPa
                        carData.Tires[1].Pressure = telemetry.m_tyresPressure[1] * 6.89476; // PSI -> kPa
                        carData.Tires[2].Pressure = telemetry.m_tyresPressure[2] * 6.89476; // PSI -> kPa
                        carData.Tires[3].Pressure = telemetry.m_tyresPressure[3] * 6.89476; // PSI -> kPa

                        carData.Tires[0].Temperature[1] = telemetry.m_tyresInnerTemperature[0];
                        carData.Tires[1].Temperature[1] = telemetry.m_tyresInnerTemperature[1];
                        carData.Tires[2].Temperature[1] = telemetry.m_tyresInnerTemperature[2];
                        carData.Tires[3].Temperature[1] = telemetry.m_tyresInnerTemperature[3];
                        carData.Tires[0].Temperature[0] = carData.Tires[0].Temperature[2] = telemetry.m_tyresSurfaceTemperature[0];
                        carData.Tires[1].Temperature[0] = carData.Tires[1].Temperature[2] = telemetry.m_tyresSurfaceTemperature[1];
                        carData.Tires[2].Temperature[0] = carData.Tires[2].Temperature[2] = telemetry.m_tyresSurfaceTemperature[2];
                        carData.Tires[3].Temperature[0] = carData.Tires[3].Temperature[2] = telemetry.m_tyresSurfaceTemperature[3];
                        carData.Tires[0].BrakeTemperature = telemetry.m_brakesTemperature[0];
                        carData.Tires[1].BrakeTemperature = telemetry.m_brakesTemperature[1];
                        carData.Tires[2].BrakeTemperature = telemetry.m_brakesTemperature[2];
                        carData.Tires[3].BrakeTemperature = telemetry.m_brakesTemperature[3];

                        carData.OilTemp = telemetry.m_engineTemperature;

                        carData.Valid = true;
                        OnTelemetry?.Invoke(this, new TelemetryUpdatedEventArgs(mCarData));
                    }
                    break;
                case F12025.PacketId.ePacketIdLapData:
                    {
                        var data = Marshalizable<F12025.PacketLapData>.FromBytes(bytes);
                        var lapdata = data.m_lapData[header.m_playerCarIndex];
                        var carData = mCarData.CarData;
                        var sessionInfo = mCarData.SessionInfo;
                        sessionInfo.Sector = lapdata.m_sector;
                        sessionInfo.CurrentSector1Time = (lapdata.m_sector1TimeMinutesPart * 60000) + lapdata.m_sector1TimeMSPart;
                        sessionInfo.CurrentSector2Time = lapdata.m_sector > 0 ? (lapdata.m_sector2TimeMinutesPart * 60000) + lapdata.m_sector2TimeMSPart : -1;
                        sessionInfo.CurrentSector3Time = lapdata.m_sector > 1 ? (int)lapdata.m_currentLapTimeInMS - ((lapdata.m_sector2TimeMinutesPart * 60000) + lapdata.m_sector2TimeMSPart) : -1;
                        sessionInfo.CurrentLapTime = (int)lapdata.m_currentLapTimeInMS;
                        sessionInfo.CurrentPosition = lapdata.m_carPosition;
                        sessionInfo.CurrentLapNumber = lapdata.m_currentLapNum;
                        sessionInfo.LastLapTime = (int)lapdata.m_lastLapTimeInMS;

                        carData.Distance = lapdata.m_lapDistance;
                        carData.InPits = lapdata.m_pitStatus == 2;

                        sessionInfo.Valid = true;
                        OnTelemetry?.Invoke(this, new TelemetryUpdatedEventArgs(mCarData));
                    }
                    break;
                case F12025.PacketId.ePacketIdSession:
                    {
                        var data = Marshalizable<F12025.PacketSessionData>.FromBytes(bytes);
                        var sessionInfo = mCarData.SessionInfo;
                        var weatherInfo = mCarData.WeatherData;
                        sessionInfo.TrackLength = data.m_trackLength;
                        sessionInfo.RemainingTime = data.m_sessionTimeLeft;
                        sessionInfo.TotalLapsCount = data.m_totalLaps;

                        switch (data.m_sessionType)
                        {
                            // 0 = unknown, 1 = P1, 2 = P2, 3 = P3, 4 = Short P
                            // 5 = Q1, 6 = Q2, 7 = Q3, 8 = Short Q, 9 = OSQ
                            // 10 = R, 11 = R2, 12 = R3, 13 = Time Trial

                            default:
                            case 0: sessionInfo.SessionState = ""; break;
                            case 1:
                            case 2:
                            case 3:
                            case 4: sessionInfo.SessionState = "Practice"; break;
                            case 5:
                            case 6:
                            case 7:
                            case 8:
                            case 9: sessionInfo.SessionState = "Qualifying"; break;
                            case 10:
                            case 11:
                            case 12: sessionInfo.SessionState = "Race"; break;
                            case 13: sessionInfo.SessionState = "Time Trial"; break;
                        }

                        if (data.m_trackId <= F12025.TrackID.Length)
                            sessionInfo.TrackName = F12025.TrackID[data.m_trackId];
                        else
                            sessionInfo.TrackName = "Unknown Track Name";

                        weatherInfo.AmbientTemp = data.m_airTemperature;
                        weatherInfo.TrackTemp = data.m_trackTemperature;

                        sessionInfo.Valid = true;
                        OnTelemetry?.Invoke(this, new TelemetryUpdatedEventArgs(mCarData));
                    }
                    break;
                case F12025.PacketId.ePacketIdParticipants:
                    {
                        var data = Marshalizable<F12025.PacketParticipantsData>.FromBytes(bytes);
                        var participant = data.m_participants[header.m_playerCarIndex];
                        var carData = mCarData.CarData;

                        F12025.DriversID.TryGetValue(participant.m_driverId, out string name);
                        carData.DriverName = name ?? participant.m_name;
                        carData.CarNumber = $"{participant.m_raceNumber}";
                        F12025.TeamsID.TryGetValue(participant.m_driverId, out string team);
                        carData.CarName = team;

                        OnTelemetry?.Invoke(this, new TelemetryUpdatedEventArgs(mCarData));
                    }
                    break;
                case F12025.PacketId.ePacketIdFinalClassification:
                    {
                        var data = Marshalizable<F12025.FinalClassificationData>.FromBytes(bytes);

                        mCarData.CarData.MotionData = new AMMotionData();
                        mCarData.CarData.Flags = TelemetryFlags.FlagChequered;
                        mCarData.SessionInfo.SessionState = "";
                        mCarData.SessionInfo.Flag = "Chequered";

                        switch (data.m_resultStatus)
                        {
                            default:
                                mCarData.SessionInfo.FinishStatus = "";
                                mCarData.CarData.Flags = TelemetryFlags.FlagNone;
                                break;
                            case 3:
                                mCarData.SessionInfo.FinishStatus = "Finished";
                                break;
                            case 4:
                                mCarData.SessionInfo.FinishStatus = "DNF";
                                break;
                            case 5:
                                mCarData.SessionInfo.FinishStatus = "DQ";
                                break;
                            case 6:
                                mCarData.SessionInfo.FinishStatus = "NC";
                                break;
                            case 7:
                                mCarData.SessionInfo.FinishStatus = "RETIRED";
                                break;
                        }

                        OnTelemetry?.Invoke(this, new TelemetryUpdatedEventArgs(mCarData));
                    }
                    break;
                case F12025.PacketId.ePacketIdEvent:
                    {
                        var data = Marshalizable<F12025.PacketEventData>.FromBytes(bytes);

                        string code = BitConverter.ToString(data.m_eventStringCode);
                        var carData = mCarData.CarData;

                        switch (code)
                        {
                            case "DRSE": carData.Electronics |= CarElectronics.DRS_EN; break;
                            case "DRSD": carData.Electronics &= ~CarElectronics.DRS_EN; break;
                            case "CHQF": carData.Flags = TelemetryFlags.FlagChequered; break;
                        }

                        OnTelemetry?.Invoke(this, new TelemetryUpdatedEventArgs(mCarData));
                    }
                    break;
                case F12025.PacketId.ePacketIdSessionHistory:
                    {
                        var data = Marshalizable<F12025.PacketSessionHistoryData>.FromBytes(bytes);

                        if (data.m_carIdx == header.m_playerCarIndex)
                        {
                            var sessionInfo = mCarData.SessionInfo;

                            if (data.m_numLaps > 0)
                            {
                                int prevLap = data.m_numLaps - 1;
                                sessionInfo.LastSector1Time = data.m_lapHistoryData[prevLap].m_sector1TimeMinutesPart * 60000 + data.m_lapHistoryData[prevLap].m_sector1TimeMSPart;
                                sessionInfo.LastSector2Time = data.m_lapHistoryData[prevLap].m_sector2TimeMinutesPart * 60000 + data.m_lapHistoryData[prevLap].m_sector2TimeMSPart;
                                sessionInfo.LastSector3Time = data.m_lapHistoryData[prevLap].m_sector3TimeMinutesPart * 60000 + data.m_lapHistoryData[prevLap].m_sector3TimeMSPart;
                            }

                            if (data.m_bestLapTimeLapNum < data.m_numLaps)
                                sessionInfo.BestLapTime = (int)data.m_lapHistoryData[data.m_bestLapTimeLapNum].m_lapTimeInMS;
                            if (data.m_bestSector1LapNum < data.m_numLaps)
                                sessionInfo.Sector1BestTime = data.m_lapHistoryData[data.m_bestSector1LapNum].m_sector1TimeMinutesPart * 60000 + data.m_lapHistoryData[data.m_bestSector1LapNum].m_sector1TimeMSPart;
                            if (data.m_bestSector2LapNum < data.m_numLaps)
                                sessionInfo.Sector2BestTime = data.m_lapHistoryData[data.m_bestSector2LapNum].m_sector2TimeMinutesPart * 60000 + data.m_lapHistoryData[data.m_bestSector2LapNum].m_sector2TimeMSPart;
                            if (data.m_bestSector3LapNum < data.m_numLaps)
                                sessionInfo.Sector3BestTime = data.m_lapHistoryData[data.m_bestSector3LapNum].m_sector3TimeMinutesPart * 60000 + data.m_lapHistoryData[data.m_bestSector3LapNum].m_sector3TimeMSPart;
                        }
                    }
                    break;
            }
        }

        private ulong m_prevSessionUID = ulong.MaxValue;

        private void ProcessPacket2022(ref byte[] bytes)
        {
            F12022.PacketHeader header = Marshalizable<F12022.PacketHeader>.FromBytes(bytes);

            if (header.m_sessionUID != m_prevSessionUID)
            {
                mCarData.LoadDefaults();
                m_prevSessionUID = header.m_sessionUID;
            }

            switch (header.m_packetId)
            {
                case F12022.PacketID.Motion:
                    {
                        F12022.PacketMotionData data = Marshalizable<F12022.PacketMotionData>.FromBytes(bytes);

                        F12022.CarMotionData carMotionData = data.m_carMotionData[header.m_playerCarIndex];

                        AMMotionData motionData = mCarData.CarData.MotionData;

                        motionData.Position = new double[] { carMotionData.m_worldPositionX, carMotionData.m_worldPositionY, carMotionData.m_worldPositionZ };

                        motionData.Pitch = carMotionData.m_pitch / (float)Math.PI;
                        motionData.Roll = carMotionData.m_roll / (float)Math.PI;
                        motionData.Yaw = carMotionData.m_yaw / (float)Math.PI;

                        motionData.Surge = carMotionData.m_gForceLongitudinal / 3f;
                        motionData.Sway = carMotionData.m_gForceLateral / 3f;
                        motionData.Heave = carMotionData.m_gForceVertical / 3f;

                        OnTelemetry?.Invoke(this, new TelemetryUpdatedEventArgs(mCarData));
                    }
                    break;
                case F12022.PacketID.CarStatus:
                    {
                        var data = Marshalizable<F12022.PacketCarStatusData>.FromBytes(bytes);
                        var status = data.m_carStatusData[header.m_playerCarIndex];
                        var sessionInfo = mCarData.SessionInfo;
                        var carData = mCarData.CarData;

                        carData.MaxRPM = status.m_maxRPM;
                        carData.AbsLevel = status.m_antiLockBrakes;
                        carData.TcLevel = status.m_tractionControl;
                        carData.EngineMap = status.m_fuelMix;
                        carData.BrakeBias = status.m_frontBrakeBias;

                        if (status.m_drsAllowed > 0)
                            carData.Electronics |= CarElectronics.DRS_EN;
                        else
                            carData.Electronics &= ~CarElectronics.DRS_EN;

                        if (status.m_pitLimiterStatus > 0)
                            carData.Electronics |= CarElectronics.Limiter;
                        else
                            carData.Electronics &= ~CarElectronics.Limiter;

                        carData.FuelCapacity = status.m_fuelCapacity;
                        carData.FuelLevel = status.m_fuelInTank;
                        carData.FuelEstimatedLaps = status.m_fuelRemainingLaps;

                        carData.Tires = carData.Tires ?? new AMTireData[4];
                        F12022.CompoundID.TryGetValue(status.m_visualTyreCompound, out string compound);
                        carData.Tires[0].Compound = compound;
                        carData.Tires[1].Compound = compound;
                        carData.Tires[2].Compound = compound;
                        carData.Tires[3].Compound = compound;

                        F12022.FiaFlags flags = (F12022.FiaFlags)status.m_vehicleFiaFlags;

                        switch (flags)
                        {
                            default:
                            case F12022.FiaFlags.None:
                            case F12022.FiaFlags.Invalid: carData.Flags = TelemetryFlags.FlagNone; sessionInfo.Flag = ""; break;
                            case F12022.FiaFlags.Blue: carData.Flags = TelemetryFlags.FlagGreen | TelemetryFlags.FlagBlue; sessionInfo.Flag = "Blue"; break;
                            case F12022.FiaFlags.Yellow: carData.Flags = TelemetryFlags.FlagYellow; sessionInfo.Flag = "Yellow"; break;
                            case F12022.FiaFlags.Red: carData.Flags = TelemetryFlags.FlagBlack; sessionInfo.Flag = "Red"; break;
                            case F12022.FiaFlags.Green: carData.Flags = TelemetryFlags.FlagGreen; sessionInfo.Flag = "Green"; break;
                        }

                        carData.Valid = true;
                        OnTelemetry?.Invoke(this, new TelemetryUpdatedEventArgs(mCarData));
                    }
                    break;
                case F12022.PacketID.CarTelemetry:
                    {
                        var data = Marshalizable<F12022.PacketCarTelemetryData>.FromBytes(bytes);
                        var telemetry = data.m_carTelemetryData[header.m_playerCarIndex];
                        var carData = mCarData.CarData;

                        carData.Gear = (short)(telemetry.m_gear + 1);
                        carData.RPM = telemetry.m_engineRPM;
                        carData.Speed = telemetry.m_speed;
                        carData.Throttle = telemetry.m_throttle;
                        carData.Brake = telemetry.m_brake;
                        carData.Clutch = telemetry.m_clutch;
                        carData.Steering = telemetry.m_steer;

                        if (telemetry.m_drs > 0)
                            carData.Electronics |= CarElectronics.DRS;
                        else
                            carData.Electronics &= ~CarElectronics.DRS;

                        carData.Tires = carData.Tires ?? new AMTireData[4];

                        carData.Tires[0].Pressure = telemetry.m_tyresPressure[0] * 6.89476; // PSI -> kPa
                        carData.Tires[1].Pressure = telemetry.m_tyresPressure[1] * 6.89476; // PSI -> kPa
                        carData.Tires[2].Pressure = telemetry.m_tyresPressure[2] * 6.89476; // PSI -> kPa
                        carData.Tires[3].Pressure = telemetry.m_tyresPressure[3] * 6.89476; // PSI -> kPa

                        carData.Tires[0].Temperature[1] = telemetry.m_tyresInnerTemperature[0];
                        carData.Tires[1].Temperature[1] = telemetry.m_tyresInnerTemperature[1];
                        carData.Tires[2].Temperature[1] = telemetry.m_tyresInnerTemperature[2];
                        carData.Tires[3].Temperature[1] = telemetry.m_tyresInnerTemperature[3];
                        carData.Tires[0].Temperature[0] = carData.Tires[0].Temperature[2] = telemetry.m_tyresSurfaceTemperature[0];
                        carData.Tires[1].Temperature[0] = carData.Tires[1].Temperature[2] = telemetry.m_tyresSurfaceTemperature[1];
                        carData.Tires[2].Temperature[0] = carData.Tires[2].Temperature[2] = telemetry.m_tyresSurfaceTemperature[2];
                        carData.Tires[3].Temperature[0] = carData.Tires[3].Temperature[2] = telemetry.m_tyresSurfaceTemperature[3];
                        carData.Tires[0].BrakeTemperature = telemetry.m_brakesTemperature[0];
                        carData.Tires[1].BrakeTemperature = telemetry.m_brakesTemperature[1];
                        carData.Tires[2].BrakeTemperature = telemetry.m_brakesTemperature[2];
                        carData.Tires[3].BrakeTemperature = telemetry.m_brakesTemperature[3];

                        carData.OilTemp = telemetry.m_engineTemperature;

                        carData.Valid = true;
                        OnTelemetry?.Invoke(this, new TelemetryUpdatedEventArgs(mCarData));
                    }
                    break;
                case F12022.PacketID.LapData:
                    {
                        var data = Marshalizable<F12022.PacketLapData>.FromBytes(bytes);
                        var lapdata = data.m_lapData[header.m_playerCarIndex];
                        var carData = mCarData.CarData;
                        var sessionInfo = mCarData.SessionInfo;
                        sessionInfo.Sector = lapdata.m_sector;
                        sessionInfo.CurrentSector1Time = lapdata.m_sector1TimeInMS;
                        sessionInfo.CurrentSector2Time = lapdata.m_sector > 0 ? lapdata.m_sector2TimeInMS : -1;
                        sessionInfo.CurrentSector3Time = lapdata.m_sector > 1 ? (int)lapdata.m_currentLapTimeInMS - lapdata.m_sector2TimeInMS : -1;
                        sessionInfo.CurrentLapTime = (int)lapdata.m_currentLapTimeInMS;
                        sessionInfo.CurrentPosition = lapdata.m_carPosition;
                        sessionInfo.CurrentLapNumber = lapdata.m_currentLapNum;
                        sessionInfo.LastLapTime = (int)lapdata.m_lastLapTimeInMS;

                        carData.Distance = lapdata.m_lapDistance;
                        carData.InPits = lapdata.m_pitStatus == 2;

                        sessionInfo.Valid = true;
                        OnTelemetry?.Invoke(this, new TelemetryUpdatedEventArgs(mCarData));
                    }
                    break;
                case F12022.PacketID.Session:
                    {
                        var data = Marshalizable<F12022.PacketSessionData>.FromBytes(bytes);
                        var sessionInfo = mCarData.SessionInfo;
                        var weatherInfo = mCarData.WeatherData;
                        sessionInfo.TrackLength = data.m_trackLength;
                        sessionInfo.RemainingTime = data.m_sessionTimeLeft;
                        sessionInfo.TotalLapsCount = data.m_totalLaps;

                        switch (data.m_sessionType)
                        {
                            // 0 = unknown, 1 = P1, 2 = P2, 3 = P3, 4 = Short P
                            // 5 = Q1, 6 = Q2, 7 = Q3, 8 = Short Q, 9 = OSQ
                            // 10 = R, 11 = R2, 12 = R3, 13 = Time Trial

                            default:
                            case 0: sessionInfo.SessionState = ""; break;
                            case 1:
                            case 2:
                            case 3:
                            case 4: sessionInfo.SessionState = "Practice"; break;
                            case 5:
                            case 6:
                            case 7:
                            case 8:
                            case 9: sessionInfo.SessionState = "Qualifying"; break;
                            case 10:
                            case 11:
                            case 12: sessionInfo.SessionState = "Race"; break;
                            case 13: sessionInfo.SessionState = "Time Trial"; break;
                        }

                        if (data.m_trackId <= F12022.TrackID.Length)
                            sessionInfo.TrackName = F12022.TrackID[data.m_trackId];
                        else
                            sessionInfo.TrackName = "Unknown Track Name";

                        weatherInfo.AmbientTemp = data.m_airTemperature;
                        weatherInfo.TrackTemp = data.m_trackTemperature;

                        sessionInfo.Valid = true;
                        OnTelemetry?.Invoke(this, new TelemetryUpdatedEventArgs(mCarData));
                    }
                    break;
                case F12022.PacketID.Participants:
                    {
                        var data = Marshalizable<F12022.PacketParticipantsData>.FromBytes(bytes);
                        var participant = data.m_participants[header.m_playerCarIndex];
                        var carData = mCarData.CarData;

                        F12022.DriversID.TryGetValue(participant.m_driverId, out string name);
                        carData.DriverName = name ?? participant.m_name;
                        carData.CarNumber = $"{participant.m_raceNumber}";
                        F12022.TeamsID.TryGetValue(participant.m_driverId, out string team);
                        carData.CarName = team;

                        OnTelemetry?.Invoke(this, new TelemetryUpdatedEventArgs(mCarData));
                    }
                    break;
                case F12022.PacketID.FinalClassification:
                    {
                        var data = Marshalizable<F12022.FinalClassificationData>.FromBytes(bytes);

                        mCarData.CarData.MotionData = new AMMotionData();
                        mCarData.CarData.Flags = TelemetryFlags.FlagChequered;
                        mCarData.SessionInfo.SessionState = "";
                        mCarData.SessionInfo.Flag = "Chequered";

                        switch (data.m_resultStatus)
                        {
                            default:
                                mCarData.SessionInfo.FinishStatus = "";
                                mCarData.CarData.Flags = TelemetryFlags.FlagNone;
                                break;
                            case 3:
                                mCarData.SessionInfo.FinishStatus = "Finished";
                                break;
                            case 4:
                                mCarData.SessionInfo.FinishStatus = "DNF";
                                break;
                            case 5:
                                mCarData.SessionInfo.FinishStatus = "DQ";
                                break;
                            case 6:
                                mCarData.SessionInfo.FinishStatus = "NC";
                                break;
                            case 7:
                                mCarData.SessionInfo.FinishStatus = "RETIRED";
                                break;
                        }

                        OnTelemetry?.Invoke(this, new TelemetryUpdatedEventArgs(mCarData));
                    }
                    break;
                case F12022.PacketID.Event:
                    {
                        var data = Marshalizable<F12022.PacketEventData>.FromBytes(bytes);

                        string code = BitConverter.ToString(data.m_eventStringCode);
                        var carData = mCarData.CarData;

                        switch (code)
                        {
                            case "DRSE": carData.Electronics |= CarElectronics.DRS_EN; break;
                            case "DRSD": carData.Electronics &= ~CarElectronics.DRS_EN; break;
                            case "CHQF": carData.Flags = TelemetryFlags.FlagChequered; break;
                        }
                        
                        OnTelemetry?.Invoke(this, new TelemetryUpdatedEventArgs(mCarData));
                    }
                    break;
                case F12022.PacketID.SessionHistory:
                    {
                        var data = Marshalizable<F12022.PacketSessionHistoryData>.FromBytes(bytes);

                        if (data.m_carIdx == header.m_playerCarIndex)
                        {
                            var sessionInfo = mCarData.SessionInfo;

                            if (data.m_numLaps > 0)
                            {
                                int prevLap = data.m_numLaps - 1;
                                sessionInfo.LastSector1Time = data.m_lapHistoryData[prevLap].m_sector1TimeInMS;
                                sessionInfo.LastSector2Time = data.m_lapHistoryData[prevLap].m_sector2TimeInMS;
                                sessionInfo.LastSector3Time = data.m_lapHistoryData[prevLap].m_sector3TimeInMS;
                            }

                            if (data.m_bestLapTimeLapNum < data.m_numLaps)
                                sessionInfo.BestLapTime = (int)data.m_lapHistoryData[data.m_bestLapTimeLapNum].m_lapTimeInMS;
                            if (data.m_bestSector1LapNum < data.m_numLaps)
                                sessionInfo.Sector1BestTime = data.m_lapHistoryData[data.m_bestSector1LapNum].m_sector1TimeInMS;
                            if (data.m_bestSector2LapNum < data.m_numLaps)
                                sessionInfo.Sector2BestTime = data.m_lapHistoryData[data.m_bestSector2LapNum].m_sector2TimeInMS;
                            if (data.m_bestSector3LapNum < data.m_numLaps)
                                sessionInfo.Sector3BestTime = data.m_lapHistoryData[data.m_bestSector3LapNum].m_sector3TimeInMS;
                        }
                    }
                    break;
            }
        }

        private void ProcessPacket2023(ref byte[] bytes)
        {
            F12023.PacketHeader header = Marshalizable<F12023.PacketHeader>.FromBytes(bytes);

            if (header.m_sessionUID != m_prevSessionUID)
            {
                mCarData.LoadDefaults();
                m_prevSessionUID = header.m_sessionUID;
            }

            switch (header.m_packetId)
            {
                case F12023.PacketID.Motion:
                    {
                        F12023.PacketMotionData data = Marshalizable<F12023.PacketMotionData>.FromBytes(bytes);

                        F12023.CarMotionData carMotionData = data.m_carMotionData[header.m_playerCarIndex];

                        AMMotionData motionData = mCarData.CarData.MotionData;

                        motionData.Position = new double[] { carMotionData.m_worldPositionX, carMotionData.m_worldPositionY, carMotionData.m_worldPositionZ };

                        motionData.Pitch = carMotionData.m_pitch / (float)Math.PI;
                        motionData.Roll = carMotionData.m_roll / (float)Math.PI;
                        motionData.Yaw = carMotionData.m_yaw / (float)Math.PI;

                        motionData.Surge = carMotionData.m_gForceLongitudinal / 3f;
                        motionData.Sway = carMotionData.m_gForceLateral / 3f;
                        motionData.Heave = carMotionData.m_gForceVertical / 3f;

                        OnTelemetry?.Invoke(this, new TelemetryUpdatedEventArgs(mCarData));
                    }
                    break;
                case F12023.PacketID.CarStatus:
                    {
                        var data = Marshalizable<F12023.PacketCarStatusData>.FromBytes(bytes);
                        var status = data.m_carStatusData[header.m_playerCarIndex];
                        var sessionInfo = mCarData.SessionInfo;
                        var carData = mCarData.CarData;

                        carData.MaxRPM = status.m_maxRPM;
                        carData.AbsLevel = status.m_antiLockBrakes;
                        carData.TcLevel = status.m_tractionControl;
                        carData.EngineMap = status.m_fuelMix;
                        carData.BrakeBias = status.m_frontBrakeBias;

                        if (status.m_drsAllowed > 0)
                            carData.Electronics |= CarElectronics.DRS_EN;
                        else
                            carData.Electronics &= ~CarElectronics.DRS_EN;

                        if (status.m_pitLimiterStatus > 0)
                            carData.Electronics |= CarElectronics.Limiter;
                        else
                            carData.Electronics &= ~CarElectronics.Limiter;

                        carData.FuelCapacity = status.m_fuelCapacity;
                        carData.FuelLevel = status.m_fuelInTank;
                        carData.FuelEstimatedLaps = status.m_fuelRemainingLaps;

                        carData.Tires = carData.Tires ?? new AMTireData[4];
                        F12022.CompoundID.TryGetValue(status.m_visualTyreCompound, out string compound);
                        carData.Tires[0].Compound = compound;
                        carData.Tires[1].Compound = compound;
                        carData.Tires[2].Compound = compound;
                        carData.Tires[3].Compound = compound;

                        F12023.FiaFlags flags = (F12023.FiaFlags)status.m_vehicleFiaFlags;

                        switch (flags)
                        {
                            default:
                            case F12023.FiaFlags.None:
                            case F12023.FiaFlags.Invalid: carData.Flags = TelemetryFlags.FlagNone; sessionInfo.Flag = ""; break;
                            case F12023.FiaFlags.Blue: carData.Flags = TelemetryFlags.FlagGreen | TelemetryFlags.FlagBlue; sessionInfo.Flag = "Blue"; break;
                            case F12023.FiaFlags.Yellow: carData.Flags = TelemetryFlags.FlagYellow; sessionInfo.Flag = "Yellow"; break;
                            case F12023.FiaFlags.Red: carData.Flags = TelemetryFlags.FlagBlack; sessionInfo.Flag = "Red"; break;
                            case F12023.FiaFlags.Green: carData.Flags = TelemetryFlags.FlagGreen; sessionInfo.Flag = "Green"; break;
                        }

                        carData.Valid = true;
                        OnTelemetry?.Invoke(this, new TelemetryUpdatedEventArgs(mCarData));
                    }
                    break;
                case F12023.PacketID.CarTelemetry:
                    {
                        var data = Marshalizable<F12023.PacketCarTelemetryData>.FromBytes(bytes);
                        var telemetry = data.m_carTelemetryData[header.m_playerCarIndex];
                        var carData = mCarData.CarData;

                        carData.Gear = (short)(telemetry.m_gear + 1);
                        carData.RPM = telemetry.m_engineRPM;
                        carData.Speed = telemetry.m_speed;
                        carData.Throttle = telemetry.m_throttle;
                        carData.Brake = telemetry.m_brake;
                        carData.Clutch = telemetry.m_clutch;
                        carData.Steering = telemetry.m_steer;

                        if (telemetry.m_drs > 0)
                            carData.Electronics |= CarElectronics.DRS;
                        else
                            carData.Electronics &= ~CarElectronics.DRS;

                        carData.Tires = carData.Tires ?? new AMTireData[4];

                        carData.Tires[0].Pressure = telemetry.m_tyresPressure[0] * 6.89476; // PSI -> kPa
                        carData.Tires[1].Pressure = telemetry.m_tyresPressure[1] * 6.89476; // PSI -> kPa
                        carData.Tires[2].Pressure = telemetry.m_tyresPressure[2] * 6.89476; // PSI -> kPa
                        carData.Tires[3].Pressure = telemetry.m_tyresPressure[3] * 6.89476; // PSI -> kPa

                        carData.Tires[0].Temperature[1] = telemetry.m_tyresInnerTemperature[0];
                        carData.Tires[1].Temperature[1] = telemetry.m_tyresInnerTemperature[1];
                        carData.Tires[2].Temperature[1] = telemetry.m_tyresInnerTemperature[2];
                        carData.Tires[3].Temperature[1] = telemetry.m_tyresInnerTemperature[3];
                        carData.Tires[0].Temperature[0] = carData.Tires[0].Temperature[2] = telemetry.m_tyresSurfaceTemperature[0];
                        carData.Tires[1].Temperature[0] = carData.Tires[1].Temperature[2] = telemetry.m_tyresSurfaceTemperature[1];
                        carData.Tires[2].Temperature[0] = carData.Tires[2].Temperature[2] = telemetry.m_tyresSurfaceTemperature[2];
                        carData.Tires[3].Temperature[0] = carData.Tires[3].Temperature[2] = telemetry.m_tyresSurfaceTemperature[3];
                        carData.Tires[0].BrakeTemperature = telemetry.m_brakesTemperature[0];
                        carData.Tires[1].BrakeTemperature = telemetry.m_brakesTemperature[1];
                        carData.Tires[2].BrakeTemperature = telemetry.m_brakesTemperature[2];
                        carData.Tires[3].BrakeTemperature = telemetry.m_brakesTemperature[3];

                        carData.OilTemp = telemetry.m_engineTemperature;

                        carData.Valid = true;
                        OnTelemetry?.Invoke(this, new TelemetryUpdatedEventArgs(mCarData));
                    }
                    break;
                case F12023.PacketID.LapData:
                    {
                        var data = Marshalizable<F12023.PacketLapData>.FromBytes(bytes);
                        var lapdata = data.m_lapData[header.m_playerCarIndex];
                        var carData = mCarData.CarData;
                        var sessionInfo = mCarData.SessionInfo;
                        sessionInfo.Sector = lapdata.m_sector;
                        sessionInfo.CurrentSector1Time = lapdata.m_sector1TimeInMS;
                        sessionInfo.CurrentSector2Time = lapdata.m_sector > 0 ? lapdata.m_sector2TimeInMS : -1;
                        sessionInfo.CurrentSector3Time = lapdata.m_sector > 1 ? (int)lapdata.m_currentLapTimeInMS - lapdata.m_sector2TimeInMS : -1;
                        sessionInfo.CurrentLapTime = (int)lapdata.m_currentLapTimeInMS;
                        sessionInfo.CurrentPosition = lapdata.m_carPosition;
                        sessionInfo.CurrentLapNumber = lapdata.m_currentLapNum;
                        sessionInfo.LastLapTime = (int)lapdata.m_lastLapTimeInMS;

                        carData.Distance = lapdata.m_lapDistance;
                        carData.InPits = lapdata.m_pitStatus == 2;

                        sessionInfo.Valid = true;
                        OnTelemetry?.Invoke(this, new TelemetryUpdatedEventArgs(mCarData));
                    }
                    break;
                case F12023.PacketID.Session:
                    {
                        var data = Marshalizable<F12023.PacketSessionData>.FromBytes(bytes);
                        var sessionInfo = mCarData.SessionInfo;
                        var weatherInfo = mCarData.WeatherData;
                        sessionInfo.TrackLength = data.m_trackLength;
                        sessionInfo.RemainingTime = data.m_sessionTimeLeft;
                        sessionInfo.TotalLapsCount = data.m_totalLaps;

                        switch (data.m_sessionType)
                        {
                            // 0 = unknown, 1 = P1, 2 = P2, 3 = P3, 4 = Short P
                            // 5 = Q1, 6 = Q2, 7 = Q3, 8 = Short Q, 9 = OSQ
                            // 10 = R, 11 = R2, 12 = R3, 13 = Time Trial

                            default:
                            case 0: sessionInfo.SessionState = ""; break;
                            case 1:
                            case 2:
                            case 3:
                            case 4: sessionInfo.SessionState = "Practice"; break;
                            case 5:
                            case 6:
                            case 7:
                            case 8:
                            case 9: sessionInfo.SessionState = "Qualifying"; break;
                            case 10:
                            case 11:
                            case 12: sessionInfo.SessionState = "Race"; break;
                            case 13: sessionInfo.SessionState = "Time Trial"; break;
                        }

                        if (data.m_trackId <= F12023.TrackID.Length)
                            sessionInfo.TrackName = F12023.TrackID[data.m_trackId];
                        else
                            sessionInfo.TrackName = "Unknown Track Name";

                        weatherInfo.AmbientTemp = data.m_airTemperature;
                        weatherInfo.TrackTemp = data.m_trackTemperature;

                        sessionInfo.Valid = true;
                        OnTelemetry?.Invoke(this, new TelemetryUpdatedEventArgs(mCarData));
                    }
                    break;
                case F12023.PacketID.Participants:
                    {
                        var data = Marshalizable<F12023.PacketParticipantsData>.FromBytes(bytes);
                        var participant = data.m_participants[header.m_playerCarIndex];
                        var carData = mCarData.CarData;

                        F12023.DriversID.TryGetValue(participant.m_driverId, out string name);
                        carData.DriverName = name ?? participant.m_name;
                        carData.CarNumber = $"{participant.m_raceNumber}";
                        F12023.TeamsID.TryGetValue(participant.m_driverId, out string team);
                        carData.CarName = team;

                        OnTelemetry?.Invoke(this, new TelemetryUpdatedEventArgs(mCarData));
                    }
                    break;
                case F12023.PacketID.FinalClassification:
                    {
                        var data = Marshalizable<F12023.FinalClassificationData>.FromBytes(bytes);

                        mCarData.CarData.MotionData = new AMMotionData();
                        mCarData.CarData.Flags = TelemetryFlags.FlagChequered;
                        mCarData.SessionInfo.SessionState = "";
                        mCarData.SessionInfo.Flag = "Chequered";

                        switch (data.m_resultStatus)
                        {
                            default:
                                mCarData.SessionInfo.FinishStatus = "";
                                mCarData.CarData.Flags = TelemetryFlags.FlagNone;
                                break;
                            case 3:
                                mCarData.SessionInfo.FinishStatus = "Finished";
                                break;
                            case 4:
                                mCarData.SessionInfo.FinishStatus = "DNF";
                                break;
                            case 5:
                                mCarData.SessionInfo.FinishStatus = "DQ";
                                break;
                            case 6:
                                mCarData.SessionInfo.FinishStatus = "NC";
                                break;
                            case 7:
                                mCarData.SessionInfo.FinishStatus = "RETIRED";
                                break;
                        }

                        OnTelemetry?.Invoke(this, new TelemetryUpdatedEventArgs(mCarData));
                    }
                    break;
                case F12023.PacketID.Event:
                    {
                        var data = Marshalizable<F12023.PacketEventData>.FromBytes(bytes);

                        string code = BitConverter.ToString(data.m_eventStringCode);
                        var carData = mCarData.CarData;

                        switch (code)
                        {
                            case "DRSE": carData.Electronics |= CarElectronics.DRS_EN; break;
                            case "DRSD": carData.Electronics &= ~CarElectronics.DRS_EN; break;
                            case "CHQF": carData.Flags = TelemetryFlags.FlagChequered; break;
                        }

                        OnTelemetry?.Invoke(this, new TelemetryUpdatedEventArgs(mCarData));
                    }
                    break;
                case F12023.PacketID.SessionHistory:
                    {
                        var data = Marshalizable<F12023.PacketSessionHistoryData>.FromBytes(bytes);

                        if (data.m_carIdx == header.m_playerCarIndex)
                        {
                            var sessionInfo = mCarData.SessionInfo;

                            if (data.m_numLaps > 0)
                            {
                                int prevLap = data.m_numLaps - 1;
                                sessionInfo.LastSector1Time = data.m_lapHistoryData[prevLap].m_sector1TimeInMS;
                                sessionInfo.LastSector2Time = data.m_lapHistoryData[prevLap].m_sector2TimeInMS;
                                sessionInfo.LastSector3Time = data.m_lapHistoryData[prevLap].m_sector3TimeInMS;
                            }

                            if (data.m_bestLapTimeLapNum < data.m_numLaps)
                                sessionInfo.BestLapTime = (int)data.m_lapHistoryData[data.m_bestLapTimeLapNum].m_lapTimeInMS;
                            if (data.m_bestSector1LapNum < data.m_numLaps)
                                sessionInfo.Sector1BestTime = data.m_lapHistoryData[data.m_bestSector1LapNum].m_sector1TimeInMS;
                            if (data.m_bestSector2LapNum < data.m_numLaps)
                                sessionInfo.Sector2BestTime = data.m_lapHistoryData[data.m_bestSector2LapNum].m_sector2TimeInMS;
                            if (data.m_bestSector3LapNum < data.m_numLaps)
                                sessionInfo.Sector3BestTime = data.m_lapHistoryData[data.m_bestSector3LapNum].m_sector3TimeInMS;
                        }
                    }
                    break;
            }
        }

        private void ProcessPacket2024(ref byte[] bytes)
        {
            F12024.PacketHeader header = Marshalizable<F12024.PacketHeader>.FromBytes(bytes);

            if (header.m_sessionUID != m_prevSessionUID)
            {
                mCarData.LoadDefaults();
                m_prevSessionUID = header.m_sessionUID;
            }

            switch (header.m_packetId)
            {
                case F12024.PacketID.Motion:
                    {
                        F12024.PacketMotionData data = Marshalizable<F12024.PacketMotionData>.FromBytes(bytes);

                        F12024.CarMotionData carMotionData = data.m_carMotionData[header.m_playerCarIndex];

                        AMMotionData motionData = mCarData.CarData.MotionData;

                        motionData.Position = new double[] { carMotionData.m_worldPositionX, carMotionData.m_worldPositionY, carMotionData.m_worldPositionZ };

                        motionData.Pitch = carMotionData.m_pitch / (float)Math.PI;
                        motionData.Roll = carMotionData.m_roll / (float)Math.PI;
                        motionData.Yaw = carMotionData.m_yaw / (float)Math.PI;

                        motionData.Surge = carMotionData.m_gForceLongitudinal / 3f;
                        motionData.Sway = carMotionData.m_gForceLateral / 3f;
                        motionData.Heave = carMotionData.m_gForceVertical / 3f;

                        OnTelemetry?.Invoke(this, new TelemetryUpdatedEventArgs(mCarData));
                    }
                    break;
                case F12024.PacketID.CarStatus:
                    {
                        var data = Marshalizable<F12024.PacketCarStatusData>.FromBytes(bytes);
                        var status = data.m_carStatusData[header.m_playerCarIndex];
                        var sessionInfo = mCarData.SessionInfo;
                        var carData = mCarData.CarData;

                        carData.MaxRPM = status.m_maxRPM;
                        carData.AbsLevel = status.m_antiLockBrakes;
                        carData.TcLevel = status.m_tractionControl;
                        carData.EngineMap = status.m_fuelMix;
                        carData.BrakeBias = status.m_frontBrakeBias;

                        if (status.m_drsAllowed > 0)
                            carData.Electronics |= CarElectronics.DRS_EN;
                        else
                            carData.Electronics &= ~CarElectronics.DRS_EN;

                        if (status.m_pitLimiterStatus > 0)
                            carData.Electronics |= CarElectronics.Limiter;
                        else
                            carData.Electronics &= ~CarElectronics.Limiter;

                        carData.FuelCapacity = status.m_fuelCapacity;
                        carData.FuelLevel = status.m_fuelInTank;
                        carData.FuelEstimatedLaps = status.m_fuelRemainingLaps;

                        carData.Tires = carData.Tires ?? new AMTireData[4];
                        F12022.CompoundID.TryGetValue(status.m_visualTyreCompound, out string compound);
                        carData.Tires[0].Compound = compound;
                        carData.Tires[1].Compound = compound;
                        carData.Tires[2].Compound = compound;
                        carData.Tires[3].Compound = compound;

                        F12024.FiaFlags flags = (F12024.FiaFlags)status.m_vehicleFiaFlags;

                        switch (flags)
                        {
                            default:
                            case F12024.FiaFlags.None:
                            case F12024.FiaFlags.Invalid: carData.Flags = TelemetryFlags.FlagNone; sessionInfo.Flag = ""; break;
                            case F12024.FiaFlags.Blue: carData.Flags = TelemetryFlags.FlagGreen | TelemetryFlags.FlagBlue; sessionInfo.Flag = "Blue"; break;
                            case F12024.FiaFlags.Yellow: carData.Flags = TelemetryFlags.FlagYellow; sessionInfo.Flag = "Yellow"; break;
                            case F12024.FiaFlags.Red: carData.Flags = TelemetryFlags.FlagBlack; sessionInfo.Flag = "Red"; break;
                            case F12024.FiaFlags.Green: carData.Flags = TelemetryFlags.FlagGreen; sessionInfo.Flag = "Green"; break;
                        }

                        carData.Valid = true;
                        OnTelemetry?.Invoke(this, new TelemetryUpdatedEventArgs(mCarData));
                    }
                    break;
                case F12024.PacketID.CarTelemetry:
                    {
                        var data = Marshalizable<F12024.PacketCarTelemetryData>.FromBytes(bytes);
                        var telemetry = data.m_carTelemetryData[header.m_playerCarIndex];
                        var carData = mCarData.CarData;

                        carData.Gear = (short)(telemetry.m_gear + 1);
                        carData.RPM = telemetry.m_engineRPM;
                        carData.Speed = telemetry.m_speed;
                        carData.Throttle = telemetry.m_throttle;
                        carData.Brake = telemetry.m_brake;
                        carData.Clutch = telemetry.m_clutch;
                        carData.Steering = telemetry.m_steer;

                        if (telemetry.m_drs > 0)
                            carData.Electronics |= CarElectronics.DRS;
                        else
                            carData.Electronics &= ~CarElectronics.DRS;

                        carData.Tires = carData.Tires ?? new AMTireData[4];

                        carData.Tires[0].Pressure = telemetry.m_tyresPressure[0] * 6.89476; // PSI -> kPa
                        carData.Tires[1].Pressure = telemetry.m_tyresPressure[1] * 6.89476; // PSI -> kPa
                        carData.Tires[2].Pressure = telemetry.m_tyresPressure[2] * 6.89476; // PSI -> kPa
                        carData.Tires[3].Pressure = telemetry.m_tyresPressure[3] * 6.89476; // PSI -> kPa

                        carData.Tires[0].Temperature[1] = telemetry.m_tyresInnerTemperature[0];
                        carData.Tires[1].Temperature[1] = telemetry.m_tyresInnerTemperature[1];
                        carData.Tires[2].Temperature[1] = telemetry.m_tyresInnerTemperature[2];
                        carData.Tires[3].Temperature[1] = telemetry.m_tyresInnerTemperature[3];
                        carData.Tires[0].Temperature[0] = carData.Tires[0].Temperature[2] = telemetry.m_tyresSurfaceTemperature[0];
                        carData.Tires[1].Temperature[0] = carData.Tires[1].Temperature[2] = telemetry.m_tyresSurfaceTemperature[1];
                        carData.Tires[2].Temperature[0] = carData.Tires[2].Temperature[2] = telemetry.m_tyresSurfaceTemperature[2];
                        carData.Tires[3].Temperature[0] = carData.Tires[3].Temperature[2] = telemetry.m_tyresSurfaceTemperature[3];
                        carData.Tires[0].BrakeTemperature = telemetry.m_brakesTemperature[0];
                        carData.Tires[1].BrakeTemperature = telemetry.m_brakesTemperature[1];
                        carData.Tires[2].BrakeTemperature = telemetry.m_brakesTemperature[2];
                        carData.Tires[3].BrakeTemperature = telemetry.m_brakesTemperature[3];

                        carData.OilTemp = telemetry.m_engineTemperature;

                        carData.Valid = true;
                        OnTelemetry?.Invoke(this, new TelemetryUpdatedEventArgs(mCarData));
                    }
                    break;
                case F12024.PacketID.LapData:
                    {
                        var data = Marshalizable<F12024.PacketLapData>.FromBytes(bytes);
                        var lapdata = data.m_lapData[header.m_playerCarIndex];
                        var carData = mCarData.CarData;
                        var sessionInfo = mCarData.SessionInfo;
                        sessionInfo.Sector = lapdata.m_sector;
                        sessionInfo.CurrentSector1Time = (lapdata.m_sector1TimeMinutesPart * 60000) + lapdata.m_sector1TimeMSPart;
                        sessionInfo.CurrentSector2Time = lapdata.m_sector > 0 ? (lapdata.m_sector2TimeMinutesPart * 60000) + lapdata.m_sector2TimeMSPart : -1;
                        sessionInfo.CurrentSector3Time = lapdata.m_sector > 1 ? (int)lapdata.m_currentLapTimeInMS - ((lapdata.m_sector2TimeMinutesPart * 60000) + lapdata.m_sector2TimeMSPart) : -1;
                        sessionInfo.CurrentLapTime = (int)lapdata.m_currentLapTimeInMS;
                        sessionInfo.CurrentPosition = lapdata.m_carPosition;
                        sessionInfo.CurrentLapNumber = lapdata.m_currentLapNum;
                        sessionInfo.LastLapTime = (int)lapdata.m_lastLapTimeInMS;

                        carData.Distance = lapdata.m_lapDistance;
                        carData.InPits = lapdata.m_pitStatus == 2;

                        sessionInfo.Valid = true;
                        OnTelemetry?.Invoke(this, new TelemetryUpdatedEventArgs(mCarData));
                    }
                    break;
                case F12024.PacketID.Session:
                    {
                        var data = Marshalizable<F12024.PacketSessionData>.FromBytes(bytes);
                        var sessionInfo = mCarData.SessionInfo;
                        var weatherInfo = mCarData.WeatherData;
                        sessionInfo.TrackLength = data.m_trackLength;
                        sessionInfo.RemainingTime = data.m_sessionTimeLeft;
                        sessionInfo.TotalLapsCount = data.m_totalLaps;

                        switch (data.m_sessionType)
                        {
                            // 0 = unknown, 1 = P1, 2 = P2, 3 = P3, 4 = Short P
                            // 5 = Q1, 6 = Q2, 7 = Q3, 8 = Short Q, 9 = OSQ
                            // 10 = R, 11 = R2, 12 = R3, 13 = Time Trial

                            default:
                            case 0: sessionInfo.SessionState = ""; break;
                            case 1:
                            case 2:
                            case 3:
                            case 4: sessionInfo.SessionState = "Practice"; break;
                            case 5:
                            case 6:
                            case 7:
                            case 8:
                            case 9: sessionInfo.SessionState = "Qualifying"; break;
                            case 10:
                            case 11:
                            case 12: sessionInfo.SessionState = "Race"; break;
                            case 13: sessionInfo.SessionState = "Time Trial"; break;
                        }

                        if (data.m_trackId <= F12024.TrackID.Length)
                            sessionInfo.TrackName = F12024.TrackID[data.m_trackId];
                        else
                            sessionInfo.TrackName = "Unknown Track Name";

                        weatherInfo.AmbientTemp = data.m_airTemperature;
                        weatherInfo.TrackTemp = data.m_trackTemperature;

                        sessionInfo.Valid = true;
                        OnTelemetry?.Invoke(this, new TelemetryUpdatedEventArgs(mCarData));
                    }
                    break;
                case F12024.PacketID.Participants:
                    {
                        var data = Marshalizable<F12024.PacketParticipantsData>.FromBytes(bytes);
                        var participant = data.m_participants[header.m_playerCarIndex];
                        var carData = mCarData.CarData;

                        F12024.DriversID.TryGetValue(participant.m_driverId, out string name);
                        carData.DriverName = name ?? participant.m_name;
                        carData.CarNumber = $"{participant.m_raceNumber}";
                        F12024.TeamsID.TryGetValue(participant.m_driverId, out string team);
                        carData.CarName = team;

                        OnTelemetry?.Invoke(this, new TelemetryUpdatedEventArgs(mCarData));
                    }
                    break;
                case F12024.PacketID.FinalClassification:
                    {
                        var data = Marshalizable<F12024.FinalClassificationData>.FromBytes(bytes);

                        mCarData.CarData.MotionData = new AMMotionData();
                        mCarData.CarData.Flags = TelemetryFlags.FlagChequered;
                        mCarData.SessionInfo.SessionState = "";
                        mCarData.SessionInfo.Flag = "Chequered";

                        switch (data.m_resultStatus)
                        {
                            default:
                                mCarData.SessionInfo.FinishStatus = "";
                                mCarData.CarData.Flags = TelemetryFlags.FlagNone;
                                break;
                            case 3:
                                mCarData.SessionInfo.FinishStatus = "Finished";
                                break;
                            case 4:
                                mCarData.SessionInfo.FinishStatus = "DNF";
                                break;
                            case 5:
                                mCarData.SessionInfo.FinishStatus = "DQ";
                                break;
                            case 6:
                                mCarData.SessionInfo.FinishStatus = "NC";
                                break;
                            case 7:
                                mCarData.SessionInfo.FinishStatus = "RETIRED";
                                break;
                        }

                        OnTelemetry?.Invoke(this, new TelemetryUpdatedEventArgs(mCarData));
                    }
                    break;
                case F12024.PacketID.Event:
                    {
                        var data = Marshalizable<F12024.PacketEventData>.FromBytes(bytes);

                        string code = BitConverter.ToString(data.m_eventStringCode);
                        var carData = mCarData.CarData;

                        switch (code)
                        {
                            case "DRSE": carData.Electronics |= CarElectronics.DRS_EN; break;
                            case "DRSD": carData.Electronics &= ~CarElectronics.DRS_EN; break;
                            case "CHQF": carData.Flags = TelemetryFlags.FlagChequered; break;
                        }

                        OnTelemetry?.Invoke(this, new TelemetryUpdatedEventArgs(mCarData));
                    }
                    break;
                case F12024.PacketID.SessionHistory:
                    {
                        var data = Marshalizable<F12024.PacketSessionHistoryData>.FromBytes(bytes);

                        if (data.m_carIdx == header.m_playerCarIndex)
                        {
                            var sessionInfo = mCarData.SessionInfo;

                            if (data.m_numLaps > 0)
                            {
                                int prevLap = data.m_numLaps - 1;
                                sessionInfo.LastSector1Time = data.m_lapHistoryData[prevLap].m_sector1TimeMinutesPart * 60000 + data.m_lapHistoryData[prevLap].m_sector1TimeMSPart;
                                sessionInfo.LastSector2Time = data.m_lapHistoryData[prevLap].m_sector2TimeMinutesPart * 60000 + data.m_lapHistoryData[prevLap].m_sector2TimeMSPart;
                                sessionInfo.LastSector3Time = data.m_lapHistoryData[prevLap].m_sector3TimeMinutesPart * 60000 + data.m_lapHistoryData[prevLap].m_sector3TimeMSPart;
                            }

                            if (data.m_bestLapTimeLapNum < data.m_numLaps)
                                sessionInfo.BestLapTime = (int)data.m_lapHistoryData[data.m_bestLapTimeLapNum].m_lapTimeInMS;
                            if (data.m_bestSector1LapNum < data.m_numLaps)
                                sessionInfo.Sector1BestTime = data.m_lapHistoryData[data.m_bestSector1LapNum].m_sector1TimeMinutesPart * 60000 + data.m_lapHistoryData[data.m_bestSector1LapNum].m_sector1TimeMSPart;
                            if (data.m_bestSector2LapNum < data.m_numLaps)
                                sessionInfo.Sector2BestTime = data.m_lapHistoryData[data.m_bestSector2LapNum].m_sector2TimeMinutesPart * 60000 + data.m_lapHistoryData[data.m_bestSector2LapNum].m_sector2TimeMSPart;
                            if (data.m_bestSector3LapNum < data.m_numLaps)
                                sessionInfo.Sector3BestTime = data.m_lapHistoryData[data.m_bestSector3LapNum].m_sector3TimeMinutesPart * 60000 + data.m_lapHistoryData[data.m_bestSector3LapNum].m_sector3TimeMSPart;
                        }
                    }
                    break;
            }
        }

        public string[] ExecutableProcessName => new string[] { $"F1_{m_version - 2000}", $"F1_{m_version - 2000}_Trial" };

        public string UserIconPath { get => settings.ExecutableIcon; set => settings.ExecutableIcon = value; }
        public string UserExecutablePath { get => settings.ExecutableLink; set => settings.ExecutableLink = value; }

        public bool IsRunning => bIsRunning;

        public event EventHandler<TelemetryUpdatedEventArgs> OnTelemetry;
        public event EventHandler OnGameStateChanged;
        public event EventHandler OnGameIconChanged;

        private F1GameSettings settings = new F1GameSettings();
        private readonly ProcessMonitor monitor;

        public Icon GetIcon()
        {
            switch (m_version)
            {
                default:
                case 2022: return Properties.Resources.F1_22;
                case 2023: return Properties.Resources.F1_23;
                case 2024: return Properties.Resources.F1_24;
                case 2025: return Properties.Resources.F1_24;
            }
        }

        public void ShowSettings(IVAzhureRacingApp app)
        {
            // TODO:
        }

        public void Start(IVAzhureRacingApp app)
        {
            if (settings.ExecutableLink != string.Empty)
                Utils.ExecuteCmd(settings.ExecutableLink);
            else
            {
                if (!Utils.RunSteamGame(SteamGameID))
                {
                    app.SetStatusText($"Steam Service is not running. Run {Name} manually!");
                }
            }
        }

        public void Quit()
        {
            try
            {
                SaveSettings();
            }
            catch { }

            monitor?.Stop();
        }

        public class F1GameSettings
        {
            public int UPDPort { get; set; } = 20777;
            public string ExecutableLink { get; set; } = string.Empty;
            public string ExecutableIcon { get; set; } = string.Empty;
            public string ProcessName { get; set; } = string.Empty;
        }
    }
}