using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace vAzhureRacingAPI
{
    public class VAzhureVolumeControl : UserControl
    {
        private int m_value = 0;
        private int m_steps = 10;
        private int m_min = 0;
        private int m_max = 100;
        private bool m_bMouseDown = false;
        private bool m_bShowValueText = true;

        [Browsable(true), Category("User Events")]
        public event EventHandler OnValueChanged;

        /// <summary>
        /// Конструктор класса
        /// </summary>
        public VAzhureVolumeControl() : base()
        {
            DoubleBuffered = true;
        }

        protected override Size DefaultSize => new Size(100, 24);

        #region User Input 
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left)
            {
                Capture = m_bMouseDown = true;
                Value = CalculatePosition(e.Location);
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
        /// Вычисление положения по курсору мыши на элементе
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        private int CalculatePosition(Point location)
        {
            int left = ClientRectangle.Left + Padding.Left;
            int right = ClientRectangle.Right - Padding.Right;

            float p = Math2.Mapf(location.X, left, right, m_min, m_max);

            return (int)Math2.Clamp(p, m_min, m_max);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (e.Button == MouseButtons.Left)
            {
                Capture = m_bMouseDown = false;
                Invalidate();
            }
        }

        #endregion

        /// <summary>
        /// Изменения полей элемента
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

        /// <summary>
        /// Отрисовка элемента
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.Clear(BackColor);
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;
            e.Graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;

            Rectangle rcClient = ClientRectWOPading;

            int height = rcClient.Height;
            int left = rcClient.Left;
            int right = rcClient.Right;
            int bottom = rcClient.Bottom;

            float v = Math2.Mapf(m_value, m_min, m_max, left, right);
            using (Pen pen = new Pen(ForeColor, 0))
            using (Pen penGray = new Pen(Color.FromArgb(ForeColor.R / 2, ForeColor.G / 2, ForeColor.B / 2), 0))
            {
                for (int t = left; t < right; ++t)
                {
                    int s = (int)Math2.Mapf(t, left, right, 0, Steps);
                    int s1 = (int)Math2.Mapf(t + 1, left, right, 0, Steps);
                    float h = Math2.Mapf(s, 0, Steps, 2, height);
                    if (s == s1) // На смену уровня не рисуем линию
                        e.Graphics.DrawLine(Enabled && (t <= v) ? pen : penGray, t, bottom, t, bottom - h);
                }
            }

            // Отображение значения
            if (m_bShowValueText && m_bMouseDown)
            {
                using (Brush brush = new SolidBrush(ForeColor))
                using (Brush brushBack = new SolidBrush(Color.FromArgb(200, BackColor.R, BackColor.G, BackColor.B)))
                using (StringFormat sf = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                {
                    string text = $"{m_value}";
                    SizeF sz = e.Graphics.MeasureString(text, Font, ClientRectangle.Size, sf);
                    RectangleF rect = new RectangleF(0, 0, sz.Width, sz.Height);
                    rect.Offset((ClientRectangle.Width - sz.Width) / 2, (ClientRectangle.Height - sz.Height) / 2);
                    e.Graphics.FillRectangle(brushBack, rect);
                    e.Graphics.DrawString(text, Font, brush, rect, sf);
                }
            }
        }

        #region User Properties

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

        [Browsable(true), Category("User Properties"), DefaultValue(true), Description("Отображать значение во время изменения")]
        public bool ShowValueText
        {
            get
            {
                return m_bShowValueText;
            }

            set
            {
                if (m_bShowValueText != value)
                {
                    m_bShowValueText = value;
                    Invalidate();
                }
            }
        }
        #endregion
    }
}