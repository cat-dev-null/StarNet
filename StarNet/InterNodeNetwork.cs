using System;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Text;
using System.Collections.Generic;
using StarNet.Packets.StarNet;
using Org.BouncyCastle.Crypto;

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
        public List<RemoteNode> Siblings { get; set; }

        private uint NextTransactionNumber { get; set; }
        private CryptoProvider CryptoProvider { get; set; }
        private object NetworkLock = new object();

        public uint GetTransactionNumber()
        {
            return NextTransactionNumber++;
        }

        public InterNodeNetwork(ushort port, CryptoProvider crypto)
        {
            NetworkClient = new UdpClient(new IPEndPoint(IPAddress.Any, port));
            CryptoProvider = crypto;
            Siblings = new List<RemoteNode>();
            NextTransactionNumber = 0;
        }

        public InterNodeNetwork(StarNetNode node, CryptoProvider crypto) : this(node.Settings.NetworkPort, crypto)
        {
            LocalNode = node;
        }

        public void Start()
        {
            NetworkClient.BeginReceive(NetworkMessageReceived, null);
            Console.WriteLine("Network: Listening on " + NetworkClient.Client.LocalEndPoint);
        }

        public void Send(StarNetPacket packet, IPEndPoint destination)
        {
            lock (NetworkLock)
            {

            }
        }

        private void NetworkMessageReceived(IAsyncResult result)
        {
            IPEndPoint endPoint = default(IPEndPoint);
            var payload = NetworkClient.EndReceive(result, ref endPoint);
            NetworkClient.BeginReceive(NetworkMessageReceived, null);
            var stream = new BinaryReader(new MemoryStream(payload), Encoding.UTF8);
            AsymmetricKeyParameter key;
            if (endPoint.Address == IPAddress.Loopback)
                key = CryptoProvider.PublicKey;
            else
            {
                // TODO: Look up key for the node that sent this message
                return;
            }
            try
            {
                var id = stream.ReadByte();
                var flags = (MessageFlags)stream.ReadByte();
                var transaction = stream.ReadUInt32();
                var timestamp = new DateTime(stream.ReadInt64(), DateTimeKind.Utc);
                // TODO: Confirm that the timestamp is further ahead of the last timestamp we saw
                // Within a certain margin of error to account for UDP not caring about message order
                // This is a crypto thing, raise a warning and discard the packet if that check fails
                if (PacketFactories.ContainsKey(id))
                {
                    var packet = PacketFactories[id]();
                    packet.Transaction = transaction;
                    packet.Read(stream);
                    int signatureLength = (int)(stream.BaseStream.Length - stream.BaseStream.Position);
                    int messageLength = (int)stream.BaseStream.Position;
                    stream.BaseStream.Seek(0, SeekOrigin.Begin);
                    byte[] message = new byte[messageLength];
                    byte[] signature = new byte[signatureLength];
                    stream.BaseStream.Read(message, 0, message.Length);
                    stream.BaseStream.Read(signature, 0, signature.Length);
                    // Verify signature
                    if (!CryptoProvider.VerifySignature(message, signature, key))
                        Console.WriteLine("Warning: Received internode network packet with bad signature from {0}", endPoint);
                    else
                    {
                        if (PacketHandlers.ContainsKey(id))
                            PacketHandlers[id](packet, endPoint, this);
                        else
                            Console.WriteLine("Warning: Unhandled internode network packet with ID {0}", id);
                        if ((flags & MessageFlags.PropegateTransaction) > 0)
                        {
                            // TODO
                        }
                        if ((flags & MessageFlags.ConfirmationRequired) > 0)
                            Send(new ConfirmationPacket(transaction), endPoint); // TODO: Don't re-handle duplicate transactions
                    }
                }
                else
                    Console.WriteLine("Warning: Received unknown internode network packet ID {0}", id);
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
        /// <summary>
        /// Requires the recipient to send a confirmation.
        /// </summary>
        ConfirmationRequired = 1,
        /// <summary>
        /// This node should propegate the transaction to the rest of the network.
        /// </summary>
        PropegateTransaction = 2
    }
}