using System;
using System.Net;

namespace GenericProtocol.Implementation {
    public delegate void ConnectionContextHandler(
        IPEndPoint endPoint);

    public delegate void ReceivedHandler<in T>(
        IPEndPoint senderEndPoint,
        T message);

    /// <summary>
    /// The status of a Connection
    /// </summary>
    public enum ConnectionStatus {
        /// <summary>
        /// Indicating that the <see cref="IClient{T}"/>
        /// or <see cref="IServer{T}"/> is currently
        /// disconnected
        /// </summary>
        Disconnected = 0,
        /// <summary>
        /// Indicating that the <see cref="IClient{T}"/>
        /// or <see cref="IServer{T}"/> is currently
        /// trying to (re-)connect
        /// </summary>
        Connecting = 1,
        /// <summary>
        /// Indicating that the <see cref="IClient{T}"/>
        /// or <see cref="IServer{T}"/> is currently
        /// bound and connected
        /// </summary>
        Connected = 2
    }

    internal static class Constants {
        internal const int ReceiveBufferSize = 1024; // Size of the receive buffer
        internal const int SendBufferSize = 1024; // Size of the send buffer
        internal const int LeadingByteSize = sizeof(int); // Size of the leading byte prefix
        internal const int MaxConnectionsBacklog = 10; // Maximum num. of sim. connection requests to queue
        internal const int PingDelay = 5000; // Delay between ping messages
        internal const int ReconnectInterval  = 500; // Interval between reconnect attempts
        internal const int DiscoveryPort = 15000; // The port use for other GenericProtocol client-discovery
    }


    public class TransferException : Exception {
        public TransferException(string message) : base(message) { }
    }
    public class NotFoundException : Exception {
        public NotFoundException(string message) : base(message) { }
    }
    public class GenericProtocolException : Exception {
        public GenericProtocolException(string message) : base(message) { }
    }
    public class NetworkInterfaceException : Exception {
        public NetworkInterfaceException(string message) : base(message) { }
    }
}