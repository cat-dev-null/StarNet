using System;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Linq;
using System.IO;

namespace StarNet
{
    class Program
    {
        public static void Main(string[] args)
        {
            if (!Directory.Exists("config"))
                Directory.CreateDirectory("config");
            var localNode = new StarNetNode(new IPEndPoint(IPAddress.Any, 21024)); // TODO: Let this be configurable?
            localNode.Start();
            while (true)
                Thread.Sleep(10000);
        }
    }
}
