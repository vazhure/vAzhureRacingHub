using System;
using System.Threading;
using System.Threading.Tasks;

namespace vAzhureRacingAPI
{
    public class ProcessMonitor
    {
        Task m_task;
        readonly int m_interval = 1000;
        readonly string [] m_process = { };
        internal volatile bool bRunning = false;
        public bool IsRunning => bRunning;
        public int Interval => m_interval;
        public string [] ProcessNames => m_process;

        /// <summary>
        /// Process monitor service
        /// </summary>
        /// <param name="processNames">List of process names (without .exe)</param>
        /// <param name="interval">monitoring interval, ms</param>
        public ProcessMonitor(string[] processNames, int interval = 1000)
        {
            m_interval = interval;
            m_process = processNames;
        }

        /// <summary>
        /// Start monitoring
        /// </summary>
        public void Start()
        {
            Stop();
            bRunning = true;
            m_task = new Task(worker, this);
            m_task.Start();
        }

        /// <summary>
        /// Stop monitoring
        /// </summary>
        public void Stop()
        {
            bRunning = false;
            m_task?.Wait();
            m_task?.Dispose();
        }

        public event EventHandler<bool> OnProcessRunningStateChanged;

        internal void NotifyState(bool bProcessRunning)
        {
            OnProcessRunningStateChanged?.Invoke(this, bProcessRunning);
        }

        readonly Action<object> worker = (object obj) =>
        {
            bool bRunning = false;
            if (obj is ProcessMonitor monitor)
            {
                while (monitor.IsRunning)
                {
                    if (Utils.IsProcessRunning(monitor.ProcessNames))
                    {
                        if (!bRunning)
                        {
                            bRunning = true;
                            monitor.NotifyState(bRunning);
                        }
                    }
                    else
                    {
                        if (bRunning)
                        {
                            bRunning = false;
                            monitor.NotifyState(bRunning);
                        }
                    }
                    Thread.Sleep(Math.Max(10, monitor.Interval));
                }
            }
        };
    }
}
