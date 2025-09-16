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

        public bool Initialize(IVAzhureRacingApp app)
        {
            game = new DCSGame();

            app.RegisterGame(game);

            return true;
        }

        public void Quit(IVAzhureRacingApp app)
        {
            game.Stop();
        }
    }
}
