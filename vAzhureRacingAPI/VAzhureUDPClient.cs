using System;
using System.Net;
using System.Net.Sockets;

namespace vAzhureRacingAPI
{
    public class VAzhureUDPClient
    {
        public event EventHandler OnConnectionError;
        public event EventHandler<byte[]> OnDataRecievedEvent;

        private struct UdpState
        {
            public UdpClient client;
            public IPEndPoint lep;
        }

        UdpState _udpstate = new UdpState();

        private volatile bool m_bRunning = false;

        public void Run(int listenPort)
        {
            if (_udpstate.client == null)
            {
                _udpstate.client = new UdpClient(listenPort);
                _udpstate.lep = new IPEndPoint(IPAddress.Any, listenPort);
            }
            try
            {
                m_bRunning = true;
                _udpstate.client.BeginReceive(new AsyncCallback(OnUdpData), _udpstate);
            }
            catch
            {
                OnConnectionError?.Invoke(this, new EventArgs());
            }
        }

        public void Stop()
        {
            m_bRunning = false;
            _udpstate.client?.Dispose();
            _udpstate.client = null;
            _udpstate.lep = null;
        }

        public virtual void OnDataReceived(ref byte[] bytes)
        {
            OnDataRecievedEvent?.Invoke(this, bytes);
        }

        private void OnUdpData(IAsyncResult result)
        {
            if (m_bRunning)
            {
                try
                {
                    UdpState s = (UdpState)result.AsyncState;

                    byte[] bytes = s.client.EndReceive(result, ref s.lep);

                    OnDataReceived(ref bytes);
                }
                catch
                {
                }
                finally
                {
                    _udpstate.client?.BeginReceive(new AsyncCallback(OnUdpData), _udpstate);
                }
            }
        }
    }
}