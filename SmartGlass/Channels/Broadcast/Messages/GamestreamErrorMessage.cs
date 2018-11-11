using System;
using DarkId.SmartGlass.Common;
using DarkId.SmartGlass.Messaging.Session;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DarkId.SmartGlass.Channels.Broadcast.Messages
{
    [BroadcastMessageType(BroadcastMessageType.GamestreamError)]
    class BroadcastErrorMessage : BroadcastBaseMessage, IConvertToException
    {
        public int ErrorType { get; set; }
        public GamestreamError ErrorValue { get; set; }

        public Exception ToException()
        {
            return new GamestreamException("Gamestream error ocurred.", ErrorValue);
        }
    }
}