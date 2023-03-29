using vAzhureRacingAPI;

namespace iRacingPlugin
{
    public class AiRacingPlugin : ICustomPlugin
    {
        readonly IRacingGame game = new IRacingGame();

        public string Name => "iRacing";

        public string Description => "Плагин iRacing для vAzhure Racing Hub\nАвтор: Журавлев Андрей";

        public ulong Version => 1L;

        public bool CanClose(IVAzhureRacingApp app)
        {
            return true;
        }

        public bool Initialize(IVAzhureRacingApp app)
        {
            app.RegisterGame(game);
            try
            {
                game.Initialize(app);
                return true;
            }
            catch { return false; }
        }

        public void Quit(IVAzhureRacingApp app)
        {
            game.Quit(app);
        }
    }
}
