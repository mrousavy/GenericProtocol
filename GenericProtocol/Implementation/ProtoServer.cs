using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace GenericProtocol.Implementation {
    public class ProtoServer<T> : IServer<T> {
        #region Properties
        public event ClientContextHandler ClientConnected;
        public event ClientContextHandler ClientDisconnected;
        public event ReceivedHandler<T> ReceivedMessage;

        private IPEndPoint EndPoint { get; }
        private Socket Socket { get; }
        private IList<IPAddress> Clients { get; }
        #endregion

        #region ctor
        /// <summary>
        /// Create a new instance of the <see cref="GenericProtocol{T}"/>
        /// with the default <see cref="AddressFamily"/> and <see cref="SocketType"/>.
        /// Use <see cref="Start"/> to bind and start the socket.
        /// </summary>
        /// <param name="address">The <see cref="IPAddress"/> to start this Protocol on</param>
        /// <param name="port">The Port to start this Protocol on</param>
        public ProtoServer(IPAddress address, int port) : 
            this(address, port, AddressFamily.InterNetwork, SocketType.Stream) { }

        /// <summary>
        /// Create a new instance of the <see cref="GenericProtocol{T}"/>.
        /// Use <see cref="Start"/> to bind and start the socket.
        /// </summary>
        /// <param name="family">The <see cref="AddressFamily"/> 
        /// this <see cref="System.Net.Sockets.Socket"/> should use</param>
        /// <param name="type">The <see cref="SocketType"/> this 
        /// <see cref="System.Net.Sockets.Socket"/> should use</param>
        /// <param name="address">The <see cref="IPAddress"/> to start this Protocol on</param>
        /// <param name="port">The Port to start this Protocol on</param>
        public ProtoServer(IPAddress address, int port, AddressFamily family, SocketType type) {
            Clients = new List<IPAddress>();
            EndPoint = new IPEndPoint(address, port);
            Socket = new Socket(family, type, ProtocolType.Tcp);
        }
        #endregion

        /// <summary>
        /// Bind and Start the Server to the set IP Address.
        /// </summary>
        public Task Start() {
            return Task.Run(() => Socket.Bind(EndPoint));
        }

        /// <summary>
        /// Shutdown the server and all active clients
        /// </summary>
        public Task Stop() {
            throw new NotImplementedException();
        }
        
        public Task Send(T message, IPAddress to) {
            throw new NotImplementedException();
        }

        public Task Broadcast(T message) {
            throw new NotImplementedException();
        }

        public void Dispose() {
            Socket?.Dispose();
        }
    }
}
