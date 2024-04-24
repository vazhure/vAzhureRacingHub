/// <summary>
/// https://documentation.beamng.com/modding/protocols/
/// </summary>

namespace BeamNG
{
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Ansi)]
    public class StructBeamNG
    {
        /// <summary>
        /// header "BNG1"
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public char[] header;

        /// <summary>
        /// World position of the car
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public float[] position;

        /// <summary>
        /// Velocity of the car
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public float[] velocity;

        /// <summary>
        /// Acceleration of the car, gravity not included
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)] 
        public float[] acceleration;

        /// <summary>
        /// Vector components of a vector pointing "up" relative to the car
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public float[] upVector;

        //Roll, pitch and yaw positions of the car
        public float rollPos;
        public float pitchPos;
        public float yawPos;

        // Roll, pitch and yaw "velocities" of the car
        public float rollRate;
        public float pitchRate;
        public float yawRate;

        //Roll, pitch and yaw "accelerations" of the car
        public float rollAcc;
        public float pitchAcc;
        public float yawAcc;

        /// <summary>
        /// Heave
        /// </summary>
        public float Heave
        {
            get => acceleration[2];
        }

        /// <summary>
        /// Sway
        /// </summary>
        public float Sway
        {
            get => acceleration[0];
        }

        /// <summary>
        /// Surge
        /// </summary>
        public float Surge
        {
            get => acceleration[1];
        }
    }
}
