using vAzhureRacingAPI;

namespace Race07Plugin
{
    public class Plugin : ICustomPlugin
    {
        public string Name => "Race 07 / GTR Plugin";

        public string Description => "Race 07 / GTR Plugin";

        public ulong Version => 1UL;

        private readonly Race07Game gtr = new Race07Game(true);
        private readonly Race07Game race07 = new Race07Game(false);

        public bool CanClose(IVAzhureRacingApp _)
        {
            return true;
        }

        public bool Initialize(IVAzhureRacingApp app)
        {
            app.RegisterGame(gtr);
            app.RegisterGame(race07);
            return true;
        }

        public void Quit(IVAzhureRacingApp app)
        {
            try
            {
                gtr.Quit();
                race07.Quit();
            }
            catch
            {

            }
        }
    }
}
