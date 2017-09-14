using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using ZeroFormatter;

namespace GenericProtocol.Implementation {
    /// <summary>
    ///     A protocol for transferring binary large objects (BLOBs) like images or files.
    /// </summary>
    public class BinaryUplink {
        // TODO:
        //#region Properties
        //public int ReceiveBufferSize { get; set; } = Constants.ReceiveBufferSize;
        //public int SendBufferSize { get; set; } = Constants.SendBufferSize;
        //private IPEndPoint EndPoint { get; }
        //private Socket Socket { get; }
        //public bool AutoReconnect { get; set; }
        //public event ReceivedHandler<byte[]> ReceivedMessage;
        //public event ConnectionContextHandler ConnectionLost;

        //#endregion

        //#region ctor
        ///// <summary>
        /////     Create a new instance of the <see cref="BinaryUplink" />
        /////     with the default <see cref="AddressFamily" /> and <see cref="SocketType" />.
        /////     Use <see cref="Connect" /> to start and connect the socket.
        ///// </summary>
        ///// <param name="address">The server's <see cref="IPAddress" /> to connect to</param>
        ///// <param name="port">The server's Port to connect to</param>
        //public BinaryUplink(IPAddress address, int port) :
        //    this(address, port, AddressFamily.InterNetwork, SocketType.Stream) { }

        ///// <summary>
        /////     Create a new instance of the <see cref="BinaryUplink" />
        /////     with the default <see cref="AddressFamily" /> and <see cref="SocketType" />.
        /////     Use <see cref="Connect" /> to start and connect the socket.
        ///// </summary>
        ///// <param name="family">
        /////     The <see cref="AddressFamily" />
        /////     this <see cref="System.Net.Sockets.Socket" /> should use
        ///// </param>
        ///// <param name="type">
        /////     The <see cref="SocketType" /> this
        /////     <see cref="System.Net.Sockets.Socket" /> should use
        ///// </param>
        ///// <param name="address">The server's <see cref="IPAddress" /> to connect to</param>
        ///// <param name="port">The server's Port to connect to</param>
        //public BinaryUplink(IPAddress address, int port, AddressFamily family, SocketType type) {
        //    EndPoint = new IPEndPoint(address, port);
        //    Socket = new Socket(family, type, ProtocolType.Tcp);
        //}
        //#endregion

        //#region Functions

        //public async Task Connect(bool seperateThread = false) {
        //    await Socket.ConnectAsync(EndPoint);

        //    if (seperateThread) {
        //        // Launch on a new Thread
        //        new Thread(StartReceiving).Start();
        //        new Thread(KeepAlive).Start();
        //    } else {
        //        // Use Tasks
        //        StartReceiving();
        //        KeepAlive();
        //    }
        //}

        //public void Disconnect() {
        //    try {
        //        Socket?.Disconnect(false);
        //        Socket?.Close();
        //        Socket?.Dispose();
        //    } catch (ObjectDisposedException) {
        //        // already stopped
        //    }
        //}

        //public async Task Send(byte[] message) {
        //    if (message == null) throw new ArgumentNullException(nameof(message));

        //    bool alive = Socket.Ping();
        //    if (!alive) throw new TransferException($"The Socket to {EndPoint} is not responding!");

        //    try {
        //        // build byte[] out of message (serialize with ZeroFormatter)
        //        ArraySegment<byte> segment = new ArraySegment<byte>(message);

        //        int size = message.Length;
        //        await SendLeading(size); // Send receiver the byte count

        //        int written = 0;
        //        while (written < size) {
        //            int send = size - written; // current buffer size
        //            if (send > ReceiveBufferSize)
        //                send = ReceiveBufferSize; // max size

        //            ArraySegment<byte> slice = segment.SliceEx(written, send); // buffered portion of array
        //            written = await Socket.SendAsync(slice, SocketFlags.None);
        //        }

        //        if (written < 1)
        //            throw new TransferException($"{written} bytes were sent! " +
        //                                        "Null bytes could mean a connection shutdown.");
        //    } catch {
        //        if (AutoReconnect) await Reconnect(); // Try reconnecting
        //        else throw; // Throw Exception if Socket should not Auto-reconnect
        //    }
        //}

        //public void Dispose() {
        //    Disconnect();
        //}

        //#endregion

        //#region Privates

        //// Endless Start reading loop
        //private async void StartReceiving() {
        //    // Loop theoretically infinetly
        //    while (true)
        //        try {
        //            // Read the leading "byte"
        //            long size = await ReadLeading();

        //            byte[] bytes = new byte[size];
        //            ArraySegment<byte> segment = new ArraySegment<byte>(bytes);
        //            // read until all data is read
        //            int read = 0;
        //            while (read < size) {
        //                long receive = size - read; // current buffer size
        //                if (receive > ReceiveBufferSize)
        //                    receive = ReceiveBufferSize; // max size

        //                ArraySegment<byte>
        //                    slice = segment.SliceEx(read, (int)receive); // get buffered portion of array
        //                read += await Socket.ReceiveAsync(slice, SocketFlags.None);
        //            }

        //            ReceivedMessage?.Invoke(EndPoint, segment.Array); // call event
        //        } catch (ObjectDisposedException) {
        //            return; // Socket was closed & disposed -> exit
        //        } catch {
        //            if (!AutoReconnect) throw; // Throw Exception if Socket should not Auto-reconnect
        //        }
        //    // Listen again after client connected
        //}

        //// Read the prefix from a message (number of following bytes)
        //private async Task<long> ReadLeading() {
        //    byte[] bytes = new byte[sizeof(long)];
        //    ArraySegment<byte> segment = new ArraySegment<byte>(bytes);
        //    // read leading bytes
        //    int read = await Socket.ReceiveAsync(segment, SocketFlags.None);

        //    if (read < 1)
        //        throw new TransferException($"{read} lead-bytes were read! " +
        //                                    "Null bytes could mean a connection shutdown.");

        //    // size of the following byte[]
        //    long size = ZeroFormatterSerializer.Deserialize<long>(segment.Array);
        //    return size;
        //}

        //// Send the prefix from a message (number of following bytes)
        //private async Task SendLeading(long size) {
        //    // build byte[] out of size
        //    byte[] bytes = ZeroFormatterSerializer.Serialize(size);
        //    ArraySegment<byte> segment = new ArraySegment<byte>(bytes);
        //    // send leading bytes
        //    int sent = await Socket.SendAsync(segment, SocketFlags.None);

        //    if (sent < 1)
        //        throw new TransferException($"{sent} lead-bytes were sent! " +
        //                                    "Null bytes could mean a connection shutdown.");
        //}

        //// Reconnect the Socket connection
        //private async Task Reconnect() {
        //    while (true) {
        //        try {
        //            Socket.Disconnect(true);
        //            await Connect();
        //            return;
        //        } catch (SocketException) {
        //            // could not connect
        //        }
        //        await Task.Delay(Constants.ReconnectInterval);
        //    }
        //}

        //// Keep server connection alive by pinging
        //private async void KeepAlive() {
        //    while (true) {
        //        await Task.Delay(Constants.PingDelay);

        //        bool isAlive = Socket.Ping();
        //        if (isAlive) continue; // Client responded

        //        ConnectionLost?.Invoke(EndPoint);
        //        // Client does not respond, try reconnecting, or disconnect & exit
        //        if (AutoReconnect) await Reconnect();
        //        else Disconnect();
        //        return;
        //    }
        //}

        //#endregion
    }
}
