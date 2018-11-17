using System;
using System.IO;
using SmartGlass.Common;
using SmartGlass.Nano;

namespace SmartGlass.Nano.Packets
{
    [InputPayloadType(InputPayloadType.ServerHandshake)]
    public class InputServerHandshake : StreamerMessage
    {
        public uint ProtocolVersion { get; private set; }
        public uint DesktopWidth { get; private set; }
        public uint DesktopHeight { get; private set; }
        public uint MaxTouches { get; private set; }
        public uint InitialFrameId { get; private set; }

        public InputServerHandshake()
            : base((uint)InputPayloadType.ServerHandshake)
        {
        }

        public InputServerHandshake(uint protocolVersion, uint desktopWidth, uint desktopHeight,
                                    uint maxTouches, uint initialFrameId)
            : base((uint)InputPayloadType.ServerHandshake)
        {
            ProtocolVersion = protocolVersion;
            DesktopWidth = desktopWidth;
            DesktopHeight = desktopHeight;
            MaxTouches = maxTouches;
            InitialFrameId = initialFrameId;
        }

        public override void DeserializeStreamer(BinaryReader reader)
        {
            ProtocolVersion = reader.ReadUInt32();
            DesktopWidth = reader.ReadUInt32();
            DesktopHeight = reader.ReadUInt32();
            MaxTouches = reader.ReadUInt32();
            InitialFrameId = reader.ReadUInt32();
        }

        public override void SerializeStreamer(BinaryWriter writer)
        {
            writer.Write(ProtocolVersion);
            writer.Write(DesktopWidth);
            writer.Write(DesktopHeight);
            writer.Write(MaxTouches);
            writer.Write(InitialFrameId);
        }
    }
}