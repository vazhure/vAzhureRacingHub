using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace vAzhureRacingAPI
{
    public partial class VAzhureTilesControl : UserControl
    {
        public VAzhureTilesControl()
        {
            InitializeComponent();
            DoubleBuffered = true;
            BackColor = Color.Transparent;
        }

        int m_titleWidth = 64; // px
        int m_titleHeight = 64; // px
        int m_titleSpace = 8; // px
        readonly int m_scrollPosition = 0; // TODO
        bool m_bShowTitle = true;
        string m_strTitle = "Title";

        public event EventHandler OnActiveTileChanged;

        [Browsable(true), Category("User Properties"), Description("Отображать заголовок")]
        public bool ShowTitle
        {
            get
            {
                return m_bShowTitle;
            }

            set
            {
                m_bShowTitle = value;
                InvalidateEx();
            }
        }

        private bool bInvalidate = true;
        private void InvalidateEx()
        {
            if (!bInvalidate)
            {
                bInvalidate = true;
                Invalidate();
            }
        }

        [Browsable(true), Category("User Properties"), Description("Текст заголовка")]
        public string Title
        {
            get
            {
                return m_strTitle;
            }

            set
            {
                m_strTitle = value;
                InvalidateEx();
            }
        }

        [Browsable(true), Category("User Properties"), Description("Ширина плиток")]
        public int TitleWidth
        {
            get
            {
                return m_titleWidth;
            }

            set
            {
                m_titleWidth = Math2.Clamp(value, 16, 256);
                InvalidateEx();
            }
        }

        [Browsable(true), Category("User Properties"), Description("Высота плиток")]
        public int TitleHeight
        {
            get
            {
                return m_titleHeight;
            }

            set
            {
                m_titleHeight = Math2.Clamp(value, 16, 256);
                InvalidateEx();
            }
        }

        [Browsable(true), Category("User Properties"), Description("Минимальный шаг между плитками")]
        public int TitleSpace
        {
            get
            {
                return m_titleSpace;
            }

            set
            {
                m_titleSpace = Math2.Clamp(value, 1, 100);
                InvalidateEx();
            }
        }

        readonly List<VAzhureTileElement> m_titlesList = new List<VAzhureTileElement>();

        VAzhureTileElement activeTitle = null;
        VAzhureTileElement hoverTitle = null;

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            InvalidateEx();
        }

        protected override void OnPaddingChanged(EventArgs e)
        {
            base.OnPaddingChanged(e);
            InvalidateEx();
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

        protected override void OnPaint(PaintEventArgs e)
        {
            bInvalidate = false;

            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
            e.Graphics.CompositingMode = CompositingMode.SourceOver;

            base.OnPaint(e);
            RectangleF rcActive = new RectangleF();

            for (int t = 0; t < m_titlesList.Count; t++)
            {
                RectangleF rc = CalculateTileRect(t);

                if (m_titlesList[t] != ActiveTitle)
                    m_titlesList[t].OnPaint(e, rc);
                else
                    rcActive = rc;
            }

            if (ActiveTitle != null)
            {
                rcActive.Size = ActiveTitle.ActiveTileSize;
                FitInRect(ref rcActive, ClientRectWOPading);
                ActiveTitle.OnPaint(e, rcActive);
            }

            RectangleF border = ClientRectangle;
            border.Inflate(-2, -2);
            using (GraphicsPath gpBprder = border.RoundedRect(4))
            using (Brush brShadow = new SolidBrush(Color.FromArgb(50, 0, 0, 0)))
            using (Pen penShadow = new Pen(Color.FromArgb(50, 0, 0, 0)))
            {
                e.Graphics.DrawPath(penShadow, gpBprder);

                if (ShowTitle && Title.Length > 0)
                {
                    e.Graphics.SetClip(gpBprder, CombineMode.Replace);
                    SizeF sz = e.Graphics.MeasureString(Title, Font);
                    RectangleF rcTitle = new RectangleF(border.Location, new SizeF(border.Width, sz.Height + 2));
                    e.Graphics.FillRectangle(brShadow, rcTitle);
                    border.Inflate(-1, -1);
                    using (StringFormat sf = new StringFormat() { Alignment = StringAlignment.Center })
                    using (Brush br = new SolidBrush(ForeColor))
                        e.Graphics.DrawString(Title, Font, br, border, sf);
                    e.Graphics.ResetClip();
                }

                e.Graphics.SetClip(gpBprder, CombineMode.Exclude);
                e.Graphics.TranslateTransform(2, 2);
                e.Graphics.FillPath(brShadow, gpBprder);
                e.Graphics.TranslateTransform(-2, -2);
                e.Graphics.ResetClip();
            }
        }

        private void FitInRect(ref RectangleF rcActive, Rectangle clientRectWOPading)
        {
            if (rcActive.X < clientRectWOPading.X)
                rcActive.X = clientRectWOPading.X;
            if (rcActive.Y < clientRectWOPading.Y)
                rcActive.Y = clientRectWOPading.Y;

            float dx = -Math2.Clamp(rcActive.Right - clientRectWOPading.Right, 0, clientRectWOPading.Right);
            float dy = -Math2.Clamp(rcActive.Bottom - clientRectWOPading.Bottom, 0, clientRectWOPading.Bottom);

            rcActive.Offset(dx, dy);
        }

        [Browsable(false)]
        public VAzhureTileElement ActiveTitle
        {
            get { return activeTitle; }
            set
            {
                if (value != activeTitle)
                {
                    if (value != null)
                    {
                        if (value.Parent == this)
                            activeTitle = value;
                    }
                    else
                        activeTitle = value;
                    InvalidateEx();
                    OnActiveTileChanged?.Invoke(this, new EventArgs());
                }
            }
        }

        [Browsable(false)]
        public VAzhureTileElement HoverTitle
        {
            get { return hoverTitle; }
            set
            {
                if (value != hoverTitle)
                {
                    if (value != null)
                    {
                        if (value.Parent == this)
                            hoverTitle = value;
                    }
                    else
                        hoverTitle = value;

                    InvalidateEx();
                }
            }
        }

        /// <summary>
        /// Размер заголовка
        /// </summary>
        /// <returns></returns>
        SizeF GetTitleSize()
        {
            using (Graphics g = Graphics.FromHwnd(Handle))
            {
                SizeF sz = g.MeasureString(Title.Length > 0 ? Title : " ", Font, ClientRectangle.Width);
                sz += new SizeF(0, 2);
                return sz;
            }
        }

        /// <summary>
        /// Вычисление размера плитки по индексу
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        RectangleF CalculateTileRect(int idx)
        {
            float space;
            int maxElementsInRow = Math.DivRem(ClientRectWOPading.Width, m_titleWidth + m_titleSpace, out int _);

            int nRow = Math.DivRem(idx, maxElementsInRow, out int _);

            if (maxElementsInRow > m_titlesList.Count)
                space = m_titleSpace;
            else
                space = (ClientRectWOPading.Width - maxElementsInRow * m_titleWidth) / (float)(maxElementsInRow - 1);

            RectangleF rc = new RectangleF(0, 0, m_titleWidth, m_titleHeight);
            rc.Offset(ClientRectWOPading.Left, ClientRectWOPading.Top - m_scrollPosition);
            rc.Offset((idx % maxElementsInRow) * (m_titleWidth + space), nRow * (m_titleHeight + m_titleSpace));

            // Выравниваем крайний элемент к краю
            if ((idx % maxElementsInRow) == maxElementsInRow - 1)
                rc.X = ClientRectWOPading.Right - rc.Width;

            if (ShowTitle)
            {
                SizeF sz = GetTitleSize();
                rc.Offset(0, sz.Height);
            }

            return rc;
        }

        public VAzhureTileElement AddTile(VAzhureTileElement title)
        {
            title.Parent = this;
            m_titlesList.Add(title);
            InvalidateEx();
            return title;
        }

        public VAzhureTileElement RemoveTile(VAzhureTileElement title)
        {
            if (title.Parent == this)
            {
                title.Parent = null;
                m_titlesList.Remove(title);
            }
            return title;
        }

        public RectangleF GetTileRect(VAzhureTileElement el)
        {
            int idx = m_titlesList.IndexOf(el);
            if (idx >= 0)
            {
                RectangleF rc = CalculateTileRect(idx);

                return rc;
            }
            return new RectangleF(new PointF(), new SizeF(m_titleWidth, m_titleHeight));
        }

        public VAzhureTileElement HitTest(Point location)
        {
            if (ActiveTitle != null)
            {
                int idx = m_titlesList.IndexOf(activeTitle);
                if (idx >= 0)
                {
                    RectangleF rc = CalculateTileRect(idx);
                    rc.Size = activeTitle.ActiveTileSize;
                    if (rc.Contains(location))
                    {
                        return ActiveTitle;
                    }
                }
            }

            for (int t = 0; t < m_titlesList.Count; t++)
            {
                RectangleF rc = CalculateTileRect(t);
                if (rc.Contains(location))
                {
                    return m_titlesList[t];
                }
            }
            return null;
        }

        protected override bool IsInputKey(Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Right:
                case Keys.Left:
                case Keys.Up:
                case Keys.Down:
                    return true;
            }

            return base.IsInputKey(keyData);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Modifiers == Keys.None)
            {
                switch (e.KeyCode)
                {
                    case Keys.Escape:
                        ActiveTitle = null;
                        e.Handled = true;
                        break;
                    case Keys.Left:
                        if (activeTitle != null)
                        {
                            int idx = m_titlesList.IndexOf(activeTitle);
                            idx = Math2.Clamp(idx - 1, 0, m_titlesList.Count - 1);
                            try
                            {
                                ActiveTitle = m_titlesList[idx];
                            }
                            catch { }
                            e.Handled = true;
                        }
                        break;

                    case Keys.Right:
                        if (activeTitle != null)
                        {
                            int idx = m_titlesList.IndexOf(activeTitle);
                            idx = Math2.Clamp(idx + 1, 0, m_titlesList.Count - 1);
                            try
                            {
                                ActiveTitle = m_titlesList[idx];
                            }
                            catch { }
                            e.Handled = true;
                        }
                        break;

                    default:
                        base.OnKeyDown(e);
                        break;
                }
            }
            else
                base.OnKeyDown(e);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            if (ActiveTitle != null && HitTest(e.Location) != null)
            {
                int idx = m_titlesList.IndexOf(activeTitle);
                idx = Math2.Clamp(idx + e.Delta > 0 ? -1 : 1, 0, m_titlesList.Count - 1);
                try
                {
                    ActiveTitle = m_titlesList[idx];
                }
                catch { }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (HitTest(e.Location) is VAzhureTileElement title)
            {
                title.OnMouseMove(e);
                HoverTitle = title;
                Cursor = Cursors.Hand;
            }
            else
            {
                HoverTitle = null;
                Cursor = Cursors.Default;
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            if (HitTest(e.Location) is VAzhureTileElement title)
            {
                title.OnMouseClick(e);
            }
            else ActiveTitle = null;
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);

            if (HitTest(e.Location) is VAzhureTileElement title)
            {
                title.OnMouseDoubleClick(e);
            }
        }

        public abstract class VAzhureTileElement
        {
            public virtual Color BackColor { get; set; } = Color.FromArgb(64, 64, 64);
            public VAzhureTilesControl Parent { get; internal set; }
            public virtual void OnMouseMove(MouseEventArgs e) { }
            public virtual void OnMouseClick(MouseEventArgs e) { }
            public virtual void OnMouseDoubleClick(MouseEventArgs e) { }
            public virtual void OnMouseDown(MouseEventArgs e) { }
            public virtual void OnMouseUp(MouseEventArgs e) { }
            public virtual void OnPaint(PaintEventArgs e, RectangleF rect) { DrawBackground(e.Graphics, rect); }

            public virtual Size ActiveTileSize
            {
                get
                {
                    if (Parent == null)
                        return new Size();
                    return new Size(Parent.TitleWidth, Parent.TitleHeight);
                }
            }

            public bool IsActive => Parent?.activeTitle == this;
            public bool IsHover => Parent?.hoverTitle == this;

            public virtual void DrawBackground(Graphics g, RectangleF rc)
            {
                const int shadow_dist = 2;
                using (Pen pen = new Pen(Color.FromArgb(BackColor.R / 2, BackColor.G / 2, BackColor.B / 2)))
                using (Brush br = new SolidBrush(BackColor))
                using (Brush brShadow = new SolidBrush(Color.FromArgb(50, 0, 0, 0)))
                using (GraphicsPath gp = rc.RoundRectEdges(8, GraphicsExt.RoundEdge.BottomLeft | GraphicsExt.RoundEdge.TopRight))
                {
                    if (IsActive)
                    {
                        g.TranslateTransform(shadow_dist, shadow_dist);
                        g.FillPath(brShadow, gp);
                        g.TranslateTransform(-shadow_dist, -shadow_dist);
                    }
                    g.FillPath(br, gp);
                    if (IsHover)
                        g.DrawPath(Pens.White, gp);
                    else
                        g.DrawPath(pen, gp);
                }
            }
        }
    }
}
