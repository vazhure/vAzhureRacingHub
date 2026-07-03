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

	static void QuaternionToMatrix(const xsim::Quaternion& q, xsim::Vector3F& outX, xsim::Vector3F& outY, xsim::Vector3F& outZ)
	{
		double x = q.m_X;
		double y = q.m_Y;
		double z = q.m_Z;
		double w = q.m_W;

		double xx = x * x;
		double yy = y * y;
		double zz = z * z;
		double xy = x * y;
		double xz = x * z;
		double yz = y * z;
		double wx = w * x;
		double wy = w * y;
		double wz = w * z;

		// Row 0 (right vector / local X axis in world space)
		outX.m_X = 1.0 - 2.0 * (yy + zz);
		outX.m_Y = 2.0 * (xy + wz);
		outX.m_Z = 2.0 * (xz - wy);

		// Row 1 (up vector / local Y axis in world space)
		outY.m_X = 2.0 * (xy - wz);
		outY.m_Y = 1.0 - 2.0 * (xx + zz);
		outY.m_Z = 2.0 * (yz + wx);

		// Row 2 (forward vector / local Z axis in world space)
		outZ.m_X = 2.0 * (xz + wy);
		outZ.m_Y = 2.0 * (yz - wx);
		outZ.m_Z = 1.0 - 2.0 * (xx + yy);
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

		// Linear velocity (world space)
		const auto& linVel = telemetry.m_LinearVelocity;
		data.VelX = linVel.m_X;
		data.VelY = linVel.m_Y;
		data.VelZ = linVel.m_Z;

		// Linear acceleration (world space from eXpanSIM)
		const auto& linAcc = telemetry.m_LinearAcceleration;
		double worldAccX = linAcc.m_X;
		double worldAccY = linAcc.m_Y;
		double worldAccZ = linAcc.m_Z;

		// Convert quaternion to orientation matrix
		xsim::Vector3F oriX, oriY, oriZ;
		QuaternionToMatrix(transform.m_rot, oriX, oriY, oriZ);

		// Transform world acceleration to local vehicle space
		// Local axis = dot(worldAcc, worldAxis)
		double localAccX = worldAccX * oriX.m_X + worldAccY * oriX.m_Y + worldAccZ * oriX.m_Z;
		double localAccY = worldAccX * oriY.m_X + worldAccY * oriY.m_Y + worldAccZ * oriY.m_Z;
		double localAccZ = worldAccX * oriZ.m_X + worldAccY * oriZ.m_Y + worldAccZ * oriZ.m_Z;

		// Map to motion platform axes
		// eXpanSIM local coords: +X=left, +Y=up, +Z=back
		// vAzhureRacingHub: Sway=lateral(X), Heave=vertical(Y), Surge=longitudinal(Z)
		data.AccX = localAccX;   // Sway (lateral)
		data.AccY = localAccY;   // Heave (vertical)
		data.AccZ = -localAccZ;  // Surge (longitudinal, inverted because +Z is back)

		// Angular velocity (already in local space in eXpanSIM)
		const auto& angVel = telemetry.m_AngularVelocity;
		data.AngVelX = angVel.m_X;
		data.AngVelY = angVel.m_Y;
		data.AngVelZ = angVel.m_Z;

		// Angular acceleration (already in local space in eXpanSIM)
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

		data.Gear = m_DashboardCache.Gear;

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
		m_DashboardCache.Gear = dashboard.m_Gear;
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
