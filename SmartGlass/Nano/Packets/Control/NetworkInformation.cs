using System;
using System.IO;
using SmartGlass.Common;
using SmartGlass.Nano;

namespace SmartGlass.Nano.Packets
{
    [ControlOpCode(ControlOpCode.NetworkInformation)]
    public class NetworkInformation : StreamerMessageWithHeader
    {
        public NetworkInformation()
            : base(ControlOpCode.NetworkInformation)
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