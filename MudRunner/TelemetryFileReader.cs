using System;
using System.IO;
using System.Text;
using System.Threading;

namespace MudRunner
{
    /// <summary>
    /// File-based telemetry reader — Index Byte Protocol v5.
    /// 
    /// Reads telemetry from a file written by MudRunner's Lua script.
    /// Uses an index-byte protocol to detect new data without corruption:
    /// 
    /// PROTOCOL:
    ///   telemetry_out.txt — single file, text line + trailing index byte
    /// 
    ///   Lua:  open("w") → write(line + "\n" + chr(index)) → close()
    ///         index increments every frame: index = (index + 1) % 255
    /// 
    ///   C#:   open(FileShare.ReadWrite) → read LAST byte (index)
    ///         if index == lastIndex → no new data → sleep, repeat
    ///         if index != lastIndex → read full content (minus last byte)
    ///         parse telemetry → update lastIndex → emit events
    /// 
    /// </summary>
    public class TelemetryFileReader : IDisposable
    {
        private const string DefaultFileName = "telemetry_out.txt";
        private const int PollIntervalMs = 10;       // 200 Hz — well above Lua's 60 Hz
        private const int SlowPollMs = 500;          // When file not found
        private const double ConnectionTimeoutSec = 2.0;
        private const int fileSize = 504; // bytes

        /// <summary>
        /// Sentinel value meaning "no valid data read yet".
        /// Since Lua uses index % 255 (range 0-254), 255 is never a valid index.
        /// </summary>
        private const byte NoDataIndex = 255;

        private readonly string _fileName;
        private Thread _pollThread;
        private volatile bool _isRunning;
        private byte _lastIndex = NoDataIndex;
        private DateTime _lastSuccessfulRead;
        private ushort _sequence;

        /// <summary>
        /// Raised when a raw telemetry line is received from Lua.
        /// </summary>
        public event EventHandler<string> OnTelemetryLine;

        /// <summary>
        /// Raised when a parsed telemetry packet is available.
        /// </summary>
        public event EventHandler<MudRunnerTelemetry.TelemetryPacket> OnTelemetryPacket;

        /// <summary>
        /// Raised when the connection state changes (true = receiving data).
        /// </summary>
        public event EventHandler<bool> OnConnectionStateChanged;

        public string FileName => _fileName;
        public bool IsRunning => _isRunning;
        public bool IsConnected { get; private set; }
        public string LastError { get; private set; } = string.Empty;
        public string FullPath { get; set; } = string.Empty;

        public TelemetryFileReader(string fileName = DefaultFileName)
        {
            _fileName = fileName;
            FullPath = fileName;
        }

        /// <summary>
        /// Start polling the telemetry file on a background thread.
        /// </summary>
        public void Start()
        {
            if (_isRunning) return;

            if (FullPath == string.Empty)
            {
                string gameDir = DetectGameDirectory();
                if (!string.IsNullOrEmpty(gameDir))
                {
                    FullPath = Path.Combine(gameDir, _fileName);
                }
                else
                {
                    FullPath = _fileName;
                }
            }

            _isRunning = true;
            _lastSuccessfulRead = DateTime.MinValue;
            _lastIndex = NoDataIndex;
            _sequence = 0;
            IsConnected = false;
            LastError = null;

            _pollThread = new Thread(PollLoop)
            {
                Name = "MudRunnerTelemetry",
                IsBackground = true,
                Priority = ThreadPriority.AboveNormal  // Responsive reads
            };
            _pollThread.Start();

            Console.WriteLine($"[MudRunner File v5] Watching: {FullPath}");
            Console.WriteLine($"[MudRunner File v5] Protocol: index byte (mod 255), FileShare.ReadWrite");
        }

        /// <summary>
        /// Stop polling and release resources.
        /// </summary>
        public void Stop()
        {
            if (!_isRunning) return;
            _isRunning = false;
            // Thread will exit on next loop iteration (volatile read)
        }

        /// <summary>
        /// Main polling loop — runs on a dedicated background thread.
        /// 
        /// Algorithm:
        ///   1. Open file with FileShare.ReadWrite (allows Lua to write)
        ///   2. Read ONLY the last byte (1 byte I/O — very fast)
        ///   3. If byte == _lastIndex → no new data → close, sleep 10ms, repeat
        ///   4. If byte != _lastIndex → read full content (minus last byte)
        ///   5. Parse telemetry → if OK, update _lastIndex, emit events
        ///   6. If parse fails → DON'T update _lastIndex → will retry next cycle
        /// </summary>
        private void PollLoop()
        {
            while (_isRunning)
            {
                try
                {
                    CheckConnectionTimeout();

                    string line = TryReadTelemetry();

                    if (line != null)
                    {
                        // Process outside of file lock
                        ProcessLine(line);
                        Thread.Sleep(PollIntervalMs);
                    }
                    else
                    {
                        if (_lastPacket.bValid)
                            OnTelemetryPacket?.Invoke(this, _lastPacket);
                        Thread.Sleep(PollIntervalMs);
                    }
                }
                catch (FileNotFoundException)
                {
                    // File doesn't exist yet — Lua hasn't started writing
                    Thread.Sleep(SlowPollMs);
                    _lastPacket.bValid = false;
                }
                catch (DirectoryNotFoundException)
                {
                    // Game directory not available
                    Thread.Sleep(SlowPollMs);
                }
                catch (IOException)
                {
                    // File locked or being written — retry shortly
                    if (_lastPacket.bValid)
                        OnTelemetryPacket?.Invoke(this, _lastPacket);

                    Thread.Sleep(PollIntervalMs);
                }
                catch (UnauthorizedAccessException)
                {
                    if (_lastPacket.bValid)
                        OnTelemetryPacket?.Invoke(this, _lastPacket);

                    Thread.Sleep(SlowPollMs);
                }
                catch (Exception ex)
                {
                    LastError = $"Read error: {ex.Message}";
                    Console.WriteLine($"[MudRunner File v5] Error: {ex.Message}");
                    Thread.Sleep(PollIntervalMs);
                }
            }

            IsConnected = false;
        }

        /// <summary>
        /// Attempt to read telemetry data from the file.
        /// 
        /// Returns: telemetry line if new valid data found, null otherwise.
        /// Side effect: updates _lastIndex only on successful read.
        /// </summary>
        private string TryReadTelemetry()
        {
            using (var fs = new FileStream(FullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                if (fs.Length != fileSize)
                {
                    // File too small — Lua might be mid-write or file empty
                    return null;
                }

                // =============================================
                // Step 1: Read ONLY the last byte (the index)
                // This is the cheap check — 1 byte I/O
                // =============================================
                fs.Position = fileSize - 1;
                int b = fs.ReadByte();
                if (b < 0) return null;

                byte currentIndex = (byte)b;

                // =============================================
                // Step 2: Compare with last known index
                // Same → no new data → skip entirely
                // =============================================
                if (currentIndex == _lastIndex)
                {
                    return null;
                }

                // =============================================
                // Step 3: Index changed — NEW data!
                // Read the telemetry line (everything except last byte)
                // =============================================
                int dataLen = (int)fs.Length - 1;
                byte[] data = new byte[dataLen];
                fs.Position = 0;
                int bytesRead = fs.Read(data, 0, dataLen);

                // Convert to string and clean up
                string rawLine = Encoding.UTF8.GetString(data, 0, bytesRead);
                string line = rawLine.Trim();

                // =============================================
                // Step 4: Validate data
                // Must start with "T|" — if not, data is garbled
                // (race condition: C# read during Lua's write)
                // =============================================
                if (string.IsNullOrEmpty(line) || !line.StartsWith("T|"))
                {
                    // DON'T update _lastIndex — will retry next poll cycle.
                    // At 100Hz vs Lua's 60Hz, we'll re-read the completed
                    // file within 10ms. Frame loss: at most 1 frame (~16ms).
                    return null;
                }

                // Valid data — update index so we don't re-read this frame
                _lastIndex = currentIndex;
                return line;
            }
        }

        MudRunnerTelemetry.TelemetryPacket _lastPacket;

        /// <summary>
        /// Parse and emit a telemetry line.
        /// </summary>
        private void ProcessLine(string line)
        {
            OnTelemetryLine?.Invoke(this, line);

            try
            {
                var packet = MudRunnerTelemetry.ParseTextLine(line);
                if (packet.bValid)
                {
                    _lastPacket = packet;
                    _sequence++;
                    packet.Sequence = _sequence;
                    OnTelemetryPacket?.Invoke(this, packet);
                    _lastSuccessfulRead = DateTime.UtcNow;

                    if (!IsConnected)
                    {
                        IsConnected = true;
                        OnConnectionStateChanged?.Invoke(this, true);
                        Console.WriteLine($"[MudRunner File v5] Connected — index byte sync active (seq={_sequence})");
                    }
                }
            }
            catch (Exception ex)
            {
                LastError = $"Parse error: {ex.Message}";
                Console.WriteLine($"[MudRunner File v5] Parse error: {ex.Message}");
            }
        }

        /// <summary>
        /// Check if connection should be considered lost (no data for N seconds).
        /// </summary>
        private void CheckConnectionTimeout()
        {
            if (!IsConnected) return;
            if (_lastSuccessfulRead == DateTime.MinValue) return;

            var elapsed = (DateTime.UtcNow - _lastSuccessfulRead).TotalSeconds;
            if (elapsed > ConnectionTimeoutSec)
            {
                IsConnected = false;
                // Reset index so we re-detect when Lua starts writing again
                _lastIndex = NoDataIndex;
                OnConnectionStateChanged?.Invoke(this, false);
                Console.WriteLine($"[MudRunner File v5] No data for {ConnectionTimeoutSec}s — connection lost.");
            }
        }

        /// <summary>
        /// Try to detect the game's installation directory from the running process.
        /// </summary>
        private string DetectGameDirectory()
        {
            try
            {
                var processes = System.Diagnostics.Process.GetProcessesByName("MudRunner");
                if (processes.Length > 0)
                {
                    var mainModule = processes[0].MainModule;
                    if (mainModule != null && !string.IsNullOrEmpty(mainModule.FileName))
                    {
                        return Path.GetDirectoryName(mainModule.FileName);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MudRunner File v5] Process detection failed: {ex.Message}");
            }

            return null;
        }

        /// <summary>
        /// Manually set the game directory if auto-detection fails.
        /// Thread-safe: stops and restarts the poll loop.
        /// </summary>
        public void SetGameDirectory(string gameDir)
        {
            if (!string.IsNullOrEmpty(gameDir))
            {
                FullPath = Path.Combine(gameDir, _fileName);
                Console.WriteLine($"[MudRunner File v5] Game directory set: {gameDir} -> {FullPath}");
            }
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
