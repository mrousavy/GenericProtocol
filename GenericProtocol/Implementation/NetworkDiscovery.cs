using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace GenericProtocol.Implementation {
    public class NetworkDiscovery : INetworkDiscovery {
        private byte[] PingerBytes { get; } = { 1 };

        public async Task<IDiscoveryResult> Discover(IPAddress networkIp, int port = Constants.DiscoveryPort) {
            // TODO: Make network discovery work
            var ip = new IPEndPoint(IPAddress.Broadcast, port);
            ArraySegment<byte> segment = new ArraySegment<byte>(PingerBytes);

            using (var client = new TcpClient(ip)) {
                int sent = await client.Client.SendAsync(segment, SocketFlags.Broadcast);

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
            ArraySegment<byte> segment = new ArraySegment<byte>(PingerBytes);
            var ip = new IPEndPoint(networkIp, port);

            var client = new TcpListener(ip);
            client.Start();

            while (true) {
                var socket = await client.AcceptSocketAsync();
                int received = await socket.ReceiveAsync(segment, SocketFlags.None); // receive from new socket
                if (received < 1) return; // Received null-byte terminator; exit function
            }
        }
    }

    public struct DiscoveryResult : IDiscoveryResult {
        public bool Any { get; set; }
        public int HostsCount { get; set; }
        public IEnumerable<IPEndPoint> Hosts { get; set; }
    }
}
