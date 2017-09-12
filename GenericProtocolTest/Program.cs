using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using GenericProtocol.Implementation;

namespace GenericProtocolTest {
    public class Program {
        private static ProtoServer<string> _server;


        private static void Main(string[] args) {
            StartServer();
            StartClient();

            while (true)
            Console.ReadKey();
        }


        private static void StartClient() {
            //var client = new GenericProtocol.Implementation.ProtoClient<string>(IPAddress.Any, 1024);
            //client.Start().GetAwaiter().GetResult();
        }

        private static void StartServer() {
            _server = new ProtoServer<string>(IPAddress.Any, 1024);
            Console.WriteLine("Starting Server...");
            _server.Start().GetAwaiter().GetResult();
            Console.WriteLine("Server started!");
            _server.ClientConnected += ClientConnected;
            _server.ReceivedMessage += MessageReceived;
        }

        private static void MessageReceived(IPAddress sender, string message) {
            Console.WriteLine($"{sender}: {message}");
        }

        private static void ClientConnected(IPAddress address) {
            _server.Send("hello!", address);
        }
    }
}
