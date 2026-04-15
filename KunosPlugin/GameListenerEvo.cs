using System;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using vAzhureRacingAPI;
using Kunos.Structs;

namespace KunosPlugin
{
    /// <summary>
    /// Обработчик Shared Memory для Assetto Corsa EVO.
    /// Содержит чтение MMF и маппинг структур SPageFile*EVO в TelemetryDataSet.
    /// </summary>
    internal static class GameListenerEvo
    {
        public static void Process(GamePlugin game, ref int packetId, out bool hasNewData)
        {
            hasNewData = false;
            if (!game.IsRunning || !game.Enabled) return;

            try
            {
                using (var memPhys = MemoryMappedFile.OpenExisting(ACEVOConstants.PhysicsFile, MemoryMappedFileRights.Read))
                using (var memGfx = MemoryMappedFile.OpenExisting(ACEVOConstants.GraphicsFile, MemoryMappedFileRights.Read))
                using (var memStat = MemoryMappedFile.OpenExisting(ACEVOConstants.StaticFile, MemoryMappedFileRights.Read))
                {
                    var physBytes = new byte[Marshal.SizeOf<SPageFilePhysics_EVO>()];
                    var gfxBytes = new byte[Marshal.SizeOf<SPageFileGraphicEvo>()];
                    var statBytes = new byte[Marshal.SizeOf<SPageFileStaticEvo>()];

                    using (var vs = memPhys.CreateViewStream(0, physBytes.Length, MemoryMappedFileAccess.Read)) vs.Read(physBytes, 0, physBytes.Length);
                    using (var vs = memGfx.CreateViewStream(0, gfxBytes.Length, MemoryMappedFileAccess.Read)) vs.Read(gfxBytes, 0, gfxBytes.Length);
                    using (var vs = memStat.CreateViewStream(0, statBytes.Length, MemoryMappedFileAccess.Read)) vs.Read(statBytes, 0, statBytes.Length);

                    var physics = SPageFilePhysics_EVO.FromBytes(physBytes);

                    // Проверяем обновление пакета
                    if (physics.packetId != packetId)
                    {
                        packetId = physics.packetId;
                        hasNewData = true;

                        var graphics = SPageFileGraphicEvo.FromBytes(gfxBytes);
                        var staticData = SPageFileStaticEvo.FromBytes(statBytes);

                        MapTelemetry(game, staticData, graphics, physics);
                    }
                }
            }
            catch
            {
                // MMF is not created yet
            }
        }

        private static void MapTelemetry(GamePlugin game, SPageFileStaticEvo stat, SPageFileGraphicEvo gfx, SPageFilePhysics_EVO phys)
        {
            var car    = game.DataSet.CarData;
            var sess   = game.DataSet.SessionInfo;
            var motion = car.MotionData;
            var weather= game.DataSet.WeatherData;

            // --- Static & Environment ---
            sess.TrackName   = stat.track?.TrimEnd('\0') ?? string.Empty;
            sess.TrackConfig = stat.track_configuration?.TrimEnd('\0') ?? string.Empty;
            sess.TrackLength = stat.track_length_m;
            sess.SessionState= stat.session_name?.TrimEnd('\0') ?? string.Empty;
            weather.AmbientTemp = stat.starting_ambient_temperature_c;
            weather.TrackTemp   = stat.starting_ground_temperature_c;

            car.DriverName = $"{gfx.driver_name?.TrimEnd('\0')} {gfx.driver_surname?.TrimEnd('\0')}".Trim();
            car.CarName    = Plugin.sVechicleInfo.GetVehicleName(gfx.car_model?.TrimEnd('\0'));
            car.FuelCapacity = gfx.max_fuel;
            car.MaxRPM       = (uint)phys.currentMaxRpm;

            // --- Session & Timing ---
            sess.CurrentLapNumber = gfx.session_state.current_lap;
            sess.TotalLapsCount   = gfx.session_state.total_lap;
            car.Position          = (int)gfx.current_pos;
            car.InPits            = gfx.is_in_pit_lane || gfx.is_in_pit_box;
            car.Distance          = gfx.npos;
            sess.RemainingTime    = gfx.session_state.time_left_ms > 0 ? gfx.session_state.time_left_ms / 1000 : -1;
            sess.RemainingLaps    = sess.TotalLapsCount > 0 ? sess.TotalLapsCount - sess.CurrentLapNumber : -1;

            sess.CurrentLapTime = gfx.current_lap_time_ms;
            sess.LastLapTime    = gfx.last_laptime_ms;
            sess.BestLapTime    = gfx.best_laptime_ms;
            sess.CurrentDelta   = gfx.delta_time_ms;

            // --- Flags ---
            car.Flags = MapFlags(gfx.flag);
            sess.Flag = gfx.flag.ToString().Replace("AC_", "").Replace("_", " ");

            // --- Electronics ---
            car.TcLevel   = (short)gfx.electronics.tc_level;
            car.AbsLevel  = (short)gfx.electronics.abs_level;
            car.EngineMap = (short)(gfx.electronics.engine_map_level + 1);
            car.Electronics = CarElectronics.None;
            if (gfx.electronics.is_pitlimiter_on) car.Electronics |= CarElectronics.Limiter;
            if (gfx.electronics.is_drs_open)      car.Electronics |= CarElectronics.DRS;
            if (gfx.tc_active)                    car.Electronics |= CarElectronics.TCS;
            if (gfx.abs_active)                   car.Electronics |= CarElectronics.ABS;

            // --- Physics & Motion ---
            car.Steering = phys.steerAngle * (float)Math.PI * 2.0f;
            car.Throttle = phys.gas;
            car.Brake    = phys.brake;
            car.Clutch   = 1.0f - phys.clutch;
            car.Speed    = phys.speedKmh;
            car.RPM      = (uint)phys.rpms;
            car.Gear     = (short)phys.gear;
            car.FuelLevel= phys.fuel;
            car.BrakeBias= phys.brakeBias * 100.0f;

            motion.LocalAcceleration = new float[] { phys.accG[0] / 9.81f, phys.accG[1] / 9.81f, phys.accG[2] / 9.81f };
            motion.LocalVelocity     = phys.localVelocity;
            motion.Pitch             = phys.pitch / (float)Math.PI;
            motion.Roll              = phys.roll / (float)Math.PI; 
            motion.Yaw               = phys.heading / (float)Math.PI;
            motion.ABSVibration      = phys.absVibrations;

            car.Tires = new AMTireData[]
            {
                MapTire(gfx.tyre_lf, phys.brakeTemp[0], phys.wheelsPressure[0]),
                MapTire(gfx.tyre_rf, phys.brakeTemp[1], phys.wheelsPressure[1]),
                MapTire(gfx.tyre_lr, phys.brakeTemp[2], phys.wheelsPressure[2]),
                MapTire(gfx.tyre_rr, phys.brakeTemp[3], phys.wheelsPressure[3])
            };

            car.Valid   = true;
            sess.Valid  = true;
            weather.Valid = true;
        }

        private static AMTireData MapTire(SMEvoTyreState tyre, float brakeTemp, float pressurePSI)
        {
            return new AMTireData
            {
                BrakeTemperature = brakeTemp,
                Pressure         = pressurePSI * 6.89476f, // PSI → kPa
                Wear             = 0.0f, // EVO Shared Memory пока не отдаёт износ шин напрямую
                Temperature      = new double[] { tyre.tyre_temperature_left, tyre.tyre_temperature_center, tyre.tyre_temperature_right },
                Compound         = tyre.tyre_compound_front?.TrimEnd('\0') ?? string.Empty
            };
        }

        private static TelemetryFlags MapFlags(ACEVO_FLAG_TYPE flag)
        {
            switch (flag)
            {
                case ACEVO_FLAG_TYPE.AC_YELLOW_FLAG: return TelemetryFlags.FlagYellow;
                case ACEVO_FLAG_TYPE.AC_WHITE_FLAG: return TelemetryFlags.FlagWhite;
                case ACEVO_FLAG_TYPE.AC_BLUE_FLAG: return TelemetryFlags.FlagBlue;
                case ACEVO_FLAG_TYPE.AC_BLACK_FLAG: return TelemetryFlags.FlagBlack;
                case ACEVO_FLAG_TYPE.AC_CHECKERED_FLAG: return TelemetryFlags.FlagChequered;
                default: return TelemetryFlags.FlagNone;
            }
        }
    }
}