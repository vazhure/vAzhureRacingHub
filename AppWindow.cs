/// 
/// Разработчик:
/// Журавлев Андрей Владимирович
/// e-mail: avjuravlev@yandex.ru
/// Developer:
/// Andrey Zhuravlev
/// e-mail: avjuravlev@yandex.com
/// 

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using vAzhureRacingAPI;

namespace vAzhureRacingHub
{
    /// <summary>
    /// Главное окно приложения
    /// </summary>
    public partial class AppWindow : MovableForm, IVAzhureRacingApp
    {
        public static readonly string LocalApplicationData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "vAzhureRacingHub");
        public static readonly string localSettingFile = Path.Combine(LocalApplicationData, "settings.json");
        public static readonly string localSettingPluginsPath = Path.Combine(LocalApplicationData, "Plugins");

        /// <summary>
        /// Конструктор класса
        /// </summary>
        public AppWindow()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.Text = ProductName;
            OnSoundLevelChanged += AppWindow_OnSoundLevelChanged;
        }

        private void LoadSettings()
        {
            settings.PluginsFolder = localSettingPluginsPath;

            if (Directory.Exists(LocalApplicationData))
            {
                if (AppSettings.LoadFromFile(localSettingFile) is AppSettings s)
                {
                    settings = s;
                }
            }
            else
            {
                try
                {
                    Directory.CreateDirectory(LocalApplicationData);
                    Directory.CreateDirectory(localSettingPluginsPath);
                }
                catch { }
            }
        }

        private const int WM_DEVICECHANGE = 0x0219;                 // device change event 
        private const int DBT_DEVICEARRIVAL = 0x8000;               // system detected a new device 
        private const int DBT_DEVICEREMOVEPENDING = 0x8003;         // about to remove, still available 
        private const int DBT_DEVICEREMOVECOMPLETE = 0x8004;
        private const int DBT_DEVTYP_PORT = 0x00000003;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        struct DEV_BROADCAST_PORT
        {
            public int dbcp_size;
            public int dbcp_devicetype;
            public int dbcp_reserved; // MSDN say "do not use"
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 255)]
            public string dbcp_name;
        }

        /// <summary>
        /// Сообщения Windows
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_DEVICECHANGE && m.LParam != IntPtr.Zero)
            {
                DEV_BROADCAST_PORT hdr_port = new DEV_BROADCAST_PORT() { dbcp_name = "" };
                try
                {
                    hdr_port = (DEV_BROADCAST_PORT)Marshal.PtrToStructure(m.LParam, typeof(DEV_BROADCAST_PORT));

                    // Отслеживаем подключения по портам
                    if (hdr_port.dbcp_devicetype == DBT_DEVTYP_PORT)
                    {
                        string port = hdr_port.dbcp_name;
                        int EventCode = m.WParam.ToInt32();

                        switch (EventCode)
                        {
                            case DBT_DEVICEARRIVAL:
                                OnDeviceArrival?.Invoke(this, new DeviceChangeEventsArgs(port));
                                BeginInvoke((Action)delegate { lblStatus.Text = $"Device connected to Port {port}"; });
                                break;
                            case DBT_DEVICEREMOVEPENDING:
                                OnDeviceRemovePending?.Invoke(this, new DeviceChangeEventsArgs(port));
                                BeginInvoke((Action)delegate { lblStatus.Text = $"Device disconnecting from Port {port}"; });
                                break;
                            case DBT_DEVICEREMOVECOMPLETE:
                                OnDeviceRemoveComplete?.Invoke(this, new DeviceChangeEventsArgs(port));
                                BeginInvoke((Action)delegate { lblStatus.Text = $"Device disconnected from Port {port}"; });
                                break;
                        }
                    }
                }
                catch
                {
                }
            }

            base.WndProc(ref m);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            WindowsFormHelper.LoadState(this);
            LoadSettings();
            InitializeServer();
            LoadPlugins();
            volumeControl.Value = SoundLevel;
            btnSound.Checked = SoundLevel > 0;
        }

        WebServer server;

        public WebServer WebServer => server;

        private void InitializeServer()
        {
            server = new WebServer();
            server.OnClientConnected += delegate (object sender, WSClient e)
            {
                SetStatusText($"Client connected with IP {e.IP}...");
            };
            server.OnClientDisconnected += delegate (object sender, WSClient e)
            {
                SetStatusText($"Client with IP {e.IP} disconnected...");
            };
            server.OnServerStarted += delegate (object sender, EventArgs e)
            {
                SetStatusText("Server started...");
            };
            server.OnServerStopped += delegate (object sender, EventArgs e)
            {
                SetStatusText("Server stopper...");
            };

            if (server.Enabled)
                server.Start();
        }

        private void StopServer()
        {
            try
            {
                SetStatusText("Stopping the server...");
                server?.Stop();
                server?.Dispose();
            }
            catch { }
            server = null;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            WindowsFormHelper.SaveState(this);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            SaveSettings();
            StopServer();
            ClosePlugins();
        }

        private void SaveSettings()
        {
            string oldSettings = ObjectSerializeHelper.GetJson(AppSettings.LoadFromFile(localSettingFile));
            string newSettings = ObjectSerializeHelper.GetJson(settings);
            if (oldSettings != newSettings)
            {
                try
                {
                    settings.SaveToFile(localSettingFile);

                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
        }

        /// <summary>
        /// Закрытие приложения
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            foreach (ICustomPlugin customPlugin in customPlugins)
            {
                if (!customPlugin.CanClose(this))
                {
                    e.Cancel = true;
                    break;
                }
            }
        }

        /// <summary>
        /// Загрузка плагинов
        /// </summary>
        private void LoadPlugins()
        {
            // Загружаем пользовательские плагины
            if (Directory.Exists(settings.PluginsFolder))
            {
                foreach (string fname in Directory.EnumerateFiles(settings.PluginsFolder, "*.dll", SearchOption.AllDirectories))
                {
                    if (PluginManager.LoadFromFile(fname) is ICustomPlugin customPlugin)
                    {
                        if (!settings.DisabledPlugins.Contains(customPlugin.Name))
                        {
                            try
                            {
                                if (customPlugin.Initialize(this))
                                {
                                    initializedPlugins.Add(customPlugin);
                                }
                                else
                                    if (customPlugin is IDisposable p)
                                    p?.Dispose();
                            }
                            catch { }
                        }
                        customPlugins.Add(customPlugin);
                    }
                }
            }
        }

        /// <summary>
        /// Завершение работы с плагинами
        /// </summary>
        private void ClosePlugins()
        {
            SetStatusText("Finishing plugins...");
            Parallel.ForEach(initializedPlugins, customPlugin =>
            {
                try
                {
                    customPlugin.Quit(this);

                }
                catch { Console.WriteLine($"{customPlugin.Name} finished with error..."); }
            });

            foreach (ICustomPlugin plugin in Plugins)
            {
                if (plugin is IDisposable p)
                {
                    try
                    {
                        p.Dispose();
                    }
                    catch { }
                }
            }

            customPlugins.Clear();
            initializedPlugins.Clear();
        }

        private void AppWindow_OnSoundLevelChanged(object sender, EventArgs e)
        {
            btnSound.Checked = SoundLevel != 0;
        }

        /// <summary>
        /// Отрисовка формы
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            int y = btnExit.Bottom + 2;
            using (Font font = new Font(SystemFonts.CaptionFont.FontFamily, 12))
            using (Brush brush = new SolidBrush(Color.FromArgb(246, 130, 17)))
            using (StringFormat sf = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            {
                e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                e.Graphics.DrawString(Properties.Resources.AppTitle, font, brush, new Rectangle(ClientRectangle.Left, ClientRectangle.Top, ClientRectangle.Width, y), sf);

                // Отрисовка логотипа на фоне
                using (var imageAttributes = new ImageAttributes())
                {
                    var colorMatrix = new ColorMatrix() { Matrix33 = 0.1f };
                    imageAttributes.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                    Rectangle destRect = new Rectangle(0, 0, ClientRectangle.Height, ClientRectangle.Height);
                    destRect.Offset((ClientRectangle.Width - ClientRectangle.Height) / 2, 0);
                    e.Graphics.DrawImage(Properties.Resources.logo, destRect, 0, 0, Properties.Resources.logo.Width, Properties.Resources.logo.Height, GraphicsUnit.Pixel, imageAttributes);
                }
            }
        }

        /// <summary>
        /// Изменение размеров клиентской области окна
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClientSizeChanged(EventArgs e)
        {
            base.OnClientSizeChanged(e);
            UpdateMaximizeButton();
            Invalidate();
        }

        /// <summary>
        /// Кнопка завершения
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Отображение формы
        /// </summary>
        /// <param name="e"></param>
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            foreach (IGamePlugin game in GamePlugins.OrderBy(o => o.Name).ToList())
            {
                titlesGames.AddTile(new GameTitle(game, this));
            }

            foreach (ICustomDevicePlugin plugin in CustomDevices.OrderBy(o => o.DeviceName).ToList())
            {
                titlesDevices.AddTile(new DeviceTitle(plugin, this));
            }

            UpdateMaximizeButton();
        }

        /// <summary>
        /// Обновление кнопки разворачивания/сворачивания окна
        /// </summary>
        private void UpdateMaximizeButton()
        {
            btnExpand.Image = WindowState == FormWindowState.Maximized ? Properties.Resources.restore_window32 : Properties.Resources.expand_window;
        }

        /// <summary>
        /// Версия приложения
        /// </summary>
        public ulong Version => 1U;

        private readonly List<ICustomDevicePlugin> customDevices = new List<ICustomDevicePlugin>();
        private readonly List<IGamePlugin> gamePlugins = new List<IGamePlugin>();
        private readonly List<ICustomPlugin> customPlugins = new List<ICustomPlugin>();
        private readonly List<ICustomPlugin> initializedPlugins = new List<ICustomPlugin>();

        public event EventHandler OnSoundLevelChanged;
        public event EventHandler<DeviceChangeEventsArgs> OnDeviceRemoveComplete;
        public event EventHandler<DeviceChangeEventsArgs> OnDeviceRemovePending;
        public event EventHandler<DeviceChangeEventsArgs> OnDeviceArrival;
        public event EventHandler<TelemetryUpdatedEventArgs> OnTelemetry;
        public event EventHandler OnGameStarted;
        public event EventHandler OnGameStopped;

        public IList<ICustomDevicePlugin> CustomDevices => customDevices;
        public IList<IGamePlugin> GamePlugins => gamePlugins;
        public IList<ICustomPlugin> Plugins => customPlugins;


        /// <summary>
        /// Разворачивание/сворачивание окна
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnExpand_Click(object sender, EventArgs e)
        {
            WindowState = WindowState == FormWindowState.Maximized ? FormWindowState.Normal : FormWindowState.Maximized;
        }

        /// <summary>
        /// Минимизация окна
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnMinimize_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        /// <summary>
        /// Активация окна
        /// </summary>
        public void AcivateForm()
        {
            Activate();
        }

        /// <summary>
        /// Регистрация пользовательского устройства
        /// </summary>
        /// <param name="customDevice"></param>
        /// <returns></returns>
        public bool RegisterDevice(ICustomDevicePlugin customDevice)
        {
            if (customDevices.Find(p => p.DeviceID == customDevice.DeviceID) == null)
            {
                customDevices.Add(customDevice);
                customDevice.OnConnected += CustomDevice_OnConnected;
                customDevice.OnDisconnected += CustomDevice_OnDisconnected;
                return true;
            }
            return false;
        }

        private void CustomDevice_OnDisconnected(object sender, EventArgs e)
        {
            if (sender is ICustomDevicePlugin plugin)
            {
                SetStatusText($"Device {plugin.DeviceName} disconnected...");
                titlesDevices.Invalidate();
            }
        }

        private void CustomDevice_OnConnected(object sender, EventArgs e)
        {
            if (sender is ICustomDevicePlugin plugin)
            {
                SetStatusText($"Device {plugin.DeviceName} connected...");
                titlesDevices.Invalidate();
            }
        }

        /// <summary>
        /// Отмена регистрация пользовательского устройства
        /// </summary>
        /// <param name="customDevice"></param>
        /// <returns></returns>
        public bool UnRegisterDevice(ICustomDevicePlugin customDevice)
        {
            customDevices.Remove(customDevice);
            customDevice.OnConnected -= CustomDevice_OnConnected;
            customDevice.OnDisconnected -= CustomDevice_OnDisconnected;

            return false;
        }

        /// <summary>
        /// Отображение окна о программе
        /// </summary>
        public void About()
        {
            MessageBox.Show(this, "version 1.0.079\r\nRelease Date: 2025-09-20", "vAzhure Racing Hub");
        }

        AppSettings settings = new AppSettings();

        public AppSettings Settings
        {
            get
            {
                return settings;
            }
            set
            {
                settings = value;
            }
        }

        /// <summary>
        /// Отображение окна настроек
        /// </summary>
        public void ShowSettings()
        {
            using (SettingsForm frm = new SettingsForm(this))
            {
                frm.ShowDialog(this);
            }
        }

        /// <summary>
        /// Уровень звука
        /// </summary>
        public int SoundLevel
        {
            get => settings.Volume;
            set
            {
                int v = Math2.Clamp(value, 0, 100);
                if (settings.Volume != v)
                {
                    settings.Volume = v;
                    OnSoundLevelChanged(this, new EventArgs());
                }
            }
        }

        public Form MainForm => this;

        public int ApplicationSoundVolume => SoundLevel;

        /// <summary>
        /// Сохраненный уровень звука
        /// </summary>
        int nPrevSoundLevel = 0;

        /// <summary>
        /// Включение/отключение звука
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSound_OnSwitch(object sender, EventArgs e)
        {
            if (btnSound.Checked == false)
            {
                SoundLevel = volumeControl.Value = 0;
            }
            else
            {
                if (nPrevSoundLevel > 1)
                    SoundLevel = volumeControl.Value = nPrevSoundLevel;
                else
                    SoundLevel = volumeControl.Value = 100;
            }
        }

        public bool RegisterGame(IGamePlugin game)
        {
            if (gamePlugins.Find(p => p.Name == game.Name) == null)
            {
                gamePlugins.Add(game);
                game.OnTelemetry += Game_OnTelemetry;
                game.OnGameStateChanged += Game_OnGameStateChanged;
                return true;
            }
            else return false;
        }

        private void Game_OnGameStateChanged(object sender, EventArgs e)
        {
            if (sender is IGamePlugin game)
            {
                BeginInvoke((Action)delegate
                {
                    lblStatus.Text = game.IsRunning ? $"Game {game.Name} started..." : $"Game {game.Name} stopped...";
                    titlesGames.Invalidate();
                });

                if (game.IsRunning)
                    OnGameStarted?.Invoke(game, new EventArgs());
                else
                    OnGameStopped?.Invoke(game, new EventArgs());
            }
        }

        private void Game_OnTelemetry(object sender, TelemetryUpdatedEventArgs e)
        {
            foreach (ICustomDevicePlugin devicePlugin in customDevices)
            {
                BeginInvoke((Action)delegate
                {
                    try
                    {
                        devicePlugin.OnTelemetry(this, e.TelemetryInfo);

                    }
                    catch { }
                });
            }

            if (server != null && server.Enabled)
            {
                BeginInvoke((Action)delegate
                {
                    server?.OnTelemetry(this, e.TelemetryInfo);
                });
            }

            BeginInvoke((Action)delegate
            {
                OnTelemetry?.Invoke(this, e);
            });
        }

        public bool UnRegisterGame(IGamePlugin game)
        {
            if (gamePlugins.Contains(game))
            {
                gamePlugins.Remove(game);
                game.OnTelemetry -= Game_OnTelemetry;
                game.OnGameStateChanged -= Game_OnGameStateChanged;
                return true;
            }
            else return false;
        }

        public void SetStatusText(string text)
        {
            BeginInvoke((Action)delegate
            {
                lblStatus.Text = text;
                lblStatus.Invalidate();
            });
        }

        private void BtnAbout_Click(object sender, EventArgs e)
        {
            About();
        }

        private void BtnSettings_Click(object sender, EventArgs e)
        {
            ShowSettings();
        }

        public class AppSettings : ICloneable
        {
            public int Volume { get; set; } = 25;
            public string PluginsFolder { get; set; } = "";
            /// <summary>
            /// Заблокированные плагины
            /// </summary>
            public List<string> DisabledPlugins { get; set; } = new List<string>();
            /// <summary>
            /// Заблокированные игры
            /// </summary>
            public List<string> DisabledGames { get; set; } = new List<string>();
            /// <summary>
            /// Пользовательские приложения
            /// </summary>
            public List<UserApp> UserApps { get; set; } = new List<UserApp>();

            public ServerSettings Server { get; set; } = new ServerSettings();

            /// <summary>
            /// Создание полной копии объекта
            /// </summary>
            /// <returns></returns>
            public object Clone()
            {
                return MemberwiseClone();
            }

            /// <summary>
            /// Чтение настроек из файла
            /// </summary>
            /// <param name="path"></param>
            /// <returns></returns>
            public static AppSettings LoadFromFile(string path)
            {
                try
                {
                    string json = File.ReadAllText(path);
                    return ObjectSerializeHelper.DeserializeJson<AppSettings>(json);
                }
                catch
                {
                    return null;
                }
            }

            public void SaveToFile(string path)
            {
                try
                {
                    File.WriteAllText(path, ObjectSerializeHelper.GetJson(this));
                }
                catch { }
            }

            public class UserApp : ICloneable
            {
                public string Name { get; set; } = "";
                public string Icon { get; set; } = "";
                public string Executable { get; set; } = "";

                public object Clone()
                {
                    return MemberwiseClone();
                }
            }

            public class ServerSettings : ICloneable
            {
                public bool Enabled { get; set; } = false;
                public int Port { get; set; } = 8080;
                public string ServerFolder { get; set; } = "";

                public object Clone()
                {
                    return MemberwiseClone();
                }
            }
        }

        private void VolumeControl_OnValueChanged(object sender, EventArgs e)
        {
            SoundLevel = volumeControl.Value;
            if (SoundLevel > 0)
                nPrevSoundLevel = SoundLevel;
            btnSound.Checked = SoundLevel > 0;
        }

        public void Restart()
        {
            Program.Restart();
        }
    }
}