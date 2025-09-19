using vAzhureRacingAPI;
using Kunos.Structs;
using static Kunos.Structs.Constants;
using static KunosPlugin.GamePlugin;

using System;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Threading;
using System.Linq;

namespace KunosPlugin
{
    internal class GameListener : VAzhureSharedMemoryClient
    {
        readonly GamePlugin[] games;
        public GameListener(GamePlugin[] list) => (games) = (list);

        readonly byte[] dataPhysics = new byte[Marshal.SizeOf(typeof(SPageFilePhysics))];
        readonly byte[] dataStatic = new byte[Marshal.SizeOf(typeof(SPageFileStatic))];
        readonly byte[] dataGraphics = new byte[Marshal.SizeOf(typeof(SPageFileGraphics))];

        int[] currentSector = { -1, -1, -1 };
        int[] lastSector = { -1, -1, -1 };
        int[] bestSector = { -1, -1, -1 };

        /// <summary>
        /// Last packet ID
        /// </summary>
        private int m_packetID = -1;

        public const int cThreadInterval = 10;
        public const int cThreadWaitInterval = 1000;
        GamePlugin m_RunningGame = null;
        int brakebiasOffset = 0;
        string sTireCompound = "";

        public override void UserFunc()
        {
            GamePlugin runningGame = games.Where(x => x.IsRunning).FirstOrDefault();

            if (runningGame != null && runningGame.Enabled)
            {
                if (m_RunningGame != runningGame)
                {
                    m_RunningGame = runningGame;
                    runningGame.NotifyGameState();
                }

                try
                {
                    using (var memoryFilePhysic = MemoryMappedFile.OpenExisting(ACsharedMemoryPhysicsFile, MemoryMappedFileRights.ReadWrite))
                    using (var viewStreamPhysic = memoryFilePhysic.CreateViewStream(0L, dataPhysics.Length, MemoryMappedFileAccess.ReadWrite))
                    using (var memoryFileStatic = MemoryMappedFile.OpenExisting(ACSharedStaticFile, MemoryMappedFileRights.ReadWrite))
                    using (var viewStreamStatic = memoryFileStatic.CreateViewStream(0L, dataStatic.Length, MemoryMappedFileAccess.ReadWrite))
                    using (var memoryFileGraphics = MemoryMappedFile.OpenExisting(ACSharedGraphicsFile, MemoryMappedFileRights.ReadWrite))
                    using (var viewStreamGraphics = memoryFileGraphics.CreateViewStream(0L, dataGraphics.Length, MemoryMappedFileAccess.ReadWrite))
                    {
                        var CarData = runningGame.DataSet.CarData;
                        var SessionInfo = runningGame.DataSet.SessionInfo;
                        var MotionData = runningGame.DataSet.CarData.MotionData;
                        var WeatherData = runningGame.DataSet.WeatherData;

                        if (viewStreamStatic.SafeMemoryMappedViewHandle != null)
                        {
                            viewStreamStatic.ReadAsync(dataStatic, 0, dataStatic.Length).Wait();

                            var pageFileStatic = Marshalizable<SPageFileStatic>.FromBytes(dataStatic);

                            string carModel = pageFileStatic.CarModel;

                            CarData.DriverName = runningGame.gameID == GameID.AC ? $"{pageFileStatic.PlayerName}" : $"{pageFileStatic.PlayerName} {pageFileStatic.PlayerSurname}";
                            CarData.CarName = Plugin.sVechicleInfo.GetVehicleName(carModel);
                            CarData.FuelCapacity = pageFileStatic.MaxFuel;
                            CarData.MaxRPM = (uint)Plugin.sVechicleInfo.GetMaxRPM(carModel, pageFileStatic.MaxRpm > 0 ? pageFileStatic.MaxRpm : 8000);

                            brakebiasOffset = Plugin.sVechicleInfo.GetBiasOffset(carModel);

                            SessionInfo.TrackName = pageFileStatic.Track;
                            SessionInfo.TrackConfig = pageFileStatic.TrackConfiguration;
                            SessionInfo.TrackLength = 1;
                        }

                        CarData.Electronics = CarElectronics.None;
                        CarData.Flags = TelemetryFlags.FlagNone;

                        if (viewStreamGraphics.SafeMemoryMappedViewHandle != null)
                        {
                            viewStreamGraphics.ReadAsync(dataGraphics, 0, dataGraphics.Length).Wait();

                            var graphics = Marshalizable<SPageFileGraphics>.FromBytes(dataGraphics);

                            sTireCompound = graphics.tyreCompound;

                            SessionInfo.CurrentLapNumber = CarData.Lap = graphics.completedLaps + 1;
                            CarData.Position = graphics.position;
                            CarData.InPits = graphics.isInPitLane > 0;
                            CarData.CarNumber = "";// graphics.playerCarID.ToString();
                            CarData.Distance = graphics.normalizedCarPosition;
                            CarData.FuelConsumptionPerLap = graphics.fuelXLap;
                            CarData.FuelEstimatedLaps = graphics.fuelEstimatedLaps;

                            SessionInfo.TotalLapsCount = graphics.numberOfLaps;
                            if (m_RunningGame.gameID == GameID.ACC)
                                SessionInfo.CurrentLapTime = graphics.isValidLap != 0 ? graphics.iCurrentTime : -1;
                            else
                                SessionInfo.CurrentLapTime = graphics.iCurrentTime;
                            SessionInfo.LastLapTime = graphics.completedLaps > 0 ? graphics.iLastTime : -1;
                            SessionInfo.BestLapTime = graphics.completedLaps > 0 ? graphics.iBestTime > 30 * 60000 ? -1 : graphics.iBestTime : -1;
                            SessionInfo.RemainingTime = (int)graphics.sessionTimeLeft;
                            int remainLaps = graphics.numberOfLaps - graphics.completedLaps;
                            SessionInfo.RemainingLaps = remainLaps > 0 ? remainLaps : -1;

                            int sector = SessionInfo.Sector = graphics.currentSectorIndex + 1;

                            if (graphics.completedLaps == 0 && currentSector[0] != -1)
                            {
                                currentSector = new int[] { -1, -1, -1 };
                                lastSector = new int[] { -1, -1, -1 };
                                bestSector = new int[] { -1, -1, -1 };
                            }

                            if (graphics.iLastTime > 0 && graphics.iLastTime < int.MaxValue && graphics.completedLaps > 0)
                            {
                                if (CarData.Lap != (graphics.completedLaps + 1))
                                {
                                    currentSector.CopyTo(lastSector, 0);
                                    for (int t = 0; t < lastSector.Length; t++)
                                        bestSector[t] = lastSector[t] > 0 ? (bestSector[t] < 0 ? lastSector[t] : Math.Min(bestSector[t], lastSector[t])) : bestSector[t];
                                }
                            }

                            if (graphics.isValidLap == 0 && (currentSector[0] > 0 || currentSector[1] > 0 || currentSector[2] > 0))
                            {
                                currentSector = new int[] { -1, -1, -1 };
                            }

                            if (graphics.isValidLap != 0)
                            {
                                currentSector[0] = graphics.currentSectorIndex == 0 ? graphics.iCurrentTime : graphics.currentSectorIndex == 1 ? graphics.lastSectorTime : currentSector[0];
                                currentSector[1] = graphics.currentSectorIndex == 1 ? graphics.iCurrentTime - currentSector[0] : currentSector[1];
                                currentSector[2] = graphics.currentSectorIndex == 2 ? graphics.iCurrentTime - currentSector[0] - currentSector[1] : currentSector[2];
                            }

                            if (runningGame.gameID != GameID.AC)
                            {
                                CarData.TcLevel = (short)graphics.TC;
                                CarData.TcLevel2 = (short)graphics.TCCut;
                                CarData.AbsLevel = (short)graphics.ABS;
                                CarData.EngineMap = (short)(graphics.EngineMap + 1);
                                CarData.Electronics |= graphics.lightsStage > 0 ? CarElectronics.Headlight : CarElectronics.None;
                                CarData.DirectionsLight = (graphics.directionLightsLeft > 0 ? DirectionsLight.Left : DirectionsLight.None) |
                                                            (graphics.directionLightsRight > 0 ? DirectionsLight.Right : DirectionsLight.None);
                            }

                            SessionInfo.CurrentDelta = graphics.iDeltaLapTime;

                            CarData.Flags |= graphics.flag == AC_FLAG_TYPE.AC_YELLOW_FLAG ? TelemetryFlags.FlagYellow : TelemetryFlags.FlagNone;
                            CarData.Flags |= graphics.flag == AC_FLAG_TYPE.AC_WHITE_FLAG ? TelemetryFlags.FlagWhite : TelemetryFlags.FlagNone;
                            CarData.Flags |= graphics.flag == AC_FLAG_TYPE.AC_BLUE_FLAG ? TelemetryFlags.FlagBlue : TelemetryFlags.FlagNone;
                            CarData.Flags |= graphics.flag == AC_FLAG_TYPE.AC_BLACK_FLAG ? TelemetryFlags.FlagBlack : TelemetryFlags.FlagNone;
                            CarData.Flags |= graphics.flag == AC_FLAG_TYPE.AC_CHECKERED_FLAG ? TelemetryFlags.FlagChequered : TelemetryFlags.FlagNone;

                            switch (graphics.status)
                            {
                                default:
                                    break;
                                case AC_STATUS.AC_OFF:
                                case AC_STATUS.AC_REPLAY:
                                    SessionInfo.CurrentLapTime = -1;
                                    break;
                            }

                            switch (graphics.session)
                            {
                                case AC_SESSION_TYPE.AC_PRACTICE:
                                    SessionInfo.SessionState = "Practice";
                                    break;
                                case AC_SESSION_TYPE.AC_HOTLAP:
                                    SessionInfo.SessionState = "Hot Lap";
                                    break;
                                case AC_SESSION_TYPE.AC_DRAG:
                                    SessionInfo.SessionState = "Drag";
                                    break;
                                case AC_SESSION_TYPE.AC_QUALIFY:
                                    SessionInfo.SessionState = "Qualifying";
                                    break;
                                case AC_SESSION_TYPE.AC_RACE:
                                    SessionInfo.SessionState = "Race";
                                    break;
                                case AC_SESSION_TYPE.AC_TIME_ATTACK:
                                    SessionInfo.SessionState = "Time attack";
                                    break;
                                default:
                                    SessionInfo.SessionState = "";
                                    break;
                            }

                            if (runningGame.gameID == GameID.AC)
                            {
                                switch (graphics.flag)
                                {
                                    case AC_FLAG_TYPE.AC_BLACK_FLAG:
                                        SessionInfo.Flag = "Black";
                                        break;
                                    case AC_FLAG_TYPE.AC_BLUE_FLAG:
                                        SessionInfo.Flag = "Blue";
                                        break;
                                    case AC_FLAG_TYPE.AC_CHECKERED_FLAG:
                                        SessionInfo.Flag = "Chequered";
                                        break;
                                    case AC_FLAG_TYPE.AC_WHITE_FLAG:
                                        SessionInfo.Flag = "White";
                                        break;
                                    case AC_FLAG_TYPE.AC_YELLOW_FLAG:
                                        SessionInfo.Flag = "Yellow";
                                        break;
                                    case AC_FLAG_TYPE.AC_GREEN_FLAG:
                                        SessionInfo.Flag = "Green";
                                        break;
                                    default:
                                        SessionInfo.Flag = "";
                                        break;
                                }
                            }
                            else
                            {
                                SessionInfo.Flag = "";

                                if (graphics.GlobalGreen > 0)
                                {
                                    SessionInfo.Flag = "Green";
                                    if (CarData.Flags == TelemetryFlags.FlagNone)
                                        CarData.Flags = TelemetryFlags.FlagGreen;
                                }
                                else
                                if (graphics.GlobalYellow > 0)
                                {
                                    SessionInfo.Flag = "Yellow";
                                    if (CarData.Flags == TelemetryFlags.FlagNone)
                                        CarData.Flags = TelemetryFlags.FlagYellow;
                                }
                                else
                                if (graphics.GlobalRed > 0)
                                {
                                    SessionInfo.Flag = "Red";
                                    if (CarData.Flags == TelemetryFlags.FlagNone)
                                        CarData.Flags = TelemetryFlags.FlagPenalty;
                                }
                                else
                                if (graphics.GlobalWhite > 0)
                                {
                                    SessionInfo.Flag = "White";
                                    if (CarData.Flags == TelemetryFlags.FlagNone)
                                        CarData.Flags = TelemetryFlags.FlagWhite;
                                }
                                else
                                if (graphics.GlobalChequered > 0)
                                {
                                    SessionInfo.Flag = "Chequered";
                                    if (CarData.Flags == TelemetryFlags.FlagNone)
                                        CarData.Flags = TelemetryFlags.FlagChequered;
                                }
                            }

                            SessionInfo.FinishStatus = graphics.GlobalChequered > 0 ? "Finished" : "";
                        }

                        viewStreamPhysic.ReadAsync(dataPhysics, 0, dataPhysics.Length).Wait();

                        var physics = SPageFilePhysics.FromBytes(dataPhysics);

                        if (physics.packetId != m_packetID) // 
                            //|| MotionData.LocalAcceleration[0] != physics.accG[0] / (float)Math.PI
                            //|| MotionData.LocalAcceleration[1] != physics.accG[1] / (float)Math.PI
                            //|| MotionData.LocalAcceleration[2] != physics.accG[2] / (float)Math.PI
                        {
                            m_packetID = physics.packetId;

                            CarData.Steering = physics.steerAngle * (float)Math.PI * 2.0f;
                            CarData.Throttle = physics.gas;
                            CarData.Brake = physics.brake;
                            CarData.Clutch = 1.0f - physics.clutch;
                            CarData.Speed = physics.speedKmh;
                            CarData.RPM = (uint)physics.rpms;
                            CarData.Gear = (short)physics.gear;
                            CarData.FuelLevel = physics.fuel;

                            CarData.IgnitionStarter = (short)(physics.ignitionOn + physics.starterEngineOn);

                            MotionData.LocalAcceleration = new float[] { physics.accG[0] / 9.81f, physics.accG[1] / 9.81f, physics.accG[2] / 9.81f };
                            MotionData.LocalVelocity = physics.localVelocity;

                            MotionData.Pitch = physics.pitch / (float) Math.PI;
                            MotionData.Roll = (runningGame.gameID == GameID.ACEVO ? 1 : -1) * physics.roll / (float)Math.PI;
                            MotionData.Yaw = physics.heading / (float)Math.PI;

                            MotionData.ABSVibration = physics.absVibrations;

                            CarData.Tires = new AMTireData[]
                            {
                                new AMTireData
                                {
                                    BrakeTemperature = physics.brakeTemp[0],
                                    Pressure = physics.wheelsPressure[0] * 6.89476,
                                    Wear = 1.0f - physics.tyreWear[0] / 100f,
                                    Temperature = physics.tyreTempM[0] > 0 ?new double []
                                    {
                                        physics.tyreTempI[0],
                                        physics.tyreTempM[0],
                                        physics.tyreTempO[0],
                                    }:
                                    new double[]
                                    {
                                        physics.tyreCoreTemperature[0],
                                        physics.tyreCoreTemperature[0],
                                        physics.tyreCoreTemperature[0],
                                    },
                                    Compound = sTireCompound
                                },
                                new AMTireData
                                {
                                    BrakeTemperature = physics.brakeTemp[1],
                                    Pressure = physics.wheelsPressure[1]* 6.89476,
                                    Wear = 1.0f -physics.tyreWear[1] / 100f,
                                    Temperature = physics.tyreTempM[1] > 0 ?new double []
                                    {
                                        physics.tyreTempI[1],
                                        physics.tyreTempM[1],
                                        physics.tyreTempO[1],
                                    }:
                                    new double[]
                                    {
                                        physics.tyreCoreTemperature[1],
                                        physics.tyreCoreTemperature[1],
                                        physics.tyreCoreTemperature[1],
                                    },
                                    Compound = sTireCompound
                                },
                                new AMTireData
                                {
                                    BrakeTemperature = physics.brakeTemp[2],
                                    Pressure = physics.wheelsPressure[2]* 6.89476,
                                    Wear = 1.0f - physics.tyreWear[2]/ 100f,
                                    Temperature = physics.tyreTempM[2] > 0 ?new double []
                                    {
                                        physics.tyreTempI[2],
                                        physics.tyreTempM[2],
                                        physics.tyreTempO[2],
                                    }:
                                    new double[]
                                    {
                                        physics.tyreCoreTemperature[2],
                                        physics.tyreCoreTemperature[2],
                                        physics.tyreCoreTemperature[2],
                                    },
                                    Compound = sTireCompound
                                },
                                new AMTireData
                                {
                                    BrakeTemperature = physics.brakeTemp[3],
                                    Pressure = physics.wheelsPressure[3]* 6.89476,
                                    Wear = 1.0f - physics.tyreWear[3]/ 100f,
                                    Temperature = physics.tyreTempM[3] > 0 ?new double []
                                    {
                                        physics.tyreTempI[3],
                                        physics.tyreTempM[3],
                                        physics.tyreTempO[3],
                                    }:
                                    new double[]
                                    {
                                        physics.tyreCoreTemperature[3],
                                        physics.tyreCoreTemperature[3],
                                        physics.tyreCoreTemperature[3],
                                    },
                                    Compound = sTireCompound
                                },
                            };

                            CarData.BrakeBias = physics.brakeBias * 100.0f + brakebiasOffset;
                            WeatherData.AmbientTemp = physics.airTemp;
                            WeatherData.TrackTemp = physics.roadTemp;

                            CarData.Electronics |= (physics.pitLimiterOn > 0 ? CarElectronics.Limiter : CarElectronics.None) |
                                (physics.drs > 0 ? CarElectronics.DRS : CarElectronics.None);

                            CarData.Electronics |= physics.tcinAction > 0 ? CarElectronics.TCS : CarElectronics.None;
                            CarData.Electronics |= physics.absInAction > 0 ? CarElectronics.ABS : CarElectronics.None;

                            SessionInfo.CurrentPosition = CarData.Position;
                            SessionInfo.LastSector1Time = lastSector[0];
                            SessionInfo.LastSector2Time = lastSector[1];
                            SessionInfo.LastSector3Time = lastSector[2];
                            SessionInfo.Sector1BestTime = bestSector[0];
                            SessionInfo.Sector2BestTime = bestSector[1];
                            SessionInfo.Sector3BestTime = bestSector[2];
                            SessionInfo.CurrentSector1Time = currentSector[0];
                            SessionInfo.CurrentSector2Time = currentSector[1];
                            SessionInfo.CurrentSector3Time = currentSector[2];
                            SessionInfo.PitSpeedLimit = 50;

                            CarData.Valid = true;
                            SessionInfo.Valid = true;
                            WeatherData.Valid = true;

                            runningGame.NotifyTelemetry();
                        }
                    }

                    Thread.Sleep(cThreadInterval);
                }
                catch { Thread.Sleep(cThreadWaitInterval); }
            }
            else
            {
                if (m_RunningGame != null)
                {
                    m_RunningGame.NotifyGameState();
                    m_RunningGame = null;
                    m_packetID = -1;
                    currentSector = new int[] { 1, -1, -1 };
                    lastSector = new int[] { 1, -1, -1 };
                    bestSector = new int[] { -1, -1, -1 };
                }
                Thread.Sleep(cThreadWaitInterval);
            }
        }
    }
}
