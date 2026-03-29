using System;
using System.Runtime.InteropServices;

namespace BusBound
{
    public class BusGameTelemetry
    {
        public static readonly int BusGameTelemetrySize = Marshal.SizeOf(typeof(Game));

        public static class BusBoundConstants
        {
            public const string SharedMemoryName = "{6E432EA0-2E71-42A2-AD36-1B946D1CE1A6}";
            public const int MaxNumWheels = 10;
            public const int MaxNumDoors = 10;
            public const int MaxNumBendyParts = 10;
            public const int MaxNumRamps = 10;
            public const int MaxNumLineStops = 250;
            public const int MaxNumIncidents = 100;
            public const int MaxNumPerks = 100;
            public const int MaxNumDistricts = 100;

            public const int RadioTrackMaxCharCount = 250;
            public const int BusModelMaxCharCount = 250;
            public const int BusLedMaxCharCount = 250;
            public const int PlayerNameMaxCharCount = 250;
            public const int WeatherNameMaxCharCount = 250;
            public const int StopNameMaxCharCount = 250;
            public const int LineNameMaxCharCount = 250;
            public const int LineNumberNameMaxCharCount = 16;
            public const int IncidentNameMaxCharCount = 250;
            public const int PerkNameMaxCharCount = 250;
            public const int DistrictIdMaxCharCount = 250;
            public const int DistrictNameMaxCharCount = 250;

            public const byte TelemetryMajorVersion = 0;
            public const byte TelemetryMinorVersion = 6;
        }

        #region Basic Types

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Vector3
        {
            public double X;
            public double Y;
            public double Z;

            public Vector3(double x, double y, double z)
            {
                X = x;
                Y = y;
                Z = z;
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Color
        {
            public byte R;
            public byte G;
            public byte B;
            public byte A;

            public Color(byte r, byte g, byte b, byte a)
            {
                R = r;
                G = g;
                B = b;
                A = a;
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Vector2
        {
            public double X;
            public double Y;

            public Vector2(double x, double y)
            {
                X = x;
                Y = y;
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Quaternion
        {
            public double X;
            public double Y;
            public double Z;
            public double W;

            public Quaternion(double x, double y, double z, double w)
            {
                X = x;
                Y = y;
                Z = z;
                W = w;
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct EulerAngles
        {
            public double Roll;
            public double Pitch;
            public double Yaw;

            public EulerAngles(double roll, double pitch, double yaw)
            {
                Roll = roll;
                Pitch = pitch;
                Yaw = yaw;
            }
        }

        #endregion

        #region Enums

        public enum GroundType : byte
        {
            Street,
            OffRoad,
            Sidewalk
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

        public enum Shift : byte
        {
            Early,
            Mid,
            Late
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

        #endregion

        #region FixSized Wrapper Structures

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct FixSizedWheel
        {
            public Wheel Data;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 35)]
            public byte[] Padding;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct FixSizedEngine
        {
            public Engine Data;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 246)]
            public byte[] Padding;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct FixSizedDoor
        {
            public Door Data;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 257)]
            public byte[] Padding;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct FixSizedRamp
        {
            public Ramp Data;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 249)]
            public byte[] Padding;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct FixSizedLine
        {
            public Line Data;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 979054)]
            public byte[] Padding;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct FixSizedIncident
        {
            public Incident Data;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 232)]
            public byte[] Padding;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct FixSizedShiftData
        {
            public ShiftData Data;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 737)]
            public byte[] Padding;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct FixSizedPlayerData
        {
            public PlayerData Data;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 770)]
            public byte[] Padding;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct FixSizedDistrict
        {
            public District Data;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 523)]
            public byte[] Padding;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct FixSizedOffice
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024)]
            public byte[] Padding;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct FixSizedLineDrive
        {
            public LineDrive Data;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 972344)]
            public byte[] Padding;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct FixSizedPlayerBus
        {
            public PlayerBus Data;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1038525)]
            public byte[] Padding;
        }

        #endregion

        #region Data Structures

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

        [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
        public struct Radio
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = BusBoundConstants.RadioTrackMaxCharCount)]
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

        [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
        public struct DestinationData
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = BusBoundConstants.StopNameMaxCharCount)]
            public string DestinationStopName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = BusBoundConstants.DistrictNameMaxCharCount)]
            public string DestinationDistrictName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = BusBoundConstants.LineNumberNameMaxCharCount)]
            public string LineId;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = BusBoundConstants.LineNameMaxCharCount)]
            public string LineName;
            public Color LineColor;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
        public struct PlayerBus
        {
            public Vector3 Position;
            public EulerAngles Rotation;

            public sbyte BendyPartCount;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = BusBoundConstants.MaxNumBendyParts)]
            public Vector3[] Bendy_Position;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = BusBoundConstants.MaxNumBendyParts)]
            public EulerAngles[] Bendy_Rotation;

            public sbyte WheelCount;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = BusBoundConstants.MaxNumWheels)]
            public FixSizedWheel[] Wheels;

            public FixSizedEngine Engine;
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
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = BusBoundConstants.MaxNumDoors)]
            public FixSizedDoor[] Doors;

            public Seating Seating;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = BusBoundConstants.BusModelMaxCharCount)]
            public string BusModelName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = BusBoundConstants.BusLedMaxCharCount)]
            public string BusLEDCustomText;
            public DestinationData Destination;

            public sbyte RampCount;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = BusBoundConstants.MaxNumRamps)]
            public FixSizedRamp[] Ramps;

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

        [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
        public struct PlayerData
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = BusBoundConstants.PlayerNameMaxCharCount)]
            public string Name;
            public uint DistanceDrivenCM;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
        public struct Weather
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = BusBoundConstants.WeatherNameMaxCharCount)]
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

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ShiftData
        {
            public uint Day;
            public Shift Shift;
            public Weather Weather;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
        public struct LineStop
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = BusBoundConstants.StopNameMaxCharCount)]
            public string Name;
            public Vector3 Position;
            public sbyte Platform;

            public sbyte Boarding;
            public sbyte Alighting;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
        public struct Line
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = BusBoundConstants.LineNameMaxCharCount)]
            public string Name;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = BusBoundConstants.LineNumberNameMaxCharCount)]
            public string Number;
            public Color Color;
            public ushort StopCount;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = BusBoundConstants.MaxNumLineStops)]
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

        [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
        public struct Incident
        {
            public float TimeStampS;
            public Vector3 Position;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = BusBoundConstants.IncidentNameMaxCharCount)]
            public string Name;
            public short MoodValueChange;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
        public struct Perk
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = BusBoundConstants.PerkNameMaxCharCount)]
            public string Name;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct LineDrive
        {
            public float TimeStampS;
            public FixSizedLine Line;

            public short PreviousStop;
            public short NextStop;
            public float TimeToNextStopS;
            public double DistanceToNextStopCM;

            public float ParkingScore;

            public Mood Mood;

            public sbyte IncidentHistoryCount;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = BusBoundConstants.MaxNumIncidents)]
            public FixSizedIncident[] IncidentHistory;

            public byte PerkCount;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = BusBoundConstants.MaxNumPerks)]
            public Perk[] ActivePerks;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
        public struct District
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = BusBoundConstants.DistrictNameMaxCharCount)]
            public string Id;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = BusBoundConstants.DistrictNameMaxCharCount)]
            public string Name;
            public byte DistrictLevel;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Office
        {
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Game
        {
            public byte TelemetryMajorVersion;
            public byte TelemetryMinorVersion;

            [MarshalAs(UnmanagedType.U1)]
            public bool IsGameActive;
            public double TimeStampMS;

            public FixSizedShiftData ShiftData;
            public FixSizedPlayerData PlayerData;
            public sbyte DistrictCount;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = BusBoundConstants.MaxNumDistricts)]
            public FixSizedDistrict[] Districts;

            public GameState State;
            public FixSizedOffice Office;

            public DriveState DriveState;
            public FixSizedLineDrive LineDrive;

            [MarshalAs(UnmanagedType.U1)]
            public bool IsPlayerPossessingBus;
            public FixSizedPlayerBus Bus;
        }

        #endregion

        #region Helper Methods

        public static class TelemetryHelpers
        {
            public static Game ReadFromPtr(IntPtr ptr)
            {
                return Marshal.PtrToStructure<Game>(ptr);
            }

            public static Game ReadFromBytes(byte[] bytes)
            {
                GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
                try
                {
                    return Marshal.PtrToStructure<Game>(handle.AddrOfPinnedObject());
                }
                finally
                {
                    handle.Free();
                }
            }
        }

        #endregion
    }
}