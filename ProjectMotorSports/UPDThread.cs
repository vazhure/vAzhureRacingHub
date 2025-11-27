using System;
using System.Net;
using System.Net.Sockets;

namespace ProjectMotorSports
{
    internal class UDPThread : IDisposable
    {
        private IPEndPoint m_endPoint = null;
        private IPAddress m_multicastGroup = null;
        private UdpClient m_udpClient = null;
        private readonly bool m_multiCast = true;
        public const int cDefaultPort = 7576;
        public const string cDefaultMulticastGroup = "224.0.0.150";
        public const UInt16 cExpectedVersion = 1;
        private readonly int m_port = cDefaultPort;
        private readonly string m_sMulticastGroup = cDefaultMulticastGroup;

        public EventHandler<UDPRaceInfo> OnRaceInfo;
        public EventHandler<UDPVehicleTelemetry> OnVehicleTelemetry;
        public EventHandler<UDPParticipantRaceState> OnParticipantRaceState;
        public EventHandler OnUdpClosed;

        private DataStore m_dataStore = new DataStore();

        public DataStore DataStore
        {
            get => m_dataStore;
        }

        public UDPThread(int port = cDefaultPort, string multicastGroup = cDefaultMulticastGroup, bool bMulticast = true)
        {
            m_multiCast = bMulticast;
            m_port = port;
            m_sMulticastGroup = multicastGroup;
        }

        private bool m_bRunning = false;

        public void Start()
        {
            m_dataStore = new DataStore();

            if (m_multiCast)
            {
                m_udpClient = new UdpClient(m_port)
                {
                    DontFragment = true
                };

                m_multicastGroup = IPAddress.Parse(m_sMulticastGroup);
                m_endPoint = new IPEndPoint(m_multicastGroup, m_port);
                m_udpClient.JoinMulticastGroup(m_multicastGroup);
            }
            else
            {
                m_endPoint = new IPEndPoint(IPAddress.Any, m_port);
                m_udpClient = new UdpClient(m_endPoint);
            }

            m_bRunning = true;
            m_udpClient.BeginReceive(new AsyncCallback(OnUdpData), m_udpClient);
        }

        public void Stop()
        {
            m_bRunning = false;
            try
            {
                m_udpClient?.Dispose();
            }
            catch
            {

            }
            finally
            {
                m_udpClient = null;
                m_endPoint = null;
            }
        }

        private void OnUdpData(IAsyncResult result)
        {
            if (m_bRunning && m_udpClient != null)
            {
                try
                {
                    byte[] bytes = m_udpClient.EndReceive(result, ref m_endPoint);

                    DecodePacket(ref bytes);
                }
                catch
                {
                }
                finally
                {
                    m_udpClient?.BeginReceive(new AsyncCallback(OnUdpData), m_udpClient);
                }
            }
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

        public void Dispose()
        {
            Stop();
        }
    }
}