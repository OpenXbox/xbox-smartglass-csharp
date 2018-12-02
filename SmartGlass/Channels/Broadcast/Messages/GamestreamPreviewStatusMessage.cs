using System;
using SmartGlass.Messaging.Session;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SmartGlass.Channels.Broadcast.Messages
{
    [BroadcastMessageType(BroadcastMessageType.PreviewStatus)]
    class GamestreamPreviewStatusMessage : BroadcastBaseMessage
    {
        public bool IsPublicPreview { get; set; }
        public bool IsInternalPreview { get; set; }
    }
}