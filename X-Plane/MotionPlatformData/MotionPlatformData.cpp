// Downloaded from https://developer.x-plane.com/code-sample/motionplatformdata/
// Modified by Andrey Zhuravlev


/*
Plugin to show how to derive motion platform data from our datarefs
Thanks to Austin for allowing us to use the original Xplane conversion code.

Version 1.0.0.1			Intitial Sandy Barbour - 05/08/2007
*/

#include <stdio.h>
#include <string.h>

#include "XPLMDisplay.h"
#include "XPLMGraphics.h"
#include "XPLMProcessing.h"
#include "XPLMDataAccess.h"

#define SHARED_MEM_NAME "Local\\XPlaneMotionData"

// Globals.
// Use MPD_ as a prefix for the global variables

// Used to store calculated motion data

// declaration of initialize_shared_memory
static bool initialize_shared_memory(void);
// declaration of deinitialize_shared_memory
static void deinitialize_shared_memory(void);

struct telemetry_state_t
{
	float Pitch;
	float Yaw;
	float Roll;
	float Side;
	float Normal;
	float Axil;
};

telemetry_state_t* shared_memory = nullptr;

// Window ID
XPLMWindowID MPD_Window = NULL;

// Datarefs
XPLMDataRef	MPD_DR_groundspeed = NULL;
XPLMDataRef	MPD_DR_fnrml_prop = NULL;
XPLMDataRef	MPD_DR_fside_prop = NULL;
XPLMDataRef	MPD_DR_faxil_prop = NULL;
XPLMDataRef	MPD_DR_fnrml_aero = NULL;
XPLMDataRef	MPD_DR_fside_aero = NULL;
XPLMDataRef	MPD_DR_faxil_aero = NULL;
XPLMDataRef	MPD_DR_fnrml_gear = NULL;
XPLMDataRef	MPD_DR_fside_gear = NULL;
XPLMDataRef	MPD_DR_faxil_gear = NULL;
XPLMDataRef	MPD_DR_m_total = NULL;
XPLMDataRef	MPD_DR_the = NULL;
XPLMDataRef	MPD_DR_psi = NULL;
XPLMDataRef	MPD_DR_phi = NULL;

//---------------------------------------------------------------------------
// Function prototypes

float MotionPlatformDataLoopCB(float elapsedMe, float elapsedSim, int counter, void* refcon);

void MotionPlatformDataDrawWindowCallback(
	XPLMWindowID         inWindowID,
	void* inRefcon);

void MotionPlatformDataHandleKeyCallback(
	XPLMWindowID         inWindowID,
	char                 inKey,
	XPLMKeyFlags         inFlags,
	char                 inVirtualKey,
	void* inRefcon,
	int                  losingFocus);

int MotionPlatformDataHandleMouseClickCallback(
	XPLMWindowID         inWindowID,
	int                  x,
	int                  y,
	XPLMMouseStatus      inMouse,
	void* inRefcon);

float MPD_fallout(float data, float low, float high);
float MPD_fltlim(float data, float min, float max);
float MPD_fltmax2(float x1, const float x2);
void MPD_CalculateMotionData(void);

//---------------------------------------------------------------------------
// SDK Mandatory Callbacks

PLUGIN_API int XPluginStart(
	char* outName,
	char* outSig,
	char* outDesc)
{
	strcpy(outName, "MotionPlatformData");
	strcpy(outSig, "vAzhureRacingHub.motionplatformdata");
	strcpy(outDesc, "A plug-in that derives motion platform data from datarefs.");

	initialize_shared_memory();

	XPLMRegisterFlightLoopCallback(MotionPlatformDataLoopCB, 1.0, NULL);

	MPD_DR_groundspeed = XPLMFindDataRef("sim/flightmodel/position/groundspeed");
	MPD_DR_fnrml_prop = XPLMFindDataRef("sim/flightmodel/forces/fnrml_prop");
	MPD_DR_fside_prop = XPLMFindDataRef("sim/flightmodel/forces/fside_prop");
	MPD_DR_faxil_prop = XPLMFindDataRef("sim/flightmodel/forces/faxil_prop");
	MPD_DR_fnrml_aero = XPLMFindDataRef("sim/flightmodel/forces/fnrml_aero");
	MPD_DR_fside_aero = XPLMFindDataRef("sim/flightmodel/forces/fside_aero");
	MPD_DR_faxil_aero = XPLMFindDataRef("sim/flightmodel/forces/faxil_aero");
	MPD_DR_fnrml_gear = XPLMFindDataRef("sim/flightmodel/forces/fnrml_gear");
	MPD_DR_fside_gear = XPLMFindDataRef("sim/flightmodel/forces/fside_gear");
	MPD_DR_faxil_gear = XPLMFindDataRef("sim/flightmodel/forces/faxil_gear");
	MPD_DR_m_total = XPLMFindDataRef("sim/flightmodel/weight/m_total");
	MPD_DR_the = XPLMFindDataRef("sim/flightmodel/position/theta");
	MPD_DR_psi = XPLMFindDataRef("sim/flightmodel/position/psi");
	MPD_DR_phi = XPLMFindDataRef("sim/flightmodel/position/phi");

	return 1;
}

//---------------------------------------------------------------------------

PLUGIN_API void	XPluginStop(void)
{
	deinitialize_shared_memory();
	XPLMUnregisterFlightLoopCallback(MotionPlatformDataLoopCB, NULL);
}

//---------------------------------------------------------------------------

PLUGIN_API int XPluginEnable(void)
{
	return 1;
}

//---------------------------------------------------------------------------

PLUGIN_API void XPluginDisable(void)
{
}

//---------------------------------------------------------------------------

PLUGIN_API void XPluginReceiveMessage(XPLMPluginID inFrom, int inMsg, void* inParam)
{
}

//---------------------------------------------------------------------------
// FlightLoop callback to calculate motion data and store it in our buffers

float MotionPlatformDataLoopCB(float elapsedMe, float elapsedSim, int counter, void* refcon)
{
	MPD_CalculateMotionData();

	return (float)0.1;
}

//---------------------------------------------------------------------------
// Original function used in the Xplane code.

float MPD_fallout(float data, float low, float high)
{
	if (data < low) return data;
	if (data > high) return data;
	if (data < ((low + high) * 0.5)) return low;
	return high;
}

//---------------------------------------------------------------------------
// Original function used in the Xplane code.

float MPD_fltlim(float data, float min, float max)
{
	if (data < min) return min;
	if (data > max) return max;
	return data;
}

//---------------------------------------------------------------------------
// Original function used in the Xplane code.

float MPD_fltmax2(float x1, const float x2)
{
	return (x1 > x2) ? x1 : x2;
}

//---------------------------------------------------------------------------
// This is original Xplane code converted to use 
// our datarefs instead of the Xplane variables

void MPD_CalculateMotionData(void)
{
	float groundspeed = XPLMGetDataf(MPD_DR_groundspeed);
	float fnrml_prop = XPLMGetDataf(MPD_DR_fnrml_prop);
	float fside_prop = XPLMGetDataf(MPD_DR_fside_prop);
	float faxil_prop = XPLMGetDataf(MPD_DR_faxil_prop);
	float fnrml_aero = XPLMGetDataf(MPD_DR_fnrml_aero);
	float fside_aero = XPLMGetDataf(MPD_DR_fside_aero);
	float faxil_aero = XPLMGetDataf(MPD_DR_faxil_aero);
	float fnrml_gear = XPLMGetDataf(MPD_DR_fnrml_gear);
	float fside_gear = XPLMGetDataf(MPD_DR_fside_gear);
	float faxil_gear = XPLMGetDataf(MPD_DR_faxil_gear);
	float m_total = XPLMGetDataf(MPD_DR_m_total);
	float the = XPLMGetDataf(MPD_DR_the);
	float psi = XPLMGetDataf(MPD_DR_psi);
	float phi = XPLMGetDataf(MPD_DR_phi);

	float ratio = MPD_fltlim(groundspeed * 0.2, 0.0, 1.0);
	float a_nrml = MPD_fallout(fnrml_prop + fnrml_aero + fnrml_gear, -0.1, 0.1) / MPD_fltmax2(m_total, 1.0);
	float a_side = (fside_prop + fside_aero + fside_gear) / MPD_fltmax2(m_total, 1.0) * ratio;
	float a_axil = (faxil_prop + faxil_aero + faxil_gear) / MPD_fltmax2(m_total, 1.0) * ratio;

	shared_memory->Pitch = the; // Pitch
	shared_memory->Yaw = psi; // Yaw
	shared_memory->Roll = phi; // Roll
	shared_memory->Side = a_side;
	shared_memory->Normal = a_nrml;
	shared_memory->Axil = a_axil;
}

HANDLE memory_mapping = nullptr;

/**
 * @brief Deinitialize the shared memory objects.
 */
static void deinitialize_shared_memory(void)
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
static bool initialize_shared_memory(void)
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

	// Defaults in the structure.

	memset(shared_memory, 0, memory_size);

	return true;
}

//---------------------------------------------------------------------------