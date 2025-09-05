///////////////////////////////////////////////////////////////////////////
// TelemetryDataFormatPino.h
///////////////////////////////////////////////////////////////////////////
// Copyright (c) Bugbear Entertainment ltd. 
// All Rights Reserved.
///////////////////////////////////////////////////////////////////////////

#pragma once


/*
* Revision 2: August 2025
* 
* Clarified game, player and session status with:
* - GAME_STATUS_IN_RACE
* - Main::playerStatusFlags
* - Session::status
*/

/*
* Revision 1: August 2025
* 
* Initial version with most things added already. We have reserved bytes to add
* more things. Unless there is great need for it, we won't break alignment now.
* And we DEFINITELY WON'T after a couple weeks once we know software relies on
* this.
*
* If you're implementing something and feel like key data is missing, and you
* can't derive it from other data, please contact us on our Discord for example
* as we may be able to fulfill your request.
* 
* NOTES:
* - Some timing data is missing in multiplayer. See Timing structs.
* - As this is the first version, treat it as experimental subject to changes.
* - We will only break alignment as a last resort...
* - ...before freezing it for good to protect implementations
* - PLEASE TEST PLAYER MOTION DATA BEFORE ENABLING MOTION PLATFORM!
*/





// Save current packing alignment and set it to 1 byte (no padding)
#pragma pack(push, 1)

//
// ROMU GAME TELEMETRY
// 
// Documents the UDP telemetry data format "Pino" which is split into packets
// by frequency as we tried to keep packet sizes reasonable (except Info).
// 
// Each packet comes with a header that helps you identify it as a Pino packet
// and sync packets by comparing sessionTime. If sessionTime matches between
// multiple packet types, you know they were sent during the same game step.
// 
// High frequency packets at approx 60Hz:
// - Main with rich player-centric data
// - Participants leaderboard: participant status, race positions etc.
// - Participants timing: basic lap time data
// - Participants timing sectors: individual sector data
// - Participants motion: orientation data
// 
// Packets at approx 2Hz:
// - Participants damage: mechanical damage state BIT-PACKED
// 
// Packets at approx 1Hz:
// - Participants info: names and other low frequency data
//
// Main packet includes ALL the participant data for player so if you don't
// intend to display opponent data anywhere, main is all you need. For motion
// support for example or telemetry analysis. It's also the only packet with
// session and player status.
// 
// Participant count is fixed at 36. Participants leaderboard packet contains
// status which also says if the participant is unused in which case its data
// can be considered garbage and ignored.
// 
// 36 participants for forward compatibility. It does NOT reflect any plans to
// actually add that many to the game.
// 
// Packets and some structs contain reserved bytes for forward compatibility in
// case data needs to be added later. Obviously these aren't meant to be read.
// We'll document any reserve bytes in this file's changelog if we add anything
// useful there to read.
//

namespace romu::GameTelemetry::Pino
{
	using S8 = char;
	using U8 = unsigned char;
	using S16 = short;
	using U16 = unsigned short;
	using S32 = int;
	using U32 = unsigned int;

	static const int PARTICIPANTS_MAX = 36; // Unlikely, forward compatibility
	static const int TRACK_ID_LENGTH_MAX = 64;
	static const int TRACK_NAME_LENGTH_MAX = 96;
	static const int CAR_ID_LENGTH_MAX = 64;
	static const int CAR_NAME_LENGTH_MAX = 96;
	static const int PLAYER_NAME_LENGTH_MAX = 24;

	static const int DAMAGE_PARTS_MAX = 56; // More than needed right now for forward compatibility
	static const int DAMAGE_BITS_PER_PART = 3;
	static const int DAMAGE_BYTES_PER_PARTICIPANT = (DAMAGE_PARTS_MAX * DAMAGE_BITS_PER_PART + 7) / 8; // Ceil

	enum PacketType : U8
	{
		PACKET_TYPE_MAIN = 0,
		PACKET_TYPE_PARTICIPANTS_LEADERBOARD = 1,
		PACKET_TYPE_PARTICIPANTS_TIMING = 2,
		PACKET_TYPE_PARTICIPANTS_TIMING_SECTORS = 3,
		PACKET_TYPE_PARTICIPANTS_MOTION = 4,
		PACKET_TYPE_PARTICIPANTS_INFO = 5,
		PACKET_TYPE_PARTICIPANTS_DAMAGE = 6
	};

	enum GameStatusFlags : U8
	{
		// Game paused within race
		GAME_STATUS_PAUSED = (1 << 0),
		GAME_STATUS_REPLAY = (1 << 1),
		GAME_STATUS_SPECTATE = (1 << 2),
		GAME_STATUS_MULTIPLAYER_CLIENT = (1 << 3),
		GAME_STATUS_MULTIPLAYER_SERVER = (1 << 4),
		// Only set when in real-time race environment, detailed player and
		// session status in Main packet.
		GAME_STATUS_IN_RACE = (1 << 5)
	};

	// NOTE: We may add more game modes and will update documentation when we
	// do. The number will simply increase so handle those modes as unknown.
	// And when it is an unknown game mode, expect any gameplay related numbers
	// like laps, time left, frags...
	enum GameMode : U8
	{
		GAME_MODE_BANGER = 0,
		GAME_MODE_DEATHMATCH = 1,
		GAME_MODE_LAST_MAN_STANDING = 2,
		GAME_MODE_OTHER = 255 // Forward compatibility for modding
	};

	// Same as with game mode, we may add these so handle unknown gracefully
	enum DamageMode : U8
	{
		DAMAGE_MODE_WRECKER = 0,
		DAMAGE_MODE_NORMAL = 1,
		DAMAGE_MODE_REALISTIC = 2,
		DAMAGE_MODE_OTHER = 255 // Forward compatibility for modding
	};

	// NOTE: Not supported in game as such, but we can pretend for hardware.
	// Bit will be set as long as condition is true!
	enum MarshalFlags : U16
	{
		MARSHAL_FLAGS_GREEN = (1 << 0),
		MARSHAL_FLAGS_LASTLAP = (1 << 1),
		MARSHAL_FLAGS_FINISH = (1 << 2),
		MARSHAL_FLAGS_DQ = (1 << 3),
		MARSHAL_FLAGS_MEATBALL = (1 << 4), // Terminal damage on player car
		MARSHAL_FLAGS_WARNING = (1 << 5), // Close to DQ in game mode rules
		MARSHAL_FLAGS_BLUE = (1 << 6), // Car behind is lapping player
		MARSHAL_FLAGS_WHITE = (1 << 7), // Player is lapping car ahead
		MARSHAL_FLAGS_COUNTDOWN1 = (1 << 14), // 1st green
		MARSHAL_FLAGS_COUNTDOWN2 = (1 << 15) // 2nd green
	};

	enum PlayerStatusFlags : U16
	{
		PLAYER_STATUS_IN_RACE = (1 << 0), // Player's car is in race / on track
		PLAYER_STATUS_CAR_DRIVABLE = (1 << 1), // Player's car is drivable
		PLAYER_STATUS_PHYSICS_RUNNING = (1 << 2), // In race AND not paused
		PLAYER_STATUS_CONTROL_PLAYER = (1 << 3), // Player is driving player's car
		PLAYER_STATUS_CONTROL_AI = (1 << 4) // AI is driving player's car
	};

	// NOTE: Global status, per participant in Participant::Status.
	enum SessionStatus : U8
	{
		SESSION_STATUS_NONE = 0, // Completely outside racing environment (main menu, garage...)
		SESSION_STATUS_PRE_RACE = 1, // Waiting for countdown to start
		SESSION_STATUS_COUNTDOWN = 2, // Waiting for green
		SESSION_STATUS_RACING = 3, // Race is on
		SESSION_STATUS_ABANDONED = 4, // Race abandoned
		SESSION_STATUS_POST_RACE = 5 // Cars still on track after race finish
	};

	// NOTE: We may add new surface types, but if we do, we will add them to
	// the end so handle anything beyond the highest value as unknown.
	enum SurfaceType : U8
	{
		SURFACE_TYPE_DEFAULT = 0, // General substance, whatever that may be
		SURFACE_TYPE_NOCONTACT = 1, // Tire not in contact with surface
		SURFACE_TYPE_TARMAC = 2,
		SURFACE_TYPE_CONCRETE = 3,
		SURFACE_TYPE_GRAVEL = 4,
		SURFACE_TYPE_DIRT = 5,
		SURFACE_TYPE_MUD = 6,
		SURFACE_TYPE_RUMBLE_LOFQ = 7, // Kerb
		SURFACE_TYPE_RUMBLE_HIFQ = 8, // Rumble strip,
		SURFACE_TYPE_WATER = 9,
		SURFACE_TYPE_METAL = 10,
		SURFACE_TYPE_WOOD = 11,
		SURFACE_TYPE_SAND = 12,
		SURFACE_TYPE_ROCKS = 13,
		SURFACE_TYPE_FOLIAGE = 14,
		SURFACE_TYPE_SLOWDOWN = 15,
		SURFACE_TYPE_SNOW = 16
	};


	//
	// CHILD STRUCTS
	//

	struct Session
	{
		char trackId[TRACK_ID_LENGTH_MAX]; // Track layout identifier like "track01_4"
		char trackName[TRACK_NAME_LENGTH_MAX]; // Track AND layout name like "Savolax Sandpit Reverse"
		float trackLength; // Lap length along route, meter

		S16 laps; // Race laps
		S16 eventLength; // Event length, second

		U8 gridSize; // ALL original participants counted
		U8 gridSizeRemaining; // Only remaining participants counted

		U8 sectorCount; // 3 except in derbies etc. Pay attention to this!
		// Fractions of normalized lap (0-1) where sectors end so for example:
		// sectorFract1: 0.33 = 33% of lap
		// sectorFract2: 0.67 = 67% of lap
		float sectorFract1;
		float sectorFract2;

		GameMode gameMode;
		DamageMode damageMode;

		SessionStatus status;

		char reserved[26];
	};

	//
	// CAUTION! We use the left handed coordinate system so:
	// X = right
	// Y = up
	// Z = forward
	//
	namespace Motion
	{
		struct Orientation
		{
			// World position, meter
			float positionX;
			float positionY;
			float positionZ;

			// Orientation quaternion
			float orientationQuaternionX;
			float orientationQuaternionY;
			float orientationQuaternionZ;
			float orientationQuaternionW;

			// Extents from center for bounding box, car space, centimeter
			U16 extentsX; // Width / 2, centimeter
			U16 extentsY; // Height / 2, centimeter
			U16 extentsZ; // Length / 2, centimeter
		};

		struct Velocity
		{
			// Local velocity, m/s
			float velocityLocalX;
			float velocityLocalY;
			float velocityLocalZ;

			// Local angular velocity, rad/s
			float angularVelocityX;
			float angularVelocityY;
			float angularVelocityZ;

			// Local acceleration, m/s^2
			float accelerationLocalX;
			float accelerationLocalY;
			float accelerationLocalZ;
		};

		struct VelocityEssential
		{
			// Velocity vector magnitude, m/s
			float velocityMagnitude;
		};
	}

	namespace Damage
	{
		// Only to document which part each index in states array refers to
		enum Part : U8
		{
			PART_ENGINE			= 0,
			PART_GEARBOX		= 1,
			PART_BRAKE_FL		= 2,
			PART_BRAKE_FR		= 3,
			PART_BRAKE_RL		= 4,
			PART_BRAKE_RR		= 5,
			PART_SUSPENSION_FL	= 6,
			PART_SUSPENSION_FR	= 7,
			PART_SUSPENSION_RL	= 8,
			PART_SUSPENSION_RR	= 9,
			PART_TIRE_FL		= 10,
			PART_TIRE_FR		= 11,
			PART_TIRE_RL		= 12,
			PART_TIRE_RR		= 13,
			PART_HEADGASKET		= 14,
			PART_RADIATOR		= 15,
			PART_PISTONS		= 16,
			PART_TIREHUB_FL		= 17,
			PART_TIREHUB_FR		= 18,
			PART_TIREHUB_RL		= 19,
			PART_TIREHUB_RR		= 20,
			PART_OILPAN			= 21,
			PART_COOLANT		= 22,
			PART_OIL			= 23,
			PART_ENDBEARINGS	= 24,
			PART_HALFSHAFT_FL	= 25,
			PART_HALFSHAFT_FR	= 26,
			PART_HALFSHAFT_RL	= 27,
			PART_HALFSHAFT_RR	= 28,
			PART_RADIATORLEAK	= 29,
			PART_ARMOR_FL		= 30,
			PART_ARMOR_FR		= 31,
			PART_ARMOR_RL		= 32,
			PART_ARMOR_RR		= 33,
			PART_ARMOR_SL		= 34, // Side left
			PART_ARMOR_SR		= 35, // Side right
			PART_MISFIRE		= 36, // Refers to possibility of misfire, see EngineFlags when misfire is actually active
			PART_COUNT			= 37
		};

		enum State : U8
		{
			STATE_OK = 0,
			STATE_DAMAGED1 = 1,
			STATE_DAMAGED2 = 2,
			STATE_DAMAGED3 = 3,
			STATE_TERMINAL = 4
		};
	}

	namespace Car
	{
		enum AssistFlags : U8
		{
			ASSIST_FLAGS_ABS_ACTIVE = (1 << 0),
			ASSIST_FLAGS_TCS_ACTIVE = (1 << 1),
			ASSIST_FLAGS_ESC_ACTIVE = (1 << 2)
		};

		enum AssistGearbox : U8
		{
			ASSIST_GEARBOX_AUTO = 0,
			ASSIST_GEARBOX_MANUAL = 1,
			ASSIST_GEARBOX_MANUAL_WITH_CLUTCH = 2
		};

		enum AssistLevel : U8
		{
			ASSIST_LEVEL_OFF = 0,
			ASSIST_LEVEL_HALF = 1,
			ASSIST_LEVEL_FULL = 2
		};

		enum AxleLocation : U8
		{
			AXLE_LOCATION_FRONT = 0,
			AXLE_LOCATION_REAR = 1,
			AXLE_LOCATION_COUNT = 2
		};

		enum DrivelineType : U8
		{
			DRIVELINE_TYPE_FWD = 0,
			DRIVELINE_TYPE_RWD = 1,
			DRIVELINE_TYPE_AWD = 2
		};

		enum EngineFlags : U8
		{
			ENGINE_FLAGS_RUNNING = (1 << 0),
			ENGINE_FLAGS_STARTING = (1 << 1),
			ENGINE_FLAGS_MISFIRING = (1 << 2),
			ENGINE_FLAGS_DANGER_TO_MANIFOLD = (1 << 7), // Shut up!!
		};

		enum TireLocation : U8
		{
			TIRE_LOCATION_FL = 0,
			TIRE_LOCATION_FR = 1,
			TIRE_LOCATION_RL = 2,
			TIRE_LOCATION_RR = 3,
			TIRE_LOCATION_COUNT = 4
		};

		struct Assists
		{
			AssistFlags flags;
			AssistGearbox assistGearbox;
			AssistLevel levelAbs;
			AssistLevel levelTcs;
			AssistLevel levelEsc;

			char reserved[3];
		};

		struct Chassis
		{
			// NOTE: UNDAMAGED chassis
			float trackWidth[AXLE_LOCATION_COUNT];
			float wheelBase;

			S32 steeringWheelLockToLock; // Steering wheel travel lock-to-lock, degree
			// NOTE: Front geometry may have ackermann so this isn't accurate
			// for individual tires
			float steeringLock; // Tire max steering angle from center, radian

			float cornerWeights[TIRE_LOCATION_COUNT]; // Static load per tire, newton

			char reserved[16];
		};

		struct Driveline
		{
			DrivelineType type;

			U8 gear; // 0 = R, 1 = N, 2 = 1st...
			U8 gearMax; // Max forward gear AKA top gear, maxGear = 5 -> 5th is max

			float speed; // Realistic speedo value calculated from driveline, m/s

			char reserved[17];
		};

		struct Engine
		{
			EngineFlags flags;

			S32 rpm;
			S32 rpmMax; // Limiter RPM
			S32 rpmRedline; // Upshift RPM
			S32 rpmIdle;

			float torque; // Newton-metre
			float power; // Watt

			float tempBlock; // Kelvin
			float tempWater; // Kelvin

			float pressureManifold; // Kilopascal
			float pressureOil; // Kilopascal

			char reserved[15];
		};

		struct Input
		{
			float throttle; // 0 - 1
			float brake; // 0 - 1
			float clutch; // 0 - 1
			float handbrake; // 0 - 1
			float steering; // -1 - +1
		};

		struct Tire
		{
			float rps; // Rotational velocity, rad/s
			float camber; // Radian
			float slipRatio; // -1 = lock up, 0 = rolling without slip, 1 = spinning twice as fast as without slip
			float slipAngle; // Radian
			float radiusUnloaded; // Meter, -1.f if wheel has detached

			float loadVertical; // Newton
			float forceLat; // Newton
			float forceLong; // Newton

			// Tire temperatures output 0 value at the moment. We may add
			// support later if it's meaningful. So you should display them if
			// they are greater than 0 Kelvin.
			float temperatureInner; // Kelvin
			float temperatureTread; // Kelvin

			float suspensionVelocity; // M/s
			// NOTE: Displacement is within the "operating travel", but
			// displacement can go negative when helper springs are compressed
			// more than expected with the car raised off the ground
			float suspensionDisplacement; // Compression, droop being 0, meter
			float suspensionDispNorm; // Normalized displacement, 0 - 1

			float positionVertical; // Tire vertical position in car space, meter

			SurfaceType surfaceType;

			char reserved[15];
		};

		struct Full
		{
			Assists assists;
			Chassis chassis;
			Driveline driveline;
			Engine engine;
			Input input;
			Motion::Orientation orientation;
			Motion::Velocity velocity;
			Tire tires[TIRE_LOCATION_COUNT];

			char reserved[14];
		};
	}

	// NOTE: Participant indice are NOT deterministic from one session to
	// another. Player index could be any of them. But once the session is on
	// they shouldn't change. Therefore when receiving participant arrays, you
	// can trust that for example index 7 in Packet::Leaderboard contains the
	// same participant's data as index 7 in Packet::Timing. You'll know if
	// indice have been rearranged based on Packet::Info's data.
	namespace Participant
	{
		enum Visibility : U8
		{
			VISIBILITY_FULL = 0, // Full data visible
			VISIBILITY_LIMITED = 1 // Only data that won't reveal too much about opponents (like motion) in derbies for example
		};

		enum Status : U8
		{
			STATUS_INVALID = 0,
			STATUS_UNUSED = 1, // Participant slot unused, participantIndex will also be 255 then
			STATUS_RACING = 2,
			STATUS_FINISH_SUCCESS = 3,
			STATUS_FINISH_ELIMINATED = 4,
			STATUS_DNF_DQ = 5,
			STATUS_DNF_RETIRED = 6,
			STATUS_DNF_TIMEOUT = 7,
			STATUS_DNF_WRECKED = 8,
			STATUS_POSE = 9 // When car is outside race environment
		};

		// NOTE: Sectors here refers to an internal track route concept, not
		// racing sectors used in timing!
		enum TrackStatus : U8
		{
			TRACK_STATUS_NORMAL = 0, // In correct sector, correct direction.
			TRACK_STATUS_OUT_OF_SECTORS = 1, // Not in any sector (no idea of direction).
			TRACK_STATUS_IN_WRONG_SECTORS = 2, // In wrong sectors (wrong "zone").
			TRACK_STATUS_WRONG_DIRECTION = 3 // Correct sectors, wrong direction.
		};

		struct Leaderboard
		{
			// NOTE: Status is STATUS_UNUSED when participant doesn't exist and
			// participant slot is unused (empty data in array element) across
			// all participant packets' arrays.
			Status status;
			TrackStatus trackStatus;

			U16 lapCurrent; // 1st lap = 1, but goes from 0 to 1 during start
			U8 position; // 1st = 1, not 0
			U8 health; // 0 - 100%

			U16 wrecks;
			U16 frags;
			U16 assists;

			S32 score; // Primary points
			S32 points; // Alternative points

			S32 deltaLeader; // Delta time to race leader, millisecond

			char reserved[8];
		};

		// CAUTION! MP is missing some properties currently:
		// - lapTimePenaltyCurrent
		// - lapTimeLast
		// - lapBest
		struct Timing
		{
			U32 lapTimeCurrent; // Lap time WITH penalty, millisecond
			U32 lapTimePenaltyCurrent; // Penalty time this lap, millisecond
			U32 lapTimeLast; // Millisecond
			U32 lapTimeBest; // Millisecond
			U8 lapBest; // Lap number for best lap

			// NOTE: These are always positive, but -1 means that ahead/behind
			// doesn't exist
			S32 deltaAhead; // Delta time to car ahead, millisecond
			S32 deltaBehind; // Delta time to car behind, millisecond

			// Lap progress along route, can also be used to get lap distance
			// traveled by multiplying by trackLength.
			float lapProgress; // 0-1

			char reserved[3];
		};

		// CAUTION! ALL sector time data missing in MP currently. But instead
		// of following GAME_STATUS_MULTIPLAYER_CLIENT, check sectorCount in
		// Packet::Main::session. If it's 0, there are no sector times either.
		// NOTE: sectorTime3 = lapTime - (sectorTime1 + sectorTime2)
		struct TimingSectors
		{
			U32 sectorTimeCurrentLap1;
			U32 sectorTimeCurrentLap2;

			U32 sectorTimeLastLap1;
			U32 sectorTimeLastLap2;

			U32 sectorTimeBestLap1;
			U32 sectorTimeBestLap2;

			// Potential lap is these three together
			U32 sectorTimeBest1;
			U32 sectorTimeBest2;
			U32 sectorTimeBest3;
		};

		struct Motion
		{
			Pino::Motion::Orientation orientation;
			Pino::Motion::VelocityEssential velocity;
		};

		struct Damage
		{
			// CAUTION: Packed values DAMAGE_BITS_PER_PART per value
			U8 damageStates[DAMAGE_BYTES_PER_PARTICIPANT];

			//
			// Example of how to unpack states
			/*
			void DecompressStates(const U8* statesComp, U8* states)
			{
				for (int i = 0; i < PART_COUNT; i++)
				{
					S32 bitPos = i * DAMAGE_BITS_PER_PART;
					S32 byteIndex = bitPos / 8;
					S32 bitOffset = bitPos % 8;

					U8 stateDec = (statesComp[byteIndex] >> bitOffset) & 0b111;

					// Handle overflow into next byte
					if (bitOffset > 5)
					{
						stateDec |= (statesComp[byteIndex + 1] << (8 - bitOffset)) & 0b111;
					}

					states[i] = stateDec;
				}
			}
			*/
		};

		struct Info
		{
			char carId[CAR_ID_LENGTH_MAX]; // Car model identifier like "car04:default"
			char carName[CAR_NAME_LENGTH_MAX]; // Car model name like "Rocket"
			char playerName[PLAYER_NAME_LENGTH_MAX];

			// For clarity, this is the index of this participant in
			// participant arrays. Will be 255 if slot is unused.
			U8 participantIndex;

			S32 lastNormalTrackStatusTime; // Last time participant was on route, in race time, millisecond
			S32 lastCollisionTime; // In race time, millisecond
			S32 lastResetTime; // In race time, millisecond

			char reserved[16];
		};
	}


	//
	// PACKETS
	//

	namespace Packet
	{
		struct Header
		{
			const U32 signature = 1869769584; // EXPECT THIS! Identifies as Pino packet
			PacketType packetType;
			GameStatusFlags statusFlags;
			S32 sessionTime; // Primary timestamp that restarts when cars enter countdown, millisecond
			S32 raceTime; // Race time from lights out, millisecond
		};

		// Main telemetry packet for player centric data. Includes everything
		// for player only that participant packets include for all cars. Only
		// difference is that Participant::Motion is not needed with Car::Full.
		//
		// You will find full car data for player, session data, player status
		// and (pseudo) marshal flags only here in Main.
		struct Main
		{
			Header header;

			MarshalFlags marshalFlagsPlayer;

			Participant::Leaderboard participantPlayerLeaderboard;
			Participant::Timing participantPlayerTiming;
			Participant::TimingSectors participantPlayerTimingSectors;
			Participant::Info participantPlayerInfo;
			Participant::Damage participantPlayerDamage;
			Car::Full carPlayer;
			Session session;
			PlayerStatusFlags playerStatusFlags;

			char reserved[126];
		};

		// This is the primary participant data packet. If you use participant
		// data (other than player's in Main) at all, you need to check each
		// participantsLeaderboard array's status enum to make sure which slot
		// is unused. That way you'll know to ignore empty/garbage elements
		// when reading other participant packets' arrays.
		struct ParticipantsLeaderboard
		{
			Header header;

			Participant::Visibility participantVisibility;

			Participant::Leaderboard participantsLeaderboard[PARTICIPANTS_MAX];

			char reserved[64];
		};

		struct ParticipantsTiming
		{
			Header header;

			Participant::Visibility participantVisibility;

			Participant::Timing participantsTiming[PARTICIPANTS_MAX];

			char reserved[64];
		};

		struct ParticipantsTimingSectors
		{
			Header header;

			Participant::Visibility participantVisibility;

			Participant::TimingSectors participantsTimingSectors[PARTICIPANTS_MAX];

			char reserved[64];
		};

		struct ParticipantsMotion
		{
			Header header;

			Participant::Visibility participantVisibility;

			Participant::Motion participantsMotion[PARTICIPANTS_MAX];
		};

		struct ParticipantsInfo
		{
			Header header;

			Participant::Visibility participantVisibility;

			Participant::Info participantsInfo[PARTICIPANTS_MAX];

			char reserved[512];
		};

		struct ParticipantsDamage
		{
			Header header;

			Participant::Visibility participantVisibility;

			Participant::Damage participantsDamage[PARTICIPANTS_MAX];

			char reserved[256];
		};
	}

}

// Restore the original packing alignment
#pragma pack(pop)