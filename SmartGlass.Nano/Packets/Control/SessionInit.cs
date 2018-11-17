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

        public override void DeserializeStreamer(BinaryReader reader)
        {
            throw new NotImplementedException();
        }

        public override void SerializeStreamer(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}