using vAzhureRacingAPI;

namespace Omsi2TelemetryPlugin
{
    public class Omsi2Plugin : ICustomPlugin
    {
        public string Name => "OMSI 2 Telemetry";
        public string Description => "OMSI 2 Shared Memory Telemetry Plugin for vAzhure Racing Hub";
        public ulong Version => 1UL;

        private Omsi2Game omsi2Game;

        public bool Initialize(IVAzhureRacingApp app)
        {
            try
            {
                omsi2Game = new Omsi2Game();
                app.RegisterGame(omsi2Game);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void Quit(IVAzhureRacingApp app)
        {
            omsi2Game = null;
        }

        public bool CanClose(IVAzhureRacingApp app) => true;
    }
}