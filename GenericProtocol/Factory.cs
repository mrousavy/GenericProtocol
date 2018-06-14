//TODO: (Implement BinaryDownlink and remove the #ifs and #defines)
//#define implemented

using System.Net;
using System.Threading.Tasks;
using GenericProtocol.Implementation;

namespace GenericProtocol
{
    /// <summary>
    ///     The <see cref="IClient{T}" /> and <see cref="IServer{T}" />
    ///     start factory
    /// </summary>
    public static class Factory
    {
        #region Client

        /// <summary>
        ///     Start and Connect a new <see cref="IClient{T}" />
        /// </summary>
        /// <typeparam name="T">The Type of object to send over the network</typeparam>
        /// <param name="address">The <see cref="IServer{T}" />'s <see cref="IPAddress" /></param>
        /// <param name="port">The <see cref="IServer{T}" />'s Port</param>
        /// <returns>A connected <see cref="IClient{T}" /> instance</returns>
        public static async Task<IClient<T>> StartNewClient<T>(string address, int port)
        {
            var client = new ProtoClient<T>(IPAddress.Parse(address), port);
            await client.Connect();
            return client;
        }

        /// <summary>
        ///     Start and Connect a new <see cref="IClient{T}" />
        /// </summary>
        /// <typeparam name="T">The Type of object to send over the network</typeparam>
        /// <param name="address">The <see cref="IServer{T}" />'s <see cref="IPAddress" /></param>
        /// <param name="port">The <see cref="IServer{T}" />'s Port</param>
        /// <param name="newThread">
        ///     Whether to use a seperate
        ///     <see cref="System.Threading.Thread" /> for all data transfers
        /// </param>
        /// <returns>A connected <see cref="IClient{T}" /> instance</returns>
        public static async Task<IClient<T>> StartNewClient<T>(string address, int port, bool newThread)
        {
            var client = new ProtoClient<T>(IPAddress.Parse(address), port);
            await client.Connect(newThread);
            return client;
        }

#if implemented
/// <summary>
///     Start and Connect a new <see cref="IClient{T}"/>
///     that sends and receives pure binary data (<see cref="byte"/> array)
/// </summary>
/// <typeparam name="T">The Type of object to send over the network</typeparam>
/// <param name="address">The <see cref="IServer{T}"/>'s <see cref="IPAddress"/></param>
/// <param name="port">The <see cref="IServer{T}"/>'s Port</param>
/// <returns>A connected <see cref="IClient{T}"/> instance</returns>
        public static async Task<IClient<byte[]>> StartNewBinaryDownlink(string address, int port) {
            var client = new BinaryDownlink(IPAddress.Parse(address), port);
            await client.Connect();
            return client;
        }

        /// <summary>
        ///     Start and Connect a new <see cref="IClient{T}"/>
        ///     that sends and receives pure binary data (<see cref="byte"/> array)
        /// </summary>
        /// <param name="address">The <see cref="IServer{T}"/>'s <see cref="IPAddress"/></param>
        /// <param name="port">The <see cref="IServer{T}"/>'s Port</param>
        /// <param name="newThread">Whether to use a seperate 
        /// <see cref="System.Threading.Thread"/> for all data transfers</param>
        /// <returns>A connected <see cref="IClient{T}"/> instance</returns>
        public static async Task<IClient<byte[]>> StartNewBinaryDownlink(string address, int port, bool newThread) {
            var client = new BinaryDownlink(IPAddress.Parse(address), port);
            await client.Connect(newThread);
            return client;
        }
#endif

        #endregion

        #region Server

        //TODO: Server

        #endregion
    }
}