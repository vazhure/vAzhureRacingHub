using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using vAzhureRacingAPI;
using static System.Net.Mime.MediaTypeNames;

namespace MotionPlatform3
{
    public partial class SettingsForm : MovableForm
    {
        readonly Plugin _plugin;

        readonly CancellationTokenSource cancellationToken = new CancellationTokenSource();
        Task stateWorker;

        public SettingsForm(Plugin plugin)
        {
            _plugin = plugin;
            _plugin.App.OnGameStarted += App_OnGameStarted;
            _plugin.App.OnGameStopped += App_OnGameStopped;

            oldMode = plugin.settings.mode;
            InitializeComponent();
        }

        private void App_OnGameStopped(object sender, EventArgs e)
        {
            if (sender is IGamePlugin game)
            {
                BeginInvoke((Action)delegate
                {
                    lblGame.Text = "No active game";
                    chkCollect.Enabled = btnTune.Enabled = false;
                    Invalidate();
                });
            }
        }

        private void App_OnGameStarted(object sender, EventArgs e)
        {
            if (sender is IGamePlugin game)
            {
                BeginInvoke((Action)delegate
                {
                    lblGame.Text = game.Name;
                    chkCollect.Enabled = btnTune.Enabled = true;
                    Invalidate();
                });
            }
        }

        private void Plugin_OnDisconnected(object sender, EventArgs e)
        {
            Plugin_OnAxisStateChanged(_plugin, new Plugin.AxisStateChanged(10, _plugin.FrontAxisState));
            Plugin_OnAxisStateChanged(_plugin, new Plugin.AxisStateChanged(11, _plugin.RearLeftAxisState));
            Plugin_OnAxisStateChanged(_plugin, new Plugin.AxisStateChanged(12, _plugin.RearRightAxisState));
            Plugin_OnAxisStateChanged(_plugin, new Plugin.AxisStateChanged(13, _plugin.RearRightAxisState));
            Close();
        }

        private void Application_OnDeviceRemoveComplete(object sender, DeviceChangeEventsArgs e)
        {
            InitComPorts();
        }

        private void Application_OnDeviceArrival(object sender, DeviceChangeEventsArgs e)
        {
            InitComPorts();

            if (!_plugin.IsConnected && MessageBox.Show(this, "Use a connected device?", $"New Device ({e.Port})", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                if (comboComPort.FindString(e.Port) is int t && t > 0)
                {
                    comboComPort.SelectedIndex = t;

                    if (comboComPort.SelectedItem is string port && int.TryParse(port.ToUpper().Replace("COM", ""), out int p))
                    {
                        _plugin.settings.ComPort = p;
                        _plugin.ReConnect();
                    }
                }
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            _plugin.OnAxisStateChanged -= Plugin_OnAxisStateChanged;
            _plugin.OnDisconnected -= Plugin_OnDisconnected;
            _plugin.App.OnDeviceArrival -= Application_OnDeviceArrival;
            _plugin.App.OnDeviceRemoveComplete -= Application_OnDeviceRemoveComplete;
            _plugin.App.OnGameStarted -= App_OnGameStarted;
            _plugin.App.OnGameStopped -= App_OnGameStopped;

            cancellationToken.Cancel();
            stateWorker.Wait(100);
        }

        MODE oldMode = MODE.Run;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Plugin_OnAxisStateChanged(_plugin, new Plugin.AxisStateChanged(10, _plugin.FrontAxisState));
            Plugin_OnAxisStateChanged(_plugin, new Plugin.AxisStateChanged(11, _plugin.RearLeftAxisState));
            Plugin_OnAxisStateChanged(_plugin, new Plugin.AxisStateChanged(12, _plugin.RearRightAxisState));
            Plugin_OnAxisStateChanged(_plugin, new Plugin.AxisStateChanged(13, _plugin.FrontRightAxisState));

            chkInvertHeave.Checked = _plugin.settings.Invert.HasFlag(MotionPlatformSettings.InvertFlags.InvertHeave);
            chkInvertRoll.Checked = _plugin.settings.Invert.HasFlag(MotionPlatformSettings.InvertFlags.InvertRoll);
            chkInvertPitch.Checked = _plugin.settings.Invert.HasFlag(MotionPlatformSettings.InvertFlags.InvertPitch);
            chkInvertSway.Checked = _plugin.settings.Invert.HasFlag(MotionPlatformSettings.InvertFlags.InvertSway);
            chkInvertSurge.Checked = _plugin.settings.Invert.HasFlag(MotionPlatformSettings.InvertFlags.InvertSurge);
            chkMotionCompensation.Checked = _plugin.settings.OpenXRMotionCompensation;

            lblOpenXRStatus.ForeColor = MotionRigPose.IsOpenXR ? Color.LightGreen : Color.White;
            lblOpenXRStatus.Text = MotionRigPose.IsOpenXR ? "OpenXR Compensation is active" : "OpenXR Compensation is not active";

            bool bconfigured = MotionRigPose.IsOpenXRCompensationConfigured;
            lblOpenXRConfigured.ForeColor = bconfigured ? Color.LightGreen : Color.White;
            btnPathOpenXRConfig.Enabled = !bconfigured;
            lblOpenXRConfigured.Text = bconfigured ? "OpenXR Compensation is configured" : "OpenXR Compensation is not configured";

            chkCollect.Checked = _plugin.settings.mode == MODE.CollectingGameData;
            sliderOveralEffects.Value = _plugin.settings.OveralCoefficient;
            sliderSmooth.Value = _plugin.settings.SmoothCoefficient;

            sliderSpeed.Minimum = _plugin.settings.MinSpeed;
            sliderSpeed.Maximum = _plugin.settings.MaxSpeed;

            sliderSpeed.Value = _plugin.settings.SpeedOverride;
            lblSpeed.Text = $"{_plugin.settings.SpeedOverride} mm/sec";

            chkEnabled.Checked = _plugin.settings.Enabled;
            chkParkOnIdle.Checked = _plugin.settings.ParkOnIdle;
            chkParkOnQuit.Checked = _plugin.settings.ParkOnQuit;

            numFrontRearMM.Value = _plugin.settings.DistanceFrontRearMM;
            numLeftRightMM.Value = _plugin.settings.DistanceLeftRightMM;
            numActuatorTravelMM.Value = _plugin.settings.ActuatorTravelMM;
            numSeatOffsetMM.Value = _plugin.settings.SeatOffsetMM;

            IGamePlugin activeGame = _plugin.App.GamePlugins.Where(game => game.IsRunning).FirstOrDefault();
            lblGame.Text = activeGame == null ? "No active game" : activeGame.Name;
            chkCollect.Enabled = btnTune.Enabled = activeGame != null;

            InitComPorts();

            try
            {
                sliderHeave.Value = _plugin.settings.HeaveCoefficient;
                sliderSurge.Value = _plugin.settings.SurgeCoefficient;
                sliderSway.Value = _plugin.settings.SwayCoefficient;
                sliderPitch.Value = _plugin.settings.PitchCoefficient;
                sliderRoll.Value = _plugin.settings.RollCoefficient;
                sliderSmooth.Value = _plugin.settings.SmoothCoefficient;
                sliderGearEffect.Value = _plugin.settings.GearChangeEffect;
            }
            catch { }

            _plugin.OnAxisStateChanged += Plugin_OnAxisStateChanged;
            _plugin.OnDisconnected += Plugin_OnDisconnected;
            _plugin.App.OnDeviceArrival += Application_OnDeviceArrival;
            _plugin.App.OnDeviceRemoveComplete += Application_OnDeviceRemoveComplete;


            stateWorker = Task.Run(() =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        if (_plugin.Status == DeviceStatus.Connected)
                            _plugin.RequestState();
                    }
                    catch { }
                    Thread.Sleep(100);

                }
            }, cancellationToken.Token);
        }

        private void InitComPorts()
        {
            string[] ports = SerialPort.GetPortNames();

            comboComPort.Items.Clear();
            comboComPort.Items.Add("not available");
            if (ports.Length > 0)
            {
                comboComPort.Items.AddRange(ports);
                string portName = $"COM{_plugin.settings.ComPort}";
                if (comboComPort.Items.Contains(portName))
                    comboComPort.SelectedItem = portName;
                else
                    comboComPort.SelectedIndex = 0;
            }

            if (comboComPort.SelectedIndex < 0)
                comboComPort.SelectedIndex = 0;

            // TODO: Mark opened ports as disabled..
            // OnPaint...
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
        }

        private void Plugin_OnAxisStateChanged(object sender, Plugin.AxisStateChanged e)
        {
            BeginInvoke((Action)delegate
            {
                string text = "UNKNOWN STATE";
                Color color = BackColor;
                Color textClr = ForeColor;

                switch (e.State.mode)
                {
                    case DEVICE_MODE.CONNECTED: text = "CONNECTED"; color = Color.Green; break;
                    case DEVICE_MODE.ALARM: text = "ALARM"; color = Color.Red; break;
                    case DEVICE_MODE.HOMEING: text = "HOMEING"; color = Color.Yellow; textClr = Color.Black; break;
                    case DEVICE_MODE.READY: text = "READY"; color = Color.Green; break;
                    case DEVICE_MODE.PARKING: text = "PARKING"; color = Color.Green; break;
                }

                btnTestSpeed.Enabled = _plugin.IsDeviceReady;

                string state = $"Homed: {e.State.flags.HasFlag(DEVICE_FLAGS.STATE_HOMED)}\r\n{e.State.speedMMperSEC} mm/sec";
                float stepsPerMM = _plugin.settings.StepsPerMM;
                int mid = (e.State.min + e.State.max) / 2;
                string pos = $"POS: {(e.State.currentpos - mid) / stepsPerMM:0.0} mm";
                string target = $"TARGET: {(e.State.targetpos - mid) / stepsPerMM:0.0} mm";

                switch (e.Addr)
                {
                    case 10:
                        {
                            lblFrontState.Text = text;
                            lblFrontState.BackColor = color;
                            lblFrontState.ForeColor = textClr;
                            lblPosFront.Text = pos;
                            lblTargetFront.Text = target;
                            toolTips.SetToolTip(pbFR, state);
                        }
                        break;
                    case 11:
                        {
                            lblRearLeftState.Text = text;
                            lblRearLeftState.BackColor = color;
                            lblRearLeftState.ForeColor = textClr;
                            lblPosRL.Text = pos;
                            lblTargetRL.Text = target;
                            toolTips.SetToolTip(pbRL, state);
                        }
                        break;
                    case 12:
                        {
                            lblRearRightState.Text = text;
                            lblRearRightState.BackColor = color;
                            lblRearRightState.ForeColor = textClr;
                            lblPosRR.Text = pos;
                            lblTargetRR.Text = target;
                            toolTips.SetToolTip(pbRR, state);
                        }
                        break;
                    case 13:
                        {
                            lblFrontRightState.Text = text;
                            lblFrontRightState.BackColor = color;
                            lblFrontRightState.ForeColor = textClr;
                            lblPosFRR.Text = pos;
                            lblTargetFRR.Text = target;
                            toolTips.SetToolTip(pbFRR, state);
                        }
                        break;
                }
            });
        }

        private void BtnHome_Click(object sender, EventArgs e)
        {
            _plugin.Home();
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void ButtonPark_Click(object sender, EventArgs e)
        {
            _plugin.Park();
        }

        private void ButtonApply_Click(object sender, EventArgs e)
        {
            int speed = sliderSpeed.Value;
            _plugin.SetSpeed(speed);
            _plugin.settings.SpeedOverride = speed;

            _plugin.settings.Invert = (chkInvertHeave.Checked ? MotionPlatformSettings.InvertFlags.InvertHeave : MotionPlatformSettings.InvertFlags.None) |
                (chkInvertSurge.Checked ? MotionPlatformSettings.InvertFlags.InvertSurge : MotionPlatformSettings.InvertFlags.None) |
                (chkInvertSway.Checked ? MotionPlatformSettings.InvertFlags.InvertSway : MotionPlatformSettings.InvertFlags.None) |
                (chkInvertRoll.Checked ? MotionPlatformSettings.InvertFlags.InvertRoll : MotionPlatformSettings.InvertFlags.None) |
                (chkInvertPitch.Checked ? MotionPlatformSettings.InvertFlags.InvertPitch : MotionPlatformSettings.InvertFlags.None);

            _plugin.settings.HeaveCoefficient = sliderHeave.Value;
            _plugin.settings.SurgeCoefficient = sliderSurge.Value;
            _plugin.settings.SwayCoefficient = sliderSway.Value;
            _plugin.settings.PitchCoefficient = sliderPitch.Value;
            _plugin.settings.RollCoefficient = sliderRoll.Value;

            _plugin.settings.OveralCoefficient = sliderOveralEffects.Value;
            _plugin.settings.SmoothCoefficient = sliderSmooth.Value;

            _plugin.settings.ParkOnIdle = chkParkOnIdle.Checked;
            _plugin.settings.ParkOnQuit = chkParkOnQuit.Checked;

            _plugin.settings.GearChangeEffect = sliderGearEffect.Value;

            _plugin.settings.DistanceFrontRearMM = (int)numFrontRearMM.Value;
            _plugin.settings.DistanceLeftRightMM = (int)numLeftRightMM.Value;

            _plugin.settings.ActuatorTravelMM = (int)numActuatorTravelMM.Value;
            _plugin.settings.SeatOffsetMM = (int)numSeatOffsetMM.Value;

            if (comboComPort.SelectedItem is string port)
            {
                try
                {
                    int p = int.Parse(port.ToUpper().Replace("COM", ""));
                    _plugin.ComPort = p;
                }
                catch
                {
                    _plugin.ComPort = 0;
                }
            }
        }

        private void SliderControl_OnValueChanged(object sender, EventArgs e)
        {
            if (sender is VAzhureSliderControl slider)
            {
                string lblName = slider.Name.Replace("slider", "lbl");
                if (Controls.Find(lblName, false).FirstOrDefault() is Label lbl)
                    lbl.Text = $"{slider.Value}%";
            }
        }

        private void BtnAlarmReset_Click(object sender, EventArgs e)
        {
            _plugin.AlarmReset();
        }

        private void SliderSpeed_OnValueChanged(object sender, EventArgs e)
        {
            if (sender is VAzhureSliderControl slider)
            {
                lblSpeed.Text = $"{slider.Value:0} mm/sec";
            }
        }

        private void BtnTune_Click(object sender, EventArgs e)
        {
            using (GameTuneDlg dlg = new GameTuneDlg(_plugin.ActiveGameData))
            {
                dlg.ShowDialog(this);
            }
        }

        private void ChkEnabled_OnSwitch(object sender, EventArgs e)
        {
            _plugin.DeviceEnabled = chkEnabled.Checked;
        }

        private void ChkCollect_OnSwitch(object sender, EventArgs e)
        {
            if (chkCollect.Checked)
            {
                if (_plugin.App.GamePlugins.Where(game => game.IsRunning).FirstOrDefault() is IGamePlugin)
                {
                    if (MessageBox.Show("Reset telemetry data and collect new data?", "Telemetry", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        _plugin.ResetActiveGameData();
                }
                _plugin.settings.mode = MODE.CollectingGameData;
            }
            else
                _plugin.settings.mode = MODE.Run;
        }

        private void BtnTestSpeed_Click(object sender, EventArgs e)
        {
            using (ManualControlForm form = new ManualControlForm(_plugin))
            {
                form.ShowDialog(this);
            }

            //if (testWorker.IsBusy)
            //    testWorker.CancelAsync();
            //else
            //{
            //    oldMode = _plugin.settings.mode;
            //    _plugin.settings.mode = MODE.Test;
            //    testWorker.RunWorkerAsync();
            //}
        }

        private void BtnSpeedSettings_Click(object sender, EventArgs e)
        {
            using (SpeedSettings settings = new SpeedSettings(_plugin))
            {
                settings.ShowDialog(this);

                if (_plugin.settings.MinSpeed != sliderSpeed.Minimum)
                {
                    sliderSpeed.Minimum = _plugin.settings.MinSpeed;
                }

                if (_plugin.settings.MaxSpeed != sliderSpeed.Maximum)
                {
                    sliderSpeed.Maximum = _plugin.settings.MaxSpeed;
                }
            }
        }

        string FormatSimHubCommand(byte[] cmd)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in cmd)
                sb.Append($"<0x{b:X2}>");
            return sb.ToString();
        }

        private void BtnSimHub_Click(object sender, EventArgs e)
        {
            SimHub.Shmotioncontroller motionsettings = new SimHub.Shmotioncontroller();

            int acc = _plugin.settings.Acceleration;
            int low = _plugin.settings.LowSpeedOverride;
            int speed = _plugin.settings.SpeedOverride;
            motionsettings.Output.Settings.SerialPort = $"COM{_plugin.ComPort}";
            motionsettings.Output.Settings.BaudRate = _plugin.settings.BaudRate;
            motionsettings.Output.Settings.RtsEnable = _plugin.settings.RtsEnable;
            motionsettings.Output.Settings.DataBits = _plugin.settings.DataBits;
            motionsettings.Output.Settings.DtrEnable = _plugin.settings.DtrEnable;

            string sAcc = FormatSimHubCommand(_plugin.GenerateCommand(COMMAND.CMD_SET_ACCEL, acc, acc, acc, acc));
            string sSpeedLow = FormatSimHubCommand(_plugin.GenerateCommand(COMMAND.CMD_SET_LOW_SPEED, low, low, low, low));
            string sSpeed = FormatSimHubCommand(_plugin.GenerateCommand(COMMAND.CMD_SET_SPEED, speed, speed, speed, speed));
            string sHome = FormatSimHubCommand(_plugin.GenerateCommand(COMMAND.CMD_HOME, 1, 1, 1, 1));
            string sState = FormatSimHubCommand(_plugin.GenerateCommand(COMMAND.CMD_GET_STATE, 1, 1, 1, 1));

            motionsettings.Output.Settings.GenericProtocolDefinition.StartCommands = new SimHub.StartCommand[]
            {
                new SimHub.StartCommand() { Command = sAcc },
                new SimHub.StartCommand() { Command = sSpeedLow },
                new SimHub.StartCommand() { Command = sSpeed },
                new SimHub.StartCommand() { Command = sHome, CommandDelay = 3000 },
                new SimHub.StartCommand() { Command = sState }
            };

            using (SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                Filter = "SimHub Controller Settings (*.shmotioncontroller)|*.shmotioncontroller",
                FilterIndex = 1,
                RestoreDirectory = true,
                OverwritePrompt = true,
            })
            {
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        string jsonNew = ObjectSerializeHelper.GetJson(motionsettings).Replace(@"\u003c", "<").Replace(@"\u003e", ">").Replace(@"\u003C", "<").Replace(@"\u003E", ">");
                        File.WriteAllText(saveFileDialog.FileName, jsonNew);
                    }
                    catch { }
                }
            }
        }

        private void OnMotionCompensation(object sender, EventArgs e)
        {
            _plugin.settings.OpenXRMotionCompensation = chkMotionCompensation.Checked;
        }

        private void BtnPathOpenXRConfig(object sender, EventArgs e)
        {
            bool bconf = MotionRigPose.PatchOpenXRConfig();
            lblOpenXRConfigured.ForeColor = bconf ? Color.LightGreen : Color.White;
            btnPathOpenXRConfig.Enabled = !bconf;
        }

        private void OnDownloadOpenXRCompensation(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/BuzzteeBear/OpenXR-MotionCompensation/releases/latest");
        }
    }

    public static class SimHub
    {
        public class Shmotioncontroller
        {
            public Output Output { get; set; } = new Output();
        }

        public class Output
        {
            public string Comments { get; set; } = null;
            public string CustomName { get; set; } = "vAzhureRacing 3DOF";
            public Settings Settings { get; set; } = new Settings();
            public string OutputId { get; set; } = "9ccfc266-9843-4b74-a38c-cb3b298943cd";
            public string TypeName { get; set; } = "GenericSerialOutputV2";
        }

        public class SettingsBuilder
        {
            public Settings[] Settings { get; set; } = { };
            public bool IsEditMode { get; set; } = false;
        }

        public class GenericProtocolDefinition
        {
            public SettingsBuilder SettingsBuilder { get; set; } = new SettingsBuilder();
            public bool ShowWaitForResponse { get; set; } = true;
            public int AxisResolution { get; set; } = 16;
            public int AxisFormat { get; set; } = 1;
            public StartCommand[] StartCommands { get; set; } = new StartCommand[] { };
            public UpdateCommand[] UpdateCommands { get; set; } = new UpdateCommand[] { new UpdateCommand() };
            public StartCommand[] StopCommands { get; set; } = new StartCommand[] { new StartCommand() { Command = "<0x00><0x14><0x07><0x00><0x01><0x00><0x00><0x00><0x01><0x00><0x00><0x00><0x01><0x00><0x00><0x00><0x01><0x00><0x00><0x00>" } };
        }

        public class ActuatorOrderingSettings
        {
            public int MaxActuatorsEx { get; set; } = 4;
            public bool ConfigurationDone { get; set; } = true;
            public bool UseParkPosition { get; set; } = false;
            public int ParkDuration { get; set; } = 5516;
            public bool UseParkPositionEx { get; set; } = false;
            public ActuatorRole[] Roles { get; set; } = new ActuatorRole[] { new ActuatorRole() { Role = 1}, new ActuatorRole() { Role = 3 }, new ActuatorRole() { Role = 4 }, new ActuatorRole() { Role = 2 } };
        }

        public class ActuatorRole
        {
            public float RangeLimit { get; set; } = 100.0f;
            public float ParkPosition { get; set; } = 50.0f;
            public int Role { get; set; } = 1;
            public bool ReverseDirection { get; set; } = false;
        }

        public class StartCommand
        {
            public bool MustWaitForMessage { get; set; } = false;
            public string WaitForMessage { get; set; } = null;
            public int WaitForDelay { get; set; } = 5000;
            public string Command { get; set; } = "<0x00><0x14><0x02><0x00><0x78><0x00><0x00><0x00><0x78><0x00><0x00><0x00><0x78><0x00><0x00><0x00><0x78><0x00><0x00><0x00>";
            public int CommandDelay { get; set; } = 100;
        }

        public class UpdateCommand
        {
            public string Command { get; set; } = "<0x00><0x14><0x0B><0x00><Axis1><Axis2><Axis3><Axis4><0x00><0x00><0x00><0x00><0x00><0x00><0x00><0x00>";
            public int CommandDelay { get; set; } = 5;
        }

        public class Settings
        {
            public bool RtsEnable { get; set; } = true;
            public bool DtrEnable { get; set; } = true;
            public bool LockSettings { get; set; } = false;
            public string SerialPort { get; set; } = "COM9";
            public int DataBits { get; set; } = 8;
            public int SerialStopBits { get; set; } = 1;
            public int SerialParity { get; set; } = 0;
            public int BaudRate { get; set; } = 115200;
            public int AfterOpenDelay { get; set; } = 0;
            public GenericProtocolDefinition GenericProtocolDefinition { get; set; } = new GenericProtocolDefinition();
            public bool SecurityAcknowledgementAccepted { get; set; } = true;
            public ActuatorOrderingSettings ActuatorOrderingSettings { get; set; } = new ActuatorOrderingSettings();

            public string OutputId { get; set; } = "d55bc933-4451-49f5-8111-4b56e12f285b";
            public bool AllowIdling { get; set; } = true;

        }
    }
}