// RennsportMmfProvider.cs — провайдер телеметрии Rennsport через Shared Memory (MMF).
//
// Структура MMF (из IL-декомпиляции GSIReader.Rennsport.Models):
//   Заголовок: 7 int32 смещений по 0x00..0x18:
//     0x00 StartSequence, 0x04 EntryList, 0x08 CarLive,
//     0x0C StageInfo,     0x10 StageLive, 0x14 TelemetryPoint, 0x18 Leaderboard
//   Каждый блок читается по динамическому смещению. Это надёжнее, чем
//   фиксировать структуру целиком — игра может менять размеры блоков.
//
// Что читаем:
//   - TelemetryPoint (offset[5]) — вся физика, моушн, шины, двигатель, топливо, AIDS.
//   - StageLive    (offset[4]) — TimeLeft, LapsLeft.
//   - StartSequence(offset[0]) — StartLightCountdown.
//   - CarLive      (offset[2]) — Position, OptimalShiftRPM, bFinished, bDisqualified,
//                                 CarLeft/Right, CompletedLaps, CurrentLap/LastLap.Time,
//                                 InPitStatus. Содержит строковые uint8[] буферы
//                                 размера StringFieldSize (по умолч. 128) — если SizeConst
//                                 неверный, поля после строк сдвинутся. Подберите в VS.
//   - StageInfo    (offset[3]) — TrackName, SessionTypeName (строковые буферы).
//   - EntryList    (offset[1]) — DriverName (поиск по CarNumber == DriverNumber).
//

using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Threading;
using vAzhureRacingAPI;

namespace RennsportPlugin
{
    public class RennsportMmfProvider : IDisposable
    {
        public event EventHandler<TelemetryUpdatedEventArgs> OnTelemetry;
        public event EventHandler OnConnectionError;

        private static readonly string[] CandidateNames =
        {
            "RennsportAPISharedMemory",
            "Local\\RennsportAPISharedMemory",
            "Global\\RennsportAPISharedMemory",
        };

        private readonly RennsportGame _game;
        private readonly int _pollIntervalMs;
        private readonly string _mmfName;

        private MemoryMappedFile _mmf;
        private MemoryMappedViewAccessor _acc;
        private Thread _thread;
        private volatile bool _running;
        private volatile bool _abort;

        private readonly object _dataLock = new object();
        private TelemetryDataSet _data;
        private int _lastUpdateCounter = -1;
        private bool _layoutWarned;
        internal DateTime _lastTelemetryTime = DateTime.UtcNow;
        private static readonly int TelemetryPointSize = Marshal.SizeOf<FTelemetryPoint>();

        public bool IsRunning => _running;

        public RennsportMmfProvider(RennsportGame game, string mmfName = null, int pollIntervalMs = 10)
        {
            _game = game ?? throw new ArgumentNullException(nameof(game));
            _mmfName = mmfName;
            _pollIntervalMs = pollIntervalMs;
        }

        public void Start()
        {
            if (_running) return;
            if (!TryOpenMmf())
            {
                OnConnectionError?.Invoke(this, EventArgs.Empty);
                return;
            }
            _abort = false;
            _thread = new Thread(MainLoop) { IsBackground = true, Name = "RennsportMmf" };
            _thread.Start();
        }

        public void Stop()
        {
            _abort = true;
            _running = false;
            try { _thread?.Join(500); } catch { }
            _thread = null;
            try { _acc?.Dispose(); } catch { }
            try { _mmf?.Dispose(); } catch { }
            _acc = null;
            _mmf = null;
        }

        public void Dispose() => Stop();

        private bool TryOpenMmf()
        {
            var names = string.IsNullOrEmpty(_mmfName) ? CandidateNames : new[] { _mmfName };
            foreach (var n in names)
            {
                try
                {
                    _mmf = MemoryMappedFile.OpenExisting(n);
                    // Открываем на чтение весь файл — размер узнаём через Capacity.
                    _acc = _mmf.CreateViewAccessor(0, 0, MemoryMappedFileAccess.Read);
                    return true;
                }
                catch (FileNotFoundException) { continue; }
                catch { continue; }
            }
            return false;
        }

        private void MainLoop()
        {
            _running = true;
            try
            {
                while (!_abort)
                {
                    try
                    {
                        PollOnce();
                    }
                    catch
                    {
                        // MMF мог закрыться (игра вышла в меню). Пробуем переоткрыть.
                        try { _acc?.Dispose(); _mmf?.Dispose(); } catch { }
                        _acc = null; _mmf = null;
                        Thread.Sleep(500);
                        if (!TryOpenMmf())
                        {
                            OnConnectionError?.Invoke(this, EventArgs.Empty);
                            Thread.Sleep(1000);
                        }
                        continue;
                    }
                    Thread.Sleep(_pollIntervalMs);
                }
            }
            finally
            {
                _running = false;
            }
        }

        private void PollOnce()
        {
            if (_acc == null) return;
            long cap = _acc.Capacity;
            if (cap < RennsportMmfConstants.HeaderSize) return;

            // 1. Читаем 7 смещений блоков из заголовка.
            int[] off = new int[7];
            for (int i = 0; i < 7; i++)
                off[i] = _acc.ReadInt32(i * 4L);

            // 2. TelemetryPoint — главный блок.
            int tpOff = off[RennsportMmfConstants.IdxTelemetryPoint];
            if (tpOff <= 0 || tpOff + TelemetryPointSize > cap) return;
            var tp = ReadStructAt<FTelemetryPoint>(tpOff);

            // Self-check layout: TimeStamp — конечный double, Speed — разумный м/с,
            // EngineRPM — 0..25000, давление LF шины — 50..500 кПа.
            // Если EngineRPM/LF.Pressure мусор, но Speed/TimeStamp нормальны —
            // значит размер FPerWheelTelemetryPoint неверен (Pack=1 vs Pack=4):
            // поля ПОСЛЕ 4-х колес (Pitch/Roll/Yaw, двигатель, топливо, AIDS) читаются
            // со сдвигом. Поля ДО колес (Speed, педали, lap) при этом корректны.
            // В этом случае поменяйте Pack у FPerWheelTelemetryPoint на 4 (или
            // добавьте [FieldOffset] из dnSpy) — и всё встанет.
            if (!_layoutWarned)
            {
                bool preWheelOk = !double.IsNaN(tp.TimeStamp) && !double.IsInfinity(tp.TimeStamp)
                                  && tp.Speed > -50 && tp.Speed < 200;
                bool postWheelOk = tp.EngineRPM > -100 && tp.EngineRPM < 30000
                                   && tp.LF.Pressure > 0 && tp.LF.Pressure < 1000
                                   && tp.EngineCoolantTemperature > -50 && tp.EngineCoolantTemperature < 200;
                if (preWheelOk && !postWheelOk)
                {
                    _layoutWarned = true;
                    // Сигнализируем через UserData, чтобы дашборд мог показать предупреждение.
                }
                else if (!preWheelOk)
                {
                    // Совсем плохо — вероятно неверный размер bool (U1 vs Bool) или
                    // MMF ещё не инициализирован. Не Warn'аем сразу, ждём стабилизации.
                }
            }

            // Пропускаем кадр, если UpdateCounter не изменился (экономия CPU).
            if (tp.ExtraMetadata.UpdateCounter == _lastUpdateCounter)
            {
                // Даже без нового кадра — раз в секунду шлём, чтобы хаб видел "живые" данные.
                // Но проще: просто return, хаб сам интерполирует.
                return;
            }
            _lastUpdateCounter = tp.ExtraMetadata.UpdateCounter;

            // 3. StageLive — TimeLeft, LapsLeft (скаляры, без массивов).
            float timeLeft = -1f;
            int lapsLeft = 0;
            int slOff = off[RennsportMmfConstants.IdxStageLive];
            if (slOff > 0 && slOff + 12 <= cap)
            {
                timeLeft = _acc.ReadSingle(slOff);
                lapsLeft = _acc.ReadInt32(slOff + 4L);
            }

            // 4. StartSequence — StartLightCountdown.
            int startLight = 0;
            int ssOff = off[RennsportMmfConstants.IdxStartSequence];
            if (ssOff > 0 && ssOff + 4 <= cap)
                startLight = _acc.ReadInt32(ssOff);

            // 5. CarLive — Position, OptimalShiftRPM, bFinished, bDisqualified,
            FRennsportAPIMemoryCarLiveUpdateData carLive = default;
            bool carLiveOk = false;
            int clOff = off[RennsportMmfConstants.IdxCarLive];
            int carLiveSize = Marshal.SizeOf<FRennsportAPIMemoryCarLiveUpdateData>();
            if (clOff > 0 && clOff + carLiveSize <= cap)
            {
                try { carLive = ReadStructAt<FRennsportAPIMemoryCarLiveUpdateData>(clOff); carLiveOk = true; }
                catch { /* SizeConst может быть неверным — не падаем */ }
            }

            // 6. StageInfo — TrackName, SessionTypeName (строковые буферы).
            FRennsportAPIMemoryGameplayStageInfoUpdateData stageInfo = default;
            bool stageInfoOk = false;
            int siOff = off[RennsportMmfConstants.IdxStageInfo];
            int stageInfoSize = Marshal.SizeOf<FRennsportAPIMemoryGameplayStageInfoUpdateData>();
            if (siOff > 0 && siOff + stageInfoSize <= cap)
            {
                try { stageInfo = ReadStructAt<FRennsportAPIMemoryGameplayStageInfoUpdateData>(siOff); stageInfoOk = true; }
                catch { }
            }

            // 7. EntryList — DriverName (ищем машину по CarNumber == DriverNumber).
            FRennsportAPIMemoryEntryListUpdateData entryList = default;
            bool entryListOk = false;
            int elOff = off[RennsportMmfConstants.IdxEntryList];
            int entryListSize = Marshal.SizeOf<FRennsportAPIMemoryEntryListUpdateData>();
            if (elOff > 0 && elOff + entryListSize <= cap)
            {
                try { entryList = ReadStructAt<FRennsportAPIMemoryEntryListUpdateData>(elOff); entryListOk = true; }
                catch { }
            }

            ApplyToTelemetry(tp, timeLeft, lapsLeft, startLight,
                carLiveOk, carLive, stageInfoOk, stageInfo, entryListOk, entryList);
        }

        // Чтение struct по смещению.
        // MemoryMappedViewAccessor.Read<T> в некоторых версиях .NET Framework
        // бросает ArgumentException для struct с [MarshalAs(U1)] bool (non-blittable).
        // Поэтому используем проверенный паттерн: ReadArray в byte[], потом
        // Marshal.PtrToStructure<T> — он уважает все marshal-атрибуты (включая U1 bool).
        // Тот же приём, что и в SimHub (ReadStruct через SafeBuffer + PtrToStructure),
        // только без unsafe.
        private T ReadStructAt<T>(long position) where T : struct
        {
            int size = Marshal.SizeOf<T>();
            byte[] buf = new byte[size];
            int read = _acc.ReadArray(position, buf, 0, size);
            if (read < size) return default;
            GCHandle h = GCHandle.Alloc(buf, GCHandleType.Pinned);
            try
            {
                return Marshal.PtrToStructure<T>(h.AddrOfPinnedObject());
            }
            finally { h.Free(); }
        }

        private void ApplyToTelemetry(FTelemetryPoint tp, float stageTimeLeft, int lapsLeft, int startLight,
            bool carLiveOk, FRennsportAPIMemoryCarLiveUpdateData carLive,
            bool stageInfoOk, FRennsportAPIMemoryGameplayStageInfoUpdateData stageInfo,
            bool entryListOk, FRennsportAPIMemoryEntryListUpdateData entryList)
        {
            lock (_dataLock)
            {
                _data = _data ?? new TelemetryDataSet(_game);
                AMCarData car = _data.CarData;
                AMSessionInfo ses = _data.SessionInfo;
                AMMotionData motion = car.MotionData;

                // --- Состояние ---
                bool paused = tp.ExtraMetadata.bIsGamePaused;
                bool userControl = tp.ExtraMetadata.bIsUserInControl;
                _data.TelemetryState = paused ? TelemetryState.Paused
                    : (userControl ? TelemetryState.Playing : TelemetryState.Replay);

                // --- Двигатель / коробка ---
                car.Speed = tp.Speed * 3.6;                       // м/с → км/ч
                car.RPM = tp.EngineRPM;
                car.MaxRPM = tp.ExtraPhysicsData.RPMMax;
                car.ShiftUpRPM = carLiveOk ? carLive.OptimalShiftRPM : 0;
                car.Gear = (short)(tp.GearIndex + 1);
                car.Steering = tp.SteeringWheelAngle;
                car.Throttle = Clamp01(tp.ThrottlePedalPos);
                car.Brake = Clamp01(tp.BrakePedalPos);
                car.Clutch = Clamp01(tp.ClutchPedalPos);
                car.BrakeBias = 100f - tp.BrakeBias * 100f;               // 0..1 → 0..100
                car.TcLevel = (short)tp.TractionControlLevel;
                car.TcLevel2 = -1;
                car.AbsLevel = (short)tp.ABSLevel;
                car.EngineMap = (short)tp.FuelMixture;
                car.IgnitionStarter = (short)(tp.bIgnition ? (tp.bStarter ? 2 : 1) : 0);

                // --- Жидкости ---
                car.OilTemp = tp.EngineOilTemperature;
                car.WaterTemp = tp.EngineCoolantTemperature;
                car.FuelLevel = tp.FuelLevel;                      // литры
                car.FuelCapacity = tp.ExtraPhysicsData.MaxFuel;    // литры (из ExtraPhysics!)
                car.OilPressure = tp.EngineOilPressure * 0.1450377377; // кПа → PSI
                car.WaterPressure = tp.EngineCoolantPressure * 0.1450377377;

                // --- Электроника / флаги ---
                CarElectronics el = CarElectronics.None;
                if (tp.bABSActive) el |= CarElectronics.ABS;
                if (tp.bTractionControlActive) el |= CarElectronics.TCS;
                if (tp.bHeadlightSwitch) el |= CarElectronics.Headlight;
                if (tp.bPitSpeedLimiter) el |= CarElectronics.Limiter;
                if (tp.bIgnition) el |= CarElectronics.Ignition;
                car.Electronics = el;
                car.AIControlled = !userControl;

                car.Distance = tp.LapDistance;
                car.Lap = tp.LapNumber;
                car.Position = carLiveOk ? carLive.Position : 0;  // CarLive.Position (GD_Position)

                car.Tires = new[]
                {
                    ToTire(tp.LF),
                    ToTire(tp.RF),
                    ToTire(tp.LR),
                    ToTire(tp.RR),
                };

                motion.Pitch = (float)(tp.Pitch / Math.PI);        // нормализованные рад (-1=±180°)
                motion.Roll = (float)(tp.Roll / Math.PI);
                motion.Yaw = (float)(tp.Yaw / Math.PI);
                motion.Sway = -tp.LatAccel;
                motion.Heave = tp.VertAccel;
                motion.Surge = -tp.LonAccel;
                motion.LocalAcceleration = new[] { motion.Sway, motion.Heave, motion.Surge };
                motion.LocalRot = new[] { tp.YawRate, tp.PitchRate, tp.RollRate };
                motion.LocalVelocity = new float[] { 0, 0, 0 };
                motion.LocalRotAcceleration = new float[] { 0, 0, 0 };
                motion.ABSVibration = tp.bABSActive ? 1f : 0f;

                // --- Сессия ---
                ses.Valid = true;
                ses.CurrentLapNumber = tp.LapNumber;
                // Текущее время круга: предпочтём CarLive.CurrentLap.Time (как SimHub),
                // иначе fallback на TimeStamp.
                if (carLiveOk && carLive.CurrentLap.Time >= 0 && !float.IsNaN(carLive.CurrentLap.Time))
                    ses.CurrentLapTime = FiniteSecondsToMs(carLive.CurrentLap.Time);
                else
                    ses.CurrentLapTime = FiniteSecondsToMs(tp.TimeStamp);
                ses.BestLapTime = tp.BestLapTime > 0 ? FiniteSecondsToMs(tp.BestLapTime) : 0;
                ses.CurrentPosition = carLiveOk ? carLive.Position : 0;
                ses.TrackLength = tp.ExtraMetadata.TrackLengthMeters;
                if (stageTimeLeft >= 0)
                {
                    ses.RemainingTime = (int)Math.Round(stageTimeLeft * 1000.0);
                    ses.RemainingLaps = lapsLeft;
                    // GD_TotalLaps = LapsLeft + GD_CompletedLaps (= max(0, LapNumber-1))
                    ses.TotalLapsCount = lapsLeft + Math.Max(0, tp.LapNumber - 1);
                }
                // StageInfo — строковые буферы (SizeConst = StringFieldSize).
                if (stageInfoOk)
                {
                    ses.SessionState = TrimString(stageInfo.GameplayStageName); // GD_SessionTypeName
                    ses.TrackName = TrimString(stageInfo.Track);                 // GD_TrackName
                }
                else
                {
                    ses.SessionState = "";
                    ses.TrackName = "";
                }
                // EntryList — DriverName (по CarNumber == DriverNumber).
                ses.DriversCount = 0;
                car.DriverName = "";
                car.CarNumber = "";
                if (entryListOk && entryList.Cars != null)
                {
                    int drivers = 0;
                    foreach (var c in entryList.Cars)
                    {
                        if (IsPopulatedEntry(c)) drivers++;
                        if (c.CarNumber == tp.DriverNumber)
                        {
                            car.CarNumber = c.CarNumber.ToString();
                            string fn = TrimString(c.FirstName);
                            string ln = TrimString(c.LastName);
                            string name = $"{fn} {ln}".Trim();
                            if (string.IsNullOrEmpty(name))
                                name = TrimString(c.ShortName);
                            if (string.IsNullOrEmpty(name) && c.CarNumber != 0)
                                name = $"Car {c.CarNumber}";
                            car.DriverName = name;
                        }
                    }
                    ses.DriversCount = drivers;
                }

                // Флаг: pit limiter → Pit; Chequered (bFinished) → Chequered;
                // Black (bDisqualified) → Black; стартовые огни горят → "";
                // иначе Green.
                if (tp.bPitSpeedLimiter) ses.Flag = "Pit";
                else if (carLiveOk && carLive.bFinished != 0) ses.Flag = "Chequered";
                else if (carLiveOk && carLive.bDisqualified != 0) ses.Flag = "Black";
                else if (startLight > 0) ses.Flag = "";
                else
                {
                    car.Flags = TelemetryFlags.FlagGreen;
                    ses.Flag = "Green";
                }

                // Pit status из CarLive (если SizeConst верный и InPitStatus читается).
                if (carLiveOk)
                {
                    string pitStatus = TrimString(carLive.InPitStatus);
                    if (pitStatus.Equals("InPitGarage", StringComparison.OrdinalIgnoreCase))
                    {
                        car.InPits = true;
                        car.PitState = 1;
                    }
                    else if (pitStatus.Equals("InPitLane", StringComparison.OrdinalIgnoreCase))
                    {
                        car.InPits = true;
                        car.PitState = 2;
                    }
                }

                // --- UserData: «сырые» Rennsport-поля для продвинутых дашбордов ---
                //var ud = _data.UserData;
                //ud["Rennsport.Source"] = "MMF";
                //ud["Rennsport.UpdateCounter"] = tp.ExtraMetadata.UpdateCounter;
                //ud["Rennsport.DriverNumber"] = tp.DriverNumber;
                //ud["Rennsport.TimeStamp"] = tp.TimeStamp;
                //ud["Rennsport.BoostPressureKPa"] = tp.BoostPressure;
                //ud["Rennsport.BoostPressureBar"] = tp.BoostPressure * 0.001; // кПа → бар (приблизительно)
                //ud["Rennsport.TurboRPM"] = tp.TurboRPM;
                //ud["Rennsport.EnginePower"] = tp.EnginePower;
                //ud["Rennsport.EngineTorque"] = tp.EngineTorque;
                //ud["Rennsport.IntakeManifoldPressure"] = tp.IntakeManifoldPressure;
                //ud["Rennsport.FuelPressure"] = tp.FuelPressure;
                //ud["Rennsport.AirTemp"] = tp.AirTemp;
                //ud["Rennsport.Altitude"] = tp.Altitude;
                //ud["Rennsport.Latitude"] = tp.Latitude;
                //ud["Rennsport.Longitude"] = tp.Longitude;
                //ud["Rennsport.Drag"] = tp.Drag;
                //ud["Rennsport.FrontDownforce"] = tp.FrontDownforce;
                //ud["Rennsport.RearDownforce"] = tp.RearDownforce;
                //ud["Rennsport.SteeringForceFeedback"] = tp.SteeringForceFeedback;
                //ud["Rennsport.SteeringShaftTorque"] = tp.SteeringShaftTorque;
                //ud["Rennsport.LapValid"] = tp.bLapValid;
                //ud["Rennsport.LapDistancePct"] = tp.LapDistancePct;
                //ud["Rennsport.StartLightCountdown"] = startLight;
                //ud["Rennsport.GearMax"] = tp.ExtraPhysicsData.GearMax;
                //ud["Rennsport.LayoutWarning"] = _layoutWarned;
                //if (carLiveOk)
                //{
                //    ud["Rennsport.CarLive.Ok"] = true;
                //    ud["Rennsport.SpotterCarLeft"] = carLive.CarLeft;     // GD_SpotterCarLeft
                //    ud["Rennsport.SpotterCarRight"] = carLive.CarRight;   // GD_SpotterCarRight
                //    ud["Rennsport.Gap"] = carLive.Gap;
                //    ud["Rennsport.Interval"] = carLive.Interval;
                //    ud["Rennsport.CompletedLaps"] = carLive.CompletedLaps;
                //    ud["Rennsport.LastLapTimeMs"] = carLive.LastLap.Time > 0
                //        ? FiniteSecondsToMs(carLive.LastLap.Time) : 0;
                //    ud["Rennsport.OnWheelRPMLightsPct"] = carLive.OnWheelRPMLightsPct;
                //    ud["Rennsport.bPitLimiter"] = carLive.bPitLimiter != 0;
                //    ud["Rennsport.bAbsActiveCarLive"] = carLive.bAbsActive != 0;
                //    ud["Rennsport.bTcActiveCarLive"] = carLive.bTcActive != 0;
                //    ud["Rennsport.PitStatus"] = TrimString(carLive.InPitStatus);
                //    ud["Rennsport.CurrentFlag"] = TrimString(carLive.CurrentFlag);
                //}
                //else
                //{
                //    ud["Rennsport.CarLive.Ok"] = false;
                //}
                //PutWheelExtras(ud, "FL", tp.LF);
                //PutWheelExtras(ud, "FR", tp.RF);
                //PutWheelExtras(ud, "RL", tp.LR);
                //PutWheelExtras(ud, "RR", tp.RR);

                car.Valid = true;

                OnTelemetry?.Invoke(this, new TelemetryUpdatedEventArgs(_data));
            }
        }

        private static AMTireData ToTire(FPerWheelTelemetryPoint w)
        {
            // Давление храним в кПа (AMTireData.Pressure по контракту — кПа).
            // IL: GD_TyrePressure* конвертирует кПа → psi, но vAzhureRacingHub ожидает кПа.
            // Износ: Rennsport 1.0 = новая → AMTireData 0 = новая → инвертируем.
            return new AMTireData
            {
                Pressure = w.Pressure,
                Wear = Clamp01(1.0 - w.Wear),
                Temperature = new[] {(double) w.TempSurfaceL, w.TempSurfaceM, w.TempSurfaceR },
                BrakeTemperature = w.BrakeTemp,
                Detached = false,
                Compound = "",
            };
        }

        private static void PutWheelExtras(System.Collections.Generic.Dictionary<string, object> ud, string tag, FPerWheelTelemetryPoint w)
        {
            string p = $"Rennsport.Wheel.{tag}.";
            ud[p + "SlipAngle"] = w.SlipAngle;
            ud[p + "SlipRatio"] = w.SlipRatio;
            ud[p + "SuspensionForce"] = w.SuspensionForce;
            ud[p + "TireLoad"] = w.TireLoad;
            ud[p + "Compression"] = w.CurrentSuspensionPushrodLength;
            ud[p + "DamperVelocity"] = w.DamperVelocity;
            ud[p + "RideHeight"] = w.RideHeight;
            ud[p + "Camber"] = w.Camber;
            ud[p + "Toe"] = w.Toe;
            ud[p + "TempCarcass"] = w.TempCarcass;
            ud[p + "TempInnerAvg"] = w.TempInnerAvg;
            ud[p + "TempSurfaceAvg"] = w.TempSurfaceAvg;
            ud[p + "ColdPressure"] = w.ColdPressure;
            ud[p + "ContactSurface"] = w.ContactSurface;
            ud[p + "WheelRPM"] = w.RPM;
        }

        private static int FiniteSecondsToMs(double seconds)
        {
            if (double.IsNaN(seconds) || double.IsInfinity(seconds) || seconds < 0) return 0;
            if (seconds > 3.6e6) return int.MaxValue; // > 1000 ч — явно мусор
            return (int)Math.Round(seconds * 1000.0);
        }

        // ByValTStr-поля уже читаются маршалером как string (обрезается по null).
        // TrimString — defensive обрезка пробелов + null-safe.
        private static string TrimString(string s) => s?.Trim() ?? "";

        // IL: IsPopulatedEntry — UserId/FirstName/LastName все непустые.
        private static bool IsPopulatedEntry(FRennsportAPIMemoryEntryListCarData entry)
        {
            return !string.IsNullOrEmpty(TrimString(entry.UserId))
                || !string.IsNullOrEmpty(TrimString(entry.FirstName))
                || !string.IsNullOrEmpty(TrimString(entry.LastName));
        }

        private static float Clamp01(double v) => v < 0 ? 0 : (v > 1 ? 1 : (float)v);
    }
}
