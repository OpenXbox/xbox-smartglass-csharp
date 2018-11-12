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

        public void Deserialize(LEReader br)
        {
        }

        public void Serialize(LEWriter bw)
        {
        }
    }
}