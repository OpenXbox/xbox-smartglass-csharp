using System;
using System.IO;
using SmartGlass.Common;
using SmartGlass.Nano;

namespace SmartGlass.Nano.Packets
{
    [ControlOpCode(ControlOpCode.NetworkTestResponse)]
    internal class NetworkTestResponse : ISerializableLE
    {
        public NetworkTestResponse()
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