using Microsoft.Win32;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace vAzhureRacingAPI
{
    public static class WindowsFormHelper
    {
        public static readonly string app_name = Application.ProductName;
        private static void SaveSetting(string name, string formName, object value)
        {
            RegistryKey reg_key = Registry.CurrentUser.OpenSubKey("Software", true);
            RegistryKey sub_key = reg_key.CreateSubKey(app_name);
            sub_key = sub_key.CreateSubKey(formName);
            sub_key.SetValue(name, value);
        }

        private static object LoadSetting(string name, string formName, object default_value)
        {
            RegistryKey reg_key = Registry.CurrentUser.OpenSubKey("Software", true);
            RegistryKey sub_key = reg_key.CreateSubKey(app_name);
            sub_key = sub_key.CreateSubKey(formName);
            return sub_key.GetValue(name, default_value);
        }

        /// <summary>
        /// Сохранение размеров и состояния окна
        /// </summary>
        /// <param name="form"></param>
        public static void SaveState(Form form)
        {
            string formName = form.GetType().ToString();
            if (form.WindowState == FormWindowState.Maximized)
            {
                SaveSetting("Maximized", formName, form.WindowState == FormWindowState.Maximized);
            }
            else if (form.WindowState == FormWindowState.Normal)
            {
                SaveSetting("Maximized", formName, false);
                SaveSetting("FormLeft", formName, form.Left);
                SaveSetting("FormTop", formName, form.DesktopBounds.Top);
                SaveSetting("FormWidth", formName, form.DesktopBounds.Width);
                SaveSetting("FormHeight", formName, form.DesktopBounds.Height);
            }
        }

        /// <summary>
        /// Восстановление размеров и состояния окна
        /// </summary>
        /// <param name="form"></param>
        public static void LoadState(Form form)
        {
            string formName = form.GetType().ToString();

            // Load saved position and size
            int left = (int)LoadSetting("FormLeft", formName, form.DesktopBounds.Left);
            int top = (int)LoadSetting("FormTop", formName, form.DesktopBounds.Top);
            int width = form.Width;
            int height = form.Height;

            if (form.FormBorderStyle == FormBorderStyle.Sizable ||
                form.FormBorderStyle == FormBorderStyle.SizableToolWindow)
            {
                width = (int)LoadSetting("FormWidth", formName, form.DesktopBounds.Width);
                height = (int)LoadSetting("FormHeight", formName, form.DesktopBounds.Height);
            }

            // Handle maximized state
            if (LoadSetting("Maximized", formName, "False") is string sMaximized &&
                sMaximized.ToLower() == "true")
            {
                form.WindowState = FormWindowState.Maximized;
                return; // No need to adjust position if maximized
            }

            // Determine the appropriate screen based on saved position
            Screen targetScreen = null;
            foreach (var screen in Screen.AllScreens)
            {
                if (screen.Bounds.Contains(new Point(left, top)))
                {
                    targetScreen = screen;
                    break;
                }
            }

            if (targetScreen == null)
            {
                targetScreen = Screen.PrimaryScreen;
            }

            // Adjust position to fit within the screen's working area
            var workArea = targetScreen.WorkingArea;
            left = Math.Max(workArea.Left, Math.Min(left, workArea.Right - width));
            top = Math.Max(workArea.Top, Math.Min(top, workArea.Bottom - height));

            // Set form's location and size
            form.Location = new Point(left, top);
            form.Width = width;
            form.Height = height;
        }

        public static int GetSystemDpi()
        {
            using (Graphics screen = Graphics.FromHwnd(IntPtr.Zero))
            {
                IntPtr hdc = screen.GetHdc();

                int virtualWidth = GetDeviceCaps(hdc, DeviceCaps.HORZRES);
                int physicalWidth = GetDeviceCaps(hdc, DeviceCaps.DESKTOPHORZRES);
                screen.ReleaseHdc(hdc);

                return (int)(96f * physicalWidth / virtualWidth);
            }
        }

        private enum DeviceCaps
        {
            /// <summary>
            /// Logical pixels inch in X
            /// </summary>
            LOGPIXELSX = 88,

            /// <summary>
            /// Horizontal width in pixels
            /// </summary>
            HORZRES = 8,

            /// <summary>
            /// Horizontal width of entire desktop in pixels
            /// </summary>
            DESKTOPHORZRES = 118
        }

        /// <summary>
        /// Retrieves device-specific information for the specified device.
        /// </summary>
        /// <param name="hdc">A handle to the DC.</param>
        /// <param name="nIndex">The item to be returned.</param>
        [DllImport("gdi32.dll")]
        private static extern int GetDeviceCaps(IntPtr hdc, DeviceCaps nIndex);
    }
}
