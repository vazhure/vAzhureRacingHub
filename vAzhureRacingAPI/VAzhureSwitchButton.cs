using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;

namespace vAzhureRacingAPI
{
    public partial class VAzhureSwitchButton : UserControl
    {
        public VAzhureSwitchButton()
        {
            InitializeComponent();
            DoubleBuffered = true;
        }

        [Browsable(true), Category("User Events")]
        public event EventHandler OnSwitch;

        private bool bChecked = false;
        private const int padding = 3;

        /// <summary>
        /// Состояние кнопки
        /// </summary>
        public bool Checked
        {
            get
            {
                return bChecked;
            }

            set
            {
                if (bChecked != value)
                {
                    bChecked = value;
                    Invalidate();
                    OnSwitch?.Invoke(this, new EventArgs());
                }
            }
        }

        /// <summary>
        /// Изображения состояний
        /// </summary>

        private Image switchOn = global::vAzhureRacingAPI.Properties.Resources.switch_true;
        private Image switchOff = global::vAzhureRacingAPI.Properties.Resources.switch_false;

        [Browsable(true), Category("Properties"), EditorBrowsable(EditorBrowsableState.Always), DefaultValue(typeof(Image), "global::vAzhureRacingAPI.Properties.Resources.switch_false"), Editor(typeof(ImageEditor), typeof(UITypeEditor))]
        public Image StateOff { get => switchOff; set { switchOff = value; Invalidate(); } }
        [Browsable(true), Category("Properties"), EditorBrowsable(EditorBrowsableState.Always), DefaultValue(typeof(Image), "global::vAzhureRacingAPI.Properties.Resources.switch_true"), Editor(typeof(ImageEditor), typeof(UITypeEditor))]
        public Image StateOn { get => switchOn; set { switchOn = value; Invalidate(); } }

        /// <summary>
        /// Подпись к кнопке
        /// </summary>
        private string sSwitchText = "";

        /// <summary>
        /// Подпись к кнопке
        /// </summary>
        [Browsable(true), Category("Properties")]
        public string SwitchText

        {
            get => sSwitchText;
            set
            {
                if (sSwitchText != value)
                {
                    sSwitchText = value;
                    Size = new Size();
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Размер текстового поля
        /// </summary>
        SizeF TextSize
        {
            get
            {
                try
                {
                    if (sSwitchText.Length <= 0 || Handle == null)
                        return new SizeF();

                    using (Graphics g = Graphics.FromHwnd(Handle))
                    {
                        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

                        return g.MeasureString(sSwitchText, Font, new Point(0, 0), StringFormat.GenericDefault);
                    }
                }
                catch { return new SizeF(); }
            }
        }

        /// <summary>
        /// Минимальный размер
        /// </summary>
        public override Size MinimumSize
        {
            get
            {
                SizeF szText = TextSize;
                return new Size(IconSize.Width + (int)szText.Width + padding, Math.Max((int)szText.Height, IconSize.Height));
            }
            set { }
        }

        /// <summary>
        /// Максимальный размер
        /// </summary>
        public override Size MaximumSize
        {
            get
            {
                SizeF szText = TextSize;
                return new Size(IconSize.Width + (int)szText.Width + padding, Math.Max((int)szText.Height, IconSize.Height));
            }
            set { }
        }

        /// <summary>
        /// Фокусировка на переключателе
        /// </summary>
        /// <param name="e"></param>
        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            Invalidate();
        }

        /// <summary>
        /// Потеря фокуса
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            Invalidate();
        }

        /// <summary>
        /// Отрисовка переключателя
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.Clear(BackColor);
            e.Graphics.DrawImageUnscaled(bChecked ? switchOn : switchOff, new Point(0, 0));
            if (sSwitchText.Length > 0)
            {
                using (Brush textcolor = new SolidBrush(ForeColor))
                using (StringFormat sf = new StringFormat() { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center })
                {
                    e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                    e.Graphics.DrawString(sSwitchText, Font, textcolor, new RectangleF(ClientRectangle.X + IconSize.Width + padding,
                        ClientRectangle.Y, ClientRectangle.Width - (IconSize.Width), ClientRectangle.Height), sf);
                }
            }
        }

        /// <summary>
        /// Размер пиктограммы
        /// </summary>
        Size IconSize => (switchOn ?? switchOff).Size;

        /// <summary>
        /// Проверка попадания курсора в пиктограмму
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        bool HitTest(Point p)
        {
            Rectangle rc = new Rectangle(ClientRectangle.Location, IconSize);
            return rc.Contains(p);
        }

        /// <summary>
        /// Клик мышкой на переключателе
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (Enabled && e.Button == MouseButtons.Left && HitTest(e.Location))
            {
                base.OnClick(e);
                Checked = !Checked;
            }
            else
                base.OnMouseClick(e);
        }

        /// <summary>
        /// Обработка нажатия кнопки
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);
            if (Enabled && Focused && e.KeyChar == ' ')
                Checked = !Checked;
        }
    }
}