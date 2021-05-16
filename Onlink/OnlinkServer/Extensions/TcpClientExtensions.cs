using System.Net.Sockets;

namespace OnlinkServer.Extensions
{
    static class TcpClientExtensions
    {
        public static bool GetConnectionState(this TcpClient client)
        {
            try
            {
                if (client == null || !client.Connected)
                    return false;

                var socket = client.Client;
                if (socket == null || !socket.Connected)
                    return false;

                // Detect if client disconnected
                if (socket.Poll(0, SelectMode.SelectRead))
                    return socket.Receive(new byte[1], SocketFlags.Peek) != 0;

                return true;
            }
            catch { return false; }
        }
    }
}