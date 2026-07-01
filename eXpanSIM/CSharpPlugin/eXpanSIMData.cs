using System;
using System.Runtime.InteropServices;

namespace eXpanSIM
{
    /// <summary>
    /// Telemetry data structure shared between eXpanSIM C++ plugin and C# client.
    /// Must match exactly the C++ struct in SharedMemoryTelemetry.hpp (pack=1).
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct eXpanSIMTelemetryData
    {
        public uint Version;           // TELEMETRY_VERSION = 1
        public uint Sequence;          // Incremented each update
        public uint Valid;             // 1 = data valid, 0 = no vehicle spawned

        // Position (world space, meters)
        public double PosX;
        public double PosY;
        public double PosZ;

        // Orientation (radians)
        public double Pitch;
        public double Yaw;
        public double Roll;

        // Linear velocity (m/s)
        public double VelX;
        public double VelY;
        public double VelZ;

        // Linear acceleration (m/s^2)
        public double AccX;
        public double AccY;
        public double AccZ;

        // Angular velocity (rad/s)
        public double AngVelX;
        public double AngVelY;
        public double AngVelZ;

        // Angular acceleration (rad/s^2)
        public double AngAccX;
        public double AngAccY;
        public double AngAccZ;

        // Center of mass offset (meters)
        public double CoMX;
        public double CoMY;
        public double CoMZ;

        // Dashboard data
        public double Speed;               // m/s
        public double Rpm;
        public double MaxRpm;
        public double DistanceTraveled;    // meters
        public double FuelConsumptionLpH;
        public double FuelTankReserveNorm; // 0.0 - 1.0

        // Session state
        public uint VehicleSpawned;        // 1 = vehicle active
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 15)]
        public uint[] Reserved;            // For future expansion

        public static readonly int Size = Marshal.SizeOf(typeof(eXpanSIMTelemetryData));
    }
}
