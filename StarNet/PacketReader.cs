using System;
using StarNet.Packets;
using Ionic.Zlib;
using System.IO;
using System.Collections.Generic;
using StarNet.Common;

namespace StarNet
{
    public class PacketReader
    {
        public delegate void PacketHandler(StarNetNode node, StarboundClient client, IPacket packet);
        delegate IPacket CreatePacketInstance();
        static Dictionary<byte, CreatePacketInstance> PacketFactories;
        static Dictionary<byte, PacketHandler> PacketHandlers;

        static PacketReader()
        {
            PacketFactories = new Dictionary<byte, CreatePacketInstance>();
            PacketFactories[ClientConnectPacket.Id] = () => new ClientConnectPacket();

            PacketHandlers = new Dictionary<byte, PacketHandler>();
        }

        public static void RegisterPacketHandler(byte packetId, PacketHandler handler)
        {
            PacketHandlers[packetId] = handler;
        }

        public static void HandlePacket(StarNetNode node, StarboundClient client, IPacket packet)
        {
            if (PacketHandlers.ContainsKey(packet.PacketId))
                PacketHandlers[packet.PacketId](node, client, packet);
        }

        public readonly int MaxPacketLength = 1048576; // 1 MB (compressed, if applicable)
        public readonly int MaxInflatedPacketLength = 10485760; // 10 MB
        public readonly int NetworkBufferLength = 1024;

        public byte[] NetworkBuffer { get; set; }

        private byte[] PacketBuffer = new byte[0];
        private long WorkingLength = long.MaxValue;
        private int DataIndex = 0;
        private bool Compressed = false;

        public PacketReader()
        {
            NetworkBuffer = new byte[NetworkBufferLength];
        }

        public IPacket[] UpdateBuffer(int length)
        {
            if (length == 0)
                return null; // TODO: This is probably a network error, handle it appropriately
            int index = PacketBuffer.Length;
            if (WorkingLength == long.MaxValue)
            {
                // We don't know the length of the packet yet, so keep going
                if (PacketBuffer.Length < index + length)
                    Array.Resize(ref PacketBuffer, index + length);
                Array.Copy(NetworkBuffer, 0, PacketBuffer, index, length);
                if (PacketBuffer.Length > 1)
                {
                    // Check to see if we have the entire length yet
                    int i;
                    for (i = 1; i < 5 && i < PacketBuffer.Length; i++)
                    {
                        if ((PacketBuffer[i] & 0x80) == 0)
                        {
                            WorkingLength = StarboundStream.ReadSignedVLQ(PacketBuffer, 1, out DataIndex);
                            DataIndex++;
                            Compressed = WorkingLength < 0;
                            if (Compressed)
                                WorkingLength = -WorkingLength;
                            if (WorkingLength > MaxPacketLength)
                                throw new IOException("Packet exceeded maximum permissible length.");
                            break;
                        }
                    }
                    if (i == 5)
                        throw new IOException("Packet exceeded maximum permissible length.");
                }
            }
            else
            {
                if (PacketBuffer.Length < index + length)
                    Array.Resize(ref PacketBuffer, index + length);
                Array.Copy(NetworkBuffer, 0, PacketBuffer, index, length);
                if (PacketBuffer.Length >= WorkingLength + DataIndex)
                {
                    // Ready to decode packet
                    var data = new byte[WorkingLength];
                    Array.Copy(PacketBuffer, DataIndex, data, 0, WorkingLength);
                    if (Compressed)
                        data = ZlibStream.UncompressBuffer(data); // TODO: Prevent compressed packets from exceeding MaxInflatedPacketLength
                    var packets = Decode(PacketBuffer[0], data);
                    Array.Copy(PacketBuffer, DataIndex + WorkingLength, PacketBuffer, 0, PacketBuffer.Length - (DataIndex + WorkingLength));
                    Array.Resize(ref PacketBuffer, (int)(PacketBuffer.Length - (DataIndex + WorkingLength)));
                    WorkingLength = long.MaxValue;
                    return packets;
                }
            }
            return null;
        }

        public IPacket[] Decode(byte packetId, byte[] payload)
        {
            var memoryStream = new MemoryStream(payload);
            var stream = new StarboundStream(memoryStream);
            List<IPacket> packets = new List<IPacket>();
            while (stream.Position < stream.Length)
            {
                IPacket packet;
                if (PacketFactories.ContainsKey(packetId))
                    packet = PacketFactories[packetId]();
                else
                    packet = new UnhandledPacket(Compressed, payload.Length, packetId);
                packet.Read(stream);
                packets.Add(packet);
            }
            return packets.ToArray();
        }
    }
}