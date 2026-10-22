-- plugins/motion_udp.lua
-- Телеметрия OMSI 2 / openOMSI -> UDP/JSON на 127.0.0.1:4123
-- Читает: Omsi2Game.cs (vAzhureRacing Hub) / motion_udp_receiver.py
-- Кроссплатформенно: Windows, Linux, macOS, Android.
--
-- Установка: файл в папку plugins/ рядом с игрой. Перезагружается
-- автоматически при сохранении.
--
-- Формат пакета: JSON, одна датаграмма за кадр. omsi.send ограничен
-- 100 датаграммами в секунду — на 60 FPS с запасом.

local PORT = 4123

-- Коэффициент пересчёта хода подвески в градусы (50 мм ~ 1°)
local ANGLE_COEFF = 0.05

-- Имена переменных автобуса (как в .osc-скриптах). Если переменной нет
-- у конкретного автобуса, вернётся 0. Точные имена: .osc файл автобуса
-- или переменная окружения OMSI_WATCH_VARS (смотреть в game.log).
local VAR = {
	throttle   = "throttle",
	brake      = "brake",
	clutch     = "clutch",
	brakeforce = "brakeforce",
	velocity   = "velocity",            -- м/с (нативные единицы OMSI)
	susp_0_l   = "axle_suspension_0_1", -- ось 0, сторона 1
	susp_0_r   = "axle_suspension_0_2", -- ось 0, сторона 2
	susp_1_l   = "axle_suspension_1_1",
	susp_1_r   = "axle_suspension_1_2",
	acc_x      = "a_trans_x",           -- боковое ускорение, м/с^2
	acc_y      = "a_trans_y",           -- продольное
	acc_z      = "a_trans_z",           -- вертикальное
	-- для панели vAzhureRacing Hub:
	engine_n      = "engine_n",
	engine_on     = "engine_on",
	tank_content  = "engine_tank_content",
	lights_fern   = "lights_fern",
	blinker_l     = "AI_Blinker_L",
	blinker_r     = "AI_Blinker_R",
	gangR         = "cockpit_gangR",
	gangN         = "cockpit_gangN",
	gang1         = "cockpit_gang1",
	gang2         = "cockpit_gang2",
	gang3         = "cockpit_gang3",
}

-- ВАЖНО: имена подвесок в OMSI идут как axle_suspension_<ось>_<сторона 1/2>,
-- а не L/R. Поправьте таблицу под свой автобус, если имена другие.

local yaw = 0.0  -- накопленный yaw, градусы

local function v(name)
	local x = omsi.var(name)
	if x == nil then return 0.0 end
	return x
end

-- Системные переменные. Если в вашей сборке openOMSI функция называется
-- иначе (omsi.sys / omsi.system / omsi.global) — поправьте здесь; при
-- отсутствии pause будет 0 и Hub просто не будет фильтровать паузу.
local function sys(name)
	local ok, x = pcall(function() return omsi.var(name) end)
	if not ok or x == nil then return 0.0 end
	return x
end

function on_frame(dt)
	if dt == nil or dt <= 0 or dt > 0.25 then dt = 0.016 end

	local s0l = v(VAR.susp_0_l)
	local s0r = v(VAR.susp_0_r)
	local s1l = v(VAR.susp_1_l)
	local s1r = v(VAR.susp_1_r)

	-- Маппинг ускорений: OMSI X=Sideways, Y=Forward, Z=Vertical
	local surge = v(VAR.acc_y)
	local sway  = -v(VAR.acc_x)
	local heave = v(VAR.acc_z)

	-- Roll — разница левой/правой подвески
	local roll = ((s0r + s1r) * 0.5 - (s0l + s1l) * 0.5) * ANGLE_COEFF

	-- Pitch — разница передней/задней оси
	local pitch = ((s0l + s0r) * 0.5 - (s1l + s1r) * 0.5) * ANGLE_COEFF

	-- Yaw — интеграл (боковое ускорение / скорость). velocity УЖЕ в м/с,
	-- перевод не нужен (в м/с^2 / м/с = рад/с).
	local velocity = v(VAR.velocity)
	if velocity > 0.1 then
		yaw = yaw + (sway / velocity) * dt * 57.29578
		if yaw > 180 then yaw = yaw - 360 end
		if yaw < -180 then yaw = yaw + 360 end
	else
		yaw = yaw * 0.95  -- стоим: затухание, без дрейфа
	end

	local packet = {
		-- управление
		throttle = v(VAR.throttle),
		brake = v(VAR.brake),
		clutch = v(VAR.clutch),
		brakeforce = v(VAR.brakeforce),
		velocity = velocity,
		-- движение (для motion platform)
		surge = surge,
		sway = sway,
		heave = heave,
		roll = roll,
		pitch = pitch,
		yaw = yaw,
		-- сырые значения
		susp_0_l = s0l, susp_0_r = s0r,
		susp_1_l = s1l, susp_1_r = s1r,
		acc_x = v(VAR.acc_x), acc_y = v(VAR.acc_y), acc_z = v(VAR.acc_z),
		-- панель vAzhureRacing Hub
		engine_n = v(VAR.engine_n),
		engine_on = v(VAR.engine_on),
		engine_tank_content = v(VAR.tank_content),
		lights_fern = v(VAR.lights_fern),
		AI_Blinker_L = v(VAR.blinker_l),
		AI_Blinker_R = v(VAR.blinker_r),
		cockpit_gangR = v(VAR.gangR),
		cockpit_gangN = v(VAR.gangN),
		cockpit_gang1 = v(VAR.gang1),
		cockpit_gang2 = v(VAR.gang2),
		cockpit_gang3 = v(VAR.gang3),
		pause = sys("pause"),
	}
	omsi.send(PORT, omsi.json.encode(packet))
end

-- Альтернатива углам из подвески (точнее, если доступно):
-- heading, pitch, bank = omsi.bus.orientation() — реальная ориентация
-- кузова из физики. Yaw тогда: производная heading по времени.
