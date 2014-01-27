using System;
using StarNet.Packets;

namespace StarNet
{
    public class ProtocolVersionPacket : IPacket
    {
        public byte PacketId { get { return 0; } }

        public uint ProtocolVersion { get; set; }

        public ProtocolVersionPacket(uint protocolVersion)
        {
            ProtocolVersion = protocolVersion;
        }

        public int Read(StarboundStream stream)
        {
            ProtocolVersion = stream.ReadUInt32();
            return 4;
        }

        public bool Write(StarboundStream stream)
        {
            stream.WriteUInt32(ProtocolVersion);
            return false;
        }
    }
}

