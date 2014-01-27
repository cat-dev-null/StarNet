using System;

namespace StarNet.Packets
{
    public class ClientConnectPacket : IPacket
    {
        public byte PacketId { get { return 6; } }

        public byte[] AssetDigest;
        public Variant Claim;
        public byte[] UUID;
        public string PlayerName;
        public string PlayerSpecies;
        public byte[] Shipworld; // TODO: Decode
        public string Account;

        public ClientConnectPacket()
        {
        }

        public void Read(StarboundStream stream)
        {
            AssetDigest = stream.ReadUInt8Array();
            Claim = stream.ReadVariant();
            bool uuid = stream.ReadBoolean();
            if (uuid)
                UUID = stream.ReadUInt8Array();
        }

        public bool Write(StarboundStream stream)
        {
            throw new NotImplementedException();
        }
    }
}