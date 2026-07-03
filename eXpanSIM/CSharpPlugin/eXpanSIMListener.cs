using System;
using System.IO.MemoryMappedFiles;
using System.Threading;
using vAzhureRacingAPI;

namespace eXpanSIM
{
    /// <summary>
    /// Reads telemetry from eXpanSIM shared memory and converts to vAzhureRacingHub format.
    /// </summary>
    internal class eXpanSIMListener : VAzhureSharedMemoryClient
    {
        readonly byte[] data = new byte[eXpanSIMTelemetryData.Size];
        readonly TelemetryDataSet dataSet;

        public eXpanSIMListener(eXpanSIMGame game)
        {
            dataSet = new TelemetryDataSet(game);
        }

        public override void UserFunc()
        {
            try
            {
                using (var memoryFile = MemoryMappedFile.OpenExisting("$eXpanSIM_Telemetry$", MemoryMappedFileRights.Read))
                using (var viewStream = memoryFile.CreateViewStream(0L, data.Length, MemoryMappedFileAccess.Read))
                {
                    viewStream.ReadAsync(data, 0, data.Length).Wait();

                    var telemetry = Marshalizable<eXpanSIMTelemetryData>.FromBytes(data);

                    AMCarData carData = dataSet.CarData;
                    AMMotionData motionData = carData.MotionData;
                    AMSessionInfo sessionInfo = dataSet.SessionInfo;

                    if (telemetry.Valid == 1 && telemetry.VehicleSpawned == 1)
                    {
                        // Orientation (radians -> normalized [-1, 1])
                        motionData.Roll = -(float)(telemetry.Roll / Math.PI);
                        motionData.Pitch = -(float)(telemetry.Pitch / Math.PI);
                        motionData.Yaw = (float)(telemetry.Yaw / Math.PI);

                        // Accelerations (m/s^2 -> g-forces for motion platform)
                        const double G = 98.1;
                        motionData.Surge = -(float)(telemetry.AccZ / G);
                        motionData.Sway = -(float)(telemetry.AccX / G);
                        motionData.Heave = (float)(telemetry.AccY / G);

                        // Angular velocities
                        motionData.LocalRot =
                        [
                            (float)telemetry.AngVelX,
                            (float)telemetry.AngVelY,
                            (float)telemetry.AngVelZ
                        ];

                        // Angular accelerations
                        motionData.LocalRotAcceleration =
                        [
                            (float)telemetry.AngAccX,
                            (float)telemetry.AngAccY,
                            (float)telemetry.AngAccZ
                        ];

                        // Position
                        motionData.Position =
                        [
                            telemetry.PosX,
                            telemetry.PosY,
                            telemetry.PosZ
                        ];

                        // Velocity
                        motionData.LocalVelocity =
                        [
                            (float)telemetry.VelX,
                            (float)telemetry.VelY,
                            (float)telemetry.VelZ
                        ];

                        // Dashboard data
                        carData.Speed = (float)(telemetry.Speed * 3.6); // m/s -> km/h
                        carData.RPM = telemetry.Rpm;
                        carData.MaxRPM = telemetry.MaxRpm;
                        carData.FuelLevel = (float)telemetry.FuelTankReserveNorm;
                        carData.Distance = telemetry.DistanceTraveled;
                        carData.Gear = (short)(telemetry.Gear + 1);

                        sessionInfo.Valid = true;
                        carData.Valid = true;
                        dataSet.TelemetryState = TelemetryState.Playing;

                        if (dataSet.GamePlugin is eXpanSIMGame game)
                            game.NotifyTelemetry(dataSet);
                    }
                    else
                    {
                        // No active vehicle
                        dataSet.LoadDefaults();
                        dataSet.TelemetryState = TelemetryState.Unknown;

                        if (dataSet.GamePlugin is eXpanSIMGame game)
                            game.NotifyTelemetry(dataSet);
                    }

                    Thread.Sleep(10); // ~100 Hz
                }
            }
            catch
            {
                Thread.Sleep(1000); // Wait for game to start
            }
        }
    }
}
