using System;
using vAzhureRacingAPI;

namespace PCars2
{
    public class PCars2Plugin : ICustomPlugin
    {
        public string Name => "PCars2 / AMS2";

        public string Description => "Плагин Project Cars 2 / Automobilista 2 для vAzhure Racing Hub\nАвтор: Журавлев Андрей";

        public ulong Version => 1L;

        public bool CanClose(IVAzhureRacingApp _)
        {
            return true;
        }

        readonly GamePlugin ams2 = new GamePlugin(GamePlugin.GameID.AMS2);
        readonly GamePlugin pc2 = new GamePlugin(GamePlugin.GameID.PC2);
        PC2AMS2Listener listener;

        public bool Initialize(IVAzhureRacingApp app)
        {
            app.RegisterGame(ams2);
            app.RegisterGame(pc2);

            listener = new PC2AMS2Listener(ams2, pc2);
            listener.StartThread();
            listener.OnThreadError += delegate (object sender, EventArgs e)
            {
                app.SetStatusText($"Ошибка процесса плагина {Name}");
                // Перезапуск процесса
                listener.StopTrhead();
                System.Threading.Thread.Sleep(1000); // Делаем паузу, чтоб не загружать процессор
                listener.StartThread();
            };

            return true;
        }

        public void Quit(IVAzhureRacingApp app)
        {
            listener?.StopTrhead();
        }
    }
}
