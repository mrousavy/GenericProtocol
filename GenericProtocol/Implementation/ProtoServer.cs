using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using ZeroFormatter;

namespace GenericProtocol.Implementation {
    public class ProtoServer<T> : IServer<T> {
        #region Properties

        public int MaxConnectionsBacklog { get; set; } = Constants.MaxConnectionsBacklog;
        public int PingDelay { get; set; } = Constants.PingDelay;
        public int ReceiveBufferSize { get; set; } = Constants.ReceiveBufferSize;
        public int SendBufferSize { get; set; } = Constants.SendBufferSize;
        public IEnumerable<IPEndPoint> Clients => Sockets.Keys;

        public event ConnectionContextHandler ClientConnected;
        public event ConnectionContextHandler ClientDisconnected;
        public event ReceivedHandler<T> ReceivedMessage;

        private IPEndPoint EndPoint { get; }
        private Socket Socket { get; }
        private IDictionary<IPEndPoint, Socket> Sockets { get; }

        #endregion

        #region ctor

        /// <summary>
        ///     Create a new instance of the <see cref="ProtoServer{T}" />
        ///     with the default <see cref="AddressFamily" /> and <see cref="SocketType" />.
        ///     Use <see cref="Start" /> to bind and start the socket.
        /// </summary>
        /// <param name="address">The <see cref="IPAddress" /> to start this Protocol on</param>
        /// <param name="port">The Port to start this Protocol on</param>
        public ProtoServer(IPAddress address, int port) :
            this(address, port, AddressFamily.InterNetwork, SocketType.Stream) { }

        /// <summary>
        ///     Create a new instance of the <see cref="ProtoServer{T}" />.
        ///     Use <see cref="Start" /> to bind and start the socket.
        /// </summary>
        /// <param name="family">
        ///     The <see cref="AddressFamily" />
        ///     this <see cref="System.Net.Sockets.Socket" /> should use
        /// </param>
        /// <param name="type">
        ///     The <see cref="SocketType" /> this
        ///     <see cref="System.Net.Sockets.Socket" /> should use
        /// </param>
        /// <param name="address">The <see cref="IPAddress" /> to start this Protocol on</param>
        /// <param name="port">The Port to start this Protocol on</param>
        public ProtoServer(IPAddress address, int port, AddressFamily family, SocketType type) {
            Sockets = new Dictionary<IPEndPoint, Socket>();
            EndPoint = new IPEndPoint(address, port);
            Socket = new Socket(family, type, ProtocolType.Tcp);
        }

        #endregion

        #region Functions

        /// <summary>
        ///     Bind and Start the Server to the set IP Address.
        /// </summary>
        public void Start(bool seperateThread = false) {
            Socket.Bind(EndPoint);

            if (seperateThread) {
                new Thread(StartListening).Start();
            } else {
                StartListening();
            }
        }

        /// <summary>
        ///     Shutdown the server and all active clients
        /// </summary>
        public void Stop() {
            foreach (KeyValuePair<IPEndPoint, Socket> kvp in Sockets)
                try {
                    DisconnectClient(kvp.Key);
                } catch {
                    // could not disconnect client
                }
        }

        public async Task Send(T message, IPEndPoint to) {
            if (message == null) {
                throw new ArgumentNullException(nameof(message));
            }

            // Build a byte array of the serialized data
            byte[] bytes = ZeroFormatterSerializer.Serialize(message);
            ArraySegment<byte> segment = new ArraySegment<byte>(bytes);

            // Find socket
            var socket = Sockets.FirstOrDefault(c => c.Key.Equals(to)).Value;
            if (socket == null) {
                throw new NetworkInterfaceException($"The IP Address {to} could not be found!");
            }

            int size = bytes.Length;
            await LeadingByteProcessor.SendLeading(socket, size).ConfigureAwait(false); // send leading size

            //TODO: Do something when sending interrupts? Wait for client to come back?
            // Write buffered
            int written = 0;
            while (written < size) {
                int send = size - written; // current buffer size
                if (send > SendBufferSize) {
                    send = SendBufferSize; // max size
                }

                ArraySegment<byte> slice = segment.SliceEx(written, send); // buffered portion of array
                written = await socket.SendAsync(slice, SocketFlags.None).ConfigureAwait(false);
            }

            if (written < 1) {
                throw new TransferException($"{written} bytes were sent! " +
                                            "Null bytes could mean a connection shutdown.");
            }
        }

        public async Task Broadcast(T message) {
            // Build list of Send(..) tasks
            List<Task> tasks = Sockets.Select(client => Send(message, client.Key)).ToList();
            // await all
            await Task.WhenAll(tasks).ConfigureAwait(false);
        }


        public bool Kick(IPEndPoint endPoint) {
            return DisconnectClient(endPoint);
        }

        public void Dispose() {
            Stop();
            Socket?.Dispose();
        }

        #endregion

        #region Privates

        // Endless Start listening loop
        private async void StartListening() {
            Socket.Listen(10);
            // Loop theoretically infinetly
            while (true) {
                var client = await Socket.AcceptAsync().ConfigureAwait(false); // Block until accept
                var endpoint = client.RemoteEndPoint as IPEndPoint; // Get remote endpoint
                Sockets.Add(endpoint, client); // Add client to dictionary

                StartReading(client); // Start listening for data
                KeepAlive(client); // Keep client alive and ping

                ClientConnected?.Invoke(endpoint); // call event
            }
            // Listen again after client connected
        }

        // Endless Start reading loop
        private async void StartReading(Socket client) {
            var endpoint = client.RemoteEndPoint as IPEndPoint; // Get remote endpoint

            // Loop theoretically infinetly
            while (true) {
                try {
                    long size = await LeadingByteProcessor.ReadLeading(client).ConfigureAwait(false); // leading

                    byte[] bytes = new byte[size];
                    ArraySegment<byte> segment = new ArraySegment<byte>(bytes);
                    //TODO: Do something when receiving interrupts? Wait for client to come back?
                    // read until all data is read
                    int read = 0;
                    while (read < size) {
                        long receive = size - read; // current buffer size
                        if (receive > ReceiveBufferSize) {
                            receive = ReceiveBufferSize; // max size
                        }

                        ArraySegment<byte>
                            slice = segment.SliceEx(read, (int)receive); // get buffered portion of array
                        read += await client.ReceiveAsync(slice, SocketFlags.None).ConfigureAwait(false);
                    }

                    if (read < 1) {
                        throw new TransferException($"{read} bytes were read! " +
                                                    "Null bytes could mean a connection shutdown.");
                    }

                    var message = ZeroFormatterSerializer.Deserialize<T>(segment.Array);

                    ReceivedMessage?.Invoke(endpoint, message); // call event
                } catch (SocketException ex) {
                    Console.WriteLine(ex.ErrorCode);
                    bool success = DisconnectClient(endpoint); // try to disconnect
                    if (success) // Exit Reading loop once successfully disconnected
                        return;
                } catch (TransferException) {
                    // 0 read bytes = null byte
                    bool success = DisconnectClient(endpoint); // try to disconnect
                    if (success) {
                        // Exit Reading loop once successfully disconnected
                        return;
                    }
                }
            } // Listen again after client connected
        }

        // Keep a Client alive by pinging
        private async void KeepAlive(Socket client) {
            while (true) {
                await Task.Delay(PingDelay).ConfigureAwait(false);

                bool isAlive = client.Ping();
                if (isAlive) {
                    continue; // Client responded
                }

                // Client does not respond, disconnect & exit
                DisconnectClient(client.RemoteEndPoint as IPEndPoint);
                return;
            }
        }

        // Disconnect a client; returns true if successful
        private bool DisconnectClient(IPEndPoint endPoint) {
            // Get all EndPoints/Sockets where the endpoint matches with this argument
            var filtered = Sockets.Where(c => c.Key.Equals(endPoint)).ToArray();
            // .count should always be 1, CAN be more -> Loop
            foreach (KeyValuePair<IPEndPoint, Socket> kvp in filtered)
                try {
                    kvp.Value.Disconnect(false); // Gracefully disconnect socket
                    kvp.Value.Close();
                    kvp.Value.Dispose();

                    Sockets.Remove(kvp.Key); // Remove from collection
                    ClientDisconnected?.Invoke(kvp.Key); // Event
                } catch {
                    // Socket is either already disconnected, or failing to disconnect. try ping
                    return !kvp.Value.Ping();
                }
            return true;
        }

        #endregion
    }
}