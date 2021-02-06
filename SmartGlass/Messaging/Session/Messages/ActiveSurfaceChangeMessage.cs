using System;
using SmartGlass.Common;

namespace SmartGlass.Messaging.Session.Messages
{
    [SessionMessageType(SessionMessageType.ActiveSurfaceChange)]
    internal record ActiveSurfaceChangeMessage : SessionMessageBase
    {
        public ActiveSurfaceType SurfaceType { get; set; }

        public StreamerConfiguration StreamerConfiguration { get; set; }

        public override void Deserialize(EndianReader reader)
        {
            SurfaceType = (ActiveSurfaceType)reader.ReadUInt16BE();

            StreamerConfiguration = new StreamerConfiguration();
            StreamerConfiguration.Deserialize(reader);
        }

        public override void Serialize(EndianWriter writer)
        {
            throw new NotSupportedException();
        }
    }
}