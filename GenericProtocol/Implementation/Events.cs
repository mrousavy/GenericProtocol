using System.Net;

namespace GenericProtocol.Implementation {

    public enum ClientContextAction {
        /// <summary>
        /// The Client just connected
        /// </summary>
        Connected,
        /// <summary>
        /// The Client disconnected or timed out
        /// </summary>
        Disconnected
    }

    public delegate void ClientContextHandler(
        ClientContextAction action,
        IPAddress clientIp);

    public delegate void ReceivedHandler<T>(
        T message,
        IPAddress senderIp);
}
