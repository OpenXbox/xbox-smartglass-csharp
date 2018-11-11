using System;
using DarkId.SmartGlass.Common;

namespace DarkId.SmartGlass.Messaging.Session.Messages
{
    class StreamerConfiguration : ISerializable
    {
        public ushort ServerTcpPort { get; set; }
        public ushort ServerUdpPort { get; set; }
        public Guid SessionId { get; set; }
        public ushort RenderWidth { get; set; }
        public ushort RenderHeight { get; set; }
        public byte[] MasterSessionKey { get; set; }

        public void Deserialize(BEReader reader)
        {
            ServerTcpPort = reader.ReadUInt16();
            ServerUdpPort = reader.ReadUInt16();
            SessionId = new Guid(reader.ReadBytes(16));
            RenderWidth = reader.ReadUInt16();
            RenderHeight = reader.ReadUInt16();
            MasterSessionKey = reader.ReadBytes(32);
        }

        public void Serialize(BEWriter writer)
        {
            throw new NotSupportedException();
        }
    }
}