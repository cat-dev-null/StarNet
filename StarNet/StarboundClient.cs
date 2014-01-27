using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Net;
using System.IO;
using Ionic.Zlib;
using StarNet.Packets;

namespace StarNet
{
    public class StarboundClient
    {
        public Socket Socket { get; set; }
        public ConcurrentQueue<IPacket> PacketQueue { get; set; }
        public StarboundServer CurrentServer { get; set; }
        public const int MaxPacketLength = 1048576; // 1 MB (compressed, if applicable)
        public const int MaxInflatedPacketLength = 10485760; // 10 MB

        internal byte[] NetworkBuffer { get; set; }

        private byte[] PacketBuffer = new byte[0];
        private long WorkingLength = long.MaxValue;
        private int DataIndex = 0;
        private bool Compressed = false;

        public StarboundClient(Socket socket)
        {
            Socket = socket;
            PacketQueue = new ConcurrentQueue<IPacket>();
        }

        public void FlushPackets()
        {
            while (PacketQueue.Count > 0)
            {
                IPacket next;
                while (!PacketQueue.TryDequeue(out next)) ;
                var memoryStream = new MemoryStream();
                var stream = new StarboundStream(memoryStream);
                var compressed = next.Write(stream);
                byte[] buffer = new byte[stream.Position];
                Array.Copy(memoryStream.GetBuffer(), buffer, buffer.Length);
                int length = buffer.Length;
                if (compressed)
                {
                    buffer = ZlibStream.CompressBuffer(buffer);
                    length = -buffer.Length;
                }
                byte[] header = new byte[StarboundStream.GetSignedVLQLength(length) + 1];
                header[0] = next.PacketId;
                int discarded;
                StarboundStream.WriteSignedVLQ(header, 1, length, out discarded);
                Console.WriteLine("Sending packet {0} (length: {1}) to {2}", next.PacketId, length, Socket.RemoteEndPoint);
                Socket.Send(header);
                Socket.Send(buffer);
                Console.WriteLine("Sent.");
            }
        }

        internal IPacket[] UpdateBuffer(int length)
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

        public IPacket[] Decode(byte packetId, byte[] packet)
        {
            // TODO: Actually decode packets
            var memoryStream = new MemoryStream(packet);
            var stream = new StarboundStream(memoryStream);
            Console.WriteLine("Got packet {0}, length: {1} bytes", packetId, packet.Length);
            return null;
        }
    }
}