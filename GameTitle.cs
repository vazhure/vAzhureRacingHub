using System.Drawing;
using System.Windows.Forms;

using vAzhureRacingAPI;

namespace vAzhureRacingHub
{
    class GameTitle : VAzhureTilesControl.VAzhureTileElement
    {
        readonly IGamePlugin gamePlugin;
        readonly IVAzhureRacingApp application;

        public GameTitle(IGamePlugin game, IVAzhureRacingApp app) => (gamePlugin, application) = (game, app);


        SizeF TitleTextSize
        {
            get
            {
                if (Parent == null || Parent.Handle == null || gamePlugin == null)
                    return new SizeF();

                using (Graphics g = Graphics.FromHwnd(Parent.Handle))
                {
                    g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                    return g.MeasureString(gamePlugin.Name, Parent.Font, new Point(0, 0), StringFormat.GenericDefault);
                }
            }
        }

        public override Size ActiveTileSize
        {
            get
            {
                if (IsActive)
                {
                    return new Size(base.ActiveTileSize.Width * 2, base.ActiveTileSize.Height + (int)TitleTextSize.Height + 2);
                }
                else
                    return base.ActiveTileSize;
            }
        }

        public override void OnPaint(PaintEventArgs e, RectangleF rect)
        {
            base.OnPaint(e, rect);

            RectangleF rcTitle = rect;
            rcTitle.Inflate(-2, -2);
            rect.Size = base.ActiveTileSize;

            Icon ico = gamePlugin.GetIcon();
            Rectangle rc = new Rectangle(new Point(), ico.Size);

            rc.Offset((int)rect.Center().X - rc.Width / 2, (int)rect.Center().Y - rc.Height / 2);

            if (IsActive)
            {
                using (StringFormat sf = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Near})
                    e.Graphics.DrawString(gamePlugin.Name, Parent.Font, Brushes.White, rcTitle, sf);
                SizeF sz = TitleTextSize;
                rect.Offset(rect.Size.Width, 0);
                rc.Offset(0, (int)sz.Height + 2);
                DrawPlayButton(e, rect);
                DrawSettingsButton(e, rect);
            }

            e.Graphics.DrawIcon(ico, rc);

            if (gamePlugin.IsRunning)
                e.Graphics.DrawRectangle(Pens.Green, rc);
        }


        RectangleF SettingsBtnRectangle(RectangleF rect)
        {
            RectangleF rcBtn = new RectangleF(0, 0, rect.Width * 0.9f, 0.3f * rect.Height);

            rcBtn.Offset((int)rect.Center().X - rcBtn.Width / 2, rect.Top + rect.Height * 0.3f - rcBtn.Height / 2);
            rcBtn.Offset(0, TitleTextSize.Height + 2);
            return rcBtn;
        }

        private void DrawSettingsButton(PaintEventArgs e, RectangleF rect)
        {
            RectangleF rcBtn = SettingsBtnRectangle(rect);

            using (Brush br = new SolidBrush(Color.FromArgb(46, 46, 46)))
                e.Graphics.FillRectangle(br, rcBtn);
            using (StringFormat sf = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            {
                e.Graphics.DrawString("OPTIONS", Parent.Font, Brushes.White, rcBtn, sf);
            }
        }

        RectangleF PlayBtnRectangle(RectangleF rect)
        {
            RectangleF rcBtn = new RectangleF(0, 0, rect.Width * 0.9f, 0.3f * rect.Height);

            rcBtn.Offset((int)rect.Center().X - rcBtn.Width / 2, rect.Bottom - rect.Height * 0.3f - rcBtn.Height / 2);
            rcBtn.Offset(0, TitleTextSize.Height + 2);

            return rcBtn;
        }

        private void DrawPlayButton(PaintEventArgs e, RectangleF rect)
        {
            RectangleF rcBtn = PlayBtnRectangle(rect);

            using (Brush br = new SolidBrush(gamePlugin.IsRunning ? Color.FromArgb(46, 46, 46) : Color.FromArgb(118, 185, 0)))
                e.Graphics.FillRectangle(br, rcBtn);

            using (StringFormat sf = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            {
                e.Graphics.DrawString("PLAY", Parent.Font, gamePlugin.IsRunning? Brushes.Gray : Brushes.White, rcBtn, sf);
            }
        }

        public override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            if (e.Button == MouseButtons.Left && !gamePlugin.IsRunning)
                gamePlugin.Start(application);
        }

        public override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            if (e.Button == MouseButtons.Left)
            {
                if (!IsActive)
                {
                    Parent.ActiveTitle = this;
                }
                else
                {
                    RectangleF rc = Parent.GetTileRect(this);
                    rc.Offset(rc.Width,0);
                    if (SettingsBtnRectangle(rc).Contains(e.Location))
                    {
                        gamePlugin.ShowSettings(application);
                    }
                    else
                    if (!gamePlugin.IsRunning && PlayBtnRectangle(rc).Contains(e.Location))
                    {
                            gamePlugin.Start(application);
                    }
                    else
                        Parent.ActiveTitle = null;
                }
            }
        }
    }
}
