// eXpanSIM Telemetry Bridge - Shared Memory Output

#pragma once

#include "pch.hpp"

namespace expansim_bridge
{
    // Shared memory name that vAzhureRacingHub client will read from
    constexpr const wchar_t* SHARED_MEMORY_NAME = L"$eXpanSIM_Telemetry$";
    constexpr DWORD SHARED_MEMORY_SIZE = 1024;

    // Version for compatibility checking
    constexpr uint32_t TELEMETRY_VERSION = 1;

#pragma pack(push, 1)
    struct TelemetryData
    {
        uint32_t Version;           // TELEMETRY_VERSION
        uint32_t Sequence;          // Incremented each update
        uint32_t Valid;             // 1 = data valid, 0 = no vehicle spawned

        // Position (world space, meters)
        double PosX;
        double PosY;
        double PosZ;

        // Orientation (radians)
        double Pitch;
        double Yaw;
        double Roll;

        // Linear velocity (m/s)
        double VelX;
        double VelY;
        double VelZ;

        // Linear acceleration (m/s^2)
        double AccX;
        double AccY;
        double AccZ;

        // Angular velocity (rad/s)
        double AngVelX;
        double AngVelY;
        double AngVelZ;

        // Angular acceleration (rad/s^2)
        double AngAccX;
        double AngAccY;
        double AngAccZ;

        // Center of mass offset (meters)
        double CoMX;
        double CoMY;
        double CoMZ;

        // Dashboard data
        double Speed;               // m/s
        double Rpm;
        double MaxRpm;
        double DistanceTraveled;    // meters
        double FuelConsumptionLpH;
        double FuelTankReserveNorm; // 0.0 - 1.0

        // Session state
        uint32_t VehicleSpawned;    // 1 = vehicle active
        uint32_t Reserved[15];      // For future expansion
    };
#pragma pack(pop)

    static_assert(sizeof(TelemetryData) <= SHARED_MEMORY_SIZE, "TelemetryData exceeds shared memory size");

    // Dashboard cache for OnDashboard -> OnTelemetry bridge
    struct DashboardCache
    {
        double Speed = 0.0;
        double Rpm = 0.0;
        double MaxRpm = 0.0;
        double DistanceTraveled = 0.0;
        double FuelConsumptionLpH = 0.0;
        double FuelTankReserveNorm = 0.0;
    };

    class SharedMemoryTelemetry
    {
    public:
        SharedMemoryTelemetry();
        ~SharedMemoryTelemetry();

        bool Initialize();
        void Shutdown();
        void WriteTelemetry(const TelemetryData& data);
        bool IsInitialized() const { return m_Initialized; }

    private:
        HANDLE m_hMapFile = nullptr;
        void* m_pBuf = nullptr;
        bool m_Initialized = false;
    };
}
