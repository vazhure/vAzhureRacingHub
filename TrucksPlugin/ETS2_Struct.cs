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
        [MarshalAs(UnmanagedType.U1)] public bool lblinker;
        [MarshalAs(UnmanagedType.U1)] public bool rblinker;
        [MarshalAs(UnmanagedType.U1)] public bool lowBeamLight;
        [MarshalAs(UnmanagedType.U1)] public bool hiBeamLight;
        [MarshalAs(UnmanagedType.U1)] public bool parkingLights;

        public float speedometer_speed;
        public float rpm;
        public float rpmMax;
        public float fuel;
        public float fuelCapacity;
        public float steering;
        public float throttle;
        public float brake;
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
        public uint gameTime;
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
    }
}