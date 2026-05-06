using System;
using System.Runtime.InteropServices;

namespace GT7Telemetry
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct GT7PacketB
    {
        // ==== ВСЕ ПОЛЯ ИЗ ПАКЕТА A (0x00 - 0x129) ====
        public int Magic;                    // 0x00
        public float PositionX;              // 0x04
        public float PositionY;              // 0x08
        public float PositionZ;              // 0x0C
        public float VelocityX;              // 0x10
        public float VelocityY;              // 0x14
        public float VelocityZ;              // 0x18
        public float Pitch;                  // 0x1C
        public float Yaw;                    // 0x20
        public float Roll;                   // 0x24
        public float OrientationToNorth;     // 0x28
        public float AngularVelocityX;       // 0x2C
        public float AngularVelocityY;       // 0x30
        public float AngularVelocityZ;       // 0x34
        public float BodyHeight;             // 0x38
        public float EngineRPM;              // 0x3C
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] IV;                    // 0x40 - 0x43
        public float FuelLevel;              // 0x44
        public float FuelCapacity;           // 0x48
        public float Speed;                  // 0x4C
        public float Boost;                  // 0x50
        public float OilPressure;            // 0x54
        public float WaterTemp;              // 0x58
        public float OilTemp;                // 0x5C
        public float TyreTempFL;             // 0x60
        public float TyreTempFR;             // 0x64
        public float TyreTempRL;             // 0x68
        public float TyreTempRR;             // 0x6C
        public int PacketId;                 // 0x70
        public short LapCount;               // 0x74
        public short TotalLaps;              // 0x76
        public int BestLapTime;              // 0x78
        public int LastLapTime;              // 0x7C
        public int DayProgression;           // 0x80
        public short RaceStartPosition;      // 0x84
        public short PreRaceNumCars;         // 0x86
        public short MinAlertRPM;            // 0x88
        public short MaxAlertRPM;            // 0x8A
        public short CalcMaxSpeed;           // 0x8C
        public uint Flags;                   // 0x8E
        public byte Gears;                   // 0x92
        public byte Throttle;                // 0x93
        public byte Brake;                   // 0x94
        public byte UnknownByte1;            // 0x95
        public float RoadPlaneX;             // 0x96
        public float RoadPlaneY;             // 0x9A
        public float RoadPlaneZ;             // 0x9E
        public float RoadPlaneDistance;      // 0xA2
        public float WheelRPSFL;             // 0xA6
        public float WheelRPSFR;             // 0xAA
        public float WheelRPSRL;             // 0xAE
        public float WheelRPSRR;             // 0xB2
        public float TyreRadiusFL;           // 0xB6
        public float TyreRadiusFR;           // 0xBA
        public float TyreRadiusRL;           // 0xBE
        public float TyreRadiusRR;           // 0xC2
        public float SuspHeightFL;           // 0xC6
        public float SuspHeightFR;           // 0xCA
        public float SuspHeightRL;           // 0xCE
        public float SuspHeightRR;           // 0xD2
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public float[] UnknownFloats;        // 0xD6 - 0xF5
        public float Clutch;                 // 0xF6
        public float ClutchEngagement;       // 0xFA
        public float RPMFromClutchToGearbox; // 0xFE
        public float TransmissionTopSpeed;   // 0x102
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public float[] GearRatios;           // 0x106 - 0x125
        public int CarCode;                  // 0x126
        
        // ==== ДОПОЛНИТЕЛЬНЫЕ ПОЛЯ ПАКЕТА B ====
        public float AccelerationX;          // 0x12A  <-- SURGE (вперёд-назад, м/с²)
        public float AccelerationY;          // 0x12E  <-- SWAY  (влево-вправо, м/с²)
        public float AccelerationZ;          // 0x132  <-- HEAVE (вверх-вниз, м/с²)

        // ==== ВСПОМОГАТЕЛЬНЫЕ СВОЙСТВА ====
        public int CurrentGear => (Gears & 0x0F);
        public int SuggestedGear => ((Gears >> 4) & 0x0F);
        public float SpeedKmh => Speed * 3.6f;
        
        /// <summary>Surge: ускорение вперёд-назад (G-force)</summary>
        public float SurgeG => AccelerationX / 9.81f;
        
        /// <summary>Sway: боковое ускорение (G-force)</summary>
        public float SwayG => AccelerationY / 9.81f;
        
        /// <summary>Heave: вертикальное ускорение (G-force)</summary>
        public float HeaveG => AccelerationZ / 9.81f;
    }
}