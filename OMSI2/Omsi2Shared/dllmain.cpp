// dllmain.cpp : Defines the entry point for the DLL application.
#include "pch.h"
#include "structs.h"

#define SHARED_MEM_NAME "$OMSI2_SHARED_MEM"


// === Экспортируемые функции плагина ===
extern "C" {
	__declspec(dllexport) void __cdecl PluginStart(void* aOwner);
	__declspec(dllexport) void __cdecl PluginFinalize();
	__declspec(dllexport) void __cdecl AccessTrigger(unsigned variableIndex, BOOL& triggerScript);
	__declspec(dllexport) void __cdecl AccessVariable(unsigned variableIndex, float& value, BOOL& writeValue);
	__declspec(dllexport) void __cdecl AccessStringVariable(unsigned variableIndex, char* firstCharacterAddress, BOOL& writeValue);
	__declspec(dllexport) void __cdecl AccessSystemVariable(unsigned variableIndex, float& value, BOOL& writeValue);
}

// === Глобальные переменные для shared memory ===
HANDLE memory_mapping = NULL;
telemetry_state_t* shared_memory = NULL;

// === Кэш для вычисления углов ===
static float cached_susp_0_L = 0.0f;
static float cached_susp_0_R = 0.0f;
static float cached_susp_1_L = 0.0f;
static float cached_susp_1_R = 0.0f;
static float cached_trans_x = 0.0f;
static float cached_trans_y = 0.0f;
static float cached_trans_z = 0.0f;
static float cached_velocity = 0.0f;   // км/ч (как в OMSI)
static float yaw_accum = 0.0f;

// Время прошлого кадра (системная переменная Time, секунды суток)
static float last_frame_time = -1.0f;

// === Функция вычисления углов и маппинга ускорений ===
// ВАЖНО: вызывается РОВНО ОДИН раз за кадр — из AccessSystemVariable при
// изменении SYS_VAR::Time. dt — реальный шаг времени между кадрами,
// а не константа 16 мс.
void UpdateMotionData(float dt)
{
	if (!shared_memory) return;

	// 1. Маппинг ускорений: OMSI -> стандарт симуляторов
	// OMSI: X=Sideways, Y=Forward, Z=Vertical
	shared_memory->Surge = cached_trans_y;         // Продольное
	shared_memory->Sway = -cached_trans_x;         // Боковое (инверсия для совместимости)
	shared_memory->Heave = cached_trans_z;         // Вертикальное

	// 2. Roll (крен) — разница левой/правой подвески
	float left_side = (cached_susp_0_L + cached_susp_1_L) * 0.5f;
	float right_side = (cached_susp_0_R + cached_susp_1_R) * 0.5f;
	shared_memory->Roll = (right_side - left_side) * shared_memory->angle_coefficient;

	// 3. Pitch (тангаж) — разница передней/задней оси
	float front_axle = (cached_susp_0_L + cached_susp_0_R) * 0.5f;
	float rear_axle = (cached_susp_1_L + cached_susp_1_R) * 0.5f;
	shared_memory->Pitch = (front_axle - rear_axle) * shared_memory->angle_coefficient;

	// 4. Yaw (рыскание) — приближённо через интеграцию бокового ускорения.
	// Скорость переводим км/ч -> м/с, чтобы единицы совпадали
	// с ускорением (м/с^2): yaw_rate получается в рад/с.
	const float velocity_ms = cached_velocity / 3.6f;
	if (velocity_ms > 0.1f && dt > 0.0f && dt < 0.25f)
	{
		float yaw_rate = shared_memory->Sway / velocity_ms;   // рад/с
		yaw_accum += yaw_rate * dt * 57.29578f;               // -> градусы

		// Ограничение диапазона [-180; +180]
		if (yaw_accum > 180.0f)  yaw_accum -= 360.0f;
		if (yaw_accum < -180.0f) yaw_accum += 360.0f;
		shared_memory->Yaw = yaw_accum;
	}
	else
	{
		// Стоим/пауза — плавное затухание, интегратор не дрейфует
		yaw_accum *= 0.95f;
		shared_memory->Yaw = yaw_accum;
	}
}

// === Инициализация shared memory ===
bool initialize_shared_memory()
{
	const DWORD memory_size = sizeof(telemetry_state_t);

	memory_mapping = CreateFileMappingA(
		INVALID_HANDLE_VALUE,
		NULL,
		PAGE_READWRITE | SEC_COMMIT,
		0,
		memory_size,
		SHARED_MEM_NAME
	);

	if (!memory_mapping) {
		return false;
	}

	// FIX: раньше при ERROR_ALREADY_EXISTS функция возвращала false,
	// shared_memory оставался NULL — а запись в него из AccessVariable
	// крашила игру. Теперь просто подключаемся к существующему маппингу.
	const BOOL already_existed = (GetLastError() == ERROR_ALREADY_EXISTS);

	shared_memory = static_cast<telemetry_state_t*>(
		MapViewOfFile(memory_mapping, FILE_MAP_ALL_ACCESS, 0, 0, 0)
		);

	if (!shared_memory) {
		CloseHandle(memory_mapping);
		memory_mapping = NULL;
		return false;
	}

	// Обнуляем ТОЛЬКО новый маппинг: если читатель телеметрии подключился
	// раньше игры, memset при рестарте дал бы ему мусор.
	if (!already_existed) {
		memset(shared_memory, 0, memory_size);
		shared_memory->angle_coefficient = 0.05f; // 50 мм подвески ~ 1°
	}

	return true;
}

// === Деинициализация ===
void deinitialize_shared_memory()
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

// === Точка входа DLL ===
BOOL APIENTRY DllMain(HMODULE hModule, DWORD ul_reason_for_call, LPVOID lpReserved)
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

// === Callbacks плагина ===
void PluginStart(void* aOwner)
{
	initialize_shared_memory();
	// сброс состояния при старте/смене карты
	last_frame_time = -1.0f;
	yaw_accum = 0.0f;
}

void PluginFinalize()
{
	deinitialize_shared_memory();
}

void AccessTrigger(unsigned variableIndex, BOOL& triggerScript)
{
	// OMSI trigger variables — пока не используются
	switch (variableIndex)
	{
	default:
		break;
	}
}

void AccessVariable(unsigned variableIndex, float& value, BOOL& writeValue)
{
	writeValue = FALSE;

	// FIX: guard — без него при неудачной инициализации shared memory
	// игра падала (запись по NULL).
	if (!shared_memory) return;

	// 1. Кэшируем "сырые" данные для вычислений
	switch (variableIndex)
	{
	case VAR_ENUM::Axle_Suspension_0_L: cached_susp_0_L = value; break;
	case VAR_ENUM::Axle_Suspension_0_R: cached_susp_0_R = value; break;
	case VAR_ENUM::Axle_Suspension_1_L: cached_susp_1_L = value; break;
	case VAR_ENUM::Axle_Suspension_1_R: cached_susp_1_R = value; break;
	case VAR_ENUM::A_Trans_X: cached_trans_x = value; break;
	case VAR_ENUM::A_Trans_Y: cached_trans_y = value; break;
	case VAR_ENUM::A_Trans_Z: cached_trans_z = value; break;
	case VAR_ENUM::Velocity: cached_velocity = value; break;
	}

	// 2. Записываем все переменные в shared memory
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

	// FIX: UpdateMotionData() удалён отсюда. Раньше он вызывался на
	// КАЖДУЮ переменную (~50 раз за кадр): интеграл Yaw накапливался
	// примерно в 50 раз быстрее реального времени, а dt=16мс был лишь
	// предположением о FPS. Углы теперь считаются один раз за кадр
	// в AccessSystemVariable по изменению SYS_VAR::Time.
}

void AccessStringVariable(unsigned variableIndex, char* firstCharacterAddress, BOOL& writeValue)
{
	writeValue = FALSE;
	// OMSI string variables — пока не используют
}

void AccessSystemVariable(unsigned variableIndex, float& value, BOOL& writeValue)
{
	writeValue = FALSE;

	// FIX: guard — тот же краш по NULL, что и в AccessVariable
	if (!shared_memory) return;

	switch (variableIndex)
	{
	case SYS_VAR::Pause: shared_memory->pause = value; break;
	case SYS_VAR::Weather_Temperature: shared_memory->Weather_Temperature = value; break;
	case SYS_VAR::Day: shared_memory->Day = value; break;
	case SYS_VAR::Month: shared_memory->Month = value; break;
	case SYS_VAR::Year: shared_memory->Year = value; break;
	case SYS_VAR::DayOfYear: shared_memory->DayOfYear = value; break;
	case SYS_VAR::Time:
		shared_memory->Time = value;
		{
			// Time обновляется один раз за кадр — идеальный тик для углов.
			float dt = (last_frame_time >= 0.0f) ? (value - last_frame_time) : 0.016f;
			last_frame_time = value;
			UpdateMotionData(dt);
		}
		break;
	}
}