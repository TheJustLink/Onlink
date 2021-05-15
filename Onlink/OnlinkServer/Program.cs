using System;

using OnlinkServer.Logging;
using OnlinkServer.Services;

namespace OnlinkServer
{
    class Program
    {
        private static void Main()
        {
            SetupConsole();
            PrintIntro();

            var servicePool = new ServicePool();
            servicePool.Logger = ConsoleLogger.Instance;

            servicePool.AddServices(new RawService(), new TelegramService());
            servicePool.StartServices();

            HandleInput();

            servicePool.StopServices();
            PrintOutro();

            Console.ReadKey(true);
        }

        private static void SetupConsole()
        {
            Console.Title = "Onlink server";
            Console.CursorSize = 1;
        }
        private static void PrintIntro()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Welcome to Onlink server, agent!");
            Console.WriteLine();

            PrintAuthor();
        }
        private static void PrintAuthor()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("Developer ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Link");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Telegram @LINKICoder");
            Console.WriteLine();
        }
        private static void PrintOutro()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Bye, agent...");
        }

        private static void HandleInput()
        {
            Console.ReadKey(true);
        }
    }
}