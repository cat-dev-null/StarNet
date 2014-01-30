using System;
using System.IO;
using System.Net;

namespace StarNet.Packets.StarNet
{
    public class AddServerPacket : StarNetPacket
    {
        public static readonly byte Id = 1;
        public override byte PacketId { get { return Id; } }

        public Guid ServerId { get; set; }
        public IPAddress IP { get; set; }
        public ushort Port { get; set; }

        public override void Read(BinaryReader stream)
        {
            ServerId = new Guid(stream.ReadBytes(Guid.Empty.ToByteArray().Length));
            IP = new IPAddress(stream.ReadBytes(sizeof(ulong)));
            Port = stream.ReadUInt16();
        }

        public override void Write(BinaryWriter stream)
        {
            stream.Write(ServerId.ToByteArray());
            stream.Write(IP.GetAddressBytes());
            stream.Write(Port);
        }

        public override MessageFlags Flags
        {
            get
            {
                return MessageFlags.ConfirmationRequired;
            }
        }
    }
}