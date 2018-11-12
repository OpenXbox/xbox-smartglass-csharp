using System;
using System.IO;
using SmartGlass.Common;
using SmartGlass.Nano;

namespace SmartGlass.Nano.Packets
{
    [InputPayloadType(InputPayloadType.Frame)]
    internal class InputFrame : ISerializableLE
    {
        public uint FrameId { get; private set; }
        public ulong Timestamp { get; private set; }
        public ulong CreatedTimestamp { get; private set; }
        public InputButtons Buttons { get; private set; }
        public InputAnalogue Analog { get; private set; }
        public InputExtension Extension  { get; private set; }

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

        public void Deserialize(LEReader br)
        {
            FrameId = br.ReadUInt32();
            Timestamp = br.ReadUInt64();
            CreatedTimestamp = br.ReadUInt64();

            Buttons.Deserialize(br);
            Analog.Deserialize(br);
            Extension.Deserialize(br);
        }

        public void Serialize(LEWriter bw)
        {
            bw.Write(FrameId);
            bw.Write(Timestamp);
            bw.Write(CreatedTimestamp);

            Buttons.Serialize(bw);
            Analog.Serialize(bw);
            Extension.Serialize(bw);
        }
    }
}