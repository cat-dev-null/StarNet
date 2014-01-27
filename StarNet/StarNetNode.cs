using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;

namespace StarNet
{
    public class StarNetNode
    {
        public const int NetworkPort = 21024;
        public const int ClientBufferLength = 1024;

        public TcpListener Listener { get; set; }
        public UdpClient NetworkClient { get; set; }
        public List<StarboundClient> Clients { get; set; }
        public ServerPool Servers { get; set; }
        public RemoteNode Siblings { get; set; }
        public Guid Id { get; set; }

        public StarNetNode(IPEndPoint endpoint)
        {
            Listener = new TcpListener(endpoint);
            Clients = new List<StarboundClient>();
            NetworkClient = new UdpClient(new IPEndPoint(IPAddress.Any, NetworkPort));
            Id = Guid.NewGuid(); // TODO: Load this from disk
        }

        public void Start()
        {
            Listener.BeginAcceptTcpClient(AcceptClient, null);
            NetworkClient.BeginReceive(NetworkMessageReceived, null);
            // TODO: Log into the network
        }

        private void AcceptClient(IAsyncResult result)
        {
            var tcpClient = Listener.EndAcceptTcpClient(result);
            var client = new StarboundClient(tcpClient);
            Clients.Add(client);
            HandleClient(client);
        }

        private async void HandleClient(StarboundClient client)
        {
            client.NetworkBuffer = new byte[ClientBufferLength];
            var stream = client.Client.GetStream();
            while (true)
            {
                var length = await stream.ReadAsync(client.NetworkBuffer, 0, ClientBufferLength);
                client.UpdateBuffer(length);
            }
        }

        private void NetworkMessageReceived(IAsyncResult result)
        {
            IPEndPoint endPoint = default(IPEndPoint);
            var message = NetworkClient.EndReceive(result, ref endPoint);
            // TODO
        }
    }
}

