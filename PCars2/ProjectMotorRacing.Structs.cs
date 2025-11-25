using System;
using System.Runtime.InteropServices;

namespace ProjectMotorRacing.Structs
{
    /// <summary>
    /// Wheel material enum
    /// </summary>
    public enum WheelMaterial
    {
        ROAD_CSM = 0x191ED,
        ROADAV_CSM = 0x1FCF4,
        ROADP_CSM = 0x1C95F,
        CONC_CSM = 0x1909F,
        CONCS_CSM = 0x1C982,
        ROADC_CSM = 0x1C320,
        RUNOFFROAD_CSM = 0x2DCC1,
        LGROAD_CSM = 0x1FAF9,
        B1ROAD_CSM = 0x1DD03,
        B2ROAD_CSM = 0x1DD7B,
        B3ROAD_CSM = 0x1DDF3,
        DAMROAD1_CSM = 0x244FE,
        COBBLES_CSM = 0x22C56,
        B1COBBLES_CSM = 0x279D4,
        RMBL_CSM = 0x19546,
        RMBM_CSM = 0x195C0,
        RMBH_CSM = 0x1935E,
        H1RMBL_CSM = 0x1E334,
        RUBRMBL_CSM = 0x2395B,
        DRAIN_CSM = 0x1C5B1,
        MARBLES_CSM = 0x231FC,
        GCRETE_CSM = 0x1FB9F,
        ASTRO_CSM = 0x1D284,
        GRASS_CSM = 0x1CE41,
        GRV_CSM = 0x166DB,
        SAND_CSM = 0x191F9,
        DIRT_CSM = 0x19844,
        BDIRT_CSM = 0x1C927,
        RSAND1_CSM = 0x1E39D,
        RSAND2_CSM = 0x1E419,
        TWALL_CSM = 0x1CFF1,
        CWALL_CSM = 0x1C80A,
        GRDR_CSM = 0x1964F,
        FNCE_CSM = 0x18D4D,
        TPRO_CSM = 0x1A09A,
        SWALL_CSM = 0x1CF7A,
        ICE_CSM = 0x158B8,
        CAR_CARBON = 0x1F329,
        CAR_PLASTIC = 0x230AB,
        CAR_STEEL = 0x1C6B1
    }

    /// <summary>
    /// Constants class
    /// </summary>
    public static class Consts
    {
        public static ushort m_expectedPacketVersion = 1;
        public static ushort s_raceInfoVersion = 1;
        public static ushort s_participantRaceStateVersion = 1;
        public static ushort s_participantTelemetryVersion = 2;

        /// <summary>
        /// Checks packet version against expected versions
        /// </summary>
        /// <param name="packetType">UDP packet type</param>
        /// <param name="version">Version to check</param>
        /// <returns>True if version matches expected</returns>
        public static bool CheckPacketVersion(UDPPacketType packetType, ushort version)
        {
            switch (packetType)
            {
                case UDPPacketType.ParticipantRaceState:
                    return version == s_participantRaceStateVersion;
                case UDPPacketType.RaceInfo:
                    return version == s_raceInfoVersion;
                case UDPPacketType.ParticipantVehicleTelemetry:
                    return version == s_participantTelemetryVersion;
                default:
                    return version == 1;
            }
        }
    }

    /// <summary>
    /// UDP packet types
    /// </summary>
    public enum UDPPacketType : byte
    {
        RaceInfo = 0x0,
        ParticipantRaceState = 0x1,
        ParticipantVehicleTelemetry = 0x2,
        SessionStopped = 0x3
    }

    /// <summary>
    /// UDP flag types with Flags attribute
    /// </summary>
    [Flags]
    public enum UDPFlagType : ushort
    {
        ChequeredFlag = 1,
        YellowFlag = 2,
        WhiteFlag = 4,
        BlueFlag = 8
    }

    /// <summary>
    /// UDP race session states
    /// </summary>
    public enum UDPRaceSessionState: byte
    {
        Inactive = 0x0,
        Active = 0x1,
        Complete = 0x2
    }

    /// <summary>
    /// Dynamic size string class
    /// </summary>
    public class DynamicSizeString
    {
        public byte m_len;
        public string m_str;
    }

    /// <summary>
    /// UDP float list class
    /// </summary>
    public class UDPFloatList
    {
        public byte m_len;
        public float[] m_data;
    }

    /// <summary>
    /// 3D vector class
    /// </summary>
    public class UDPVec3
    {
        public float x;
        public float y;
        public float z;
    }

    /// <summary>
    /// 4D quaternion class
    /// </summary>
    public class UDPQuat
    {
        public float x;
        public float y;
        public float z;
        public float w;
    }

    /// <summary>
    /// Race info class
    /// </summary>
    public class UDPRaceInfo
    {
        public byte m_packetType;
        public ushort m_packetVersion;
        public string m_track;
        public string m_layout;
        public string m_season;
        public string m_weather;
        public string m_session;
        public string m_gameMode;
        public float m_layoutLength;
        public float m_duration;
        public float m_overtime;
        public float m_ambientTemperature;
        public float m_trackTemperature;
        public byte m_isLaps;
        public UDPRaceSessionState m_state;
        public byte m_numParticipants;
    }

    /// <summary>
    /// Participant race state class
    /// </summary>
    public class UDPParticipantRaceState
    {
        public byte m_packetType;
        public ushort m_packetVersion;
        public int m_vehicleId;
        public byte m_isPlayer;
        public string m_vehicleName;
        public string m_driverName;
        public string m_liveryId;
        public string m_vehicleClass;
        public int m_racePos;
        public int m_currentLap;
        public float m_currentLapTime;
        public float m_bestLapTime;
        public float m_lapProgress;
        public int m_currentSector;
        public float[] m_currentSectorTimes;
        public float[] m_bestSectorTimes;
        public byte m_inPits;
        public byte m_sessionFinished;
        public byte m_dq;
        public UDPFlagType m_flags;
    }

    /// <summary>
    /// Vehicle telemetry wheel class
    /// </summary>
    public class UDPVehicleTelemetryWheel
    {
        public int m_contactMaterialHash;
        public float m_angVel;
        public float m_linearSpeed;
        public UDPVec3 m_slideLS;
        public UDPVec3 m_forceLS;
        public UDPVec3 m_momentLS;
        public float m_contactRadius;
        public float m_pressure;
        public float m_inclination;
        public float m_slipRatio;
        public float m_slipAngle;
        public UDPVec3 m_tread;
        public float m_carcass;
        public float m_internalAir;
        public float m_wellAir;
        public float m_rim;
        public float m_brake;
        public float m_springStrain;
        public float m_damperVelocity;
        public float m_hubTorque;
        public float m_hubPower;
        public float m_wheelTorque;
        public float m_wheelPower;
    }

    /// <summary>
    /// Vehicle telemetry wheel list class
    /// </summary>
    public class UDPVehicleTelemetryWheelList
    {
        public byte m_len;
        public UDPVehicleTelemetryWheel[] m_data;
    }

    /// <summary>
    /// Vehicle telemetry chassis class
    /// </summary>
    public class UDPVehicleTelemetryChassis
    {
        public UDPVec3 m_posWS;
        public UDPQuat m_quat;
        public UDPVec3 m_angularVelocityWS;
        public UDPVec3 m_angularVelocityLS;
        public UDPVec3 m_velocityWS;
        public UDPVec3 m_velocityLS;
        public UDPVec3 m_accelerationWS;
        public UDPVec3 m_accelerationLS;
        public float m_overallSpeed;
        public float m_forwardSpeed;
        public float m_sideslip;
    }

    /// <summary>
    /// Vehicle telemetry gear class
    /// </summary>
    public class UDPVehicleTelemetryGear
    {
        public float m_upshiftRPM;
        public float m_downshiftRPM;
    }

    /// <summary>
    /// Vehicle telemetry drivetrain class
    /// </summary>
    public class UDPVehicleTelemetryDrivetrain
    {
        public float m_engineRPM;
        public float m_engineRevRatio;
        public float m_engineTorque;
        public float m_enginePower;
        public float m_engineLoad;
        public float m_engineTurboRPM;
        public float m_engineTurboBoostPressure;
        public float m_fuelRemaining;
        public float m_fuelUseRate;
        public float m_engineOilPressure;
        public float m_engineOilTemperature;
        public float m_engineCoolantTemperature;
        public float m_exhaustGasTemperature;
        public float m_motorRPM;
        public float m_batteryRemaining;
        public float m_batteryUseRate;
        public float m_transmissionRPM;
        public float m_gearboxInputRPM;
        public float m_gearboxOutputRPM;
        public float m_gearboxTorque;
        public float m_gearboxPower;
        public float m_gearboxLoadIn;
        public float m_gearboxLoadOut;
        public float m_timeSinceShift;
        public float m_estDrivenSpeed;
        public float m_outputTorque;
        public float m_outputPower;
        public float m_outputEfficiency;
        public byte m_starterActive;
        public byte m_engineRunning;
        public byte m_engineFanRunning;
        public byte m_revLimiterActive;
        public byte m_tractionControlActive;
        public byte m_speedLimiterEnabled;
        public byte m_speedLimiterActive;
        public UDPVehicleTelemetryGear[] m_gear;
    }

    /// <summary>
    /// Vehicle telemetry suspension class
    /// </summary>
    public class UDPVehicleTelemetrySuspension
    {
        public UDPFloatList m_avgLoads;
        public float m_loadBias;
    }

    /// <summary>
    /// Vehicle telemetry input class
    /// </summary>
    public class UDPVehicleTelemetryInput
    {
        public float m_steering;
        public float m_accelerator;
        public float m_brake;
        public float m_clutch;
        public float m_handbrake;
        public int m_gear;
    }

    /// <summary>
    /// Vehicle telemetry setup class
    /// </summary>
    public class UDPVehicleTelemetrySetup
    {
        public float m_brakeBias;
        public float m_frontAntiRollStiffness;
        public float m_rearAntiRollStiffness;
        public float m_regenLimit;
        public float m_deployLimit;
        public byte m_absLevel;
        public byte m_tcsLevel;
    }

    /// <summary>
    /// Vehicle telemetry general class
    /// </summary>
    public class UDPVehicleTelemetryGeneral
    {
        public UDPVec3 m_centerOfGravity;
        public float m_steeringWheelAngle;
        public float m_totalMass;
        public float m_drivenWheelAngVel;
        public float m_nonDrivenWheelAngVel;
        public float m_estRollingSpeed;
        public float m_estLinearSpeed;
        public float m_totalBrakeForce;
        public byte m_absActive;
    }

    /// <summary>
    /// Vehicle telemetry constant class
    /// </summary>
    public class UDPVehicleTelemetryConstant
    {
        public UDPVec3 m_chassisBBMin;
        public UDPVec3 m_chassisBBMax;
        public float m_starterIdleRPM;
        public float m_engineTorquePeakRPM;
        public float m_enginePowerPeakRPM;
        public float m_engineMaxRPM;
        public float m_engineMaxTorque;
        public float m_engineMaxPower;
        public float m_engineMaxBoost;
        public float m_fuelCapacity;
        public float m_batteryCapacity;
        public float m_trackWidthFront;
        public float m_trackWidthRear;
        public float m_wheelbase;
        public byte m_numberOfWheels;
        public byte m_numberOfForwardGears;
        public byte m_numberOfReverseGears;
        public byte m_isHybrid;
    }

    /// <summary>
    /// Vehicle telemetry class
    /// </summary>
    public class UDPVehicleTelemetry
    {
        public byte m_packetType;
        public ushort m_packetVersion;
        public int m_vehicleId;
        public UDPVehicleTelemetryWheel[] m_wheels;
        public UDPVehicleTelemetryChassis m_chassis;
        public UDPVehicleTelemetryDrivetrain m_drivetrain;
        public UDPVehicleTelemetrySuspension m_suspension;
        public UDPVehicleTelemetryInput m_input;
        public UDPVehicleTelemetrySetup m_setup;
        public UDPVehicleTelemetryGeneral m_general;
        public UDPVehicleTelemetryConstant m_constant;
    }

    public static class MemoryReader
    {
        // Helper methods for reading basic types
        public static byte ReadByte(IntPtr ptr) => Marshal.ReadByte(ptr);
        public static ushort ReadUShort(IntPtr ptr) => BitConverter.ToUInt16(Marshal.PtrToStructure<byte[]>(ptr), 0);
        public static int ReadInt(IntPtr ptr) => BitConverter.ToInt32(Marshal.PtrToStructure<byte[]>(ptr), 0);
        public static float ReadFloat(IntPtr ptr) => BitConverter.ToSingle(Marshal.PtrToStructure<byte[]>(ptr), 0);
        public static string ReadString(IntPtr ptr, int length) => Marshal.PtrToStringAnsi(ptr, length);

        // Read DynamicSizeString
        public static DynamicSizeString ReadDynamicSizeString(IntPtr ptr, long offset)
        {
            var result = new DynamicSizeString();
            IntPtr address = new IntPtr(ptr.ToInt64() + offset);

            result.m_len = ReadByte(address);
            address = new IntPtr(address.ToInt64() + 1);

            result.m_str = ReadString(address, result.m_len);

            return result;
        }

        // Read UDPFloatList
        public static UDPFloatList ReadUDPFloatList(IntPtr ptr, long offset)
        {
            var result = new UDPFloatList();
            IntPtr address = new IntPtr(ptr.ToInt64() + offset);

            result.m_len = ReadByte(address);
            address = new IntPtr(address.ToInt64() + 1);

            result.m_data = new float[result.m_len];
            for (int i = 0; i < result.m_len; i++)
            {
                result.m_data[i] = ReadFloat(new IntPtr(address.ToInt64() + i * 4));
            }

            return result;
        }

        // Read UDPVec3
        public static UDPVec3 ReadUDPVec3(IntPtr ptr, long offset)
        {
            var result = new UDPVec3();
            IntPtr address = new IntPtr(ptr.ToInt64() + offset);

            result.x = ReadFloat(address);
            result.y = ReadFloat(new IntPtr(address.ToInt64() + 4));
            result.z = ReadFloat(new IntPtr(address.ToInt64() + 8));

            return result;
        }

        // Read UDPQuat
        public static UDPQuat ReadUDPQuat(IntPtr ptr, long offset)
        {
            var result = new UDPQuat();
            IntPtr address = new IntPtr(ptr.ToInt64() + offset);

            result.x = ReadFloat(address);
            result.y = ReadFloat(new IntPtr(address.ToInt64() + 4));
            result.z = ReadFloat(new IntPtr(address.ToInt64() + 8));
            result.w = ReadFloat(new IntPtr(address.ToInt64() + 12));

            return result;
        }

        // Read UDPRaceInfo
        public static UDPRaceInfo ReadUDPRaceInfo(IntPtr ptr, long offset)
        {
            var result = new UDPRaceInfo();
            IntPtr address = new IntPtr(ptr.ToInt64() + offset);

            result.m_packetType = ReadByte(address);
            address = new IntPtr(address.ToInt64() + 1);

            result.m_packetVersion = ReadUShort(address);
            address = new IntPtr(address.ToInt64() + 2);

            result.m_track = ReadDynamicSizeString(ptr, address.ToInt64()).m_str;
            address = new IntPtr(address.ToInt64() + (result.m_track.Length));

            result.m_layout = ReadDynamicSizeString(ptr, address.ToInt64()).m_str;
            address = new IntPtr(address.ToInt64() + (result.m_layout.Length));

            result.m_season = ReadDynamicSizeString(ptr, address.ToInt64()).m_str;
            address = new IntPtr(address.ToInt64() + (result.m_season.Length));

            result.m_weather = ReadDynamicSizeString(ptr, address.ToInt64()).m_str;
            address = new IntPtr(address.ToInt64() + (result.m_weather.Length));

            result.m_session = ReadDynamicSizeString(ptr, address.ToInt64()).m_str;
            address = new IntPtr(address.ToInt64() + (result.m_session.Length));

            result.m_gameMode = ReadDynamicSizeString(ptr, address.ToInt64()).m_str;
            address = new IntPtr(address.ToInt64() + (result.m_gameMode.Length));

            result.m_layoutLength = ReadFloat(address);
            address = new IntPtr(address.ToInt64() + 4);

            result.m_duration = ReadFloat(address);
            address = new IntPtr(address.ToInt64() + 4);

            result.m_overtime = ReadFloat(address);
            address = new IntPtr(address.ToInt64() + 4);

            result.m_ambientTemperature = ReadFloat(address);
            address = new IntPtr(address.ToInt64() + 4);

            result.m_trackTemperature = ReadFloat(address);
            address = new IntPtr(address.ToInt64() + 4);

            result.m_isLaps = ReadByte(address);
            address = new IntPtr(address.ToInt64() + 1);

            result.m_state = (UDPRaceSessionState)ReadByte(address);
            address = new IntPtr(address.ToInt64() + 1);

            result.m_numParticipants = ReadByte(address);
            address = new IntPtr(address.ToInt64() + 1);

            return result;
        }

        // Read UDPParticipantRaceState
        public static UDPParticipantRaceState ReadUDPParticipantRaceState(IntPtr ptr, long offset)
        {
            var result = new UDPParticipantRaceState();
            IntPtr address = new IntPtr(ptr.ToInt64() + offset);

            result.m_packetType = ReadByte(address);
            address = new IntPtr(address.ToInt64() + 1);

            result.m_packetVersion = ReadUShort(address);
            address = new IntPtr(address.ToInt64() + 2);

            result.m_vehicleId = ReadInt(address);
            address = new IntPtr(address.ToInt64() + 4);

            result.m_isPlayer = ReadByte(address);
            address = new IntPtr(address.ToInt64() + 1);

            result.m_vehicleName = ReadDynamicSizeString(ptr, address.ToInt64()).m_str;
            address = new IntPtr(address.ToInt64() + (result.m_vehicleName.Length));

            result.m_driverName = ReadDynamicSizeString(ptr, address.ToInt64()).m_str;
            address = new IntPtr(address.ToInt64() + (result.m_driverName.Length));

            result.m_liveryId = ReadDynamicSizeString(ptr, address.ToInt64()).m_str;
            address = new IntPtr(address.ToInt64() + (result.m_liveryId.Length));

            result.m_vehicleClass = ReadDynamicSizeString(ptr, address.ToInt64()).m_str;
            address = new IntPtr(address.ToInt64() + (result.m_vehicleClass.Length));

            result.m_racePos = ReadInt(address);
            address = new IntPtr(address.ToInt64() + 4);

            result.m_currentLap = ReadInt(address);
            address = new IntPtr(address.ToInt64() + 4);

            result.m_currentLapTime = ReadFloat(address);
            address = new IntPtr(address.ToInt64() + 4);

            result.m_bestLapTime = ReadFloat(address);
            address = new IntPtr(address.ToInt64() + 4);

            result.m_lapProgress = ReadFloat(address);
            address = new IntPtr(address.ToInt64() + 4);

            result.m_currentSector = ReadInt(address);
            address = new IntPtr(address.ToInt64() + 4);

            result.m_currentSectorTimes = new float[ReadUShort(address)];
            address = new IntPtr(address.ToInt64() + 2);

            for (int i = 0; i < result.m_currentSectorTimes.Length; i++)
            {
                result.m_currentSectorTimes[i] = ReadFloat(address);
                address = new IntPtr(address.ToInt64() + 4);
            }

            result.m_bestSectorTimes = new float[ReadUShort(address)];
            address = new IntPtr(address.ToInt64() + 2);

            for (int i = 0; i < result.m_bestSectorTimes.Length; i++)
            {
                result.m_bestSectorTimes[i] = ReadFloat(address);
                address = new IntPtr(address.ToInt64() + 4);
            }

            result.m_inPits = ReadByte(address);
            address = new IntPtr(address.ToInt64() + 1);

            result.m_sessionFinished = ReadByte(address);
            address = new IntPtr(address.ToInt64() + 1);

            result.m_dq = ReadByte(address);
            address = new IntPtr(address.ToInt64() + 1);

            result.m_flags = (UDPFlagType)ReadUShort(address);
            //address = new IntPtr(address.ToInt64() + 2);

            return result;
        }

        // Read UDPVehicleTelemetryWheel
        public static UDPVehicleTelemetryWheel ReadUDPVehicleTelemetryWheel(IntPtr ptr, long offset)
        {
            var result = new UDPVehicleTelemetryWheel();
            IntPtr address = new IntPtr(ptr.ToInt64() + offset);

            result.m_contactMaterialHash = ReadInt(address);
            address = new IntPtr(address.ToInt64() + 4);

            result.m_angVel = ReadFloat(address);
            address = new IntPtr(address.ToInt64() + 4);

            result.m_linearSpeed = ReadFloat(address);
            address = new IntPtr(address.ToInt64() + 4);

            result.m_slideLS = ReadUDPVec3(ptr, address.ToInt64());
            address = new IntPtr(address.ToInt64() + 12);

            result.m_forceLS = ReadUDPVec3(ptr, address.ToInt64());
            address = new IntPtr(address.ToInt64() + 12);

            result.m_momentLS = ReadUDPVec3(ptr, address.ToInt64());
            address = new IntPtr(address.ToInt64() + 12);

            result.m_contactRadius = ReadFloat(address);
            address = new IntPtr(address.ToInt64() + 4);

            result.m_pressure = ReadFloat(address);
            address = new IntPtr(address.ToInt64() + 4);

            result.m_inclination = ReadFloat(address);
            address = new IntPtr(address.ToInt64() + 4);

            result.m_slipRatio = ReadFloat(address);
            address = new IntPtr(address.ToInt64() + 4);

            result.m_slipAngle = ReadFloat(address);
            address = new IntPtr(address.ToInt64() + 4);

            result.m_tread = ReadUDPVec3(ptr, address.ToInt64());
            address = new IntPtr(address.ToInt64() + 12);

            result.m_carcass = ReadFloat(address);
            address = new IntPtr(address.ToInt64() + 4);

            result.m_internalAir = ReadFloat(address);
            address = new IntPtr(address.ToInt64() + 4);

            result.m_wellAir = ReadFloat(address);
            address = new IntPtr(address.ToInt64() + 4);

            result.m_rim = ReadFloat(address);
            address = new IntPtr(address.ToInt64() + 4);

            result.m_brake = ReadFloat(address);
            address = new IntPtr(address.ToInt64() + 4);

            result.m_springStrain = ReadFloat(address);
            address = new IntPtr(address.ToInt64() + 4);

            result.m_damperVelocity = ReadFloat(address);
            address = new IntPtr(address.ToInt64() + 4);

            result.m_hubTorque = ReadFloat(address);
            address = new IntPtr(address.ToInt64() + 4);

            result.m_hubPower = ReadFloat(address);
            address = new IntPtr(address.ToInt64() + 4);

            result.m_wheelTorque = ReadFloat(address);
            address = new IntPtr(address.ToInt64() + 4);

            result.m_wheelPower = ReadFloat(address);
            address = new IntPtr(address.ToInt64() + 4);

            return result;
        }

        // Read UDPVehicleTelemetryWheelList
        public static UDPVehicleTelemetryWheelList ReadUDPVehicleTelemetryWheelList(IntPtr ptr, long offset)
        {
            var result = new UDPVehicleTelemetryWheelList();
            IntPtr address = new IntPtr(ptr.ToInt64() + offset);

            result.m_len = ReadByte(address);
            address = new IntPtr(address.ToInt64() + 1);

            result.m_data = new UDPVehicleTelemetryWheel[result.m_len];
            for (int i = 0; i < result.m_len; i++)
            {
                result.m_data[i] = ReadUDPVehicleTelemetryWheel(ptr, address.ToInt64());
                address = new IntPtr(address.ToInt64() + (i + 1) * 64); // Assuming 64 bytes per element
            }

            return result;
        }

        // Read UDPVehicleTelemetryChassis
        public static UDPVehicleTelemetryChassis ReadUDPVehicleTelemetryChassis(IntPtr ptr, long offset)
        {
            var result = new UDPVehicleTelemetryChassis();
            IntPtr address = new IntPtr(ptr.ToInt64() + offset);

            result.m_posWS = ReadUDPVec3(ptr, address.ToInt64());
            address = new IntPtr(address.ToInt64() + 12);

            result.m_quat = ReadUDPQuat(ptr, address.ToInt64());
            address = new IntPtr(address.ToInt64() + 16);

            result.m_angularVelocityWS = ReadUDPVec3(ptr, address.ToInt64());
            address = new IntPtr(address.ToInt64() + 12);

            result.m_angularVelocityLS = ReadUDPVec3(ptr, address.ToInt64());
            address = new IntPtr(address.ToInt64() + 12);

            result.m_velocityWS = ReadUDPVec3(ptr, address.ToInt64());
            address = new IntPtr(address.ToInt64() + 12);

            result.m_velocityLS = ReadUDPVec3(ptr, address.ToInt64());
            address = new IntPtr(address.ToInt64() + 12);

            result.m_accelerationWS = ReadUDPVec3(ptr, address.ToInt64());
            address = new IntPtr(address.ToInt64() + 12);

            result.m_accelerationLS = ReadUDPVec3(ptr, address.ToInt64());
            address = new IntPtr(address.ToInt64() + 12);

            result.m_overallSpeed = ReadFloat(address);
            address = new IntPtr(address.ToInt64() + 4);

            result.m_forwardSpeed = ReadFloat(address);
            address = new IntPtr(address.ToInt64() + 4);

            result.m_sideslip = ReadFloat(address);
            //address = new IntPtr(address.ToInt64() + 4);

            return result;
        }

        // Read UDPVehicleTelemetryGear
        public static UDPVehicleTelemetryGear ReadUDPVehicleTelemetryGear(IntPtr ptr, long offset)
        {
            var result = new UDPVehicleTelemetryGear();
            IntPtr address = new IntPtr(ptr.ToInt64() + offset);

            result.m_upshiftRPM = ReadFloat(address);
            address = new IntPtr(address.ToInt64() + 4);

            result.m_downshiftRPM = ReadFloat(address);
            //address = new IntPtr(address.ToInt64() + 4);

            return result;
        }

        // Read UDPVehicleTelemetryDrivetrain
        public static UDPVehicleTelemetryDrivetrain ReadUDPVehicleTelemetryDrivetrain(IntPtr ptr, long offset)
        {
            var result = new UDPVehicleTelemetryDrivetrain();
            IntPtr address = new IntPtr(ptr.ToInt64() + offset);

            result.m_engineRPM = ReadFloat(address);
            address = new IntPtr(address.ToInt64() + 4);

            result.m_engineRevRatio = ReadFloat(address);
            address = new IntPtr(address.ToInt64() + 4);

            result.m_engineTorque = ReadFloat(address);
            address = new IntPtr(address.ToInt64() + 4);

            result.m_enginePower = ReadFloat(address);
            address = new IntPtr(address.ToInt64() + 4);

            result.m_engineLoad = ReadFloat(address);
            address = new IntPtr(address.ToInt64() + 4);

            result.m_engineTurboRPM = ReadFloat(address);
            address = new IntPtr(address.ToInt64() + 4);

            result.m_engineTurboBoostPressure = ReadFloat(address);
            address = new IntPtr(address.ToInt64() + 4);

            result.m_fuelRemaining = ReadFloat(address);
            address = new IntPtr(address.ToInt64() + 4);

            result.m_fuelUseRate = ReadFloat(address);
            address = new IntPtr(address.ToInt64() + 4);

            result.m_engineOilPressure = ReadFloat(address);
            address = new IntPtr(address.ToInt64() + 4);

            result.m_engineOilTemperature = ReadFloat(address);
            address = new IntPtr(address.ToInt64() + 4);

            result.m_engineCoolantTemperature = ReadFloat(address);
            address = new IntPtr(address.ToInt64() + 4);

            result.m_exhaustGasTemperature = ReadFloat(address);
            address = new IntPtr(address.ToInt64() + 4);

            result.m_motorRPM = ReadFloat(address);
            address = new IntPtr(address.ToInt64() + 4);

            result.m_batteryRemaining = ReadFloat(address);
            address = new IntPtr(address.ToInt64() + 4);

            result.m_batteryUseRate = ReadFloat(address);
            address = new IntPtr(address.ToInt64() + 4);

            result.m_transmissionRPM = ReadFloat(address);
            address = new IntPtr(address.ToInt64() + 4);

            result.m_gearboxInputRPM = ReadFloat(address);
            address = new IntPtr(address.ToInt64() + 4);

            result.m_gearboxOutputRPM = ReadFloat(address);
            address = new IntPtr(address.ToInt64() + 4);

            result.m_gearboxTorque = ReadFloat(address);
            address = new IntPtr(address.ToInt64() + 4);

            result.m_gearboxPower = ReadFloat(address);
            address = new IntPtr(address.ToInt64() + 4);

            result.m_gearboxLoadIn = ReadFloat(address);
            address = new IntPtr(address.ToInt64() + 4);

            result.m_gearboxLoadOut = ReadFloat(address);
            address = new IntPtr(address.ToInt64() + 4);

            result.m_timeSinceShift = ReadFloat(address);
            address = new IntPtr(address.ToInt64() + 4);

            result.m_estDrivenSpeed = ReadFloat(address);
            address = new IntPtr(address.ToInt64() + 4);

            result.m_outputTorque = ReadFloat(address);
            address = new IntPtr(address.ToInt64() + 4);

            result.m_outputPower = ReadFloat(address);
            address = new IntPtr(address.ToInt64() + 4);

            result.m_outputEfficiency = ReadFloat(address);
            address = new IntPtr(address.ToInt64() + 4);

            result.m_starterActive = ReadByte(address);
            address = new IntPtr(address.ToInt64() + 1);

            result.m_engineRunning = ReadByte(address);
            address = new IntPtr(address.ToInt64() + 1);

            result.m_engineFanRunning = ReadByte(address);
            address = new IntPtr(address.ToInt64() + 1);

            result.m_revLimiterActive = ReadByte(address);
            address = new IntPtr(address.ToInt64() + 1);

            result.m_tractionControlActive = ReadByte(address);
            address = new IntPtr(address.ToInt64() + 1);

            result.m_speedLimiterEnabled = ReadByte(address);
            address = new IntPtr(address.ToInt64() + 1);

            result.m_speedLimiterActive = ReadByte(address);
            address = new IntPtr(address.ToInt64() + 1);

            result.m_gear = new UDPVehicleTelemetryGear[ReadByte(address)];
            address = new IntPtr(address.ToInt64() + 1);

            for (int i = 0; i < result.m_gear.Length; i++)
            {
                result.m_gear[i] = ReadUDPVehicleTelemetryGear(ptr, address.ToInt64());
                address = new IntPtr(address.ToInt64() + 16); // Assuming 16 bytes per element
            }

            return result;
        }

        // Read UDPVehicleTelemetrySuspension
        public static UDPVehicleTelemetrySuspension ReadUDPVehicleTelemetrySuspension(IntPtr ptr, long offset)
        {
            var result = new UDPVehicleTelemetrySuspension();
            IntPtr address = new IntPtr(ptr.ToInt64() + offset);

            result.m_avgLoads = ReadUDPFloatList(ptr, address.ToInt64());
            address = new IntPtr(address.ToInt64() + (result.m_avgLoads.m_len * 4));

            result.m_loadBias = ReadFloat(address);
            //address = new IntPtr(address.ToInt64() + 4);

            return result;
        }

        // Read UDPVehicleTelemetryInput
        public static UDPVehicleTelemetryInput ReadUDPVehicleTelemetryInput(IntPtr ptr, long offset)
        {
            var result = new UDPVehicleTelemetryInput();
            IntPtr address = new IntPtr(ptr.ToInt64() + offset);

            result.m_steering = ReadFloat(address);
            address = new IntPtr(address.ToInt64() + 4);

            result.m_accelerator = ReadFloat(address);
            address = new IntPtr(address.ToInt64() + 4);

            result.m_brake = ReadFloat(address);
            address = new IntPtr(address.ToInt64() + 4);

            result.m_clutch = ReadFloat(address);
            address = new IntPtr(address.ToInt64() + 4);

            result.m_handbrake = ReadFloat(address);
            address = new IntPtr(address.ToInt64() + 4);

            result.m_gear = ReadInt(address);
            //address = new IntPtr(address.ToInt64() + 4);

            return result;
        }

        // Read UDPVehicleTelemetrySetup
        public static UDPVehicleTelemetrySetup ReadUDPVehicleTelemetrySetup(IntPtr ptr, long offset)
        {
            var result = new UDPVehicleTelemetrySetup();
            IntPtr address = new IntPtr(ptr.ToInt64() + offset);

            result.m_brakeBias = ReadFloat(address);
            address = new IntPtr(address.ToInt64() + 4);

            result.m_frontAntiRollStiffness = ReadFloat(address);
            address = new IntPtr(address.ToInt64() + 4);

            result.m_rearAntiRollStiffness = ReadFloat(address);
            address = new IntPtr(address.ToInt64() + 4);

            result.m_regenLimit = ReadFloat(address);
            address = new IntPtr(address.ToInt64() + 4);

            result.m_deployLimit = ReadFloat(address);
            address = new IntPtr(address.ToInt64() + 4);

            result.m_absLevel = ReadByte(address);
            address = new IntPtr(address.ToInt64() + 1);

            result.m_tcsLevel = ReadByte(address);
            //address = new IntPtr(address.ToInt64() + 1);

            return result;
        }

        // Read UDPVehicleTelemetryGeneral
        public static UDPVehicleTelemetryGeneral ReadUDPVehicleTelemetryGeneral(IntPtr ptr, long offset)
        {
            var result = new UDPVehicleTelemetryGeneral();
            IntPtr address = new IntPtr(ptr.ToInt64() + offset);

            result.m_centerOfGravity = ReadUDPVec3(ptr, address.ToInt64());
            address = new IntPtr(address.ToInt64() + 12);

            result.m_steeringWheelAngle = ReadFloat(address);
            address = new IntPtr(address.ToInt64() + 4);

            result.m_totalMass = ReadFloat(address);
            address = new IntPtr(address.ToInt64() + 4);

            result.m_drivenWheelAngVel = ReadFloat(address);
            address = new IntPtr(address.ToInt64() + 4);

            result.m_nonDrivenWheelAngVel = ReadFloat(address);
            address = new IntPtr(address.ToInt64() + 4);

            result.m_estRollingSpeed = ReadFloat(address);
            address = new IntPtr(address.ToInt64() + 4);

            result.m_estLinearSpeed = ReadFloat(address);
            address = new IntPtr(address.ToInt64() + 4);

            result.m_totalBrakeForce = ReadFloat(address);
            address = new IntPtr(address.ToInt64() + 4);

            result.m_absActive = ReadByte(address);
            //address = new IntPtr(address.ToInt64() + 1);

            return result;
        }

        // Read UDPVehicleTelemetryConstant
        public static UDPVehicleTelemetryConstant ReadUDPVehicleTelemetryConstant(IntPtr ptr, long offset)
        {
            var result = new UDPVehicleTelemetryConstant();
            IntPtr address = new IntPtr(ptr.ToInt64() + offset);

            result.m_chassisBBMin = ReadUDPVec3(ptr, address.ToInt64());
            address = new IntPtr(address.ToInt64() + 12);

            result.m_chassisBBMax = ReadUDPVec3(ptr, address.ToInt64());
            address = new IntPtr(address.ToInt64() + 12);

            result.m_starterIdleRPM = ReadFloat(address);
            address = new IntPtr(address.ToInt64() + 4);

            result.m_engineTorquePeakRPM = ReadFloat(address);
            address = new IntPtr(address.ToInt64() + 4);

            result.m_enginePowerPeakRPM = ReadFloat(address);
            address = new IntPtr(address.ToInt64() + 4);

            result.m_engineMaxRPM = ReadFloat(address);
            address = new IntPtr(address.ToInt64() + 4);

            result.m_engineMaxTorque = ReadFloat(address);
            address = new IntPtr(address.ToInt64() + 4);

            result.m_engineMaxPower = ReadFloat(address);
            address = new IntPtr(address.ToInt64() + 4);

            result.m_engineMaxBoost = ReadFloat(address);
            address = new IntPtr(address.ToInt64() + 4);

            result.m_fuelCapacity = ReadFloat(address);
            address = new IntPtr(address.ToInt64() + 4);

            result.m_batteryCapacity = ReadFloat(address);
            address = new IntPtr(address.ToInt64() + 4);

            result.m_trackWidthFront = ReadFloat(address);
            address = new IntPtr(address.ToInt64() + 4);

            result.m_trackWidthRear = ReadFloat(address);
            address = new IntPtr(address.ToInt64() + 4);

            result.m_wheelbase = ReadFloat(address);
            address = new IntPtr(address.ToInt64() + 4);

            result.m_numberOfWheels = ReadByte(address);
            address = new IntPtr(address.ToInt64() + 1);

            result.m_numberOfForwardGears = ReadByte(address);
            address = new IntPtr(address.ToInt64() + 1);

            result.m_numberOfReverseGears = ReadByte(address);
            address = new IntPtr(address.ToInt64() + 1);

            result.m_isHybrid = ReadByte(address);
            //address = new IntPtr(address.ToInt64() + 1);

            return result;
        }

        // Read UDPVehicleTelemetry
        public static UDPVehicleTelemetry ReadUDPVehicleTelemetry(IntPtr ptr, long offset)
        {
            var result = new UDPVehicleTelemetry();
            IntPtr address = new IntPtr(ptr.ToInt64() + offset);

            result.m_packetType = ReadByte(address);
            address = new IntPtr(address.ToInt64() + 1);

            result.m_packetVersion = ReadUShort(address);
            address = new IntPtr(address.ToInt64() + 2);

            result.m_vehicleId = ReadInt(address);
            address = new IntPtr(address.ToInt64() + 4);

            result.m_wheels = ReadUDPVehicleTelemetryWheelList(ptr, address.ToInt64()).m_data;
            address = new IntPtr(address.ToInt64() + (result.m_wheels.Length * 64)); // 64 bytes per element

            result.m_chassis = ReadUDPVehicleTelemetryChassis(ptr, address.ToInt64());
            address = new IntPtr(address.ToInt64() + 64);

            result.m_drivetrain = ReadUDPVehicleTelemetryDrivetrain(ptr, address.ToInt64());
            address = new IntPtr(address.ToInt64() + 128);

            result.m_suspension = ReadUDPVehicleTelemetrySuspension(ptr, address.ToInt64());
            address = new IntPtr(address.ToInt64() + 40);

            result.m_input = ReadUDPVehicleTelemetryInput(ptr, address.ToInt64());
            address = new IntPtr(address.ToInt64() + 24);

            result.m_setup = ReadUDPVehicleTelemetrySetup(ptr, address.ToInt64());
            address = new IntPtr(address.ToInt64() + 24);

            result.m_general = ReadUDPVehicleTelemetryGeneral(ptr, address.ToInt64());
            address = new IntPtr(address.ToInt64() + 40);

            result.m_constant = ReadUDPVehicleTelemetryConstant(ptr, address.ToInt64());
            //address = new IntPtr(address.ToInt64() + 40);

            return result;
        }
    }
}