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

        internal override void DeserializeStreamer(EndianReader reader)
        {
            ProtocolVersion = reader.ReadUInt32LE();
            DesktopWidth = reader.ReadUInt32LE();
            DesktopHeight = reader.ReadUInt32LE();
            MaxTouches = reader.ReadUInt32LE();
            InitialFrameId = reader.ReadUInt32LE();
        }

        internal override void SerializeStreamer(EndianWriter writer)
        {
            writer.WriteLE(ProtocolVersion);
            writer.WriteLE(DesktopWidth);
            writer.WriteLE(DesktopHeight);
            writer.WriteLE(MaxTouches);
            writer.WriteLE(InitialFrameId);
        }
    }
}