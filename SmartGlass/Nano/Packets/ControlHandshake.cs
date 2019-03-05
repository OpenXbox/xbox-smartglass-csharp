using System;
using System.IO;
using SmartGlass.Common;
using SmartGlass.Nano;

namespace SmartGlass.Nano.Packets
{
    [NanoPayloadType(NanoPayloadType.ControlHandshake)]
    public class ControlHandshake : INanoPacket
    {
        public NanoChannel Channel { get; set; }
        public RtpHeader Header { get; set; }
        public ControlHandshakeType Type { get; internal set; }
        public ushort ConnectionId { get; internal set; }

        public ControlHandshake()
        {
            Header = new RtpHeader()
            {
                PayloadType = NanoPayloadType.ControlHandshake
            };
        }

        public ControlHandshake(ControlHandshakeType type, ushort connectionId)
        {
            Header = new RtpHeader()
            {
                PayloadType = NanoPayloadType.ControlHandshake
            };
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