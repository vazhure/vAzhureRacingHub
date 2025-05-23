using System;
using System.Windows.Forms;

namespace MotionPlatform3
{
    public partial class ManualControlForm : Form
    {
        private readonly Plugin _plugin;
        private int _heaveUnits;
        private float _stepsPerMM;

        public ManualControlForm(Plugin plugin)
        {
            InitializeComponent();
            _plugin = plugin;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            _heaveUnits = (_plugin.FrontAxisState.max - _plugin.FrontAxisState.min) / 2;
            sliderHeave.Minimum = -(int)_heaveUnits;
            sliderHeave.Maximum = (int)_heaveUnits;

            float x = _plugin.settings.DistanceLeftRightMM;
            float y = _plugin.settings.DistanceFrontRearMM;
            float z = _plugin.settings.ActuatorTravelMM;

            // Maximum calculated rig angle forward
            float maxAngleY = (float)Math.Atan2(z, y) * 57.3f;
            // Maximum calculated rig angle sideway
            float maxAngleX = (float)Math.Atan2(z, x) * 57.3f;

            sliderPitch.Minimum = (int)(-maxAngleY*100f);
            sliderPitch.Maximum = (int)(maxAngleY * 100f);

            sliderRoll.Minimum = (int)(-maxAngleX * 100f);
            sliderRoll.Maximum= (int)(maxAngleX * 100f);

            _stepsPerMM = _plugin.settings.StepsPerMM;
            UpdateLabels();
        }

        protected override void OnClosed(EventArgs e)
        {
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
            float heave = sliderHeave.Value / (float)_heaveUnits;
            float pitch = sliderPitch.Value / (float)sliderPitch.Maximum;
            float roll = sliderRoll.Value / (float)sliderRoll.Maximum;

            UpdateLabels();
            DoMove(pitch, roll, heave);
        }

        private void UpdateLabels()
        {
            BeginInvoke((Action)delegate
                {
                    lblHeave.Text = $"{sliderHeave.Value / _stepsPerMM:0.0} mm";
                    lblPitch.Text = $"{0.01f * sliderPitch.Value:0.0}°";
                    lblRoll.Text = $"{0.01f * sliderRoll.Value:0.0}°";
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

                UpdateLabels();
            }
        }
    }
}
