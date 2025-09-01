using Sojaner.MemoryScanner;
using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Threading;
using vAzhureRacingAPI;

namespace WreckfestPlugin
{
    public class Plugin : ICustomPlugin
    {
        WreckFestGame wreckFestGame = new WreckFestGame();

        public string Name => "Wreckfest Plugin";

        public string Description => "Author: PEZZALUCIFER";

        public ulong Version => 1L;

        public bool CanClose(IVAzhureRacingApp app)
        {
            return true;
        }

        public bool Initialize(IVAzhureRacingApp app)
        {
            app.RegisterGame(wreckFestGame);

            return true;
        }

        public void Quit(IVAzhureRacingApp app)
        {
            wreckFestGame.Finalize();
        }
    }

    public class WreckFestGame : IGamePlugin
    {
        public string Name => "Wreckfest";

        public uint SteamGameID => 228380U;

        public string[] ExecutableProcessName => new string [] {"Wreckfest", "Wreckfest_x64" };

        string iconPath = "";
        string exePath = "";

        public string UserIconPath { get => iconPath; set => iconPath = value; }
        public string UserExecutablePath { get => exePath; set => exePath = value; }

        bool bRunning = false;
        public bool IsRunning => bRunning;

        public event EventHandler<TelemetryUpdatedEventArgs> OnTelemetry;
        public event EventHandler OnGameStateChanged;
        public event EventHandler OnGameIconChanged;

        private Thread scanThread = null;

        public System.Drawing.Icon GetIcon()
        {
            return Properties.Resources.Wreckfest;
        }

        readonly TelemetryDataSet dataset;

        public WreckFestGame()
        {
            dataset = new TelemetryDataSet(this);

            monitor = new ProcessMonitor(ExecutableProcessName, 500);
            monitor.OnProcessRunningStateChanged += delegate (object o, bool b)
            {
                bRunning = b;
                if (b == false)
                {
                    dataset?.LoadDefaults();
                    OnTelemetry?.Invoke(this, new TelemetryUpdatedEventArgs(dataset));
                }
                OnGameStateChanged?.Invoke(this, new EventArgs());
            };

            monitor.Start();
        }

        public void Finalize()
        {
            monitor.Stop();
            bScanning = false;
            scanThread?.Join();
            scanThread = null;
        }

        RegularMemoryScan scan = null;
        private IVAzhureRacingApp _app = null;

        public void ShowSettings(IVAzhureRacingApp app)
        {
            _app = app;

            using (SettingsForm form = new SettingsForm())
            {
                if (form.ShowDialog(app.MainForm) == System.Windows.Forms.DialogResult.OK)
                {
                    if (scanThread != null)
                    {
                        bScanning = false;
                        scanThread.Join();
                        scanThread = null;
                    }

                    Process process = Process.GetProcesses().Where(p => p.ProcessName.StartsWith("Wreckfest")).FirstOrDefault();
                    wfstProcess = process ?? null;

                    if (wfstProcess != null && !process.ProcessName.Contains("64"))
                    {
                        scan = new RegularMemoryScan(wfstProcess, 0, 2147483647);
                        scan.ScanProgressChanged += new RegularMemoryScan.ScanProgressedEventHandler(Scan_ScanProgressChanged);
                        scan.ScanCompleted += new RegularMemoryScan.ScanCompletedEventHandler(Scan_ScanCompleted);
                        scan.ScanCanceled += new RegularMemoryScan.ScanCanceledEventHandler(Scan_ScanCanceled);
                        scan.StartScanForString("carRootNode" + form.PlayerIdx);
                    }
                    else
                    {
                        app.SetStatusText($"32 bit Wreckfest not running!");
                    }
                }
            }
        }

        public void Start(IVAzhureRacingApp app)
        {
            if (!Utils.RunSteamGame(SteamGameID))
            {
                app.SetStatusText($"Steam Service is not running. Run {Name} manually!");
            }
        }

        void Scan_ScanCanceled(object sender, ScanCanceledEventArgs e)
        {
            bScanning = false;
            _app?.SetStatusText("Wreckfest scan canceled...");
        }

        void Scan_ScanCompleted(object sender, ScanCompletedEventArgs e)
        {
            try
            {
                if (e.MemoryAddresses == null || e.MemoryAddresses.Length == 0)
                {
                    _app?.SetStatusText("Wreckfest scan failed...");
                    return;
                }

                memoryAddress = e.MemoryAddresses[0] - ((4 * 4 * 4) + 4); //offset backwards from found address to start of matrix
                scanThread = new Thread(Run);
                scanThread.Start();
                _app?.SetStatusText("Wreckfest scan completed...");
            }
            catch
            {
                _app?.SetStatusText("Wreckfest scan error...");
            }
        }

        delegate void Progress(object sender, ScanProgressChangedEventArgs e);
        void Scan_ScanProgressChanged(object sender, ScanProgressChangedEventArgs e)
        {
            _app?.SetStatusText($"Wreckfest scan progress {e.Progress}%");
        }

        private readonly ProcessMonitor monitor;
        Process wfstProcess = null;
        int memoryAddress = 0;
        volatile bool bScanning = false;

        private void Run()
        {
            Matrix4x4 lastTransform = Matrix4x4.Identity;
            bool lastFrameValid = false;
            Vector3 lastVelocity = Vector3.Zero;
            float lastYaw = 0.0f;

            Stopwatch sw = new Stopwatch();
            sw.Start();

            ProcessMemoryReader reader = new ProcessMemoryReader
            {
                ReadProcess = wfstProcess
            };

            uint readSize = 4 * 4 * 4;
            byte[] readBuffer = new byte[readSize];
            reader.OpenProcess();

            bScanning = true;

            while (bScanning) // while process is running
            {
                try
                {
                    float dt = (float)sw.ElapsedMilliseconds / 1000.0f;

                    reader.ReadProcessMemory((IntPtr)memoryAddress, readSize, out int byteReadSize, readBuffer);

                    if (byteReadSize == 0)
                    {
                        continue;
                    }

                    float[] floats = new float[4 * 4];

                    Buffer.BlockCopy(readBuffer, 0, floats, 0, readBuffer.Length);

                    Matrix4x4 transform = new Matrix4x4(floats[0], floats[1], floats[2], floats[3]
                                                        , floats[4], floats[5], floats[6], floats[7]
                                                        , floats[8], floats[9], floats[10], floats[11]
                                                        , floats[12], floats[13], floats[14], floats[15]);

                    Vector3 rht = new Vector3(transform.M11, transform.M12, transform.M13);
                    Vector3 up = new Vector3(transform.M21, transform.M22, transform.M23);
                    Vector3 fwd = new Vector3(transform.M31, transform.M32, transform.M33);

                    float rhtMag = rht.Length();
                    float upMag = up.Length();
                    float fwdMag = fwd.Length();

                    //reading garbage
                    if (rhtMag < 0.9f || upMag < 0.9f || fwdMag < 0.9f)
                    {
                        bScanning = false;
                        break;
                    }

                    if (!lastFrameValid)
                    {
                        lastTransform = transform;
                        lastFrameValid = true;
                        lastVelocity = Vector3.Zero;
                        lastYaw = 0.0f;
                        continue;
                    }

                    if (dt <= 0)
                        dt = 1.0f;

                    Vector3 worldVelocity = (transform.Translation - lastTransform.Translation) / dt;
                    lastTransform = transform;

                    Matrix4x4 rotation = new Matrix4x4();
                    rotation = transform;
                    rotation.M41 = 0.0f;
                    rotation.M42 = 0.0f;
                    rotation.M43 = 0.0f;

                    Matrix4x4 rotInv = new Matrix4x4();
                    Matrix4x4.Invert(rotation, out rotInv);

                    Vector3 localVelocity = Vector3.Transform(worldVelocity, rotInv);

                    Vector3 localAcceleration = (localVelocity - lastVelocity);
                    lastVelocity = localVelocity;

                    dataset.CarData.MotionData.LocalVelocity = new[] { localVelocity.X, localVelocity.Z, localVelocity.X }; 
                    dataset.CarData.MotionData.LocalAcceleration = new[] { localAcceleration.X / 9.81f, localAcceleration.Z / 98.1f, localAcceleration.Y / 9.81f };

                    float pitch = (float)Math.Asin(-fwd.Y);
                    float yaw = (float)Math.Atan2(fwd.X, fwd.Z);

                    float roll = 0.0f;
                    Vector3 rhtPlane = rht;
                    rhtPlane.Y = 0;
                    rhtPlane = Vector3.Normalize(rhtPlane);

                    if (rhtPlane.Length() <= float.Epsilon)
                    {
                        roll = -(float)(Math.Sign(rht.Y) * Math.PI * 0.5f);
                    }
                    else
                    {
                        roll = -(float)Math.Asin(Vector3.Dot(up, rhtPlane));
                    }

                    dataset.CarData.MotionData.Pitch = -0.5f * pitch / (float)Math.PI;
                    dataset.CarData.MotionData.Yaw = yaw / (float)Math.PI;
                    dataset.CarData.MotionData.Roll = -0.5f * roll / (float)Math.PI;

                    lastYaw = yaw;

                    if (IsRunning)
                    {
                        sw.Restart();
                        OnTelemetry?.Invoke(this, new TelemetryUpdatedEventArgs(dataset));
                    }

                    if (sw.ElapsedMilliseconds > 500)
                    {
                        bScanning = false;
                    }
                    
                    Thread.Sleep(10);
                }
                catch 
                {
                    bScanning = false;
                    Thread.Sleep(1000);
                }
            }

            bScanning = false;

            reader.CloseHandle();
        }
    }
}