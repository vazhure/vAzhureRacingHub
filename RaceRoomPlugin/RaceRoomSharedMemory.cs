/// <summary>
/// RaceRoom Shared Memory API definitions.
/// Source: https://github.com/sector3studios/r3e-api
/// </summary>
using System;
using System.Runtime.InteropServices;

namespace RaceRoom.Structs
{
    /// <summary>
    /// Constant values and enumerations for RaceRoom shared memory API.
    /// </summary>
    public static class Constant
    {
        /// <summary>
        /// Name of the shared memory segment.
        /// </summary>
        public const string SharedMemoryName = "$R3E";

        /// <summary>
        /// Major version numbers for API compatibility checking.
        /// </summary>
        public enum VersionMajor
        {
            /// <summary>
            /// Current major version number to test against.
            /// </summary>
            R3E_VERSION_MAJOR = 3
        }

        /// <summary>
        /// Minor version numbers for API compatibility checking.
        /// </summary>
        public enum VersionMinor
        {
            /// <summary>
            /// Current minor version number to test against.
            /// </summary>
            R3E_VERSION_MINOR = 5
        }

        /// <summary>
        /// Available game modes in RaceRoom.
        /// </summary>
        public enum GameMode
        {
            /// <summary>Data not available.</summary>
            Unavailable = -1,
            /// <summary>Track test mode.</summary>
            TrackTest = 0,
            /// <summary>Leaderboard challenge mode.</summary>
            LeaderboardChallenge = 1,
            /// <summary>Competition mode.</summary>
            Competition = 2,
            /// <summary>Single race mode.</summary>
            SingleRace = 3,
            /// <summary>Championship mode.</summary>
            Championship = 4,
            /// <summary>Multiplayer mode.</summary>
            Multiplayer = 5,
            /// <summary>Ranked multiplayer mode.</summary>
            MultiplayerRanked = 6,
            /// <summary>Trial mode (Try Before You Buy).</summary>
            TryBeforeYouBuy = 7,
        }

        /// <summary>
        /// Session types available in the game.
        /// </summary>
        public enum Session
        {
            /// <summary>Data not available.</summary>
            Unavailable = -1,
            /// <summary>Practice session.</summary>
            Practice = 0,
            /// <summary>Qualifying session.</summary>
            Qualify = 1,
            /// <summary>Race session.</summary>
            Race = 2,
            /// <summary>Warmup session.</summary>
            Warmup = 3,
        }

        /// <summary>
        /// Phases within a session.
        /// </summary>
        public enum SessionPhase
        {
            /// <summary>Data not available.</summary>
            Unavailable = -1,
            /// <summary>Multiplayer race start countdown in garage.</summary>
            Garage = 1,
            /// <summary>Gridwalk or track walkthrough phase.</summary>
            Gridwalk = 2,
            /// <summary>Formation lap or rolling start preparation.</summary>
            Formation = 3,
            /// <summary>Countdown to race start is ongoing.</summary>
            Countdown = 4,
            /// <summary>Race is actively running (green flag).</summary>
            Green = 5,
            /// <summary>Session has ended (checkered flag).</summary>
            Checkered = 6,
        }

        /// <summary>
        /// Control source for a vehicle.
        /// </summary>
        public enum Control
        {
            /// <summary>Data not available.</summary>
            Unavailable = -1,
            /// <summary>Controlled by the actual player.</summary>
            Player = 0,
            /// <summary>Controlled by AI.</summary>
            AI = 1,
            /// <summary>Controlled by a network entity.</summary>
            Remote = 2,
            /// <summary>Controlled by a replay or ghost.</summary>
            Replay = 3,
        }

        /// <summary>
        /// Status of the pit window during a session.
        /// </summary>
        public enum PitWindow
        {
            /// <summary>Data not available.</summary>
            Unavailable = -1,
            /// <summary>Pit stops are not enabled for this session.</summary>
            Disabled = 0,
            /// <summary>Pit stops enabled but not currently allowed.</summary>
            Closed = 1,
            /// <summary>Pit stop is currently allowed.</summary>
            Open = 2,
            /// <summary>Currently performing pit stop actions.</summary>
            Stopped = 3,
            /// <summary>Mandatory pit stop has been completed.</summary>
            Completed = 4,
        }

        /// <summary>
        /// Status of mandatory pit stop requirements.
        /// </summary>
        public enum PitStopStatus
        {
            /// <summary>No mandatory pit stops.</summary>
            Unavailable = -1,
            /// <summary>Mandatory pit stop for two tyres not yet served.</summary>
            UnservedTwoTyres = 0,
            /// <summary>Mandatory pit stop for four tyres not yet served.</summary>
            UnservedFourTyres = 1,
            /// <summary>Mandatory pit stop has been served.</summary>
            Served = 2,
        }

        /// <summary>
        /// Finish status of a driver in a session.
        /// </summary>
        public enum FinishStatus
        {
            /// <summary>Data not available.</summary>
            Unavailable = -1,
            /// <summary>Still on track, session not finished.</summary>
            None = 0,
            /// <summary>Finished session normally.</summary>
            Finished = 1,
            /// <summary>Did not finish (DNF).</summary>
            DNF = 2,
            /// <summary>Did not qualify (DNQ).</summary>
            DNQ = 3,
            /// <summary>Did not start (DNS).</summary>
            DNS = 4,
            /// <summary>Disqualified (DQ).</summary>
            DQ = 5,
        }

        /// <summary>
        /// Format type for session length configuration.
        /// </summary>
        public enum SessionLengthFormat
        {
            /// <summary>Data not available.</summary>
            Unavailable = -1,
            /// <summary>Session length defined by time.</summary>
            TimeBased = 0,
            /// <summary>Session length defined by number of laps.</summary>
            LapBased = 1,
            /// <summary>Time-based session with an extra lap after time expires.</summary>
            TimeAndLapBased = 2
        }

        /// <summary>
        /// Selection options in the pit menu.
        /// </summary>
        public enum PitMenuSelection
        {
            /// <summary>Pit menu unavailable.</summary>
            Unavailable = -1,
            /// <summary>Pit menu preset selected.</summary>
            Preset = 0,
            /// <summary>Penalty action.</summary>
            Penalty = 1,
            /// <summary>Driver change action.</summary>
            Driverchange = 2,
            /// <summary>Fuel action.</summary>
            Fuel = 3,
            /// <summary>Front tires action.</summary>
            Fronttires = 4,
            /// <summary>Rear tires action.</summary>
            Reartires = 5,
            /// <summary>Body repair action.</summary>
            Body = 6,
            /// <summary>Front wing action.</summary>
            Frontwing = 7,
            /// <summary>Rear wing action.</summary>
            Rearwing = 8,
            /// <summary>Suspension action.</summary>
            Suspension = 9,
            /// <summary>Top navigation button.</summary>
            ButtonTop = 10,
            /// <summary>Bottom navigation button.</summary>
            ButtonBottom = 11,
            /// <summary>Maximum enum value (nothing selected).</summary>
            Max = 12
        }

        /// <summary>
        /// Tire compound types.
        /// </summary>
        public enum TireType
        {
            /// <summary>Data not available.</summary>
            Unavailable = -1,
            /// <summary>Option compound (softer).</summary>
            Option = 0,
            /// <summary>Prime compound (harder).</summary>
            Prime = 1,
        }

        /// <summary>
        /// Tire subtype classifications.
        /// </summary>
        public enum TireSubtype
        {
            /// <summary>Data not available.</summary>
            Unavailable = -1,
            /// <summary>Primary compound.</summary>
            Primary = 0,
            /// <summary>Alternate compound.</summary>
            Alternate = 1,
            /// <summary>Soft compound.</summary>
            Soft = 2,
            /// <summary>Medium compound.</summary>
            Medium = 3,
            /// <summary>Hard compound.</summary>
            Hard = 4
        }

        /// <summary>
        /// Track surface material types.
        /// </summary>
        public enum MtrlType
        {
            /// <summary>Data not available.</summary>
            Unavailable = -1,
            /// <summary>No material / unknown.</summary>
            None = 0,
            /// <summary>Tarmac / asphalt surface.</summary>
            Tarmac = 1,
            /// <summary>Grass surface.</summary>
            Grass = 2,
            /// <summary>Dirt surface.</summary>
            Dirt = 3,
            /// <summary>Gravel surface.</summary>
            Gravel = 4,
            /// <summary>Rumble strip / kerb.</summary>
            Rumble = 5,
            /// <summary>Concrete surface.</summary>
            Concrete = 6
        }

        /// <summary>
        /// Powertrain types for vehicles.
        /// </summary>
        public enum EngineType
        {
            /// <summary>Traditional internal combustion engine.</summary>
            COMBUSTION = 0,
            /// <summary>Fully electric powertrain.</summary>
            ELECTRIC = 1,
            /// <summary>Hybrid powertrain (combustion + electric).</summary>
            HYBRID = 2,
        }
    }

    /// <summary>
    /// Generic structure for race duration values across multiple races.
    /// </summary>
    /// <typeparam name="T">Numeric type for duration values.</typeparam>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct RaceDuration<T>
    {
        /// <summary>Duration for Race 1.</summary>
        public T Race1;
        /// <summary>Duration for Race 2.</summary>
        public T Race2;
        /// <summary>Duration for Race 3.</summary>
        public T Race3;
    }

    /// <summary>
    /// Generic 3D vector structure.
    /// </summary>
    /// <typeparam name="T">Numeric type for vector components.</typeparam>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct Vector3<T>
    {
        /// <summary>X component.</summary>
        public T X;
        /// <summary>Y component (typically up).</summary>
        public T Y;
        /// <summary>Z component.</summary>
        public T Z;
    }

    /// <summary>
    /// Generic orientation structure using Euler angles.
    /// </summary>
    /// <typeparam name="T">Numeric type for angle values (radians).</typeparam>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct Orientation<T>
    {
        /// <summary>Pitch angle (rotation around X axis).</summary>
        public T Pitch;
        /// <summary>Yaw angle (rotation around Y axis).</summary>
        public T Yaw;
        /// <summary>Roll angle (rotation around Z axis).</summary>
        public T Roll;
    }

    /// <summary>
    /// Generic structure for sector start positions on a track.
    /// </summary>
    /// <typeparam name="T">Numeric type for sector values.</typeparam>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct SectorStarts<T>
    {
        /// <summary>Start position of Sector 1.</summary>
        public T Sector1;
        /// <summary>Start position of Sector 2.</summary>
        public T Sector2;
        /// <summary>Start position of Sector 3.</summary>
        public T Sector3;
    }

    /// <summary>
    /// High-precision physics data for the player's vehicle only.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct PlayerData
    {
        /// <summary>Unique identifier for the player.</summary>
        public Int32 UserId;

        /// <summary>
        /// Virtual physics simulation time in ticks.
        /// </summary>
        /// <remarks>1 tick = 1/400th of a second</remarks>
        public Int32 GameSimulationTicks;

        /// <summary>
        /// Virtual physics simulation time in seconds.
        /// </summary>
        public Double GameSimulationTime;

        /// <summary>Car position in world space coordinates.</summary>
        public Vector3<Double> Position;

        /// <summary>
        /// Car velocity in world space.
        /// </summary>
        /// <remarks>Unit: Meters per second (m/s)</remarks>
        public Vector3<Double> Velocity;

        /// <summary>
        /// Car velocity in local (vehicle) space.
        /// </summary>
        /// <remarks>Unit: Meters per second (m/s)</remarks>
        public Vector3<Double> LocalVelocity;

        /// <summary>
        /// Car acceleration in world space.
        /// </summary>
        /// <remarks>Unit: Meters per second squared (m/s²)</remarks>
        public Vector3<Double> Acceleration;

        /// <summary>
        /// Car acceleration in local (vehicle) space.
        /// </summary>
        /// <remarks>Unit: Meters per second squared (m/s²)</remarks>
        public Vector3<Double> LocalAcceleration;

        /// <summary>
        /// Car body orientation as Euler angles.
        /// </summary>
        /// <remarks>Unit: Radians</remarks>
        public Vector3<Double> Orientation;

        /// <summary>Car body rotation rates.</summary>
        public Vector3<Double> Rotation;

        /// <summary>
        /// Angular acceleration (torque divided by inertia).
        /// </summary>
        public Vector3<Double> AngularAcceleration;

        /// <summary>
        /// Angular velocity in world space.
        /// </summary>
        /// <remarks>Unit: Radians per second</remarks>
        public Vector3<Double> AngularVelocity;

        /// <summary>
        /// Angular velocity in local (vehicle) space.
        /// </summary>
        /// <remarks>Unit: Radians per second</remarks>
        public Vector3<Double> LocalAngularVelocity;

        /// <summary>G-forces experienced by driver in local car space.</summary>
        public Vector3<Double> LocalGforce;

        /// <summary>Total steering force transmitted through steering column.</summary>
        public Double SteeringForce;
        /// <summary>Steering force as percentage of maximum.</summary>
        public Double SteeringForcePercentage;

        /// <summary>Current engine torque output.</summary>
        public Double EngineTorque;

        /// <summary>
        /// Current aerodynamic downforce.
        /// </summary>
        /// <remarks>Unit: Newtons (N)</remarks>
        public Double CurrentDownforce;

        /// <summary>Reserved fields for future ERS/hybrid system data.</summary>
        public Double Voltage;
        public Double ErsLevel;
        public Double PowerMguH;
        public Double PowerMguK;
        public Double TorqueMguK;

        /// <summary>
        /// Suspension deflection values for each tire.
        /// </summary>
        /// <remarks>Unit: Meters</remarks>
        public TireData<Double> SuspensionDeflection;
        /// <summary>
        /// Suspension velocity values for each tire.
        /// </summary>
        /// <remarks>Unit: Meters per second</remarks>
        public TireData<Double> SuspensionVelocity;
        /// <summary>
        /// Camber angles for each tire.
        /// </summary>
        /// <remarks>Unit: Radians</remarks>
        public TireData<Double> Camber;
        /// <summary>
        /// Ride height values for each tire.
        /// </summary>
        /// <remarks>Unit: Meters</remarks>
        public TireData<Double> RideHeight;
        /// <summary>Front wing height setting.</summary>
        public Double FrontWingHeight;
        /// <summary>Front anti-roll bar stiffness.</summary>
        public Double FrontRollAngle;
        /// <summary>Rear anti-roll bar stiffness.</summary>
        public Double RearRollAngle;
        /// <summary>Third spring suspension deflection (front).</summary>
        public Double ThirdSpringSuspensionDeflectionFront;
        /// <summary>Third spring suspension velocity (front).</summary>
        public Double ThirdSpringSuspensionVelocityFront;
        /// <summary>Third spring suspension deflection (rear).</summary>
        public Double ThirdSpringSuspensionDeflectionRear;
        /// <summary>Third spring suspension velocity (rear).</summary>
        public Double ThirdSpringSuspensionVelocityRear;

        /// <summary>Reserved fields for future expansion.</summary>
        public Double Unused1;
        public Double Unused2;
        public Double Unused3;
    }

    /// <summary>
    /// Current status of various racing flags.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct Flags
    {
        /// <summary>
        /// Yellow flag status.
        /// </summary>
        /// <remarks>-1 = no data, 0 = not active, 1 = active</remarks>
        public Int32 Yellow;
        /// <summary>
        /// Whether current slot caused the yellow flag.
        /// </summary>
        /// <remarks>-1 = no data, 0 = didn't cause, 1 = caused</remarks>
        public Int32 YellowCausedIt;
        /// <summary>
        /// Whether overtaking is allowed under yellow flag.
        /// </summary>
        /// <remarks>-1 = no data, 0 = not allowed, 1 = allowed</remarks>
        public Int32 YellowOvertake;
        /// <summary>
        /// Number of positions gained illegally under yellow flag.
        /// </summary>
        /// <remarks>-1 = no data, 0 = none gained, n = positions to return</remarks>
        public Int32 YellowPositionsGained;
        /// <summary>Yellow flag status per sector.</summary>
        public Sectors<Int32> SectorYellow;
        /// <summary>
        /// Distance to nearest yellow flag incident.
        /// </summary>
        /// <remarks>Unit: Meters; -1.0 if no yellow flag exists</remarks>
        public Single ClosestYellowDistanceIntoTrack;
        /// <summary>
        /// Blue flag status.
        /// </summary>
        /// <remarks>-1 = no data, 0 = not active, 1 = active</remarks>
        public Int32 Blue;
        /// <summary>
        /// Black flag status.
        /// </summary>
        /// <remarks>-1 = no data, 0 = not active, 1 = active</remarks>
        public Int32 Black;
        /// <summary>
        /// Green flag status.
        /// </summary>
        /// <remarks>-1 = no data, 0 = not active, 1 = active</remarks>
        public Int32 Green;
        /// <summary>
        /// Checkered flag status.
        /// </summary>
        /// <remarks>-1 = no data, 0 = not active, 1 = active</remarks>
        public Int32 Checkered;
        /// <summary>
        /// White flag status.
        /// </summary>
        /// <remarks>-1 = no data, 0 = not active, 1 = active</remarks>
        public Int32 White;
        /// <summary>
        /// Black and white flag status with reason code.
        /// </summary>
        /// <remarks>
        /// -1 = no data, 0 = not active,
        /// 1 = blue flag 1st warning, 2 = blue flag 2nd warning,
        /// 3 = wrong way, 4 = cutting track
        /// </remarks>
        public Int32 BlackAndWhite;
    }

    /// <summary>
    /// Damage state for various vehicle components.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct CarDamage
    {
        /// <summary>
        /// Engine damage level.
        /// </summary>
        /// <remarks>Range: 0.0 (undamaged) - 1.0 (destroyed); -1.0 = N/A</remarks>
        public Single Engine;
        /// <summary>
        /// Transmission damage level.
        /// </summary>
        /// <remarks>Range: 0.0 - 1.0; -1.0 = N/A</remarks>
        public Single Transmission;
        /// <summary>
        /// Aerodynamics damage level.
        /// </summary>
        /// <remarks>
        /// Range: 0.0 - 1.0 (approximate); 0.0 doesn't necessarily mean completely destroyed.
        /// -1.0 = N/A
        /// </remarks>
        public Single Aerodynamics;
        /// <summary>
        /// Suspension damage level.
        /// </summary>
        /// <remarks>Range: 0.0 - 1.0; -1.0 = N/A</remarks>
        public Single Suspension;
        /// <summary>Reserved fields for future expansion.</summary>
        public Single Unused1;
        public Single Unused2;
    }

    /// <summary>
    /// Generic structure for tire-specific data (FL, FR, RL, RR).
    /// </summary>
    /// <typeparam name="T">Numeric type for tire values.</typeparam>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct TireData<T>
    {
        /// <summary>Front-left tire value.</summary>
        public T FrontLeft;
        /// <summary>Front-right tire value.</summary>
        public T FrontRight;
        /// <summary>Rear-left tire value.</summary>
        public T RearLeft;
        /// <summary>Rear-right tire value.</summary>
        public T RearRight;
    }

    /// <summary>
    /// State of pit menu selections and actions.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct PitMenuState
    {
        /// <summary>Pit menu preset selection.</summary>
        public Int32 Preset;
        /// <summary>Penalty action state.</summary>
        public Int32 Penalty;
        /// <summary>Driver change action state.</summary>
        public Int32 Driverchange;
        /// <summary>Fuel action state.</summary>
        public Int32 Fuel;
        /// <summary>Front tires action state.</summary>
        public Int32 FrontTires;
        /// <summary>Rear tires action state.</summary>
        public Int32 RearTires;
        /// <summary>Body repair action state.</summary>
        public Int32 Body;
        /// <summary>Front wing action state.</summary>
        public Int32 FrontWing;
        /// <summary>Rear wing action state.</summary>
        public Int32 RearWing;
        /// <summary>Suspension action state.</summary>
        public Int32 Suspension;
        /// <summary>Top navigation button state.</summary>
        public Int32 ButtonTop;
        /// <summary>Bottom navigation button state.</summary>
        public Int32 ButtonBottom;
    }

    /// <summary>
    /// Pending cut-track penalties for a vehicle.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct CutTrackPenalties
    {
        /// <summary>
        /// Drive-through penalty time remaining.
        /// </summary>
        /// <remarks>
        /// -1.0 = none pending; otherwise penalty time depends on type:
        /// drive-through active = 0.0, stop-and-go = time to stay,
        /// slow-down = time left to give back, etc.
        /// </remarks>
        public Single DriveThrough;
        /// <summary>Stop-and-go penalty time remaining.</summary>
        public Single StopAndGo;
        /// <summary>Pit stop penalty time remaining.</summary>
        public Single PitStop;
        /// <summary>Time deduction penalty value.</summary>
        public Single TimeDeduction;
        /// <summary>Slow-down penalty time remaining.</summary>
        public Single SlowDown;
    }

    /// <summary>
    /// Drag Reduction System (DRS) status and configuration.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct DRS
    {
        /// <summary>
        /// Whether DRS is equipped and allowed.
        /// </summary>
        /// <remarks>0 = No, 1 = Yes, -1 = N/A</remarks>
        public Int32 Equipped;
        /// <summary>
        /// Whether DRS activation is currently available.
        /// </summary>
        /// <remarks>0 = No, 1 = Yes, -1 = N/A</remarks>
        public Int32 Available;
        /// <summary>
        /// Number of DRS activations remaining this lap.
        /// </summary>
        /// <remarks>
        /// In sessions with unlimited activations, this starts at Int32.MaxValue.
        /// -1 = N/A
        /// </remarks>
        public Int32 NumActivationsLeft;
        /// <summary>
        /// Whether DRS is currently engaged.
        /// </summary>
        /// <remarks>0 = No, 1 = Yes, -1 = N/A</remarks>
        public Int32 Engaged;
    }

    /// <summary>
    /// Push-to-Pass system status and configuration.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct PushToPass
    {
        /// <summary>
        /// PTP system availability status.
        /// </summary>
        /// <remarks>
        /// 1 = system exists, -1 = N/A.
        /// For applicable systems: 2 = charging, 3 = charged
        /// </remarks>
        public Int32 Available;
        /// <summary>Whether PTP is currently engaged.</summary>
        public Int32 Engaged;
        /// <summary>Amount of PTP boosts remaining.</summary>
        public Int32 AmountLeft;
        /// <summary>
        /// Time remaining for current PTP engagement.
        /// </summary>
        /// <remarks>Unit: Seconds</remarks>
        public Single EngagedTimeLeft;
        /// <summary>
        /// Time remaining until next PTP activation is available.
        /// </summary>
        /// <remarks>Unit: Seconds</remarks>
        public Single WaitTimeLeft;
    }

    /// <summary>
    /// Temperature information for a single tire.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct TireTempInformation
    {
        /// <summary>Current temperature readings across tire tread.</summary>
        public TireTemperature<Single> CurrentTemp;
        /// <summary>
        /// Optimal operating temperature.
        /// </summary>
        /// <remarks>Unit: Celsius</remarks>
        public Single OptimalTemp;
        /// <summary>
        /// Cold temperature threshold.
        /// </summary>
        /// <remarks>Unit: Celsius</remarks>
        public Single ColdTemp;
        /// <summary>
        /// Hot temperature threshold.
        /// </summary>
        /// <remarks>Unit: Celsius</remarks>
        public Single HotTemp;
    }

    /// <summary>
    /// Temperature information for a single brake.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct BrakeTemp
    {
        /// <summary>Current brake temperature.</summary>
        public Single CurrentTemp;
        /// <summary>
        /// Optimal operating temperature.
        /// </summary>
        /// <remarks>Unit: Celsius</remarks>
        public Single OptimalTemp;
        /// <summary>
        /// Cold temperature threshold.
        /// </summary>
        /// <remarks>Unit: Celsius</remarks>
        public Single ColdTemp;
        /// <summary>
        /// Hot temperature threshold.
        /// </summary>
        /// <remarks>Unit: Celsius</remarks>
        public Single HotTemp;
    }

    /// <summary>
    /// Temperature readings across three points of a tire tread.
    /// </summary>
    /// <typeparam name="T">Numeric type for temperature values.</typeparam>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct TireTemperature<T>
    {
        /// <summary>Temperature at inner (left) section of tire.</summary>
        public T Left;
        /// <summary>Temperature at center section of tire.</summary>
        public T Center;
        /// <summary>Temperature at outer (right) section of tire.</summary>
        public T Right;
    }

    /// <summary>
    /// Driver assistance system settings and status.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct AidSettings
    {
        /// <summary>
        /// ABS system status.
        /// </summary>
        /// <remarks>-1 = N/A, 0 = off, 1 = on, 5 = currently active</remarks>
        public Int32 Abs;
        /// <summary>
        /// Traction Control system status.
        /// </summary>
        /// <remarks>-1 = N/A, 0 = off, 1 = on, 5 = currently active</remarks>
        public Int32 Tc;
        /// <summary>
        /// ESP (Electronic Stability Program) status.
        /// </summary>
        /// <remarks>-1 = N/A, 0 = off, 1 = on low, 2 = on medium, 3 = on high, 5 = currently active</remarks>
        public Int32 Esp;
        /// <summary>
        /// Countersteer assist status.
        /// </summary>
        /// <remarks>-1 = N/A, 0 = off, 1 = on, 5 = currently active</remarks>
        public Int32 Countersteer;
        /// <summary>
        /// Cornering assist status.
        /// </summary>
        /// <remarks>-1 = N/A, 0 = off, 1 = on, 5 = currently active</remarks>
        public Int32 Cornering;
    }

    /// <summary>
    /// Generic structure for sector-specific values.
    /// </summary>
    /// <typeparam name="T">Numeric type for sector values.</typeparam>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct Sectors<T>
    {
        /// <summary>Value for Sector 1.</summary>
        public T Sector1;
        /// <summary>Value for Sector 2.</summary>
        public T Sector2;
        /// <summary>Value for Sector 3.</summary>
        public T Sector3;
    }

    /// <summary>
    /// Basic identification and configuration data for a driver/vehicle.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct DriverInfo
    {
        /// <summary>
        /// Driver name (UTF-8 encoded).
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public byte[] Name;
        /// <summary>Car number displayed on vehicle.</summary>
        public Int32 CarNumber;
        /// <summary>Vehicle class identifier.</summary>
        public Int32 ClassId;
        /// <summary>Vehicle model identifier.</summary>
        public Int32 ModelId;
        /// <summary>Team identifier.</summary>
        public Int32 TeamId;
        /// <summary>Livery/paint scheme identifier.</summary>
        public Int32 LiveryId;
        /// <summary>Manufacturer identifier.</summary>
        public Int32 ManufacturerId;
        /// <summary>Unique user identifier.</summary>
        public Int32 UserId;
        /// <summary>Starting grid slot identifier.</summary>
        public Int32 SlotId;
        /// <summary>Performance index for class balancing.</summary>
        public Int32 ClassPerformanceIndex;
        /// <summary>
        /// Powertrain type.
        /// </summary>
        /// <remarks>See Constant.EngineType enum</remarks>
        public Int32 EngineType;
        /// <summary>
        /// Vehicle width.
        /// </summary>
        /// <remarks>Unit: Meters</remarks>
        public Single CarWidth;
        /// <summary>
        /// Vehicle length.
        /// </summary>
        /// <remarks>Unit: Meters</remarks>
        public Single CarLength;
        /// <summary>Driver skill/pace rating.</summary>
        public Single Rating;
        /// <summary>Driver sportsmanship reputation score.</summary>
        public Single Reputation;
        /// <summary>Reserved fields for future expansion.</summary>
        public Single Unused1;
        public Single Unused2;
    }

    /// <summary>
    /// Real-time scoring and positioning data for a driver.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct DriverData
    {
        /// <summary>Basic driver and vehicle identification.</summary>
        public DriverInfo DriverInfo;
        /// <summary>
        /// Session finish status.
        /// </summary>
        /// <remarks>See Constant.FinishStatus enum</remarks>
        public Int32 FinishStatus;
        /// <summary>
        /// Overall race position.
        /// </summary>
        /// <remarks>1 = first place</remarks>
        public Int32 Place;
        /// <summary>Position within driver's performance class.</summary>
        public Int32 PlaceClass;
        /// <summary>
        /// Distance traveled around current lap.
        /// </summary>
        /// <remarks>Unit: Meters</remarks>
        public Single LapDistance;
        /// <summary>
        /// Fraction of current lap completed.
        /// </summary>
        /// <remarks>Range: 0.0 - 1.0; -1.0 = N/A</remarks>
        public Single LapDistanceFraction;
        /// <summary>Vehicle position in world space.</summary>
        public Vector3<Single> Position;
        /// <summary>Current track sector number (1, 2, or 3).</summary>
        public Int32 TrackSector;
        /// <summary>Number of completed laps.</summary>
        public Int32 CompletedLaps;
        /// <summary>Whether current lap time is valid (no track limits violations).</summary>
        public Int32 CurrentLapValid;
        /// <summary>
        /// Current lap time.
        /// </summary>
        /// <remarks>Unit: Seconds; -1.0 = N/A</remarks>
        public Single LapTimeCurrentSelf;
        /// <summary>Sector times for current lap.</summary>
        public Sectors<Single> SectorTimeCurrentSelf;
        /// <summary>Sector times for previous lap.</summary>
        public Sectors<Single> SectorTimePreviousSelf;
        /// <summary>Personal best sector times.</summary>
        public Sectors<Single> SectorTimeBestSelf;
        /// <summary>
        /// Time gap to car ahead.
        /// </summary>
        /// <remarks>Unit: Seconds; -1.0 = N/A</remarks>
        public Single TimeDeltaFront;
        /// <summary>
        /// Time gap to car behind.
        /// </summary>
        /// <remarks>Unit: Seconds; -1.0 = N/A</remarks>
        public Single TimeDeltaBehind;
        /// <summary>
        /// Mandatory pit stop requirement status.
        /// </summary>
        /// <remarks>See Constant.PitStopStatus enum</remarks>
        public Int32 PitStopStatus;
        /// <summary>Whether vehicle is currently in pit lane.</summary>
        public Int32 InPitlane;
        /// <summary>Total number of pit stops performed.</summary>
        public Int32 NumPitstops;
        /// <summary>Active penalty information.</summary>
        public CutTrackPenalties Penalties;
        /// <summary>
        /// Current vehicle speed.
        /// </summary>
        /// <remarks>Unit: Meters per second</remarks>
        public Single CarSpeed;
        /// <summary>
        /// Front tire compound type.
        /// </summary>
        /// <remarks>See Constant.TireType enum</remarks>
        public Int32 TireTypeFront;
        /// <summary>
        /// Rear tire compound type.
        /// </summary>
        /// <remarks>See Constant.TireType enum</remarks>
        public Int32 TireTypeRear;
        /// <summary>
        /// Front tire subtype.
        /// </summary>
        /// <remarks>See Constant.TireSubtype enum</remarks>
        public Int32 TireSubtypeFront;
        /// <summary>
        /// Rear tire subtype.
        /// </summary>
        /// <remarks>See Constant.TireSubtype enum</remarks>
        public Int32 TireSubtypeRear;
        /// <summary>Base penalty weight applied to vehicle.</summary>
        public Single BasePenaltyWeight;
        /// <summary>Additional penalty weight from driver aids usage.</summary>
        public Single AidPenaltyWeight;
        /// <summary>
        /// DRS engagement status.
        /// </summary>
        /// <remarks>-1 = unavailable, 0 = not engaged, 1 = engaged</remarks>
        public Int32 DrsState;
        /// <summary>
        /// Push-to-Pass engagement status.
        /// </summary>
        /// <remarks>-1 = unavailable, 0 = not engaged, 1 = engaged</remarks>
        public Int32 PtpState;
        /// <summary>
        /// Virtual energy tank level (for hybrid/electric vehicles).
        /// </summary>
        /// <remarks>Range: 0.0 - 1.0; -1.0 = unavailable</remarks>
        public Single VirtualEnergy;
        /// <summary>
        /// Type of active penalty.
        /// </summary>
        /// <remarks>
        /// -1 = unavailable, 0 = DriveThrough, 1 = StopAndGo,
        /// 2 = Pitstop, 3 = Time, 4 = Slowdown, 5 = Disqualify
        /// </remarks>
        public Int32 PenaltyType;
        /// <summary>
        /// Reason code for the active penalty.
        /// </summary>
        /// <remarks>
        /// Values depend on PenaltyType. See source comments for complete mapping.
        /// </remarks>
        public Int32 PenaltyReason;
        /// <summary>
        /// Engine ignition/running state.
        /// </summary>
        /// <remarks>
        /// -1 = unavailable, 0 = ignition off, 1 = ignition on but not running,
        /// 2 = ignition on and starter running, 3 = ignition on and running
        /// </remarks>
        public Int32 EngineState;
        /// <summary>
        /// Vehicle body orientation.
        /// </summary>
        /// <remarks>Unit: Euler angles (radians)</remarks>
        public Vector3<Single> Orientation;
        /// <summary>Reserved fields for future expansion.</summary>
        public Single Unused1;
        public Single Unused2;
        public Single Unused3;
    }

    /// <summary>
    /// Main shared memory structure for RaceRoom telemetry data.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct RaceRoomSharedMemory
    {
        #region Version Information
        /// <summary>API major version number.</summary>
        public Int32 VersionMajor;
        /// <summary>API minor version number.</summary>
        public Int32 VersionMinor;
        /// <summary>Byte offset to NumCars variable in driver data array.</summary>
        public Int32 AllDriversOffset;
        /// <summary>Size of DriverData structure in bytes.</summary>
        public Int32 DriverDataSize;
        #endregion

        #region Game State
        /// <summary>
        /// Current game mode.
        /// </summary>
        /// <remarks>See Constant.GameMode enum</remarks>
        public Int32 GameMode;
        /// <summary>Whether game is currently paused.</summary>
        public Int32 GamePaused;
        /// <summary>Whether player is currently in game menus.</summary>
        public Int32 GameInMenus;
        /// <summary>Whether game is currently playing a replay.</summary>
        public Int32 GameInReplay;
        /// <summary>Whether game is running in VR mode.</summary>
        public Int32 GameUsingVr;
        /// <summary>Whether player's vehicle is currently in garage.</summary>
        public Int32 GamePlayerInGarage;
        #endregion

        #region High Detail Player Data
        /// <summary>
        /// High-precision physics data for player's vehicle only.
        /// </summary>
        public PlayerData Player;
        #endregion

        #region Event and Session Information
        /// <summary>
        /// Name of current track (UTF-8 encoded).
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public byte[] TrackName;
        /// <summary>
        /// Name of current track layout (UTF-8 encoded).
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public byte[] LayoutName;
        /// <summary>Unique track identifier.</summary>
        public Int32 TrackId;
        /// <summary>Unique layout identifier.</summary>
        public Int32 LayoutId;
        /// <summary>
        /// Total length of track layout.
        /// </summary>
        /// <remarks>Unit: Meters</remarks>
        public Single LayoutLength;
        /// <summary>Normalized start positions for each sector (0.0 - 1.0 of lap).</summary>
        public SectorStarts<Single> SectorStartFactors;
        /// <summary>Configured race durations by lap count.</summary>
        public RaceDuration<Int32> RaceSessionLaps;
        /// <summary>Configured race durations by time.</summary>
        public RaceDuration<Int32> RaceSessionMinutes;
        /// <summary>
        /// Index of current event in championship.
        /// </summary>
        /// <remarks>0-indexed; -1 = N/A</remarks>
        public Int32 EventIndex;
        /// <summary>
        /// Type of current session.
        /// </summary>
        /// <remarks>See Constant.Session enum</remarks>
        public Int32 SessionType;
        /// <summary>
        /// Iteration number of current session type.
        /// </summary>
        /// <remarks>1 = first, 2 = second, etc.; -1 = N/A</remarks>
        public Int32 SessionIteration;
        /// <summary>
        /// Session length format type.
        /// </summary>
        /// <remarks>See Constant.SessionLengthFormat enum</remarks>
        public Int32 SessionLengthFormat;
        /// <summary>
        /// Pit lane speed limit.
        /// </summary>
        /// <remarks>Unit: Meters per second</remarks>
        public Single SessionPitSpeedLimit;
        /// <summary>
        /// Current phase of the session.
        /// </summary>
        /// <remarks>See Constant.SessionPhase enum</remarks>
        public Int32 SessionPhase;
        /// <summary>
        /// Status of starting lights.
        /// </summary>
        /// <remarks>
        /// -1 = unavailable, 0 = off, 1-5 = red lights counting down, 6 = green light
        /// </remarks>
        public Int32 StartLights;
        /// <summary>
        /// Tire wear multiplier setting.
        /// </summary>
        /// <remarks>-1 = no data, 0 = disabled, 1-4 = wear multipliers</remarks>
        public Int32 TireWearActive;
        /// <summary>
        /// Fuel consumption multiplier setting.
        /// </summary>
        /// <remarks>-1 = no data, 0 = disabled, 1-4 = consumption multipliers</remarks>
        public Int32 FuelUseActive;
        /// <summary>
        /// Total number of laps in race session.
        /// </summary>
        /// <remarks>-1 if not in race mode</remarks>
        public Int32 NumberOfLaps;
        /// <summary>
        /// Total session duration.
        /// </summary>
        /// <remarks>Unit: Seconds; -1.0 = N/A (time-based sessions only)</remarks>
        public Single SessionTimeDuration;
        /// <summary>
        /// Remaining session time.
        /// </summary>
        /// <remarks>Unit: Seconds; -1.0 = N/A (time-based sessions only)</remarks>
        public Single SessionTimeRemaining;
        /// <summary>
        /// Server-configured maximum incident points before penalty.
        /// </summary>
        /// <remarks>-1 = N/A</remarks>
        public Int32 MaxIncidentPoints;
        /// <summary>Reserved fields for future expansion.</summary>
        public Single EventUnused1;
        public Single EventUnused2;
        #endregion

        #region Pit Stop Information
        /// <summary>
        /// Current pit window status.
        /// </summary>
        /// <remarks>See Constant.PitWindow enum</remarks>
        public Int32 PitWindowStatus;
        /// <summary>
        /// Start of mandatory pit window.
        /// </summary>
        /// <remarks>Unit: Minutes (time-based) or Lap number; -1 = N/A</remarks>
        public Int32 PitWindowStart;
        /// <summary>
        /// End of mandatory pit window.
        /// </summary>
        /// <remarks>Unit: Minutes (time-based) or Lap number; -1 = N/A</remarks>
        public Int32 PitWindowEnd;
        /// <summary>
        /// Whether vehicle is in pit lane.
        /// </summary>
        /// <remarks>-1 = N/A</remarks>
        public Int32 InPitlane;
        /// <summary>Currently selected pit menu item.</summary>
        public Int32 PitMenuSelection;
        /// <summary>State of all pit menu options.</summary>
        public PitMenuState PitMenuState;
        /// <summary>
        /// Current pit stop state machine value.
        /// </summary>
        /// <remarks>
        /// -1 = N/A, 0 = None, 1 = Requested, 2 = Entering,
        /// 3 = Stopped, 4 = Exiting
        /// </remarks>
        public Int32 PitState;
        /// <summary>
        /// Total expected duration of pit stop.
        /// </summary>
        /// <remarks>Unit: Seconds</remarks>
        public Single PitTotalDuration;
        /// <summary>
        /// Elapsed time of current pit stop.
        /// </summary>
        /// <remarks>Unit: Seconds</remarks>
        public Single PitElapsedTime;
        /// <summary>
        /// Bitmask of active pit actions.
        /// </summary>
        /// <remarks>
        /// -1 = N/A, 0 = None, 1 = Preparing,
        /// combination of: 2=Penalty, 4=DriverChange, 8=Refuel,
        /// 16=FrontTires, 32=RearTires, 64=Body, 128=FrontWing,
        /// 256=RearWing, 512=Suspension
        /// </remarks>
        public Int32 PitAction;
        /// <summary>
        /// Number of pit stops already performed.
        /// </summary>
        /// <remarks>-1 = N/A</remarks>
        public Int32 NumPitstopsPerformed;
        /// <summary>
        /// Minimum required total pit stop duration.
        /// </summary>
        /// <remarks>Unit: Seconds</remarks>
        public Single PitMinDurationTotal;
        /// <summary>
        /// Minimum remaining pit stop duration.
        /// </summary>
        /// <remarks>Unit: Seconds</remarks>
        public Single PitMinDurationLeft;
        #endregion

        #region Scoring and Timing
        /// <summary>Current status of all flag types.</summary>
        public Flags Flags;
        /// <summary>
        /// Current overall race position.
        /// </summary>
        /// <remarks>1 = first place</remarks>
        public Int32 Position;
        /// <summary>Position within performance class.</summary>
        public Int32 PositionClass;
        /// <summary>
        /// Session finish status.
        /// </summary>
        /// <remarks>See Constant.FinishStatus enum</remarks>
        public Int32 FinishStatus;
        /// <summary>
        /// Total number of track limit violation warnings.
        /// </summary>
        /// <remarks>-1 = N/A</remarks>
        public Int32 CutTrackWarnings;
        /// <summary>Active penalty information.</summary>
        public CutTrackPenalties Penalties;
        /// <summary>Total count of pending penalties.</summary>
        public Int32 NumPenalties;
        /// <summary>
        /// Number of completed laps.
        /// </summary>
        /// <remarks>If value is 6, player is on 7th lap; -1 = N/A</remarks>
        public Int32 CompletedLaps;
        /// <summary>Whether current lap is valid for timing.</summary>
        public Int32 CurrentLapValid;
        /// <summary>Current track sector (1, 2, or 3).</summary>
        public Int32 TrackSector;
        /// <summary>
        /// Distance traveled on current lap.
        /// </summary>
        /// <remarks>Unit: Meters</remarks>
        public Single LapDistance;
        /// <summary>
        /// Fraction of current lap completed.
        /// </summary>
        /// <remarks>Range: 0.0 - 1.0; -1.0 = N/A</remarks>
        public Single LapDistanceFraction;
        /// <summary>
        /// Best lap time of session leader.
        /// </summary>
        /// <remarks>Unit: Seconds; -1.0 = N/A</remarks>
        public Single LapTimeBestLeader;
        /// <summary>
        /// Best lap time of class leader.
        /// </summary>
        /// <remarks>Unit: Seconds; -1.0 = N/A</remarks>
        public Single LapTimeBestLeaderClass;
        /// <summary>
        /// Sector times from fastest lap of session.
        /// </summary>
        /// <remarks>Unit: Seconds; -1.0 = N/A</remarks>
        public Sectors<Single> SectorTimesSessionBestLap;
        /// <summary>
        /// Personal best lap time.
        /// </summary>
        /// <remarks>Unit: Seconds; -1.0 = none</remarks>
        public Single LapTimeBestSelf;
        /// <summary>Personal best sector times.</summary>
        public Sectors<Single> SectorTimesBestSelf;
        /// <summary>
        /// Previous lap time.
        /// </summary>
        /// <remarks>Unit: Seconds; -1.0 = none</remarks>
        public Single LapTimePreviousSelf;
        /// <summary>Previous lap sector times.</summary>
        public Sectors<Single> SectorTimesPreviousSelf;
        /// <summary>
        /// Current lap time (in progress).
        /// </summary>
        /// <remarks>Unit: Seconds; -1.0 = none</remarks>
        public Single LapTimeCurrentSelf;
        /// <summary>Current lap sector times (in progress).</summary>
        public Sectors<Single> SectorTimesCurrentSelf;
        /// <summary>
        /// Time delta to session leader.
        /// </summary>
        /// <remarks>Unit: Seconds; -1.0 = N/A</remarks>
        public Single LapTimeDeltaLeader;
        /// <summary>
        /// Time delta to class leader.
        /// </summary>
        /// <remarks>Unit: Seconds; -1.0 = N/A</remarks>
        public Single LapTimeDeltaLeaderClass;
        /// <summary>
        /// Time gap to car ahead.
        /// </summary>
        /// <remarks>Unit: Seconds; -1.0 = N/A</remarks>
        public Single TimeDeltaFront;
        /// <summary>
        /// Time gap to car behind.
        /// </summary>
        /// <remarks>Unit: Seconds; -1.0 = N/A</remarks>
        public Single TimeDeltaBehind;
        /// <summary>
        /// Delta between current lap and personal best.
        /// </summary>
        /// <remarks>Unit: Seconds; -1000.0 = N/A</remarks>
        public Single TimeDeltaBestSelf;
        /// <summary>
        /// Best individual sector times (any lap).
        /// </summary>
        /// <remarks>Unit: Seconds; -1.0 = N/A</remarks>
        public Sectors<Single> BestIndividualSectorTimeSelf;
        /// <summary>Best sector times of session leader.</summary>
        public Sectors<Single> BestIndividualSectorTimeLeader;
        /// <summary>Best sector times of class leader.</summary>
        public Sectors<Single> BestIndividualSectorTimeLeaderClass;
        /// <summary>Current incident point total.</summary>
        public Int32 IncidentPoints;
        /// <summary>
        /// Lap validity state.
        /// </summary>
        /// <remarks>
        /// -1 = N/A, 0 = current and next lap valid,
        /// 1 = current lap invalid, 2 = current and next lap invalid
        /// </remarks>
        public Int32 LapValidState;
        /// <summary>
        /// Validity of previous lap.
        /// </summary>
        /// <remarks>-1 = N/A, 0 = invalid, 1 = valid</remarks>
        public Int32 PrevLapValid;
        /// <summary>
        /// Battery discharge rate (hybrid/electric vehicles).
        /// </summary>
        /// <remarks>Range: 0.0 - 1.0; -1.0 = N/A</remarks>
        public Single DischargeRate;
        /// <summary>
        /// Regenerative braking coefficient.
        /// </summary>
        /// <remarks>-1.0 = N/A</remarks>
        public Single BrakeRegen;
        /// <summary>Reserved field for future expansion.</summary>
        public Single Unused1;
        #endregion

        #region Vehicle Information
        /// <summary>Vehicle configuration and identification.</summary>
        public DriverInfo VehicleInfo;
        /// <summary>
        /// Player name (UTF-8 encoded).
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public byte[] PlayerName;
        #endregion

        #region Vehicle State
        /// <summary>
        /// Control source for player's vehicle.
        /// </summary>
        /// <remarks>See Constant.Control enum</remarks>
        public Int32 ControlType;
        /// <summary>
        /// Current vehicle speed.
        /// </summary>
        /// <remarks>Unit: Meters per second</remarks>
        public Single CarSpeed;
        /// <summary>
        /// Current engine speed.
        /// </summary>
        /// <remarks>Unit: Radians per second</remarks>
        public Single EngineRps;
        /// <summary>
        /// Maximum engine speed.
        /// </summary>
        /// <remarks>Unit: Radians per second</remarks>
        public Single MaxEngineRps;
        /// <summary>
        /// Recommended upshift RPM.
        /// </summary>
        /// <remarks>Unit: Radians per second</remarks>
        public Single UpshiftRps;
        /// <summary>
        /// Current gear selection.
        /// </summary>
        /// <remarks>-2 = N/A, -1 = reverse, 0 = neutral, 1+ = forward gears</remarks>
        public Int32 Gear;
        /// <summary>
        /// Total number of forward gears.
        /// </summary>
        /// <remarks>-1 = N/A</remarks>
        public Int32 NumGears;
        /// <summary>
        /// World-space position of vehicle center of gravity.
        /// </summary>
        /// <remarks>Y axis points up</remarks>
        public Vector3<Single> CarCgLocation;
        /// <summary>
        /// Vehicle body orientation.
        /// </summary>
        /// <remarks>Unit: Radians (Euler angles)</remarks>
        public Orientation<Single> CarOrientation;
        /// <summary>
        /// Local-space acceleration of vehicle body.
        /// </summary>
        /// <remarks>
        /// From car center: +X=left, +Y=up, +Z=back.
        /// Unit: Meters per second squared
        /// </remarks>
        public Vector3<Single> LocalAcceleration;
        /// <summary>
        /// Total vehicle mass.
        /// </summary>
        /// <remarks>Unit: Kilograms; includes car + penalty weight + fuel</remarks>
        public Single TotalMass;
        /// <summary>
        /// Remaining fuel.
        /// </summary>
        /// <remarks>
        /// Unit: Liters. FuelPerLap shows estimation when insufficient data,
        /// then maximum recorded fuel per lap. Not valid for remote players.
        /// </remarks>
        public Single FuelLeft;
        /// <summary>
        /// Fuel tank capacity.
        /// </summary>
        /// <remarks>Unit: Liters</remarks>
        public Single FuelCapacity;
        /// <summary>
        /// Estimated fuel consumption per lap.
        /// </summary>
        /// <remarks>Unit: Liters</remarks>
        public Single FuelPerLap;
        /// <summary>
        /// Remaining virtual energy (hybrid/electric).
        /// </summary>
        /// <remarks>
        /// Unit: Mega-Joules. -1.0 when insufficient data, then max recorded per lap.
        /// Not valid for remote players.
        /// </remarks>
        public Single VirtualEnergyLeft;
        /// <summary>
        /// Virtual energy capacity.
        /// </summary>
        /// <remarks>Unit: Mega-Joules</remarks>
        public Single VirtualEnergyCapacity;
        /// <summary>
        /// Estimated virtual energy consumption per lap.
        /// </summary>
        /// <remarks>Unit: Mega-Joules</remarks>
        public Single VirtualEnergyPerLap;
        /// <summary>
        /// Engine coolant temperature.
        /// </summary>
        /// <remarks>Unit: Celsius; Not valid for AI or remote players</remarks>
        public Single EngineTemp;
        /// <summary>
        /// Engine oil temperature.
        /// </summary>
        /// <remarks>Unit: Celsius; Not valid for AI or remote players</remarks>
        public Single EngineOilTemp;
        /// <summary>
        /// Fuel system pressure.
        /// </summary>
        /// <remarks>Unit: Kilopascals; Not valid for AI or remote players</remarks>
        public Single FuelPressure;
        /// <summary>
        /// Engine oil pressure.
        /// </summary>
        /// <remarks>Unit: Kilopascals; Not valid for AI or remote players</remarks>
        public Single EngineOilPressure;
        /// <summary>
        /// Turbocharger boost pressure.
        /// </summary>
        /// <remarks>Unit: Bar; -1.0 = N/A; Not valid for AI or remote players</remarks>
        public Single TurboPressure;
        /// <summary>
        /// Throttle pedal input.
        /// </summary>
        /// <remarks>Range: 0.0 - 1.0; -1.0 = N/A; Not valid for AI or remote players</remarks>
        public Single Throttle;
        /// <summary>Raw throttle sensor value.</summary>
        public Single ThrottleRaw;
        /// <summary>
        /// Brake pedal input.
        /// </summary>
        /// <remarks>Range: 0.0 - 1.0; -1.0 = N/A; Not valid for AI or remote players</remarks>
        public Single Brake;
        /// <summary>Raw brake sensor value.</summary>
        public Single BrakeRaw;
        /// <summary>
        /// Clutch pedal input.
        /// </summary>
        /// <remarks>Range: 0.0 - 1.0; -1.0 = N/A; Not valid for AI or remote players</remarks>
        public Single Clutch;
        /// <summary>Raw clutch sensor value.</summary>
        public Single ClutchRaw;
        /// <summary>
        /// Steering wheel input.
        /// </summary>
        /// <remarks>Range: -1.0 (full left) to 1.0 (full right); Not valid for AI or remote players</remarks>
        public Single SteerInputRaw;
        /// <summary>
        /// Steering lock angle.
        /// </summary>
        /// <remarks>Unit: Degrees from center to full lock; Not valid for AI or remote players</remarks>
        public Int32 SteerLockDegrees;
        /// <summary>
        /// Steering wheel rotation range.
        /// </summary>
        /// <remarks>Unit: Degrees full left to full right; Not valid for AI or remote players</remarks>
        public Int32 SteerWheelRangeDegrees;
        /// <summary>Current driver aid settings.</summary>
        public AidSettings AidSettings;
        /// <summary>DRS system status.</summary>
        public DRS Drs;
        /// <summary>
        /// Pit lane speed limiter status.
        /// </summary>
        /// <remarks>-1 = N/A, 0 = inactive, 1 = active</remarks>
        public Int32 PitLimiter;
        /// <summary>Push-to-Pass system status.</summary>
        public PushToPass PushToPass;
        /// <summary>
        /// Brake bias setting.
        /// </summary>
        /// <remarks>
        /// 0.3 = 30% rear bias, etc.
        /// -1.0 = N/A; Not valid for AI or remote players
        /// </remarks>
        public Single BrakeBias;
        /// <summary>
        /// Total DRS activations available.
        /// </summary>
        /// <remarks>-1 = N/A or unlimited</remarks>
        public Int32 DrsNumActivationsTotal;
        /// <summary>
        /// Total PTP activations available.
        /// </summary>
        /// <remarks>-1 = N/A, no per-lap restriction, or unlimited</remarks>
        public Int32 PtpNumActivationsTotal;
        /// <summary>
        /// Battery state of charge.
        /// </summary>
        /// <remarks>Range: 0.0 - 100.0 percent; -1.0 = N/A</remarks>
        public Single BatterySoC;
        /// <summary>
        /// Brake cooling water tank level.
        /// </summary>
        /// <remarks>Unit: Liters; -1.0 = N/A</remarks>
        public Single WaterLeft;
        /// <summary>
        /// ABS system setting level.
        /// </summary>
        /// <remarks>-1.0 = N/A</remarks>
        public Int32 AbsSetting;
        /// <summary>
        /// Headlight status.
        /// </summary>
        /// <remarks>
        /// -1 = N/A or not equipped, 0 = off, 1 = on, 2 = strobing
        /// </remarks>
        public Int32 HeadLights;
        /// <summary>
        /// Steering wheel maximum rotation setting.
        /// </summary>
        /// <remarks>
        /// -1 = N/A, 0 = Auto mode, 180-1800 = Manual degrees.
        /// Not valid for AI or remote players.
        /// </remarks>
        public Int32 SteerWheelMaxRotation;
        /// <summary>Reserved field for future expansion.</summary>
        public Single VehicleUnused1;
        #endregion

        #region Tire Data
        /// <summary>
        /// Tire compound type (deprecated).
        /// </summary>
        /// <remarks>
        /// See Constant.TireType enum. Use TireTypeFront/Rear instead.
        /// </remarks>
        public Int32 TireType;
        /// <summary>
        /// Tire rotation speed.
        /// </summary>
        /// <remarks>Unit: Radians per second</remarks>
        public TireData<Single> TireRps;
        /// <summary>
        /// Tire linear speed.
        /// </summary>
        /// <remarks>Unit: Meters per second</remarks>
        public TireData<Single> TireSpeed;
        /// <summary>
        /// Tire grip level.
        /// </summary>
        /// <remarks>Range: 0.0 - 1.0; -1.0 = N/A</remarks>
        public TireData<Single> TireGrip;
        /// <summary>
        /// Tire wear level.
        /// </summary>
        /// <remarks>Range: 0.0 - 1.0; -1.0 = N/A</remarks>
        public TireData<Single> TireWear;
        /// <summary>
        /// Tire flat spot detection.
        /// </summary>
        /// <remarks>-1 = N/A, 0 = false, 1 = true</remarks>
        public TireData<Int32> TireFlatspot;
        /// <summary>
        /// Tire pressure.
        /// </summary>
        /// <remarks>Unit: Kilopascals; -1.0 = N/A; Not valid for AI or remote players</remarks>
        public TireData<Single> TirePressure;
        /// <summary>
        /// Dirt accumulation on tire.
        /// </summary>
        /// <remarks>Range: 0.0 - 1.0; -1.0 = N/A</remarks>
        public TireData<Single> TireDirt;
        /// <summary>
        /// Tire temperature data.
        /// </summary>
        /// <remarks>
        /// Unit: Celsius. Not valid for AI or remote players.
        /// </remarks>
        public TireData<TireTempInformation> TireTemp;
        /// <summary>
        /// Front tire compound type.
        /// </summary>
        /// <remarks>See Constant.TireType enum</remarks>
        public Int32 TireTypeFront;
        /// <summary>
        /// Rear tire compound type.
        /// </summary>
        /// <remarks>See Constant.TireType enum</remarks>
        public Int32 TireTypeRear;
        /// <summary>
        /// Front tire subtype.
        /// </summary>
        /// <remarks>See Constant.TireSubtype enum</remarks>
        public Int32 TireSubtypeFront;
        /// <summary>
        /// Rear tire subtype.
        /// </summary>
        /// <remarks>See Constant.TireSubtype enum</remarks>
        public Int32 TireSubtypeRear;
        /// <summary>
        /// Brake temperature data.
        /// </summary>
        /// <remarks>Unit: Celsius; Not valid for AI or remote players</remarks>
        public TireData<BrakeTemp> BrakeTemp;
        /// <summary>
        /// Brake pressure.
        /// </summary>
        /// <remarks>Unit: KiloNewtons; -1.0 = N/A; Not valid for AI or remote players</remarks>
        public TireData<Single> BrakePressure;
        /// <summary>
        /// Traction control setting level.
        /// </summary>
        /// <remarks>-1.0 = N/A</remarks>
        public Int32 TractionControlSetting;
        /// <summary>Engine mapping selection.</summary>
        public Int32 EngineMapSetting;
        /// <summary>Engine brake setting level.</summary>
        public Int32 EngineBrakeSetting;
        /// <summary>
        /// Traction control intervention level.
        /// </summary>
        /// <remarks>Range: 0.0 - 100.0 percent; -1.0 = N/A</remarks>
        public Single TractionControlPercent;
        /// <summary>
        /// Surface material under each tire.
        /// </summary>
        /// <remarks>See Constant.MtrlType enum</remarks>
        public TireData<Int32> TireOnMtrl;
        /// <summary>
        /// Vertical load on each tire.
        /// </summary>
        /// <remarks>Unit: Newtons; -1.0 = N/A</remarks>
        public TireData<Single> TireLoad;
        #endregion

        #region Damage State
        /// <summary>
        /// Damage levels for vehicle components.
        /// </summary>
        /// <remarks>Not valid for AI or remote players</remarks>
        public CarDamage CarDamage;
        #endregion

        #region Driver Array
        /// <summary>
        /// Total number of vehicles in session (including player).
        /// </summary>
        public Int32 NumCars;
        /// <summary>
        /// Array of driver data in finishing order.
        /// </summary>
        /// <remarks>Maximum 128 entries</remarks>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
        public DriverData[] DriverData;
        #endregion

        /// <summary>
        /// Returns the size of the RaceRoomSharedMemory structure in bytes.
        /// </summary>
        /// <returns>Structure size in bytes</returns>
        public static int Length()
        {
            return Marshal.SizeOf(typeof(RaceRoomSharedMemory));
        }
    }
}