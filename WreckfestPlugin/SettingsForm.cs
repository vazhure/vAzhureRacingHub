using System;
using System.Windows.Forms;

namespace WreckfestPlugin
{
    public partial class SettingsForm : Form
    {
        static bool bMultiplayer = false;
        public SettingsForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            playerComboBox.SelectedItem = playerComboBox.Items[0];
            playerComboBox.Enabled = bMultiplayer == true;
        }

        public string PlayerIdx => playerComboBox.SelectedItem == null? "00" : playerComboBox.SelectedItem.ToString();

        private void CbMulti_CheckStateChanged(object sender, EventArgs e)
        {
            playerComboBox.Enabled = bMultiplayer = cbMulti.Checked;
            if (!cbMulti.Checked)
            {
                playerComboBox.SelectedItem = playerComboBox.Items[0];
            }
        }
    }
}