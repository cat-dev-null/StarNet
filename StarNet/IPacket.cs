using System;

namespace StarNet
{
    public interface IPacket
    {
        void Decode(byte[] data);
        byte[] Encode();
    }
}