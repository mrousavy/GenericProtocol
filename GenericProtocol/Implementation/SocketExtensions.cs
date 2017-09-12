using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace GenericProtocol.Implementation {
    public static class SocketExtensions {
        /// <summary>
        /// Ping this <see cref="Socket"/> to check for an active connection
        /// </summary>
        /// <param name="socket">The socket to ping</param>
        /// <returns>True if the <see cref="Socket"/> responds</returns>
        public static bool Ping(this Socket socket) {
            try {
                return !(socket.Poll(1, SelectMode.SelectRead) && socket.Available == 0);
            } catch (SocketException) { return false; }
        }
    }
}
