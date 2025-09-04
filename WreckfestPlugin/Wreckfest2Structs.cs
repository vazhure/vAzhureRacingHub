using System;
using System.Runtime.InteropServices;

namespace WreckfestPlugin
{
    public static class Wreckfest2Structs
    {
        public static class Const
        {
            public const int PARTICIPANTS_MAX = 0x24; // 36
            public const int TRACK_ID_LENGTH_MAX = 0x40; // 64
            public const int TRACK_NAME_LENGTH_MAX = 0x60; // 96
            public const int CAR_ID_LENGTH_MAX = 0x40; // 64
            public const int CAR_NAME_LENGTH_MAX = 0x60; // 96
            public const int PLAYER_NAME_LENGTH_MAX = 0x18; // 24
            public const int DAMAGE_PARTS_MAX = 0x38;
            public const int DAMAGE_BITS_PER_PART = 0x3;
            public const int DAMAGE_BYTES_PER_PARTICIPANT = 0x15;
            public const int TIRE_LOCATION_COUNT = 0x4;
            public const int AXLE_LOCATION_COUNT = 0x2;
            public const uint PINO_SIGNATURE = 0x6f726b70;
        }

        public enum PacketType : byte
        {
            PACKET_TYPE_MAIN = 0x0,
            PACKET_TYPE_PARTICIPANTS_LEADERBOARD = 0x1,
            PACKET_TYPE_PARTICIPANTS_TIMING = 0x2,
            PACKET_TYPE_PARTICIPANTS_TIMING_SECTORS = 0x3,
            PACKET_TYPE_PARTICIPANTS_MOTION = 0x4,
            PACKET_TYPE_PARTICIPANTS_INFO = 0x5,
            PACKET_TYPE_PARTICIPANTS_DAMAGE = 0x6
        }

        [Flags]
        public enum GameStatusFlags : byte
        {
            GAME_STATUS_PAUSED = 0x1,
            GAME_STATUS_REPLAY = 0x2,
            GAME_STATUS_SPECTATE = 0x4,
            GAME_STATUS_MULTIPLAYER_CLIENT = 0x8,
            GAME_STATUS_MULTIPLAYER_SERVER = 0x10
        }

        public enum GameMode : byte
        {
            GAME_MODE_BANGER = 0x0,
            GAME_MODE_DEATHMATCH = 0x1,
            GAME_MODE_LAST_MAN_STANDING = 0x2,
            GAME_MODE_OTHER = 0xff
        }

        public enum DamageMode : byte
        {
            DAMAGE_MODE_WRECKER = 0x0,
            DAMAGE_MODE_NORMAL = 0x1,
            DAMAGE_MODE_REALISTIC = 0x2,
            DAMAGE_MODE_OTHER = 0xff
        }

        [Flags]
        public enum MarshalFlags : ushort
        {
            MARSHAL_FLAGS_GREEN = 0x1,
            MARSHAL_FLAGS_LASTLAP = 0x2,
            MARSHAL_FLAGS_FINISH = 0x4,
            MARSHAL_FLAGS_DQ = 0x8,
            MARSHAL_FLAGS_MEATBALL = 0x10,
            MARSHAL_FLAGS_WARNING = 0x20,
            MARSHAL_FLAGS_BLUE = 0x40,
            MARSHAL_FLAGS_WHITE = 0x80,
            MARSHAL_FLAGS_COUNTDOWN1 = 0x4000,
            MARSHAL_FLAGS_COUNTDOWN2 = 0x8000
        }

        public enum SurfaceType : byte
        {
            SURFACE_TYPE_DEFAULT = 0x0,
            SURFACE_TYPE_NOCONTACT = 0x1,
            SURFACE_TYPE_TARMAC = 0x2,
            SURFACE_TYPE_CONCRETE = 0x3,
            SURFACE_TYPE_GRAVEL = 0x4,
            SURFACE_TYPE_DIRT = 0x5,
            SURFACE_TYPE_MUD = 0x6,
            SURFACE_TYPE_RUMBLE_LOFQ = 0x7,
            SURFACE_TYPE_RUMBLE_HIFQ = 0x8,
            SURFACE_TYPE_WATER = 0x9,
            SURFACE_TYPE_METAL = 0xa,
            SURFACE_TYPE_WOOD = 0xb,
            SURFACE_TYPE_SAND = 0xc,
            SURFACE_TYPE_ROCKS = 0xd,
            SURFACE_TYPE_FOLIAGE = 0xe,
            SURFACE_TYPE_SLOWDOWN = 0xf,
            SURFACE_TYPE_SNOW = 0x10
        }

        public enum DamagePart : byte
        {
            PART_ENGINE = 0x0,
            PART_GEARBOX = 0x1,
            PART_BRAKE_FL = 0x2,
            PART_BRAKE_FR = 0x3,
            PART_BRAKE_RL = 0x4,
            PART_BRAKE_RR = 0x5,
            PART_SUSPENSION_FL = 0x6,
            PART_SUSPENSION_FR = 0x7,
            PART_SUSPENSION_RL = 0x8,
            PART_SUSPENSION_RR = 0x9,
            PART_TIRE_FL = 0xa,
            PART_TIRE_FR = 0xb,
            PART_TIRE_RL = 0xc,
            PART_TIRE_RR = 0xd,
            PART_HEADGASKET = 0xe,
            PART_RADIATOR = 0xf,
            PART_PISTONS = 0x10,
            PART_TIREHUB_FL = 0x11,
            PART_TIREHUB_FR = 0x12,
            PART_TIREHUB_RL = 0x13,
            PART_TIREHUB_RR = 0x14,
            PART_OILPAN = 0x15,
            PART_COOLANT = 0x16,
            PART_OIL = 0x17,
            PART_ENDBEARINGS = 0x18,
            PART_HALFSHAFT_FL = 0x19,
            PART_HALFSHAFT_FR = 0x1a,
            PART_HALFSHAFT_RL = 0x1b,
            PART_HALFSHAFT_RR = 0x1c,
            PART_RADIATORLEAK = 0x1d,
            PART_ARMOR_FL = 0x1e,
            PART_ARMOR_FR = 0x1f,
            PART_ARMOR_RL = 0x20,
            PART_ARMOR_RR = 0x21,
            PART_ARMOR_SL = 0x22,
            PART_ARMOR_SR = 0x23,
            PART_MISFIRE = 0x24,
            PART_COUNT = 0x25
        }

        public enum DamageState : byte
        {
            STATE_OK = 0x0,
            STATE_DAMAGED1 = 0x1,
            STATE_DAMAGED2 = 0x2,
            STATE_DAMAGED3 = 0x3,
            STATE_TERMINAL = 0x4
        }

        [Flags]
        public enum AssistFlags : byte
        {
            ASSIST_FLAGS_ABS_ACTIVE = 0x1,
            ASSIST_FLAGS_TCS_ACTIVE = 0x2,
            ASSIST_FLAGS_ESC_ACTIVE = 0x4
        }

        public enum AssistGearbox : byte
        {
            ASSIST_GEARBOX_AUTO = 0x0,
            ASSIST_GEARBOX_MANUAL = 0x1,
            ASSIST_GEARBOX_MANUAL_WITH_CLUTCH = 0x2
        }

        public enum AssistLevel : byte
        {
            ASSIST_LEVEL_OFF = 0x0,
            ASSIST_LEVEL_HALF = 0x1,
            ASSIST_LEVEL_FULL = 0x2
        }

        public enum AxleLocation : byte
        {
            AXLE_LOCATION_FRONT = 0x0,
            AXLE_LOCATION_REAR = 0x1,
            AXLE_LOCATION_COUNT = 0x2
        }

        public enum DrivelineType : byte
        {
            DRIVELINE_TYPE_FWD = 0x0,
            DRIVELINE_TYPE_RWD = 0x1,
            DRIVELINE_TYPE_AWD = 0x2
        }

        [Flags]
        public enum EngineFlags : byte
        {
            ENGINE_FLAGS_RUNNING = 0x1,
            ENGINE_FLAGS_STARTING = 0x2,
            ENGINE_FLAGS_MISFIRING = 0x4,
            ENGINE_FLAGS_DANGER_TO_MANIFOLD = 0x80
        }

        public enum TireLocation : byte
        {
            TIRE_LOCATION_FL = 0x0,
            TIRE_LOCATION_FR = 0x1,
            TIRE_LOCATION_RL = 0x2,
            TIRE_LOCATION_RR = 0x3,
            TIRE_LOCATION_COUNT = 0x4
        }

        public enum ParticipantVisibility : byte
        {
            VISIBILITY_FULL = 0x0,
            VISIBILITY_LIMITED = 0x1
        }

        public enum ParticipantStatus : byte
        {
            STATUS_INVALID = 0x0,
            STATUS_UNUSED = 0x1,
            STATUS_RACING = 0x2,
            STATUS_FINISH_SUCCESS = 0x3,
            STATUS_FINISH_ELIMINATED = 0x4,
            STATUS_DNF_DQ = 0x5,
            STATUS_DNF_RETIRED = 0x6,
            STATUS_DNF_TIMEOUT = 0x7,
            STATUS_DNF_WRECKED = 0x8
        }

        public enum ParticipantTrackStatus : byte
        {
            TRACK_STATUS_NORMAL = 0x0,
            TRACK_STATUS_OUT_OF_SECTORS = 0x1,
            TRACK_STATUS_IN_WRONG_SECTORS = 0x2,
            TRACK_STATUS_WRONG_DIRECTION = 0x3
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
        public struct Session
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Const.TRACK_ID_LENGTH_MAX)]
            public string trackId;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Const.TRACK_NAME_LENGTH_MAX)]
            public string trackName;

            public float trackLength;
            public short laps;
            public short eventLength;
            public byte gridSize;
            public byte gridSizeRemaining;
            public byte sectorCount;
            public float sectorFract1;
            public float sectorFract2;
            public GameMode gameMode;
            public DamageMode damageMode;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 27)]
            public byte[] reserved;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct CarMotionOrientation
        {
            public float positionX;
            public float positionY;
            public float positionZ;
            public float orientationQuaternionX;
            public float orientationQuaternionY;
            public float orientationQuaternionZ;
            public float orientationQuaternionW;
            public ushort extentsX;
            public ushort extentsY;
            public ushort extentsZ;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct CarMotionVelocity
        {
            public float velocityLocalX;
            public float velocityLocalY;
            public float velocityLocalZ;
            public float angularVelocityX;
            public float angularVelocityY;
            public float angularVelocityZ;
            public float accelerationLocalX;
            public float accelerationLocalY;
            public float accelerationLocalZ;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct CarMotionVelocityEssential
        {
            public float velocityMagnitude;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct CarAssists
        {
            public AssistFlags flags;
            public AssistGearbox assistGearbox;
            public AssistLevel levelAbs;
            public AssistLevel levelTcs;
            public AssistLevel levelEsc;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public byte[] reserved;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct CarChassis
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = Const.AXLE_LOCATION_COUNT)]
            public float[] trackWidth;

            public float wheelBase;
            public int steeringWheelLockToLock;
            public float steeringLock;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = Const.TIRE_LOCATION_COUNT)]
            public float[] cornerWeights;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public byte[] reserved;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct CarDriveline
        {
            public DrivelineType type;
            public byte gear;
            public byte gearMax;
            public float speed;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 17)]
            public byte[] reserved;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct CarEngine
        {
            public EngineFlags flags;
            public int rpm;
            public int rpmMax;
            public int rpmRedline;
            public int rpmIdle;
            public float torque;
            public float power;
            public float tempBlock;
            public float tempWater;
            public float pressureManifold;
            public float pressureOil;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 15)]
            public byte[] reserved;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct CarInput
        {
            public float throttle;
            public float brake;
            public float clutch;
            public float handbrake;
            public float steering;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct CarTire
        {
            public float rps;
            public float camber;
            public float slipRatio;
            public float slipAngle;
            public float radiusUnloaded;
            public float loadVertical;
            public float forceLat;
            public float forceLong;
            public float temperatureInner;
            public float temperatureTread;
            public float suspensionVelocity;
            public float suspensionDisplacement;
            public float suspensionDispNorm;
            public float positionVertical;
            public SurfaceType surfaceType;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 15)]
            public byte[] reserved;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct CarFull
        {
            public CarAssists assists;
            public CarChassis chassis;
            public CarDriveline driveline;
            public CarEngine engine;
            public CarInput input;
            public CarMotionOrientation orientation;
            public CarMotionVelocity velocity;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = Const.TIRE_LOCATION_COUNT)]
            public CarTire[] tires;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 14)]
            public byte[] reserved;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ParticipantLeaderboard
        {
            public ParticipantStatus status;
            public ParticipantTrackStatus trackStatus;
            public ushort lapCurrent;
            public byte position;
            public byte health;
            public ushort wrecks;
            public ushort frags;
            public ushort assists;
            public int score;
            public int points;
            public int deltaLeader;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] reserved;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ParticipantTiming
        {
            public uint lapTimeCurrent;
            public uint lapTimePenaltyCurrent;
            public uint lapTimeLast;
            public uint lapTimeBest;
            public byte lapBest;
            public int deltaAhead;
            public int deltaBehind;
            public float lapProgress;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public byte[] reserved;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ParticipantTimingSectors
        {
            public uint sectorTimeCurrentLap1;
            public uint sectorTimeCurrentLap2;
            public uint sectorTimeLastLap1;
            public uint sectorTimeLastLap2;
            public uint sectorTimeBestLap1;
            public uint sectorTimeBestLap2;
            public uint sectorTimeBest1;
            public uint sectorTimeBest2;
            public uint sectorTimeBest3;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ParticipantMotion
        {
            public CarMotionOrientation orientation;
            public CarMotionVelocityEssential velocity;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ParticipantDamage
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = Const.DAMAGE_BYTES_PER_PARTICIPANT)]
            public byte[] damageStates;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
        public struct ParticipantInfo
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Const.CAR_ID_LENGTH_MAX)]
            public string carId;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Const.CAR_NAME_LENGTH_MAX)]
            public string carName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Const.PLAYER_NAME_LENGTH_MAX)]
            public string playerName;
            public byte participantIndex;
            public int lastNormalTrackStatusTime;
            public int lastCollisionTime;
            public int lastResetTime;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public byte[] reserved;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct PacketHeader
        {
            public uint signature;
            public PacketType packetType;
            public GameStatusFlags statusFlags;
            public int sessionTime;
            public int raceTime;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct PacketMain
        {
            public PacketHeader header;
            public MarshalFlags marshalFlagsPlayer;
            public ParticipantLeaderboard participantPlayerLeaderboard;
            public ParticipantTiming participantPlayerTiming;
            public ParticipantTimingSectors participantPlayerTimingSectors;
            public ParticipantInfo participantPlayerInfo;
            public ParticipantDamage participantPlayerDamage;
            public CarFull carPlayer;
            public Session session;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
            public byte[] reserved;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct PacketParticipantsLeaderboard
        {
            public PacketHeader header;
            public ParticipantVisibility participantVisibility;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = Const.PARTICIPANTS_MAX)]
            public ParticipantLeaderboard[] participantsLeaderboard;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
            public byte[] reserved;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct PacketParticipantsTiming
        {
            public PacketHeader header;
            public ParticipantVisibility participantVisibility;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = Const.PARTICIPANTS_MAX)]
            public ParticipantTiming[] participantsTiming;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
            public byte[] reserved;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct PacketParticipantsTimingSectors
        {
            public PacketHeader header;
            public ParticipantVisibility participantVisibility;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = Const.PARTICIPANTS_MAX)]
            public ParticipantTimingSectors[] participantsTimingSectors;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
            public byte[] reserved;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct PacketParticipantsMotion
        {
            public PacketHeader header;
            public ParticipantVisibility participantVisibility;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = Const.PARTICIPANTS_MAX)]
            public ParticipantMotion[] participantsMotion;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct PacketParticipantsInfo
        {
            public PacketHeader header;
            public ParticipantVisibility participantVisibility;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = Const.PARTICIPANTS_MAX)]
            public ParticipantInfo[] participantsInfo;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 512)]
            public byte[] reserved;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct PacketParticipantsDamage
        {
            public PacketHeader header;
            public ParticipantVisibility participantVisibility;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = Const.PARTICIPANTS_MAX)]
            public ParticipantDamage[] participantsDamage;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public byte[] reserved;
        }
    }
}
