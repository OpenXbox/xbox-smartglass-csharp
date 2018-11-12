using System;
using System.IO;
using SmartGlass.Common;
using SmartGlass.Nano;

namespace SmartGlass.Nano.Packets
{
    [ControlOpCode(ControlOpCode.SessionCreate)]
    internal class SessionCreate : ISerializableLE
    {
        public SessionCreate()
        {
        }

        public void Deserialize(LEReader br)
        {
        }

        public void Serialize(LEWriter bw)
        {
        }
    }
}