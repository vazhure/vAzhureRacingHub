// TelemetryWriter.cpp

#include "pch.hpp"
#include "TelemetryWriter.hpp"

#include <xsim/generated/VehicleSetupInfoV1.hpp>
#include <xsim/generated/RigidTransformV1.hpp>
#include <xsim/generated/BodyTelemetryDataV1.hpp>
#include <xsim/generated/DashboardStateV1.hpp>

namespace plugin
{
	TelemetryWriter::TelemetryWriter()
	{
		m_SharedMem.Initialize();
		sequence = 0;
		spawned = false;
	}

	void TelemetryWriter::Start(const xsim::VehicleSetupInfoV1& vehicleSetup)
	{
		// Initialize shared memory with vehicle info
		expansim_bridge::TelemetryData data{};
		data.Version = expansim_bridge::TELEMETRY_VERSION;
		data.Valid = 1;
		data.VehicleSpawned = 1;
		sequence = 0;
		spawned = true;

		const auto& vehicleConfig = vehicleSetup.m_MotorVehicle.m_Vehicle;
		const auto& bodyConfig = vehicleConfig.m_Body;
		const auto& engineConfig = vehicleSetup.m_MotorVehicle.m_Engine;

		m_DashboardCache.MaxRpm = engineConfig.m_CombustionEngine.m_MaxRpm.Value();

		m_SharedMem.WriteTelemetry(data);
	}

	void TelemetryWriter::WriteTelemetry(
		float dt,
		const xsim::RigidTransformV1& transform,
		const xsim::BodyTelemetryDataV1& telemetry)
	{
		expansim_bridge::TelemetryData data{};
		data.Version = expansim_bridge::TELEMETRY_VERSION;
		data.Valid = 1;
		data.VehicleSpawned = spawned ? 1 : 0;
		data.Sequence = ++sequence;

		// Position from interpolated transform
		const auto& pos = transform.m_pos;
		data.PosX = pos.m_X;
		data.PosY = pos.m_Y;
		data.PosZ = pos.m_Z;

		// Orientation (PitchYawRoll)
		const auto& rot = telemetry.m_PitchYawRoll;
		data.Pitch = rot.m_Pitch;
		data.Yaw = rot.m_Yaw;
		data.Roll = rot.m_Roll;

		// Linear velocity
		const auto& linVel = telemetry.m_LinearVelocity;
		data.VelX = linVel.m_X;
		data.VelY = linVel.m_Y;
		data.VelZ = linVel.m_Z;

		// Linear acceleration
		const auto& linAcc = telemetry.m_LinearAcceleration;
		data.AccX = linAcc.m_X;
		data.AccY = linAcc.m_Y;
		data.AccZ = linAcc.m_Z;

		// Angular velocity
		const auto& angVel = telemetry.m_AngularVelocity;
		data.AngVelX = angVel.m_X;
		data.AngVelY = angVel.m_Y;
		data.AngVelZ = angVel.m_Z;

		// Angular acceleration
		const auto& angAcc = telemetry.m_AngularAcceleration;
		data.AngAccX = angAcc.m_X;
		data.AngAccY = angAcc.m_Y;
		data.AngAccZ = angAcc.m_Z;

		// Center of mass
		const auto& com = telemetry.m_CenterOfMass;
		data.CoMX = com.m_X;
		data.CoMY = com.m_Y;
		data.CoMZ = com.m_Z;

		// Copy cached dashboard data
		data.Speed = m_DashboardCache.Speed;
		data.Rpm = m_DashboardCache.Rpm;
		data.MaxRpm = m_DashboardCache.MaxRpm;
		data.DistanceTraveled = m_DashboardCache.DistanceTraveled;
		data.FuelConsumptionLpH = m_DashboardCache.FuelConsumptionLpH;
		data.FuelTankReserveNorm = m_DashboardCache.FuelTankReserveNorm;

		m_SharedMem.WriteTelemetry(data);
	}

	void TelemetryWriter::WriteDashboard(
		float dt,
		const xsim::DashboardStateV1& dashboard)
	{
		m_DashboardCache.Speed = dashboard.m_Speed;
		m_DashboardCache.Rpm = dashboard.m_Rpm;
		m_DashboardCache.DistanceTraveled = dashboard.m_DistanceTraveled;
		m_DashboardCache.FuelConsumptionLpH = dashboard.m_FuelConsumption.m_LitersPerHour;
		m_DashboardCache.FuelTankReserveNorm = dashboard.m_FuelTankReserveNorm;
	}

	void TelemetryWriter::Reset()
	{
		spawned = false;
		expansim_bridge::TelemetryData data{};
		data.Version = expansim_bridge::TELEMETRY_VERSION;
		data.Valid = 0;
		data.VehicleSpawned = 0;
		m_SharedMem.WriteTelemetry(data);
	}
}
