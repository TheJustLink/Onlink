using System;
using System.Text;

using System.Net;
using System.Net.Sockets;

namespace OnlinkServer
{
    class Program
    {
        private const int ServerPort = 1337;

        private static void Main()
        {
            var listener = CreateListener();

            try
            {
                Handle(listener);
            }
            catch (Exception exception)
            {
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(exception);
            }
            finally
            {
                listener.Stop();
            }

            Console.ReadKey(true);
        }

        private static TcpListener CreateListener()
        {
            return new TcpListener(IPAddress.Any, ServerPort);
        }
        private static void Handle(TcpListener listener)
        {
            listener.Start();

            Console.ForegroundColor = ConsoleColor.Yellow;
            while (true)
            {
                Console.WriteLine("Wait connenction...");

                var client = listener.AcceptTcpClient();
                Console.WriteLine("Connected client - " + client.Client.LocalEndPoint);

                using var networkStream = client.GetStream();
                var buffer = Encoding.UTF8.GetBytes("Hello!");
                networkStream.Write(buffer, 0, buffer.Length);

                Console.WriteLine("Client disconnected - " + client.Client.LocalEndPoint);
                client.Close();

                Console.WriteLine();
            }
        }
    }
}
