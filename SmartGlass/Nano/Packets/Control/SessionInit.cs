using System;
using System.IO;
using SmartGlass.Common;
using SmartGlass.Nano;

namespace SmartGlass.Nano.Packets
{
    [ControlOpCode(ControlOpCode.SessionInit)]
    public class SessionInit : StreamerMessageWithHeader
    {
        public SessionInit()
            : base(ControlOpCode.SessionInit)
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