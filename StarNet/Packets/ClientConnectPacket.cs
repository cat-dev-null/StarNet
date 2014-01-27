using System;

namespace StarNet.Packets
{
    public class ClientConnectPacket : IPacket
    {
        public byte PacketId { get { return 6; } }

        public string AssetDigest;
        public Variant Claim;
        public byte[] UUID;
        public string PlayerName;
        public string Species;
        public byte[] Shipworld; // TODO: Decode this
        public string Account;

        public ClientConnectPacket()
        {
        }

        public ClientConnectPacket(string assetDigest, Variant claim, byte[] uuid, string playerName,
            string species, byte[] shipworld, string account)
        {
            AssetDigest = assetDigest;
            Claim = claim;
            UUID = uuid;
            PlayerName = playerName;
            Species = species;
            Shipworld = shipworld;
            Account = account;
        }

        public void Read(StarboundStream stream)
        {
            AssetDigest = stream.ReadString();
            Claim = stream.ReadVariant();
            bool uuid = stream.ReadBoolean();
            if (uuid)
                UUID = stream.ReadUInt8Array(16);
            PlayerName = stream.ReadString();
            Species = stream.ReadString();
            Shipworld = stream.ReadUInt8Array();
            Account = stream.ReadString();
        }

        public bool Write(StarboundStream stream)
        {
            throw new NotImplementedException();
        }
    }
}