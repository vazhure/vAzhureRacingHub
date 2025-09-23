using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace vAzhureRacingAPI
{
    public partial class VAzhureSliderControl : UserControl
    {
        public VAzhureSliderControl()
        {
            InitializeComponent();
            DoubleBuffered = true;
        }

        private int m_value = 0;
        private int m_steps = 10;
        private int m_min = 0;
        private int m_max = 100;
        private int m_stepSmall = 1;
        private int m_stepBig = 5;

        private bool m_bVertical = false;
        private bool m_bMouseDown = false;
        private const int cBallSize = 16;
        private Color m_SliderColor = Color.FromArgb(59, 153, 252);

        [Browsable(true), Category("User Events")]
        public event EventHandler OnValueChanged;

        protected override bool IsInputKey(Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Right:
                case Keys.Left:
                case Keys.Up:
                case Keys.Down:
                case Keys.PageUp:
                case Keys.PageDown:
                case Keys.Home:
                case Keys.End:
                    return true;

                case Keys.Shift | Keys.Right:
                case Keys.Shift | Keys.Left:
                case Keys.Shift | Keys.Up:
                case Keys.Shift | Keys.Down:
                    return true;
            }

            return base.IsInputKey(keyData);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Modifiers == Keys.None || e.Modifiers == Keys.Shift)
                switch (e.KeyCode)
                {
                    case Keys.Home:
                        Value = m_min;
                        break;
                    case Keys.End:
                        Value = m_max;
                        break;
                    case Keys.Left:
                    case Keys.Down:
                        Value -= e.Modifiers == Keys.Shift ? m_stepBig : m_stepSmall;
                        break;
                    case Keys.PageDown:
                        Value -= m_stepBig;
                        break;
                    case Keys.PageUp:
                        Value += m_stepBig;
                        break;
                    case Keys.Right:
                    case Keys.Up:
                        Value += e.Modifiers == Keys.Shift ? m_stepBig : m_stepSmall;
                        break;
                    default:
                        base.OnKeyDown(e);
                        break;
                }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left && HitTest(e.Location))
            {
                Capture = m_bMouseDown = true;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (e.Button == MouseButtons.Left && m_bMouseDown)
            {
                Value = CalculatePosition(e.Location);
            }
        }

        /// <summary>
        /// Проверка попадания в ползунок
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        private bool HitTest(Point location)
        {
            Rectangle rcClient = ClientRectWOPading;

            if (m_bVertical)
            {
                int dia = (int)Math2.Clamp(rcClient.Width * 0.75, 0, 16);
                float p = Math2.Mapf(m_value, m_min, m_max, rcClient.Height - dia, 0);
                Rectangle rcSlider = new Rectangle(rcClient.Left + (rcClient.Width - dia) / 2, rcClient.Top, dia, dia);
                rcSlider.Offset(0, (int)p);

                return rcSlider.Contains(location);
            }
            else
            {
                int dia = (int)(rcClient.Height * 0.75);
                Rectangle rcSlider = new Rectangle(rcClient.Left, rcClient.Top + (rcClient.Height - dia) / 2, dia, dia);
                float p = Math2.Mapf(m_value, m_min, m_max, 0, rcClient.Width - dia);
                rcSlider.Offset((int)p, 0);
                return rcSlider.Contains(location);
            }
        }

        /// <summary>
        /// Вычисление значения ползунка по курсору мыши
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        private int CalculatePosition(Point location)
        {
            Rectangle rcClient = ClientRectWOPading;

            if (m_bVertical)
            {
                int dia = (int)Math2.Clamp(rcClient.Width * 0.75, 0, 16);
                float p = Math2.Mapf(location.Y, rcClient.Top + dia / 2, rcClient.Bottom - dia / 2, m_max, m_min);
                return (int)Math2.Clamp(p, m_min, m_max);
            }
            else
            {
                int dia = (int)Math2.Clamp(rcClient.Height * 0.75, 0, 16);
                float p = Math2.Mapf(location.X, rcClient.Left + dia / 2, rcClient.Right - dia / 2, m_min, m_max);
                return (int)Math2.Clamp(p, m_min, m_max);
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (e.Button == MouseButtons.Left)
                Capture = m_bMouseDown = false;
        }

        /// <summary>
        /// Изменение полей
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaddingChanged(EventArgs e)
        {
            base.OnPaddingChanged(e);
            Invalidate();
        }

        /// <summary>
        /// Изменение размеров элемента
        /// </summary>
        /// <param name="e"></param>
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            Invalidate();
        }

        /// <summary>
        /// Размер клиентской области окна без полей
        /// </summary>
        public Rectangle ClientRectWOPading
        {
            get
            {
                return new Rectangle(ClientRectangle.Left + Padding.Left, ClientRectangle.Top + Padding.Top,
                    ClientRectangle.Width - Padding.Horizontal, ClientRectangle.Height - Padding.Vertical);
            }
        }

        public override Size MinimumSize { get => new Size(cBallSize, cBallSize); }

        protected override Size DefaultSize => new Size(100, cBallSize);

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            Invalidate();
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            Invalidate();
        }

        /// <summary>
        /// Отрисовка слайдера
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            Rectangle rcClient = ClientRectWOPading;

            if (m_bVertical)
            {
                int cH = Math2.Clamp(Math.Max(2, rcClient.Width / 5), 0, 4);
                int dH = Math2.Clamp(Math.Max(4, rcClient.Width / 4), 0, 8);
                int dia = (int)Math2.Clamp(rcClient.Width * 0.75, 0, 16);
                float p = Math2.Mapf(m_value, m_min, m_max, rcClient.Height - dia, 0);

                Rectangle rcMid = new Rectangle(rcClient.Left + (rcClient.Width - cH) / 2, rcClient.Top + dia / 2, cH, rcClient.Height - dia);
                Rectangle rcActive = new Rectangle(rcClient.Left + (rcClient.Width - dH) / 2, rcClient.Top + (int)p + dia / 2, dH, rcClient.Bottom - dia / 2 - (int)p);
                Rectangle rcSlider = new Rectangle(rcClient.Left + (rcClient.Width - dia) / 2, rcClient.Top, dia, dia);
                rcSlider.Offset(0, (int)p);

                using (var brSemitransparent = new SolidBrush(Color.FromArgb(127, 255, 255, 255)))
                using (var brSlider = new SolidBrush(m_SliderColor))
                using (var penShadow = new Pen(Color.FromArgb(m_SliderColor.R / 2, m_SliderColor.G / 2, m_SliderColor.B / 2), 0))
                using (var penHighLight = new Pen(Color.LightGray, 0))
                {
                    e.Graphics.FillRoundedRect(brSemitransparent, rcMid, cH / 2);
                    e.Graphics.FillRoundedRect(brSlider, rcActive, dH / 2);
                    e.Graphics.FillEllipse(brSlider, rcSlider);
                    e.Graphics.DrawEllipse(Focused? penHighLight: penShadow, rcSlider);
                }
            }
            else
            {
                int cH = Math2.Clamp(Math.Max(2, rcClient.Height / 5), 0, 4);
                int dH = Math2.Clamp(Math.Max(4, rcClient.Height / 4), 0, 8);
                int dia = (int)Math2.Clamp(rcClient.Height * 0.75, 0, 16);
                float p = Math2.Mapf(m_value, m_min, m_max, 0, rcClient.Width - dia);

                Rectangle rcMid = new Rectangle(rcClient.Left + dia / 2, rcClient.Top + (rcClient.Height - cH) / 2, rcClient.Width - dia, cH);
                Rectangle rcActive = new Rectangle(rcClient.Left + dia / 2, rcClient.Top + (rcClient.Height - dH) / 2, (int)p, dH);
                Rectangle rcSlider = new Rectangle(rcClient.Left, rcClient.Top + (rcClient.Height - dia) / 2, dia, dia);
                rcSlider.Offset((int)p, 0);

                using (var brSemitransparent = new SolidBrush(Color.FromArgb(127, 255, 255, 255)))
                using (var brSlider = new SolidBrush(m_SliderColor))
                using (var penShadow = new Pen(Color.FromArgb(m_SliderColor.R / 2, m_SliderColor.G / 2, m_SliderColor.B / 2), 0))
                using (var penHighLight = new Pen(Color.LightGray, 0))
                {
                    e.Graphics.FillRoundedRect(brSemitransparent, rcMid, cH / 2);
                    e.Graphics.FillRoundedRect(brSlider, rcActive, dH / 2);
                    e.Graphics.FillEllipse(brSlider, rcSlider);
                    e.Graphics.DrawEllipse(Focused ? penHighLight : penShadow, rcSlider);
                }
            }
        }

        #region User Properties

        [Browsable(true), Category("User Properties"), Description("Маленький шаг изменения значений с клавиатуры")]
        public int SmallStep
        {
            get
            {
                return m_stepSmall;
            }

            set
            {
                m_stepSmall = value;
            }
        }

        [Browsable(true), Category("User Properties"), Description("Большой шаг изменения значений с клавиатуры")]
        public int BigStep
        {
            get
            {
                return m_stepBig;
            }

            set
            {
                m_stepBig = value;
            }
        }

        [Browsable(true), Category("User Properties"), Description("Цвет слайдера")]
        public Color SliderColor
        {
            get
            {
                return m_SliderColor;
            }

            set
            {
                if (m_SliderColor != value)
                {
                    m_SliderColor = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Ориентация слайдера
        /// </summary>
        [Browsable(true), Category("User Properties")]
        public bool Vertical
        {
            get
            {
                return m_bVertical;
            }

            set
            {
                if (m_bVertical != value)
                {
                    m_bVertical = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Минимальное значение слайдера
        /// </summary>
        [Browsable(true), Category("User Properties")]
        public int Minimum
        {
            get
            {
                return m_min;
            }

            set
            {
                if (m_min != value && value < m_max)
                {
                    m_min = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Максимальное значение слайдера
        /// </summary>
        [Browsable(true), Category("User Properties")]
        public int Maximum
        {
            get
            {
                return m_max;
            }

            set
            {
                if (m_max != value && value > m_min)
                {
                    m_max = value;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Актуальное значение
        /// </summary>
        [Browsable(true), Category("User Properties")]
        public int Value
        {
            get
            {
                return m_value;
            }

            set
            {
                if (m_value != value)
                {
                    m_value = Math2.Clamp(value, m_min, m_max);
                    Invalidate();
                    OnValueChanged?.Invoke(this, new EventArgs());
                }
            }
        }

        /// <summary>
        /// Количество визуальных шагов
        /// </summary>
        [Browsable(true), Category("User Properties"), Description("Количество визуальных шагов")]
        public int Steps
        {
            get
            {
                return m_steps;
            }

            set
            {
                if (m_steps != value && value > 0)
                {
                    m_steps = value;
                    Invalidate();
                }
            }
        }
        #endregion
    }
}
