
using System;
using System.Runtime.InteropServices;
using vAzhureRacingAPI;

#if DEBUG
using static BusBound.BusGameTelemetry;
#endif

namespace BusBound
{
    public class Plugin : ICustomPlugin
    {
        public string Name => "Bus Bound Game";

        public string Description => "Bus Bound Game Plugin";

        public ulong Version => 1UL;

        public bool CanClose(IVAzhureRacingApp app)
        {
            return true;
        }

        private BusGame busGame;

        public bool Initialize(IVAzhureRacingApp app)
        {
            try
            {
                busGame = new BusGame();
                app.RegisterGame(busGame);
#if DEBUG
                Console.WriteLine($"Gear: {sizeof(Gear)}");
                Console.WriteLine($"Vector3: {Marshal.SizeOf(typeof(Vector3))}");
                Console.WriteLine($"Color: {Marshal.SizeOf(typeof(Color))}");
                Console.WriteLine($"Vector2: {Marshal.SizeOf(typeof(Vector2))}");
                Console.WriteLine($"Quaternion: {Marshal.SizeOf(typeof(Quaternion))}");
                Console.WriteLine($"EulerAngles: {Marshal.SizeOf(typeof(EulerAngles))}");
                Console.WriteLine($"Wheel: {Marshal.SizeOf(typeof(Wheel))}");
                Console.WriteLine($"Engine: {Marshal.SizeOf(typeof(Engine))}");
                Console.WriteLine($"Door: {Marshal.SizeOf(typeof(Door))}");
                Console.WriteLine($"Radio: {Marshal.SizeOf(typeof(Radio))}");
                Console.WriteLine($"Ramp: {Marshal.SizeOf(typeof(Ramp))}");
                Console.WriteLine($"Seating: {Marshal.SizeOf(typeof(Seating))}");
                Console.WriteLine($"DestinationData: {Marshal.SizeOf(typeof(DestinationData))}");
                Console.WriteLine($"PlayerBus: {Marshal.SizeOf(typeof(PlayerBus))}");
                Console.WriteLine($"Weather: {Marshal.SizeOf(typeof(Weather))}");
                Console.WriteLine($"ShiftData: {Marshal.SizeOf(typeof(ShiftData))}");
                Console.WriteLine($"LineStop: {Marshal.SizeOf(typeof(LineStop))}");
                Console.WriteLine($"Line: {Marshal.SizeOf(typeof(Line))}");
                Console.WriteLine($"Mood: {Marshal.SizeOf(typeof(Mood))}");
                Console.WriteLine($"Incident: {Marshal.SizeOf(typeof(Incident))}");
                Console.WriteLine($"Perk: {Marshal.SizeOf(typeof(Perk))}");
                Console.WriteLine($"LineDrive: {Marshal.SizeOf(typeof(LineDrive))}");
                Console.WriteLine($"District: {Marshal.SizeOf(typeof(District))}");
                Console.WriteLine($"Game: {Marshal.SizeOf(typeof(Game))}");
#endif
            }
            catch
            {
                return false;
            }
            return true;
        }

        public void Quit(IVAzhureRacingApp app)
        {
            
        }
    }
}
