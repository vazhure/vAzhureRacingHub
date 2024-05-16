using System.Runtime.InteropServices;

namespace rFactor2plugin
{
    class RFactor1
    {
        public static readonly string RF_SHARED_MEMORY_NAME = "$rFactorShared$";

        internal enum RfGamePhase : byte
        {
            garage = 0,
            warmUp = 1,
            gridWalk = 2,
            formation = 3,
            countdown = 4,
            greenFlag = 5,
            fullCourseYellow = 6,
            sessionStopped = 7,
            sessionOver = 8
        }

        internal enum RfYellowFlagState : sbyte
        {
            invalid = -1,
            noFlag = 0,
            pending = 1,
            pitClosed = 2,
            pitLeadLap = 3,
            pitOpen = 4,
            lastLap = 5,
            resume = 6,
            raceHalt = 7
        }

        internal enum RfSurfaceType : byte
        {
            dry = 0,
            wet = 1,
            grass = 2,
            dirt = 3,
            gravel = 4,
            kerb = 5
        }

        internal enum RfSector : sbyte
        {
            sector3 = 0,
            sector1 = 1,
            sector2 = 2
        }

        internal enum RfFinishStatus : sbyte
        {
            none = 0,
            finished = 1,
            dnf = 2,
            dq = 3
        }

        internal enum RfControl : sbyte
        {
            nobody = -1,
            player = 0,
            ai = 1,
            remote = 2,
            replay = 3
        }

        internal enum RfWheelIndex
        {
            frontLeft = 0,
            frontRight = 1,
            rearLeft = 2,
            rearRight = 3
        }

        // Our world coordinate system is left-handed, with +y pointing up.
        // The local vehicle coordinate system is as follows:
        //   +x points out the left side of the car (from the driver's perspective)
        //   +y points out the roof
        //   +z points out the back of the car
        // Rotations are as follows:
        //   +x pitches up
        //   +y yaws to the right
        //   +z rolls to the right

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct RfVec3
        {
            public float x, y, z;
            public static implicit operator double[](RfVec3 vec3)
            {
                return new double[] { vec3.x, vec3.y, vec3.y };
            }

            public static implicit operator float[](RfVec3 vec3)
            {
                return new float[] { (float)vec3.x, (float)vec3.y, (float)vec3.y };
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
        internal struct RfWheel
        {
            /// <summary>
            /// radians/sec
            /// </summary>
            public float rotation;
            /// <summary>
            /// meters
            /// </summary>
            public float suspensionDeflection;
            /// <summary>
            /// meters
            /// </summary>
            public float rideHeight;
            /// <summary>
            /// Newtons
            /// </summary>
            public float tireLoad;
            /// <summary>
            /// Newtons
            /// </summary>
            public float lateralForce;
            /// <summary>
            /// an approximation of what fraction of the contact patch is sliding
            /// </summary>
            public float gripFract;
            /// <summary>
            /// Celsius
            /// </summary>
            public float brakeTemp;
            /// <summary>
            /// kPa
            /// </summary>
            public float pressure;
            /// <summary>
            /// Celsius, left/center/right (not to be confused with inside/center/outside!)
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public float[] temperature;
            /// <summary>
            /// wear (0.0-1.0, fraction of maximum) ... this is not necessarily proportional with grip loss
            /// </summary>
            public float wear;
            /// <summary>
            /// the material prefixes from the TDF file
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string terrainName;
            /// <summary>
            /// 0=dry, 1=wet, 2=grass, 3=dirt, 4=gravel, 5=rumblestrip
            /// </summary>
            public RfSurfaceType surfaceType;
            /// <summary>
            /// whether tire is flat
            /// </summary>
            [MarshalAs(UnmanagedType.U1)]
            public bool flat;
            /// <summary>
            /// whether wheel is detached
            /// </summary>
            [MarshalAs(UnmanagedType.U1)]
            public bool detached;
        };

        // scoring info only updates twice per second (values interpolated when deltaTime > 0)!
        [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
        internal struct RfVehicleInfo
        {
            /// <summary>
            /// driver name
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string driverName;
            /// <summary>
            /// laps completed
            /// </summary>
            public short totalLaps;
            /// <summary>
            /// 0=sector3, 1=sector1, 2=sector2 (don't ask why)
            /// </summary>
            public sbyte sector;
            /// <summary>
            /// 0=none, 1=finished, 2=DNF, 3=DQ
            /// </summary>
            public RfFinishStatus finishStatus;
            /// <summary>
            /// current distance around track
            /// </summary>
            public float lapDist;
            /// <summary>
            /// lateral position with respect to *very approximate* "center" path
            /// </summary>
            public float pathLateral;
            /// <summary>
            /// track edge (w.r.t. "center" path) on same side of track as vehicle
            /// </summary>
            public float trackEdge;
            /// <summary>
            /// best sector 1
            /// </summary>
            public float bestSector1;
            /// <summary>
            /// best sector 2 (plus sector 1)
            /// </summary>
            public float bestSector2;
            /// <summary>
            /// best lap time
            /// </summary>
            public float bestLapTime;
            /// <summary>
            /// last sector 1
            /// </summary>
            public float lastSector1;
            /// <summary>
            /// last sector 2 (plus sector 1)
            /// </summary>
            public float lastSector2;
            /// <summary>
            /// last lap time
            /// </summary>
            public float lastLapTime;
            /// <summary>
            /// current sector 1 if valid
            /// </summary>
            public float curSector1;
            /// <summary>
            /// current sector 2 (plus sector 1) if valid
            /// no current lap time because it instantly becomes "last"
            /// </summary>
            public float curSector2;

            /// <summary>
            /// number of pitstops made
            /// </summary>
            public short numPitstops;
            /// <summary>
            /// number of outstanding penalties
            /// </summary>
            public short numPenalties;
            /// <summary>
            /// is this the player's vehicle
            /// </summary>
            [MarshalAs(UnmanagedType.U1)]
            public bool isPlayer;
            /// <summary>
            /// who's in control: -1=nobody (shouldn't get this), 0=local player, 1=local AI, 2=remote, 3=replay (shouldn't get this)
            /// </summary>
            public RfControl control;
            /// <summary>
            /// between pit entrance and pit exit (not always accurate for remote vehicles)
            /// </summary>
            [MarshalAs(UnmanagedType.U1)]
            public bool inPits;
            /// <summary>
            /// 1-based position
            /// </summary>
            public byte place;
            /// <summary>
            /// vehicle class
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string vehicleClass;

            // Dash Indicators
            /// <summary>
            /// time behind vehicle in next higher place
            /// </summary>
            public float timeBehindNext;
            /// <summary>
            /// laps behind vehicle in next higher place
            /// </summary>
            public int lapsBehindNext;
            /// <summary>
            /// time behind leader
            /// </summary>
            public float timeBehindLeader;
            /// <summary>
            /// laps behind leader
            /// </summary>
            public int lapsBehindLeader;
            /// <summary>
            /// time this lap was started
            /// </summary>
            public float lapStartET;

            // Position and derivatives
            /// <summary>
            /// world position in meters
            /// </summary>
            public RfVec3 pos;
            /// <summary>
            /// rad, use (360-yaw*57.2978)%360 for heading in degrees
            /// </summary>
            public float yaw;
            /// <summary>
            /// rad
            /// </summary>
            public float pitch;
            /// <summary>
            /// rad
            /// </summary>
            public float roll;
            /// <summary>
            /// meters/sec
            /// </summary>
            public float speed;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
        internal struct RfShared
        {
            /// <summary>
            /// API version
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
            public string version;

            /// <summary>
            /// time since last scoring update (seconds)
            /// </summary>
            public float deltaTime;
            /// <summary>
            /// current lap number
            /// </summary>
            public int lapNumber;
            /// <summary>
            /// time this lap was started
            /// </summary>
            public float lapStartET;
            /// <summary>
            /// current track name
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string trackName;

            // Position and derivatives
            /// <summary>
            /// world position in meters
            /// </summary>
            public RfVec3 pos;
            /// <summary>
            /// velocity (meters/sec) in local vehicle coordinates
            /// </summary>
            public RfVec3 localVel;
            /// <summary>
            /// acceleration (meters/sec^2) in local vehicle coordinates
            /// </summary>
            public RfVec3 localAccel;

            // Orientation and derivatives
            /// <summary>
            /// top row of orientation matrix (also converts local vehicle vectors into world X using dot product)
            /// </summary>
            public RfVec3 oriX;
            /// <summary>
            /// mid row of orientation matrix (also converts local vehicle vectors into world Y using dot product)
            /// </summary>
            public RfVec3 oriY;
            /// <summary>
            /// bot row of orientation matrix (also converts local vehicle vectors into world Z using dot product)
            /// </summary>
            public RfVec3 oriZ;
            /// <summary>
            /// rotation (radians/sec) in local vehicle coordinates
            /// </summary>
            public RfVec3 localRot;
            /// <summary>
            /// rotational acceleration (radians/sec^2) in local vehicle coordinates
            /// </summary>
            public RfVec3 localRotAccel;
            /// <summary>
            /// meters/sec
            /// </summary>
            public float speed;

            // Vehicle status
            /// <summary>
            /// -1=reverse, 0=neutral, 1+=forward gears
            /// </summary>
            public int gear;
            /// <summary>
            /// engine RPM
            /// </summary>
            public float engineRPM;
            /// <summary>
            /// Celsius
            /// </summary>
            public float engineWaterTemp;
            /// <summary>
            /// Celsius
            /// </summary>
            public float engineOilTemp;
            /// <summary>
            /// clutch RPM
            /// </summary>
            public float clutchRPM;

            // Driver input
            /// <summary>
            /// ranges  0.0-1.0
            /// </summary>
            public float unfilteredThrottle;
            /// <summary>
            /// ranges  0.0-1.0
            /// </summary>
            public float unfilteredBrake;
            /// <summary>
            /// ranges -1.0-1.0 (left to right)
            /// </summary>
            public float unfilteredSteering;
            /// <summary>
            /// ranges  0.0-1.0
            /// </summary>
            public float unfilteredClutch;

            // Misc
            /// <summary>
            /// force on steering arms
            /// state/damage info
            /// </summary>
            public float steeringArmForce;

            /// <summary>
            /// amount of fuel (liters)
            /// </summary>
            public float fuel;
            /// <summary>
            /// rev limit
            /// </summary>
            public float engineMaxRPM;
            /// <summary>
            /// number of scheduled pitstops
            /// </summary>
            public sbyte scheduledStops;
            /// <summary>
            /// whether overheating icon is shown
            /// </summary>
            [MarshalAs(UnmanagedType.U1)]
            public bool overheating;
            /// <summary>
            /// whether any parts (besides wheels) have been detached
            /// </summary>
            [MarshalAs(UnmanagedType.U1)]
            public bool detached;
            /// <summary>
            /// dent severity at 8 locations around the car (0=none, 1=some, 2=more)
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] dentSeverity;
            /// <summary>
            /// time of last impact
            /// </summary>
            public float lastImpactET;
            /// <summary>
            /// magnitude of last impact
            /// </summary>
            public float lastImpactMagnitude;
            /// <summary>
            /// location of last impact
            /// </summary>
            public RfVec3 lastImpactPos;

            /// <summary>
            /// wheel info (front left, front right, rear left, rear right)
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public RfWheel[] wheel;

            // scoring info only updates twice per second (values interpolated when deltaTime > 0)!
            /// <summary>
            /// current session
            /// </summary>
            public int session;
            /// <summary>
            /// current time
            /// </summary>
            public float currentET;
            /// <summary>
            /// ending time
            /// </summary>
            public float endET;
            /// <summary>
            /// maximum laps
            /// </summary>
            public int maxLaps;
            /// <summary>
            /// distance around track
            /// </summary>
            public float lapDist;
            /// <summary>
            /// current number of vehicles
            /// </summary>
            public int numVehicles;
            public RfGamePhase gamePhase;
            public RfYellowFlagState yellowFlagState;
            /// <summary>
            /// whether there are any local yellows at the moment in each sector (not sure if sector 0 is first or last, so test)
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public RfYellowFlagState[] sectorFlag;
            /// <summary>
            /// start light frame (number depends on track)
            /// </summary>
            public byte startLight;
            /// <summary>
            /// number of red lights in start sequence
            /// </summary>
            public byte numRedLights;
            /// <summary>
            /// in real time as opposed to at the monitor
            /// </summary>
            [MarshalAs(UnmanagedType.U1)]
            public bool inRealtime;
            /// <summary>
            /// player name (including possible multiplayer override)
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string playerName;

            // weather
            /// <summary>
            /// temperature (Celsius)
            /// </summary>
            public float ambientTemp;
            /// <summary>
            /// temperature (Celsius)
            /// </summary>
            public float trackTemp;
            /// <summary>
            /// wind speed
            /// </summary>
            public RfVec3 wind;

            /// <summary>
            /// array of vehicle scoring info's
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
            public RfVehicleInfo[] vehicle;
        };
    }
}