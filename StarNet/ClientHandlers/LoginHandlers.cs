using System;
using StarNet.Packets;

namespace StarNet.ClientHandlers
{
    internal static class LoginHandlers
    {
        public static void Register()
        {
            PacketReader.RegisterPacketHandler(ClientConnectPacket.Id, HandleClientConnect);
        }

        public static void HandleClientConnect(StarNetNode node, StarboundClient client, IPacket _packet)
        {
            var packet = (ClientConnectPacket)_packet;
            var guid = new Guid(packet.UUID);
            Console.WriteLine("{0} ({1}) logged in from {2} as {3}", packet.Account, guid, client.Socket.RemoteEndPoint, packet.PlayerName);
            // TODO: Send Handshake Challenge, save/parse shipworld, etc
        }
    }
}