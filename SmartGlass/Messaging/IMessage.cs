using System;
using System.Net;
using SmartGlass.Common;

namespace SmartGlass.Messaging
{
    interface IMessage<THeader> : IMessage where THeader : IMessageHeader
    {
        new THeader Header { get; set; }
    }

    interface IMessage : ISerializable
    {
        DateTime ClientReceivedTimestamp { get; set; }
        IPEndPoint Origin { get; set; }
        IMessageHeader Header { get; }
    }
}