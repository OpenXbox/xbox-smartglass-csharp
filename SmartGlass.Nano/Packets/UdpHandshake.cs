using System;
using System.IO;
using SmartGlass.Common;
using SmartGlass.Nano;

namespace SmartGlass.Nano.Packets
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

        public void Deserialize(BinaryReader br)
        {
            Type = (ControlHandshakeType)br.ReadByte();
        }

        public void Serialize(BinaryWriter bw)
        {
            bw.Write((byte)Type);
        }
    }
}