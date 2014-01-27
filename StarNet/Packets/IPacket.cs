using System;
using System.IO;

namespace StarNet.Packets
{
    public interface IPacket
    {
        void Read(BinaryReader reader);
        void Write(BinaryWriter writer);
    }
}