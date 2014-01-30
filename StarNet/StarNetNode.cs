using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using StarNet.Packets;
using StarNet.Database;
using NHibernate.Linq;
using System.Linq;
using StarNet.ClientHandlers;
using System.Text;
using System.IO;
using StarNet.Packets.Starbound;

namespace StarNet
{
    public class StarNetNode
    {
        public const int ClientBufferLength = 1024;
        public const int ProtocolVersion = 636;

        public delegate void HandleNetworkMessage(BinaryReader stream, IPEndPoint source);

        public SharedDatabase Database { get; set; }
        public TcpListener Listener { get; set; }
        public List<StarboundClient> Clients { get; set; }
        public ServerPool Servers { get; set; }
        public LocalSettings Settings { get; set; }
        public InterNodeNetwork Network { get; set; }

        private CryptoProvider CryptoProvider { get; set; }

        public StarNetNode(SharedDatabase database, LocalSettings settings, CryptoProvider crypto, IPEndPoint endpoint)
        {
            Settings = settings;
            Database = database;
            Listener = new TcpListener(endpoint);
            Clients = new List<StarboundClient>();
            CryptoProvider = crypto;
            Network = new InterNodeNetwork(this, crypto);
            RegisterHandlers();
        }

        private void RegisterHandlers()
        {
            LoginHandlers.Register();
            LocalHandlers.Register();
        }

        public void Start()
        {
            Network.Start();
            Listener.Start();
            Listener.BeginAcceptSocket(AcceptClient, null);
            Console.WriteLine("Starbound: Listening on " + Listener.LocalEndpoint);
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
                    PacketReader.HandlePacket(this, client, packet);
            }
            client.Socket.BeginReceive(client.PacketReader.NetworkBuffer, 0, client.PacketReader.NetworkBuffer.Length,
                SocketFlags.None, ClientDataReceived, client);
        }
    }
}

