using System;
using System.IO;
using SmartGlass.Common;
using SmartGlass.Nano;

namespace SmartGlass.Nano.Packets
{
    [InputPayloadType(InputPayloadType.Frame)]
    public class InputFrame : ISerializableLE
    {
        public uint FrameId { get; private set; }
        public ulong Timestamp { get; private set; }
        public ulong CreatedTimestamp { get; private set; }
        public InputButtons Buttons { get; private set; }
        public InputAnalogue Analog { get; private set; }
        public InputExtension Extension { get; private set; }

        public InputFrame()
        {
        }

        public InputFrame(uint frameId, ulong timestamp, ulong createdTimestamp,
                          InputButtons buttons, InputAnalogue analog, InputExtension extension)
        {
            FrameId = frameId;
            Timestamp = timestamp;
            CreatedTimestamp = createdTimestamp;
            Buttons = buttons;
            Analog = analog;
            Extension = extension;
        }

        void ISerializableLE.Deserialize(LEReader br)
        {
            FrameId = br.ReadUInt32();
            Timestamp = br.ReadUInt64();
            CreatedTimestamp = br.ReadUInt64();

            ((ISerializableLE)Buttons).Deserialize(br);
            ((ISerializableLE)Analog).Deserialize(br);
            ((ISerializableLE)Extension).Deserialize(br);
        }

        void ISerializableLE.Serialize(LEWriter bw)
        {
            bw.Write(FrameId);
            bw.Write(Timestamp);
            bw.Write(CreatedTimestamp);

            ((ISerializableLE)Buttons).Serialize(bw);
            ((ISerializableLE)Analog).Serialize(bw);
            ((ISerializableLE)Extension).Serialize(bw);
        }
    }
}