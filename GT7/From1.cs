using System;
using System.Windows.Forms;

namespace GT7Telemetry
{
    public partial class Form1 : Form
    {
        private GT7TelemetryReceiver receiver;
        private System.Windows.Forms.Timer uiUpdateTimer;
        private GT7PacketA lastPacket;

        public Form1()
        {
            InitializeComponent();
            InitializeUI();
        }

        private void InitializeUI()
        {
            this.Text = "GT7 Telemetry Receiver";
            this.Size = new System.Drawing.Size(800, 600);
            
            // IP адрес PS
            var lblIP = new Label { Text = "PS IP:", Location = new System.Drawing.Point(10, 10), Width = 50 };
            var txtIP = new TextBox { Text = "192.168.1.100", Location = new System.Drawing.Point(60, 8), Width = 120 };
            
            // Кнопки
            var btnStart = new Button { Text = "Start", Location = new System.Drawing.Point(200, 5), Width = 80 };
            var btnStop = new Button { Text = "Stop", Location = new System.Drawing.Point(290, 5), Width = 80, Enabled = false };
            
            // Лог
            var txtLog = new TextBox 
            { 
                Multiline = true, 
                ScrollBars = ScrollBars.Vertical,
                Location = new System.Drawing.Point(10, 40),
                Size = new System.Drawing.Size(770, 200),
                ReadOnly = true,
                Font = new System.Drawing.Font("Consolas", 9)
            };
            
            // Данные телеметрии
            var lblTelemetry = new Label 
            { 
                Text = "Telemetry:", 
                Location = new System.Drawing.Point(10, 250),
                Font = new System.Drawing.Font("Microsoft Sans Serif", 10, System.Drawing.FontStyle.Bold)
            };
            
            var txtTelemetry = new TextBox
            {
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Location = new System.Drawing.Point(10, 275),
                Size = new System.Drawing.Size(770, 270),
                ReadOnly = true,
                Font = new System.Drawing.Font("Consolas", 10)
            };
            
            this.Controls.AddRange(new Control[] { lblIP, txtIP, btnStart, btnStop, txtLog, lblTelemetry, txtTelemetry });
            
            // Обработчики
            btnStart.Click += (s, e) =>
            {
                receiver = new GT7TelemetryReceiver(txtIP.Text.Trim());
                receiver.OnPacketReceived += (packet) => 
                {
                    lastPacket = packet;
                };
                receiver.OnLogMessage += (msg) => 
                {
                    this.Invoke(new Action(() => 
                    {
                        txtLog.AppendText(msg + Environment.NewLine);
                        txtLog.ScrollToCaret();
                    }));
                };
                receiver.OnError += (ex) => 
                {
                    this.Invoke(new Action(() => txtLog.AppendText($"ERROR: {ex.Message}{Environment.NewLine}")));
                };
                
                receiver.Start();
                btnStart.Enabled = false;
                btnStop.Enabled = true;
            };
            
            btnStop.Click += (s, e) =>
            {
                receiver?.Stop();
                receiver = null;
                btnStart.Enabled = true;
                btnStop.Enabled = false;
            };
            
            // Таймер обновления UI (30 FPS)
            uiUpdateTimer = new System.Windows.Forms.Timer { Interval = 33 };
            uiUpdateTimer.Tick += (s, e) =>
            {
                if (lastPacket.PacketId == 0) return;
                
                var p = lastPacket;
                txtTelemetry.Text = 
                    $"Packet ID: {p.PacketId}{Environment.NewLine}" +
                    $"Speed: {p.SpeedKmh:F1} km/h ({p.Speed:F1} m/s){Environment.NewLine}" +
                    $"RPM: {p.EngineRPM:F0}{Environment.NewLine}" +
                    $"Gear: {p.CurrentGear} (Suggested: {p.SuggestedGear}){Environment.NewLine}" +
                    $"Throttle: {p.Throttle}/255  Brake: {p.Brake}/255{Environment.NewLine}" +
                    $"Position: [{p.PositionX:F2}, {p.PositionY:F2}, {p.PositionZ:F2}]{Environment.NewLine}" +
                    $"Velocity: [{p.VelocityX:F2}, {p.VelocityY:F2}, {p.VelocityZ:F2}]{Environment.NewLine}" +
                    $"Lap: {p.LapCount}/{p.TotalLaps}{Environment.NewLine}" +
                    $"Best Lap: {FormatTime(p.BestLapTime)}  Last: {FormatTime(p.LastLapTime)}{Environment.NewLine}" +
                    $"Fuel: {p.FuelLevel:F2}/{p.FuelCapacity:F2} L{Environment.NewLine}" +
                    $"Boost: {p.Boost:F2}{Environment.NewLine}" +
                    $"Tyre Temp FL: {p.TyreTempFL:F1}°C  FR: {p.TyreTempFR:F1}°C{Environment.NewLine}" +
                    $"Tyre Temp RL: {p.TyreTempRL:F1}°C  RR: {p.TyreTempRR:F1}°C{Environment.NewLine}" +
                    $"Body Height: {p.BodyHeight:F3}{Environment.NewLine}" +
                    $"Clutch: {p.Clutch:F2}  Engagement: {p.ClutchEngagement:F2}{Environment.NewLine}" +
                    $"Flags: 0x{p.Flags:X8}{Environment.NewLine}" +
                    $"Car Code: {p.CarCode}";
            };
            uiUpdateTimer.Start();
        }

        private string FormatTime(int ms)
        {
            if (ms <= 0) return "--:--.---";
            var ts = TimeSpan.FromMilliseconds(ms);
            return $"{ts.Minutes:D2}:{ts.Seconds:D2}.{ts.Milliseconds:D3}";
        }
    }
}