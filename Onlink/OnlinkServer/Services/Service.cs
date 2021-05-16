using System;
using System.Threading;

using OnlinkServer.Logging;

namespace OnlinkServer.Services
{
    abstract class Service
    {
        public bool IsRunning
        {
            get => _isRunning;
            private set
            {
                if (value)
                {
                    Logger.Log(Name + " started");
                }
                else
                {
                    _isStopping = false;
                    Logger.Log(Name + " stopped");
                }
                _isRunning = value;
            }
        }
        public string Name => GetType().Name;
        public ILogger Logger = EmptyLogger.Instance;

        private Thread _thread;
        private bool _isRunning;
        private bool _isStopping;

        public Service()
        {
            _thread = new Thread(ThreadHandler);
            _thread.IsBackground = true;
            _thread.Priority = ThreadPriority.Highest;
        }

        public void Start()
        {
            if (IsRunning) return;

            Logger.Log(Name + " starting...");
            _thread.Start();
        }
        public void Stop()
        {
            if (IsRunning == false) return;

            Logger.Log(Name + " stopping...");
            _isStopping = true;
        }
        ~Service() => Stop();

        private void ThreadHandler()
        {
            OnStart();
            IsRunning = true;

            while (IsRunning)
            {
                try
                {
                    ThreadLoopTick();
                }
                catch (Exception exception)
                {
                    Logger.Exception(exception);
                }
            }
        }
        private void ThreadLoopTick()
        {
            if (_isStopping)
            {
                OnStop();
                IsRunning = false;
            }
            else Handle();
        }

        protected abstract void OnStart();
        protected abstract void Handle();
        protected abstract void OnStop();

        protected void Wait(int ms) => Thread.Sleep(ms);
    }
}