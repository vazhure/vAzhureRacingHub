using System;
using System.IO;
using System.Xml;
using vAzhureRacingAPI;

// TODO: Пропатчить файл hardware_settings_config.xml в моих документах в диалоге настроек игры

namespace Codemasters
{
    public class GamePlugin : IGamePlugin, IDisposable
    {
        readonly CodemastersGame _game;
        readonly UDPClient _client;
        readonly ProcessMonitor monitor;
        public int Port { get; set; } = 20777;

        public string Name
        {
            get
            {
                switch (_game)
                {
                    case CodemastersGame.DIRT4: return "DiRT 4";
                    case CodemastersGame.DIRTRALLY: return "DiRT Rally";
                    case CodemastersGame.DIRTRALLY20: return "DiRT Rally 2.0";
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
                    case CodemastersGame.DIRTRALLY: return 310560U;
                    case CodemastersGame.DIRTRALLY20: return 690790U;
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
                    case CodemastersGame.DIRTRALLY: return new string[] { "drt" };
                    case CodemastersGame.DIRTRALLY20: return new string[] { "dirtrally2" };
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

        public enum CodemastersGame { DIRT4, DIRTRALLY, DIRTRALLY20 };
        public GamePlugin(CodemastersGame game)
        {
            _game = game;
            _client = new UDPClient(this);
            _client.OnDataArrived += OnDataArrived;

            monitor = new ProcessMonitor(ExecutableProcessName, 500);
            monitor.OnProcessRunningStateChanged += delegate (object o, bool bRunning)
            {
                bIsRunning = bRunning;
                if (bRunning)
                    _client?.Run(Port);
                else
                    _client?.Stop();
                OnGameStateChanged?.Invoke(this, new EventArgs());
            };
            monitor.Start();
        }

        private void OnDataArrived(object sender, TelemetryUpdatedEventArgs e)
        {
            OnTelemetry?.Invoke(this, e);
        }

        private bool bIsRunning = false;

        public void Start(IVAzhureRacingApp app)
        {
            PatchXML();
            if (!Utils.RunSteamGame(SteamGameID))
            {
                app.SetStatusText($"Ошибка запуска игры {Name}!");
            }
        }

        public void ShowSettings(IVAzhureRacingApp app)
        {
            // TODO
        }

        public System.Drawing.Icon GetIcon()
        {
            switch (_game)
            {
                default:
                case CodemastersGame.DIRT4: return Properties.Resources.dirt4;
                case CodemastersGame.DIRTRALLY: return Properties.Resources.drt_rally;
                case CodemastersGame.DIRTRALLY20: return Properties.Resources.dirtrally2;
            }
        }

        void PatchXML()
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
                        xmlDoc.Save(config_file_name);
                    }
                    catch
                    {
                    }
                }
            }
        }

        public void Dispose()
        {
            try
            {
                monitor?.Stop();
                _client?.Stop();
            }
            catch { }
        }
    }
}