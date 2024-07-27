using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using vAzhureRacingAPI;

namespace vAzhureRacingHub
{
    public class WebServer : IDisposable
    {
        public static string sStrServerPath = Path.Combine(AppWindow.LocalApplicationData, "server");
        int m_nPort = 8080;

        private volatile bool IsWorking = false;
        private Task serverTask = null;
        private Task updateTask = null;
        private static readonly List<WSClient> clients = new List<WSClient>();
        private Socket listeningSocket = null;

        private CancellationTokenSource tokenSource;
        private CancellationToken token;

        static readonly string[] gears = { "R", "N", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14" };
        const string purple = "#bc00bc";
        const string green = " #00bc00";
        const string red = "#c10c0c";
        const string transparent = "transparent";

        public event EventHandler<WSClient> OnClientConnected;
        public event EventHandler<WSClient> OnClientDisconnected;
        public event EventHandler OnServerStarted;
        public event EventHandler OnServerStopped;

        private const string cSettingsFileName = "webserver.json";

        public WebServer()
        {
            LoadSettings(Path.Combine(AppWindow.LocalApplicationData, cSettingsFileName));
        }

        public bool Enabled { get; set; } = false;

        private void LoadSettings(string path)
        {
            if (File.Exists(path))
            {
                try
                {
                    string json = File.ReadAllText(path);
                    WebServerSettings settings = ObjectSerializeHelper.DeserializeJson<WebServerSettings>(json);
                    sStrServerPath = settings.ServerPath;
                    m_nPort = settings.Port;
                    Enabled = settings.Enabled;
                }
                catch { }
            }
        }

        private void SaveSettings(string path)
        {
            try
            {
                WebServerSettings settings = new WebServerSettings() { Enabled = Enabled, Port = m_nPort, ServerPath = sStrServerPath };
                File.WriteAllText(path, settings.GetJson());
            }
            catch { }
        }

        public string[] Clients
        {
            get
            {
                lock (clients)
                {
                    try
                    {
                        return clients.Count == 0 ? new string[] { } : clients.Select(x => x.IP).ToArray();
                    }
                    catch
                    {
                        return new string[] { };
                    }
                }
            }
        }

        public string ServerPath
        {
            get
            {
                return sStrServerPath;
            }

            set
            {
                if (sStrServerPath != value)
                {
                    sStrServerPath = value;

                    if (IsRunning)
                    {
                        Stop();
                        Start();
                    }
                }
            }
        }

        internal void StoreSettings()
        {
            SaveSettings(Path.Combine(AppWindow.LocalApplicationData, cSettingsFileName));
        }

        public int Port
        {
            get
            {
                return m_nPort;
            }

            set
            {
                if (m_nPort != value)
                {
                    m_nPort = value;

                    if (IsRunning)
                    {
                        Stop();
                        Start();
                    }
                }
            }
        }

        public TelemetryDataSet DataSet { get; private set; } = new TelemetryDataSet(null);

        public void OnTelemetry(IVAzhureRacingApp _, TelemetryDataSet data)
        {
            DataSet = data;
        }

        public bool IsRunning
        {
            get
            {
                return updateTask != null && serverTask != null;
            }
        }

        public bool Start()
        {
            tokenSource = new CancellationTokenSource();
            token = tokenSource.Token;

            serverTask = new Task(ServerThread, this, token);
            updateTask = new Task(UpdateThread, this, token);
            try
            {
                serverTask.Start();
                updateTask.Start();
            }
            catch
            {
                return false;
            }

            OnServerStarted?.Invoke(this, new EventArgs());

            return true;
        }

        public void Stop()
        {
            listeningSocket?.Close();
            IsWorking = false;
            tokenSource?.Cancel();
            serverTask?.Wait(1000);
            serverTask = null;
            updateTask?.Wait(1000);
            updateTask = null;
            listeningSocket?.Dispose();
            listeningSocket = null;
        }

        public void NotifyClientConnected(WSClient ws)
        {
            OnClientConnected?.Invoke(this, ws);
        }

        public void NotifyClientDisconnected(WSClient ws)
        {
            OnClientDisconnected?.Invoke(this, ws);
        }

        public void NotifyServerStopped()
        {
            OnServerStopped?.Invoke(this, new EventArgs());
            IsWorking = false;
        }

        public void Dispose()
        {
            StoreSettings();
        }

        readonly Action<object> UpdateThread = (object obj) =>
        {
            if (obj is WebServer wsp)
            {
                wsp.IsWorking = true;
                while (wsp.IsWorking)
                {
                    lock (clients)
                    {
                        foreach (WSClient client in clients.ToArray())
                        {
                            if (!client.Ping())
                            {
                                clients.Remove(client);
                                wsp?.NotifyClientDisconnected(client);
                                client.Dispose();
                            }
                        }

                        if (clients.Count == 0)
                        {
                            Thread.Sleep(100);
                            continue;
                        }
                    }

                    if (wsp.DataSet == null || wsp.DataSet.GamePlugin == null)
                    {
                        Thread.Sleep(100);
                        continue;
                    }

                    try
                    {
                        WSPacket wSPacket = new WSPacket
                        {
                            SessionType = wsp.DataSet.SessionInfo.SessionState ?? "",
                            Lap = wsp.DataSet.CarData.Lap.ToString(),
                            Position = wsp.DataSet.CarData.Position.ToString(),
                            DriverName = wsp.DataSet.CarData.DriverName ?? "",
                            CarName = wsp.DataSet.CarData.CarName ?? "",
                            CarNumber = wsp.DataSet.CarData.CarNumber ?? "",
                            TrackName = wsp.DataSet.SessionInfo.TrackName ?? "",
                            Speed = $"{wsp.DataSet.CarData.Speed:000}",
                            RPM = $"{wsp.DataSet.CarData.RPM:00000}",
                            Gear = wsp.DataSet.CarData.Gear < gears.Length ? gears[wsp.DataSet.CarData.Gear] : "",
                            AirTemp = FormattableString.Invariant($"{wsp.DataSet.WeatherData.AmbientTemp:N1}&degC"),
                            RoadTemp = FormattableString.Invariant($"{wsp.DataSet.WeatherData.TrackTemp:N1}&degC"),
                            OilTemp = FormattableString.Invariant($"{wsp.DataSet.CarData.OilTemp:N1}&degC"),
                            WaterTemp = FormattableString.Invariant($"{wsp.DataSet.CarData.WaterTemp:N1}&degC"),
                            Throttle = $"{(int)(wsp.DataSet.CarData.Throttle * 100)}%",
                            Brake = $"{(int)(wsp.DataSet.CarData.Brake * 100)}%",
                            Clutch = $"{(int)(wsp.DataSet.CarData.Clutch * 100)}%",
                            TC = $"{wsp.DataSet.CarData.TcLevel}",
                            TC2 = $"{wsp.DataSet.CarData.TcLevel2}",
                            ABS = $"{wsp.DataSet.CarData.AbsLevel}",
                            MAP = $"{wsp.DataSet.CarData.EngineMap}",
                            FuelLevel = FormattableString.Invariant($"{wsp.DataSet.CarData.FuelLevel:N2}l"),
                            Sector = $"{wsp.DataSet.SessionInfo.Sector}",
                            FinishStatus = $"{wsp.DataSet.SessionInfo.FinishStatus}",
                        };

                        if (wsp.DataSet.SessionInfo.RemainingTime > 0)
                        {
                            TimeSpan ts = TimeSpan.FromMilliseconds(wsp.DataSet.SessionInfo.RemainingTime);
                            wSPacket.Remain = ts.Hours > 0 ? $"{ts.Hours}h {ts.Minutes:00}m {ts.Seconds:00}s" : $"{ts.Minutes:00}m {ts.Seconds:00}s";
                        }
                        else
                        {
                            if (wsp.DataSet.SessionInfo.RemainingLaps > 0)
                                wSPacket.Remain = wsp.DataSet.SessionInfo.RemainingLaps == 1? "Last Lap" : $"{wsp.DataSet.SessionInfo.RemainingLaps} laps";
                            else
                                wSPacket.Remain = "";
                        }

                        if (wsp.DataSet.SessionInfo.CurrentLapTime > 0)
                        {
                            TimeSpan ts = TimeSpan.FromMilliseconds(wsp.DataSet.SessionInfo.CurrentLapTime);
                            wSPacket.LapTime = $"{ts.Minutes}:{ts.Seconds:00}.{ts.Milliseconds:000}";
                        }
                        else
                            wSPacket.LapTime = "";

                        if (wsp.DataSet.SessionInfo.LastLapTime > 0)
                        {
                            TimeSpan ts = TimeSpan.FromMilliseconds(wsp.DataSet.SessionInfo.LastLapTime);
                            wSPacket.LastLapTime = $"{ts.Minutes}:{ts.Seconds:00}.{ts.Milliseconds:000}";
                        }
                        else
                            wSPacket.LastLapTime = "";

                        if (wsp.DataSet.SessionInfo.BestLapTime > 0)
                        {
                            TimeSpan ts = TimeSpan.FromMilliseconds(wsp.DataSet.SessionInfo.BestLapTime);
                            wSPacket.BestLapTime = ts.Minutes > 0 ? $"{ts.Minutes}:{ts.Seconds:00}.{ts.Milliseconds:000}" : $"{ts.Seconds:00}.{ts.Milliseconds:000}";
                        }
                        else
                            wSPacket.BestLapTime = "";

                        if (wsp.DataSet.SessionInfo.CurrentSector1Time > 0)
                        {
                            TimeSpan ts = TimeSpan.FromMilliseconds(wsp.DataSet.SessionInfo.CurrentSector1Time);
                            wSPacket.Sector1Time = $"{ts.Seconds:00}.{ts.Milliseconds:000}";


                            if (wSPacket.Sector == "1")
                                wSPacket.Sector1Color = transparent;
                            else
                            {
                                if (wsp.DataSet.SessionInfo.CurrentSector1Time < wsp.DataSet.SessionInfo.Sector1BestTime && wsp.DataSet.SessionInfo.Sector1BestTime > 0)
                                    wSPacket.Sector1Color = green;
                                else
                                    wSPacket.Sector1Color = transparent;
                            }
                        }
                        else
                        {
                            wSPacket.Sector1Time = "";
                            wSPacket.Sector1Color = transparent;
                        }

                        if (wsp.DataSet.SessionInfo.CurrentSector2Time > 0)
                        {
                            TimeSpan ts = TimeSpan.FromMilliseconds(wsp.DataSet.SessionInfo.CurrentSector2Time);
                            wSPacket.Sector2Time = ts.Minutes > 0 ? $"{ts.Minutes}:{ts.Seconds:00}.{ts.Milliseconds:000}" : $"{ts.Seconds:00}.{ts.Milliseconds:000}";


                            if (wSPacket.Sector == "2")
                                wSPacket.Sector2Color = transparent;
                            else
                            {
                                if (wsp.DataSet.SessionInfo.CurrentSector2Time < wsp.DataSet.SessionInfo.Sector2BestTime && wsp.DataSet.SessionInfo.Sector2BestTime > 0)
                                    wSPacket.Sector2Color = green;
                                else
                                    wSPacket.Sector2Color = transparent;
                            }
                        }
                        else
                        {
                            wSPacket.Sector2Time = "";
                            wSPacket.Sector2Color = transparent;
                        }

                        if (wsp.DataSet.SessionInfo.CurrentSector3Time > 0)
                        {
                            TimeSpan ts = TimeSpan.FromMilliseconds(wsp.DataSet.SessionInfo.CurrentSector3Time);
                            wSPacket.Sector3Time = ts.Minutes > 0 ? $"{ts.Minutes}:{ts.Seconds:00}.{ts.Milliseconds:000}" : $"{ts.Seconds:00}.{ts.Milliseconds:000}";

                            if (wSPacket.Sector == "3")
                                wSPacket.Sector3Color = transparent;
                            else
                            {
                                if (wsp.DataSet.SessionInfo.CurrentSector3Time < wsp.DataSet.SessionInfo.Sector3BestTime && wsp.DataSet.SessionInfo.Sector1BestTime > 0)
                                    wSPacket.Sector3Color = green;
                                else
                                    wSPacket.Sector3Color = transparent;
                            }
                        }
                        else
                        {
                            wSPacket.Sector3Time = "";
                            wSPacket.Sector3Color = transparent;
                        }

                        if (Math.Abs(wsp.DataSet.SessionInfo.CurrentDelta) > 10 && Math.Abs(wsp.DataSet.SessionInfo.CurrentDelta) < 60000)
                        {
                            string sign = wsp.DataSet.SessionInfo.CurrentDelta < 0 ? "-" : "";
                            TimeSpan ts = TimeSpan.FromMilliseconds(Math.Abs(wsp.DataSet.SessionInfo.CurrentDelta));
                            wSPacket.Delta = $"{sign}{ts.Seconds:00}.{ts.Milliseconds:000}";
                            wSPacket.DeltaColor = sign == "" ? red : green;
                        }
                        else
                        {
                            wSPacket.Delta = "";
                            wSPacket.DeltaColor = transparent;
                        }

                        float t = wsp.DataSet.CarData.MaxRPM > 0 ? (float)wsp.DataSet.CarData.RPM / (float)wsp.DataSet.CarData.MaxRPM : 0;
                        t = (t - 0.5f) * 9.0f / 0.5f;
                        wSPacket.GearColor = t < 6 ? "white" : t > 8 ? "red" : "orange";

                        switch (wsp.DataSet.CarData.Flags)
                        {
                            default:
                            case TelemetryFlags.FlagNone:
                                wSPacket.Flag = "";
                                break;
                            case TelemetryFlags.FlagBlack:
                                wSPacket.Flag = "Black";
                                break;
                            case TelemetryFlags.FlagBlue:
                                wSPacket.Flag = "Blue";
                                break;
                            case TelemetryFlags.FlagYellow:
                                wSPacket.Flag = "Yellow";
                                break;
                            case TelemetryFlags.FlagGreen:
                                wSPacket.Flag = "Green";
                                break;
                            case TelemetryFlags.FlagChequered:
                                wSPacket.Flag = "Chequered";
                                break;
                            case TelemetryFlags.FlagWhite:
                                wSPacket.Flag = "White";
                                break;
                        }

                        lock (clients)
                        {
                            foreach (WSClient client in clients.ToArray())
                            {
                                try
                                {
                                    client.SendMessage(wSPacket.ToString());
                                }
                                catch
                                {
                                }
                            }
                        }
                    }
                    catch { }
                    Thread.Sleep(30);
                }
            }
        };

        readonly Action<object> ServerThread = (object obj) =>
        {
            if (obj is WebServer wsp)
            {
                wsp.listeningSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                wsp.listeningSocket.Bind(new IPEndPoint(IPAddress.Any, port: wsp.Port));
                wsp.listeningSocket.Listen(10);

                wsp.IsWorking = true;
                while (wsp.IsWorking)
                {
                    try
                    {
                        var clientSocket = wsp.listeningSocket.Accept();

                        WSClient ws = new WSClient(clientSocket);

                        if (ws.Handshake())
                        {
                            lock (clients)
                            {
                                clients.Add(ws);
                            }
                            wsp.NotifyClientConnected(ws);
                            ws.Disconnected += delegate (object sender, EventArgs e)
                            {
                                if (sender is WSClient client)
                                {
                                    lock (clients)
                                    {
                                        clients.Remove(client);
                                    }
                                }
                            };
                        }
                        else
                        {
                            ws.Dispose();
                        }
                    }
                    catch { }
                }
                wsp.NotifyServerStopped();
            }
        };
    }

    public class WebServerSettings
    {
        public bool Enabled { get; set; }
        public string ServerPath { get; set; } = "";
        public int Port { get; set; } = 8080;
    }

    public class WSPacket
    {
        public string SessionType { get; set; } = "";
        public string Flag { get; set; } = "";
        public string DriverName { get; set; } = "";
        public string CarName { get; set; } = "";
        public string CarNumber { get; set; } = "#1";
        public string TrackName { get; set; } = "";
        public string Position { get; set; } = "";
        public string Lap { get; set; } = "";
        public string LapTime { get; set; } = "";
        public string Delta { get; set; } = "";
        public string DeltaColor { get; set; } = "Transparent";
        public string Remain { get; set; } = "";
        public string LastLapTime { get; set; } = "";
        public string BestLapTime { get; set; } = "";
        public string Sector1Time { get; set; } = "";
        public string Sector2Time { get; set; } = "";
        public string Sector3Time { get; set; } = "";
        public string Sector1Color { get; set; } = "Transparent";
        public string Sector2Color { get; set; } = "Transparent";
        public string Sector3Color { get; set; } = "Transparent";
        public string AirTemp { get; set; } = "21&degC";
        public string OilTemp { get; set; } = "100&degC";
        public string WaterTemp { get; set; } = "100&degC";
        public string RoadTemp { get; set; } = "29&degC";
        public string Gear { get; set; } = "N";
        public string Speed { get; set; } = "000";
        public string RPM { get; set; } = "00000";
        public string Clutch { get; set; } = "0%";
        public string Brake { get; set; } = "0%";
        public string Throttle { get; set; } = "0%";
        public string FuelLevel { get; set; } = "0.0l";
        public string FuelPerLap { get; set; } = "";
        public string TC { get; set; } = "";
        public string TC2 { get; set; } = "";
        public string ABS { get; set; } = "";
        public string MAP { get; set; } = "";
        public string Sector { get; set; } = "";
        public string GearColor { get; set; } = "";

        public string FinishStatus = "";

        public override string ToString()
        {
            return this.GetJson();
        }
    }
}