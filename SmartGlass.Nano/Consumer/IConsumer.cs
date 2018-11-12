using System;
using SmartGlass.Nano.Packets;

namespace SmartGlass.Nano.Consumer
{
    public interface IAudioFormatConsumer
    {
        void ConsumeAudioFormat(AudioFormat format);
    }

    public interface IAudioDataConsumer
    {
        void ConsumeAudioData(AudioData data);
    }

    public interface IAudioConsumer : IAudioFormatConsumer, IAudioDataConsumer
    {

    }

    public interface IVideoFormatConsumer
    {
        void ConsumeVideoFormat(VideoFormat format);
    }

    public interface IVideoDataConsumer
    {
        void ConsumeVideoData(VideoData data);
    }

    public interface IVideoConsumer : IVideoFormatConsumer, IVideoDataConsumer
    {

    }

    public interface IConsumer : IAudioConsumer, IVideoConsumer
    {
    }
}
