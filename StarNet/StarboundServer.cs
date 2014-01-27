using System;
using System.Net;
using System.Collections.Generic;

namespace StarNet
{
    public class StarboundServer
    {
        public Guid Id { get; set; }
        public IPEndPoint EndPoint { get; set; }
        public int TotalPlayers { get; set; }
        public TimeSpan Delay { get; set; }
        public ServerStatus Status { get; set; }
        public List<Coordinates3D> CoordinatesOwned { get; set; }

        public StarboundServer(IPEndPoint endPoint)
        {
            EndPoint = endPoint;
            Status = ServerStatus.Healthy;
            CoordinatesOwned = new List<Coordinates3D>();
        }
    }
}