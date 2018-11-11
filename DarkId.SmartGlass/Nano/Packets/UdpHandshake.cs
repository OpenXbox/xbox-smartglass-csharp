using System;
using System.IO;
using DarkId.SmartGlass.Common;
using DarkId.SmartGlass.Nano;

namespace DarkId.SmartGlass.Nano.Packets
{
    [RtpPayloadType(RtpPayloadType.UDPHandshake)]
    internal class UdpHandshake : ISerializableLE
    {
        public ControlHandshakeType Type { get; private set; }

        public UdpHandshake()
        {
        }
        
        public UdpHandshake(ControlHandshakeType type)
        {
            Type = type;
        }

        public void Deserialize(LEReader br)
        {
            Type = (ControlHandshakeType)br.ReadByte();
        }

        public void Serialize(LEWriter bw)
        {
            bw.Write((byte)Type);
        }
    }
}