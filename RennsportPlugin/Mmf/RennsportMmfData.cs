// RennsportMmfData.cs — структуры данных Rennsport Shared Memory (MMF).
//
// !!! РЕАЛЬНАЯ СТРУКТУРА !!!
// Получена из IL-декомпиляции GSIReader.Rennsport.Models.dll (ридер SimHub).
//
// Архитектура MMF:
//   Заголовок (0x00..0x1B): 7 int32 смещений блоков:
//     0x00 = StartSequence, 0x04 = EntryList, 0x08 = CarLive,
//     0x0C = StageInfo,     0x10 = StageLive, 0x14 = TelemetryPoint,
//     0x18 = Leaderboard
//   Каждый блок читается по динамическому смещению из заголовка.
//   Это делает layout устойчивым к версиям игры — даже если структура
//   меняется в размере, смещения блоков читаются из заголовка.
//

using System.Runtime.InteropServices;

namespace RennsportPlugin
{
    internal static class RennsportMmfConstants
    {
        // Из Constants / LeaderboardConstants в IL.
        public const int MaxCars = 0x18;        // 24
        public const int MaxSectors = 9;
        public const int MaxPenalties = 0xF;    // 15
        public const int PenaltiesPerList = 3;

        // Размер фиксированных строковых uint8[] буферов. На живой игре выяснилось,
        // что РАЗНЫЕ поля имеют РАЗНЫЕ размеры. Все строки зашиты точными значениями:
        //   - StageInfo: ContestName=128, GameplayStageName=64, Track=128
        //   - EntryListCarData: все 6 строк = 64 (UserId, FirstName, LastName, ShortName, Nationality, Team)
        //   - CarLive: все 6 строк = 64 (UserId, CarId, FrontTireType, RearTireType, InPitStatus, CurrentFlag)
        //   - PenaltyData: PenaltyReason=64, PenaltyName=64

        // Индексы смещений в заголовке MMF (0x00..0x18, 7 int32).
        public const int IdxStartSequence = 0;
        public const int IdxEntryList = 1;
        public const int IdxCarLive = 2;
        public const int IdxStageInfo = 3;
        public const int IdxStageLive = 4;
        public const int IdxTelemetryPoint = 5;
        public const int IdxLeaderboard = 6;
        public const int HeaderSize = 28;       // 7 * 4
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct FPerWheelTelemetryPoint
    {
        public float RideHeight;
        public float Camber;
        public float Caster;
        public float ColdPressure;
        public float Pressure;            // кПа
        public float TempAir;
        public float TempCarcass;
        public float TempInnerL;
        public float TempInnerM;
        public float TempInnerR;
        public float TempInnerAvg;
        public float TempSurfaceL;
        public float TempSurfaceM;
        public float TempSurfaceR;
        public float TempSurfaceAvg;
        public float Toe;
        public float Wear;                // 1.0 = новая (Rennsport convention)
        public float WearL;
        public float WearM;
        public float WearR;
        public float SlipAngle;
        public float SlipRatio;
        public float OptimumTemperatureFraction;
        public float RPM;                 // об/мин колеса (не двигателя!)
        public float BrakeTemp;
        public float BrakeTorque;
        public float AvailableForce;
        public float SlipSpeed;
        public float GripFraction;
        public float LateralForce;
        public float LongitudinalForce;
        public float CurrentSuspensionPushrodLength;
        public float SuspensionForce;
        public float TireSpringRate;
        public float RimLoad;
        public float TireLoad;
        public uint ContactSurface;
        public float Grip;
        public float DynamicGripMult;
        public float Compression;
        public float DamperVelocity;
        public float TireWearGrip;
        public float TireHeatFraction;
        public float TireHeatMult;
        public float VehicleLoadFraction;
        public float LateralDeflectionFromRingCm;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FTelemetryPointExtraMetadata
    {
        [MarshalAs(UnmanagedType.U1)] public bool bIsGamePaused;
        [MarshalAs(UnmanagedType.U1)] public bool bIsUserInControl;
        public int UpdateCounter;
        public float TrackLengthMeters;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FTelemetryPointExtraPhysics
    {
        public byte GearMax;
        public float RPMMax;             // MaxEngineRPM (GD_MaxRpm)
        public float MaxFuel;            // MaxFuel (GD_MaxFuel)
        public double LocalVelX;
        public double LocalVelY;
        public double LocalVelZ;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FTelemetryPoint
    {
        public FTelemetryPointExtraMetadata ExtraMetadata;    // @0 (12 байт)
        public double TimeStamp;                              // @16 (0x10) ✓ подтверждено
        public double ServerWorldTimeSeconds;                 // @24
        [MarshalAs(UnmanagedType.U1)] public bool bABSActive;            // @32
        public float BrakeABSCutPct;                          // @36
        public int DriverNumber;                              // @40
        [MarshalAs(UnmanagedType.U1)] public bool bStabilityControlEnabled; // @44
        [MarshalAs(UnmanagedType.U1)] public bool bStabilityControlActive;  // @45
        [MarshalAs(UnmanagedType.U1)] public bool bTractionControlActive;   // @46
        public float TractionControlCutPct;                   // @48
        public float ThrottlePedalPos;                        // @52
        public float BrakePedalPos;                           // @56
        public float ClutchPedalPos;                          // @60
        public float SteeringWheelAngle;                      // @64
        public float SteeringShaftTorque;                     // @68
        public float SteeringForceFeedback;                   // @72
        public float Speed;                                   // @76 (м/с)
        public int GearIndex;                                 // @80 (-1=R, 0=N, 1=first)
        public int LapNumber;                                 // @84
        public int BestLapNumber;                             // @88
        public float BestLapTime;                             // @92 (сек)
        public float LapDistance;                             // @96 (м)
        public float LapDistancePct;                          // @100
        public float CenterFrontRideHeight;                   // @104
        public float CenterRideHeight;                        // @108
        public float CenterRearRideHeight;                    // @112
        public float LatAccel;                                // @116
        public float LonAccel;                                // @120
        public float VertAccel;                               // @124
        public FPerWheelTelemetryPoint LF;                    // @128
        public FPerWheelTelemetryPoint RF;                    // @128+sizeof(FPerWheel)
        public FPerWheelTelemetryPoint LR;
        public FPerWheelTelemetryPoint RR;
        public float Pitch;           // рад
        public float PitchRate;
        public float Roll;            // рад
        public float RollRate;
        public float Yaw;             // рад
        public float YawRate;
        public float FrontWingHeight;
        public float FrontRideHeight;
        public float RearRideHeight;
        public float Drag;
        public float FrontDownforce;
        public float RearDownforce;
        public float EstimatedDraftEffect;
        public float EngineCoolantPressure;   // кПа
        public float EngineCoolantTemperature;// °C (GD_WaterTemperature)
        public float EngineRPM;               // об/мин (GD_Rpms)
        public float EnginePower;
        public float EngineTorque;            // GD_EngineTorque
        public float ElectricMotorRPM;
        public float ElectricMotorPower;
        public float ElectricMotorTorque;
        public float TransmissionRPM;
        public float TransmissionPower;
        public float TransmissionTorque;
        public float TurboRPM;
        public float BoostPressure;           // кПа абс. (GD_Turbo → bar)
        public float EngineOilPressure;       // кПа (GD_OilPressure → psi)
        public float EngineOilTemperature;    // °C (GD_OilTemperature)
        public float FuelDensity;
        public float FuelLevel;               // л (GD_Fuel)
        public float FuelPressure;
        public float IntakeManifoldPressure;
        public float DumpValveFraction;
        public float ThrottlePressureDrop;
        public float AirPressure;
        public float AirTemp;                 // °C (GD_AirTemperature)
        public double Altitude;
        public double Latitude;
        public double Longitude;
        public int ABSLevel;                  // GD_ABSLevel
        [MarshalAs(UnmanagedType.U1)] public bool bABSEnabled;
        public float BrakeBias;               // 0..1 (GD_BrakeBias → ×100)
        public int FuelMixture;               // GD_EngineMap
        [MarshalAs(UnmanagedType.U1)] public bool bHeadlightFlash;
        [MarshalAs(UnmanagedType.U1)] public bool bHeadlightSwitch;
        [MarshalAs(UnmanagedType.U1)] public bool bIgnition;            // GD_EngineIgnitionOn
        [MarshalAs(UnmanagedType.U1)] public bool bPitSpeedLimiter;     // GD_PitLimiterOn
        [MarshalAs(UnmanagedType.U1)] public bool bStarter;             // GD_EngineStarted
        public int TractionControlLevel;      // GD_TCLevel
        [MarshalAs(UnmanagedType.U1)] public bool bTractionControlEnabled;
        [MarshalAs(UnmanagedType.U1)] public bool bDriverEventMarkerEnabled;
        [MarshalAs(UnmanagedType.U1)] public bool bLapValid;            // GD_IsLapValid
        public int Extra1;
        public int Extra2;
        public int Extra3;
        public FTelemetryPointExtraPhysics ExtraPhysicsData;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct FRennsportAPIMemoryGameplayStageLiveUpdateData
    {
        public float TimeLeft;            // сек (GD_SessionTimeLeft) @0
        public int LapsLeft;              // GD_TotalLaps @4
        public int CurrentlyObservedCar;  // @8
        // BestSectors: float32[9] @12 — не объявлено (читаем при необходимости)
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct FRennsportAPIMemoryStartSequenceUpdateData
    {
        public int StartLightCountdown;   // @0
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct FVector
    {
        public float X;
        public float Y;
        public float Z;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct FRennsportAPIMemoryLapData
    {
        public float Time;     // сек
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = RennsportMmfConstants.MaxSectors)]
        public float[] Sectors;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct FRennsportAPIMemoryPenaltyData
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string PenaltyReason;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string PenaltyName;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct FRennsportAPIMemoryCarLiveUpdateData
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string UserId;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string CarId;
        public int CarNumber;
        public int Position;              // GD_Position
        public int CompletedLaps;
        public FRennsportAPIMemoryLapData CurrentLap;   // GD_CurrentLapTime (Time)
        public FRennsportAPIMemoryLapData BestLap;
        public FRennsportAPIMemoryLapData LastLap;      // GD_LastLapTime (Time)
        public float LapProgress;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string FrontTireType;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string RearTireType;
        public float TotalTime;
        public float Gap;
        public float Interval;
        public float Speed;
        public float EngineRPM;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string InPitStatus;        // "InPitGarage" / "InPitLane" / ...
        public byte bDisqualified;        // GD_Flag_Black
        public byte bDidNotFinish;
        public byte bDidNotStart;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = RennsportMmfConstants.PenaltiesPerList)]
        public FRennsportAPIMemoryPenaltyData[] PendingPenalties;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = RennsportMmfConstants.PenaltiesPerList)]
        public FRennsportAPIMemoryPenaltyData[] ServedPenalties;
        public float SumTimePenaltiesAdded;
        public float SteeringInput;
        public float ThrottleInput;
        public float BrakeInput;
        public float ClutchInput;
        public int Gear;
        public FVector BarycentricCoordinates;
        public byte bIsConnected;
        public byte bFinished;            // GD_Flag_Checkered
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string CurrentFlag;
        public float MaxEngineRPM;        // дублирует FTelemetryPoint.ExtraPhysicsData.RPMMax
        public float OnWheelRPMLightsPct;
        public float OptimalShiftRPM;     // GD_Redline → ShiftUpRPM
        public byte bPitLimiter;
        public byte bAbsActive;
        public byte bTcActive;
        public byte bEspActive;
        public int CarLeft;               // GD_SpotterCarLeft
        public int CarRight;              // GD_SpotterCarRight
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct FRennsportAPIMemoryGameplayStageInfoUpdateData
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string ContestName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string GameplayStageName;  // GD_SessionTypeName
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string Track;              // GD_TrackName
        // BalanceOfPerformanceData[] — не объявлено (опционально, в конце struct)
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct FRennsportAPIMemoryEntryListCarData
    {
        public int CarNumber;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string UserId;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string FirstName;    
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string LastName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string ShortName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string Nationality;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string Team;
    }

    // IL: sequential sealed, .pack 1. Cars[MaxCars=24].
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct FRennsportAPIMemoryEntryListUpdateData
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = RennsportMmfConstants.MaxCars)]
        public FRennsportAPIMemoryEntryListCarData[] Cars;
    }
}
