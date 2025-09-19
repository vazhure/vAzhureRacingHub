using System;
using System.Drawing;
using System.Text;
using vAzhureRacingAPI;
using RaceRoom.Structs;
using System.IO.MemoryMappedFiles;
using System.IO;
using System.Threading;

namespace RaceRoomPlugin
{
    internal class R3EListener : VAzhureSharedMemoryClient, IGamePlugin
    {
        TelemetryDataSet dataSet;

        public R3EListener()
        {
            dataSet = new TelemetryDataSet(this);
        }

        public bool Enabled { get; set; } = true;

        public string Name => "RaceRoom";

        public uint SteamGameID => 211500U;

        public string[] ExecutableProcessName => new string[] { "RRRE64", "RRRE" };

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

        const int cThreadInterval = 10;

        public event EventHandler<TelemetryUpdatedEventArgs> OnTelemetry;
        public event EventHandler OnGameStateChanged;
        public event EventHandler OnGameIconChanged;

        readonly byte[] data = new byte[RaceRoomSharedMemory.Length()];
        private bool bGameRunning = false;

        public bool IsRunning
        {
            get
            {
                return Utils.IsProcessRunning(ExecutableProcessName);
            }
        }

        private bool bEmptyTelemetry = false;

        public override void UserFunc()
        {
            if (IsRunning && Enabled)
            {
                if (!bGameRunning)
                {
                    bGameRunning = true;
                    OnGameStateChanged?.Invoke(this, new EventArgs());
                }

                try
                {
                    using (var memoryFile = MemoryMappedFile.OpenExisting(Constant.SharedMemoryName, MemoryMappedFileRights.Read, HandleInheritability.None))
                    using (var viewStream = memoryFile.CreateViewStream(0L, data.Length, MemoryMappedFileAccess.Read))
                    {
                        viewStream.ReadAsync(data, 0, data.Length).Wait();
                        var pageFileContent = Marshalizable<RaceRoomSharedMemory>.FromBytes(data);

                        bool bInGame = pageFileContent.GamePaused == 0 &&
                            pageFileContent.GameInReplay == 0 &&
                            pageFileContent.GameInMenus == 0 &&
                            pageFileContent.ControlType == (int)Constant.Control.Player;

                        if (bInGame)
                        {
                            // Dash
                            dataSet.CarData.Lap = pageFileContent.CompletedLaps + 1;
                            dataSet.CarData.Position = pageFileContent.Position;
                            dataSet.CarData.Speed = pageFileContent.CarSpeed * 3.6f;
                            dataSet.CarData.RPM = pageFileContent.EngineRps * (60 / (2 * (Single)Math.PI));
                            dataSet.CarData.MaxRPM = pageFileContent.MaxEngineRps * (60 / (2 * (Single)Math.PI));
                            dataSet.CarData.ShiftUpRPM = pageFileContent.UpshiftRps * (60 / (2 * (Single)Math.PI));
                            dataSet.CarData.Gear = (short)((pageFileContent.Gear < -1 ? -1 : pageFileContent.Gear) + 1); // R is -1
                            dataSet.CarData.FuelLevel = pageFileContent.FuelLeft;
                            dataSet.CarData.FuelConsumptionPerLap = pageFileContent.FuelPerLap;
                            dataSet.CarData.AbsLevel = (short)(pageFileContent.AidSettings.Abs > 0 ? 1 : 0);
                            dataSet.CarData.TcLevel = (short)(pageFileContent.AidSettings.Tc > 0 ? 1 : 0);
                            dataSet.CarData.DriverName = BytesToString(pageFileContent.PlayerName);
                            dataSet.CarData.CarName = BytesToString(pageFileContent.VehicleInfo.Name);
                            dataSet.CarData.CarNumber = pageFileContent.VehicleInfo.CarNumber.ToString();
                            dataSet.CarData.BrakeBias = pageFileContent.BrakeBias >= 0 ? pageFileContent.BrakeBias * 100.0f : 50;
                            dataSet.CarData.Brake = pageFileContent.BrakeRaw > 0 ? pageFileContent.BrakeRaw : 0;
                            dataSet.CarData.Throttle = pageFileContent.ThrottleRaw > 0 ? pageFileContent.ThrottleRaw : 0;
                            dataSet.CarData.Clutch = pageFileContent.ClutchRaw > 0 ? pageFileContent.ClutchRaw : 0;
                            dataSet.CarData.Steering = 0.5f * pageFileContent.SteerInputRaw * (float)(pageFileContent.SteerWheelRangeDegrees * Math.PI / 180.0f);
                            dataSet.CarData.OilTemp = pageFileContent.EngineOilTemp;
                            dataSet.CarData.OilPressure = pageFileContent.EngineOilPressure;
                            dataSet.CarData.WaterTemp = pageFileContent.EngineWaterTemp;
                            dataSet.CarData.Distance = pageFileContent.LapDistance;
                            dataSet.CarData.InPits = pageFileContent.InPitlane > 0;
                            dataSet.CarData.PitState = (short) pageFileContent.PitState;

                            dataSet.WeatherData.AmbientTemp = 21;
                            dataSet.WeatherData.TrackTemp = 29;

                            dataSet.SessionInfo.DriversCount = pageFileContent.NumCars;
                            dataSet.SessionInfo.TotalLapsCount = pageFileContent.NumberOfLaps;
                            dataSet.SessionInfo.CurrentLapTime = (int)(pageFileContent.LapTimeCurrentSelf * 1000.0f); // seconds to milliseconds, -1 - undefined
                            dataSet.SessionInfo.LastLapTime = (int)(pageFileContent.LapTimePreviousSelf * 1000.0f);
                            dataSet.SessionInfo.BestLapTime = (int)(pageFileContent.LapTimeBestSelf * 1000.0f);
                            dataSet.SessionInfo.RemainingTime = (int)(pageFileContent.SessionTimeRemaining * 1000);
                            dataSet.SessionInfo.RemainingLaps = pageFileContent.NumberOfLaps - pageFileContent.CompletedLaps;
                            dataSet.SessionInfo.CurrentDelta = (int)(pageFileContent.TimeDeltaBestSelf * 1000);
                            dataSet.SessionInfo.TrackLength = pageFileContent.LayoutLength;
                            dataSet.SessionInfo.PitSpeedLimit = pageFileContent.SessionPitSpeedLimit * 3.6; ;

                            int sector = pageFileContent.TrackSector == 0 ? 3 : pageFileContent.TrackSector;

                            dataSet.SessionInfo.Sector = sector;
                            dataSet.CarData.Lap = dataSet.SessionInfo.CurrentLapNumber = pageFileContent.CompletedLaps + 1;
                            dataSet.SessionInfo.BestLapTime = (int)(pageFileContent.LapTimePreviousSelf * 1000.0f);
                            dataSet.SessionInfo.Sector1BestTime = (int)(pageFileContent.SectorTimesSessionBestLap.Sector1 * 1000);
                            dataSet.SessionInfo.Sector2BestTime = (int)((pageFileContent.SectorTimesSessionBestLap.Sector2 - pageFileContent.SectorTimesSessionBestLap.Sector1) * 1000);
                            dataSet.SessionInfo.Sector3BestTime = (int)((pageFileContent.SectorTimesSessionBestLap.Sector3 - pageFileContent.SectorTimesSessionBestLap.Sector2) * 1000);

                            dataSet.SessionInfo.CurrentSector1Time = sector == 1 ?
                                (int)(pageFileContent.LapTimeCurrentSelf * 1000.0f) :
                                (int)(pageFileContent.SectorTimesCurrentSelf.Sector1 * 1000);
                            dataSet.SessionInfo.CurrentSector2Time = sector == 2 && pageFileContent.SectorTimesCurrentSelf.Sector1 > 0 ?
                                (int)((pageFileContent.LapTimeCurrentSelf - pageFileContent.SectorTimesCurrentSelf.Sector1) * 1000.0f) :
                                (int)((pageFileContent.SectorTimesCurrentSelf.Sector2 - pageFileContent.SectorTimesCurrentSelf.Sector1) * 1000);
                            dataSet.SessionInfo.CurrentSector3Time = sector == 3 && pageFileContent.SectorTimesCurrentSelf.Sector1 > 0
                                && pageFileContent.SectorTimesCurrentSelf.Sector2 > 0 ?
                                (int)((pageFileContent.LapTimeCurrentSelf - pageFileContent.SectorTimesCurrentSelf.Sector2) * 1000.0f) :
                                (int)((pageFileContent.SectorTimesCurrentSelf.Sector3 - pageFileContent.SectorTimesCurrentSelf.Sector2) * 1000);
                            dataSet.SessionInfo.LastLapTime = (int)(pageFileContent.LapTimePreviousSelf * 1000.0f);
                            dataSet.SessionInfo.LastSector1Time = (int)(pageFileContent.SectorTimesPreviousSelf.Sector1 * 1000);
                            dataSet.SessionInfo.LastSector2Time = (int)((pageFileContent.SectorTimesPreviousSelf.Sector2 - pageFileContent.SectorTimesPreviousSelf.Sector1) * 1000);
                            dataSet.SessionInfo.LastSector3Time = (int)((pageFileContent.SectorTimesPreviousSelf.Sector3 - pageFileContent.SectorTimesPreviousSelf.Sector2) * 1000);
                            dataSet.SessionInfo.TrackName = BytesToString(pageFileContent.TrackName);
                            dataSet.SessionInfo.TrackConfig = BytesToString(pageFileContent.LayoutName);
                            dataSet.SessionInfo.DriversCount = pageFileContent.NumCars;
                            dataSet.SessionInfo.CurrentPosition = pageFileContent.Position;
                            dataSet.SessionInfo.CurrentLapTime = (int)(pageFileContent.LapTimeCurrentSelf * 1000.0f);
                            dataSet.SessionInfo.TotalLapsCount = pageFileContent.NumberOfLaps;
                            dataSet.SessionInfo.RemainingTime = (int)(pageFileContent.SessionTimeRemaining * 1000);
                            dataSet.SessionInfo.CurrentDelta = (int)(pageFileContent.TimeDeltaBestSelf * 1000);
                            dataSet.SessionInfo.FinishStatus = pageFileContent.FinishStatus == (int)Constant.FinishStatus.Finished ? "Finished" :
                                pageFileContent.FinishStatus == (int)Constant.FinishStatus.DNF ? "DNF" :
                                pageFileContent.FinishStatus == (int)Constant.FinishStatus.DQ ? "DQ" :
                                pageFileContent.FinishStatus == (int)Constant.FinishStatus.DNS ? "DNS" :
                                pageFileContent.FinishStatus == (int)Constant.FinishStatus.DNQ ? "DNQ" : "";

                            switch (pageFileContent.SessionType)
                            {
                                case (int)Constant.Session.Practice:
                                    dataSet.SessionInfo.SessionState = "Practice";
                                    break;
                                case (int)Constant.Session.Race:
                                    dataSet.SessionInfo.SessionState = "Race";
                                    break;
                                case (int)Constant.Session.Qualify:
                                    dataSet.SessionInfo.SessionState = "Qualifying";
                                    break;
                                case (int)Constant.Session.Warmup:
                                    dataSet.SessionInfo.SessionState = "Warmup";
                                    break;
                            }

                            dataSet.CarData.Tires = new AMTireData[]
                            {
                                new AMTireData
                                {
                                    Wear = 1f -pageFileContent.TireWear.FrontLeft,
                                    BrakeTemperature = pageFileContent.BrakeTemp.FrontLeft.CurrentTemp,
                                    Pressure = pageFileContent.TirePressure.FrontLeft,
                                    Temperature = new double []
                                    {
                                        pageFileContent.TireTemp.FrontLeft.CurrentTemp.Left,
                                        pageFileContent.TireTemp.FrontLeft.CurrentTemp.Center,
                                        pageFileContent.TireTemp.FrontLeft.CurrentTemp.Right,
                                    }
                                },
                                new AMTireData
                                {
                                    Wear = 1f -pageFileContent.TireWear.FrontRight,
                                    BrakeTemperature = pageFileContent.BrakeTemp.FrontRight.CurrentTemp,
                                    Pressure = pageFileContent.TirePressure.FrontLeft,
                                    Temperature = new double []
                                    {
                                        pageFileContent.TireTemp.FrontRight.CurrentTemp.Left,
                                        pageFileContent.TireTemp.FrontRight.CurrentTemp.Center,
                                        pageFileContent.TireTemp.FrontRight.CurrentTemp.Right,
                                    }
                                },
                                new AMTireData
                                {
                                    Wear = 1f -pageFileContent.TireWear.RearLeft,
                                    BrakeTemperature = pageFileContent.BrakeTemp.RearLeft.CurrentTemp,
                                    Pressure = pageFileContent.TirePressure.RearLeft,
                                    Temperature = new double []
                                    {
                                        pageFileContent.TireTemp.RearLeft.CurrentTemp.Left,
                                        pageFileContent.TireTemp.RearLeft.CurrentTemp.Center,
                                        pageFileContent.TireTemp.RearLeft.CurrentTemp.Right,
                                    }
                                },
                                new AMTireData
                                {
                                    Wear = 1f -pageFileContent.TireWear.RearRight,
                                    BrakeTemperature = pageFileContent.BrakeTemp.RearRight.CurrentTemp,
                                    Pressure = pageFileContent.TirePressure.RearLeft,
                                    Temperature = new double []
                                    {
                                        pageFileContent.TireTemp.RearRight.CurrentTemp.Left,
                                        pageFileContent.TireTemp.RearRight.CurrentTemp.Center,
                                        pageFileContent.TireTemp.RearRight.CurrentTemp.Right,
                                    }
                                },
                            };

                            switch ((Constant.TireSubtype)pageFileContent.TireSubtypeFront)
                            {
                                case Constant.TireSubtype.Hard:
                                    dataSet.CarData.Tires[0].Compound = "Hard";
                                    dataSet.CarData.Tires[1].Compound = "Hard";
                                    break;
                                case Constant.TireSubtype.Soft:
                                    dataSet.CarData.Tires[0].Compound = "Soft";
                                    dataSet.CarData.Tires[1].Compound = "soft";
                                    break;
                                case Constant.TireSubtype.Unavailable:
                                    dataSet.CarData.Tires[0].Compound = "";
                                    dataSet.CarData.Tires[1].Compound = "";
                                    break;
                                case Constant.TireSubtype.Medium:
                                    dataSet.CarData.Tires[0].Compound = "Medium";
                                    dataSet.CarData.Tires[1].Compound = "Medium";
                                    break;
                                case Constant.TireSubtype.Primary:
                                    dataSet.CarData.Tires[0].Compound = "Primary";
                                    dataSet.CarData.Tires[1].Compound = "Primary";
                                    break;
                                case Constant.TireSubtype.Alternate:
                                    dataSet.CarData.Tires[0].Compound = "Alternate";
                                    dataSet.CarData.Tires[1].Compound = "Alternate";
                                    break;
                            }

                            switch ((Constant.TireSubtype)pageFileContent.TireSubtypeRear)
                            {
                                case Constant.TireSubtype.Hard:
                                    dataSet.CarData.Tires[2].Compound = "Hard";
                                    dataSet.CarData.Tires[3].Compound = "Hard";
                                    break;
                                case Constant.TireSubtype.Soft:
                                    dataSet.CarData.Tires[2].Compound = "Soft";
                                    dataSet.CarData.Tires[3].Compound = "soft";
                                    break;
                                case Constant.TireSubtype.Unavailable:
                                    dataSet.CarData.Tires[2].Compound = "";
                                    dataSet.CarData.Tires[3].Compound = "";
                                    break;
                                case Constant.TireSubtype.Medium:
                                    dataSet.CarData.Tires[2].Compound = "Medium";
                                    dataSet.CarData.Tires[3].Compound = "Medium";
                                    break;
                                case Constant.TireSubtype.Primary:
                                    dataSet.CarData.Tires[2].Compound = "Primary";
                                    dataSet.CarData.Tires[3].Compound = "Primary";
                                    break;
                                case Constant.TireSubtype.Alternate:
                                    dataSet.CarData.Tires[2].Compound = "Alternate";
                                    dataSet.CarData.Tires[3].Compound = "Alternate";
                                    break;
                            }

                            dataSet.CarData.Electronics = (pageFileContent.PitLimiter > 0 ? CarElectronics.Limiter : CarElectronics.None);
                            dataSet.CarData.Electronics |= (pageFileContent.Drs.Engaged > 0 ? CarElectronics.DRS : CarElectronics.None);
                            dataSet.CarData.Electronics |= (pageFileContent.Drs.Equipped > 0 ? CarElectronics.DRS_EN: CarElectronics.None);
                            dataSet.CarData.Electronics |= (pageFileContent.AidSettings.Abs == 5 ? CarElectronics.ABS : CarElectronics.None);
                            dataSet.CarData.Electronics |= (pageFileContent.AidSettings.Tc == 5 ? CarElectronics.TCS : CarElectronics.None);
                            dataSet.CarData.Flags = TelemetryFlags.FlagNone;

                            if (pageFileContent.SessionPhase == (int)Constant.SessionPhase.Checkered)
                            {
                                dataSet.CarData.Flags = TelemetryFlags.FlagChequered;
                                dataSet.SessionInfo.Flag = "Chequered";
                            }

                            if (pageFileContent.SessionPhase == (int)Constant.SessionPhase.Green)
                            {
                                dataSet.CarData.Flags = TelemetryFlags.FlagGreen;
                                dataSet.SessionInfo.Flag = "Green";
                            }

                            if (pageFileContent.Flags.Blue == 1)
                            {
                                dataSet.CarData.Flags = TelemetryFlags.FlagBlue;
                                dataSet.SessionInfo.Flag = "Blue";
                            }

                            if (pageFileContent.Flags.Yellow == 1)
                            {
                                dataSet.CarData.Flags = TelemetryFlags.FlagYellow;
                                dataSet.SessionInfo.Flag = "Yellow";
                            }

                            if (pageFileContent.Flags.Black == 1)
                            {
                                dataSet.CarData.Flags = TelemetryFlags.FlagBlack;
                                dataSet.SessionInfo.Flag = "Black";
                            }

                            if (pageFileContent.Flags.White == 1)
                            {
                                dataSet.CarData.Flags = TelemetryFlags.FlagWhite;
                                dataSet.SessionInfo.Flag = "White";
                            }

                            dataSet.CarData.MotionData.Pitch = pageFileContent.CarOrientation.Pitch / (float)Math.PI;
                            dataSet.CarData.MotionData.Roll = pageFileContent.CarOrientation.Roll / (float)Math.PI;
                            dataSet.CarData.MotionData.Yaw = pageFileContent.CarOrientation.Yaw / (float)Math.PI;
                            dataSet.CarData.MotionData.Sway = pageFileContent.LocalAcceleration.X / 30f;
                            dataSet.CarData.MotionData.Heave = pageFileContent.LocalAcceleration.Y / 30f;
                            dataSet.CarData.MotionData.Surge = -pageFileContent.LocalAcceleration.Z / 30f;

                            dataSet.CarData.MotionData.LocalRotAcceleration = new float[] { (float)pageFileContent.Player.AngularAcceleration.X,
                                (float)pageFileContent.Player.AngularAcceleration.Y, (float)pageFileContent.Player.AngularAcceleration.Z };
                            dataSet.CarData.MotionData.LocalVelocity = new float[] { (float)pageFileContent.Player.LocalVelocity.X,
                                (float)pageFileContent.Player.LocalVelocity.Y, (float)pageFileContent.Player.LocalVelocity.Z };
                            dataSet.CarData.MotionData.LocalRot = new float[] { (float)pageFileContent.Player.Rotation.X,
                                (float)pageFileContent.Player.Rotation.Y, (float)pageFileContent.Player.Rotation.Z };

                            dataSet.CarData.Valid = true;
                            dataSet.SessionInfo.Valid = true;
                            dataSet.WeatherData.Valid = true;

                            bEmptyTelemetry = false;
                            OnTelemetry(this, new TelemetryUpdatedEventArgs(dataSet));
                        }
                        else
                        {
                            if (!bEmptyTelemetry)
                            {
                                bEmptyTelemetry = true;
                                dataSet = new TelemetryDataSet(this);
                                OnTelemetry(this, new TelemetryUpdatedEventArgs(dataSet));
                                Thread.Sleep(cThreadInterval * 10); // Not too fast.. Processor high load
                                return;
                            }
                        }
                    }
                }
                catch { }
                Thread.Sleep(cThreadInterval);
            }
            else
            {
                if (bGameRunning)
                {
                    bGameRunning = false;
                    OnGameStateChanged(this, new EventArgs());
                }

                Thread.Sleep(1000);
            }
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

        public void Start(IVAzhureRacingApp app)
        {
            if (!Utils.RunSteamGame(SteamGameID))
            {
                app.SetStatusText($"Steam Service is not running. Run {Name} manually!");
            }
        }

        public void ShowSettings(IVAzhureRacingApp app)
        {
            // TODO
        }

        public Icon GetIcon()
        {
            return Properties.Resources.RRRE;
        }
    }
}
