using System;
using System.Net;

namespace GenericProtocol.Implementation {

    public delegate void ConnectionContextHandler(
        IPEndPoint endPoint);

    public delegate void ReceivedHandler<T>(
        IPEndPoint senderEndPoint,
        T message);

    public static class Constants {
        public const int ReceiveBufferSize = 1024; // Byte buffer size for receiving data
        public const int MaxConnectionsBacklog = 10; // Maximum connections for server listening
        public const int PingDelay = 5000; // Ping every x milliseconds
    }


    public class TransferException : Exception {
        public TransferException(string message) : base(message) { }
    }
}
