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
        public float[] position;
        /// <summary>
        /// Car speed in Meters per-second
        /// </summary>
        [MarshalAs(UnmanagedType.R4)]
        public float speed;
        /// <summary>
        /// Velocity in world space (X, Y, Z), WorldSpeed
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.R4, SizeConst = 3)]
        public float[] velocity;

        //VehicleRightDirectionX float32 // World space right direction
        //VehicleRightDirectionY float32 // World space right direction
        //VehicleRightDirectionZ float32 // World space right direction
        //VehicleForwardDirectionX float32 // World space forward direction
        //VehicleForwardDirectionY float32 // World space forward direction
        //VehicleForwardDirectionZ float32 // World space forward direction
        /// <summary>
        /// //left_dir, XR, Roll, ZR
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.R4, SizeConst = 3)]
        public float[] roll; 
        /// <summary>
        /// forward_dir, XD, YD, ZD
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.R4, SizeConst = 3)]
        public float[] pitch; 
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.R4, SizeConst = 4)]
        public float[] suspension_position;
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.R4, SizeConst = 4)]
        public float[] suspension_velocity;
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.R4, SizeConst = 4)]
        public float[] wheel_patch_speed;
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
        public float[] brake_temp;
        /// <summary>
        /// wheels pressure PSI
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.R4, SizeConst = 4)]
        public float[] tyre_pressure;
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
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct WRC_DATA
    {
        public long packet_uid;
        public float game_total_time;
        public float game_delta_time;
        public long game_frame_count;
        public float shiftlights_fraction;
        public float shiftlights_rpm_start;
        public float shiftlights_rpm_end;
        public byte shiftlights_rpm_valid;
        public byte vehicle_gear_index;
        public byte vehicle_gear_index_neutral;
        public byte vehicle_gear_index_reverse;
        public byte vehicle_gear_maximum;
        public float vehicle_speed;
        public float vehicle_transmission_speed;
        public float vehicle_position_x;
        public float vehicle_position_y;
        public float vehicle_position_z;
        public float vehicle_velocity_x;
        public float vehicle_velocity_y;
        public float vehicle_velocity_z;
        public float vehicle_acceleration_x;
        public float vehicle_acceleration_y;
        public float vehicle_acceleration_z;
        /// <summary>
        /// Left
        /// </summary>
        public float vehicle_roll_x; 
        public float vehicle_roll_y;
        public float vehicle_roll_z;
        /// <summary>
        /// Forward
        /// </summary>
        public float vehicle_pitch_x;
        public float vehicle_pitch_y;
        public float vehicle_pitch_z;
        /// <summary>
        /// UP
        /// </summary>
        public float vehicle_yaw_x; 
        public float vehicle_yaw_y;
        public float vehicle_yaw_z;
        public float vehicle_hub_position_bl;
        public float vehicle_hub_position_br;
        public float vehicle_hub_position_fl;
        public float vehicle_hub_position_fr;
        public float vehicle_hub_velocity_bl;
        public float vehicle_hub_velocity_br;
        public float vehicle_hub_velocity_fl;
        public float vehicle_hub_velocity_fr;
        public float vehicle_cp_forward_speed_bl;
        public float vehicle_cp_forward_speed_br;
        public float vehicle_cp_forward_speed_fl;
        public float vehicle_cp_forward_speed_fr;
        public float vehicle_brake_temperature_bl;
        public float vehicle_brake_temperature_br;
        public float vehicle_brake_temperature_fl;
        public float vehicle_brake_temperature_fr;
        public float vehicle_engine_rpm_max;
        public float vehicle_engine_rpm_idle;
        public float vehicle_engine_rpm_current;
        public float vehicle_throttle;
        public float vehicle_brake;
        public float vehicle_clutch;
        public float vehicle_steering;
        public float vehicle_handbrake;
        public float stage_current_time;
        public double stage_current_distance;
        public double stage_length;
    }
}