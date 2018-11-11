using System;
using System.IO;
using SmartGlass.Common;
using SmartGlass.Nano;

namespace SmartGlass.Nano.Packets
{
    [ControlOpCode(ControlOpCode.ControllerEvent)]
    internal class ControllerEvent : ISerializableLE
    {
        public ControllerEventType Type { get; private set; }
        public byte ControllerIndex { get; private set; }

        public ControllerEvent()
        {
        }
        
        public ControllerEvent(ControllerEventType controllerEvent,
                               byte controllerIndex)
        {
            Type = controllerEvent;
            ControllerIndex = controllerIndex;
        }

        public void Deserialize(LEReader br)
        {
            Type = (ControllerEventType)br.ReadByte();
            ControllerIndex = br.ReadByte();
        }

        public void Serialize(LEWriter bw)
        {
            bw.Write((byte)Type);
            bw.Write(ControllerIndex);
        }
    }
}