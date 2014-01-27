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
        public const int ProtocolVersion = 635;

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
            Listener.Start();
            Listener.BeginAcceptSocket(AcceptClient, null);
            NetworkClient.BeginReceive(NetworkMessageReceived, null);
            // TODO: Log into the network
        }

        private void AcceptClient(IAsyncResult result)
        {
            var socket = Listener.EndAcceptSocket(result);
            Console.WriteLine("Got connection from {0}", socket.RemoteEndPoint);
            var client = new StarboundClient(socket);
            Clients.Add(client);
            client.NetworkBuffer = new byte[ClientBufferLength];
            client.PacketQueue.Enqueue(new ProtocolVersionPacket(ProtocolVersion));
            client.FlushPackets();
            client.Socket.BeginReceive(client.NetworkBuffer, 0, ClientBufferLength, SocketFlags.None, ClientDataReceived, client);
        }

        private void ClientDataReceived(IAsyncResult result)
        {
            var client = (StarboundClient)result.AsyncState;
            Console.WriteLine("Got data from " + client.Socket.RemoteEndPoint);
            var length = client.Socket.EndReceive(result);
            var packets = client.UpdateBuffer(length);
            if (packets != null && packets.Length > 0)
            {
                // TODO: Handle packets
            }
            client.Socket.BeginReceive(client.NetworkBuffer, 0, ClientBufferLength, SocketFlags.None, ClientDataReceived, client);
        }

        private void NetworkMessageReceived(IAsyncResult result)
        {
            IPEndPoint endPoint = default(IPEndPoint);
            var message = NetworkClient.EndReceive(result, ref endPoint);
            // TODO
        }
    }
}

