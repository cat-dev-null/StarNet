using System;
using StarNet.Common;

namespace StarNet.Packets.Starbound
{
    public class ServerDisconnectPacket : IStarboundPacket
    {
        public static readonly byte Id = 2;
        public byte PacketId { get { return Id; } }

        public ServerDisconnectPacket()
        {
        }

        public void Read(StarboundStream stream)
        {
            throw new NotImplementedException();
        }

        public bool Write(StarboundStream stream)
        {
            throw new NotImplementedException();
        }
    }
}