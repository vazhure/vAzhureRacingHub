using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MotionPlatform3
{
    public partial class ManualControlForm : Form
    {
        private readonly Plugin _plugin;
        private int _heaveUnits;
        private float _stepsPerMM;

        readonly CancellationTokenSource cancellationToken = new CancellationTokenSource();
        Task task;

        public ManualControlForm(Plugin plugin)
        {
            InitializeComponent();
            _plugin = plugin;
        }

        volatile float heave = 0;
        volatile float pitch = 0;
        volatile float roll = 0;

        volatile float _heave = float.MaxValue;
        volatile float _pitch = float.MaxValue;
        volatile float _roll = float.MaxValue;

        private MODE _oldMode;

        PosFilter pitchFilter;
        PosFilter rollFilter;
        PosFilter heaveFilter;

        float minSpeedMM = 0.1f;
        float minSpeedAngularX = 0.1f;
        float minSpeedAngularY = 0.1f;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            _oldMode = _plugin.settings.mode;
            _plugin.settings.mode = MODE.Test;

            _heaveUnits = (_plugin.FrontAxisState.max - _plugin.FrontAxisState.min) / 2;
            sliderHeave.Minimum = -(int)_heaveUnits;
            sliderHeave.Maximum = (int)_heaveUnits;

            // Maximum calculated rig angle forward
            float maxAngleY = _plugin.settings.MaxPitchAngle * 57.3f;
            // Maximum calculated rig angle sideway
            float maxAngleX = _plugin.settings.MaxRollAngle * 57.3f;

            minSpeedMM = 20f / Math.Max(_plugin.settings.ActuatorTravelMM, 1);
            minSpeedAngularX = 2f / maxAngleX;
            minSpeedAngularY = 2f / maxAngleY;

            pitchFilter = new PosFilter(0, minSpeedAngularY);
            rollFilter = new PosFilter(0, minSpeedAngularX);
            heaveFilter = new PosFilter(0, minSpeedMM);

            sliderPitch.Minimum = (int)(-maxAngleY * 100f);
            sliderPitch.Maximum = (int)(maxAngleY * 100f);

            sliderRoll.Minimum = (int)(-maxAngleX * 100f);
            sliderRoll.Maximum = (int)(maxAngleX * 100f);

            _stepsPerMM = _plugin.settings.StepsPerMM;
            task = Task.Run(() =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        if (Math.Abs(pitch - _pitch) > float.Epsilon
                            || Math.Abs(roll - _roll) > float.Epsilon
                            || Math.Abs(heave - _heave) > float.Epsilon)
                        {
                            _pitch = (float)pitchFilter.UpdatePosition(pitch);  // [-1..1]
                            _roll = (float)rollFilter.UpdatePosition(roll);     // [-1..1]
                            _heave = (float)heaveFilter.UpdatePosition(heave);  // [-1..1]
                            UpdateLabels();
                            DoMove(_pitch, _roll, _heave);
                        }
                    }
                    catch { }
                    Thread.Sleep(15);
                }
            }, cancellationToken.Token);
            chkReducedSpeed.Checked = true;
        }

        protected override void OnClosed(EventArgs e)
        {
            cancellationToken.Cancel();
            task.Wait(100);

            _plugin.settings.mode = _oldMode;
            base.OnClosed(e);
            DoMove(0, 0, 0);
        }

        void DoMove(float pitch, float roll, float heave)
        {
            (float posFL, float posFR, float posRL, float posRR) = _plugin.CalculateLegPos(pitch, roll, heave);
            _plugin.Move((int)posFL, (int)posRL, (int)posRR, (int)posFR);
        }

        private void OnSlider(object sender, EventArgs e)
        {
            heave = sliderHeave.Value / (float)sliderHeave.Maximum;
            pitch = sliderPitch.Value / (float)sliderPitch.Maximum;
            roll = sliderRoll.Value / (float)sliderRoll.Maximum;
        }

        private void UpdateLabels()
        {
            BeginInvoke((Action)delegate
                {
                    lblHeave.Text = $"{_heave * sliderHeave.Maximum / _stepsPerMM:0.0} mm";
                    lblPitch.Text = $"{sliderPitch.Maximum * 0.01f * _pitch:0.0}°";
                    lblRoll.Text = $"{sliderPitch.Maximum * 0.01f * _roll:0.0}°";
                });
        }

        private void OnReset(object sender, EventArgs e)
        {
            if ((sender as Control).Tag is string name)
            {

                switch (name)
                {
                    case "Heave":
                        sliderHeave.Value = 0;
                        break;
                    case "Pitch":
                        sliderPitch.Value = 0;
                        break;
                    case "Roll":
                        sliderRoll.Value = 0;
                        break;
                }
            }
        }

        private void ChkReducedSpeed_OnSwitch(object sender, EventArgs e)
        {
            // Maximum calculated rig angle forward
            float maxAngleY = _plugin.settings.MaxPitchAngle * 57.3f;
            // Maximum calculated rig angle sideway
            float maxAngleX = _plugin.settings.MaxRollAngle * 57.3f;

            bool bReduced = chkReducedSpeed.Checked;
            pitchFilter.SetSpeed(bReduced ? minSpeedAngularY : _plugin.settings.OpenXRMotionCompensationAngularSpeed / maxAngleY);
            rollFilter.SetSpeed(bReduced ? minSpeedAngularX : _plugin.settings.OpenXRMotionCompensationAngularSpeed / maxAngleX);
            heaveFilter.SetSpeed(bReduced ? minSpeedMM : 2f * _plugin.settings.SpeedOverride / _plugin.settings.ActuatorTravelMM);
        }
    }
}