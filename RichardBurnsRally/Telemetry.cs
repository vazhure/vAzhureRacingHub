using System.Runtime.InteropServices;

namespace RichardBurnsRally
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct TireSegment
    {
        public float temperature;
        public float wear;
    };

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Tire
    {
        public float pressure;
        public float temperature;
        public float carcassTemperature;
        public float treadTemperature;
        public uint currentSegment;
        public TireSegment segment1;
        public TireSegment segment2;
        public TireSegment segment3;
        public TireSegment segment4;
        public TireSegment segment5;
        public TireSegment segment6;
        public TireSegment segment7;
        public TireSegment segment8;
    };

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BrakeDisk
    {
        public float layerTemperature;
        public float temperature;
        public float wear;
    };

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Wheel
    {
        public BrakeDisk brakeDisk;
        public Tire tire;
    };

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Damper
    {
        public float damage;
        public float pistonVelocity;
    };

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Suspension
    {
        public float springDeflection;
        public float rollbarForce;
        public float springForce;
        public float damperForce;
        public float strutForce;
        public int helperSpringIsActive;
        public Damper damper;
        public Wheel wheel;
    };

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Engine
    {
        public float rpm;
        public float radiatorCoolantTemperature;
        public float engineCoolantTemperature;
        public float engineTemperature;
    };

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Motion
    {
        /// <summary>
        /// Forward/backward.
        /// </summary>
        public float surge;
        /// <summary>
        /// Left/right.
        /// </summary>
        public float sway;
        /// <summary>
        /// Up/down.
        /// </summary>
        public float heave;
        /// <summary>
        /// Rotation about longitudinal axis.
        /// </summary>
        public float roll;
        /// <summary>
        /// Rotation about transverse axis.
        /// </summary>
        public float pitch;
        /// <summary>
        /// Rotation about normal axis.
        /// </summary>
        public float yaw;
    };

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Car
    {
        public int index;
        /// <summary>
        /// Speed of the car in kph or mph.
        /// </summary>
        public float speed;
        public float positionX;
        public float positionY;
        public float positionZ;
        public float roll;
        public float pitch;
        public float yaw;
        public Motion velocities;
        public Motion accelerations;
        public Engine engine;
        /// <summary>
        /// Suspension data: LF, RF, LB, RB.
        /// </summary>
        public Suspension suspensionLF;
        /// <summary>
        /// Suspension data: LF, RF, LB, RB.
        /// </summary>
        public Suspension suspensionRF;
        /// <summary>
        /// Suspension data: LF, RF, LB, RB.
        /// </summary>
        public Suspension suspensionLB;
        /// <summary>
        /// Suspension data: LF, RF, LB, RB.
        /// </summary>
        public Suspension suspensionRB;
    };

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Control
    {
        public float steering;
        public float throttle;
        public float brake;
        public float handbrake;
        public float clutch;
        public int gear;
        public float footbrakePressure;
        public float handbrakePressure;
    };

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Stage
    {
        public int index;
        /// <summary>
        /// The position on the driveline.
        /// </summary>
        public float progress;
        /// <summary>
        /// The total race time.
        /// </summary>
        public float raceTime;
        public float driveLineLocation;
        public float distanceToEnd;
    };

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct TelemetryData
    {
        public uint totalSteps;
        public Stage stage;
        public Control control;
        public Car car;
    };
}