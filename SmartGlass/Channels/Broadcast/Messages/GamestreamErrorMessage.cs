using System;
using SmartGlass.Common;
using SmartGlass.Messaging.Session;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SmartGlass.Channels.Broadcast.Messages
{
    /// <summary>
    /// Broadcast error message.
    /// Sent from console to client when an error occurs.
    /// </summary>
    [BroadcastMessageType(BroadcastMessageType.GamestreamError)]
    class BroadcastErrorMessage : BroadcastBaseMessage, IConvertToException
    {
        /// <summary>
        /// Type of error
        /// </summary>
        /// <value>The type of the error.</value>
        public int ErrorType { get; set; }
        /// <summary>
        /// Error value
        /// </summary>
        /// <value>The error value.</value>
        public GamestreamError ErrorValue { get; set; }

        public Exception ToException()
        {
            return new GamestreamException("Gamestream error ocurred.", ErrorValue);
        }
    }
}
