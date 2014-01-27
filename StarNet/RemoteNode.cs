using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;

namespace StarNet
{
	public class RemoteNode
	{
        public Guid Id { get; set; }
        public IPEndPoint EndPoint { get; set; }

        public RemoteNode(IPEndPoint endPoint, Guid id = default(Guid))
        {
            EndPoint = endPoint;
            if (id == default(Guid))
                id = Guid.NewGuid();
            else
                Id = id;
        }
	}
}