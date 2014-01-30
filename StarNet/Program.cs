using System;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Linq;
using System.IO;
using StarNet.Database;
using Newtonsoft.Json;
using System.Text;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Crypto;
using StarNet.Packets.StarNet;

namespace StarNet
{
    class Program
    {
        public const int Version = 1;

        private static AsymmetricCipherKeyPair ServerKey;

        public static void Main(string[] args)
        {
            if (!File.Exists("node.key") || !File.Exists("node.key.pub"))
            {
                Console.WriteLine("Error: Please generate an RSA keypair with an empty password and save it to node.key and node.key.pub\n" +
                    "$ openssl genrsa -out node.key && openssl rsa -pubout -in node.key -out node.key.pub\n" +
                    "Distribute node.key.pub to a network authority node and add it to the trusted nodes with:\n" +
                    "$ mono StarNet.exe add-node <ip address> <port> <path/to/key.pub>");
                return;
            }
            LoadKeys();
            LocalSettings settings = new LocalSettings();
            if (File.Exists("node.config"))
            {
                JsonConvert.PopulateObject(File.ReadAllText("node.config"), settings);
                File.WriteAllText("node.config", JsonConvert.SerializeObject(settings, Formatting.Indented));
            }
            else
                settings = FirstRun(settings);
            var sharedDatabase = new SharedDatabase(settings.ConnectionString);
            if (args.Length != 0)
                HandleArguments(args, settings);
            else
            {
                var localNode = new StarNetNode(sharedDatabase, settings, 
                    new CryptoProvider(ServerKey), new IPEndPoint(IPAddress.Any, settings.StarboundPort));
                Console.WriteLine("Starting node {0}", localNode.Settings.UUID);
                localNode.Start();
                while (true)
                    Thread.Sleep(10000);
            }
        }

        static void LoadKeys()
        {
            var reader = new PemReader(new StreamReader("node.key"));
            try
            {
                ServerKey = (AsymmetricCipherKeyPair)reader.ReadObject();
            }
            catch
            {
                Console.WriteLine("Error: Unable to read key file.");
                throw;
            }
            finally
            {
                reader.Reader.Close();
            }
        }

        static LocalSettings FirstRun(LocalSettings settings)
        {
            Console.WriteLine("StarNet Node Setup");
            GetValueFromUser("Starbound port (21025): ", v => settings.StarboundPort = ushort.Parse(v));
            GetValueFromUser("StarNet port (21024): ", v => settings.NetworkPort = ushort.Parse(v));
            GetValueFromUser("PostgreSQL Connection String: ", v => settings.ConnectionString = v, false);
            File.WriteAllText("node.config", JsonConvert.SerializeObject(settings, Formatting.Indented));
            return settings;
        }

        delegate void ApplyValue(string result);
        static void GetValueFromUser(string prompt, ApplyValue apply, bool allowDefaultValue = true)
        {
            while (true)
            {
                Console.Write(prompt);
                var response = Console.ReadLine();
                if (!string.IsNullOrEmpty(response))
                {
                    try
                    {
                        apply(response);
                        break;
                    }
                    catch
                    {
                    }
                }
                else if (allowDefaultValue)
                    break;
            }
        }

        static void HandleArguments(string[] args, LocalSettings settings)
        {
            var action = args[0];
            try
            {
                switch (action)
                {
                    case "ping":
                        PingServer(settings);
                        break;
                }
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine("Error: not enough parameters for {0}", action);
            }
        }

        private static void PingServer(LocalSettings settings)
        {
            var network = new InterNodeNetwork(settings.NetworkPort, new CryptoProvider(ServerKey), true);
            var endPoint = new IPEndPoint(IPAddress.Loopback, settings.NetworkPort);
            network.Start();
            var packet = new PingPacket();
            var reset = new ManualResetEvent(false);
            DateTime sent = default(DateTime);
            packet.ConfirmationReceived += (sender, e) =>
            {
                Console.WriteLine("<- PONG {0}ms", (int)(DateTime.Now - sent).TotalMilliseconds);
                reset.Set();
            };
            sent = DateTime.Now;
            network.Send(packet, endPoint);
            Console.WriteLine("-> PING {0}", endPoint);
            if (!reset.WaitOne(20000))
                Console.WriteLine("Timed out.");
        }
    }
}