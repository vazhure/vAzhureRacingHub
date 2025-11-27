using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ProjectMotorSports
{
    internal class UDPThread : IDisposable
    {
        static IPEndPoint m_endPoint = null;
        static IPAddress m_multicastGroup = null;
        static UdpClient m_udpClient = null;
        static bool m_multiCast = true;
        static readonly int m_defaultPort = 7576;
        static readonly string m_defaultMulticastGroup = "224.0.0.150";
        public readonly static UInt16 m_expectedVersion = 1;

        public EventHandler<UDPRaceInfo> OnRaceInfo;
        public EventHandler<UDPVehicleTelemetry> OnVehicleTelemetry;
        public EventHandler<UDPParticipantRaceState> OnParticipantRaceState;
        public EventHandler OnUdpClosed;

        private DataStore m_dataStore = new DataStore();

        public DataStore DataStore
        {
            get => m_dataStore;
        }

        public UDPThread( string[] args)
        {
            m_multiCast = GetOptionValue<bool>(args, "multicast", true);
            int port = GetOptionValue<int>(args, "port", m_defaultPort);
            string multicastGroup = GetOptionValue<string>(args, "multicast_group", m_defaultMulticastGroup);

            if (m_multiCast)
            {
                m_udpClient = new UdpClient(port);
                m_multicastGroup = IPAddress.Parse(multicastGroup);
                m_endPoint = new IPEndPoint(m_multicastGroup, port);
                m_udpClient.JoinMulticastGroup(m_multicastGroup);
            }
            else
            {
                m_endPoint = new IPEndPoint(IPAddress.Any, port);
                m_udpClient = new UdpClient(m_endPoint);
            }
        }

        private T GetOptionValue<T>(string[] args, string option, T defaultValue)
        {
            foreach (string s in args)
            {
                if (s.Contains(option))
                {
                    string[] split = s.Split('=');
                    if (split.Length > 1)
                    {
                        return (T)Convert.ChangeType(split[1], typeof(T));
                    }
                    else
                    {
                        return defaultValue;
                    }
                }
            }

            return defaultValue;
        }

        private void DecodePacket(ref byte[] data)
        {
            byte packetType = data[0];
            if (packetType == (byte)UDPPacketType.RaceInfo)
            {
                UDPRaceInfo p = UDPRaceInfo.Decode(ref data, 1);
                m_dataStore.WriteRaceInfo(p);
                OnRaceInfo?.Invoke(this, p);
            }
            else if (packetType == (byte)UDPPacketType.ParticipantRaceState)
            {
                UDPParticipantRaceState p = UDPParticipantRaceState.Decode(ref data, 1);
                m_dataStore.WriteRaceState(p);
                OnParticipantRaceState?.Invoke(this, p);
            }
            else if (packetType == (byte)UDPPacketType.ParticipantVehicleTelemetry)
            {
                UDPVehicleTelemetry p = UDPVehicleTelemetry.Decode(ref data, 1);
                m_dataStore.WriteTelemetry(p);
                OnVehicleTelemetry?.Invoke(this, p);
            }

            m_dataStore.WriteTimestamp();
        }

        private void DoWork(object thread)
        {
            // Create an endpoint to store the sender's address
            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);

            while (!m_aborted)
            {
                // Receive data from a client
                try
                {
                    byte[] data = m_udpClient.Receive(ref sender);
                    DecodePacket(ref data);
                }
                catch (SocketException)
                {
                    OnUdpClosed?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public void StartThread()
        {
            m_aborted = false;
            m_dataStore = new DataStore();
            m_updateThread = new Thread(new ParameterizedThreadStart(DoWork));
            m_updateThread.Start(this);
        }

        public void StopThread()
        {
            if (!m_aborted)
            {
                m_aborted = true;
                try
                {
                    if (!m_updateThread.Join(1000))
                    {
                        m_updateThread.Abort();
                    }
                }
                catch { }
                m_dataStore = new DataStore();
            }
        }

        public void Dispose()
        {
            m_udpClient?.Close();
        }

        private volatile bool m_aborted = false;
        private Thread m_updateThread;
    }
}