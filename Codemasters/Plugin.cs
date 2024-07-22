using System.Collections.Generic;
using vAzhureRacingAPI;

namespace Codemasters
{
    public class Plugin : ICustomPlugin
    {
        public string Name => "Codemasters Games Plugin";

        public string Description => "Codemasters Games Plugin";

        public ulong Version => 1L;

        public bool CanClose(IVAzhureRacingApp app)
        {
            return true;
        }

        private readonly List<GamePlugin> games = new List<GamePlugin>() 
        { 
            new GamePlugin(GamePlugin.CodemastersGame.DIRT4), 
            new GamePlugin(GamePlugin.CodemastersGame.DIRTRALLY), 
            new GamePlugin(GamePlugin.CodemastersGame.DIRTRALLY20),
            new GamePlugin(GamePlugin.CodemastersGame.WRCG),
            new GamePlugin(GamePlugin.CodemastersGame.EAWRC),
            new GamePlugin(GamePlugin.CodemastersGame.GRID2019),
            new GamePlugin(GamePlugin.CodemastersGame.GRIDLEGENDS),
        };

        public bool Initialize(IVAzhureRacingApp app)
        {
            bool bResult = true;
            foreach (GamePlugin game in games)
                bResult &= app.RegisterGame(game);

            return bResult;
        }

        public void Quit(IVAzhureRacingApp app)
        {
            foreach (GamePlugin game in games)
            {
                game?.Dispose();
            }
        }
    }
}
