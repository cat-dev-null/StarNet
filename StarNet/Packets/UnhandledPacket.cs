using System;
using System.IO;

namespace StarNet.Packets
{
    public class UnhandledPacket : IPacket
    {
        public UnhandledPacket()
        {
        }

        public void Read(BinaryReader reader)
        {
            throw new NotImplementedException();
        }

        public void Write(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}

