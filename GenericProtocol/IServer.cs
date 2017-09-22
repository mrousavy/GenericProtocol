using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using GenericProtocol.Implementation;
using ZeroFormatter;

namespace GenericProtocol {
    /// <summary>
    ///     A <see cref="GenericProtocol" /> Server.
    ///     <para />
    ///     (<see cref="T" /> should be marked as <see cref="ZeroFormattableAttribute" />)
    /// </summary>
    /// <typeparam name="T">
    ///     The Type of the Messages to use
    ///     (has to be ZeroFormatter marked, see: <see href="https://github.com/neuecc/ZeroFormatter" />)
    /// </typeparam>
    public interface IServer<T> : IDisposable {
        /// <summary>
        ///     Delay between Client pings in milliseconds
        /// </summary>
        int PingDelay { get; set; }
        /// <summary>
        ///     The size of the receive buffers (should be equal or 
        ///     less than bandwidth)
        /// </summary>
        int ReceiveBufferSize { get; set; }
        /// <summary>
        ///     The size of the send buffers (should be equal or 
        ///     less than bandwidth)
        /// </summary>
        int SendBufferSize { get; set; }
        /// <summary>
        ///     The maximum clients to queue on simultanious connection attempts
        /// </summary>
        int MaxConnectionsBacklog { get; set; }
        /// <summary>
        ///     An list of all the connected clients' <see cref="IPEndPoint"/>s
        /// </summary>
        IEnumerable<IPEndPoint> Clients { get; }

        /// <summary>
        ///     Event for just connected clients
        /// </summary>
        event ConnectionContextHandler ClientConnected;
        /// <summary>
        ///     Event for just disconnected clients
        /// </summary>
        event ConnectionContextHandler ClientDisconnected;
        /// <summary>
        ///     Event for received messages
        /// </summary>
        event ReceivedHandler<T> ReceivedMessage;

        /// <summary>
        ///     Bind the Socket to the set IP Address
        ///     and start listening for incoming connections
        /// </summary>
        /// <param name="seperateThread">
        ///     True, if the <see cref="ProtoClient{T}" />
        ///     should be operating on a seperate Thread.
        /// </param>
        void Start(bool seperateThread);

        /// <summary>
        ///     Stop the Server and disconnect all Clients
        ///     gracefully
        /// </summary>
        void Stop();

        /// <summary>
        ///     Send a new Message to the Client
        /// </summary>
        /// <param name="message">
        ///     The message object
        ///     to serialize and send to the client
        /// </param>
        /// <param name="to">
        ///     The client (IP + Port)
        ///     to send the message to
        /// </param>
        Task Send(T message, IPEndPoint to);

        /// <summary>
        ///     Broadcast a new Message to all connected Clients
        /// </summary>
        /// <param name="message">
        ///     The message object
        ///     to serialize and send to the client
        /// </param>
        Task Broadcast(T message);

        /// <summary>
        ///     Gracefully disconnect a Client with the given
        ///     IP Address and Port and returns true if successful
        /// </summary>
        /// <param name="endPoint">
        ///     The Client to kick by IP Address and Port
        /// </param>
        /// <exception cref="NotFoundException">
        ///     Thrown when the given <see cref="IPEndPoint" />
        ///     could not be found in the connected clients.
        /// </exception>
        /// <returns>
        ///     Indicating whether the disconnect was successful
        /// </returns>
        bool Kick(IPEndPoint endPoint);
    }
}