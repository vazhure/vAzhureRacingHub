/*
rfSharedStruct.hpp
by Dan Allongo (daniel.s.allongo@gmail.com)

This is the structure of the shared memory map
It's nearly identical to the original structures specified in InternalsPlugin.hpp,
but with pragma pack 1 specified to get the most compact representation.
This means that you need to watch your types very closely!
*/

#pragma once

#define RF_SHARED_MEMORY_NAME "$rFactorShared$"
#define RF_SHARED_MEMORY_VERSION "3.0.0.0"
#define RF_SHARED_MEMORY_MAX_VSI_SIZE 64
#define RF_SHARED_MEMORY_ACC_SMOOTH_FACTOR 0.02f
#define RF_SHARED_MEMORY_ROT_SMOOTH_FACTOR 0.65f

typedef enum {
  garage = 0,
  warmUp = 1,
  gridWalk = 2,
  formation = 3,
  countdown = 4,
  greenFlag = 5,
  fullCourseYellow = 6,
  sessionStopped = 7,
  sessionOver = 8
} rfGamePhase;

typedef enum {
  invalid = -1,
  noFlag = 0,
  pending = 1,
  pitClosed = 2,
  pitLeadLap = 3,
  pitOpen = 4,
  lastLap = 5,
  resume = 6,
  raceHalt = 7
} rfYellowFlagState;

typedef enum {
  dry = 0,
  wet = 1,
  grass = 2,
  dirt = 3,
  gravel = 4,
  kerb = 5
} rfSurfaceType;

typedef enum {
  sector3 = 0,
  sector1 = 1,
  sector2 = 2
} rfSector;

typedef enum {
  none = 0,
  finished = 1,
  dnf = 2,
  dq = 3
} rfFinishStatus;

typedef enum {
  nobody = -1,
  player = 0,
  ai = 1,
  remote = 2,
  replay = 3
} rfControl;

typedef enum {
  frontLeft = 0,
  frontRight = 1,
  rearLeft = 2,
  rearRight = 3
} rfWheelIndex;

#pragma pack(push, 1)

// Our world coordinate system is left-handed, with +y pointing up.
// The local vehicle coordinate system is as follows:
//   +x points out the left side of the car (from the driver's perspective)
//   +y points out the roof
//   +z points out the back of the car
// Rotations are as follows:
//   +x pitches up
//   +y yaws to the right
//   +z rolls to the right

struct rfVec3 {
  float x, y, z;
};

struct rfWheel {
  float rotation;               // radians/sec
  float suspensionDeflection;   // meters
  float rideHeight;             // meters
  float tireLoad;               // Newtons
  float lateralForce;           // Newtons
  float gripFract;              // an approximation of what fraction of the contact patch is sliding
  float brakeTemp;              // Celsius
  float pressure;               // kPa
  float temperature[3];         // Celsius, left/center/right (not to be confused with inside/center/outside!)
  float wear;                   // wear (0.0-1.0, fraction of maximum) ... this is not necessarily proportional with grip loss
  char  terrainName[16];        // the material prefixes from the TDF file
  unsigned char surfaceType;    // 0=dry, 1=wet, 2=grass, 3=dirt, 4=gravel, 5=rumblestrip
  bool flat;                    // whether tire is flat
  bool detached;                // whether wheel is detached
};

// scoring info only updates twice per second (values interpolated when deltaTime > 0)!
struct rfVehicleInfo {
  char driverName[32];          // driver name
  short totalLaps;              // laps completed
  signed char sector;           // 0=sector3, 1=sector1, 2=sector2 (don't ask why)
  signed char finishStatus;     // 0=none, 1=finished, 2=dnf, 3=dq
  float lapDist;                // current distance around track
  float pathLateral;            // lateral position with respect to *very approximate* "center" path
  float trackEdge;              // track edge (w.r.t. "center" path) on same side of track as vehicle

  float bestSector1;            // best sector 1
  float bestSector2;            // best sector 2 (plus sector 1)
  float bestLapTime;            // best lap time
  float lastSector1;            // last sector 1
  float lastSector2;            // last sector 2 (plus sector 1)
  float lastLapTime;            // last lap time
  float curSector1;             // current sector 1 if valid
  float curSector2;             // current sector 2 (plus sector 1) if valid
  // no current laptime because it instantly becomes "last"

  short numPitstops;            // number of pitstops made
  short numPenalties;           // number of outstanding penalties
  bool isPlayer;                // is this the player's vehicle
  signed char control;          // who's in control: -1=nobody (shouldn't get this), 0=local player, 1=local AI, 2=remote, 3=replay (shouldn't get this)
  bool inPits;                  // between pit entrance and pit exit (not always accurate for remote vehicles)
  unsigned char place;          // 1-based position
  char vehicleClass[32];        // vehicle class

  // Dash Indicators
  float timeBehindNext;         // time behind vehicle in next higher place
  long lapsBehindNext;          // laps behind vehicle in next higher place
  float timeBehindLeader;       // time behind leader
  long lapsBehindLeader;        // laps behind leader
  float lapStartET;             // time this lap was started

  // Position and derivatives
  rfVec3 pos;					// world position in meters

  float yaw;					// rad, use (360-yaw*57.2978)%360 for heading in degrees
  float pitch;					// rad
  float roll;					// rad

  float speed;					// meters/sec
};

struct rfShared {
  char version[8];				// API version
  // Time
  float deltaTime;              // time since last scoring update (seconds)
  long lapNumber;               // current lap number
  float lapStartET;             // time this lap was started
  char trackName[64];           // current track name

  // Position and derivatives
  rfVec3 pos;               // world position in meters
  rfVec3 localVel;          // velocity (meters/sec) in local vehicle coordinates
  rfVec3 localAccel;        // acceleration (meters/sec^2) in local vehicle coordinates

  // Orientation and derivatives
  rfVec3 oriX;              // top row of orientation matrix (also converts local vehicle vectors into world X using dot product)
  rfVec3 oriY;              // mid row of orientation matrix (also converts local vehicle vectors into world Y using dot product)
  rfVec3 oriZ;              // bot row of orientation matrix (also converts local vehicle vectors into world Z using dot product)
  rfVec3 localRot;          // rotation (radians/sec) in local vehicle coordinates
  rfVec3 localRotAccel;     // rotational acceleration (radians/sec^2) in local vehicle coordinates

  float speed;				// meters/sec

  // Vehicle status
  long gear;                    // -1=reverse, 0=neutral, 1+=forward gears
  float engineRPM;              // engine RPM
  float engineWaterTemp;        // Celsius
  float engineOilTemp;          // Celsius
  float clutchRPM;              // clutch RPM

  // Driver input
  float unfilteredThrottle;     // ranges  0.0-1.0
  float unfilteredBrake;        // ranges  0.0-1.0
  float unfilteredSteering;     // ranges -1.0-1.0 (left to right)
  float unfilteredClutch;       // ranges  0.0-1.0

  // Misc
  float steeringArmForce;       // force on steering arms
  // state/damage info
  float fuel;                   // amount of fuel (liters)
  float engineMaxRPM;           // rev limit
  unsigned char scheduledStops; // number of scheduled pitstops
  bool  overheating;            // whether overheating icon is shown
  bool  detached;               // whether any parts (besides wheels) have been detached
  unsigned char dentSeverity[8];// dent severity at 8 locations around the car (0=none, 1=some, 2=more)
  float lastImpactET;           // time of last impact
  float lastImpactMagnitude;    // magnitude of last impact
  rfVec3 lastImpactPos;     // location of last impact

  rfWheel wheel[4];        // wheel info (front left, front right, rear left, rear right)

  // scoring info only updates twice per second (values interpolated when deltaTime > 0)!
  long session;                 // current session
  float currentET;              // current time
  float endET;                  // ending time
  long  maxLaps;                // maximum laps
  float lapDist;                // distance around track

  long numVehicles;             // current number of vehicles

  unsigned char gamePhase;   

  signed char yellowFlagState;

  signed char sectorFlag[3];      // whether there are any local yellows at the moment in each sector (not sure if sector 0 is first or last, so test)
  unsigned char startLight;       // start light frame (number depends on track)
  unsigned char numRedLights;     // number of red lights in start sequence
  bool inRealtime;                // in realtime as opposed to at the monitor
  char playerName[32];            // player name (including possible multiplayer override)

  // weather
  float ambientTemp;              // temperature (Celsius)
  float trackTemp;                // temperature (Celsius)
  rfVec3 wind;                // wind speed

  rfVehicleInfo vehicle[RF_SHARED_MEMORY_MAX_VSI_SIZE];  // array of vehicle scoring info's
};

#pragma pack(pop)
