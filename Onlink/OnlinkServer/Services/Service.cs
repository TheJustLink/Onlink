using System;
using System.Threading;

using OnlinkServer.Logs;

namespace OnlinkServer.Services
{
    abstract class Service
    {
        public bool IsRunning { get; private set; }
        public string Name => GetType().Name;

        public ILogger Logger = EmptyLogger.Instance;

        private Thread _thread;
        private bool _isInterrupting;

        public void Start()
        {
            if (IsRunning) Stop();

            Logger.Log(Name + " starting...");

            _thread = new Thread(ThreadLoop);
            _thread.IsBackground = true;
            _thread.Priority = ThreadPriority.Highest;
            _thread.Start();

            Logger.Log(Name + " started");
        }
        public void Stop()
        {
            if (IsRunning == false)
                return;

            Logger.Log(Name + " stopping...");

            _isInterrupting = true;
            _thread.Interrupt();
        }
        ~Service() => Stop();

        private void ThreadLoop()
        {
            IsRunning = true;

            while (IsRunning && !_isInterrupting)
            {
                try
                {
                    Handle();
                }
                catch (ThreadInterruptedException) { }
                catch (Exception exception)
                {
                    Logger.Exception(exception);
                }
            }

            Logger.Log(Name + " stopped");

            _isInterrupting = false;
            IsRunning = false;
        }

        protected abstract void Handle();
        protected void Wait(int ms) => Thread.Sleep(ms);
    }
}