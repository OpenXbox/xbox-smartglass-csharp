using System;
using System.IO;
using DarkId.SmartGlass.Common;
using DarkId.SmartGlass.Nano;

namespace DarkId.SmartGlass.Nano.Packets
{
    [InputPayloadType(InputPayloadType.ServerHandshake)]
    internal class InputServerHandshake : ISerializableLE
    {
        public uint ProtocolVersion { get; private set; }
        public uint DesktopWidth { get; private set; }
        public uint DesktopHeight { get; private set; }
        public uint MaxTouches { get; private set; }
        public uint InitialFrameId { get; private set; }

        public InputServerHandshake()
        {
        }
        
        public InputServerHandshake(uint protocolVersion, uint desktopWidth, uint desktopHeight,
                                    uint maxTouches, uint initialFrameId)
        {
            ProtocolVersion = protocolVersion;
            DesktopWidth = desktopWidth;
            DesktopHeight = desktopHeight;
            MaxTouches = maxTouches;
            InitialFrameId = initialFrameId;
        }

        public void Deserialize(LEReader br)
        {
            ProtocolVersion = br.ReadUInt32();
            DesktopWidth = br.ReadUInt32();
            DesktopHeight = br.ReadUInt32();
            MaxTouches = br.ReadUInt32();
            InitialFrameId = br.ReadUInt32();
        }

        public void Serialize(LEWriter bw)
        {
            bw.Write(ProtocolVersion);
            bw.Write(DesktopWidth);
            bw.Write(DesktopHeight);
            bw.Write(MaxTouches);
            bw.Write(InitialFrameId);
        }
    }
}