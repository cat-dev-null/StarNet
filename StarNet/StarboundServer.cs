using System;
using System.Net;
using System.Collections.Generic;
using StarNet.Common;

namespace StarNet
{
    public class StarboundServer
    {
        public Guid Id { get; set; }
        public IPEndPoint EndPoint { get; set; }
        public int TotalPlayers { get; set; }
        public TimeSpan Delay { get; set; }
        public ServerStatus Status { get; set; }
        public List<Coordinates2D> CoordinatesOwned { get; set; }
        public string Password { get; set; }

        public StarboundServer(IPEndPoint endPoint)
        {
            EndPoint = endPoint;
            Status = ServerStatus.Healthy;
            CoordinatesOwned = new List<Coordinates2D>();
        }
    }
}