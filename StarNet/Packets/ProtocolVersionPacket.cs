using System;
using StarNet.Packets;
using StarNet.Common;

namespace StarNet.Packets
{
    public class ProtocolVersionPacket : IPacket
    {
        public static readonly byte Id = 0;
        public byte PacketId { get { return Id; } }

        public uint ProtocolVersion { get; set; }

        public ProtocolVersionPacket(uint protocolVersion)
        {
            ProtocolVersion = protocolVersion;
        }

        public void Read(StarboundStream stream)
        {
            ProtocolVersion = stream.ReadUInt32();
        }

        public bool Write(StarboundStream stream)
        {
            stream.WriteUInt32(ProtocolVersion);
            return false;
        }
    }
}

