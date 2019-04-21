using System;
using System.IO;
using SmartGlass.Common;
using SmartGlass.Nano;

namespace SmartGlass.Nano.Packets
{
    [NanoPayloadType(NanoPayloadType.UDPHandshake)]
    public class UdpHandshake : INanoPacket
    {
        public NanoChannel Channel { get; set; }
        public RtpHeader Header { get; set; }
        public ControlHandshakeType Type { get; private set; }

        public UdpHandshake()
        {
            Header = new RtpHeader()
            {
                PayloadType = NanoPayloadType.UDPHandshake
            };
        }

        public UdpHandshake(ControlHandshakeType type)
        {
            Header = new RtpHeader()
            {
                PayloadType = NanoPayloadType.UDPHandshake
            };
            Type = type;
        }

        public void Deserialize(EndianReader br)
        {
            Type = (ControlHandshakeType)br.ReadByte();
        }

        public void Serialize(EndianWriter bw)
        {
            bw.Write((byte)Type);
        }
    }
}