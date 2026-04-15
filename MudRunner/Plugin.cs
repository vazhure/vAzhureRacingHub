using System;
using vAzhureRacingAPI;

namespace MudRunner
{
    /// <summary>
    /// Plugin entry point for vAzhureRacingAPI framework.
    /// 
    /// This plugin registers MudRunner as a supported game in the
    /// vAzhureRacingAPI system, enabling telemetry data collection
    /// and dashboard display for MudRunner.
    /// 
    /// </summary>
    public class Plugin : ICustomPlugin
    {
        public string Name => "MudRunner Game Plugin";

        public string Description => "Telemetry plugin for MudRunner (Spintires: MudRunner). " +
            "Requires the telemetry.lua mod appended to truck.lua. " +
            "Receives data via file polling (telemetry_out.txt).";

        public ulong Version => 1UL;

        /// <summary>
        /// Called when the plugin is loaded by vAzhureRacingAPI.
        /// Registers MudRunner as a supported game.
        /// </summary>
        public bool Initialize(IVAzhureRacingApp app)
        {
            try
            {
                var mudRunnerGame = new MudRunnerGame();
                app.RegisterGame(mudRunnerGame);
#if DEBUG
                Console.WriteLine("[MudRunner Plugin] Successfully registered MudRunner game.");
                Console.WriteLine($"[MudRunner Plugin] Transport: file ({mudRunnerGame.TelemetryFileName})");
                Console.WriteLine("[MudRunner Plugin] Version: 3 (file-based, pipe blocked by game)");
#endif
                return true;
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine($"[MudRunner Plugin] Initialization failed: {ex.Message}");
#endif
                return false;
            }
        }

        /// <summary>
        /// Called when the plugin is about to be unloaded.
        /// </summary>
        public void Quit(IVAzhureRacingApp app)
        {
#if DEBUG
            Console.WriteLine("[MudRunner Plugin] Shutting down.");
#endif
        }

        /// <summary>
        /// Called to check if the plugin can be safely closed.
        /// </summary>
        public bool CanClose(IVAzhureRacingApp app)
        {
            return true;
        }
    }
}
