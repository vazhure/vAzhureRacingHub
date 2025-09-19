using System;
using System.Configuration.Assemblies;
using System.Drawing;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using vAzhureRacingAPI;

namespace DCS
{
    public class XPlaneGame : IGamePlugin
    {
        public string Name => "X-Plane 11";

        public uint SteamGameID => 269950U;

        public string[] ExecutableProcessName => new[] { "X-Plane" }; // TODO:

        private string strUserIcon = "";

        public string UserIconPath { get => strUserIcon; set => strUserIcon = value; }
        public string UserExecutablePath { get => settings.ExecutablePath; set => settings.ExecutablePath = value; }

        public bool IsRunning => Utils.IsProcessRunning(ExecutableProcessName);

        public event EventHandler<TelemetryUpdatedEventArgs> OnTelemetry;
        public event EventHandler OnGameStateChanged;
        public event EventHandler OnGameIconChanged;

        private GameSettings settings = new GameSettings();

        private readonly ProcessMonitor monitor;
        private readonly TelemetryDataSet dataSet;

        CustomSharedMemClient customSharedMemClient;

        public static readonly string sXPlaneMMFName = @"Local\XPlaneMotionData";

        public XPlaneGame()
        {
            dataSet = new TelemetryDataSet(this);

            monitor = new ProcessMonitor(ExecutableProcessName);
            monitor.OnProcessRunningStateChanged += (process, bRunning) =>
            {
                if (bRunning)
                {
                    customSharedMemClient = new CustomSharedMemClient();
                    customSharedMemClient.StartThread();
                    customSharedMemClient.OnUserFunc += delegate(object sender, EventArgs ea)
                    {
                        ProcessSharedMemory();
                    };
                }
                else
                {
                    customSharedMemClient?.StopTrhead();
                    dataSet.LoadDefaults();
                    OnTelemetry?.Invoke(this, new TelemetryUpdatedEventArgs(dataSet));
                }
                OnGameStateChanged?.Invoke(this, EventArgs.Empty);
            };
            monitor.Start();
        }

        private readonly byte[] dataStatic = new byte[XPlaneTelemetrySize]; 

        private void ProcessSharedMemory()
        {
            try
            {
                using (var mmf = MemoryMappedFile.OpenExisting(sXPlaneMMFName, MemoryMappedFileRights.ReadWrite))
                using (var mmfView = mmf.CreateViewStream(0L, XPlaneTelemetrySize, MemoryMappedFileAccess.ReadWrite))
                {
                    mmfView.ReadAsync(dataStatic, 0, XPlaneTelemetrySize).Wait();

                    XPlaneTelemetry xPlaneTelemetry = Marshalizable<XPlaneTelemetry>.FromBytes(dataStatic);

                    AMMotionData data = new AMMotionData()
                    {
                        Pitch = xPlaneTelemetry.Pitch * 0.0055555555555556f,
                        Roll = xPlaneTelemetry.Roll * 0.0055555555555556f,
                        Yaw = xPlaneTelemetry.Yaw * 0.0055555555555556f,
                        Heave = xPlaneTelemetry.Axil / 10.0f,
                    };

                    dataSet.CarData.MotionData = data;

                    OnTelemetry?.Invoke(this, new TelemetryUpdatedEventArgs(dataSet));
                }
            }
            catch 
            {
                AMMotionData data = new AMMotionData()
                {
                    Pitch = 0,
                    Roll = 0,
                    Yaw = 0,
                };

                dataSet.CarData.MotionData = data;
                OnTelemetry?.Invoke(this, new TelemetryUpdatedEventArgs(dataSet));
            }
        }

        public Icon GetIcon()
        {
            return Properties.Resources.X_Plane;
        }

        readonly string assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public void ShowSettings(IVAzhureRacingApp app)
        {
            string args = string.Format($"/root, \"{Path.Combine(assemblyPath, "X-Plane")}\"");
            System.Diagnostics.Process.Start("explorer.exe", args);
        }

        public void Start(IVAzhureRacingApp app)
        {
            if (settings.ExecutablePath != string.Empty)
            {
                if (Utils.ExecuteCmd(settings.ExecutablePath))
                    return;
            }

            if (!Utils.RunSteamGame(SteamGameID))
            {
                app.SetStatusText($"Steam Service is not running. Run {Name} manually!");
            }
        }

        internal void Stop()
        {
            customSharedMemClient?.StopTrhead();
        }

        public class GameSettings
        {
            public string ExecutablePath = "";
        }

        public static readonly int XPlaneTelemetrySize = Marshal.SizeOf(typeof(XPlaneTelemetry));
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct XPlaneTelemetry
    {
        public float Pitch;
        public float Yaw;
        public float Roll;
        public float Side;
        public float Normal;
        public float Axil;
    };

    public class CustomSharedMemClient : VAzhureSharedMemoryClient
    {
        public event EventHandler OnUserFunc;
        public override void UserFunc()
        {
            OnUserFunc?.Invoke(this, EventArgs.Empty);
            Thread.Sleep(10);
        }
    }
}