using System;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Linq;
using System.IO;
using StarNet.Database;
using Newtonsoft.Json;

namespace StarNet
{
    class Program
    {
        public const int Version = 1;

        public static void Main(string[] args)
        {
            LocalSettings settings = null;
            if (File.Exists("node.config"))
                settings = JsonConvert.DeserializeObject<LocalSettings>(File.ReadAllText("node.config"));
            if (settings == null)
            {
                // First run
                Console.Write("Please enter a postgresql connection string.\n" +
                    "Example: Server=127.0.0.1;Database=starnet;User Id=user_name;Password=password;\n" +
                    "Connection string: ");
                var cs = Console.ReadLine();
                settings = new LocalSettings(cs);
                File.WriteAllText("node.config", JsonConvert.SerializeObject(settings, Formatting.Indented));
            }
            var sharedDatabase = new SharedDatabase(settings.ConnectionString);
            if (args.Length != 0)
            {
                // Execute some sort of command, like adding servers
            }
            else
            {
                var localNode = new StarNetNode(sharedDatabase, settings, new IPEndPoint(IPAddress.Any, 21024)); // TODO: Let this be configurable?
                Console.WriteLine("Starting node {0}", localNode.Settings.UUID);
                localNode.Start();
                while (true)
                    Thread.Sleep(10000);
            }
        }
    }
}