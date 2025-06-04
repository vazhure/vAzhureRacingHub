using System;
using System.Runtime.InteropServices;

namespace Kunos.Structs
{
    public struct ACCVec3
    {
        public float x;
        public float y;
        public float z;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Unicode)]
    public struct EVOGraphics
    {
        public int PacketId;
        public AC_STATUS Status;
        public AC_SESSION_TYPE Session;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 15)]
        public string CurrentTime;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 15)]
        public string LastTime;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 15)]
        public string BestTime;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 15)]
        public string Split;
        public int CompletedLaps;
        public int Position;
        public int iCurrentTime;
        public int iLastTime;
        public int iBestTime;
        public float SessionTimeLeft;
        public float DistanceTraveled;
        public int IsInPit;
        public int CurrentSectorIndex;
        public int LastSectorTime;
        public int NumberOfLaps;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 33)]
        public string TyreCompound;
        public float ReplayTimeMultiplier;
        public float NormalizedCarPosition;
        public int CarCount;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 60)]
        public ACCVec3[] CarCoordinates;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 60)]
        public int[] CarIDs;
        public int PlayerCarID;
        public float PenaltyTime;
        public AC_FLAG_TYPE Flag;
        public AC_PENALTY Penalty;
        public int IdealLineOn;
        public int IsInPitLane;
        public float SurfaceGrip;
        public int MandatoryPitDone;
        public float WindSpeed;
        public float WindDirection;
        public int IsSetupMenuVisible;
        public int MainDisplayIndex;
        public int SecondaryDisplayIndex;
        public int TC;
        public int TCCut;
        public int EngineMap;
        public int ABS;
        public float FuelXLap;
        public int RainLights;
        public int FlashingLights;
        public int LightsStage;
        public float ExhaustTemperature;
        public int WiperLV;
        public int DriverStintTotalTimeLeft;
        public int DriverStintTimeLeft;
        public int RainTyres;
        public int SessionIndex;
        public float UsedFuel;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 15)]
        public string deltaLapTime;
        public int iDeltaLapTime;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 15)]
        public string EstimatedLapTime;
        public int iEstimatedLapTime;
        public int isDeltaPositive;
        public int iSplit;
        public int isValidLap;
        public float fuelEstimatedLaps;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 33)]
        public string trackStatus;
        public int missingMandatoryPits;
        public float clock;
        public int directionLightsLeft;
        public int directionLightsRight;
        public int globalYellow;
        public int globalYellow1;
        public int globalYellow2;
        public int globalYellow3;
        public int globalWhite;
        public int globalGreen;
        public int globalChequered;
        public int globalRed;
        public int mfdTyreSet;
        public float mfdFuelToAdd;
        public float mfdTyrePressureLF;
        public float mfdTyrePressureRF;
        public float mfdTyrePressureLR;
        public float mfdTyrePressureRR;
        public TRACK_GRIP_STATUS trackGripStatus;
        public RAIN_INTENSITY rainIntensity;
        public RAIN_INTENSITY rainIntensityIn10min;
        public RAIN_INTENSITY rainIntensityIn30min;
        public int currentTyreSet;
        public int strategyTyreSet;
        public int gapAhead;
        public int gapBehind;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Unicode)]
    public struct EVOPhysics
    {
        public int PacketId;

        /// <summary>
        /// Throttle input [0..1]
        /// </summary>
        public float Gas;

        /// <summary>
        /// Brake input [0..1]
        /// </summary>
        public float Brake;

        public float Fuel;

        public int Gear;

        public int Rpms;

        public float SteerAngle;

        public float SpeedKmh;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public float[] Velocity;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public float[] AccG;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] WheelSlip;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] WheelLoad;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] WheelsPressure;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] WheelAngularSpeed;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] TyreWear;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] TyreDirtyLevel;
        
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] TyreCoreTemperature;
        
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] CamberRad;
        
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] SuspensionTravel;

        public float Drs;

        public float TC;

        /// <summary>
        /// Yaw, radians
        /// </summary>
        public float Heading;

        /// <summary>
        /// Pitch, radians
        /// </summary>
        public float Pitch;

        /// <summary>
        /// Roll, radians
        /// </summary>
        public float Roll;

        public float CgHeight;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        public float[] CarDamage;

        public int NumberOfTyresOut;

        public int PitLimiterOn;

        public float Abs;

        public float KersCharge;

        public float KersInput;

        public int AutoShifterOn;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public float[] RideHeight;

        public float Turbo;

        public float Ballast;

        public float AirDensity;

        public float AirTemp;

        public float RoadTemp;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public float[] LocalAngularVelocity;

        public float FinalFF;

        public float PerformanceMeter;

        public int EngineBrake;

        public int ErsRecoveryLevel;

        public int ErsPowerLevel;

        public int ErsHeatCharging;

        public int ErsisCharging;

        public float KersCurrentKJ;

        public int DrsAvailable;

        public int DrsEnabled;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] BrakeTemp;

        /// <summary>
        /// Clutch input [0..1]
        /// </summary>
        public float Clutch;
        
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] TyreTempI;
        
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] TyreTempM;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)] 
        public float[] TyreTempO;

        public int IsAIControlled;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
        public Coordinates[] TyreContactPoint;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
        public Coordinates[] TyreContactNormal;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
        public Coordinates[] TyreContactHeading;

        public float BrakeBias;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public float[] LocalVelocity;

        public int P2PActivation;
               
        public int P2PStatus;

        public float currentMaxRpm;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] mz;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] fx;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] fy;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] slipRatio;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] slipAngle;

        public int tcinAction;

        public int absinAction;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] suspensionDamage;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] tyreTemp;

        public float waterTemperature;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)] 
        public float[] brakePressure;

        public int frontBrakeCompound;
               
        public int rearBrakeCompound;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)] 
        public float[] padLife;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)] 
        public float[] discLife;

        public int ignitionOn;

        public int starterEngineOn;

        public int isEngineRunning;

        public float kerbVibration;

        public float slipVibrations;

        public float gVibrations;

        public float absVibrations;
    }
}
