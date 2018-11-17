using System;
using System.IO;
using SmartGlass.Common;
using SmartGlass.Nano;

namespace SmartGlass.Nano.Packets
{
    [InputPayloadType(InputPayloadType.Frame)]
    public class InputFrame : StreamerMessage
    {
        public uint FrameId { get; private set; }
        public ulong Timestamp { get; private set; }
        public ulong CreatedTimestamp { get; private set; }
        public InputButtons Buttons { get; private set; }
        public InputAnalogue Analog { get; private set; }
        public InputExtension Extension { get; private set; }

        public InputFrame()
            : base((uint)InputPayloadType.Frame)
        {
        }

        public InputFrame(uint frameId, ulong timestamp, ulong createdTimestamp,
                          InputButtons buttons, InputAnalogue analog, InputExtension extension)
            : base((uint)InputPayloadType.Frame)
        {
            FrameId = frameId;
            Timestamp = timestamp;
            CreatedTimestamp = createdTimestamp;
            Buttons = buttons;
            Analog = analog;
            Extension = extension;
        }

        public override void DeserializeStreamer(BinaryReader reader)
        {
            FrameId = reader.ReadUInt32();
            Timestamp = reader.ReadUInt64();
            CreatedTimestamp = reader.ReadUInt64();

            ((ISerializableLE)Buttons).Deserialize(reader);
            ((ISerializableLE)Analog).Deserialize(reader);
            ((ISerializableLE)Extension).Deserialize(reader);
        }

        public override void SerializeStreamer(BinaryWriter writer)
        {
            writer.Write(FrameId);
            writer.Write(Timestamp);
            writer.Write(CreatedTimestamp);

            ((ISerializableLE)Buttons).Serialize(writer);
            ((ISerializableLE)Analog).Serialize(writer);
            ((ISerializableLE)Extension).Serialize(writer);
        }
    }
}