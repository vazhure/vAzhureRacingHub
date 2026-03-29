// Plugin.cs - 2026-03-27 Updated with LowPassFilter integration and Thread Safety fixes

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using vAzhureRacingAPI;

namespace MotionPlatform3
{
    public class Plugin : ICustomPlugin, ICustomDevicePlugin
    {
        public string Name => "3DOF Motion Platform";
        public string Description => "3DOF Motion Platform";
        public ulong Version => 2UL; // Version bump for filter update
        public string DeviceName => "3DOF Motion Platform";
        public string DeviceDescription => "3DOF Motion Platform";
        public string DeviceID => "vAZHURE3DOF";

        private DeviceStatus status = DeviceStatus.Unknown;
        public DeviceStatus Status
        {
            get => status;
            set => status = value;
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
        private IVAzhureRacingApp vAzhureRacingApp;
        public IVAzhureRacingApp App => vAzhureRacingApp;

        public static string AssemblyPath => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public bool CanClose(IVAzhureRacingApp app) => true;

        private MotionRigPose rigPose;

        // =====================================================================
        // FILTERING INTEGRATION
        // =====================================================================
        // Sample rate determined by Thread.Sleep(5) in MainTread -> ~200Hz
        private const double UPDATE_RATE_HZ = 200.0;
        private MotionFilterSet motionFilters;

        // OpenXR Filters (kept separate as per original logic)
        private PosFilter posPitchFilter;
        private PosFilter posRollFilter;
        private PosFilter posHeaveFilter;

        public bool Initialize(IVAzhureRacingApp app)
        {
            vAzhureRacingApp = app;
            rigPose = new MotionRigPose();

            settings = MotionPlatformSettings.LoadSettings(Path.Combine(AssemblyPath, "settings.json"));

            // Initialize new Low-Pass Filters
            motionFilters = new MotionFilterSet(
                sampleRateHz: UPDATE_RATE_HZ,
                pitchRollCutoff: 8.0,   // 8Hz for fast response
                heaveCutoff: 4.0,       // 4Hz for smoothness
                swaySurgeCutoff: 5.0    // 5Hz intermediate
            );

            // OpenXR Filters Initialization
            posPitchFilter = new PosFilter(0, settings.OpenXRMotionCompensationAngularSpeed * Math.PI / 180f);
            posRollFilter = new PosFilter(0, settings.OpenXRMotionCompensationAngularSpeed * Math.PI / 180f);
            posHeaveFilter = new PosFilter(0, settings.OpenXRMotionCompensationLinearSpeed);

            app.RegisterDevice(this);
            app.OnDeviceArrival += (sender, e) => { if ($"COM{settings.ComPort}" == e.Port.ToUpper()) ReConnect(); };
            app.OnDeviceRemoveComplete += (sender, e) => { if ($"COM{settings.ComPort}" == e.Port.ToUpper()) Disconnect(); };

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
            if (!IsConnected) return;
            lock (serialPort)
            {
                try
                {
                    byte[] data = GenerateCommand(COMMAND.CMD_PARK, 1, 1, 1, 1);
                    serialPort.Write(data, 0, PCCMD_SIZE);
                }
                catch (Exception e) { Console.WriteLine($"{e.Message}"); }
            }
        }

        internal void Move(int fl, int rl, int rr, int fr = 0)
        {
            if (!IsConnected || !IsDeviceReady) return;

            lock (serialPort)
            {
                try
                {
                    fl = Math2.Clamp(fl, FrontAxisState.min, FrontAxisState.max);
                    rl = Math2.Clamp(rl, RearLeftAxisState.min, RearLeftAxisState.max);
                    rr = Math2.Clamp(rr, RearRightAxisState.min, RearRightAxisState.max);
                    fr = Math2.Clamp(fr, FrontRightAxisState.min, FrontRightAxisState.max);
                    byte[] data = GenerateCommand(COMMAND.CMD_MOVE, fl, rl, rr, fr);
                    serialPort.Write(data, 0, PCCMD_SIZE);
                }
                catch (Exception e) { Console.WriteLine($"{e.Message}"); }
            }
        }

        internal void Home()
        {
            if (!IsConnected) return;

            // Reset filters when homing to prevent stale state
            motionFilters.Reset();

            lock (serialPort)
            {
                try
                {
                    byte[] data = GenerateCommand(COMMAND.CMD_HOME, 1, 1, 1, 1);
                    serialPort.Write(data, 0, PCCMD_SIZE);
                }
                catch (Exception e) { Console.WriteLine($"{e.Message}"); }
            }
        }

        internal void AlarmReset()
        {
            if (!IsConnected) return;
            lock (serialPort)
            {
                try
                {
                    byte[] data = GenerateCommand(COMMAND.CMD_CLEAR_ALARM, 1, 1, 1, 1);
                    serialPort.Write(data, 0, PCCMD_SIZE);
                }
                catch (Exception e) { Console.WriteLine($"{e.Message}"); }
            }
        }

        internal void ReConnect() { Disconnect(); Connect(); }

        internal void SetSpeed(int speed, bool bLow = false)
        {
            if (!IsConnected) return;
            lock (serialPort)
            {
                try
                {
                    speed = Math2.Clamp(speed, settings.MinSpeed, settings.MaxSpeed);
                    byte[] data = GenerateCommand(bLow ? COMMAND.CMD_SET_LOW_SPEED : COMMAND.CMD_SET_SPEED, speed, speed, speed, speed);
                    serialPort.Write(data, 0, PCCMD_SIZE);
                }
                catch (Exception e) { Console.WriteLine($"{e.Message}"); }
            }
        }

        internal void SetAcceleration(int acc)
        {
            if (!IsConnected) return;
            lock (serialPort)
            {
                try
                {
                    byte[] data = GenerateCommand(COMMAND.CMD_SET_ACCEL, acc, acc, acc, acc);
                    serialPort.Write(data, 0, PCCMD_SIZE);
                }
                catch (Exception e) { Console.WriteLine($"{e.Message}"); }
            }
        }

        private readonly CancellationTokenSource tokenSource = new CancellationTokenSource();
        volatile bool bTaskRunning = false;
        private Task mainLoop;

        private void Disconnect()
        {
            lock (serialPort)
            {
                try
                {
                    if (serialPort.IsOpen)
                        serialPort.Close();
                }
                catch { }
            }

            if (Status == DeviceStatus.Connected)
            {
                FrontAxisState = new AXIS_STATE();
                RearLeftAxisState = new AXIS_STATE();
                RearRightAxisState = new AXIS_STATE();
                FrontRightAxisState = new AXIS_STATE();
                Status = DeviceStatus.Disconnected;
                OnDisconnected?.Invoke(this, new EventArgs());
            }
        }

        readonly Dictionary<int, bool> devMap = new Dictionary<int, bool>();
        int ConnectedLinearAxes => devMap.Count;

        private void Connect()
        {
            Status = DeviceStatus.Unknown;
            if (settings.ComPort == 0) return;

            devMap.Clear();

            lock (serialPort)
            {
                try
                {
                    serialPort.PortName = $"COM{settings.ComPort}";
                    serialPort.BaudRate = settings.BaudRate;
                    serialPort.WriteTimeout = 500;
                    serialPort.ReadTimeout = 500;
                    serialPort.Open();
                    serialPort.DiscardInBuffer();
                    serialPort.DiscardOutBuffer();
                }
                catch
                {
                    Status = DeviceStatus.ConnectionError;
                    return;
                }
            }

            Status = serialPort.IsOpen ? DeviceStatus.Connected : DeviceStatus.ConnectionError;

            if (IsConnected)
            {
                SetSpeed(settings.SpeedOverride);
                SetSpeed(settings.LowSpeedOverride, true);
                SetAcceleration(settings.Acceleration);
                RequestState();
                RequestPID();
                OnConnected?.Invoke(this, new EventArgs());
            }
        }

        internal byte[] GenerateCommand(COMMAND cmd, int par1, int par2, int par3, int par4 = 0)
        {
            byte[] arr = new byte[PCCMD_SIZE];
            GCHandle h = default;
            try
            {
                h = GCHandle.Alloc(arr, GCHandleType.Pinned);
                PCCMD cpmd = new PCCMD() { header = 0, cmd = cmd, len = (byte)PCCMD_SIZE, data = new int[] { par1, par2, par3, par4 } };
                Marshal.StructureToPtr(cpmd, h.AddrOfPinnedObject(), false);
                return arr;
            }
            catch { return arr; }
            finally { if (h.IsAllocated) h.Free(); }
        }

        public void RequestPID()
        {
            if (!IsConnected) return;
            lock (serialPort)
            {
                try
                {
                    byte[] data = GenerateCommand(COMMAND.CMD_GET_PID_STATE, 1, 1, 1, 1);
                    serialPort.Write(data, 0, PCCMD_SIZE);
                }
                catch (Exception e) { Console.WriteLine($"{e.Message}"); }
            }
        }

        public void RequestState()
        {
            if (!IsConnected) return;
            lock (serialPort)
            {
                try
                {
                    byte[] data = GenerateCommand(COMMAND.CMD_GET_STATE, 1, 1, 1, 1);
                    serialPort.Write(data, 0, PCCMD_SIZE);
                }
                catch (Exception e) { Console.WriteLine($"{e.Message}"); }
            }
        }

        private TelemetryDataSet tds = new TelemetryDataSet(null);
        public TelemetryDataSet Telemetry { get => tds; internal set => tds = value; }

        public void OnTelemetry(IVAzhureRacingApp _, TelemetryDataSet data)
        {
            tds = data;
            if (settings.mode == MODE.CollectingGameData)
            {
                if (data.CarData?.MotionData != null)
                {
                    string name = data.GamePlugin.Name;
                    if (settings.gamesData.FirstOrDefault(o => o.GameName == name) is GameData gd) gd.Update(data.CarData.MotionData);
                    else settings.gamesData.Add(new GameData(name));
                }
            }
            else bDataArrived = true;
        }

        public void Quit(IVAzhureRacingApp _)
        {
            settings?.SaveSettings(Path.Combine(AssemblyPath, "settings.json"));
            bTaskRunning = false;
            mainLoop.Wait();
            if (settings.ParkOnQuit) Park();
        }

        public void ShowProperties(IVAzhureRacingApp app)
        {
            using (SettingsForm form = new SettingsForm(this)) form.ShowDialog(app.MainForm);
        }

        void ICustomDevicePlugin.Initialize(IVAzhureRacingApp app) { }

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
                        if (plugin.ReadyToSend)
                        {
                            if (plugin.bDataArrived)
                            {
                                plugin.bDataArrived = false;
                                plugin.ProcessTelemetry2();
                                _tm = DateTime.Now;
                            }
                            else
                            {
                                if (plugin.Telemetry?.GamePlugin is IGamePlugin gamePlugin && gamePlugin.IsRunning)
                                {
                                    TimeSpan ts = DateTime.Now - _tm;
                                    if (ts.TotalSeconds > 1 && plugin.settings.ParkOnIdle) plugin.ProcessIdle();
                                    else plugin.ProcessTelemetry2();
                                }
                                else plugin.Telemetry = null;
                            }
                        }
                    }
                    catch { }
                    finally { Thread.Sleep(5); } // Determines UPDATE_RATE_HZ (200Hz)
                }
            }
        };

        private void ProcessIdle()
        {
            if (!settings.Enabled || !IsConnected || settings.mode != MODE.Run) return;

            // Feed 0 to filters to smoothly return platform to center
            // The filter will naturally decay towards 0 based on its physics
            float filteredPitch = motionFilters.Pitch.Filter(0);
            float filteredRoll = motionFilters.Roll.Filter(0);
            float filteredHeave = motionFilters.Heave.Filter(0);
            float filteredSway = motionFilters.Sway.Filter(0);
            float filteredSurge = motionFilters.Surge.Filter(0);

            float overal = settings.OveralCoefficient / 100.0f;

            (float posFL, float posFR, float posRL, float posRR) = CalculateLegPos(
                (filteredPitch + filteredSurge) * overal,
                (filteredRoll + filteredSway) * overal,
                -filteredHeave * overal
            );

            Move((int)posFL, (int)posRL, (int)posRR, (int)posFR);
        }

        internal void ResetActiveGameData()
        {
            if (App.GamePlugins.Where(game => game.IsRunning).FirstOrDefault() is IGamePlugin activeGame)
            {
                string name = activeGame.Name;
                if (settings.gamesData.FirstOrDefault(o => o.GameName == name) is GameData gd) gd.Reset();
                else { gd = new GameData(name); gd.Reset(); settings.gamesData.Add(gd); }
            }
        }

        internal GameData ActiveGameData
        {
            get
            {
                if (App.GamePlugins.Where(game => game.IsRunning).FirstOrDefault() is IGamePlugin activeGame)
                {
                    string name = activeGame.Name;
                    if (settings.gamesData.FirstOrDefault(o => o.GameName == name) is GameData gd) return gd;
                    else { gd = new GameData(name); settings.gamesData.Add(gd); return gd; }
                }
                return new GameData();
            }
        }

        internal void RestoreActiveGameData()
        {
            string name = tds?.GamePlugin?.Name;
            if (settings.gamesData.FirstOrDefault(o => o.GameName == name) is GameData gd) settings.gamesData.Remove(gd);
        }

        internal void DoHeave(float delta)
        {
            if (!settings.Enabled) return;
            float heave = (settings.Invert.HasFlag(MotionPlatformSettings.InvertFlags.InvertHeave) ? -1.0f : 1.0f) * Math2.Mapf(delta, -1, 1, -1.0f, 1.0f);
            heave = motionFilters.Heave.Filter(heave);
            (float posFL, float posFR, float posRL, float posRR) = CalculateLegPos(0, 0, -heave);
            Move((int)posFL, (int)posRL, (int)posRR, (int)posFR);
        }

        internal void DoRoll(float delta)
        {
            if (!settings.Enabled) return;
            float roll = (settings.Invert.HasFlag(MotionPlatformSettings.InvertFlags.InvertRoll) ? -1.0f : 1.0f) * Math2.Mapf(delta, -1, 1, -1.0f, 1.0f);
            roll = motionFilters.Roll.Filter(roll);
            (float posFL, float posFR, float posRL, float posRR) = CalculateLegPos(0, roll, 0);
            Move((int)posFL, (int)posRL, (int)posRR, (int)posFR);
        }

        internal void DoPitch(float delta)
        {
            if (!settings.Enabled) return;
            float pitch = (settings.Invert.HasFlag(MotionPlatformSettings.InvertFlags.InvertPitch) ? -1.0f : 1.0f) * Math2.Mapf(delta, -1, 1, -1.0f, 1.0f);
            pitch = motionFilters.Pitch.Filter(pitch);
            (float posFL, float posFR, float posRL, float posRR) = CalculateLegPos(pitch, 0, 0);
            Move((int)posFL, (int)posRL, (int)posRR, (int)posFR);
        }

        /// <summary>
        /// Calculate leg positions based on pitch/roll/heave [-1..1]
        /// </summary>
        internal (float, float, float, float) CalculateLegPos(float pitch, float roll, float heave)
        {
            float y = 0.5f * settings.DistanceFrontRearMM;
            float x = 0.5f * settings.DistanceLeftRightMM;
            float z = settings.ActuatorTravelMM;
            float heaveMM = z * 0.5f * Math.Abs(heave);

            pitch = Math2.Clamp(pitch, -1, 1);
            roll = Math2.Clamp(roll, -1, 1);

            Vector3 vHeave = new Vector3(0, 0, z * (0.5f + heave * 0.5f));

            float fpitch = new Vector3(settings.DistanceFrontRearMM, settings.DistanceLeftRightMM * roll, 0).Length();
            float froll = new Vector3(settings.DistanceFrontRearMM * pitch, settings.DistanceLeftRightMM, 0).Length();

            float pitchAngle = (float)Math.Atan2(z - heaveMM * 2f, fpitch) * settings.LimitPitchAngle;
            float rollAngle = (float)Math.Atan2(z - heaveMM * 2f, froll) * settings.LimitRollAngle;

            if (settings.OpenXRMotionCompensation && DeviceEnabled && IsConnected && IsDeviceReady)
            {
                rigPose?.SetPose(posPitchFilter.UpdatePosition(pitch * pitchAngle), posRollFilter.UpdatePosition(roll * rollAngle), posHeaveFilter.UpdatePosition(Math.Sign(heave) * heaveMM));
            }

            Vector3[] cornersZero = {
                new Vector3(-x, y, 0), new Vector3(x, y, 0),
                new Vector3(-x, -y,0), new Vector3(x, -y, 0),
            };

            if (ConnectedLinearAxes < 4) cornersZero[0] = cornersZero[1] = new Vector3(0f, y, 0);

            Matrix4x4 m = Matrix4x4.CreateRotationX(pitch * pitchAngle) * Matrix4x4.CreateRotationY(roll * rollAngle);

            float[] Lengths = new float[4];
            for (int t = 0; t < cornersZero.Length; t++)
            {
                var v = Vector3.Transform(cornersZero[t], m) + vHeave;
                Lengths[t] = (v - cornersZero[t]).Length() / z;
            }

            int posFL = (int)Math2.Mapf(Math2.Clamp(Lengths[0], 0, 1), 0, 1, FrontAxisState.min, FrontAxisState.max);
            int posFR = (int)Math2.Mapf(Math2.Clamp(Lengths[1], 0, 1), 0, 1, FrontRightAxisState.min, FrontRightAxisState.max);
            int posRL = (int)Math2.Mapf(Math2.Clamp(Lengths[2], 0, 1), 0, 1, RearLeftAxisState.min, RearLeftAxisState.max);
            int posRR = (int)Math2.Mapf(Math2.Clamp(Lengths[3], 0, 1), 0, 1, RearRightAxisState.min, RearRightAxisState.max);

            return (posFL, posFR, posRL, posRR);
        }

        int _oldGear = int.MinValue; float _gearSign = 1; DateTime _gearSwitched = DateTime.Now;
        GameData gd = null;

        /// <summary>
        /// 
        /// </summary>
        private void ProcessTelemetry2()
        {
            if (!settings.Enabled || !IsConnected || settings.mode != MODE.Run) return;

            string name = tds?.GamePlugin?.Name;
            if (gd == null || gd.GameName != name) gd = settings.gamesData.FirstOrDefault(o => o.GameName == name) ?? new GameData(name);

            float absPitch = Math.Max(Math.Abs(gd.minPitch), Math.Abs(gd.maxPitch));
            float absRoll = Math.Max(Math.Abs(gd.minRoll), Math.Abs(gd.maxRoll));
            float absHeave = Math.Max(Math.Abs(gd.minHeave), Math.Abs(gd.maxHeave));
            float absSway = Math.Max(Math.Abs(gd.minSway), Math.Abs(gd.maxSway));
            float absSurge = Math.Max(Math.Abs(gd.minSurge), Math.Abs(gd.maxSurge));

            if (absPitch <= float.Epsilon) absPitch = 1;
            if (absRoll <= float.Epsilon) absRoll = 1;
            if (absHeave <= float.Epsilon) absHeave = 1;
            if (absSway <= float.Epsilon) absSway = 1;
            if (absSurge <= float.Epsilon) absSurge = 1;

            float gearEffect = settings.GearChangeEffect / 100.0f;
            float overal = settings.OveralCoefficient / 100.0f;
            float pitchAmount = settings.PitchCoefficient / 100.0f;
            float rollAmount = settings.RollCoefficient / 100.0f;
            float heavAmount = settings.HeaveCoefficient / 100.0f;
            float swayAmoun = settings.SwayCoefficient / 100.0f;
            float surgeAmount = settings.SurgeCoefficient / 100.0f;

            if (_oldGear != tds.CarData.Gear && tds.CarData.Clutch < 0.99)
            {
                if (Math.Abs(tds.CarData.Speed) > settings.GearChangeMinSpeed) _gearSwitched = DateTime.Now;
                _gearSign = tds.CarData.Gear == 1 ? 0 : -1;
                _oldGear = tds.CarData.Gear;
            }
            gearEffect *= (float)(1 - tds.CarData.Clutch);

            // 1. Calculate RAW values
            float rawPitch = (settings.Invert.HasFlag(MotionPlatformSettings.InvertFlags.InvertPitch) ? -1.0f : 1.0f) * pitchAmount * Math2.Mapf(tds.CarData.MotionData.Pitch + gd.offsetPitch, -absPitch, absPitch, -1.0f, 1.0f, false, settings.ClipByRange);
            float rawRoll = (settings.Invert.HasFlag(MotionPlatformSettings.InvertFlags.InvertRoll) ? -1.0f : 1.0f) * rollAmount * Math2.Mapf(tds.CarData.MotionData.Roll + gd.offsetRoll, -absRoll, absRoll, -1.0f, 1.0f, false, settings.ClipByRange);
            float rawHeave = (settings.Invert.HasFlag(MotionPlatformSettings.InvertFlags.InvertHeave) ? -1.0f : 1.0f) * heavAmount * Math2.Mapf(tds.CarData.MotionData.Heave + gd.offsetHeave, -absHeave, absHeave, -1.0f, 1.0f, false, settings.ClipByRange);
            float rawSway = (settings.Invert.HasFlag(MotionPlatformSettings.InvertFlags.InvertSway) ? -1.0f : 1.0f) * swayAmoun * Math2.Mapf(tds.CarData.MotionData.Sway + gd.offsetSway, -absSway, absSway, -1.0f, 1.0f, false, settings.ClipByRange);
            float rawSurge = (settings.Invert.HasFlag(MotionPlatformSettings.InvertFlags.InvertSurge) ? -1.0f : 1.0f) * surgeAmount * Math2.Mapf(tds.CarData.MotionData.Surge + gd.offsetSurge, -absSurge, absSurge, -1.0f, 1.0f, false, settings.ClipByRange);

            // Gear switch effect on raw pitch
            float gearTS = (float)(DateTime.Now - _gearSwitched).TotalMilliseconds;
            if (gearTS < (settings.GearChangeRampUp + settings.GearChangeRampDown + settings.GearChangePulse))
            {
                float up = settings.GearChangeRampUp + settings.GearChangePulse;
                if (gearTS <= up) rawPitch += _gearSign * Math2.Mapf(gearTS, 0f, settings.GearChangeRampUp, 0, gearEffect, true);
                else rawPitch += _gearSign * Math2.Mapf(gearTS, up, up + settings.GearChangeRampDown, gearEffect, 0f, true);
            }

            // 2. Apply Filtering (using new LowPassFilter)
            float filteredPitch = motionFilters.Pitch.Filter(rawPitch);
            float filteredRoll = motionFilters.Roll.Filter(rawRoll);
            float filteredHeave = motionFilters.Heave.Filter(rawHeave);
            float filteredSway = motionFilters.Sway.Filter(rawSway);
            float filteredSurge = motionFilters.Surge.Filter(rawSurge);

            // 3. Blend Raw vs Filtered (SmoothCoefficient)
            float coeff = settings.SmoothCoefficient / 100.0f;
            float pitch = Math2.Mapf(coeff, 0.0f, 1.0f, rawPitch, filteredPitch);
            float roll = Math2.Mapf(coeff, 0.0f, 1.0f, rawRoll, filteredRoll);
            float heave = Math2.Mapf(coeff, 0.0f, 1.0f, rawHeave, filteredHeave);
            float sway = Math2.Mapf(coeff, 0.0f, 1.0f, rawSway, filteredSway);
            float surge = Math2.Mapf(coeff, 0.0f, 1.0f, rawSurge, filteredSurge);

            (float posFL, float posFR, float posRL, float posRR) = CalculateLegPos((pitch + surge) * overal, (roll + sway) * overal, -heave * overal);

            // Always send to prevent drift between game and reality
            Move((int)posFL, (int)posRL, (int)posRR, (int)posFR);
        }

        public const int cMaxAxesCount = 4;
        private readonly AXIS_STATE[] states = new AXIS_STATE[cMaxAxesCount];
        private PID_STATE pid_state = new PID_STATE();
        public bool IsPIDAvailable { get; private set; } = false;
        public PID_STATE PidState => pid_state;

        public AXIS_STATE FrontAxisState { get => states[0]; private set => states[0] = value; }
        public AXIS_STATE RearLeftAxisState { get => states[1]; private set => states[1] = value; }
        public AXIS_STATE RearRightAxisState { get => states[2]; private set => states[2] = value; }
        public AXIS_STATE FrontRightAxisState { get => states[3]; private set => states[3] = value; }

        public class AxisStateChanged : EventArgs
        {
            public AxisStateChanged(int addr, AXIS_STATE state) => (Addr, State) = (addr, state);
            public int Addr { get; private set; }
            public AXIS_STATE State { get; private set; }
        }
        public event EventHandler<AxisStateChanged> OnAxisStateChanged;
        public event EventHandler<PID_STATE> OnPidState;

        private readonly byte[] data = new byte[20];
        const int cFirstAddr = 10;
        const int cLastAddr = 13;
        const int cPIDAddr = 255;

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            // CRITICAL: Lock the port to prevent conflict with Write operations in other threads
            lock (serialPort)
            {
                // Double check if port is still open (can close between event trigger and handler)
                if (!serialPort.IsOpen) return;

                try
                {
                    while (serialPort.BytesToRead >= AXIS_STATE_SIZE + 2)
                    {
                        int addr = serialPort.ReadByte();
                        if (addr == cPIDAddr)
                        {
                            if (serialPort.ReadByte() == AXIS_STATE_SIZE)
                            {
                                if (serialPort.Read(data, 0, AXIS_STATE_SIZE) == AXIS_STATE_SIZE)
                                {
                                    try
                                    {
                                        pid_state = Marshalizable<PID_STATE>.FromBytes(data);
                                        IsPIDAvailable = true;
                                        OnPidState?.Invoke(this, pid_state);
                                    }
                                    catch { }
                                    continue;
                                }
                            }
                        }

                        if (addr >= cFirstAddr && addr <= cLastAddr)
                        {
                            if (serialPort.ReadByte() == AXIS_STATE_SIZE)
                            {
                                if (serialPort.Read(data, 0, AXIS_STATE_SIZE) == AXIS_STATE_SIZE)
                                {
                                    try
                                    {
                                        AXIS_STATE state = Marshalizable<AXIS_STATE>.FromBytes(data);
                                        states[addr - cFirstAddr] = state;

                                        if (state.mode != DEVICE_MODE.UNKNOWN)
                                            devMap[addr] = true;

                                        OnAxisStateChanged?.Invoke(this, new AxisStateChanged(addr, state));
                                    }
                                    catch { }
                                }
                            }
                        }
                    }
                }
                catch (IOException) { /* Port closed during read */ }
                catch (Exception ex) { Console.WriteLine($"Serial Error: {ex.Message}"); }
            }
        }

        internal void SetPIDValue(int value, COMMAND cmd)
        {
            if (!IsConnected) return;
            lock (serialPort)
            {
                try
                {
                    byte[] data = GenerateCommand(cmd, value, 1, 1, 1);
                    serialPort.Write(data, 0, PCCMD_SIZE);
                }
                catch (Exception e) { Console.WriteLine($"{e.Message}"); }
            }
        }

        internal void WritePID()
        {
            if (!IsConnected) return;
            lock (serialPort)
            {
                try
                {
                    byte[] data = GenerateCommand(COMMAND.CMD_STORE_PID, 1, 1, 1, 1);
                    serialPort.Write(data, 0, PCCMD_SIZE);
                }
                catch (Exception e) { Console.WriteLine($"{e.Message}"); }
            }
        }

        internal void ReadPID()
        {
            if (!IsConnected) return;
            lock (serialPort)
            {
                try
                {
                    byte[] data = GenerateCommand(COMMAND.CMD_RESTORE_PID, 1, 1, 1, 1);
                    serialPort.Write(data, 0, PCCMD_SIZE);
                }
                catch (Exception e) { Console.WriteLine($"{e.Message}"); }
            }
        }

        internal bool IsDeviceReady => ConnectedLinearAxes > 1;

        internal bool IsDeviceHomed => states[0].flags.HasFlag(DEVICE_FLAGS.STATE_HOMED) &&
                                       states[1].flags.HasFlag(DEVICE_FLAGS.STATE_HOMED) &&
                                       states[2].flags.HasFlag(DEVICE_FLAGS.STATE_HOMED);

        // THREAD SAFE properties
        internal bool IsConnected
        {
            get
            {
                lock (serialPort)
                {
                    try { return serialPort != null && serialPort.IsOpen; }
                    catch { return false; }
                }
            }
        }

        public bool ReadyToSend
        {
            get
            {
                lock (serialPort)
                {
                    try { return serialPort?.BytesToWrite == 0; }
                    catch { return false; }
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

        public float offsetHeave = 0;
        public float offsetSurge = 0;
        public float offsetSway = 0;

        public float offsetPitch = 0;
        public float offsetRoll = 0;
        public float offsetYaw = 0;

        public GameData()
        {
            maxABSVibration = 1;
            maxPitch = 30f / 180f;
            maxRoll = 30f / 180f;
            maxHeave = 1;
            maxSurge = 1;
            maxSway = 1;
            maxYaw = 1;

            minABSVibration = -1;
            minPitch = -30f / 180f;
            minRoll = -30f / 180f;
            minHeave = -1;
            minSurge = -1;
            minSway = -1;
            minYaw = -1;
        }

        public GameData(string gameName) : this()
        {
            GameName = gameName;
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

            offsetHeave = 0;
            offsetSurge = 0;
            offsetSway = 0;

            offsetPitch = 0;
            offsetRoll = 0;
            offsetYaw = 0;
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

    public class FilterSettings
    {
        public float[] Sway { get; set; } = { 1, 1, 0.02f, 1, 0.02f, 0.0f };
        public float[] Surge { get; set; } = { 1, 1, 0.02f, 1, 0.02f, 0.0f };
        public float[] Heave { get; set; } = { 1, 1, 0.02f, 1, 0.02f, 0.0f };
        public int Pitch { get; set; } = 3;
        public int Roll { get; set; } = 3;
        public float MaxInputData { get; set; } = 1f;
    }

    public enum MODE { Run, CollectingGameData, Test };

    public class MotionPlatformSettings : ICloneable
    {
        public string[] LinearAxesNames { get; set; } = { "Front", "Rear Left", "Rear Right", "Front Right" };
        public float[] Offsets { get; set; } = { 0, 0, 0, 0 };
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
        public int DataBits { get; set; } = 8;
        public StopBits StopBits { get; set; } = StopBits.One;
        public Handshake Handshake { get; set; } = Handshake.None;
        public bool RtsEnable { get; set; } = true;
        public bool DtrEnable { get; set; } = true;

        /// <summary>
        /// Статус разрешения
        /// </summary>
        public bool Enabled { get; set; } = true;
        public bool ParkOnIdle { get; set; } = true;
        public bool ParkOnQuit { get; set; } = true;
        public int SpeedOverride { get; set; } = 100;
        public int Acceleration { get; set; } = 5000;
        public int MaxSpeed { get; set; } = 120;
        public int MinSpeed { get; set; } = 10;
        public int LowSpeedOverride { get; set; } = 10;
        public int PitchCoefficient { get; set; } = 100;
        public int RollCoefficient { get; set; } = 100;
        public int HeaveCoefficient { get; set; } = 100;
        public int SurgeCoefficient { get; set; } = 100;
        public int SwayCoefficient { get; set; } = 100;
        public int OveralCoefficient { get; set; } = 100;
        public bool ClipByRange { get; set; } = false;
        public float Linearity { get; set; } = 1.0f; // not used since 2025-05-30
        public int SmoothCoefficient { get; set; } = 80;

        public int DistanceFrontRearMM { get; set; } = 500;
        public int DistanceLeftRightMM { get; set; } = 500;
        public int ActuatorTravelMM { get; set; } = 85;
        public int SeatOffsetMM { get; set; } = 400;

        public int GearChangeEffect { get; set; } = 30;
        public int GearChangeMinSpeed { get; set; } = 5;
        public int GearChangeRampUp { get; set; } = 150;
        public int GearChangePulse { get; set; } = 10;
        public int GearChangeRampDown { get; set; } = 150;

        public float LimitPitchAngle { get; set; } = 1;
        public float LimitRollAngle { get; set; } = 1;

        /// <summary>
        /// Enable or disable OpenXR Motion Compensation
        /// </summary>
        public bool OpenXRMotionCompensation { get; set; } = false;
        public float OpenXRMotionCompensationAngularSpeed { get; set; } = 10f;
        public float OpenXRMotionCompensationLinearSpeed { get; set; } = 100f;

        /// <summary>
        /// Returns max angle for Pitch, radians
        /// </summary>
        public float MaxPitchAngle
        {
            get { return (float)Math.Atan2(ActuatorTravelMM, DistanceFrontRearMM); }
        }

        /// <summary>
        /// Returns max angle for Roll, radians
        /// </summary>
        public float MaxRollAngle
        {
            get { return (float)Math.Atan2(ActuatorTravelMM, DistanceLeftRightMM); }
        }

        public FilterSettings FilterSettings { get; set; } = new FilterSettings();
        public float StepsPerMM { get; set; } = 200;
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
        /// <summary>
        /// Linear actuator mode
        /// </summary>
        public DEVICE_MODE mode; // MODE
        /// <summary>
        /// Linear actuator flags
        /// </summary>
        public DEVICE_FLAGS flags; // FLAGS
        /// <summary>
        /// Target speed, mm per second
        /// </summary>
        public ushort speedMMperSEC;
        /// <summary>
        /// Current position, pulses
        /// </summary>
        public int currentpos;
        /// <summary>
        /// Target position, pulses
        /// </summary>
        public int targetpos;
        /// <summary>
        /// Minimal position, pulses
        /// </summary>
        public int min;
        /// <summary>
        /// Maximal position, pulses
        /// </summary>
        public int max;
    }


    [StructLayout(LayoutKind.Sequential)]
    public struct PID_STATE
    {
        public byte version;
        public PID_FLAGS flags;
        public ushort masterPidBlend;
        public float masterPidKp;
        public float masterPidKi;
        public float masterPidKd;
        public float masterPidKs;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PCCMD
    {
        public byte header;
        public byte len;
        public COMMAND cmd;
        public byte reserved; // alignment byte
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.I4, SizeConst = 4)]
        public int[] data;
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
        SET_ALARM,
        CMD_SET_LOW_SPEED,
        CMD_SET_ACCEL,
        CMD_MOVE_SH,
        CMD_SET_PID_KP,      // 0x0C
        CMD_SET_PID_KI,      // 0x0D
        CMD_SET_PID_KD,      // 0x0E
        CMD_SET_PID_KS,      // 0x0F
        CMD_SET_PID_ENABLE,  // 0x10
        CMD_SET_PID_BLEND,   // 0x11
        CMD_GET_PID_STATE,   // 0x12
        CMD_STORE_PID,       // 0x13
        CMD_RESTORE_PID      // 0x14
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

    [Flags]
    public enum PID_FLAGS : byte
    {
        PID_NONE = 0,
        PID_ENABLED = 1,
        PID_MASTER_SYNC = 1 << 1,
        PID_DIAG_ENABLED = 1 << 2,  // Diagnostics: set false after validation if SimHub parsing is affected.
    };
}