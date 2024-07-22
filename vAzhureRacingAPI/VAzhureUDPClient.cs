using System;
using System.Net;
using System.Net.Sockets;
using System.Timers;

namespace vAzhureRacingAPI
{
    public class VAzhureUDPClient
    {
        public event EventHandler OnConnectionError;
        public event EventHandler OnTimeout;
        public event EventHandler<byte[]> OnDataReceivedEvent;

        private struct UdpState
        {
            public UdpClient client;
            public IPEndPoint lep;
        }

        UdpState _udpstate = new UdpState();

        readonly Timer timer = new Timer();
        DateTime dtLast = new DateTime();

        private volatile bool m_bRunning = false;

        public void Run(int listenPort, int timeout = 1000)
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
                timer.Stop();
                timer.Interval = timeout;
                timer.AutoReset = true;
                timer.Elapsed += delegate (object sender, ElapsedEventArgs e)
                {
                    TimeSpan ts = DateTime.Now - dtLast;
                    if (ts.TotalMilliseconds > timeout)
                    {
                        OnTimeout?.Invoke(this, new EventArgs());
                        timer.Enabled = false;
                    }
                };
                timer.Start();
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
            timer.Stop();
        }

        public virtual void OnDataReceived(ref byte[] bytes)
        {
            OnDataReceivedEvent?.Invoke(this, bytes);
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
                    dtLast = DateTime.Now;
                    timer.Enabled = true;
                }
            }
        }
    }
}