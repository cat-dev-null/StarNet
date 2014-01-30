using System;
using System.IO;
using System.Net;
using StarNet.Packets.StarNet;

namespace StarNet
{
    public static class LocalHandlers
    {
        public static void Register()
        {
            InterNodeNetwork.RegisterPacketHandler(PingPacket.Id, HandlePing);
        }

        public static void HandlePing(StarNetPacket _packet, IPEndPoint source, InterNodeNetwork network)
        {
            // Do nothing, this is handled at a lower level
        }
    }
}