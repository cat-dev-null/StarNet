using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;

namespace StarNet
{
    public class StarNetHub
    {
        public TcpListener Listener { get; set; }
        public List<StarboundClient> Clients { get; set; }
        public ServerPool Servers { get; set; }
        public RemoteNode Siblings { get; set; }

        public StarNetHub(IPEndPoint endpoint)
        {
            Listener = new TcpListener(endpoint);
            Clients = new List<StarboundClient>();
        }

        public void Start()
        {
            Listener.BeginAcceptTcpClient(AcceptClient, null);
        }

        private void AcceptClient(IAsyncResult result)
        {
            var tcpClient = Listener.EndAcceptTcpClient(result);
            var client = new StarboundClient(tcpClient);
            Clients.Add(client);
        }
    }
}

