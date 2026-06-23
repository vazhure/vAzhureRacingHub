using IL2;
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
        private IL2Game il2Game;

        public bool Initialize(IVAzhureRacingApp app)
        {
            game = new DCSGame();
            xPlaneGame11 = new XPlaneGame();
            xPlaneGame12 = new XPlaneGame(12);
            il2Game = new IL2Game();

            app.RegisterGame(game);
            app.RegisterGame(xPlaneGame11);
            app.RegisterGame(xPlaneGame12);
            app.RegisterGame(il2Game);

            return true;
        }

        public void Quit(IVAzhureRacingApp app)
        {
            game.Stop();
            game.Dispose();
            xPlaneGame11.Stop();
            xPlaneGame12.Stop();
            il2Game.Dispose();
        }
    }
}