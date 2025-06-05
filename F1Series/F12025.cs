using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace F1Series
{
    public static class F12025
    {
        // Different packet types
        public enum PacketId : byte
        {
            /// <summary>
            /// Contains all motion data for player’s car – only sent while player is in control
            /// </summary>
            ePacketIdMotion = 0,
            /// <summary>
            /// Data about the session – track, time left
            /// </summary>
            ePacketIdSession = 1,
            /// <summary>
            /// Data about all the lap times of cars in the session
            /// </summary>
            ePacketIdLapData = 2,
            /// <summary>
            /// Various notable events that happen during a session
            /// </summary>
            ePacketIdEvent = 3,
            /// <summary>
            /// List of participants in the session, mostly relevant for multiplayer
            /// </summary>
            ePacketIdParticipants = 4,
            /// <summary>
            /// Packet detailing car setups for cars in the race
            /// </summary>
            ePacketIdCarSetups = 5,
            /// <summary>
            /// Telemetry data for all cars
            /// </summary>
            ePacketIdCarTelemetry = 6,
            /// <summary>
            /// Status data for all cars
            /// </summary>
            ePacketIdCarStatus = 7,
            /// <summary>
            /// Final classification confirmation at the end of a race
            /// </summary>
            ePacketIdFinalClassification = 8,
            /// <summary>
            /// Information about players in a multiplayer lobby
            /// </summary>
            ePacketIdLobbyInfo = 9,
            /// <summary>
            /// Damage status for all cars
            /// </summary>
            ePacketIdCarDamage = 10,
            /// <summary>
            /// Lap and tyre data for session
            /// </summary>
            ePacketIdSessionHistory = 11,
            /// <summary>
            /// Extended tyre set data
            /// </summary>
            ePacketIdTyreSets = 12,
            /// <summary>
            /// Extended motion data for player car
            /// </summary>
            ePacketIdMotionEx = 13,
            /// <summary>
            /// Time Trial specific data
            /// </summary>
            ePacketIdTimeTrial = 14,
            /// <summary>
            /// Lap positions on each lap so a chart can be conpublic structed
            /// </summary>
            ePacketIdLapPositions = 15,
            ePacketIdMax
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct PacketHeader
        {
            /// <summary>
            /// 2025
            /// </summary>
            public ushort m_packetFormat;
            /// <summary>
            /// Game year - last two digits e.g. 25
            /// </summary>
            public byte m_gameYear;
            /// <summary>
            /// Game major version - "X.00"
            /// </summary>
            public byte m_gameMajorVersion;
            /// <summary>
            /// Game minor version - "1.XX"
            /// </summary>
            public byte m_gameMinorVersion;
            /// <summary>
            /// Version of this packet type
            /// </summary>
            public byte m_packetVersion;
            /// <summary>
            /// Identifier for the packet type
            /// </summary>
            public PacketId m_packetId;
            /// <summary>
            /// Unique identifier for the session
            /// </summary>
            public ulong m_sessionUID;
            /// <summary>
            /// Session timestamp
            /// </summary>
            public float m_sessionTime;
            /// <summary>
            /// Identifier for the frame the data was retrieved on
            /// </summary>
            public uint m_frameIdentifier;
            /// <summary>
            /// Overall identifier for the frame the data was retrieved on, doesn't go back after flashbacks
            /// </summary>
            public uint m_overallFrameIdentifier;
            /// <summary>
            /// Index of player's car in the array
            /// </summary>
            public byte m_playerCarIndex;
            /// <summary>
            /// Index of secondary player's car in the array (splitscreen) - 255 if no second player
            /// </summary>
            public byte m_secondaryPlayerCarIndex;
        };

        //-----------------------------------------------------------------------------
        // Motion - 1349 bytes
        //-----------------------------------------------------------------------------

        //-----------------------------------------------------------------------------
        // Motion data for one car
        //-----------------------------------------------------------------------------
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct CarMotionData
        {
            /// <summary>
            /// World space X position - metres
            /// </summary>
            public float m_worldPositionX;
            /// <summary>
            /// // World space Y position
            /// </summary>
            public float m_worldPositionY;
            /// <summary>
            /// World space Z position
            /// </summary>
            public float m_worldPositionZ;
            /// <summary>
            /// Velocity in world space X - meters/s
            /// </summary>
            public float m_worldVelocityX;
            /// <summary>
            /// Velocity in world space Y
            /// </summary>
            public float m_worldVelocityY;
            /// <summary>
            /// Velocity in world space Z
            /// </summary>
            public float m_worldVelocityZ;
            /// <summary>
            /// World space forward X direction (normalized)
            /// </summary>
            public short m_worldForwardDirX;
            /// <summary>
            /// World space forward Y direction (normalized)
            /// </summary>
            public short m_worldForwardDirY;
            /// <summary>
            /// World space forward Z direction (normalized)
            /// </summary>
            public short m_worldForwardDirZ;
            /// <summary>
            /// World space right X direction (normalized)
            /// </summary>
            public short m_worldRightDirX;
            /// <summary>
            /// World space right Y direction (normalized)
            /// </summary>
            public short m_worldRightDirY;
            /// <summary>
            /// World space right Z direction (normalized)
            /// </summary>
            public short m_worldRightDirZ;
            /// <summary>
            /// Lateral G-Force component
            /// </summary>
            public float m_gForceLateral;
            /// <summary>
            /// Longitudinal G-Force component
            /// </summary>
            public float m_gForceLongitudinal;
            /// <summary>
            /// Vertical G-Force component
            /// </summary>
            public float m_gForceVertical;
            /// <summary>
            /// Yaw angle in radians
            /// </summary>
            public float m_yaw;
            /// <summary>
            /// Pitch angle in radians
            /// </summary>
            public float m_pitch;
            /// <summary>
            /// Roll angle in radians
            /// </summary>
            public float m_roll;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct PacketMotionData
        {
            /// <summary>
            /// Header
            /// </summary>
            public PacketHeader m_header;

            /// <summary>
            /// Packet specific data
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = cs_maxNumCarsInUDPData)]
            public CarMotionData[] m_carMotionData;  // Data for all cars on track
        };

        const int cs_maxNumCarsInUDPData = 22;
        const int cs_maxParticipantNameLen = 32;
        const int cs_maxTyreStints = 8;
        const int cs_maxNumTyreSets = 13 + 7; // 13 slick and 7 wet weather

        //-----------------------------------------------------------------------------
        // Session - 753 bytes
        //-----------------------------------------------------------------------------
        const int cs_maxMarshalsZonePerLap = 21;
        const int cs_maxWeatherForecastSamples = 64;
        const int cs_maxSessionsInWeekend = 12;

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct MarshalZone
        {
            /// <summary>
            /// Fraction (0..1) of way through the lap the marshal zone starts
            /// </summary>
            public float m_zoneStart;
            /// <summary>
            /// -1 = invalid/unknown, 0 = none, 1 = green, 2 = blue, 3 = yellow
            /// </summary>
            public sbyte m_zoneFlag;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct WeatherForecastSample
        {
            /// <summary>
            /// 0 = unknown, see appendix
            /// </summary>
            public byte m_sessionType;
            /// <summary>
            /// Time in minutes the forecast is for
            /// </summary>
            public byte m_timeOffset;
            /// <summary>
            /// Weather - 0 = clear, 1 = light cloud, 2 = overcast, 3 = light rain, 4 = heavy rain, 5 = storm
            /// </summary>
            public byte m_weather;
            /// <summary>
            /// Track temp. in degrees celsius
            /// </summary>
            public sbyte m_trackTemperature;
            /// <summary>
            /// Track temp. change - 0 = up, 1 = down, 2 = no change
            /// </summary>
            public sbyte m_trackTemperatureChange;
            /// <summary>
            /// Air temp. in degrees celsius
            /// </summary>
            public sbyte m_airTemperature;
            /// <summary>
            /// Air temp. change - 0 = up, 1 = down, 2 = no change
            /// </summary>
            public sbyte m_airTemperatureChange;
            /// <summary>
            /// Rain percentage (0-100)
            /// </summary>
            public byte m_rainPercentage;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct PacketSessionData
        {
            /// <summary>
            /// Header
            /// </summary>
            public PacketHeader m_header;

            // Packet specific data
            /// <summary>
            /// Weather - 0 = clear, 1 = light cloud, 2 = overcast, 3 = light rain, 4 = heavy rain, 5 = storm
            /// </summary>
            public byte m_weather;

            /// <summary>
            /// Track temperature in degrees Celsius
            /// </summary>
            public sbyte m_trackTemperature;

            /// <summary>
            /// Air temperature in degrees Celsius
            /// </summary>
            public sbyte m_airTemperature;

            /// <summary>
            /// Total number of laps in this race
            /// </summary>
            public byte m_totalLaps;

            /// <summary>
            /// Track length in metres
            /// </summary>
            public ushort m_trackLength;

            /// <summary>
            /// 0 = unknown, see appendix
            /// </summary>
            public byte m_sessionType;

            /// <summary>
            /// -1 for unknown, see appendix
            /// </summary>
            public sbyte m_trackId;

            /// <summary>
            /// Formula, 0 = F1 Modern, 1 = F1 Classic, 2 = F2, 3 = F1 Generic, 4 = Beta, 6 = Esports, 8 = F1 World, 9 = F1 Elimination
            /// </summary>
            public byte m_formula;

            /// <summary>
            /// Time left in session in seconds
            /// </summary>
            public ushort m_sessionTimeLeft;

            /// <summary>
            /// Session duration in seconds
            /// </summary>
            public ushort m_sessionDuration;

            /// <summary>
            /// Pit speed limit in kilometres per hour
            /// </summary>
            public byte m_pitSpeedLimit;

            /// <summary>
            /// Whether the game is paused - network game only
            /// </summary>
            public byte m_gamePaused;

            /// <summary>
            /// Whether the player is spectating
            /// </summary>
            public byte m_isSpectating;

            /// <summary>
            /// Index of the car being spectated
            /// </summary>
            public byte m_spectatorCarIndex;

            /// <summary>
            /// SLI Pro support, 0 = inactive, 1 = active
            /// </summary>
            public byte m_sliProNativeSupport;

            /// <summary>
            /// Number of marshal zones to follow
            /// </summary>
            public byte m_numMarshalZones;
            /// <summary>
            /// List of marshal zones - max 21
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = cs_maxMarshalsZonePerLap)]
            public MarshalZone[] m_marshalZones;

            /// <summary>
            /// 0 = no safety car, 1 = full, 2 = virtual, 3 = formation lap
            /// </summary>
            public byte m_safetyCarStatus;

            /// <summary>
            /// 0 = offline, 1 = online
            /// </summary>
            public byte m_networkGame;

            /// <summary>
            /// Number of weather samples to follow
            /// </summary>
            public byte m_numWeatherForecastSamples;

            /// <summary>
            /// Array of weather forecast samples
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = cs_maxWeatherForecastSamples)]
            public WeatherForecastSample[] m_weatherForecastSamples;

            /// <summary>
            /// 0 = Perfect, 1 = Approximate
            /// </summary>
            public byte m_forecastAccuracy;

            /// <summary>
            /// AI difficulty - 0-110
            /// </summary>
            public byte m_aiDifficulty;

            /// <summary>
            /// Identifier for season - persists across saves
            /// </summary>
            public uint m_seasonLinkIdentifier;

            /// <summary>
            /// Identifier for weekend - persists across saves
            /// </summary>
            public uint m_weekendLinkIdentifier;

            /// <summary>
            /// Identifier for session - persists across saves
            /// </summary>
            public uint m_sessionLinkIdentifier;

            /// <summary>
            /// Ideal lap to pit on for current strategy (player)
            /// </summary>
            public byte m_pitStopWindowIdealLap;

            /// <summary>
            /// Latest lap to pit on for current strategy (player)
            /// </summary>
            public byte m_pitStopWindowLatestLap;

            /// <summary>
            /// Predicted position to rejoin at (player)
            /// </summary>
            public byte m_pitStopRejoinPosition;

            /// <summary>
            /// 0 = off, 1 = on
            /// </summary>
            public byte m_steeringAssist;

            /// <summary>
            /// 0 = off, 1 = low, 2 = medium, 3 = high
            /// </summary>
            public byte m_brakingAssist;

            /// <summary>
            /// 1 = manual, 2 = manual & suggested gear, 3 = auto
            /// </summary>
            public byte m_gearboxAssist;

            /// <summary>
            /// 0 = off, 1 = on
            /// </summary>
            public byte m_pitAssist;

            /// <summary>
            /// 0 = off, 1 = on
            /// </summary>
            public byte m_pitReleaseAssist;

            /// <summary>
            /// 0 = off, 1 = on
            /// </summary>
            public byte m_ERSAssist;

            /// <summary>
            /// 0 = off, 1 = on
            /// </summary>
            public byte m_DRSAssist;

            /// <summary>
            /// 0 = off, 1 = corners only, 2 = full
            /// </summary>
            public byte m_dynamicRacingLine;

            /// <summary>
            /// 0 = 2D, 1 = 3D
            /// </summary>
            public byte m_dynamicRacingLineType;

            /// <summary>
            /// Game mode id - see appendix
            /// </summary>
            public byte m_gameMode;

            /// <summary>
            /// Ruleset - see appendix
            /// </summary>
            public byte m_ruleSet;

            /// <summary>
            /// Local time of day - minutes since midnight
            /// </summary>
            public uint m_timeOfDay;

            /// <summary>
            /// 0 = None, 2 = Very ushort, 3 = ushort, 4 = Medium, 5 = Medium Long, 6 = Long, 7 = Full
            /// </summary>
            public byte m_sessionLength;

            /// <summary>
            /// 0 = MPH, 1 = KPH
            /// </summary>
            public byte m_speedUnitsLeadPlayer;

            /// <summary>
            /// 0 = Celsius, 1 = Fahrenheit
            /// </summary>
            public byte m_temperatureUnitsLeadPlayer;

            /// <summary>
            /// 0 = MPH, 1 = KPH
            /// </summary>
            public byte m_speedUnitsSecondaryPlayer;

            /// <summary>
            /// 0 = Celsius, 1 = Fahrenheit
            /// </summary>
            public byte m_temperatureUnitsSecondaryPlayer;

            /// <summary>
            /// Number of safety cars called during session
            /// </summary>
            public byte m_numSafetyCarPeriods;

            /// <summary>
            /// Number of virtual safety cars called during session
            /// </summary>
            public byte m_numVirtualSafetyCarPeriods;

            /// <summary>
            /// Number of red flags called during session
            /// </summary>
            public byte m_numRedFlagPeriods;

            /// <summary>
            /// 0 = Off, 1 = On
            /// </summary>
            public byte m_equalCarPerformance;

            /// <summary>
            /// 0 = None, 1 = Flashbacks, 2 = Auto-recovery
            /// </summary>
            public byte m_recoveryMode;

            /// <summary>
            /// 0 = Low, 1 = Medium, 2 = High, 3 = Unlimited
            /// </summary>
            public byte m_flashbackLimit;

            /// <summary>
            /// 0 = Simplified, 1 = Realistic
            /// </summary>
            public byte m_surfaceType;

            /// <summary>
            /// 0 = Easy, 1 = Hard
            /// </summary>
            public byte m_lowFuelMode;

            /// <summary>
            /// 0 = Manual, 1 = Assisted
            /// </summary>
            public byte m_raceStarts;

            /// <summary>
            /// 0 = Surface only, 1 = Surface & Carcass
            /// </summary>
            public byte m_tyreTemperature;

            /// <summary>
            /// 0 = On, 1 = Off
            /// </summary>
            public byte m_pitLaneTyreSim;

            /// <summary>
            /// 0 = Off, 1 = Reduced, 2 = Standard, 3 = Simulation
            /// </summary>
            public byte m_carDamage;

            /// <summary>
            /// 0 = Reduced, 1 = Standard, 2 = Simulation
            /// </summary>
            public byte m_carDamageRate;

            /// <summary>
            /// 0 = Off, 1 = Player-to-Player Off, 2 = On
            /// </summary>
            public byte m_collisions;

            /// <summary>
            /// 0 = Disabled, 1 = Enabled
            /// </summary>
            public byte m_collisionsOffForFirstLapOnly;

            /// <summary>
            /// 0 = On, 1 = Off (Multiplayer)
            /// </summary>
            public byte m_mpUnsafePitRelease;

            /// <summary>
            /// 0 = Disabled, 1 = Enabled (Multiplayer)
            /// </summary>
            public byte m_mpOffForGriefing;

            /// <summary>
            /// 0 = Regular, 1 = Strict
            /// </summary>
            public byte m_cornerCuttingStringency;

            /// <summary>
            /// 0 = Off, 1 = On
            /// </summary>
            public byte m_parcFermeRules;

            /// <summary>
            /// 0 = Automatic, 1 = Broadcast, 2 = Immersive
            /// </summary>
            public byte m_pitStopExperience;

            /// <summary>
            /// 0 = Off, 1 = Reduced, 2 = Standard, 3 = Increased
            /// </summary>
            public byte m_safetyCar;

            /// <summary>
            /// 0 = Broadcast, 1 = Immersive
            /// </summary>
            public byte m_safetyCarExperience;

            /// <summary>
            /// 0 = Off, 1 = On
            /// </summary>
            public byte m_formationLap;

            /// <summary>
            /// 0 = Broadcast, 1 = Immersive
            /// </summary>
            public byte m_formationLapExperience;

            /// <summary>
            /// 0 = Off, 1 = Reduced, 2 = Standard, 3 = Increased
            /// </summary>
            public byte m_redFlags;

            /// <summary>
            /// 0 = Off, 1 = On
            /// </summary>
            public byte m_affectsLicenceLevelSolo;

            /// <summary>
            /// 0 = Off, 1 = On
            /// </summary>
            public byte m_affectsLicenceLevelMP;

            /// <summary>
            /// Number of session in following array
            /// </summary>
            public byte m_numSessionsInWeekend;

            /// <summary>
            /// List of session types to show weekend public structure - see appendix for types
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = cs_maxSessionsInWeekend)]
            public byte[] m_weekendStructure;

            /// <summary>
            /// Distance in m around track where sector 2 starts
            /// </summary>
            public float m_sector2LapDistanceStart;

            /// <summary>
            /// Distance in m around track where sector 3 starts
            /// </summary>
            public float m_sector3LapDistanceStart;
        };


        //-----------------------------------------------------------------------------
        // Lap - 1285 bytes
        //-----------------------------------------------------------------------------

        //-----------------------------------------------------------------------------
        // Lap data about one car
        //-----------------------------------------------------------------------------
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct LapData
        {
            /// <summary>
            /// Last lap time in milliseconds
            /// </summary>
            public uint m_lastLapTimeInMS;              // Last lap time in milliseconds

            /// <summary>
            /// Current time around the lap in milliseconds
            /// </summary>
            public uint m_currentLapTimeInMS;

            /// <summary>
            /// Sector 1 time milliseconds part
            /// </summary>
            public ushort m_sector1TimeMSPart;

            /// <summary>
            /// Sector 1 whole minute part
            /// </summary>
            public byte m_sector1TimeMinutesPart;

            /// <summary>
            /// Sector 2 time milliseconds part
            /// </summary>
            public ushort m_sector2TimeMSPart;

            /// <summary>
            /// Sector 2 whole minute part
            /// </summary>
            public byte m_sector2TimeMinutesPart;

            /// <summary>
            /// Time delta to car in front milliseconds part
            /// </summary>
            public ushort m_deltaToCarInFrontMSPart;

            /// <summary>
            /// Time delta to car in front whole minute part
            /// </summary>
            public byte m_deltaToCarInFrontMinutesPart;

            /// <summary>
            /// Time delta to race leader milliseconds part
            /// </summary>
            public ushort m_deltaToRaceLeaderMSPart;

            /// <summary>
            /// Time delta to race leader whole minute part
            /// </summary>
            public byte m_deltaToRaceLeaderMinutesPart;

            /// <summary>
            /// Distance vehicle is around current lap in metres – could be negative if line hasn’t been crossed yet
            /// </summary>
            public float m_lapDistance;

            /// <summary>
            /// Total distance travelled in session in metres – could be negative if line hasn’t been crossed yet
            /// </summary>
            public float m_totalDistance;

            /// <summary>
            /// Delta in seconds for safety car
            /// </summary>
            public float m_safetyCarDelta;

            /// <summary>
            /// Car race position
            /// </summary>
            public byte m_carPosition;

            /// <summary>
            /// Current lap number
            /// </summary>
            public byte m_currentLapNum;

            /// <summary>
            /// Pit status (0 = none, 1 = pitting, 2 = in pit area)
            /// </summary>
            public byte m_pitStatus;

            /// <summary>
            /// Number of pit stops taken in this race
            /// </summary>
            public byte m_numPitStops;

            /// <summary>
            /// Sector (0 = sector1, 1 = sector2, 2 = sector3)
            /// </summary>
            public byte m_sector;

            /// <summary>
            /// Current lap invalid - 0 = valid, 1 = invalid
            /// </summary>
            public byte m_currentLapInvalid;

            /// <summary>
            /// Accumulated time penalties in seconds to be added
            /// </summary>
            public byte m_penalties;

            /// <summary>
            /// Accumulated number of warnings issued
            /// </summary>
            public byte m_totalWarnings;

            /// <summary>
            /// Accumulated number of corner cutting warnings issued
            /// </summary>
            public byte m_cornerCuttingWarnings;

            /// <summary>
            /// Num drive through pens left to serve
            /// </summary>
            public byte m_numUnservedDriveThroughPens;

            /// <summary>
            /// Num stop go pens left to serve
            /// </summary>
            public byte m_numUnservedStopGoPens;

            /// <summary>
            /// Grid position the vehicle started the race in
            /// </summary>
            public byte m_gridPosition;

            /// <summary>
            /// Status of driver - 0 = in garage, 1 = flying lap, 2 = in lap, 3 = out lap, 4 = on track
            /// </summary>
            public byte m_driverStatus;

            /// <summary>
            /// Result status - 0 = invalid, 1 = inactive, 2 = active, 3 = finished, 4 = didnotfinish, 5 = disqualified, 6 = not classified, 7 = retired
            /// </summary>
            public byte m_resultStatus;

            /// <summary>
            /// Pit lane timing, 0 = inactive, 1 = active
            /// </summary>
            public byte m_pitLaneTimerActive;

            /// <summary>
            /// If active, the current time spent in the pit lane in ms
            /// </summary>
            public ushort m_pitLaneTimeInLaneInMS;

            /// <summary>
            /// Time of the actual pit stop in ms
            /// </summary>
            public ushort m_pitStopTimerInMS;

            /// <summary>
            /// Whether the car should serve a penalty at this stop
            /// </summary>
            public byte m_pitStopShouldServePen;

            /// <summary>
            /// Fastest speed through speed trap for this car in kmph
            /// </summary>
            public float m_speedTrapFastestSpeed;

            /// <summary>
            /// Lap no the fastest speed was achieved, 255 = not set
            /// </summary>
            public byte m_speedTrapFastestLap;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct PacketLapData
        {
            public PacketHeader m_header;               // Header

            // Packet specific data
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = cs_maxNumCarsInUDPData)]
            public LapData[] m_lapData;      // Lap data for all cars on track

            public byte m_timeTrialPBCarIdx;                    // Index of Personal Best car in time trial (255 if invalid)
            public byte m_timeTrialRivalCarIdx;                 // Index of Rival car in time trial (255 if invalid)
        };


        //-----------------------------------------------------------------------------
        // Event - 45 bytes
        //-----------------------------------------------------------------------------

        const int cs_eventStringCodeLen = 4;

        // The event details packet is different for each type of event.
        // Make sure only the correct type is interpreted.
        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        public struct EventDataDetails
        {
            [FieldOffset(0)]
            public FastestLap fastestLap;
            [FieldOffset(0)]
            public Retirement retirement;
            [FieldOffset(0)]
            public DRSDisabled dRSDisabled;
            [FieldOffset(0)]
            public TeamMateInPits teamMateInPits;
            [FieldOffset(0)]
            public RaceWinner raceWinner;
            [FieldOffset(0)]
            public Penalty penalty;
            [FieldOffset(0)]
            public SpeedTrap speedTrap;
            [FieldOffset(0)]
            public StartLights startLights;
            [FieldOffset(0)]
            public DriveThroughPenaltyServed driveThroughPenaltyServed;
            [FieldOffset(0)]
            public StopGoPenaltyServed stopGoPenaltyServedl;
            [FieldOffset(0)]
            public Flashback flashback;
            [FieldOffset(0)]
            public Buttons buttons;
            [FieldOffset(0)]
            public Overtake overtake;
            [FieldOffset(0)]
            public SafetyCar safetyCar;
            [FieldOffset(0)]
            public Collision collision;

            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            public struct FastestLap
            {
                /// <summary>
                /// Vehicle index of the car achieving the fastest lap
                /// </summary>
                public byte vehicleIdx;

                /// <summary>
                /// Lap time in seconds
                /// </summary>
                public float lapTime;
            };

            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            public struct Retirement
            {
                /// <summary>
                /// Vehicle index of the car that is retiring
                /// </summary>
                public byte vehicleIdx;

                /// <summary>
                /// Result reason - 
                /// 0 = invalid,
                /// 1 = retired,
                /// 2 = finished,
                /// 3 = terminal damage,
                /// 4 = inactive,
                /// 5 = not enough laps completed,
                /// 6 = black flagged,
                /// 7 = red flagged,
                /// 8 = mechanical failure,
                /// 9 = session skipped,
                /// 10 = session simulated
                /// </summary>
                public byte reason;
            };

            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            public struct DRSDisabled
            {
                /// <summary>
                /// 0 = Wet track, 1 = Safety car deployed, 2 = Red flag, 3 = Min lap not reached
                /// </summary>
                public byte reason;     
            };

            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            public struct TeamMateInPits
            {
                /// <summary>
                /// Vehicle index of team mate
                /// </summary>
                public byte vehicleIdx;
            };

            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            public struct RaceWinner
            {
                /// <summary>
                /// Vehicle index of the race winner
                /// </summary>
                public byte vehicleIdx; 
            };

            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            public struct Penalty
            {
                /// <summary>
                /// Penalty type – see Appendices
                /// </summary>
                public byte penaltyType;

                /// <summary>
                /// Infringement type – see Appendices
                /// </summary>
                public byte infringementType;

                /// <summary>
                /// Vehicle index of the car the penalty is applied to
                /// </summary>
                public byte vehicleIdx;

                /// <summary>
                /// Vehicle index of the other car involved
                /// </summary>
                public byte otherVehicleIdx;

                /// <summary>
                /// Time gained, or time spent doing action in seconds
                /// </summary>
                public byte time;

                /// <summary>
                /// Lap the penalty occurred on
                /// </summary>
                public byte lapNum;

                /// <summary>
                /// Number of places gained by this
                /// </summary>
                public byte placesGained;
            };

            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            public struct SpeedTrap
            {
                /// <summary>
                /// Vehicle index of the vehicle triggering the speed trap
                /// </summary>
                public byte vehicleIdx;

                /// <summary>
                /// Top speed achieved in kilometres per hour
                /// </summary>
                public float speed;

                /// <summary>
                /// Overall fastest speed in session = 1, otherwise 0
                /// </summary>
                public byte isOverallFastestInSession;

                /// <summary>
                /// Fastest speed for driver in session = 1, otherwise 0
                /// </summary>
                public byte isDriverFastestInSession;

                /// <summary>
                /// Vehicle index of the vehicle that is the fastest in this session
                /// </summary>
                public byte fastestVehicleIdxInSession;

                /// <summary>
                /// Speed of the vehicle that is the fastest in this session
                /// </summary>
                public float fastestSpeedInSession;
            };

            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            public struct StartLights
            {
                /// <summary>
                /// Number of lights showing
                /// </summary>
                public byte numLights;                  
            };

            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            public struct DriveThroughPenaltyServed
            {
                /// <summary>
                /// Vehicle index of the vehicle serving drive through
                /// </summary>
                public byte vehicleIdx;                 
            };

            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            public struct StopGoPenaltyServed
            {
                /// <summary>
                /// Vehicle index of the vehicle serving stop go
                /// </summary>
                public byte vehicleIdx;
                /// <summary>
                /// Time spent serving stop go in seconds
                /// </summary>
                public float stopTime;                   
            };

            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            public struct Flashback
            {
                /// <summary>
                /// Frame identifier flashed back to
                /// </summary>
                public uint flashbackFrameIdentifier;  
                /// <summary>
                /// Session time flashed back to
                /// </summary>
                public float flashbackSessionTime;       
            };

            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            public struct Buttons
            {
                /// <summary>
                /// Bit flags specifying which buttons are being pressed currently - see appendices
                /// </summary>
                public uint buttonStatus;              
            };

            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            public struct Overtake
            {
                /// <summary>
                /// Vehicle index of the vehicle overtaking
                /// </summary>
                public byte overtakingVehicleIdx;
                /// <summary>
                /// Vehicle index of the vehicle being overtaken
                /// </summary>
                public byte beingOvertakenVehicleIdx;   
            };

            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            public struct SafetyCar
            {
                /// <summary>
                /// Safety car type - 
                /// 0 = No Safety Car,
                /// 1 = Full Safety Car,
                /// 2 = Virtual Safety Car,
                /// 3 = Formation Lap Safety Car
                /// </summary>
                public byte safetyCarType;

                /// <summary>
                /// Event type - 
                /// 0 = Deployed,
                /// 1 = Returning,
                /// 2 = Returned,
                /// 3 = Resume Race
                /// </summary>
                public byte eventType;
            };

            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            public struct Collision
            {
                /// <summary>
                /// Vehicle index of the first vehicle involved in the collision
                /// </summary>
                public byte vehicle1Idx;

                /// <summary>
                /// Vehicle index of the second vehicle involved in the collision
                /// </summary>
                public byte vehicle2Idx;
            };
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct PacketEventData
        {
            // Valid event strings
            public const string cs_sessionStartedEventCode = "SSTA";
            public const string cs_sessionEndedEventCode = "SEND";
            public const string cs_fastestLapEventCode = "FTLP";
            public const string cs_retirementEventCode = "RTMT";
            public const string cs_drsEnabledEventCode = "DRSE";
            public const string cs_drsDisabledEventCode = "DRSD";
            public const string cs_teamMateInPitsEventCode = "TMPT";
            public const string cs_chequeredFlagEventCode = "CHQF";
            public const string cs_raceWinnerEventCode = "RCWN";
            public const string cs_penaltyEventCode = "PENA";
            public const string cs_speedTrapEventCode = "SPTP";
            public const string cs_startLightsEventCode = "STLG";
            public const string cs_lightsOutEventCode = "LGOT";
            public const string cs_driveThroughServedEventCode = "DTSV";
            public const string cs_stopGoServedEventCode = "SGSV";
            public const string cs_flashbackEventCode = "FLBK";
            public const string cs_buttonStatusEventCode = "BUTN";
            public const string cs_redFlagEventCode = "RDFL";
            public const string cs_overtakeEventCode = "OVTK";
            public const string cs_safetyCarEventCode = "SCAR";
            public const string cs_collisionEventCode = "COLL";

            public PacketHeader m_header;               // Header

            // Packet specific data

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = cs_eventStringCodeLen)]
            public byte[] m_eventStringCode;       // Event string code
            public EventDataDetails m_eventDetails;                                 // Event details - should be interpreted differently for each type
        };


        //-----------------------------------------------------------------------------
        // Participants - 1284 bytes
        //-----------------------------------------------------------------------------

        // RGB value of a colour
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct LiveryColour
        {
            byte red;
            byte green;
            byte blue;
        };

        //-----------------------------------------------------------------------------
        // Data about one participant
        //-----------------------------------------------------------------------------
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ParticipantData
        {
            /// <summary>
            /// Whether the vehicle is AI (1) or Human (0) controlled
            /// </summary>
            public byte m_aiControlled;

            /// <summary>
            /// Driver id - see appendix, 255 if network human
            /// </summary>
            public byte m_driverId;

            /// <summary>
            /// Network id - unique identifier for network players
            /// </summary>
            public byte m_networkId;

            /// <summary>
            /// Team id - see appendix
            /// </summary>
            public byte m_teamId;

            /// <summary>
            /// My team flag - 1 = My Team, 0 = otherwise
            /// </summary>
            public byte m_myTeam;

            /// <summary>
            /// Race number of the car
            /// </summary>
            public byte m_raceNumber;

            /// <summary>
            /// Nationality of the driver
            /// </summary>
            public byte m_nationality;

            /// <summary>
            /// Name of participant in UTF-8 format – null terminated
            /// Will be truncated with ... (U+2026) if too long
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = cs_maxParticipantNameLen)]
            public string m_name;

            /// <summary>
            /// The player's UDP setting, 0 = restricted, 1 = public
            /// </summary>
            public byte m_yourTelemetry;

            /// <summary>
            /// The player's show online names setting, 0 = off, 1 = on
            /// </summary>
            public byte m_showOnlineNames;

            /// <summary>
            /// F1 World tech level
            /// </summary>
            public ushort m_techLevel;

            /// <summary>
            /// 1 = Steam, 3 = PlayStation, 4 = Xbox, 6 = Origin, 255 = unknown
            /// </summary>
            public byte m_platform;

            /// <summary>
            /// Number of colours valid for this car
            /// </summary>
            public byte m_numColours;

            /// <summary>
            /// Colours for the car
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public LiveryColour[] m_liveryColours;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct PacketParticipantsData
        {
            /// <summary>
            /// Header
            /// </summary>
            public PacketHeader m_header;

            /// <summary>
            /// Number of active cars in the data - should match number of cars on HUD
            /// </summary>
            public byte m_numActiveCars;            
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = cs_maxNumCarsInUDPData)]
            public ParticipantData[] m_participants;
        };


        //-----------------------------------------------------------------------------
        // Car Setups - 1133 bytes
        //-----------------------------------------------------------------------------

        //-----------------------------------------------------------------------------
        // Data about one car setup
        //-----------------------------------------------------------------------------
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct CarSetupData
        {
            /// <summary>
            /// Front wing aero
            /// </summary>
            public byte m_frontWing;

            /// <summary>
            /// Rear wing aero
            /// </summary>
            public byte m_rearWing;

            /// <summary>
            /// Differential adjustment on throttle (percentage)
            /// </summary>
            public byte m_onThrottle;

            /// <summary>
            /// Differential adjustment off throttle (percentage)
            /// </summary>
            public byte m_offThrottle;

            /// <summary>
            /// Front camber angle (suspension geometry)
            /// </summary>
            public float m_frontCamber;

            /// <summary>
            /// Rear camber angle (suspension geometry)
            /// </summary>
            public float m_rearCamber;

            /// <summary>
            /// Front toe angle (suspension geometry)
            /// </summary>
            public float m_frontToe;

            /// <summary>
            /// Rear toe angle (suspension geometry)
            /// </summary>
            public float m_rearToe;

            /// <summary>
            /// Front suspension
            /// </summary>
            public byte m_frontSuspension;

            /// <summary>
            /// Rear suspension
            /// </summary>
            public byte m_rearSuspension;

            /// <summary>
            /// Front anti-roll bar
            /// </summary>
            public byte m_frontAntiRollBar;

            /// <summary>
            /// Rear anti-roll bar
            /// </summary>
            public byte m_rearAntiRollBar;

            /// <summary>
            /// Front ride height
            /// </summary>
            public byte m_frontSuspensionHeight;

            /// <summary>
            /// Rear ride height
            /// </summary>
            public byte m_rearSuspensionHeight;

            /// <summary>
            /// Brake pressure (percentage)
            /// </summary>
            public byte m_brakePressure;

            /// <summary>
            /// Brake bias (percentage)
            /// </summary>
            public byte m_brakeBias;

            /// <summary>
            /// Engine braking (percentage)
            /// </summary>
            public byte m_engineBraking;

            /// <summary>
            /// Rear left tyre pressure (PSI)
            /// </summary>
            public float m_rearLeftTyrePressure;

            /// <summary>
            /// Rear right tyre pressure (PSI)
            /// </summary>
            public float m_rearRightTyrePressure;

            /// <summary>
            /// Front left tyre pressure (PSI)
            /// </summary>
            public float m_frontLeftTyrePressure;

            /// <summary>
            /// Front right tyre pressure (PSI)
            /// </summary>
            public float m_frontRightTyrePressure;

            /// <summary>
            /// Ballast
            /// </summary>
            public byte m_ballast;

            /// <summary>
            /// Fuel load
            /// </summary>
            public float m_fuelLoad;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct PacketCarSetupData
        {
            /// <summary>
            /// Header
            /// </summary>
            public PacketHeader m_header;

            /// <summary>
            /// Packet specific data
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = cs_maxNumCarsInUDPData)]
            public CarSetupData[] m_carSetupData;

            /// <summary>
            /// Value of front wing after next pit stop - player only
            /// </summary>
            public float m_nextFrontWingValue;   
        };


        //-----------------------------------------------------------------------------
        // Car Telemetry - 1352 bytes
        //-----------------------------------------------------------------------------

        //-----------------------------------------------------------------------------
        // Telemetry data for one car
        //-----------------------------------------------------------------------------
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct CarTelemetryData
        {
            /// <summary>
            /// Speed of car in kilometres per hour
            /// </summary>
            public ushort m_speed;

            /// <summary>
            /// Amount of throttle applied (0.0 to 1.0)
            /// </summary>
            public float m_throttle;

            /// <summary>
            /// Steering (-1.0 (full lock left) to 1.0 (full lock right))
            /// </summary>
            public float m_steer;

            /// <summary>
            /// Amount of brake applied (0.0 to 1.0)
            /// </summary>
            public float m_brake;

            /// <summary>
            /// Amount of clutch applied (0 to 100)
            /// </summary>
            public byte m_clutch;

            /// <summary>
            /// Gear selected (1-8, N=0, R=-1)
            /// </summary>
            public sbyte m_gear;

            /// <summary>
            /// Engine RPM
            /// </summary>
            public ushort m_engineRPM;

            /// <summary>
            /// 0 = off, 1 = on
            /// </summary>
            public byte m_drs;

            /// <summary>
            /// Rev lights indicator (percentage)
            /// </summary>
            public byte m_revLightsPercent;

            /// <summary>
            /// Rev lights (bit 0 = leftmost LED, bit 14 = rightmost LED)
            /// </summary>
            public ushort m_revLightsBitValue;

            /// <summary>
            /// Brakes temperature (celsius)
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public ushort[] m_brakesTemperature;

            /// <summary>
            /// Tyres surface temperature (celsius)
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] m_tyresSurfaceTemperature;

            /// <summary>
            /// Tyres inner temperature (celsius)
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] m_tyresInnerTemperature;

            /// <summary>
            /// Engine temperature (celsius)
            /// </summary>
            public ushort m_engineTemperature;

            /// <summary>
            /// Tyre pressure (PSI)
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public float[] m_tyresPressure;

            /// <summary>
            /// Driving surface, see appendices
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] m_surfaceType;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct PacketCarTelemetryData
        {
            /// <summary>
            /// Header
            /// </summary>
            public PacketHeader m_header;

            /// <summary>
            /// Data for all cars on track
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = cs_maxNumCarsInUDPData)]
            public CarTelemetryData[] m_carTelemetryData;

            /// <summary>
            /// Index of MFD panel open - 
            /// 255 = MFD closed,
            /// Single player, race – 0 = Car setup, 1 = Pits,
            /// 2 = Damage, 3 = Engine, 4 = Temperatures,
            /// May vary depending on game mode
            /// </summary>
            public byte m_mfdPanelIndex;

            /// <summary>
            /// Index of MFD panel open for the secondary player - 
            /// See above
            /// </summary>
            public byte m_mfdPanelIndexSecondaryPlayer;

            /// <summary>
            /// Suggested gear for the player (1-8), 0 if no gear suggested
            /// </summary>
            public sbyte m_suggestedGear;
        };


        //-----------------------------------------------------------------------------
        // Car Status - 1239 bytes
        //-----------------------------------------------------------------------------

        //-----------------------------------------------------------------------------
        // Car status data for one car
        //-----------------------------------------------------------------------------
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct CarStatusData
        {
            /// <summary>
            /// Traction control - 0 = off, 1 = medium, 2 = full
            /// </summary>
            public byte m_tractionControl;

            /// <summary>
            /// 0 (off) - 1 (on)
            /// </summary>
            public byte m_antiLockBrakes;

            /// <summary>
            /// Fuel mix - 0 = lean, 1 = standard, 2 = rich, 3 = max
            /// </summary>
            public byte m_fuelMix;

            /// <summary>
            /// Front brake bias (percentage)
            /// </summary>
            public byte m_frontBrakeBias;

            /// <summary>
            /// Pit limiter status - 0 = off, 1 = on
            /// </summary>
            public byte m_pitLimiterStatus;

            /// <summary>
            /// Current fuel mass
            /// </summary>
            public float m_fuelInTank;

            /// <summary>
            /// Fuel capacity
            /// </summary>
            public float m_fuelCapacity;

            /// <summary>
            /// Fuel remaining in terms of laps (value on MFD)
            /// </summary>
            public float m_fuelRemainingLaps;

            /// <summary>
            /// Cars max RPM, point of rev limiter
            /// </summary>
            public ushort m_maxRPM;

            /// <summary>
            /// Cars idle RPM
            /// </summary>
            public ushort m_idleRPM;

            /// <summary>
            /// Maximum number of gears
            /// </summary>
            public byte m_maxGears;

            /// <summary>
            /// 0 = not allowed, 1 = allowed
            /// </summary>
            public byte m_drsAllowed;

            /// <summary>
            /// 0 = DRS not available, non-zero - DRS will be available in [X] metres
            /// </summary>
            public ushort m_drsActivationDistance;

            /// <summary>
            /// F1 Modern - 16 = C5, 17 = C4, 18 = C3, 19 = C2, 20 = C1, 21 = C0, 22 = C6, 7 = inter, 8 = wet
            /// F1 Classic - 9 = dry, 10 = wet
            /// F2 – 11 = super soft, 12 = soft, 13 = medium, 14 = hard, 15 = wet
            /// </summary>
            public byte m_actualTyreCompound;

            /// <summary>
            /// F1 visual (can be different from actual compound)
            /// 16 = soft, 17 = medium, 18 = hard, 7 = inter, 8 = wet
            /// F1 Classic – same as above
            /// F2 ‘20, 15 = wet, 19 – super soft, 20 = soft, 21 = medium, 22 = hard
            /// </summary>
            public byte m_visualTyreCompound;

            /// <summary>
            /// Age in laps of the current set of tyres
            /// </summary>
            public byte m_tyresAgeLaps;

            /// <summary>
            /// -1 = invalid/unknown, 0 = none, 1 = green, 2 = blue, 3 = yellow
            /// </summary>
            public sbyte m_vehicleFIAFlags;

            /// <summary>
            /// Engine power output of ICE (W)
            /// </summary>
            public float m_enginePowerICE;

            /// <summary>
            /// Engine power output of MGU-K (W)
            /// </summary>
            public float m_enginePowerMGUK;

            /// <summary>
            /// ERS energy store in Joules
            /// </summary>
            public float m_ersStoreEnergy;

            /// <summary>
            /// ERS deployment mode, 0 = none, 1 = medium, 2 = hotlap, 3 = overtake
            /// </summary>
            public byte m_ersDeployMode;

            /// <summary>
            /// ERS energy harvested this lap by MGU-K
            /// </summary>
            public float m_ersHarvestedThisLapMGUK;

            /// <summary>
            /// ERS energy harvested this lap by MGU-H
            /// </summary>
            public float m_ersHarvestedThisLapMGUH;

            /// <summary>
            /// ERS energy deployed this lap
            /// </summary>
            public float m_ersDeployedThisLap;

            /// <summary>
            /// Whether the car is paused in a network game
            /// </summary>
            public byte m_networkPaused;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct PacketCarStatusData
        {
            /// <summary>
            /// Header
            /// </summary>
            public PacketHeader m_header;

            /// <summary>
            /// data for all cars on track
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = cs_maxNumCarsInUDPData)]
            public CarStatusData[] m_carStatusData;      
        };


        //-----------------------------------------------------------------------------
        // Final Classification - 1042 bytes
        //-----------------------------------------------------------------------------

        //-----------------------------------------------------------------------------
        // Data about one participant's final results
        //-----------------------------------------------------------------------------
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct FinalClassificationData
        {
            /// <summary>
            /// Finishing position
            /// </summary>
            public byte m_position;

            /// <summary>
            /// Number of laps completed
            /// </summary>
            public byte m_numLaps;

            /// <summary>
            /// Grid position of the car
            /// </summary>
            public byte m_gridPosition;

            /// <summary>
            /// Number of points scored
            /// </summary>
            public byte m_points;

            /// <summary>
            /// Number of pit stops made
            /// </summary>
            public byte m_numPitStops;

            /// <summary>
            /// Result status - 0 = invalid, 1 = inactive, 2 = active, 3 = finished, 4 = didnotfinish, 5 = disqualified, 6 = not classified, 7 = retired
            /// </summary>
            public byte m_resultStatus;

            /// <summary>
            /// Result reason - 0 = invalid, 1 = retired, 2 = finished, 3 = terminal damage, 4 = inactive, 5 = not enough laps completed, 6 = black flagged
            /// 7 = red flagged, 8 = mechanical failure, 9 = session skipped, 10 = session simulated
            /// </summary>
            public byte m_resultReason;

            /// <summary>
            /// Best lap time of the session in milliseconds
            /// </summary>
            public uint m_bestLapTimeInMS;

            /// <summary>
            /// Total race time in seconds without penalties
            /// </summary>
            public double m_totalRaceTime;

            /// <summary>
            /// Total penalties accumulated in seconds
            /// </summary>
            public byte m_penaltiesTime;

            /// <summary>
            /// Number of penalties applied to this driver
            /// </summary>
            public byte m_numPenalties;

            /// <summary>
            /// Number of tyres stints up to maximum
            /// </summary>
            public byte m_numTyreStints;

            /// <summary>
            /// Actual tyres used by this driver
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = cs_maxTyreStints)]
            public byte[] m_tyreStintsActual;

            /// <summary>
            /// Visual tyres used by this driver
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = cs_maxTyreStints)]
            public byte[] m_tyreStintsVisual;

            /// <summary>
            /// The lap number stints end on
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = cs_maxTyreStints)]
            public byte[] m_tyreStintsEndLaps;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct PacketFinalClassificationData
        {
            /// <summary>
            /// Header
            /// </summary>
            public PacketHeader m_header;

            /// <summary>
            /// Number of cars in the final classification
            /// </summary>
            public byte m_numCars;          
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = cs_maxNumCarsInUDPData)]
            public FinalClassificationData[] m_classificationData;
        };


        //-----------------------------------------------------------------------------
        // Lobby Info - 954 bytes
        //-----------------------------------------------------------------------------

        //-----------------------------------------------------------------------------
        // Data about one participant
        //-----------------------------------------------------------------------------
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct LobbyInfoData
        {
            /// <summary>
            /// Whether the vehicle is AI (1) or Human (0) controlled
            /// </summary>
            public byte m_aiControlled;

            /// <summary>
            /// Team id - see appendix (255 if no team currently selected)
            /// </summary>
            public byte m_teamId;

            /// <summary>
            /// Nationality of the driver
            /// </summary>
            public byte m_nationality;

            /// <summary>
            /// 1 = Steam, 3 = PlayStation, 4 = Xbox, 6 = Origin, 255 = unknown
            /// </summary>
            public byte m_platform;

            /// <summary>
            /// Name of participant in UTF-8 format – null terminated
            /// Will be truncated with ... (U+2026) if too long
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = cs_maxParticipantNameLen)]
            public string m_name;

            /// <summary>
            /// Car number of the player
            /// </summary>
            public byte m_carNumber;

            /// <summary>
            /// The player's UDP setting, 0 = restricted, 1 = public
            /// </summary>
            public byte m_yourTelemetry;

            /// <summary>
            /// The player's show online names setting, 0 = off, 1 = on
            /// </summary>
            public byte m_showOnlineNames;

            /// <summary>
            /// F1 World tech level
            /// </summary>
            public ushort m_techLevel;

            /// <summary>
            /// 0 = not ready, 1 = ready, 2 = spectating
            /// </summary>
            public byte m_readyStatus;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct PacketLobbyInfoData
        {
            /// <summary>
            /// Header
            /// </summary>
            public PacketHeader m_header;

            /// <summary>
            /// // Number of players in the lobby data
            /// </summary>
            public byte m_numPlayers;               
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = cs_maxNumCarsInUDPData)]
            public LobbyInfoData[] m_lobbyPlayers;
        };


        //-----------------------------------------------------------------------------
        // Car Damage - 1041 bytes
        //-----------------------------------------------------------------------------

        //-----------------------------------------------------------------------------
        // Car damage data for one car
        //-----------------------------------------------------------------------------
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct CarDamageData
        {
            /// <summary>
            /// Tyre wear (percentage)
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public float[] m_tyresWear;

            /// <summary>
            /// Tyre damage (percentage)
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] m_tyresDamage;

            /// <summary>
            /// Brakes damage (percentage)
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] m_brakesDamage;

            /// <summary>
            /// Tyre blisters value (percentage)
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] m_tyreBlisters;

            /// <summary>
            /// Front left wing damage (percentage)
            /// </summary>
            public byte m_frontLeftWingDamage;

            /// <summary>
            /// Front right wing damage (percentage)
            /// </summary>
            public byte m_frontRightWingDamage;

            /// <summary>
            /// Rear wing damage (percentage)
            /// </summary>
            public byte m_rearWingDamage;

            /// <summary>
            /// Floor damage (percentage)
            /// </summary>
            public byte m_floorDamage;

            /// <summary>
            /// Diffuser damage (percentage)
            /// </summary>
            public byte m_diffuserDamage;

            /// <summary>
            /// Sidepod damage (percentage)
            /// </summary>
            public byte m_sidepodDamage;

            /// <summary>
            /// Indicator for DRS fault, 0 = OK, 1 = fault
            /// </summary>
            public byte m_drsFault;

            /// <summary>
            /// Indicator for ERS fault, 0 = OK, 1 = fault
            /// </summary>
            public byte m_ersFault;

            /// <summary>
            /// Gear box damage (percentage)
            /// </summary>
            public byte m_gearBoxDamage;

            /// <summary>
            /// Engine damage (percentage)
            /// </summary>
            public byte m_engineDamage;

            /// <summary>
            /// Engine wear MGU-H (percentage)
            /// </summary>
            public byte m_engineMGUHWear;

            /// <summary>
            /// Engine wear ES (percentage)
            /// </summary>
            public byte m_engineESWear;

            /// <summary>
            /// Engine wear CE (percentage)
            /// </summary>
            public byte m_engineCEWear;

            /// <summary>
            /// Engine wear ICE (percentage)
            /// </summary>
            public byte m_engineICEWear;

            /// <summary>
            /// Engine wear MGU-K (percentage)
            /// </summary>
            public byte m_engineMGUKWear;

            /// <summary>
            /// Engine wear TC (percentage)
            /// </summary>
            public byte m_engineTCWear;

            /// <summary>
            /// Engine blown, 0 = OK, 1 = fault
            /// </summary>
            public byte m_engineBlown;

            /// <summary>
            /// Engine seized, 0 = OK, 1 = fault
            /// </summary>
            public byte m_engineSeized;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct PacketCarDamageData
        {
            /// <summary>
            /// Header
            /// </summary>
            public PacketHeader m_header;

            /// <summary>
            /// data for all cars on track
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = cs_maxNumCarsInUDPData)]
            public CarDamageData[] m_carDamageData;      
        };


        //-----------------------------------------------------------------------------
        // Session History - 1460 bytes
        //-----------------------------------------------------------------------------

        const int cs_maxNumLapsInHistory = 100;

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct LapHistoryData
        {
            /// <summary>
            /// Lap time in milliseconds
            /// </summary>
            public uint m_lapTimeInMS;

            /// <summary>
            /// Sector 1 milliseconds part
            /// </summary>
            public ushort m_sector1TimeMSPart;

            /// <summary>
            /// Sector 1 whole minute part
            /// </summary>
            public byte m_sector1TimeMinutesPart;

            /// <summary>
            /// Sector 2 time milliseconds part
            /// </summary>
            public ushort m_sector2TimeMSPart;

            /// <summary>
            /// Sector 2 whole minute part
            /// </summary>
            public byte m_sector2TimeMinutesPart;

            /// <summary>
            /// Sector 3 time milliseconds part
            /// </summary>
            public ushort m_sector3TimeMSPart;

            /// <summary>
            /// Sector 3 whole minute part
            /// </summary>
            public byte m_sector3TimeMinutesPart;

            /// <summary>
            /// 0x01 bit set-lap valid, 0x02 bit set-sector 1 valid, 0x04 bit set-sector 2 valid, 0x08 bit set-sector 3 valid
            /// </summary>
            public byte m_lapValidBitFlags;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct TyreStintHistoryData
        {
            /// <summary>
            /// Lap the tyre usage ends on (255 if current tyre)
            /// </summary>
            public byte m_endLap;

            /// <summary>
            /// Actual tyres used by this driver
            /// </summary>
            public byte m_tyreActualCompound;

            /// <summary>
            /// Visual tyres used by this driver
            /// </summary>
            public byte m_tyreVisualCompound;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct PacketSessionHistoryData
        {
            /// <summary>
            /// Header
            /// </summary>
            public PacketHeader m_header;               

            /// <summary>
            /// Index of the car this lap data relates to
            /// </summary>
            public byte m_carIdx;

            /// <summary>
            /// Number of laps in the data (including current partial lap)
            /// </summary>
            public byte m_numLaps;

            /// <summary>
            /// Number of tyre stints in the data
            /// </summary>
            public byte m_numTyreStints;

            /// <summary>
            /// Lap the best lap time was achieved on
            /// </summary>
            public byte m_bestLapTimeLapNum;

            /// <summary>
            /// Lap the best Sector 1 time was achieved on
            /// </summary>
            public byte m_bestSector1LapNum;

            /// <summary>
            /// Lap the best Sector 2 time was achieved on
            /// </summary>
            public byte m_bestSector2LapNum;

            /// <summary>
            /// Lap the best Sector 3 time was achieved on
            /// </summary>
            public byte m_bestSector3LapNum;


            [MarshalAs(UnmanagedType.ByValArray, SizeConst = cs_maxNumLapsInHistory)]
            public LapHistoryData[] m_lapHistoryData;   // 100 laps of data max
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = cs_maxTyreStints)]
            public TyreStintHistoryData[] m_tyreStintsHistoryData;
        };


        //-----------------------------------------------------------------------------
        // Tyre Sets - 231 bytes
        //-----------------------------------------------------------------------------

        //-----------------------------------------------------------------------------
        // Data about one tyre set
        //-----------------------------------------------------------------------------
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct TyreSetData
        {
            /// <summary>
            /// Actual tyre compound used
            /// </summary>
            public byte m_actualTyreCompound;

            /// <summary>
            /// Visual tyre compound used
            /// </summary>
            public byte m_visualTyreCompound;

            /// <summary>
            /// Tyre wear (percentage)
            /// </summary>
            public byte m_wear;

            /// <summary>
            /// Whether this set is currently available
            /// </summary>
            public byte m_available;

            /// <summary>
            /// Recommended session for tyre set, see appendix
            /// </summary>
            public byte m_recommendedSession;

            /// <summary>
            /// Laps left in this tyre set
            /// </summary>
            public byte m_lifeSpan;

            /// <summary>
            /// Max number of laps recommended for this compound
            /// </summary>
            public byte m_usableLife;

            /// <summary>
            /// Lap delta time in milliseconds compared to fitted set
            /// </summary>
            public short m_lapDeltaTime;

            /// <summary>
            /// Whether the set is fitted or not
            /// </summary>
            public byte m_fitted;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct PacketTyreSetsData
        {
            /// <summary>
            /// Header
            /// </summary>
            public PacketHeader m_header;

            /// <summary>
            /// Index of the car this data relates to
            /// </summary>
            public byte m_carIdx;

            /// <summary>
            /// Array of tyre set data, including 13 dry and 7 wet tyre sets
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = cs_maxNumTyreSets)]
            public TyreSetData[] m_tyreSetData;

            /// <summary>
            /// Index into the array of fitted tyre sets
            /// </summary>
            public byte m_fittedIdx;
        };


        //-----------------------------------------------------------------------------
        // Motion Ex - 273 bytes
        //-----------------------------------------------------------------------------

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct PacketMotionExData
        {
            /// <summary>
            /// Header
            /// </summary>
            public PacketHeader m_header;

            /// <summary>
            /// Array of suspension position for each wheel (RL, RR, FL, FR)
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public float[] m_suspensionPosition;

            /// <summary>
            /// Array of suspension velocity for each wheel (RL, RR, FL, FR)
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public float[] m_suspensionVelocity;

            /// <summary>
            /// Array of suspension acceleration for each wheel (RL, RR, FL, FR)
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public float[] m_suspensionAcceleration;

            /// <summary>
            /// Speed of each wheel
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public float[] m_wheelSpeed;

            /// <summary>
            /// Slip ratio for each wheel
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public float[] m_wheelSlipRatio;

            /// <summary>
            /// Slip angles for each wheel
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public float[] m_wheelSlipAngle;

            /// <summary>
            /// Lateral forces for each wheel
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public float[] m_wheelLatForce;

            /// <summary>
            /// Longitudinal forces for each wheel
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public float[] m_wheelLongForce;

            /// <summary>
            /// Height of centre of gravity above ground
            /// </summary>
            public float m_heightOfCOGAboveGround;

            /// <summary>
            /// Velocity in local space X - metres/s
            /// </summary>
            public float m_localVelocityX;

            /// <summary>
            /// Velocity in local space Y
            /// </summary>
            public float m_localVelocityY;

            /// <summary>
            /// Velocity in local space Z
            /// </summary>
            public float m_localVelocityZ;

            /// <summary>
            /// Angular velocity x-component - radians/s
            /// </summary>
            public float m_angularVelocityX;

            /// <summary>
            /// Angular velocity y-component
            /// </summary>
            public float m_angularVelocityY;

            /// <summary>
            /// Angular velocity z-component
            /// </summary>
            public float m_angularVelocityZ;

            /// <summary>
            /// Angular acceleration x-component - radians/s/s
            /// </summary>
            public float m_angularAccelerationX;

            /// <summary>
            /// Angular acceleration y-component
            /// </summary>
            public float m_angularAccelerationY;

            /// <summary>
            /// Angular acceleration z-component
            /// </summary>
            public float m_angularAccelerationZ;

            /// <summary>
            /// Current front wheels angle in radians
            /// </summary>
            public float m_frontWheelsAngle;

            /// <summary>
            /// Vertical forces for each wheel
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public float[] m_wheelVertForce;

            /// <summary>
            /// Front plank edge height above road surface
            /// </summary>
            public float m_frontAeroHeight;

            /// <summary>
            /// Rear plank edge height above road surface
            /// </summary>
            public float m_rearAeroHeight;

            /// <summary>
            /// Roll angle of the front suspension
            /// </summary>
            public float m_frontRollAngle;

            /// <summary>
            /// Roll angle of the rear suspension
            /// </summary>
            public float m_rearRollAngle;

            /// <summary>
            /// Yaw angle of the chassis relative to the direction of motion - radians
            /// </summary>
            public float m_chassisYaw;

            /// <summary>
            /// Pitch angle of the chassis relative to the direction of motion - radians
            /// </summary>
            public float m_chassisPitch;

            /// <summary>
            /// Camber of each wheel in radians
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public float[] m_wheelCamber;

            /// <summary>
            /// Camber gain for each wheel in radians, difference between active camber and dynamic camber
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public float[] m_wheelCamberGain;
        };


        //-----------------------------------------------------------------------------
        // Time Trial - 101 bytes
        //-----------------------------------------------------------------------------

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct TimeTrialDataSet
        {
            /// <summary>
            /// Index of the car this data relates to
            /// </summary>
            public byte m_carIdx;

            /// <summary>
            /// Team id - see appendix
            /// </summary>
            public byte m_teamId;

            /// <summary>
            /// Lap time in milliseconds
            /// </summary>
            public uint m_lapTimeInMS;

            /// <summary>
            /// Sector 1 time in milliseconds
            /// </summary>
            public uint m_sector1TimeInMS;

            /// <summary>
            /// Sector 2 time in milliseconds
            /// </summary>
            public uint m_sector2TimeInMS;

            /// <summary>
            /// Sector 3 time in milliseconds
            /// </summary>
            public uint m_sector3TimeInMS;

            /// <summary>
            /// Traction control - 0 = assist off, 1 = assist on
            /// </summary>
            public byte m_tractionControl;

            /// <summary>
            /// Gearbox assist - 0 = assist off, 1 = assist on
            /// </summary>
            public byte m_gearboxAssist;

            /// <summary>
            /// Anti-lock brakes - 0 = assist off, 1 = assist on
            /// </summary>
            public byte m_antiLockBrakes;

            /// <summary>
            /// Equal car performance - 0 = Realistic, 1 = Equal
            /// </summary>
            public byte m_equalCarPerformance;

            /// <summary>
            /// Custom setup - 0 = No, 1 = Yes
            /// </summary>
            public byte m_customSetup;

            /// <summary>
            /// Valid - 0 = invalid, 1 = valid
            /// </summary>
            public byte m_valid;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct PacketTimeTrialData
        {
            /// <summary>
            /// Header
            /// </summary>
            public PacketHeader m_header;

            /// <summary>
            /// Player session best data set
            /// </summary>
            public TimeTrialDataSet m_playerSessionBestDataSet;

            /// <summary>
            /// Personal best data set
            /// </summary>
            public TimeTrialDataSet m_personalBestDataSet;

            /// <summary>
            /// Rival data set
            /// </summary>
            public TimeTrialDataSet m_rivalDataSet;
        };


        //-----------------------------------------------------------------------------
        // Lap Positions - 1131 bytes
        //-----------------------------------------------------------------------------

        //-----------------------------------------------------------------------------
        // Packet to send UDP data about the lap positions in a session. It details
        // the positions of all the drivers at the start of each lap
        //-----------------------------------------------------------------------------
        const int cs_maxNumLapsInLapPositionsHistoryPacket = 50;

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct PacketLapPositionsData
        {
            /// <summary>
            /// Header
            /// </summary>
            public PacketHeader m_header;

            /// <summary>
            /// Number of laps in the data
            /// </summary>
            public byte m_numLaps;

            /// <summary>
            /// Index of the lap where the data starts, 0 indexed
            /// </summary>
            public byte m_lapStart;

            /// <summary>
            /// Array holding the position of the car in a given lap, 0 if no record
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = cs_maxNumLapsInLapPositionsHistoryPacket * cs_maxNumCarsInUDPData)]
            public byte[,] m_positionForVehicleIdx;
        };

        public static readonly Dictionary<int, string> DriversID = new Dictionary<int, string>
        {
            {0,     "Carlos Sainz"},
            {2,     "Daniel Ricciardo"},
            {3,     "Fernando Alonso"},
            {4,     "Felipe Massa"},
            {7,     "Lewis Hamilton"},
            {9,     "Max Verstappen"},
            {10     ,"Nico Hülkenburg"},
            {11     ,"Kevin Magnussen"},
            {14     ,"Sergio Pérez"},
            {15     ,"Valtteri Bottas"},
            {17     ,"Esteban Ocon"},
            {19     ,"Lance Stroll"},
            {20     ,"Arron Barnes"},
            {21     ,"Martin Giles"},
            {22     ,"Alex Murray"},
            {23     ,"Lucas Roth"},
            {24     ,"Igor Correia"},
            {25     ,"Sophie Levasseur"},
            {26     ,"Jonas Schiffer"},
            {27     ,"Alain Forest"},
            {28     ,"Jay Letourneau"},
            {29     ,"Esto Saari"},
            {30     ,"Yasar Atiyeh"},
            {31     ,"Callisto Calabresi"},
            {32     ,"Naota Izumi"},
            {33     ,"Howard Clarke"},
            {34     ,"Lars Kaufmann"},
            {35     ,"Marie Laursen"},
            {36     ,"Flavio Nieves"},
            {38     ,"Klimek Michalski"},
            {39     ,"Santiago Moreno"},
            {62     ,"Alexander Albon"},
            {70     ,"Rashid Nair"},
            {71     ,"Jack Tremblay"},
            {77     ,"Ayrton Senna"},
            {80     ,"Guanyu Zhou"},
            {83     ,"Juan Manuel Correa"},
            {90     ,"Michael Schumacher"},
            {94     ,"Yuki Tsunoda"},
            {102    ,"Aidan Jackson"},
            {109    ,"Jenson Button"},
            {110    ,"David Coulthard"},
            {112    ,"Oscar Piastri"},
            {113    ,"Liam Lawson"},
            {116    ,"Richard Verschoor"},
            {123    ,"Enzo Fittipaldi"},
            {125    ,"Mark Webber"},
            {126    ,"Jacques Villeneuve"},
            {127    ,"Callie Mayer"},
            {132    ,"Logan Sargeant"},
            {136    ,"Jack Doohan"},
            {137    ,"Amaury Cordeel"},
            {138    ,"Dennis Hauger"},
            {145    ,"Zane Maloney"},
            {146    ,"Victor Martins"},
            {147    ,"Oliver Bearman"},
            {148    ,"Jak Crawford"},
            {149    ,"Isack Hadjar"},
            {152    ,"Roman Stanek"},
            {153    ,"Kush Maini"},
            {156    ,"Brendon Leigh"},
            {157    ,"David Tonizza"},
            {164    ,"Joshua Dürksen"},
            {165    ,"Andrea-Kimi Antonelli"},
            {166    ,"Ritomo Miyata"},
            {167    ,"Rafael Villagómez"},
            {168    ,"Zak O’Sullivan"},
            {169    ,"Pepe Marti"},
            {170    ,"Sonny Hayes"},
            {171    ,"Joshua Pearce"},
            {172    ,"Callum Voisin"},
            {173    ,"Matias Zagazeta"},
            {174    ,"Nikola Tsolov"},
            {175    ,"Tim Tramnitz"},
            {185    ,"Luca Cortez"},
        };

        public static readonly Dictionary<int, string> TeamsID = new Dictionary<int, string>
        {
            {0, "Mercedes"},
            {1, "Ferrari"},
            {2, "Red Bull Racing"},
            {3, "Williams"},
            {4, "Aston Martin"},
            {5, "Alpine"},
            {6, "RB"},
            {7, "Haas"},
            {8, "McLaren"},
            {9, "Sauber"},
            {41, "F1 Generic"},
            {104, "F1 Custom Team"},
            {129, "Konnersport"},
            {142, "APXGP ‘24"},
            {154, "APXGP ‘25"},
            {155, "Konnersport ‘24"},
            {158, "Art GP ‘24"},
            {159, "Campos ‘24"},
            {160, "Rodin Motorsport ‘24"},
            {161, "AIX Racing ‘24"},
            {162, "DAMS ‘24"},
            {163, "Hitech ‘24"},
            {164, "MP Motorsport ‘24"},
            {165, "Prema ‘24"},
            {166, "Trident ‘24"},
            {167, "Van Amersfoort Racing ‘24"},
            {168, "Invicta ‘24"},
            {185, "Mercedes ‘24"},
            {186, "Ferrari ‘24"},
            {187, "Red Bull Racing ‘24"},
            {188, "Williams ‘24"},
            {189, "Aston Martin ‘24"},
            {190, "Alpine ‘24"},
            {191, "RB ‘24"},
            {192, "Haas ‘24"},
            {193, "McLaren ‘24"},
            {194, "Sauber ‘24"},
        };

        /// <summary>
        /// string index == ID
        /// </summary>
        public static readonly string[] TrackID = new string[]
        {
            "Melbourne",
            "",
            "Shanghai",
            "Sakhir (Bahrain)",
            "Catalunya",
            "Monaco",
            "Montreal",
            "Silverstone",
            "",
            "Hungaroring",
            "Spa",
            "Monza",
            "Singapore",
            "Suzuka",
            "Abu Dhabi",
            "Texas",
            "Brazil",
            "Austria",
            "",
            "Mexico",
            "Baku (Azerbaijan)",
            "",
            "",
            "",
            "",
            "",
            "Zandvoort",
            "Imola",
            "Jeddah",
            "Miami",
            "Las Vegas",
            "Losail",
            "",
            "",
            "",
            "",
            "",
            "",
            "Silverstone (Reverse)",
            "Austria (Reverse)",
            "Zandvoort (Reverse)",
        };
    }
}
