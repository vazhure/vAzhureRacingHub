local SHTelemetry = {}
local SHTelemetrySocket

SHTelemetry.TelemetryIP = "127.0.0.1"
SHTelemetry.TelemetryPort = 10025

local socket
local json = require("json")

function SHTelemetry.onSimulationStart()
	log.write("SH Telemetry", log.INFO, "SH Telemetry export started")

	package.path = package.path .. ";.\\LuaSocket\\?.lua"
	package.path  = package.path..";"..lfs.currentdir().."/Scripts/?.lua"
	package.cpath = package.cpath .. ";.\\LuaSocket\\?.dll"
	socket = require("socket")

	SHTelemetrySocket = socket.udp()
	SHTelemetrySocket:settimeout(0)
	SHTelemetrySocket:setpeername(SHTelemetry.TelemetryIP, SHTelemetry.TelemetryPort)
end

function SHTelemetry.onSimulationStop()
	log.write("SH Telemetry", log.INFO, "SH Telemetry export stopped")

	if SHTelemetrySocket ~= nil then
		if SHTelemetrySocket then
			socket.try(SHTelemetrySocket:send(""))
			SHTelemetrySocket:close()
			SHTelemetrySocket = nil
		end
	end
end

function SHTelemetry.onSimulationFrame()
	if SHTelemetrySocket then
		local telemetry = {}
		
		telemetry.packetTime = socket.gettime() * 1000
		telemetry.selfData = Export.LoGetSelfData()
		telemetry.angularVelocity = Export.LoGetAngularVelocity()
		telemetry.modelTime = Export.LoGetModelTime() -- returns current model time (args - 0, results - 1 (sec))
		telemetry.mechInfo = Export.LoGetMechInfo()
		telemetry.missionStartTime = Export.LoGetMissionStartTime() -- returns mission start time (args - 0, results - 1 (sec))
		telemetry.pilotName = Export.LoGetPilotName() -- (args - 0, results - 1 (text string))
		telemetry.playerPlaneId = Export.LoGetPlayerPlaneId() -- (args - 0, results - 1 (number))
		telemetry.indicatedAirSpeed = Export.LoGetIndicatedAirSpeed() -- (args - 0, results - 1 (m/s))
		telemetry.vectorVelocity = Export.LoGetVectorVelocity()
		telemetry.trueAirSpeed = Export.LoGetTrueAirSpeed() -- (args - 0, results - 1 (m/s))
		telemetry.altitudeAboveSeaLevel = Export.LoGetAltitudeAboveSeaLevel() -- (args - 0, results - 1 (meters))
		telemetry.altitudeAboveGroundLevel = Export.LoGetAltitudeAboveGroundLevel() -- (args - 0, results - 1 (meterst))
		telemetry.angleOfAttack = Export.LoGetAngleOfAttack() -- (args - 0, results - 1 (rad))
		telemetry.accelerationUnits = Export.LoGetAccelerationUnits() -- (args - 0, results - table {x = Nx,y = NY,z = NZ} 1 (G))
		telemetry.verticalVelocity = Export.LoGetVerticalVelocity() -- (args - 0, results - 1(m/s))
		telemetry.machNumber = Export.LoGetMachNumber() -- (args - 0, results - 1)
		telemetry.magneticYaw = Export.LoGetMagneticYaw() -- (args - 0, results - 1 (rad)
		telemetry.glideDeviation = Export.LoGetGlideDeviation() -- (args - 0,results - 1)( -1 < result < 1)
		telemetry.sideDeviation = Export.LoGetSideDeviation() -- (args - 0,results - 1)( -1 < result < 1)
		telemetry.slipBallPosition = Export.LoGetSlipBallPosition() -- (args - 0,results - 1)( -1 < result < 1)
		telemetry.basicAtmospherePressure = Export.LoGetBasicAtmospherePressure() -- (args - 0,results - 1) (mm hg)
		telemetry.controlPanel_HSI = Export.LoGetControlPanel_HSI() -- (args - 0,results - table)
		telemetry.engineInfo = Export.LoGetEngineInfo()
		telemetry.mCPState = Export.LoGetMCPState()
		telemetry.payloadInfo = Export.LoGetPayloadInfo()
		telemetry.shakeAmplitude = Export.LoGetShakeAmplitude()
		telemetry.wingTargets = Export.LoGetWingTargets()
		telemetry.wingInfo = Export.LoGetWingInfo()

		local pitch, bank, yaw = Export.LoGetADIPitchBankYaw() -- (args - 0, results - 3 (rad))
		telemetry.aDIPitch = pitch 
		telemetry.aDIBank = bank 
		telemetry.aDIYaw = yaw 

		telemetry.LeftGear = Export.LoGetAircraftDrawArgumentValue(6)
		telemetry.NoseGear = Export.LoGetAircraftDrawArgumentValue(1)
		telemetry.RightGear = Export.LoGetAircraftDrawArgumentValue(4)
		telemetry.drawGearPos = Export.LoGetAircraftDrawArgumentValue(3)
		telemetry.drawFlapsPos1 = Export.LoGetAircraftDrawArgumentValue(9)
		telemetry.drawFlapsPos2 = Export.LoGetAircraftDrawArgumentValue(10)
		telemetry.drawSpeedBrake = Export.LoGetAircraftDrawArgumentValue(21)
		telemetry.drawRefuelBoom = Export.LoGetAircraftDrawArgumentValue(22)

		local mainPanel = Export.GetDevice(0)

		--log.write("SH Telemetry", log.INFO, encode(telemetry))

		socket.try(SHTelemetrySocket:send(json:encode(telemetry)))
	end
end

-- Register callbacks with DCS to be called at the appropriate moment
DCS.setUserCallbacks(SHTelemetry)
