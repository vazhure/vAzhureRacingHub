using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using vAzhureRacingAPI;

namespace KunosPlugin
{
    public class Plugin : ICustomPlugin
    {
        public string Name => "Kunos Games";

        public string Description => "Плагин игр AC и ACC для vAzhure Racing Hub\nАвтор: Журавлев Андрей Владимирович";

        internal const string cKunosData = "vehicle.json";

        internal static VechicleInfo sVechicleInfo = new VechicleInfo();

        public ulong Version => 1L;

        public bool CanClose(IVAzhureRacingApp app)
        {
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

        readonly GamePlugin acc = new GamePlugin(GamePlugin.GameID.ACC);
        readonly GamePlugin ac = new GamePlugin(GamePlugin.GameID.AC);
        readonly GamePlugin acevo = new GamePlugin(GamePlugin.GameID.ACEVO);

        GameListener listener;
        public bool Initialize(IVAzhureRacingApp app)
        {
            app.RegisterGame(ac);
            app.RegisterGame(acevo);
            app.RegisterGame(acc);

            string filename = Path.Combine(AssemblyPath, cKunosData);

            sVechicleInfo = VechicleInfo.Load(filename);

            listener = new GameListener(new GamePlugin[] { ac, acc, acevo });
            listener.StartThread();

            return true;
        }

        public void Quit(IVAzhureRacingApp app)
        {
            listener.StopTrhead();
            listener = null;
        }
    }
}
