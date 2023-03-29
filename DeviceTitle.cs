using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using vAzhureRacingAPI;

namespace vAzhureRacingHub
{
    class DeviceTitle : VAzhureTilesControl.VAzhureTileElement
    {
        readonly ICustomDevicePlugin customDevicePlugin;
        readonly IVAzhureRacingApp application;

        public DeviceTitle(ICustomDevicePlugin device, IVAzhureRacingApp app) => (customDevicePlugin, application) = (device, app);

        public override void OnPaint(PaintEventArgs e, RectangleF rect)
        {
            base.OnPaint(e, rect);
            Image ico = customDevicePlugin.DevicePictogram;
            RectangleF rc = new Rectangle(new Point(), ico.Size);

            if (rc.Width > rect.Width * 0.8f || rc.Height > rect.Height * 0.8f)
            {
                float s = Math.Max(rect.Width * 0.8f, rect.Height * 0.8f) / Math.Max(rc.Width, rc.Height);

                rc.Width *= s;
                rc.Height *= s;
            }

            rc.Offset((int)rect.Center().X - rc.Width / 2, (int)rect.Center().Y - rc.Height / 2);

            e.Graphics.DrawImage(ico, rc);

            Icon icon = customDevicePlugin.Status == DeviceStatus.Connected ? Properties.Resources.Overlay_ok : Properties.Resources.Overlay_cancel;
            Rectangle rcState = new Rectangle(new Point(), icon.Size);
            rcState.Offset((int)rect.Right - rcState.Width - 2, (int)rect.Bottom - rcState.Height - 2);
            e.Graphics.DrawIcon(icon, rcState);
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
