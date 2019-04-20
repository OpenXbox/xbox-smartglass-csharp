using System;
using System.Collections.Generic;
using SmartGlass.Nano.Packets;

namespace SmartGlass.Nano.Consumer
{
    public static class AudioAssembler
    {
        public static AACFrame AssembleAudioFrame(AudioData data, AACProfile profile,
                                                int samplingFreq, byte channels)
        {
            return new AACFrame(data.Data, data.Timestamp, data.FrameId, data.Flags,
                                profile, samplingFreq, channels);
        }
    }
}
