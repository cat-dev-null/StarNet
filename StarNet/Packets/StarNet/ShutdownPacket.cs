using System;
using System.IO;

namespace StarNet
{
    public class ShutdownPacket : StarNetPacket
    {
        public static readonly byte Id = 2;
        public override byte PacketId { get { return Id; } }

        public override void Read(BinaryReader stream)
        {
        }

        public override void Write(BinaryWriter stream)
        {
        }

        public override MessageFlags Flags
        {
            get
            {
                return MessageFlags.None;
            }
        }
    }
}