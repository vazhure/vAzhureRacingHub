// eXpanSIM Telemetry Bridge - Shared Memory Implementation

#include "pch.hpp"
#include "SharedMemoryTelemetry.hpp"

namespace expansim_bridge
{
    SharedMemoryTelemetry::SharedMemoryTelemetry()
    {
    }

    SharedMemoryTelemetry::~SharedMemoryTelemetry()
    {
        Shutdown();
    }

    bool SharedMemoryTelemetry::Initialize()
    {
        if (m_Initialized)
            return true;

        // Create shared memory file mapping
        m_hMapFile = CreateFileMappingW(
            INVALID_HANDLE_VALUE,
            nullptr,
            PAGE_READWRITE,
            0,
            SHARED_MEMORY_SIZE,
            SHARED_MEMORY_NAME
        );

        if (!m_hMapFile)
        {
            // If already exists, open it
            if (GetLastError() == ERROR_ALREADY_EXISTS)
            {
                m_hMapFile = OpenFileMappingW(FILE_MAP_ALL_ACCESS, FALSE, SHARED_MEMORY_NAME);
                if (!m_hMapFile)
                {
                    OutputDebugStringW(L"[eXpanSIM Bridge] Failed to open existing file mapping\n");
                    return false;
                }
            }
            else
            {
                OutputDebugStringW(L"[eXpanSIM Bridge] Failed to create file mapping\n");
                return false;
            }
        }

        m_pBuf = MapViewOfFile(m_hMapFile, FILE_MAP_ALL_ACCESS, 0, 0, SHARED_MEMORY_SIZE);
        if (!m_pBuf)
        {
            OutputDebugStringW(L"[eXpanSIM Bridge] Failed to map view of file\n");
            CloseHandle(m_hMapFile);
            m_hMapFile = nullptr;
            return false;
        }

        // Initialize with zeros
        memset(m_pBuf, 0, SHARED_MEMORY_SIZE);

        // Write version
        auto* data = static_cast<TelemetryData*>(m_pBuf);
        data->Version = TELEMETRY_VERSION;

        m_Initialized = true;
        OutputDebugStringW(L"[eXpanSIM Bridge] Shared memory initialized\n");
        return true;
    }

    void SharedMemoryTelemetry::Shutdown()
    {
        if (!m_Initialized)
            return;

        // Zero-out buffer before shutdown
        if (m_pBuf)
        {
            memset(m_pBuf, 0, SHARED_MEMORY_SIZE);
        }

        if (m_pBuf)
        {
            UnmapViewOfFile(m_pBuf);
            m_pBuf = nullptr;
        }

        if (m_hMapFile)
        {
            CloseHandle(m_hMapFile);
            m_hMapFile = nullptr;
        }

        m_Initialized = false;
        OutputDebugStringW(L"[eXpanSIM Bridge] Shared memory shutdown\n");
    }

    void SharedMemoryTelemetry::WriteTelemetry(const TelemetryData& telemetry)
    {
        if (!m_Initialized || !m_pBuf)
            return;

        auto* dest = static_cast<TelemetryData*>(m_pBuf);
        *dest = telemetry;
        dest->Version = TELEMETRY_VERSION; // Ensure version is always set
    }
}
