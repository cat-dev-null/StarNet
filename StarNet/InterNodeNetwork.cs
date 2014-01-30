using System;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Text;
using System.Collections.Concurrent;
using System.Collections.Generic;
using StarNet.Packets.StarNet;
using Org.BouncyCastle.Crypto;
using System.Linq;
using System.Threading;
using System.Diagnostics;

namespace StarNet
{
    public class InterNodeNetwork
    {
        public delegate void PacketHandler(StarNetPacket packet, IPEndPoint endPoint, InterNodeNetwork network);
        public delegate void HandleResponse(StarNetPacket response, uint transactionId);
        delegate StarNetPacket CreatePacketInstance();
        static Dictionary<byte, CreatePacketInstance> PacketFactories;
        private static Dictionary<byte, PacketHandler> PacketHandlers { get; set; }

        static InterNodeNetwork()
        {
            PacketHandlers = new Dictionary<byte, PacketHandler>();
            PacketFactories = new Dictionary<byte, CreatePacketInstance>();
            PacketFactories[ConfirmationPacket.Id] = () => new ConfirmationPacket();
            PacketFactories[PingPacket.Id] = () => new PingPacket();
            PacketFactories[ShutdownPacket.Id] = () => new ShutdownPacket();
        }

        public static void RegisterPacketHandler(byte id, PacketHandler handler)
        {
            PacketHandlers[id] = handler;
        }

        private class PendingResponse
        {
            public StarNetPacket OriginalPacket { get; set; }
            public int Transaction { get; set; }
            public HandleResponse ResponseHandler { get; set; }
        }

        public UdpClient NetworkClient { get; set; }
        public StarNetNode LocalNode { get; set; }
        public List<RemoteNode> Network { get; set; }

        private List<StarNetPacket> PacketRetryList { get; set; }
        private List<PendingResponse> PendingResponses { get; set; }
        private object NetworkLock = new object();
        private Timer RetryTimer { get; set; }
        private uint NextTransactionNumber { get; set; }
        private CryptoProvider CryptoProvider { get; set; }

        public uint GetTransactionNumber()
        {
            return NextTransactionNumber++;
        }

        public InterNodeNetwork(CryptoProvider crypto)
        {
            NetworkClient = new UdpClient(AddressFamily.InterNetwork);
            CryptoProvider = crypto;
            Network = new List<RemoteNode>();
            PacketRetryList = new List<StarNetPacket>();
            PendingResponses = new List<PendingResponse>();
            RetryTimer = new Timer(DoRetries);
            NextTransactionNumber = 0;
        }

        public InterNodeNetwork(ushort port, CryptoProvider crypto) : this(crypto)
        {
            NetworkClient = new UdpClient(new IPEndPoint(IPAddress.Any, port));
        }

        public InterNodeNetwork(StarNetNode node, CryptoProvider crypto) : this(node.Settings.NetworkPort, crypto)
        {
            LocalNode = node;
        }

        private void DoRetries(object discarded)
        {
            lock (NetworkLock)
            {
                for (int i = 0; i < PacketRetryList.Count; i++)
                {
                    if (PacketRetryList[i].ScheduledRetry < DateTime.Now)
                    {
                        var packet = PacketRetryList[i];
                        PacketRetryList.RemoveAt(i--);
                        packet.Retries++;
                        Send(packet, packet.Destination);
                    }
                }
            }
        }

        public void Start()
        {
            NetworkClient.BeginReceive(NetworkMessageReceived, null);
            if (LocalNode != null)
                Console.WriteLine("Network: Listening on " + NetworkClient.Client.LocalEndPoint);
            RetryTimer.Change(10000, 10000);
        }

        public void Stop()
        {
            try
            {
                RetryTimer.Dispose();
                NetworkClient.Close();
            }
            catch
            {
            }
        }

        public RemoteNode GetNode(IPEndPoint endPoint)
        {
            return Network.SingleOrDefault(s => s.EndPoint == endPoint);
        }

        public void Send(StarNetPacket packet, IPEndPoint destination)
        {
            var memory = new MemoryStream();
            var stream = new BinaryWriter(memory);
            stream.Write(packet.PacketId);
            stream.Write((byte)packet.Flags);
            if (packet.AssignTransactionId)
                packet.Transaction = GetTransactionNumber();
            stream.Write(packet.Transaction);
            stream.Write(DateTime.UtcNow.Ticks);
            packet.Write(stream);
            var payload = new byte[memory.Position];
            memory.Seek(0, SeekOrigin.Begin);
            memory.Read(payload, 0, payload.Length);
            var signature = CryptoProvider.SignMessage(payload);
            memory.Write(signature, 0, signature.Length);
            payload = new byte[memory.Position];
            memory.Seek(0, SeekOrigin.Begin);
            memory.Read(payload, 0, payload.Length);
            packet.Destination = destination;
            packet.ScheduledRetry = DateTime.UtcNow.AddSeconds(10);
            if (packet.Retries == 0 && (packet.Flags & MessageFlags.ConfirmationRequired) > 0)
                lock (NetworkLock) PacketRetryList.Add(packet);
            NetworkClient.Send(payload, payload.Length, destination);
        }

        private void HandleConfirmation(StarNetPacket packet)
        {
            lock (NetworkLock)
            {
                for (int i = 0; i < PacketRetryList.Count; i++)
                {
                    if (PacketRetryList[i].Transaction == packet.Transaction)
                    {
                        PacketRetryList[i].OnConfirmationReceived();
                        PacketRetryList.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        private void NetworkMessageReceived(IAsyncResult result)
        {
            IPEndPoint endPoint = default(IPEndPoint);
            var payload = NetworkClient.EndReceive(result, ref endPoint);
            NetworkClient.BeginReceive(NetworkMessageReceived, null);
            var stream = new BinaryReader(new MemoryStream(payload), Encoding.UTF8);
            AsymmetricKeyParameter key;
            RemoteNode node = null;
            if (endPoint.Address.Equals(IPAddress.Loopback))
                key = CryptoProvider.PublicKey;
            else
            {
                node = GetNode(endPoint);
                if (node == null)
                    return;
                key = node.PublicKey;
            }
            try
            {
                var id = stream.ReadByte();
                var flags = (MessageFlags)stream.ReadByte();
                var transaction = stream.ReadUInt32();
                var timestamp = new DateTime(stream.ReadInt64(), DateTimeKind.Utc);
                if (node != null)
                {
                    // TODO: Evaluate this margin of error, and this whole system in general that prevents resubmission
                    if (Math.Abs((timestamp - node.PreviousMessageTimestamp).TotalSeconds) > 10)
                        return;
                    node.PreviousMessageTimestamp = timestamp;
                }
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
                        if (id == ConfirmationPacket.Id)
                            HandleConfirmation(packet);
                        else if (PacketHandlers.ContainsKey(id))
                            PacketHandlers[id](packet, endPoint, this);
                        else
                            Console.WriteLine("Warning: Unhandled internode network packet with ID {0}", id);
                        if ((flags & MessageFlags.PropegateTransaction) > 0 && endPoint.Address == IPAddress.Loopback)
                        {
                            foreach (var target in Network)
                            {
                                if (target != node)
                                    Send(packet, target.EndPoint);
                            }
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
                Console.WriteLine("Warning: Error parsing internetwork message: {0}", e.GetType().Name);
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
        /// Only works for messages that originate from localhost.
        /// </summary>
        PropegateTransaction = 2,
        /// <summary>
        /// Indicates that this packet is a response to an earlier request.
        /// </summary>
        IsResponse = 3
    }
}