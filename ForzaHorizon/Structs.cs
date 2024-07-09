using System.Runtime.InteropServices;
/// <summary>
/// https://support.forzamotorsport.net/hc/en-us/articles/21742934024211-Forza-Motorsport-Data-Out-Documentation
/// FORZA_DATA_OUT_PORT = 5300;
/// FORZA_HOST_PORT = 5200;
/// </summary>
namespace ForzaMotorsport
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct FHSled
    {
        //Sled
        /// <summary>
        /// 1 when race is on. = 0 when in menus/race stopped
        /// </summary>
        public int IsRaceOn;
        /// <summary>
        /// Can overflow to 0 eventually
        /// </summary>
        public uint TimestampMS;
        public float EngineMaxRpm;
        public float EngineIdleRpm;
        public float CurrentEngineRpm;
        /// <summary>
        /// In the car's local space; right
        /// </summary>
        public float AccelerationX;
        /// <summary>
        /// In the car's local space; up
        /// </summary>
        public float AccelerationY;
        /// <summary>
        /// In the car's local space; forward
        /// </summary>

        public float AccelerationZ;
        /// <summary>
        /// In the car's local space; right
        /// </summary>
        public float VelocityX;
        /// <summary>
        /// In the car's local space; up
        /// </summary>
        public float VelocityY;
        /// <summary>
        /// In the car's local space; forward
        /// </summary>
        public float VelocityZ;
        // In the car's local space; X = pitch, Y = yaw, Z = roll
        public float AngularVelocityX;
        public float AngularVelocityY;
        public float AngularVelocityZ;
        public float Yaw;
        public float Pitch;
        public float Roll;
        /// <summary>
        /// Suspension travel normalized: 0.0f = max stretch; 1.0 = max compression
        /// </summary>
        public float NormalizedSuspensionTravelFrontLeft;
        /// <summary>
        /// Suspension travel normalized: 0.0f = max stretch; 1.0 = max compression
        /// </summary>
        public float NormalizedSuspensionTravelFrontRight;
        /// <summary>
        /// Suspension travel normalized: 0.0f = max stretch; 1.0 = max compression
        /// </summary>
        public float NormalizedSuspensionTravelRearLeft;
        /// <summary>
        /// Suspension travel normalized: 0.0f = max stretch; 1.0 = max compression
        /// </summary>
        public float NormalizedSuspensionTravelRearRight;
        /// <summary>
        /// Tire normalized slip ratio, = 0 means 100% grip and |ratio| > 1.0 means loss of grip.
        /// </summary>
        public float TireSlipRatioFrontLeft;
        /// <summary>
        /// Tire normalized slip ratio, = 0 means 100% grip and |ratio| > 1.0 means loss of grip.
        /// </summary>
        public float TireSlipRatioFrontRight;
        /// <summary>
        /// Tire normalized slip ratio, = 0 means 100% grip and |ratio| > 1.0 means loss of grip.
        /// </summary>
        public float TireSlipRatioRearLeft;
        /// <summary>
        /// Tire normalized slip ratio, = 0 means 100% grip and |ratio| > 1.0 means loss of grip.
        /// </summary>
        public float TireSlipRatioRearRight;
        /// <summary>
        /// Wheels rotation speed radians/sec. 
        /// </summary>
        public float WheelRotationSpeedFrontLeft;
        /// <summary>
        /// Wheels rotation speed radians/sec. 
        /// </summary>
        public float WheelRotationSpeedFrontRight;
        /// <summary>
        /// Wheels rotation speed radians/sec. 
        /// </summary>
        public float WheelRotationSpeedRearLeft;
        /// <summary>
        /// Wheels rotation speed radians/sec. 
        /// </summary>
        public float WheelRotationSpeedRearRight;
        // = 1 when wheel is on rumble strip, = 0 when off.
        public int WheelOnRumbleStripFrontLeft;
        public int WheelOnRumbleStripFrontRight;
        public int WheelOnRumbleStripRearLeft;
        public int heelOnRumbleStripRearRight;
        // = from 0 to 1, where 1 is the deepest puddle
        public float WheelInPuddleDepthFrontLeft;
        public float WheelInPuddleDepthFrontRight;
        public float WheelInPuddleDepthRearLeft;
        public float WheelInPuddleDepthRearRight;
        // Non-dimensional surface rumble values passed to controller force feedback
        public float SurfaceRumbleFrontLeft;
        public float SurfaceRumbleFrontRight;
        public float SurfaceRumbleRearLeft;
        public float SurfaceRumbleRearRight;
        /// <summary>
        /// Tire normalized slip angle, = 0 means 100% grip and |angle| > 1.0 means loss of grip.
        /// </summary>
        public float TireSlipAngleFrontLeft;
        /// <summary>
        /// Tire normalized slip angle, = 0 means 100% grip and |angle| > 1.0 means loss of grip.
        /// </summary>
        public float TireSlipAngleFrontRight;
        /// <summary>
        /// Tire normalized slip angle, = 0 means 100% grip and |angle| > 1.0 means loss of grip.
        /// </summary>
        public float TireSlipAngleRearLeft;
        /// <summary>
        /// Tire normalized slip angle, = 0 means 100% grip and |angle| > 1.0 means loss of grip.
        /// </summary>
        public float TireSlipAngleRearRight;
        /// <summary>
        /// Tire normalized combined slip, = 0 means 100% grip and |slip| > 1.0 means loss of grip.
        /// </summary>
        public float TireCombinedSlipFrontLeft;
        /// <summary>
        /// Tire normalized combined slip, = 0 means 100% grip and |slip| > 1.0 means loss of grip.
        /// </summary>
        public float TireCombinedSlipFrontRight;
        /// <summary>
        /// Tire normalized combined slip, = 0 means 100% grip and |slip| > 1.0 means loss of grip.
        /// </summary>
        public float TireCombinedSlipRearLeft;
        /// <summary>
        /// Tire normalized combined slip, = 0 means 100% grip and |slip| > 1.0 means loss of grip.
        /// </summary>
        public float TireCombinedSlipRearRight;
        /// <summary>
        /// Actual suspension travel in meters
        /// </summary>
        public float SuspensionTravelMetersFrontLeft;
        /// <summary>
        /// Actual suspension travel in meters
        /// </summary>
        public float SuspensionTravelMetersFrontRight;
        /// <summary>
        /// Actual suspension travel in meters
        /// </summary>
        public float SuspensionTravelMetersRearLeft;
        /// <summary>
        /// Actual suspension travel in meters
        /// </summary>
        public float SuspensionTravelMetersRearRight;
        /// <summary>
        /// Unique ID of the car make/model
        /// </summary>
        public int CarOrdinal;
        // Between 0 (D -- worst cars) and 7 (X class -- best cars) inclusive         
        public int CarClass;
        // Between 100 (worst car) and 999 (best car) inclusive
        public int CarPerformanceIndex;
        /// <summary>
        /// 0 = FWD, 1 = RWD, 2 = AWD 
        /// </summary>
        public int DrivetrainType;
        /// <summary>
        /// Number of cylinders in the engine
        /// </summary>
        public int NumCylinders;

        public static readonly int nSize = Marshal.SizeOf(typeof(FHSled));
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct FHDash
    {
        /// <summary>
        /// 1 when race is on. = 0 when in menus/race stopped
        /// </summary>
        public int IsRaceOn;
        /// <summary>
        /// Can overflow to 0 eventually
        /// </summary>
        public uint TimestampMS;
        public float EngineMaxRpm;
        public float EngineIdleRpm;
        public float CurrentEngineRpm;
        /// <summary>
        /// In the car's local space; X = right, Y = up, Z = forward
        /// </summary>
        public float AccelerationX;
        public float AccelerationY;
        public float AccelerationZ;
        /// <summary>
        /// In the car's local space; X = right, Y = up, Z = forward
        /// </summary>
        public float VelocityX;
        public float VelocityY;
        public float VelocityZ;
        /// <summary>
        /// In the car's local space; X = pitch, Y = yaw, Z = roll
        /// </summary>
        public float AngularVelocityX;
        public float AngularVelocityY;
        public float AngularVelocityZ;
        public float Yaw;
        public float Pitch;
        public float Roll;
        /// <summary>
        /// Suspension travel normalized: 0.0f = max stretch; 1.0 = max compression
        /// </summary>
        public float NormalizedSuspensionTravelFrontLeft;
        public float NormalizedSuspensionTravelFrontRight;
        public float NormalizedSuspensionTravelRearLeft;
        public float NormalizedSuspensionTravelRearRight;
        /// <summary>
        /// Tire normalized slip ratio, = 0 means 100% grip and |ratio| > 1.0 means loss of grip.
        /// </summary>
        public float TireSlipRatioFrontLeft;
        public float TireSlipRatioFrontRight;
        public float TireSlipRatioRearLeft;
        public float TireSlipRatioRearRight;
        /// <summary>
        /// Wheels rotation speed radians/sec. 
        /// </summary>
        public float WheelRotationSpeedFrontLeft;
        public float WheelRotationSpeedFrontRight;
        public float WheelRotationSpeedRearLeft;
        public float WheelRotationSpeedRearRight;
        /// <summary>
        /// 1 when wheel is on rumble strip, = 0 when off.
        /// </summary>
        public int WheelOnRumbleStripFrontLeft;
        public int WheelOnRumbleStripFrontRight;
        public int WheelOnRumbleStripRearLeft;
        public int heelOnRumbleStripRearRight;
        /// <summary>
        /// from 0 to 1, where 1 is the deepest puddle
        /// </summary>
        public float WheelInPuddleDepthFrontLeft;
        public float WheelInPuddleDepthFrontRight;
        public float WheelInPuddleDepthRearLeft;
        public float WheelInPuddleDepthRearRight;
        /// <summary>
        /// Non-dimensional surface rumble values passed to controller force feedback
        /// </summary>
        public float SurfaceRumbleFrontLeft;
        public float SurfaceRumbleFrontRight;
        public float SurfaceRumbleRearLeft;
        public float SurfaceRumbleRearRight;
        /// <summary>
        /// Tire normalized slip angle, = 0 means 100% grip and |angle| > 1.0 means loss of grip.
        /// </summary>
        public float TireSlipAngleFrontLeft;
        public float TireSlipAngleFrontRight;
        public float TireSlipAngleRearLeft;
        public float TireSlipAngleRearRight;
        /// <summary>
        /// Tire normalized combined slip, = 0 means 100% grip and |slip| > 1.0 means loss of grip.
        /// </summary>
        public float TireCombinedSlipFrontLeft;
        public float TireCombinedSlipFrontRight;
        public float TireCombinedSlipRearLeft;
        public float TireCombinedSlipRearRight;
        /// <summary>
        /// Actual suspension travel in meters
        /// </summary>
        public float SuspensionTravelMetersFrontLeft;
        public float SuspensionTravelMetersFrontRight;
        public float SuspensionTravelMetersRearLeft;
        public float SuspensionTravelMetersRearRight;
        /// <summary>
        /// Unique ID of the car make/model
        /// </summary>
        public int CarOrdinal;
        /// <summary>
        /// Between 0 (D -- worst cars) and 7 (X class -- best cars) inclusive         
        /// </summary>
        public int CarClass;
        /// <summary>
        /// Between 100 (worst car) and 999 (best car) inclusive
        /// </summary>
        public int CarPerformanceIndex;
        /// <summary>
        /// 0 = FWD, 1 = RWD, 2 = AWD
        /// </summary>
        public int DrivetrainType;
        /// <summary>
        /// Number of cylinders in the engine
        /// </summary>
        public int NumCylinders;

        public int CarTypeRaw;
                             
        public int Unknown1;
        public int Unknown2;

        public float PositionX;
        public float PositionY;
        public float PositionZ;
        public float Speed;
        public float Power;
        public float Torque;
        public float TireTempFrontLeft;
        public float TireTempFrontRight;
        public float TireTempRearLeft;
        public float TireTempRearRight;
        public float Boost;
        public float Fuel;
        public float DistanceTraveled;
        public float BestLapTime;
        public float LastLapTime;
        public float CurrentLapTime;
        public float CurrentRaceTime;
        public ushort LapNumber;
        public byte RacePosition;
        public byte Accel;
        public byte Brake;
        public byte Clutch;
        public byte HandBrake;
        public byte Gear;
        public sbyte Steer;
        public sbyte NormalizedDrivingLine;
        public sbyte NormalizedAIBrakeDifference;
        //// additional
        //public float TireWearFrontLeft;
        //public float TireWearFrontRight;
        //public float TireWearRearLeft;
        //public float TireWearRearRight;
        //// ID for track
        //public int TrackOrdinal;

        public static readonly int nSize = Marshal.SizeOf(typeof(FHDash));
    }

    public enum ForzaDataVersion : byte
    {
        Unknown = 0,
        Sled = 1,
        CarDash = 2,
        HorizonCarDash = 3
    }

    public struct ForzaDataStruct
    {
        /// <summary>
        /// Protocol version
        /// </summary>
        public ForzaDataVersion Version;

        /// <summary>
        /// Sled data
        /// </summary>
        public FHSled Sled;

        /// <summary>
        /// Car dash data
        /// </summary>
        public FHDash? CarDash;

        /// <summary>
        /// Horizon specific car dash data
        /// </summary>
        public byte[] HorizonCarDash;
    }

    public struct FH8
    {
        // = 1 when race is on. = 0 when in menus/race stopped …
        public int IsRaceOn;
        
        // Can overflow to 0 eventually
        public uint TimestampMS;
        public float EngineMaxRpm;
        public float EngineIdleRpm;
        public float CurrentEngineRpm;
        
        // In the car's local space; X = right, Y = up, Z = forward
        public float AccelerationX;
        public float AccelerationY;
        public float AccelerationZ;
        
        // In the car's local space; X = right, Y = up, Z = forward
        public float VelocityX;
        public float VelocityY;
        public float VelocityZ;
        
        // In the car's local space; X = pitch, Y = yaw, Z = roll
        public float AngularVelocityX;
        public float AngularVelocityY;
        public float AngularVelocityZ;
        
        public float Yaw;
        public float Pitch;
        public float Roll;
        
        // Suspension travel normalized: 0.0f = max stretch; 1.0 = max compression
        public float NormalizedSuspensionTravelFrontLeft;
        public float NormalizedSuspensionTravelFrontRight;
        public float NormalizedSuspensionTravelRearLeft;
        public float NormalizedSuspensionTravelRearRight;
        
        // Tire normalized slip ratio, = 0 means 100% grip and |ratio| > 1.0 means loss of grip.
        public float TireSlipRatioFrontLeft;
        public float TireSlipRatioFrontRight;
        public float TireSlipRatioRearLeft;
        public float TireSlipRatioRearRight;
        
        // Wheels rotation speed radians/sec. 
        public float WheelRotationSpeedFrontLeft;
        public float WheelRotationSpeedFrontRight;
        public float WheelRotationSpeedRearLeft;
        public float WheelRotationSpeedRearRight;
        
        // = 1 when wheel is on rumble strip, = 0 when off.
        public int WheelOnRumbleStripFrontLeft;
        public int WheelOnRumbleStripFrontRight;
        public int WheelOnRumbleStripRearLeft;
        public int heelOnRumbleStripRearRight;
        
        // = from 0 to 1, where 1 is the deepest puddle
        public float WheelInPuddleDepthFrontLeft;
        public float WheelInPuddleDepthFrontRight;
        public float WheelInPuddleDepthRearLeft;
        public float WheelInPuddleDepthRearRight;
        
        // Non-dimensional surface rumble values passed to controller force feedback
        public float SurfaceRumbleFrontLeft;
        public float SurfaceRumbleFrontRight;
        public float SurfaceRumbleRearLeft;
        public float SurfaceRumbleRearRight;
        
        // Tire normalized slip angle, = 0 means 100% grip and |angle| > 1.0 means loss of grip.
        public float TireSlipAngleFrontLeft;
        public float TireSlipAngleFrontRight;
        public float TireSlipAngleRearLeft;
        public float TireSlipAngleRearRight;
        
        // Tire normalized combined slip, = 0 means 100% grip and |slip| > 1.0 means loss of grip.
        public float TireCombinedSlipFrontLeft;
        public float TireCombinedSlipFrontRight;
        public float TireCombinedSlipRearLeft;
        public float TireCombinedSlipRearRight;
        
        // Actual suspension travel in meters
        public float SuspensionTravelMetersFrontLeft;
        public float SuspensionTravelMetersFrontRight;
        public float SuspensionTravelMetersRearLeft;
        public float SuspensionTravelMetersRearRight;
        
        // Unique ID of the car make/model
        public int CarOrdinal;
        
        // Between 0 (D -- worst cars) and 7 (X class -- best cars) inclusive         
        public int CarClass;
        
        // Between 100 (worst car) and 999 (best car) inclusive
        public int CarPerformanceIndex;
        
        // 0 = FWD, 1 = RWD, 2 = AWD
        public int DrivetrainType;
        
        // Number of cylinders in the engine
        public int NumCylinders;
        public float PositionX;
        public float PositionY;
        public float PositionZ;
        public float Speed;
        public float Power;
        public float Torque;
        public float TireTempFrontLeft;
        public float TireTempFrontRight;
        public float TireTempRearLeft;
        public float TireTempRearRight;
        public float Boost;
        public float Fuel;
        public float DistanceTraveled;
        public float BestLap;
        public float LastLap;
        public float CurrentLap;
        public float CurrentRaceTime;
        public short LapNumber;
        public byte RacePosition;
        public byte Accel;
        public byte Brake;
        public byte Clutch;
        public byte HandBrake;
        public byte Gear;
        public sbyte Steer;
        public sbyte NormalizedDrivingLine;
        public sbyte NormalizedAIBrakeDifference;
        
        public float TireWearFrontLeft;
        public float TireWearFrontRight;
        public float TireWearRearLeft;
        public float TireWearRearRight;
        
        // ID for track
        public int TrackOrdinal;
    }
}