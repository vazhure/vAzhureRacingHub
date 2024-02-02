/// 
/// Разработчик:
/// Журавлев Андрей Владимирович
/// e-mail: avjuravlev@yandex.ru
/// 

using System;
using System.Drawing;
using System.Windows.Forms;

namespace vAzhureRacingAPI
{
    public enum DeviceStatus { Unknown = -1, Disconnected = 0, Connected = 1, ConnectionError = 2};
    public interface ICustomDevicePlugin
    {
        event EventHandler OnConnected;
        event EventHandler OnDisconnected;

        /// <summary>
        /// Имя устройства
        /// </summary>
        string DeviceName { get; }

        string DeviceDescription { get; }

        /// <summary>
        /// Уникальный идентификатор устройства
        /// </summary>
        string DeviceID { get; }

        /// <summary>
        /// Состояние устройства
        /// </summary>
        DeviceStatus Status { get; }

        /// <summary>
        /// Разрешение устройства
        /// </summary>
        bool DeviceEnabled { get; set; }

        /// <summary>
        /// Пиктограмма устройства
        /// </summary>
        Image DevicePictogram { get; }

        /// <summary>
        /// Вернуть форму настройки свойств устройства
        /// </summary>
        void ShowProperties(IVAzhureRacingApp app);

        void Initialize(IVAzhureRacingApp app);

        void OnTelemetry(IVAzhureRacingApp app, TelemetryDataSet data);
    }
}