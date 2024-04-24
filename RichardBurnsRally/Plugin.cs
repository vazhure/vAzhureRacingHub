using vAzhureRacingAPI;

namespace RichardBurnsRally
{
    public class Plugin : ICustomPlugin
    {
        public string Name => "Richard Burns Rally Plugin";

        public string Description => "Richard Burns Rally Plugin";

        public ulong Version => 1UL;

        public bool CanClose(IVAzhureRacingApp app)
        {
            // TODO
            return true;
        }

        readonly RBRGame game = new RBRGame();

        public bool Initialize(IVAzhureRacingApp app)
        {
            // TODO
            app.RegisterGame(game);
            return true;
        }

        public void Quit(IVAzhureRacingApp app)
        {
            game?.OnQuit();
        }
    }
}