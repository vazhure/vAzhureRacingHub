using System;
using System.Drawing;
using System.Runtime.InteropServices;
using vAzhureRacingAPI;

/// MotionSim UDP protocol
/// Activation: Can be enabled and configured in Options > Other > MotionSim enabled.

namespace BeamNG
{
    public class Plugin : ICustomPlugin
    {
        public string Name => "BeamNG.Drive Plugin";

        public string Description => "BeamNG.Drive Plugin";

        public ulong Version => 1UL;

        public bool CanClose(IVAzhureRacingApp app)
        {
            return true;
        }

        readonly BeamNGGame game = new BeamNGGame();

        public bool Initialize(IVAzhureRacingApp app)
        {
            try
            {
                app.RegisterGame(game);
            }
            catch { }
            return true;
        }

        public void Quit(IVAzhureRacingApp app)
        {
            try
            {
                game.Finish();
            }
            catch { }
        }
    }

    public class BeamNGGame : VAzhureUDPClient, IGamePlugin
    {
        public string Name => "BeamNG.Drive";

        public uint SteamGameID => 284160U;

        public string[] ExecutableProcessName => new string[] { "BeamNG.drive" };

        string iconPath = "";
        string exePath = "";

        public string UserIconPath { get => iconPath; set => iconPath = value; }
        public string UserExecutablePath { get => exePath; set => exePath = value; }

        public bool IsRunning => Utils.IsProcessRunning(ExecutableProcessName);

        public event EventHandler<TelemetryUpdatedEventArgs> OnTelemetry;
        public event EventHandler OnGameStateChanged;
        public event EventHandler OnGameIconChanged;

        readonly ProcessMonitor monitor;
        readonly int UdpPort = 4444;

        TelemetryDataSet dataSet = null;

        public BeamNGGame()
        {
            monitor = new ProcessMonitor(ExecutableProcessName, 500);
            monitor.OnProcessRunningStateChanged += delegate (object o, bool bRunning)
            {
                if (bRunning)
                {
                    dataSet = new TelemetryDataSet(this);
                    Run(UdpPort);
                }
                else
                    Stop();
                OnGameStateChanged?.Invoke(this, new EventArgs());
            };
            monitor.Start();
        }

        public Icon GetIcon()
        {
            return Properties.Resources.BeamNG;
        }

        public void ShowSettings(IVAzhureRacingApp _)
        {
            // TODO
        }

        public void Start(IVAzhureRacingApp _)
        {
            Utils.RunSteamGame(SteamGameID);
        }

        public override void OnDataReceived(ref byte[] bytes)
        {
            if (bytes.Length < Marshal.SizeOf(typeof(StructBeamNG)))
                return;
            try
            {
                if (bytes[0] == 'B' && bytes[1] == 'N' && bytes[2] == 'G' && bytes[3] == '1')
                {
                    StructBeamNG structBeamNG = StructBeamNG.FromBytes(bytes);

                    dataSet.CarData.MotionData.Heave = structBeamNG.Heave / 100f;
                    dataSet.CarData.MotionData.Sway = -structBeamNG.Sway / 100f;
                    dataSet.CarData.MotionData.Surge = structBeamNG.Surge / 100f;
                    dataSet.CarData.MotionData.Pitch = structBeamNG.pitchPos;
                    dataSet.CarData.MotionData.Roll = structBeamNG.rollPos;
                    dataSet.CarData.MotionData.Yaw = structBeamNG.yawPos / (float)Math.PI;
                    dataSet.CarData.MotionData.Position = new double[] { structBeamNG.position[0], structBeamNG.position[1], structBeamNG.position[2] };
                    dataSet.CarData.MotionData.LocalVelocity = structBeamNG.velocity;
                }
            }
            catch { }
            finally
            {
                OnTelemetry?.Invoke(this, new TelemetryUpdatedEventArgs(dataSet));
            }
        }

        internal void Finish()
        {
            try
            {
                Stop();
                monitor.Stop();
            }
            catch { }
        }
    }
}