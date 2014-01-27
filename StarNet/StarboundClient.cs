using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Net;

namespace StarNet
{
	public class StarboundClient
	{
        public TcpClient Client { get; set; }
        public ConcurrentQueue<IPacket> PacketQueue { get; set; }
        public StarboundServer CurrentServer { get; set; }

        public StarboundClient(TcpClient client)
        {
            Client = client;
            PacketQueue = new ConcurrentQueue<IPacket>();
        }
	}
}