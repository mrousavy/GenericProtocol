using System.Net;

namespace GenericProtocol.Implementation {

    public delegate void ClientContextHandler(
        IPAddress clientIp);

    public delegate void ReceivedHandler<T>(
        IPAddress senderIp,
        T message);

    public static class Constants {
        public const int ReceiveBufferSize = 1024;
        public const int MaxConnectionsBacklog = 10;
    }
}
