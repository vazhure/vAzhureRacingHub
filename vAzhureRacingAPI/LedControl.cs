using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace vAzhureRacingAPI
{
    public partial class LedControl : UserControl
    {
        public event EventHandler OnColorChanged;
        Color ledColor = Color.Black;

        [Browsable(true)]
        public Color LedColor
        {
            get
            {
                return ledColor;
            }
            set
            {
                if (ledColor != value)
                {
                    ledColor = value;
                    Invalidate();
                    OnColorChanged?.Invoke(this, new EventArgs());
                }
            }
        }
        public LedControl()
        {
            InitializeComponent();
            Cursor = Cursors.Hand;
            DoubleBuffered = true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            Rectangle rc = ClientRectangle;
            rc.Width -= 1;
            rc.Height -= 1;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            using (SolidBrush sb = new SolidBrush(LedColor))
                g.FillEllipse(sb, rc);
            g.DrawEllipse(Pens.Silver, rc);
        }


        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left)
            {
                DoDragDrop(ledColor, DragDropEffects.Copy);
            }
        }

        protected override void OnDragEnter(DragEventArgs drgevent)
        {
            base.OnDragEnter(drgevent);

            if (drgevent.Data.GetDataPresent(typeof(Color)))
            {
                drgevent.Effect = DragDropEffects.Copy;
            }
            else
                drgevent.Effect = DragDropEffects.None;
        }

        protected override void OnDragOver(DragEventArgs drgevent)
        {
            base.OnDragOver(drgevent);

            if (drgevent.Data.GetDataPresent(typeof(Color)))
            {
                drgevent.Effect = DragDropEffects.Copy;
                if (ModifierKeys.HasFlag(Keys.Control))
                try
                {
                    Color c = (Color)drgevent.Data.GetData(drgevent.Data.GetFormats().First());
                    LedColor = c;
                }
                catch { drgevent.Effect = DragDropEffects.None; }
            }
            else 
                drgevent.Effect = DragDropEffects.None;
        }

        protected override void OnDragDrop(DragEventArgs drgevent)
        {
            base.OnDragDrop(drgevent);

            if (drgevent.Data.GetDataPresent(typeof(Color)))
            {
                drgevent.Effect = DragDropEffects.Copy;
                try
                {
                    Color c = (Color)drgevent.Data.GetData(drgevent.Data.GetFormats().First());
                    LedColor = c;
                }
                catch { drgevent.Effect = DragDropEffects.None; }
            }
            else
                drgevent.Effect = DragDropEffects.None;
        }
    }
}
