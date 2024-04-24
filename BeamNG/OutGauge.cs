using System;
using System.Runtime.InteropServices;

/// <summary>
/// OutGauge UDP protocol
/// Activation: can be enabled in Options > Other > Utilities > OutGauge support.
/// https://documentation.beamng.com/modding/protocols/
/// </summary>

namespace BeamNG
{
    [StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Ansi)]
    public class OutGauge
    {
        /// <summary>
        /// time in milliseconds (to check order) // N/A, hardcoded to 0
        /// </summary>
        public uint time;
        /// <summary>
        /// Car name // N/A, hardcoded to "beam"
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4)]
        public string car;
        /// <summary>
        /// Info (see OG_x below)
        /// </summary>
        public ushort flags;
        /// <summary>
        /// Reverse:0, Neutral:1, First:2...
        /// </summary>
        public byte gear;
        /// <summary>
        /// Unique ID of viewed player (0 = none) // N/A, hardcoded to 0
        /// </summary>
        public byte plid;
        /// <summary>
        /// M/S
        /// </summary>
        public float speed;
        /// <summary>
        /// RPM
        /// </summary>
        public float rpm;
        /// <summary>
        /// BAR
        /// </summary>
        public float turbo;
        /// <summary>
        /// C
        /// </summary>
        public float engTemp;
        /// <summary>
        /// 0 to 1
        /// </summary>
        public float fuel;
        /// <summary>
        /// BAR // N/A, hardcoded to 0
        /// </summary>
        public float oilPressure;
        /// <summary>
        /// C
        /// </summary>
        public float oilTemp;
        /// <summary>
        /// Dash lights available (see DL_x below)
        /// </summary>
        public uint dashLights;
        /// <summary>
        /// Dash lights currently switched on
        /// </summary>
        public uint showLights;
        /// <summary>
        /// 0 to 1
        /// </summary>
        public float throttle;
        /// <summary>
        /// 0 to 1
        /// </summary>
        public float brake;
        /// <summary>
        /// 0 to 1
        /// </summary>
        public float clutch;
        /// <summary>
        /// Usually Fuel // N/A, hardcoded to ""
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public string display1;
        /// <summary>
        /// Usually Settings // N/A, hardcoded to ""
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public string display2;
        /// <summary>
        /// optional - only if OutGauge ID is specified
        /// </summary>
        public int id;

        //CONSTANTS
        // OG_x - bits for OutGaugePack Flags
        /// <summary>
        /// key // N/A
        /// </summary>
        public const int OG_SHIFT = 1;
        /// <summary>
        /// key // N/A
        /// </summary>
        public const int OG_CTRL = 2;
        /// <summary>
        /// show turbo gauge
        /// </summary>
        public const int OG_TURBO = 8192;
        /// <summary>
        /// if not set - user prefers MILES
        /// </summary>
        public const int OG_KM = 16384;
        /// <summary>
        /// if not set - user prefers PSI
        /// </summary>
        public const int OG_BAR = 32768;

        [Flags]
        public enum OutGaugePack : ushort
        {
            DL_SHIFT = 1 << 0,            // - shift light
            DL_FULLBEAM = 1 << 1,         // - full beam
            DL_HANDBRAKE = 1 << 2,        // - handbrake
            DL_PITSPEED = 1 << 3,         // - pit speed limiter // N/A
            DL_TC = 1 << 4,               // - TC active or switched off
            DL_SIGNAL_L = 1 << 5,         // - left turn signal
            DL_SIGNAL_R = 1 << 6,         // - right turn signal
            DL_SIGNAL_ANY = 1 << 7,       // - shared turn signal // N/A
            DL_OILWARN = 1 << 8,          // - oil pressure warning
            DL_BATTERY = 1 << 9,          // - battery warning
            DL_ABS = 1 << 10,             // - ABS active or switched off
            DL_SPARE = 1 << 11            // - N / A
        }
    }
}
