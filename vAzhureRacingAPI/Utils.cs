using System.Diagnostics;

namespace vAzhureRacingAPI
{
    public static class Utils
    {
        /// <summary>
        /// Запуск игры Steam по ID 
        /// </summary>
        /// <param name="steamID"></param>
        /// <returns></returns>
        public static bool RunSteamGame(uint steamID)
        {
            string cmd = $"steam://rungameid/{steamID}";

            if (!IsProcessRunning(new string[] { "steam", "SteamService" }))
            {
                return false;
            }

            try
            {
                Process.Start(cmd);
            }
            catch 
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Выполнение команды
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public static bool ExecuteCmd(string cmd)
        {
            try
            {
                Process.Start(cmd);
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Проверяет запущен ли какой либо из процессов с именем из списка {names}
        /// </summary>
        /// <param name="names">Имена процессов</param>
        /// <returns></returns>
        public static bool IsProcessRunning(string[] names)
        {
            try
            {
                foreach (string name in names)
                {
                    if (Process.GetProcessesByName(name).Length > 0)
                        return true;
                }
            }
            catch { }
            return false;
        }
    }
}
