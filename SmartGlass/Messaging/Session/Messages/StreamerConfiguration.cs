using System;
using SmartGlass.Common;

namespace SmartGlass.Messaging.Session.Messages
{
    record StreamerConfiguration : ISerializable
    {
        public ushort ServerTcpPort { get; set; }
        public ushort ServerUdpPort { get; set; }
        public Guid SessionId { get; set; }
        public ushort RenderWidth { get; set; }
        public ushort RenderHeight { get; set; }
        public byte[] MasterSessionKey { get; set; }

        public void Deserialize(EndianReader reader)
        {
            ServerTcpPort = reader.ReadUInt16BE();
            ServerUdpPort = reader.ReadUInt16BE();
            SessionId = new Guid(reader.ReadBytes(16));
            RenderWidth = reader.ReadUInt16BE();
            RenderHeight = reader.ReadUInt16BE();
            MasterSessionKey = reader.ReadBytes(32);
        }

        public void Serialize(EndianWriter writer)
        {
            throw new NotSupportedException();
        }
    }
}