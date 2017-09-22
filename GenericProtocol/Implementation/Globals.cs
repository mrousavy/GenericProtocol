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

    public static class Constants {
        public const int ReceiveBufferSize = 1024; // Byte buffer size for receiving data
        public const int SendBufferSize = 1024; // Byte buffer size for sending data

        // Number of bytes to reserve for the byte size that's going to get sent/received
        public const int LeadingByteSize = sizeof(int);

        public const int MaxConnectionsBacklog = 10; // Maximum connections for server listening
        public const int PingDelay = 5000; // Ping every x milliseconds
        public const int ReconnectInterval = 500; // Try reconnecting every x milliseconds

        public const int DiscoveryPort = 15000; // The port use for other GenericProtocol client-discovery
    }


    public class TransferException : Exception {
        public TransferException(string message) : base(message) { }
    }

    public class NotFoundException : Exception {
        public NotFoundException(string message) : base(message) { }
    }

    public class NetworkInterfaceException : Exception {
        public NetworkInterfaceException(string message) : base(message) { }
    }
}