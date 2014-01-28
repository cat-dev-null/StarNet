using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;

namespace StarNet
{
    public class ServerPool
    {
        private List<StarboundServer> Pool { get; set; }

        public ServerPool()
        {
            Pool = new List<StarboundServer>();
        }

        public void AddServer(IPEndPoint endPoint)
        {
        }
    }
}