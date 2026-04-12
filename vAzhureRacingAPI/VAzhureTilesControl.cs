using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
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

        private int m_titleWidth = 64;
        private int m_titleHeight = 64;
        private int m_titleSpace = 8;
        private bool m_bShowTitle = true;
        private string m_strTitle = "Title";
        public bool ButtonsWithoutOptions { get; set; } = false;

        private readonly List<VAzhureTileElement> m_titlesList = new List<VAzhureTileElement>();
        private VAzhureTileElement activeTitle;
        private VAzhureTileElement hoverTitle;

        // Кеширование геометрии
        private readonly Dictionary<VAzhureTileElement, RectangleF> _tileRects = new Dictionary<VAzhureTileElement, RectangleF>();
        private RectangleF _activeTileRect = RectangleF.Empty;
        private bool _layoutDirty = true;

        // Кастомный скроллинг
        private int scrollOffset = 0;
        private int maxScroll = 0;
        private const int ScrollBarWidth = 6;

        public event EventHandler OnActiveTileChanged;

        [Browsable(true), Category("User Properties"), Description("Отображать заголовок")]
        public bool ShowTitle { get => m_bShowTitle; set { m_bShowTitle = value; InvalidateEx(); } }

        [Browsable(true), Category("User Properties"), Description("Текст заголовка")]
        public string Title { get => m_strTitle; set { m_strTitle = value; InvalidateEx(); } }

        [Browsable(true), Category("User Properties"), Description("Ширина плиток")]
        public int TitleWidth { get => m_titleWidth; set { m_titleWidth = Math2.Clamp(value, 16, 256); InvalidateEx(); } }

        [Browsable(true), Category("User Properties"), Description("Высота плиток")]
        public int TitleHeight { get => m_titleHeight; set { m_titleHeight = Math2.Clamp(value, 16, 256); InvalidateEx(); } }

        [Browsable(true), Category("User Properties"), Description("Шаг между плитками")]
        public int TitleSpace { get => m_titleSpace; set { m_titleSpace = Math2.Clamp(value, 1, 100); InvalidateEx(); } }

        public Rectangle ClientRectWOPading => new Rectangle(
            ClientRectangle.Left + Padding.Left,
            ClientRectangle.Top + Padding.Top,
            ClientRectangle.Width - Padding.Horizontal,
            ClientRectangle.Height - Padding.Vertical);

        private void InvalidateEx() { _layoutDirty = true; Invalidate(); }
        protected override void OnSizeChanged(EventArgs e) { base.OnSizeChanged(e); InvalidateEx(); }
        protected override void OnPaddingChanged(EventArgs e) { base.OnPaddingChanged(e); InvalidateEx(); }

        #region Layout & Scroll
        private void UpdateLayout()
        {
            _tileRects.Clear();
            if (m_titlesList.Count == 0) return;

            float startY = ClientRectWOPading.Top + (ShowTitle ? GetTitleSize().Height : 0);
            int cols = Math.Max(1, (int)(ClientRectWOPading.Width / (m_titleWidth + m_titleSpace)));
            float totalWidth = cols * m_titleWidth + (cols - 1) * m_titleSpace;
            float startX = ClientRectWOPading.Left + Math.Max(0, (ClientRectWOPading.Width - totalWidth) / 2f);

            int rows = (int)Math.Ceiling((double)m_titlesList.Count / cols);
            float totalHeight = startY + rows * m_titleHeight + Math.Max(0, rows - 1) * m_titleSpace;

            maxScroll = Math.Max(0, (int)(totalHeight - ClientRectWOPading.Height));
            // 🔑 Автоматический сброс/коррекция при ресайзе
            scrollOffset = Math.Max(0, Math.Min(scrollOffset, maxScroll));

            for (int i = 0; i < m_titlesList.Count; i++)
            {
                int col = i % cols;
                int row = i / cols;
                float x = startX + col * (m_titleWidth + m_titleSpace);
                float y = startY + row * (m_titleHeight + m_titleSpace) - scrollOffset;
                _tileRects[m_titlesList[i]] = new RectangleF(x, y, m_titleWidth, m_titleHeight);
            }
            _layoutDirty = false;
        }

        private void UpdateActiveTileRect()
        {
            if (activeTitle == null) { _activeTileRect = RectangleF.Empty; return; }
            if (!_tileRects.TryGetValue(activeTitle, out RectangleF orig)) return;

            Size sz = activeTitle.ActiveTileSize;
            float x = Math.Max(ClientRectWOPading.Left, Math.Min(orig.X, ClientRectWOPading.Right - sz.Width));
            float y = Math.Max(ClientRectWOPading.Top, Math.Min(orig.Y, ClientRectWOPading.Bottom - sz.Height));
            _activeTileRect = new RectangleF(x, y, sz.Width, sz.Height);
        }

        // 🔑 Автоскролл, чтобы активная плитка всегда была видна целиком
        private void EnsureTileVisible(VAzhureTileElement tile)
        {
            if (tile == null || !_tileRects.TryGetValue(tile, out var rect)) return;
            Size sz = tile.ActiveTileSize;
            float tileScreenY = rect.Y;
            float tileScreenBottom = tileScreenY + sz.Height;
            float viewHeight = ClientRectWOPading.Height;

            if (tileScreenY < 0)
                scrollOffset += (int)tileScreenY; // tileScreenY отрицательный
            else if (tileScreenBottom > viewHeight)
                scrollOffset += (int)(tileScreenBottom - viewHeight);

            scrollOffset = Math.Max(0, Math.Min(scrollOffset, maxScroll));
        }
        #endregion

        #region Paint
        protected override void OnPaint(PaintEventArgs e)
        {
            if (_layoutDirty) UpdateLayout();
            UpdateActiveTileRect();

            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
            e.Graphics.CompositingMode = CompositingMode.SourceOver;
            base.OnPaint(e);

            foreach (var tile in m_titlesList)
                if (tile != activeTitle && _tileRects.TryGetValue(tile, out RectangleF rect))
                    tile.OnPaint(e, rect);

            if (activeTitle != null && _activeTileRect != RectangleF.Empty)
                activeTitle.OnPaint(e, _activeTileRect);

            // Рамка и заголовок
            RectangleF border = ClientRectangle;
            border.Inflate(-2, -2);
            using (GraphicsPath gpBorder = border.RoundedRect(4))
            using (Brush brShadow = new SolidBrush(Color.FromArgb(50, 0, 0, 0)))
            using (Pen penShadow = new Pen(Color.FromArgb(50, 0, 0, 0)))
            {
                e.Graphics.DrawPath(penShadow, gpBorder);

                if (ShowTitle && !string.IsNullOrEmpty(Title))
                {
                    e.Graphics.SetClip(gpBorder, CombineMode.Replace);
                    SizeF sz = e.Graphics.MeasureString(Title, Font);
                    RectangleF rcTitle = new RectangleF(border.Location, new SizeF(border.Width, sz.Height + 2));
                    e.Graphics.FillRectangle(brShadow, rcTitle);
                    border.Inflate(-1, -1);
                    using (StringFormat sf = new StringFormat() { Alignment = StringAlignment.Center })
                    using (Brush br = new SolidBrush(ForeColor))
                        e.Graphics.DrawString(Title, Font, br, border, sf);
                    e.Graphics.ResetClip();
                }

                e.Graphics.SetClip(gpBorder, CombineMode.Exclude);
                e.Graphics.TranslateTransform(2, 2);
                e.Graphics.FillPath(brShadow, gpBorder);
                e.Graphics.TranslateTransform(-2, -2);
                e.Graphics.ResetClip();
            }

            // 🔑 Отрисовка тонкой полосы прокрутки поверх контента
            if (maxScroll > 0)
            {
                float trackHeight = ClientRectWOPading.Height;
                float thumbHeight = Math.Max(20, (float)ClientRectWOPading.Height * ClientRectWOPading.Height / (ClientRectWOPading.Height + maxScroll));
                float thumbY = (scrollOffset / (float)maxScroll) * (trackHeight - thumbHeight);

                RectangleF track = new RectangleF(ClientRectWOPading.Right - ScrollBarWidth, ClientRectWOPading.Top, ScrollBarWidth, trackHeight);
                RectangleF thumb = new RectangleF(ClientRectWOPading.Right - ScrollBarWidth, ClientRectWOPading.Top + thumbY, ScrollBarWidth, thumbHeight);

                using (Brush brTrack = new SolidBrush(Color.FromArgb(30, 0, 0, 0)))
                using (Brush brThumb = new SolidBrush(Color.FromArgb(120, 200, 200, 200)))
                {
                    e.Graphics.FillRectangle(brTrack, track);
                    e.Graphics.FillRectangle(brThumb, thumb);
                }
            }
        }

        private SizeF GetTitleSize()
        {
            using (Graphics g = CreateGraphics())
            {
                SizeF sz = g.MeasureString(!string.IsNullOrEmpty(Title) ? Title : " ", Font, ClientRectangle.Width);
                sz.Height += 4;
                return sz;
            }
        }
        #endregion

        #region Public API & Input
        public RectangleF GetTileRect(VAzhureTileElement el)
        {
            if (el == activeTitle && _activeTileRect != RectangleF.Empty) return _activeTileRect;
            return _tileRects.TryGetValue(el, out var rect) ? rect : RectangleF.Empty;
        }

        public VAzhureTileElement HitTest(Point location)
        {
            // Игнорируем клики по области скроллбара
            if (maxScroll > 0 && location.X > ClientRectWOPading.Right - ScrollBarWidth)
                return null;

            if (activeTitle != null && _activeTileRect.Contains(location)) return activeTitle;
            foreach (var kvp in _tileRects)
                if (kvp.Value.Contains(location)) return kvp.Key;
            return null;
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
                if (activeTitle == title) ActiveTitle = null;
                InvalidateEx();
            }
            return title;
        }

        [Browsable(false)]
        public VAzhureTileElement ActiveTitle
        {
            get => activeTitle;
            set
            {
                if (value != activeTitle)
                {
                    if (value != null && value.Parent != this && value.Parent != null) return;
                    activeTitle = value;
                    InvalidateEx();
                    OnActiveTileChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        [Browsable(false)]
        public VAzhureTileElement HoverTitle
        {
            get => hoverTitle;
            set
            {
                if (value != hoverTitle)
                {
                    if (value != null && value.Parent != this && value.Parent != null) return;
                    hoverTitle = value;
                    InvalidateEx();
                }
            }
        }

        protected override bool IsInputKey(Keys keyData)
        {
            return (keyData == Keys.Right || keyData == Keys.Left || keyData == Keys.Up || keyData == Keys.Down) 
                || base.IsInputKey(keyData);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Modifiers == Keys.None)
            {
                switch (e.KeyCode)
                {
                    case Keys.Escape: ActiveTitle = null; e.Handled = true; break;
                    case Keys.Left: NavigateTile(-1); e.Handled = true; break;
                    case Keys.Right: NavigateTile(1); e.Handled = true; break;
                    case Keys.Up: ScrollBy(-1); e.Handled = true; break;
                    case Keys.Down: ScrollBy(1); e.Handled = true; break;
                    default: base.OnKeyDown(e); break;
                }
            }
            else base.OnKeyDown(e);
        }

        private void ScrollBy(int direction)
        {
            if (maxScroll > 0)
            {
                int step = m_titleHeight + m_titleSpace;
                scrollOffset = Math.Max(0, Math.Min(maxScroll, scrollOffset + direction * step));
                InvalidateEx();
            }
        }

        private void NavigateTile(int dir)
        {
            if (activeTitle == null) return;
            int idx = m_titlesList.IndexOf(activeTitle) + dir;
            idx = Math2.Clamp(idx, 0, m_titlesList.Count - 1);
            ActiveTitle = m_titlesList[idx];
        }
        
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            if (maxScroll > 0)
            {
                scrollOffset -= e.Delta / 3; // Чувствительность прокрутки
                                             // 🔑 Исправлена опечатка: scrollScroll -> scrollOffset
                scrollOffset = Math.Max(0, Math.Min(scrollOffset, maxScroll));
                InvalidateEx();
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            VAzhureTileElement tile = HitTest(e.Location);
            if (tile != null) { tile.OnMouseMove(e); HoverTitle = tile; Cursor = Cursors.Hand; }
            else { HoverTitle = null; Cursor = Cursors.Default; }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            VAzhureTileElement tile = HitTest(e.Location);
            if (tile != null)
            {
                if (ButtonsWithoutOptions) { tile.OnMouseClick(e); return; }
                if (activeTitle != tile) { ActiveTitle = tile; return; }
                tile.OnMouseClick(e);
            }
            else ActiveTitle = null;
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            VAzhureTileElement tile = HitTest(e.Location);
            if (tile != null) tile.OnMouseDoubleClick(e);
        }
        #endregion

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

            public virtual Size ActiveTileSize => Parent == null ? new Size() : new Size(Parent.TitleWidth, Parent.TitleHeight);
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
                    if (IsActive) { g.TranslateTransform(shadow_dist, shadow_dist); g.FillPath(brShadow, gp); g.TranslateTransform(-shadow_dist, -shadow_dist); }
                    g.FillPath(br, gp);
                    g.DrawPath(IsHover ? Pens.White : pen, gp);
                }
            }
        }
    }
}