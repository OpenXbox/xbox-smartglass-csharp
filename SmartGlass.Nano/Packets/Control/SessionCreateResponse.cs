using System;
using System.IO;
using SmartGlass.Common;
using SmartGlass.Nano;

namespace SmartGlass.Nano.Packets
{
    [ControlOpCode(ControlOpCode.SessionCreateResponse)]
    internal class SessionCreateResponse : ISerializableLE
    {
        public SessionCreateResponse()
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