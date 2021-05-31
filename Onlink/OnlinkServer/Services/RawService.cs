using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace OnlinkServer.Services
{
    class RawService : Service
    {
        public const int TickRate = 10;

        public readonly IPAddress NetworkInterface;
        public readonly int Port;

        private readonly TcpListener TcpListener;
        private readonly List<NetworkClient> NetworkClients;

        public RawService(int port) : this(IPAddress.Any, port) { }
        public RawService(IPAddress networkInterface, int port)
        {
            NetworkInterface = networkInterface;
            Port = port;

            TcpListener = new TcpListener(NetworkInterface, Port);
            NetworkClients = new List<NetworkClient>();
        }

        protected override void OnStart()
        {
            TcpListener.Start();
        }
        protected override void OnStop()
        {
            TcpListener.Stop();
            DisconnectAllClients();
        }
        private void DisconnectAllClients()
        {
            for (var i = 0; i < NetworkClients.Count; )
                NetworkClients[i].Disconnect();
        }

        protected override void Handle()
        {
            HandleListening();
            HandleClients();

            Wait(TickRate);
        }

        private void HandleListening()
        {
            var hasConnection = TcpListener.Pending();
            if (hasConnection)
                ReceiveConnection();
        }
        private void ReceiveConnection()
        {
            var client = TcpListener.AcceptTcpClient();
            AddClient(client);
        }

        private void AddClient(TcpClient client)
        {
            Logger.Log("Connected - " + client.Client.RemoteEndPoint);

            var networkClient = new NetworkClient(client);
            networkClient.Disconnected += OnClientDisconnected;
            networkClient.Logger = Logger;

            NetworkClients.Add(networkClient);
        }
        private void OnClientDisconnected(NetworkClient client)
        {
            NetworkClients.Remove(client);
            Logger.Log("Client disconnected :/");
        }

        private void HandleClients()
        {
            NetworkClients.ForEach(HandleClient);
        }
        private void HandleClient(NetworkClient client)
        {
            client.Handle();
        }
    }
}