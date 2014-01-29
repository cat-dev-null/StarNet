using System;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Linq;
using System.IO;
using StarNet.Database;

namespace StarNet
{
    class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length != 0)
            {
                // Run some sort of command
            }
            else
            {
                var database = new StarNetDatabase("node.db");
                var localNode = new StarNetNode(database, new IPEndPoint(IPAddress.Any, 21024)); // TODO: Let this be configurable?
                Console.WriteLine("Starting node {0}", localNode.Id);
                localNode.Start();
                while (true)
                    Thread.Sleep(10000);
            }
        }
    }
}
