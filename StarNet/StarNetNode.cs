using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using StarNet.Packets;

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
            Console.WriteLine("Listening on " + Listener.LocalEndpoint);
            // TODO: Log into the network
        }

        private void AcceptClient(IAsyncResult result)
        {
            var socket = Listener.EndAcceptSocket(result);
            Console.WriteLine("New connection from {0}", socket.RemoteEndPoint);
            var client = new StarboundClient(socket);
            Clients.Add(client);
            client.PacketQueue.Enqueue(new ProtocolVersionPacket(ProtocolVersion));
            client.FlushPackets();
            client.Socket.BeginReceive(client.PacketReader.NetworkBuffer, 0, client.PacketReader.NetworkBuffer.Length,
                SocketFlags.None, ClientDataReceived, client);
        }

        private void ClientDataReceived(IAsyncResult result)
        {
            var client = (StarboundClient)result.AsyncState;
            var length = client.Socket.EndReceive(result);
            var packets = client.PacketReader.UpdateBuffer(length);
            if (packets != null && packets.Length > 0)
            {
                foreach (var packet in packets)
                {
                    Console.WriteLine("Received {1} ({0}) from {2}", packet.PacketId, packet.GetType().Name, client.Socket.RemoteEndPoint);
                }
            }
            client.Socket.BeginReceive(client.PacketReader.NetworkBuffer, 0, client.PacketReader.NetworkBuffer.Length,
                SocketFlags.None, ClientDataReceived, client);
        }

        private void NetworkMessageReceived(IAsyncResult result)
        {
            IPEndPoint endPoint = default(IPEndPoint);
            var message = NetworkClient.EndReceive(result, ref endPoint);
            // TODO
        }
    }
}

