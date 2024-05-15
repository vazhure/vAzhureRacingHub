/// 
/// Разработчик:
/// Журавлев Андрей Владимирович
/// e-mail: avjuravlev@yandex.ru
/// 

using System;
using System.Collections.Generic;

namespace vAzhureRacingAPI
{
    public interface IVAzhureRacingApp
    {
        /// <summary>
        /// Изменился уровень звука приложения
        /// </summary>
        event EventHandler OnSoundLevelChanged;

        event EventHandler<DeviceChangeEventsArgs> OnDeviceRemovePending;
        event EventHandler<DeviceChangeEventsArgs> OnDeviceRemoveComplete;
        event EventHandler<DeviceChangeEventsArgs> OnDeviceArrival;
        event EventHandler OnGameStarted;
        event EventHandler OnGameStopped;
        event EventHandler<TelemetryUpdatedEventArgs> OnTelemetry;

        /// <summary>
        /// Версия приложения
        /// </summary>
        ulong Version { get; }

        /// <summary>
        /// Отобразить окно сведений о программе
        /// </summary>
        void About();

        /// <summary>
        /// Рестарт приложения
        /// </summary>
        void Restart();

        /// <summary>
        /// Отобразить окно настроек приложения
        /// </summary>
        void ShowSettings();

        /// <summary>
        /// Сделать форму приложения активной
        /// </summary>
        void AcivateForm();

        System.Windows.Forms.Form MainForm { get; }

        /// <summary>
        /// Зарегистрировать устройство
        /// </summary>
        /// <param name="customDevice"></param>
        /// <returns></returns>
        bool RegisterDevice(ICustomDevicePlugin customDevice);

        /// <summary>
        /// Удалить устройство из списка зарегистрированных устройств
        /// </summary>
        /// <param name="customDevice"></param>
        /// <returns></returns>
        bool UnRegisterDevice(ICustomDevicePlugin customDevice);

        bool RegisterGame(IGamePlugin game);
        bool UnRegisterGame(IGamePlugin game);

        /// <summary>
        /// Перечень зарегистрированных устройств
        /// </summary>
        IList<ICustomDevicePlugin> CustomDevices { get; }
        IList<IGamePlugin> GamePlugins { get; }

        int ApplicationSoundVolume {get; }

        void SetStatusText(string text);
    }

    public class DeviceChangeEventsArgs : EventArgs
    {
        public DeviceChangeEventsArgs(string port) => Port = port;
        public string Port{ get; private set; }
    }
}