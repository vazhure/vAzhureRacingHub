using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using vAzhureRacingAPI;

namespace Nextion35Dash
{
    public partial class LedGroup : UserControl
    {
        [Browsable(true), Category("User Events")]
        public event EventHandler OnChanged;
        [Browsable(true), Category("User Events")]
        public event EventHandler OnClickAdd;
        [Browsable(true), Category("User Events")]
        public event EventHandler OnClickRemove;
        [Browsable(true), Category("User Events")]
        public event EventHandler OnEndDrag;

        public LedGroup()
        {
            InitializeComponent();
            ledControl1.OnColorChanged += delegate (object o, EventArgs a) { OnChanged?.Invoke(this, new EventArgs()); };
            ledControl2.OnColorChanged += delegate (object o, EventArgs a) { OnChanged?.Invoke(this, new EventArgs()); };
            ledControl3.OnColorChanged += delegate (object o, EventArgs a) { OnChanged?.Invoke(this, new EventArgs()); };
            ledControl4.OnColorChanged += delegate (object o, EventArgs a) { OnChanged?.Invoke(this, new EventArgs()); };
            ledControl5.OnColorChanged += delegate (object o, EventArgs a) { OnChanged?.Invoke(this, new EventArgs()); };
            ledControl6.OnColorChanged += delegate (object o, EventArgs a) { OnChanged?.Invoke(this, new EventArgs()); };
            ledControl7.OnColorChanged += delegate (object o, EventArgs a) { OnChanged?.Invoke(this, new EventArgs()); };
            ledControl8.OnColorChanged += delegate (object o, EventArgs a) { OnChanged?.Invoke(this, new EventArgs()); };
            numRev.ValueChanged += delegate (object o, EventArgs a) { OnChanged?.Invoke(this, new EventArgs()); };
            chkBlink.CheckedChanged += delegate (object o, EventArgs a) { OnChanged?.Invoke(this, new EventArgs()); };
            AllowDrop = true;
        }

        public LedGroup(LedState<RGB8> ls) : this()
        {
            Initialize(ls);
        }

        public void Initialize(LedState<RGB8> ls)
        {
            Led1.LedColor = ls.Leds[0].ToColor();
            Led2.LedColor = ls.Leds[1].ToColor();
            Led3.LedColor = ls.Leds[2].ToColor();
            Led4.LedColor = ls.Leds[3].ToColor();
            Led5.LedColor = ls.Leds[4].ToColor();
            Led6.LedColor = ls.Leds[5].ToColor();
            Led7.LedColor = ls.Leds[6].ToColor();
            Led8.LedColor = ls.Leds[7].ToColor();
            Blink = ls.Blink;
            Rev = ls.RPM;
        }

        public LedState<RGB8> State
        {
            get
            {
                return new LedState<RGB8>() { Blink = Blink, RPM = (int)Rev, Leds = new RGB8[] 
                    {
                        RGB8.FromColor(Led1.LedColor),
                        RGB8.FromColor(Led2.LedColor),
                        RGB8.FromColor(Led3.LedColor),
                        RGB8.FromColor(Led4.LedColor),
                        RGB8.FromColor(Led5.LedColor),
                        RGB8.FromColor(Led6.LedColor),
                        RGB8.FromColor(Led7.LedColor),
                        RGB8.FromColor(Led8.LedColor),
                    } 
                };
            }
        }

        [Bindable(true), Category("User Properties")]
        public bool Blink { get => chkBlink.Checked; set => chkBlink.Checked = value; }
        [Bindable(true), Category("User Properties")]
        public decimal Rev { get => numRev.Value; set => numRev.Value = value; }
        [Bindable(true), Category("User Properties")]
        public LedControl Led1 { get => ledControl1; }
        [Bindable(true), Category("User Properties")]
        public LedControl Led2 { get => ledControl2; }
        [Bindable(true), Category("User Properties")]
        public LedControl Led3 { get => ledControl3; }
        [Bindable(true), Category("User Properties")]
        public LedControl Led4 { get => ledControl4; }
        [Bindable(true), Category("User Properties")]
        public LedControl Led5 { get => ledControl5; }
        [Bindable(true), Category("User Properties")]
        public LedControl Led6 { get => ledControl6; }
        [Bindable(true), Category("User Properties")]
        public LedControl Led7 { get => ledControl7; }
        [Bindable(true), Category("User Properties")]
        public LedControl Led8 { get => ledControl8; }

        private void OnPlus(object sender, EventArgs e)
        {
            OnClickAdd?.Invoke(this, new EventArgs());
        }

        private void OnMinus(object sender, EventArgs e)
        {
            OnClickRemove?.Invoke(this, new EventArgs());
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Rectangle rc = new Rectangle(e.ClipRectangle.Left + 2, e.ClipRectangle.Top + 2, 4, e.ClipRectangle.Height - 4);
            ControlPaint.DrawGrabHandle(e.Graphics, rc, true, true);
            rc.Offset(5, 0);
            ControlPaint.DrawGrabHandle(e.Graphics, rc, true, true);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left && e.X < 10)
            {
                if (DoDragDrop(this, DragDropEffects.Move) != DragDropEffects.None)
                    OnEndDrag?.Invoke(this, new EventArgs());
            }
        }

        protected override void OnDragOver(DragEventArgs drgevent)
        {
            base.OnDragOver(drgevent);

            if (drgevent.Data.GetData(typeof(LedGroup)) is LedGroup ledGroup && ledGroup.Parent is FlowLayoutPanel2 panel)
            {
                Point p = panel.PointToClient(new Point(drgevent.X, drgevent.Y));
                var item = panel.GetChildAtPoint(p);
                if (item != null && item != ledGroup)
                {
                    int index = panel.Controls.GetChildIndex(item, false);
                    panel.Controls.SetChildIndex(ledGroup, index);
                    panel.Invalidate();

                }
                drgevent.Effect = DragDropEffects.All;
                return;
            }
            drgevent.Effect = DragDropEffects.None;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.X < 10)
                Cursor = Cursors.Hand;
            else
                Cursor = Cursors.Default;
        }
    }
}
