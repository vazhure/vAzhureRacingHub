using System;
using System.ComponentModel;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using vAzhureRacingAPI;

namespace MotionPlatform3
{
    public partial class SettingsForm : MovableForm
    {
        readonly Plugin _plugin;

        readonly BackgroundWorker testWorker;
        readonly BackgroundWorker stateWorker;
        public SettingsForm(Plugin plugin)
        {
            _plugin = plugin;
            oldMode = plugin.settings.mode;
            InitializeComponent();

            testWorker = new BackgroundWorker() { WorkerReportsProgress = true, WorkerSupportsCancellation = true };
            testWorker.DoWork += TestWorker_DoWork;
            testWorker.RunWorkerCompleted += TestWorker_RunWorkerCompleted;

            stateWorker = new BackgroundWorker() { WorkerReportsProgress = true, WorkerSupportsCancellation = true };
            stateWorker.DoWork += delegate
            {
                while (!stateWorker.CancellationPending)
                {
                    try
                    {
                        if (_plugin.Status == DeviceStatus.Connected)
                            _plugin.RequestState();
                    }
                    catch { }
                    Thread.Sleep(100);
                }
            };
        }

        private void Plugin_OnDisconnected(object sender, EventArgs e)
        {
            Plugin_OnAxisStateChanged(_plugin, new Plugin.AxisStateChanged(10, _plugin.FrontAxisState));
            Plugin_OnAxisStateChanged(_plugin, new Plugin.AxisStateChanged(11, _plugin.RearLeftAxisState));
            Plugin_OnAxisStateChanged(_plugin, new Plugin.AxisStateChanged(12, _plugin.RearRightAxisState));
            Plugin_OnAxisStateChanged(_plugin, new Plugin.AxisStateChanged(13, _plugin.RearRightAxisState));
            if (testWorker.IsBusy)
                testWorker.CancelAsync();
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

            testWorker?.CancelAsync();
            stateWorker?.CancelAsync();
        }

        MODE oldMode = MODE.Run;

        private void TestWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            BeginInvoke((Action)delegate
            {
                btnTestSpeed.Text = "TEST";
                _plugin.settings.mode = oldMode;
            });
        }

        private void TestWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (sender is BackgroundWorker worker)
            {
                BeginInvoke((Action)delegate
                {
                    btnTestSpeed.Text = "STOP";
                });

                DateTime dtStart = DateTime.Now;

                while (!worker.CancellationPending)
                {
                    TimeSpan ts = DateTime.Now - dtStart;

                    double heave = 0.5 * Math.Sin(Math.PI * (ts.TotalMilliseconds / 2000));

                    try
                    {
                        if (_plugin.ReadyToSend)
                            _plugin.DoHeave((float)heave);
                    }
                    catch { worker.CancelAsync(); }

                    System.Threading.Thread.Sleep(10);
                }
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Plugin_OnAxisStateChanged(_plugin, new Plugin.AxisStateChanged(10, _plugin.FrontAxisState));
            Plugin_OnAxisStateChanged(_plugin, new Plugin.AxisStateChanged(11, _plugin.RearLeftAxisState));
            Plugin_OnAxisStateChanged(_plugin, new Plugin.AxisStateChanged(12, _plugin.RearRightAxisState));

            chkInvertHeave.Checked = _plugin.settings.Invert.HasFlag(MotionPlatformSettings.InvertFlags.InvertHeave);
            chkInvertRoll.Checked = _plugin.settings.Invert.HasFlag(MotionPlatformSettings.InvertFlags.InvertRoll);
            chkInvertPitch.Checked = _plugin.settings.Invert.HasFlag(MotionPlatformSettings.InvertFlags.InvertPitch);
            chkInvertSway.Checked = _plugin.settings.Invert.HasFlag(MotionPlatformSettings.InvertFlags.InvertSway);
            chkInvertSurge.Checked = _plugin.settings.Invert.HasFlag(MotionPlatformSettings.InvertFlags.InvertSurge);

            chkCollect.Checked = _plugin.settings.mode == MODE.CollectingGameData;
            sliderOveralEffects.Value = _plugin.settings.OveralCoefficient;
            sliderSmooth.Value = _plugin.settings.SmoothCoefficient;

            sliderSpeed.Minimum = _plugin.settings.MinSpeed;
            sliderSpeed.Maximum = _plugin.settings.MaxSpeed;

            sliderLinearity.Value = (int)Math.Ceiling(Math2.Mapf(_plugin.settings.Linearity, 1, 2, 100, 0));

            sliderSpeed.Value = _plugin.settings.SpeedOverride;
            lblSpeed.Text = $"{_plugin.settings.SpeedOverride} mm/sec";

            chkEnabled.Checked = _plugin.settings.Enabled;

            lblGame.Text = _plugin.Telemetry?.GamePlugin?.Name;

            InitComPorts();

            try
            {
                sliderHeave.Value = _plugin.settings.HeaveCoefficient;
                sliderSurge.Value = _plugin.settings.SurgeCoefficient;
                sliderSway.Value = _plugin.settings.SwayCoefficient;
                sliderPitch.Value = _plugin.settings.PitchCoefficient;
                sliderRoll.Value = _plugin.settings.RollCoefficient;
            }
            catch { }

            _plugin.OnAxisStateChanged += Plugin_OnAxisStateChanged;
            _plugin.OnDisconnected += Plugin_OnDisconnected;
            _plugin.App.OnDeviceArrival += Application_OnDeviceArrival;
            _plugin.App.OnDeviceRemoveComplete += Application_OnDeviceRemoveComplete;

            stateWorker.RunWorkerAsync();
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

                lblGame.Text = _plugin.Telemetry?.GamePlugin?.Name;
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

            _plugin.settings.Linearity = Math2.Mapf(sliderLinearity.Value, 100, 0, 1, 2);

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

        private void BtnResetData_Click(object sender, EventArgs e)
        {
            _plugin.ResetActiveGameData();
        }

        private void ChkEnabled_OnSwitch(object sender, EventArgs e)
        {
            _plugin.DeviceEnabled = chkEnabled.Checked;
        }

        private void ChkCollect_OnSwitch(object sender, EventArgs e)
        {
            _plugin.settings.mode = chkCollect.Checked ? MODE.CollectingGameData : MODE.Run;
        }

        private void BtnTestSpeed_Click(object sender, EventArgs e)
        {
            if (testWorker.IsBusy)
                testWorker.CancelAsync();
            else
            {
                oldMode = _plugin.settings.mode;
                _plugin.settings.mode = MODE.Test;
                testWorker.RunWorkerAsync();
            }
        }

        Label mouseControl = null;
        Point ptDown = new Point();

        private void OnLabelMouseDown(object sender, MouseEventArgs e)
        {
            if (_plugin.settings.mode != MODE.Test)
            {
                mouseControl = sender as Label;
                ptDown = e.Location;
                oldMode = _plugin.settings.mode;
                _plugin.settings.mode = MODE.Test;
            }
        }

        private void OnLabelMouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button.HasFlag(MouseButtons.Left))
            {
                if (mouseControl != null)
                {
                    float dX = Math2.Clamp(e.X - ptDown.X, -100, 100);
                    float dY = Math2.Clamp(e.Y - ptDown.Y, -100, 100);
                    float delta = (Math.Abs(dX) > Math.Abs(dY) ? dX : dY) / 100.0f;

                    switch (mouseControl.Name)
                    {
                        case "labelHeave":
                            _plugin.DoHeave(delta);
                            break;
                        case "labelPitch":
                            _plugin.DoPitch(delta);
                            break;
                        case "labelRoll":
                            _plugin.DoRoll(delta);
                            break;
                    }
                }
            }
            else
                mouseControl = null;
        }

        private void OnLabelMouseUp(object sender, MouseEventArgs e)
        {
            _plugin.settings.mode = oldMode;
            switch (mouseControl?.Name)
            {
                case "labelHeave":
                    _plugin.DoHeave(0);
                    break;
                case "labelPitch":
                    _plugin.DoPitch(0);
                    break;
                case "labelRoll":
                    _plugin.DoRoll(0);
                    break;
            }
            mouseControl = null;
        }
    }
}