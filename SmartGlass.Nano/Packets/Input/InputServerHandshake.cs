using System;
using System.IO;
using SmartGlass.Common;
using SmartGlass.Nano;

namespace SmartGlass.Nano.Packets
{
    [InputPayloadType(InputPayloadType.ServerHandshake)]
    public class InputServerHandshake : ISerializableLE
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

        void ISerializableLE.Deserialize(BinaryReader br)
        {
            ProtocolVersion = br.ReadUInt32();
            DesktopWidth = br.ReadUInt32();
            DesktopHeight = br.ReadUInt32();
            MaxTouches = br.ReadUInt32();
            InitialFrameId = br.ReadUInt32();
        }

        void ISerializableLE.Serialize(BinaryWriter bw)
        {
            bw.Write(ProtocolVersion);
            bw.Write(DesktopWidth);
            bw.Write(DesktopHeight);
            bw.Write(MaxTouches);
            bw.Write(InitialFrameId);
        }
    }
}