using System;
using System.Threading.Tasks;
using GenericProtocol.Implementation;

namespace GenericProtocol {
    /// <summary>
    ///     A <see cref="GenericProtocol" /> Client.
    ///     <para />
    ///     (<see cref="T" /> should be marked as <see cref="ZeroFormatter.ZeroFormattableAttribute" />)
    /// </summary>
    /// <typeparam name="T">
    ///     The Type of the Messages to use
    ///     (has to be ZeroFormatter marked, see: <see href="https://github.com/neuecc/ZeroFormatter" />)
    /// </typeparam>
    public interface IClient<T> : IDisposable {
        /// <summary>
        ///     Indicating whether this <see cref="IClient{T}"/>
        ///     should automatically reconnect to the <see cref="IServer{T}"/>
        ///     once a connection is lost
        /// </summary>
        bool AutoReconnect { get; set; }
        /// <summary>
        ///     The size of receive buffers (should be equal or 
        ///     less than bandwidth)
        /// </summary>
        int ReceiveBufferSize { get; set; }
        /// <summary>
        ///     The size of send buffers (should be equal or 
        ///     less than bandwidth)
        /// </summary>
        int SendBufferSize { get; set; }
        /// <summary>
        ///     Event for received messages
        /// </summary>
        event ReceivedHandler<T> ReceivedMessage;
        /// <summary>
        ///     Event on connection to server loss
        /// </summary>
        event ConnectionContextHandler ConnectionLost;

        /// <summary>
        ///     Connect the Socket to the set IP Address
        ///     and start receiving messages
        /// </summary>
        /// <param name="seperateThread">
        ///     True, if the <see cref="ProtoClient{T}" />
        ///     should be operating on a seperate Thread.
        /// </param>
        Task Connect(bool seperateThread);

        /// <summary>
        ///     Gracefully disconnect from the Server
        /// </summary>
        void Disconnect();

        /// <summary>
        ///     Send a new Message to the Server
        /// </summary>
        /// <param name="message">
        ///     The message object
        ///     to serialize and send to the server
        /// </param>
        Task Send(T message);
    }
}