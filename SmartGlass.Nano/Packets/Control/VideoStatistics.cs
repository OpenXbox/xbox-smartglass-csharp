using System;
using System.IO;
using SmartGlass.Common;
using SmartGlass.Nano;

namespace SmartGlass.Nano.Packets
{
    [ControlOpCode(ControlOpCode.VideoStatistics)]
    public class VideoStatistics : StreamerMessageWithHeader
    {
        public VideoStatistics()
            : base(ControlOpCode.VideoStatistics)
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