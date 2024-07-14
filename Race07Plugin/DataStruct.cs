using System.Runtime.InteropServices;

namespace Race07Plugin
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct DataStruct
    {
        /// <summary>
        /// This structure allows for a number of parameters to be
        /// passed from the user to the exteral application via control input
        /// in the game. The ISIInputType enum describes which element of this
        /// array corresponds to which in-game control. Note that this data
        /// is public floating point, and can be configured in the game to be driven
        /// by an analog input device (joystick). The user may also map the
        /// control to a keyboard key, or a digital controller button. This
        /// means that the value may be anywhere between 0.0 and 1.0. Also note
        /// that these values will not be debounced; if the user
        /// maps the "External Signal Up" control to a key on the keyboard,
        /// the coresponding value in this array will remain 1.0 for as public int
        /// as the user holds the key down.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public float [] userInput; 
        /// <summary>
        /// // Engine speed, Radians Per Second.
        /// </summary>
        public float rpm;
        /// <summary>
        /// For use with an "analog" rpm display.
        /// </summary>
        public float maxEngineRPS;
        /// <summary>
        /// KPa
        /// </summary>
        public float fuelPressure;
        /// <summary>
        /// Current liters of fuel in the tank(s).
        /// </summary>
        public float fuel;
        /// <summary>
        /// Maximum capacity of fuel tank(s).
        /// </summary>
        public float fuelCapacityLiters; 
        public float engineWaterTemp; 
        public float engineOilTemp; 
        public float engineOilPressure;
        /// <summary>
        /// meters per second
        /// </summary>
        public float carSpeed;
        /// <summary>
        /// # of laps in race, or -1 if player is not in
        /// race mode (player is in practice or test mode).
        /// </summary>
        public int numberOfLaps; 
        /// <summary>
        /// How many laps the player has completed. If this
        /// value is 6, the player is on his 7th lap. -1 = n/a
        /// </summary>
        public int completedLaps; 
        /// <summary>
        ///  Seconds. -1.0 = none
        /// </summary>
        public float lapTimeBest; 
        /// <summary>
        /// Seconds. -1.0 = none
        /// </summary>
        public float lapTimePrevious; 
        /// <summary>
        /// Seconds. -1.0 = none
        /// </summary>
        public float lapTimeCurrent; 
        /// <summary>
        /// Current position. 1 = first place.
        /// </summary>
        public int position; 
        /// <summary>
        /// Number of cars (including the player) in the race.
        /// </summary>
        public int numCars; 
        /// <summary>
        /// -2 = no data available, -1 = reverse, 0 = neutral,
        /// 1 = first gear... (valid range -1 to 7).
        /// </summary>
        public int gear;
        /// <summary>
        /// tire values from [0]=left to [2]=right
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public float [] tirefrontleft;
        /// <summary>
        /// tire values from [0]=left to [2]=right
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public float [] tirefrontright;
        /// <summary>
        /// tire values from [0]=left to [2]=right
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public float [] tirerearleft; 
        /// <summary>
        /// tire values from [0]=left to [2]=right
        /// across the tread of each tire.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public float [] tirerearright; 
        /// <summary>
        /// Number of penalties pending for the player.
        /// </summary>
        public int numPenalties; // 
        /// <summary>
        /// Physical location of car's Center of Gravity in world space, X,Y,Z... Y=up.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public float [] carCGLoc; 
        /// <summary>
        /// Orientation, radians
        /// </summary>
        public float pitch;
        /// <summary>
        /// Orientation, radians
        /// </summary>
        public float yaw;
        /// <summary>
        /// Orientation, radians
        /// </summary>
        public float roll;
        /// <summary>
        /// Force left-right
        /// </summary>
        public float lateral; 
        /// <summary>
        /// //force up-down
        /// </summary>
        public float vertical; 
        /// <summary>
        /// //force faster, slower
        /// </summary>
        public float longintitudinal; 
    }
}