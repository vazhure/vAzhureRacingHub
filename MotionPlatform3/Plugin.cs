﻿// Ignore Spelling: app

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using vAzhureRacingAPI;

namespace MotionPlatform3
{
    public class Plugin : ICustomPlugin, ICustomDevicePlugin
    {
        public string Name => "3DOF Motion Platform";

        public string Description => "3DOF Motion Platform";

        public ulong Version => 1UL;

        public string DeviceName => "3DOF Motion Platform";

        public string DeviceDescription => "3DOF Motion Platform";

        public string DeviceID => "vAZHURE3DOF";

        private DeviceStatus status = DeviceStatus.Unknown;
        public DeviceStatus Status
        {
            get => status;
            set
            {
                status = value;
            }
        }

        public bool DeviceEnabled
        {
            get => settings.Enabled;
            set => settings.Enabled = value;
        }

        public int ComPort
        {
            get => settings.ComPort;
            set
            {
                if (value != settings.ComPort)
                {
                    settings.ComPort = value;
                    ReConnect();
                }
            }
        }

        public Image DevicePictogram => Properties.Resources.ico_3dof.ToBitmap();

        public event EventHandler OnConnected;
        public event EventHandler OnDisconnected;

        internal SerialPort serialPort;
        internal MotionPlatformSettings settings = new MotionPlatformSettings();

        /// <summary>
        /// Полный путь к DLL сборки
        /// </summary>
        public static string AssemblyPath
        {
            get
            {
                return Path.GetDirectoryName(Assembly.GetCallingAssembly().Location);
            }
        }

        public bool CanClose(IVAzhureRacingApp app)
        {
            return true;
        }

        public bool Initialize(IVAzhureRacingApp app)
        {
            settings = MotionPlatformSettings.LoadSettings(Path.Combine(AssemblyPath, "settings.json"));
            //  settings.gamesData.Add(new GameData("Dirt Rally 2.0", new AMMotionData()));
            app.RegisterDevice(this);
            app.OnDeviceArrival += delegate (object sender, DeviceChangeEventsArgs e)
            {
                if ($"COM{settings.ComPort}" == e.Port.ToUpper())
                    ReConnect();
            };

            app.OnDeviceRemoveComplete += delegate (object sender, DeviceChangeEventsArgs e)
            {
                if ($"COM{settings.ComPort}" == e.Port.ToUpper())
                    Disconnect();
            };

            serialPort = new SerialPort
            {
                DataBits = settings.DataBits,
                Parity = settings.Parity,
                StopBits = settings.StopBits,
                Handshake = settings.Handshake,
                RtsEnable = settings.RtsEnable,
                DtrEnable = settings.DtrEnable
            };

            serialPort.DataReceived += SerialPort_DataReceived;
            mainLoop = new Task(MainTread, this, tokenSource.Token);

            mainLoop.Start();

            ReConnect();

            return true;
        }

        internal void Park()
        {
            if (!IsConnected)
                return;
            lock (serialPort)
            {
                try
                {
                    byte[] data = GenerateCommand(COMMAND.CMD_PARK, 1, 1, 1);
                    serialPort.Write(data, 0, PCCMD_SIZE);
                }
                catch { }
            }
        }

        internal void Home()
        {
            if (!IsConnected)
                return;
            lock (serialPort)
            {
                try
                {
                    byte[] data = GenerateCommand(COMMAND.CMD_HOME, 1, 1, 1);
                    serialPort.Write(data, 0, PCCMD_SIZE);
                }
                catch { }
            }
        }

        internal void AlarmReset()
        {
            if (!IsConnected)
                return;
            lock (serialPort)
            {
                try
                {
                    byte[] data = GenerateCommand(COMMAND.CMD_CLEAR_ALARM, 1, 1, 1);
                    serialPort.Write(data, 0, PCCMD_SIZE);
                }
                catch { }
            }
        }

        private void ReConnect()
        {
            Disconnect();
            Connect();
        }

        public readonly int cMIN_DELAY = 27;
        public readonly int cMAX_DELAY = 250;

        internal void SetPulseDelay(int delay)
        {
            if (!IsConnected)
                return;
            lock (serialPort)
            {
                try
                {
                    delay = Math2.Clamp(delay, cMIN_DELAY, cMAX_DELAY);
                    byte[] data = GenerateCommand(COMMAND.CMD_SET_SPEED, delay, delay, delay);
                    serialPort.Write(data, 0, PCCMD_SIZE);
                }
                catch { }
            }
        }

        private readonly CancellationTokenSource tokenSource = new CancellationTokenSource();
        volatile bool bTaskRunning = false;
        private Task mainLoop;

        private void Disconnect()
        {
            try
            {
                if (serialPort.IsOpen)
                {
                    serialPort.Close();
                }
            }
            catch
            {
            }
            FrontAxisState = new AXIS_STATE();
            RearLeftAxisState = new AXIS_STATE();
            RearRightAxisState = new AXIS_STATE();
            status = DeviceStatus.Disconnected;
            OnDisconnected?.Invoke(this, new EventArgs());
        }

        private void Connect()
        {
            if (settings.ComPort == 0)
                return;

            serialPort.PortName = $"COM{settings.ComPort}";
            serialPort.BaudRate = settings.BaudRate;

            lock (serialPort)
            {
                try
                {
                    serialPort.Open();
                    serialPort.DiscardInBuffer();
                    serialPort.DiscardOutBuffer();
                }
                catch
                {
                    Status = DeviceStatus.ConnectionError;
                    OnConnected?.Invoke(this, new EventArgs());
                    return;
                }
            }

            Status = serialPort.IsOpen ? DeviceStatus.Connected : DeviceStatus.ConnectionError;
            OnConnected?.Invoke(this, new EventArgs());

            RequestState();
            SetPulseDelay(settings.SpeedOverride);
        }

        byte[] GenerateCommand(COMMAND cmd, int par1, int par2, int par3, byte reserved = 0)
        {
            byte[] arr = new byte[PCCMD_SIZE];

            GCHandle h = default;

            try
            {
                h = GCHandle.Alloc(arr, GCHandleType.Pinned);
                PCCMD cpmd = new PCCMD() { header = 0, cmd = cmd, len = (byte)PCCMD_SIZE, data = new int[] { par1, par2, par3 }, reserved = reserved };
                Marshal.StructureToPtr<PCCMD>(cpmd, h.AddrOfPinnedObject(), false);
                return arr;
            }
            catch
            {
                return arr;
            }
            finally
            {
                if (h.IsAllocated)
                {
                    h.Free();
                }
            }
        }

        public void RequestState()
        {
            if (!IsConnected)
                return;
            lock (serialPort)
            {
                try
                {
                    byte[] data = GenerateCommand(COMMAND.CMD_GET_STATE, 1, 1, 1);
                    serialPort.Write(data, 0, PCCMD_SIZE);
                }
                catch { }
            }
        }

        private TelemetryDataSet tds = new TelemetryDataSet(null);

        public TelemetryDataSet Telemetry => tds;

        public void OnTelemetry(IVAzhureRacingApp app, TelemetryDataSet data)
        {
            tds = data;

            if (settings.mode == MODE.CollectingGameData)
            {
                if (data.CarData?.MotionData != null)
                {
                    string name = data.GamePlugin.Name;
                    if (settings.gamesData.FirstOrDefault(o => o.GameName == name) is GameData gd)
                    {
                        gd.Update(data.CarData.MotionData);
                    }
                    else
                    {
                        settings.gamesData.Add(new GameData(name));
                    }
                }
            }
            else
            {
                bDataArrived = true;
            }
        }

        public void Quit(IVAzhureRacingApp app)
        {
            // TODO:
            settings?.SaveSettings(Path.Combine(AssemblyPath, "settings.json"));

            bTaskRunning = false;
            mainLoop.Wait();
        }

        public void ShowProperties(IVAzhureRacingApp app)
        {
            using (SettingsForm form = new SettingsForm(this))
            {
                form.ShowDialog(app.MainForm);
            }
        }

        void ICustomDevicePlugin.Initialize(IVAzhureRacingApp app)
        {
        }

        internal volatile bool bDataArrived = true;

        readonly Action<object> MainTread = (object obj) =>
        {
            DateTime _tm = DateTime.Now;

            if (obj is Plugin plugin)
            {
                plugin.bTaskRunning = true;
                while (plugin.bTaskRunning)
                {
                    try
                    {
                        if (plugin.serialPort?.BytesToWrite == 0)
                        {
                            if (plugin.bDataArrived)
                            {
                                plugin.bDataArrived = false;
                                plugin.ProcessTelemetry();
                                _tm = DateTime.Now;
                            }
                            else
                            {
                                TimeSpan ts = DateTime.Now - _tm;
                                if (ts.TotalSeconds > 1)
                                {
                                    plugin.ProcessIdle();
                                }
                            }
                        }
                    }
                    catch
                    {

                    }
                    finally
                    {
                        Thread.Sleep(5);
                    }
                }
            }
        };

        internal void ResetActiveGameData()
        {
            string name = tds?.GamePlugin?.Name;
            if (settings.gamesData.FirstOrDefault(o => o.GameName == name) is GameData gd)
                gd.Reset();
        }

        private readonly Filter FrontFilter = new Filter();
        private readonly Filter RLFilter = new Filter();
        private readonly Filter RRFilter = new Filter();

        int _lastPosFront = 0;
        int _lastPosRL = 0;
        int _lastPosRR = 0;

        private void ProcessIdle()
        {
            if (!settings.Enabled || !IsConnected)
                return;

            int middle = (FrontAxisState.min + FrontAxisState.max) / 2;

            int posFront = (int)FrontFilter.Smooth(middle, 0.5);
            int posRL = (int)RLFilter.Smooth(middle, 0.5);
            int posRR = (int)RRFilter.Smooth(middle, 0.5);

            if (_lastPosFront != posFront || _lastPosRL != posRL || _lastPosRR != posRR)
            {
                byte[] data = GenerateCommand(COMMAND.CMD_MOVE, posFront, posRL, posRR);

                lock (serialPort)
                {
                    try
                    {
                        serialPort.Write(data, 0, PCCMD_SIZE);
                    }
                    catch { }
                }

                _lastPosFront = posFront;
                _lastPosRL = posRL;
                _lastPosRR = posRR;
            }
        }

        private void ProcessTelemetry()
        {
            if (!settings.Enabled || !IsConnected)
                return;
            try
            {
                string name = tds?.GamePlugin?.Name;
                if (settings.gamesData.FirstOrDefault(o => o.GameName == name) is GameData gd)
                {
                    float absPitch = Math.Max(Math.Abs(gd.minPitch), Math.Abs(gd.maxPitch));
                    float absRoll = Math.Max(Math.Abs(gd.minRoll), Math.Abs(gd.maxRoll));
                    float absHeave = Math.Max(Math.Abs(gd.minHeave), Math.Abs(gd.maxHeave));
                    float absSway = Math.Max(Math.Abs(gd.minSway), Math.Abs(gd.maxSway));
                    float absSurge = Math.Max(Math.Abs(gd.minSurge), Math.Abs(gd.maxSurge));

                    float overal = settings.OveralCoefficient / 100.0f;
                    float pitchAmount = settings.PitchCoefficient / 100.0f;
                    float rollAmount = settings.RollCoefficient / 100.0f;
                    float heavAmount = settings.HeaveCoefficient / 100.0f;
                    float swayAmoun = settings.SwayCoefficient / 100.0f;
                    float surgeAmount = settings.SurgeCoefficient / 100.0f;

                    float pitch = overal * (settings.Invert.HasFlag(MotionPlatformSettings.InvertFlags.InvertPitch) ? -1.0f : 1.0f) * pitchAmount * Math2.Mapf(tds.CarData.MotionData.Pitch, -absPitch, absPitch, -1.0f, 1.0f);
                    float roll = overal * (settings.Invert.HasFlag(MotionPlatformSettings.InvertFlags.InvertRoll) ? -1.0f : 1.0f) * rollAmount * Math2.Mapf(tds.CarData.MotionData.Roll, -absRoll, absRoll, -1.0f, 1.0f);
                    float heave = overal * (settings.Invert.HasFlag(MotionPlatformSettings.InvertFlags.InvertHeave) ? -1.0f : 1.0f) * heavAmount * Math2.Mapf(tds.CarData.MotionData.Heave, -absHeave, absHeave, -1.0f, 1.0f);
                    float sway = overal * (settings.Invert.HasFlag(MotionPlatformSettings.InvertFlags.InvertSway) ? -1.0f : 1.0f) * swayAmoun * Math2.Mapf(tds.CarData.MotionData.Sway, -absSway, absSway, -1.0f, 1.0f);
                    float surge = overal * (settings.Invert.HasFlag(MotionPlatformSettings.InvertFlags.InvertSurge) ? -1.0f : 1.0f) * surgeAmount * Math2.Mapf(tds.CarData.MotionData.Surge, -absSurge, absSurge, -1.0f, 1.0f);

                    pitch = (float)((float)Math.Sign(pitch) * Math.Pow(Math2.Clamp(Math.Abs(pitch), 0, 1.0f), settings.Linearity));
                    roll = (float)((float)Math.Sign(roll) * Math.Pow(Math2.Clamp(Math.Abs(roll), 0, 1.0f), settings.Linearity));
                    heave = (float)((float)Math.Sign(heave) * Math.Pow(Math2.Clamp(Math.Abs(heave), 0, 1.0f), settings.Linearity));
                    sway = (float)((float)Math.Sign(sway) * Math.Pow(Math2.Clamp(Math.Abs(sway), 0, 1.0f), settings.Linearity));
                    surge = (float)((float)Math.Sign(surge) * Math.Pow(Math2.Clamp(Math.Abs(surge), 0, 1.0f), settings.Linearity));

                    int posFront = (int)Math2.Mapf(-heave + pitch + surge, -1.0f, 1.0f, FrontAxisState.min, FrontAxisState.max);
                    int posRL = (int)Math2.Mapf(-heave - pitch - surge + roll + sway, -1.0f, 1.0f, RearLeftAxisState.min, RearLeftAxisState.max);
                    int posRR = (int)Math2.Mapf(-heave - pitch - surge - roll - sway, -1.0f, 1.0f, RearRightAxisState.min, RearRightAxisState.max);

                    posFront = Math2.Clamp(posFront, FrontAxisState.min, FrontAxisState.max);
                    posRL = Math2.Clamp(posRL, RearLeftAxisState.min, RearLeftAxisState.max);
                    posRR = Math2.Clamp(posRR, RearRightAxisState.min, RearRightAxisState.max);

                    double coeff = settings.SmoothCoefficient / 100.0;
                    posFront = (int)FrontFilter.Smooth(posFront, coeff);
                    posRL = (int)RLFilter.Smooth(posRL, coeff);
                    posRR = (int)RRFilter.Smooth(posRR, coeff);

                    if (_lastPosFront != posFront || _lastPosRL != posRL || _lastPosRR != posRR)
                    {
                        byte[] data = GenerateCommand(COMMAND.CMD_MOVE, posFront, posRL, posRR);

                        lock (serialPort)
                        {
                            try
                            {
                                serialPort.Write(data, 0, PCCMD_SIZE);
                            }
                            catch { }
                        }

                        _lastPosFront = posFront;
                        _lastPosRL = posRL;
                        _lastPosRR = posRR;
                    }
                }
            }
            catch { }
        }

        // ACTUAL STATES
        private readonly AXIS_STATE[] states = new AXIS_STATE[3];

        public AXIS_STATE FrontAxisState
        {
            get => states[0];
            private set => states[0] = value;
        }

        public AXIS_STATE RearLeftAxisState
        {
            get => states[1];
            private set => states[1] = value;
        }

        public AXIS_STATE RearRightAxisState
        {
            get => states[2];
            private set => states[2] = value;
        }

        public class AxisStateChanged : EventArgs
        {
            public AxisStateChanged(int addr, AXIS_STATE state) => (Addr, State) = (addr, state);
            public int Addr { get; private set; }
            public AXIS_STATE State { get; private set; }
        }

        public event EventHandler<AxisStateChanged> OnAxisStateChanged;

        //const int AXIS_STATE_SIZE = 20;
        //const int PCCMD_SIZE = 16;

        private readonly byte[] data = new byte[20];

        const int cFirstAddr = 10;
        const int cLastAddr = 12;

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            while (serialPort.BytesToRead >= AXIS_STATE_SIZE + 2)
            {
                int addr = serialPort.ReadByte();
                if (addr >= cFirstAddr && addr <= cLastAddr)
                {
                    if (serialPort.ReadByte() == AXIS_STATE_SIZE)
                    {
                        if (serialPort.Read(data, 0, AXIS_STATE_SIZE) == AXIS_STATE_SIZE)
                            try
                            {
                                GCHandle h = GCHandle.Alloc(data, GCHandleType.Pinned);
                                AXIS_STATE state = (AXIS_STATE)Marshal.PtrToStructure(h.AddrOfPinnedObject(), typeof(AXIS_STATE));
                                h.Free();

                                states[addr - 10] = state;
                                OnAxisStateChanged?.Invoke(this, new AxisStateChanged(addr, state));
                            }
                            catch { }
                    }
                }
            }
        }

        internal bool IsDeviceReady
        {
            get
            {
                return (states[0].mode == DEVICE_MODE.CONNECTED || states[0].mode == DEVICE_MODE.READY) &&
                    (states[1].mode == DEVICE_MODE.CONNECTED || states[1].mode == DEVICE_MODE.READY) &&
                    (states[2].mode == DEVICE_MODE.CONNECTED || states[2].mode == DEVICE_MODE.READY);
            }
        }

        internal bool IsDeviceHomed
        {
            get
            {
                return states[0].flags.HasFlag(DEVICE_FLAGS.STATE_HOMED) &&
                    states[1].flags.HasFlag(DEVICE_FLAGS.STATE_HOMED) &&
                    states[2].flags.HasFlag(DEVICE_FLAGS.STATE_HOMED);
            }
        }

        internal bool IsConnected
        {
            get
            {
                try
                {
                    return serialPort != null && serialPort.IsOpen;
                }
                catch
                {
                    return false;
                }
            }
        }

        static readonly int AXIS_STATE_SIZE = Marshal.SizeOf(typeof(AXIS_STATE));
        static readonly int PCCMD_SIZE = Marshal.SizeOf(typeof(PCCMD));
    }

    public class GameData : ICloneable
    {
        public string GameName = "";

        public float minABSVibration = float.MaxValue;
        public float minPitch = float.MaxValue;
        public float minRoll = float.MaxValue;
        public float minHeave = float.MaxValue;
        public float minSurge = float.MaxValue;
        public float minSway = float.MaxValue;
        public float minYaw = float.MaxValue;

        public float maxABSVibration = float.MinValue;
        public float maxPitch = float.MinValue;
        public float maxRoll = float.MinValue;
        public float maxHeave = float.MinValue;
        public float maxSurge = float.MinValue;
        public float maxSway = float.MinValue;
        public float maxYaw = float.MinValue;

        public GameData()
        {
        }

        public GameData(string gameName)
        {
            GameName = gameName;
            Reset();
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public void Reset()
        {
            maxABSVibration = float.MinValue;
            maxPitch = float.MinValue;
            maxRoll = float.MinValue;
            maxHeave = float.MinValue;
            maxSurge = float.MinValue;
            maxSway = float.MinValue;
            maxYaw = float.MinValue;

            minABSVibration = float.MaxValue;
            minPitch = float.MaxValue;
            minRoll = float.MaxValue;
            minHeave = float.MaxValue;
            minSurge = float.MaxValue;
            minSway = float.MaxValue;
            minYaw = float.MaxValue;
        }

        public void Update(AMMotionData data)
        {
            minABSVibration = Math.Min(minABSVibration, data.ABSVibration);
            maxABSVibration = Math.Max(maxABSVibration, data.ABSVibration);

            // Acceleration
            minHeave = Math.Min(minHeave, data.Heave);
            maxHeave = Math.Max(maxHeave, data.Heave);
            minSurge = Math.Min(minSurge, data.Surge);
            maxSurge = Math.Max(maxSurge, data.Surge);
            minSway = Math.Min(minSway, data.Sway);
            maxSway = Math.Max(maxSway, data.Sway);

            // Skew
            minPitch = Math.Min(minPitch, data.Pitch);
            maxPitch = Math.Max(maxPitch, data.Pitch);
            minRoll = Math.Min(minRoll, data.Roll);
            maxRoll = Math.Max(maxRoll, data.Roll);
            minYaw = Math.Min(minYaw, data.Yaw);
            maxYaw = Math.Max(maxYaw, data.Yaw);
        }

        public static bool IsEqual(AMMotionData a, AMMotionData b)
        {
            return a.Heave == b.Heave && a.Surge == b.Surge && a.Sway == b.Sway && a.Roll == b.Roll && a.Pitch == b.Pitch;
        }
    }

    public enum MODE { Run, CollectingGameData };

    public class MotionPlatformSettings : ICloneable
    {
        /// <summary>
        /// Номер ком-порта
        /// </summary>
        public int ComPort { get; set; } = 0;

        public MODE mode = MODE.Run;
        /// <summary>
        /// Скорость соединения
        /// </summary>
        public int BaudRate { get; set; } = 115200;
        public Parity Parity { get; set; } = Parity.None;
        public int DataBits { get; internal set; } = 8;
        public StopBits StopBits { get; internal set; } = StopBits.One;
        public Handshake Handshake { get; internal set; } = Handshake.None;
        public bool RtsEnable { get; internal set; } = true;
        public bool DtrEnable { get; internal set; } = true;

        /// <summary>
        /// Статус разрешения
        /// </summary>
        public bool Enabled { get; set; } = true;
        public int SpeedOverride { get; set; } = 100;
        public int PitchCoefficient { get; set; } = 100;
        public int RollCoefficient { get; set; } = 100;
        public int HeaveCoefficient { get; set; } = 100;
        public int SurgeCoefficient { get; set; } = 100;
        public int SwayCoefficient { get; set; } = 100;
        public int OveralCoefficient { get; set; } = 100;
        public float Linearity { get; set; } = 1.0f;

        public int SmoothCoefficient { get; set; } = 50;
        /// <summary>
        /// Inversion flags
        /// </summary>
        [Flags]
        public enum InvertFlags { None = 0, InvertPitch = 1, InvertRoll = 1 << 1, InvertHeave = 1 << 2, InvertSurge = 1 << 3, InvertSway = 1 << 4 };

        public InvertFlags Invert { get; set; } = InvertFlags.None;

        public List<GameData> gamesData = new List<GameData>();

        public object Clone()
        {
            return MemberwiseClone();
        }

        public bool SaveSettings(string path)
        {
            string json = "";
            try
            {
                json = File.ReadAllText(path);
            }
            catch { }

            string jsonNew = ObjectSerializeHelper.GetJson(this);

            if (json != jsonNew)
            {
                try
                {
                    File.WriteAllText(path, jsonNew);
                }
                catch { return false; }
            }

            return true;
        }

        public static MotionPlatformSettings LoadSettings(string path)
        {
            try
            {
                string json = File.ReadAllText(path);

                return ObjectSerializeHelper.DeserializeJson<MotionPlatformSettings>(json);
            }
            catch { return new MotionPlatformSettings(); }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct AXIS_STATE
    {
        public DEVICE_MODE mode; // MODE
        public DEVICE_FLAGS flags; // FLAGS
        public byte pulseDelay;
        public int currentpos;
        public int targetpos;
        public int min;
        public int max;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PCCMD
    {
        public byte header;
        public byte len;
        public COMMAND cmd;
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.I4, SizeConst = 3)]
        public int[] data;
        public byte reserved;
    }

    public enum COMMAND : byte
    {
        CMD_HOME,
        CMD_MOVE,
        CMD_SET_SPEED,
        CMD_DISABLE,
        CMD_ENABLE,
        CMD_GET_STATE,
        CMD_CLEAR_ALARM,
        CMD_PARK,
    };

    public enum DEVICE_MODE : byte
    {
        UNKNOWN,
        CONNECTED,
        DISABLED,
        HOMEING,
        PARKING,
        READY,
        ALARM
    };

    [Flags]
    public enum DEVICE_FLAGS : byte
    {
        NONE = 0,
        STATE_ON_LIMIT_SWITCH = 1,
        STATE_HOMED = 1 << 1,
    };

    public class Filter
    {
        private bool _isInitialized;
        private double _previousAverage;

        public double Average { get; private set; }

        public void Reset() { _isInitialized = false; }

        public Filter()
        {
            Reset();
        }

        public double Smooth(double val, double coeff)
        {
            double mul = Math2.Clamp(1.0 - coeff, 0.0, 1.0);

            if (!_isInitialized)
            {
                Average = val;
                _previousAverage = Average;
                _isInitialized = true;
                return _previousAverage;
            }

            Average = ((val - _previousAverage) * mul) + _previousAverage;

            return _previousAverage = Average;
        }
    }
}