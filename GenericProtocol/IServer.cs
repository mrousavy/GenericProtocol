using GenericProtocol.Implementation;
using System;
using System.Net;
using System.Threading.Tasks;
using ZeroFormatter;

namespace GenericProtocol {
    /// <summary>
    /// A <see cref="GenericProtocol"/> Server.
    /// <para/>
    /// (<see cref="T"/> should be marked as <see cref="ZeroFormattableAttribute"/>)
    /// </summary>
    /// <typeparam name="T">The Type of the Messages to use
    /// (has to be ZeroFormatter marked, see: <see href="https://github.com/neuecc/ZeroFormatter"/>)</typeparam>
    public interface IServer<T> : IDisposable {
        event ClientContextHandler ClientConnected;
        event ClientContextHandler ClientDisconnected;
        event ReceivedHandler<T> ReceivedMessage;

        /// <summary>
        /// Bind the Socket to the set IP Address 
        /// and start listening for incoming connections
        /// </summary>
        Task Start();

        /// <summary>
        /// Stop the Server and disconnect all Clients
        /// gracefully
        /// </summary>
        void Stop();

        /// <summary>
        /// Send a new Message to the Client
        /// </summary>
        /// <param name="message">The message object
        /// to serialize and send to the client</param>
        /// <param name="to">The client to send the 
        /// message to</param>
        Task Send(T message, IPAddress to);

        /// <summary>
        /// Broadcast a new Message to all connected Clients
        /// </summary>
        /// <param name="message">The message object
        /// to serialize and send to the client</param>
        Task Broadcast(T message);
    }
}
