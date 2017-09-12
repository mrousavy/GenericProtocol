using GenericProtocol.Implementation;
using System;
using System.Net.Sockets;

namespace GenericProtocol {
    public interface IServer<T> : IDisposable {
        event ClientContextHandler ClientConnected;
        event ClientContextHandler ClientDisconnected;
        event ReceivedHandler<T> ReceivedMessage; 

        /// <summary>
        /// Bind the Socket to the set IP Address 
        /// and start listening for incoming connections
        /// </summary>
        void Start();

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
        void Send(T message);
    }
}
