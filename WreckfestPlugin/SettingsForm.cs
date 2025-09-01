using System;
using System.Windows.Forms;

namespace WreckfestPlugin
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            playerComboBox.SelectedItem = playerComboBox.Items[0];
        }

        public string PlayerIdx => playerComboBox.SelectedItem == null? "00" : playerComboBox.SelectedItem.ToString();
    }
}