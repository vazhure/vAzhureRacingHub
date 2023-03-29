/// 
/// Разработчик:
/// Журавлев Андрей Владимирович
/// e-mail: avjuravlev@yandex.ru
/// 

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace vAzhureRacingHub
{
    static class Program
    {
        /// <summary>
        /// Главное окно программы
        /// </summary>
        static Form sMainForm = null;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        //[MTAThread]
        static void Main()
        {
            // Предотвращаем повторный запуск
            try
            {
                Process[] list = Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName);
                if (list.Length > 1)
                {
                    foreach (Process p in list)
                    {
                        if (p != Process.GetCurrentProcess())
                            SetForegroundWindow(p.MainWindowHandle);
                    }
                    
                    return;
                }
            }
            catch { }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(sMainForm = new AppWindow());
        }

        /// <summary>
        /// Перезапуск программы
        /// </summary>
        public static void Restart()
        {
            sMainForm?.Close();
            Application.Restart();
            Environment.Exit(0);
        }

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
    }
}
