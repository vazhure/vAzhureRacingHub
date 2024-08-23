using System.Runtime.InteropServices;

/// <summary>
/// https://modding.scssoft.com/wiki/Documentation/Engine/SDK/Telemetry
/// </summary>

namespace TrucksPlugin
{
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public struct ETS2_minimal
    {
        /// <summary>
        /// Is the telemetry running or it is paused?
        /// </summary>
        [MarshalAs(UnmanagedType.U1)] public bool running;
        /// <summary>
        /// Left blinker is on?
        /// </summary>
        [MarshalAs(UnmanagedType.U1)] public bool lblinker;
        /// <summary>
        /// Right blinker is on?
        /// </summary>
        [MarshalAs(UnmanagedType.U1)] public bool rblinker;
        /// <summary>
        /// Low Beam is on?
        /// </summary>
        [MarshalAs(UnmanagedType.U1)] public bool lowBeamLight;
        /// <summary>
        /// Hi Beam is on?
        /// </summary>
        [MarshalAs(UnmanagedType.U1)] public bool hiBeamLight;
        /// <summary>
        /// Parking lights is on?
        /// </summary>
        [MarshalAs(UnmanagedType.U1)] public bool parkingLights;

        /// <summary>
        /// Speed in meters per second 
        /// </summary>
        public float speedometer_speed;
        /// <summary>
        /// RPM
        /// </summary>
        public float rpm;
        /// <summary>
        /// limitation RPM
        /// </summary>
        public float rpmMax;
        /// <summary>
        /// Fuel, liters
        /// </summary>
        public float fuel;
        /// <summary>
        /// Fuel capacity, liters
        /// </summary>
        public float fuelCapacity;
        /// <summary>
        /// Steering [-1..1]
        /// </summary>
        public float steering;
        /// <summary>
        /// Steering Input [0..1]
        /// </summary>
        public float throttle;
        /// <summary>
        /// Brake Input [0..1]
        /// </summary>
        public float brake;
        /// <summary>
        /// Clutch Input [0..1]
        /// </summary>
        public float clutch;
        /// <summary>
        /// The value of truck's navigation eta (in second).
        /// This is the value used by the advisor.
        /// </summary>
        public float navigationTime;

        public int gear;
        /// <summary>
        /// Gear currently displayed on dashboard
        /// > 0 - Forward gears
        /// 0 - Neutral
        /// < 0 - Reverse gears
        /// </summary>
        public int displayedGear;
        /// <summary>
        /// Represented in number of in-game minutes since beginning (i.e. 00:00)
        /// of the first in-game day.
        /// </summary>
        public uint gameTime;
        /// <summary>
        /// Total time spend on the job in game minutes
        /// </summary>
        public uint deliveryTime;

        public struct Fvector
        {
            public float x;
            public float y;
            public float z;
            public static implicit operator float[](Fvector vec3)
            {
                return new float[] { vec3.x, vec3.y, vec3.z };
            }
        }

        public Fvector linear_velocity;
        public Fvector angular_velocity;
        public Fvector linear_acceleration;
        public Fvector angular_acceleration;
        public Fvector cabin_angular_velocity;
        public Fvector cabin_angular_acceleration;

        public struct Dvector
        {
            public double x;
            public double y;
            public double z;

            public static implicit operator double[](Dvector vec3)
            {
                return new double[] { vec3.x, vec3.y, vec3.z };
            }
        }

        public struct Euler
        {
            /// <summary>
            /// Heading.
            /// 
            /// Stored in unit range where <0,1) corresponds to <0,360).
            /// 
            /// The angle is measured counterclockwise in horizontal plane when looking
            /// from top where 0 corresponds to forward (north), 0.25 to left (west),
            /// 0.5 to backward (south) and 0.75 to right (east).
            /// </summary>
            public float heading;

            /// <summary>
            /// Pitch
            /// 
            /// Stored in unit range where <-0.25,0.25> corresponds to <-90,90>.
            /// 
            /// The pitch angle is zero when in horizontal direction,
            /// with positive values pointing up (0.25 directly to zenith),
            /// and negative values pointing down (-0.25 directly to nadir).</summary>
            public float pitch;

            /// <summary>
            /// Roll
            /// 
            /// Stored in unit range where <-0.5,0.5> corresponds to <-180,180>.
            /// 
            /// The angle is measured in counterclockwise when looking in direction of
            /// the roll axis.
            /// </summary>
            public float roll;
        }

        public struct Dplacement
        {
            /// <summary>
            /// Position
            /// </summary>
            public Dvector position;
            /// <summary>
            /// Orientation
            /// </summary>
            public Euler orientation;
            /// <summary>
            /// Explicit padding.
            /// </summary>
            public uint _padding;
        }

        public Dplacement ws_truck_placement;

        // Extra

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)] public string shifterType;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)] public string cargo;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)] public string destinationCity;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)] public string destinationCompany;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)] public string sourceCity;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)] public string sourceCompany;

        /// <summary>
        /// Number of selectors (e.g. range/splitter toggles)
        /// </summary>
        public uint selectorCount;
        /// <summary>
        /// Planned job distance in simulated kilometers.
        /// Does not include distance driven using ferry.
        /// </summary>
        public uint planned_distanceKM;
        /// <summary>
        /// Offset from the game_time simulated in the local economy to the
        /// game time of the Convoy multiplayer server.
        ///
        /// The value of this channel can change frequently during the Convoy
        /// session.For example when the user enters the desktop, the local
        /// economy time stops however the multiplayer time continues to run
        /// so the value will start to change.
        ///
        /// Represented in in-game minutes. Set to 0 when multiplayer is not active.
        /// </summary>
        public uint multiplayerTimeOffset;
        /// <summary>
        /// Time until next rest stop.
        ///
        /// When the fatigue simulation is disabled, the behavior of this channel
        /// is implementation dependent. The game might provide the value which would
        /// apply if it was enabled or provide no value at all.
        ///
        /// Represented in in-game minutes
        /// </summary>
        public uint restStop;
        /// <summary>
        /// Scale applied to distance and time to compensate
        /// for the scale of the map(e.g. 1s of real time corresponds to local_scale
        /// seconds of simulated game time).
        ///
        /// Games which use real 1:1 maps will not provide this
        /// channel
        /// </summary>
        public float localScale;
        /// <summary>
        ///  AdBlue tank capacity in liters
        /// </summary>
        public float adBlueFuelCapacity;
        /// <summary>
        /// Amount of AdBlue in liters
        /// </summary>
        public float truckAdblueFuelLevelLiters;
        /// <summary>
        /// Average consumption of the adblue in liters/km
        /// </summary>
        public float truckFuelConsumptionAverageLiters;
        /// <summary>
        /// Speed selected for the cruise control in m/s
        ///
        /// Is zero if cruise control is disabled.
        /// </summary>
        public float truckCruise_controlSpeedMS;
        /// <summary>
        /// Estimated range of truck with current amount of fuel in km
        /// </summary>
        public float truckFuelRangeKm;
        /// <summary>
        /// Voltage of the battery in volts
        /// </summary>
        public float truckBatteryVoltage;
        /// <summary>
        /// The value of the odometer in km
        /// </summary>
        public float truckOdometerKM;
        /// <summary>
        /// The value of truck's navigation distance (in meters).
        ///
        /// This is the value used by the advisor
        /// </summary>
        public float truckNavigationDistanceMeters;
        /// <summary>
        /// The value of truck's navigation eta (in second).
        ///
        /// This is the value used by the advisor.
        /// </summary>
        public float truckNavigationTimeSeconds;
        /// <summary>
        /// The value of truck's navigation speed limit (in m/s).
        /// 
        /// This is the value used by the advisor and respects the
        /// current state of the "Route Advisor speed limit" option.
        /// </summary>
        public float truckNavigationSpeedLimitMS;
        /// <summary>
        /// Pressure of the oil in psi
        /// </summary>
        public float truckOilPressure;
        /// <summary>
        /// Temperature of the oil in degrees celsius
        /// </summary>
        public float truckOilTemperature;
        /// <summary>
        /// Temperature of the water in degrees celsius
        /// </summary>
        public float truckWaterTemperature;

        /// <summary>
        /// Is the parking brake enabled?
        /// </summary>
        [MarshalAs(UnmanagedType.U1)] public bool truckBrakeParking;
        /// <summary>
        /// Is the engine brake enabled?
        /// </summary>
        [MarshalAs(UnmanagedType.U1)] public bool truckBrakeMotor;
        /// <summary>
        /// Is the low fuel warning active?
        /// </summary>
        [MarshalAs(UnmanagedType.U1)] public bool truckFuelWarning;
        /// <summary>
        /// Is the battery voltage/not charging warning active?
        /// </summary>
        [MarshalAs(UnmanagedType.U1)] public bool truckBatteryVoltageWarning;
        /// <summary>
        /// Is the electric enabled?
        /// </summary>
        [MarshalAs(UnmanagedType.U1)] public bool truckElectricEnabled;
        /// <summary>
        /// Is the engine enabled?
        /// </summary>
        [MarshalAs(UnmanagedType.U1)] public bool truckEngineEnabled;
        /// <summary>
        /// Are the hazard warning light enabled?
        ///
        /// This represents the logical enable state of the hazard warning.
        /// If it is true as long it is enabled regardless of the physical
        /// enabled state of the light (i.e.it does not blink).
        /// </summary>
        [MarshalAs(UnmanagedType.U1)] public bool truckHazardWarning;
        /// <summary>
        /// Are the wipers enabled?
        /// </summary>
        [MarshalAs(UnmanagedType.U1)] public bool truckWipers;
    }
}