using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using GenericProtocol.Implementation;

namespace GenericProtocol {
    /// <summary>
    ///     A protocol to discover other GenericProtocol
    ///     <see cref="IClient{T}"/>s in your network
    ///     via a UDP Broadcast
    /// </summary>
    public interface INetworkDiscovery {
        /// <summary>
        ///     Discover all <see cref="IClient{T}"/>s in the
        ///     given network
        /// </summary>
        /// <param name="networkIp">
        ///     The network's IP to search for <see cref="IClient{T}"/>s in
        /// </param>
        /// <param name="port">
        ///     The port to use for the discovery service
        ///     (same as used in <see cref="Host"/>)
        /// </param>
        /// <returns>
        ///     A <see cref="IDiscoveryResult"/> containing all
        ///     discovery results
        /// </returns>
        Task<IDiscoveryResult> Discover(IPAddress networkIp, int port = Constants.DiscoveryPort);

        /// <summary>
        ///     Start a new Listener on the given network which
        ///     responds to <see cref="Discover"/> calls
        /// </summary>
        /// <param name="networkIp">
        ///     The network's IP to start listening in
        /// </param>
        /// <param name="port">
        ///     The port to use for the discovery service
        /// </param>
        void Host(IPAddress networkIp, int port = Constants.DiscoveryPort);
    }


    /// <summary>
    ///     The Result of a <see cref="INetworkDiscovery.Discover"/> call
    /// </summary>
    public interface IDiscoveryResult {
        /// <summary>
        ///     True if the <see cref="INetworkDiscovery"/>
        ///     found one or more <see cref="IClient{T}"/>s in the network
        /// </summary>
        bool Any { get; set; }
        /// <summary>
        ///     The count of the hosts that responded in the network
        /// </summary>
        int HostsCount { get; set; }
        /// <summary>
        ///     An <see cref="IEnumerable{T}"/> of <see cref="IPEndPoint"/>s
        ///     representing all Hosts that responded to a
        ///     <see cref="INetworkDiscovery.Discover"/> call
        /// </summary>
        IEnumerable<IPEndPoint> Hosts { get; set; }
    }
}
