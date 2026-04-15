using System;
using System.Runtime.InteropServices;

namespace MudRunner
{
    /// <summary>
    /// MudRunner telemetry data structures and text protocol parser.
    /// </summary>
    public static class MudRunnerTelemetry
    {
        public const int MaxWheels = 16;
        public const byte ProtocolVersion = 1;

        #region Basic Types

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Vec3
        {
            public float X;
            public float Y;
            public float Z;

            public Vec3(float x, float y, float z)
            {
                X = x; Y = y; Z = z;
            }

            public float Length => (float)Math.Sqrt(X * X + Y * Y + Z * Z);
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct WheelData
        {
            public float SuspensionCompression;
            public float AngularVelocity;
            public float LinearVelocity;
            public float Radius;
            public float MudDepth;
            public float WaterDepth;
            public float SteeringAngle;
            public byte ContactType; // 0=None, 1=Ground, 2=Mud, 3=Water, 4=Asphalt
            public byte IsPowered;
            public byte IsSteered;
            public byte Padding;
        }

        #endregion

        #region Enums

        public enum GameState : byte
        {
            Unknown = 0,
            MainMenu = 1,
            InGame = 2,
            Paused = 3,
            Loading = 4
        }

        public enum SurfaceType : byte
        {
            None = 0,
            Asphalt = 1,
            Dirt = 2,
            Mud = 3,
            Water = 4,
            Grass = 5,
            Snow = 6,
            Ice = 7
        }

        #endregion

        #region Main Telemetry Structure

        /// <summary>
        /// Parsed telemetry packet from Lua text protocol.
        /// Populated by ParseTextLine() from pipe data.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct TelemetryPacket
        {
            // Header
            public ushort Sequence;      // Packet sequence counter (assigned by C#)

            // Timestamp
            public double Timestamp;     // Game time in seconds

            // Position and Orientation
            public float PositionX;
            public float PositionY;
            public float PositionZ;
            public float AngVelX;          // Smoothed angular velocity X (rad/s)
            public float AngVelZ;          // Smoothed angular velocity Z (rad/s)
            public float AngVelY;          // Smoothed angular velocity Y (rad/s)

            public float AccelX;          // 
            public float AccelZ;          // 
            public float AccelY;          // 

            // Linear Velocity
            public float VelocityX;      // m/s
            public float VelocityY;      // m/s
            public float VelocityZ;      // m/s
            public float SpeedKmh;       // km/h (magnitude of velocity)

            // Linear Acceleration
            public float SmoothSurge;    // Filtered surge acceleration
            public float SmoothSway;     // Filtered sway acceleration
            public float SmoothHeave;    // Filtered heave acceleration

            // Driver Input
            public float SteeringAngle;  // radians
            public float Throttle;       // 0.0 - 1.0
            public float Brake;          // 0.0 - 1.0
            public float Clutch;         // 0.0 - 1.0 (not available in MudRunner, always 1.0)
            public int Gear;             // Current gear (0=N, -1=R, 1+=forward)

            // Engine
            public float RPM;            // Engine RPM (estimated)
            public float MaxRPM;         // Maximum RPM (6000)
            public float EngineLoad;     // Engine tension 0.0 - 1.0
            public float Turbo;          // Turbo boost 0.0 - 1.0

            // Vehicle State
            public float DamageFactor;   // 0.0 - 1.0 (1.0 = destroyed)
            // NOTE: Fuel is NOT exposed by MudRunner's Lua API.
            // Fuel level is handled internally by the C++ engine.
            public float Fuel;           // Always 0 (not available from Lua)
            public float SuspensionAvg;  // Average suspension compression
            public byte Handbrake;       // 1 = active
            public byte DiffLock;        // 1 = locked
            public byte AWD;             // 1 = all wheel drive engaged
            public byte EngineRunning;   // 1 = engine on
            public byte Headlights;      // 1 = headlights on
            public byte WinchActive;     // 1 = winch in use
            public byte ParkingBrake;    // 1 = parking brake

            // Surface Info
            public float MaxMudDepth;
            public float MaxWaterDepth;
            public SurfaceType CurrentSurface;

            // Wheels
            public byte WheelCount;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaxWheels)]
            public WheelData[] Wheels;

            public bool bValid;
        }

        #endregion

        #region Text Protocol Parser

        /// <summary>
        /// Parses telemetry line from the Lua text protocol.
        /// </summary>
        public static TelemetryPacket ParseTextLine(string line)
        {
            var packet = new TelemetryPacket
            {
                Clutch = 1.0f,
                MaxRPM = 6000f,
                bValid = false
            };

            try
            {
                if (string.IsNullOrEmpty(line) || !line.StartsWith("T|"))
                    return packet;

                // Initialize wheels array
                packet.Wheels = new WheelData[MaxWheels];

                string[] parts = line.Split('|');
                if (parts.Length < 36)
                    return packet;

                int i = 1; // Skip "T" prefix

                // Timestamp
                packet.Timestamp = ParseFloat(parts[i++]);

                // Speed
                packet.SpeedKmh = ParseFloat(parts[i++]);

                // Driver Input
                packet.SteeringAngle = ParseFloat(parts[i++]); // degrees from Lua
                packet.Throttle = ParseFloat(parts[i++]);
                packet.Brake = ParseFloat(parts[i++]);
                packet.Gear = ParseInt(parts[i++]) + 1;
                packet.Handbrake = ParseByte(parts[i++]);
                packet.DiffLock = ParseByte(parts[i++]);
                packet.AWD = ParseByte(parts[i++]);
                packet.EngineRunning = ParseByte(parts[i++]);
                packet.Headlights = ParseByte(parts[i++]);

                // Linear Velocity
                packet.VelocityX = ParseFloat(parts[i++]);
                packet.VelocityY = ParseFloat(parts[i++]);
                packet.VelocityZ = ParseFloat(parts[i++]);

                // Angular Velocity
                packet.AngVelX = ParseFloat(parts[i++]); // angVelX
                packet.AngVelY = ParseFloat(parts[i++]); // angVelY
                packet.AngVelZ = ParseFloat(parts[i++]); // angVelZ

                // Linear Acceleration
                packet.AccelX = ParseFloat(parts[i++]); // accelX
                packet.AccelY = ParseFloat(parts[i++]); // accelY
                packet.AccelZ = ParseFloat(parts[i++]); // accelZ

                // Smoothed motion data
                packet.SmoothSurge = ParseFloat(parts[i++]);
                packet.SmoothSway = ParseFloat(parts[i++]);
                packet.SmoothHeave = ParseFloat(parts[i++]);

                // Position
                packet.PositionX = ParseFloat(parts[i++]);
                packet.PositionY = ParseFloat(parts[i++]);
                packet.PositionZ = ParseFloat(parts[i++]);

                // Engine
                packet.RPM = ParseFloat(parts[i++]);
                packet.MaxRPM = ParseFloat(parts[i++]);
                packet.EngineLoad = ParseFloat(parts[i++]);
                packet.Turbo = ParseFloat(parts[i++]);

                // Vehicle State
                packet.DamageFactor = ParseFloat(parts[i++]);
                packet.SuspensionAvg = ParseFloat(parts[i++]);
                packet.MaxMudDepth = ParseFloat(parts[i++]);
                packet.MaxWaterDepth = ParseFloat(parts[i++]);

                // Fuel: NOT available in MudRunner Lua. Always 0.
                packet.Fuel = 0f;

                // Wheels
                packet.WheelCount = ParseByte(parts[i++]);

                // Determine surface type
                if (packet.MaxWaterDepth > 0.1f)
                    packet.CurrentSurface = SurfaceType.Water;
                else if (packet.MaxMudDepth > 0.1f)
                    packet.CurrentSurface = SurfaceType.Mud;
                else
                    packet.CurrentSurface = SurfaceType.Dirt;

                packet.bValid = true;
            }
            catch
            {
                // Return default packet on parse error
            }

            return packet;
        }

        #endregion

        #region Helper Methods

        private static float ParseFloat(string s)
        {
            if (float.TryParse(s, System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture, out float result))
                return result;
            return 0f;
        }

        private static int ParseInt(string s)
        {
            if (int.TryParse(s, System.Globalization.NumberStyles.Integer,
                System.Globalization.CultureInfo.InvariantCulture, out int result))
                return result;
            return 0;
        }

        private static byte ParseByte(string s)
        {
            if (byte.TryParse(s, out byte result))
                return result;
            return 0;
        }

        #endregion
    }
}
