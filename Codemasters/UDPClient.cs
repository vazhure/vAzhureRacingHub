using Codemasters.Structs;
using System;
using System.Runtime.InteropServices;
using vAzhureRacingAPI;

namespace Codemasters
{
    internal class UDPClient : VAzhureUDPClient
    {
        public event EventHandler<TelemetryUpdatedEventArgs> OnDataArrived;

        readonly int structSize;
        readonly TelemetryDataSet dataset;

        public UDPClient(GamePlugin game)
        {
            structSize = Marshal.SizeOf(typeof(DIRT_4_DATA_MODE3));
            dataset = new TelemetryDataSet(game);
        }

        public override void OnDataRecieved(ref byte[] bytes)
        {
            if (bytes?.Length == structSize)
            {
                try
                {
                    DIRT_4_DATA_MODE3 data = DIRT_4_DATA_MODE3.FromBytes(bytes);
                    AMCarData carData = dataset.CarData;
                    AMSessionInfo sessionInfo = dataset.SessionInfo;
                    AMWeatherData aMWeatherData = dataset.WeatherData;
                    AMMotionData aMMotionData = dataset.CarData.MotionData;

                    aMMotionData.LocalAcceleration = new float[] { data.gforce_lateral, data.gforce_longitudinal, data.gforce_longitudinal };

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