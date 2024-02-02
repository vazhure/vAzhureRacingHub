using Codemasters.Structs;
using System;
using System.IO;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using vAzhureRacingAPI;

namespace Codemasters
{
    internal class UDPClient : VAzhureUDPClient
    {
        public event EventHandler<TelemetryUpdatedEventArgs> OnDataArrived;

        readonly int structSize;
        readonly TelemetryDataSet dataset;

        readonly GamePlugin.CodemastersGame gameid;

        public UDPClient(GamePlugin game, GamePlugin.CodemastersGame id)
        {
            gameid = id;
            dataset = new TelemetryDataSet(game);
            structSize = Marshal.SizeOf(typeof(DIRT_4_DATA_MODE3));
        }

        private float _vY = 0;
        private float _acc = 0;
        private DateTime _tm = DateTime.Now;

        public override void OnDataReceived(ref byte[] bytes)
        {
            if (bytes?.Length >= structSize)
            {
                try
                {
                    TimeSpan ts = DateTime.Now - _tm;
                    _tm = DateTime.Now;

                    DIRT_4_DATA_MODE3 data = DIRT_4_DATA_MODE3.FromBytes(bytes);

                    AMCarData carData = dataset.CarData;
                    AMSessionInfo sessionInfo = dataset.SessionInfo;
                    AMWeatherData aMWeatherData = dataset.WeatherData;
                    AMMotionData aMMotionData = dataset.CarData.MotionData;

                    aMMotionData.LocalAcceleration = new float[] { 0, 0, 0 };

                    carData.Gear = data.gear > 9 ? (short)0 : (short)(data.gear + 1); // Rear gear == 10
                    carData.Lap = (int)data.lap;
                    carData.MaxRPM = (uint)(data.max_rpm * 10.0f);
                    carData.RPM = (uint)(data.engine_rate * 10.0f);
                    carData.Speed = data.speed * 3.6f; // meters per second -> km per hour
                    carData.Position = (int)data.race_position;
                    carData.FuelLevel = data.fuel_in_tank;
                    carData.FuelCapacity = data.fuel_capacity;
                    carData.Distance = data.lap_distance;

                    sessionInfo.Sector = (int)data.race_sector;
                    //sessionInfo.CurrentSector1Time = (int)(data.sector_time_1 * 1000.0f);
                    //sessionInfo.CurrentSector2Time = (int)(data.sector_time_2 * 1000.0f);

                    aMWeatherData.AmbientTemp = 21;
                    aMWeatherData.TrackTemp = 21;

                    sessionInfo.CurrentLapTime = (int)(data.lap_time * 1000.0f);
                    sessionInfo.TrackLength = data.total_distance;
                    sessionInfo.TotalLapsCount = (int)data.total_laps;
                    sessionInfo.RemainingTime = (int)data.total_time;

                    carData.InPits = false;
                    carData.PitState = 0;
                    switch (data.in_pits)
                    {
                        default:
                        case 0:
                            sessionInfo.SessionState = "";
                            break;
                        case 1:
                            sessionInfo.SessionState = "Pitting";
                            carData.PitState = 2;
                            break;
                        case 2:
                            carData.InPits = true;
                            carData.PitState = 3;
                            sessionInfo.SessionState = "In Pits";
                            break;
                        case 9.5f:
                            sessionInfo.SessionState = "Race";
                            break;
                        case 10f:
                            sessionInfo.SessionState = "Time attack";
                            break;
                        case 170f:
                            sessionInfo.SessionState = "Qualifying";
                            break;
                    }

                    carData.Brake = data.brake_input;
                    carData.Throttle = data.throttle_input;
                    carData.Clutch = data.clutch_input;
                    carData.Steering = data.steering_input * (float)Math.PI;

                    if (gameid == GamePlugin.CodemastersGame.WRCG)
                    {
                        float accZ = ts.TotalMilliseconds > 0 ? (data.speed > 10 ? 1000.0f * (data.velocity[2] - _vY) / (float)ts.TotalMilliseconds : 0) : _acc;
                        _vY = data.velocity[2];

                        aMMotionData.Pitch = data.forward_dir[2];
                        aMMotionData.Roll = data.left_dir[2];

                        aMMotionData.Heave = _acc = accZ;
                        aMMotionData.Sway = data.gforce_lateral;
                        aMMotionData.Surge = -data.gforce_longitudinal;
                    }
                    else
                    {
                        Vector3 f = Vector3.Normalize(new Vector3(data.forward_dir[0], data.forward_dir[1], data.forward_dir[2]));
                        Vector3 l = Vector3.Normalize(new Vector3(data.left_dir[0], data.left_dir[1], data.left_dir[2]));

                        float accZ = ts.TotalMilliseconds > 0 ? (data.speed > 10 ? 1000.0f * (data.velocity[1] - _vY) / (float)ts.TotalMilliseconds : 0) : _acc;
                        _vY = data.velocity[1];

                        aMMotionData.Pitch = (float)Math2.Clamp(Math.Asin(f.Y), -Math2.halfPI, Math2.halfPI);
                        aMMotionData.Roll = (float)Math2.Clamp(Math.Asin(-l.Y), -Math2.halfPI, Math2.halfPI);

                        aMMotionData.Heave = _acc = accZ;
                        aMMotionData.Sway = data.gforce_lateral;
                        aMMotionData.Surge = data.gforce_longitudinal;
                    }


                    carData.Tires = new AMTireData[]
                    {
                        new AMTireData()
                        {
                            Pressure = data.tyre_pressure[0],
                            Temperature = new double[]{ 60, 60, 60, 60},
                            Wear = 0,
                            BrakeTemperature = data.brake_temp[0],
                            Detached = false
                        },
                        new AMTireData()
                        {
                            Pressure = data.tyre_pressure[1],
                            Temperature = new double[]{ 60, 60, 60, 60},
                            Wear = 0,
                            BrakeTemperature = data.brake_temp[1],
                            Detached = false
                        },
                        new AMTireData()
                        {
                            Pressure = data.tyre_pressure[2],
                            Temperature = new double[]{ 60, 60, 60, 60},
                            Wear = 0,
                            BrakeTemperature = data.brake_temp[2],
                            Detached = false
                        },
                        new AMTireData()
                        {
                            Pressure = data.tyre_pressure[3],
                            Temperature = new double[]{ 60, 60, 60, 60},
                            Wear = 0,
                            BrakeTemperature = data.brake_temp[3],
                            Detached = false
                        },
                    };

                    OnDataArrived?.Invoke(this, new TelemetryUpdatedEventArgs(dataset));
                }
                catch { }
            }
        }
    }
}