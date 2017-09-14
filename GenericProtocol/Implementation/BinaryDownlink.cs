using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using ZeroFormatter;

namespace GenericProtocol.Implementation {
    /// <summary>
    ///     The server for the protocol for transferring binary large objects (BLOBs) like images or files.
    /// </summary>
    public class BinaryDownlink {
        // TODO:
        //#region Properties

        //public int MaxConnectionsBacklog = Constants.MaxConnectionsBacklog;
        //public int ReceiveBufferSize = Constants.ReceiveBufferSize;

        //public event ConnectionContextHandler ClientConnected;
        //public event ConnectionContextHandler ClientDisconnected;
        //public event ReceivedHandler<byte[]> ReceivedMessage;

        //private IPEndPoint EndPoint { get; }
        //private Socket Socket { get; }
        //private IDictionary<IPEndPoint, Socket> Clients { get; }

        //#endregion

        //#region ctor

        ///// <summary>
        /////     Create a new instance of the <see cref="ProtoServer{T}" />
        /////     with the default <see cref="AddressFamily" /> and <see cref="SocketType" />.
        /////     Use <see cref="Start" /> to bind and start the socket.
        ///// </summary>
        ///// <param name="address">The <see cref="IPAddress" /> to start this Protocol on</param>
        ///// <param name="port">The Port to start this Protocol on</param>
        //public BinaryDownlink(IPAddress address, int port) :
        //    this(address, port, AddressFamily.InterNetwork, SocketType.Stream) { }

        ///// <summary>
        /////     Create a new instance of the <see cref="ProtoServer{T}" />.
        /////     Use <see cref="Start" /> to bind and start the socket.
        ///// </summary>
        ///// <param name="family">
        /////     The <see cref="AddressFamily" />
        /////     this <see cref="System.Net.Sockets.Socket" /> should use
        ///// </param>
        ///// <param name="type">
        /////     The <see cref="SocketType" /> this
        /////     <see cref="System.Net.Sockets.Socket" /> should use
        ///// </param>
        ///// <param name="address">The <see cref="IPAddress" /> to start this Protocol on</param>
        ///// <param name="port">The Port to start this Protocol on</param>
        //public BinaryDownlink(IPAddress address, int port, AddressFamily family, SocketType type) {
        //    Clients = new Dictionary<IPEndPoint, Socket>();
        //    EndPoint = new IPEndPoint(address, port);
        //    Socket = new Socket(family, type, ProtocolType.Tcp);
        //}

        //#endregion

        //#region Functions

        ///// <summary>
        /////     Bind and Start the Server to the set IP Address.
        ///// </summary>
        //public void Start(bool seperateThread = false) {
        //    Socket.Bind(EndPoint);

        //    if (seperateThread) new Thread(StartListening).Start();
        //    else StartListening();
        //}

        ///// <summary>
        /////     Shutdown the server and all active clients
        ///// </summary>
        //public void Stop() {
        //    foreach (KeyValuePair<IPEndPoint, Socket> kvp in Clients)
        //        try {
        //            DisconnectClient(kvp.Value);
        //        } catch {
        //            // could not disconnect client
        //        }
        //}

        //public async Task Send(byte[] message, IPEndPoint to) {
        //    if (message == null) throw new ArgumentNullException(nameof(message));

        //    // Build a byte array of the serialized data
        //    ArraySegment<byte> segment = new ArraySegment<byte>(message);

        //    // Find socket
        //    var socket = Clients.FirstOrDefault(c => c.Key.Equals(to)).Value;
        //    if (socket == null) throw new Exception($"The IP Address {to} could not be found!");

        //    int size = message.Length;
        //    await SendLeading(size, socket); // send leading size

        //    // Write buffered
        //    int written = 0;
        //    while (written < size) {
        //        int send = size - written; // current buffer size
        //        if (send > ReceiveBufferSize)
        //            send = ReceiveBufferSize; // max size

        //        ArraySegment<byte> slice = segment.SliceEx(written, send); // buffered portion of array
        //        written = await socket.SendAsync(slice, SocketFlags.None);
        //    }

        //    if (written < 1)
        //        throw new TransferException($"{written} bytes were sent! " +
        //                                    "Null bytes could mean a connection shutdown.");
        //}

        //public async Task Broadcast(byte[] message) {
        //    // Build list of Send(..) tasks
        //    List<Task> tasks = Clients.Select(client => Send(message, client.Key)).ToList();
        //    // await all
        //    await Task.WhenAll(tasks);
        //}


        //public bool Kick(IPEndPoint endPoint) {
        //    return DisconnectClient(endPoint);
        //}

        //public void Dispose() {
        //    Stop();
        //    Socket?.Dispose();
        //}

        //#endregion

        //#region Privates

        //// Endless Start listening loop
        //private async void StartListening() {
        //    Socket.Listen(10);
        //    // Loop theoretically infinetly
        //    while (true)
        //        try {
        //            var client = await Socket.AcceptAsync(); // Block until accept
        //            var endpoint = client.RemoteEndPoint as IPEndPoint; // Get remote endpoint
        //            Clients.Add(endpoint, client); // Add client to dictionary

        //            StartReading(client); // Start listening for data
        //            KeepAlive(client); // Keep client alive and ping

        //            ClientConnected?.Invoke(endpoint); // call event
        //        } catch (SocketException ex) {
        //            Console.WriteLine(ex.ErrorCode);
        //            //return;
        //        }
        //    // Listen again after client connected
        //}

        //// Endless Start reading loop
        //private async void StartReading(Socket client) {
        //    var endpoint = client.RemoteEndPoint as IPEndPoint; // Get remote endpoint

        //    // Loop theoretically infinetly
        //    while (true)
        //        try {
        //            long size = await ReadLeading(client); // leading "byte"

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
        //                read += await client.ReceiveAsync(slice, SocketFlags.None);
        //            }

        //            if (read < 1)
        //                throw new TransferException($"{read} bytes were read! " +
        //                                            "Null bytes could mean a connection shutdown.");

        //            ReceivedMessage?.Invoke(endpoint, segment.Array); // call event
        //        } catch (SocketException ex) {
        //            Console.WriteLine(ex.ErrorCode);
        //            bool success = DisconnectClient(client); // try to disconnect
        //            if (success) // Exit Reading loop once successfully disconnected
        //                return;
        //        } catch (TransferException) {
        //            // 0 read bytes = null byte
        //            bool success = DisconnectClient(client); // try to disconnect
        //            if (success) // Exit Reading loop once successfully disconnected
        //                return;
        //        }
        //    // Listen again after client connected
        //}

        //// Keep a Client alive by pinging
        //private async void KeepAlive(Socket client) {
        //    while (true) {
        //        await Task.Delay(Constants.PingDelay);

        //        bool isAlive = client.Ping();
        //        if (isAlive) continue; // Client responded

        //        // Client does not respond, disconnect & exit
        //        DisconnectClient(client);
        //        return;
        //    }
        //}

        //// Disconnect a client; returns true if successful
        //private bool DisconnectClient(Socket client) {
        //    KeyValuePair<IPEndPoint, Socket>[] filtered = Clients.Where(c => c.Value == client).ToArray();
        //    foreach (KeyValuePair<IPEndPoint, Socket> kvp in filtered)
        //        try {
        //            kvp.Value.Disconnect(false); // Gracefully disconnect socket
        //            kvp.Value.Close();
        //            kvp.Value.Dispose();

        //            Clients.Remove(kvp.Key); // Remove from collection
        //            ClientDisconnected?.Invoke(kvp.Key); // Event
        //        } catch {
        //            // Socket is either already disconnected, or failing to disconnect. try ping
        //            return !kvp.Value.Ping();
        //        }
        //    return true;
        //}

        //// Disconnect a client; returns true if successful
        //private bool DisconnectClient(IPEndPoint endPoint) {
        //    KeyValuePair<IPEndPoint, Socket>[] filtered = Clients.Where(c => c.Key.Equals(endPoint)).ToArray();
        //    foreach (KeyValuePair<IPEndPoint, Socket> kvp in filtered)
        //        try {
        //            kvp.Value.Disconnect(false); // Gracefully disconnect socket
        //            kvp.Value.Close();
        //            kvp.Value.Dispose();

        //            Clients.Remove(kvp.Key); // Remove from collection
        //            ClientDisconnected?.Invoke(kvp.Key); // Event
        //        } catch {
        //            // Socket is either already disconnected, or failing to disconnect. try ping
        //            return !kvp.Value.Ping();
        //        }
        //    return true;
        //}

        //// Read the prefix from a message (number of following bytes)
        //private async Task<long> ReadLeading(Socket client) {
        //    byte[] bytes = new byte[sizeof(long)];
        //    ArraySegment<byte> segment = new ArraySegment<byte>(bytes);
        //    // read leading bytes
        //    int read = await client.ReceiveAsync(segment, SocketFlags.None);

        //    if (read < 1)
        //        throw new TransferException($"{read} lead-bytes were read! " +
        //                                    "Null bytes could mean a connection shutdown.");

        //    // size of the following byte[]
        //    long size = ZeroFormatterSerializer.Deserialize<long>(segment.Array);
        //    return size;
        //}

        //// Send the prefix from a message (number of following bytes)
        //private async Task SendLeading(long size, Socket client) {
        //    // build byte[] out of size
        //    byte[] bytes = ZeroFormatterSerializer.Serialize(size);
        //    ArraySegment<byte> segment = new ArraySegment<byte>(bytes);
        //    // send leading bytes
        //    int sent = await client.SendAsync(segment, SocketFlags.None);

        //    if (sent < 1)
        //        throw new TransferException($"{sent} lead-bytes were sent! " +
        //                                    "Null bytes could mean a connection shutdown.");
        //}

        //#endregion
    }
}
