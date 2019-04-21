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
            Buttons = new InputButtons();
            Analog = new InputAnalogue();
            Extension = new InputExtension();
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

        internal override void DeserializeStreamer(EndianReader reader)
        {
            FrameId = reader.ReadUInt32LE();
            Timestamp = reader.ReadUInt64LE();
            CreatedTimestamp = reader.ReadUInt64LE();

            ((ISerializable)Buttons).Deserialize(reader);
            ((ISerializable)Analog).Deserialize(reader);
            ((ISerializable)Extension).Deserialize(reader);
        }

        internal override void SerializeStreamer(EndianWriter writer)
        {
            writer.WriteLE(FrameId);
            writer.WriteLE(Timestamp);
            writer.WriteLE(CreatedTimestamp);

            ((ISerializable)Buttons).Serialize(writer);
            ((ISerializable)Analog).Serialize(writer);
            ((ISerializable)Extension).Serialize(writer);
        }
    }
}