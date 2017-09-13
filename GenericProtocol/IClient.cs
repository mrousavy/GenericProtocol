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
        bool AutoReconnect { get; set; }
        event ReceivedHandler<T> ReceivedMessage;

        /// <summary>
        ///     Connect the Socket to the set IP Address
        ///     and start receiving messages
        /// </summary>
        /// <param name="seperateThread">
        ///     True, if the <see cref="ProtoClient{T}" />
        ///     should be operating on a seperate Thread.
        /// </param>
        Task Start(bool seperateThread);

        /// <summary>
        ///     Gracefully disconnect from the Server
        /// </summary>
        void Stop();

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