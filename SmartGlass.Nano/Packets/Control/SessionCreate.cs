using System;
using System.IO;
using SmartGlass.Common;
using SmartGlass.Nano;

namespace SmartGlass.Nano.Packets
{
    [ControlOpCode(ControlOpCode.SessionCreate)]
    public class SessionCreate : StreamerMessageWithHeader
    {
        public SessionCreate()
            : base(ControlOpCode.SessionCreate)
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