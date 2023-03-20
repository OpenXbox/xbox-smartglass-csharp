using System;
using SmartGlass.Common;

namespace SmartGlass.Messaging.Session.Messages
{
    [SessionMessageType(SessionMessageType.TitleTextConfiguration)]
    internal record TitleTextConfigurationMessage : TextConfiguration
    {
    }
}
