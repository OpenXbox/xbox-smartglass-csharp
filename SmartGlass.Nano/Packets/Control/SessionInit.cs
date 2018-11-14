using System;
using System.IO;
using SmartGlass.Common;
using SmartGlass.Nano;

namespace SmartGlass.Nano.Packets
{
    [ControlOpCode(ControlOpCode.SessionInit)]
    internal class SessionInit : ISerializableLE
    {
        public SessionInit()
        {
        }

        public void Deserialize(BinaryReader br)
        {
        }

        public void Serialize(BinaryWriter bw)
        {
        }
    }
}