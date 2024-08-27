using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using vAzhureRacingAPI;

namespace Nextion35Dash
{
    public class NextionDevice : DeviceClass, ICustomDevicePlugin
    {
        private Thread worker = null;
        private volatile bool _dataArrived = false;

        readonly string[] gears = new string[] { "R", "N", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19" };

        public string DeviceName => "Nextion";

        public string DeviceDescription => IsConnected ? $"{DeviceName}(COM{settings.ComPort})" : DeviceName;

        public string DeviceID => "vAzhureRacingNextion35Dash";

        DeviceStatus status = DeviceStatus.ConnectionError;

        private readonly SerialPort serialPort = new SerialPort() { WriteTimeout = 1000 };

        public override bool IsPrimaryDevice => settings.PrimaryDevice;
        public override void SetPrimaryDevice(bool bSet)
        {
            settings.PrimaryDevice = bSet;
        }

        public DeviceStatus Status
        {
            get
            {
                return status;
            }
            set
            {
                if (status != value)
                {
                    status = value;

                    switch (status)
                    {
                        case DeviceStatus.Connected:
                            OnConnected?.Invoke(this, new EventArgs());
                            break;
                        case DeviceStatus.Disconnected:
                            OnDisconnected?.Invoke(this, new EventArgs());
                            break;
                        case DeviceStatus.ConnectionError:
                            OnDisconnected?.Invoke(this, new EventArgs());
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Имя файла настроек устройства
        /// </summary>
        internal const string cSettingsFileName = "settings.json";
        internal const string cPresetsFileName = "presets.json";

        internal readonly string cCustomDirectory = "Custom";

        internal List<CustomLeds> customLeds = new List<CustomLeds>();

        internal void LoadCustomLeds(string assemblyPath)
        {
            customLeds.Clear();
            string path = Path.Combine(assemblyPath, cCustomDirectory);
            foreach (string fname in Directory.EnumerateFiles(path, "*.led", SearchOption.AllDirectories))
            {
                CustomLeds leds = new CustomLeds();
                if (leds.LoadFromFile(fname))
                {
                    customLeds.Add(leds);
                }
            }
        }

        /// <summary>
        /// Чтение настроек устройства из файла
        /// </summary>
        /// <param name="assemblyPath">Директория для сохранения настроек</param>
        internal void LoadSettings(string assemblyPath)
        {
            try
            {
                string json = File.ReadAllText(Path.Combine(assemblyPath, cSettingsFileName));
                settings = ObjectSerializeHelper.DeserializeJson<DeviceSettings>(json);

                string profilesPath = Path.Combine(assemblyPath, cPresetsFileName);
                if (File.Exists(profilesPath))
                {
                    json = File.ReadAllText(profilesPath);
                    customLeds = ObjectSerializeHelper.DeserializeJson<List<CustomLeds>>(json);
                }
                else
                    LoadCustomLeds(assemblyPath);
            }
            catch { }
        }

        /// <summary>
        /// Сохранение настроек устройства в файл
        /// </summary>
        /// <param name="assemblyPath">Директория для сохранения настроек</param>
        internal void SaveSettings(string assemblyPath)
        {
            string json = "";
            try
            {
                json = File.ReadAllText(Path.Combine(assemblyPath, cSettingsFileName));
            }
            catch { }

            string jsonNew = ObjectSerializeHelper.GetJson(settings);

            if (json != jsonNew)
            {
                try
                {
                    File.WriteAllText(Path.Combine(assemblyPath, cSettingsFileName), jsonNew);
                }
                catch { }
            }

            json = "";
            try
            {
                json = File.ReadAllText(Path.Combine(assemblyPath, cPresetsFileName));
            }
            catch { }

            jsonNew = customLeds.GetJson();

            if (json != jsonNew)
            {
                try
                {
                    File.WriteAllText(Path.Combine(assemblyPath, cPresetsFileName), jsonNew);
                }
                catch { }
            }
        }

        bool bEnabled = false;

        public bool DeviceEnabled
        {
            get
            {
                return bEnabled;
            }
            set
            {
                bEnabled = value;
            }
        }

        public Image DevicePictogram => Properties.Resources.NextionDash35small;

        DeviceSettings settings = new DeviceSettings();

        public DeviceSettings Settings
        {
            get
            {
                return settings;
            }
            set
            {
                if (settings.ComPort != value.ComPort || settings.BaudRate != value.BaudRate)
                {
                    settings = value;
                    ReConnect();
                }
                else
                    settings = value;
            }
        }

        /// <summary>
        /// Повторное подключение с новыми настройками
        /// </summary>
        public void ReConnect()
        {
            Disconnect();
            Connect();
        }

        /// <summary>
        /// Статус соединения с портом
        /// </summary>
        public override bool IsConnected
        {
            get
            {
                return serialPort != null && serialPort.IsOpen;
            }
        }

        /// <summary>
        /// Соединение по последовательному порту
        /// </summary>
        public void Connect()
        {
            if (Settings.ComPort == 0)
                return;

            serialPort.PortName = $"COM{Settings.ComPort}";
            serialPort.BaudRate = Settings.BaudRate;

            try
            {
                serialPort.Open();
            }
            catch
            {
                Status = DeviceStatus.ConnectionError;
                return;
            }

            Status = serialPort.IsOpen ? DeviceStatus.Connected : DeviceStatus.ConnectionError;

            if (serialPort.IsOpen)
            {
                if (!SendData("page", $"page {settings.IntroPage}"))
                {
                    return;
                }

                ClearCommandsCash();

                UpdateLeds();

                bAbort = false;
                worker = new Thread(new ThreadStart(Main_Thread));
                worker.Start();
            }
        }

        private const int cTimeOut = 5000;

        /// <summary>
        /// Отсоединение устройства
        /// </summary>
        public void Disconnect()
        {
            if (IsConnected || bRunning)
            {
                bAbort = true;
                DateTime dt = DateTime.Now;
                while (bRunning && (DateTime.Now - dt).TotalMilliseconds < cTimeOut)
                    Thread.Sleep(100);
                try
                {
                    serialPort.Close();
                }
                catch { }
                finally
                {
                    worker = null;
                    Status = DeviceStatus.Disconnected;
                }
            }
        }

        public event EventHandler OnConnected;
        public event EventHandler OnDisconnected;

        public void ShowProperties(IVAzhureRacingApp app)
        {
            using (Form form = new NextionPropertiesForm(this, app))
            {
                form.ShowDialog(app.MainForm);
            }
        }

        /// <summary>
        /// Инициализация устройства
        /// </summary>
        /// <param name="app"></param>
        public void Initialize(IVAzhureRacingApp app)
        {
            Connect();
        }

        #region Main

        bool GetBlinkState()
        {
            const int interval = 500;
            TimeSpan elapsedSpan = new TimeSpan(DateTime.Now.Ticks);
            return elapsedSpan.TotalMilliseconds % (interval * 2) <= interval;
        }

        const int cYellow565 = 65504;
        const int cBlue565 = 31;
        const int cOrange565 = 64512;
        const int cBlack565 = 0;
        const int cGray565 = 10565;
        const int cWhite565 = 65535;
        const int cRed565 = 63488;
        const int cGreen565 = 2016;
        const int cDarkGreen565 = 1024;
        const int cBlue3420 = 3420;

        const int cTimeout = 5; // seconds

        DateTime _timeLastFrame = DateTime.Now;

        volatile bool bAbort = false;
        volatile bool bRunning = false;
        readonly byte[] led_data = new byte[Marshal.SizeOf(typeof(LED_DATA))];

        byte GetLedsState()
        {

            byte state = 0;

            if (m_dataSet.CarData.ShiftUpRPM > 0 && m_dataSet.CarData.RPM >= m_dataSet.CarData.ShiftUpRPM)
                return 0xFF;

            float t = m_dataSet.CarData.MaxRPM > 0 ? (float)m_dataSet.CarData.RPM / (float)m_dataSet.CarData.MaxRPM : 0;
            t = (t - 0.5f) * 9 / 0.5f;

            if (t > 0)
                for (int b = 0; b < (int)t; b++)
                    state |= (byte)(1 << b);

            return state;
        }

        public void ActivatePage(string page)
        {
            SendData("page", page, true);
        }

        public void TestBrightness(bool bOn = true)
        {
            LED_DATA data = new LED_DATA()
            {
                cmd = 0,
                brightness = (byte)(settings.EnableLeds ? settings.LedBrightness : 0),
                rev_mask = (byte)(bOn ? 0xff : 0),
                lim_mask = 1 | (1 << 7),
                flags = 0x00,
                limitator = new RGB8[] { new RGB8 { R = 0, G = 0, B = 255 } },
                color = new RGB8[] {
                    new RGB8 { R = 255, G = 255, B = 255 },
                    new RGB8 { R = 255, G = 255, B = 255 },
                    new RGB8 { R = 255, G = 255, B = 255 },
                    new RGB8 { R = 255, G = 255, B = 255 },
                    new RGB8 { R = 255, G = 255, B = 255 },
                    new RGB8 { R = 255, G = 255, B = 255 },
                    new RGB8 { R = 255, G = 255, B = 255 },
                    new RGB8 { R = 255, G = 255, B = 255 },
                }
            };

            byte[] raw = GetBytes(data);

            if (!raw.SequenceEqual(led_data))
            {
                Array.Copy(raw, led_data, led_data.Length);
                WriteBytes(raw);
            }
        }

        bool WriteBytes(byte[] data)
        {
            if (IsConnected)
            {
                lock (serialPort)
                {
                    try
                    {
                        serialPort.Write(data, 0, data.Length);
                    }
                    catch
                    {
                        Status = DeviceStatus.ConnectionError;
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// TODO: instead of led mask - levels argument, change color to black (off led)
        /// blinking flag instead of rev_mask and lim_mask
        /// </summary>
        byte UpdateLeds(RGB8[] rGB8s = null)
        {
            // LED data
            byte ledstate = GetLedsState();
            byte[] raw;
            if (rGB8s == null)
            {
                LED_DATA data = new LED_DATA()
                {
                    cmd = 0,
                    brightness = (byte)(settings.EnableLeds ? settings.LedBrightness : 0),
                    rev_mask = ledstate,
                    lim_mask = 1 | (1 << 7),
                    flags = m_dataSet.CarData.Electronics.HasFlag(CarElectronics.Limiter) ? (byte)0x01 : (byte)0x00,
                    limitator = new RGB8[] { new RGB8 { R = 0, G = 0, B = 255 } },
                    color = ledstate == 0xff ?
                    new RGB8[] {
                            new RGB8 { R = 255, G = 0, B = 0 },
                            new RGB8 { R = 255, G = 0, B = 0 },
                            new RGB8 { R = 255, G = 0, B = 0 },
                            new RGB8 { R = 255, G = 0, B = 0 },
                            new RGB8 { R = 255, G = 0, B = 0 },
                            new RGB8 { R = 255, G = 0, B = 0 },
                            new RGB8 { R = 255, G = 0, B = 0 },
                            new RGB8 { R = 255, G = 0, B = 0 },
                    } :
                    new RGB8[] {
                            new RGB8 { R = 0, G = 255, B = 0 },
                            new RGB8 { R = 0, G = 255, B = 0 },
                            new RGB8 { R = 0, G = 255, B = 0 },
                            new RGB8 { R = 255, G = 255, B = 0 },
                            new RGB8 { R = 255, G = 255, B = 0 },
                            new RGB8 { R = 255, G = 255, B = 0 },
                            new RGB8 { R = 255, G = 0, B = 0 },
                            new RGB8 { R = 255, G = 0, B = 0 },
                    }
                };

                bool bBlinkLeft = m_dataSet.CarData.DirectionsLight.HasFlag(DirectionsLight.Left);
                bool bBlinkRight = m_dataSet.CarData.DirectionsLight.HasFlag(DirectionsLight.Right);

                if (Settings.DefaultPage != "truck" && (bBlinkLeft || bBlinkRight))
                {
                    data.lim_mask = (byte)((bBlinkLeft ? 0x03 : 0) | (bBlinkRight ? 0xc0 : 0));
                    data.limitator = new RGB8[] { new RGB8 { R = 255, G = 255, B = 0 } };
                    data.flags = 0x01;
                }

                raw = GetBytes(data);
            }
            else
            {
                LED_DATA data = new LED_DATA()
                {
                    cmd = 0,
                    brightness = (byte)(settings.EnableLeds ? settings.LedBrightness : 0),
                    rev_mask = 0xFF,
                    lim_mask = 1 | (1 << 7),
                    flags = m_dataSet.CarData.Electronics.HasFlag(CarElectronics.Limiter) ? (byte)0x01 : (byte)0x00,
                    limitator = new RGB8[] { new RGB8 { R = 0, G = 0, B = 255 } },
                    color = rGB8s
                };
                raw = GetBytes(data);
            }

            if (!raw.SequenceEqual(led_data))
            {
                Array.Copy(raw, led_data, led_data.Length);
                WriteBytes(raw);
            }

            return ledstate;
        }

        private CustomLeds currentLedProfile = null;
        string oldPage = "";

        bool _oldLedState = false;

        readonly string[] days = { "MON", "TUE", "WED", "THU", "FRI", "SAT", "SUN" };

        /// <summary>
        /// Основной цикл процесса
        /// </summary>
        private void Main_Thread()
        {
            bRunning = true;

            // Ограничиваем FPS в пределах 25-50
            int fps = Math2.Clamp(Settings.Frequency, 25, 50);

            while (!bAbort)
            {
                if (!IsConnected)
                    break;

                if (_oldLedState != Settings.EnableLeds)
                {
                    UpdateLeds();
                    _oldLedState = Settings.EnableLeds;
                }

                if (!Settings.Enabled)
                {
                    Thread.Sleep(1000);
                    continue;
                }

                try
                {
                    AMSessionInfo sessionInfo = m_dataSet.SessionInfo;
                    AMCarData carData = m_dataSet.CarData;
                    AMWeatherData weatherData = m_dataSet.WeatherData;
                    var userData = m_dataSet.UserData;

                    if ((Nextion35Plugin.sPlugin.GetPrimaryDevice() == this) && _dataArrived)
                    {
                        _dataArrived = false;
                        _timeLastFrame = DateTime.Now;

                        if (oldPage != Settings.DefaultPage)
                        {
                            ClearCommandsCash();
                            oldPage = Settings.DefaultPage;

                        }

                        /*bool bPageChanged = */SendData("page", $"page {settings.DefaultPage}");

                        TimeSpan current = TimeSpan.FromMilliseconds(sessionInfo.CurrentLapTime > 0 ? sessionInfo.CurrentLapTime : 0);

                        if (settings.DefaultPage == "truck")
                        {
                            int zS = (int)(337 + Math2.Mapd(carData.Speed, 0, 128.747537, 0, 228, true)) % 360;
                            int zR = (int)(343 + Math2.Mapd(carData.RPM, 0, 3000, 0, 217, true)) % 360;
                            int zF = (int)(343 + Math2.Mapd(carData.FuelLevel, 0, carData.FuelCapacity, 0, 213, true)) % 360;
                            int zW = (int)(343 + Math2.Mapd((carData.WaterTemp * 1.8) + 32, 100, 300, 0, 213, true)) % 360; // Fahrenheit
                            int zO = (int)(343 + Math2.Mapd(carData.OilPressure, 0, 100, 0, 213, true)) % 360; // PSI
                            
                            bool bBlinkLeft = m_dataSet.CarData.DirectionsLight.HasFlag(DirectionsLight.Left);
                            bool bBlinkRight = m_dataSet.CarData.DirectionsLight.HasFlag(DirectionsLight.Right);
                            bool bEngine = carData.Electronics.HasFlag(CarElectronics.Ignition);
                            int va0 = (bBlinkLeft ? 1 : 0) | (bBlinkRight ? 2 : 0);

                            SendData("va0", FormattableString.Invariant($"va0.val={va0}"));
                            SendData("zS", FormattableString.Invariant($"zS.val={zS}"));
                            SendData("zR", FormattableString.Invariant($"zR.val={zR}"));
                            SendData("zW", FormattableString.Invariant($"zW.val={zW}"));
                            SendData("zF", FormattableString.Invariant($"zF.val={zF}"));
                            SendData("zO", FormattableString.Invariant($"zO.val={zO}"));
                            SendData("pB", bEngine ? "vis pB,0" : "vis pB,1");

                            if (userData.ContainsKey("bool.truckBrakeParking") && userData["bool.truckBrakeParking"] is bool bParkingBrake)
                                SendData("pB", bParkingBrake ? "vis pP,1" : "vis pP,0");
                            else
                                SendData("pB", "vis pP,0");

                            if (userData.ContainsKey("float.truckCruise_controlSpeedMS") && userData["float.truckCruise_controlSpeedMS"] is float fCruiseControl)
                                SendData("pC", fCruiseControl > 0 ? "vis pC,1" : "vis pC,0");
                            else
                                SendData("pC", "vis pC,0");

                            if (userData.ContainsKey("float.adBlueFuelCapacity") && userData.ContainsKey("float.truckAdblueFuelLevelLiters") &&
                                userData["float.adBlueFuelCapacity"] is float capacity && userData["float.truckAdblueFuelLevelLiters"] is float level)
                            {
                                int zF1 = (int)(343 + Math2.Mapd(level, 0, capacity, 0, 213, true)) % 360;
                                SendData("z0", FormattableString.Invariant($"z0.val={zF1}"));
                            }

                            if (userData.ContainsKey("uint.gameTime") && userData["uint.gameTime"] is uint gameTime)
                            {
                                var tm = TimeSpan.FromMinutes(gameTime);
                                int day = tm.Days % 7;
                                SendData("curlap", $"cur.txt=\"{days[day]} {tm.ToString("hh':'mm")}\"");
                            }
                            else
                            {
                                SendData("curlap", $"cur.txt=\"\"");
                            }

                            if (sessionInfo.PitSpeedLimit > 0)
                                SendData("spd.bco", carData.Speed >= sessionInfo.PitSpeedLimit + 5 ? "spd.bco=" + cRed565.ToString() : "spd.bco=" + cBlack565.ToString());
                            else
                                SendData("spd.bco", "spd.bco=" + cBlack565.ToString());

                            userData.TryGetValue("bool.parkingLights", out object bParkingLights);
                            userData.TryGetValue("bool.lowBeamLight", out object bLowBeamLight);
                            userData.TryGetValue("bool.hiBeamLight", out object bHiBeamLight);

                            if (bHiBeamLight is bool lightHi && lightHi)
                                SendData("Headlight", "hl.pic=11");
                            else if (bLowBeamLight is bool lightLo && lightLo)
                                SendData("Headlight", "hl.pic=3");
                            else if (bParkingLights is bool lightPark && lightPark)
                                SendData("Headlight", "hl.pic=12");
                            else
                                SendData("Headlight", "hl.pic=4");
                        }
                        else
                        {
                            string curlap = current.TotalMilliseconds > 0 ? current.ToString("mm':'ss':'fff") : "--:--:--";
                            SendData("curlap", $"cur.txt=\"{curlap}\"");
                            SendData("Headlight", "hl.pic=" + (carData.Electronics.HasFlag(CarElectronics.Headlight) ? 3 : 4));
                        }

                        string gear = (carData.Gear >= 0 && carData.Gear < gears.Length) ? gears[carData.Gear].ToString() : " ";

                        int bar = carData.MaxRPM > 0 ? (int)(100.0f * ((float)carData.RPM / (float)carData.MaxRPM)) : 0;

                        TimeSpan best = TimeSpan.FromMilliseconds(sessionInfo.BestLapTime > 0 ? sessionInfo.BestLapTime : 0);
                        TimeSpan diff = TimeSpan.FromMilliseconds(Math.Abs(sessionInfo.CurrentDelta));
                        char diffSign = sessionInfo.CurrentDelta >= 0 ? '+' : '-';

                        if (settings.EnableLeds)
                        {
                            if (currentLedProfile?.Game != m_dataSet.GamePlugin?.Name)
                                currentLedProfile = null;

                            if (customLeds.Count > 0 && (currentLedProfile == null || !customLeds.Contains(currentLedProfile)))
                            {
                                currentLedProfile = customLeds.Where(p => p.Game == m_dataSet.GamePlugin?.Name && p.Vechicle == carData.CarName).FirstOrDefault();
                            }

                            if (currentLedProfile != null)
                            {
                                UpdateLeds(currentLedProfile.GetLedState((int)carData.RPM).Leds);
                            }
                            else
                                UpdateLeds();
                        }

                        // Main
                        SendData("gear", FormattableString.Invariant($"gear.txt=\"{gear}\""));
                        SendData("position", "pos.txt=\"" + (sessionInfo.CurrentPosition > 0 ? sessionInfo.CurrentPosition.ToString(CultureInfo.InvariantCulture) : "") + '\"');
                        SendData("lap", "lap.txt=\"" + (sessionInfo.CurrentLapNumber > 0 ? sessionInfo.CurrentLapNumber.ToString(CultureInfo.InvariantCulture) : "") + '\"');
                        SendData("rpm", FormattableString.Invariant($"rpm.txt=\"{(int)carData.RPM}\""));
                        SendData("speed", FormattableString.Invariant($"spd.txt=\"{(settings.SpeedUnits == DeviceSettings.SpeedUnitsEnum.kmph ? (int)carData.Speed : (int)(carData.Speed * 0.62137119))}\""));
                        SendData("rpmBar", FormattableString.Invariant($"bar.val={bar}"));
                        SendData("fuel", FormattableString.Invariant($"fuel.txt=\"{carData.FuelLevel:N2}\""));
                        SendData("brake bias", FormattableString.Invariant($"bias.txt=\"{carData.BrakeBias:N1}\""));

                        string bestlap = best.TotalMilliseconds > 0 ? best.ToString("mm':'ss':'fff") : "--:--:--";

                        SendData("bestlap", $"best.txt=\"{bestlap}\"");
                        string delta = "--.--";

                        if (sessionInfo.CurrentDelta != -1000000)
                        {
                            if (diff.TotalMinutes >= 1)
                                delta = string.Format("{0}{1}", diffSign, diff.ToString("mm':'ss'.'f"));
                            else
                                delta = string.Format("{0}{1}", diffSign, diff.ToString("ss'.'fff"));
                        }

                        SendData("delta", $"delta.txt=\"{delta}\"");
                        SendData("delta txt color", "delta.pco=" + cWhite565.ToString());
                        SendData("delta color", "delta.bco=" + (sessionInfo.CurrentDelta > 0 ? cRed565 : Math.Abs(sessionInfo.CurrentDelta) < 10 ? cBlack565 : cDarkGreen565)).ToString();

                        int fuelClr = carData.FuelCapacity > 0 && carData.FuelLevel / carData.FuelCapacity < 0.1 ? cRed565 : cBlack565;
                        SendData("fuel bco", "fuel.bco=" + fuelClr.ToString());

                        // Electronics
                        if (carData.Electronics.HasFlag(CarElectronics.DRS))
                        {
                            SendData("DRS", "drs.pco=" + cWhite565.ToString());
                            SendData("DRS BG", "drs.bco=" + cBlue565.ToString());
                        }
                        else
                        if (carData.Electronics.HasFlag(CarElectronics.DRS_EN))
                        {
                            SendData("DRS", "drs.pco=" + cBlack565.ToString());
                            SendData("DRS BG", "drs.bco=" + cYellow565.ToString());
                        }
                        else
                        {
                            SendData("DRS", "drs.pco=" + cGray565.ToString());
                            SendData("DRS BG", "drs.bco=" + cBlack565.ToString());
                        }

                        SendData("ABS", "abs.pco=" + (carData.Electronics.HasFlag(CarElectronics.ABS) ? cYellow565.ToString() : cWhite565.ToString()));
                        SendData("TCS", "tc.pco=" + (carData.Electronics.HasFlag(CarElectronics.TCS) ? cBlue3420.ToString() : cWhite565.ToString()));

                        SendData("TC level", carData.TcLevel >= 0 ? $"tc.txt=\"{carData.TcLevel}\"" : "tc.txt=\"\"");
                        SendData("TC level color", carData.Electronics.HasFlag(CarElectronics.TCS) ? $"drs.pco={cBlue565}" : $"drs.pco={cBlack565}");
                        SendData("TC2 level", carData.TcLevel2 >= 0 ? $"tc2.txt=\"{carData.TcLevel2}\"" : "tc2.txt=\"\"");
                        SendData("ABS level", carData.AbsLevel >= 0 ? $"abs.txt=\"{carData.AbsLevel}\"" : "abs.txt=\"\"");
                        SendData("ABS color", carData.Electronics.HasFlag(CarElectronics.ABS) ? $"drs.pco={cYellow565}" : $"drs.pco={cBlack565}");
                        SendData("MAP level", carData.EngineMap >= 0 ? $"map.txt=\"{carData.EngineMap}\"" : "map.txt=\"\"");
                        SendData("front compound", $"cm1.txt=\"{carData.Tires[0].Compound}\"");
                        SendData("rear compound", $"cm2.txt=\"{carData.Tires[2].Compound}\"");

                        SendData("Oil Temp", $"ot.txt=\"{carData.OilTemp:N1}\"");
                        SendData("Oil Press", $"op.txt=\"{carData.OilPressure:N1}\"");
                        SendData("Water Temp", $"wt.txt=\"{carData.WaterTemp:N1}\"");
                        SendData("Water Press", $"wp.txt=\"{carData.WaterPressure:N1}\"");

                        if (sessionInfo.RemainingTime > 0)
                        {
                            TimeSpan remain = TimeSpan.FromMilliseconds(sessionInfo.RemainingTime);
                            if (remain.Hours > 0)
                            {
                                SendData("remain", $"t0.txt=\"{remain:hh':'mm':'ss}\"");
                            }
                            else
                            {
                                if (remain.Minutes > 0)
                                    SendData("remain", $"t0.txt=\"{remain:mm':'ss}\"");
                                else
                                    SendData("remain", $"t0.txt=\"{remain:ss'.'f}\"");
                            }
                        }
                        else
                        {
                            if (sessionInfo.RemainingLaps > 0)
                            {
                                SendData("remain", $"t0.txt=\"{sessionInfo.RemainingLaps}\"");
                            }
                            else
                            {
                                SendData("remain", $"t0.txt=\"\"");
                            }
                        }

                        if (carData.Flags.HasFlag(TelemetryFlags.FlagGreen))
                            SendData("YellowFlag", "fly.bco=" + cDarkGreen565.ToString());
                        else
                            SendData("YellowFlag", "fly.bco=" + (carData.Flags.HasFlag(TelemetryFlags.FlagYellow) ? cYellow565.ToString() : cBlack565.ToString()));

                        if (carData.Flags.HasFlag(TelemetryFlags.FlagBlue))
                            SendData("BlueFlag", "flb.bco=" + cBlue565.ToString());
                        else
                        {
                            if (carData.Flags.HasFlag(TelemetryFlags.FlagWhite))
                                SendData("BlueFlag", "flb.bco=" + cWhite565.ToString());
                            else
                                SendData("BlueFlag", "flb.bco=" + cBlack565.ToString());
                        }

                        if (carData.Tires != null)
                        {
                            if (carData.Tires[0].Detached)
                                SendData("pressure FL", "pfl.txt=\"\"");
                            else
                                SendData("pressure FL", settings.PressureUnits == DeviceSettings.PressureUnitsEnum.kPa ? FormattableString.Invariant($"pfl.txt=\"{carData.Tires[0].Pressure:N1}\"") :
                                    FormattableString.Invariant($"pfl.txt=\"{carData.Tires[0].Pressure / 6.89476:N1}\""));
                            if (carData.Tires[1].Detached)
                                SendData("pressure FR", "pfr.txt=\"\"");
                            else
                                SendData("pressure FR", settings.PressureUnits == DeviceSettings.PressureUnitsEnum.kPa ? FormattableString.Invariant($"pfr.txt=\"{carData.Tires[1].Pressure:N1}\"") :
                                    FormattableString.Invariant($"pfr.txt=\"{carData.Tires[1].Pressure / 6.89476:N1}\""));
                            if (carData.Tires[2].Detached)
                                SendData("pressure RL", "prl.txt=\"\"");
                            else
                                SendData("pressure RL", settings.PressureUnits == DeviceSettings.PressureUnitsEnum.kPa ? FormattableString.Invariant($"prl.txt=\"{carData.Tires[2].Pressure:N1}\"") :
                                    FormattableString.Invariant($"prl.txt=\"{carData.Tires[2].Pressure / 6.89476:N1}\""));
                            if (carData.Tires[3].Detached)
                                SendData("pressure RR", "prr.txt=\"\"");
                            else
                                SendData("pressure RR", settings.PressureUnits == DeviceSettings.PressureUnitsEnum.kPa ? FormattableString.Invariant($"prr.txt=\"{carData.Tires[3].Pressure:N1}\"") :
                                    FormattableString.Invariant($"prr.txt=\"{carData.Tires[3].Pressure / 6.89476:N1}\""));

                            // Temperature
                            double tfl = carData.Tires == null ? 30 : (carData.Tires[0].Temperature[0] + carData.Tires[0].Temperature[1] + carData.Tires[0].Temperature[2]) / 3;
                            double tfr = carData.Tires == null ? 30 : (carData.Tires[1].Temperature[0] + carData.Tires[1].Temperature[1] + carData.Tires[1].Temperature[2]) / 3;
                            double trl = carData.Tires == null ? 30 : (carData.Tires[2].Temperature[0] + carData.Tires[2].Temperature[1] + carData.Tires[2].Temperature[2]) / 3;
                            double trr = carData.Tires == null ? 30 : (carData.Tires[3].Temperature[0] + carData.Tires[3].Temperature[1] + carData.Tires[3].Temperature[2]) / 3;

                            if (carData.Tires[0].Detached)
                                SendData("temp FL", $"fl.pco={cBlack565}");
                            else
                                SendData("temp FL", $"fl.pco={GetTempColor(tfl)}");

                            if (carData.Tires[1].Detached)
                                SendData("temp FR", $"fr.pco={cBlack565}");
                            else
                                SendData("temp FR", $"fr.pco={GetTempColor(tfr)}");
                            if (carData.Tires[2].Detached)
                                SendData("temp RL", $"rl.pco={cBlack565}");
                            else
                                SendData("temp RL", $"rl.pco={GetTempColor(trl)}");
                            if (carData.Tires[3].Detached)
                                SendData("temp RR", $"rr.pco={cBlack565}");
                            else
                                SendData("temp RR", $"rr.pco={GetTempColor(trr)}");
                        }
                        SendData("Air temp", FormattableString.Invariant($"tair.txt=\"{weatherData.AmbientTemp:N1}C\""));
                        SendData("Track temp", FormattableString.Invariant($"ttr.txt=\"{weatherData.TrackTemp:N1}C\""));
                    }

                    string datetime = DateTime.Now.ToString("dd MMM yyyy HH:mm:ss", DateTimeFormatInfo.InvariantInfo);

                    if ((DateTime.Now - _timeLastFrame).TotalSeconds > cTimeout)
                    {
                        SendData("page", $"page {settings.SleepPage}", true);
                        SendData("clock", $"dt.txt=\"{datetime}\"");
                        UpdateLeds();
                        ClearCommandsCash();
                        Thread.Sleep(1000);
                        continue;
                    }
                    else
                    {
                        bool blink = GetBlinkState();
                        int revColor = GetLedsState() == 0xff ? cRed565 : cBlack565;
                        SendData("RevLimiter", "gear.bco=" + revColor.ToString());
                        SendData("Limiter", "lim.bco=" + (carData.Electronics.HasFlag(CarElectronics.Limiter) && blink ? cOrange565.ToString() : revColor.ToString()));
                        SendData("Limiter text", "lim.pco=" + (carData.Electronics.HasFlag(CarElectronics.Limiter) && blink ? cWhite565.ToString() : revColor.ToString()));
                        SendData("clock", $"dt.txt=\"{datetime}\"");
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                Thread.Sleep(Settings.Enabled ? 1000 / fps : 1000);
            }

            SendData("page1", $"page {settings.IntroPage}");
            ClearCommandsCash();
            bRunning = false;
            Status = DeviceStatus.Disconnected;
        }

        private void ClearCommandsCash()
        {
            cmdMap.Clear();
        }

        /// <summary>
        /// Sent commands map
        /// </summary>
        readonly Dictionary<string, string> cmdMap = new Dictionary<string, string>();

        /// <summary>
        /// Data sending to Nextion Display
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private bool SendData(string cmd, string data, bool bForced = false)
        {
            if (!IsConnected)
                return false;

            // Send data if content changed
            if (bForced || !cmdMap.ContainsKey(cmd) || cmdMap[cmd] != data)
            {
                lock (serialPort)
                {
                    try
                    {
                        serialPort.Write(data);
                        serialPort.Write(new byte[] { 0xFF, 0xFF, 0xFF }, 0, 3);
                    }
                    catch
                    {
                        Status = DeviceStatus.ConnectionError;
                        return false;
                    }
                }
                cmdMap[cmd] = data;
                return true;
            }
            return false;
        }

        const double minTemp = 20;
        const double hotTemp = 80;
        const double maxTemp = 200;
        readonly static Color normColor = Color.Blue;
        readonly static Color hotColor = Color.Green;
        readonly static Color overheatedColor = Color.Red;

        /// <summary>
        /// Returns color for current temperature
        /// </summary>
        /// <param name="temp"></param>
        /// <param name="min"></param>
        /// <param name="hot"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        static short GetTempColor(double temp, double min = minTemp, double hot = hotTemp, double max = maxTemp)
        {
            if (temp < min)
                return ColorTo565(Color.Blue);

            if (temp > max)
                return ColorTo565(Color.Yellow);

            if (temp < hot)
            {
                int r = (int)Math2.Mapd(temp, min, hot, normColor.R, hotColor.R);
                int g = (int)Math2.Mapd(temp, min, hot, normColor.G, hotColor.G);
                int b = (int)Math2.Mapd(temp, min, hot, normColor.B, hotColor.B);
                return ColorTo565(Color.FromArgb(r, g, b));
            }
            else
            {
                int r = (int)Math2.Mapd(temp, hot, max, hotColor.R, overheatedColor.R);
                int g = (int)Math2.Mapd(temp, hot, max, hotColor.G, overheatedColor.G);
                int b = (int)Math2.Mapd(temp, hot, max, hotColor.B, overheatedColor.B);
                return ColorTo565(Color.FromArgb(r, g, b));
            }
        }

        /// <summary>
        /// 16 bit Color conversation to 565
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static short ColorTo565(Color c)
        {
            byte r5 = (byte)Math2.Mapd(c.R, 0, 255, 0, 31);
            byte g6 = (byte)Math2.Mapd(c.G, 0, 255, 0, 63);
            byte b5 = (byte)Math2.Mapd(c.B, 0, 255, 0, 31);

            return (short)((r5 << 11) | (g6 << 5) | b5);
        }

        /// <summary>
        /// Convert Structure to array of Bytes
        /// </summary>
        /// <typeparam name="T">structure type</typeparam>
        /// <param name="obj">structure object</param>
        /// <returns>array of bytes</returns>
        public static byte[] GetBytes<T>(T obj)
        {
            int size = Marshal.SizeOf(obj);

            byte[] arr = new byte[size];

            GCHandle h = default;

            try
            {
                h = GCHandle.Alloc(arr, GCHandleType.Pinned);

                Marshal.StructureToPtr<T>(obj, h.AddrOfPinnedObject(), false);
            }
            finally
            {
                if (h.IsAllocated)
                {
                    h.Free();
                }
            }

            return arr;
        }

        internal TelemetryDataSet m_dataSet = new TelemetryDataSet(null);

        public void OnTelemetry(IVAzhureRacingApp app, TelemetryDataSet data)
        {
            m_dataSet = data;
            _dataArrived = true;
        }

        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct RGB8
    {
        public byte R;
        public byte G;
        public byte B;

        internal string ToHex()
        {
            return $"#{R:X2}{G:X2}{B:X2}";
        }

        internal Color ToColor()
        {
            return Color.FromArgb(R, G, B);
        }

        public static RGB8 FromColor(Color c)
        {
            return new RGB8() { R = c.R, G = c.G, B = c.B };
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct LED_DATA
    {
        public byte cmd;         // всегда 0
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct, SizeConst = 8)]
        public RGB8[] color;     // Массив цветов светодиодов
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct, SizeConst = 1)]
        public RGB8[] limitator; // Цвет диодов лимитатора
        public byte rev_mask;    // Rev. indicator leds mask
        public byte lim_mask;    // Lim. indicator leds mask
        public byte brightness;  // Яркость светодиодов
        public byte flags;       // flags: 0 - none, 0x01 - limitator mode, 0xff - power off
    }

    public enum PresetMode { Auto, Manual };

    public class DeviceSettings : ICloneable
    {
        /// <summary>
        /// Яркость светодиодов [0..255]
        /// </summary>
        public int LedBrightness { get; set; } = 20;
        /// <summary>
        /// Яркость экрана
        /// </summary>
        public int ScreenBrightness { get; set; } = 127;
        /// <summary>
        /// Номер ком-порта
        /// </summary>
        public int ComPort { get; set; } = 0;
        /// <summary>
        /// Скорость соединения
        /// </summary>
        public int BaudRate { get; set; } = 57600;
        /// <summary>
        /// Статус разрешения
        /// </summary>
        public bool Enabled { get; set; } = true;
        /// <summary>
        /// Разрешение на работу светодиодной полосы
        /// </summary>
        public bool EnableLeds { get; set; } = true;
        /// <summary>
        /// Активный профиль светодиодной индикации
        /// </summary>
        public string ActiveProfile { get; set; } = "";
        /// <summary>
        /// Имя отображаемой страницы по умолчанию
        /// </summary>
        public string DefaultPage { get; set; } = "page0";
        public string IntroPage { get; set; } = "intro";
        public string SleepPage { get; set; } = "black";

        public string[] Pages { get; set; } = { "page0", "page1", "page2" };
        /// <summary>
        /// Частота обновления, кадров в секунду
        /// </summary>
        public int Frequency { get; set; } = 25;
        /// <summary>
        /// Режим пресетов
        /// </summary>
        public PresetMode PresetMode = PresetMode.Auto; // Auto
        /// <summary>
        /// Пользовательский пресет
        /// </summary>
        public string UserPreset = "";
        /// <summary>
        /// Единица изменения скорости: 0 - км/ч, 1 - миль/ч
        /// </summary>
        public SpeedUnitsEnum SpeedUnits = SpeedUnitsEnum.kmph;
        /// <summary>
        /// Единица изменения давления: 0 - kPa, 1 - PSI
        /// </summary>
        public PressureUnitsEnum PressureUnits = PressureUnitsEnum.PSI;

        public bool PrimaryDevice = false;

        public enum SpeedUnitsEnum : int { kmph = 0, mph = 1 }
        public enum PressureUnitsEnum : int { kPa = 0, PSI = 1 }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }

    public class CustomLeds
    {
        public string Vechicle { get; set; } = "";
        public string Game { get; set; } = "";
        public List<LedState<RGB8>> RPMMap { get; set; } = new List<LedState<RGB8>>();

        public void SortByRPM()
        {
            RPMMap.Sort((x, y) => x.RPM.CompareTo(y.RPM));
        }

        public LedState<RGB8> GetLedState(int rpm)
        {
            if (RPMMap.Count > 0)
            {
                if (rpm < RPMMap.First().RPM)
                    return new LedState<RGB8>() { RPM = rpm, Blink = false, Leds = new RGB8[8] }; // Black

                if (rpm > RPMMap.Last().RPM)
                    return RPMMap.Last();

                return RPMMap.FindLast(p => p.RPM <= rpm);
            }
            else
                return new LedState<RGB8>() { RPM = rpm, Blink = false, Leds = new RGB8[8] }; // Black
        }

        public static CustomLeds CreateDefaults(string carName, string gameName)
        {
            return new CustomLeds()
            {
                Vechicle = carName,
                Game = gameName,
                RPMMap = new List<LedState<RGB8>>()
                {
                    new LedState<RGB8>() { RPM = 6000, Blink = false, Leds = new RGB8[] {
                            new RGB8 { R = 0, G = 255, B = 0 },
                            new RGB8 { R = 0, G = 255, B = 0 },
                            new RGB8 { R = 0, G = 255, B = 0 },
                            new RGB8 { R = 0, G = 0, B = 0 },
                            new RGB8 { R = 0, G = 0, B = 0 },
                            new RGB8 { R = 0, G = 0, B = 0 },
                            new RGB8 { R = 0, G = 0, B = 0 },
                            new RGB8 { R = 0, G = 0, B = 0 },
                    }},
                    new LedState<RGB8>() { RPM = 7000, Blink = false, Leds = new RGB8[] {
                            new RGB8 { R = 0, G = 255, B = 0 },
                            new RGB8 { R = 0, G = 255, B = 0 },
                            new RGB8 { R = 0, G = 255, B = 0 },
                            new RGB8 { R = 0, G = 255, B = 0 },
                            new RGB8 { R = 0, G = 0, B =  0},
                            new RGB8 { R = 0, G = 0, B = 0 },
                            new RGB8 { R = 0, G = 0, B = 0 },
                            new RGB8 { R = 0, G = 0, B = 0 },
                    }},
                    new LedState<RGB8>() { RPM = 7500, Blink = false, Leds = new RGB8[] {
                            new RGB8 { R = 0, G = 255, B = 0 },
                            new RGB8 { R = 0, G = 255, B = 0 },
                            new RGB8 { R = 0, G = 255, B = 0 },
                            new RGB8 { R = 0, G = 255, B = 0 },
                            new RGB8 { R = 0, G = 0, B = 255},
                            new RGB8 { R = 0, G = 0, B = 255 },
                            new RGB8 { R = 0, G = 0, B = 0 },
                            new RGB8 { R = 0, G = 0, B = 0 },
                    }},
                    new LedState<RGB8>() { RPM = 8000, Blink = false, Leds = new RGB8[] {
                            new RGB8 { R = 0, G = 255, B = 0 },
                            new RGB8 { R = 0, G = 255, B = 0 },
                            new RGB8 { R = 0, G = 255, B = 0 },
                            new RGB8 { R = 0, G = 255, B = 0 },
                            new RGB8 { R = 0, G = 0, B = 255},
                            new RGB8 { R = 0, G = 0, B = 255 },
                            new RGB8 { R = 255, G = 0, B = 0 },
                            new RGB8 { R = 255, G = 0, B = 0 },
                    }},
                }
            };
        }

        public void SaveToFile(string filename)
        {
            try
            {
                string folder = Path.GetDirectoryName(filename);

                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                File.WriteAllText(filename, ToString());
            }
            catch { }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(Game);
            sb.AppendLine(Vechicle);
            foreach (LedState<RGB8> ls in RPMMap)
                sb.AppendLine($"{ls.RPM},{ls.Leds[0].ToHex()},{ls.Leds[1].ToHex()},{ls.Leds[2].ToHex()},{ls.Leds[3].ToHex()},{ls.Leds[4].ToHex()},{ls.Leds[5].ToHex()},{ls.Leds[6].ToHex()},{ls.Leds[7].ToHex()},{ls.Blink}");

            return sb.ToString();
        }

        public bool LoadFromFile(string filename)
        {
            try
            {
                string[] lines = File.ReadAllLines(filename);

                Game = lines[0]; // Game name in the first line of text file
                Vechicle = lines[1]; // Car name in the second line of text file

                for (int t = 2; t < lines.Count(); t++)
                {
                    if (LedState<RGB8>.FromString(lines[t], out LedState<RGB8> ls))
                        RPMMap.Add(ls);
                    else
                        return false;
                }

                SortByRPM();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public CustomLeds Clone()
        {
            return this.MemberwiseClone() as CustomLeds;
        }
    }

    public static class LedsExt
    {
        public static void Copy(this CustomLeds leds, CustomLeds obj)
        {
            leds.Vechicle = obj.Vechicle;
            leds.Game = obj.Game;
            leds.RPMMap = obj.RPMMap.ToList();
        }
    }

    public class LedState<T>
    {
        public int RPM { get; set; }
        public T[] Leds { get; set; } = new T[8];
        public bool Blink { get; set; } = false;

        public bool IsSame(LedState<T> ls)
        {
            return Blink == ls.Blink && RPM == ls.RPM && Leds.SequenceEqual(ls.Leds);
        }

        public static bool FromString(string data, out LedState<T> ls)
        {
            ls = new LedState<T>();

            string[] arr = data.Split(", ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            if (arr.Length == 10) // RPM, LED1, LED2, LED3, LED4, LED5, LED6, LED7, LED8, BLINK
            {
                if (!int.TryParse(arr[0], out int rpm))
                    return false;

                ls.RPM = rpm;
                Color c;

                for (int t = 1; t < 9; t++)
                {
                    ref string val = ref arr[t];

                    try
                    {
                        c = ColorTranslator.FromHtml(val);

                        if (ls.Leds is RGB8[] rgb)
                        {
                            rgb[t - 1].R = c.R;
                            rgb[t - 1].G = c.G;
                            rgb[t - 1].B = c.B;
                        }
                        else
                            if (ls.Leds is RGBW8[] rgbw)
                        {
                            rgbw[t - 1].R = c.R;
                            rgbw[t - 1].G = c.G;
                            rgbw[t - 1].B = c.B;
                        }
                    }
                    catch
                    {
                        return false;
                    }
                }

                bool.TryParse(arr.LastOrDefault(), out bool blink);
                ls.Blink = blink;
            }

            return true;
        }

        public LedState<T> Clone()
        {
            return MemberwiseClone() as LedState<T>;
        }
    }
}