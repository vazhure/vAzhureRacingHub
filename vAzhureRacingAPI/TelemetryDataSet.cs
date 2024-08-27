using System;
using System.Collections.Generic;

namespace vAzhureRacingAPI
{
    public class TelemetryUpdatedEventArgs : EventArgs
    {
        public TelemetryUpdatedEventArgs(TelemetryDataSet dataset)
        {
            DataSet = dataset;
        }

        private readonly TelemetryDataSet DataSet;
        /// <summary>
        /// Gets the telemetry info object.
        /// </summary>
        public TelemetryDataSet TelemetryInfo { get { return DataSet; } }
    }

    public class TelemetryDataSet : ICloneable
    {
        public TelemetryDataSet(IGamePlugin game) => GamePlugin = game;

        public IGamePlugin GamePlugin { get; private set; }

        public TelemetryState TelemetryState { get; set; } = TelemetryState.Unknown;

        private AMSessionInfo sessionInfo;
        public AMSessionInfo SessionInfo
        {
            get
            {
                if (sessionInfo == null)
                    sessionInfo = new AMSessionInfo();
                return sessionInfo;
            }
            set => sessionInfo = value;
        }
        private AMCarData carData;
        public AMCarData CarData
        {
            get
            {
                if (carData == null)
                    carData = new AMCarData();
                return carData;
            }
            set => carData = value;
        }

        private AMWeatherData weatherData;
        public AMWeatherData WeatherData
        {
            get
            {
                if (weatherData == null)
                    weatherData = new AMWeatherData();
                return weatherData;
            }
            set => weatherData = value;
        }

        private Dictionary<string, object> userData;
        public Dictionary<string, object> UserData
        {
            get
            {
                if (userData == null)
                    userData = new Dictionary<string, object>();

                return userData;
            }
            set => userData = value;
        }

        public void LoadDefaults()
        {
            sessionInfo = new AMSessionInfo();
            carData = new AMCarData();
            weatherData = new AMWeatherData();
            userData = new Dictionary<string, object>();
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }

    public enum TelemetryState { Unknown, Playing, Paused, Replay, Menu };

    [Flags]
    public enum DirectionsLight { None = 0, Left = 1, Right = 1 << 1, Booth = Left | Right };

    [Flags]
    public enum TelemetryFlags : short
    {
        FlagNone = 0,
        FlagBlue = 1,
        FlagYellow = 1 << 1,
        FlagBlack = 1 << 2,
        FlagWhite = 1 << 3,
        FlagChequered = 1 << 4,
        FlagPenalty = 1 << 5,
        FlagGreen = 1 << 6,
        FlagSlowDown = 1 << 7,
        FlagStopAndGo = 1 << 8,
        FlagDriveThrough = 1 << 9,
        FlagPitStop = 1 << 10,
        FlagTimeDeduction = 1 << 11
    };

    [Flags]
    public enum CarElectronics
    {
        None = 0,
        /// <summary>
        /// ABS active
        /// </summary>
        ABS = 1,
        /// <summary>
        /// Traction Control active
        /// </summary>
        TCS = 1 << 1,
        /// <summary>
        /// Headlights on
        /// </summary>
        Headlight = 1 << 2,
        /// <summary>
        /// Wipers on
        /// </summary>
        WipersOn = 1 << 3,
        /// <summary>
        /// Pit Limiter on
        /// </summary>
        Limiter = 1 << 4,
        /// <summary>
        /// Ignition on
        /// </summary>
        Ignition = 1 << 5,
        /// <summary>
        /// Kers active
        /// </summary>
        KERS = 1 << 6,
        /// <summary>
        /// DRS active
        /// </summary>
        DRS = 1 << 7,
        /// <summary>
        /// Handbrake active
        /// </summary>
        Handbrake = 1 << 8,
        /// <summary>
        /// DRS Enabled
        /// </summary>
        DRS_EN = 1 << 9,
    }

    // [front left, front right, rear left, rear right]

    public class AMTireData : ICloneable
    {
        /// <summary>
        /// Pressure, kPa
        /// </summary>
        public double Pressure { get; set; } = 0;
        /// <summary>
        /// Tire Wear, 0 - New, 1 - Old
        /// </summary>
        public double Wear { get; set; } = 0;
        /// <summary>
        /// Detached Wheel State
        /// </summary>
        public bool Detached { get; set; } = false;
        /// <summary>
        /// Tire temperature, Celsius [left carcass, center, right carcass]
        /// </summary>
        public double[] Temperature { get; set; } = { 0, 0, 0 };
        /// <summary>
        /// Тип резины / состав или название
        /// </summary>
        public string Compound { get; set; } = "";
        /// <summary>
        /// Brake temperature, Celsius
        /// </summary>
        public double BrakeTemperature { get; set; } = 0;
        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }

    public class AMWeatherData : ICloneable
    {
        public bool Valid { get; set; } = false;
        /// <summary>
        /// Ambient Temperature, Celsius
        /// </summary>
        public double AmbientTemp { get; set; } = 20;
        /// <summary>
        /// Track Temperature, Celsius
        /// </summary>
        public double TrackTemp { get; set; } = 20;
        /// <summary>
        /// Wind Speed vector, meters per second 
        /// </summary>
        public double[] WindSpeed { get; set; } = { 1, 0, 0 };
        /// <summary>
        /// raining severity [0.0 .. 1.0]
        /// </summary>
        public double Raining { get; set; } = 0;

        public object Clone()
        {
            return MemberwiseClone();
        }
    }

    public class AMCarData : ICloneable
    {
        public bool Valid { get; set; } = false;
        /// <summary>
        /// Car Driver Name
        /// </summary>
        public string DriverName { get; set; } = "";
        /// <summary>
        /// Car Number
        /// </summary>
        public string CarNumber { get; set; } = "";
        /// <summary>
        /// Car Model
        /// </summary>
        public string CarName { get; set; } = "";
        /// <summary>
        /// Car Class
        /// </summary>
        public string CarClass { get; set; } = "";

        /// <summary>
        /// Car Manufacturer / Brand
        /// </summary>
        public string Manufacturer { get; set; } = "";
        /// <summary>
        /// Current Gear
        /// </summary>
        public short Gear { get; set; } = 0;
        /// <summary>
        /// Speed, km per hour
        /// </summary>
        public double Speed { get; set; } = 0;
        /// <summary>
        /// RPM
        /// </summary>
        public double RPM { get; set; } = 0;
        /// <summary>
        /// Max RPM
        /// </summary>
        public double MaxRPM { get; set; } = 0;
        /// <summary>
        /// Shift-up RPM
        /// </summary>
        public double ShiftUpRPM { get; set; } = 0;
        /// <summary>
        /// Oil Temperature, Celsius
        /// </summary>
        public double OilTemp { get; set; } = 0;
        /// <summary>
        /// Oil Pressure, PSI
        /// </summary>
        public double OilPressure { get; set; } = 0;
        /// <summary>
        /// Water Temperature, Celsius
        /// </summary>
        public double WaterTemp { get; set; } = 0;
        /// <summary>
        /// Water Pressure, PSI
        /// </summary>
        public double WaterPressure { get; set; } = 0;
        /// <summary>
        /// Steering wheel rotation angle
        /// </summary>
        public double Steering { get; set; } = 0;
        /// <summary>
        /// Throttle pedal position, [0..1], -1 - undefined
        /// </summary>
        public double Throttle { get; set; } = 0;
        /// <summary>
        /// Clutch pedal position, [0..1], -1 - undefined
        /// </summary>
        public double Clutch { get; set; } = 0;
        /// <summary>
        /// Brake pedal position, [0..1], -1 - undefined
        /// </summary>
        public double Brake { get; set; } = 0;
        /// <summary>
        /// Front Brake Bias, [0..100]
        /// </summary>
        public float BrakeBias { get; set; } = 0;
        /// <summary>
        /// Traction Control Level, 0 - off
        /// </summary>
        public short TcLevel { get; set; } = -1;
        /// <summary>
        /// Traction Control Level 2, 0 - off
        /// </summary>
        public short TcLevel2 { get; set; } = -1;
        /// <summary>
        /// Engine Map
        /// </summary>
        public short EngineMap { get; set; } = -1;
        /// <summary>
        /// ABS Level, 0 - off
        /// </summary>
        public short AbsLevel { get; set; } = -1;
        /// <summary>
        /// Ignition / Starter state : 0 - off, 1 - Ignition, 2 - Ignition + Starter
        /// </summary>
        public short IgnitionStarter { get; set; } = 0;
        /// <summary>
        /// Fuel Level, liters
        /// </summary>
        public double FuelLevel { get; set; } = 0;
        /// <summary>
        /// Fuel Tank Capacity, liters
        /// -1 - не задано
        /// </summary>
        public double FuelCapacity { get; set; } = -1;
        /// <summary>
        /// Estimated Fuel consumption per lap, liters. 
        /// -1 - не задано
        /// </summary>
        public double FuelConsumptionPerLap { get; set; } = -1;
        /// <summary>
        /// Estimated fuel laps.
        /// -1 - не задано
        /// </summary>
        public double FuelEstimatedLaps { get; set; } = -1;
        /// <summary>
        /// Car under AI control
        /// </summary>
        public bool AIControlled { get; set; } = false;

        public TelemetryFlags Flags { get; set; } = TelemetryFlags.FlagNone;

        public CarElectronics Electronics { get; set; } = CarElectronics.None;

        /// <summary>
        /// FL, FR, RL, RR
        /// </summary>
        public AMTireData[] Tires { get; set; } = { new AMTireData(), new AMTireData(), new AMTireData(), new AMTireData() };
        /// <summary>
        /// Lap distance in meters
        /// </summary>
        public double Distance { get; set; } = 0;
        /// <summary>
        /// Distance to pit place, -1 - not set
        /// </summary>
        public double PitDistance { get; set; } = -1;
        public bool InPits { get; set; } = false;
        /// <summary>
        /// Pit state: 0=none, 1=request, 2=entering, 3=stopped, 4=exiting
        /// </summary>
        public short PitState { get; set; } = 0;

        /// <summary>
        /// Поворотники
        /// </summary>
        public DirectionsLight DirectionsLight { get; set; } = DirectionsLight.None;

        private AMMotionData motionData;
        public AMMotionData MotionData
        {
            get
            {
                if (motionData == null)
                    motionData = new AMMotionData();
                return motionData;
            }
            set => motionData = value;
        }

        public int Position { get; set; }
        public int Lap { get; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }

    [Serializable]
    public class AMMotionData : ICloneable
    {
        public double[] Position { get; set; } = { 0, 0, 0 };
        /// <summary>
        /// forward and back:
        /// -1 = -180°, +1 = +180°
        /// </summary>
        public float Pitch { get; set; } = 0;
        /// <summary>
        /// Left and right:
        /// -1 = -180°, +1 = +180°
        /// </summary>
        public float Roll { get; set; } = 0;
        /// <summary>
        /// Rotation:
        /// -1 = -180°, +1 = +180°
        /// </summary>
        public float Yaw { get; set; } = 0;
        /// <summary>
        /// Longitudinal acceleration // Surge
        /// </summary>
        public float Surge
        {
            get => LocalAcceleration[2];
            set => LocalAcceleration[2] = value;
        }
        /// <summary>
        /// Up / Down movements
        /// </summary>
        public float Heave
        {
            get => LocalAcceleration[1];
            set => LocalAcceleration[1] = value;
        }
        /// <summary>
        /// Lateral acceleration / Sway
        /// </summary>
        public float Sway
        {
            get => LocalAcceleration[0];
            set => LocalAcceleration[0] = value;
        }
        /// <summary>
        /// (meters/sec^2)
        /// </summary>
        public float[] LocalAcceleration { get; set; } = { 0, 0, 0 };
        /// <summary>
        /// (radians/sec^2)
        /// </summary>
        public float[] LocalRotAcceleration { get; set; } = { 0, 0, 0 };
        /// <summary>
        /// (radians/sec)
        /// </summary>
        public float[] LocalRot { get; set; } = { 0, 0, 0 };
        /// <summary>
        /// (meters/sec)
        /// </summary>
        public float[] LocalVelocity { get; set; } = { 0, 0, 0 };

        /// <summary>
        /// Amount of ABS vibration
        /// </summary>
        public float ABSVibration { get; set; } = 0;

        public object Clone()
        {
            return MemberwiseClone();
        }
    }

    public class AMSessionInfo : ICloneable
    {
        public bool Valid { get; set; } = false;
        public string SessionState { get; set; } = "";
        public string TrackName { get; set; } = "";
        public double TrackLength { get; set; } = 0;
        public string TrackConfig { get; set; } = "";
        public string FinishStatus { get; set; } = "";
        public int DriversCount { get; set; }
        public int TotalLapsCount { get; set; }
        /// <summary>
        /// time, ms
        /// </summary>
        public int RemainingTime { get; set; }
        /// <summary>
        /// time, ms
        /// </summary>
        public int CurrentSector1Time { get; set; }
        /// <summary>
        /// time, ms
        /// </summary>
        public int CurrentSector2Time { get; set; }
        /// <summary>
        /// time, ms
        /// </summary>
        public int CurrentSector3Time { get; set; }
        /// <summary>
        /// time, ms
        /// </summary>
        public int CurrentLapTime { get; set; }
        /// <summary>
        /// time, ms
        /// </summary>
        public int LastSector1Time { get; set; }
        /// <summary>
        /// time, ms
        /// </summary>
        public int LastSector2Time { get; set; }
        /// <summary>
        /// time, ms
        /// </summary>
        public int LastSector3Time { get; set; }
        /// <summary>
        /// time, ms
        /// </summary>
        public int LastLapTime { get; set; }
        /// <summary>
        /// time, ms
        /// </summary>
        public int Sector1BestTime { get; set; }
        /// <summary>
        /// time, ms
        /// </summary>
        public int Sector2BestTime { get; set; }
        /// <summary>
        /// time, ms
        /// </summary>
        public int Sector3BestTime { get; set; }
        /// <summary>
        /// time, ms
        /// </summary>
        public int BestLapTime { get; set; }
        /// <summary>
        /// time, ms
        /// </summary>
        public int CurrentDelta { get; set; }
        public int CurrentLapNumber { get; set; }
        public int CurrentPosition { get; set; }
        /// <summary>
        /// Km/h
        /// </summary>
        public double PitSpeedLimit { get; set; }
        /// <summary>
        /// Current Flag
        /// </summary>
        public string Flag { get; set; } = "";
        /// <summary>
        /// Current Sector
        /// </summary>
        public int Sector { get; set; }
        public int RemainingLaps { get; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}