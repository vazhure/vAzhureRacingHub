using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace GT7Telemetry
{
    public class GT7TelemetryReceiver : IDisposable
    {
        private UdpClient udpClient;
        private Thread receiveThread;
        private Thread heartbeatThread;
        private bool isRunning = false;
        private string playstationIP;
        private int sendPort = 33739;    // Порт для heartbeat (отправка на PS)
        private int receivePort = 33740; // Порт для приёма телеметрии
        private Salsa20 salsa20;
        private IPEndPoint psEndPoint;
        private int packetCount = 0;
        private DateTime lastHeartbeatTime;

        // Событие для передачи распарсенного пакета в UI
        public event Action<GT7PacketA> OnPacketReceived;
        public event Action<string> OnLogMessage;
        public event Action<Exception> OnError;

        public bool IsRunning => isRunning;

        public GT7TelemetryReceiver(string playstationIP)
        {
            this.playstationIP = playstationIP;
            this.salsa20 = new Salsa20();
            
            // Ключ Salsa20: первые 32 байта строки
            string keyString = "Simulator Interface Packet GT7 ver 0.0";
            byte[] key = Encoding.ASCII.GetBytes(keyString);
            // Берём ровно 32 байта
            byte[] key32 = new byte[32];
            Buffer.BlockCopy(key, 0, key32, 0, 32);
            
            // Nonce будет формироваться из IV каждого пакета
            salsa20.SetKey(key32, new byte[8]);
            
            psEndPoint = new IPEndPoint(IPAddress.Parse(playstationIP), sendPort);
        }

        public void Start()
        {
            if (isRunning) return;
            
            isRunning = true;
            
            // Создаём UDP клиент для приёма (bind на 33740)
            udpClient = new UdpClient(receivePort);
            udpClient.Client.ReceiveBufferSize = 65535;
            
            // Запускаем потоки
            receiveThread = new Thread(ReceiveLoop) { IsBackground = true };
            receiveThread.Start();
            
            heartbeatThread = new Thread(HeartbeatLoop) { IsBackground = true };
            heartbeatThread.Start();
            
            Log("Приёмник запущен. Ожидание данных от PS...");
        }

        public void Stop()
        {
            isRunning = false;
            udpClient?.Close();
            udpClient?.Dispose();
            udpClient = null;
            
            // Ждём завершения потоков
            receiveThread?.Join(1000);
            heartbeatThread?.Join(1000);
            
            Log("Приёмник остановлен.");
        }

        private void ReceiveLoop()
        {
            IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
            
            while (isRunning)
            {
                try
                {
                    byte[] data = udpClient.Receive(ref remoteEndPoint);
                    
                    if (data.Length < 296) continue; // Минимальный размер пакета A
                    
                    // Расшифровываем
                    DecryptPacket(data);
                    
                    // Парсим структуру
                    GT7PacketA packet = ByteArrayToStructure<GT7PacketA>(data);
                    
                    packetCount++;
                    
                    // Вызываем событие в UI-потоке
                    OnPacketReceived?.Invoke(packet);
                }
                catch (ObjectDisposedException)
                {
                    break; // Нормальное завершение
                }
                catch (Exception ex)
                {
                    if (isRunning) OnError?.Invoke(ex);
                }
            }
        }

		private void HeartbeatLoop()
		{
			byte[] heartbeat = new byte[4];
			heartbeat[0] = (byte)'B';  // <-- БЫЛО 'A', СТАЛО 'B'
			heartbeat[1] = 0;
			heartbeat[2] = 0;
			heartbeat[3] = 0;
			
			while (isRunning)
			{
				try
				{
					udpClient?.Send(heartbeat, heartbeat.Length, psEndPoint);
				}
				catch (Exception ex) { /* ... */ }
				
				Thread.Sleep(5000); // Каждые 5 секунд
			}
		}

        private void DecryptPacket(byte[] data)
        {
            // Извлекаем IV из позиции 0x40 (64 байт)
            // IV = 4 байта, начиная с offset 0x40
			if (data.Length < 68) return;
			
			byte[] iv = new byte[4];
			Buffer.BlockCopy(data, 0x40, iv, 0, 4);
			uint iv1 = BitConverter.ToUInt32(iv, 0);
			
			uint iv2 = iv1 ^ 0xDEADBEEF;  // <-- 0xDEADBEAF (для A), 0xDEADBEEF (для B)
			
			byte[] nonce = new byte[8];
			Buffer.BlockCopy(BitConverter.GetBytes(iv2), 0, nonce, 0, 4);
			Buffer.BlockCopy(BitConverter.GetBytes(iv1), 0, nonce, 4, 4);
            
            // Переинициализируем Salsa20 с новым nonce
            string keyString = "Simulator Interface Packet GT7 ver 0.0";
            byte[] key = Encoding.ASCII.GetBytes(keyString);
            byte[] key32 = new byte[32];
            Buffer.BlockCopy(key, 0, key32, 0, 32);
            
            salsa20.SetKey(key32, nonce);
            salsa20.Decrypt(data, data.Length);
        }

        private T ByteArrayToStructure<T>(byte[] bytes) where T : struct
        {
            T structure;
            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            try
            {
                IntPtr ptr = handle.AddrOfPinnedObject();
                structure = Marshal.PtrToStructure<T>(ptr);
            }
            finally
            {
                handle.Free();
            }
            return structure;
        }

        private void Log(string message)
        {
            OnLogMessage?.Invoke($"[{DateTime.Now:HH:mm:ss.fff}] {message}");
        }

        public void Dispose()
        {
            Stop();
        }
    }
}