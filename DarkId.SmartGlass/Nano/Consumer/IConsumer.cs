using System;
using DarkId.SmartGlass.Nano.Packets;

namespace DarkId.SmartGlass.Nano.Consumer
{
    internal interface IConsumer
    {
        void ConsumeVideoFormat(VideoFormat format);
        void ConsumeVideoData(VideoData data);
        void ConsumeAudioFormat(AudioFormat format);
        void ConsumeAudioData(AudioData data);
    }
}
