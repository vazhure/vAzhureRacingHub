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

        readonly GamePlugin.CodemastersGame gameid;

        public UDPClient(GamePlugin game, GamePlugin.CodemastersGame id)
        {
            gameid = id;
            dataset = new TelemetryDataSet(game);
            structSize = id == GamePlugin.CodemastersGame.EAWRC ? Marshal.SizeOf(typeof(WRC_DATA)) : Marshal.SizeOf(typeof(DIRT_4_DATA_MODE3));
            OnTimeout += UDPClient_OnTimeout;
        }

        private void UDPClient_OnTimeout(object sender, EventArgs e)
        {
            dataset?.LoadDefaults();
            OnDataArrived?.Invoke(this, new TelemetryUpdatedEventArgs(dataset));
        }

        private float _vY = 0;
        private float _acc = 0;
        private DateTime _tm = DateTime.Now;
        //readonly StringBuilder sb = new StringBuilder();

        public void Finish()
        {
            ////if (gameid == GamePlugin.CodemastersGame.EAWRC)
            //{
            //    if (sb.Length > 0)
            //    {
            //        string file_name = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "My Games", "WRC", "telemetry.csv");
            //        File.WriteAllText(file_name, sb.ToString());
            //    }
            //}
        }

        float _last_laptime = -1;

        public override void OnDataReceived(ref byte[] bytes)
        {
            if (gameid == GamePlugin.CodemastersGame.EAWRC)
            {
                if (bytes?.Length == structSize)
                {
                    WRC_DATA data = Marshalizable<WRC_DATA>.FromBytes(bytes);

                    AMCarData carData = dataset.CarData;
                    AMSessionInfo sessionInfo = dataset.SessionInfo;
                    AMWeatherData aMWeatherData = dataset.WeatherData;
                    AMMotionData aMMotionData = dataset.CarData.MotionData;

                    carData.Gear = (short)(data.vehicle_gear_index == data.vehicle_gear_index_reverse ? 0 : (data.vehicle_gear_index + 1));
                    carData.MaxRPM = data.vehicle_engine_rpm_max;
                    carData.RPM = (uint)(data.vehicle_engine_rpm_current);
                    carData.Speed = data.vehicle_speed * 3.6f; // meters per second -> km per hour
                    carData.Distance = data.stage_current_distance;
                    carData.FuelLevel = 45;
                    carData.FuelCapacity = 60;

                    sessionInfo.CurrentLapTime = (int)(data.stage_current_time * 1000.0f);
                    sessionInfo.CurrentDelta = (int)(data.game_delta_time * 1000.0f);
                    sessionInfo.TrackLength = data.stage_length;
                    sessionInfo.SessionState = "Race";
                    sessionInfo.Flag = "Green";
                    sessionInfo.TotalLapsCount = 1;

                    aMMotionData.Heave = data.vehicle_acceleration_y / 20f;

                    double x = data.vehicle_acceleration_x * data.vehicle_roll_z - data.vehicle_acceleration_z * data.vehicle_roll_x;
                    double y = data.vehicle_acceleration_x * data.vehicle_roll_x + data.vehicle_acceleration_z * data.vehicle_roll_z;

                    //Vector3 fwd = new Vector3(data.vehicle_pitch_x, data.vehicle_pitch_y, data.vehicle_pitch_z);
                    //Vector3 rht = new Vector3(data.vehicle_roll_x, data.vehicle_roll_y, data.vehicle_roll_z);
                    //Vector3 up = Vector3.Cross(fwd, rht);
                    //Vector3 pos = new Vector3(data.vehicle_position_x, data.vehicle_position_y, data.vehicle_position_z);

                    //Matrix4x4 transform = new Matrix4x4(rht.X, rht.Y, rht.Z, 0.0f,
                    //                        up.X, up.Y, up.Z, 0.0f,
                    //                        fwd.X, fwd.Y, fwd.Z, 0.0f,
                    //                        pos.X, pos.Y, pos.Z, 1.0f);

                    //// TODO:
                    //Vector3 acc = Vector3.Transform(new Vector3(data.vehicle_acceleration_x, data.vehicle_acceleration_y, data.vehicle_acceleration_z), transform);

                    aMMotionData.Sway = (float)(y / 20.0);
                    aMMotionData.Surge = -(float)(x / 20.0);

                    aMMotionData.Pitch = data.vehicle_pitch_y;
                    aMMotionData.Roll = data.vehicle_roll_y;

                    carData.Throttle = data.vehicle_throttle;
                    carData.Brake = data.vehicle_brake;
                    carData.Clutch = data.vehicle_clutch;

                    aMWeatherData.AmbientTemp = 21;
                    aMWeatherData.TrackTemp = 21;

                    //sb.AppendLine($"{data.vehicle_acceleration_x:0.000};{data.vehicle_acceleration_y:0.000};{data.vehicle_acceleration_z:0.000};" +
                    //    $"{data.vehicle_pitch_x:0.000};{data.vehicle_pitch_y:0.000};{data.vehicle_pitch_z:0.000};" +
                    //    $"{data.vehicle_roll_x:0.000};{data.vehicle_roll_y:0.000};{data.vehicle_roll_z:0.000};" +
                    //    $"{data.vehicle_yaw_x:0.000};{data.vehicle_yaw_y:0.000};{data.vehicle_yaw_z:0.000}");

                    OnDataArrived?.Invoke(this, new TelemetryUpdatedEventArgs(dataset));
                }
                else
                {
                    dataset.LoadDefaults();

                    OnDataArrived?.Invoke(this, new TelemetryUpdatedEventArgs(dataset));
                }

                return;
            }

            if (bytes?.Length >= structSize)
            {
                try
                {
                    TimeSpan ts = DateTime.Now - _tm;
                    _tm = DateTime.Now;

                    DIRT_4_DATA_MODE3 data = Marshalizable<DIRT_4_DATA_MODE3>.FromBytes(bytes);

                    AMCarData carData = dataset.CarData;
                    AMSessionInfo sessionInfo = dataset.SessionInfo;
                    AMWeatherData aMWeatherData = dataset.WeatherData;
                    AMMotionData aMMotionData = dataset.CarData.MotionData;

                    aMMotionData.LocalAcceleration = new float[] { 0, 0, 0 };

                    carData.Gear = data.gear > 9 ? (short)0 : (short)(data.gear + 1); // Rear gear == 10
                    carData.Lap = (int)data.lap + 1;
                    carData.MaxRPM = (uint)(data.max_rpm * 10.0f);
                    carData.RPM = (uint)(data.engine_rate * 10.0f);
                    carData.Speed = data.speed * 3.6f; // meters per second -> km per hour
                    carData.Position = (int)data.race_position;
                    carData.FuelLevel = data.fuel_in_tank;
                    carData.FuelCapacity = data.fuel_capacity;
                    carData.Distance = data.lap_distance;
                    carData.Lap = (int)data.lap;
                    carData.Position = (int)data.race_position;
                    sessionInfo.Sector = (int)data.race_sector;

                    sessionInfo.Flag = "Green";
                    carData.Flags = data.lap_time > 0 ? TelemetryFlags.FlagGreen : TelemetryFlags.FlagNone;
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
                            sessionInfo.SessionState = "Race";
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

                    //sb.AppendLine($"{data.gforce_lateral:0.000};{data.gforce_longitudinal:0.000};" +
                    //    $"{data.pitch[0]:0.000};{data.pitch[1]:0.000};{data.pitch[2]:0.000};" +
                    //    $"{data.roll[0]:0.000};{data.roll[1]:0.000};{data.roll[2]:0.000};");

                    float accZ = 0;

                    if (gameid == GamePlugin.CodemastersGame.WRCG)
                    {
                        //Vector3 fwd = -new Vector3(data.pitch[0], data.pitch[1], data.pitch[2]);
                        //Vector3 rht = -new Vector3(data.roll[0], data.roll[1], data.roll[2]);
                        //Vector3 up = Vector3.Cross(rht, fwd);
                        //Vector3 pos = new Vector3(data.position[0], data.position[1], data.position[2]);

                        //Matrix4x4 transform = new Matrix4x4(rht.X, rht.Z, rht.Y, 0.0f,
                        //                        up.X, up.Z, up.Y, 0.0f,
                        //                        fwd.X, fwd.Z, fwd.Y, 0.0f,
                        //                        pos.X, pos.Z, pos.Y, 1.0f);

                        accZ = ts.TotalMilliseconds > 0 ? (data.speed > 10 ? 1000.0f * (data.velocity[2] - _vY) / (float)ts.TotalMilliseconds : 0) : _acc;
                        _vY = data.velocity[2];

                        aMMotionData.Pitch = -data.pitch[2];
                        aMMotionData.Roll = data.roll[2];
                    }
                    else
                    {
                        accZ = ts.TotalMilliseconds > 0 ? (data.speed > 10 ? 1000.0f * (data.velocity[1] - _vY) / (float)ts.TotalMilliseconds : 0) : _acc;
                        _vY = data.velocity[1];

                        aMMotionData.Pitch = data.pitch[1];
                        aMMotionData.Roll = -data.roll[1];
                    }

                    _acc = accZ;

                    switch (gameid)
                    {
                        default:
                            {
                                aMMotionData.Heave = accZ / 98.1f;
                                aMMotionData.Sway = data.gforce_lateral;
                                aMMotionData.Surge = data.gforce_longitudinal;
                            }
                            break;
                        case GamePlugin.CodemastersGame.DIRT4:
                        case GamePlugin.CodemastersGame.DIRTRALLY20:
                        case GamePlugin.CodemastersGame.GRID2019:
                            {
                                aMMotionData.Heave = accZ / 98.1f;
                                aMMotionData.Sway = data.gforce_lateral / (float)Math.PI;
                                aMMotionData.Surge = data.gforce_longitudinal / (float)Math.PI;
                            }
                            break;
                        case GamePlugin.CodemastersGame.DIRTRALLY:
                            {
                                aMMotionData.Heave = accZ / 98.1f;
                                aMMotionData.Sway = 0; // non correct data in gforce_lateral and gforce_longitudinal
                                aMMotionData.Surge = 0;
                            }
                            break;
                        case GamePlugin.CodemastersGame.WRCG:
                            {
                                aMMotionData.Heave = accZ / (9.81f * (float)Math.PI);
                                aMMotionData.Sway = -data.gforce_lateral / (9.81f * (float)Math.PI);
                                aMMotionData.Surge = -data.gforce_longitudinal / (9.81f * (float)Math.PI);
                                if (_last_laptime != data.lap_time)
                                {
                                    _last_laptime = data.lap_time;
                                }
                                else if ((data.engine_rate + 100) <= data.idle_rpm)
                                    dataset.CarData.MotionData = new AMMotionData(); // Replay ?
                            }
                            break;
                    }

                    carData.Tires = new AMTireData[]
                    {
                        new AMTireData()
                        {
                            Pressure = data.tyre_pressure[0] * 6.89476,
                            Temperature = new double[]{ 60, 60, 60, 60},
                            Wear = 0,
                            BrakeTemperature = data.brake_temp[0],
                            Detached = false
                        },
                        new AMTireData()
                        {
                            Pressure = data.tyre_pressure[1] * 6.89476,
                            Temperature = new double[]{ 60, 60, 60, 60},
                            Wear = 0,
                            BrakeTemperature = data.brake_temp[1],
                            Detached = false
                        },
                        new AMTireData()
                        {
                            Pressure = data.tyre_pressure[2] * 6.89476,
                            Temperature = new double[]{ 60, 60, 60, 60},
                            Wear = 0,
                            BrakeTemperature = data.brake_temp[2],
                            Detached = false
                        },
                        new AMTireData()
                        {
                            Pressure = data.tyre_pressure[3] * 6.89476,
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