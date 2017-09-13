using GenericProtocol.Implementation;
using System;
using System.Threading.Tasks;
using ZeroFormatter;

namespace GenericProtocol {
    /// <summary>
    /// A <see cref="GenericProtocol"/> Client.
    /// <para/>
    /// (<see cref="T"/> should be marked as <see cref="ZeroFormattableAttribute"/>)
    /// </summary>
    /// <typeparam name="T">The Type of the Messages to use
    /// (has to be ZeroFormatter marked, see: <see href="https://github.com/neuecc/ZeroFormatter"/>)</typeparam>
    public interface IClient<T> : IDisposable {
        event ReceivedHandler<T> ReceivedMessage;

        bool AutoReconnect { get; set; }

        /// <summary>
        /// Connect the Socket to the set IP Address
        /// and start receiving messages
        /// </summary>
        Task Start();

        /// <summary>
        /// Gracefully disconnect from the Server
        /// </summary>
        void Stop();

        /// <summary>
        /// Send a new Message to the Server
        /// </summary>
        /// <param name="message">The message object
        /// to serialize and send to the server</param>
        Task Send(T message);
    }
}
