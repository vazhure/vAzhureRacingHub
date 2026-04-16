using Kunos.Structs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
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
        readonly GamePlugin acrally = new GamePlugin(GamePlugin.GameID.ACRALLY);

        GameListener listener;
        public bool Initialize(IVAzhureRacingApp app)
        {
            app.RegisterGame(ac);
            app.RegisterGame(acevo);
            app.RegisterGame(acc);
            app.RegisterGame(acrally);

            string filename = Path.Combine(AssemblyPath, cKunosData);

            sVechicleInfo = VechicleInfo.Load(filename);

            listener = new GameListener(new GamePlugin[] { ac, acc, acevo, acrally });
            listener.StartThread();

#if DEBUG
            Console.WriteLine($"TyreState      : {Marshal.SizeOf<SMEvoTyreState>()} (256)");
            Console.WriteLine($"DamageState    : {Marshal.SizeOf<SMEvoDamageState>()} (128)");
            Console.WriteLine($"PitInfo        : {Marshal.SizeOf<SMEvoPitInfo>()} (64)");
            Console.WriteLine($"Electronics    : {Marshal.SizeOf<SMEvoElectronics>()} (128)");
            Console.WriteLine($"Instrumentation: {Marshal.SizeOf<SMEvoInstrumentation>()} (128)");
            Console.WriteLine($"SessionState   : {Marshal.SizeOf<SMEvoSessionState>()} (256)");
            Console.WriteLine($"TimingState    : {Marshal.SizeOf<SMEvoTimingState>()} (256)");
            Console.WriteLine($"AssistsState   : {Marshal.SizeOf<SMEvoAssistsState>()} (64)");
            Console.WriteLine($"Physics        : {Marshal.SizeOf<SPageFilePhysics>()} (800)");
            Console.WriteLine($"StaticEvo      : {Marshal.SizeOf<SPageFileStaticEvo>()} (208)");
            Console.WriteLine($"GraphicEvo     : {Marshal.SizeOf<SPageFileGraphicEvo>()}");
#endif

            return true;
        }

        public void Quit(IVAzhureRacingApp app)
        {
            listener.StopTrhead();
            listener = null;
        }
    }
}
