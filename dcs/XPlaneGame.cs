using System;
using System.Collections.Generic;
using System.Configuration.Assemblies;
using System.Drawing;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using vAzhureRacingAPI;

namespace DCS
{
    public class XPlaneGame : IGamePlugin
    {
        private int Version { get; set; } = 11; 
        public string Name => $"X-Plane {Version}";

        public uint SteamGameID => Version == 11 ? 269950U : 2014780;

        public string[] ExecutableProcessName => new[] { "X-Plane" };

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

        public XPlaneGame(int version = 11)
        {
            Version = version;
            dataSet = new TelemetryDataSet(this);

            monitor = new ProcessMonitor(ExecutableProcessName);
            monitor.OnProcessRunningStateChanged += (process, bRunning) =>
            {
                if (bRunning)
                {
                    if (Version == 11)
                    {
                        customSharedMemClient = new CustomSharedMemClient();
                        customSharedMemClient.StartThread();
                        customSharedMemClient.OnUserFunc += delegate (object sender, EventArgs ea)
                        {
                            ProcessSharedMemory();
                        };
                    }
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

#if DEBUG
    private readonly List<XPlaneTelemetry> telemetry = new List<XPlaneTelemetry>(10000000);
#endif

        private void ProcessSharedMemory()
        {
            try
            {
                using (var mmf = MemoryMappedFile.OpenExisting(sXPlaneMMFName, MemoryMappedFileRights.ReadWrite))
                using (var mmfView = mmf.CreateViewStream(0L, XPlaneTelemetrySize, MemoryMappedFileAccess.ReadWrite))
                {
                    mmfView.ReadAsync(dataStatic, 0, XPlaneTelemetrySize).Wait();

                    XPlaneTelemetry xPlaneTelemetry = Marshalizable<XPlaneTelemetry>.FromBytes(dataStatic);

#if DEBUG
                    if (telemetry.Count < telemetry.Capacity)
                        telemetry.Add(xPlaneTelemetry);
#endif
                    if (xPlaneTelemetry.Axil == 0 && xPlaneTelemetry.Normal == 0 && xPlaneTelemetry.Roll == 0 && xPlaneTelemetry.Side == 0 && xPlaneTelemetry.Pitch == 0 && xPlaneTelemetry.Yaw == 0)
                        dataSet.CarData.MotionData = new AMMotionData();
                    else
                        dataSet.CarData.MotionData = new AMMotionData()
                        {
                            Pitch = xPlaneTelemetry.Pitch / 180.0f, // to radians
                            Roll = xPlaneTelemetry.Roll / 180.0f, // to radians
                            Yaw = xPlaneTelemetry.Yaw / 180.0f, // to radians
                            Heave = (xPlaneTelemetry.Normal - 9.81f) / 9.81f,
                            Surge = -xPlaneTelemetry.Axil / 9.81f,
                            Sway = xPlaneTelemetry.Side / 9.81f
                        };

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
            return Version == 11 ? Properties.Resources.X_Plane : Properties.Resources.X_Plane12;
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

#if DEBUG
            if (telemetry.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var tele in telemetry)
                    sb.AppendLine($"{tele.Pitch};{tele.Roll};{tele.Yaw};{tele.Side};{tele.Axil};{tele.Normal};");

                File.WriteAllText(Path.Combine(assemblyPath, "telemetry.csv"), sb.ToString());
            }
#endif
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
        /// <summary>
        /// Pitch, degrees
        /// </summary>
        public float Pitch;
        /// <summary>
        /// Yaw, degrees
        /// </summary>
        public float Yaw;
        /// <summary>
        /// Roll, degrees
        /// </summary>
        public float Roll;
        /// <summary>
        /// Gear/ground forces - sideways - ACF X, newtons
        /// </summary>
        public float Side;
        /// <summary>
        /// Gear/ground forces - upward - ACF Y, newtons 
        /// </summary>
        public float Normal;
        /// <summary>
        /// Gear/ground forces - backward - ACF Z, newtons
        /// </summary>
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