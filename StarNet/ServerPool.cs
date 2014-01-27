using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;

namespace StarNet
{
	public class ServerPool
	{
        public List<StarboundServer> Pool { get; set; }

        public ServerPool()
        {
            Pool = new List<StarboundServer>();
            // TODO: Load pool from disk, perhaps from other nodes
        }
	}
}