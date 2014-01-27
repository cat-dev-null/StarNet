using System;
using System.IO;

namespace StarNet.Packets
{
    public interface IPacket
    {
        byte PacketId { get; }

        void Read(StarboundStream stream);
        /// <summary>
        /// Write the packet's payload to the specified stream.
        /// </summary>
        /// <param name="stream">The stream to write to.</param>
        /// <returns>True if the payload should be compressed.</returns>
        bool Write(StarboundStream stream);
    }
}