using System.Drawing;
using System.Windows.Forms;
using vAzhureRacingAPI;

namespace vAzhureRacingHub
{
    class GameTitle : VAzhureTilesControl.VAzhureTileElement
    {
        readonly IGamePlugin gamePlugin;
        readonly IVAzhureRacingApp application;

        public GameTitle(IGamePlugin game, IVAzhureRacingApp app)
        {
            gamePlugin = game;
            application = app;
        }

        private SizeF _titleSizeCache;
        private SizeF TitleTextSize
        {
            get
            {
                if (_titleSizeCache == SizeF.Empty && gamePlugin != null && Parent != null && Parent.Handle != null)
                {
                    using (Graphics g = Graphics.FromHwnd(Parent.Handle))
                    {
                        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                        _titleSizeCache = g.MeasureString(gamePlugin.Name, Parent.Font);
                    }
                }
                return _titleSizeCache;
            }
        }

        public override Size ActiveTileSize
        {
            get
            {
                if (!IsActive) return base.ActiveTileSize;
                return new Size(base.ActiveTileSize.Width * 2, base.ActiveTileSize.Height + (int)TitleTextSize.Height + 72);
            }
        }

        private void GetLocalButtonRects(out RectangleF settings, out RectangleF play)
        {
            float btnW = ActiveTileSize.Width * 0.85f;
            float btnH = 28f;
            float startY = base.ActiveTileSize.Height + TitleTextSize.Height + 6f;

            settings = new RectangleF((ActiveTileSize.Width - btnW) / 2f, startY, btnW, btnH);
            play = new RectangleF((ActiveTileSize.Width - btnW) / 2f, startY + btnH + 6f, btnW, btnH);
        }

        public override void OnPaint(PaintEventArgs e, RectangleF rect)
        {
            base.OnPaint(e, rect);
            if (gamePlugin == null) return;

            Icon ico = gamePlugin.GetIcon();
            if (ico != null)
            {
                Rectangle rcIcon = new Rectangle(Point.Empty, ico.Size);
                PointF center = new PointF(rect.X + rect.Width / 2f, rect.Y + base.ActiveTileSize.Height / 2f);
                rcIcon.Offset((int)center.X - rcIcon.Width / 2, (int)center.Y - rcIcon.Height / 2);
                e.Graphics.DrawIcon(ico, rcIcon);
                if (gamePlugin.IsRunning) e.Graphics.DrawRectangle(Pens.Green, rcIcon);
            }

            if (IsActive)
            {
                GetLocalButtonRects(out RectangleF settings, out RectangleF play);
                settings.Offset(rect.X, rect.Y);
                play.Offset(rect.X, rect.Y);

                RectangleF rcTitle = new RectangleF(rect.X, rect.Y + base.ActiveTileSize.Height, rect.Width, TitleTextSize.Height);
                using (StringFormat sf = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Near })
                    e.Graphics.DrawString(gamePlugin.Name, Parent.Font, Brushes.White, rcTitle, sf);

                using (Brush brSet = new SolidBrush(Color.FromArgb(46, 46, 46)))
                using (Brush brPlay = new SolidBrush(gamePlugin.IsRunning ? Color.FromArgb(46, 46, 46) : Color.FromArgb(118, 185, 0)))
                using (Brush brPlayTxt = new SolidBrush(gamePlugin.IsRunning ? Color.Gray : Color.White))
                using (StringFormat sfBtn = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                {
                    e.Graphics.FillRectangle(brSet, settings);
                    e.Graphics.DrawString("OPTIONS", Parent.Font, Brushes.White, settings, sfBtn);

                    e.Graphics.FillRectangle(brPlay, play);
                    e.Graphics.DrawString("PLAY", Parent.Font, brPlayTxt, play, sfBtn);
                }
            }
        }

        public override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            if (Parent == null || !IsActive) return;

            RectangleF tileRect = Parent.GetTileRect(this);
            PointF localMouse = new PointF(e.Location.X - tileRect.X, e.Location.Y - tileRect.Y);

            GetLocalButtonRects(out RectangleF settings, out RectangleF play);

            if (settings.Contains(localMouse))
                gamePlugin.ShowSettings(application);
            else if (play.Contains(localMouse) && !gamePlugin.IsRunning)
                gamePlugin.Start(application);
            else
                Parent.ActiveTitle = null;
        }

        public override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (Parent == null) return;

            if (IsActive)
            {
                RectangleF tileRect = Parent.GetTileRect(this);
                PointF localMouse = new PointF(e.Location.X - tileRect.X, e.Location.Y - tileRect.Y);
                GetLocalButtonRects(out RectangleF settings, out RectangleF play);

                Parent.Cursor = (settings.Contains(localMouse) || play.Contains(localMouse)) 
                    ? Cursors.Hand : Cursors.Default;
            }
            else Parent.Cursor = Cursors.Hand;
        }

        public override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            if (e.Button == MouseButtons.Left && !gamePlugin.IsRunning)
                gamePlugin.Start(application);
        }
    }
}