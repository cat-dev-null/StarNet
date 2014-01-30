using System;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Text;
using System.Collections.Generic;
using StarNet.Packets.StarNet;

namespace StarNet
{
    public class InterNodeNetwork
    {
        public delegate void PacketHandler(StarNetPacket packet, IPEndPoint endPoint, InterNodeNetwork network);
        delegate StarNetPacket CreatePacketInstance();
        static Dictionary<byte, CreatePacketInstance> PacketFactories;
        private static Dictionary<byte, PacketHandler> PacketHandlers { get; set; }

        static InterNodeNetwork()
        {
            PacketHandlers = new Dictionary<byte, PacketHandler>();
            PacketFactories = new Dictionary<byte, CreatePacketInstance>();
            PacketFactories[ConfirmationPacket.Id] = () => new ConfirmationPacket();
            PacketFactories[AddServerPacket.Id] = () => new AddServerPacket();
        }

        public static void RegisterPacketHandler(byte id, PacketHandler handler)
        {
            PacketHandlers[id] = handler;
        }

        public UdpClient NetworkClient { get; set; }
        public StarNetNode LocalNode { get; set; }

        private uint NextTransactionNumber { get; set; }

        public uint GetTransactionNumber()
        {
            return NextTransactionNumber++;
        }

        public InterNodeNetwork(StarNetNode node)
        {
            LocalNode = node;
            NetworkClient = new UdpClient(new IPEndPoint(IPAddress.Any, node.Settings.NetworkPort));
            NextTransactionNumber = 0;
        }

        public void Start()
        {
            NetworkClient.BeginReceive(NetworkMessageReceived, null);
            Console.WriteLine("Network: Listening on " + NetworkClient.Client.LocalEndPoint);
        }

        public void Send(StarNetPacket packet, IPEndPoint destination)
        {
            // TODO
        }

        private void NetworkMessageReceived(IAsyncResult result)
        {
            IPEndPoint endPoint = default(IPEndPoint);
            var payload = NetworkClient.EndReceive(result, ref endPoint);
            NetworkClient.BeginReceive(NetworkMessageReceived, null);
            var stream = new BinaryReader(new MemoryStream(payload), Encoding.UTF8);
            // TODO: Verify payload authenticity with cryptographic signature
            // The IP can probably be spoofed and then we'll look silly
            if (endPoint.Address != IPAddress.Loopback)
                return;
            try
            {
                var id = stream.ReadByte();
                var flags = (MessageFlags)stream.ReadByte();
                var transaction = stream.ReadUInt32();
                if (PacketFactories.ContainsKey(id))
                {
                    var packet = PacketFactories[id]();
                    packet.Transaction = transaction;
                    packet.Read(stream);
                    if (PacketHandlers.ContainsKey(id))
                        PacketHandlers[id](packet, endPoint, this);
                    else
                        Console.WriteLine("Warning: Unhandled internode network packet with ID {0}", id);
                }
                else
                    Console.WriteLine("Warning: Received unknown internode network packet ID {0}", id);
                if ((flags & MessageFlags.ConfirmationRequired) > 0)
                    Send(new ConfirmationPacket(transaction), endPoint); // TODO: Don't re-handle duplicate transactions
            }
            catch (Exception e)
            {
                Console.WriteLine("Warning: Error parsing internetwork message ({0})", e.GetType().Name);
            }
        }
    }

    [Flags]
    public enum MessageFlags
    {
        None = 0,
        ConfirmationRequired = 1
    }
}