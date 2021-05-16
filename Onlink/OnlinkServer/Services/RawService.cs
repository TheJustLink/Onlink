using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using OnlinkServer.Extensions;

namespace OnlinkServer.Services
{
    class RawService : Service
    {
        public const int AcceptTickRate = 10;
        public const int ClientTickRate = 1;

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
            var hasConnection = TcpListener.Pending();
            if (hasConnection)
                ReceiveConnection();

            Wait(AcceptTickRate);
        }
        private void ReceiveConnection()
        {
            var client = TcpListener.AcceptTcpClient();
            Logger.Log("Connected - " + client.Client.RemoteEndPoint);

            HandleClientAsync(client);
        }

        private async void HandleClientAsync(TcpClient client)
        {
            await Task.Run(() =>
            {
                var stream = client.GetStream();
                var lastHandleTime = DateTime.Now;
                var deltaTime = 0d;

                while (IsRunning && client.GetConnectionState())
                {
                    if (CancellationTokenSource.IsCancellationRequested)
                        break;

                    deltaTime = (DateTime.Now - lastHandleTime).TotalSeconds;
                    lastHandleTime = DateTime.Now;

                    HandleClient(stream, deltaTime);

                    Wait(ClientTickRate);
                }

                stream.Dispose();
                client.Close();

                Logger.Log("Client disconnected :/");
            }, CancellationTokenSource.Token);
        }

        private double _elapsedTime;
        private void HandleClient(NetworkStream stream, double deltaTime)
        {
            _elapsedTime += deltaTime;

            if (_elapsedTime < 3)
                return;
            else _elapsedTime = 0;

            var buffer = Encoding.UTF8.GetBytes("Aboba");
            stream.Write(buffer, 0, buffer.Length);
            Logger.Log("<Msg sended>");
        }
    }
}