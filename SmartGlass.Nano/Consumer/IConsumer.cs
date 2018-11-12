using System;
using SmartGlass.Nano.Packets;

namespace SmartGlass.Nano.Consumer
{
    internal interface IConsumer
    {
        void ConsumeVideoFormat(VideoFormat format);
        void ConsumeVideoData(VideoData data);
        void ConsumeAudioFormat(AudioFormat format);
        void ConsumeAudioData(AudioData data);
    }
}
