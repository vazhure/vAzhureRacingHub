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
            public float m_worldPositionX;           // World space X position - metres
            public float m_worldPositionY;           // World space Y position
            public float m_worldPositionZ;           // World space Z position
            public float m_worldVelocityX;           // Velocity in world space X - metres/s
            public float m_worldVelocityY;           // Velocity in world space Y
            public float m_worldVelocityZ;           // Velocity in world space Z
            public short m_worldForwardDirX;         // World space forward X direction (normalised)
            public short m_worldForwardDirY;         // World space forward Y direction (normalised)
            public short m_worldForwardDirZ;         // World space forward Z direction (normalised)
            public short m_worldRightDirX;           // World space right X direction (normalised)
            public short m_worldRightDirY;           // World space right Y direction (normalised)
            public short m_worldRightDirZ;           // World space right Z direction (normalised)
            public float m_gForceLateral;            // Lateral G-Force component
            public float m_gForceLongitudinal;       // Longitudinal G-Force component
            public float m_gForceVertical;           // Vertical G-Force component
            public float m_yaw;                      // Yaw angle in radians
            public float m_pitch;                    // Pitch angle in radians
            public float m_roll;                     // Roll angle in radians
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct PacketMotionData
        {
            public PacketHeader m_header;               // Header

            // Packet specific data
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
            public float m_zoneStart;       // Fraction (0..1) of way through the lap the marshal zone starts
            public sbyte m_zoneFlag;        // -1 = invalid/unknown, 0 = none, 1 = green, 2 = blue, 3 = yellow
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct WeatherForecastSample
        {
            public byte m_sessionType;              // 0 = unknown, see appendix
            public byte m_timeOffset;               // Time in minutes the forecast is for
            public byte m_weather;                  // Weather - 0 = clear, 1 = light cloud, 2 = overcast, 3 = light rain, 4 = heavy rain, 5 = storm
            public sbyte m_trackTemperature;         // Track temp. in degrees celsius
            public sbyte m_trackTemperatureChange;   // Track temp. change - 0 = up, 1 = down, 2 = no change
            public sbyte m_airTemperature;           // Air temp. in degrees celsius
            public sbyte m_airTemperatureChange;     // Air temp. change - 0 = up, 1 = down, 2 = no change
            public byte m_rainPercentage;           // Rain percentage (0-100)
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct PacketSessionData
        {
            public PacketHeader m_header;               // Header

            // Packet specific data
            public byte m_weather;                          // Weather - 0 = clear, 1 = light cloud, 2 = overcast, 3 = light rain, 4 = heavy rain, 5 = storm
            public sbyte m_trackTemperature;                 // Track temp. in degrees celsius
            public sbyte m_airTemperature;                   // Air temp. in degrees celsius
            public byte m_totalLaps;                        // Total number of laps in this race
            public ushort m_trackLength;                      // Track length in metres
            public byte m_sessionType;                      // 0 = unknown, see appendix
            public sbyte m_trackId;                          // -1 for unknown, see appendix
            public byte m_formula;                          // Formula, 0 = F1 Modern, 1 = F1 Classic, 2 = F2, 3 = F1 Generic, 4 = Beta, 6 = Esports, 8 = F1 World, 9 = F1 Elimination
            public ushort m_sessionTimeLeft;                   // Time left in session in seconds
            public ushort m_sessionDuration;                   // Session duration in seconds
            public byte m_pitSpeedLimit;                    // Pit speed limit in kilometres per hour
            public byte m_gamePaused;                       // Whether the game is paused - network game only
            public byte m_isSpectating;                     // Whether the player is spectating
            public byte m_spectatorCarIndex;                // Index of the car being spectated
            public byte m_sliProNativeSupport;              // SLI Pro support, 0 = inactive, 1 = active
            public byte m_numMarshalZones;                  // Number of marshal zones to follow
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = cs_maxMarshalsZonePerLap)]
            public MarshalZone[] m_marshalZones;  // List of marshal zones - max 21
            public byte m_safetyCarStatus;                  // 0 = no safety car, 1 = full, 2 = virtual, 3 = formation lap
            public byte m_networkGame;                      // 0 = offline, 1 = online
            public byte m_numWeatherForecastSamples;        // Number of weather samples to follow
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = cs_maxWeatherForecastSamples)]
            public WeatherForecastSample[] m_weatherForecastSamples;   // Array of weather forecast samples
            public byte m_forecastAccuracy;                 // 0 = Perfect, 1 = Approximate
            public byte m_aiDifficulty;                     // AI difficulty - 0-110
            public uint m_seasonLinkIdentifier;             // Identifier for season - persists across saves
            public uint m_weekendLinkIdentifier;            // Identifier for weekend - persists across saves
            public uint m_sessionLinkIdentifier;            // Identifier for session - persists across saves
            public byte m_pitStopWindowIdealLap;            // Ideal lap to pit on for current strategy (player)
            public byte m_pitStopWindowLatestLap;           // Latest lap to pit on for current strategy (player)
            public byte m_pitStopRejoinPosition;            // Predicted position to rejoin at (player)
            public byte m_steeringAssist;                   // 0 = off, 1 = on
            public byte m_brakingAssist;                    // 0 = off, 1 = low, 2 = medium, 3 = high
            public byte m_gearboxAssist;                    // 1 = manual, 2 = manual & suggested gear, 3 = auto
            public byte m_pitAssist;                        // 0 = off, 1 = on
            public byte m_pitReleaseAssist;                 // 0 = off, 1 = on
            public byte m_ERSAssist;                        // 0 = off, 1 = on
            public byte m_DRSAssist;                        // 0 = off, 1 = on
            public byte m_dynamicRacingLine;                // 0 = off, 1 = corners only, 2 = full
            public byte m_dynamicRacingLineType;            // 0 = 2D, 1 = 3D
            public byte m_gameMode;                         // Game mode id - see appendix
            public byte m_ruleSet;                          // Ruleset - see appendix
            public uint m_timeOfDay;                        // Local time of day - minutes since midnight
            public byte m_sessionLength;                    // 0 = None, 2 = Very ushort, 3 = ushort, 4 = Medium, 5 = Medium Long, 6 = Long, 7 = Full
            public byte m_speedUnitsLeadPlayer;             // 0 = MPH, 1 = KPH
            public byte m_temperatureUnitsLeadPlayer;       // 0 = Celsius, 1 = Fahrenheit
            public byte m_speedUnitsSecondaryPlayer;        // 0 = MPH, 1 = KPH
            public byte m_temperatureUnitsSecondaryPlayer;  // 0 = Celsius, 1 = Fahrenheit
            public byte m_numSafetyCarPeriods;              // Number of safety cars called during session
            public byte m_numVirtualSafetyCarPeriods;       // Number of virtual safety cars called during session
            public byte m_numRedFlagPeriods;                // Number of red flags called during session
            public byte m_equalCarPerformance;              // 0 = Off, 1 = On
            public byte m_recoveryMode;                     // 0 = None, 1 = Flashbacks, 2 = Auto-recovery
            public byte m_flashbackLimit;                   // 0 = Low, 1 = Medium, 2 = High, 3 = Unlimited
            public byte m_surfaceType;                      // 0 = Simplified, 1 = Realistic
            public byte m_lowFuelMode;                      // 0 = Easy, 1 = Hard
            public byte m_raceStarts;                       // 0 = Manual, 1 = Assisted
            public byte m_tyreTemperature;                  // 0 = Surface only, 1 = Surface & Carcass
            public byte m_pitLaneTyreSim;                   // 0 = On, 1 = Off
            public byte m_carDamage;                        // 0 = Off, 1 = Reduced, 2 = Standard, 3 = Simulation
            public byte m_carDamageRate;                    // 0 = Reduced, 1 = Standard, 2 = Simulation
            public byte m_collisions;                       // 0 = Off, 1 = Player-to-Player Off, 2 = On
            public byte m_collisionsOffForFirstLapOnly;     // 0 = Disabled, 1 = Enabled
            public byte m_mpUnsafePitRelease;               // 0 = On, 1 = Off (Multiplayer)
            public byte m_mpOffForGriefing;                 // 0 = Disabled, 1 = Enabled (Multiplayer)
            public byte m_cornerCuttingStringency;          // 0 = Regular, 1 = Strict
            public byte m_parcFermeRules;                   // 0 = Off, 1 = On
            public byte m_pitStopExperience;                // 0 = Automatic, 1 = Broadcast, 2 = Immersive
            public byte m_safetyCar;                        // 0 = Off, 1 = Reduced, 2 = Standard, 3 = Increased
            public byte m_safetyCarExperience;              // 0 = Broadcast, 1 = Immersive
            public byte m_formationLap;                     // 0 = Off, 1 = On
            public byte m_formationLapExperience;           // 0 = Broadcast, 1 = Immersive
            public byte m_redFlags;                         // 0 = Off, 1 = Reduced, 2 = Standard, 3 = Increased
            public byte m_affectsLicenceLevelSolo;          // 0 = Off, 1 = On
            public byte m_affectsLicenceLevelMP;            // 0 = Off, 1 = On
            public byte m_numSessionsInWeekend;             // Number of session in following array
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = cs_maxSessionsInWeekend)]
            public byte[] m_weekendStructure;    // List of session types to show weekend public structure - see appendix for types
            public float m_sector2LapDistanceStart;          // Distance in m around track where sector 2 starts
            public float m_sector3LapDistanceStart;          // Distance in m around track where sector 3 starts
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
            public uint m_lastLapTimeInMS;              // Last lap time in milliseconds
            public uint m_currentLapTimeInMS;           // Current time around the lap in milliseconds
            public ushort m_sector1TimeMSPart;            // Sector 1 time milliseconds part
            public byte m_sector1TimeMinutesPart;       // Sector 1 whole minute part
            public ushort m_sector2TimeMSPart;            // Sector 2 time milliseconds part
            public byte m_sector2TimeMinutesPart;       // Sector 2 whole minute part
            public ushort m_deltaToCarInFrontMSPart;      // Time delta to car in front milliseconds part
            public byte m_deltaToCarInFrontMinutesPart; // Time delta to car in front whole minute part
            public ushort m_deltaToRaceLeaderMSPart;      // Time delta to race leader milliseconds part
            public byte m_deltaToRaceLeaderMinutesPart; // Time delta to race leader whole minute part
            public float m_lapDistance;                  // Distance vehicle is around current lap in metres – could be negative if line hasn’t been crossed yet
            public float m_totalDistance;                // Total distance travelled in session in metres – could be negative if line hasn’t been crossed yet
            public float m_safetyCarDelta;               // Delta in seconds for safety car
            public byte m_carPosition;                  // Car race position
            public byte m_currentLapNum;                // Current lap number
            public byte m_pitStatus;                    // 0 = none, 1 = pitting, 2 = in pit area
            public byte m_numPitStops;                  // Number of pit stops taken in this race
            public byte m_sector;                       // 0 = sector1, 1 = sector2, 2 = sector3
            public byte m_currentLapInvalid;            // Current lap invalid - 0 = valid, 1 = invalid
            public byte m_penalties;                    // Accumulated time penalties in seconds to be added
            public byte m_totalWarnings;                // Accumulated number of warnings issued
            public byte m_cornerCuttingWarnings;        // Accumulated number of corner cutting warnings issued
            public byte m_numUnservedDriveThroughPens;  // Num drive through pens left to serve
            public byte m_numUnservedStopGoPens;        // Num stop go pens left to serve
            public byte m_gridPosition;                 // Grid position the vehicle started the race in
            public byte m_driverStatus;                 // Status of driver - 0 = in garage, 1 = flying lap, 2 = in lap, 3 = out lap, 4 = on track
            public byte m_resultStatus;                 // Result status - 0 = invalid, 1 = inactive, 2 = active, 3 = finished, 4 = didnotfinish, 5 = disqualified, 6 = not classified, 7 = retired
            public byte m_pitLaneTimerActive;           // Pit lane timing, 0 = inactive, 1 = active
            public ushort m_pitLaneTimeInLaneInMS;        // If active, the current time spent in the pit lane in ms
            public ushort m_pitStopTimerInMS;             // Time of the actual pit stop in ms
            public byte m_pitStopShouldServePen;        // Whether the car should serve a penalty at this stop
            public float m_speedTrapFastestSpeed;        // Fastest speed through speed trap for this car in kmph
            public byte m_speedTrapFastestLap;          // Lap no the fastest speed was achieved, 255 = not set
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
                public byte vehicleIdx; // Vehicle index of car achieving fastest lap
                public float lapTime;    // Lap time is in seconds
            };

            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            public struct Retirement
            {
                public byte vehicleIdx; // Vehicle index of car retiring
                public byte reason;     // Result reason - 0 = invalid, 1 = retired, 2 = finished, 3 = terminal damage, 4 = inactive, 5 = not enough laps completed, 6 = black flagged
                                        // 7 = red flagged, 8 = mechanical failure, 9 = session skipped, 10 = session simulated
            };

            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            public struct DRSDisabled
            {
                public byte reason;     // 0 = Wet track, 1 = Safety car deployed, 2 = Red flag, 3 = Min lap not reached
            };

            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            public struct TeamMateInPits
            {
                public byte vehicleIdx; // Vehicle index of team mate
            };

            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            public struct RaceWinner
            {
                public byte vehicleIdx; // Vehicle index of the race winner
            };

            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            public struct Penalty
            {
                public byte penaltyType;        // Penalty type – see Appendices
                public byte infringementType;   // Infringement type – see Appendices
                public byte vehicleIdx;         // Vehicle index of the car the penalty is applied to
                public byte otherVehicleIdx;    // Vehicle index of the other car involved
                public byte time;               // Time gained, or time spent doing action in seconds
                public byte lapNum;// Lap the penalty occurred on
                public byte placesGained;       // Number of places gained by this
            };

            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            public struct SpeedTrap
            {
                public byte vehicleIdx;                 // Vehicle index of the vehicle triggering speed trap
                public float speed;                      // Top speed achieved in kilometres per hour
                public byte isOverallFastestInSession;  // Overall fastest speed in session = 1, otherwise 0
                public byte isDriverFastestInSession;   // Fastest speed for driver in session = 1, otherwise 0
                public byte fastestVehicleIdxInSession; // Vehicle index of the vehicle that is the fastest in this session
                public float fastestSpeedInSession;      // Speed of the vehicle that is the fastest in this session
            };

            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            public struct StartLights
            {
                public byte numLights;                  // Number of lights showing
            };

            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            public struct DriveThroughPenaltyServed
            {
                public byte vehicleIdx;                 // Vehicle index of the vehicle serving drive through
            };

            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            public struct StopGoPenaltyServed
            {
                public byte vehicleIdx;                 // Vehicle index of the vehicle serving stop go
                public float stopTime;                   // Time spent serving stop go in seconds
            };

            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            public struct Flashback
            {
                public uint flashbackFrameIdentifier;  // Frame identifier flashed back to
                public float flashbackSessionTime;       // Session time flashed back to
            };

            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            public struct Buttons
            {
                public uint buttonStatus;              // Bit flags specifying which buttons are being pressed currently - see appendices
            };

            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            public struct Overtake
            {
                public byte overtakingVehicleIdx;       // Vehicle index of the vehicle overtaking
                public byte beingOvertakenVehicleIdx;   // Vehicle index of the vehicle being overtaken
            };

            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            public struct SafetyCar
            {
                public byte safetyCarType;              // 0 = No Safety Car, 1 = Full Safety Car, 2 = Virtual Safety Car, 3 = Formation Lap Safety Car
                public byte eventType;                  // 0 = Deployed, 1 = Returning, 2 = Returned, 3 = Resume Race
            };

            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            public struct Collision
            {
                public byte vehicle1Idx;                // Vehicle index of the first vehicle involved in the collision
                public byte vehicle2Idx;                // Vehicle index of the second vehicle involved in the collision
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
            public byte m_aiControlled;                     // Whether the vehicle is AI (1) or Human (0) controlled
            public byte m_driverId;                         // Driver id - see appendix, 255 if network human
            public byte m_networkId;                        // Network id - unique identifier for network players
            public byte m_teamId;                           // Team id - see appendix
            public byte m_myTeam;                           // My team flag - 1 = My Team, 0 = otherwise
            public byte m_raceNumber;                       // Race number of the car
            public byte m_nationality;                      // Nationality of the driver

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = cs_maxParticipantNameLen)]
            public string m_name;   // Name of participant in UTF-8 format – null terminated
                                    // Will be truncated with ... (U+2026) if too long
            public byte m_yourTelemetry;                    // The player's UDP setting, 0 = restricted, 1 = public
            public byte m_showOnlineNames;                  // The player's show online names setting, 0 = off, 1 = on
            public ushort m_techLevel;                        // F1 World tech level
            public byte m_platform;                         // 1 = Steam, 3 = PlayStation, 4 = Xbox, 6 = Origin, 255 = unknown
            public byte m_numColours;                       // Number of colours valid for this car
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public LiveryColour[] m_liveryColours;                 // Colours for the car
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct PacketParticipantsData
        {
            public PacketHeader m_header;                       // Header

            // Packet specific data
            public byte m_numActiveCars;            // Number of active cars in the data - should match number of cars on HUD
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
            public byte m_frontWing;                // Front wing aero
            public byte m_rearWing;                 // Rear wing aero
            public byte m_onThrottle;               // Differential adjustment on throttle (percentage)
            public byte m_offThrottle;              // Differential adjustment off throttle (percentage)
            public float m_frontCamber;              // Front camber angle (suspension geometry)
            public float m_rearCamber;               // Rear camber angle (suspension geometry)
            public float m_frontToe;                 // Front toe angle (suspension geometry)
            public float m_rearToe;                  // Rear toe angle (suspension geometry)
            public byte m_frontSuspension;          // Front suspension
            public byte m_rearSuspension;           // Rear suspension
            public byte m_frontAntiRollBar;         // Front anti-roll bar
            public byte m_rearAntiRollBar;          // Front anti-roll bar
            public byte m_frontSuspensionHeight;    // Front ride height
            public byte m_rearSuspensionHeight;     // Rear ride height
            public byte m_brakePressure;            // Brake pressure (percentage)
            public byte m_brakeBias;                // Brake bias (percentage)
            public byte m_engineBraking;            // Engine braking (percentage)
            public float m_rearLeftTyrePressure;     // Rear left tyre pressure (PSI)
            public float m_rearRightTyrePressure;    // Rear right tyre pressure (PSI)
            public float m_frontLeftTyrePressure;    // Front left tyre pressure (PSI)
            public float m_frontRightTyrePressure;   // Front right tyre pressure (PSI)
            public byte m_ballast;                  // Ballast
            public float m_fuelLoad;                 // Fuel load
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct PacketCarSetupData
        {
            public PacketHeader m_header;               // Header

            // Packet specific data

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = cs_maxNumCarsInUDPData)]
            public CarSetupData[] m_carSetupData;

            public float m_nextFrontWingValue;   // Value of front wing after next pit stop - player only
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
            public ushort m_speed;                        // Speed of car in kilometres per hour
            public float m_throttle;                     // Amount of throttle applied (0.0 to 1.0)
            public float m_steer;                        // Steering (-1.0 (full lock left) to 1.0 (full lock right))
            public float m_brake;                        // Amount of brake applied (0.0 to 1.0)
            public byte m_clutch;                       // Amount of clutch applied (0 to 100)
            public sbyte m_gear;                         // Gear selected (1-8, N=0, R=-1)
            public ushort m_engineRPM;                    // Engine RPM
            public byte m_drs;                          // 0 = off, 1 = on
            public byte m_revLightsPercent;             // Rev lights indicator (percentage)
            public ushort m_revLightsBitValue;            // Rev lights (bit 0 = leftmost LED, bit 14 = rightmost LED)
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public ushort[] m_brakesTemperature;         // Brakes temperature (celsius)
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] m_tyresSurfaceTemperature;   // Tyres surface temperature (celsius)
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] m_tyresInnerTemperature;     // Tyres inner temperature (celsius)
            public ushort m_engineTemperature;            // Engine temperature (celsius)
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public float[] m_tyresPressure;             // Tyre pressure (PSI)
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] m_surfaceType;               // Driving surface, see appendices
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct PacketCarTelemetryData
        {
            public PacketHeader m_header;               // Header

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = cs_maxNumCarsInUDPData)]
            // Packet specific data
            public CarTelemetryData[] m_carTelemetryData;   // data for all cars on track

            public byte m_mfdPanelIndex;                // Index of MFD panel open - 255 = MFD closed
                                                        // Single player, race – 0 = Car setup, 1 = Pits
                                                        // 2 = Damage, 3 =  Engine, 4 = Temperatures
                                                        // May vary depending on game mode
            public byte m_mfdPanelIndexSecondaryPlayer; // See above
            public sbyte m_suggestedGear;                // Suggested gear for the player (1-8), 0 if no gear suggested
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
            public byte m_tractionControl;                  // Traction control - 0 = off, 1 = medium, 2 = full
            public byte m_antiLockBrakes;                   // 0 (off) - 1 (on)
            public byte m_fuelMix;                          // Fuel mix - 0 = lean, 1 = standard, 2 = rich, 3 = max
            public byte m_frontBrakeBias;                   // Front brake bias (percentage)
            public byte m_pitLimiterStatus;                 // Pit limiter status - 0 = off, 1 = on
            public float m_fuelInTank;                       // Current fuel mass
            public float m_fuelCapacity;                     // Fuel capacity
            public float m_fuelRemainingLaps;                // Fuel remaining in terms of laps (value on MFD)
            public ushort m_maxRPM;                           // Cars max RPM, point of rev limiter
            public ushort m_idleRPM;                          // Cars idle RPM
            public byte m_maxGears;                         // Maximum number of gears
            public byte m_drsAllowed;                       // 0 = not allowed, 1 = allowed
            public ushort m_drsActivationDistance;            // 0 = DRS not available, non-zero - DRS will be available in [X] metres
            public byte m_actualTyreCompound;               // F1 Modern - 16 = C5, 17 = C4, 18 = C3, 19 = C2, 20 = C1, 21 = C0, 22 = C6, 7 = inter, 8 = wet
                                                            // F1 Classic - 9 = dry, 10 = wet
                                                            // F2 – 11 = super soft, 12 = soft, 13 = medium, 14 = hard, 15 = wet
            public byte m_visualTyreCompound;               // F1 visual (can be different from actual compound)
                                                            // 16 = soft, 17 = medium, 18 = hard, 7 = inter, 8 = wet
                                                            // F1 Classic – same as above
                                                            // F2 ‘20, 15 = wet, 19 – super soft, 20 = soft, 21 = medium, 22 = hard
            public byte m_tyresAgeLaps;                     // Age in laps of the current set of tyres
            public sbyte m_vehicleFIAFlags;                  // -1 = invalid/unknown, 0 = none, 1 = green, 2 = blue, 3 = yellow
            public float m_enginePowerICE;                   // Engine power output of ICE (W)
            public float m_enginePowerMGUK;                  // Engine power output of MGU-K (W)
            public float m_ersStoreEnergy;                   // ERS energy store in Joules
            public byte m_ersDeployMode;                    // ERS deployment mode, 0 = none, 1 = medium, 2 = hotlap, 3 = overtake
            public float m_ersHarvestedThisLapMGUK;          // ERS energy harvested this lap by MGU-K
            public float m_ersHarvestedThisLapMGUH;          // ERS energy harvested this lap by MGU-H
            public float m_ersDeployedThisLap;               // ERS energy deployed this lap
            public byte m_networkPaused;                    // Whether the car is paused in a network game
        };
        [StructLayout(LayoutKind.Sequential, Pack = 1)]

        public struct PacketCarStatusData
        {
            public PacketHeader m_header;               // Header

            // Packet specific data

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = cs_maxNumCarsInUDPData)]
            public CarStatusData[] m_carStatusData;      // data for all cars on track
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
            public byte m_position;                             // Finishing position
            public byte m_numLaps;                              // Number of laps completed
            public byte m_gridPosition;                         // Grid position of the car
            public byte m_points;                               // Number of points scored
            public byte m_numPitStops;                          // Number of pit stops made
            public byte m_resultStatus;                         // Result status - 0 = invalid, 1 = inactive, 2 = active, 3 = finished, 4 = didnotfinish, 5 = disqualified, 6 = not classified, 7 = retired
            public byte m_resultReason;                         // Result reason - 0 = invalid, 1 = retired, 2 = finished, 3 = terminal damage, 4 = inactive, 5 = not enough laps completed, 6 = black flagged
                                                                // 7 = red flagged, 8 = mechanical failure, 9 = session skipped, 10 = session simulated
            public uint m_bestLapTimeInMS;                      // Best lap time of the session in milliseconds
            public double m_totalRaceTime;                        // Total race time in seconds without penalties
            public byte m_penaltiesTime;                        // Total penalties accumulated in seconds
            public byte m_numPenalties;                         // Number of penalties applied to this driver
            public byte m_numTyreStints;                        // Number of tyres stints up to maximum
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = cs_maxTyreStints)]
            public byte[] m_tyreStintsActual;   // Actual tyres used by this driver
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = cs_maxTyreStints)]
            public byte[] m_tyreStintsVisual;   // Visual tyres used by this driver
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = cs_maxTyreStints)]
            public byte[] m_tyreStintsEndLaps;  // The lap number stints end on
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct PacketFinalClassificationData
        {
            public PacketHeader m_header;               // Header

            // Packet specific data
            public byte m_numCars;          // Number of cars in the final classification
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
            public byte m_aiControlled;                     // Whether the vehicle is AI (1) or Human (0) controlled
            public byte m_teamId;                           // Team id - see appendix (255 if no team currently selected)
            public byte m_nationality;                      // Nationality of the driver
            public byte m_platform;                         // 1 = Steam, 3 = PlayStation, 4 = Xbox, 6 = Origin, 255 = unknown
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = cs_maxParticipantNameLen)]
            public string m_name;   // Name of participant in UTF-8 format – null terminated
                                    // Will be truncated with ... (U+2026) if too long
            public byte m_carNumber;                        // Car number of the player
            public byte m_yourTelemetry;                    // The player's UDP setting, 0 = restricted, 1 = public
            public byte m_showOnlineNames;                  // The player's show online names setting, 0 = off, 1 = on
            public ushort m_techLevel;                        // F1 World tech level
            public byte m_readyStatus;                      // 0 = not ready, 1 = ready, 2 = spectating
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct PacketLobbyInfoData
        {
            public PacketHeader m_header;                       // Header

            // Packet specific data
            public byte m_numPlayers;               // Number of players in the lobby data
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
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public float[] m_tyresWear;                     // Tyre wear (percentage)
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] m_tyresDamage;                   // Tyre damage (percentage)
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] m_brakesDamage;                  // Brakes damage (percentage)
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] m_tyreBlisters;                  // Tyre blisters value (percentage)
            public byte m_frontLeftWingDamage;              // Front left wing damage (percentage)
            public byte m_frontRightWingDamage;             // Front right wing damage (percentage)
            public byte m_rearWingDamage;                   // Rear wing damage (percentage)
            public byte m_floorDamage;                      // Floor damage (percentage)
            public byte m_diffuserDamage;                   // Diffuser damage (percentage)
            public byte m_sidepodDamage;                    // Sidepod damage (percentage)
            public byte m_drsFault;                         // Indicator for DRS fault, 0 = OK, 1 = fault
            public byte m_ersFault;                         // Indicator for ERS fault, 0 = OK, 1 = fault
            public byte m_gearBoxDamage;                    // Gear box damage (percentage)
            public byte m_engineDamage;                     // Engine damage (percentage)
            public byte m_engineMGUHWear;                   // Engine wear MGU-H (percentage)
            public byte m_engineESWear;                     // Engine wear ES (percentage)
            public byte m_engineCEWear;                     // Engine wear CE (percentage)
            public byte m_engineICEWear;                    // Engine wear ICE (percentage)
            public byte m_engineMGUKWear;                   // Engine wear MGU-K (percentage)
            public byte m_engineTCWear;                     // Engine wear TC (percentage)
            public byte m_engineBlown;                      // Engine blown, 0 = OK, 1 = fault
            public byte m_engineSeized;                     // Engine seized, 0 = OK, 1 = fault
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct PacketCarDamageData
        {
            public PacketHeader m_header;               // Header

            // Packet specific data
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = cs_maxNumCarsInUDPData)]
            public CarDamageData[] m_carDamageData;      // data for all cars on track
        };


        //-----------------------------------------------------------------------------
        // Session History - 1460 bytes
        //-----------------------------------------------------------------------------

        const int cs_maxNumLapsInHistory = 100;

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct LapHistoryData
        {
            public uint m_lapTimeInMS;              // Lap time in milliseconds
            public ushort m_sector1TimeMSPart;        // Sector 1 milliseconds part
            public byte m_sector1TimeMinutesPart;   // Sector 1 whole minute part
            public ushort m_sector2TimeMSPart;        // Sector 2 time milliseconds part
            public byte m_sector2TimeMinutesPart;   // Sector 2 whole minute part
            public ushort m_sector3TimeMSPart;        // Sector 3 time milliseconds part
            public byte m_sector3TimeMinutesPart;   // Sector 3 whole minute part
            public byte m_lapValidBitFlags;         // 0x01 bit set-lap valid,      0x02 bit set-sector 1 valid
                                                    // 0x04 bit set-sector 2 valid, 0x08 bit set-sector 3 valid
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct TyreStintHistoryData
        {
            public byte m_endLap;               // Lap the tyre usage ends on (255 if current tyre)
            public byte m_tyreActualCompound;   // Actual tyres used by this driver
            public byte m_tyreVisualCompound;   // Visual tyres used by this driver
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct PacketSessionHistoryData
        {
            public PacketHeader m_header;               // Header

            // Packet specific data
            public byte m_carIdx;                   // Index of the car this lap data relates to
            public byte m_numLaps;                  // Num laps in the data (including current partial lap)
            public byte m_numTyreStints;            // Number of tyre stints in the data

            public byte m_bestLapTimeLapNum;        // Lap the best lap time was achieved on
            public byte m_bestSector1LapNum;        // Lap the best Sector 1 time was achieved on
            public byte m_bestSector2LapNum;        // Lap the best Sector 2 time was achieved on
            public byte m_bestSector3LapNum;        // Lap the best Sector 3 time was achieved on


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
            public byte m_actualTyreCompound;               // Actual tyre compound used
            public byte m_visualTyreCompound;               // Visual tyre compound used
            public byte m_wear;                             // Tyre wear (percentage)
            public byte m_available;                        // Whether this set is currently available
            public byte m_recommendedSession;               // Recommended session for tyre set, see appendix
            public byte m_lifeSpan;                         // Laps left in this tyre set
            public byte m_usableLife;                       // Max number of laps recommended for this compound
            public short m_lapDeltaTime;                     // Lap delta time in milliseconds compared to fitted set
            public byte m_fitted;                           // Whether the set is fitted or not
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct PacketTyreSetsData
        {
            public PacketHeader m_header;                           // Header
            public byte m_carIdx;                           // Index of the car this data relates to
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = cs_maxNumTyreSets)]
            public TyreSetData[] m_tyreSetData;   // 13 (dry) + 7 (wet)
            public byte m_fittedIdx;                        // Index into array of fitted tyre
        };


        //-----------------------------------------------------------------------------
        // Motion Ex - 273 bytes
        //-----------------------------------------------------------------------------

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct PacketMotionExData
        {
            public PacketHeader m_header;               // Header

            // Extra player car ONLY data
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public float[] m_suspensionPosition;        // Note: All wheel arrays have the following order:
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public float[] m_suspensionVelocity;        // RL, RR, FL, FR
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public float[] m_suspensionAcceleration;    // RL, RR, FL, FR
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public float[] m_wheelSpeed;                // Speed of each wheel
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public float[] m_wheelSlipRatio;            // Slip ratio for each wheel
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public float[] m_wheelSlipAngle;            // Slip angles for each wheel
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public float[] m_wheelLatForce;             // Lateral forces for each wheel
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public float[] m_wheelLongForce;            // Longitudinal forces for each wheel
            public float m_heightOfCOGAboveGround;       // Height of centre of gravity above ground
            public float m_localVelocityX;               // Velocity in local space X - metres/s
            public float m_localVelocityY;               // Velocity in local space Y
            public float m_localVelocityZ;               // Velocity in local space Z
            public float m_angularVelocityX;             // Angular velocity x-component - radians/s
            public float m_angularVelocityY;             // Angular velocity y-component
            public float m_angularVelocityZ;             // Angular velocity z-component
            public float m_angularAccelerationX;         // Angular acceleration x-component - radians/s/s
            public float m_angularAccelerationY;         // Angular acceleration y-component
            public float m_angularAccelerationZ;         // Angular acceleration z-component
            public float m_frontWheelsAngle;             // Current front wheels angle in radians
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public float[] m_wheelVertForce;            // Vertical forces for each wheel
            public float m_frontAeroHeight;              // Front plank edge height above road surface
            public float m_rearAeroHeight;               // Rear plank edge height above road surface
            public float m_frontRollAngle;               // Roll angle of the front suspension
            public float m_rearRollAngle;                // Roll angle of the rear suspension
            public float m_chassisYaw;                   // Yaw angle of the chassis relative to the direction of motion - radians
            public float m_chassisPitch;                 // Pitch angle of the chassis relative to the direction of motion - radians
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public float[] m_wheelCamber;               // Camber of each wheel in radians
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public float[] m_wheelCamberGain;           // Camber gain for each wheel in radians, difference between active camber and dynamic camber
        };


        //-----------------------------------------------------------------------------
        // Time Trial - 101 bytes
        //-----------------------------------------------------------------------------

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct TimeTrialDataSet
        {
            public byte m_carIdx;                   // Index of the car this data relates to
            public byte m_teamId;                   // Team id - see appendix
            public uint m_lapTimeInMS;              // Lap time in milliseconds
            public uint m_sector1TimeInMS;          // Sector 1 time in milliseconds
            public uint m_sector2TimeInMS;          // Sector 2 time in milliseconds
            public uint m_sector3TimeInMS;          // Sector 3 time in milliseconds
            public byte m_tractionControl;          // 0 = assist off, 1 = assist on
            public byte m_gearboxAssist;            // 0 = assist off, 1 = assist on
            public byte m_antiLockBrakes;           // 0 = assist off, 1 = assist on
            public byte m_equalCarPerformance;      // 0 = Realistic, 1 = Equal
            public byte m_customSetup;              // 0 = No, 1 = Yes
            public byte m_valid;                    // 0 = invalid, 1 = valid
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct PacketTimeTrialData
        {
            public PacketHeader m_header;                       // Header

            public TimeTrialDataSet m_playerSessionBestDataSet;     // Player session best data set
            public TimeTrialDataSet m_personalBestDataSet;          // Personal best data set
            public TimeTrialDataSet m_rivalDataSet;                 // Rival data set
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
            public PacketHeader m_header;                   // Header

            // Packet specific data
            public byte m_numLaps;                  // Number of laps in the data
            public byte m_lapStart;                 // Index of the lap where the data starts, 0 indexed

            // Array holding the position of the car in a given lap, 0 if no record
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
