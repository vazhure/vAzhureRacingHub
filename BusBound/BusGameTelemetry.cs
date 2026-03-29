using System.Runtime.InteropServices;

namespace BusBound
{
    public class BusGameTelemetry
    {
        public static readonly int BusGameTelemetrySize = Marshal.SizeOf(typeof(Game));

        /// <summary>
        /// Shared memory name
        /// </summary>
        public const string BUS_BOUND_SHARED_MEMORY_NAME = "{6E432EA0-2E71-42A2-AD36-1B946D1CE1A6}";

        public const int MAX_NUM_WHEELS = 10;
        public const int MAX_NUM_DOORS = 10;
        public const int MAX_NUM_BENDY_PARTS = 10;
        public const int MAX_NUM_RAMPS = 10;
        public const int MAX_NUM_LINE_STOPS = 250;
        public const int MAX_NUM_INCIDENTS = 100;
        public const int MAX_NUM_PERKS = 100;
        public const int MAX_NUM_DISTRICTS = 100;

        public const int RADIO_TRACK_MAX_CHAR_COUNT = 250;
        public const int BUS_MODEL_MAX_CHAR_COUNT = 250;
        public const int BUS_LED_MAX_CHAR_COUNT = 250;
        public const int PLAYER_NAME_MAX_CHAR_COUNT = 250;
        public const int WEATHER_NAME_MAX_CHAR_COUNT = 250;
        public const int STOP_NAME_MAX_CHAR_COUNT = 250;
        public const int LINE_NAME_MAX_CHAR_COUNT = 250;
        public const int LINE_NUMBER_NAME_MAX_CHAR_COUNT = 16;
        public const int INCIDENT_NAME_MAX_CHAR_COUNT = 250;
        public const int PERK_NAME_MAX_CHAR_COUNT = 250;
        public const int DISTRICT_Id_MAX_CHAR_COUNT = 250;
        public const int DISTRICT_NAME_MAX_CHAR_COUNT = 250;

        // Обновлена версия с 5 на 6
        public const int BUS_BOUND_TELEMETRY_MAJOR_VERSION = 0;
        public const int BUS_BOUND_TELEMETRY_MINOR_VERSION = 6;

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Vector3
        {
            public double x;
            public double y;
            public double z;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Color
        {
            public byte r;
            public byte g;
            public byte b;
            public byte a;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Vector2
        {
            public double x;
            public double y;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Quaternion
        {
            public double x;
            public double y;
            public double z;
            public double w;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct EulerAngles
        {
            public double roll;
            public double pitch;
            public double yaw;
        }

        public enum GroundType : byte
        {
            Street,
            OffRoad,
            Sidewalk
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Wheel
        {
            public Vector3 RelativePosition;
            public Vector3 RelativeRestingPosition;
            public Vector3 AxleDirection;
            public Vector3 SuspensionAxis;
            public Vector3 SuspensionForceOffset;

            public float SpringRate;
            public float SpringPreload;
            public float SuspensionDampingRatio;
            public float SuspensionMaxRaise;
            public float SuspensionMaxDrop;
            public float SuspensionOffset;

            public float SteeringAngleInDeg;
            public float MaxSteeringAngleInDeg;
            public float RadiusCM;
            public float AngleInDeg;
            public float AngularVelocityCMS;
            [MarshalAs(UnmanagedType.U1)]
            public bool IsInAir;
            public GroundType Ground;

            public float SpringForce;
            public float BrakeTorque;
            public float DriveTorque;

            [MarshalAs(UnmanagedType.U1)]
            public bool IsSkidding;
            public float SkidMagnitude;
            public Vector3 SkidNormal;

            [MarshalAs(UnmanagedType.U1)]
            public bool IsSlipping;
            public float SlipMagnitude;
            public float SlipAngle;

            [MarshalAs(UnmanagedType.U1)]
            public bool HasABS;
            [MarshalAs(UnmanagedType.U1)]
            public bool IsABSActivated;
            public float ABSFactor;
            [MarshalAs(UnmanagedType.U1)]
            public bool IsLocked;
        }

        public enum Gear : byte
        {
            Neutral,
            Drive,
            Reverse
        }

        public enum EngineType : byte
        {
            None,
            Diesel,
            Electric,
            Cng,
            Hydrogen,
            Any
        }

        public enum MainLightState : byte
        {
            Off,
            Parking,
            Spot
        }

        public enum WheelModeState : byte
        {
            WheelsLocked,
            ShiftMode,
            CrabMode
        }

        public enum BusStopBrakeState : byte
        {
            Open,
            Triggered,
            Closed
        }

        public enum IndicatorState : byte
        {
            Off,
            Left,
            Right
        }

        public enum IgnitionState : byte
        {
            Off,
            PowerOn,
            AllOn,
            EngineStarter
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Engine
        {
            public EngineType EngineType;
            public float RPM;
            public float MaxRPM;
            public sbyte Gear;
            public IgnitionState Ignition;
            [MarshalAs(UnmanagedType.U1)]
            public bool IsOn;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Door
        {
            [MarshalAs(UnmanagedType.U1)]
            public bool IsOpen;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Radio
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = RADIO_TRACK_MAX_CHAR_COUNT)]
            public string TrackName;
            [MarshalAs(UnmanagedType.U1)]
            public bool IsOn;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Ramp
        {
            public int AssignedDoor;
            public float RampExtension;
            [MarshalAs(UnmanagedType.U1)]
            public bool IsRampMoving;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Seating
        {
            public byte SumStanding;
            public byte SumSitting;
            public byte FreeStanding;
            public byte FreeSitting;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct DestinationData
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = STOP_NAME_MAX_CHAR_COUNT)]
            public string DestinationStopName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = DISTRICT_NAME_MAX_CHAR_COUNT)]
            public string DestinationDistrictName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = LINE_NUMBER_NAME_MAX_CHAR_COUNT)]
            public string LineId;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = LINE_NAME_MAX_CHAR_COUNT)]
            public string LineName;
            public Color LineColor;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct FixSizedStructWheels
        {
            public Wheel Data;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 258 - 226)]
            public byte[] reserved;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct FixSizedStructEngine
        {
            public Engine Data;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 258 - 18)]
            public byte[] reserved;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct FixSizedStructDoor
        {
            public Door Data;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 258 - 1)]
            public byte[] reserved;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct FixSizedStructRamp
        {
            public Ramp Data;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 258 - 9)]
            public byte[] reserved;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct FixSizedStructLine
        {
            public Line Data;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024 * 1024 - 69522)]
            public byte[] reserved;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct FixSizedStructBus
        {
            public PlayerBus Data;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024 * 1024 - 10066)]
            public byte[] reserved;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct FixSizedStructLineDrive
        {
            public LineDrive Data;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024 * 1024 * 2 - 1124808)]
            public byte[] reserved;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct FixSizedStructIncident
        {
            public Incident Data;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 512 - 280)]
            public byte[] reserved;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct FixSizedStructShiftData
        {
            public ShiftData Data;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024 - 290)]
            public byte[] reserved;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct FixSizedStructPlayerData
        {
            public PlayerData Data;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024 - 254)]
            public byte[] reserved;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct FixSizedStructDistrict
        {
            public District Data;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024 - 501)]
            public byte[] reserved;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct FixSizedStructOffice
        {
            public Office Data;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024)]
            public byte[] reserved;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct PlayerBus
        {
            public Vector3 Position;
            public EulerAngles Rotation;

            public sbyte BendyPartCount;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_NUM_BENDY_PARTS)]
            public Vector3[] Bendy_Position;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_NUM_BENDY_PARTS)]
            public EulerAngles[] Bendy_Rotation;

            public sbyte WheelCount;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_NUM_WHEELS)]
            public FixSizedStructWheels[] Wheels;

            public FixSizedStructEngine Engine;
            public Gear CurrentAutomaticGear;
            public float SpeedCMS;

            public Vector3 Velocity;
            public Vector3 Acceleration;
            public Vector3 LocalVelocity;
            public Vector3 LocalAcceleration;
            public Vector3 RotationalAcceleration;

            public float Throttle;
            public float Brake;
            public float SteeringAngle;
            public float MaxSteeringAngle;

            public sbyte DoorCount;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_NUM_DOORS)]
            public FixSizedStructDoor[] Doors;

            public Seating Seating;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = BUS_MODEL_MAX_CHAR_COUNT)]
            public string BusModelName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = BUS_LED_MAX_CHAR_COUNT)]
            public string BusLEDCustomText;
            public DestinationData Destination;

            public sbyte RampCount;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_NUM_RAMPS)]
            public FixSizedStructRamp[] Ramps;

            public BusStopBrakeState BusStopBrake;
            [MarshalAs(UnmanagedType.U1)]
            public bool IsParkingBrakeActive;
            [MarshalAs(UnmanagedType.U1)]
            public bool IsHandbrakeActive;
            public sbyte RetarderLevel;
            [MarshalAs(UnmanagedType.U1)]
            public bool IsKneeling;
            [MarshalAs(UnmanagedType.U1)]
            public bool IsSpeedLimiterActive;

            public IndicatorState IndicatorLightState;
            public MainLightState MainLightState;
            [MarshalAs(UnmanagedType.U1)]
            public bool IsFarLightOn;

            [MarshalAs(UnmanagedType.U1)]
            public bool IsCashierLightsOn;
            [MarshalAs(UnmanagedType.U1)]
            public bool IsAmberLightsOn;
            [MarshalAs(UnmanagedType.U1)]
            public bool IsPassengerLightsOn;
            [MarshalAs(UnmanagedType.U1)]
            public bool IsStopPaddleOn;
            [MarshalAs(UnmanagedType.U1)]
            public bool IsRedOverrideOn;
            [MarshalAs(UnmanagedType.U1)]
            public bool IsCockpitLightOn;
            [MarshalAs(UnmanagedType.U1)]
            public bool IsWarningLightsOn;

            [MarshalAs(UnmanagedType.U1)]
            public bool IsHornActivated;
            [MarshalAs(UnmanagedType.U1)]
            public bool AreWipersActive;

            [MarshalAs(UnmanagedType.U1)]
            public bool IsExteriorMirrorDefrostActive;
            [MarshalAs(UnmanagedType.U1)]
            public bool IsVariableGeometryTurboBrakeActive;
            [MarshalAs(UnmanagedType.U1)]
            public bool IsHighIdleActive;
            [MarshalAs(UnmanagedType.U1)]
            public bool IsParkLightsActive;
            [MarshalAs(UnmanagedType.U1)]
            public bool IsFan1Active;
            [MarshalAs(UnmanagedType.U1)]
            public bool IsFan2Active;
            [MarshalAs(UnmanagedType.U1)]
            public bool IsNoiseSuppressionActive;
            [MarshalAs(UnmanagedType.U1)]
            public bool IsLeftSideInteriorLightsActive;
            [MarshalAs(UnmanagedType.U1)]
            public bool IsRightSideInteriorLightsActive;
            [MarshalAs(UnmanagedType.U1)]
            public bool IsPedalAdjustmentActive;
            [MarshalAs(UnmanagedType.U1)]
            public bool IsDriverHeatherFanActive;
            [MarshalAs(UnmanagedType.U1)]
            public bool IsRearHeaterFanActive;
            [MarshalAs(UnmanagedType.U1)]
            public bool IsFrontHeatherFanActive;
            [MarshalAs(UnmanagedType.U1)]
            public bool IsServiceDoorActive;
            [MarshalAs(UnmanagedType.U1)]
            public bool IsWarningLightStartActive;
            [MarshalAs(UnmanagedType.U1)]
            public bool IsWarningLightsRedYellowActive;
            [MarshalAs(UnmanagedType.U1)]
            public bool IsWarningLightsCancelActive;
            [MarshalAs(UnmanagedType.U1)]
            public bool IsOkActive;
            [MarshalAs(UnmanagedType.U1)]
            public bool IsTestActive;
            [MarshalAs(UnmanagedType.U1)]
            public bool IsUnlockActive;
            [MarshalAs(UnmanagedType.U1)]
            public bool IsLockBackDoorActive;
            [MarshalAs(UnmanagedType.U1)]
            public bool IsFrontKneelingActive;
            [MarshalAs(UnmanagedType.U1)]
            public bool IsStopRequestActive;
            [MarshalAs(UnmanagedType.U1)]
            public bool IsDeepSnowMudActive;
            [MarshalAs(UnmanagedType.U1)]
            public bool IsRearDoorActuationActive;
            [MarshalAs(UnmanagedType.U1)]
            public bool IsRearDoorLockoutActive;
            [MarshalAs(UnmanagedType.U1)]
            public bool IsTVScreensActive;
            [MarshalAs(UnmanagedType.U1)]
            public bool IsDashboardLightsActive;
            [MarshalAs(UnmanagedType.U1)]
            public bool IsWinterModeForGearboxActive;
            [MarshalAs(UnmanagedType.U1)]
            public bool IsMuteSpeakersActive;
            [MarshalAs(UnmanagedType.U1)]
            public bool IsHeatedDriverSeatActive;
            [MarshalAs(UnmanagedType.U1)]
            public bool IsAssistedHillStartActive;
            [MarshalAs(UnmanagedType.U1)]
            public bool IsHoldToReleaseBreakReleaseActive;
            [MarshalAs(UnmanagedType.U1)]
            public bool IsNineOneOneActive;
            [MarshalAs(UnmanagedType.U1)]
            public bool IsNineOneOneRedCoverOpen;

            public float ACStrength;
            public float Temperature;
            public float ACZone;
            public float AirRecirculation;
            public float ClimateControl;
            public float DefrosterFan;
            public float MultiTool;
            public float OverHeadLHFan;
            public float OverHeadRHFan;
            public float WiperWasher;
            public float LHRemoteMirror;
            public float RHRemoteMirror;
            public float PanelLights;

            public WheelModeState ActiveWheelMode;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct PlayerData
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = PLAYER_NAME_MAX_CHAR_COUNT)]
            public string Name;
            public uint DistanceDrivenCM;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Weather
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = WEATHER_NAME_MAX_CHAR_COUNT)]
            public string Name;
            public float Wetness;
            public float Puddle;
            public float Rain;
            public float Fog;
            public float Wind;
            public float Cloud;
            public float Temperature;
            public int WeatherTheme;
        }

        public enum Shift : byte
        {
            Early,
            Mid,
            Late
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ShiftData
        {
            public uint Day;
            public Shift Shift;
            public Weather Weather;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct LineStop
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = STOP_NAME_MAX_CHAR_COUNT)]
            public string Name;
            public Vector3 Position;
            public sbyte Platform;
            public sbyte Boarding;
            public sbyte Alighting; // Исправлена опечатка
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Line
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = LINE_NAME_MAX_CHAR_COUNT)]
            public string Name;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = LINE_NUMBER_NAME_MAX_CHAR_COUNT)]
            public string Number;
            public Color Color;
            public ushort StopCount;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_NUM_LINE_STOPS)]
            public LineStop[] Stops;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Mood
        {
            public short MoodValue;
            public short MaxMoodValue;
            public sbyte MoodLevel;
            public sbyte MaxMoodLevel;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Incident
        {
            public float TimeStampS;
            public Vector3 Position;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = INCIDENT_NAME_MAX_CHAR_COUNT)]
            public string Name;
            public short MoodValueChange;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Perk
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = PERK_NAME_MAX_CHAR_COUNT)]
            public string Name;
        }

        public enum GameState : byte
        {
            Office,
            Drive
        }

        public enum DriveState : byte
        {
            OutOfDrive,
            LineDrive,
            OtherDrive
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct LineDrive
        {
            public float TimeStampS;
            public FixSizedStructLine Line;
            public short PreviousStop;
            public short NextStop;
            public float TimeToNextStopS;
            public double DistanceToNextStopCM;
            public float ParkingScore;
            public Mood Mood;
            public sbyte IncidentHistoryCount;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_NUM_INCIDENTS)]
            public FixSizedStructIncident[] IncidentHistory;
            public byte PerkCount;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_NUM_PERKS)]
            public Perk[] ActivePerks;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct District
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = DISTRICT_NAME_MAX_CHAR_COUNT)]
            public string Id;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = DISTRICT_NAME_MAX_CHAR_COUNT)]
            public string Name;
            public byte DistrictLevel;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Office
        {
            // Пустая структура, как в C++
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Game
        {
            // Добавлены поля версии, отсутствовавшие в предыдущей версии C#
            [MarshalAs(UnmanagedType.U1)]
            public byte TelemetryMajorVersion;
            [MarshalAs(UnmanagedType.U1)]
            public byte TelemetryMinorVersion;

            [MarshalAs(UnmanagedType.U1)]
            public bool IsGameActive;
            public double TimeStampMS;

            public FixSizedStructShiftData ShiftData;
            public FixSizedStructPlayerData PlayerData;
            public sbyte DistrictCount;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_NUM_DISTRICTS)]
            public FixSizedStructDistrict[] Districts;

            public GameState State;
            public FixSizedStructOffice Office;

            public DriveState DriveState;
            public FixSizedStructLineDrive LineDrive;

            [MarshalAs(UnmanagedType.U1)]
            public bool IsPlayerPossessingBus;
            public FixSizedStructBus Bus;
        }
    }
}