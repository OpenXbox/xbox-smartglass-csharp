using System;
using System.IO;
using SmartGlass.Common;
using SmartGlass.Nano;

namespace SmartGlass.Nano.Packets
{
    [ControlOpCode(ControlOpCode.NetworkInformation)]
    internal class NetworkInformation : ISerializableLE
    {
        public NetworkInformation()
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