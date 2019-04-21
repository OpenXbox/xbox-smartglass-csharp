using System;
using System.IO;
using SmartGlass.Common;
using SmartGlass.Nano;

namespace SmartGlass.Nano.Packets
{
    [ControlOpCode(ControlOpCode.NetworkTestResponse)]
    public class NetworkTestResponse : StreamerMessageWithHeader
    {
        public NetworkTestResponse()
            : base(ControlOpCode.NetworkTestResponse)
        {
        }

        internal override void DeserializeStreamer(EndianReader reader)
        {
            throw new NotImplementedException();
        }

        internal override void SerializeStreamer(EndianWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}