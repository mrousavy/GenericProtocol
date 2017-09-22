using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using ZeroFormatter;

namespace GenericProtocol.Implementation {
    public class ProtoClient<T> : IClient<T> {
        #region Properties

        public int ReceiveBufferSize { get; set; } = Constants.ReceiveBufferSize;
        public int SendBufferSize { get; set; } = Constants.SendBufferSize;
        public ConnectionStatus ConnectionStatus { get; set; } // Status of connection

        public event ReceivedHandler<T> ReceivedMessage;
        public event ConnectionContextHandler ConnectionLost;

        public bool AutoReconnect { get; set; }

        private IPEndPoint EndPoint { get; } // Server (remote) EndPoint
        private Socket Socket { get; } // Actual underlying socket

        #endregion

        #region ctor

        /// <summary>
        ///     Create a new instance of the <see cref="ProtoClient{T}" />
        ///     with the default <see cref="AddressFamily" /> and <see cref="SocketType" />.
        ///     Use <see cref="Connect" /> to start and connect the socket.
        /// </summary>
        /// <param name="address">The server's <see cref="IPAddress" /> to connect to</param>
        /// <param name="port">The server's Port to connect to</param>
        public ProtoClient(IPAddress address, int port) :
            this(address, port, AddressFamily.InterNetwork, SocketType.Stream) { }

        /// <summary>
        ///     Create a new instance of the <see cref="ProtoClient{T}" />
        ///     Use <see cref="Connect" /> to start and connect the socket.
        /// </summary>
        /// <param name="family">
        ///     The <see cref="AddressFamily" />
        ///     this <see cref="System.Net.Sockets.Socket" /> should use
        /// </param>
        /// <param name="type">
        ///     The <see cref="SocketType" /> this
        ///     <see cref="System.Net.Sockets.Socket" /> should use
        /// </param>
        /// <param name="address">The server's <see cref="IPAddress" /> to connect to</param>
        /// <param name="port">The server's Port to connect to</param>
        public ProtoClient(IPAddress address, int port, AddressFamily family, SocketType type) {
            EndPoint = new IPEndPoint(address, port);
            Socket = new Socket(family, type, ProtocolType.Tcp);
        }

        #endregion

        #region Functions

        public async Task Connect(bool seperateThread = false) {
            if (ConnectionStatus == ConnectionStatus.Connected) throw new Exception("Already connected!");

            await Socket.ConnectAsync(EndPoint).ConfigureAwait(false);
            ConnectionStatus = ConnectionStatus.Connected;

            if (seperateThread) {
                // Launch on a new Thread
                new Thread(StartReceiving).Start();
                new Thread(KeepAlive).Start();
            } else {
                // Use Tasks
                StartReceiving();
                KeepAlive();
            }
        }

        public void Disconnect() {
            try {
                Socket?.Disconnect(false);
                ConnectionStatus = ConnectionStatus.Disconnected;
                Socket?.Close();
                Socket?.Dispose();
            } catch (ObjectDisposedException) {
                // already stopped
            }
        }

        public async Task Send(T message) {
            if (message == null) throw new ArgumentNullException(nameof(message));

            bool alive = Socket.Ping();
            if (!alive) throw new TransferException($"The Socket to {EndPoint} is not responding!");

            try {
                // build byte[] out of message (serialize with ZeroFormatter)
                byte[] bytes = ZeroFormatterSerializer.Serialize(message);
                ArraySegment<byte> segment = new ArraySegment<byte>(bytes);

                int size = bytes.Length;
                await LeadingByteProcessor.SendLeading(Socket, size).ConfigureAwait(false); // Send receiver the byte count

                //TODO: Decide whether to catch errors in buffer-loop and continue once fixed or cancel whole send?
                int written = 0;
                while (written < size) {
                    int send = size - written; // current buffer size
                    if (send > SendBufferSize)
                        send = SendBufferSize; // max size

                    ArraySegment<byte> slice = segment.SliceEx(written, send); // buffered portion of array
                    written = await Socket.SendAsync(slice, SocketFlags.None).ConfigureAwait(false);
                }

                if (written < 1)
                    throw new TransferException($"{written} bytes were sent! " +
                                                "Null bytes could mean a connection shutdown.");
            } catch (SocketException) {
                ConnectionLost?.Invoke(EndPoint);
                // On any error - cancel whole buffered writing
                if (AutoReconnect) {
                    await Reconnect().ConfigureAwait(false); // Try reconnecting and re-send everything once reconnected
                } else {
                    throw; // Throw if we're not trying to reconnect
                }
            }
        }

        public void Dispose() => Disconnect();

        #endregion

        #region Privates

        // Endless Start reading loop
        private async void StartReceiving() {
            // Loop theoretically infinetly
            while (true) {
                try {
                    // Read the leading "byte"
                    long size = await LeadingByteProcessor.ReadLeading(Socket).ConfigureAwait(false);

                    byte[] bytes = new byte[size];
                    ArraySegment<byte> segment = new ArraySegment<byte>(bytes);
                    //TODO: Decide whether to catch errors in buffer-loop and continue once fixed or cancel whole receive?
                    // read until all data is read
                    int read = 0;
                    while (read < size) {
                        long receive = size - read; // current buffer size
                        if (receive > ReceiveBufferSize) {
                            receive = ReceiveBufferSize; // max size
                        }

                        ArraySegment<byte>
                            slice = segment.SliceEx(read, (int)receive); // get buffered portion of array
                        read += await Socket.ReceiveAsync(slice, SocketFlags.None).ConfigureAwait(false);
                    }

                    var message = ZeroFormatterSerializer.Deserialize<T>(segment.Array);

                    ReceivedMessage?.Invoke(EndPoint, message); // call event
                } catch (ObjectDisposedException) {
                    return; // Socket was closed & disposed -> exit
                } catch (SocketException) {
                    ConnectionLost?.Invoke(EndPoint);
                    if (!AutoReconnect) {
                        await Reconnect().ConfigureAwait(false); // Try reconnecting on an error, then continue receiving
                    }
                }
                // Listen again after client connected
            }
        }

        // Reconnect the Socket connection
        private async Task Reconnect() {
            // Don't reconnect if we're already reconnecting somewhere else
            if (ConnectionStatus == ConnectionStatus.Connecting) return;

            ConnectionStatus = ConnectionStatus.Connecting; // Connecting...
            while (true) {
                try {
                    Socket.Disconnect(true); // Disconnect and reserve socket
                    await Socket.ConnectAsync(EndPoint).ConfigureAwait(false); // Connect to Server
                    ConnectionStatus = ConnectionStatus.Connected;
                    return;
                } catch (SocketException) {
                    // could not connect
                }
                await Task.Delay(Constants.ReconnectInterval).ConfigureAwait(false); // Try to reconnect all x milliseconds
            }
        }

        // Keep server connection alive by pinging
        private async void KeepAlive() {
            while (true) {
                await Task.Delay(Constants.PingDelay).ConfigureAwait(false);

                bool isAlive = Socket.Ping(); // Try to ping the server
                if (isAlive) continue; // Client responded, continue pinger

                // ---- Socket is NOT alive: ---- //
                ConnectionLost?.Invoke(EndPoint);
                // Client does not respond, try reconnecting, or disconnect & exit
                if (AutoReconnect) {
                    await Reconnect().ConfigureAwait(false); // Wait for reconnect
                } else {
                    Disconnect(); // Stop and exit
                    return;
                }
            }
        }

        #endregion
    }
}