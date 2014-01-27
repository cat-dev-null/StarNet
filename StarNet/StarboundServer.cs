using System;
using System.Net;

namespace StarNet
{
    public class StarboundServer
    {
        public Guid Id { get; set; }
        public IPEndPoint EndPoint { get; set; }
        public int TotalPlayers { get; set; }

        public StarboundServer(IPEndPoint endPoint, Guid id = default(Guid))
        {
            EndPoint = endPoint;
            if (id == default(Guid))
                id = Guid.NewGuid();
            else
                Id = id;
        }
    }
}