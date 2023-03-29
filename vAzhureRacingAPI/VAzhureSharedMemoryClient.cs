using System;
using System.Threading;

namespace vAzhureRacingAPI
{
    public class VAzhureSharedMemoryClient
    {
        private volatile bool bRunning = false;
        private volatile bool bAbort = false;

        public event EventHandler OnThreadError;

        /// <summary>
        /// Main thread
        /// </summary>
        private Thread thread = null;

        public void StartThread()
        {
            if (thread == null)
            {
                bAbort = false;
                thread = new Thread(new ThreadStart(Main));
                thread.Start();
            }
        }

        public bool IsProcessRunning => bRunning;
        private const int cTimeOut = 5000;

        public void StopTrhead()
        {
            if (bRunning)
            {
                bAbort = true;

                DateTime dt = DateTime.Now;

                while (bRunning && (DateTime.Now - dt).TotalMilliseconds < cTimeOut)
                    Thread.Sleep(100);
            }

            thread = null;
            bRunning = false;
        }

        private void Main()
        {
            bRunning = true;

            try
            {
                while (bRunning && !bAbort)
                {
                    UserFunc();
                }
            }
            catch
            {
                OnThreadError?.Invoke(this, new EventArgs());
            }
            finally
            {
                bRunning = false;
            }
        }

        public virtual void UserFunc()
        {
            try
            {
                Thread.Sleep(1000);
            }
            catch { }
        }
    }
}
