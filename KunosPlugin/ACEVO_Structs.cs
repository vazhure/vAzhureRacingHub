using System.Runtime.InteropServices;

namespace Kunos.Structs
{
    /// <summary>
    /// Memory-Mapped File names for Assetto Corsa EVO shared memory interface.
    /// </summary>
    public static class ACEVOConstants
    {
        public const string PhysicsFile = @"Local\acpmf_evo_physics";
        public const string GraphicsFile = @"Local\acpmf_evo_graphics";
        public const string StaticFile = @"Local\acpmf_evo_static";
    }

    #region Enums
    /// <summary>
    /// Current operational state of the simulator.
    /// </summary>
    public enum ACEVO_STATUS
    {
        /// <summary>Simulator is not running / no session active</summary>
        AC_OFF = 0,
        /// <summary>A replay is currently being played back</summary>
        AC_REPLAY = 1,
        /// <summary>Live driving session is active</summary>
        AC_LIVE = 2,
        /// <summary>Session is paused</summary>
        AC_PAUSE = 3
    }

    /// <summary>
    /// Type of racing session currently loaded.
    /// </summary>
    public enum ACEVO_SESSION_TYPE
    {
        /// <summary>Session type not yet determined</summary>
        AC_UNKNOWN = -1,
        /// <summary>Time attack / qualifying session</summary>
        AC_TIME_ATTACK = 0,
        /// <summary>Race session</summary>
        AC_RACE = 1,
        /// <summary>Hot-stint practice</summary>
        AC_HOT_STINT = 2,
        /// <summary>Untimed cruise</summary>
        AC_CRUISE = 3
    }

    /// <summary>
    /// Race flag currently shown to the driver.
    /// </summary>
    public enum ACEVO_FLAG_TYPE
    {
        /// <summary>No flag displayed</summary>
        AC_NO_FLAG = 0,
        /// <summary>Slow vehicle ahead on track</summary>
        AC_WHITE_FLAG = 1,
        /// <summary>Track clear — racing resumed</summary>
        AC_GREEN_FLAG = 2,
        /// <summary>Session stopped due to incident or hazard</summary>
        AC_RED_FLAG = 3,
        /// <summary>Lapped car must yield to the race leader</summary>
        AC_BLUE_FLAG = 4,
        /// <summary>Hazard present — no overtaking</summary>
        AC_YELLOW_FLAG = 5,
        /// <summary>Driver disqualified / must pit immediately</summary>
        AC_BLACK_FLAG = 6,
        /// <summary>Warning for unsportsmanlike behaviour</summary>
        AC_BLACK_WHITE_FLAG = 7,
        /// <summary>Session or race has ended</summary>
        AC_CHECKERED_FLAG = 8,
        /// <summary>Mechanical problem — car must pit</summary>
        AC_ORANGE_CIRCLE_FLAG = 9,
        /// <summary>Slippery surface ahead on track</summary>
        AC_RED_YELLOW_STRIPES_FLAG = 10
    }

    /// <summary>
    /// Where on the circuit the car is currently positioned.
    /// </summary>
    public enum ACEVO_CAR_LOCATION
    {
        /// <summary>Position not yet determined</summary>
        ACEVO_UNASSIGNED = 0,
        /// <summary>Car is inside the pit lane</summary>
        ACEVO_PITLANE = 1,
        /// <summary>Car is at the pit-lane entry</summary>
        ACEVO_PITENTRY = 2,
        /// <summary>Car is at the pit-lane exit</summary>
        ACEVO_PITEXIT = 3,
        /// <summary>Car is on the racing circuit</summary>
        ACEVO_TRACK = 4
    }

    /// <summary>
    /// Powertrain type of the player car.
    /// </summary>
    public enum ACEVO_ENGINE_TYPE
    {
        /// <summary>Traditional petrol/diesel internal combustion engine</summary>
        ACEVO_INTERNAL_COMBUSTION = 0,
        /// <summary>Fully electric powertrain</summary>
        ACEVO_ELECTRIC_MOTOR = 1
    }

    /// <summary>
    /// Initial grip conditions at session start.
    /// </summary>
    public enum ACEVO_STARTING_GRIP
    {
        /// <summary>Track grip at minimum</summary>
        ACEVO_GREEN = 0,
        /// <summary>Track grip in advanced (fast) stage</summary>
        ACEVO_FAST = 1,
        /// <summary>Track conditions starting at optimum grip</summary>
        ACEVO_OPTIMUM = 2
    }
    #endregion

    #region Nested Structures
    /// <summary>
    /// Complete state of a single tyre corner. Embedded four times in SPageFileGraphicEvo (lf, rf, lr, rr).
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public struct SMEvoTyreState
    {
        /// <summary>Combined tyre slip magnitude</summary>
        public float slip;
        /// <summary>Tyre is locked under braking (true = locking)</summary>
        [MarshalAs(UnmanagedType.I1)] public bool _lock;
        /// <summary>Tyre inflation pressure (PSI)</summary>
        public float tyre_pression;
        /// <summary>Average tyre carcass temperature in °C</summary>
        public float tyre_temperature_c;
        /// <summary>Brake disc temperature in °C</summary>
        public float brake_temperature_c;
        /// <summary>Hydraulic brake pressure applied at this corner</summary>
        public float brake_pressure;
        /// <summary>Inner-edge tyre temperature in °C</summary>
        public float tyre_temperature_left;
        /// <summary>Centre-tread tyre temperature in °C</summary>
        public float tyre_temperature_center;
        /// <summary>Outer-edge tyre temperature in °C</summary>
        public float tyre_temperature_right;
        /// <summary>Name of the compound fitted on the front axle</summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 33)] public string tyre_compound_front;
        /// <summary>Name of the compound fitted on the rear axle</summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 33)] public string tyre_compound_rear;
        /// <summary>Pressure as a 0–1 fraction of the target range</summary>
        public float tyre_normalized_pressure;
        /// <summary>Inner-edge temperature as a 0–1 fraction of optimal range</summary>
        public float tyre_normalized_temperature_left;
        /// <summary>Centre temperature as a 0–1 fraction of optimal range</summary>
        public float tyre_normalized_temperature_center;
        /// <summary>Outer-edge temperature as a 0–1 fraction of optimal range</summary>
        public float tyre_normalized_temperature_right;
        /// <summary>Brake temperature as a 0–1 fraction of optimal operating range</summary>
        public float brake_normalized_temperature;
        /// <summary>Core tyre temperature as a 0–1 fraction of optimal range</summary>
        public float tyre_normalized_temperature_core;
    }

    /// <summary>
    /// Structural damage level for each body zone of the car (0.0 = undamaged, 1.0 = destroyed).
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SMEvoDamageState
    {
        /// <summary>Damage on the front body / nose</summary>
        public float damage_front;
        /// <summary>Damage on the rear body / diffuser</summary>
        public float damage_rear;
        /// <summary>Damage on the left side of the body</summary>
        public float damage_left;
        /// <summary>Damage on the right side of the body</summary>
        public float damage_right;
        /// <summary>Damage on the central / underfloor area</summary>
        public float damage_center;
        /// <summary>Damage on the front-left suspension</summary>
        public float damage_suspension_lf;
        /// <summary>Damage on the front-right suspension</summary>
        public float damage_suspension_rf;
        /// <summary>Damage on the rear-left suspension</summary>
        public float damage_suspension_lr;
        /// <summary>Damage on the rear-right suspension</summary>
        public float damage_suspension_rr;
    }

    /// <summary>
    /// Status of each pit-stop service action. −1 = will not perform, 0 = completed, 1 = in progress.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SMEvoPitInfo
    {
        /// <summary>Body-repair action state</summary>
        public sbyte damage;
        /// <summary>Refuelling action state</summary>
        public sbyte fuel;
        /// <summary>Front-left tyre change state</summary>
        public sbyte tyres_lf;
        /// <summary>Front-right tyre change state</summary>
        public sbyte tyres_rf;
        /// <summary>Rear-left tyre change state</summary>
        public sbyte tyres_lr;
        /// <summary>Rear-right tyre change state</summary>
        public sbyte tyres_rr;
    }

    /// <summary>
    /// All driver-adjustable electronic aid and setup settings.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SMEvoElectronics
    {
        /// <summary>Traction-control level (0 = off, higher = more aggressive)</summary>
        public sbyte tc_level;
        /// <summary>TC throttle-cut aggressiveness level</summary>
        public sbyte tc_cut_level;
        /// <summary>ABS intervention level (0 = off)</summary>
        public sbyte abs_level;
        /// <summary>Electronic stability-control level (0 = off)</summary>
        public sbyte esc_level;
        /// <summary>Electronic brake-balance adjustment level</summary>
        public sbyte ebb_level;
        /// <summary>Front brake-bias ratio (e.g. 0.56 = 56% front)</summary>
        public float brake_bias;
        /// <summary>Engine map / power mode selection</summary>
        public sbyte engine_map_level;
        /// <summary>Turbo wastegate or boost target setting</summary>
        public float turbo_level;
        /// <summary>ERS power-deployment strategy map</summary>
        public sbyte ers_deployment_map;
        /// <summary>ERS recharge aggressiveness setting</summary>
        public float ers_recharge_map;
        /// <summary>ERS heat-based charging is enabled</summary>
        [MarshalAs(UnmanagedType.I1)] public bool is_ers_heat_charging_on;
        /// <summary>ERS overtake (maximum-deploy) mode is active</summary>
        [MarshalAs(UnmanagedType.I1)] public bool is_ers_overtake_mode_on;
        /// <summary>DRS flap is currently open</summary>
        [MarshalAs(UnmanagedType.I1)] public bool is_drs_open;
        /// <summary>Differential lock level under power</summary>
        public sbyte diff_power_level;
        /// <summary>Differential lock level on lift / coast</summary>
        public sbyte diff_coast_level;
        /// <summary>Front bump (compression) damper stiffness level</summary>
        public sbyte front_bump_damper_level;
        /// <summary>Front rebound damper stiffness level</summary>
        public sbyte front_rebound_damper_level;
        /// <summary>Rear bump (compression) damper stiffness level</summary>
        public sbyte rear_bump_damper_level;
        /// <summary>Rear rebound damper stiffness level</summary>
        public sbyte rear_rebound_damper_level;
        /// <summary>Ignition switch is on</summary>
        [MarshalAs(UnmanagedType.I1)] public bool is_ignition_on;
        /// <summary>Pit-speed limiter is active</summary>
        [MarshalAs(UnmanagedType.I1)] public bool is_pitlimiter_on;
        /// <summary>Selected vehicle performance / power mode index</summary>
        public sbyte active_performance_mode;
    }

    /// <summary>
    /// Cockpit light, display, and instrumentation panel states.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SMEvoInstrumentation
    {
        /// <summary>Main exterior light stage (0 = off)</summary>
        public sbyte main_light_stage;
        /// <summary>Auxiliary / special lights level</summary>
        public sbyte special_light_stage;
        /// <summary>Interior cockpit illumination level</summary>
        public sbyte cockpit_light_stage;
        /// <summary>Windscreen wiper speed (0 = off)</summary>
        public sbyte wiper_level;
        /// <summary>Rear rain light is on</summary>
        [MarshalAs(UnmanagedType.I1)] public bool rain_lights;
        /// <summary>Left turn indicator is active</summary>
        [MarshalAs(UnmanagedType.I1)] public bool direction_light_left;
        /// <summary>Right turn indicator is active</summary>
        [MarshalAs(UnmanagedType.I1)] public bool direction_light_right;
        /// <summary>Flashing lights are active</summary>
        [MarshalAs(UnmanagedType.I1)] public bool flashing_lights;
        /// <summary>Hazard lights are illuminated</summary>
        [MarshalAs(UnmanagedType.I1)] public bool warning_lights;
        /// <summary>Index of the currently focused display device</summary>
        public sbyte selected_display_index;
        /// <summary>Active page index on displays</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)] public byte[] display_current_page_index;
        /// <summary>Headlights are on and visible to other drivers</summary>
        [MarshalAs(UnmanagedType.I1)] public bool are_headlights_visible;
    }

    /// <summary>
    /// Server-side session lifecycle information.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public struct SMEvoSessionState
    {
        /// <summary>Name of the current session phase (e.g. 'Race', 'Qualify')</summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 33)] public string phase_name;
        /// <summary>Formatted remaining session time (HH:MM:SS)</summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 15)] public string time_left;
        /// <summary>Remaining session time in milliseconds</summary>
        public int time_left_ms;
        /// <summary>Formatted wait time before session start</summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 15)] public string wait_time;
        /// <summary>Total laps scheduled for this session</summary>
        public int total_lap;
        /// <summary>Current lap number being driven</summary>
        public int current_lap;
        /// <summary>Number of starting lights currently illuminated</summary>
        public int lights_on;
        /// <summary>Starting-light sequence mode identifier</summary>
        public int lights_mode;
        /// <summary>Track lap length in kilometres</summary>
        public float lap_length_km;
        /// <summary>Non-zero when the session is ending</summary>
        public int end_session_flag;
        /// <summary>Formatted countdown to the next session</summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 15)] public string time_to_next_session;
        /// <summary>Player has lost connection to the game server</summary>
        [MarshalAs(UnmanagedType.I1)] public bool disconnected_from_server;
        /// <summary>Season restart option is available to the player</summary>
        [MarshalAs(UnmanagedType.I1)] public bool restart_season_enabled;
        /// <summary>Drive button is enabled in the UI</summary>
        [MarshalAs(UnmanagedType.I1)] public bool ui_enable_drive;
        /// <summary>Setup screen is accessible from the UI</summary>
        [MarshalAs(UnmanagedType.I1)] public bool ui_enable_setup;
        /// <summary>Ready-to-proceed indicator is blinking</summary>
        [MarshalAs(UnmanagedType.I1)] public bool is_ready_to_next_blinking;
        /// <summary>Waiting-for-players lobby screen is shown</summary>
        [MarshalAs(UnmanagedType.I1)] public bool show_waiting_for_players;
    }

    /// <summary>
    /// Lap timing and delta values displayed on the HUD.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public struct SMEvoTimingState
    {
        /// <summary>Current lap time as a formatted string</summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 15)] public string current_laptime;
        /// <summary>Delta vs. current reference lap (formatted)</summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 15)] public string delta_current;
        /// <summary>Sign of delta_current: +1 slower, −1 faster, 0 = hidden</summary>
        public int delta_current_p;
        /// <summary>Last completed lap time as a formatted string</summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 15)] public string last_laptime;
        /// <summary>Delta vs. last lap (formatted)</summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 15)] public string delta_last;
        /// <summary>Sign of delta_last: +1 slower, −1 faster, 0 = hidden</summary>
        public int delta_last_p;
        /// <summary>Personal best lap time as a formatted string</summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 15)] public string best_laptime;
        /// <summary>Theoretical best lap (sum of best sectors) as a formatted string</summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 15)] public string ideal_laptime;
        /// <summary>Total elapsed session time as a formatted string</summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 15)] public string total_time;
        /// <summary>Current lap has been invalidated (track-limits violation, etc.)</summary>
        [MarshalAs(UnmanagedType.I1)] public bool is_invalid;
    }

    /// <summary>
    /// Driver-assist settings currently active for the player car.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SMEvoAssistsState
    {
        /// <summary>Automatic gearshift aid level (0 = off)</summary>
        public byte auto_gear;
        /// <summary>Automatic throttle blip on downshift (0 = off)</summary>
        public byte auto_blip;
        /// <summary>Automatic clutch management (0 = off)</summary>
        public byte auto_clutch;
        /// <summary>Automatic clutch during the rolling start (0 = off)</summary>
        public byte auto_clutch_on_start;
        /// <summary>Manual ignition and electric start required (0 = automatic)</summary>
        public byte manual_ignition_e_start;
        /// <summary>Pit-speed limiter activates automatically (0 = manual)</summary>
        public byte auto_pit_limiter;
        /// <summary>Standing-start launch assistance active (0 = off)</summary>
        public byte standing_start_assist;
        /// <summary>Auto-steer correction strength (0.0 = off, 1.0 = maximum)</summary>
        public float auto_steer;
        /// <summary>Arcade-style stability aid level (0.0 = off, 1.0 = maximum)</summary>
        public float arcade_stability_control;
    }
    #endregion

    #region Main Structures
    /// <summary>
    /// Raw physics telemetry updated every simulation step. Contains all low-level vehicle dynamics data.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SPageFilePhysics_EVO
    {
        /// <summary>Incrementing counter — detect new data packets by comparing to previous value</summary>
        public int packetId;
        /// <summary>Throttle pedal position (0.0 = released, 1.0 = full throttle)</summary>
        public float gas;
        /// <summary>Brake pedal position (0.0 = released, 1.0 = full brake)</summary>
        public float brake;
        /// <summary>Remaining fuel in litres</summary>
        public float fuel;
        /// <summary>Engaged gear: 0 = reverse, 1 = neutral, 2+ = forward gears</summary>
        public int gear;
        /// <summary>Engine speed in revolutions per minute</summary>
        public int rpms;
        /// <summary>Normalised steering angle (−1.0 = full left, +1.0 = full right)</summary>
        public float steerAngle;
        /// <summary>Vehicle speed in km/h</summary>
        public float speedKmh;
        /// <summary>World-space velocity vector [X, Y, Z] in m/s</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)] public float[] velocity;
        /// <summary>Acceleration in G [lateral X, longitudinal Y, vertical Z]</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)] public float[] accG;
        /// <summary>Tyre slip value per wheel [FL, FR, RL, RR]</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)] public float[] wheelSlip;
        /// <summary>Vertical tyre load in Newtons [FL, FR, RL, RR]</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)] public float[] wheelLoad;
        /// <summary>Tyre inflation pressure in PSI [FL, FR, RL, RR]</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)] public float[] wheelsPressure;
        /// <summary>Wheel rotational speed in rad/s [FL, FR, RL, RR]</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)] public float[] wheelAngularSpeed;
        /// <summary>Tyre wear level (0.0 = new, 1.0 = fully worn) [FL, FR, RL, RR]</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)] public float[] tyreWear;
        /// <summary>Amount of dirt/debris on each tyre surface [FL, FR, RL, RR]</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)] public float[] tyreDirtyLevel;
        /// <summary>Core temperature of each tyre in °C [FL, FR, RL, RR]</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)] public float[] tyreCoreTemperature;
        /// <summary>Wheel camber angle in radians per corner [FL, FR, RL, RR]</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)] public float[] camberRAD;
        /// <summary>Suspension compression travel in metres [FL, FR, RL, RR]</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)] public float[] suspensionTravel;
        /// <summary>DRS flap state (0.0 = closed, 1.0 = fully open)</summary>
        public float drs;
        /// <summary>Traction control cut intensity (0.0 = inactive, 1.0 = maximum)</summary>
        public float tc;
        /// <summary>Vehicle heading relative to world north in radians</summary>
        public float heading;
        /// <summary>Chassis pitch angle in radians (positive = nose up)</summary>
        public float pitch;
        /// <summary>Chassis roll angle in radians (positive = right side down)</summary>
        public float roll;
        /// <summary>Height of the centre of gravity above the ground in metres</summary>
        public float cgHeight;
        /// <summary>Damage level per body zone [front, rear, left, right, centre] (0.0–1.0)</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)] public float[] carDamage;
        /// <summary>Number of tyres currently outside track limits</summary>
        public int numberOfTyresOut;
        /// <summary>Pit-speed limiter active (0 = off, 1 = on)</summary>
        public int pitLimiterOn;
        /// <summary>ABS intervention intensity (0.0 = inactive, 1.0 = fully active)</summary>
        public float abs;
        /// <summary>KERS/ERS battery state of charge (0.0–1.0)</summary>
        public float kersCharge;
        /// <summary>KERS/ERS power delivery level currently being deployed (0.0–1.0)</summary>
        public float kersInput;
        /// <summary>Automatic gearshift aid active (0 = manual, 1 = auto)</summary>
        public int autoShifterOn;
        /// <summary>Ride height at front and rear axle in metres [front, rear]</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)] public float[] rideHeight;
        /// <summary>Current turbo boost pressure in bar</summary>
        public float turboBoost;
        /// <summary>Additional ballast added to the car in kg</summary>
        public float ballast;
        /// <summary>Ambient air density in kg/m³</summary>
        public float airDensity;
        /// <summary>Ambient air temperature in °C</summary>
        public float airTemp;
        /// <summary>Road surface temperature in °C</summary>
        public float roadTemp;
        /// <summary>Angular velocity in the car's local frame [pitch, yaw, roll] in rad/s</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)] public float[] localAngularVel;
        /// <summary>Final force-feedback torque value sent to the wheel (Nm)</summary>
        public float finalFF;
        /// <summary>Real-time delta vs. best lap (positive = ahead of reference)</summary>
        public float performanceMeter;
        /// <summary>Engine-braking setting level (higher = more engine braking)</summary>
        public int engineBrake;
        /// <summary>ERS energy-recovery intensity level</summary>
        public int ersRecoveryLevel;
        /// <summary>ERS power-deployment level</summary>
        public int ersPowerLevel;
        /// <summary>ERS heat-charging mode active (0 = off, 1 = on)</summary>
        public int ersHeatCharging;
        /// <summary>ERS currently recovering energy (0 = deploying, 1 = charging)</summary>
        public int ersIsCharging;
        /// <summary>Energy stored in the KERS/ERS battery in kilojoules</summary>
        public float kersCurrentKJ;
        /// <summary>DRS can be activated (0 = no, 1 = yes)</summary>
        public int drsAvailable;
        /// <summary>DRS is open and active (0 = closed, 1 = open)</summary>
        public int drsEnabled;
        /// <summary>Brake disc temperature per corner in °C [FL, FR, RL, RR]</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)] public float[] brakeTemp;
        /// <summary>Clutch pedal position (0.0 = engaged, 1.0 = fully disengaged)</summary>
        public float clutch;
        /// <summary>Tyre inner-edge temperature per wheel in °C [FL, FR, RL, RR]</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)] public float[] tyreTempI;
        /// <summary>Tyre mid-tread temperature per wheel in °C [FL, FR, RL, RR]</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)] public float[] tyreTempM;
        /// <summary>Tyre outer-edge temperature per wheel in °C [FL, FR, RL, RR]</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)] public float[] tyreTempO;
        /// <summary>Car is driven by AI (0 = player, 1 = AI)</summary>
        [MarshalAs(UnmanagedType.I1)] public bool isAIControlled;
        /// <summary>3-D world-space contact point of each tyre with the road [FL,FR,RL,RR][X,Y,Z]</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)] public float[] tyreContactPoint;
        /// <summary>Road-surface normal vector at each tyre contact point [FL,FR,RL,RR][X,Y,Z]</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)] public float[] tyreContactNormal;
        /// <summary>Heading vector at each tyre contact point [FL,FR,RL,RR][X,Y,Z]</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)] public float[] tyreContactHeading;
        /// <summary>Front brake-bias ratio (e.g. 0.56 = 56% front)</summary>
        public float brakeBias;
        /// <summary>Velocity in the car's local reference frame [X, Y, Z] in m/s</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)] public float[] localVelocity;
        /// <summary>Remaining Push-to-Pass activations</summary>
        public int P2PActivations;
        /// <summary>Push-to-Pass status (0 = inactive, 1 = active)</summary>
        public int P2PStatus;
        /// <summary>Current rev-limiter ceiling in RPM</summary>
        public int currentMaxRpm;
        /// <summary>Self-aligning tyre torque (Mz) per wheel [FL, FR, RL, RR] in Nm</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)] public float[] mz;
        /// <summary>Longitudinal tyre force (Fx) per wheel [FL, FR, RL, RR] in N</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)] public float[] fx;
        /// <summary>Lateral tyre force (Fy) per wheel [FL, FR, RL, RR] in N</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)] public float[] fy;
        /// <summary>Longitudinal slip ratio per tyre [FL, FR, RL, RR]</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)] public float[] slipRatio;
        /// <summary>Lateral slip angle per tyre in radians [FL, FR, RL, RR]</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)] public float[] slipAngle;
        /// <summary>Traction control currently cutting power (0 = no, 1 = yes)</summary>
        [MarshalAs(UnmanagedType.I1)] public bool tcinAction;
        /// <summary>ABS currently modulating brakes (0 = no, 1 = yes)</summary>
        [MarshalAs(UnmanagedType.I1)] public bool absInAction;
        /// <summary>Suspension structural damage per corner (0.0–1.0) [FL, FR, RL, RR]</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)] public float[] suspensionDamage;
        /// <summary>Representative tyre surface temperature per wheel in °C [FL, FR, RL, RR]</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)] public float[] tyreTemp;
        /// <summary>Engine coolant temperature in °C</summary>
        public float waterTemp;
        /// <summary>Braking torque at each wheel in Nm [FL, FR, RL, RR]</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)] public float[] brakeTorque;
        /// <summary>Front brake-pad compound identifier</summary>
        public int frontBrakeCompound;
        /// <summary>Rear brake-pad compound identifier</summary>
        public int rearBrakeCompound;
        /// <summary>Brake-pad remaining life per corner (0.0–1.0) [FL, FR, RL, RR]</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)] public float[] padLife;
        /// <summary>Brake-disc remaining life per corner (0.0–1.0) [FL, FR, RL, RR]</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)] public float[] discLife;
        /// <summary>Ignition switch state (0 = off, 1 = on)</summary>
        [MarshalAs(UnmanagedType.I1)] public bool ignitionOn;
        /// <summary>Starter motor currently cranking (0 = no, 1 = yes)</summary>
        [MarshalAs(UnmanagedType.I1)] public bool starterEngineOn;
        /// <summary>Engine is running (0 = stopped, 1 = running)</summary>
        [MarshalAs(UnmanagedType.I1)] public bool isEngineRunning;
        /// <summary>Vibration intensity transmitted from kerb strikes</summary>
        public float kerbVibration;
        /// <summary>Vibration intensity caused by tyre slip</summary>
        public float slipVibrations;
        /// <summary>Vibration intensity from road surface texture</summary>
        public float roadVibrations;
        /// <summary>Vibration intensity generated by ABS pulsing</summary>
        public float absVibrations;

        /// <summary>Deserializes the structure from a raw byte array.</summary>
        public static SPageFilePhysics_EVO FromBytes(byte[] bytes)
        {
            var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            try { return (SPageFilePhysics_EVO)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(SPageFilePhysics_EVO)); }
            catch { return default; }
            finally { handle.Free(); }
        }
    }

    /// <summary>
    /// Main HUD and graphics telemetry page. Updated each rendered frame. Contains embedded sub-structs for tyres, damage, electronics, timing, and session state.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public struct SPageFileGraphicEvo
    {
        /// <summary>Incrementing counter — detect new frames by comparing to previous value</summary>
        public int packetId;
        /// <summary>Current simulator operational state (see ACEVO_STATUS)</summary>
        public ACEVO_STATUS status;
        /// <summary>Unique ID of the car currently shown by the camera (Part A)</summary>
        public ulong focused_car_id_a;
        /// <summary>Unique ID of the car currently shown by the camera (Part B)</summary>
        public ulong focused_car_id_b;
        /// <summary>Unique ID of the player's own car (Part A)</summary>
        public ulong player_car_id_a;
        /// <summary>Unique ID of the player's own car (Part B)</summary>
        public ulong player_car_id_b;
        /// <summary>Engine speed in RPM for HUD display</summary>
        public short rpm;
        /// <summary>Rev limiter is cutting fuel/ignition (bouncing off limiter)</summary>
        [MarshalAs(UnmanagedType.I1)] public bool is_rpm_limiter_on;
        /// <summary>Engine RPM is in the upshift window</summary>
        [MarshalAs(UnmanagedType.I1)] public bool is_change_up_rpm;
        /// <summary>Engine RPM is in the downshift window</summary>
        [MarshalAs(UnmanagedType.I1)] public bool is_change_down_rpm;
        /// <summary>Traction control is actively intervening this frame</summary>
        [MarshalAs(UnmanagedType.I1)] public bool tc_active;
        /// <summary>ABS is actively modulating brake pressure this frame</summary>
        [MarshalAs(UnmanagedType.I1)] public bool abs_active;
        /// <summary>Electronic stability control is intervening this frame</summary>
        [MarshalAs(UnmanagedType.I1)] public bool esc_active;
        /// <summary>Launch control system is engaged</summary>
        [MarshalAs(UnmanagedType.I1)] public bool launch_active;
        /// <summary>Ignition switch is on</summary>
        [MarshalAs(UnmanagedType.I1)] public bool is_ignition_on;
        /// <summary>Engine is running</summary>
        [MarshalAs(UnmanagedType.I1)] public bool is_engine_running;
        /// <summary>KERS/ERS battery is currently being charged</summary>
        [MarshalAs(UnmanagedType.I1)] public bool kers_is_charging;
        /// <summary>Car is travelling in the wrong direction on track</summary>
        [MarshalAs(UnmanagedType.I1)] public bool is_wrong_way;
        /// <summary>DRS activation is permitted in this section</summary>
        [MarshalAs(UnmanagedType.I1)] public bool is_drs_available;
        /// <summary>High-voltage battery pack is in charging state</summary>
        [MarshalAs(UnmanagedType.I1)] public bool battery_is_charging;
        /// <summary>Maximum ERS deployment energy for this lap has been consumed</summary>
        [MarshalAs(UnmanagedType.I1)] public bool is_max_kj_per_lap_reached;
        /// <summary>Maximum ERS charge energy for this lap has been stored</summary>
        [MarshalAs(UnmanagedType.I1)] public bool is_max_charge_kj_per_lap_reached;
        /// <summary>Displayed speed in km/h</summary>
        public short display_speed_kmh;
        /// <summary>Displayed speed in mph</summary>
        public short display_speed_mph;
        /// <summary>Displayed speed in m/s</summary>
        public short display_speed_ms;
        /// <summary>Speed delta vs. pit-lane limit (negative = under limit)</summary>
        public float pitspeeding_delta;
        /// <summary>Current gear as an integer (same encoding as physics gear)</summary>
        public short gear_int;
        /// <summary>Engine RPM as a fraction of redline (0.0–1.0)</summary>
        public float rpm_percent;
        /// <summary>Throttle pedal position as a fraction (0.0–1.0)</summary>
        public float gas_percent;
        /// <summary>Brake pressure as a fraction (0.0–1.0)</summary>
        public float brake_percent;
        /// <summary>Handbrake engagement as a fraction (0.0–1.0)</summary>
        public float handbrake_percent;
        /// <summary>Clutch disengagement as a fraction (1.0–0.0)</summary>
        public float clutch_percent;
        /// <summary>Steering wheel position (−1.0 = full left, +1.0 = full right)</summary>
        public float steering_percent;
        /// <summary>Global force-feedback output strength</summary>
        public float ffb_strength;
        /// <summary>Per-car force-feedback gain multiplier</summary>
        public float car_ffb_multiplier;
        /// <summary>Coolant temperature as a fraction of optimal operating range</summary>
        public float water_temperature_percent;
        /// <summary>Coolant system pressure in bar</summary>
        public float water_pressure_bar;
        /// <summary>Fuel system pressure in bar</summary>
        public float fuel_pressure_bar;
        /// <summary>Coolant temperature in °C</summary>
        public sbyte water_temperature_c;
        /// <summary>Ambient air temperature in °C</summary>
        public sbyte air_temperature_c;
        /// <summary>Engine oil temperature in °C</summary>
        public float oil_temperature_c;
        /// <summary>Engine oil pressure in bar</summary>
        public float oil_pressure_bar;
        /// <summary>Exhaust gas temperature in °C</summary>
        public float exhaust_temperature_c;
        /// <summary>Lateral G-force (positive = rightward)</summary>
        public float g_forces_x;
        /// <summary>Longitudinal G-force (positive = under acceleration)</summary>
        public float g_forces_y;
        /// <summary>Vertical G-force (positive = upward)</summary>
        public float g_forces_z;
        /// <summary>Absolute turbo boost pressure in bar</summary>
        public float turbo_boost;
        /// <summary>Current boost stage or map level</summary>
        public float turbo_boost_level;
        /// <summary>Turbo boost as a fraction of maximum (0.0–1.0)</summary>
        public float turbo_boost_perc;
        /// <summary>Steering wheel rotation in degrees from centre</summary>
        public int steer_degrees;
        /// <summary>Distance driven in the current session in km</summary>
        public float current_km;
        /// <summary>Total odometer / career distance in km</summary>
        public uint total_km;
        /// <summary>Total driving time accumulated in seconds</summary>
        public uint total_driving_time_s;
        /// <summary>In-game time of day — hours (0–23)</summary>
        public int time_of_day_hours;
        /// <summary>In-game time of day — minutes (0–59)</summary>
        public int time_of_day_minutes;
        /// <summary>In-game time of day — seconds (0–59)</summary>
        public int time_of_day_seconds;
        /// <summary>Delta vs. reference lap in milliseconds (signed)</summary>
        public int delta_time_ms;
        /// <summary>Current lap time in milliseconds</summary>
        public int current_lap_time_ms;
        /// <summary>Predicted final lap time in milliseconds</summary>
        public int predicted_lap_time_ms;
        /// <summary>Fuel remaining in the tank in litres</summary>
        public float fuel_liter_current_quantity;
        /// <summary>Fuel remaining as a fraction of tank capacity</summary>
        public float fuel_liter_current_quantity_percent;
        /// <summary>Average fuel consumption rate in litres per km</summary>
        public float fuel_liter_per_km;
        /// <summary>Average fuel economy in km per litre</summary>
        public float km_per_fuel_liter;
        /// <summary>Engine output torque in Nm</summary>
        public float current_torque;
        /// <summary>Engine output power in brake horsepower</summary>
        public int current_bhp;
        /// <summary>Full tyre state for the front-left corner</summary>
        public SMEvoTyreState tyre_lf;
        /// <summary>Full tyre state for the front-right corner</summary>
        public SMEvoTyreState tyre_rf;
        /// <summary>Full tyre state for the rear-left corner</summary>
        public SMEvoTyreState tyre_lr;
        /// <summary>Full tyre state for the rear-right corner</summary>
        public SMEvoTyreState tyre_rr;
        /// <summary>Normalised track position (0.0 = start/finish line, 1.0 = one full lap)</summary>
        public float npos;
        /// <summary>KERS/ERS charge level as a fraction (0.0–1.0)</summary>
        public float kers_charge_perc;
        /// <summary>KERS/ERS power currently being deployed as a fraction</summary>
        public float kers_current_perc;
        /// <summary>Seconds driver input remains locked (e.g. after collision penalty)</summary>
        public float control_lock_time;
        /// <summary>Damage levels for each body zone of the car</summary>
        public SMEvoDamageState car_damage;
        /// <summary>Current track zone the car occupies (see ACEVO_CAR_LOCATION)</summary>
        public ACEVO_CAR_LOCATION car_location;
        /// <summary>Status of each pit-stop service item</summary>
        public SMEvoPitInfo pit_info;
        /// <summary>Fuel consumed since session start in litres</summary>
        public float fuel_liter_used;
        /// <summary>Average fuel consumed per lap in litres</summary>
        public float fuel_liter_per_lap;
        /// <summary>Estimated number of laps achievable with remaining fuel</summary>
        public float laps_possible_with_fuel;
        /// <summary>High-voltage battery temperature in °C</summary>
        public float battery_temperature;
        /// <summary>High-voltage battery pack voltage in V</summary>
        public float battery_voltage;
        /// <summary>Instantaneous fuel consumption in litres per km</summary>
        public float instantaneous_fuel_liter_per_km;
        /// <summary>Instantaneous fuel economy in km per litre</summary>
        public float instantaneous_km_per_fuel_liter;
        /// <summary>How well current RPM suits the engaged gear (1.0 = ideal window)</summary>
        public float gear_rpm_window;
        /// <summary>Current state of all cockpit lights and displays</summary>
        public SMEvoInstrumentation instrumentation;
        /// <summary>Minimum allowed setting for each instrumentation item</summary>
        public SMEvoInstrumentation instrumentation_min_limit;
        /// <summary>Maximum allowed setting for each instrumentation item</summary>
        public SMEvoInstrumentation instrumentation_max_limit;
        /// <summary>Current electronic aid and setup values</summary>
        public SMEvoElectronics electronics;
        /// <summary>Minimum allowed value for each electronics setting</summary>
        public SMEvoElectronics electronics_min_limit;
        /// <summary>Maximum allowed value for each electronics setting</summary>
        public SMEvoElectronics electronics_max_limit;
        /// <summary>Flags which electronics fields the driver can adjust in-session</summary>
        public SMEvoElectronics electronics_is_modifiable;
        /// <summary>Total laps completed in the session</summary>
        public int total_lap_count;
        /// <summary>Current race position (1 = leader)</summary>
        public uint current_pos;
        /// <summary>Total number of cars in the session</summary>
        public uint total_drivers;
        /// <summary>Last completed lap time in milliseconds</summary>
        public int last_laptime_ms;
        /// <summary>Personal best lap time in milliseconds</summary>
        public int best_laptime_ms;
        /// <summary>Flag shown specifically to this driver</summary>
        public ACEVO_FLAG_TYPE flag;
        /// <summary>Flag shown to all drivers on track</summary>
        public ACEVO_FLAG_TYPE global_flag;
        /// <summary>Number of forward gears the car has</summary>
        public uint max_gears;
        /// <summary>Powertrain type of the car (see ACEVO_ENGINE_TYPE)</summary>
        public ACEVO_ENGINE_TYPE engine_type;
        /// <summary>Car is equipped with a KERS/ERS system</summary>
        [MarshalAs(UnmanagedType.I1)] public bool has_kers;
        /// <summary>This is the final scheduled lap of the race</summary>
        [MarshalAs(UnmanagedType.I1)] public bool is_last_lap;
        /// <summary>Display name of the active vehicle performance/power mode</summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 33)] public string performance_mode_name;
        /// <summary>Raw differential coast-lock value from setup</summary>
        public float diff_coast_raw_value;
        /// <summary>Raw differential power-lock value from setup</summary>
        public float diff_power_raw_value;
        /// <summary>Cumulative time penalty from track-limit cuts in ms</summary>
        public int race_cut_gained_time_ms;
        /// <summary>Distance to the penalty trigger in metres</summary>
        public int distance_to_deadline;
        /// <summary>Running delta time accrued from track-limit violations</summary>
        public float race_cut_current_delta;
        /// <summary>Session lifecycle and countdown information</summary>
        public SMEvoSessionState session_state;
        /// <summary>HUD lap times and delta display values</summary>
        public SMEvoTimingState timing_state;
        /// <summary>Network round-trip ping to the server in ms</summary>
        public int player_ping;
        /// <summary>Measured network latency in ms</summary>
        public int player_latency;
        /// <summary>Client CPU usage in percent</summary>
        public int player_cpu_usage;
        /// <summary>Average client CPU usage in percent</summary>
        public int player_cpu_usage_avg;
        /// <summary>Network Quality-of-Service score</summary>
        public int player_qos;
        /// <summary>Average QoS score over the session</summary>
        public int player_qos_avg;
        /// <summary>Current rendered frames per second</summary>
        public int player_fps;
        /// <summary>Average FPS over the session</summary>
        public int player_fps_avg;
        /// <summary>Driver's first name</summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 33)] public string driver_name;
        /// <summary>Driver's surname</summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 33)] public string driver_surname;
        /// <summary>Identifier or display name of the car model</summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 33)] public string car_model;
        /// <summary>Car is stationary inside its assigned pit box</summary>
        [MarshalAs(UnmanagedType.I1)] public bool is_in_pit_box;
        /// <summary>Car is anywhere within the pit lane</summary>
        [MarshalAs(UnmanagedType.I1)] public bool is_in_pit_lane;
        /// <summary>Current lap is valid and counts for timing</summary>
        [MarshalAs(UnmanagedType.I1)] public bool is_valid_lap;
        /// <summary>World-space position of up to 60 cars [car_index][X, Y, Z]</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 180)] public float[] car_coordinates;
        /// <summary>Time gap to the car immediately ahead in seconds</summary>
        public float gap_ahead;
        /// <summary>Time gap to the car immediately behind in seconds</summary>
        public float gap_behind;
        /// <summary>Number of cars actively participating in the session</summary>
        public byte active_cars;
        /// <summary>Target fuel consumption per lap in litres</summary>
        public float fuel_per_lap;
        /// <summary>Estimated laps remaining with current fuel</summary>
        public float fuel_estimated_laps;
        /// <summary>All driver-assist levels currently active</summary>
        public SMEvoAssistsState assists_state;
        /// <summary>Maximum fuel tank capacity of the car in litres</summary>
        public float max_fuel;
        /// <summary>Maximum turbo boost pressure in bar</summary>
        public float max_turbo_boost;
        /// <summary>Car is restricted to a single tyre compound for both axles</summary>
        [MarshalAs(UnmanagedType.I1)] public bool use_single_compound;

        /// <summary>Deserializes the structure from a raw byte array.</summary>
        public static SPageFileGraphicEvo FromBytes(byte[] bytes)
        {
            var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            try { return (SPageFileGraphicEvo)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(SPageFileGraphicEvo)); }
            catch { return default; }
            finally { handle.Free(); }
        }
    }

    /// <summary>
    /// Static session metadata. Written once when a session loads and does not change while driving.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public struct SPageFileStaticEvo
    {
        /// <summary>Shared-memory interface version string</summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 15)] public string sm_version;
        /// <summary>AC Evo game build version string</summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 15)] public string ac_evo_version;
        /// <summary>Type of the current session (see ACEVO_SESSION_TYPE)</summary>
        public ACEVO_SESSION_TYPE session;
        /// <summary>Human-readable session name (e.g. 'Race 1')</summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 33)] public string session_name;
        /// <summary>Unique identifier of the event within the championship</summary>
        public byte event_id;
        /// <summary>Unique identifier of this session within the event</summary>
        public byte session_id;
        /// <summary>Tyre grip condition at session start (see ACEVO_STARTING_GRIP)</summary>
        public ACEVO_STARTING_GRIP starting_grip;
        /// <summary>Ambient air temperature at session start in °C</summary>
        public float starting_ambient_temperature_c;
        /// <summary>Road surface temperature at session start in °C</summary>
        public float starting_ground_temperature_c;
        /// <summary>Weather is fixed and will not change during the session</summary>
        [MarshalAs(UnmanagedType.I1)] public bool is_static_weather;
        /// <summary>Session ends by elapsed time rather than lap count</summary>
        [MarshalAs(UnmanagedType.I1)] public bool is_timed_race;
        /// <summary>Session is an online multiplayer event</summary>
        [MarshalAs(UnmanagedType.I1)] public bool is_online;
        /// <summary>Total sessions in this event (e.g. 3 = practice + qualify + race)</summary>
        public int number_of_sessions;
        /// <summary>Country / nation name associated with the event or track</summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 33)] public string nation;
        /// <summary>Geographic longitude of the track location in decimal degrees</summary>
        public float longitude;
        /// <summary>Geographic latitude of the track location in decimal degrees</summary>
        public float latitude;
        /// <summary>Track identifier or name</summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 33)] public string track;
        /// <summary>Track layout variant or configuration name</summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 33)] public string track_configuration;
        /// <summary>Total lap length of the track in metres</summary>
        public float track_length_m;

        /// <summary>Deserializes the structure from a raw byte array.</summary>
        public static SPageFileStaticEvo FromBytes(byte[] bytes)
        {
            var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            try { return (SPageFileStaticEvo)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(SPageFileStaticEvo)); }
            catch { return default; }
            finally { handle.Free(); }
        }
    }
    #endregion
}