using System;
using vAzhureRacingAPI;

namespace RaceRoomPlugin
{
    public class R3EPlugin : ICustomPlugin
    {
        public string Name => "RaceRoom Racing Experience";

        public string Description => "Плагин RaceRoom Racing Experience\nАвтор: Журавлев Андрей";

        public ulong Version => 1;

        public bool CanClose(IVAzhureRacingApp app)
        {
            return true;
        }

        readonly R3EListener game = new R3EListener();

        public bool Initialize(IVAzhureRacingApp app)
        {
            app.RegisterGame(game);

            game.StartThread();
            game.OnThreadError += delegate (object sender, EventArgs e)
            {
                app.SetStatusText($"Ошибка процесса плагина {Name}");
                // Перезапуск процесса
                game.StopTrhead();
                System.Threading.Thread.Sleep(1000); // Делаем паузу, чтоб не загружать процессор
                game.StartThread();
            };
            return true;
        }

        public void Quit(IVAzhureRacingApp app)
        {
            game?.StopTrhead();
        }
    }
}
