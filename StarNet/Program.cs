using System;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Linq;

namespace StarNet
{
    class Program
    {
        public static void Main(string[] args)
        {
            var localNode = new StarNetNode(new IPEndPoint(IPAddress.Any, 21024)); // TODO: Let this be configurable?
            localNode.Start();
            while (true)
                Thread.Sleep(10000);
        }
    }
}
