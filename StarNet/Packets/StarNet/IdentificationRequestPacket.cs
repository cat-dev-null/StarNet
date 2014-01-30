using System;
using System.IO;
using System.Net;

namespace StarNet.Packets.StarNet
{
    public class IdentificationRequestPacket : StarNetPacket
    {
        public static readonly byte Id = 3;
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