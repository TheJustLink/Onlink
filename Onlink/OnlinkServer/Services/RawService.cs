using System;
using System.Collections.Generic;
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

        private bool IsAllClientsClosed
            => ClientHandlers.Count == 0;

        private readonly TcpListener TcpListener;
        private readonly List<Task> ClientHandlers;
        private readonly CancellationTokenSource CancellationTokenSource;

        public RawService(int port) : this(IPAddress.Any, port) { }
        public RawService(IPAddress networkInterface, int port)
        {
            NetworkInterface = networkInterface;
            Port = port;

            TcpListener = new TcpListener(NetworkInterface, Port);
            ClientHandlers = new List<Task>();
            CancellationTokenSource = new CancellationTokenSource();
        }

        protected override void OnStart()
            => TcpListener.Start();
        protected override void OnStop()
            => CloseClientHandlers();
        private void CloseClientHandlers()
        {
            TcpListener.Stop();
            CancellationTokenSource.Cancel();

            WaitCloseClientHandlers();
        }
        private void WaitCloseClientHandlers()
        {
            while (IsAllClientsClosed == false)
                Wait(10);
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

            var clientHandler = CreateClientHandler(client);
            ClientHandlers.Add(clientHandler);

            StartClientHandlerAsync(clientHandler);
        }

        private async void StartClientHandlerAsync(Task handler)
        {
            await handler.ContinueWith((handler) => ClientHandlers.Remove(handler));
        }
        private Task CreateClientHandler(TcpClient client)
        {
            return Task.Run(() => HandleClient(client), CancellationTokenSource.Token);
        }
        private void HandleClient(TcpClient client)
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