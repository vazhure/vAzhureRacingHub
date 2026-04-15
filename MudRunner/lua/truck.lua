------------------------------------------
-- File:		truck.lua
-- Description:
-- Date:		15/10/11 7:39 PM
-- Modified:    2026/04/15 - File based telemetry added for vAzhure Racing Hub
--------------------------------------
-- Automatically included
-----------------------------------------------------------------------
function InitTruck(truck)
    -- common params
    truck.engineTension = 0
    truck.heavy = 0
    truck.turbo = 0
    truck.trans = 0
    truck.transReverse = 0
    truck.revving = 0

    -- exhaust params
    truck.exhaustStartFactor = 0
    truck.exhaustCoolFactor = 0
    truck.exhaustLargeSmokeCooldown = 0
    truck.exhaustSmokeFogTimer = 0
    truck.exhaustSmokeFogTensionSumm = 0
    truck.steamFactor = 0
    truck.isSteaming = false

    -- sound params
    truck.prevSteeringAngle = 0
    truck.prevPedalAccelerate = 0
    truck.prevPedalBrake = 0
    truck.isBreakPulled = false

    truck.linVelPrev = 0
    truck.chassisPrevLinVel = m3MakeZero()
    truck.chassisPrevAngVel = m3MakeZero()

    truck.elapsedStartTime = 0
    truck.elapsedDropletsTime = 0
end
-----------------------------------------------------------------------
function StopTruckEngine(truck)
    C_SoundStop(truck.soundEngineIdle)
    C_SoundStop(truck.soundEngineNeutral)
    C_SoundStop(truck.soundEngineLow)
    C_SoundStop(truck.soundEngineHigh)
    C_SoundStop(truck.soundEngineHeavy)
    C_SoundStop(truck.soundEngineTurbo)
    C_SoundStop(truck.soundEngineTrans)
    C_SoundStop(truck.soundEngineStart)
    C_SoundStop(truck.soundReverse)

    truck.elapsedStartTime = 0
    truck.engineTension = 0
    truck.turbo = 0
    truck.trans = 0
end
-----------------------------------------------------------------------
function SkipTruckEngineStart(truck)
    truck.elapsedStartTime =
        _max(truck.engineStartDelay, truck.exhaustStartTime) + 8
end
-----------------------------------------------------------------------
function GetTruckEngineTension(truck)
    local tension = truck.engineTension
    local engineTensionFadeInTime = 0.5
    local engineTensionFadeOutTime = 3.5
    local startEngineTension = 0.6
    if (truck.elapsedStartTime > truck.engineStartDelay +
        engineTensionFadeInTime) then
        tension = _max(tension, (1.0 - _min(
                           (truck.elapsedStartTime -
                               (truck.engineStartDelay + engineTensionFadeInTime)) /
                               engineTensionFadeOutTime, 1)) *
                           startEngineTension)
    else
        tension = _max(tension,
                       (truck.elapsedStartTime - truck.engineStartDelay) /
                           engineTensionFadeInTime * startEngineTension)
    end
    return tension
end
function ProcessTruckEngineSound(truck, truckInput, elapsedTime)
    local engineSoundsVolumeMult = lerp(envSoundsVolumeMult, 1.0, 0.5)
    truck.revving = _max(truck.revving - elapsedTime / 0.8, 0)
    if (not (truck.soundEngineRev == nil)) then
        if (truck.engineTension > 0.1 and truckInput.pedalAccelerate > EPSILON and
            truckInput.pedalAccelerate > truck.prevPedalAccelerate +
            truck.revving * 0.5 + 0.01) then
            C_SoundPlay(truck.soundEngineRev, false, true)
            C_SoundFadeIn(truck.soundEngineRev, 0)
            truck.revving = 1
            -- C_TraceDebugString( string.format("prevPedalAccelerate %.2f", truck.prevPedalAccelerate) )	
        end
        C_SoundSetVolumeMultiplier(truck.soundEngineRev, (0.4 +
                                       truck.engineTension * 0.4) *
                                       engineSoundsVolumeMult)

        if (truckInput.pedalAccelerate < EPSILON or truckInput.pedalAccelerate <
            truck.prevPedalAccelerate - 0.05) then
            truck.revving = 0
            -- truck.prevPedalAccelerate = 0
            C_SoundFadeOut(truck.soundEngineRev, 0.25)
        else
            -- truck.prevPedalAccelerate = truckInput.pedalAccelerate
        end
        truck.prevPedalAccelerate = truckInput.pedalAccelerate
    end
    local revFactor = truck.revving
    revFactor = -revFactor * 0.1

    local engineTension = GetTruckEngineTension(truck)
    C_ShakeTruck(truck.this, engineTension)
    local tHeavyMin = 0.6
    local tHeavyMax = 1.0
    local heavy =
        saturate((engineTension - tHeavyMin) / (tHeavyMax - tHeavyMin))
    if (heavy > truck.heavy) then
        truck.heavy = _min(truck.heavy + elapsedTime * 2.0, heavy)
    else
        truck.heavy = _max(truck.heavy - elapsedTime * 1.0, heavy)
    end
    if (truck.heavy > 0) then
        C_SoundPlay(truck.soundEngineHeavy, true)
        C_SoundSetVolumeMultiplier(truck.soundEngineHeavy,
                                   truck.heavy * engineSoundsVolumeMult)
        C_SoundSetFrequencyRatio(truck.soundEngineHeavy, 1.0 + revFactor)
    else
        C_SoundStop(truck.soundEngineHeavy)
    end

    -- engine mix
    local tLowMin = 0.01
    local tLowMax = 0.6
    local tHighMin = 0.4
    local tHighMax = 0.9
    local low = saturate((engineTension - tLowMin) / (tLowMax - tLowMin))
    local high = saturate((engineTension - tHighMin) / (tHighMax - tHighMin))

    local soundIdle = _max(1.0 - low - high, 0)
    if (soundIdle > EPSILON) then
        if (not (truck.soundEngineNeutral == nil) and (truckInput.gear == 0)) then
            C_SoundStop(truck.soundEngineIdle)
            C_SoundPlay(truck.soundEngineNeutral, true)
            C_SoundSetVolumeMultiplier(truck.soundEngineNeutral,
                                       soundIdle * engineSoundsVolumeMult)
        else
            C_SoundStop(truck.soundEngineNeutral)
            C_SoundPlay(truck.soundEngineIdle, true)
            C_SoundSetVolumeMultiplier(truck.soundEngineIdle,
                                       soundIdle * engineSoundsVolumeMult)
        end
    else
        C_SoundStop(truck.soundEngineIdle)
        C_SoundStop(truck.soundEngineNeutral)
    end

    local soundLow = _pow(low, 0.5) * (1.0 - high)
    if (soundLow > EPSILON) then
        C_SoundPlay(truck.soundEngineLow, true)
        C_SoundSetVolumeMultiplier(truck.soundEngineLow,
                                   soundLow * engineSoundsVolumeMult)
        C_SoundSetFrequencyRatio(truck.soundEngineLow,
                                 0.8 + low * 0.2 + revFactor)
    else
        C_SoundStop(truck.soundEngineLow)
    end

    local highMult = 1
    if (not (truck.soundEngineHeavy == nil)) then
        highMult = lerp(1, truck.engineHeavyVolume, truck.heavy * truck.heavy)
    end
    local soundHigh = _pow(high, 0.5) * highMult
    if (soundHigh > EPSILON) then
        C_SoundPlay(truck.soundEngineHigh, true)
        C_SoundSetVolumeMultiplier(truck.soundEngineHigh,
                                   soundHigh * engineSoundsVolumeMult)
        C_SoundSetFrequencyRatio(truck.soundEngineHigh,
                                 0.8 + high * 0.3 + revFactor)
    else
        C_SoundStop(truck.soundEngineHigh)
    end
    -- C_RenderDebugString( string.format("low %.2f, high %.2f, engineTension %.2f, truck.heavy", low, high, engineTension, truck.heavy) )

    if (not (truck.soundEngineTurbo == nil)) then
        local turbo = saturate((engineTension - 0.1) / 0.6)
        if (turbo > truck.turbo) then
            truck.turbo = _min(truck.turbo + elapsedTime * 0.4, turbo)
        else
            truck.turbo = _max(truck.turbo - elapsedTime * 0.2, turbo)
        end
        if (truck.turbo > 0) then
            C_SoundPlay(truck.soundEngineTurbo, true)

            C_SoundSetVolumeMultiplier(truck.soundEngineTurbo,
                                       truck.turbo * envSoundsVolumeMult * 0.5)
            C_SoundSetFrequencyRatio(truck.soundEngineTurbo,
                                     0.2 + truck.turbo * 1.0)
        else
            C_SoundStop(truck.soundEngineTurbo)
        end
    end
    if (not (truck.soundEngineTrans == nil)) then
        local trans = 0
        if (truckInput.gear > 0 and truckInput.gear < 3) then
            trans = engineTension * engineTension
        end
        if (trans > truck.trans) then
            truck.trans = _min(truck.trans + elapsedTime * 0.1, trans)
        else
            truck.trans = _max(truck.trans - elapsedTime * 0.5, trans)
        end
        if (truck.trans > 0) then
            C_SoundPlay(truck.soundEngineTrans, true)

            C_SoundSetVolumeMultiplier(truck.soundEngineTrans, truck.trans * 1.6)
            C_SoundSetFrequencyRatio(truck.soundEngineTrans,
                                     0.6 + truck.trans * 0.6)
        else
            C_SoundStop(truck.soundEngineTrans)
        end
    end
    -- C_RenderDebugString( string.format("tension %.2f turbo %.2f trans %.2f", engineTension, truck.turbo, truck.trans) )	

    if (not (truck.soundReverse == nil)) then
        local transReverse = 0
        if (truckInput.gear < 0) then transReverse = truck.engineTension end
        if (transReverse > truck.transReverse) then
            truck.transReverse = _min(truck.transReverse + elapsedTime * 2.0,
                                      transReverse)
        else
            truck.transReverse = _max(truck.transReverse - elapsedTime * 0.5,
                                      transReverse)
        end
        if (truck.transReverse > 0.01) then
            C_SoundPlay(truck.soundReverse, true)

            if (not truck.disableReversePitch) then
                C_SoundSetVolumeMultiplier(truck.soundReverse,
                                           truck.transReverse * 1.0)
                C_SoundSetFrequencyRatio(truck.soundReverse,
                                         0.8 + truck.transReverse * 0.4)
            end
        else
            C_SoundStop(truck.soundReverse)
        end
    end
end
-----------------------------------------------------------------------
function SpawnAirHissDust(hissOrg, hissScale)
    if (not (hissOrg == nil)) then
        C_SetParticlesBrand("honk_dust")
        for n = 1, 24 do
            local partVel = m3MakeVec(randoms(), 0, randoms())
            m3Normalize(partVel)
            m3Mul(partVel, 6 * hissScale)

            partVel.y = -2
            C_AddParticle(hissOrg, partVel, 0.4 * hissScale, 0.12)
        end
    end
end
-----------------------------------------------------------------------
function ProcessTruckSound(truck, truckInput, wheels, elapsedTime)
    local linVelL = m3Length(truckInput.linVelVector)
    -- brake
    if (truckInput.pedalBrake > 0.05 and truckInput.pedalBrake >
        truck.prevPedalBrake) then
        if (not truck.isBreakPulled) then
            C_SoundPlay(truck.soundBrakePull)
            truck.isBreakPulled = true
        end
    end
    if (truckInput.pedalBrake < 0.10 and truckInput.pedalBrake <
        truck.prevPedalBrake) then
        if (truck.isBreakPulled) then
            if (not (truck.soundBrakeRelease == nil)) then
                C_SoundPlay(truck.soundBrakeRelease)
                C_SoundSetVolumeMultiplier(truck.soundBrakeRelease,
                                           2.0 * envSoundsVolumeMult)
                SpawnAirHissDust(truckInput.hissOrg, truckInput.hissScale)
            end
            truck.isBreakPulled = false
        end
    end
    if (truckInput.pedalBrake > 0.05 and linVelL > 2.5) then
        C_SoundPlay(truck.soundBrakesSqueal, true)
        C_SoundFadeIn(truck.soundBrakesSqueal, 0.5)
        C_SoundSetVolumeMultiplier(truck.soundBrakesSqueal, _min(linVelL * 0.1,
                                                                 0.8) *
                                       envSoundsVolumeMult)
    else
        C_SoundStop(truck.soundBrakesSqueal)
    end
    truck.prevPedalBrake = truckInput.pedalBrake

    if (truckInput.isHandbrake and not truck.wasHandbrake) then
        C_SoundPlay(truck.soundHandbrake, false, true)
    end
    truck.wasHandbrake = truckInput.isHandbrake

    -- transmission
    if (not (truckInput.gear == truck.prevGear) and not (truckInput.gear == 0)) then
        C_SoundPlay(truck.soundGear, false, true)
        truck.revving = 0
        truck.prevPedalAccelerate = 0
    else
        if ((not (truckInput.isDifferentialLocked == truck.wasDifferentialLocked)) or
            (not (truckInput.isAllWheelDrive == truck.wasAllWheelDrive))) then
            C_SoundPlay(truck.soundDiffLock, false, true)
        end
    end
    truck.wasDifferentialLocked = truckInput.isDifferentialLocked
    truck.wasAllWheelDrive = truckInput.isAllWheelDrive
    truck.prevGear = truckInput.gear
    if (not (truckInput.isHeadLight == truck.wasHeadLight)) then
        C_SoundPlay(truck.soundHeadLight, false, true)
    end
    truck.wasHeadLight = truckInput.isHeadLight

    if (truckInput.isEngineIgnited) then
        C_SoundStop(truck.soundEngineStop)
        if (truck.elapsedStartTime == 0) then
            C_SoundPlay(truck.soundEngineStart)
        end
        truck.elapsedStartTime = truck.elapsedStartTime + elapsedTime
        if (truck.elapsedStartTime > truck.engineStartDelay) then
            ProcessTruckEngineSound(truck, truckInput, elapsedTime)
        end
    else
        C_SoundStop(truck.soundEngineStart)
        if (truck.elapsedStartTime > 0) then
            C_SoundPlay(truck.soundEngineStop, false, true)
            C_ShakeTruck(truck.this, 1)
            StopTruckEngine(truck)
        end
    end
    if (linVelL > 1.0) then
        local vA = m3Diff(truckInput.linVelVector, truck.chassisPrevLinVel)
        m3Div(vA, elapsedTime)

        local fA = m3Dot(truckInput.linVelVector, vA) / linVelL
        if (fA < -2.0) then
            C_SoundPlay(truck.soundAbruptStop)
            C_SoundSetVolumeMultiplier(truck.soundAbruptStop,
                                       clamp(-fA * 0.1, 0.4, 1.0))
        end
    end
    local angVelL = m3Length(truckInput.angVelVector)
    if (angVelL > 0.5) then
        local vA = m3Diff(truckInput.angVelVector, truck.chassisPrevAngVel)
        m3Div(vA, elapsedTime)

        local fA = m3Length(vA)
        if (fA > 0.8) then
            -- C_TraceDebugString( string.format("fA = %.2f", fA) )
            C_SoundPlay(truck.soundAbruptStop)
            C_SoundSetVolumeMultiplier(truck.soundAbruptStop,
                                       clamp(fA * 0.2, 0.4, 1.4))
        end
    end
    truck.chassisPrevAngVel = truckInput.angVelVector
    truck.chassisPrevLinVel = truckInput.linVelVector

    --
    if (truckInput.isEngineIgnited and truck.airHissInterval > 0) then
        if (truck.breaksAirTime == nil or truckInput.pedalBrake > 0) then
            truck.breaksAirTime = truck.airHissInterval * (1 + _random())
        end
        truck.breaksAirTime = truck.breaksAirTime - elapsedTime
        if (truck.breaksAirTime < 0) then
            if (not (truck.soundBrakeRelease == nil)) then
                C_SoundPlay(truck.soundBrakeRelease)
                C_SoundSetVolumeMultiplier(truck.soundBrakeRelease,
                                           envSoundsVolumeMult)
                SpawnAirHissDust(truckInput.hissOrg, truckInput.hissScale)
            end
            truck.breaksAirTime = truck.airHissInterval * (1 + _random())
        end
    end

    -- play suspension stress sounds
    if (truck.suspStressFactors == nil) then
        truck.suspStressFactors = {}
        for i, wheel in ipairs(wheels) do
            truck.suspStressFactors[i] = wheel.suspStressFactor
        end
    end

    -- local wheelsDirt = 0
    local wheelsGrass = 0
    local wheelsMud = 0
    local wheelsSpinning = 0
    local wheelsSteer = 0
    local wheelsWater = 0

    for i, wheel in ipairs(wheels) do
        local dSusp =
            _abs(wheel.suspStressFactor - truck.suspStressFactors[i]) /
                elapsedTime
        if (dSusp > 1) then
            -- C_TraceDebugString( "STRESS" )
            C_SoundPlay(truck.soundChassisStress)
            C_SoundSetVolumeMultiplier(truck.soundChassisStress,
                                       clamp(dSusp * 0.1, 0.2, 1.0))
        end
        truck.suspStressFactors[i] = wheel.suspStressFactor

        -- ural as a reference
        local radiusScale = _min(wheel.radius / 0.58, 1.4)
        local angVelParam = _abs(wheel.angVelDamped) / 6
        -- if (debrisType == "fragments_grass") then
        wheelsGrass = _max(wheelsGrass,
                           angVelParam * wheel.contactFriction * radiusScale)
        -- else
        --	wheelsDirt = _max(wheelsDirt, angVelParam * wheel.contactFriction)
        -- end
        if (wheel.contactFriction < EPSILON) then
            wheelsSpinning = _max(wheelsSpinning,
                                  angVelParam * 0.3 * radiusScale)
        end

        local tWater = _min(wheel.waterDepth / 0.4, 1.4)
        wheelsMud = _max(wheelsMud,
                         angVelParam * _min(wheel.mudDepth / 0.3, 1) *
                             radiusScale)
        wheelsWater = _max(wheelsWater, angVelParam * tWater * radiusScale)
        wheelsSteer = _max(wheelsSteer,
                           wheel.contactFriction * 0.5 * radiusScale)
    end
    -- wheelsDirt = saturate(wheelsDirt - 0.1)
    wheelsWater = saturate(wheelsWater - 0.1)
    wheelsGrass = saturate(wheelsGrass * (1 - wheelsWater) - 0.1)
    wheelsMud = saturate(wheelsMud * (1 - wheelsWater) - 0.1)
    wheelsSpinning = wheelsSpinning * (1 - wheelsWater)
    wheelsSteer = wheelsSteer * (1 - wheelsWater)

    if (wheelsGrass > 0) then
        C_SoundPlay(truck.soundWheelGrass, true)
        C_SoundFadeIn(truck.soundWheelGrass, 0.25)
        C_SoundSetVolumeMultiplier(truck.soundWheelGrass, (0.4 + wheelsGrass *
                                       0.8) * envSoundsVolumeMult)
        C_SoundSetFrequencyRatio(truck.soundWheelGrass, 0.8 + wheelsGrass * 0.4)
    else
        C_SoundFadeOut(truck.soundWheelGrass, 0.25)
    end
    if (wheelsSpinning > 0.1) then
        C_SoundPlay(truck.soundWheelSpinning, true)
        C_SoundFadeIn(truck.soundWheelSpinning, 0.1)
        C_SoundSetVolumeMultiplier(truck.soundWheelSpinning,
                                   wheelsSpinning * envSoundsVolumeMult)
        C_SoundSetFrequencyRatio(truck.soundWheelSpinning,
                                 0.6 + wheelsSpinning * 0.6)
    else
        C_SoundFadeOut(truck.soundWheelSpinning, 0.1)
    end
    if (wheelsMud > 0 and wheelsMud < 0.5) then
        C_SoundPlay(truck.soundWheelMud, true)
        C_SoundFadeIn(truck.soundWheelMud, 0.1)
        C_SoundSetVolumeMultiplier(truck.soundWheelMud, (0.4 + wheelsMud / 0.5 *
                                       0.6) * envSoundsVolumeMult)
        C_SoundSetFrequencyRatio(truck.soundWheelMud,
                                 0.8 + wheelsMud / 0.5 * 0.4)
    else
        C_SoundFadeOut(truck.soundWheelMud, 0.1)
    end
    if (wheelsMud > 0.5) then
        C_SoundPlay(truck.soundWheelExtrude, true)
        C_SoundFadeIn(truck.soundWheelExtrude, 0.1)
        C_SoundSetVolumeMultiplier(truck.soundWheelExtrude, (0.4 +
                                       (wheelsMud - 0.5) * 2.0) *
                                       envSoundsVolumeMult)
        C_SoundSetFrequencyRatio(truck.soundWheelExtrude,
                                 0.8 + (wheelsMud - 0.5) * 0.4)
    else
        C_SoundFadeOut(truck.soundWheelExtrude, 0.1)
    end
    -- C_RenderDebugString( string.format("wheelsWater %.2f waterHit %.2f", wheelsWater, waterHit) )
    if (wheelsWater > 0) then
        C_SoundPlay(truck.soundWheelWater, true)
        C_SoundFadeIn(truck.soundWheelWater, 0.5)
        C_SoundSetVolumeMultiplier(truck.soundWheelWater, (0.8 + wheelsWater *
                                       0.4) * envSoundsVolumeMult)
        C_SoundSetFrequencyRatio(truck.soundWheelWater, 0.8 + wheelsWater * 0.4)
    else
        C_SoundFadeOut(truck.soundWheelWater, 0.5)
    end

    -- wheel steering noise
    local deltaSteer = wheelsSteer *
                           _abs(
                               truckInput.steeringAngle -
                                   truck.prevSteeringAngle) / elapsedTime
    if (deltaSteer > 0.05) then
        C_SoundPlay(truck.soundWheelSteer, true)
        C_SoundFadeIn(truck.soundWheelSteer, 0.1)
        C_SoundSetVolumeMultiplier(truck.soundWheelSteer, _min(
                                       (deltaSteer - 0.05) / 0.8, 1.0) *
                                       envSoundsVolumeMult)
    else
        C_SoundFadeOut(truck.soundWheelSteer, 0.1)
    end
    truck.prevSteeringAngle = truckInput.steeringAngle
end
-----------------------------------------------------------------------
function ProcessTruckEffects(truck, truckInput, wheels, exhausts, intakes,
                             steams, elapsedTime)
    local linVelL = m3Length(truckInput.linVelVector)
    local tDamage = saturate((truckInput.damageFactor - 0.7) / 0.1)

    local smokeFogOpacity = -1
    if (truckInput.isEngineIgnited) then
        truck.exhaustSmokeFogTimer = truck.exhaustSmokeFogTimer + elapsedTime
        truck.exhaustSmokeFogTensionSumm =
            truck.exhaustSmokeFogTensionSumm + truck.engineTension * elapsedTime
        if (truck.exhaustSmokeFogTimer > 4) then
            smokeFogOpacity = truck.exhaustSmokeFogTensionSumm /
                                  truck.exhaustSmokeFogTimer
            truck.exhaustSmokeFogTimer = 0
            truck.exhaustSmokeFogTensionSumm = 0
        end
    else
        truck.exhaustSmokeFogTimer = 0
        truck.exhaustSmokeFogTensionSumm = 0
    end

    local isExhaustGurgle = false
    if (table.getn(exhausts) > 0) then
        local exhaustAmount = 0
        if (truckInput.isEngineIgnited) then
            local exhaustTime = 1.5
            local exhaustFadeTime = 4.0
            exhaustAmount = truck.engineTension * truck.engineTension
            if (truck.elapsedStartTime < truck.engineStartDelay + exhaustTime) then
                if (truck.elapsedStartTime > truck.exhaustStartTime) then
                    exhaustAmount = 1
                end
            else
                local engineStartExhaust = 1 -
                                               _min(
                                                   (truck.elapsedStartTime -
                                                       (truck.engineStartDelay +
                                                           exhaustTime)) /
                                                       exhaustFadeTime, 1)
                exhaustAmount = _min(exhaustAmount + engineStartExhaust, 1.0)
            end
            if (truck.isEngineIgnited and truck.elapsedStartTime <
                truck.engineStartDelay) then
                C_ShakeTruck(truck.this, engineStartExhaust)
            end
            if (truck.elapsedStartTime > truck.engineStartDelay and
                not (truckInput.hissOrg == nil)) then
                local dustFadeTime = 1.0
                local dustAmount = 1 -
                                       _min(
                                           (truck.elapsedStartTime -
                                               truck.engineStartDelay) /
                                               dustFadeTime, 1)
                local numDustParticles =
                    GetNumEmitParticles(0.002, dustAmount, elapsedTime)
                if (numDustParticles >= 1) then
                    C_SetParticlesBrand("honk_dust")
                    for n = 1, numDustParticles do
                        local partVel = m3MakeVec(randoms(), 0, randoms())
                        m3Normalize(partVel)
                        m3Mul(partVel,
                              (3 + dustAmount * 5) * truckInput.hissScale)

                        partVel.y = -2
                        C_AddParticle(truckInput.hissOrg, partVel,
                                      0.4 * truckInput.hissScale, 0.12)
                    end
                end
            end
            exhaustAmount = lerp(exhaustAmount, saturate(exhaustAmount * 8.0),
                                 tDamage)
        end

        local exhaustThreshold = 0.06
        if (exhaustAmount > exhaustThreshold) then
            local d = (exhaustAmount - exhaustThreshold) /
                          (1.0 - exhaustThreshold) * elapsedTime
            truck.exhaustStartFactor = _min(truck.exhaustStartFactor + d * 0.8,
                                            2.0)
            truck.exhaustCoolFactor = _min(
                                          truck.exhaustCoolFactor + d * 0.8 *
                                              (1.0 - tDamage), 16.0)
        else
            local d = (exhaustThreshold - exhaustAmount) / exhaustThreshold *
                          elapsedTime
            truck.exhaustStartFactor = _max(truck.exhaustStartFactor - d * 2.0,
                                            0)
            truck.exhaustCoolFactor = _max(truck.exhaustCoolFactor - d * 0.6, 0)
        end
        -- C_RenderDebugString( string.format("exhaustStartFactor %.2f exhaustCoolFactor %.2f", truck.exhaustStartFactor, truck.exhaustCoolFactor) )

        local exhaustLargeSmokeCooldownTime = 0.6
        if (exhaustAmount > 0.25 and truck.exhaustCoolFactor < 8.0) then
            truck.exhaustLargeSmokeCooldown = _min(
                                                  truck.exhaustLargeSmokeCooldown +
                                                      elapsedTime,
                                                  exhaustLargeSmokeCooldownTime)
        else
            truck.exhaustLargeSmokeCooldown = _max(
                                                  truck.exhaustLargeSmokeCooldown -
                                                      elapsedTime, 0)
        end

        if (truckInput.isEngineIgnited and truck.elapsedStartTime >
            truck.exhaustStartTime) then
            local tStart = _max(0, 1.0 - truck.exhaustStartFactor)

            -- More exhaust if there were none for a short period of time
            exhaustAmount = exhaustAmount * (0.8 + tStart * 0.6)
            exhaustAmount = _min(exhaustAmount + truck.revving, 1.0)
            -- exhaustAmount = 1	
            -- C_RenderDebugString( string.format("exhaustAmount %.2f", exhaustAmount) )		

            local isExhaustShot = false
            if (_random() < tDamage * truckInput.torqueFactor * 0.2) then
                isExhaustShot = true
            end

            -- larger particles when truck is moving fast
            local exhaustScale = 1.0 + tStart * 0.2 +
                                     clamp((linVelL - 4.0) / 8.0, 0.0, 2.0)
            for i, exhaust in ipairs(exhausts) do
                local waterHeight = C_GetWaterHeightAt(m2MakeVec(
                                                           exhaust.exhaustOrg.x,
                                                           exhaust.exhaustOrg.z))
                local isSubmerged = false
                if (exhaust.exhaustOrg.y < waterHeight) then
                    isSubmerged = true

                    if (truckInput.isInWorld and exhaust.exhaustOrg.y >
                        waterHeight - 1.0) then
                        local numSurfParticles =
                            GetNumEmitParticles(0.04, 1, elapsedTime)
                        if (numSurfParticles >= 1) then
                            local spawnOrg = m3CopyVec(exhaust.exhaustVel)
                            m3Normalize(spawnOrg)
                            m3Mul(spawnOrg, 0.4)
                            spawnOrg.x = spawnOrg.x + exhaust.exhaustOrg.x
                            spawnOrg.y = waterHeight - 0.2
                            spawnOrg.z = spawnOrg.z + exhaust.exhaustOrg.z

                            C_SetParticlesBrand("water_surf")
                            for n = 1, numSurfParticles do
                                local partOrg = m3CopyVec(spawnOrg)
                                partOrg.x = partOrg.x + (_random() - 0.5) * 0.4
                                partOrg.z = partOrg.z + (_random() - 0.5) * 0.4

                                local partVel = m3MakeZero()
                                partVel.x = _random() - 0.5
                                partVel.y = 2.0 + _random() * 1.0
                                partVel.z = _random() - 0.5

                                C_AddParticleColor(partOrg, partVel,
                                                   truckInput.waterColor, 0.8,
                                                   0.6)
                            end
                        end
                    end
                end

                local partVel = m3CopyVec(exhaust.exhaustVel)
                -- add chassis velocity before multiply
                m3Add(partVel, truckInput.linVelVector)
                m3Mul(partVel, (0.8 + exhaustAmount * 0.2) * 0.8)

                -- approximate area of most smoke
                local cloudExhaustOrg = m3MulMake(truckInput.linVelVector, -0.6)
                m3Add(cloudExhaustOrg, exhaust.exhaustOrg)
                m3Add(cloudExhaustOrg, m3MulMake(partVel, 0.6))

                if (not exhaust.isLight and smokeFogOpacity >= 0) then
                    C_SetParticlesBrand("exhaust_fog")
                    C_AddParticle(cloudExhaustOrg, m3MulMake(partVel, 0.1), 1.0,
                                  1.0 + smokeFogOpacity)
                end

                if (isSubmerged) then
                    isExhaustGurgle = true
                else
                    if (exhaustAmount > EPSILON) then
                        -- C_RenderDebugPoint(exhaust.exhaustOrg)				
                        local numParticles =
                            GetNumEmitParticles(0.04, _max(exhaustAmount, 0.5),
                                                elapsedTime)
                        if (numParticles > 0) then
                            local exhaustOpacity = 0.3 +
                                                       saturate(
                                                           1.0 -
                                                               (truck.exhaustCoolFactor -
                                                                   2.0) / 4.0) *
                                                       0.4
                            exhaustOpacity = exhaustOpacity +
                                                 (1.0 -
                                                     saturate(
                                                         truck.exhaustCoolFactor /
                                                             0.5)) * 0.8
                            -- exhaustOpacity = exhaustOpacity * 1.4
                            if (exhaustAmount < 0.25) then
                                exhaustOpacity =
                                    exhaustOpacity * exhaustAmount / 0.25
                            end

                            -- Black exhaust when there were none for a long period of time
                            local exhaustBrightness = _max(0.1, 0.8 - tStart)
                            exhaustOpacity =
                                lerp(exhaustOpacity, 1.0,
                                     _min(tDamage + truck.revving, 1.0))
                            exhaustBrightness =
                                lerp(exhaustBrightness, 0.0, tDamage)

                            if (exhaust.isLight) then
                                exhaustOpacity = exhaustOpacity * 0.2
                            end
                            C_SetParticlesBrand("exhaust_small")
                            for n = 1, numParticles do
                                C_AddParticle(exhaust.exhaustOrg, partVel,
                                              exhaustScale, exhaustOpacity,
                                              exhaustBrightness)
                            end

                            if (not exhaust.isLight and
                                truck.exhaustLargeSmokeCooldown ==
                                exhaustLargeSmokeCooldownTime) then
                                C_SetParticlesBrand("exhaust_large")
                                C_AddParticle(cloudExhaustOrg,
                                              m3MulMake(partVel, 0.4),
                                              exhaustScale, exhaustOpacity,
                                              exhaustBrightness)
                            end
                        end

                        if (isExhaustShot) then
                            C_SetParticlesBrand("engine_fire")
                            C_AddParticle(exhaust.exhaustOrg, partVel, 1, 1)
                            C_SoundPlay(truck.soundExhaustShot, false, true)
                            C_SoundSetVolumeMultiplier(truck.soundExhaustShot,
                                                       1.0 * envSoundsVolumeMult)

                            C_SetParticlesBrand("engine_sparks")
                            for n = 1, 16 do
                                local partSparksVel =
                                    m3MakeVec(partVel.x + randoms() * 4,
                                              partVel.y + _random() * 2,
                                              partVel.z + randoms() * 4)
                                m3Mul(partSparksVel, 0.5 + _random() * 0.5)
                                C_AddParticle(exhaust.exhaustOrg, partSparksVel)
                            end
                        end
                    end

                    if (truck.elapsedStartTime > truck.exhaustStartTime) then
                        local numHeatParticles =
                            GetNumEmitParticles(0.1 - exhaustAmount * 0.08, 1,
                                                elapsedTime)
                        if (numHeatParticles > 0) then
                            local partHeatVel = m3CopyVec(exhaust.exhaustVel)
                            -- add chassis velocity before multiply
                            m3Add(partHeatVel, truckInput.linVelVector)
                            m3Mul(partHeatVel, (0.8 + exhaustAmount * 0.4) * 0.8)
                            partHeatVel.x =
                                partHeatVel.x + randoms() * exhaustAmount * 2
                            partHeatVel.y =
                                partHeatVel.y + randoms() * exhaustAmount * 2
                            partHeatVel.z =
                                partHeatVel.z + randoms() * exhaustAmount * 2

                            local exhaustHeatScale
                            if (exhaust.isLight) then
                                exhaustHeatScale = exhaustScale * 0.8
                            else
                                exhaustHeatScale =
                                    exhaustScale * 0.6 + exhaustAmount * 0.8
                            end
                            C_SetParticlesBrand("exhaust_heat")
                            for n = 1, numHeatParticles do
                                C_AddParticle(exhaust.exhaustOrg, partHeatVel,
                                              exhaustHeatScale, 0.5)
                            end
                        end
                    end
                end
            end
        end
        if (truck.exhaustLargeSmokeCooldown == exhaustLargeSmokeCooldownTime) then
            truck.exhaustLargeSmokeCooldown = 0
        end
    end
    if (isExhaustGurgle) then
        C_SoundPlay(truck.soundWaterGurgle, true)
        C_SoundFadeIn(truck.soundWaterGurgle, 0.2)
    else
        C_SoundFadeOut(truck.soundWaterGurgle, 0.2)
    end

    if (truck.intakeSubmergedTime == nil) then
        truck.intakeSubmergedTime = {}
        for i, intake in ipairs(intakes) do
            truck.intakeSubmergedTime[i] = 0
        end
    end
    for i, intake in ipairs(intakes) do
        if (truckInput.isEngineIgnited) then
            -- local intakeSubmergePoint = m3CopyVec(intake.intakeOrg)
            -- intakeSubmergePoint.y = intakeSubmergePoint.y - 0.35
            if (intake.intakeOrg.y <
                C_GetWaterHeightAt(
                    m2MakeVec(intake.intakeOrg.x, intake.intakeOrg.z))) then
                --[[local tBurst = 1.0 - saturate((truck.intakeSubmergedTime[i] - 1.0) / 2.0)
				if (tBurst > EPSILON) then
					--C_TraceDebugString( "BURST" )
					C_SetParticlesBrand("water_burst_medium")		
 					
					local partVel = m3MulMake(intake.intakeDir,2.5)
					partVel.y = partVel.y * 0.5 + 0.5
					
					local numParticles = GetNumEmitParticles(0.08 / intake.size, 1.0, elapsedTime)
					for n = 1, numParticles  do		
						local partPos = m3MakeVec(
							intake.intakeOrg.x - intake.intakeDir.x * 0.4 + (_random() - 0.5) * intake.size * 0.5,
							intake.intakeOrg.y - intake.intakeDir.y * 0.4 + (_random() - 0.5) * intake.size * 0.5,
							intake.intakeOrg.z - intake.intakeDir.z * 0.4 + (_random() - 0.5) * intake.size * 0.5
						)
						C_AddParticleColor(partPos, partVel, truckInput.waterColor, 0.8, 0.6 * tBurst)						
					end
					C_SoundPlay(truck.soundExhaustShot)		
					C_SoundSetVolumeMultiplier(truck.soundExhaustShot, 0.5 * envSoundsVolumeMult)
				end]]
                truck.intakeSubmergedTime[i] = clamp(
                                                   truck.intakeSubmergedTime[i] +
                                                       elapsedTime, 1, 3.1)
            else
                truck.intakeSubmergedTime[i] = _max(
                                                   truck.intakeSubmergedTime[i] -
                                                       elapsedTime, 0)
            end

            local engineTension = GetTruckEngineTension(truck)
            engineTension = _min(engineTension + truck.revving * 0.5, 1.0)

            if (truck.intakeSubmergedTime[i] <= EPSILON and engineTension > 0.25) then
                local numParticles = GetNumEmitParticles(0.03 / intake.size,
                                                         1.0, elapsedTime)
                for n = 1, numParticles do
                    local partPos = m3MakeVec(
                                        intake.intakeDir.x * 0.6 +
                                            intake.intakeOrg.x +
                                            (_random() - 0.5) * intake.size,
                                        intake.intakeDir.y * 0.6 +
                                            intake.intakeOrg.y +
                                            (_random() - 0.5) * intake.size,
                                        intake.intakeDir.z * 0.6 +
                                            intake.intakeOrg.z +
                                            (_random() - 0.5) * intake.size)
                    local partVel = m3Diff(intake.intakeOrg, partPos)
                    m3Normalize(partVel)
                    m3Mul(partVel, 2.0)
                    m3Add(partVel, truckInput.linVelVector)

                    C_SetParticlesBrand("intake")
                    C_AddParticle(partPos, partVel, 1.0, engineTension)

                    C_SetParticlesBrand("intake_distortion")
                    C_AddParticle(partPos, partVel, 1.0, engineTension)
                end
            end
        end
    end

    -- engine and wheels steam
    for i, steam in ipairs(steams) do
        local steamDamage = 0
        if (truckInput.isEngineIgnited) then
            steamDamage = tDamage

            C_SetParticlesBrand("water_flow")
            local partCol = m3MakeVec(0, 0, 0)
            local numParticles = GetNumEmitParticles(0.1, tDamage *
                                                         steam.steamScale,
                                                     elapsedTime)
            if (numParticles > 0) then
                if (steam.steamOrg.y <
                    C_GetWaterHeightAt(
                        m2MakeVec(steam.steamOrg.x, steam.steamOrg.z))) then
                    numParticles = 0
                end
            end
            for n = 1, numParticles do
                local partVel = m3MakeVec(0, -0.5, 0)
                m3Add(partVel, m3MulMake(truckInput.linVelVector, 0.8))

                local partOrg = m3CopyVec(steam.steamOrg)
                partOrg.x = partOrg.x + randoms() * 0.5
                partOrg.z = partOrg.z + randoms() * 0.5
                C_AddParticleColor(partOrg, partVel, partCol, 1.0, 2)
            end
        end

        -- steamDamage = 1
        local steamAmount = steamDamage * 0.25
        if (truck.isSteaming) then
            steamAmount = _max(steamAmount, truck.steamFactor)

            truck.steamFactor = _max(truck.steamFactor - 0.06 * elapsedTime, 0)
            if (truck.steamFactor == 0) then truck.isSteaming = false end
        else
            local steamSubmergePoint = m3CopyVec(steam.steamOrg)
            steamSubmergePoint.y = steamSubmergePoint.y - 0.2

            truck.isSteaming = false
            if (steamSubmergePoint.y <
                C_GetWaterHeightAt(
                    m2MakeVec(steamSubmergePoint.x, steamSubmergePoint.z))) then
                truck.isSubmerged = true
            end
            truck.steamFactor = saturate(truck.steamFactor +
                                             (truck.engineTension * 0.1 - 0.01) *
                                             elapsedTime)
        end

        if (steamAmount > 0) then
            C_SetParticlesBrand("steam")

            local numParticles = GetNumEmitParticles(0.1, steamAmount *
                                                         steam.steamScale,
                                                     elapsedTime)
            for n = 1, numParticles do
                local partVel = m3MakeVec(randoms() * 1.5, 0.5 + _random() * 1,
                                          randoms() * 1.5)
                m3Mul(partVel, steam.steamScale)
                m3Add(partVel, m3MulMake(truckInput.linVelVector, 0.8))

                local partOrg = m3CopyVec(steam.steamOrg)
                partOrg.x = partOrg.x + randoms()
                partOrg.y = partOrg.y + (_random() - 0.8)
                partOrg.z = partOrg.z + randoms()
                C_AddParticle(partOrg, partVel, 1.0 + truck.steamFactor * 0.2,
                              0.4 + steamAmount * 0.6 - _min(linVelL * 0.1, 0.4))
            end
        end
        local heatFactor = 1
        if (not truckInput.isEngineIgnited) then
            heatFactor = saturate((truck.steamFactor - 0.6) / 0.2)
        end
        if (linVelL < 0.2 and heatFactor > 0) then
            local numHeatParticles = GetNumEmitParticles(0.3, heatFactor,
                                                         elapsedTime)
            if (numHeatParticles > 0) then
                C_SetParticlesBrand("steam_heat")
                for n = 1, numHeatParticles do
                    local partVel = m3MakeVec(randoms() * 1.5,
                                              0.5 + _random() * 1,
                                              randoms() * 1.5)
                    m3Mul(partVel, steam.steamScale)
                    m3Add(partVel, m3MulMake(truckInput.linVelVector, 0.8))

                    local partOrg = m3CopyVec(steam.steamOrg)
                    partOrg.x = partOrg.x + randoms()
                    partOrg.y = partOrg.y + (_random() - 0.8)
                    partOrg.z = partOrg.z + randoms()
                    C_AddParticle(partOrg, partVel, 1.0, 0.4 + heatFactor * 0.6)
                end
            end
        end
        -- C_RenderDebugString( string.format("steamAmount %.2f", steamAmount) )
    end

    if (truck.wheelsSteamFactors == nil) then
        truck.wheelsSteamFactors = {}
        truck.wheelsHeatFactors = {}
        truck.wheelsSteaming = {}
        for i, wheel in ipairs(wheels) do
            truck.wheelsSteamFactors[i] = 0
            truck.wheelsHeatFactors[i] = 0
            truck.wheelsSteaming[i] = false
        end
    end

    C_SetParticlesBrand("steam")
    for iWheel, wheel in ipairs(wheels) do
        local velSpin = _abs(wheel.angVelDamped) * wheel.radius -
                            _abs(wheel.linVel)
        if (velSpin > 0.5) then
            truck.wheelsHeatFactors[iWheel] = _min(
                                                  truck.wheelsHeatFactors[iWheel] +
                                                      wheel.contactFriction *
                                                      velSpin * 0.0002, 1)
        else
            truck.wheelsHeatFactors[iWheel] = _max(
                                                  truck.wheelsHeatFactors[iWheel] -
                                                      0.004, 0)
        end
        local heatSteam =
            saturate((truck.wheelsHeatFactors[iWheel] - 0.5) / 1.5)
        if (truck.wheelsSteaming[iWheel] or heatSteam > 0) then
            -- truck.wheelsSteamFactors[iWheel] = 1			
            local steamAmount =
                _max(truck.wheelsSteamFactors[iWheel], heatSteam)
            if (steamAmount > 0) then
                local numParticles = GetNumEmitParticles(0.2, steamAmount,
                                                         elapsedTime)
                if (numParticles > 0) then
                    local partScale = _max(wheel.radius / 0.6, 1.0) +
                                          steamAmount * 0.1
                    for n = 1, numParticles do
                        local partVel = m3MakeVec(randoms() * 0.6,
                                                  0.2 + _random() * 0.4,
                                                  randoms() * 0.6)
                        m3Add(partVel, truckInput.linVelVector)
                        C_AddParticle(wheel.org, partVel, partScale,
                                      0.1 + steamAmount * 0.3)
                    end
                end
                truck.wheelsSteamFactors[iWheel] = _max(
                                                       truck.wheelsSteamFactors[iWheel] -
                                                           0.08 * elapsedTime, 0)
            else
                truck.wheelsSteaming[iWheel] = false
            end
        else
            truck.wheelsSteaming[iWheel] = false
            if (wheel.org.y <
                C_GetWaterHeightAt(m2MakeVec(wheel.org.x, wheel.org.z))) then
                truck.wheelsSteaming[iWheel] = true
            end
            truck.wheelsSteamFactors[iWheel] = saturate(
                                                   truck.wheelsSteamFactors[iWheel] +
                                                       (linVelL * 0.3 +
                                                           _abs(
                                                               wheel.angVelDamped) *
                                                           0.008 - 0.01) *
                                                       elapsedTime)
        end
        -- C_RenderDebugString( string.format("wheelsSteamFactors %.2f wheelsHeatFactors %.2f", truck.wheelsSteamFactors[iWheel], truck.wheelsHeatFactors[iWheel]) )
    end
end
-----------------------------------------------------------------------
function ProcessTruckWheels(truck, linVelVector, wheels, elapsedTime)
    local numWheelsFactor = 1
    if (table.getn(wheels) > 6) then
        numWheelsFactor = _max(1 - (table.getn(wheels) - 6) / 4, 0.5)
    end
    local wheelsSkid = 0
    for iWheel, wheel in ipairs(wheels) do
        if (wheel.waterDepth < 0.1) then
            -- Dust particles
            if (wheel.dustCoef > 0) then
                local dustAmount = _max(_abs(wheel.angVel) * wheel.radius,
                                        wheel.linVel) - 0.5
                dustAmount = saturate(dustAmount * 0.1) * wheel.dustCoef *
                                 (1.0 - wheel.mudCoef)
                -- fewer dust when position remains the same
                dustAmount = dustAmount * saturate(wheel.linVel / 2.0 + 0.2) *
                                 wheel.contactFriction

                local numDustParticles =
                    GetNumEmitParticles(0.06, dustAmount * numWheelsFactor,
                                        elapsedTime)
                -- C_RenderDebugString( string.format("dustAmount %.2f", dustAmount) )

                local dustOrg = m3MakeVec(wheel.org.x, wheel.org.y, wheel.org.z)
                dustOrg.y = dustOrg.y - wheel.radius

                local dustOpacity = 0.2 + dustAmount * 0.4
                local dustScale = wheel.radius / 0.63 *
                                      (1.4 + wheel.linVel * 0.1)
                -- C_RenderDebugString( string.format("dustAmount %.2f", dustAmount) )

                C_SetParticlesBrand("dust_small")
                for n = 1, numDustParticles do
                    local partVel = m3MakeVec(randoms() * wheel.linVel * 0.5,
                                              1.2,
                                              randoms() * wheel.linVel * 0.5)
                    C_AddParticle(dustOrg, partVel, dustScale, dustOpacity, 0.9)
                end
            end

            -- Debris particles
            -- local debrisAmount = 0

            -- if (wheel.mudCoef < 100) then
            local angLinVel = _abs(wheel.angVel) * wheel.radius
            local linAngVel = wheel.linVel
            if (wheel.angVel < 0) then linAngVel = -linAngVel end
            local axleVel = m3Dot(wheel.linVelVector, wheel.axleDir)
            local slideT = saturate((_abs(axleVel) - 1.5) * 0.25) *
                               wheel.contactFriction
            wheelsSkid = _max(wheelsSkid, slideT)

            -- ural as a reference
            local radiusScale = _min(wheel.radius / 0.58, 1.4)
            local wheelSpin = angLinVel - linAngVel
            wheelsSkid = _max(wheelsSkid,
                              saturate((_abs(wheelSpin) - 0.1) / 1.5) *
                                  wheel.contactFriction * 0.5 * radiusScale)
            -- C_RenderDebugString( string.format("wheel %d - wheelSpin %.2f", iWheel - 1, wheelSpin) )	

            local _slideT = slideT
            local _axleDir = m3CopyVec(wheel.axleDir)
            if (axleVel < 0) then
                m3Negate(_axleDir)
                if (wheel.isRightSided) then _slideT = 0 end
            else
                if (not wheel.isRightSided) then _slideT = 0 end
            end
            if (wheel.mudCoef > 0.25 or wheel.debrisCoef > EPSILON) then
                local debrisAmount = saturate(
                                         (wheel.contactFriction * angLinVel -
                                             5.0) * 0.5) * 0.5 *
                                         wheel.debrisCoef
                debrisAmount = _max(debrisAmount, saturate(
                                        (wheel.mudCoef * angLinVel - 2.0) * 0.2))
                -- debrisAmount = 1				

                C_SetParticlesBrand("fragments_dirt")
                if (debrisAmount > EPSILON) then
                    local numDebrisParticles =
                        GetNumEmitParticles(0.02,
                                            debrisAmount * numWheelsFactor,
                                            elapsedTime)
                    -- C_RenderDebugString( string.format("mudDepthT %.2f", mudDepthT) )

                    local basisCos = wheel.wheelDir
                    local basisSin = wheel.suspDir
                    for n = 1, numDebrisParticles do
                        local a = (-0.2 + _random() * 0.7) * PI
                        if (wheel.angVel > 0) then
                            a = PI - a
                        end
                        local r = m2MakeVec(_cos(a), _sin(a))

                        local partOrg = m3CopyVec(wheel.axleDir)
                        m3Mul(partOrg, randoms() * wheel.width)

                        m3Add(partOrg,
                              m3MulMake(basisCos, r.x * (wheel.radius + 0.025)))
                        m3Add(partOrg,
                              m3MulMake(basisSin, r.y * (wheel.radius + 0.025)))
                        m3Add(partOrg, wheel.org)

                        -- perpendicular radial coords
                        local r_p = m2MakePerpCW(r)
                        local partVel = m3MulMake(basisCos, r_p.x)
                        m3Add(partVel, m3MulMake(basisSin, r_p.y))
                        if (wheel.angVel < 0) then
                            m3Negate(partVel)
                        end
                        m3Add(partVel, m3MulMake(wheel.axleDir, 0.2 * randoms()))
                        m3Mul(partVel,
                              _min(angLinVel, 6) * (0.6 + randoms() * 0.4))
                        m3Add(partVel, m3MulMake(linVelVector, 0.8))

                        C_AddParticle(partOrg, partVel)
                    end
                end
                if (_slideT * wheel.debrisCoef > EPSILON) then
                    -- sliding dirt particles
                    -- C_RenderDebugString( string.format("wheel %d - _slideT %.2f", iWheel - 1, _slideT) )	
                    local emitVel = m3MulMake(_axleDir, 0.5)
                    emitVel.y = emitVel.y + 0.8 + _slideT * 1
                    m3Add(emitVel, m3MulMake(linVelVector, 0.25))

                    local partOrg = m3CopyVec(wheel.org)
                    local _axle = (0.1 + _random() * 0.25) * wheel.width
                    partOrg.x = partOrg.x + _axleDir.x * _axle
                    partOrg.z = partOrg.z + _axleDir.z * _axle
                    partOrg.y = partOrg.y - wheel.radius + 0.1

                    local partDir = m3CopyVec(_axleDir)
                    m3Add(partDir, m3MulMake(wheel.suspDir, 0.5 + _slideT))
                    m3Normalize(partDir)

                    local numBurstParticles =
                        GetNumEmitParticles(0.02, 1.0, elapsedTime)
                    C_SetParticlesBrand("dirt_burst_medium")
                    for n = 1, numBurstParticles do
                        -- ural as a reference
                        local partScale =
                            clamp(wheel.radius / 0.58, 0.8, 1.2) + _slideT * 0.2

                        local partVel = m3CopyVec(emitVel)
                        partVel.x = partVel.x + randoms() * 1.5
                        partVel.y = partVel.y + _random() * 1
                        partVel.z = partVel.z + randoms() * 1.5
                        m3Mul(partVel, partScale)

                        C_AddParticle(partOrg, partVel, partScale,
                                      0.8 + _slideT * 0.4)
                    end
                end
            end
            -- sliding dust particles
            if (_slideT > EPSILON) then
                local emitVel = m3MulMake(_axleDir, 0.5)
                emitVel.y = 0.25
                m3Add(emitVel, m3MulMake(linVelVector, 0.33))

                local numDustParticles =
                    GetNumEmitParticles(0.05, 1.0, elapsedTime)
                C_SetParticlesBrand("dust_small")

                local partOrg = m3CopyVec(wheel.org)
                partOrg.y = partOrg.y - wheel.radius

                local partScale = wheel.radius / 0.58 * 0.8 + _slideT * 0.8
                local partOpacity = 0.6 + wheel.debrisCoef * 0.4
                local partBrightness = 0.8 - wheel.debrisCoef * 0.4
                for n = 1, numDustParticles do
                    -- ural as a reference
                    local partVel = m3CopyVec(emitVel)
                    partVel.x = partVel.x + randoms() * 1.5
                    partVel.y = partVel.y + _random() * 0.8 + 0.4
                    partVel.z = partVel.z + randoms() * 1.5
                    m3Mul(partVel, partScale)

                    C_AddParticle(partOrg, partVel, partScale, partOpacity,
                                  partBrightness)
                end
            end

            if (wheel.wetCoef > 0.4 and wheel.waterDepth < 0.1) then
                -- if (wheel.waterDepth < wheel.radius) then
                local wetCoefT = _min(wheel.wetCoef * 2.5, 1.0)
                local angLinVel = _abs(wheel.angVel) * wheel.radius
                local splashAmount =
                    saturate((wetCoefT * angLinVel - 2.0) * 0.2)
                -- local splashAmount = saturate((1 * angLinVel - 2.0) * 0.4)
                local splashScale = 0.4 + _min(wheel.radius / 0.6, 1.2) * 0.6

                C_SetParticlesBrand("water_splash_small_simple")
                if (splashAmount > EPSILON) then
                    local numSplashParticles =
                        GetNumEmitParticles(0.016,
                                            splashAmount * numWheelsFactor,
                                            elapsedTime)
                    -- C_RenderDebugString( string.format("mudDepthT %.2f", mudDepthT) )
                    local emitVel = _min(angLinVel, 4) * 0.8

                    local basisCos = wheel.wheelDir
                    local basisSin = wheel.suspDir
                    for n = 1, numSplashParticles do
                        local a = (0.1 + _random()) * PI
                        if (wheel.angVel > 0) then
                            a = PI - a
                        end
                        local r = m2MakeVec(_cos(a), _sin(a))

                        local partOrg = m3CopyVec(wheel.axleDir)
                        m3Mul(partOrg, randoms() * wheel.width)
                        m3Add(partOrg, m3MulMake(basisCos, r.x * wheel.radius))
                        m3Add(partOrg, m3MulMake(basisSin, r.y * wheel.radius))
                        m3Mul(partOrg, 0.72)
                        m3Add(partOrg, wheel.org)

                        -- perpendicular radial coords
                        local r_p = m2MakePerpCW(r)
                        local partVel = m3MulMake(basisCos, r_p.x)
                        m3Add(partVel, m3MulMake(basisSin, r_p.y))
                        if (wheel.angVel < 0) then
                            m3Negate(partVel)
                        end
                        m3Mul(partVel, emitVel)
                        partVel.y = partVel.y * 0.5

                        m3Add(partVel, m3MulMake(linVelVector, 0.8))

                        C_AddParticleColor(partOrg, partVel, wheel.waterColor,
                                           splashScale, wetCoefT * 0.24)
                    end
                end
            end

            -- fast skid particles
            if (wheel.mudDepth < 0.1 and wheel.waterDepth < 0.1) then
                local fastSkidAmount = saturate((_abs(wheelSpin) - 4.0) / 4.0) *
                                           wheel.contactFriction
                if (fastSkidAmount > EPSILON) then
                    local numSkidParticles =
                        GetNumEmitParticles(0.006, numWheelsFactor, elapsedTime)
                    -- C_RenderDebugString( string.format("fastSkidAmount %.2f", fastSkidAmount) )							
                    C_SetParticlesBrand("dirt_burst_medium_skid")

                    local basisCos = wheel.wheelDir
                    local basisSin = wheel.suspDir
                    for n = 1, numSkidParticles do
                        local a = (-0.55 + _random() * 0.15) * PI
                        if (wheel.angVel > 0) then
                            a = PI - a
                        end
                        local r = m2MakeVec(_cos(a), _sin(a))

                        local partOrg = m3CopyVec(wheel.org)
                        m3Add(partOrg, m3MulMake(basisCos, r.x * wheel.radius))
                        m3Add(partOrg, m3MulMake(basisSin, r.y * wheel.radius))
                        partOrg.y = partOrg.y + 0.1

                        -- perpendicular radial coords
                        local r_p = m2MakePerpCW(r)
                        local partVel = m3MulMake(basisCos, r_p.x)
                        m3Add(partVel, m3MulMake(basisSin, r_p.y))
                        if (wheel.angVel < 0) then
                            m3Negate(partVel)
                        end
                        m3Add(partVel, m3MulMake(wheel.axleDir, 0.2 * randoms()))
                        m3Mul(partVel, _min(angLinVel * 0.8, 6))
                        m3Add(partVel, m3MulMake(linVelVector, 0.8))

                        -- kraz as a reference
                        local partScale = _min(wheel.radius / 0.63, 2) * 3.2

                        C_AddParticle(partOrg, partVel, partScale, (0.4 +
                                          wheel.debrisCoef * 0.6) *
                                          fastSkidAmount)
                    end
                end
            end

        end
    end

    if (wheelsSkid > 0.1) then
        C_SoundPlay(truck.soundWheelSkid, true)
        C_SoundFadeIn(truck.soundWheelSkid, 0.1)
        C_SoundSetVolumeMultiplier(truck.soundWheelSkid,
                                   wheelsSkid * envSoundsVolumeMult)
    else
        C_SoundFadeOut(truck.soundWheelSkid, 0.1)
    end
end
-----------------------------------------------------------------------
function ProcessTruckWaves(truck, truckInput, wave, flows, elapsedTime)
    local linVelL = m3Length(truckInput.linVelVector)
    local waveOffset = m3Diff(wave.posRight, wave.posLeft)
    local waveLength = m3Length(waveOffset)
    local emitBurstAmount = saturate((_abs(linVelL) - 2.0) / 2.0 + wave.hump *
                                         2.5) * saturate(waveLength * 0.5)
    local numBurstParticles = GetNumEmitParticles(0.028, emitBurstAmount,
                                                  elapsedTime)
    if (numBurstParticles > 0) then
        local partFluidVel = m3CopyVec(truckInput.linVelVector)
        m3Mul(partFluidVel, 0.8)
        partFluidVel.y = emitBurstAmount * 0.5 + 2.0

        local orgOffset = m3MulMake(truckInput.linVelVector, 0.1)
        for n = 1, numBurstParticles do
            local partScale = 0.5 + saturate((_abs(linVelL) - 1.0) / 8.0) * 0.5
            local partOrg = m3MulMake(waveOffset, _random())
            m3Add(partOrg, wave.posLeft)
            m3Add(partOrg, orgOffset)
            partOrg.y = partOrg.y - 0.25

            C_SetParticlesBrand("water_fluid_large")
            C_AddParticleColor(partOrg, partFluidVel, truckInput.waterColor,
                               0.8 + emitBurstAmount * 0.3)
        end

        local soundAmount = saturate((_abs(linVelL) - 1.5) / 4.0)
        if (soundAmount > soundWaterParam + 0.2) then
            -- C_TraceDebugString( "FLOW HIT" )
            soundWaterParam = soundAmount

            local nHit = math.random(1, #soundWaterHits)
            C_SoundPlay(soundWaterHits[nHit], false, true,
                        m3Lerp(wave.posLeft, wave.posRight, 0.5))
            C_SoundSetVolumeMultiplier(soundWaterHits[nHit],
                                       (0.6 + soundAmount * 0.6) *
                                           envSoundsVolumeMult)
        end
    end

    local isFlowSound = false
    for i, flow in ipairs(flows) do
        local flowOffset = m3Diff(m3MakeVec(flow.v1_.x, flow.v1_.y, flow.v1_.z),
                                  flow.v0)
        local flowLength = m3Length(flowOffset)
        local flowIntensity = flow.v1_.w * 2.0
        local d = _min(0.1 / flowLength, 0.1)

        local numFlowParticles = GetNumEmitParticles(0.1, flowLength *
                                                         _pow(flowIntensity, 0.5),
                                                     elapsedTime)
        if (numFlowParticles >= 1) then
            -- local partVel = m3CopyVec(truckInput.linVelVector)
            local partVel = m3MulMake(truckInput.linVelVector, 0.8)
            partVel.y = partVel.y - 1.5

            C_SetParticlesBrand("water_flow")
            for n = 1, numFlowParticles do
                -- C_TraceDebugString( "FLOW" )
                local t = _random()
                local partOrg = m3MulMake(flowOffset, t * (1 - d) + d)
                m3Add(partOrg, flow.v0)
                C_AddParticleColor(partOrg, partVel, truckInput.waterColor, 1,
                                   0.2 + flowIntensity * 0.3)
            end
        end
        isFlowSound = true
    end
    if (isFlowSound) then
        C_SoundPlay(truck.soundWaterDropping, true)
        C_SoundFadeIn(truck.soundWaterDropping, 1.0)
    else
        C_SoundFadeOut(truck.soundWaterDropping, 1.0)
    end
end
-----------------------------------------------------------------------
function ProcessTruck(truck, truckInput, wheels, exhausts, intakes, steams,
                      wave, flows, elapsedTime)
    if (truck.prevGear == nil) then
        truck.prevGear = truckInput.gear
        truck.wasHandbrake = truckInput.isHandbrake
        truck.wasDifferentialLocked = truckInput.isDifferentialLocked
        truck.wasAllWheelDrive = truckInput.isAllWheelDrive
        truck.wasHeadLight = truckInput.isHeadLight
    end
    if (elapsedTime > EPSILON) then
        -- local dTime = 0.2
        -- if (truckInput.torqueFactor > truck.engineTension) then
        -- dTime = 0.5
        -- end
        -- local deltaTime = 0.2
        -- if (truckInput.torqueFactor > truck.engineTension) then
        -- deltaTime = deltaTime + _max(truckInput.torqueFactor - 0.2)
        -- end
        truck.engineTension = lerp(truck.engineTension, truckInput.torqueFactor,
                                   _min(1.0, elapsedTime / 0.4))
        -- C_RenderDebugString( string.format("truck.engineTension %.2f truckInput.torqueFactor %.2f", truck.engineTension, truckInput.torqueFactor) )

        ProcessTruckSound(truck, truckInput, wheels, elapsedTime)
        ProcessTruckEffects(truck, truckInput, wheels, exhausts, intakes,
                            steams, elapsedTime)

        if (truckInput.isInWorld) then
            ProcessTruckWheels(truck, truckInput.linVelVector, wheels,
                               elapsedTime)
            ProcessTruckWaves(truck, truckInput, wave, flows, elapsedTime)
        end
    end
end
-----------------------------------------------------------------------
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

    local line = string.format(
                     "T|%010.3f|%07.2f|%08.4f|%06.3f|%06.3f|%02d|%02d|%02d|%02d|%02d|%02d|" ..
                         "%010.4f|%010.4f|%010.4f|%010.4f|%010.4f|%010.4f|%010.4f|%010.4f|%010.4f|" ..
                         "%010.4f|%010.4f|%010.4f|%012.2f|%012.2f|%012.2f|%05d|%05d|%06.3f|%06.3f|" ..
                         "%06.3f|%07.4f|%07.2f|%07.2f|%02d",
                     telemetryState.elapsedTime, speedKmh, steerAngle, throttle,
                     brake, gear, handbrake, diffLock, awd, engineRunning,
                     headlights, linVelX, linVelY, linVelZ, angVelX, angVelY,
                     angVelZ, accelX, accelY, accelZ,
                     telemetryState.smoothSurge, telemetryState.smoothSway,
                     telemetryState.smoothHeave, posX, posY, posZ, estimatedRPM,
                     maxRPM, engineTension, turbo, damageFactor,
                     avgSuspCompression, maxMudDepth, maxWaterDepth, wheelCount)

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

    if truckInput ~= nil and truckInput.isInWorld then
        UpdateTelemetry(truck, truckInput, wheels, elapsedTime)
    end
end

-- ============================================================
--  >>>>> END OF TELEMETRY CODE <<<<<<
-- ============================================================
-- EOF
