using System;

using FlatBuffers;
using vAzhureRacingAPI;

namespace KartKraft
{
    public class KKPlugin : ICustomPlugin
    {
        public string Name => "KartKraft";

        public string Description => "KartKraft Plugin";

        public ulong Version => 1UL;

        private readonly KKGame game = new KKGame();

        public bool CanClose(IVAzhureRacingApp app)
        {
            // TODO:
            return true;
        }

        public bool Initialize(IVAzhureRacingApp app)
        {
            // TODO:
            app.RegisterGame(game);

            return true;
        }

        public void Quit(IVAzhureRacingApp app)
        {
            game.OnQuit();
        }
    }

    public class KKGame : VAzhureUDPClient, IGamePlugin
    {
        public string Name => "KartKraft";

        public uint SteamGameID => 406350U;

        public int UdpPort = 5000;

        public string[] ExecutableProcessName => new string[] { "project_k", "project_k-Win64-Shipping" };

        string sUserIconPath = "";
        string sUserExecutablePath = "";
        private bool bIsRunning = false;
        private readonly ProcessMonitor monitor;

        public string UserIconPath { get => sUserIconPath; set => sUserIconPath = value; }
        public string UserExecutablePath { get => sUserExecutablePath; set => sUserExecutablePath = value; }

        
        public KKGame()
        {
            mCarData = new TelemetryDataSet(this);

            monitor = new ProcessMonitor(ExecutableProcessName, 1000);
            monitor.OnProcessRunningStateChanged += delegate (object o, bool bRunning)
            {
                bIsRunning = bRunning;
                if (bRunning)
                    Run(UdpPort);
                else
                    Stop();
                OnGameStateChanged?.Invoke(this, new EventArgs());
            };
            monitor.Start();
        }

        public bool IsRunning
        {
            get => bIsRunning;
        }

        public event EventHandler<TelemetryUpdatedEventArgs> OnTelemetry;
        public event EventHandler OnGameStateChanged;
        public event EventHandler OnGameIconChanged;

        public System.Drawing.Icon GetIcon()
        {
            return Properties.Resources.kartkraft;
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

        TelemetryDataSet mCarData = null;

        public override void OnDataReceived(ref byte[] bytes)
        {
            bool bHasData = false;

            mCarData = mCarData ?? new TelemetryDataSet(this);

            if (bytes != null)
            {
                ByteBuffer b = new ByteBuffer(bytes);

                if (Frame.FrameBufferHasIdentifier(b))
                {
                    Frame frame = Frame.GetRootAsFrame(b);

                    if (frame.Dash.HasValue)
                    {
                        mCarData.CarData.Flags = TelemetryFlags.FlagGreen;
                        mCarData.SessionInfo.Flag = "Green";
                        mCarData.SessionInfo.CurrentLapTime = (int)(frame.Dash.Value.CurrentLap * 1000.0f);
                        mCarData.SessionInfo.BestLapTime = (int)(frame.Dash.Value.BestLap * 1000.0f);
                        mCarData.SessionInfo.LastLapTime = (int)(frame.Dash.Value.LastLap * 1000.0f);
                        mCarData.SessionInfo.Sector = frame.Dash.Value.SectorCount;
                        mCarData.CarData.Speed = frame.Dash.Value.Speed * 3.6f;
                        mCarData.CarData.Position = frame.Dash.Value.Pos;
                        mCarData.CarData.RPM = frame.Dash.Value.Rpm;
                        mCarData.CarData.Lap = frame.Dash.Value.LapCount;
                        mCarData.CarData.Gear = (short)(frame.Dash.Value.Gear + 1);
                        mCarData.CarData.Steering = (frame.Dash.Value.Steer * Math.PI) / 180.0;
                        mCarData.CarData.Brake = Math2.Clamp(frame.Dash.Value.Brake / 0.999f, 0, 1);
                        mCarData.CarData.Throttle = Math2.Clamp(frame.Dash.Value.Throttle / 0.999f, 0, 1);
                        mCarData.CarData.DriverName = "Driver";
                        mCarData.CarData.CarClass = "Kart";
                        mCarData.CarData.FuelLevel = 1;
                        mCarData.CarData.FuelCapacity = 2;
                        mCarData.SessionInfo.SessionState = "Race";
                        bHasData = true;
                    }

                    if (frame.VehicleConfig.HasValue)
                    {
                        mCarData.CarData.MaxRPM = frame.VehicleConfig.Value.RpmMax;
                    }

                    if (frame.Session.HasValue)
                    {
                        mCarData.SessionInfo.DriversCount = frame.Session.Value.VehiclesLength;

                        if (frame.Session.Value.VehiclesLength > 0)
                        {
                            if (frame.Session.Value.Vehicles(0).Value.State == VehicleState.StartGrid)
                            {
                                mCarData = new TelemetryDataSet(this);
                            }
                            else
                            {
                                mCarData.SessionInfo.SessionState = frame.Session.Value.Vehicles(0).Value.State == VehicleState.Finished ? "Finished" : "Race";
                                mCarData.CarData.Flags = frame.Session.Value.Vehicles(0).Value.State == VehicleState.Finished ? TelemetryFlags.FlagChequered : TelemetryFlags.FlagGreen;
                            }
                        }

                        bHasData = true;
                    }

                    if (frame.SessionConfig.HasValue)
                    {
                        mCarData.SessionInfo.TotalLapsCount = (int)frame.SessionConfig.Value.LapLimit;
                        mCarData.SessionInfo.RemainingTime = (int)frame.SessionConfig.Value.TimeLimit * 1000;
                    }

                    if (frame.TrackConfig.HasValue)
                    {
                        mCarData.SessionInfo.TrackName = frame.TrackConfig.Value.Name;
                        mCarData.SessionInfo.TrackConfig = "";
                    }

                    if (frame.Motion.HasValue)
                    {
                        mCarData.CarData.MotionData.Pitch = frame.Motion.Value.Pitch / (10.0f * (float)Math.PI);
                        mCarData.CarData.MotionData.Roll = frame.Motion.Value.Roll / (10.0f * (float)Math.PI);
                        mCarData.CarData.MotionData.Yaw = frame.Motion.Value.Yaw / 180.0f;

                        mCarData.CarData.MotionData.Surge = frame.Motion.Value.AccelerationX / (9.81f * (float)Math.PI);
                        mCarData.CarData.MotionData.Sway = -frame.Motion.Value.AccelerationY / (9.81f * (float)Math.PI);
                        mCarData.CarData.MotionData.Heave = frame.Motion.Value.AccelerationZ / (9.81f * (float)Math.PI);

                        mCarData.CarData.MotionData.Position = new double[] { frame.Motion.Value.WorldPositionX, frame.Motion.Value.WorldPositionY, frame.Motion.Value.WorldPositionZ };
                        mCarData.CarData.MotionData.LocalVelocity = new float[] { frame.Motion.Value.VelocityX, frame.Motion.Value.VelocityY, frame.Motion.Value.VelocityZ };
                        bHasData = true;
                    }
                }

                if (bHasData)
                    OnTelemetry.Invoke(this, new TelemetryUpdatedEventArgs(mCarData));
            }
        }

        internal void OnQuit()
        {
            try
            {
                Stop();
                monitor.Stop();
            }
            catch { };
        }
    }
}
