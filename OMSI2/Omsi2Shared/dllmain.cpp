// dllmain.cpp : Defines the entry point for the DLL application.
#include "pch.h"
#include "structs.h"

#define SHARED_MEM_NAME "$OMSI2_SHARED_MEM"

// Function declarations
__declspec(dllexport) void PluginStart(void* aOwner);
__declspec(dllexport) void PluginFinalize();
__declspec(dllexport) void AccessTrigger(unsigned variableIndex, bool& triggerScript);
__declspec(dllexport) void AccessVariable(unsigned variableIndex, float& value, bool& writeValue);
__declspec(dllexport) void AccessStringVariable(unsigned variableIndex, char* firstCharacterAddress, bool& writeValue);
__declspec(dllexport) void AccessSystemVariable(unsigned variableIndex, float& value, bool& writeValue);

BOOL APIENTRY DllMain( HMODULE hModule,
                       DWORD  ul_reason_for_call,
                       LPVOID lpReserved
                     )
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
    case DLL_THREAD_ATTACH:
    case DLL_THREAD_DETACH:
    case DLL_PROCESS_DETACH:
        break;
    }
    return TRUE;
}

HANDLE memory_mapping = NULL;

telemetry_state_t* shared_memory = NULL;

void deinitialize_shared_memory(void)
{
    if (shared_memory) {
        UnmapViewOfFile(shared_memory);
        shared_memory = NULL;
    }

    if (memory_mapping) {
        CloseHandle(memory_mapping);
        memory_mapping = NULL;
    }
}

bool initialize_shared_memory(void)
{
    // Setup the mapping.

    const DWORD memory_size = sizeof(telemetry_state_t);
    memory_mapping = CreateFileMappingA(INVALID_HANDLE_VALUE, NULL, PAGE_READWRITE | SEC_COMMIT, 0, memory_size, SHARED_MEM_NAME);
    if (!memory_mapping) {
        deinitialize_shared_memory();
        return false;
    }
    if (GetLastError() == ERROR_ALREADY_EXISTS) {
        deinitialize_shared_memory();
        return false;
    }

    shared_memory = static_cast<telemetry_state_t*>(MapViewOfFile(memory_mapping, FILE_MAP_ALL_ACCESS, 0, 0, 0));
    if (!shared_memory) {
        deinitialize_shared_memory();
        return false;
    }

    memset(shared_memory, 0, memory_size);

    return true;
}

void PluginStart(void* aOwner)
{
    // OMSI plugin starting
    initialize_shared_memory();
}

void PluginFinalize()
{
    // OMSI plugin ending
    deinitialize_shared_memory();
}

void AccessTrigger(unsigned variableIndex, bool& triggerScript)
{
    // TODO:
    switch (variableIndex)
    {
    default:
        break;
    }
}

void AccessVariable(unsigned variableIndex, float& value, bool& writeValue)
{
    // read OMSI 2 variables
    writeValue = false;

    switch (variableIndex)
    {
        case VAR_ENUM::Throttle: shared_memory->Throttle = value; break;
        case VAR_ENUM::Brake: shared_memory->Brake = value; break;
        case VAR_ENUM::Clutch: shared_memory->Clutch = value; break;
        case VAR_ENUM::Brakeforce: shared_memory->Brakeforce = value; break;
        case VAR_ENUM::Velocity: shared_memory->Velocity = value; break;
        case VAR_ENUM::Axle_Suspension_0_L: shared_memory->Axle_Suspension_0_L = value; break;
        case VAR_ENUM::Axle_Suspension_0_R: shared_memory->Axle_Suspension_0_R = value; break;
        case VAR_ENUM::Axle_Suspension_1_L: shared_memory->Axle_Suspension_1_L = value; break;
        case VAR_ENUM::Axle_Suspension_1_R: shared_memory->Axle_Suspension_1_R = value; break;
        case VAR_ENUM::A_Trans_X: shared_memory->A_Trans_X = value; break;
        case VAR_ENUM::A_Trans_Y: shared_memory->A_Trans_Y = value; break;
        case VAR_ENUM::A_Trans_Z: shared_memory->A_Trans_Z = value; break;
        case VAR_ENUM::elec_busbar_main: shared_memory->elec_busbar_main = value; break;
        case VAR_ENUM::engine_n: shared_memory->engine_n = value; break;
        case VAR_ENUM::haltewunsch: shared_memory->haltewunsch = value; break;
        case VAR_ENUM::lights_fern: shared_memory->lights_fern = value; break;
        case VAR_ENUM::lights_blinkgeber: shared_memory->lights_blinkgeber = value; break;
        case VAR_ENUM::cockpit_light_batterie: shared_memory->cockpit_light_batterie = value; break;
        case VAR_ENUM::cockpit_oeldruck: shared_memory->cockpit_oeldruck = value; break;
        case VAR_ENUM::cockpit_light_feststellbremse: shared_memory->cockpit_light_feststellbremse = value; break;
        case VAR_ENUM::engine_temperature: shared_memory->engine_temperature = value; break;
        case VAR_ENUM::cockpit_light_masterfailure: shared_memory->cockpit_light_masterfailure = value; break;
        case VAR_ENUM::engine_tank_content: shared_memory->engine_tank_content = value; break;
        case VAR_ENUM::bremse_ABS_eingriff: shared_memory->bremse_ABS_eingriff = value; break;
        case VAR_ENUM::cockpit_light_tuerkontrolle: shared_memory->cockpit_light_tuerkontrolle = value; break;
        case VAR_ENUM::lights_warnblinkgeber: shared_memory->lights_warnblinkgeber = value; break;
        case VAR_ENUM::cockpit_light_retarder_direkt: shared_memory->cockpit_light_retarder_direkt = value; break;
        case VAR_ENUM::lights_stand: shared_memory->lights_stand = value; break;
        case VAR_ENUM::cockpit_light_ASR_off: shared_memory->cockpit_light_ASR_off = value; break;
        case VAR_ENUM::engine_ASR_eingriff: shared_memory->engine_ASR_eingriff = value; break;
        case VAR_ENUM::kmcounter_km: shared_memory->kmcounter_km = value; break;
        case VAR_ENUM::kmcounter_m: shared_memory->kmcounter_m = value; break;
        case VAR_ENUM::Cabinair_Temp: shared_memory->Cabinair_Temp = value; break;
        case VAR_ENUM::cockpit_uhr_sek: shared_memory->cockpit_uhr_sek = value; break;
        case VAR_ENUM::haltewunschlampe: shared_memory->haltewunschlampe = value; break;
        case VAR_ENUM::AI_Blinker_L: shared_memory->AI_Blinker_L = value; break;
        case VAR_ENUM::AI_Blinker_R: shared_memory->AI_Blinker_R = value; break;
        case VAR_ENUM::AI_Light: shared_memory->AI_Light = value; break;
        case VAR_ENUM::AI_Interiorlight: shared_memory->AI_Interiorlight = value; break;
        case VAR_ENUM::engine_on: shared_memory->engine_on = value; break;
        case VAR_ENUM::tank_percent: shared_memory->tank_percent = value; break;
        case VAR_ENUM::cockpit_gangR: shared_memory->cockpit_gangR = value; break;
        case VAR_ENUM::cockpit_gangN: shared_memory->cockpit_gangN = value; break;
        case VAR_ENUM::cockpit_gang1: shared_memory->cockpit_gang1 = value; break;
        case VAR_ENUM::cockpit_gang2: shared_memory->cockpit_gang2 = value; break;
        case VAR_ENUM::cockpit_gang3: shared_memory->cockpit_gang3 = value; break;
        case VAR_ENUM::engine_tank_capacity: shared_memory->engine_tank_capacity = value; break;
    }
}

void AccessStringVariable(unsigned variableIndex, char* firstCharacterAddress, bool& writeValue)
{
    // OMSI String variables
    writeValue = false;

    switch (variableIndex)
    {
    case 0:
        break;
    }
}

void AccessSystemVariable(unsigned variableIndex, float& value, bool& writeValue)
{
    // OMSI system variables
    writeValue = false;

    switch (variableIndex)
    {
        case SYS_VAR::Pause: shared_memory->pause = value != 0; break;
        case SYS_VAR::Weather_Temperature: shared_memory->Weather_Temperature = value; break;
        case SYS_VAR::Time: shared_memory->Time = value; break;
        case SYS_VAR::Day: shared_memory->Day = value; break;
        case SYS_VAR::Month: shared_memory->Month = value; break;
        case SYS_VAR::Year: shared_memory->Year = value; break;
        case SYS_VAR::DayOfYear: shared_memory->DayOfYear = value; break;
    }
}
