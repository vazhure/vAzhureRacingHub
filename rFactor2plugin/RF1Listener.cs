using System;
using System.Drawing;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using vAzhureRacingAPI;

namespace rFactor2plugin
{
    internal class RF1Listener : VAzhureSharedMemoryClient
    {
        readonly byte[] data = new byte[Marshal.SizeOf(typeof(RFactor1.RfShared))];

        readonly TelemetryDataSet dataSet;
        public RF1Listener(RF1Game game)
        {
            dataSet = new TelemetryDataSet(game);
        }

        public override void UserFunc()
        {
            try
            {
                using (var memoryFile = MemoryMappedFile.OpenExisting(RFactor1.RF_SHARED_MEMORY_NAME, MemoryMappedFileRights.Read))
                using (var viewStream = memoryFile.CreateViewStream(0L, data.Length, MemoryMappedFileAccess.Read))
                {
                    viewStream.ReadAsync(data, 0, data.Length);

                    AMCarData carData = dataSet.CarData;
                    AMSessionInfo sessionInfo = dataSet.SessionInfo;
                    AMWeatherData weatherData = dataSet.WeatherData;
                    AMMotionData motionData = carData.MotionData;

                    RFactor1.RfShared rfShared = Marshalizable<RFactor1.RfShared>.FromBytes(data);

                    if (rfShared.gamePhase != RFactor1.RfGamePhase.garage)
                    {
                        switch (rfShared.gamePhase)
                        {
                            case RFactor1.RfGamePhase.warmUp: sessionInfo.SessionState = "Warmup"; break;
                            case RFactor1.RfGamePhase.formation: sessionInfo.SessionState = "Formation"; break;
                            case RFactor1.RfGamePhase.greenFlag: sessionInfo.SessionState = "Green"; break;
                            case RFactor1.RfGamePhase.fullCourseYellow: sessionInfo.SessionState = "FCY"; break;
                            case RFactor1.RfGamePhase.countdown: sessionInfo.SessionState = "Countdown"; break;
                            case RFactor1.RfGamePhase.sessionOver: sessionInfo.SessionState = "Over"; break;
                            case RFactor1.RfGamePhase.sessionStopped: sessionInfo.SessionState = "Stopped"; break;
                            default: sessionInfo.SessionState = ""; break;
                        }

                        sessionInfo.DriversCount = rfShared.numVehicles;

                        if (rfShared.vehicle.Where(o => o.isPlayer).FirstOrDefault() is RFactor1.RfVehicleInfo vi)
                        {
                            carData.DriverName = vi.driverName;
                            carData.CarName = carData.CarClass = vi.vehicleClass;
                            sessionInfo.Sector = vi.sector == 0 ? 3 : vi.sector;
                            sessionInfo.CurrentPosition = vi.place;

                            sessionInfo.BestLapTime = (int)(vi.bestLapTime * 1000f);
                            sessionInfo.Sector1BestTime = (int)(vi.bestSector1 * 1000f);
                            sessionInfo.Sector2BestTime = (int)(vi.bestSector2 * 1000f);
                            sessionInfo.LastLapTime = (int)(vi.lastLapTime * 1000f);

                            carData.InPits = vi.inPits;

                            switch ( rfShared.sectorFlag[vi.sector])
                            {
                                case RFactor1.RfYellowFlagState.noFlag: carData.Flags = TelemetryFlags.FlagNone; break;
                                case RFactor1.RfYellowFlagState.lastLap: carData.Flags = TelemetryFlags.FlagWhite; break;
                                case RFactor1.RfYellowFlagState.resume: carData.Flags = TelemetryFlags.FlagGreen; break;
                                case RFactor1.RfYellowFlagState.raceHalt: carData.Flags = TelemetryFlags.FlagYellow; break;
                            }
                            
                            switch (vi.finishStatus)
                            {
                                case RFactor1.RfFinishStatus.finished:  sessionInfo.FinishStatus = "Finished"; break;
                                case RFactor1.RfFinishStatus.dnf: sessionInfo.FinishStatus = "DNF"; break;
                                case RFactor1.RfFinishStatus.dq: sessionInfo.FinishStatus = "DQ"; break;
                                default: sessionInfo.FinishStatus = ""; break;
                            }
                        }

                        carData.Clutch = rfShared.unfilteredClutch;
                        carData.Throttle = rfShared.unfilteredThrottle;
                        carData.Brake = rfShared.unfilteredBrake;
                        carData.Steering = rfShared.unfilteredSteering;
                        carData.FuelLevel = rfShared.fuel;
                        carData.Gear = (short)(rfShared.gear + 1);
                        carData.DriverName = rfShared.playerName;
                        carData.Lap = rfShared.lapNumber + 1;
                        carData.Speed = rfShared.speed * 3.6f;
                        carData.RPM = rfShared.engineRPM;
                        carData.ShiftUpRPM = carData.MaxRPM = rfShared.engineMaxRPM;
                        carData.WaterTemp = rfShared.engineWaterTemp;
                        carData.OilTemp = rfShared.engineOilTemp;
                        carData.Distance = rfShared.lapDist;

                        motionData.Pitch = rfShared.localRot.x / (float)Math.PI; // Check !
                        motionData.Roll = rfShared.localRot.z / (float)Math.PI; // Check !
                        motionData.Yaw = rfShared.localRot.y / (float)Math.PI; // Check !

                        motionData.Sway = 0.3f * rfShared.localAccel.x / 9.81f;
                        motionData.Surge = 0.3f * -rfShared.localAccel.z / 9.81f;
                        motionData.Heave = 0.3f * rfShared.localAccel.y / 9.81f;
                        
                        motionData.Position = rfShared.pos;
                        motionData.LocalVelocity = rfShared.localVel;
                        motionData.LocalRotAcceleration = rfShared.localRotAccel;
                        motionData.LocalRot = rfShared.localRot;

                        carData.Tires = new AMTireData[]
                        {
                            new AMTireData
                            {
                                BrakeTemperature = rfShared.wheel[0].brakeTemp,
                                Pressure = rfShared.wheel[0].pressure,
                                Wear = 1.0f - rfShared.wheel[0].wear,
                                Temperature = new double []
                                {
                                    rfShared.wheel[0].temperature[0],
                                    rfShared.wheel[0].temperature[1],
                                    rfShared.wheel[0].temperature[2],
                                },
                                Detached = rfShared.wheel[0].detached
                            },
                            new AMTireData
                            {
                                BrakeTemperature = rfShared.wheel[1].brakeTemp,
                                Pressure = rfShared.wheel[1].pressure,
                                Wear = 1.0f - rfShared.wheel[1].wear,
                                Temperature = new double []
                                {
                                    rfShared.wheel[1].temperature[0],
                                    rfShared.wheel[1].temperature[1],
                                    rfShared.wheel[1].temperature[2],
                                },
                                Detached = rfShared.wheel[0].detached
                            },
                            new AMTireData
                            {
                                BrakeTemperature = rfShared.wheel[2].brakeTemp,
                                Pressure = rfShared.wheel[2].pressure,
                                Wear = 1.0f - rfShared.wheel[0].wear,
                                Temperature = new double []
                                {
                                    rfShared.wheel[2].temperature[0],
                                    rfShared.wheel[2].temperature[1],
                                    rfShared.wheel[2].temperature[2],
                                },
                                Detached = rfShared.wheel[0].detached
                            },
                            new AMTireData
                            {
                                BrakeTemperature = rfShared.wheel[3].brakeTemp,
                                Pressure = rfShared.wheel[3].pressure,
                                Wear = 1.0f - rfShared.wheel[3].wear,
                                Temperature = new double []
                                {
                                    rfShared.wheel[3].temperature[0],
                                    rfShared.wheel[3].temperature[1],
                                    rfShared.wheel[3].temperature[2],
                                },
                                Detached = rfShared.wheel[3].detached
                            },
                        };

                        sessionInfo.RemainingTime = rfShared.endET > rfShared.currentET ? 0 :
                                (int)((rfShared.endET - rfShared.currentET) * 1000f);

                        sessionInfo.CurrentLapTime = (int)((rfShared.currentET - rfShared.lapStartET) * 1000f);
                        sessionInfo.CurrentLapNumber = rfShared.lapNumber;
                        sessionInfo.TrackName = rfShared.trackName;
                        sessionInfo.RemainingLaps = rfShared.maxLaps > 0 ? rfShared.maxLaps - rfShared.lapNumber : -1;
                        weatherData.TrackTemp = rfShared.trackTemp;
                        weatherData.AmbientTemp = rfShared.ambientTemp;

                        if (dataSet.GamePlugin is RF1Game game)
                            game.NotifyTelemetry(dataSet);
                    }

                    Thread.Sleep(5);
                }
            }
            catch { }
        }
    }

    public class RF1Game : IGamePlugin
    {
        public string Name => "rFactor";

        public uint SteamGameID => 339790;

        public string[] ExecutableProcessName => new string[] { "rFactor" };

        internal string sIconPath = "";
        internal string sExecutablePpath = "";

        public string UserIconPath { get => sIconPath; set => sIconPath = value; }
        public string UserExecutablePath { get => sExecutablePpath; set => sExecutablePpath = value; }

        internal bool bRunning = false;
        public bool IsRunning => bRunning;

        public event EventHandler<TelemetryUpdatedEventArgs> OnTelemetry;
        public event EventHandler OnGameStateChanged;
        public event EventHandler OnGameIconChanged;

        readonly ProcessMonitor processMonitor = new ProcessMonitor(new string[] { "rFactor" });
        private readonly RF1Listener rF1Listener;

        public RF1Game()
        {
            rF1Listener = new RF1Listener(this);

            rF1Listener.OnThreadError += delegate (object sender, EventArgs e)
            {
                rF1Listener.StopTrhead();
                rF1Listener.StartThread();
            };

            processMonitor.OnProcessRunningStateChanged += delegate (object sender, bool running) 
            {
                bRunning = running;
                OnGameStateChanged?.Invoke(this, new EventArgs());
                if (running)
                    rF1Listener.StartThread();
                else
                    rF1Listener.StopTrhead();
            };

            processMonitor.Start();
        }

        public void Quit()
        {
            rF1Listener.StopTrhead();
            processMonitor.Stop();
        }

        internal void NotifyTelemetry(TelemetryDataSet tds)
        {
            OnTelemetry?.Invoke(this, new TelemetryUpdatedEventArgs(tds));
        }

        public Icon GetIcon()
        {
            // TODO:
            return Properties.Resources.rFactor1;
        }

        public void ShowSettings(IVAzhureRacingApp app)
        {
            // TODO:
        }

        public void Start(IVAzhureRacingApp app)
        {
            if (!Utils.RunSteamGame(SteamGameID))
            {
                app.SetStatusText($"Ошибка запуска игры {Name}!");
            }
        }
    }
}
