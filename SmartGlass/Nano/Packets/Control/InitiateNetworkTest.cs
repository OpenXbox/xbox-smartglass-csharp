using System;
using System.IO;
using SmartGlass.Common;
using SmartGlass.Nano;

namespace SmartGlass.Nano.Packets
{
    [ControlOpCode(ControlOpCode.InitiateNetworkTest)]
    public class InitiateNetworkTest : StreamerMessageWithHeader
    {
        public InitiateNetworkTest()
            : base(ControlOpCode.InitiateNetworkTest)
        {
        }

        internal override void DeserializeStreamer(BinaryReader reader)
        {
            throw new NotImplementedException();
        }

        internal override void SerializeStreamer(BinaryWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}