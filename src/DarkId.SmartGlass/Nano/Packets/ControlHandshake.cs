using System;
using System.IO;
using DarkId.SmartGlass.Common;
using DarkId.SmartGlass.Nano;

namespace DarkId.SmartGlass.Nano.Packets
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

        public void Deserialize(LEReader br)
        {
            Type = (ControlHandshakeType)br.ReadByte();
            ConnectionId = br.ReadUInt16();
        }

        public void Serialize(LEWriter bw)
        {
            bw.Write((byte)Type);
            bw.Write(ConnectionId);
        }
    }
}