#pragma once

enum VAR_ENUM
{
    Throttle,
    Brake,
    Clutch,
    Brakeforce,
    Velocity,
    Axle_Suspension_0_L,
    Axle_Suspension_0_R,
    Axle_Suspension_1_L,
    Axle_Suspension_1_R,
    A_Trans_X,
    A_Trans_Y,
    A_Trans_Z,
    elec_busbar_main,
    engine_n,
    haltewunsch,
    lights_fern,
    lights_blinkgeber,
    cockpit_light_batterie,
    cockpit_oeldruck,
    cockpit_light_feststellbremse,
    engine_temperature,
    cockpit_light_masterfailure,
    engine_tank_content,
    bremse_ABS_eingriff,
    cockpit_light_tuerkontrolle,
    lights_warnblinkgeber,
    cockpit_light_retarder_direkt,
    lights_stand,
    cockpit_light_ASR_off,
    engine_ASR_eingriff,
    kmcounter_km,
    kmcounter_m,
    Cabinair_Temp,
    cockpit_uhr_sek,
    haltewunschlampe,
    AI_Blinker_L,
    AI_Blinker_R,
    AI_Light,
    AI_Interiorlight,
    engine_on,
    tank_percent,
    cockpit_gangR,
    cockpit_gangN,
    cockpit_gang1,
    cockpit_gang2,
    cockpit_gang3,
    engine_tank_capacity,
};

enum SYS_VAR
{
    Pause,
    Weather_Temperature,
    Time,
    Day,
    Month,
    Year,
    DayOfYear,
};

#pragma pack(push)
#pragma pack(1)

struct telemetry_state_t
{
    float Throttle;
    float Brake;
    float Clutch;
    float Brakeforce;
    float Velocity;
    float Axle_Suspension_0_L;
    float Axle_Suspension_0_R;
    float Axle_Suspension_1_L;
    float Axle_Suspension_1_R;
    float A_Trans_X;
    float A_Trans_Y;
    float A_Trans_Z;
    float elec_busbar_main;
    float engine_n;
    float haltewunsch;
    float lights_fern;
    float lights_blinkgeber;
    float cockpit_light_batterie;
    float cockpit_oeldruck;
    float cockpit_light_feststellbremse;
    float engine_temperature;
    float cockpit_light_masterfailure;
    float engine_tank_content;
    float bremse_ABS_eingriff;
    float cockpit_light_tuerkontrolle;
    float lights_warnblinkgeber;
    float cockpit_light_retarder_direkt;
    float lights_stand;
    float cockpit_light_ASR_off;
    float engine_ASR_eingriff;
    float kmcounter_km;
    float kmcounter_m;
    float Cabinair_Temp;
    float cockpit_uhr_sek;
    float haltewunschlampe;
    float AI_Blinker_L;
    float AI_Blinker_R;
    float AI_Light;
    float AI_Interiorlight;
    float engine_on;
    float tank_percent;
    float cockpit_gangR;
    float cockpit_gangN;
    float cockpit_gang1;
    float cockpit_gang2;
    float cockpit_gang3;
    float engine_tank_capacity;
    float pause;
    float Weather_Temperature;
    float Time;
    float Day;
    float Month;
    float Year;
    float DayOfYear;
};

#pragma pack(pop)
