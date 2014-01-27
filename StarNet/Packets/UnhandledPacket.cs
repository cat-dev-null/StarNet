using System;
using System.IO;

namespace StarNet.Packets
{
    public class UnhandledPacket : IPacket
    {
        public byte PacketId { get; set; }
        public bool Compressed { get; set; }
        public byte[] Data { get; set; }
        private int Length { get; set; }

        public UnhandledPacket(bool compressed, int length, byte packetId)
        {
            Compressed = compressed;
            Length = length;
            Data = new byte[Length];
            PacketId = packetId;
        }

        public int Read(StarboundStream stream)
        {
            stream.Read(Data, 0, Data.Length);
            return Data.Length;
        }

        public bool Write(StarboundStream stream)
        {
            stream.Write(Data, 0, Data.Length);
            return Compressed;
        }
    }
}

