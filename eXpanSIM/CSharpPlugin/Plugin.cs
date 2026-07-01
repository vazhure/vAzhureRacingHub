using vAzhureRacingAPI;

namespace eXpanSIM
{
    /// <summary>
    /// vAzhureRacingHub custom plugin entry point for eXpanSIM.
    /// </summary>
    public class Plugin : ICustomPlugin
    {
        public string Name => "eXpanSIM";
        public string Description => "eXpanSIM universal vehicle simulator plugin for vAzhure Racing Hub";
        public ulong Version => 1UL;

        private eXpanSIMGame game;

        public bool CanClose(IVAzhureRacingApp app) => true;

        public bool Initialize(IVAzhureRacingApp app)
        {
            game = new eXpanSIMGame();
            app.RegisterGame(game);
            return true;
        }

        public void Quit(IVAzhureRacingApp app)
        {
            game?.Dispose();
        }
    }
}
