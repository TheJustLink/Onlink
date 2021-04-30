using System;
using System.Text;

using System.Net.Sockets;

namespace OnlinkClient
{
    class Program
    {
        private const int ServerPort = 1337;
        private const string Localhost = "127.0.0.1";

        private static void Main()
        {
            var client = GetConnection(Localhost, ServerPort);

            try
            {
                Handle(client);
            }
            catch (Exception exception)
            {
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(exception.Message);
            }
            finally
            {
                client.Close();
            }

            Console.ReadKey(true);
        }

        private static TcpClient GetConnection(string host, int port)
        {
            return new TcpClient(host, port);
        }
        private static void Handle(TcpClient client)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Connected to " + client.Client.RemoteEndPoint);

            using var networkStream = client.GetStream();
            var buffer = new byte[256];
            var length = networkStream.Read(buffer, 0, buffer.Length);

            var message = Encoding.UTF8.GetString(buffer, 0, length);
            Console.WriteLine("Server message: " + message);
        }
    }
}
