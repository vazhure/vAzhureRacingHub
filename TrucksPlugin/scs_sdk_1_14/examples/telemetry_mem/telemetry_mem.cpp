/**
 * @brief Example of using a shared memory to pass a mostly fixed set of telemetry
 * data to another process.
 *
 * Note that use of the shared memory reduces precision of the output and
 * increases latency between event and possible reaction.
 */

 // Windows stuff.
#define WINVER 0x0500
#define _WIN32_WINNT 0x0500
#include <windows.h>
#include <stdio.h>
#include <stdlib.h>
#include <assert.h>
#include <stdarg.h>

// Shared memory name
#define SHARED_MEM_NAME "Local\\ETS2"

// SDK
#include "scssdk_telemetry.h"
#include "eurotrucks2/scssdk_eut2.h"
#include "eurotrucks2/scssdk_telemetry_eut2.h"
#include "amtrucks/scssdk_ats.h"
#include "amtrucks/scssdk_telemetry_ats.h"

#define UNUSED(x)

/**
 * @name Callbacks remembered from the initialization info.
 */
 //@{
scs_telemetry_register_for_channel_t register_for_channel = NULL;
scs_telemetry_unregister_from_channel_t unregister_from_channel = NULL;
scs_log_t game_log = NULL;
//@}

/**
 * @brief Prints message to game log.
 */
void log_line(const scs_log_type_t type, const char* const text, ...)
{
	if (!game_log) {
		return;
	}
	char formated[1000];

	va_list args;
	va_start(args, text);
	vsnprintf_s(formated, sizeof(formated), _TRUNCATE, text, args);
	formated[sizeof(formated) - 1] = 0;
	va_end(args);

	game_log(type, formated);
}

const size_t MAX_SUPPORTED_WHEEL_COUNT = 8;

#define SCS_STR_LEN 64

#pragma pack(push)
#pragma pack(1)

/**
 * @brief The layout of the shared memory.
*/
struct telemetry_state_t
{
	scs_u8_t				running;					// Is the telemetry running or it is paused?
	scs_u8_t				lblinker;					// SCS_TELEMETRY_TRUCK_CHANNEL_lblinker
	scs_u8_t				rblinker;					// SCS_TELEMETRY_TRUCK_CHANNEL_rblinker
	scs_u8_t				lowBeamLight;				// SCS_TELEMETRY_TRUCK_CHANNEL_light_low_beam
	scs_u8_t				hiBeamLight;				// SCS_TELEMETRY_TRUCK_CHANNEL_light_high_beam
	scs_u8_t				parkingLights;				// SCS_TELEMETRY_TRUCK_CHANNEL_light_parking

	scs_float_t				speedometer_speed;			// SCS_TELEMETRY_TRUCK_CHANNEL_speed
	scs_float_t				rpm;						// SCS_TELEMETRY_TRUCK_CHANNEL_engine_rpm
	scs_float_t				rpmMax;						// SCS_TELEMETRY_CONFIG_ATTRIBUTE_rpm_limit
	scs_float_t				fuel;						// SCS_TELEMETRY_TRUCK_CHANNEL_fuel
	scs_float_t				fuelCapacity;				// SCS_TELEMETRY_CONFIG_ATTRIBUTE_fuel_capacity
	scs_float_t				steering;					// SCS_TELEMETRY_TRUCK_CHANNEL_effective_steering
	scs_float_t				throttle;					// SCS_TELEMETRY_TRUCK_CHANNEL_effective_throttle
	scs_float_t				brake;						// SCS_TELEMETRY_TRUCK_CHANNEL_effective_brake
	scs_float_t				clutch;						// SCS_TELEMETRY_TRUCK_CHANNEL_effective_clutch
	scs_float_t				navigationTime;				// SCS_TELEMETRY_TRUCK_CHANNEL_navigation_time

	scs_s32_t				gear;						// SCS_TELEMETRY_TRUCK_CHANNEL_engine_gear
	scs_s32_t				displayedGear;				// SCS_TELEMETRY_TRUCK_CHANNEL_displayed_gear
	scs_u32_t				gameTime;					// SCS_TELEMETRY_CHANNEL_game_time
	scs_u32_t				deliveryTime;				// SCS_TELEMETRY_CONFIG_ATTRIBUTE_delivery_time

	scs_value_fvector_t		linear_valocity;			// SCS_TELEMETRY_TRUCK_CHANNEL_local_linear_velocity
	scs_value_fvector_t		angular_velocity;			// SCS_TELEMETRY_TRUCK_CHANNEL_local_angular_velocity
	scs_value_fvector_t		linear_acceleration;		// SCS_TELEMETRY_TRUCK_CHANNEL_local_linear_acceleration
	scs_value_fvector_t		angular_acceleration;		// SCS_TELEMETRY_TRUCK_CHANNEL_local_angular_acceleration
	scs_value_fvector_t		cabin_angular_velocity;		// SCS_TELEMETRY_TRUCK_CHANNEL_cabin_angular_velocity
	scs_value_fvector_t		cabin_angular_acceleration;	// SCS_TELEMETRY_TRUCK_CHANNEL_cabin_angular_acceleration

	scs_value_dplacement_t	ws_truck_placement;			// SCS_TELEMETRY_TRUCK_CHANNEL_world_placement
	
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// extra data
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////

	char			shifterType[SCS_STR_LEN];			// SCS_TELEMETRY_CONFIG_ATTRIBUTE_shifter_type
	char			cargo[SCS_STR_LEN];					// SCS_TELEMETRY_CONFIG_ATTRIBUTE_cargo
	char			destinationCity[SCS_STR_LEN];		// SCS_TELEMETRY_CONFIG_ATTRIBUTE_destination_city
	char			destinationCompany[SCS_STR_LEN];	// SCS_TELEMETRY_CONFIG_ATTRIBUTE_destination_company
	char			sourceCity[SCS_STR_LEN];			// SCS_TELEMETRY_CONFIG_ATTRIBUTE_source_city
	char			sourceCompany[SCS_STR_LEN];			// SCS_TELEMETRY_CONFIG_ATTRIBUTE_source_company

	scs_u32_t		selectorCount;						// SCS_TELEMETRY_CONFIG_ATTRIBUTE_selector_count
	scs_u32_t		planned_distanceKM;					// SCS_TELEMETRY_CONFIG_ATTRIBUTE_planned_distance_km
	scs_u32_t		multiplayerTimeOffset;				// SCS_TELEMETRY_CHANNEL_multiplayer_time_offset
	scs_u32_t		restStop;							// SCS_TELEMETRY_CHANNEL_next_rest_stop

	scs_float_t		localScale;							// SCS_TELEMETRY_CHANNEL_local_scale
	scs_float_t		adBlueFuelCapacity;					// SCS_TELEMETRY_CONFIG_ATTRIBUTE_adblue_capacity

	scs_float_t		truckAdblueFuelLevelLiters;			// SCS_TELEMETRY_TRUCK_CHANNEL_adblue
	scs_float_t		truckFuelConsumptionAverageLiters;	// SCS_TELEMETRY_TRUCK_CHANNEL_fuel_average_consumption
	scs_float_t		truckCruise_controlSpeedMS;			// SCS_TELEMETRY_TRUCK_CHANNEL_cruise_control
	scs_float_t		truckFuelRangeKm;					// SCS_TELEMETRY_TRUCK_CHANNEL_fuel_range
	scs_float_t		truckBatteryVoltage;				// SCS_TELEMETRY_TRUCK_CHANNEL_battery_voltage
	scs_float_t		truckOdometerKM;					// SCS_TELEMETRY_TRUCK_CHANNEL_odometer
	scs_float_t		truckNavigationDistanceMeters;		// SCS_TELEMETRY_TRUCK_CHANNEL_navigation_distance
	scs_float_t		truckNavigationTimeSeconds;			// SCS_TELEMETRY_TRUCK_CHANNEL_navigation_time
	scs_float_t		truckNavigationSpeedLimitMS;		// SCS_TELEMETRY_TRUCK_CHANNEL_navigation_speed_limit
	scs_float_t		truckOilPressure;					// SCS_TELEMETRY_TRUCK_CHANNEL_oil_pressure
	scs_float_t		truckOilTemperature;				// SCS_TELEMETRY_TRUCK_CHANNEL_oil_temperature
	scs_float_t		truckWaterTemperature;				// SCS_TELEMETRY_TRUCK_CHANNEL_water_temperature

	scs_u8_t		truckBrakeParking;					// SCS_TELEMETRY_TRUCK_CHANNEL_parking_brake
	scs_u8_t		truckBrakeMotor;					// SCS_TELEMETRY_TRUCK_CHANNEL_motor_brake
	scs_u8_t		truckFuelWarning;					// SCS_TELEMETRY_TRUCK_CHANNEL_fuel_warning
	scs_u8_t		truckBatteryVoltageWarning;			// SCS_TELEMETRY_TRUCK_CHANNEL_battery_voltage_warning
	scs_u8_t		truckElectricEnabled;				// SCS_TELEMETRY_TRUCK_CHANNEL_electric_enabled
	scs_u8_t		truckEngineEnabled;					// SCS_TELEMETRY_TRUCK_CHANNEL_engine_enabled
	scs_u8_t		truckHazardWarning;					// SCS_TELEMETRY_TRUCK_CHANNEL_hazard_warning
	scs_u8_t		truckWipers;						// SCS_TELEMETRY_TRUCK_CHANNEL_wipers
};

#pragma pack(pop)

/**
 * @brief Handle of the memory mapping.
 */
HANDLE memory_mapping = NULL;

/**
 * @brief Block inside the shared memory.
 */
telemetry_state_t* shared_memory = NULL;

/**
 * @brief Deinitialize the shared memory objects.
 */
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

/**
 * @brief Initialize the shared memory objects.
 */
bool initialize_shared_memory(void)
{
	// Setup the mapping.

	const DWORD memory_size = sizeof(telemetry_state_t);
	memory_mapping = CreateFileMappingA(INVALID_HANDLE_VALUE, NULL, PAGE_READWRITE | SEC_COMMIT, 0, memory_size, SHARED_MEM_NAME);
	if (!memory_mapping) {
		log_line(SCS_LOG_TYPE_error, "Unable to create shared memory %08X", GetLastError());
		deinitialize_shared_memory();
		return false;
	}
	if (GetLastError() == ERROR_ALREADY_EXISTS) {
		log_line(SCS_LOG_TYPE_error, "Shared memory is already in use.");
		deinitialize_shared_memory();
		return false;
	}

	shared_memory = static_cast<telemetry_state_t*>(MapViewOfFile(memory_mapping, FILE_MAP_ALL_ACCESS, 0, 0, 0));
	if (!shared_memory) {
		log_line(SCS_LOG_TYPE_error, "Unable to map the view %08X", GetLastError());
		deinitialize_shared_memory();
		return false;
	}

	// Defaults in the structure.

	memset(shared_memory, 0, memory_size);

	// We are always initialized in the paused state.

	shared_memory->running = 0;

	// No wheels until we get corresponding configuration message.

	return true;
}


/**
 * @brief Float storage callback.
 *
 * Can be used together with SCS_TELEMETRY_CHANNEL_FLAG_no_value in which case it
 * will store zero if the value is not available.
 */
SCSAPI_VOID telemetry_store_bool(const scs_string_t name, const scs_u32_t index, const scs_value_t* const value, const scs_context_t context)
{
	assert(context);
	scs_u8_t* const storage = static_cast<scs_u8_t*>(context);

	if (value) {
		assert(value->type == SCS_VALUE_TYPE_bool);
		*storage = value->value_bool.value;
	}
	else {
		*storage = false;
	}
}

/**
 * @brief Float storage callback.
 *
 * Can be used together with SCS_TELEMETRY_CHANNEL_FLAG_no_value in which case it
 * will store zero if the value is not available.
 */
SCSAPI_VOID telemetry_store_float(const scs_string_t name, const scs_u32_t index, const scs_value_t* const value, const scs_context_t context)
{
	assert(context);
	scs_float_t* const storage = static_cast<scs_float_t*>(context);

	if (value) {
		assert(value->type == SCS_VALUE_TYPE_float);
		*storage = value->value_float.value;
	}
	else {
		*storage = 0.0f;
	}
}

/**
 * @brief s32 storage callback.
 *
 * Can be used together with SCS_TELEMETRY_CHANNEL_FLAG_no_value in which case it
 * will store zero if the value is not available.
 */
SCSAPI_VOID telemetry_store_s32(const scs_string_t name, const scs_u32_t index, const scs_value_t* const value, const scs_context_t context)
{
	assert(context);
	scs_s32_t* const storage = static_cast<scs_s32_t*>(context);

	if (value) {
		assert(value->type == SCS_VALUE_TYPE_s32);
		*storage = value->value_s32.value;
	}
	else {
		*storage = 0;
	}
}

/**
 * @brief s32 storage callback.
 *
 * Can be used together with SCS_TELEMETRY_CHANNEL_FLAG_no_value in which case it
 * will store zero if the value is not available.
 */
SCSAPI_VOID telemetry_store_u32(const scs_string_t name, const scs_u32_t index, const scs_value_t* const value, const scs_context_t context)
{
	assert(context);
	scs_u32_t* const storage = static_cast<scs_u32_t*>(context);

	if (value) {
		assert(value->type == SCS_VALUE_TYPE_u32);
		*storage = value->value_u32.value;
	}
	else {
		*storage = 0;
	}
}


/**
 * @brief Orientation storage callback.
 *
 * Can be used together with SCS_TELEMETRY_CHANNEL_FLAG_no_value in which case it
 * will store zero if the value is not available.
 */
SCSAPI_VOID telemetry_store_orientation(const scs_string_t name, const scs_u32_t index, const scs_value_t* const value, const scs_context_t context)
{
	assert(context);
	scs_value_euler_t* const storage = static_cast<scs_value_euler_t*>(context);

	if (value) {
		assert(value->type == SCS_VALUE_TYPE_euler);
		*storage = value->value_euler;
	}
	else {
		storage->heading = 0.0f;
		storage->pitch = 0.0f;
		storage->roll = 0.0f;
	}
}

/**
 * @brief Vector storage callback.
 *
 * Can be used together with SCS_TELEMETRY_CHANNEL_FLAG_no_value in which case it
 * will store zero if the value is not available.
 */
SCSAPI_VOID telemetry_store_fvector(const scs_string_t name, const scs_u32_t index, const scs_value_t* const value, const scs_context_t context)
{
	assert(context);
	scs_value_fvector_t* const storage = static_cast<scs_value_fvector_t*>(context);

	if (value) {
		assert(value->type == SCS_VALUE_TYPE_fvector);
		*storage = value->value_fvector;
	}
	else {
		storage->x = 0.0f;
		storage->y = 0.0f;
		storage->z = 0.0f;
	}
}

/**
 * @brief Placement storage callback.
 *
 * Can be used together with SCS_TELEMETRY_CHANNEL_FLAG_no_value in which case it
 * will store zeros if the value is not available.
 */
SCSAPI_VOID telemetry_store_dplacement(const scs_string_t name, const scs_u32_t index, const scs_value_t* const value, const scs_context_t context)
{
	assert(context);
	scs_value_dplacement_t* const storage = static_cast<scs_value_dplacement_t*>(context);

	if (value) {
		assert(value->type == SCS_VALUE_TYPE_dplacement);
		*storage = value->value_dplacement;
	}
	else {
		storage->position.x = 0.0;
		storage->position.y = 0.0;
		storage->position.z = 0.0;
		storage->orientation.heading = 0.0f;
		storage->orientation.pitch = 0.0f;
		storage->orientation.roll = 0.0f;
	}
}

/**
 * @brief Finds attribute with specified name in the configuration structure.
 *
 * Returns NULL if the attribute was not found or if it is not of the expected type.
 */
const scs_named_value_t* find_attribute(const scs_telemetry_configuration_t& configuration, const char* const name, const scs_u32_t index, const scs_value_type_t expected_type)
{
	for (const scs_named_value_t* current = configuration.attributes; current->name; ++current) {
		if ((current->index != index) || (strcmp(current->name, name) != 0)) {
			continue;
		}
		if (current->value.type == expected_type) {
			return current;
		}
		log_line(SCS_LOG_TYPE_error, "Attribute %s has unexpected type %u", name, static_cast<unsigned>(current->value.type));
		break;
	}
	return NULL;
}

/**
 * @brief Called whenever the game pauses or unpauses its telemetry output.
 */
SCSAPI_VOID telemetry_pause(const scs_event_t event, const void* const UNUSED(event_info), const scs_context_t UNUSED(context))
{
	shared_memory->running = (event == SCS_TELEMETRY_EVENT_started) ? 1 : 0;
}

/**
 * @brief Called whenever configuration changes.
 */
SCSAPI_VOID telemetry_configuration(const scs_event_t event, const void* const event_info, const scs_context_t UNUSED(context))
{
	const struct scs_telemetry_configuration_t* const info = static_cast<const scs_telemetry_configuration_t*>(event_info);
	if (strcmp(info->id, SCS_TELEMETRY_CONFIG_truck) == 0) {

		const scs_named_value_t* const rpmMax = find_attribute(*info, SCS_TELEMETRY_CONFIG_ATTRIBUTE_rpm_limit, SCS_U32_NIL, SCS_VALUE_TYPE_float);
		const scs_named_value_t* const fuelCapacity = find_attribute(*info, SCS_TELEMETRY_CONFIG_ATTRIBUTE_fuel_capacity, SCS_U32_NIL, SCS_VALUE_TYPE_float);
		const scs_named_value_t* const adBluefuelCapacity = find_attribute(*info, SCS_TELEMETRY_CONFIG_ATTRIBUTE_adblue_capacity, SCS_U32_NIL, SCS_VALUE_TYPE_float);

		const scs_named_value_t* const adBlueFuelCapacity = find_attribute(*info, SCS_TELEMETRY_CONFIG_ATTRIBUTE_adblue_capacity, SCS_U32_NIL, SCS_VALUE_TYPE_float);

		shared_memory->rpmMax = rpmMax ? rpmMax->value.value_float.value : 0;
		shared_memory->fuelCapacity = fuelCapacity ? fuelCapacity->value.value_float.value : 0;
		shared_memory->adBlueFuelCapacity = fuelCapacity ? fuelCapacity->value.value_float.value : 0;
		shared_memory->adBlueFuelCapacity = adBlueFuelCapacity ? adBlueFuelCapacity->value.value_float.value : 0;
	}

	if (strcmp(info->id, SCS_TELEMETRY_CONFIG_hshifter) == 0) {
		const scs_named_value_t* const selectorCount = find_attribute(*info, SCS_TELEMETRY_CONFIG_ATTRIBUTE_selector_count, SCS_U32_NIL, SCS_VALUE_TYPE_u32);
		shared_memory->selectorCount = selectorCount ? selectorCount->value.value_u32.value : 0;
	}

	if (strcmp(info->id, SCS_TELEMETRY_CONFIG_controls) == 0) {
		const scs_named_value_t* const shifterType = find_attribute(*info, SCS_TELEMETRY_CONFIG_ATTRIBUTE_shifter_type, SCS_U32_NIL, SCS_VALUE_TYPE_string);
		shifterType ? strcpy_s(shared_memory->shifterType, shifterType->value.value_string.value) : shared_memory->shifterType[0] = '\0';
	}

	if (strcmp(info->id, SCS_TELEMETRY_CONFIG_job) == 0) {
		const scs_named_value_t* const deliveryTime = find_attribute(*info, SCS_TELEMETRY_CONFIG_ATTRIBUTE_delivery_time, SCS_U32_NIL, SCS_VALUE_TYPE_u32);
		shared_memory->deliveryTime = deliveryTime ? deliveryTime->value.value_u32.value : 0;
		const scs_named_value_t* const cargo = find_attribute(*info, SCS_TELEMETRY_CONFIG_ATTRIBUTE_cargo, SCS_U32_NIL, SCS_VALUE_TYPE_string);
		cargo ? strcpy_s(shared_memory->cargo, cargo->value.value_string.value) : shared_memory->cargo[0] = '\0';
		const scs_named_value_t* const destinationCity = find_attribute(*info, SCS_TELEMETRY_CONFIG_ATTRIBUTE_destination_city, SCS_U32_NIL, SCS_VALUE_TYPE_string);
		destinationCity ? strcpy_s(shared_memory->destinationCity, destinationCity->value.value_string.value) : shared_memory->destinationCity[0] = '\0';
		const scs_named_value_t* const destinationCompany = find_attribute(*info, SCS_TELEMETRY_CONFIG_ATTRIBUTE_destination_company, SCS_U32_NIL, SCS_VALUE_TYPE_string);
		destinationCompany ? strcpy_s(shared_memory->destinationCompany, destinationCompany->value.value_string.value) : shared_memory->destinationCompany[0] = '\0';
		const scs_named_value_t* const sourceCity = find_attribute(*info, SCS_TELEMETRY_CONFIG_ATTRIBUTE_source_city, SCS_U32_NIL, SCS_VALUE_TYPE_string);
		sourceCity ? strcpy_s(shared_memory->sourceCity, sourceCity->value.value_string.value) : shared_memory->sourceCity[0] = '\0';
		const scs_named_value_t* const sourceCompany = find_attribute(*info, SCS_TELEMETRY_CONFIG_ATTRIBUTE_source_company, SCS_U32_NIL, SCS_VALUE_TYPE_string);
		sourceCompany ? strcpy_s(shared_memory->sourceCompany, sourceCompany->value.value_string.value) : shared_memory->sourceCompany[0] = '\0';
		const scs_named_value_t* const planned_distanceKM = find_attribute(*info, SCS_TELEMETRY_CONFIG_ATTRIBUTE_planned_distance_km, SCS_U32_NIL, SCS_VALUE_TYPE_u32);
		shared_memory->planned_distanceKM = planned_distanceKM ? planned_distanceKM->value.value_u32.value : 0;
	}
}

/**
 * @brief Telemetry API initialization function.
 *
 * See scssdk_telemetry.h
 */
SCSAPI_RESULT scs_telemetry_init(const scs_u32_t version, const scs_telemetry_init_params_t* const params)
{
	// We currently support only one version of the API.

	if (version != SCS_TELEMETRY_VERSION_1_00) {
		return SCS_RESULT_unsupported;
	}
	const scs_telemetry_init_params_v100_t* const version_params = static_cast<const scs_telemetry_init_params_v100_t*>(params);
	game_log = version_params->common.log;

	// Check application version.

	log_line(SCS_LOG_TYPE_message, "Game '%s' %u.%u", version_params->common.game_id, SCS_GET_MAJOR_VERSION(version_params->common.game_version), SCS_GET_MINOR_VERSION(version_params->common.game_version));

	if (strcmp(version_params->common.game_id, SCS_GAME_ID_EUT2) == 0) {

		// Below the minimum version there might be some missing features (only minor change) or
		// incompatible values (major change).

		if (version_params->common.game_version < SCS_TELEMETRY_EUT2_GAME_VERSION_1_03) { // Fixed the wheels.count attribute
			log_line(SCS_LOG_TYPE_error, "Too old version of the game");
			game_log = NULL;
			return SCS_RESULT_unsupported;
		}

		if (version_params->common.game_version < SCS_TELEMETRY_EUT2_GAME_VERSION_1_07) { // Fixed the angular acceleration calculation
			log_line(SCS_LOG_TYPE_warning, "This version of the game has less precise output of angular acceleration of the cabin");
		}

		// Future versions are fine as long the major version is not changed.

		const scs_u32_t IMPLEMENTED_VERSION = SCS_TELEMETRY_EUT2_GAME_VERSION_CURRENT;
		if (SCS_GET_MAJOR_VERSION(version_params->common.game_version) > SCS_GET_MAJOR_VERSION(IMPLEMENTED_VERSION)) {
			log_line(SCS_LOG_TYPE_warning, "Too new major version of the game, some features might behave incorrectly");
		}
	}
	else if (strcmp(version_params->common.game_id, SCS_GAME_ID_ATS) == 0) {

		// Below the minimum version there might be some missing features (only minor change) or
		// incompatible values (major change).

		const scs_u32_t MINIMAL_VERSION = SCS_TELEMETRY_ATS_GAME_VERSION_1_00;
		if (version_params->common.game_version < MINIMAL_VERSION) {
			log_line(SCS_LOG_TYPE_warning, "WARNING: Too old version of the game, some features might behave incorrectly");
		}

		// Future versions are fine as long the major version is not changed.

		const scs_u32_t IMPLEMENTED_VERSION = SCS_TELEMETRY_ATS_GAME_VERSION_CURRENT;
		if (SCS_GET_MAJOR_VERSION(version_params->common.game_version) > SCS_GET_MAJOR_VERSION(IMPLEMENTED_VERSION)) {
			log_line(SCS_LOG_TYPE_warning, "WARNING: Too new major version of the game, some features might behave incorrectly");
		}
	}
	else {
		log_line(SCS_LOG_TYPE_warning, "Unsupported game, some features or values might behave incorrectly");
	}

	// Register for events. Note that failure to register those basic events
	// likely indicates invalid usage of the api or some critical problem. As the
	// example requires all of them, we can not continue if the registration fails.

	const bool events_registered =
		(version_params->register_for_event(SCS_TELEMETRY_EVENT_paused, telemetry_pause, NULL) == SCS_RESULT_ok) &&
		(version_params->register_for_event(SCS_TELEMETRY_EVENT_started, telemetry_pause, NULL) == SCS_RESULT_ok) &&
		(version_params->register_for_event(SCS_TELEMETRY_EVENT_configuration, telemetry_configuration, NULL) == SCS_RESULT_ok)
		;
	if (!events_registered) {

		// Registrations created by unsuccessful initialization are
		// cleared automatically so we can simply exit.

		log_line(SCS_LOG_TYPE_error, "Unable to register event callbacks");
		game_log = NULL;
		return SCS_RESULT_generic_error;
	}

	// Initialize the shared memory.

	if (!initialize_shared_memory()) {
		log_line(SCS_LOG_TYPE_error, "Unable to initialize shared memory");
		game_log = NULL;
		return SCS_RESULT_generic_error;
	}

	// Register all changes we are interested in. Note that some wheel-related channels will be initialized when we
	// receive a configuration event. The channel might be missing if the game does not support it (SCS_RESULT_not_found)
	// or if does not support the requested type (SCS_RESULT_unsupported_type). For purpose of this example we ignore
	// the failures so the unsupported channels will remain at theirs default value.

#define register_channel(name, index, type, field) version_params->register_for_channel(SCS_TELEMETRY_##name, index, SCS_VALUE_TYPE_##type, SCS_TELEMETRY_CHANNEL_FLAG_no_value, telemetry_store_##type, &shared_memory->field);

	register_channel(TRUCK_CHANNEL_world_placement, SCS_U32_NIL, dplacement, ws_truck_placement);

	register_channel(TRUCK_CHANNEL_lblinker, SCS_U32_NIL, bool, lblinker);
	register_channel(TRUCK_CHANNEL_rblinker, SCS_U32_NIL, bool, rblinker);
	register_channel(TRUCK_CHANNEL_light_low_beam, SCS_U32_NIL, bool, lowBeamLight);
	register_channel(TRUCK_CHANNEL_light_high_beam, SCS_U32_NIL, bool, hiBeamLight);
	register_channel(TRUCK_CHANNEL_light_parking, SCS_U32_NIL, bool, parkingLights);

	register_channel(TRUCK_CHANNEL_speed, SCS_U32_NIL, float, speedometer_speed);
	register_channel(TRUCK_CHANNEL_engine_rpm, SCS_U32_NIL, float, rpm);

	register_channel(TRUCK_CHANNEL_fuel, SCS_U32_NIL, float, fuel);

	register_channel(TRUCK_CHANNEL_engine_gear, SCS_U32_NIL, s32, gear);
	register_channel(TRUCK_CHANNEL_displayed_gear, SCS_U32_NIL, s32, displayedGear);
	register_channel(CHANNEL_game_time, SCS_U32_NIL, u32, gameTime);
	register_channel(TRUCK_CHANNEL_navigation_time, SCS_U32_NIL, float, navigationTime);

	register_channel(TRUCK_CHANNEL_input_steering, SCS_U32_NIL, float, steering);
	register_channel(TRUCK_CHANNEL_input_throttle, SCS_U32_NIL, float, throttle);
	register_channel(TRUCK_CHANNEL_input_brake, SCS_U32_NIL, float, brake);
	register_channel(TRUCK_CHANNEL_input_clutch, SCS_U32_NIL, float, clutch);

	register_channel(TRUCK_CHANNEL_local_linear_velocity, SCS_U32_NIL, fvector, linear_valocity);
	register_channel(TRUCK_CHANNEL_local_angular_velocity, SCS_U32_NIL, fvector, angular_velocity);
	register_channel(TRUCK_CHANNEL_local_linear_acceleration, SCS_U32_NIL, fvector, linear_acceleration);
	register_channel(TRUCK_CHANNEL_local_angular_acceleration, SCS_U32_NIL, fvector, angular_acceleration);
	register_channel(TRUCK_CHANNEL_cabin_angular_velocity, SCS_U32_NIL, fvector, cabin_angular_velocity);
	register_channel(TRUCK_CHANNEL_cabin_angular_acceleration, SCS_U32_NIL, fvector, cabin_angular_acceleration);

	// extra data

	register_channel(CHANNEL_local_scale, SCS_U32_NIL, float, localScale);
	register_channel(TRUCK_CHANNEL_adblue, SCS_U32_NIL, float, truckAdblueFuelLevelLiters);
	register_channel(TRUCK_CHANNEL_fuel_average_consumption, SCS_U32_NIL, float, truckFuelConsumptionAverageLiters);
	register_channel(TRUCK_CHANNEL_fuel_average_consumption, SCS_U32_NIL, float, truckFuelConsumptionAverageLiters);
	register_channel(TRUCK_CHANNEL_cruise_control, SCS_U32_NIL, float, truckCruise_controlSpeedMS);
	register_channel(TRUCK_CHANNEL_fuel_range, SCS_U32_NIL, float, truckFuelRangeKm);
	register_channel(TRUCK_CHANNEL_battery_voltage, SCS_U32_NIL, float, truckBatteryVoltage);
	register_channel(TRUCK_CHANNEL_odometer, SCS_U32_NIL, float, truckOdometerKM);
	register_channel(TRUCK_CHANNEL_navigation_distance, SCS_U32_NIL, float, truckNavigationDistanceMeters);
	register_channel(TRUCK_CHANNEL_navigation_time, SCS_U32_NIL, float, truckNavigationTimeSeconds);
	register_channel(TRUCK_CHANNEL_navigation_speed_limit, SCS_U32_NIL, float, truckNavigationSpeedLimitMS);
	register_channel(TRUCK_CHANNEL_oil_pressure, SCS_U32_NIL, float, truckOilPressure);
	register_channel(TRUCK_CHANNEL_oil_temperature, SCS_U32_NIL, float, truckOilTemperature);
	register_channel(TRUCK_CHANNEL_water_temperature, SCS_U32_NIL, float, truckWaterTemperature);

	register_channel(CHANNEL_multiplayer_time_offset, SCS_U32_NIL, u32, multiplayerTimeOffset);
	register_channel(CHANNEL_next_rest_stop, SCS_U32_NIL, u32, restStop);

	register_channel(TRUCK_CHANNEL_parking_brake, SCS_U32_NIL, bool, truckBrakeParking);
	register_channel(TRUCK_CHANNEL_motor_brake, SCS_U32_NIL, bool, truckBrakeMotor);
	register_channel(TRUCK_CHANNEL_fuel_warning, SCS_U32_NIL, bool, truckFuelWarning);
	register_channel(TRUCK_CHANNEL_battery_voltage_warning, SCS_U32_NIL, bool, truckBatteryVoltageWarning);
	register_channel(TRUCK_CHANNEL_electric_enabled, SCS_U32_NIL, bool, truckElectricEnabled);
	register_channel(TRUCK_CHANNEL_engine_enabled, SCS_U32_NIL, bool, truckEngineEnabled);
	register_channel(TRUCK_CHANNEL_hazard_warning, SCS_U32_NIL, bool, truckHazardWarning);
	register_channel(TRUCK_CHANNEL_wipers, SCS_U32_NIL, bool, truckWipers);

#undef register_channel

	// Remember other the functions we will use in the future.

	register_for_channel = version_params->register_for_channel;
	unregister_from_channel = version_params->unregister_from_channel;

	// We are done.

	log_line(SCS_LOG_TYPE_message, "Memory telemetry initialized");
	return SCS_RESULT_ok;
}

/**
 * @brief Telemetry API deinitialization function.
 *
 * See scssdk_telemetry.h
 */
SCSAPI_VOID scs_telemetry_shutdown(void)
{
	// Any cleanup needed. The registrations will be removed automatically
	// so there is no need to do that manually.

	deinitialize_shared_memory();

	unregister_from_channel = NULL;
	register_for_channel = NULL;
	game_log = NULL;
}

// Cleanup

BOOL APIENTRY DllMain(
	HMODULE module,
	DWORD  reason_for_call,
	LPVOID reseved
)
{
	return TRUE;
}

// EOF //
