using System;
using System.Drawing;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using vAzhureRacingAPI;

namespace Omsi2TelemetryPlugin
{
    public class Omsi2Game : IGamePlugin
    {
        public string Name => "OMSI 2";
        public uint SteamGameID => 252140U;
        public string[] ExecutableProcessName => new[] { "omsi", "openomsi" };

        // Должен совпадать с PORT в plugins/motion_udp.lua (openOMSI)
        private const int UdpTelemetryPort = 4123;

        private string sUserIcon = string.Empty;
        private string sExecutablePath = string.Empty;

        public string UserIconPath { get => sUserIcon; set => sUserIcon = value; }
        public string UserExecutablePath { get => sExecutablePath; set => sExecutablePath = value; }
        public bool IsRunning => Utils.IsProcessRunning(ExecutableProcessName);

        public event EventHandler<TelemetryUpdatedEventArgs> OnTelemetry;
        public event EventHandler OnGameStateChanged;
        public event EventHandler OnGameIconChanged;

        public Icon GetIcon() => Properties.Resources.Omsi2;
        public void ShowSettings(IVAzhureRacingApp app) { }

        public void Start(IVAzhureRacingApp app)
        {
            if (!Utils.RunSteamGame(SteamGameID))
                app.SetStatusText($"Steam Service is not running. Run {Name} manually!");
        }

        private readonly ProcessMonitor monitor;
        private readonly TelemetryDataSet dataSet;
        private readonly CustomSharedMemClient customSharedMemClient;
        private readonly UdpTelemetryClient udpClient;

        ~Omsi2Game()
        {
            monitor.Stop();
            customSharedMemClient?.StopTrhead();
            udpClient?.StopThread();
        }

        public Omsi2Game()
        {
            dataSet = new TelemetryDataSet(this);
            monitor = new ProcessMonitor(ExecutableProcessName);
            customSharedMemClient = new CustomSharedMemClient();
            udpClient = new UdpTelemetryClient(UdpTelemetryPort);

            customSharedMemClient.OnUserFunc += (s, e) => ProcessSharedMemory();
            udpClient.OnTelemetry += (s, json) => ProcessUdpTelemetry(json);

            monitor.OnProcessRunningStateChanged += (process, bRunning) =>
            {
                bool bIsOpenOmsi = monitor.ProcessNames.Contains("openomsi");

                if (bRunning)
                {
                    if (bIsOpenOmsi)
                        udpClient.StartThread();
                    else
                        customSharedMemClient.StartThread();
                }
                else
                {
                    customSharedMemClient?.StopTrhead();
                    udpClient?.StopThread();
                    dataSet.LoadDefaults();
                    OnTelemetry?.Invoke(this, new TelemetryUpdatedEventArgs(dataSet));
                }
                OnGameStateChanged?.Invoke(this, EventArgs.Empty);
            };
            monitor.Start();
        }

        private readonly byte[] dataStatic = new byte[Omsi2Telemetry.DataSize];

        private void ProcessSharedMemory()
        {
            try
            {
                using (var mmf = MemoryMappedFile.OpenExisting(
                    Omsi2Telemetry.SharedMemoryName,
                    MemoryMappedFileRights.Read))
                using (var mmfView = mmf.CreateViewStream(0L, dataStatic.Length, MemoryMappedFileAccess.Read))
                {
                    int bytesRead = mmfView.Read(dataStatic, 0, dataStatic.Length);
                    if (bytesRead != dataStatic.Length) return;

                    var telemetry = Omsi2Telemetry.Helpers.ReadFromBytes(dataStatic);

                    // Пропускаем данные на паузе
                    if (telemetry.pause > 0.5f) return;

                    FillDataSet(
                        throttle: telemetry.Throttle,
                        brake: telemetry.Brake,
                        clutch: telemetry.Clutch,
                        velocityMs: telemetry.Velocity,
                        engineRpm: telemetry.engine_n,
                        fuel: telemetry.engine_tank_content,
                        gearR: telemetry.cockpit_gangR,
                        gearN: telemetry.cockpit_gangN,
                        gear1: telemetry.cockpit_gang1,
                        gear2: telemetry.cockpit_gang2,
                        gear3: telemetry.cockpit_gang3,
                        engineOn: telemetry.engine_on,
                        lightsFern: telemetry.lights_fern,
                        blinkerL: telemetry.AI_Blinker_L,
                        blinkerR: telemetry.AI_Blinker_R,
                        surge: telemetry.Surge,
                        sway: telemetry.Sway,
                        heave: telemetry.Heave,
                        pitch: telemetry.Pitch,
                        roll: telemetry.Roll,
                        yaw: telemetry.Yaw,
                        ambientTemp: null);
                }
            }
            catch
            {
                ResetMotionData();
            }
        }

        // Изменили тип аргумента с JObject на OmsiUdpPacket
        private void ProcessUdpTelemetry(OmsiUdpPacket t)
        {
            try
            {
                if (t == null) return;

                // Пропускаем данные на паузе
                if (t.pause > 0.5f) return;

                FillDataSet(
                    throttle: t.throttle,
                    brake: t.brake,
                    clutch: t.clutch,
                    velocityMs: t.velocity,
                    engineRpm: t.engine_n,
                    fuel: t.engine_tank_content,
                    gearR: t.cockpit_gangR,
                    gearN: t.cockpit_gangN,
                    gear1: t.cockpit_gang1,
                    gear2: t.cockpit_gang2,
                    gear3: t.cockpit_gang3,
                    engineOn: t.engine_on,
                    lightsFern: t.lights_fern,
                    blinkerL: t.AI_Blinker_L,
                    blinkerR: t.AI_Blinker_R,
                    surge: t.surge,
                    sway: t.sway,
                    heave: t.heave,
                    pitch: t.pitch,
                    roll: t.roll,
                    yaw: t.yaw,
                    ambientTemp: null);
            }
            catch
            {
                ResetMotionData();
            }
        }


        // === Общее заполнение TelemetryDataSet (одинаково для обоих источников) ===
        private void FillDataSet(
            float throttle, float brake, float clutch,
            float velocityMs, float engineRpm, float fuel,
            float gearR, float gearN, float gear1, float gear2, float gear3,
            float engineOn, float lightsFern,
            float blinkerL, float blinkerR,
            float surge, float sway, float heave,
            float pitch, float roll, float yaw,
            float? ambientTemp)
        {
            var carData = dataSet.CarData;

            // === Управление ===
            carData.Throttle = throttle;
            carData.Brake = brake;
            carData.Clutch = clutch;

            // Скорость: м/с → км/ч
            carData.Speed = Math.Abs(velocityMs * 3.6f);
            carData.RPM = engineRpm;
            carData.MaxRPM = 2600f;
            carData.FuelLevel = fuel;

            // === Передача ===
            short gear = 1;
            if (gearR > 0.5f) gear = 0;
            else if (gear1 > 0.5f) gear = 2;
            else if (gear2 > 0.5f) gear = 3;
            else if (gear3 > 0.5f) gear = 4;
            else if (gearN > 0.5f) gear = 1;
            carData.Gear = gear;

            // === Электроника ===
            carData.Electronics = CarElectronics.None;
            if (engineOn > 0.5f)
                carData.Electronics |= CarElectronics.Ignition;
            if (lightsFern > 0.5f)
                carData.Electronics |= CarElectronics.Headlight;

            // === Поворотники ===
            carData.DirectionsLight = DirectionsLight.None;
            if (blinkerL > 0.5f && blinkerR > 0.5f)
                carData.DirectionsLight = DirectionsLight.Booth;
            else if (blinkerL > 0.5f)
                carData.DirectionsLight = DirectionsLight.Left;
            else if (blinkerR > 0.5f)
                carData.DirectionsLight = DirectionsLight.Right;

            // === Движение ===
            carData.MotionData = new AMMotionData()
            {
                Surge = surge / 10f,
                Sway = sway / 10f,
                Heave = heave / 10f,
                Pitch = pitch,
                Roll = roll,
                Yaw = yaw
            };

            // === Погода ===
            if (ambientTemp.HasValue)
                dataSet.WeatherData.AmbientTemp = ambientTemp.Value;

            OnTelemetry?.Invoke(this, new TelemetryUpdatedEventArgs(dataSet));
        }

        private void ResetMotionData()
        {
            dataSet.CarData.MotionData = new AMMotionData
            {
                Pitch = 0,
                Roll = 0,
                Yaw = 0,
                Surge = 0,
                Sway = 0,
                Heave = 0
            };
            OnTelemetry?.Invoke(this, new TelemetryUpdatedEventArgs(dataSet));
        }
    }

    // === UDP-слушатель телеметрии openOMSI (motion_udp.lua) ===
    public class UdpTelemetryClient
    {
        public event EventHandler<OmsiUdpPacket> OnTelemetry;

        private readonly int port;
        private volatile bool bRunning = false;

        public IPEndPoint m_endPoint;
        private UdpClient m_udpClient;

        public UdpTelemetryClient(int port) => this.port = port;

        public void StartThread()
        {
            if (bRunning) return;
            
            bRunning = true;
            m_endPoint = new IPEndPoint(IPAddress.Any, port);
            m_udpClient = new UdpClient(m_endPoint) { Client = { ReceiveTimeout = 250 } };
            m_udpClient.BeginReceive(new AsyncCallback(OnUdpData), m_udpClient);
        }

        public void StopThread()
        {
            bRunning = false;
            try
            {
                m_udpClient?.Dispose();
            }
            catch
            {

            }
            finally
            {
                m_udpClient = null;
                m_endPoint = null;
            }
        }

        private void OnUdpData(IAsyncResult result)
        {
            if (bRunning && m_udpClient != null)
            {
                try
                {
                    byte[] bytes = m_udpClient.EndReceive(result, ref m_endPoint);

                    string jsonString = Encoding.UTF8.GetString(bytes);

                    var packet = ObjectSerializeHelper.DeserializeJson<OmsiUdpPacket>(jsonString);

                    if (packet != null)
                    {
                        OnTelemetry?.Invoke(this, packet);
                    }
                }
                catch
                {
                }
                finally
                {
                    m_udpClient?.BeginReceive(new AsyncCallback(OnUdpData), m_udpClient);
                }
            }
        }
    }

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