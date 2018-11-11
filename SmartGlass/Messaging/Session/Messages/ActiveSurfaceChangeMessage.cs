using System;
using SmartGlass.Common;

namespace SmartGlass.Messaging.Session.Messages
{
    [SessionMessageType(SessionMessageType.ActiveSurfaceChange)]
    internal class ActiveSurfaceChangeMessage : SessionMessageBase
    {
        public ActiveSurfaceType SurfaceType { get; set; }

        public StreamerConfiguration StreamerConfiguration { get; set; }

        public override void Deserialize(BEReader reader)
        {
            SurfaceType = (ActiveSurfaceType)reader.ReadUInt16();

            StreamerConfiguration = new StreamerConfiguration();
            StreamerConfiguration.Deserialize(reader);
        }

        public override void Serialize(BEWriter writer)
        {
            throw new NotSupportedException();
        }
    }
}