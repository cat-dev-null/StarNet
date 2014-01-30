using System;
using System.IO;

namespace StarNet
{
    public abstract class StarNetPacket
    {
        public uint Transaction { get; set; }
        public abstract MessageFlags Flags { get; }
        public abstract byte PacketId { get; }
        public abstract void Read(BinaryReader stream);
        public abstract void Write(BinaryWriter stream);
    }
}