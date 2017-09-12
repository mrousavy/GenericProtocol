using System.Net;

namespace GenericProtocol.Implementation {

    public delegate void ClientContextHandler(
        IPAddress clientIp);

    public delegate void ReceivedHandler<T>(
        IPAddress senderIp,
        T message);

    public static class Constants {
        public const int ReceiveBufferSize = 1024; // Byte buffer size for receiving data
        public const int MaxConnectionsBacklog = 10; // Maximum connections for server listening
        public const int PingDelay = 5000; // Ping every x milliseconds
    }
}
