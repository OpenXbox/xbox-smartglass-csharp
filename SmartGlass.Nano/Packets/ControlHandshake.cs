using System;
using System.IO;
using SmartGlass.Common;
using SmartGlass.Nano;

namespace SmartGlass.Nano.Packets
{
    [RtpPayloadType(RtpPayloadType.Control)]
    internal class ControlHandshake : ISerializableLE
    {
        public ControlHandshakeType Type { get; internal set; }
        public ushort ConnectionId { get; internal set; }

        public ControlHandshake()
        {
        }

        public ControlHandshake(ControlHandshakeType type, ushort connectionId)
        {
            Type = type;
            ConnectionId = connectionId;
        }

        public void Deserialize(BinaryReader reader)
        {
            Type = (ControlHandshakeType)reader.ReadByte();
            ConnectionId = reader.ReadUInt16();
        }

        public void Serialize(BinaryWriter bw)
        {
            bw.Write((byte)Type);
            bw.Write(ConnectionId);
        }
    }
}