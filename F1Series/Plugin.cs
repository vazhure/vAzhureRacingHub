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

        public bool Initialize(IVAzhureRacingApp app)
        {
            // TODO

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

            return true;
        }

        public void Quit(IVAzhureRacingApp app)
        {
            // TODO
        }
    }
}