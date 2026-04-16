------------------------------------------
-- File:                telemetry.lua
-- Description:         Telemetry export for MudRunner (File-based)
--
-- HOW TO USE:
--   Append this code to the END of your truck.lua file.
--   The game only loads truck.lua — standalone files are ignored.
--
-- Transport:           Local file (io.open)
-- Output file:         telemetry_out.txt  (in game working directory)
--
-- SSD wear:            ~300 bytes × 60 Hz = 18 KB/sec = 1.6 GB/day.
--   Same file overwritten each frame (same NTFS sectors).
--   Modern SSD endurance: 600+ TBW. 1.6 GB/day = 0.3% per year.
--   If concerned, change TelemetryUpdateInterval to 0.1 (10 Hz).
--
-- Compatible with: vAzhureRacingAPI MudRunner Plugin
------------------------------------------
-- ============================================================
--  >>>>> COPY EVERYTHING BELOW INTO truck.lua <<<<<<
-- ============================================================
-- ============================================================
-- TELEMETRY MODULE (Fixed-Width Guaranteed)
-- ============================================================
TelemetryFileName = "telemetry_out.txt"
TelemetryUpdateInterval = 0.016
TelemetryScreenDebug = true

telemetryState = {
    enabled = false,
    elapsedTime = 0,
    frameCount = 0,
    lastWriteTime = 0,
    writeIndex = 0,
    smoothSurge = 0,
    smoothSway = 0,
    smoothHeave = 0,
    prevLinVelX = 0,
    prevLinVelY = 0,
    prevLinVelZ = 0,
    fileWriteOk = false,
    fileWriteCount = 0,
    fileWriteErrors = 0,
    lastWasInGame = false
}

local function clamp(v, min, max)
    return v < min and min or (v > max and max or v)
end

local function teleScreen(msg)
    if TelemetryScreenDebug and C_RenderDebugString ~= nil then
        C_RenderDebugString(msg)
    end
end

local function telemetrySmooth(current, previous, alpha)
    return previous + alpha * (current - previous)
end

local function telemetrySendLine(line)
    telemetryState.writeIndex = (telemetryState.writeIndex + 1) % 255
    local file = io.open(TelemetryFileName, "w")
    if file == nil then
        telemetryState.fileWriteOk = false
        telemetryState.fileWriteErrors = telemetryState.fileWriteErrors + 1
        return false
    end
    file:write(line .. "\n")
    file:write(string.char(telemetryState.writeIndex))
    file:flush()
    file:close()
    telemetryState.fileWriteOk = true
    telemetryState.fileWriteCount = telemetryState.fileWriteCount + 1
    return true
end

local function InitTelemetry()
    telemetryState.enabled = true
    telemetryState.frameCount = 0
    telemetryState.lastWriteTime = 0
    telemetryState.writeIndex = 0
    telemetryState.fileWriteOk = false
    if io == nil then return end
    local testFile = io.open(TelemetryFileName, "w")
    if testFile ~= nil then
        testFile:write("TELEMETRY_TEST_OK\n")
        testFile:write(string.char(42))
        testFile:close()
    end
end

local function CalcVehicleOrientation(w1, w2, w3, w4)
    local frontX = (w1.x + w2.x) * 0.5
    local frontZ = (w1.z + w2.z) * 0.5
    local rearX = (w3.x + w4.x) * 0.5
    local rearZ = (w3.z + w4.z) * 0.5

    local wheelbase = math.sqrt((frontX - rearX) ^ 2 + (frontZ - rearZ) ^ 2)
    local frontY = (w1.y + w2.y) * 0.5
    local rearY = (w3.y + w4.y) * 0.5

    local pitchRad = math.atan2(frontY - rearY, wheelbase)
    local pitchDeg = math.deg(pitchRad)

    local leftY = (w1.y + w3.y) * 0.5
    local rightY = (w2.y + w4.y) * 0.5
    local trackWidth = math.sqrt((w1.x - w2.x) ^ 2 + (w1.z - w2.z) ^ 2)
    local rollRad = math.atan2(leftY - rightY, trackWidth)
    local rollDeg = math.deg(rollRad)

    local yawRad = math.atan2(frontX - rearX, frontZ - rearZ)
    local yawDeg = math.deg(yawRad)
    if yawDeg < 0 then yawDeg = yawDeg + 360 end

    return pitchDeg, rollDeg, yawDeg
end

local function getSafeWheelData(wheelsTable, idx)
    local w = wheelsTable and wheelsTable[idx]
    if not w or not w.org then return {x = 0, y = 0, z = 0, r = 0} end
    return {
        x = clamp(w.org.x, -9999.9999, 9999.9999),
        y = clamp(w.org.y, -9999.9999, 9999.9999),
        z = clamp(w.org.z, -9999.9999, 9999.9999),
        r = clamp(w.radius or 0, -9999.9999, 9999.9999)
    }
end

-- ============================================================
--  MAIN TELEMETRY UPDATE (Fixed-Width)
-- ============================================================
local function UpdateTelemetry(truck, truckInput, wheels, elapsedTime)
    if not telemetryState.enabled then return end

    local isInGame = (truck ~= nil)
    if telemetryState.lastWasInGame ~= isInGame then
        telemetryState.lastWasInGame = isInGame
    end

    telemetryState.elapsedTime = telemetryState.elapsedTime + elapsedTime
    if telemetryState.elapsedTime - telemetryState.lastWriteTime <
        TelemetryUpdateInterval then return end

    local dt = telemetryState.elapsedTime - telemetryState.lastWriteTime
    telemetryState.lastWriteTime = telemetryState.elapsedTime
    telemetryState.frameCount = telemetryState.frameCount + 1

    local steerAngle = tonumber(
                           truckInput.steeringAngle or truckInput.steering or 0)
    local throttle = tonumber(truckInput.pedalAccelerate or 0)
    local brake = tonumber(truckInput.pedalBrake or 0)
    local gear = tonumber(truckInput.gear) or 0
    local damageFactor = tonumber(truckInput.damageFactor or 0)
    local handbrake = truckInput.isHandbrake and 1 or 0
    local diffLock = truckInput.isDifferentialLocked and 1 or 0
    local awd = truckInput.isAllWheelDrive and 1 or 0
    local engineRunning = truckInput.isEngineIgnited and 1 or 0
    local headlights = truckInput.isHeadLight and 1 or 0

    local velVec = truckInput.linVelVector or {x = 0, y = 0, z = 0}
    local angVec = truckInput.angVelVector or {x = 0, y = 0, z = 0}
    local linVelX = velVec.x or 0
    local linVelY = velVec.y or 0
    local linVelZ = velVec.z or 0
    local angVelX = angVec.x or 0
    local angVelY = angVec.y or 0
    local angVelZ = angVec.z or 0

    local wheel1 = getSafeWheelData(wheels, 1)
    local wheel2 = getSafeWheelData(wheels, 2)
    local wheel3 = getSafeWheelData(wheels, 3)
    local wheel4 = getSafeWheelData(wheels, 4)

    local engineTension = tonumber(truck.engineTension or 0)
    local turbo = tonumber(truck.turbo or 0)
    local speedKmh = math.sqrt(linVelX * linVelX + linVelY * linVelY + linVelZ *
                                   linVelZ) * 3.6

    local posX, posY, posZ = 0, 0, 0
    if truckInput.hissOrg then
        posX = truckInput.hissOrg.x or 0
        posY = truckInput.hissOrg.y or 0
        posZ = truckInput.hissOrg.z or 0
    end

    local alpha = 0.3
    local accelX, accelY, accelZ = 0, 0, 0
    if dt > 0.001 then
        accelX = (linVelX - telemetryState.prevLinVelX) / dt
        accelY = (linVelY - telemetryState.prevLinVelY) / dt
        accelZ = (linVelZ - telemetryState.prevLinVelZ) / dt
    end

    telemetryState.smoothSurge = telemetrySmooth(accelX,
                                                 telemetryState.smoothSurge,
                                                 alpha)
    telemetryState.smoothSway = telemetrySmooth(accelZ,
                                                telemetryState.smoothSway, alpha)
    telemetryState.smoothHeave = telemetrySmooth(accelY,
                                                 telemetryState.smoothHeave,
                                                 alpha)

    telemetryState.prevLinVelX = linVelX
    telemetryState.prevLinVelY = linVelY
    telemetryState.prevLinVelZ = linVelZ

    local avgSuspCompression = 0
    local wheelCount = 0
    local maxMudDepth = 0
    local maxWaterDepth = 0
    if wheels ~= nil then
        for _, wheel in ipairs(wheels) do
            wheelCount = wheelCount + 1
            if wheel.mudDepth then
                maxMudDepth = math.max(maxMudDepth, wheel.mudDepth)
            end
            if wheel.waterDepth then
                maxWaterDepth = math.max(maxWaterDepth, wheel.waterDepth)
            end
            if wheel.suspStressFactor then
                avgSuspCompression = avgSuspCompression + wheel.suspStressFactor
            end
        end
        if wheelCount > 0 then
            avgSuspCompression = avgSuspCompression / wheelCount
        end
    end

    local estimatedRPM = 600 + engineTension * 5400
    local maxRPM = 6000

    steerAngle = clamp(steerAngle, -100, 100)
    throttle = clamp(throttle, 0, 1)
    brake = clamp(brake, 0, 1)
    gear = clamp(math.floor(gear), -99, 99)
    damageFactor = clamp(damageFactor, 0, 1)
    handbrake = clamp(handbrake, 0, 1)
    diffLock = clamp(diffLock, 0, 1)
    awd = clamp(awd, 0, 1)
    engineRunning = clamp(engineRunning, 0, 1)
    headlights = clamp(headlights, 0, 1)

    linVelX = clamp(linVelX, -9999.9999, 9999.9999)
    linVelY = clamp(linVelY, -9999.9999, 9999.9999)
    linVelZ = clamp(linVelZ, -9999.9999, 9999.9999)
    angVelX = clamp(angVelX, -999.9999, 999.9999)
    angVelY = clamp(angVelY, -999.9999, 999.9999)
    angVelZ = clamp(angVelZ, -999.9999, 999.9999)
    accelX = clamp(accelX, -999.9999, 999.9999)
    accelY = clamp(accelY, -999.9999, 999.9999)
    accelZ = clamp(accelZ, -999.9999, 999.9999)
    telemetryState.smoothSurge = clamp(telemetryState.smoothSurge, -999.9999,
                                       999.9999)
    telemetryState.smoothSway = clamp(telemetryState.smoothSway, -999.9999,
                                      999.9999)
    telemetryState.smoothHeave = clamp(telemetryState.smoothHeave, -999.9999,
                                       999.9999)

    posX = clamp(posX, -999999.99, 999999.99)
    posY = clamp(posY, -999999.99, 999999.99)
    posZ = clamp(posZ, -999999.99, 999999.99)

    estimatedRPM = clamp(estimatedRPM, 0, 99999)
    maxRPM = clamp(maxRPM, 0, 99999)
    engineTension = clamp(engineTension, 0, 99.999)
    turbo = clamp(turbo, 0, 99.999)
    avgSuspCompression = clamp(avgSuspCompression, 0, 9.9999)
    maxMudDepth = clamp(maxMudDepth, 0, 999.99)
    maxWaterDepth = clamp(maxWaterDepth, 0, 999.99)
    wheelCount = clamp(wheelCount, 0, 99)
    speedKmh = clamp(speedKmh, 0, 9999.99)
    telemetryState.elapsedTime = clamp(telemetryState.elapsedTime, 0, 99999.999)

    local pitch, roll, yaw = CalcVehicleOrientation(wheel1, wheel2, wheel3,
                                                    wheel4)

    local line = string.format(
                     "T|%010.3f|%07.2f|%08.4f|%06.3f|%06.3f|%02d|%02d|%02d|%02d|%02d|%02d|" ..
                         "%010.4f|%010.4f|%010.4f|%010.4f|%010.4f|%010.4f|%010.4f|%010.4f|%010.4f|" ..
                         "%010.4f|%010.4f|%010.4f|%012.2f|%012.2f|%012.2f|%05d|%05d|%06.3f|%06.3f|" ..
                         "%06.3f|%07.4f|%07.2f|%07.2f|%02d|" ..
                         "%010.4f|%010.4f|%010.4f|%010.4f|" ..
                         "%010.4f|%010.4f|%010.4f|%010.4f|" ..
                         "%010.4f|%010.4f|%010.4f|%010.4f|" ..
                         "%010.4f|%010.4f|%010.4f|%010.4f|" ..
                         "%010.4f|%010.4f|%010.4f", telemetryState.elapsedTime,
                     speedKmh, steerAngle, throttle, brake, gear, handbrake,
                     diffLock, awd, engineRunning, headlights, linVelX, linVelY,
                     linVelZ, angVelX, angVelY, angVelZ, accelX, accelY, accelZ,
                     telemetryState.smoothSurge, telemetryState.smoothSway,
                     telemetryState.smoothHeave, posX, posY, posZ, estimatedRPM,
                     maxRPM, engineTension, turbo, damageFactor,
                     avgSuspCompression, maxMudDepth, maxWaterDepth, wheelCount,
                     wheel1.x, wheel1.y, wheel1.z, wheel1.r, wheel2.x, wheel2.y,
                     wheel2.z, wheel2.r, wheel3.x, wheel3.y, wheel3.z, wheel3.r,
                     wheel4.x, wheel4.y, wheel4.z, wheel4.r, pitch, roll, yaw)

    telemetrySendLine(line)
end

-- ============================================================
--  HOOK
-- ============================================================
local _originalProcessTruck = ProcessTruck
function ProcessTruck(truck, truckInput, wheels, exhausts, intakes, steams,
                      wave, flows, elapsedTime)
    if _originalProcessTruck ~= nil then
        _originalProcessTruck(truck, truckInput, wheels, exhausts, intakes,
                              steams, wave, flows, elapsedTime)
    end

    if telemetryState.enabled == false then InitTelemetry() end

    if truck.soundEngineIdle ~= nil then
        if truckInput ~= nil and truckInput.isInWorld then
            UpdateTelemetry(truck, truckInput, wheels, elapsedTime)
        end
    end
end

-- ============================================================
--  >>>>> END OF TELEMETRY CODE <<<<<<
-- ============================================================
-- EOF
