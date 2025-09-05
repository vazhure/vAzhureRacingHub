using System;
using System.Drawing;
using System.IO;
using System.Numerics;
using System.Reflection;
using System.Runtime.InteropServices;
using vAzhureRacingAPI;

namespace WreckfestPlugin
{
    public class WreckFest2Game : VAzhureUDPClient, IGamePlugin
    {
        public string Name => "Wreckfest 2";

        public uint SteamGameID => 1203190u;

        public string[] ExecutableProcessName => new []{ "Wreckfest2" };

        string iconPath = "";
        string exePath = "";

        public string UserIconPath { get => iconPath; set => iconPath = value; }
        public string UserExecutablePath { get => exePath; set => exePath = value; }


        bool _bRunning = false;

        public bool IsRunning => _bRunning;

        public event EventHandler<TelemetryUpdatedEventArgs> OnTelemetry;
        public event EventHandler OnGameStateChanged;
        public event EventHandler OnGameIconChanged;

        private readonly GameSettings settings;

        private readonly ProcessMonitor monitor;
        private readonly TelemetryDataSet telemetryDataSet;

        public WreckFest2Game()
        {

#if DEBUG
            Console.WriteLine($"sizeof PacketHeader is {Marshal.SizeOf(new Wreckfest2Structs.PacketHeader())}");
            Console.WriteLine($"sizeof PacketMain is {Marshal.SizeOf(new Wreckfest2Structs.PacketMain())}");
            Console.WriteLine($"sizeof PacketParticipantsDamage is {Marshal.SizeOf(new Wreckfest2Structs.PacketParticipantsDamage())}");
            Console.WriteLine($"sizeof PacketParticipantsInfo is {Marshal.SizeOf(new Wreckfest2Structs.PacketParticipantsInfo())}");
            Console.WriteLine($"sizeof ParticipantInfo is {Marshal.SizeOf(new Wreckfest2Structs.ParticipantInfo())}");
            Console.WriteLine($"sizeof PacketParticipantsLeaderboard is {Marshal.SizeOf(new Wreckfest2Structs.PacketParticipantsLeaderboard())}");
            Console.WriteLine($"sizeof PacketParticipantsMotion is {Marshal.SizeOf(new Wreckfest2Structs.PacketParticipantsMotion())}");
            Console.WriteLine($"sizeof PacketParticipantsTiming is {Marshal.SizeOf(new Wreckfest2Structs.PacketParticipantsTiming())}");
            Console.WriteLine($"sizeof PacketParticipantsTimingSectors is {Marshal.SizeOf(new Wreckfest2Structs.PacketParticipantsTimingSectors())}");
#endif
            telemetryDataSet = new TelemetryDataSet(this);
            settings = LoadSettings(Name);
            monitor = new ProcessMonitor(ExecutableProcessName);

            monitor.OnProcessRunningStateChanged += (object o, bool bRunning) =>
            {
                _bRunning = bRunning;

                if (bRunning)
                {
                    Run(settings.PortUDP, 5000);
                }
                else
                {
                    telemetryDataSet.LoadDefaults();
                    OnTelemetry?.Invoke(this, new TelemetryUpdatedEventArgs(telemetryDataSet));
                    Stop();
                }

                OnGameStateChanged?.Invoke(this, new EventArgs());
            };

            monitor.Start();
        }

        public void Finalize()
        {
            Stop();
            SaveSettings(settings, Name);
        }

        public override void OnDataReceived(ref byte[] bytes)
        {
            Wreckfest2Structs.PacketHeader header = Marshalizable<Wreckfest2Structs.PacketHeader>.FromBytes(bytes);

            switch (header.packetType)
            {
                case Wreckfest2Structs.PacketType.PACKET_TYPE_PARTICIPANTS_INFO:
                    {
                        //Wreckfest2Structs.PacketParticipantsInfo participantsInfo = Marshalizable<Wreckfest2Structs.PacketParticipantsInfo>.FromBytes(bytes);
                        // header 14
                        // enum 1
                        // 36x213 = 7668
                        // + reserved 510
                    }
                    break;

                case Wreckfest2Structs.PacketType.PACKET_TYPE_MAIN:
                    {
                        AMMotionData motionData = telemetryDataSet.CarData.MotionData;
                        
                        Wreckfest2Structs.PacketMain main = Marshalizable<Wreckfest2Structs.PacketMain>.FromBytes(bytes);

                        motionData.Surge = 0.3f * main.carPlayer.velocity.accelerationLocalZ * 0.10197162129779283f;
                        motionData.Heave = 0.3f * main.carPlayer.velocity.accelerationLocalY * 0.10197162129779283f;
                        motionData.Sway = -0.3f * main.carPlayer.velocity.accelerationLocalX * 0.10197162129779283f;

                        motionData.LocalVelocity = new float[] { main.carPlayer.velocity.velocityLocalX, main.carPlayer.velocity.velocityLocalY, main.carPlayer.velocity.velocityLocalZ };

                        Quaternion quaternion = new Quaternion(main.carPlayer.orientation.orientationQuaternionX, main.carPlayer.orientation.orientationQuaternionY, main.carPlayer.orientation.orientationQuaternionZ, main.carPlayer.orientation.orientationQuaternionW);

                        Vector3 pyr = Math2.GetPYRFromQuaternion(quaternion);

                        float pitch = -pyr.X;
                        float yaw = -pyr.Y;
                        float roll = Math2.LoopAngleRad(-pyr.Z, (float)Math.PI * 0.5f);

                        motionData.Pitch = 0.5f * pitch / (float)Math.PI;
                        motionData.Yaw = yaw / (float)Math.PI;
                        motionData.Roll = 0.5f * roll / (float)Math.PI;

                        telemetryDataSet.CarData.Speed = Math.Abs(main.carPlayer.driveline.speed * 3.6);
                        telemetryDataSet.CarData.RPM = main.carPlayer.engine.rpm;
                        telemetryDataSet.CarData.MaxRPM = main.carPlayer.engine.rpmMax;
                        telemetryDataSet.CarData.ShiftUpRPM = main.carPlayer.engine.rpmRedline;
                        telemetryDataSet.CarData.Gear = main.carPlayer.driveline.gear;
                        telemetryDataSet.CarData.WaterTemp = main.carPlayer.engine.tempWater;
                        telemetryDataSet.CarData.OilPressure = main.carPlayer.engine.pressureOil;
                        telemetryDataSet.CarData.Brake = main.carPlayer.input.brake;
                        telemetryDataSet.CarData.Clutch = main.carPlayer.input.clutch;
                        telemetryDataSet.CarData.Throttle = main.carPlayer.input.throttle;

                        telemetryDataSet.CarData.Lap = telemetryDataSet.SessionInfo.CurrentLapNumber = main.participantPlayerLeaderboard.lapCurrent;
                        telemetryDataSet.CarData.Position = telemetryDataSet.SessionInfo.CurrentPosition = main.participantPlayerLeaderboard.position;
                        telemetryDataSet.CarData.DriverName = main.participantPlayerInfo.playerName;
                        telemetryDataSet.CarData.CarName = main.participantPlayerInfo.carName;
                        telemetryDataSet.CarData.CarNumber = main.participantPlayerInfo.carId;

                        telemetryDataSet.SessionInfo.TrackName = main.session.trackName;
                        telemetryDataSet.SessionInfo.TrackLength = main.session.trackLength;
                        telemetryDataSet.SessionInfo.TotalLapsCount = main.session.laps;

                        telemetryDataSet.SessionInfo.CurrentLapTime = (int) main.participantPlayerTiming.lapTimeCurrent;
                        telemetryDataSet.SessionInfo.BestLapTime = (int)main.participantPlayerTiming.lapTimeBest;
                        telemetryDataSet.SessionInfo.LastLapTime = (int)main.participantPlayerTiming.lapTimeLast;
                        telemetryDataSet.SessionInfo.Sector1BestTime = (int)main.participantPlayerTimingSectors.sectorTimeBest1;
                        telemetryDataSet.SessionInfo.Sector2BestTime = (int)main.participantPlayerTimingSectors.sectorTimeBest2;
                        telemetryDataSet.SessionInfo.Sector3BestTime = (int)main.participantPlayerTimingSectors.sectorTimeBest3;

                        AMCarData carData = telemetryDataSet.CarData;

                        carData.Tires = carData.Tires ?? new AMTireData[4];

                        carData.Tires[0].Temperature[1] = main.carPlayer.tires[0].temperatureInner;
                        carData.Tires[1].Temperature[1] = main.carPlayer.tires[1].temperatureInner;
                        carData.Tires[2].Temperature[1] = main.carPlayer.tires[2].temperatureInner;
                        carData.Tires[3].Temperature[1] = main.carPlayer.tires[3].temperatureInner;

                        carData.Tires[0].Pressure = 140;
                        carData.Tires[1].Pressure = 140;
                        carData.Tires[2].Pressure = 140;
                        carData.Tires[3].Pressure = 140;

                        carData.Tires[0].Detached = main.participantPlayerDamage.damageStates[(int)Wreckfest2Structs.DamagePart.PART_TIRE_FL] == (byte)Wreckfest2Structs.DamageState.STATE_TERMINAL;
                        carData.Tires[1].Detached = main.participantPlayerDamage.damageStates[(int)Wreckfest2Structs.DamagePart.PART_TIRE_FR] == (byte)Wreckfest2Structs.DamageState.STATE_TERMINAL;
                        carData.Tires[2].Detached = main.participantPlayerDamage.damageStates[(int)Wreckfest2Structs.DamagePart.PART_TIRE_RL] == (byte)Wreckfest2Structs.DamageState.STATE_TERMINAL;
                        carData.Tires[3].Detached = main.participantPlayerDamage.damageStates[(int)Wreckfest2Structs.DamagePart.PART_TIRE_RR] == (byte)Wreckfest2Structs.DamageState.STATE_TERMINAL;

                        carData.Tires[0].Temperature[0] = carData.Tires[0].Temperature[2] = main.carPlayer.tires[0].temperatureTread;
                        carData.Tires[1].Temperature[0] = carData.Tires[1].Temperature[2] = main.carPlayer.tires[1].temperatureTread;
                        carData.Tires[2].Temperature[0] = carData.Tires[2].Temperature[2] = main.carPlayer.tires[2].temperatureTread;
                        carData.Tires[3].Temperature[0] = carData.Tires[3].Temperature[2] = main.carPlayer.tires[3].temperatureTread;

                        switch (main.marshalFlagsPlayer)
                        {
                            case Wreckfest2Structs.MarshalFlags.MARSHAL_FLAGS_GREEN:
                                telemetryDataSet.SessionInfo.Flag = "Green";
                                telemetryDataSet.CarData.Flags = TelemetryFlags.FlagGreen;
                                break;
                            case Wreckfest2Structs.MarshalFlags.MARSHAL_FLAGS_WARNING:
                                telemetryDataSet.SessionInfo.Flag = "Yellow";
                                telemetryDataSet.CarData.Flags = TelemetryFlags.FlagGreen | TelemetryFlags.FlagYellow;
                                break;
                            case Wreckfest2Structs.MarshalFlags.MARSHAL_FLAGS_FINISH:
                                telemetryDataSet.SessionInfo.Flag = "Finish";
                                telemetryDataSet.CarData.Flags = TelemetryFlags.FlagChequered;
                                break;
                            case Wreckfest2Structs.MarshalFlags.MARSHAL_FLAGS_BLUE:
                                telemetryDataSet.SessionInfo.Flag = "Blue";
                                telemetryDataSet.CarData.Flags = TelemetryFlags.FlagGreen | TelemetryFlags.FlagBlue;
                                break;
                            case Wreckfest2Structs.MarshalFlags.MARSHAL_FLAGS_LASTLAP:
                                telemetryDataSet.SessionInfo.Flag = "White";
                                telemetryDataSet.CarData.Flags = TelemetryFlags.FlagWhite;
                                break;
                            case Wreckfest2Structs.MarshalFlags.MARSHAL_FLAGS_DQ:
                                telemetryDataSet.SessionInfo.Flag = "Black";
                                telemetryDataSet.CarData.Flags = TelemetryFlags.FlagBlack;
                                break;
                            default:
                                telemetryDataSet.SessionInfo.Flag = String.Empty;
                                telemetryDataSet.CarData.Flags = TelemetryFlags.FlagNone;
                                break;
                        }

                        OnTelemetry?.Invoke(this, new TelemetryUpdatedEventArgs((TelemetryDataSet) telemetryDataSet.Clone()));
                    }
                    break;
            }
        }

        public Icon GetIcon()
        {
            return Properties.Resources.Wreckfest2;
        }

        public void ShowSettings(IVAzhureRacingApp app)
        {
            //
        }

        public void Start(IVAzhureRacingApp app)
        {
            if (settings.Executable != string.Empty)
            {
                if (Utils.ExecuteCmd(settings.Executable))
                    return;
            }

            if (!Utils.RunSteamGame(SteamGameID))
            {
                app.SetStatusText($"Steam Service is not running. Run {Name} manually!");
            }
        }

        public static GameSettings LoadSettings(string name)
        {
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetCallingAssembly().Location), $"{name}.json");
            if (File.Exists(path))
            {
                try
                {
                    string json = File.ReadAllText(path);

                    return ObjectSerializeHelper.DeserializeJson<GameSettings>(json);
                }
                catch { }
            }
            return new GameSettings();
        }

        public static void SaveSettings(GameSettings settings, string name)
        {
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetCallingAssembly().Location), $"{name}.json");
            string json = "";
            if (File.Exists(path))
            {
                try
                {
                    json = File.ReadAllText(path);
                }
                catch { }
            }
            string jsonNew = settings.GetJson();
            if (json != jsonNew)
            {
                try
                {
                    File.WriteAllText(path, jsonNew);
                }
                catch { }
            }
        }
    }
}