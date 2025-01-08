using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common.Networking
{
    public enum RejectReason : byte
    {
        //
        // Summary:
        //     No response was received from the server (because the client has no internet
        //     connection, the server is offline, no server is listening on the target endpoint,
        //     etc.).
        NoConnection,
        //
        // Summary:
        //     The client is already connected.
        AlreadyConnected,
        //
        // Summary:
        //     The server is full.
        ServerFull,
        //
        // Summary:
        //     The connection attempt was rejected.
        Rejected,
        //
        // Summary:
        //     The connection attempt was rejected and custom data may have been included with
        //     the rejection message.
        Custom
    }
}
