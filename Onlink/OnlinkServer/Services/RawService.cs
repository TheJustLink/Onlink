using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace OnlinkServer.Services
{
    class RawService : Service
    {
        public readonly IPAddress NetworkInterface;
        public readonly int Port;

        private readonly TcpListener TcpListener;
        private readonly CancellationTokenSource CancellationTokenSource;

        public RawService(int port) : this(IPAddress.Any, port) { }
        public RawService(IPAddress networkInterface, int port)
        {
            NetworkInterface = networkInterface;
            Port = port;

            TcpListener = new TcpListener(NetworkInterface, Port);
            CancellationTokenSource = new CancellationTokenSource();

            Starting += OnStarting;
            Stopping += OnStopping;
        }

        private void OnStarting()
            => TcpListener.Start();
        private void OnStopping()
        {
            TcpListener.Stop();
            CancellationTokenSource.Cancel();
        }

        protected override void Handle()
        {
            Logger.Log("Hi from " + Name);
            Wait(3000);

            var hasConnection = TcpListener.Pending();
            if (!hasConnection) return;

            var client = TcpListener.AcceptTcpClient();
            Logger.Log("Connected - " + client.Client.RemoteEndPoint);

            HandleClientAsync(client);
        }

        private async void HandleClientAsync(TcpClient client)
        {
            await Task.Run(() =>
            {
                while (client.Connected && !CancellationTokenSource.Token.IsCancellationRequested)
                    HandleClient(client);

                Logger.Log("Client disconnected :/");
            }, CancellationTokenSource.Token);
        }
        private void HandleClient(TcpClient client)
        {
            Logger.Log("I AM NIGGA - " + client.Client.RemoteEndPoint);
            Wait(3000);
        }
    }
}