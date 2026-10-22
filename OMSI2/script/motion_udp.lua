local PORT = 4123

local VAR = {
	throttle   = "throttle",
	brake      = "brake",
	clutch     = "clutch",
	brakeforce = "brakeforce",
	velocity   = "velocity",
	acc_x      = "a_trans_x",           
	acc_y      = "a_trans_y",
	acc_z      = "a_trans_z",
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

local function v(name)
	local x = omsi.var(name)
	if x == nil then return 0.0 end
	return x
end

local function sys(name)
	local ok, x = pcall(function() return omsi.var(name) end)
	if not ok or x == nil then return 0.0 end
	return x
end

function on_frame(dt)
	if dt == nil or dt <= 0 or dt > 0.25 then dt = 0.016 end

	local surge = v(VAR.acc_y)
	local sway  = -v(VAR.acc_x)
	local heave = v(VAR.acc_z)

	local true_pitch = 0.0
	local true_roll = 0.0
	local true_yaw = 0.0

	if omsi.bus and omsi.bus.orientation then
		local heading, p, b = omsi.bus.orientation()
		
		true_pitch = (p or 0.0) * 0.3183099
		true_roll = (b or 0.0) * 0.3183099
		true_yaw = (heading or 0.0) * 0.3183099

		if true_pitch > 1.0 then true_pitch = 1.0 elseif true_pitch < -1.0 then true_pitch = -1.0 end
		if true_roll > 1.0 then true_roll = 1.0 elseif true_roll < -1.0 then true_roll = -1.0 end
		
		if true_yaw > 1.0 then true_yaw = true_yaw - 2.0 end
		if true_yaw < -1.0 then true_yaw = true_yaw + 2.0 end
	end

	local velocity = v(VAR.velocity)
	
	if velocity > 0.0 and velocity == v(VAR.velocity) then
		velocity = velocity / 3.6 
	end

	local packet = {
		throttle = v(VAR.throttle),
		brake = v(VAR.brake),
		clutch = v(VAR.clutch),
		brakeforce = v(VAR.brakeforce),
		velocity = velocity,
		surge = surge,
		sway = sway,
		heave = heave,
		roll = true_roll * 0.1,
		pitch = true_pitch * 0.1,
		yaw = true_yaw * 0.1,
		susp_0_l = v("axle_suspension_0_1"), susp_0_r = v("axle_suspension_0_2"),
		susp_1_l = v("axle_suspension_1_1"), susp_1_r = v("axle_suspension_1_2"),
		acc_x = v(VAR.acc_x), acc_y = v(VAR.acc_y), acc_z = v(VAR.acc_z),
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
