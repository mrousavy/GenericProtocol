using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace GenericProtocol.Implementation {
    internal static class LeadingByteProcessor {
        /// <summary>
        /// Read the prefix from a message (number of following bytes)
        /// </summary>
        /// <param name="socket">The socket to read the leading bytes from</param>
        /// <returns>Returns an <c>await</c>able <see cref="Task"/></returns>
        internal static async Task<int> ReadLeading(Socket socket) {
            byte[] bytes = new byte[Constants.LeadingByteSize];
            ArraySegment<byte> segment = new ArraySegment<byte>(bytes);
            // read leading bytes
            int read = await socket.ReceiveAsync(segment, SocketFlags.None);

            if (read < 1)
                throw new TransferException($"{read} lead-bytes were read! " +
                                            "Null bytes could mean a connection shutdown.");

            // size of the following byte[]
            int size = BitConverter.ToInt32(segment.Array, 0);
            return size;
        }

        /// <summary>
        /// Send the prefix from a message (number of following bytes)
        /// </summary>
        /// <param name="socket">The socket to read the leading bytes from</param>
        /// <param name="size">The size of the following message (= leading byte's value)</param>
        /// <returns>Returns an <c>await</c>able <see cref="Task"/></returns>
        internal static async Task SendLeading(Socket socket, int size) {
            // build byte[] out of size
            byte[] bytes = BitConverter.GetBytes(size);
            ArraySegment<byte> segment = new ArraySegment<byte>(bytes);
            // send leading bytes
            int sent = await socket.SendAsync(segment, SocketFlags.None);

            if (sent < 1)
                throw new TransferException($"{sent} lead-bytes were sent! " +
                                            "Null bytes could mean a connection shutdown.");
        }
    }
}
