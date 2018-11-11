using System;
using System.IO;
using DarkId.SmartGlass.Common;
using DarkId.SmartGlass.Nano;

namespace DarkId.SmartGlass.Nano.Packets
{
    internal class StreamerMessageWithHeader : ISerializableLE
    {
        private static ISerializableLE CreateFromControlOpCode(ControlOpCode opCode)
        {
            var type = ControlOpCodeAttribute.GetTypeForMessageType(opCode);
            if (type == null)
            {
                return null;
            }

            return (ISerializableLE)Activator.CreateInstance(type);
        }

        public ControlHeader Header { get; private set; }
        public ISerializableLE Payload { get; private set; }

        public StreamerMessageWithHeader()
        {
        }
        
        public StreamerMessageWithHeader(ControlHeader header, ISerializableLE payload)
        {
            Header = header;
            Payload = payload;
        }

        public void Deserialize(LEReader br)
        {
            Header.Deserialize(br);
            Payload = CreateFromControlOpCode(Header.OpCode);
            Payload.Deserialize(br);
        }

        public void Serialize(LEWriter bw)
        {
            Header.Serialize(bw);
            Payload.Serialize(bw);
        }
    }
}