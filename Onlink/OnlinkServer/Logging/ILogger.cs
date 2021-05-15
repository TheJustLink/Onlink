using System;

namespace OnlinkServer.Logging
{
    interface ILogger
    {
        void Log(string msg);
        void Warning(string msg);
        void Error(string msg);
        void Exception(Exception exception);
    }
}