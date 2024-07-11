using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

/*
How do I enable the UDP Telemetry Output?
In F1 22, UDP telemetry output is controlled via the in-game menus. To enable this, enter the options menu from the main menu (triangle / Y), then enter the settings menu - the UDP option will be at the bottom of the list. From there you will be able to enable / disable the UDP output, configure the IP address and port for the receiving application, toggle broadcast mode and set the send rate. Broadcast mode transmits the data across the network subnet to allow multiple devices on the same subnet to be able to receive this information. When using broadcast mode it is not necessary to set a target IP address, just a target port for applications to listen on.

Advanced PC Users: You can additionally edit the game’s configuration XML file to configure UDP output. The file is located here (after an initial boot of the game):

...\Documents\My Games\<game_folder>\hardwaresettings\hardware_settings_config.xml

You should see the tag:

< motion >
  ...
  < udp enabled = "false" broadcast =”false” ip = "127.0.0.1" port = "20777" sendRate =”20” format =”2022” yourTelemetry =”restricted” />
                 ...
</ motion >

Here you can set the values manually. Note that any changes made within the game when it is running will overwrite any changes made manually. Note the enabled flag is now a state.
*/

namespace F1Series
{
    public static class F12022
    {
        public enum PacketID : byte
        {
            Motion,
            Session,
            LapData,
            Event,
            Participants,
            CarSetups,
            CarTelemetry,
            CarStatus,
            FinalClassification,
            LobbyInfo,
            CarDamage,
            SessionHistory
        };

        public enum SurfaceType : byte
        {
            Tarmac = 0,
            RumbleStrip = 1,
            Concrete = 2,
            Rock = 3,
            Gravel = 4,
            Mud = 5,
            Sand = 6,
            Grass = 7,
            Water = 8,
            Cobblestone = 9,
            Metal = 10,
            Ridged = 11,
        }

        public enum FiaFlags : sbyte
        {
            Invalid = -1, None = 0, Green = 1, Blue = 2, Yellow = 3, Red = 4
        }

        [Flags]
        public enum ButtonFlags : uint
        {
            CrossorA = 0x00000001,
            TriangleorY = 0x00000002,
            CircleorB = 0x00000004,
            SquareorX = 0x00000008,
            DpadLeft = 0x00000010,
            DpadRight = 0x00000020,
            DpadUp = 0x00000040,
            DpadDown = 0x00000080,
            OptionsorMenu = 0x00000100,
            L1orLB = 0x00000200,
            R1orRB = 0x00000400,
            L2orLT = 0x00000800,
            R2orRT = 0x00001000,
            LeftStickClick = 0x00002000,
            RightStickClick = 0x00004000,
            RightStickLeft = 0x00008000,
            RightStickRight = 0x00010000,
            RightStickUp = 0x00020000,
            RightStickDown = 0x00040000,
            Special = 0x00080000,
            UDPAction1 = 0x00100000,
            UDPAction2 = 0x00200000,
            UDPAction3 = 0x00400000,
            UDPAction4 = 0x00800000,
            UDPAction5 = 0x01000000,
            UDPAction6 = 0x02000000,
            UDPAction7 = 0x04000000,
            UDPAction8 = 0x08000000,
            UDPAction9 = 0x10000000,
            UDPAction10 = 0x20000000,
            UDPAction11 = 0x40000000,
            UDPAction12 = 0x80000000,
        }

        // F1 visual (can be different from actual compound)
        // 16 = soft, 17 = medium, 18 = hard, 7 = inter, 8 = wet
        // F1 Classic – same as above
        // F2 ‘19, 15 = wet, 19 – super soft, 20 = soft
        // 21 = medium , 22 = hard

        public static readonly Dictionary<int, string> CompoundID = new Dictionary<int, string>
        {
            {16, "soft"},
            {20, "soft"},
            {17, "medium"},
            {21, "medium" } ,
            {18, "hard"},
            {22,"hard" },
            {7,   "inter"},
            {8,  "wet" },
            {15, "wet"},
            {19, "super soft"},
        };

        public static readonly Dictionary<string, string> EventStringCodes = new Dictionary<string, string>
        {
            {"SSTA", "Session Started"},
            {"SEND", "Session Ended"},
            {"FTLP", "Fastest Lap"},
            {"RTMT", "Retirement"},
            {"DRSE", "DRS enabled"},
            {"DRSD", "DRS disabled"},
            {"TMPT", "Team mate in pits"},
            {"CHQF", "Chequered flag"},
            {"RCWN", "Race Winner"},
            {"PENA", "Penalty Issued"},
            {"SPTP", "Speed Trap Triggered"},
            {"STLG", "Start lights"},
            {"LGOT", "Lights out"},
            {"DTSV", "Drive through served"},
            {"SGSV", "Stop go served"},
            {"FLBK", "Flashback"},
            {"BUTN", "Button status"},
        };

        public static readonly Dictionary<int, string> GameModeID = new Dictionary<int, string>
        {
            {0  ,"Event Mode"},
            {3  ,"Grand Prix"},
            {5  ,"Time Trial"},
            {6  ,"Splitscreen"},
            {7  ,"Online Custom"},
            {8  ,"Online League"},
            {11 ,"Career Invitational"},
            {12 ,"Championship Invitational"},
            {13 ,"Championship"},
            {14 ,"Online Championship"},
            {15 ,"Online Weekly Event"},
            {19 ,"Career ‘22"},
            {20 ,"Career ’22 Online"},
            {127,"Benchmark"},
        };

        public static readonly Dictionary<int, string> TeamsID = new Dictionary<int, string>
        {
            {0  ,"Mercedes"},
            {1  ,"Ferrari"},
            {2  ,"Red Bull Racing"},
            {3  ,"Williams"},
            {4  ,"Aston Martin"},
            {5  ,"Alpine"},
            {6  ,"Alpha Tauri"},
            {7  ,"Haas"},
            {8  ,"McLaren"},
            {9  ,"Alfa Romeo"},
            {85 ,"Mercedes 2020"},
            {86 ,"Ferrari 2020"},
            {87 ,"Red Bull 2020"},
            {88 ,"Williams 2020"},
            {89 ,"Racing Point 2020"},
            {90 ,"Renault 2020"},
            {91 ,"Alpha Tauri 2020"},
            {92 ,"Haas 2020"},
            {93 ,"McLaren 2020"},
            {94 ,"Alfa Romeo 2020"},
            {95 ,"Aston Martin DB11 V12"},
            {96 ,"Aston Martin Vantage F1 Edition"},
            {97 ,"Aston Martin Vantage Safety Car"},
            {98 ,"Ferrari F8 Tributo"},
            {99 ,"Ferrari Roma"},
            {100,"McLaren 720S"},
            {101,"McLaren Artura"},
            {102,"Mercedes AMG GT Black Series Safety Car"},
            {103,"Mercedes AMG GTR Pro"},
            {104,"F1 Custom Team"},
            {106,"Prema ‘21"},
            {107,"Uni-Virtuosi ‘21"},
            {108,"Carlin ‘21"},
            {109,"Hitech ‘21"},
            {110,"Art GP ‘21"},
            {111,"MP Motorsport ‘21"},
            {112,"Charouz ‘21"},
            {113,"Dams ‘21"},
            {114,"Campos ‘21"},
            {115,"BWT ‘21"},
            {116,"Trident ‘21"},
            {117,"Mercedes AMG GT Black Series"},
            {118,"Prema ‘22"},
            {119,"Virtuosi ‘22"},
            {120,"Carlin ‘22"},
            {121,"Hitech ‘22"},
            {122,"Art GP ‘22"},
            {123,"MP Motorsport ‘22"},
            {124,"Charouz ‘22"},
            {125,"Dams ‘22"},
            {126,"Campos ‘22"},
            {127,"Van Amersfoort Racing ‘22"},
            {128,"Trident ‘22"},
        };

        public static readonly Dictionary<int, string> DriversID = new Dictionary<int, string>
        {
            { 0,"Carlos Sainz"},
            { 1,"Daniil Kvyat"},
            { 2,"Daniel Ricciardo"},
            { 3,"Fernando Alonso"},
            { 4,"Felipe Massa"},
            { 6,"Kimi Räikkönen"},
            { 7,"Lewis Hamilton"},
            { 9,"Max Verstappen"},
            { 10,"Nico Hulkenburg"},
            { 11,"Kevin Magnussen"},
            { 12,"Romain Grosjean"},
            { 13,"Sebastian Vettel"},
            { 14,"Sergio Perez"},
            { 15,"Valtteri Bottas"},
            { 17,"Esteban Ocon"},
            { 19,"Lance Stroll"},
            { 20,"Arron Barnes"},
            { 21,"Martin Giles"},
            { 22,"Alex Murray"},
            { 23,"Lucas Roth"},
            { 24,"Igor Correia"},
            { 25,"Sophie Levasseur"},
            { 26,"Jonas Schiffer"},
            { 27,"Alain Forest"},
            { 28,"Jay Letourneau"},
            { 29,"Esto Saari"},
            { 30,"Yasar Atiyeh"},
            { 31,"Callisto Calabresi"},
            { 32,"Naota Izum"},
            { 33,"Howard Clarke"},
            { 34,"Wilheim Kaufmann"},
            { 35,"Marie Laursen"},
            { 36,"Flavio Nieves"},
            { 37,"Peter Belousov"},
            { 38,"Klimek Michalski"},
            { 39,"Santiago Moreno"},
            { 40,"Benjamin Coppens"},
            { 41,"Noah Visser"},
            { 42,"Gert Waldmuller"},
            { 43,"Julian Quesada"},
            { 44,"Daniel Jones"},
            { 45,"Artem Markelov"},
            { 46,"Tadasuke Makino"},
            { 47,"Sean Gelael"},
            { 48,"Nyck De Vries"},
            { 49,"Jack Aitken"},
            { 50,"George Russell"},
            { 51,"Maximilian Günther"},
            { 52,"Nirei Fukuzumi"},
            { 53,"Luca Ghiotto"},
            { 54,"Lando Norris"},
            { 55,"Sérgio Sette Câmara"},
            { 56,"Louis Delétraz"},
            { 57,"Antonio Fuoco"},
            { 58,"Charles Leclerc"},
            { 59,"Pierre Gasly"},
            { 62,"Alexander Albon"},
            { 63,"Nicholas Latifi"},
            { 64,"Dorian Boccolacci"},
            { 65,"Niko Kari"},
            { 66,"Roberto Merhi"},
            { 67,"Arjun Maini"},
            { 68,"Alessio Lorandi"},
            { 69,"Ruben Meijer"},
            { 70,"Rashid Nair"},
            { 71,"Jack Tremblay"},
            { 72,"Devon Butler"},
            { 73,"Lukas Weber"},
            { 74,"Antonio Giovinazzi"},
            { 75,"Robert Kubica"},
            { 76,"Alain Prost"},
            { 77,"Ayrton Senna"},
            { 78,"Nobuharu Matsushita"},
            { 79,"Nikita Mazepin"},
            { 80,"Guanya Zhou"},
            { 81,"Mick Schumacher"},
            { 82,"Callum Ilott"},
            { 83,"Juan Manuel Correa"},
            { 84,"Jordan King"},
            { 85,"Mahaveer Raghunathan"},
            { 86,"Tatiana Calderon"},
            { 87,"Anthoine Hubert"},
            { 88,"Guiliano Alesi"},
            { 89,"Ralph Boschung"},
            { 90,"Michael Schumacher"},
            { 91,"Dan Ticktum"},
            { 92,"Marcus Armstrong"},
            { 93,"Christian Lundgaard"},
            { 94,"Yuki Tsunoda"},
            { 95,"Jehan Daruvala"},
            { 96,"Gulherme Samaia"},
            { 97,"Pedro Piquet"},
            { 98,"Felipe Drugovich"},
            { 99,"Robert Schwartzman"},
            { 100,"Roy Nissany"},
            { 101,"Marino Sato"},
            { 102,"Aidan Jackson"},
            { 103,"Casper Akkerman"},
            { 109,"Jenson Button"},
            { 110,"David Coulthard"},
            { 111,"Nico Rosberg"},
            { 112,"Oscar Piastri"},
            { 113,"Liam Lawson"},
            { 114,"Juri Vips"},
            { 115,"Theo Pourchaire"},
            { 116,"Richard Verschoor"},
            { 117,"Lirim Zendeli"},
            { 118,"David Beckmann"},
            { 121,"Alessio Deledda"},
            { 122,"Bent Viscaal"},
            { 123,"Enzo Fittipaldi"},
            { 125,"Mark Webber"},
            { 126,"Jacques Villeneuve"},
            { 127,"Jake Hughes"},
            { 128,"Frederik Vesti"},
            { 129,"Olli Caldwell"},
            { 130,"Logan Sargeant"},
            { 131,"Cem Bolukbasi"},
            { 132,"Ayuma Iwasa"},
            { 133,"Clement Novolak"},
            { 134,"Dennis Hauger"},
            { 135,"Calan Williams"},
            { 136,"Jack Doohan"},
            { 137,"Amaury Cordeel"},
            { 138,"Mika Hakkinen"},
        };

        public static readonly string[] TrackID = new string[]
        {
            "Melbourne", // 0
            "Paul Ricard",
            "Shanghai",
            "Sakhir (Bahrain)",
            "Catalunya",
            "Monaco",
            "Montreal",
            "Silverstone",
            "Hockenheim",
            "Hungaroring",
            "Spa",
            "Monza",
            "Singapore",
            "Suzuka",
            "Abu Dhabi",
            "Texas",
            "Brazil",
            "Austria",
            "Sochi",
            "Mexico",
            "Baku (Azerbaijan)",
            "Sakhir Short",
            "Silverstone Short",
            "Texas Short",
            "Suzuka Short",
            "Hanoi",
            "Zandvoort",
            "Imola",
            "Portimão",
            "Jeddah",
            "Miami", // 30
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct PacketHeader
        {
            /// <summary>
            /// 2022
            /// </summary>
            public ushort m_packetFormat;            
            /// <summary>
            /// Game major version - "X.00"
            /// </summary>
            public byte m_gameMajorVersion;        
            /// <summary>
            /// Game minor version - "1.XX"
            /// </summary>
            public byte m_gameMinorVersion;        
            /// <summary>
            /// Version of this packet type, all start from 1
            /// </summary>
            public byte m_packetVersion;           
            /// <summary>
            /// Identifier for the packet type, see below
            /// </summary>
            [MarshalAs(UnmanagedType.U1)]
            public PacketID m_packetId;                
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
            /// Index of player's car in the array
            /// </summary>
            public byte m_playerCarIndex;
            /// <summary>
            /// Index of secondary player's car in the array (splitscreen)
            /// 255 if no second player
            /// </summary>
            public byte m_secondaryPlayerCarIndex;                               
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct CarMotionData
        {
            public float m_worldPositionX;           // World space X position
            public float m_worldPositionY;           // World space Y position
            public float m_worldPositionZ;           // World space Z position
            public float m_worldVelocityX;           // Velocity in world space X
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

        /// <summary>
        /// Frequency: Rate as specified in menus
        /// Size: 1464 bytes
        /// Version: 1
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct PacketMotionData
        {
            public PacketHeader m_header;                  // Header

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 22)]
            public CarMotionData[] m_carMotionData;      // Data for all cars on track

            // Extra player car ONLY data
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public float[] m_suspensionPosition;       // Note: All wheel arrays have the following order:
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public float[] m_suspensionVelocity;       // RL, RR, FL, FR
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public float[] m_suspensionAcceleration;  // RL, RR, FL, FR
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public float[] m_wheelSpeed;              // Speed of each wheel
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public float[] m_wheelSlip;                // Slip ratio for each wheel
            public float m_localVelocityX;             // Velocity in local space
            public float m_localVelocityY;             // Velocity in local space
            public float m_localVelocityZ;             // Velocity in local space
            public float m_angularVelocityX;       // Angular velocity x-component
            public float m_angularVelocityY;            // Angular velocity y-component
            public float m_angularVelocityZ;            // Angular velocity z-component
            public float m_angularAccelerationX;        // Angular velocity x-component
            public float m_angularAccelerationY;   // Angular velocity y-component
            public float m_angularAccelerationZ;        // Angular velocity z-component
            public float m_frontWheelsAngle;            // Current front wheels angle in radians
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct MarshalZone
        {
            public float m_zoneStart;   // Fraction (0..1) of way through the lap the marshal zone starts
            public sbyte m_zoneFlag;    // -1 = invalid/unknown, 0 = none, 1 = green, 2 = blue, 3 = yellow, 4 = red
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct WeatherForecastSample
        {
            public byte m_sessionType;              // 0 = unknown, 1 = P1, 2 = P2, 3 = P3, 4 = Short P, 5 = Q1
                                                    // 6 = Q2, 7 = Q3, 8 = Short Q, 9 = OSQ, 10 = R, 11 = R2
                                                    // 12 = R3, 13 = Time Trial
            public byte m_timeOffset;               // Time in minutes the forecast is for
            public byte m_weather;                  // Weather - 0 = clear, 1 = light cloud, 2 = overcast
                                                    // 3 = light rain, 4 = heavy rain, 5 = storm
            public sbyte m_trackTemperature;         // Track temp. in degrees Celsius
            public sbyte m_trackTemperatureChange;   // Track temp. change – 0 = up, 1 = down, 2 = no change
            public sbyte m_airTemperature;           // Air temp. in degrees celsius
            public sbyte m_airTemperatureChange;     // Air temp. change – 0 = up, 1 = down, 2 = no change
            public byte m_rainPercentage;           // Rain percentage (0-100)
        };

        /// <summary>
        /// Frequency: 2 per second
        /// Size: 632 bytes
        /// Version: 1
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct PacketSessionData
        {
            public PacketHeader m_header;                  // Header

            public byte m_weather;                // Weather - 0 = clear, 1 = light cloud, 2 = overcast
                                                  // 3 = light rain, 4 = heavy rain, 5 = storm
            public sbyte m_trackTemperature;        // Track temp. in degrees celsius
            public sbyte m_airTemperature;          // Air temp. in degrees celsius
            public byte m_totalLaps;              // Total number of laps in this race
            public ushort m_trackLength;               // Track length in metres
            public byte m_sessionType;            // 0 = unknown, 1 = P1, 2 = P2, 3 = P3, 4 = Short P
                                                  // 5 = Q1, 6 = Q2, 7 = Q3, 8 = Short Q, 9 = OSQ
                                                  // 10 = R, 11 = R2, 12 = R3, 13 = Time Trial
            public sbyte m_trackId;                 // -1 for unknown, see appendix
            public byte m_formula;                    // Formula, 0 = F1 Modern, 1 = F1 Classic, 2 = F2,
                                                      // 3 = F1 Generic, 4 = Beta, 5 = Supercars
                                                      // 6 = Esports, 7 = F2 2021
            public ushort m_sessionTimeLeft;       // Time left in session in seconds
            public ushort m_sessionDuration;       // Session duration in seconds
            public byte m_pitSpeedLimit;          // Pit speed limit in kilometres per hour
            public byte m_gamePaused;                // Whether the game is paused – network game only
            public byte m_isSpectating;           // Whether the player is spectating
            public byte m_spectatorCarIndex;      // Index of the car being spectated
            public byte m_sliProNativeSupport;    // SLI Pro support, 0 = inactive, 1 = active
            public byte m_numMarshalZones;            // Number of marshal zones to follow
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 21)]
            public MarshalZone[] m_marshalZones;             // List of marshal zones – max 21
            public byte m_safetyCarStatus;           // 0 = no safety car, 1 = full
                                                     // 2 = virtual, 3 = formation lap
            public byte m_networkGame;               // 0 = offline, 1 = online
            public byte m_numWeatherForecastSamples; // Number of weather samples to follow
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 56)]
            public WeatherForecastSample[] m_weatherForecastSamples;   // Array of weather forecast samples
            public byte m_forecastAccuracy;          // 0 = Perfect, 1 = Approximate
            public byte m_aiDifficulty;              // AI Difficulty rating – 0-110
            public uint m_seasonLinkIdentifier;      // Identifier for season - persists across saves
            public uint m_weekendLinkIdentifier;     // Identifier for weekend - persists across saves
            /// <summary>
            /// // Identifier for session - persists across saves
            /// </summary>
            public uint m_sessionLinkIdentifier;     
            public byte m_pitStopWindowIdealLap;     // Ideal lap to pit on for current strategy (player)
            public byte m_pitStopWindowLatestLap;    // Latest lap to pit on for current strategy (player)
            public byte m_pitStopRejoinPosition;     // Predicted position to rejoin at (player)
            public byte m_steeringAssist;            // 0 = off, 1 = on
            public byte m_brakingAssist;             // 0 = off, 1 = low, 2 = medium, 3 = high
            public byte m_gearboxAssist;             // 1 = manual, 2 = manual & suggested gear, 3 = auto
            public byte m_pitAssist;                 // 0 = off, 1 = on
            public byte m_pitReleaseAssist;          // 0 = off, 1 = on
            public byte m_ERSAssist;                 // 0 = off, 1 = on
            public byte m_DRSAssist;                 // 0 = off, 1 = on
            public byte m_dynamicRacingLine;         // 0 = off, 1 = corners only, 2 = full
            public byte m_dynamicRacingLineType;     // 0 = 2D, 1 = 3D
            public byte m_gameMode;                  // Game mode id - see appendix
            public byte m_ruleSet;                   // Ruleset - see appendix
            public uint m_timeOfDay;                 // Local time of day - minutes since midnight
            public byte m_sessionLength;             // 0 = None, 2 = Very Short, 3 = Short, 4 = Medium
                                                     // 5 = Medium Long, 6 = Long, 7 = Full
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct LapData
        {
            public uint m_lastLapTimeInMS;            // Last lap time in milliseconds
            public uint m_currentLapTimeInMS;     // Current time around the lap in milliseconds
            public ushort m_sector1TimeInMS;           // Sector 1 time in milliseconds
            public ushort m_sector2TimeInMS;           // Sector 2 time in milliseconds
            public float m_lapDistance;         // Distance vehicle is around current lap in metres – could
                                                // be negative if line hasn’t been crossed yet
            public float m_totalDistance;       // Total distance travelled in session in metres – could
                                                // be negative if line hasn’t been crossed yet
            public float m_safetyCarDelta;            // Delta in seconds for safety car
            /// <summary>
            /// Car race position
            /// </summary>
            public byte m_carPosition;             // 
            /// <summary>
            /// Current lap number
            /// </summary>
            public byte m_currentLapNum;       // 
            /// <summary>
            /// 0 = none, 1 = pitting, 2 = in pit area
            /// </summary>
            public byte m_pitStatus;
            public byte m_numPitStops;                 // Number of pit stops taken in this race
            /// <summary>
            ///  0 = sector1, 1 = sector2, 2 = sector3
            /// </summary>
            public byte m_sector;
            /// <summary>
            /// Current lap invalid - 0 = valid, 1 = invalid
            /// </summary>
            public byte m_currentLapInvalid;       // 
            public byte m_penalties;               // Accumulated time penalties in seconds to be added
            public byte m_warnings;                  // Accumulated number of warnings issued
            public byte m_numUnservedDriveThroughPens;  // Num drive through pens left to serve
            public byte m_numUnservedStopGoPens;        // Num stop go pens left to serve
            public byte m_gridPosition;            // Grid position the vehicle started the race in
            public byte m_driverStatus;            // Status of driver - 0 = in garage, 1 = flying lap
                                                   // 2 = in lap, 3 = out lap, 4 = on track
            public byte m_resultStatus;              // Result status - 0 = invalid, 1 = inactive, 2 = active
                                                     // 3 = finished, 4 = didnotfinish, 5 = disqualified
                                                     // 6 = not classified, 7 = retired
            public byte m_pitLaneTimerActive;          // Pit lane timing, 0 = inactive, 1 = active
            public ushort m_pitLaneTimeInLaneInMS;      // If active, the current time spent in the pit lane in ms
            public ushort m_pitStopTimerInMS;           // Time of the actual pit stop in ms
            public byte m_pitStopShouldServePen;       // Whether the car should serve a penalty at this stop
        };

        /// <summary>
        /// Frequency: Rate as specified in menus
        /// Size: 972 bytes
        /// Version: 1
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct PacketLapData
        {
            public PacketHeader m_header;              // Header

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 22)]
            public LapData[] m_lapData;         // Lap data for all cars on track

            public byte m_timeTrialPBCarIdx;  // Index of Personal Best car in time trial (255 if invalid)
            public byte m_timeTrialRivalCarIdx;   // Index of Rival car in time trial (255 if invalid)
        };

        //////////////////////////

        // The event details packet is different for each type of event.
        // Make sure only the correct type is interpreted.
        [StructLayout(LayoutKind.Explicit)]
        public struct EventDataDetails // Union
        {
            [FieldOffset(0)]
            public FastestLap fastestLap;
            [FieldOffset(0)]
            public Retirement retirement;
            [FieldOffset(0)]
            public TeamMateInPits teamMateInPits;
            [FieldOffset(0)]
            public RaceWinner raceWinner;
            [FieldOffset(0)]
            public Penalty penalty;
            [FieldOffset(0)]
            public SpeedTrap speedTrap;
            [FieldOffset(0)]
            public StartLights startLIghts;
            [FieldOffset(0)]
            public DriveThroughPenaltyServed driveThroughPenaltyServed;
            [FieldOffset(0)]
            public StopGoPenaltyServed stopGoPenaltyServed;
            [FieldOffset(0)]
            public Flashback flashback;
            [FieldOffset(0)]
            public Buttons buttons;

            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            public struct FastestLap
            {
                public byte vehicleIdx; // Vehicle index of car achieving fastest lap
                public float lapTime;    // Lap time is in seconds
            }

            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            public struct Retirement
            {
                public byte vehicleIdx; // Vehicle index of car retiring
            }

            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            public struct TeamMateInPits
            {
                public byte vehicleIdx; // Vehicle index of team mate
            }

            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            public struct RaceWinner
            {
                public byte vehicleIdx; // Vehicle index of the race winner
            }

            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            public struct Penalty
            {
                public byte penaltyType;      // Penalty type – see Appendices
                public byte infringementType;     // Infringement type – see Appendices
                public byte vehicleIdx;           // Vehicle index of the car the penalty is applied to
                public byte otherVehicleIdx;      // Vehicle index of the other car involved
                public byte time;                 // Time gained, or time spent doing action in seconds
                public byte lapNum;               // Lap the penalty occurred on
                public byte placesGained;         // Number of places gained by this
            }

            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            public struct SpeedTrap
            {
                public byte vehicleIdx;       // Vehicle index of the vehicle triggering speed trap
                public float speed;            // Top speed achieved in kilometres per hour
                public byte isOverallFastestInSession; // Overall fastest speed in session = 1, otherwise 0
                public byte isDriverFastestInSession;  // Fastest speed for driver in session = 1, otherwise 0
                public byte fastestVehicleIdxInSession;// Vehicle index of the vehicle that is the fastest
                                                       // in this session
                public float fastestSpeedInSession;      // Speed of the vehicle that is the fastest
                                                         // in this session
            }

            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            public struct StartLights
            {
                public byte numLights;            // Number of lights showing
            }

            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            public struct DriveThroughPenaltyServed
            {
                public byte vehicleIdx;                 // Vehicle index of the vehicle serving drive through
            }

            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            public struct StopGoPenaltyServed
            {
                public byte vehicleIdx;                 // Vehicle index of the vehicle serving stop go
            }

            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            public struct Flashback
            {
                public uint flashbackFrameIdentifier;  // Frame identifier flashed back to
                public float flashbackSessionTime;       // Session time flashed back to
            }

            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            public struct Buttons
            {
                public uint m_buttonStatus;    // Bit flags specifying which buttons are being pressed
                                               // currently - see appendices
            }
        };

        /// <summary>
        /// Frequency: When the event occurs
        /// Size: 40 bytes
        /// Version: 1
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct PacketEventData
        {
            public PacketHeader m_header;                  // Header

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] m_eventStringCode;     // Event string code, see below
            public EventDataDetails m_eventDetails;            // Event details - should be interpreted differently
                                                               // for each type
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
        public struct ParticipantData
        {
            public byte m_aiControlled;           // Whether the vehicle is AI (1) or Human (0) controlled
            public byte m_driverId;       // Driver id - see appendix, 255 if network human
            public byte m_networkId;      // Network id – unique identifier for network players
            public byte m_teamId;                 // Team id - see appendix
            public byte m_myTeam;                 // My team flag – 1 = My Team, 0 = otherwise
            /// <summary>
            /// Race number of the car
            /// </summary>
            public byte m_raceNumber;
            public byte m_nationality;            // Nationality of the driver
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 48)]
            public string m_name;               // Name of participant in UTF-8 format – null terminated
                                                // Will be truncated with … (U+2026) if too long
            public byte m_yourTelemetry;          // The player's UDP setting, 0 = restricted, 1 = public
        };

        /// <summary>
        /// Frequency: Every 5 seconds
        /// Size: 1257 bytes
        /// Version: 1
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct PacketParticipantsData
        {
            public PacketHeader m_header;            // Header

            public byte m_numActiveCars;  // Number of active cars in the data – should match number of
                                          // cars on HUD
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 22)]
            public ParticipantData[] m_participants;
        };

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
            public float m_rearLeftTyrePressure;     // Rear left tyre pressure (PSI)
            public float m_rearRightTyrePressure;    // Rear right tyre pressure (PSI)
            public float m_frontLeftTyrePressure;    // Front left tyre pressure (PSI)
            public float m_frontRightTyrePressure;   // Front right tyre pressure (PSI)
            public byte m_ballast;                  // Ballast
            public float m_fuelLoad;                 // Fuel load
        };

        /// <summary>
        /// Frequency: 2 per second
        /// Size: 1102 bytes
        /// Version: 1
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct PacketCarSetupData
        {
            public PacketHeader m_header;            // Header

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 22)]
            public CarSetupData[] m_carSetups;
        };

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
            /// Tyres pressure (PSI)
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public float[] m_tyresPressure;
            /// <summary>
            /// Driving surface, see appendices
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] m_surfaceType;
        };

        /// <summary>
        /// Frequency: Rate as specified in menus
        /// Size: 1347 bytes
        /// Version: 1
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct PacketCarTelemetryData
        {
            public PacketHeader m_header;        // Header

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 22)]
            public CarTelemetryData[] m_carTelemetryData;

            public byte m_mfdPanelIndex;       // Index of MFD panel open - 255 = MFD closed
                                               // Single player, race – 0 = Car setup, 1 = Pits
                                               // 2 = Damage, 3 =  Engine, 4 = Temperatures
                                               // May vary depending on game mode
            public byte m_mfdPanelIndexSecondaryPlayer;   // See above
            public sbyte m_suggestedGear;       // Suggested gear for the player (1-8)
                                                // 0 if no gear suggested
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct CarStatusData
        {
            /// <summary>
            /// Traction control - 0 = off, 1 = medium, 2 = full
            /// </summary>
            public byte m_tractionControl;          // 
            /// <summary>
            /// 0 (off) - 1 (on)
            /// </summary>
            public byte m_antiLockBrakes;           // 
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
            public float m_fuelInTank;               // Current fuel mass
            public float m_fuelCapacity;             // Fuel capacity
            public float m_fuelRemainingLaps;        // Fuel remaining in terms of laps (value on MFD)
            public ushort m_maxRPM;                   // Cars max RPM, point of rev limiter
            public ushort m_idleRPM;                  // Cars idle RPM
            public byte m_maxGears;                 // Maximum number of gears
            public byte m_drsAllowed;               // 0 = not allowed, 1 = allowed
            public ushort m_drsActivationDistance;    // 0 = DRS not available, non-zero - DRS will be available
                                                      // in [X] metres
            public byte m_actualTyreCompound;    // F1 Modern - 16 = C5, 17 = C4, 18 = C3, 19 = C2, 20 = C1
                                                 // 7 = inter, 8 = wet
                                                 // F1 Classic - 9 = dry, 10 = wet
                                                 // F2 – 11 = super soft, 12 = soft, 13 = medium, 14 = hard
                                                 // 15 = wet
            public byte m_visualTyreCompound;       // F1 visual (can be different from actual compound)
                                                    // 16 = soft, 17 = medium, 18 = hard, 7 = inter, 8 = wet
                                                    // F1 Classic – same as above
                                                    // F2 ‘19, 15 = wet, 19 – super soft, 20 = soft
                                                    // 21 = medium , 22 = hard
            public byte m_tyresAgeLaps;             // Age in laps of the current set of tyres
            public sbyte m_vehicleFiaFlags;    // -1 = invalid/unknown, 0 = none, 1 = green
                                               // 2 = blue, 3 = yellow, 4 = red
            public float m_ersStoreEnergy;           // ERS energy store in Joules
            public byte m_ersDeployMode;            // ERS deployment mode, 0 = none, 1 = medium
                                                    // 2 = hotlap, 3 = overtake
            public float m_ersHarvestedThisLapMGUK;  // ERS energy harvested this lap by MGU-K
            public float m_ersHarvestedThisLapMGUH;  // ERS energy harvested this lap by MGU-H
            public float m_ersDeployedThisLap;       // ERS energy deployed this lap
            public byte m_networkPaused;            // Whether the car is paused in a network game
        };

        /// <summary>
        /// Frequency: Rate as specified in menus
        /// Size: 1058 bytes
        /// Version: 1
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct PacketCarStatusData
        {
            public PacketHeader m_header;     // Header

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 22)]
            public CarStatusData[] m_carStatusData;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct FinalClassificationData
        {
            public byte m_position;              // Finishing position
            public byte m_numLaps;               // Number of laps completed
            public byte m_gridPosition;          // Grid position of the car
            public byte m_points;                // Number of points scored
            public byte m_numPitStops;           // Number of pit stops made
            public byte m_resultStatus;          // Result status - 0 = invalid, 1 = inactive, 2 = active
                                                 // 3 = finished, 4 = didnotfinish, 5 = disqualified
                                                 // 6 = not classified, 7 = retired
            public uint m_bestLapTimeInMS;       // Best lap time of the session in milliseconds
            public double m_totalRaceTime;         // Total race time in seconds without penalties
            public byte m_penaltiesTime;         // Total penalties accumulated in seconds
            public byte m_numPenalties;          // Number of penalties applied to this driver
            public byte m_numTyreStints;         // Number of tyres stints up to maximum
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] m_tyreStintsActual;   // Actual tyres used by this driver
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] m_tyreStintsVisual;   // Visual tyres used by this driver
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] m_tyreStintsEndLaps;  // The lap number stints end on
        };

        /// <summary>
        /// Frequency: Once at the end of a race
        /// Size: 1015 bytes
        /// Version: 1
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct PacketFinalClassificationData
        {
            public PacketHeader m_header;                      // Header

            public byte m_numCars;          // Number of cars in the final classification
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 22)]
            public FinalClassificationData[] m_classificationData;
        };


        [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
        public struct LobbyInfoData
        {
            public byte m_aiControlled;            // Whether the vehicle is AI (1) or Human (0) controlled
            public byte m_teamId;                  // Team id - see appendix (255 if no team currently selected)
            public byte m_nationality;             // Nationality of the driver
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 48)]
            public string m_name;        // Name of participant in UTF-8 format – null terminated
                                         // Will be truncated with ... (U+2026) if too long
            public byte m_carNumber;               // Car number of the player
            public byte m_readyStatus;             // 0 = not ready, 1 = ready, 2 = spectating
        };

        /// <summary>
        /// Frequency: Two every second when in the lobby
        /// Size: 1191 bytes
        /// Version: 1
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct PacketLobbyInfoData
        {
            public PacketHeader m_header;                       // Header

            // Packet specific data
            public byte m_numPlayers;               // Number of players in the lobby data
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 22)]
            public LobbyInfoData[] m_lobbyPlayers;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct CarDamageData
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public float[] m_tyresWear;                     // Tyre wear (percentage)
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] m_tyresDamage;                   // Tyre damage (percentage)
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] m_brakesDamage;                  // Brakes damage (percentage)
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
        }

        /// <summary>
        /// Frequency: 2 per second
        /// Size: 948 bytes
        /// Version: 1 
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct PacketCarDamageData
        {
            public PacketHeader m_header;               // Header

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 22)]
            public CarDamageData[] m_carDamageData;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct LapHistoryData
        {
            public uint m_lapTimeInMS;           // Lap time in milliseconds
            public ushort m_sector1TimeInMS;       // Sector 1 time in milliseconds
            public ushort m_sector2TimeInMS;       // Sector 2 time in milliseconds
            public ushort m_sector3TimeInMS;       // Sector 3 time in milliseconds
            public byte m_lapValidBitFlags;      // 0x01 bit set-lap valid,      0x02 bit set-sector 1 valid
                                                 // 0x04 bit set-sector 2 valid, 0x08 bit set-sector 3 valid
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct TyreStintHistoryData
        {
            public byte m_endLap;                // Lap the tyre usage ends on (255 of current tyre)
            public byte m_tyreActualCompound;    // Actual tyres used by this driver
            public byte m_tyreVisualCompound;    // Visual tyres used by this driver
        };

        /// <summary>
        /// Frequency: 20 per second but cycling through cars
        /// Size: 1155 bytes
        /// Version: 1
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct PacketSessionHistoryData
        {
            public PacketHeader m_header;                   // Header

            public byte m_carIdx;                   // Index of the car this lap data relates to
            public byte m_numLaps;                  // Num laps in the data (including current partial lap)
            public byte m_numTyreStints;            // Number of tyre stints in the data

            public byte m_bestLapTimeLapNum;        // Lap the best lap time was achieved on
            public byte m_bestSector1LapNum;        // Lap the best Sector 1 time was achieved on
            public byte m_bestSector2LapNum;        // Lap the best Sector 2 time was achieved on
            public byte m_bestSector3LapNum;        // Lap the best Sector 3 time was achieved on

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 100)]
            public LapHistoryData[] m_lapHistoryData;   // 100 laps of data max
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public TyreStintHistoryData[] m_tyreStintsHistoryData;
        };
    }
}