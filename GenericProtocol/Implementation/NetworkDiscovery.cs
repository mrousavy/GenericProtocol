using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace GenericProtocol.Implementation {
    public class NetworkDiscovery : INetworkDiscovery {
        // Property to write on (this is just a writebuffer dump)
        private byte[] PingerBytes { get; } = { 1 };

        public async Task<IDiscoveryResult> Discover(IPAddress networkIp, int port = Constants.DiscoveryPort) {
            // TODO: Make network discovery work
            var ip = new IPEndPoint(IPAddress.Broadcast, port);
            var segment = new ArraySegment<byte>(PingerBytes);
            
            // Open sender socket and dispose on finish
            using (var client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp)) {
                client.EnableBroadcast = true;
                await client.ConnectAsync(ip); // Broadcast message and block until received (even though it's UDP?)
                int sent = await client.SendAsync(segment, SocketFlags.Broadcast);

                // Build result
                var result = new DiscoveryResult {
                    Any = sent > 0,
                    HostsCount = -1
                };
                return result;
            }
        }

        public async void Host(IPAddress networkIp, int port = Constants.DiscoveryPort) {
            // TODO: Make network discovery work
            var ip = new IPEndPoint(networkIp, port);
            var segment = new ArraySegment<byte>(PingerBytes);

            // Open listener socket and dispose on error
            using (var listener = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp)) {
                listener.EnableBroadcast = true;
                listener.Bind(ip); // bind to given IP Address
                listener.Listen(Constants.MaxConnectionsBacklog); // Listen for incoming connections
                while (true) { // Loop until error
                    var client = await listener.AcceptAsync(); // Wait until client connects
                    int received = await listener.ReceiveAsync(segment, SocketFlags.None); // receive from new socket
                    if (received < 1) break; // Received null-byte terminator; exit function
                }
            }
        }
    }

    public struct DiscoveryResult : IDiscoveryResult {
        public bool Any { get; set; }
        public int HostsCount { get; set; }
        public IEnumerable<IPEndPoint> Hosts { get; set; }
    }
}
