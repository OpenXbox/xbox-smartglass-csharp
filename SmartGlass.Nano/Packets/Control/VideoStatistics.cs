using System;
using System.IO;
using SmartGlass.Common;
using SmartGlass.Nano;

namespace SmartGlass.Nano.Packets
{
    [ControlOpCode(ControlOpCode.VideoStatistics)]
    internal class VideoStatistics : ISerializableLE
    {
        public VideoStatistics()
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