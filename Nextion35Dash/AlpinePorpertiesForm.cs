using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Windows.Forms;
using vAzhureRacingAPI;

namespace Nextion35Dash
{
    public partial class AlpinePorpertiesForm : MovableForm
    {
        readonly AlpineWheelPlateDevice nextion;

        readonly DeviceSettings oldSettings;

        readonly IVAzhureRacingApp application = null;

        readonly string [] BaudRates = new string[]{"115200",
            "57600",
            "38400",
            "19200",
            "14400",
            "9600",
            "4800",
            "2400",
            "1200",
            "600",
            "300"};

        public AlpinePorpertiesForm(AlpineWheelPlateDevice device, IVAzhureRacingApp app) : base()
        {
            InitializeComponent();
            ShowInTaskbar = false;
            nextion = device;
            application = app;
            oldSettings = device.Settings.Clone() as DeviceSettings;
            application.OnDeviceArrival += Application_OnDeviceArrival;
            application.OnDeviceRemoveComplete += Application_OnDeviceRemoveComplete;
            treeProfiles.ItemHeight = 24;
            oldLeds = nextion?.customLeds.ToList();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (nextion.Settings.Pages?.Count() > 0)
            {
                comboPage.Items.Clear();
                comboPage.Items.AddRange(nextion.Settings.Pages);
            }
            comboPage.SelectedItem = nextion.Settings.DefaultPage;
            chkMainDevice.Checked = nextion.Settings.PrimaryDevice;
            InitPresets();
        }

        readonly Dictionary<string, int> image_map = new Dictionary<string, int>();

        readonly List<CustomLedsA> oldLeds;

        void AddVechicle(CustomLedsA leds)
        {
            TreeNode node;

            if (treeProfiles.Nodes.ContainsKey(leds.Game))
                node = treeProfiles.Nodes[leds.Game];
            else
            {
                image_map.TryGetValue(leds.Game, out int idx);
                node = treeProfiles.Nodes.Add(leds.Game, leds.Game);
                node.SelectedImageIndex = node.ImageIndex = idx;
            }

            TreeNode vechicle = node.Nodes.Add(leds.Vechicle, leds.Vechicle);
            vechicle.SelectedImageIndex = vechicle.ImageIndex = node.ImageIndex;
            vechicle.Tag = leds;

            if (treeProfiles.SelectedNode == null && leds.Vechicle == nextion.m_dataSet.CarData.CarName && leds.Game == nextion.m_dataSet.GamePlugin.Name)
            {
                treeProfiles.SelectedNode = vechicle;
            }
        }

        private void InitPresets()
        {
            if (treeProfiles.ImageList == null)
            {
                ImageList imageList = new ImageList() { ImageSize = new System.Drawing.Size(16, 16), ColorDepth = ColorDepth.Depth32Bit };

                imageList.Images.Add(Properties.Resources.user16);

                int idx = 1;
                foreach (var gamePlugin in Nextion35Plugin.sApp.GamePlugins)
                {
                    imageList.Images.Add(gamePlugin.GetIcon());
                    image_map[gamePlugin.Name] = idx++;

                }
                treeProfiles.ImageList = imageList;
            }

            treeProfiles.Nodes.Clear();

            TreeNode user = treeProfiles.Nodes.Add("User Profiles");
            user.SelectedImageIndex = user.ImageIndex = 0;

            foreach (CustomLedsA leds in nextion?.customLeds)
            {
                AddVechicle(leds);
            }

            comboPresets.SelectedIndex = nextion.Settings.PresetMode == PresetMode.Auto ? 0 : 1;

            TreeProfiles_AfterSelect(treeProfiles, new TreeViewEventArgs(null));
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            application.OnDeviceArrival -= Application_OnDeviceArrival;
            application.OnDeviceRemoveComplete -= Application_OnDeviceRemoveComplete;
            treeProfiles.ImageList.Dispose();
        }

        private void Application_OnDeviceRemoveComplete(object sender, DeviceChangeEventsArgs e)
        {
            InitComPorts();
        }

        private void Application_OnDeviceArrival(object sender, DeviceChangeEventsArgs e)
        {
            InitComPorts();

            if (MessageBox.Show(this, "Use Device?", $"New Device detected ({e.Port})", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                if (comboComPort.FindString(e.Port) is int t && t > 0)
                {
                    comboComPort.SelectedIndex = t;

                    if (comboComPort.SelectedItem is string port && int.TryParse(port.ToUpper().Replace("COM", ""), out int p))
                    {
                        nextion.Settings.ComPort = p;
                        nextion.ReConnect();
                    }
                }
            }
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            InitComPorts();

            UpdateImage();

            chkEnabled.Checked = nextion.Settings.Enabled;
            chkEnableLeds.Checked = nextion.Settings.EnableLeds;
            sliderLedBrightness.Value = Math2.Clamp(oldSettings.LedBrightness, sliderLedBrightness.Minimum, sliderLedBrightness.Maximum);
            lblLedBrightness.Text = $"{sliderLedBrightness.Value}";

            try
            {
                comboSpeedUnits.SelectedIndex = (int)nextion.Settings.SpeedUnits;
            }
            catch
            {
                comboSpeedUnits.SelectedIndex = 0;
            }

            try
            {
                comboPressureUnits.SelectedIndex = (int)nextion.Settings.PressureUnits;
            }
            catch
            {
                comboPressureUnits.SelectedIndex = 0;
            }
        }

        private void UpdateImage()
        {
            //pictureBox1.Image = nextion.Settings.Enabled ? nextion.Settings.EnableLeds ? Properties.Resources.NextionDash35 : Properties.Resources.NextionDash35_noLeds
            //    : Properties.Resources.NextionDash35_off;
        }

        private void InitComPorts()
        {
            string[] ports = SerialPort.GetPortNames();

            comboComPort.Items.Clear();
            comboComPort.Items.Add("Not connected");
            if (ports.Length > 0)
            {
                comboComPort.Items.AddRange(ports);
                string portName = $"COM{nextion.Settings.ComPort}";
                if (comboComPort.Items.Contains(portName))
                    comboComPort.SelectedItem = portName;
                else
                    comboComPort.SelectedIndex = 0;
            }

            if (comboComPort.SelectedIndex < 0)
                comboComPort.SelectedIndex = 0;

            string baudRate = nextion.Settings.BaudRate.ToString();
            comboBaudRate.Items.Clear();
            comboBaudRate.Items.AddRange(BaudRates);
            if (comboBaudRate.Items.Contains(baudRate))
                comboBaudRate.SelectedItem = baudRate;
        }

        private void ChkEnabled_OnSwitch(object sender, EventArgs e)
        {
            nextion.Settings.Enabled = chkEnabled.Checked;
            UpdateImage();
        }

        private void ChkEnableLeds_OnSwitch(object sender, EventArgs e)
        {
            sliderLedBrightness.Enabled = nextion.Settings.EnableLeds = chkEnableLeds.Checked;
            UpdateImage();
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            nextion.Settings = oldSettings;
            nextion.customLeds = oldLeds;
        }

        private void SliderLedBrightness_OnValueChanged(object sender, EventArgs e)
        {
            if (nextion.IsConnected)
            {
                nextion.Settings.LedBrightness = sliderLedBrightness.Value;
                if (sliderLedBrightness.Capture)
                    BeginInvoke((Action)delegate () { nextion.TestBrightness(); });
            }

            BeginInvoke((Action)delegate () { lblLedBrightness.Text = $"{sliderLedBrightness.Value}"; });
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            oldSettings.Enabled = chkEnabled.Checked;
            oldSettings.EnableLeds = chkEnableLeds.Checked;
            oldSettings.LedBrightness = sliderLedBrightness.Value;
            oldSettings.PresetMode = comboPresets.SelectedIndex == 0 ? PresetMode.Auto : PresetMode.Manual;
            oldSettings.DefaultPage = comboPage.SelectedItem as string ?? oldSettings.DefaultPage;
            oldSettings.SpeedUnits = (DeviceSettings.SpeedUnitsEnum) comboSpeedUnits.SelectedIndex;
            oldSettings.PressureUnits = (DeviceSettings.PressureUnitsEnum)comboPressureUnits.SelectedIndex;
            oldSettings.PrimaryDevice = chkMainDevice.Checked;

            if (comboComPort.SelectedItem is string port)
            {
                try
                {
                    int p = int.Parse(port.ToUpper().Replace("COM", ""));
                    oldSettings.ComPort = p;
                }
                catch 
                {
                    oldSettings.ComPort = 0;
                }
            }

            if (comboBaudRate.SelectedItem is string baudrate)
            {
                try
                {
                    int b = int.Parse(baudrate);
                    oldSettings.BaudRate = b;
                }
                catch { }
            }

            nextion.Settings = oldSettings;
        }

        private void SliderLedBrightness_MouseUp(object sender, MouseEventArgs e)
        {
            if (nextion.IsConnected)
            {
                nextion.Settings.LedBrightness = sliderLedBrightness.Value;
                BeginInvoke((Action)delegate () { nextion.TestBrightness(false); });
            }
        }

        private void ComboPresets_SelectedIndexChanged(object sender, EventArgs e)
        {
            nextion.Settings.PresetMode = comboPresets.SelectedIndex == 0 ? PresetMode.Auto : PresetMode.Manual;
        }

        private void TreeProfiles_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            EditNode(e.Node);
        }

        Keys keyDown = Keys.None;

        private void TreeProfiles_KeyUp(object sender, KeyEventArgs e)
        {
            if (keyDown == Keys.None)
                return;

            if (e.KeyCode == Keys.Delete)
            {
                if (sender is TreeView tree && tree.SelectedNode is TreeNode node)
                {
                    DeleteNode(node);
                }
            }    
            
            if (e.KeyCode == Keys.Enter)
            {
                if (sender is TreeView tree && tree.SelectedNode is TreeNode node)
                {
                    if (node.Nodes.Count > 0)
                    {
                        if (node.IsExpanded)
                            node.Collapse();
                        else
                            node.Expand();
                    }
                    else
                        EditNode(node);
                }
            }

            keyDown = Keys.None;
        }

        private void EditNode(TreeNode node)
        {
            //if (node?.Tag is CustomLeds leds)
            //{
            //    using (LedsEditor frmEdit = new LedsEditor(nextion, ref leds))
            //    {
            //        frmEdit.ShowDialog(this);
            //    }
            //}
        }

        private void DeleteNode(TreeNode node)
        {
            if (node.ImageIndex == 0 && node.Parent == null)
            {
                MessageBox.Show(this, "Prohibited!");
            }
            else
            {
                string msg = node.Nodes.Count > 0 ? $"Remove all Profiles for {node.Text}?" : $"Remove Profile {node.Text}?";
                if (MessageBox.Show(this, msg, "Profiles", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    if (node.Tag is CustomLedsA leds)
                    {
                        nextion.customLeds.Remove(leds);
                    }
                    else
                        foreach (TreeNode n in node.Nodes)
                        {
                            if (n.Tag is CustomLedsA led)
                                nextion.customLeds.Remove(led);
                        }
                    node.Remove();
                }
            }
        }

        private void TreeProfiles_KeyDown(object sender, KeyEventArgs e)
        {
            keyDown = e.KeyCode;
        }

        private void TreeProfiles_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node is TreeNode node)
            {
                btnEdit.Enabled = node?.Tag != null;
                btnDelete.Enabled = node?.ImageIndex != 0;
                toolTips.SetToolTip(btnDelete, node?.Tag != null ? "Remove Preset" : "Remove Game Presets");
                toolTips.SetToolTip(btnAdd, node?.ImageIndex == 0 ? "Append User Preset" : "Create Preset for current vehicle");
            }
            else
            {
                btnEdit.Enabled = btnDelete.Enabled = false;
                toolTips.SetToolTip(btnAdd, "Create Preset for current vehicle");
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (treeProfiles.SelectedNode is TreeNode node)
            {
                EditNode(node);
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (treeProfiles.SelectedNode is TreeNode node)
            {
                DeleteNode(node);
            }
        }

        bool EditCar(string gameName, string vechicleName)
        {
            if (treeProfiles.Nodes.Find(gameName, false).FirstOrDefault() is TreeNode game)
            {
                if (game.Nodes.Find(vechicleName, false).FirstOrDefault() is TreeNode car)
                {
                    EditNode(car);
                    return true;
                }
            }

            return false;
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (nextion.m_dataSet != null && nextion.m_dataSet.CarData.CarName != null && nextion.m_dataSet.GamePlugin != null && nextion.m_dataSet.GamePlugin.IsRunning)
            {
                if (EditCar(nextion.m_dataSet.GamePlugin.Name, nextion.m_dataSet.CarData.CarName))
                    return;
                else
                {
                    CustomLedsA customLeds = CustomLedsA.CreateDefaults(nextion.m_dataSet.CarData.CarName, nextion.m_dataSet.GamePlugin.Name);
                    nextion.customLeds.Add(customLeds);
                    AddVechicle(customLeds);
                    EditCar(nextion.m_dataSet.GamePlugin.Name, nextion.m_dataSet.CarData.CarName);
                }
            }
            else
            {
                if (MessageBox.Show(this, "Create User Preset?", "Create Preset", MessageBoxButtons.YesNo) != DialogResult.Yes)
                {
                    // TODO:
                    //CustomLeds customLeds = CustomLeds.CreateDefaults(nextion.m_dataSet.CarData.CarName, nextion.m_dataSet.GamePlugin.Name);
                    //AddVechicle(customLeds);
                    //EditCar(nextion.m_dataSet.GamePlugin.Name, nextion.m_dataSet.CarData.CarName);
                }
            }
        }

        private void ComboPage_SelectedIndexChanged(object sender, EventArgs e)
        {
            string page = comboPage.SelectedItem as string ?? "page0";
            nextion.Settings.DefaultPage = page;
        }

        private void NextionPropertiesForm_Load(object sender, EventArgs e)
        {
            // TODO:
        }

        private void BtnFirmware_Click(object sender, EventArgs e)
        {
            // TODO:
        }

        private void ChkMainDevice_OnSwitch(object sender, EventArgs e)
        {
            if (chkMainDevice.Checked)
                Nextion35Plugin.sPlugin.SetPrimaryDevice(nextion);
        }
    }
}