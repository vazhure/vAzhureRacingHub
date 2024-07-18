using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using vAzhureRacingAPI;

namespace Codemasters
{
    public class GamePlugin : IGamePlugin, IDisposable
    {
        readonly CodemastersGame _game;
        readonly UDPClient _client;
        readonly ProcessMonitor monitor;
        public int Port { get => settings.Port; set => settings.Port = value; } 

        public class GameSettings
        {
            public int Port { get; set; } = 20777;
        }

        GameSettings settings = new GameSettings();

        public string Name
        {
            get
            {
                switch (_game)
                {
                    case CodemastersGame.DIRT4: return "DiRT 4";
                    case CodemastersGame.DIRT5: return "DiRT 5";
                    case CodemastersGame.DIRTRALLY: return "DiRT Rally";
                    case CodemastersGame.DIRTRALLY20: return "DiRT Rally 2.0";
                    case CodemastersGame.WRCG: return "WRC Generations";
                    case CodemastersGame.EAWRC: return "EA SPORTS WRC";
                    case CodemastersGame.GRID2019: return "Grid (2019)";
                }
                return "error";
            }
        }

        public uint SteamGameID
        {
            get
            {
                switch (_game)
                {
                    case CodemastersGame.DIRT4: return 421020U;
                    case CodemastersGame.DIRT5: return 1038250U;
                    case CodemastersGame.DIRTRALLY: return 310560U;
                    case CodemastersGame.DIRTRALLY20: return 690790U;
                    case CodemastersGame.WRCG: return 1953520U;
                    case CodemastersGame.EAWRC: return 1849250U;
                    case CodemastersGame.GRID2019: return 703860U;
                }
                return 0U;
            }
        }

        public string[] ExecutableProcessName
        {
            get
            {
                switch (_game)
                {
                    case CodemastersGame.DIRT4: return new string[] { "dirt4" };
                    case CodemastersGame.DIRT5: return new string[] { "dirt5" };
                    case CodemastersGame.DIRTRALLY: return new string[] { "drt" };
                    case CodemastersGame.DIRTRALLY20: return new string[] { "dirtrally2" };
                    case CodemastersGame.WRCG: return new string[] { "WRCG" };
                    case CodemastersGame.EAWRC: return new string[] { "wrc" };
                    case CodemastersGame.GRID2019: return new string[] { "Grid_dx12" };
                }
                return new string[] { };
            }
        }

        string userIconPath;
        public string UserIconPath { get => userIconPath; set => userIconPath = value; }
        string userExecutablePath;
        public string UserExecutablePath { get => userExecutablePath; set => userExecutablePath = value; }

        public bool IsRunning => Utils.IsProcessRunning(ExecutableProcessName);

        public event EventHandler<TelemetryUpdatedEventArgs> OnTelemetry;
        public event EventHandler OnGameStateChanged;
        public event EventHandler OnGameIconChanged;

        public enum CodemastersGame { DIRT4, DIRT5, DIRTRALLY, DIRTRALLY20, WRCG, EAWRC, GRID2019 };
        public GamePlugin(CodemastersGame game)
        {
            if (game == CodemastersGame.WRCG)
                Port = 20888;

            LoadSettings();

            _game = game;
            _client = new UDPClient(this, game);
            _client.OnDataArrived += OnDataArrived;

            monitor = new ProcessMonitor(ExecutableProcessName, 500);
            monitor.OnProcessRunningStateChanged += delegate (object o, bool bRunning)
            {
                if (bRunning)
                    _client?.Run(Port);
                else
                    _client?.Stop();
                OnGameStateChanged?.Invoke(this, new EventArgs());
            };
            monitor.Start();
        }

        private void LoadSettings()
        {
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetCallingAssembly().Location), $"{Name}.json");
            if (File.Exists(path))
            {
                try
                {
                    string json = File.ReadAllText(path);

                    settings = ObjectSerializeHelper.DeserializeJson<GameSettings>(json);
                }
                catch { }
            }
        }

        private void OnDataArrived(object sender, TelemetryUpdatedEventArgs e)
        {
            OnTelemetry?.Invoke(this, e);
        }

        public void Start(IVAzhureRacingApp app)
        {
            if (!Utils.RunSteamGame(SteamGameID))
            {
                app.SetStatusText($"Ошибка запуска игры {Name}!");
            }
        }

        public void ShowSettings(IVAzhureRacingApp app)
        {
            PatchXML(true);
        }

        public System.Drawing.Icon GetIcon()
        {
            switch (_game)
            {
                default:
                case CodemastersGame.DIRT4: return Properties.Resources.dirt4;
                case CodemastersGame.DIRT5: return Properties.Resources.dirt5;
                case CodemastersGame.DIRTRALLY: return Properties.Resources.drt_rally;
                case CodemastersGame.DIRTRALLY20: return Properties.Resources.dirtrally2;
                case CodemastersGame.WRCG: return Properties.Resources.WRCG;
                case CodemastersGame.EAWRC: return Properties.Resources.eawrc;
                case CodemastersGame.GRID2019: return Properties.Resources.Grid;
            }
        }

        void PatchXML(bool bAskToPatch)
        {
            if (_game == CodemastersGame.WRCG)
            {
                StringBuilder sb = new StringBuilder();

                bool bPatched = false;
                bool[] bLineExist = { false, false, false, false };

                string config_file_name = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "My Games", "WRCG", "UserSettings.cfg");

                if (File.Exists(config_file_name))
                {
                    string[] cfg = File.ReadAllLines(config_file_name);

                    foreach (string line in cfg)
                    {
                        if (line.StartsWith("WRC.Telemetry.TelemetryRate") && line != "WRC.Telemetry.TelemetryRate = 60;")
                        {
                            sb.AppendLine("WRC.Telemetry.TelemetryRate = 60;");
                            bPatched = bLineExist[0] = true;
                        }
                        if (line.StartsWith("WRC.Telemetry.TelemetryPort") && line != $"WRC.Telemetry.TelemetryPort = {Port};")
                        {
                            sb.AppendLine($"WRC.Telemetry.TelemetryPort = {Port};");
                            bPatched = bLineExist[1] = true;
                        }
                        if (line.StartsWith("WRC.Telemetry.EnableTelemetry") && line != "WRC.Telemetry.EnableTelemetry = true;")
                        {
                            sb.AppendLine("WRC.Telemetry.EnableTelemetry = true;");
                            bPatched = bLineExist[2] = true;
                        }
                        if (line.StartsWith("WRC.Telemetry.TelemetryAdress") && line != "WRC.Telemetry.TelemetryAdress = \"127.0.1.1\"")
                        {
                            sb.AppendLine("WRC.Telemetry.TelemetryAdress = \"127.0.1.1\"");
                            bPatched = bLineExist[3] = true;
                        }
                    }
                }

                for (int t = 0; t < bLineExist.Length; t++)
                {
                    if (bLineExist[t] == false)
                    {
                        switch (t)
                        {
                            case 0: sb.AppendLine("WRC.Telemetry.TelemetryRate = 60;"); bPatched = true; break;
                            case 1: sb.AppendLine($"WRC.Telemetry.TelemetryPort = {Port};"); bPatched = true; break;
                            case 2: sb.AppendLine("WRC.Telemetry.EnableTelemetry = true;"); bPatched = true; break;
                            case 3: sb.AppendLine("WRC.Telemetry.TelemetryAdress = \"127.0.1.1\""); bPatched = true; break;
                        }
                    }
                }

                if (bPatched)
                {
                    try
                    {
                        if (bAskToPatch == false || MessageBox.Show("Config file not patched! Patch now?", Name, MessageBoxButtons.YesNo) == DialogResult.Yes)
                            File.WriteAllText(config_file_name, sb.ToString());
                    }
                    catch { }
                }
            }
            else
            {
                string config_file_name = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "My Games", Name, "hardwaresettings", "hardware_settings_config.xml");

                if (File.Exists(config_file_name))
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    string xml = File.ReadAllText(config_file_name);
                    bool bChanged = false;
                    using (StringReader sr = new StringReader(xml))
                    {
                        xmlDoc.Load(sr);
                        if (xmlDoc.SelectNodes("//motion_platform//udp") is XmlNodeList nodes)
                        {
                            if (nodes.Count == 1)
                            {
                                XmlNode xn = nodes.Item(0);

                                if (xn.Attributes.GetNamedItem("enabled") is XmlAttribute enabled)
                                {
                                    if (enabled.Value != "true")
                                    {
                                        enabled.Value = "true";
                                        bChanged = true;
                                    }
                                }

                                if (xn.Attributes.GetNamedItem("port") is XmlAttribute port)
                                {
                                    if (port.Value != $"{Port}")
                                    {
                                        port.Value = $"{Port}";
                                        bChanged = true;
                                    }
                                }

                                if (xn.Attributes.GetNamedItem("extradata") is XmlAttribute extradata)
                                {
                                    if (extradata.Value != "3")
                                    {
                                        extradata.Value = "3";
                                        bChanged = true;
                                    }
                                }
                            }
                        }
                    }

                    if (bChanged)
                    {
                        try
                        {
                            if (bAskToPatch == false || MessageBox.Show("Config file not patched! Patch now?", Name, MessageBoxButtons.YesNo) == DialogResult.Yes)
                                xmlDoc.Save(config_file_name);
                        }
                        catch (Exception e)
                        {
                            MessageBox.Show(e.Message);
                        }
                    }
                }
            }
        }

        public void Dispose()
        {
            SaveSettings();

            try
            {
                monitor?.Stop();
                _client?.Finish();
                _client?.Stop();
            }
            catch { }
        }

        private void SaveSettings()
        {
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetCallingAssembly().Location), $"{Name}.json");
            string json = "";
            if (File.Exists(path))
            {
                try
                {
                    json = File.ReadAllText(path);
                }
                catch { }
            }
            string jsonNew = settings.GetJson();
            if (json != jsonNew)
            {
                try
                {
                    File.WriteAllText(path, jsonNew);
                }
                catch { }
            }
        }
    }
}