using System;
using System.Drawing;
using System.Windows.Forms;
using vAzhureRacingAPI;

namespace vAzhureRacingHub
{
    class DeviceTitle : VAzhureTilesControl.VAzhureTileElement
    {
        readonly ICustomDevicePlugin customDevicePlugin;
        readonly IVAzhureRacingApp application;

        public DeviceTitle(ICustomDevicePlugin device, IVAzhureRacingApp app)
        {
            customDevicePlugin = device;
            application = app;
        }

        public override void OnPaint(PaintEventArgs e, RectangleF rect)
        {
            base.OnPaint(e, rect);
            if (customDevicePlugin?.DevicePictogram == null) return;

            Image ico = customDevicePlugin.DevicePictogram;
            RectangleF rc = new RectangleF(0, 0, ico.Width, ico.Height);

            // Масштабируем иконку до 80% плитки
            float scale = Math.Min((rect.Width * 0.8f) / rc.Width, (rect.Height * 0.8f) / rc.Height);
            rc.Width *= scale;
            rc.Height *= scale;

            // Центрируем
            rc.Offset(rect.X + (rect.Width - rc.Width) / 2f, rect.Y + (rect.Height - rc.Height) / 2f);
            e.Graphics.DrawImage(ico, rc);

            // Статус подключения
            Icon icon = customDevicePlugin.Status == DeviceStatus.Connected 
                ? Properties.Resources.Overlay_ok 
                : Properties.Resources.Overlay_cancel;
            
            if (icon != null)
            {
                RectangleF rcState = new RectangleF(0, 0, icon.Width, icon.Height);
                rcState.Offset(rect.Right - rcState.Width - 2, rect.Bottom - rcState.Height - 2);
                e.Graphics.DrawIcon(icon, Rectangle.Round(rcState));
            }
        }

        public override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            if (e.Button == MouseButtons.Left)
            {
                customDevicePlugin.ShowProperties(application);
            }
        }
    }
}