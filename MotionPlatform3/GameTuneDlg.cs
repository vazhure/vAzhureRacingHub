using System;
using System.Linq;
using System.Windows.Forms;
using vAzhureRacingAPI;

namespace MotionPlatform3
{
    public partial class GameTuneDlg : MovableForm
    {
        readonly GameData _gd;
        public GameTuneDlg(GameData data)
        {
            InitializeComponent();
            _gd = data;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            numHeaveMax.Value = (decimal)Math.Abs(Math.Max(_gd.minHeave, _gd.maxHeave));
            numSurgeMax.Value = (decimal)Math.Abs(Math.Max(_gd.minSurge, _gd.maxSurge));
            numSwayMax.Value = (decimal)Math.Abs(Math.Max(_gd.minSway, _gd.maxSway));
            numPitchMax.Value = (decimal)Math.Abs(Math.Max(_gd.minPitch, _gd.maxPitch));
            numRollMax.Value = (decimal)Math.Abs(Math.Max(_gd.minRoll, _gd.maxRoll));
            numYawMax.Value = (decimal)Math.Abs(Math.Max(_gd.minYaw, _gd.maxYaw));

            numHeaveOffset.Value = (decimal)_gd.offsetHeave;
            numSurgeOffset.Value = (decimal)_gd.offsetSurge;
            numSwayOffset.Value = (decimal)_gd.offsetSway;
            numPitchOffset.Value = (decimal)_gd.offsetPitch;
            numRollOffset.Value = (decimal)_gd.offsetRoll;
            numYawOffset.Value = (decimal)_gd.offsetYaw;
        }

        private void BtnApply_Click(object sender, EventArgs e)
        {
            _gd.offsetHeave = (float)numHeaveOffset.Value;
            _gd.offsetSurge = (float)numSurgeOffset.Value;
            _gd.offsetSway = (float)numSwayOffset.Value;
            _gd.offsetPitch = (float)numPitchOffset.Value;
            _gd.offsetRoll = (float)numRollOffset.Value;
            _gd.offsetYaw = (float)numYawOffset.Value;

            _gd.minHeave = -(float)numHeaveMax.Value;
            _gd.maxHeave = (float)numHeaveMax.Value;
            _gd.minSurge = -(float)numSurgeMax.Value;
            _gd.maxSurge = (float)numSurgeMax.Value;
            _gd.minSway = -(float)numSwayMax.Value;
            _gd.maxSway = (float)numSwayMax.Value;
            _gd.minPitch = -(float)numPitchMax.Value;
            _gd.maxPitch = (float)numPitchMax.Value;
            _gd.minRoll = -(float)numRollMax.Value;
            _gd.maxRoll = (float)numRollMax.Value;
            _gd.minYaw = -(float)numYawMax.Value;
            _gd.maxYaw = (float)numYawMax.Value;
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Button6_Click(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.Tag is string cmd)
            {
                switch (cmd)
                {
                    case "All":
                        {
                            string[] names = { "Pitch", "Roll", "Yaw", "Sway", "Surge", "Heave" };
                            foreach (string name in names)
                            {
                                if (Controls.Find($"num{name}Max", false).FirstOrDefault() is NumericUpDown numMax)
                                    numMax.Value = 1;
                                if (Controls.Find($"num{name}Offset", false).FirstOrDefault() is NumericUpDown numOffset)
                                    numOffset.Value = 0;
                            }
                        }
                        break;
                    default:
                        {
                            if (Controls.Find($"num{cmd}Max", false).FirstOrDefault() is NumericUpDown numMax)
                                numMax.Value = 1;
                            if (Controls.Find($"num{cmd}Offset", false).FirstOrDefault() is NumericUpDown numOffset)
                                numOffset.Value = 0;
                        }
                        break;
                }
            }
        }
    }
}