using System;

namespace WreckfestPlugin
{
    [Serializable]
    public class GameSettings
    {
        public string Executable { get; set; } = string.Empty;
        public string IP { get; set; } = "127.0.0.1";
        public int PortUDP { get; set; } = 23123;
    }

    public class Config
    {
        public class Udp
        {
            public int enabled { get; set; } = 1;
            public string ip { get; set; } = "127.0.0.1";
            public int port { get; set; } = 23123;
        }
        public class Logging
        {
            public string format = "none";
        }
        public Udp[] udp { get; set; } = { new Udp() };
        public Logging[] logging { get; set; } = { new Logging() };
    }
}