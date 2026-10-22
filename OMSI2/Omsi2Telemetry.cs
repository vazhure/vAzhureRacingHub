using System.Runtime.InteropServices;

namespace Omsi2TelemetryPlugin
{
    public class OmsiUdpPacket
    {
        public float throttle { get; set; }
        public float brake { get; set; }
        public float clutch { get; set; }
        public float brakeforce { get; set; }
        public float velocity { get; set; }
        public float surge { get; set; }
        public float sway { get; set; }
        public float heave { get; set; }
        public float roll { get; set; }
        public float pitch { get; set; }
        public float yaw { get; set; }
        public float engine_n { get; set; }
        public float engine_on { get; set; }
        public float engine_tank_content { get; set; }
        public float lights_fern { get; set; }
        public float AI_Blinker_L { get; set; }
        public float AI_Blinker_R { get; set; }
        public float cockpit_gangR { get; set; }
        public float cockpit_gangN { get; set; }
        public float cockpit_gang1 { get; set; }
        public float cockpit_gang2 { get; set; }
        public float cockpit_gang3 { get; set; }
        public float pause { get; set; }
    }

    public static class Omsi2Telemetry
    {
        public const string SharedMemoryName = "$OMSI2_SHARED_MEM";
        public static readonly int DataSize = Marshal.SizeOf(typeof(TelemetryState));

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct TelemetryState
        {
            // === Исходные данные от OMSI 2 ===
            public float Throttle;
            public float Brake;
            public float Clutch;
            public float Brakeforce;
            public float Velocity;

            public float Axle_Suspension_0_L;
            public float Axle_Suspension_0_R;
            public float Axle_Suspension_1_L;
            public float Axle_Suspension_1_R;

            public float A_Trans_X;
            public float A_Trans_Y;
            public float A_Trans_Z;

            public float elec_busbar_main;
            public float engine_n;
            public float haltewunsch;
            public float lights_fern;
            public float lights_blinkgeber;
            public float cockpit_light_batterie;
            public float cockpit_oeldruck;
            public float cockpit_light_feststellbremse;
            public float engine_temperature;
            public float cockpit_light_masterfailure;
            public float engine_tank_content;
            public float bremse_ABS_eingriff;
            public float cockpit_light_tuerkontrolle;
            public float lights_warnblinkgeber;
            public float cockpit_light_retarder_direkt;
            public float lights_stand;
            public float cockpit_light_ASR_off;
            public float engine_ASR_eingriff;
            public float kmcounter_km;
            public float kmcounter_m;
            public float Cabinair_Temp;
            public float cockpit_uhr_sek;
            public float haltewunschlampe;
            public float AI_Blinker_L;
            public float AI_Blinker_R;
            public float AI_Light;
            public float AI_Interiorlight;
            public float engine_on;
            public float tank_percent;
            public float cockpit_gangR;
            public float cockpit_gangN;
            public float cockpit_gang1;
            public float cockpit_gang2;
            public float cockpit_gang3;
            public float engine_tank_capacity;

            // Системные (pause — float, как в C++)
            public float pause;
            public float Weather_Temperature;
            public float Time;
            public float Day;
            public float Month;
            public float Year;
            public float DayOfYear;

            // === ВЫЧИСЛЯЕМЫЕ ПОЛЯ ===
            public float Surge;      // Продольное ускорение
            public float Sway;       // Боковое ускорение
            public float Heave;      // Вертикальное ускорение
            public float Pitch;      // Тангаж (градусы)
            public float Roll;       // Крен (градусы)
            public float Yaw;        // Рыскание (градусы)
            public float angle_coefficient; // Коэффициент вычислений
        }

        public static class Helpers
        {
            public static TelemetryState ReadFromBytes(byte[] data)
            {
                GCHandle handle = GCHandle.Alloc(data, GCHandleType.Pinned);
                try
                {
                    return Marshal.PtrToStructure<TelemetryState>(handle.AddrOfPinnedObject());
                }
                finally
                {
                    handle.Free();
                }
            }
        }

    }
}