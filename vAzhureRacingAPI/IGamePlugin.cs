using System;
using System.Drawing;

namespace vAzhureRacingAPI
{
    public interface IGamePlugin
    {
        event EventHandler<TelemetryUpdatedEventArgs> OnTelemetry;
        event EventHandler OnGameStateChanged;
        event EventHandler OnGameIconChanged;

        /// <summary>
        /// Имя игры
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// Идентификатор игры в магазине Steam
        /// </summary>
        uint SteamGameID { get; }
        /// <summary>
        /// Имя или имена процессов игры
        /// </summary>
        string [] ExecutableProcessName { get; }

        /// <summary>
        /// Пользовательская иконка
        /// </summary>
        string UserIconPath { get; set; }

        /// <summary>
        /// Пользовательский путь к исполняемому файлу
        /// </summary>
        string UserExecutablePath { get; set; }

        /// <summary>
        /// Запуск приложения
        /// </summary>
        void Start(IVAzhureRacingApp app);

        void ShowSettings(IVAzhureRacingApp app);

        bool IsRunning { get; }

        /// <summary>
        /// Пиктограмма игры
        /// </summary>
        /// <returns></returns>
        Icon GetIcon();
    }
}
