using System;
using SmartGlass.Common;

namespace SmartGlass.Messaging.Session.Messages
{
    [SessionMessageType(SessionMessageType.TitleTouch)]
    internal record TitleTouchMessage : TouchMessage
    {
    }
}
