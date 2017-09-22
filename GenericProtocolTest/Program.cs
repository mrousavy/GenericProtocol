using System;
using System.Net;
using GenericProtocol;
using GenericProtocol.Implementation;

namespace GenericProtocolTest {
    public class Program {
        private static ProtoServer<string> _server;
        private static ProtoClient<string> _client;
        private static readonly bool TestServer = false;
        private static readonly bool TestClient = true;
        private static readonly IPAddress ServerIp = IPAddress.Parse("10.0.105.1");

        private static void Main(string[] args) {
            INetworkDiscovery discovery = new NetworkDiscovery();
            discovery.Host(IPAddress.Any);
            discovery.Discover();

            if (TestServer)
                StartServer();
            if (TestClient)
                StartClient();

            Console.WriteLine("\n");

            while (true) {
                string text = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(text)) continue;

                if (TestClient) {
                    SendToServer(text);
                } else {
                    SendToClients(text);
                }
            }
        }


        private static void StartClient() {
            _client = new ProtoClient<string>(ServerIp, 1024) {AutoReconnect = true};
            _client.ReceivedMessage += ClientMessageReceived;
            _client.ConnectionLost += Client_ConnectionLost;

            Console.WriteLine("Connecting");
            _client.Connect().GetAwaiter().GetResult();
            Console.WriteLine("Connected!");
            _client.Send("Hello Server!").GetAwaiter().GetResult();
        }

        private static void SendToServer(string message) { _client?.Send(message); }

        private static void SendToClients(string message) { _server?.Broadcast(message); }

        private static void Client_ConnectionLost(IPEndPoint endPoint) {
            Console.WriteLine($"Connection lost! {endPoint.Address}");
        }

        private static void StartServer() {
            _server = new ProtoServer<string>(IPAddress.Any, 1024);
            Console.WriteLine("Starting Server...");
            _server.Start();
            Console.WriteLine("Server started!");
            _server.ClientConnected += ClientConnected;
            _server.ReceivedMessage += ServerMessageReceived;
        }

        private static async void ServerMessageReceived(IPEndPoint sender, string message) {
            Console.WriteLine($"{sender}: {message}");
            await _server.Send($"Hello {sender}!", sender);
        }
        private static void ClientMessageReceived(IPEndPoint sender, string message) {
            Console.WriteLine($"{sender}: {message}");
        }

        private static async void ClientConnected(IPEndPoint address) {
            await _server.Send($"Hello {address}!", address);
        }
    }
}
