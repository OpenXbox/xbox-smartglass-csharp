using System;
using System.Net;
using DarkId.SmartGlass.Common;

namespace DarkId.SmartGlass.Messaging
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