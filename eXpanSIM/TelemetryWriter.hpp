// TelemetryWriter.hpp
// Replaces TelemetryRecorder - writes to shared memory instead of CSV

#pragma once

#include "pch.hpp"
#include "SharedMemoryTelemetry.hpp"

#include <xsim/generated/VehicleSetupInfoV1.hpp>
#include <xsim/generated/RigidTransformV1.hpp>
#include <xsim/generated/BodyTelemetryDataV1.hpp>
#include <xsim/generated/DashboardStateV1.hpp>

namespace plugin
{
	class TelemetryWriter
	{
	public:
		TelemetryWriter();
		~TelemetryWriter() = default;

		void Start(const xsim::VehicleSetupInfoV1& vehicleSetup);
		void WriteTelemetry(
			float dt,
			const xsim::RigidTransformV1& transform,
			const xsim::BodyTelemetryDataV1& telemetry);
		void WriteDashboard(
			float dt,
			const xsim::DashboardStateV1& dashboard);
		void Reset();

	private:
		expansim_bridge::SharedMemoryTelemetry m_SharedMem;
		expansim_bridge::DashboardCache m_DashboardCache{};
		uint32_t sequence;
		bool spawned;
	};
}
