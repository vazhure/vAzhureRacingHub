using vAzhureRacingAPI;

namespace WreckfestPlugin
{
    public class Plugin : ICustomPlugin
    {
        private readonly WreckFestGame wreckFestGame = new WreckFestGame();
        private readonly WreckFest2Game wreckFest2Game = new WreckFest2Game();

        public string Name => "Wreckfest Plugin";

        public string Description => "Wreckfest series Plugin";

        public ulong Version => 1L;

        public bool CanClose(IVAzhureRacingApp app)
        {
            return true;
        }

        public bool Initialize(IVAzhureRacingApp app)
        {
            app.RegisterGame(wreckFestGame);
            app.RegisterGame(wreckFest2Game);

            return true;
        }

        public void Quit(IVAzhureRacingApp app)
        {
            wreckFestGame.Finalize();
        }
    }
}