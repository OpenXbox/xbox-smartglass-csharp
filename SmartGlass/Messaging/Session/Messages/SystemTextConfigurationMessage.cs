using System;
using SmartGlass.Common;

namespace SmartGlass.Messaging.Session.Messages
{
    [SessionMessageType(SessionMessageType.SystemTextConfiguration)]
    internal record SystemTextConfigurationMessage : TextConfiguration
    {
    }
}
