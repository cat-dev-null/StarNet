using System;
using System.IO;
using StarNet.Common;

namespace StarNet.Packets.Starbound
{
    public class UnhandledPacket : IStarboundPacket
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

        public void Read(StarboundStream stream)
        {
            stream.Read(Data, 0, Data.Length);
        }

        public bool Write(StarboundStream stream)
        {
            stream.Write(Data, 0, Data.Length);
            return Compressed;
        }
    }
}

