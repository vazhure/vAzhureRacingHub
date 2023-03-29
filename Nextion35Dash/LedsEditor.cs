using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using vAzhureRacingAPI;

namespace Nextion35Dash
{
    public partial class LedsEditor : MovableForm
    {
        readonly NextionDevice nextion;
        readonly ContextMenuStrip cm = new ContextMenuStrip();
        private readonly CustomLeds currentProfile = null;
        private readonly CustomLeds oldProfile = null;
        private readonly CustomLeds profile = null;

        public LedsEditor(NextionDevice device, ref CustomLeds leds)
        {
            nextion = device;
            oldProfile = leds?.Clone();
            profile = leds;
            currentProfile = leds?.Clone();
            InitializeComponent();
            DoubleBuffered = true;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            List<string> games = new List<string>();

            // Формируем контекстное меню для сохранённых профилей
            foreach (CustomLeds leds in nextion?.customLeds)
            {
                ToolStripMenuItem item = null;

                if (!games.Contains(leds.Game))
                {
                    games.Add(leds.Game);

                    if (Nextion35Plugin.sApp.GamePlugins.Where(p => p.Name == leds.Game).FirstOrDefault() is IGamePlugin game)
                    {
                        item = new ToolStripMenuItem() { Name = leds.Game, Text = leds.Game, Image = game.GetIcon().ToBitmap()};
                    }
                    else
                        item = new ToolStripMenuItem() { Name = leds.Game, Text = leds.Game };

                    cm.Items.Add(item);
                }
                else
                {
                    if (cm.Items.Find(leds.Game, false).FirstOrDefault() is ToolStripMenuItem menuItem)
                    {
                        item = menuItem;
                    }
                }
             
                if (item != null)
                {
                    ToolStripMenuItem mi = new ToolStripMenuItem() { Name = leds.Vechicle, Text = leds.Vechicle, Tag = leds };
                    mi.Click += delegate (object sender, EventArgs args) { OnProfile(leds); };
                    item.DropDownItems.Add(mi);
                }
            }

            if (currentProfile != null)
            {
                OnProfile(currentProfile);
                lblGame.Text = currentProfile.Game;
                lblVehicle.Text = currentProfile.Vechicle;
            }

            btnLoadProfile.Click += delegate (object o, EventArgs args) 
            {
                Point pt = new Point(btnLoadProfile.Width, btnLoadProfile.Height);
                cm.Show(btnLoadProfile, pt); 
            };
        }

        private void OnProfile(CustomLeds customLeds)
        {
            flowLayoutPanel.Visible = false;
            flowLayoutPanel.SuspendLayout();
            flowLayoutPanel.Controls.Clear();

            List<Control> newItems = new List<Control>();

            foreach (LedState<RGB8> ls in customLeds.RPMMap)
            {
                LedGroup group = new LedGroup();
                try
                {
                    group.Initialize(ls);

                    group.OnClickAdd += Group_OnClickAdd;
                    group.OnClickRemove += Group_OnClickRemove;
                    
                    newItems.Add(group);
                }
                catch { }
            }

            if (newItems.Count > 0)
                flowLayoutPanel.Controls.AddRange(newItems.ToArray());
            
            flowLayoutPanel.ResumeLayout();
            flowLayoutPanel.Visible = true;
        }

        private void Group_OnClickRemove(object sender, EventArgs e)
        {
            if (sender is LedGroup group)
            {
                flowLayoutPanel.Controls.Remove(group);
            }
        }

        private void Group_OnClickAdd(object sender, EventArgs e)
        {
            if (sender is LedGroup group)
            {
                int idx = flowLayoutPanel.Controls.IndexOf(group);
                try
                {
                    LedState<RGB8> lsNew = group.State;
                    LedGroup groupNew = new LedGroup(lsNew);
                    groupNew.OnClickAdd += Group_OnClickAdd;
                    groupNew.OnClickRemove += Group_OnClickRemove;

                    flowLayoutPanel.Controls.Add(groupNew);
                    flowLayoutPanel.Controls.SetChildIndex(groupNew, idx + 1);
                }
                catch { }
            }
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            profile.RPMMap.Clear();
            foreach (Control c in flowLayoutPanel.Controls)
            {
                if (c is LedGroup group)
                {
                    profile.RPMMap.Add(group.State.Clone() as LedState<RGB8>);
                }
            }
        }

        private void BtnApply_Click(object sender, EventArgs e)
        {
            profile.RPMMap.Clear();
            foreach (Control c in flowLayoutPanel.Controls)
            {
                if (c is LedGroup group)
                {
                    profile.RPMMap.Add(group.State.Clone() as LedState<RGB8>);
                }    
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            profile.Copy(oldProfile);

            DialogResult = DialogResult.Cancel;
        }
    }
}