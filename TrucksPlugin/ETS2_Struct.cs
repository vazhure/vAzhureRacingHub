using System.Runtime.InteropServices;

/// <summary>
/// https://github.com/Funbit/ets2-sdk-plugin
/// </summary>

namespace TrucksPlugin
{
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public struct ETS2_Struct
    {
        public uint time;
        public uint paused;

        public uint ets2_telemetry_plugin_revision;
        public uint ets2_version_major;
        public uint ets2_version_minor;

        public byte align1;
        public byte align2;

        #region Rev1
        /// <summary>
        /// Is the engine enabled? 
        /// deprecated and removed since rev 5
        /// </summary>
        [MarshalAs(UnmanagedType.I1)] public bool _engine_enabled;
        /// <summary>
        /// Is Trailer attached?
        /// </summary>
        [MarshalAs(UnmanagedType.I1)] public bool trailer_attached;

        /// <summary>
        /// Speed, meters per second
        /// If negative - reverse movement
        /// </summary>
        public float speed;

        /// <summary>
        /// Represents vehicle space linear acceleration of the truck measured
        /// in meters per second^2
        /// </summary>
        public float accelerationX;
        public float accelerationY;
        public float accelerationZ;

        public float coordinateX;
        public float coordinateY;
        public float coordinateZ;

        /// <summary>
        /// Heading
        /// Stored in unit range where <0,1) corresponds to <0,360).
        /// </summary>
        public float Heading;
        /// <summary>
        /// Pitch
        /// Stored in unit range where <-0.25,0.25> corresponds to <-90,90>.
        /// </summary>
        public float Pitch;
        /// <summary>
        /// Roll
        /// Stored in unit range where <-0.5,0.5> corresponds to <-180,180>.
        /// </summary>
        public float Roll;

        /// <summary>
        /// Gear currently selected in the engine.
        /// > 0 - Forward gears
        /// 0 - Neutral
        /// < 0 - Reverse gears
        /// </summary>
        public int gear;
        public int gears;
        public int gearRanges;         //TODO:fix
        public int gearRangeActive;    //TODO:fix

        /// <summary>
        /// RPM of the engine
        /// </summary>
        public float engineRpm;
        public float engineRpmMax;

        /// <summary>
        /// Amount of fuel in liters
        /// </summary>
        public float fuel;
        public float fuelCapacity;
        public float fuelRate;             // ! Not working
        /// <summary>
        /// Average consumption of the fuel in liters/km
        /// </summary>
        public float fuelAvgConsumption;

        /// <summary>
        /// Steering received from input <-1;1>
        /// </summary>
        public float userSteer;
        /// <summary>
        /// Throttle received from input <0;1>
        /// </summary>
        public float userThrottle;
        /// <summary>
        /// Brake received from input <0;1>
        /// </summary>
        public float userBrake;
        /// <summary>
        /// Clutch received from input <0;1>
        /// </summary>
        public float userClutch;

        /// <summary>
        /// Steering as used by the simulation <-1;1>
        /// </summary>
        public float gameSteer;
        /// <summary>
        /// Throttle pedal input as used by the simulation <0;1>
        /// </summary>
        public float gameThrottle;
        /// <summary>
        /// Brake pedal input as used by the simulation <0;1>
        /// </summary>
        public float gameBrake;
        /// <summary>
        /// Clutch pedal input as used by the simulation <0;1>
        /// </summary>
        public float gameClutch;

        // truck & trailer
        public float truckWeight;      //TODO:fix
        public float trailerWeight;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public int[] modelType;
        /// <summary>
        /// deprecated
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public int[] _trailerType;
        #endregion

        public byte align3;
        public byte align4;

        #region Rev2
        public long time_abs;
        public int gears_reverse;

        // Trailer ID & display name
        public float trailerMass;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string trailerId;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string trailerName;

        // Job information
        public int jobIncome;
        public int time_abs_delivery;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string citySrc;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string cityDst;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string compSrc;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string compDst;
        #endregion

        #region Rev3
        public int retarderBrake;
        /// <summary>
        /// Gearbox slot the h-shifter handle is currently in.
        /// </summary>
        public int shifterSlot;
        /// <summary>
        /// Enabled state of range/splitter selector toggles.
        /// </summary>
        public int shifterToggle;
        public int fill;

        /// <summary>
        /// Speed selected for the cruise control in m/s
        /// </summary>
        [MarshalAs(UnmanagedType.I1)] public bool cruiseControl;
        [MarshalAs(UnmanagedType.I1)] public bool wipers;

        /// <summary>
        /// Is the parking brake enabled?
        /// </summary>
        [MarshalAs(UnmanagedType.I1)] public bool parkBrake;
        /// <summary>
        /// Is the motor brake enabled?
        /// </summary>
        [MarshalAs(UnmanagedType.I1)] public bool motorBrake;

        [MarshalAs(UnmanagedType.I1)] public bool electricEnabled;
        [MarshalAs(UnmanagedType.I1)] public bool engineEnabled;

        [MarshalAs(UnmanagedType.I1)] public bool blinkerLeftActive;
        [MarshalAs(UnmanagedType.I1)] public bool blinkerRightActive;
        [MarshalAs(UnmanagedType.I1)] public bool blinkerLeftOn;
        [MarshalAs(UnmanagedType.I1)] public bool blinkerRightOn;

        [MarshalAs(UnmanagedType.I1)] public bool lightsParking;
        [MarshalAs(UnmanagedType.I1)] public bool lightsBeamLow;
        [MarshalAs(UnmanagedType.I1)] public bool lightsBeamHigh;

        public uint lightsAuxFront;
        public uint lightsAuxRoof;

        [MarshalAs(UnmanagedType.I1)] public bool lightsBeacon;
        [MarshalAs(UnmanagedType.I1)] public bool lightsBrake;
        [MarshalAs(UnmanagedType.I1)] public bool lightsReverse;

        [MarshalAs(UnmanagedType.I1)] public bool batteryVoltageWarning;
        /// <summary>
        /// Is the air pressure warning active?
        /// </summary>
        [MarshalAs(UnmanagedType.I1)] public bool airPressureWarning;
        /// <summary>
        /// Are the emergency brakes active as result of low air pressure?
        /// </summary>
        [MarshalAs(UnmanagedType.I1)] public bool airPressureEmergency;
        [MarshalAs(UnmanagedType.I1)] public bool adblueWarning;
        [MarshalAs(UnmanagedType.I1)] public bool oilPressureWarning;
        [MarshalAs(UnmanagedType.I1)] public bool waterTemperatureWarning;

        public float airPressure;
        public float brakeTemperature;
        public int fuelWarning;
        public float adblue;
        public float adblueConsumption;
        public float oilPressure;
        public float oilTemperature;
        public float waterTemperature;
        public float batteryVoltage;
        public float lightsDashboard;
        public float wearEngine;
        public float wearTransmission;
        public float wearCabin;
        public float wearChassis;
        public float wearWheels;
        public float wearTrailer;
        public float truckOdometer;
        public float cruiseControlSpeed;

        // General info about the truck etc;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string truckMake;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string truckMakeId;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string truckModel;
        #endregion


        #region Rev4

        public float fuelWarningFactor;
        public float adblueCapacity;
        public float airPressureWarningValue;
        public float airPressureEmergencyValue;
        public float oilPressureWarningValue;
        public float waterTemperatureWarningValue;
        public float batteryVoltageWarningValue;

        public uint retarderStepCount;

        public float cabinPositionX;
        public float cabinPositionY;
        public float cabinPositionZ;
        public float headPositionX;
        public float headPositionY;
        public float headPositionZ;
        public float hookPositionX;
        public float hookPositionY;
        public float hookPositionZ;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public string shifterType; // "arcade", "automatic", "manual", "hshifter"

        public float localScale; // time scale
        public int nextRestStop; // in minutes
        public float trailerCoordinateX;
        public float trailerCoordinateY;
        public float trailerCoordinateZ;
        public float trailerRotationX;
        public float trailerRotationY;
        public float trailerRotationZ;

        /// <summary>
        /// Gear currently displayed on dashboard
        /// > 0 - Forward gears
        /// 0 - Neutral
        /// < 0 - Reverse gears
        /// </summary>
        public int displayedGear;
        public float navigationDistance;
        public float navigationTime;
        public float navigationSpeedLimit;
        #endregion
    }
}