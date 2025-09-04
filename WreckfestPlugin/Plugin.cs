using vAzhureRacingAPI;

namespace WreckfestPlugin
{
    public class Plugin : ICustomPlugin
    {
        private WreckFestGame wreckFestGame;
        private WreckFest2Game wreckFest2Game;

        public string Name => "Wreckfest Plugin";

        public string Description => "Wreckfest series Plugin";

        public ulong Version => 1L;

        public bool CanClose(IVAzhureRacingApp app)
        {
            return true;
        }

        public bool Initialize(IVAzhureRacingApp app)
        {
            wreckFestGame = new WreckFestGame();
            app.RegisterGame(wreckFestGame);
            wreckFest2Game = new WreckFest2Game();
            app.RegisterGame(wreckFest2Game);

            return true;
        }

        public void Quit(IVAzhureRacingApp app)
        {
            wreckFestGame.Finalize();
            wreckFest2Game.Finalize();
        }
    }
}