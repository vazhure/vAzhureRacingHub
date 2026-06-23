using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using vAzhureRacingAPI;

namespace IL2
{
    /// <summary>
    /// Плагин телеметрии для игры IL-2 Sturmovik (движок IL-2 Great Battles).
    /// </summary>
    public class IL2Game : VAzhureUDPClient, IGamePlugin, IDisposable
    {
        public string Name => "IL-2 Sturmovik";

        /// <summary>
        /// Базовый Steam AppID серии IL-2 Great Battles.
        /// </summary>
        public uint SteamGameID => 307960U;

        /// <summary>
        /// Имя процесса исполняемого файла IL-2 Great Battles.
        /// </summary>
        public string[] ExecutableProcessName => new[] { "Il-2" };

        private string strUserIcon = "";

        public string UserIconPath { get => strUserIcon; set => strUserIcon = value; }
        public string UserExecutablePath { get => settings.ExecutablePath; set => settings.ExecutablePath = value; }

        public bool IsRunning => Utils.IsProcessRunning(ExecutableProcessName);

        public event EventHandler<TelemetryUpdatedEventArgs> OnTelemetry;
        public event EventHandler OnGameStateChanged;
        public event EventHandler OnGameIconChanged;

        private readonly GameSettings settings;

        private readonly ProcessMonitor monitor;
        private readonly TelemetryDataSet dataSet;

        /// <summary>Магический идентификатор пакета телеметрии IL-2 GB ("IL" + версия 1).</summary>
        private const uint IL2_PACKET_MAGIC = 0x494C0100;

        public IL2Game()
        {
            settings = new GameSettings();

            dataSet = new TelemetryDataSet(this);

            try
            {
                string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), $"{Name}.json");
                if (File.Exists(path))
                {
                    string json = File.ReadAllText(path);
                    settings = ObjectSerializeHelper.DeserializeJson<GameSettings>(json);
                }
            }
            catch { }

            monitor = new ProcessMonitor(ExecutableProcessName);
            monitor.OnProcessRunningStateChanged += (process, bRunning) =>
            {
                if (bRunning)
                {
                    Run(settings.Port);
                }
                else
                {
                    dataSet.LoadDefaults();
                    OnTelemetry?.Invoke(this, new TelemetryUpdatedEventArgs(dataSet));
                    Stop();
                }
                OnGameStateChanged?.Invoke(this, EventArgs.Empty);
            };
            monitor.Start();
        }

        public System.Drawing.Icon GetIcon()
        {
            return DCS.Properties.Resources.Il_2;
        }

        /// <summary>
        /// Имя секции motion device в startup.cfg.
        /// IL-2 GB использует формат: "[KEY = motiondevice]".
        /// </summary>
        private const string MotionDeviceSection = "[KEY = motiondevice]";

        /// <summary>
        /// Ключи, которые плагин устанавливает в секции motiondevice.
        /// </summary>
        private static readonly string[] MotionDeviceKeys = { "addr", "port", "enable", "decimation" };

        /// <summary>
        /// Показывает диалог патча startup.cfg игры для включения встроенной
        /// UDP-телеметрии IL-2 GB.
        /// </summary>
        public void ShowSettings(IVAzhureRacingApp app)
        {
            try
            {
                if (MessageBox.Show(
                        $"Patch IL-2 startup.cfg to enable UDP telemetry on port {settings.Port}?",
                        "Patch",
                        MessageBoxButtons.YesNoCancel,
                        MessageBoxIcon.Question,
                        MessageBoxDefaultButton.Button1) != DialogResult.Yes)
                {
                    return;
                }

                string configPath = ResolveConfigPath(app);
                if (string.IsNullOrEmpty(configPath))
                    return;

                if (IsAlreadyPatched(configPath))
                {
                    app.SetStatusText($"{Name} already patched! Config: {configPath}");
                    return;
                }

                PatchConfig(configPath);
                app.SetStatusText($"{Name} patched successfully! Config: {configPath}");
            }
            catch (UnauthorizedAccessException)
            {
                app.SetStatusText($"Failed to patch {Name}: access denied (run as admin)");
            }
            catch (Exception ex)
            {
                app.SetStatusText($"Failed to patch {Name}: {ex.Message}");
            }
        }

        /// <summary>
        /// Проверяет, пропатчен ли уже startup.cfg.
        /// </summary>
        private bool IsAlreadyPatched(string configPath)
        {
            if (!File.Exists(configPath))
                return false;

            string content = File.ReadAllText(configPath);
            string[] lines = content.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

            int sectionStart = -1;
            for (int i = 0; i < lines.Length; i++)
            {
                if (IsSectionHeader(lines[i], MotionDeviceSection))
                {
                    sectionStart = i;
                    break;
                }
            }

            if (sectionStart < 0)
                return false;

            int sectionEnd = lines.Length;
            for (int i = sectionStart + 1; i < lines.Length; i++)
            {
                if (IsAnySectionHeader(lines[i]))
                {
                    sectionEnd = i;
                    break;
                }
            }

            var foundValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            for (int i = sectionStart + 1; i < sectionEnd; i++)
            {
                string key = ParseIniKey(lines[i]);
                if (key != null)
                {
                    string value = ParseIniValue(lines[i]);
                    foundValues[key] = value;
                }
            }

            if (!foundValues.TryGetValue("addr", out string addr) ||
                !string.Equals(addr.Trim('"'), settings.IP, StringComparison.OrdinalIgnoreCase))
                return false;

            if (!foundValues.TryGetValue("port", out string port) ||
                !int.TryParse(port, out int portNum) || portNum != settings.Port)
                return false;

            if (!foundValues.TryGetValue("enable", out string enable) ||
                !string.Equals(enable.Trim(), "true", StringComparison.OrdinalIgnoreCase))
                return false;

            if (!foundValues.TryGetValue("decimation", out string decimation) ||
                !int.TryParse(decimation, out int decVal) || decVal != 2)
                return false;

            return true;
        }

        /// <summary>
        /// Возвращает путь к startup.cfg. Если путь ещё не сохранён в настройках,
        /// показывает диалог выбора файла.
        /// </summary>
        private string ResolveConfigPath(IVAzhureRacingApp app)
        {
            if (!string.IsNullOrEmpty(settings.ConfigPath) && File.Exists(settings.ConfigPath))
                return settings.ConfigPath;

            using (var dialog = new OpenFileDialog())
            {
                dialog.Title = "Select IL-2 startup.cfg";
                dialog.Filter = "Config files (*.cfg)|*.cfg|All files (*.*)|*.*";
                dialog.FileName = "startup.cfg";

                string steamPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
                    "Steam", "steamapps", "common");

                if (Directory.Exists(steamPath))
                    dialog.InitialDirectory = steamPath;

                if (dialog.ShowDialog() != DialogResult.OK)
                {
                    app.SetStatusText($"{Name}: file selection cancelled");
                    return null;
                }

                settings.ConfigPath = dialog.FileName;
                SaveSettings();

                return dialog.FileName;
            }
        }

        /// <summary>
        /// Сохраняет текущие настройки в JSON.
        /// </summary>
        private void SaveSettings()
        {
            try
            {
                string path = Path.Combine(
                    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                    $"{Name}.json");

                string json = ObjectSerializeHelper.GetJson(settings);
                File.WriteAllText(path, json);
            }
            catch { }
        }

        /// <summary>
        /// Патчит startup.cfg: гарантирует наличие секции [KEY = motiondevice]
        /// с корректными значениями addr/port/enable/decimation.
        /// </summary>
        private void PatchConfig(string configPath)
        {
            string backupPath = configPath + ".bak";
            if (!File.Exists(backupPath))
                File.Copy(configPath, backupPath, overwrite: false);

            string content = File.ReadAllText(configPath);
            string newline = content.Contains("\r\n") ? "\r\n" : "\n";
            string[] lines = content.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

            int sectionStart = -1;
            for (int i = 0; i < lines.Length; i++)
            {
                if (IsSectionHeader(lines[i], MotionDeviceSection))
                {
                    sectionStart = i;
                    break;
                }
            }

            var targetValues = new Dictionary<string, string>
            {
                { "addr",        $"{settings.IP}" },
                { "port",        settings.Port.ToString() },
                { "enable",      "true" },
                { "decimation",  "2" },
            };

            var result = new List<string>(lines.Length + 16);

            if (sectionStart < 0)
            {
                result.AddRange(lines);

                while (result.Count > 0 && string.IsNullOrWhiteSpace(result[result.Count - 1]))
                    result.RemoveAt(result.Count - 1);

                result.Add("");
                result.Add(MotionDeviceSection);
                foreach (string key in MotionDeviceKeys)
                    result.Add($"{key} = {targetValues[key]}");
                result.Add("[END]");
                result.Add("");
            }
            else
            {
                for (int i = 0; i <= sectionStart; i++)
                    result.Add(lines[i]);

                int sectionEnd = lines.Length;
                for (int i = sectionStart + 1; i < lines.Length; i++)
                {
                    if (IsAnySectionHeader(lines[i]))
                    {
                        sectionEnd = i;
                        break;
                    }
                }

                var updatedKeys = new HashSet<string>();
                for (int i = sectionStart + 1; i < sectionEnd; i++)
                {
                    string line = lines[i];
                    string key = ParseIniKey(line);

                    if (key != null && targetValues.ContainsKey(key))
                    {
                        result.Add(RebuildIniLine(line, key, targetValues[key]));
                        updatedKeys.Add(key);
                    }
                    else
                    {
                        result.Add(line);
                    }
                }

                foreach (string key in MotionDeviceKeys)
                {
                    if (!updatedKeys.Contains(key))
                        result.Add($"{key} = {targetValues[key]}");
                }

                for (int i = sectionEnd; i < lines.Length; i++)
                    result.Add(lines[i]);
            }

            string newContent = string.Join(newline, result);
            File.WriteAllText(configPath, newContent, new System.Text.UTF8Encoding(false));
        }

        /// <summary>
        /// Проверяет, является ли строка заголовком секции вида [KEY = name] или [name].
        /// </summary>
        private static bool IsAnySectionHeader(string line)
        {
            if (string.IsNullOrWhiteSpace(line)) return false;
            string trimmed = line.Trim();
            return trimmed.StartsWith("[") && trimmed.EndsWith("]");
        }

        /// <summary>
        /// Проверяет, является ли строка заголовком конкретной секции.
        /// </summary>
        private static bool IsSectionHeader(string line, string expectedHeader)
        {
            if (string.IsNullOrWhiteSpace(line)) return false;
            return string.Equals(line.Trim(), expectedHeader, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Извлекает имя ключа из строки вида "key = value" или "key=value".
        /// Возвращает null для комментариев и пустых строк.
        /// </summary>
        private static string ParseIniKey(string line)
        {
            if (string.IsNullOrWhiteSpace(line)) return null;
            string trimmed = line.TrimStart();
            if (trimmed.StartsWith(";") || trimmed.StartsWith("#") || trimmed.StartsWith("["))
                return null;

            int eq = trimmed.IndexOf('=');
            if (eq <= 0) return null;

            string key = trimmed.Substring(0, eq).Trim();
            return string.IsNullOrEmpty(key) ? null : key;
        }

        /// <summary>
        /// Извлекает значение из строки вида "key = value".
        /// Убирает комментарии после // и обрезает пробелы.
        /// </summary>
        private static string ParseIniValue(string line)
        {
            if (string.IsNullOrWhiteSpace(line)) return null;

            int eq = line.IndexOf('=');
            if (eq < 0) return null;

            string value = line.Substring(eq + 1);

            int comment = value.IndexOf("//");
            if (comment >= 0)
                value = value.Substring(0, comment);

            return value.Trim();
        }

        /// <summary>
        /// Пересобирает строку INI с новым значением, сохраняя исходный отступ
        /// и хвостовой комментарий (если был).
        /// </summary>
        private static string RebuildIniLine(string originalLine, string key, string newValue)
        {
            int indentLen = 0;
            while (indentLen < originalLine.Length && char.IsWhiteSpace(originalLine[indentLen]))
                indentLen++;
            string indent = originalLine.Substring(0, indentLen);

            string rest = originalLine.Substring(indentLen);
            string comment = "";
            int semi = rest.IndexOf(';');
            int hash = rest.IndexOf('#');
            int commentPos = -1;
            if (semi >= 0 && hash >= 0) commentPos = Math.Min(semi, hash);
            else if (semi >= 0) commentPos = semi;
            else if (hash >= 0) commentPos = hash;

            if (commentPos >= 0)
            {
                comment = " " + rest.Substring(commentPos).TrimStart();
            }

            return $"{indent}{key} = {newValue}{comment}";
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

        private readonly AMMotionData defaults = new AMMotionData();

        /// <summary>
        /// Обработка входящего UDP-пакета с телеметрией.
        /// Формат данных — БИНАРНЫЙ, соответствует struct IL2API (44 байта, Pack=4).
        /// </summary>
        public override void OnDataReceived(ref byte[] bytes)
        {
            try
            {
                if (bytes == null || bytes.Length < IL2API.Size)
                    return;

                IL2API telemetry = Marshalizable<IL2API>.FromBytes(bytes);

                if (telemetry.packetID != IL2_PACKET_MAGIC)
                    return;

                float rollDeg = telemetry.roll * (180f / (float)Math.PI);
                float pitchDeg = telemetry.pitch * (180f / (float)Math.PI);
                float yawDeg = telemetry.yaw * (180f / (float)Math.PI);

                if (settings.LoopRollAngle)
                    rollDeg = LoopAngle(rollDeg, 90f);

                float roll = rollDeg / 180f;
                float pitch = pitchDeg / 180f;
                float yaw = yawDeg / 180f;

                roll = ((roll + 1f) % 2f) - 1f;
                pitch = ((pitch + 1f) % 2f) - 1f;
                yaw = ((yaw + 1f) % 2f) - 1f;

                float surge = telemetry.accX;
                float sway = telemetry.accY;
                float heave = telemetry.accZ;

                const float MAX_ACC = 2f;
                surge = Math2.Clamp(surge, -MAX_ACC, MAX_ACC);
                sway = Math2.Clamp(sway, -MAX_ACC, MAX_ACC);
                heave = Math2.Clamp(heave, -MAX_ACC, MAX_ACC);

                dataSet.CarData.MotionData = new AMMotionData()
                {
                    Roll = roll,
                    Pitch = pitch,
                    Yaw = yaw,
                    Surge = surge,
                    Sway = sway,
                    Heave = heave
                };

                OnTelemetry?.Invoke(this, new TelemetryUpdatedEventArgs(dataSet));
            }
            catch { }
        }

        /// <summary>
        /// Сворачивает углы крена, превышающие minMag по модулю, в диапазон
        /// [-180+minMag, 180-minMag].
        /// </summary>
        private static float LoopAngle(float angle, float minMag)
        {
            float absAngle = Math.Abs(angle);
            if (absAngle <= minMag)
                return angle;

            float direction = angle / absAngle;
            float loopedAngle = (180.0f * direction) - angle;
            return loopedAngle;
        }

        public void Dispose()
        {
            monitor?.Stop();
            SaveSettings();
        }
    }

    /// <summary>
    /// Настройки плагина IL-2. Сериализуются в JSON рядом со сборкой.
    /// </summary>
    public class GameSettings
    {
        /// <summary>
        /// UDP-порт, на который IL-2 GB шлёт бинарную телеметрию.
        /// По умолчанию 4321 — стандартный порт IL-2 GB telemetry.
        /// </summary>
        public int Port { get; set; } = 4321;

        /// <summary>IP-адрес для привязки UDP-клиента (обычно 127.0.0.1).</summary>
        public string IP { get; set; } = "127.0.0.1";

        /// <summary>Путь к исполняемому файлу IL-2 (Il-2.exe) для ручного запуска.</summary>
        public string ExecutablePath = "";

        /// <summary>Путь к startup.cfg, выбранный пользователем.</summary>
        public string ConfigPath { get; set; } = "";

        /// <summary>
        /// Если true, крен &gt; ±90° сворачивается обратно
        /// Полезно для motion-платформ, чтобы избежать полного оборота при перевороте.
        /// </summary>
        public bool LoopRollAngle { get; set; } = true;
    }

    /// <summary>
    /// Бинарная структура телеметрии IL-2 Great Battles
    /// Сериализуется прямо в UDP-пакет, Pack=4, размер = 44 байта.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Unicode)]
    public struct IL2API
    {
        /// <summary>Магический идентификатор пакета: 0x494C0100 ("IL" + версия 1).</summary>
        public uint packetID;

        /// <summary>Игровой тик на момент отправки.</summary>
        public uint tick;

        /// <summary>Рысканье (yaw), радианы.</summary>
        public float yaw;

        /// <summary>Тангаж (pitch), радианы.</summary>
        public float pitch;

        /// <summary>Крен (roll), радианы.</summary>
        public float roll;

        /// <summary>Угловая скорость по оси X, рад/с.</summary>
        public float spinX;

        /// <summary>Угловая скорость по оси Y, рад/с.</summary>
        public float spinY;

        /// <summary>Угловая скорость по оси Z, рад/с.</summary>
        public float spinZ;

        /// <summary>Ускорение по оси X (Surge).</summary>
        public float accX;

        /// <summary>Ускорение по оси Y (Sway).</summary>
        public float accY;

        /// <summary>Ускорение по оси Z (Heave).</summary>
        public float accZ;

        /// <summary>Размер структуры в байтах (11 полей × 4 байта = 44).</summary>
        public static readonly int Size = Marshal.SizeOf(typeof(IL2API));
    }
}