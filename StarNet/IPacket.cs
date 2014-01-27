using System;
using System.IO;

namespace StarNet
{
    public interface IPacket
    {
        void Read(BinaryReader reader);
        void Write(BinaryWriter writer);
    }
}