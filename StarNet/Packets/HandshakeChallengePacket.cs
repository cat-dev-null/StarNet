using System;
using StarNet.Common;

namespace StarNet.Packets
{
    public class HandshakeChallengePacket : IPacket
    {
        public static readonly byte Id = 3;
        public byte PacketId { get { return Id; } }

        public string ClaimMessage { get; set; }
        public string Salt { get; set; }
        public int Rounds { get; set; }

        public HandshakeChallengePacket()
        {
            ClaimMessage = "";
        }

        public HandshakeChallengePacket(string salt, int rounds = 5000) : this()
        {
            Salt = salt;
            Rounds = rounds;
        }

        public void Read(StarboundStream stream)
        {
            ClaimMessage = stream.ReadString();
            Salt = stream.ReadString();
            Rounds = stream.ReadInt32();
        }

        public bool Write(StarboundStream stream)
        {
            stream.WriteString(ClaimMessage);
            stream.WriteString(Salt);
            stream.WriteInt32(Rounds);
            return false;
        }
    }
}