using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using vAzhureRacingAPI;

namespace Nextion35Dash
{
    public class Nextion35Plugin : ICustomPlugin
    {
        public string Name => "Nextion 3.5\"";

        public string Description => "Плагин поддержки приборной панели Nextion\nАвтор: Журавлев Андрей";

        public ulong Version => 1;

        private readonly NextionDevice nextionDevice = new NextionDevice();
        private readonly AlpineWheelPlateDevice alpineDevice = new AlpineWheelPlateDevice();

        internal static IVAzhureRacingApp sApp = null;
        internal static Nextion35Plugin sPlugin = null;

        public bool CanClose(IVAzhureRacingApp app)
        {
            return true;
        }

        public void SetPrimaryDevice(DeviceClass device)
        {
            foreach (DeviceClass deviceClass in devices)
            {
                deviceClass.SetPrimaryDevice(deviceClass == device);
            }    
        }

        public DeviceClass GetPrimaryDevice()
        {
            IEnumerable<DeviceClass> connectedDevices = devices.Where(d => d.IsConnected);

            if (connectedDevices.Count() == 1)
                return connectedDevices.First();

            return devices.Where(d => d.IsPrimaryDevice).FirstOrDefault();
        }

        readonly List<DeviceClass> devices = new List<DeviceClass>();

        public bool Initialize(IVAzhureRacingApp app)
        {
            sApp = app;
            sPlugin = this;
            nextionDevice.LoadSettings(AssemblyPath);
            app.RegisterDevice(nextionDevice);
            alpineDevice.LoadSettings(AssemblyPath);
            app.RegisterDevice(alpineDevice);

            devices.Add(nextionDevice);
            devices.Add(alpineDevice);

            app.OnDeviceArrival += delegate (object sender, DeviceChangeEventsArgs e)
              {
                  if (!nextionDevice.IsConnected)
                  {
                      if ($"COM{nextionDevice.Settings.ComPort}" == e.Port.ToUpper())
                          nextionDevice.Connect();
                  }
                  if (!alpineDevice.IsConnected)
                  {
                      if ($"COM{alpineDevice.Settings.ComPort}" == e.Port.ToUpper())
                          alpineDevice.Connect();
                  }
              };

            app.OnDeviceRemoveComplete += delegate (object sender, DeviceChangeEventsArgs e)
            {
                if (nextionDevice.IsConnected)
                {
                    if ($"COM{nextionDevice.Settings.ComPort}" == e.Port.ToUpper())
                        nextionDevice.Disconnect();
                }
                if (alpineDevice.IsConnected)
                {
                    if ($"COM{alpineDevice.Settings.ComPort}" == e.Port.ToUpper())
                        alpineDevice.Disconnect();
                }
            };

            nextionDevice.Connect();
            alpineDevice.Connect();

            return true;
        }

        /// <summary>
        /// Полный путь к DLL сборки
        /// </summary>
        public static string AssemblyPath
        {
            get
            {
                return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            }
        }

        public void Quit(IVAzhureRacingApp app)
        {
            nextionDevice.Disconnect();
            nextionDevice.SaveSettings(AssemblyPath);
            alpineDevice.Disconnect();
            alpineDevice.SaveSettings(AssemblyPath);
        }
    }

    public class DeviceClass
    {
        virtual public bool IsConnected { get; } = false;
        virtual public bool IsPrimaryDevice { get; } = false;
        virtual public void SetPrimaryDevice(bool bSet = true) { }
    }
}
