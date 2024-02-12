/// Author:
/// Zhuravlev Andrey
/// E-mail: v.azhure@gmail.com
///
/// This file is subject to the terms and conditions defined in
/// file 'LICENSE.txt', which is part of this source code package.
///

using System.Runtime.InteropServices;

namespace Codemasters.Structs
{
    public enum CodeMastersTeamID
    {
        RedBull = 0,
        Ferari = 1,
        McLaren = 2,
        Renault = 3,
        Merc = 4,
        Sauber = 5,
        ForceIndia = 6,
        Williams = 7,
        ToroRosso = 8,
        Haas = 11,
        Manor = 12,
    }
    public enum CodemastersTracksID
    {
        Australia = 0,
        Malaysia = 1,
        China = 2,
        Bahrain = 3,
        Spain = 4,
        Monaco = 5,
        Canada = 6,
        Britain = 7,
        Germany = 8,
        Hungary = 9,
        Belgium = 10,
        Italy = 11,
        Singapore = 12,
        Japan = 13,
        AbuDhabi = 14,
        USA = 15,
        Brazil = 16,
        Austria = 17,
        Russia = 18,
        Mexico = 19,
        Azerbaijan = 20,
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class DIRT_4_DATA_MODE2
    {
        public float total_time;
        public float lap_time;
        public float lap_distance;
        public float total_distance;
        public float position_x;
        public float position_y;
        public float position_z;
        public float speed;
        public float velocity_x;
        public float velocity_y;
        public float velocity_z;
        public float left_dir_x;
        public float left_dir_y;
        public float left_dir_z;
        public float forward_dir_x;
        public float forward_dir_y;
        public float forward_dir_z;
        public float suspension_position_bl;
        public float suspension_position_br;
        public float suspension_position_fl;
        public float suspension_position_fr;
        public float suspension_velocity_bl;
        public float suspension_velocity_br;
        public float suspension_velocity_fl;
        public float suspension_velocity_fr;
        public float wheel_patch_speed_bl;
        public float wheel_patch_speed_br;
        public float wheel_patch_speed_fl;
        public float wheel_patch_speed_fr;
        public float throttle_input;
        public float steering_input;
        public float brake_input;
        public float clutch_input;
        public float gear;
        public float gforce_lateral;
        public float gforce_longitudinal;
        public float lap;
        public float engine_rate;
        public float native_sli_support;
        public float race_position;
        public float kers_level;
        public float kers_level_max;
        public float drs;
        public float traction_control;
        public float abs;
        public float fuel_in_tank;
        public float fuel_capacity;
        public float in_pits;
        public float race_sector;
        public float sector_time_1;
        public float sector_time_2;
        public float brake_temp_bl;
        public float brake_temp_br;
        public float brake_temp_fl;
        public float brake_temp_fr;
        public float tyre_pressure_bl;
        public float tyre_pressure_br;
        public float tyre_pressure_fl;
        public float tyre_pressure_fr;
        public float laps_completed;
        public float total_laps;
        public float track_length;
        public float last_lap_time;

        public static DIRT_4_DATA_MODE2 FromBytes(byte[] data)
        {
            DIRT_4_DATA_MODE2 d4 = new DIRT_4_DATA_MODE2();
            try
            {
                GCHandle h = GCHandle.Alloc(data, GCHandleType.Pinned);
                d4 = (DIRT_4_DATA_MODE2)Marshal.PtrToStructure(h.AddrOfPinnedObject(), typeof(DIRT_4_DATA_MODE2));
                h.Free();
            }
            catch { }

            return d4;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class DIRT_4_DATA_MODE3
    {
        [MarshalAs(UnmanagedType.R4)]
        public float total_time;
        /// <summary>
        /// Current lap time in seconds
        /// </summary>
        [MarshalAs(UnmanagedType.R4)]
        public float lap_time; 
        [MarshalAs(UnmanagedType.R4)]
        public float lap_distance;
        [MarshalAs(UnmanagedType.R4)]
        public float total_distance;
        /// <summary>
        /// World space position (X, Y, Z)
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.R4, SizeConst = 3)]
        public float [] position;
        /// <summary>
        /// Car speed in Meters per-second
        /// </summary>
        [MarshalAs(UnmanagedType.R4)]
        public float speed;
        /// <summary>
        /// Velocity in world space (X, Y, Z)
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.R4, SizeConst = 3)]
        public float [] velocity;
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.R4, SizeConst = 3)]
        public float [] roll;
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.R4, SizeConst = 3)]
        public float [] pitch;
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.R4, SizeConst = 4)]
        public float [] suspension_position;
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.R4, SizeConst = 4)]
        public float [] suspension_velocity;
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.R4, SizeConst = 4)]
        public float [] wheel_patch_speed;
        /// <summary>
        /// Throttle pedal position
        /// </summary>
        [MarshalAs(UnmanagedType.R4)]
        public float throttle_input;
        /// <summary>
        /// Steering wheel position
        /// </summary>
        [MarshalAs(UnmanagedType.R4)]
        public float steering_input;
        /// <summary>
        /// Brake pedal position
        /// </summary>
        [MarshalAs(UnmanagedType.R4)]
        public float brake_input;
        /// <summary>
        /// Clutch pedal position
        /// </summary>
        [MarshalAs(UnmanagedType.R4)]
        public float clutch_input;
        /// <summary>
        /// Selected Gear
        /// </summary>
        [MarshalAs(UnmanagedType.R4)]
        public float gear;
        [MarshalAs(UnmanagedType.R4)]
        public float gforce_lateral;
        [MarshalAs(UnmanagedType.R4)]
        public float gforce_longitudinal;
        /// <summary>
        /// Current lap
        /// </summary>
        [MarshalAs(UnmanagedType.R4)]
        public float lap;
        [MarshalAs(UnmanagedType.R4)]
        public float engine_rate; // speed of engine  [rpm / 10]
        [MarshalAs(UnmanagedType.R4)]
        public float native_sli_support;
        [MarshalAs(UnmanagedType.R4)]
        public float race_position;
        [MarshalAs(UnmanagedType.R4)]
        public float kers_level;
        [MarshalAs(UnmanagedType.R4)]
        public float kers_level_max;
        [MarshalAs(UnmanagedType.R4)]
        public float drs;
        [MarshalAs(UnmanagedType.R4)]
        public float traction_control;
        [MarshalAs(UnmanagedType.R4)]
        public float abs;
        [MarshalAs(UnmanagedType.R4)]
        public float fuel_in_tank;
        [MarshalAs(UnmanagedType.R4)]
        public float fuel_capacity;
        /// <summary>
        /// // 0 = none, 1 = pitting, 2 = in pit area, 9.5 - race, 10 = time trail / time attack, 170 = quali, practice, championsmode
        /// </summary>
        [MarshalAs(UnmanagedType.R4)]
        public float in_pits;
        [MarshalAs(UnmanagedType.R4)]
        public float race_sector;
        [MarshalAs(UnmanagedType.R4)]
        public float sector_time_1;
        [MarshalAs(UnmanagedType.R4)]
        public float sector_time_2;
        /// <summary>
        /// brakes temperature (centigrade)
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.R4, SizeConst = 4)]
        public float [] brake_temp;
        /// <summary>
        /// wheels pressure PSI
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.R4, SizeConst = 4)]
        public float [] tyre_pressure;
        [MarshalAs(UnmanagedType.R4)]
        public float laps_completed;
        [MarshalAs(UnmanagedType.R4)]
        public float total_laps;
        [MarshalAs(UnmanagedType.R4)]
        public float track_length;
        [MarshalAs(UnmanagedType.R4)]
        public float last_lap_time;
        /// <summary>
        /// Maximum RPM, Revolutions per minute
        /// </summary>
        [MarshalAs(UnmanagedType.R4)]
        public float max_rpm;
        /// <summary>
        /// Idle RPM, Revolutions per minute
        /// </summary>
        [MarshalAs(UnmanagedType.R4)]
        public float idle_rpm;
        /// <summary>
        /// Max gears
        /// </summary>
        [MarshalAs(UnmanagedType.R4)]
        public float max_gears;

        public static DIRT_4_DATA_MODE3 FromBytes(byte[] data)
        {
            DIRT_4_DATA_MODE3 d4 = new DIRT_4_DATA_MODE3();
            try
            {
                GCHandle h = GCHandle.Alloc(data, GCHandleType.Pinned);
                d4 = (DIRT_4_DATA_MODE3)Marshal.PtrToStructure(h.AddrOfPinnedObject(), typeof(DIRT_4_DATA_MODE3));
                h.Free();
            }
            catch { }

            return d4;
        }
    }
}
