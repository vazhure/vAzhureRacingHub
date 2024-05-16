/*
 rFactorSharedMemoryMap.cpp
 by Dan Allongo (daniel.s.allongo@gmail.com)

 Based on the ISI sample code found at http://rfactor.net/web/rf1/devcorner/
 Provides a basic memory map export of the telemetry and scoring data
 The export is nearly 1:1 with redundant/gratuitous data removed from the 
 vehicle info array and the addition of vehicle speed being pre-calculated
 since everyone needs that anyway. Position, rotation, and orientation are
 interpolated in between scoring updates (every 0.5 seconds). True raw values 
 are given when deltaTime == 0.
*/

#include "rFactorSharedMemoryMap.hpp"
#include <math.h>
#include <stdio.h>


// plugin information
unsigned g_uPluginID          = 0;
char     g_szPluginName[]     = PLUGIN_NAME;
unsigned g_uPluginVersion     = 003;
unsigned g_uPluginObjectCount = 1;
InternalsPluginInfo g_PluginInfo;

// interface to plugin information
extern "C" __declspec(dllexport)
const char* __cdecl GetPluginName() { return g_szPluginName; }

extern "C" __declspec(dllexport)
unsigned __cdecl GetPluginVersion() { return g_uPluginVersion; }

extern "C" __declspec(dllexport)
unsigned __cdecl GetPluginObjectCount() { return g_uPluginObjectCount; }

// get the plugin-info object used to create the plugin.
extern "C" __declspec(dllexport)
PluginObjectInfo* __cdecl GetPluginObjectInfo( const unsigned uIndex ) {
  switch(uIndex) {
    case 0:
      return  &g_PluginInfo;
    default:
      return 0;
  }
}


// InternalsPluginInfo class

InternalsPluginInfo::InternalsPluginInfo() {
  // put together a name for this plugin
  sprintf( m_szFullName, "%s - %s", g_szPluginName, InternalsPluginInfo::GetName() );
}

const char*    InternalsPluginInfo::GetName()     const { return SharedMemoryMapPlugin::GetName(); }
const char*    InternalsPluginInfo::GetFullName() const { return m_szFullName; }
const char*    InternalsPluginInfo::GetDesc()     const { return g_szPluginName; }
const unsigned InternalsPluginInfo::GetType()     const { return SharedMemoryMapPlugin::GetType(); }
const char*    InternalsPluginInfo::GetSubType()  const { return SharedMemoryMapPlugin::GetSubType(); }
const unsigned InternalsPluginInfo::GetVersion()  const { return SharedMemoryMapPlugin::GetVersion(); }
void*          InternalsPluginInfo::Create()      const { return new SharedMemoryMapPlugin(); }


// InternalsPlugin class

const char SharedMemoryMapPlugin::m_szName[] = PLUGIN_NAME;
const char SharedMemoryMapPlugin::m_szSubType[] = "Internals";
const unsigned SharedMemoryMapPlugin::m_uID = 1;
const unsigned SharedMemoryMapPlugin::m_uVersion = 3;  // set to 3 for InternalsPluginV3 functionality and added graphical and vehicle info

PluginObjectInfo *SharedMemoryMapPlugin::GetInfo() {
  return &g_PluginInfo;
}

void SharedMemoryMapPlugin::Startup() {
	char tag[256] = {};
	strcpy(tag, RF_SHARED_MEMORY_NAME);
	char exe[1024] = {};
	GetModuleFileName(NULL, exe, sizeof(exe));
	char pid[8] = {};
	sprintf(pid, "%d", GetCurrentProcessId());
	// append processId for dedicated server to allow multiple instances
	if (strstr(exe, "Dedicated.exe") != NULL) {
		strcat(tag, pid);
	}
	// init handle and try to create, read if existing
	hMap = INVALID_HANDLE_VALUE;
	hMap = CreateFileMapping(INVALID_HANDLE_VALUE, NULL, PAGE_READWRITE, 0, sizeof(rfShared), TEXT(tag));
	if (hMap == NULL) {
		if (GetLastError() == (DWORD)183) {
			hMap = OpenFileMapping(FILE_MAP_ALL_ACCESS, FALSE, TEXT(tag));
			if (hMap == NULL) {
				// unable to create or read existing
				mapped = FALSE;
				return;
			}
		}
	}
	pBuf = (rfShared*)MapViewOfFile(hMap, FILE_MAP_ALL_ACCESS, 0, 0, sizeof(rfShared));
	if (pBuf == NULL) {
		// failed to map memory buffer
		CloseHandle(hMap);
		mapped = FALSE;
		return;
	}
	mapped = TRUE;
	if (mapped) {
		memset(pBuf, 0, sizeof(rfShared));
		strcpy(pBuf->version, RF_SHARED_MEMORY_VERSION);
	}
	return;
}

void SharedMemoryMapPlugin::Shutdown() {
	// release buffer and close handle
	if (mapped) {
		memset(pBuf, 0, sizeof(rfShared));
	}
	if (pBuf) {
		UnmapViewOfFile(pBuf);
	}
	if (hMap) {
		CloseHandle(hMap);
	}
	mapped = FALSE;
}

void SharedMemoryMapPlugin::StartSession() {
	// zero-out buffer at start of session
	if (mapped) {
		memset(pBuf, 0, sizeof(rfShared));
		strcpy(pBuf->version, RF_SHARED_MEMORY_VERSION);
	}
	cLastScoringUpdate = 0;
	cDelta = 0;
	scoring = { 0 };
}

void SharedMemoryMapPlugin::EndSession() {
	// zero-out buffer at end of session
	StartSession();
}

void SharedMemoryMapPlugin::EnterRealtime() {
	inRealtime = TRUE;
}

void SharedMemoryMapPlugin::ExitRealtime() {
	inRealtime = FALSE;
}

void SharedMemoryMapPlugin::UpdateTelemetry( const TelemInfoV2 &info ) {
	if (mapped) {
		// update clock delta
		cDelta = (float)(clock() - cLastScoringUpdate) / (float)CLOCKS_PER_SEC;

		// TelemInfoBase
		pBuf->deltaTime = cDelta;
		pBuf->lapNumber = info.mLapNumber;
		pBuf->lapStartET = info.mLapStartET;
		strcpy(pBuf->trackName, info.mTrackName);
		pBuf->pos = { info.mPos.x, info.mPos.y, info.mPos.z };
		pBuf->localVel = { info.mLocalVel.x, info.mLocalVel.y, info.mLocalVel.z };
		pBuf->localAccel = { info.mLocalAccel.x, info.mLocalAccel.y, info.mLocalAccel.z };
		pBuf->oriX = { info.mOriX.x, info.mOriX.y, info.mOriX.z };
		pBuf->oriY = { info.mOriY.x, info.mOriY.y, info.mOriY.z };
		pBuf->oriZ = { info.mOriZ.x, info.mOriZ.y, info.mOriZ.z };
		pBuf->localRot = { info.mLocalRot.x, info.mLocalRot.y, info.mLocalRot.z };
		pBuf->localRotAccel = { info.mLocalRotAccel.x, info.mLocalRotAccel.y, info.mLocalRotAccel.z };
		pBuf->speed = sqrtf((info.mLocalVel.x * info.mLocalVel.x) +
			(info.mLocalVel.y * info.mLocalVel.y) +
			(info.mLocalVel.z * info.mLocalVel.z));
		pBuf->gear = info.mGear;
		pBuf->engineRPM = info.mEngineRPM;
		pBuf->engineWaterTemp = info.mEngineWaterTemp;
		pBuf->engineOilTemp = info.mEngineOilTemp;
		pBuf->clutchRPM = info.mClutchRPM;
		pBuf->unfilteredThrottle = info.mUnfilteredThrottle;
		pBuf->unfilteredBrake = info.mUnfilteredBrake;
		pBuf->unfilteredSteering = info.mUnfilteredSteering;
		pBuf->unfilteredClutch = info.mUnfilteredClutch;
		pBuf->steeringArmForce = info.mSteeringArmForce;

		// TelemInfoV2
		pBuf->fuel = info.mFuel;
		pBuf->engineMaxRPM = info.mEngineMaxRPM;
		pBuf->scheduledStops = info.mScheduledStops;
		pBuf->overheating = info.mOverheating;
		pBuf->detached = info.mDetached;
		for (int i = 0; i < 8; i++) {
			pBuf->dentSeverity[i] = info.mDentSeverity[i];
		}
		pBuf->lastImpactET = info.mLastImpactET;
		pBuf->lastImpactMagnitude = info.mLastImpactMagnitude;
		pBuf->lastImpactPos = { info.mLastImpactPos.x, info.mLastImpactPos.y, info.mLastImpactPos.z };

		for (int i = 0; i < 4; i++) {
			// TelemWheel
			pBuf->wheel[i].rotation = info.mWheel[i].mRotation;
			pBuf->wheel[i].suspensionDeflection = info.mWheel[i].mSuspensionDeflection;
			pBuf->wheel[i].rideHeight = info.mWheel[i].mRideHeight;
			pBuf->wheel[i].tireLoad = info.mWheel[i].mTireLoad;
			pBuf->wheel[i].lateralForce = info.mWheel[i].mLateralForce;
			pBuf->wheel[i].gripFract = info.mWheel[i].mGripFract;
			pBuf->wheel[i].brakeTemp = info.mWheel[i].mBrakeTemp;
			pBuf->wheel[i].pressure = info.mWheel[i].mPressure;
			for (int j = 0; j < 3; j++) {
				pBuf->wheel[i].temperature[j] = info.mWheel[i].mTemperature[j];
			}

			//TelemWheelV2
			pBuf->wheel[i].wear = info.mWheel[i].mWear;
			strcpy(pBuf->wheel[i].terrainName, info.mWheel[i].mTerrainName);
			pBuf->wheel[i].surfaceType = info.mWheel[i].mSurfaceType;
			pBuf->wheel[i].flat = info.mWheel[i].mFlat;
			pBuf->wheel[i].detached = info.mWheel[i].mDetached;
		}
		
		// interpolation of scoring info
		if (cDelta > 0.0f && cDelta < 0.55f) {
			// ScoringInfoBase
			pBuf->currentET = scoring.currentET + cDelta;

			// ScoringInfoV2
			pBuf->inRealtime = inRealtime;

			for (int i = 0; i < RF_SHARED_MEMORY_MAX_VSI_SIZE; i++) {
				if (i < scoring.numVehicles) {
					// VehicleScoringInfoV2
					// applying 0.05x acceleration seems to help the interpolation
					rfVec3 localRotAccel = { scoring.vehicle[i].localRotAccel.x, scoring.vehicle[i].localRotAccel.y, scoring.vehicle[i].localRotAccel.z };
					rfVec3 localAccel = { scoring.vehicle[i].localAccel.x, scoring.vehicle[i].localAccel.y, scoring.vehicle[i].localAccel.z };
					rfVec3 localRot = { scoring.vehicle[i].localRot.x, scoring.vehicle[i].localRot.y, scoring.vehicle[i].localRot.z };
					rfVec3 localVel = { scoring.vehicle[i].localVel.x, scoring.vehicle[i].localVel.y, scoring.vehicle[i].localVel.z };
					
					localRot.x += localRotAccel.x * cDelta * RF_SHARED_MEMORY_ACC_SMOOTH_FACTOR;
					localRot.y += localRotAccel.y * cDelta * RF_SHARED_MEMORY_ACC_SMOOTH_FACTOR;
					localRot.z += localRotAccel.z * cDelta * RF_SHARED_MEMORY_ACC_SMOOTH_FACTOR;

					localVel.x += localAccel.x * cDelta * RF_SHARED_MEMORY_ACC_SMOOTH_FACTOR;
					localVel.y += localAccel.y * cDelta * RF_SHARED_MEMORY_ACC_SMOOTH_FACTOR;
					localVel.z += localAccel.z * cDelta * RF_SHARED_MEMORY_ACC_SMOOTH_FACTOR;

					// rotate and normalize orientation vectors (normalizing shouldn't be necessary if this is correct)
					rfVec3 oriX = { scoring.vehicle[i].oriX.x, scoring.vehicle[i].oriX.y, scoring.vehicle[i].oriX.z };
					rfVec3 oriY = { scoring.vehicle[i].oriY.x, scoring.vehicle[i].oriY.y, scoring.vehicle[i].oriY.z };
					rfVec3 oriZ = { scoring.vehicle[i].oriZ.x, scoring.vehicle[i].oriZ.y, scoring.vehicle[i].oriZ.z };
					rfVec3 wRot = { ((oriX.x * localRot.x) + (oriX.y * localRot.y) + (oriX.z * localRot.z)) * cDelta * RF_SHARED_MEMORY_ROT_SMOOTH_FACTOR,
						((oriY.x * localRot.x) + (oriY.y * localRot.y) + (oriY.z * localRot.z)) * cDelta * RF_SHARED_MEMORY_ROT_SMOOTH_FACTOR,
						((oriZ.x * localRot.x) + (oriZ.y * localRot.y) + (oriZ.z * localRot.z)) * cDelta * RF_SHARED_MEMORY_ROT_SMOOTH_FACTOR };
					rfVec3 tmpX, tmpY, tmpZ;
					float tmpLen;
					
					// X
					// rotate by z
					tmpZ.x = oriX.x * cosf(wRot.z) - oriX.y * -sinf(wRot.z);
					tmpZ.y = oriX.x * -sinf(wRot.z) + oriX.y * cosf(wRot.z);
					tmpZ.z = oriX.z;
					// rotate by y
					tmpY.x = tmpZ.x * cosf(wRot.y) + tmpZ.z * -sinf(wRot.y);
					tmpY.y = tmpZ.y;
					tmpY.z = tmpZ.z * cosf(wRot.y) - tmpZ.x * -sinf(wRot.y);
					// rotate by x
					tmpX.x = tmpY.x;
					tmpX.y = tmpY.y * cosf(wRot.x) - tmpY.z * -sinf(wRot.x);
					tmpX.z = tmpY.y * -sinf(wRot.x) + tmpY.z * cosf(wRot.x);
					tmpLen = sqrtf(tmpX.x * tmpX.x + tmpX.y * tmpX.y + tmpX.z * tmpX.z);
					if (tmpLen > 0) {
						oriX = { tmpX.x / tmpLen, tmpX.y / tmpLen, tmpX.z / tmpLen };
					}

					// Y
					// rotate by z
					tmpZ.x = oriY.x * cosf(wRot.z) - oriY.y * -sinf(wRot.z);
					tmpZ.y = oriY.x * -sinf(wRot.z) + oriY.y * cosf(wRot.z);
					tmpZ.z = oriY.z;
					// rotate by y
					tmpY.x = tmpZ.x * cosf(wRot.y) + tmpZ.z * -sinf(wRot.y);
					tmpY.y = tmpZ.y;
					tmpY.z = tmpZ.z * cosf(wRot.y) - tmpZ.x * -sinf(wRot.y);
					// rotate by x
					tmpX.x = tmpY.x;
					tmpX.y = tmpY.y * cosf(wRot.x) - tmpY.z * -sinf(wRot.x);
					tmpX.z = tmpY.y * -sinf(wRot.x) + tmpY.z * cosf(wRot.x);
					tmpLen = sqrtf(tmpX.x * tmpX.x + tmpX.y * tmpX.y + tmpX.z * tmpX.z);
					if (tmpLen > 0) {
						oriY = { tmpX.x / tmpLen, tmpX.y / tmpLen, tmpX.z / tmpLen };
					}

					// Z
					// rotate by z
					tmpZ.x = oriZ.x * cosf(wRot.z) - oriZ.y * -sinf(wRot.z);
					tmpZ.y = oriZ.x * -sinf(wRot.z) + oriZ.y * cosf(wRot.z);
					tmpZ.z = oriZ.z;
					// rotate by y
					tmpY.x = tmpZ.x * cosf(wRot.y) + tmpZ.z * -sinf(wRot.y);
					tmpY.y = tmpZ.y;
					tmpY.z = tmpZ.z * cosf(wRot.y) - tmpZ.x * -sinf(wRot.y);
					// rotate by x
					tmpX.x = tmpY.x;
					tmpX.y = tmpY.y * cosf(wRot.x) - tmpY.z * -sinf(wRot.x);
					tmpX.z = tmpY.y * -sinf(wRot.x) + tmpY.z * cosf(wRot.x);
					tmpLen = sqrtf(tmpX.x * tmpX.x + tmpX.y * tmpX.y + tmpX.z * tmpX.z);
					if (tmpLen > 0) {
						oriZ = { tmpX.x / tmpLen, tmpX.y / tmpLen, tmpX.z / tmpLen };
					}
					
					// position
					rfVec3 pos = { scoring.vehicle[i].pos.x, scoring.vehicle[i].pos.y, scoring.vehicle[i].pos.z };
					pos.x += ((oriX.x * localVel.x) + (oriX.y * localVel.y) + (oriX.z * localVel.z)) * cDelta;
					pos.y += ((oriY.x * localVel.x) + (oriY.y * localVel.y) + (oriY.z * localVel.z)) * cDelta;
					pos.z += ((oriZ.x * localVel.x) + (oriZ.y * localVel.y) + (oriZ.z * localVel.z)) * cDelta;
					pBuf->vehicle[i].pos = { pos.x, pos.y, pos.z };

					pBuf->vehicle[i].yaw = atan2f(oriZ.x, oriZ.z);
					pBuf->vehicle[i].pitch = atan2f(-oriY.z, sqrtf(oriX.z * oriX.z + oriZ.z * oriZ.z));
					pBuf->vehicle[i].roll = atan2f(oriY.x, sqrtf(oriX.x * oriX.x + oriZ.x * oriZ.x));

					// interpolate speed
					pBuf->vehicle[i].speed = sqrtf((localVel.x * localVel.x) + (localVel.y * localVel.y) + (localVel.z * localVel.z));

					// VehicleScoringInfo
					pBuf->vehicle[i].lapDist = scoring.vehicle[i].lapDist - localVel.z * cDelta;

					continue;
				}
			}
		}
	}
}

void SharedMemoryMapPlugin::UpdateScoring( const ScoringInfoV2 &info ) {
	if (mapped) {
		cLastScoringUpdate = clock();

		pBuf->deltaTime = 0;

		// update internal state
		scoring.currentET = info.mCurrentET;
		scoring.numVehicles = info.mNumVehicles;
		strcpy(scoring.plrFileName, info.mPlrFileName);
		for (int i = 0; i < RF_SHARED_MEMORY_MAX_VSI_SIZE; i++) {
			if (i < scoring.numVehicles) {
				scoring.vehicle[i].lapDist = info.mVehicle[i].mLapDist;
				scoring.vehicle[i].localAccel = { info.mVehicle[i].mLocalAccel.x, info.mVehicle[i].mLocalAccel.y, info.mVehicle[i].mLocalAccel.z };
				scoring.vehicle[i].localRot = { info.mVehicle[i].mLocalRot.x, info.mVehicle[i].mLocalRot.y, info.mVehicle[i].mLocalRot.z };
				scoring.vehicle[i].localRotAccel = { info.mVehicle[i].mLocalRotAccel.x, info.mVehicle[i].mLocalRotAccel.y, info.mVehicle[i].mLocalRotAccel.z };
				scoring.vehicle[i].localVel = { info.mVehicle[i].mLocalVel.x, info.mVehicle[i].mLocalVel.y, info.mVehicle[i].mLocalVel.z };
				scoring.vehicle[i].oriX = { info.mVehicle[i].mOriX.x, info.mVehicle[i].mOriX.y, info.mVehicle[i].mOriX.z };
				scoring.vehicle[i].oriY = { info.mVehicle[i].mOriY.x, info.mVehicle[i].mOriY.y, info.mVehicle[i].mOriY.z };
				scoring.vehicle[i].oriZ = { info.mVehicle[i].mOriZ.x, info.mVehicle[i].mOriZ.y, info.mVehicle[i].mOriZ.z };
				scoring.vehicle[i].pos = { info.mVehicle[i].mPos.x, info.mVehicle[i].mPos.y, info.mVehicle[i].mPos.z };
				continue;
			}
			scoring.vehicle[i] = { 0 };
		}

		// ScoringInfoBase
		pBuf->session = info.mSession;
		pBuf->currentET = info.mCurrentET;
		pBuf->endET = info.mEndET;
		pBuf->maxLaps = info.mMaxLaps;
		pBuf->lapDist = info.mLapDist;
		pBuf->numVehicles = info.mNumVehicles;

		// ScoringInfoV2
		pBuf->gamePhase = info.mGamePhase;
		pBuf->yellowFlagState = info.mYellowFlagState;
		for (int i = 0; i < 3; i++) {
			pBuf->sectorFlag[i] = info.mSectorFlag[i];
		}
		pBuf->startLight = info.mStartLight;
		pBuf->numRedLights = info.mNumRedLights;
		pBuf->inRealtime = inRealtime;
		strcpy(pBuf->playerName, info.mPlayerName);
		pBuf->ambientTemp = info.mAmbientTemp;
		pBuf->trackTemp = info.mTrackTemp;
		pBuf->wind = { info.mWind.x, info.mWind.y, info.mWind.z };

		for (int i = 0; i < RF_SHARED_MEMORY_MAX_VSI_SIZE; i++) {
			if (i < info.mNumVehicles) {
				// VehicleScoringInfo
				strcpy(pBuf->vehicle[i].driverName, info.mVehicle[i].mDriverName);
				pBuf->vehicle[i].totalLaps = info.mVehicle[i].mTotalLaps;
				pBuf->vehicle[i].sector = info.mVehicle[i].mSector;
				pBuf->vehicle[i].finishStatus = info.mVehicle[i].mFinishStatus;
				pBuf->vehicle[i].lapDist = info.mVehicle[i].mLapDist;
				pBuf->vehicle[i].pathLateral = info.mVehicle[i].mPathLateral;
				pBuf->vehicle[i].trackEdge = info.mVehicle[i].mTrackEdge;
				pBuf->vehicle[i].bestSector1 = info.mVehicle[i].mBestSector1;
				pBuf->vehicle[i].bestSector2 = info.mVehicle[i].mBestSector2;
				pBuf->vehicle[i].bestLapTime = info.mVehicle[i].mBestLapTime;
				pBuf->vehicle[i].lastSector1 = info.mVehicle[i].mLastSector1;
				pBuf->vehicle[i].lastSector2 = info.mVehicle[i].mLastSector2;
				pBuf->vehicle[i].lastLapTime = info.mVehicle[i].mLastLapTime;
				pBuf->vehicle[i].curSector1 = info.mVehicle[i].mCurSector1;
				pBuf->vehicle[i].curSector2 = info.mVehicle[i].mCurSector2;
				pBuf->vehicle[i].numPitstops = info.mVehicle[i].mNumPitstops;
				pBuf->vehicle[i].numPenalties = info.mVehicle[i].mNumPenalties;

				// VehicleScoringInfoV2
				pBuf->vehicle[i].isPlayer = info.mVehicle[i].mIsPlayer;
				pBuf->vehicle[i].control = info.mVehicle[i].mControl;
				pBuf->vehicle[i].inPits = info.mVehicle[i].mInPits;
				pBuf->vehicle[i].place = info.mVehicle[i].mPlace;
				strcpy(pBuf->vehicle[i].vehicleClass, info.mVehicle[i].mVehicleClass);
				pBuf->vehicle[i].timeBehindNext = info.mVehicle[i].mTimeBehindNext;
				pBuf->vehicle[i].lapsBehindNext = info.mVehicle[i].mLapsBehindNext;
				pBuf->vehicle[i].timeBehindLeader = info.mVehicle[i].mTimeBehindLeader;
				pBuf->vehicle[i].lapsBehindLeader = info.mVehicle[i].mLapsBehindLeader;
				pBuf->vehicle[i].lapStartET = info.mVehicle[i].mLapStartET;
				pBuf->vehicle[i].pos = { info.mVehicle[i].mPos.x, info.mVehicle[i].mPos.y, info.mVehicle[i].mPos.z };
				pBuf->vehicle[i].yaw = atan2f(info.mVehicle[i].mOriZ.x, info.mVehicle[i].mOriZ.z);
				pBuf->vehicle[i].pitch = atan2f(-info.mVehicle[i].mOriY.z, 
					sqrtf(info.mVehicle[i].mOriX.z * info.mVehicle[i].mOriX.z + 
						info.mVehicle[i].mOriZ.z * info.mVehicle[i].mOriZ.z));
				pBuf->vehicle[i].roll = atan2f(info.mVehicle[i].mOriY.x, 
					sqrtf(info.mVehicle[i].mOriX.x * info.mVehicle[i].mOriX.x + 
						info.mVehicle[i].mOriZ.x * info.mVehicle[i].mOriZ.x));
				pBuf->vehicle[i].speed = sqrtf((info.mVehicle[i].mLocalVel.x * info.mVehicle[i].mLocalVel.x) +
					(info.mVehicle[i].mLocalVel.y * info.mVehicle[i].mLocalVel.y) +
					(info.mVehicle[i].mLocalVel.z * info.mVehicle[i].mLocalVel.z));
				continue;
			}
			pBuf->vehicle[i] = { 0 };
		}
	}
}
