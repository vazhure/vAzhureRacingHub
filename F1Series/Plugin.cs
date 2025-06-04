using System;
using System.Runtime.InteropServices;
using vAzhureRacingAPI;

namespace F1Series
{
    public class Plugin : ICustomPlugin
    {
        public string Name => "F1 Series Plugin";

        public string Description => "F1 Series Plugin";

        public ulong Version => 1UL;

        public bool CanClose(IVAzhureRacingApp _)
        {
            return true;
        }

        private readonly F1Game f12022Game = new F1Game(2022);
        private readonly F1Game f12023Game = new F1Game(2023);
        private readonly F1Game f12024Game = new F1Game(2024);
        private readonly F1Game f12025Game = new F1Game(2025);

        public bool Initialize(IVAzhureRacingApp app)
        {
            try
            {
                app.RegisterGame(f12022Game);
                app.RegisterGame(f12023Game);
                app.RegisterGame(f12024Game);
                app.RegisterGame(f12025Game);
            }
            catch { return false; }

            // F1 2022
            //Console.WriteLine($"Packet size of PacketCarDamageData {Marshal.SizeOf(typeof(F12022.PacketCarDamageData))}");
            //Console.WriteLine($"Packet size of PacketCarSetupData {Marshal.SizeOf(typeof(F12022.PacketCarSetupData))}");
            //Console.WriteLine($"Packet size of PacketCarStatusData {Marshal.SizeOf(typeof(F12022.PacketCarStatusData))}");
            //Console.WriteLine($"Packet size of PacketCarTelemetryData {Marshal.SizeOf(typeof(F12022.PacketCarTelemetryData))}");
            //Console.WriteLine($"Packet size of PacketEventData {Marshal.SizeOf(typeof(F12022.PacketEventData))}");
            //Console.WriteLine($"Packet size of PacketFinalClassificationData {Marshal.SizeOf(typeof(F12022.PacketFinalClassificationData))}");
            //Console.WriteLine($"Packet size of PacketLapData {Marshal.SizeOf(typeof(F12022.PacketLapData))}");
            //Console.WriteLine($"Packet size of PacketLobbyInfoData {Marshal.SizeOf(typeof(F12022.PacketLobbyInfoData))}");
            //Console.WriteLine($"Packet size of PacketMotionData {Marshal.SizeOf(typeof(F12022.PacketMotionData))}");
            //Console.WriteLine($"Packet size of PacketParticipantsData {Marshal.SizeOf(typeof(F12022.PacketParticipantsData))}");
            //Console.WriteLine($"Packet size of PacketSessionData {Marshal.SizeOf(typeof(F12022.PacketSessionData))}");
            //Console.WriteLine($"Packet size of PacketSessionHistoryData {Marshal.SizeOf(typeof(F12022.PacketSessionHistoryData))}");

            // F1 2023
            //Console.WriteLine($"Packet size of PacketCarDamageData {Marshal.SizeOf(typeof(F12023.PacketCarDamageData))}");
            //Console.WriteLine($"Packet size of PacketCarSetupData {Marshal.SizeOf(typeof(F12023.PacketCarSetupData))}");
            //Console.WriteLine($"Packet size of PacketCarStatusData {Marshal.SizeOf(typeof(F12023.PacketCarStatusData))}");
            //Console.WriteLine($"Packet size of PacketCarTelemetryData {Marshal.SizeOf(typeof(F12023.PacketCarTelemetryData))}");
            //Console.WriteLine($"Packet size of PacketEventData {Marshal.SizeOf(typeof(F12023.PacketEventData))}");
            //Console.WriteLine($"Packet size of PacketFinalClassificationData {Marshal.SizeOf(typeof(F12023.PacketFinalClassificationData))}");
            //Console.WriteLine($"Packet size of PacketLapData {Marshal.SizeOf(typeof(F12023.PacketLapData))}");
            //Console.WriteLine($"Packet size of PacketLobbyInfoData {Marshal.SizeOf(typeof(F12023.PacketLobbyInfoData))}");
            //Console.WriteLine($"Packet size of PacketMotionData {Marshal.SizeOf(typeof(F12023.PacketMotionData))}");
            //Console.WriteLine($"Packet size of PacketParticipantsData {Marshal.SizeOf(typeof(F12023.PacketParticipantsData))}");
            //Console.WriteLine($"Packet size of PacketSessionData {Marshal.SizeOf(typeof(F12023.PacketSessionData))}");
            //Console.WriteLine($"Packet size of PacketSessionHistoryData {Marshal.SizeOf(typeof(F12023.PacketSessionHistoryData))}");
            //Console.WriteLine($"Packet size of PacketMotionExData {Marshal.SizeOf(typeof(F12023.PacketMotionExData))}");
            //Console.WriteLine($"Packet size of PacketTyreSetsData {Marshal.SizeOf(typeof(F12023.PacketTyreSetsData))}");

            // F1 2024
            //Console.WriteLine($"Packet size of PacketMotionData {Marshal.SizeOf(typeof(F12024.PacketMotionData))}");
            //Console.WriteLine($"Packet size of PacketMotionExData {Marshal.SizeOf(typeof(F12024.PacketMotionExData))}");
            //Console.WriteLine($"Packet size of PacketLapData {Marshal.SizeOf(typeof(F12024.PacketLapData))}");
            //Console.WriteLine($"Packet size of PacketLobbyInfoData {Marshal.SizeOf(typeof(F12024.PacketLobbyInfoData))}");
            //Console.WriteLine($"Packet size of PacketParticipantsData {Marshal.SizeOf(typeof(F12024.PacketParticipantsData))}");
            //Console.WriteLine($"Packet size of PacketSessionData {Marshal.SizeOf(typeof(F12024.PacketSessionData))}");
            //Console.WriteLine($"Packet size of PacketSessionHistoryData {Marshal.SizeOf(typeof(F12024.PacketSessionHistoryData))}");
            //Console.WriteLine($"Packet size of PacketTimeTrialData {Marshal.SizeOf(typeof(F12024.PacketTimeTrialData))}");
            //Console.WriteLine($"Packet size of PacketTyreSetsData {Marshal.SizeOf(typeof(F12024.PacketTyreSetsData))}");
            //Console.WriteLine($"Packet size of PacketCarDamageData {Marshal.SizeOf(typeof(F12024.PacketCarDamageData))}");
            //Console.WriteLine($"Packet size of PacketCarSetupData {Marshal.SizeOf(typeof(F12024.PacketCarSetupData))}");
            //Console.WriteLine($"Packet size of PacketCarStatusData {Marshal.SizeOf(typeof(F12024.PacketCarStatusData))}");
            //Console.WriteLine($"Packet size of PacketCarTelemetryData {Marshal.SizeOf(typeof(F12024.PacketCarTelemetryData))}");
            //Console.WriteLine($"Packet size of PacketEventData {Marshal.SizeOf(typeof(F12024.PacketEventData))}");



            // F1 2025
            //Console.WriteLine($"Packet size of PacketMotionData {Marshal.SizeOf(typeof(F12025.PacketMotionData))}"); //1349
            //Console.WriteLine($"Packet size of PacketMotionExData {Marshal.SizeOf(typeof(F12025.PacketMotionExData))}"); //273
            //Console.WriteLine($"Packet size of PacketLapData {Marshal.SizeOf(typeof(F12025.PacketLapData))}"); //1285
            //Console.WriteLine($"Packet size of PacketLobbyInfoData {Marshal.SizeOf(typeof(F12025.PacketLobbyInfoData))}"); //954
            //Console.WriteLine($"Packet size of PacketParticipantsData {Marshal.SizeOf(typeof(F12025.PacketParticipantsData))}"); //1284
            //Console.WriteLine($"Packet size of PacketSessionData {Marshal.SizeOf(typeof(F12025.PacketSessionData))}"); //753
            //Console.WriteLine($"Packet size of PacketSessionHistoryData {Marshal.SizeOf(typeof(F12025.PacketSessionHistoryData))}"); // 1460
            //Console.WriteLine($"Packet size of PacketTimeTrialData {Marshal.SizeOf(typeof(F12025.PacketTimeTrialData))}"); // 101
            //Console.WriteLine($"Packet size of PacketTyreSetsData {Marshal.SizeOf(typeof(F12025.PacketTyreSetsData))}"); // 231
            //Console.WriteLine($"Packet size of PacketCarDamageData {Marshal.SizeOf(typeof(F12025.PacketCarDamageData))}"); // 1041
            //Console.WriteLine($"Packet size of PacketCarSetupData {Marshal.SizeOf(typeof(F12025.PacketCarSetupData))}"); // 1133
            //Console.WriteLine($"Packet size of PacketCarStatusData {Marshal.SizeOf(typeof(F12025.PacketCarStatusData))}"); //1239
            //Console.WriteLine($"Packet size of PacketCarTelemetryData {Marshal.SizeOf(typeof(F12025.PacketCarTelemetryData))}"); // X
            //Console.WriteLine($"Packet size of PacketEventData {Marshal.SizeOf(typeof(F12025.PacketEventData))}"); // 45

            return true;
        }

        public void Quit(IVAzhureRacingApp app)
        {
            f12022Game.Quit();
            f12023Game.Quit();
            f12024Game.Quit();
            f12025Game.Quit();
        }
    }
}