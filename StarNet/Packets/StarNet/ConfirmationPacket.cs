using System;
using System.IO;
using System.Net;

namespace StarNet.Packets.StarNet
{
    public class ConfirmationPacket : StarNetPacket
    {
        public static readonly byte Id = 0;
        public override byte PacketId { get { return Id; } }

        public ConfirmationPacket()
        {
        }

        public ConfirmationPacket(uint transaction)
        {
            Transaction = Transaction;
        }

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