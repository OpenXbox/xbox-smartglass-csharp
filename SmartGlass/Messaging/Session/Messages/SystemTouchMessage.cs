using System;
using SmartGlass.Common;

namespace SmartGlass.Messaging.Session.Messages
{
    [SessionMessageType(SessionMessageType.SystemTouch)]
    internal record SystemTouchMessage : TouchMessage
    {
    }
}
