using System;
using System.Net;

namespace StarNet
{
    public class StarboundServer
    {
        public Guid Id { get; set; }
        public IPEndPoint EndPoint { get; set; }

        public StarboundServer(IPEndPoint endPoint, Guid id = Guid.Empty)
        {
            EndPoint = endPoint;
            if (id == Guid.Empty)
                id = Guid.NewGuid();
            else
                Id = id;
        }
    }
}