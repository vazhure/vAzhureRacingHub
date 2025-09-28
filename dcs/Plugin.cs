using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vAzhureRacingAPI;

namespace DCS
{
    public class Plugin : ICustomPlugin
    {
        public string Name => "DCS World";

        public string Description => "DCS World Plugin";

        public ulong Version => 1UL;

        public bool CanClose(IVAzhureRacingApp app)
        {
            return true;
        }

        private DCSGame game;
        private XPlaneGame xPlaneGame11;
        private XPlaneGame xPlaneGame12;

        public bool Initialize(IVAzhureRacingApp app)
        {
            game = new DCSGame();
            xPlaneGame11 = new XPlaneGame();
            xPlaneGame12 = new XPlaneGame(12);

            app.RegisterGame(game);
            app.RegisterGame(xPlaneGame11);
            app.RegisterGame(xPlaneGame12);

            return true;
        }

        public void Quit(IVAzhureRacingApp app)
        {
            game.Stop();
            game.Dispose();
            xPlaneGame11.Stop();
            xPlaneGame12.Stop();
        }
    }
}
