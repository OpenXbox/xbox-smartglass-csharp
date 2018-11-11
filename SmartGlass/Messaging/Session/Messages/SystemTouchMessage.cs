using System;
using SmartGlass.Common;

namespace SmartGlass.Messaging.Session.Messages
{
    [SessionMessageType(SessionMessageType.SystemTouch)]
    internal class SystemTouchMessage : TouchMessage
    {
    }
}
