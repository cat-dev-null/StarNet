using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using Org.BouncyCastle.Crypto;

namespace StarNet
{
    public class RemoteNode
    {
        public Guid Id { get; set; }
        public IPEndPoint EndPoint { get; set; }
        public DateTime PreviousMessageTimestamp { get; set; }
        public AsymmetricKeyParameter PublicKey { get; set; }
        public UdpClient Client { get; set; }

        public RemoteNode(IPEndPoint endPoint, Guid id = default(Guid))
        {
            EndPoint = endPoint;
            if (id == default(Guid))
                id = Guid.NewGuid();
            else
                Id = id;
            PreviousMessageTimestamp = DateTime.MinValue;
            Client = new UdpClient();
            Client.Connect(endPoint);
        }
    }
}