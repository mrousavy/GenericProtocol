using System.Net;

namespace GenericProtocol.Implementation {

    public delegate void ClientContextHandler(
        IPAddress clientIp);

    public delegate void ReceivedHandler<T>(
        IPAddress senderIp,
        T message);
}
