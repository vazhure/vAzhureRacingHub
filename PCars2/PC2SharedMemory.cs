/// Description: 
/// Storage structure for storing and updating shared memory
/// 
/// Copyright(c) MWL.All rights reserved.
/// 
/// C# adaptation: Zhuravlev Andrey
/// v.azhure@gmail.com
/// 

using System;
using System.Runtime.InteropServices;
using System.Text;

namespace PC2SharedMemory
{
    #region Enums
    public enum SHARED_MEMORY_VERSION : uint
    {
        none = 0,
        version1 = 1,
        version2 = 2,
        version3 = 3,
        version4 = 4,
        version5 = 5,
        version6 = 6,
        version7 = 7,
        version8 = 8,
        actual = version8
    }

    /// <summary>
    /// Game State enumeration
    /// </summary>
    public enum GameState : uint
    {
        EXITED = 0,
        FRONT_END,
        INPLAYING,
        INPAUSED,
        ININMENU_TIME_TICKING,
        INRESTARTING,
        INREPLAY,
        FRONT_END_REPLAY,
    }

    /// <summary>
    /// Session State
    /// </summary>
    public enum SessionState : uint
    {
        INVALID = 0,
        PRACTICE,
        TEST,
        QUALIFY,
        FORMATION_LAP,
        RACE,
        TIME_ATTACK,
    }

    /// <summary>
    /// Race State
    /// </summary>
    public enum RaceState : uint
    {
        INVALID,
        NOT_STARTED,
        RACING,
        FINISHED,
        DISQUALIFIED,
        RETIRED,
        DNF,
    }

    /// <summary>
    /// Flags
    /// </summary>
    public enum FlagColour : uint
    {
        /// <summary>
        /// Not used for actual flags, only for some query functions
        /// </summary>
        NONE = 0,
        /// <summary>
        /// End of danger zone, or race started
        /// </summary>
        GREEN,
        /// <summary>
        /// Faster car wants to overtake the participant
        /// </summary>
        BLUE,
        /// <summary>
        /// Slow car in area
        /// </summary>
        WHITE_SLOW_CAR,
        /// <summary>
        /// Final Lap
        /// </summary>
        WHITE_FINAL_LAP,
        /// <summary>
        /// Huge collisions where one or more cars become wrecked and block the track
        /// </summary>
        RED,
        /// <summary>
        /// Danger on the racing surface itself
        /// </summary>
        YELLOW,
        /// <summary>
        /// Danger that wholly or partly blocks the racing surface
        /// </summary>
        DOUBLE_YELLOW,
        /// <summary>
        /// Unsportsmanlike conduct
        /// </summary>
        BLACK_AND_WHITE,
        /// <summary>
        /// Mechanical Failure
        /// </summary>
        BLACK_ORANGE_CIRCLE,
        /// <summary>
        /// Participant disqualified
        /// </summary>
        BLACK,
        /// <summary>
        /// Chequered flag
        /// </summary>
        CHEQUERED,
    }

    /// <summary>
    /// Flag reason
    /// </summary>
    public enum FlagReason : uint
    {
        NONE = 0,
        SOLO_CRASH,
        VEHICLE_CRASH,
        VEHICLE_OBSTRUCTION,
    }

    /// <summary>
    /// Pit mode
    /// </summary>
    public enum PitMode : uint
    {
        NONE = 0,
        DRIVING_INTO_PITS,
        IN_PIT,
        DRIVING_OUT_OF_PITS,
        IN_GARAGE,
        DRIVING_OUT_OF_GARAGE,
    }

    /// <summary>
    /// Pit sheldue
    /// </summary>
    public enum PitSheldue : uint
    {
        NONE = 0, // Nothing scheduled
        PLAYER_REQUESTED, // Used for standard pit sequence - requested by player
        ENGINEER_REQUESTED, // Used for standard pit sequence - requested by engineer
        DAMAGE_REQUESTED, // Used for standard pit sequence - requested by engineer for damage
        MANDATORY, // Used for standard pit sequence - requested by engineer from career enforced lap number
        DRIVE_THROUGH, // Used for drive-through penalty
        STOP_GO, // Used for stop-go penalty
        PITSPOT_OCCUPIED, // Used for drive-through when pitspot is occupied
    }

    /// <summary>
    /// Terrain Materials
    /// </summary>
    public enum TerrainMaterials : uint
    {
        ROAD = 0,
        LOW_GRIP_ROAD,
        BUMPY_ROAD1,
        BUMPY_ROAD2,
        BUMPY_ROAD3,
        MARBLES,
        GRASSY_BERMS,
        GRASS,
        GRAVEL,
        BUMPY_GRAVEL,
        RUMBLE_STRIPS,
        DRAINS,
        TYREWALLS,
        CEMENTWALLS,
        GUARDRAILS,
        SAND,
        BUMPY_SAND,
        DIRT,
        BUMPY_DIRT,
        DIRT_ROAD,
        BUMPY_DIRT_ROAD,
        PAVEMENT,
        DIRT_BANK,
        WOOD,
        DRY_VERGE,
        EXIT_RUMBLE_STRIPS,
        GRASSCRETE,
        LONG_GRASS,
        SLOPE_GRASS,
        COBBLES,
        SAND_ROAD,
        BAKED_CLAY,
        ASTROTURF,
        SNOWHALF,
        SNOWFULL,
        DAMAGED_ROAD1,
        TRAIN_TRACK_ROAD,
        BUMPYCOBBLES,
        ARIES_ONLY,
        ORION_ONLY,
        B1RUMBLES,
        B2RUMBLES,
        ROUGH_SAND_MEDIUM,
        ROUGH_SAND_HEAVY,
        SNOWWALLS,
        ICE_ROAD,
        RUNOFF_ROAD,
        ILLEGAL_STRIP,
    }

    /// <summary>
    /// Crash damage state
    /// </summary>
    public enum CrashDamageState : uint
    {
        NONE = 0,
        OFFTRACK,
        LARGE_PROP,
        SPINNING,
        ROLLING,
    }
    #endregion

    #region Flags
    /// <summary>
    /// Car Flags
    /// </summary>
    [Flags]
    public enum CarFlags : uint
    {
        /// <summary>
        /// Headlight state
        /// </summary>
        HEADLIGHT = (1 << 0),
        /// <summary>
        /// Engine state
        /// </summary>
        ENGINE_ACTIVE = (1 << 1),
        /// <summary>
        /// Engine warning
        /// </summary>
        ENGINE_WARNING = (1 << 2),
        /// <summary>
        /// Speed limiter state
        /// </summary>
        SPEED_LIMITER = (1 << 3),
        /// <summary>
        /// ABS state
        /// </summary>
        ABS = (1 << 4),
        /// <summary>
        /// Handbrake state
        /// </summary>
        HANDBRAKE = (1 << 5),
    }

    [Flags]
    public enum TyreFlags : uint
    {
        /// <summary>
        /// Tyres attached state
        /// </summary>
        ATTACHED = (1 << 0),
        /// <summary>
        /// Tyres inflated state
        /// </summary>
        INFLATED = (1 << 1),
        /// <summary>
        /// Tyres on ground state
        /// </summary>
        IS_ON_GROUND = (1 << 2),
    }
    #endregion

    /// <summary>
    /// All Participant information 
    /// Max: 64
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct ParticipantInfo
    {
        [MarshalAs(UnmanagedType.I1)]
        public bool mIsActive;
        [MarshalAs(UnmanagedType.Struct)]
        public CharString mName;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public float[] mWorldPosition; // [ UNITS = World Space  X  Y  Z ]
        [MarshalAs(UnmanagedType.R4)]
        public float mCurrentLapDistance; // [ UNITS = Meters ]   [ RANGE = 0.0f->... ]    [ UNSET = 0.0f ]
        [MarshalAs(UnmanagedType.U4)]
        public uint mRacePosition; // [ RANGE = 1->... ]   [ UNSET = 0 ]
        [MarshalAs(UnmanagedType.U4)]
        public uint mLapsCompleted; // [ RANGE = 0->... ]   [ UNSET = 0 ]
        [MarshalAs(UnmanagedType.U4)]
        public uint mCurrentLap; // [ RANGE = 0->... ]   [ UNSET = 0 ]
        [MarshalAs(UnmanagedType.I4)]
        public int mCurrentSector; // [ RANGE = 0->... ]   [ UNSET = -1 ]
    }

    /// <summary>
    /// C string adapter
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct CharString
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public char[] text;

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            foreach (char b in text)
                if (b == 0)
                    break;
                else
                    sb.Append(b);

            return sb.ToString();
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct CharString40
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 40)]
        public char[] text;

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            foreach (char b in text)
                if (b == 0)
                    break;
                else
                    sb.Append(b);

            return sb.ToString();
        }
    }

    /// <summary>
    /// Main PC2 shared memory structure
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct PC2SharedMemoryStruct
    {
        /// <summary>
        /// Data packet version
        /// </summary>
        [MarshalAs(UnmanagedType.U4)]
        public uint mVersion; 
        [MarshalAs(UnmanagedType.U4)]
        public uint mBuildVersionNumber; // [ RANGE = 0->... ]   [ UNSET = 0 ]

        [MarshalAs(UnmanagedType.U4)]
        public GameState mGameState;
        [MarshalAs(UnmanagedType.U4)]
        public SessionState mSessionState;
        [MarshalAs(UnmanagedType.U4)]
        public RaceState mRaceState;

        [MarshalAs(UnmanagedType.I4)]
        public int mViewedParticipantIndex; // [ RANGE = 0->STORED_PARTICIPANTS_MAX ]   [ UNSET = -1 ]
        [MarshalAs(UnmanagedType.I4)]
        public int mNumParticipants; // [ RANGE = 0->STORED_PARTICIPANTS_MAX ]   [ UNSET = -1 ]
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct, SizeConst = 64)]
        public ParticipantInfo[] mParticipantInfo;

        [MarshalAs(UnmanagedType.R4)]
        public float mUnfilteredThrottle; // [ RANGE = 0.0f->1.0f ]
        [MarshalAs(UnmanagedType.R4)]
        public float mUnfilteredBrake; // [ RANGE = 0.0f->1.0f ]
        [MarshalAs(UnmanagedType.R4)]
        public float mUnfilteredSteering; // [ RANGE = -1.0f->1.0f ]
        [MarshalAs(UnmanagedType.R4)]
        public float mUnfilteredClutch; // [ RANGE = 0.0f->1.0f ]

        #region Vehicle information
        [MarshalAs(UnmanagedType.Struct)]
        public CharString mCarName;
        [MarshalAs(UnmanagedType.Struct)]
        public CharString mCarClassName;
        #endregion

        #region Event information
        [MarshalAs(UnmanagedType.U4)]
        public uint mLapsInEvent; // [ RANGE = 0->... ]   [ UNSET = 0 ]
        [MarshalAs(UnmanagedType.Struct)]
        public CharString mTrackLocation; // untranslated shortened English name
        [MarshalAs(UnmanagedType.Struct)]
        public CharString mTrackVariation; // untranslated shortened English variation description
        [MarshalAs(UnmanagedType.R4)]
        public float mTrackLength; // [ UNITS = Meters ]   [ RANGE = 0.0f->... ]    [ UNSET = 0.0f ]
        #endregion

        #region Timings
        [MarshalAs(UnmanagedType.I4)]
        public int mNumSectors; // [ RANGE = 0->... ]   [ UNSET = -1 ]
        [MarshalAs(UnmanagedType.U1)]
        public bool mLapInvalidated; // [ UNITS = public boolean ]   [ RANGE = false->true ]   [ UNSET = false ]
        [MarshalAs(UnmanagedType.R4)]
        public float mBestLapTime; // [ UNITS = seconds ]   [ RANGE = 0.0f->... ]   [ UNSET = -1.0f ]
        [MarshalAs(UnmanagedType.R4)]
        public float mLastLapTime; // [ UNITS = seconds ]   [ RANGE = 0.0f->... ]   [ UNSET = 0.0f ]
        [MarshalAs(UnmanagedType.R4)]
        public float mCurrentTime; // [ UNITS = seconds ]   [ RANGE = 0.0f->... ]   [ UNSET = 0.0f ]
        [MarshalAs(UnmanagedType.R4)]
        public float mSplitTimeAhead; // [ UNITS = seconds ]   [ RANGE = 0.0f->... ]   [ UNSET = -1.0f ]
        [MarshalAs(UnmanagedType.R4)]
        public float mSplitTimeBehind; // [ UNITS = seconds ]   [ RANGE = 0.0f->... ]   [ UNSET = -1.0f ]
        [MarshalAs(UnmanagedType.R4)]
        public float mSplitTime; // [ UNITS = seconds ]   [ RANGE = 0.0f->... ]   [ UNSET = 0.0f ]
        [MarshalAs(UnmanagedType.R4)]
        public float mEventTimeRemaining; // [ UNITS = milliseconds ]   [ RANGE = 0.0f->... ]   [ UNSET = -1.0f ]
        [MarshalAs(UnmanagedType.R4)]
        public float mPersonalFastestLapTime; // [ UNITS = seconds ]   [ RANGE = 0.0f->... ]   [ UNSET = -1.0f ]
        [MarshalAs(UnmanagedType.R4)]
        public float mWorldFastestLapTime; // [ UNITS = seconds ]   [ RANGE = 0.0f->... ]   [ UNSET = -1.0f ]
        [MarshalAs(UnmanagedType.R4)]
        public float mCurrentSector1Time; // [ UNITS = seconds ]   [ RANGE = 0.0f->... ]   [ UNSET = -1.0f ]
        [MarshalAs(UnmanagedType.R4)]
        public float mCurrentSector2Time; // [ UNITS = seconds ]   [ RANGE = 0.0f->... ]   [ UNSET = -1.0f ]
        [MarshalAs(UnmanagedType.R4)]
        public float mCurrentSector3Time; // [ UNITS = seconds ]   [ RANGE = 0.0f->... ]   [ UNSET = -1.0f ]
        [MarshalAs(UnmanagedType.R4)]
        public float mFastestSector1Time; // [ UNITS = seconds ]   [ RANGE = 0.0f->... ]   [ UNSET = -1.0f ]
        [MarshalAs(UnmanagedType.R4)]
        public float mFastestSector2Time; // [ UNITS = seconds ]   [ RANGE = 0.0f->... ]   [ UNSET = -1.0f ]
        [MarshalAs(UnmanagedType.R4)]
        public float mFastestSector3Time; // [ UNITS = seconds ]   [ RANGE = 0.0f->... ]   [ UNSET = -1.0f ]
        [MarshalAs(UnmanagedType.R4)]
        public float mPersonalFastestSector1Time; // [ UNITS = seconds ]   [ RANGE = 0.0f->... ]   [ UNSET = -1.0f ]
        [MarshalAs(UnmanagedType.R4)]
        public float mPersonalFastestSector2Time; // [ UNITS = seconds ]   [ RANGE = 0.0f->... ]   [ UNSET = -1.0f ]
        [MarshalAs(UnmanagedType.R4)]
        public float mPersonalFastestSector3Time; // [ UNITS = seconds ]   [ RANGE = 0.0f->... ]   [ UNSET = -1.0f ]
        [MarshalAs(UnmanagedType.R4)]
        public float mWorldFastestSector1Time; // [ UNITS = seconds ]   [ RANGE = 0.0f->... ]   [ UNSET = -1.0f ]
        [MarshalAs(UnmanagedType.R4)]
        public float mWorldFastestSector2Time; // [ UNITS = seconds ]   [ RANGE = 0.0f->... ]   [ UNSET = -1.0f ]
        [MarshalAs(UnmanagedType.R4)]
        public float mWorldFastestSector3Time; // [ UNITS = seconds ]   [ RANGE = 0.0f->... ]   [ UNSET = -1.0f ]
        #endregion

        #region Flags
        [MarshalAs(UnmanagedType.U4)]
        public FlagColour mHighestFlagColour;
        [MarshalAs(UnmanagedType.U4)]
        public FlagReason mHighestFlagReason;
        #endregion

        #region Pit Info
        [MarshalAs(UnmanagedType.U4)]
        public PitMode mPitMode;
        [MarshalAs(UnmanagedType.U4)]
        public PitSheldue mPitSchedule;
        #endregion

        #region Car State
        [MarshalAs(UnmanagedType.U4)]
        public CarFlags mCarFlags;
        [MarshalAs(UnmanagedType.R4)]
        public float mOilTempCelsius; // [ UNITS = Celsius ]   [ UNSET = 0.0f ]
        [MarshalAs(UnmanagedType.R4)]
        public float mOilPressureKPa; // [ UNITS = Kilo-pascal ]   [ RANGE = 0.0f->... ]   [ UNSET = 0.0f ]
        [MarshalAs(UnmanagedType.R4)]
        public float mWaterTempCelsius; // [ UNITS = Celsius ]   [ UNSET = 0.0f ]
        [MarshalAs(UnmanagedType.R4)]
        public float mWaterPressureKPa; // [ UNITS = Kilo-pascal ]   [ RANGE = 0.0f->... ]   [ UNSET = 0.0f ]
        [MarshalAs(UnmanagedType.R4)]
        public float mFuelPressureKPa; // [ UNITS = Kilo-pascal ]   [ RANGE = 0.0f->... ]   [ UNSET = 0.0f ]
        [MarshalAs(UnmanagedType.R4)]
        public float mFuelLevel; // [ RANGE = 0.0f->1.0f ]
        [MarshalAs(UnmanagedType.R4)]
        public float mFuelCapacity; // [ UNITS = Liters ]   [ RANGE = 0.0f->1.0f ]   [ UNSET = 0.0f ]
        [MarshalAs(UnmanagedType.R4)]
        public float mSpeed; // [ UNITS = Meters per-second ]   [ RANGE = 0.0f->... ]
        [MarshalAs(UnmanagedType.R4)]
        public float mRpm; // [ UNITS = Revolutions per minute ]   [ RANGE = 0.0f->... ]   [ UNSET = 0.0f ]
        [MarshalAs(UnmanagedType.R4)]
        public float mMaxRPM; // [ UNITS = Revolutions per minute ]   [ RANGE = 0.0f->... ]   [ UNSET = 0.0f ]
        [MarshalAs(UnmanagedType.R4)]
        public float mBrake; // [ RANGE = 0.0f->1.0f ]
        [MarshalAs(UnmanagedType.R4)]
        public float mThrottle; // [ RANGE = 0.0f->1.0f ]
        [MarshalAs(UnmanagedType.R4)]
        public float mClutch; // [ RANGE = 0.0f->1.0f ]
        [MarshalAs(UnmanagedType.R4)]
        public float mSteering; // [ RANGE = -1.0f->1.0f ]
        [MarshalAs(UnmanagedType.I4)]
        public int mGear; // [ RANGE = -1 (Reverse)  0 (Neutral)  1 (Gear 1)  2 (Gear 2)  etc... ]   [ UNSET = 0 (Neutral) ]
        [MarshalAs(UnmanagedType.I4)]
        public int mNumGears; // [ RANGE = 0->... ]   [ UNSET = -1 ]
        [MarshalAs(UnmanagedType.R4)]
        public float mOdometerKM; // [ RANGE = 0.0f->... ]   [ UNSET = -1.0f ]
        [MarshalAs(UnmanagedType.I1)]
        public bool mAntiLockActive; // [ UNITS = public boolean ]   [ RANGE = false->true ]   [ UNSET = false ]
        [MarshalAs(UnmanagedType.I4)]
        public int mLastOpponentCollisionIndex; // [ RANGE = 0->STORED_PARTICIPANTS_MAX ]   [ UNSET = -1 ]
        [MarshalAs(UnmanagedType.R4)]
        public float mLastOpponentCollisionMagnitude; // [ RANGE = 0.0f->... ]
        [MarshalAs(UnmanagedType.U1)]
        public bool mBoostActive; // [ UNITS = public boolean ]   [ RANGE = false->true ]   [ UNSET = false ]
        [MarshalAs(UnmanagedType.R4)]
        public float mBoostAmount; // [ RANGE = 0.0f->100.0f ] 
        #endregion

        #region Motion & Device Related
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public float[] mOrientation; // [ UNITS = Euler Angles ]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public float[] mLocalVelocity; // [ UNITS = Meters per-second ]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public float[] mWorldVelocity; // [ UNITS = Meters per-second ]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public float[] mAngularVelocity; // [ UNITS = Radians per-second ]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public float[] mLocalAcceleration; // [ UNITS = Meters per-second ]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public float[] mWorldAcceleration; // [ UNITS = Meters per-second ]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public float[] mExtentsCentre; // [ UNITS = Local Space  X  Y  Z ]

        public float Heave
        {
            get { return (mLocalAcceleration[1] / 9.81f); }
        }

        public float Surge
        {
            get { return (mLocalAcceleration[2] / 9.81f); }
        }

        public float Sway
        {
            get { return (mLocalAcceleration[0] / 9.81f); }
        }
        #endregion

        #region Wheels and Tires
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public TyreFlags[] mTyreFlags;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public TerrainMaterials[] mTerrain;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] mTyreY; // [ UNITS = Local Space  Y ]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] mTyreRPS; // [ UNITS = Revolutions per second ]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] mTyreSlipSpeed; // OBSOLETE, kept for backward compatibility only
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] mTyreTemp; // [ UNITS = Celsius ]   [ UNSET = 0.0f ]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] mTyreGrip; // OBSOLETE, kept for backward compatibility only
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] mTyreHeightAboveGround; // [ UNITS = Local Space  Y ]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] mTyreLateralStiffness; // OBSOLETE, kept for backward compatibility only
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] mTyreWear; // [ RANGE = 0.0f->1.0f ]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] mBrakeDamage; // [ RANGE = 0.0f->1.0f ]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] mSuspensionDamage; // [ RANGE = 0.0f->1.0f ]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] mBrakeTempCelsius; // [ UNITS = Celsius ]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] mTyreTreadTemp; // [ UNITS = Kelvin ]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] mTyreLayerTemp; // [ UNITS = Kelvin ]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] mTyreCarcassTemp; // [ UNITS = Kelvin ]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] mTyreRimTemp; // [ UNITS = Kelvin ]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] mTyreInternalAirTemp; // [ UNITS = Kelvin ]
        #endregion

        #region Car Damage
        [MarshalAs(UnmanagedType.I4)]
        public CrashDamageState mCrashState;
        [MarshalAs(UnmanagedType.R4)]
        public float mAeroDamage; // [ RANGE = 0.0f->1.0f ]
        [MarshalAs(UnmanagedType.R4)]
        public float mEngineDamage; // [ RANGE = 0.0f->1.0f ]
        #endregion

        #region Weather
        [MarshalAs(UnmanagedType.R4)]
        public float mAmbientTemperature; // [ UNITS = Celsius ]   [ UNSET = 25.0f ]
        [MarshalAs(UnmanagedType.R4)]
        public float mTrackTemperature; // [ UNITS = Celsius ]   [ UNSET = 30.0f ]
        [MarshalAs(UnmanagedType.R4)]
        public float mRainDensity; // [ UNITS = How much rain will fall ]   [ RANGE = 0.0f->1.0f ]
        [MarshalAs(UnmanagedType.R4)]
        public float mWindSpeed; // [ RANGE = 0.0f->100.0f ]   [ UNSET = 2.0f ]
        [MarshalAs(UnmanagedType.R4)]
        public float mWindDirectionX; // [ UNITS = Normalized Vector X ]
        [MarshalAs(UnmanagedType.R4)]
        public float mWindDirectionY; // [ UNITS = Normalized Vector Y ]
        [MarshalAs(UnmanagedType.R4)]
        public float mCloudBrightness; // [ RANGE = 0.0f->... ]
        #endregion

        // Sequence Number to help slightly with data public integrity reads
        [MarshalAs(UnmanagedType.I4)]
        volatile public int mSequenceNumber; // 0 at the start, incremented at start and end of writing, so odd when Shared Memory is being filled, even when the memory is not being touched

        #region Additional car variables
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] mWheelLocalPositionY; // [ UNITS = Local Space  Y ]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] mSuspensionTravel; // [ UNITS = meters ] [ RANGE 0.f =>... ]  [ UNSET =  0.0f ]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] mSuspensionVelocity; // [ UNITS = Rate of change of push-rod deflection ] [ RANGE 0.f =>... ]  [ UNSET =  0.0f ]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] mAirPressure; // [ UNITS = PSI ]  [ RANGE 0.f =>... ]  [ UNSET =  0.0f ]
        [MarshalAs(UnmanagedType.R4)]
        public float mEngineSpeed; // [ UNITS = Rad/s ] [UNSET = 0.f ]
        [MarshalAs(UnmanagedType.R4)]
        public float mEngineTorque; // [ UNITS = Newton Meters] [UNSET = 0.f ] [ RANGE = 0.0f->... ]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public float[] mWings; // [ RANGE = 0.0f->1.0f ] [UNSET = 0.f ]
        [MarshalAs(UnmanagedType.R4)]
        public float mHandBrake; // [ RANGE = 0.0f->1.0f ] [UNSET = 0.f ]
        #endregion

        #region Additional race variables
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public float[] mCurrentSector1Times; // [ UNITS = seconds ]   [ RANGE = 0.0f->... ]   [ UNSET = -1.0f ]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public float[] mCurrentSector2Times; // [ UNITS = seconds ]   [ RANGE = 0.0f->... ]   [ UNSET = -1.0f ]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public float[] mCurrentSector3Times; // [ UNITS = seconds ]   [ RANGE = 0.0f->... ]   [ UNSET = -1.0f ]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public float[] mFastestSector1Times; // [ UNITS = seconds ]   [ RANGE = 0.0f->... ]   [ UNSET = -1.0f ]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public float[] mFastestSector2Times; // [ UNITS = seconds ]   [ RANGE = 0.0f->... ]   [ UNSET = -1.0f ]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public float[] mFastestSector3Times; // [ UNITS = seconds ]   [ RANGE = 0.0f->... ]   [ UNSET = -1.0f ]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public float[] mFastestLapTimes; // [ UNITS = seconds ]   [ RANGE = 0.0f->... ]   [ UNSET = -1.0f ]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public float[] mLastLapTimes; // [ UNITS = seconds ]   [ RANGE = 0.0f->... ]   [ UNSET = -1.0f ]
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 64)]
        public bool[] mLapsInvalidated; // [ UNITS = public boolean for all participants ]   [ RANGE = false->true ]   [ UNSET = false ]
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U4, SizeConst = 64)]
        public RaceState[] mRaceStates;
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U4, SizeConst = 64)]
        public PitMode[] mPitModes;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64 * 3)]
        public float[] mOrientations; // [ UNITS = Euler Angles ]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public float[] mSpeeds; // [ UNITS = Meters per-second ]   [ RANGE = 0.0f->... ]
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct, SizeConst = 64)]
        public CharString[] mCarNames;
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct, SizeConst = 64)]
        public CharString[] mCarClassNames;

        [MarshalAs(UnmanagedType.I4)]
        public int mEnforcedPitStopLap; // [ UNITS = in which lap there will be a mandatory pit-stop] [ RANGE = 0.0f->... ] [ UNSET = -1 ]
        [MarshalAs(UnmanagedType.Struct)]
        public CharString mTranslatedTrackLocation;
        [MarshalAs(UnmanagedType.Struct)]
        public CharString mTranslatedTrackVariation;

        [MarshalAs(UnmanagedType.R4)]
        public float mBrakeBias;      // [ RANGE = 0.0f->1.0f... ]   [ UNSET = -1.0f ]
        [MarshalAs(UnmanagedType.R4)]
        public float mTurboBoostPressure; //	 RANGE = 0.0f->1.0f... ]   [ UNSET = -1.0f ]
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct, SizeConst = 4)]
        public CharString40[] mTyreCompound;// [ strings  ]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public PitSheldue[] mPitSchedules;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public FlagColour [] mHighestFlagColours;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public FlagReason[] mHighestFlagReasons;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public uint[] mNationalities;
        [MarshalAs(UnmanagedType.R4)]
        public float mSnowDensity;

        #endregion

        /// <summary>
        /// Make struct from byte array
        /// </summary>
        /// <param name="data">data bytes</param>
        /// <returns>PC2SharedMemoryStruct</returns>
        public static PC2SharedMemoryStruct FromBytes(byte[] data)
        {
            PC2SharedMemoryStruct pc2 = new PC2SharedMemoryStruct();
            try
            {
                GCHandle h = GCHandle.Alloc(data, GCHandleType.Pinned);
                pc2 = (PC2SharedMemoryStruct)Marshal.PtrToStructure(h.AddrOfPinnedObject(), typeof(PC2SharedMemoryStruct));
                h.Free();
            }
            catch { }

            return pc2;
        }

        /// <summary>
        /// Returns length of structure in bytes
        /// </summary>
        /// <returns></returns>
        public static int Length()
        {
            return Marshal.SizeOf(typeof(PC2SharedMemoryStruct));
        }
    }
}