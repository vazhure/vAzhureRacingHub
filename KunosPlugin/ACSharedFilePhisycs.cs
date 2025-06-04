/// Author:
/// Zhuravlev Andrey
/// E-mail: v.azhure@gmail.com
///
/// This file is subject to the terms and conditions defined in
/// file 'LICENSE.txt', which is part of this source code package.
///

using System;
using System.Runtime.InteropServices;

namespace Kunos.Structs
{
    public class Constants
    {
        /// <summary>
        /// Assetto Corsa shared memory file name
        /// </summary>
        public const string ACsharedMemoryPhysicsFile = @"Local\acpmf_physics";
        public const string ACSharedStaticFile = @"Local\acpmf_static";
        public const string ACSharedGraphicsFile = @"Local\acpmf_graphics";
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Ansi)]
    public struct Coordinates
    {
        public float X;
        public float Y;
        public float Z;
    }

    public enum TRACK_GRIP_STATUS
    {
        GREEN = 0,
        FAST = 1,
        OPTIMUM = 2,
        GREASY = 3,
        DAMP = 4,
        WET = 5,
        FLOODED = 6
    }

    public enum RAIN_INTENSITY
    {
        NO_RAIN = 0,
        DRIZZLE = 1,
        LIGHT_RAIN = 2,
        MEDIUM_RAIN = 3,
        HEAVY_RAIN = 4,
        THUNDERSTORM = 5
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Unicode)]
    public struct SPageFilePhysics
    {
        public int packetId;
        public float gas;
        public float brake;
        public float fuel;
        public int gear;
        public int rpms;
        public float steerAngle;
        public float speedKmh;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public float [] velocity;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public float [] accG;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float [] wheelSlip;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float [] wheelLoad;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float [] wheelsPressure;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float [] wheelAngularSpeed;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float [] tyreWear;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float [] tyreDirtyLevel;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float [] tyreCoreTemperature;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float [] camberRAD;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float [] suspensionTravel;
        
        public float drs;
        public float tc;
        /// <summary>
        /// Car yaw orientation 
        /// </summary>
        public float heading;
        /// <summary>
        /// Car pitch orientation
        /// </summary>
        public float pitch;
        /// <summary>
        /// Car roll orientation
        /// </summary>
        public float roll;
        public float cgHeight;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        public float [] carDamage;
        public int numberOfTyresOut;
        public int pitLimiterOn;
        public float abs;
        /// <summary>
        /// Not used in ACC 
        /// </summary>
        public float kersCharge;
        /// <summary>
        /// Not used in ACC 
        /// </summary>
        public float kersInput;
        public int autoShifterOn;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public float [] rideHeight;
        public float turboBoost;
        public float ballast;
        public float airDensity;
        public float airTemp;
        public float roadTemp;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public float [] localAngularVel;
        public float finalFF;
        /// <summary>
        /// Not used in ACC 
        /// </summary>
        public float performanceMeter;
        /// <summary>
        /// Not used in ACC 
        /// </summary>
        public int engineBrake;
        /// <summary>
        /// Not used in ACC 
        /// </summary>
        public int ersRecoveryLevel;
        /// <summary>
        /// Not used in ACC 
        /// </summary>
        public int ersPowerLevel;
        /// <summary>
        /// Not used in ACC 
        /// </summary>
        public int ersHeatCharging;
        /// <summary>
        /// Not used in ACC 
        /// </summary>
        public int ersIsCharging;
        /// <summary>
        /// Not used in ACC 
        /// </summary>
        public float kersCurrentKJ;
        /// <summary>
        /// Not used in ACC 
        /// </summary>
        public int drsAvailable;
        /// <summary>
        /// Not used in ACC 
        /// </summary>
        public int drsEnabled;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float [] brakeTemp;
        public float clutch;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float [] tyreTempI;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float [] tyreTempM;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float [] tyreTempO;

        public int isAIControlled;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
        public float[,] tyreContactPoint;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
        public float[,] tyreContactNormal;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
        public float[,] tyreContactHeading;

        public float brakeBias;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public float [] localVelocity;

        public int P2PActivations;
        public int P2PStatus;
        public int currentMaxRpm;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float [] mz;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float [] fx;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float [] fy;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float [] slipRatio;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float [] slipAngle;


        public int tcinAction;
        public int absInAction;
        /// <summary>
        /// Suspensions damage levels [FL, FR, RL, RR]
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float [] suspensionDamage;
        /// <summary>
        /// Tires core temperatures [FL, FR, RL, RR]
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float [] tyreTemp;

        public float waterTemp;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] brakePressure;
        public int frontBrakeCompound;
        public int rearBrakeCompound;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] padLife;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] discLife;
        /// <summary>
        /// Ignition switch set to on?
        /// </summary>
        public int ignitionOn;
        /// <summary>
        /// Starter Switch set to on?
        /// </summary>
        public int starterEngineOn;
        /// <summary>
        /// Engine running?
        /// </summary>
        public int isEngineRunning;
        public float kerbVibrations;
        public float slipVibrations;
        public float gVibrations;
        public float absVibrations;

        public static SPageFilePhysics FromBytes(byte[] bytes)
        {
            try
            {
                GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
                SPageFilePhysics stuff = (SPageFilePhysics)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(SPageFilePhysics));
                handle.Free();
                return stuff;
            }
            catch { return new SPageFilePhysics(); }
        }
    }

    public enum AC_STATUS
    {
        AC_OFF = 0,
        AC_REPLAY = 1,
        AC_LIVE = 2,
        AC_PAUSE = 3
    }

    public enum AC_SESSION_TYPE
    {
        AC_UNKNOWN = -1,
        AC_PRACTICE = 0,
        AC_QUALIFY = 1,
        AC_RACE = 2,
        AC_HOTLAP = 3,
        AC_TIME_ATTACK = 4,
        AC_DRIFT = 5,
        AC_DRAG = 6,
    }

    public enum AC_WHEELS
    {
        FL = 0,
        FR = 1,
        RL = 2,
        RR = 3,
    }

    public enum AC_FLAG_TYPE
    {
        AC_NO_FLAG = 0,
        AC_BLUE_FLAG = 1,
        AC_YELLOW_FLAG = 2,
        AC_BLACK_FLAG = 3,
        AC_WHITE_FLAG = 4,
        AC_CHECKERED_FLAG = 5,
        AC_PENALTY_FLAG = 6,
        AC_GREEN_FLAG = 7,
        AC_ORANGE_FLAG = 8
    }

    public enum AC_PENALTY
    {
        ACC_NONE = 0,
        ACC_DRIVETHROUGH_CUTTING = 1,
        ACC_STOPANDGO_10_CUTTING = 2,
        ACC_STOPANDGO_20_CUTTING = 3,
        ACC_STOPANDGO_30_CUTTING = 4,
        ACC_DISQUALIFIED_CUTTING = 5,
        ACC_REMOVEBESTLAPTIME_CUTTING = 6,
        ACC_DRIVETHROUGH_PITSPEEDING = 7,
        ACC_STOPANDGO_10_PITSPEEDING = 8,
        ACC_STOPANDGO_20_PITSPEEDING = 9,
        ACC_STOPANDGO_30_PITSPEEDING = 10,
        ACC_DISQUALIFIED_PITSPEEDING = 11,
        ACC_REMOVEBESTLAPTIME_PITSPEEDING = 12,
        ACC_DISQUALIFIED_IGNOREDMANDATORYPIT = 13,
        ACC_POSTRACETIME = 14,
        ACC_DISQUALIFIED_TROLLING = 15,
        ACC_DISQUALIFIED_PITENTRY = 16,
        ACC_DISQUALIFIED_PITEXIT = 17,
        ACC_DISQUALIFIED_WRONGWAY = 18,
        ACC_DRIVETHROUGH_IGNOREDDRIVERSTINT = 19,
        ACC_DISQUALIFIED_IGNOREDDRIVERSTINT = 20,
        ACC_DISQUALIFIED_EXCEEDEDDRIVERSTINTLIMIT = 21
    }

    public enum ACC_STATUS
    {
        ACC_OFF = 0,
        ACC_REPLAY = 1,
        ACC_LIVE = 2,
        ACC_PAUSE = 3
    }

    public enum ACC_SESSION_TYPE
    {
        ACC_UNKNOWN = -1,
        ACC_PRACTICE = 0,
        ACC_QUALIFY = 1,
        ACC_RACE = 2,
        ACC_HOTLAP = 3,
        ACC_TIME_ATTACK = 4,
        ACC_DRIFT = 5,
        ACC_DRAG = 6,
        ACC_HOTSTINT = 7,
        ACC_HOTSTINTSUPERPOLE = 8
    }

    public enum ACC_WHEELS_TYPE
    {
        ACC_FRONTLEFT = 0,
        ACC_FRONTRIGHT = 1,
        ACC_REARLEFT = 2,
        ACC_REARRIGHT = 3,
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Unicode)]
    public struct SPageFileStatic
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 15)]
        public String SMVersion;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 15)]
        public String ACVersion;

        // session static info
        public int NumberOfSessions;
        public int NumCars;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 33)]
        public String CarModel;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 33)]
        public String Track;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 33)]
        public String PlayerName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 33)]
        public String PlayerSurname;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 33)]
        public String PlayerNick;

        public int SectorCount;

        // car static info
        public float MaxTorque;
        public float MaxPower;
        public int MaxRpm;
        public float MaxFuel;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] SuspensionMaxTravel;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] TyreRadius;

        // since 1.5
        public float MaxTurboBoost;
        public float Deprecated1; // AirTemp since 1.6 in physic
        public float Deprecated2; // RoadTemp since 1.6 in physic
        public int PenaltiesEnabled;
        public float AidFuelRate;
        public float AidTireRate;
        public float AidMechanicalDamage;
        public int AidAllowTyreBlankets;
        public float AidStability;
        public int AidAutoClutch;
        public int AidAutoBlip;

        // since 1.7.1
        /// <summary>
        /// Not used in ACC 
        /// </summary>
        public int HasDRS;
        /// <summary>
        /// Not used in ACC 
        /// </summary>
        public int HasERS;
        /// <summary>
        /// Not used in ACC 
        /// </summary>
        public int HasKERS;
        /// <summary>
        /// Not used in ACC 
        /// </summary>
        public float KersMaxJoules;
        /// <summary>
        /// Not used in ACC 
        /// </summary>
        public int EngineBrakeSettingsCount;
        /// <summary>
        /// Not used in ACC 
        /// </summary>
        public int ErsPowerControllerCount;

        // since 1.7.2
        /// <summary>
        /// Not used in ACC 
        /// </summary>
        public float TrackSPlineLength;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 33)]
        /// <summary>
        /// Not used in ACC 
        /// </summary>
        public string TrackConfiguration;

        // since 1.10.2
        public float ErsMaxJ;

        // since 1.13
        /// <summary>
        /// Not used in ACC
        /// </summary>
        public int IsTimedRace;
        /// <summary>
        /// Not used in ACC
        /// </summary>
        public int HasExtraLap;
        /// <summary>
        /// Not used in ACC
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 33)]
        public String CarSkin;
        /// <summary>
        /// Not used in ACC
        /// </summary>
        public int ReversedGridPositions;
        public int PitWindowStart;
        public int PitWindowEnd;
        public int IsOnline;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 33)]
        public String dryTyresName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 33)]
        public String wetTyresName;
    };

    [StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Unicode)]
    public struct SPageFileGraphics
    {
        public int packetId;
        public AC_STATUS status;
        public AC_SESSION_TYPE session;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 15)]
        public String currentTime;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 15)]
        public String lastTime;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 15)]
        public String bestTime;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 15)]
        public String split;
        /// <summary>
        /// No of completed laps 
        /// </summary>
        public int completedLaps;
        /// <summary>
        /// Current player position
        /// </summary>
        public int position;
        /// <summary>
        /// Current lap time in milliseconds
        /// </summary>
        public int iCurrentTime;
        public int iLastTime;
        public int iBestTime;
        /// <summary>
        /// Session time left 
        /// </summary>
        public float sessionTimeLeft;
        public float distanceTraveled;
        public int isInPit;
        public int currentSectorIndex;
        public int lastSectorTime;
        public int numberOfLaps;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 33)]
        public String tyreCompound;
        /// <summary>
        /// Not used in ACC
        /// </summary>
        public float replayTimeMultiplier;
        /// <summary>
        /// Car position on track spline (0.0 start to 1.0 finish)
        /// </summary>
        public float normalizedCarPosition;
        /// <summary>
        /// Number of cars on track
        /// </summary>
        public int activeCars;
        /// <summary>
        /// Coordinates of cars on track
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 180)]
        public float [,]carCoordinates;
        /// <summary>
        /// Car IDs of cars on track 
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 60)]
        public int [] carID;
        /// <summary>
        /// Номер на стартовой позиции в ACC
        /// </summary>
        public int playerCarID;
        public float penaltyTime;
        public AC_FLAG_TYPE flag;
        public AC_PENALTY penalty;
        public int idealLineOn;

        // since 1.5
        public int isInPitLane;
        public float surfaceGrip;

        // since 1.13
        public int mandatoryPitDone;

        /// <summary>
        /// Wind speed in m/s 
        /// </summary>
        public float windSpeed;
        /// <summary>
        /// wind direction in radians 
        /// </summary>
        public float windDirection;

        public int isSetupMenuVisible;

        public int mainDisplayIndex;
        public int secondaryDisplayIndex;
        public int TC;
        public int TCCut;
        public int EngineMap;
        public int ABS;
        /// <summary>
        /// Average fuel consumed per lap in liters
        /// </summary>
        public float fuelXLap;
        public int rainLights;
        public int flashingLights;
        public int lightsStage;
        public float exhaustTemperature;
        public int wiperLV;
        public int driverStintTotalTimeLeft;
        public int driverStintTimeLeft;
        public int rainTyres;
        public int sessionIndex;
        public float usedFuel; //Since last refuel

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 15)]
        public String deltaLapTime;
        public int iDeltaLapTime;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 15)]
        public String estimatedLapTime;
        public int iEstimatedLapTime;

        public int isDeltaPositive;
        /// <summary>
        /// Last split time in milliseconds
        /// </summary>
        public int iSplit;
        /// <summary>
        /// Check if Lap is valid for timing
        /// </summary>
        public int isValidLap;
        public float fuelEstimatedLaps;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 33)]
        public String trackStatus;
        public int missingMandatoryPits;

        /// <summary>
        /// Time of day in seconds
        /// </summary>
        public float Clock;
        public int directionLightsLeft;
        public int directionLightsRight;
        /// <summary>
        /// Yellow Flag is out?
        /// </summary>
        public int GlobalYellow;
        public int GlobalYellow1;
        public int GlobalYellow2;
        public int GlobalYellow3;
        public int GlobalWhite;
        public int GlobalGreen;
        public int GlobalChequered;
        public int GlobalRed;
        public int mfdTyreSet;
        public float mfdFuelToAdd;
        public float mfdTyrePressureLF;
        public float mfdTyrePressureRF;
        public float mfdTyrePressureLR;
        public float mfdTyrePressureRR;
        public TRACK_GRIP_STATUS trackGripStatus;
        public RAIN_INTENSITY rainIntensity;
        public RAIN_INTENSITY rainIntensityIn10min;
        public RAIN_INTENSITY rainIntensityIn30min;
        public int currentTyreSet;
        public int strategyTyreSet;
        /// <summary>
        /// Distance in ms to car in front
        /// </summary>
        public int gapAhead;
        /// <summary>
        /// Distance in ms to car behind
        /// </summary>
        public int gapBehind;
    }

    #region UDP Data
    // Data to send for initial handshake, update mode selection, and dismissal.
    [StructLayout(LayoutKind.Sequential, Pack = 0)]
    public struct Handshaker
    {
        public Handshaker(HandshakeOperation operationId, uint identifier = 1, uint version = 1)
        {
            this.identifier = identifier;
            this.version = version;
            this.operationId = operationId;
        }
        [MarshalAs(UnmanagedType.U4)]
        public uint identifier; // Android, iOS, currently not used.
        [MarshalAs(UnmanagedType.U4)]
        public uint version; // Expected AC remote telemetry interface version.
        [MarshalAs(UnmanagedType.U4)]
        public HandshakeOperation operationId; // Type of handshake packet.
        public enum HandshakeOperation
        {
            Connect,
            CarInfo,
            Lapinfo,
            Disconnect
        };
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
    public struct HandshakerResponse
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 50)]
        public string carName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 50)]
        public string driverName;
        [MarshalAs(UnmanagedType.U4)]
        public uint identifier; // Status code from the server, currently just '4242' to see that it works.
        [MarshalAs(UnmanagedType.U4)]
        public uint version; // Server version, not yet supported.
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 50)]
        public string trackName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 50)]
        public string trackConfig;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
    public struct RTLap
    {
        [MarshalAs(UnmanagedType.U4)]
        public int carIdentifierNumber;
        [MarshalAs(UnmanagedType.U4)]
        public int lap;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 50)]
        public string driverName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 50)]
        public string carName;
        [MarshalAs(UnmanagedType.U4)]
        public int time;
    };

    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Unicode, Pack = 1, Size = 328)]
    public struct RTCarInfo
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 2), FieldOffset(0 * 4)]
        public string identifier;
        [MarshalAs(UnmanagedType.U4), FieldOffset(1 * 4)]
        public int size;
        [MarshalAs(UnmanagedType.R4), FieldOffset(2 * 4)]
        public float speed_Kmh;
        [MarshalAs(UnmanagedType.R4), FieldOffset(3 * 4)]
        public float speed_Mph;
        [MarshalAs(UnmanagedType.R4), FieldOffset(4 * 4)]
        public float speed_Ms;
        [MarshalAs(UnmanagedType.U1), FieldOffset(5 * 4)]
        public bool isAbsEnabled;
        [MarshalAs(UnmanagedType.U1), FieldOffset(5 * 4 + 1)]
        public bool isAbsInAction;
        [MarshalAs(UnmanagedType.U1), FieldOffset(5 * 4 + 2)]
        public bool isTcInAction;
        [MarshalAs(UnmanagedType.U1), FieldOffset(5 * 4 + 3)]
        public bool isTcEnabled;
        [MarshalAs(UnmanagedType.U1), FieldOffset(6 * 4 + 2)]
        public bool isInPit;
        [MarshalAs(UnmanagedType.U1), FieldOffset(6 * 4 + 3)]
        public bool isEngineLimiterOn;
        [MarshalAs(UnmanagedType.R4), FieldOffset(7 * 4)]
        public float accG_vertical;
        [MarshalAs(UnmanagedType.R4), FieldOffset(8 * 4)]
        public float accG_horizontal;
        [MarshalAs(UnmanagedType.R4), FieldOffset(9 * 4)]
        public float accG_frontal;
        [MarshalAs(UnmanagedType.U4), FieldOffset(10 * 4)]
        public int lapTime;
        [MarshalAs(UnmanagedType.U4), FieldOffset(11 * 4)]
        public int lastLap;
        [MarshalAs(UnmanagedType.U4), FieldOffset(12 * 4)]
        public int bestLap;
        [MarshalAs(UnmanagedType.U4), FieldOffset(13 * 4)]
        public int lapCount;
        [MarshalAs(UnmanagedType.R4), FieldOffset(14 * 4)]
        public float gas;
        [MarshalAs(UnmanagedType.R4), FieldOffset(15 * 4)]
        public float brake;
        [MarshalAs(UnmanagedType.R4), FieldOffset(16 * 4)]
        public float clutch;
        [MarshalAs(UnmanagedType.R4), FieldOffset(17 * 4)]
        public float engineRPM;
        [MarshalAs(UnmanagedType.R4), FieldOffset(18 * 4)]
        public float steer;
        [MarshalAs(UnmanagedType.U4), FieldOffset(19 * 4)]
        public int gear;
        [MarshalAs(UnmanagedType.R4), FieldOffset(20 * 4)]
        public float cgHeight;
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.R4, SizeConst = 4), FieldOffset(21 * 4)]
        public float[] wheelAngularSpeed;
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.R4, SizeConst = 4), FieldOffset(25 * 4)]
        public float[] slipAngle;
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.R4, SizeConst = 4), FieldOffset(29 * 4)]
        public float[] slipAngle_ContactPatch;
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.R4, SizeConst = 4), FieldOffset(33 * 4)]
        public float[] slipRatio;
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.R4, SizeConst = 4), FieldOffset(37 * 4)]
        public float[] tyreSlip;
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.R4, SizeConst = 4), FieldOffset(41 * 4)]
        public float[] ndSlip;
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.R4, SizeConst = 4), FieldOffset(45 * 4)]
        public float[] load;
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.R4, SizeConst = 4), FieldOffset(49 * 4)]
        public float[] Dy;
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.R4, SizeConst = 4), FieldOffset(53 * 4)]
        public float[] Mz;
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.R4, SizeConst = 4), FieldOffset(57 * 4)]
        public float[] tyreDirtyLevel;
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.R4, SizeConst = 4), FieldOffset(61 * 4)]
        public float[] camberRAD;
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.R4, SizeConst = 4), FieldOffset(65 * 4)]
        public float[] tyreRadius;
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.R4, SizeConst = 4), FieldOffset(69 * 4)]
        public float[] tyreLoadedRadius;
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.R4, SizeConst = 4), FieldOffset(73 * 4)]
        public float[] suspensionHeight;
        [MarshalAs(UnmanagedType.R4), FieldOffset(77 * 4)]
        public float carPositionNormalized;
        [MarshalAs(UnmanagedType.R4), FieldOffset(78 * 4)]
        public float carSlope;
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.R4, SizeConst = 3), FieldOffset(79 * 4)]
        public float[] carCoordinates;
    };
    #endregion
}
