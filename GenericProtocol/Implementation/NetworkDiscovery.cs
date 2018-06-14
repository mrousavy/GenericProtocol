﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace GenericProtocol.Implementation
{
    public class NetworkDiscovery : INetworkDiscovery
    {
        // Property to write on (this is just a writebuffer dump)
        private byte[] PingerBytes { get; } = { 1 };

        public async Task<IDiscoveryResult> Discover(int port = Constants.DiscoveryPort)
        {
            // TODO: Make network discovery work
            var ip = new IPEndPoint(IPAddress.Broadcast, port);
            var segment = new ArraySegment<byte>(PingerBytes);

            // Iterate through all interfaces and send broadcast on each
            foreach (var netInterface in NetworkInterface.GetAllNetworkInterfaces())
            {
                foreach (var address in netInterface.GetIPProperties().UnicastAddresses.Select(a => a.Address))
                {
                    if (address.IsIPv6LinkLocal) continue; // Skip IPv6

                    // Open sender socket and dispose on finish
                    using (var client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
                    {
                        client.EnableBroadcast = true; // By default this is disabled
                        client.Bind(new IPEndPoint(address, port));
                        int sent = await client.SendToAsync(segment, SocketFlags.Broadcast, ip);

                        // Build result
                        var result = new DiscoveryResult(sent > 0, -1, null);
                        return result;
                    }
                }
            }

            throw new NetworkInterfaceException("No network interfaces were found!");
        }

        public async void Host(IPAddress networkIp, int port = Constants.DiscoveryPort)
        {
            // TODO: Make network discovery work
            var ip = new IPEndPoint(networkIp, port);
            var segment = new ArraySegment<byte>(PingerBytes);

            // Open listener socket and dispose on error
            using (var listener = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
            {
                listener.EnableBroadcast = true;
                listener.Bind(ip); // bind to given IP Address
                // TODO: listener.Listen(Constants.MaxConnectionsBacklog); // Listen for incoming connections
                while (true)
                {
                    // Loop until error
                    // TODO: var client = await listener.AcceptAsync(); // Wait until client connects
                    int received = await listener.ReceiveAsync(segment, SocketFlags.None); // receive from new socket
                    if (received < 1) break; // Received null-byte terminator; exit function
                }
            }
        }
    }

    public struct DiscoveryResult : IDiscoveryResult, IEquatable<DiscoveryResult>
    {
        public bool Any { get; }
        public int HostsCount { get; }
        public IEnumerable<IPEndPoint> Hosts { get; }

        public DiscoveryResult(bool any, int count, IEnumerable<IPEndPoint> hosts)
        {
            Any = any;
            HostsCount = count;
            Hosts = hosts;
        }

        public bool Equals(DiscoveryResult other) =>
            Any == other.Any && HostsCount == other.HostsCount && Equals(Hosts, other.Hosts);

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is DiscoveryResult && Equals((DiscoveryResult) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Any.GetHashCode();
                hashCode = (hashCode * 397) ^ HostsCount;
                hashCode = (hashCode * 397) ^ (Hosts != null ? Hosts.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}