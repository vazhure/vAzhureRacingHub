/// 
/// Разработчик:
/// Журавлев Андрей Владимирович
/// e-mail: avjuravlev@yandex.ru
/// 

namespace vAzhureRacingAPI
{
    public interface ICustomPlugin
    {
        /// <summary>
        /// Название плагина
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Краткое описание
        /// </summary>
        string Description { get; }
        /// <summary>
        /// Версия плагина
        /// </summary>
        ulong Version { get; }
        /// <summary>
        /// Инициализация плагина
        /// </summary>
        /// <param name="app"></param>
        bool Initialize(IVAzhureRacingApp app);
        /// <summary>
        /// Запрос на завершение приложения
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        bool CanClose(IVAzhureRacingApp app);
        /// <summary>
        /// Завершение приложения
        /// </summary>
        /// <param name="app"></param>
        void Quit(IVAzhureRacingApp app);
    }
}
