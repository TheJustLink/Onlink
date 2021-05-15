using System;

namespace OnlinkServer.Logging
{
    class ConsoleLogger : ILogger
    {
        public static ConsoleLogger Instance { get; } = new ConsoleLogger();
        private ConsoleLogger() { }

        public void Log(string msg)
        {
            CustomLog($"[LOG] {msg}", ConsoleColor.White);
        }
        public void Warning(string msg)
        {
            CustomLog($"[WARNING] {msg}", ConsoleColor.Yellow);
        }
        public void Error(string msg)
        {
            CustomLog($"[ERROR] {msg}", ConsoleColor.Red);
        }
        public void Exception(Exception exception)
        {
            CustomLog($"[EXCEPTION] {exception.Message} =>\n{exception.StackTrace}", ConsoleColor.Red);
        }

        public void CustomLog(string msg, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(msg);
        }
    }
}