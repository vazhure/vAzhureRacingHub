using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MotionPlatform3
{
    public partial class SpeedSettings : Form
    {
        readonly Plugin _plugin;

        public SpeedSettings(Plugin plugin)
        {
            _plugin = plugin;
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            numAcceleration.Value = _plugin.settings.Acceleration;
            numMinSpeed.Value = _plugin.settings.MinSpeed;
            numSlowSpeed.Value = _plugin.settings.LowSpeedOverride;
            numMaxSpeed.Value = _plugin.settings.MaxSpeed;
            btnApply.Enabled = false;
        }

        private void OnValueChanged(object sender, EventArgs e)
        {
            btnApply.Enabled = numAcceleration.Value != _plugin.settings.Acceleration |
                numMinSpeed.Value != _plugin.settings.MinSpeed |
            numSlowSpeed.Value != _plugin.settings.LowSpeedOverride|
            numMaxSpeed.Value != _plugin.settings.MaxSpeed;
        }

        private void BtnApply_Click(object sender, EventArgs e)
        {
            if (_plugin.settings.Acceleration != (int)numAcceleration.Value)
            {
                _plugin.settings.Acceleration = (int)numAcceleration.Value;
                _plugin.SetAcceleration(_plugin.settings.Acceleration);
            }
            
            if (_plugin.settings.LowSpeedOverride != (int)numSlowSpeed.Value)
            {
                _plugin.settings.LowSpeedOverride = (int)numSlowSpeed.Value;
                _plugin.SetSpeed(_plugin.settings.MinSpeed, true);
            }

            _plugin.settings.MinSpeed = (int)numMinSpeed.Value;
            _plugin.settings.MaxSpeed = (int)numMaxSpeed.Value;

            btnApply.Enabled = false;
        }
    }
}
