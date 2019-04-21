using System;
using System.IO;
using SmartGlass.Common;
using SmartGlass.Nano;

namespace SmartGlass.Nano.Packets
{
    [ControlOpCode(ControlOpCode.ControllerEvent)]
    public class ControllerEvent : StreamerMessageWithHeader
    {
        public ControllerEventType Type { get; private set; }
        public byte ControllerIndex { get; private set; }

        public ControllerEvent()
            : base(ControlOpCode.ControllerEvent)
        {
        }

        public ControllerEvent(ControllerEventType controllerEvent,
                               byte controllerIndex)
            : base(ControlOpCode.ControllerEvent)
        {
            Type = controllerEvent;
            ControllerIndex = controllerIndex;
        }

        internal override void DeserializeStreamer(EndianReader reader)
        {
            Type = (ControllerEventType)reader.ReadByte();
            ControllerIndex = reader.ReadByte();
        }

        internal override void SerializeStreamer(EndianWriter writer)
        {
            writer.Write((byte)Type);
            writer.Write(ControllerIndex);
        }
    }
}