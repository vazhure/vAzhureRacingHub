using System;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Threading;
using vAzhureRacingAPI;
using KartRacingPro;
using System.Threading.Tasks;

namespace KRP
{
    public class Plugin : ICustomPlugin
    {
        public string Name => "Kart Racing Pro Plugin";

        public string Description => "Kart Racing Pro Plugin";

        public ulong Version => 0UL;

        public bool CanClose(IVAzhureRacingApp app)
        {
            return true;
        }

        readonly KRP game = new KRP();

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
            game.Dispose();
        }
    }

    public class KRP : VAzhureSharedMemoryClient, IGamePlugin, IDisposable
    {
        public string Name => "Kart Racing Pro";

        public uint SteamGameID => 415600;

        public string[] ExecutableProcessName => new string[] { "kart" };

        string sUserIcon = string.Empty;
        string sUserExecutablePath = string.Empty;
        public string UserIconPath { get => sUserIcon; set => sUserIcon = value; }
        public string UserExecutablePath { get => sUserExecutablePath; set => sUserExecutablePath = value; }

        public bool IsRunning => bGameRunning;

        public event EventHandler<TelemetryUpdatedEventArgs> OnTelemetry;
        public event EventHandler OnGameStateChanged;
        public event EventHandler OnGameIconChanged;

        readonly ProcessMonitor monitor;

        public KRP()
        {
            telemetry = new TelemetryDataSet(this);
            monitor = new ProcessMonitor(ExecutableProcessName);
            monitor.Start();
            monitor.OnProcessRunningStateChanged += Monitor_OnProcessRunningStateChanged;
            if (Utils.IsProcessRunning(ExecutableProcessName))
                StartThread();
        }

        bool bGameRunning = false;

        private void Monitor_OnProcessRunningStateChanged(object sender, bool bRunning)
        {
            bGameRunning = bRunning;
            OnGameStateChanged?.Invoke(this, new EventArgs());
            if (bRunning)
                StartThread();
            else
                StopTrhead();
        }

        const string cKartTelemetryInfo = @"Local\KRPSMP_KartTelemetryInfo";
        const string cKartSessionInfo = @"Local\KRPSMP_KartSessionInfo";
        const string cKartEventInfo = @"Local\KRPSMP_KartEventInfo";
        const string cKartLapInfo = @"Local\KRPSMP_KartLapInfo";
        const string cKartSplitInfo = @"Local\KRPSMP_KartSplitInfo";
        const string cPluginInfo = @"Local\KRPSMP_PluginInfo";
        const string cRaceEventInfo = @"Local\KRPSMP_RaceEventInfo";
        const string cRaceSessionInfo = @"Local\KRPSMP_RaceSessionInfo";

        readonly byte[] dataPluginInfo = new byte[Marshal.SizeOf(typeof(PluginInfo))];
        readonly byte[] dataKartTelemetryInfo = new byte[Marshal.SizeOf(typeof(KartTelemetryInfo))];
        readonly byte[] dataKartSessionInfo = new byte[Marshal.SizeOf(typeof(KartSessionInfo))];
        readonly byte[] dataKartLapInfo = new byte[Marshal.SizeOf(typeof(KartLapInfo))];
        readonly byte[] dataKartEventInfo = new byte[Marshal.SizeOf(typeof(KartEventInfo))];
        readonly byte[] dataKartSplitInfo = new byte[Marshal.SizeOf(typeof(KartSplitInfo))];
        readonly byte[] dataRaceInfo = new byte[Marshal.SizeOf(typeof(RaceEventInfo))];
        readonly byte[] dataRaceSessionInfo = new byte[Marshal.SizeOf(typeof(RaceSessionInfo))];

        readonly TelemetryDataSet telemetry;

        public override void UserFunc()
        {
            try
            {
                using (var pluginInfo = MemoryMappedFile.OpenExisting(cPluginInfo, MemoryMappedFileRights.ReadWrite))
                using (var viewPluginInfo = pluginInfo.CreateViewStream(0L, dataPluginInfo.Length, MemoryMappedFileAccess.ReadWrite))
                
                using (var raceEventInfo = MemoryMappedFile.OpenExisting(cRaceEventInfo, MemoryMappedFileRights.ReadWrite))
                using (var viewRaceEventInfo = raceEventInfo.CreateViewStream(0L, dataRaceInfo.Length, MemoryMappedFileAccess.ReadWrite))

                using (var raceSessionEventInfo = MemoryMappedFile.OpenExisting(cRaceSessionInfo, MemoryMappedFileRights.ReadWrite))
                using (var viewRaceSessionEventInfo = raceSessionEventInfo.CreateViewStream(0L, dataRaceSessionInfo.Length, MemoryMappedFileAccess.ReadWrite))

                using (var kartTelemetryInfo = MemoryMappedFile.OpenExisting(cKartTelemetryInfo, MemoryMappedFileRights.ReadWrite))
                using (var viewKartTelemetryInfo = kartTelemetryInfo.CreateViewStream(0L, dataKartTelemetryInfo.Length, MemoryMappedFileAccess.ReadWrite))

                using (var kartSessionInfo = MemoryMappedFile.OpenExisting(cKartSessionInfo, MemoryMappedFileRights.ReadWrite))
                using (var viewKartSessionInfo = kartSessionInfo.CreateViewStream(0L, dataKartSessionInfo.Length, MemoryMappedFileAccess.ReadWrite))

                using (var kartLapInfo = MemoryMappedFile.OpenExisting(cKartLapInfo, MemoryMappedFileRights.ReadWrite))
                using (var viewKartLapInfo = kartLapInfo.CreateViewStream(0L, dataKartLapInfo.Length, MemoryMappedFileAccess.ReadWrite))

                using (var eventLapInfo = MemoryMappedFile.OpenExisting(cKartEventInfo, MemoryMappedFileRights.ReadWrite))
                using (var viewKartEventInfo = eventLapInfo.CreateViewStream(0L, dataKartEventInfo.Length, MemoryMappedFileAccess.ReadWrite))

                using (var kartSplitInfo = MemoryMappedFile.OpenExisting(cKartSplitInfo, MemoryMappedFileRights.ReadWrite))
                using (var viewKartSplitInfo = kartSplitInfo.CreateViewStream(0L, dataKartSplitInfo.Length, MemoryMappedFileAccess.ReadWrite))
                {
                    var CarData = telemetry.CarData;
                    var SessionInfo = telemetry.SessionInfo;
                    var MotionData = telemetry.CarData.MotionData;
                    var WeatherData = telemetry.WeatherData;

                    bool bData = false;

                    //if (viewPluginInfo.SafeMemoryMappedViewHandle != null)
                    //{
                    //    viewPluginInfo.ReadAsync(dataPluginInfo, 0, dataPluginInfo.Length);
                    //}

                    bool bIsReplay = false;

                    RaceType raceType = RaceType.Replay;

                    if (viewRaceEventInfo.SafeMemoryMappedViewHandle != null)
                    {
                        viewRaceEventInfo.ReadAsync(dataRaceInfo, 0, dataRaceInfo.Length).Wait();

                        var data = Marshalizable<RaceEventInfo>.FromBytes(dataRaceInfo);

                        bIsReplay = data.m_RaceEvent.m_iType == RaceType.Replay;
                        raceType = data.m_RaceEvent.m_iType;
                    }

                    if (!bIsReplay && viewKartTelemetryInfo.SafeMemoryMappedViewHandle != null)
                    {
                        viewKartTelemetryInfo.ReadAsync(dataKartTelemetryInfo, 0, dataKartTelemetryInfo.Length).Wait();

                        KartTelemetryInfo data = Marshalizable<KartTelemetryInfo>.FromBytes(dataKartTelemetryInfo);

                        CarData.Gear = (short)(data.m_KartData.m_iGear + 1);
                        CarData.FuelLevel = data.m_KartData.m_fFuel;
                        CarData.Speed = data.m_KartData.m_fSpeedometer * 3.6f;
                        CarData.RPM = data.m_KartData.m_iRPM;
                        CarData.Throttle = data.m_KartData.m_fInputThrottle;
                        CarData.Brake = data.m_KartData.m_fInputBrake;
                        CarData.Clutch = 1.0f - data.m_KartData.m_fInputClutch;
                        CarData.Steering = data.m_KartData.m_fInputSteer / 180.0f;
                        CarData.WaterTemp = data.m_KartData.m_fWaterTemperature;

                        CarData.MotionData.Pitch = data.m_KartData.m_fPitch / 180.0f;
                        CarData.MotionData.Roll = -data.m_KartData.m_fRoll / 180.0f;
                        CarData.MotionData.Yaw = data.m_KartData.m_fYaw / 180.0f;
                        CarData.MotionData.Heave = data.m_KartData.m_fAccelerationY / 9.81f;
                        CarData.MotionData.Sway = -data.m_KartData.m_fAccelerationX / 9.81f;
                        CarData.MotionData.Surge = data.m_KartData.m_fAccelerationZ / 9.81f;

                        bData = true;
                    }

                    if (viewKartEventInfo.SafeMemoryMappedViewHandle != null)
                    {
                        viewKartEventInfo.ReadAsync(dataKartEventInfo, 0, dataKartEventInfo.Length).Wait();

                        KartEventInfo data = Marshalizable<KartEventInfo>.FromBytes(dataKartEventInfo);

                        CarData.FuelCapacity = (short)data.m_KartEvent.m_fMaxFuel;
                        CarData.ShiftUpRPM = data.m_KartEvent.m_iShiftRPM;
                        CarData.MaxRPM = data.m_KartEvent.m_iLimiter;
                        CarData.Gear =(short) (data.m_KartEvent.m_iDriveType == 2 ? CarData.Gear : 2);

                        CarData.DriverName = data.m_KartEvent.m_szDriverName;
                        CarData.CarName = data.m_KartEvent.m_szKartName;
                        
                        SessionInfo.TrackName = data.m_KartEvent.m_szTrackName;
                        SessionInfo.TrackLength = data.m_KartEvent.m_fTrackLength;

                        bData = true;
                    }

                    if (viewKartSessionInfo.SafeMemoryMappedViewHandle != null)
                    {
                        viewKartSessionInfo.ReadAsync(dataKartSessionInfo, 0, dataKartSessionInfo.Length).Wait();

                        KartSessionInfo data = Marshalizable<KartSessionInfo>.FromBytes(dataKartSessionInfo);

                        WeatherData.AmbientTemp = data.m_KartSession.m_fAirTemperature;
                        WeatherData.TrackTemp = data.m_KartSession.m_fTrackTemperature;
                        WeatherData.Raining = data.m_KartSession.m_iConditions == 2 ? 1.0 : 0;

                        if (raceType == RaceType.Race)
                            switch (data.m_KartSession.m_iSession)
                            {
                                case 0:
                                    SessionInfo.SessionState = "Testing"; break;
                                case 1:
                                    SessionInfo.SessionState = "Practice"; break;
                                case 2:
                                    SessionInfo.SessionState = "Qualify"; break;
                                case 3:
                                    SessionInfo.SessionState = "Warmup"; break;
                                case 4:
                                    SessionInfo.SessionState = "Qualify heat"; break;
                                case 5:
                                    SessionInfo.SessionState = "Second chance heat"; break;
                                case 6:
                                    SessionInfo.SessionState = "Prefinal"; break;
                                case 7:
                                    SessionInfo.SessionState = "Final"; break;
                                default:
                                    SessionInfo.SessionState = ""; break;
                            }
                        else
                        if (raceType == RaceType.Challenge)
                            switch (data.m_KartSession.m_iSession)
                            {
                                case 0:
                                    SessionInfo.SessionState = "Waiting"; break;
                                case 1:
                                    SessionInfo.SessionState = "Practice"; break;
                                case 2:
                                    SessionInfo.SessionState = "Race"; break;
                                default:
                                    SessionInfo.SessionState = ""; break;
                            }
                        bData = true;
                    }

                    if (viewRaceSessionEventInfo.SafeMemoryMappedViewHandle != null)
                    {
                        viewRaceSessionEventInfo.ReadAsync(dataRaceSessionInfo, 0, dataRaceSessionInfo.Length).Wait();

                        RaceSessionInfo data = Marshalizable<RaceSessionInfo>.FromBytes(dataRaceSessionInfo);

                        SessionInfo.RemainingTime = data.m_RaceSession.m_iSessionLength == 0 ? -1 : data.m_RaceSession.m_iSessionLength;
                        SessionInfo.TotalLapsCount = data.m_RaceSession.m_iSessionNumLaps;
                        SessionInfo.DriversCount = data.m_RaceSession.m_iNumEntries;

                        bData = true;
                    }

                    if (viewKartLapInfo.SafeMemoryMappedViewHandle != null)
                    {
                        viewKartLapInfo.ReadAsync(dataKartLapInfo, 0, dataKartLapInfo.Length).Wait();

                        KartLapInfo data = Marshalizable<KartLapInfo>.FromBytes(dataKartLapInfo);

                        SessionInfo.CurrentLapNumber = CarData.Lap = data.m_KartLap.m_iLapNum;
                        SessionInfo.CurrentLapTime = data.m_KartLap.m_iLapTime;
                        CarData.Valid = data.m_KartLap.m_iInvalid == 0;
                        CarData.Position = data.m_KartLap.m_iPos;

                        bData = true;
                    }


                    if (viewKartSplitInfo.SafeMemoryMappedViewHandle != null)
                    {
                        viewKartSplitInfo.ReadAsync(dataKartSplitInfo, 0, dataKartSplitInfo.Length).Wait();

                        KartSplitInfo data = Marshalizable<KartSplitInfo>.FromBytes(dataKartSplitInfo);

                        SessionInfo.Sector = data.m_KartSplit.m_iSplit;
                        SessionInfo.CurrentDelta = data.m_KartSplit.m_iBestDiff;

                        bData = true;
                    }

                    if (bData)
                    {
                        OnTelemetry?.Invoke(this, new TelemetryUpdatedEventArgs(telemetry));
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                Thread.Sleep(5);
            }
        }

        public System.Drawing.Icon GetIcon()
        {
            return Properties.Resources.kart;
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

        public void Dispose()
        {
            monitor.Stop();
            StopTrhead();
        }
    }
}