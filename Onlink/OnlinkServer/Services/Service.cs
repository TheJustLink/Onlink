using System;
using System.Threading;

using OnlinkServer.Logging;

namespace OnlinkServer.Services
{
    abstract class Service
    {
        public bool IsRunning { get; private set; }
        public string Name => GetType().Name;
        public ILogger Logger = EmptyLogger.Instance;

        public event Action Starting;
        public event Action Stopping;

        private Thread _thread;
        private bool _isStopping;

        public Service()
        {
            _thread = new Thread(ThreadLoop);
            _thread.IsBackground = true;
            _thread.Priority = ThreadPriority.Highest;
        }

        public void Start()
        {
            if (IsRunning) return;

            Logger.Log(Name + " starting...");
            Starting?.Invoke();

            _thread.Start();
        }
        public void Stop()
        {
            if (IsRunning == false) return;

            Logger.Log(Name + " stopping...");
            Stopping?.Invoke();

            _isStopping = true;
        }
        ~Service() => Stop();

        private void ThreadLoop()
        {
            Logger.Log(Name + " started");
            IsRunning = true;

            while (IsRunning && !_isStopping)
            {
                try
                {
                    Handle();
                }
                catch (Exception exception)
                {
                    Logger.Exception(exception);
                }
            }

            Logger.Log(Name + " stopped");
            _isStopping = false;
            IsRunning = false;
        }

        protected abstract void Handle();
        protected void Wait(int ms) => Thread.Sleep(ms);
    }
}