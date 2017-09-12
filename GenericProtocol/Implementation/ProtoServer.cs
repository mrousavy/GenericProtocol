using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using ZeroFormatter;

namespace GenericProtocol.Implementation {
    public class ProtoServer<T> : IServer<T> {
        #region Properties

        public const int MaxConnectionsBacklog = 10;
        public const int ReceiveBufferSize = 1024;

        public event ClientContextHandler ClientConnected;
        public event ClientContextHandler ClientDisconnected;
        public event ReceivedHandler<T> ReceivedMessage;

        private IPEndPoint EndPoint { get; }
        private Socket Socket { get; }
        private IDictionary<IPAddress, Socket> Clients { get; }
        #endregion

        #region ctor
        /// <summary>
        /// Create a new instance of the <see cref="ProtoServer{T}"/>
        /// with the default <see cref="AddressFamily"/> and <see cref="SocketType"/>.
        /// Use <see cref="Start"/> to bind and start the socket.
        /// </summary>
        /// <param name="address">The <see cref="IPAddress"/> to start this Protocol on</param>
        /// <param name="port">The Port to start this Protocol on</param>
        public ProtoServer(IPAddress address, int port) :
            this(address, port, AddressFamily.InterNetwork, SocketType.Stream) { }

        /// <summary>
        /// Create a new instance of the <see cref="ProtoServer{T}"/>.
        /// Use <see cref="Start"/> to bind and start the socket.
        /// </summary>
        /// <param name="family">The <see cref="AddressFamily"/> 
        /// this <see cref="System.Net.Sockets.Socket"/> should use</param>
        /// <param name="type">The <see cref="SocketType"/> this 
        /// <see cref="System.Net.Sockets.Socket"/> should use</param>
        /// <param name="address">The <see cref="IPAddress"/> to start this Protocol on</param>
        /// <param name="port">The Port to start this Protocol on</param>
        public ProtoServer(IPAddress address, int port, AddressFamily family, SocketType type) {
            Clients = new Dictionary<IPAddress, Socket>();
            EndPoint = new IPEndPoint(address, port);
            Socket = new Socket(family, type, ProtocolType.Tcp);
        }
        #endregion

        /// <summary>
        /// Bind and Start the Server to the set IP Address.
        /// </summary>
        public async Task Start() {
            await Task.Run(() => Socket.Bind(EndPoint));
            StartListening();
        }

        // Endless Start listening loop
        private async void StartListening() {
            Socket.Listen(10);
            // Loop theoretically infinetly
            while (true) {
                try {
                    var client = await Socket.AcceptAsync(); // Block until accept
                    var endpoint = client.RemoteEndPoint as IPEndPoint; // Get remote endpoint
                    var address = endpoint?.Address; // Get IP address
                    Clients.Add(address, client); // Add client to dictionary

                    StartReading(client);

                    ClientConnected?.Invoke(address); // call event
                } catch (SocketException ex) {
                    Console.WriteLine(ex.ErrorCode);
                    //return;
                }
                // Listen again after client connected
            }
        }

        // Endless Start reading loop
        private async void StartReading(Socket client) {
            var endpoint = client.RemoteEndPoint as IPEndPoint; // Get remote endpoint
            var address = endpoint?.Address; // Get IP address

            // Loop theoretically infinetly
            while (true) {
                try {
                    byte[] bytes = new byte[ReceiveBufferSize];
                    ArraySegment<byte> segment = new ArraySegment<byte>(bytes);
                    int read = await client.ReceiveAsync(segment, SocketFlags.None);

                    var message = ZeroFormatterSerializer.Deserialize<T>(segment.Array);

                    ReceivedMessage?.Invoke(address, message); // call event
                } catch (SocketException ex) {
                    Console.WriteLine(ex.ErrorCode);
                    bool success = DisconnectClient(client); // try to disconnect
                    if(success) // Exit Reading loop once successfully disconnected
                        return;
                }
                // Listen again after client connected
            }
        }

        // Disconnect a client; returns true if successful
        private bool DisconnectClient(Socket client) {
            IEnumerable<KeyValuePair<IPAddress, Socket>> filtered = Clients.Where(c => c.Value == client);
            foreach (KeyValuePair<IPAddress, Socket> kvp in filtered) {
                try {
                    kvp.Value.Disconnect(false);
                    kvp.Value.Close();
                    kvp.Value.Dispose();
                    Clients.Remove(kvp.Key);
                } catch {
                    // could not disconnect socket
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Shutdown the server and all active clients
        /// </summary>
        public void Stop() {
            foreach (KeyValuePair<IPAddress, Socket> kvp in Clients) {
                try {
                    DisconnectClient(kvp.Value);
                } catch {
                    // could not disconnect client
                }
            }
        }

        public Task Send(T message, IPAddress to) {
            if(message == null) throw new ArgumentNullException(nameof(message));

            byte[] bytes = ZeroFormatterSerializer.Serialize(message);
            throw new NotImplementedException();
        }

        public Task Broadcast(T message) {
            throw new NotImplementedException();
        }

        public void Dispose() {
            Stop();
            Socket?.Dispose();
        }
    }
}
