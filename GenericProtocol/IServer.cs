using System;
using System.Collections.Generic;
using System.Text;

namespace GenericProtocol {
    public interface IServer {

        event EventHandler ClientConnected;

        /// <summary>
        /// Bind the Socket to the set IP Address 
        /// and start listening for incoming connections
        /// </summary>
        void Start();
    }
}
