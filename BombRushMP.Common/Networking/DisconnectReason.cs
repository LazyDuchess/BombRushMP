using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common.Networking
{
    public enum DisconnectReason : byte
    {
        //
        // Summary:
        //     No connection was ever established.
        NeverConnected,
        //
        // Summary:
        //     The connection attempt was rejected by the server.
        ConnectionRejected,
        //
        // Summary:
        //     The active transport detected a problem with the connection.
        TransportError,
        //
        // Summary:
        //     The connection timed out.
        //
        // Remarks:
        //     This also acts as the fallback reason—if a client disconnects and the message
        //     containing the real reason is lost in transmission, it can't be resent as the
        //     connection will have already been closed. As a result, the other end will time
        //     out the connection after a short period of time and this will be used as the
        //     reason.
        TimedOut,
        //
        // Summary:
        //     The client was forcibly disconnected by the server.
        Kicked,
        //
        // Summary:
        //     The server shut down.
        ServerStopped,
        //
        // Summary:
        //     The disconnection was initiated by the client.
        Disconnected,
        //
        // Summary:
        //     The connection's loss and/or resend rates exceeded the maximum acceptable thresholds,
        //     or a reliably sent message could not be delivered.
        PoorConnection
    }
}
