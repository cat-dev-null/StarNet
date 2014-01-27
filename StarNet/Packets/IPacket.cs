using System;
using System.IO;

namespace StarNet.Packets
{
    public interface IPacket
    {
        byte PacketId { get; }

        /// <summary>
        /// Reads the packet's payload from the specified stream.
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        /// <returns>The final length of the payload.</returns>
        int Read(StarboundStream stream);
        /// <summary>
        /// Write the packet's payload to the specified stream.
        /// </summary>
        /// <param name="stream">The stream to write to.</param>
        /// <returns>True if the payload should be compressed.</returns>
        bool Write(StarboundStream stream);
    }
}