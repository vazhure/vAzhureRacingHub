-- ============================================================
--  ГЛОБАЛЬНАЯ ПЕРЕМЕННАЯ ДЛЯ ОТСЛЕЖИВАНИЯ АКТИВНОЙ МАШИНЫ
-- ============================================================
local ActiveTruck = nil

-- ============================================================
--  HOOK
-- ============================================================
local _originalProcessTruck = ProcessTruck
function ProcessTruck(truck, truckInput, wheels, exhausts, intakes, steams, wave, flows, elapsedTime)
    if _originalProcessTruck ~= nil then
        _originalProcessTruck(truck, truckInput, wheels, exhausts, intakes, steams, wave, flows, elapsedTime)
    end

    if telemetryState.enabled == false then InitTelemetry() end

    -- --- ЛОГИКА ОПРЕДЕЛЕНИЯ АКТИВНОЙ МАШИНЫ ---

    -- 1. Если есть ввод от игрока (газ, тормоз, руль) - это точно наша машина.
    if truckInput.pedalAccelerate > 0.01 or truckInput.pedalBrake > 0.01 or math.abs(truckInput.steeringAngle) > 0.05 then
        ActiveTruck = truck
    end

    -- 2. Если двигатель только что ЗАВЕЛСЯ (isEngineIgnited стал true) - значит, игрок сел и завел.
    -- Отслеживаем изменение состояния через сохранение предыдущего значения в самом объекте truck.
    local engineJustStarted = false
    if truckInput.isEngineIgnited then
        if truck.prevEngineIgnited == nil or truck.prevEngineIgnited == false then
            engineJustStarted = true
        end
    end
    -- Сохраняем текущее состояние для следующего кадра
    truck.prevEngineIgnited = truckInput.isEngineIgnited

    if engineJustStarted then
        ActiveTruck = truck
    end

    -- 3. Если ActiveTruck еще не выбран (например, первый запуск игры), 
    -- но двигатель заведен - берем эту машину.
    if ActiveTruck == nil and truckInput.isEngineIgnited then
        ActiveTruck = truck
    end

    -- --- ФИЛЬТР ЗАПИСИ ---
    -- Записываем, только если это запомненная активная машина и у нее есть звук двигателя (фильтр прицепов).
    if ActiveTruck == truck and truck.soundEngineIdle ~= nil then
        if truckInput ~= nil and truckInput.isInWorld then
            UpdateTelemetry(truck, truckInput, wheels, elapsedTime)
        end
    end
end