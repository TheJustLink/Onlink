using System;

namespace OnlinkServer.Logging
{
    class EmptyLogger : ILogger
    {
        public static EmptyLogger Instance { get; } = new EmptyLogger();
        private EmptyLogger() { }

        public void Error(string msg) { }
        public void Exception(Exception exception) { }
        public void Log(string msg) { }
        public void Warning(string msg) { }
    }
}