using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vAzhureRacingAPI;

namespace TrucksPlugin
{
    public class Plugin : ICustomPlugin
    {
        public string Name => "Truck Simulators Plugin";

        public string Description => "ATS and ETS Simulators Plugin";

        public ulong Version => 1UL;

        readonly TruckGame ets2 = new TruckGame(TruckGame.GameID.ETS2);
        readonly TruckGame ats = new TruckGame(TruckGame.GameID.ATS);

        public bool CanClose(IVAzhureRacingApp app)
        {
            return true;
        }

        public bool Initialize(IVAzhureRacingApp app)
        {
            app.RegisterGame(ats);
            app.RegisterGame(ets2);
            
            return true;
        }

        public void Quit(IVAzhureRacingApp app)
        {
            ats.Dispose();
            ets2.Dispose();
        }
    }
}
