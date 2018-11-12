using System;
using SmartGlass.Nano.Packets;

namespace SmartGlass.Nano.Consumer
{
    public interface IAudioFormatConsumer
    {
        void ConsumeAudioFormat(object sender, AudioFormatEventArgs args);
    }

    public interface IAudioDataConsumer
    {
        void ConsumeAudioData(object sender, AudioDataEventArgs args);
    }

    public interface IAudioConsumer : IAudioFormatConsumer, IAudioDataConsumer
    {

    }

    public interface IVideoFormatConsumer
    {
        void ConsumeVideoFormat(object sender, VideoFormatEventArgs args);
    }

    public interface IVideoDataConsumer
    {
        void ConsumeVideoData(object sender, VideoDataEventArgs args);
    }

    public interface IVideoConsumer : IVideoFormatConsumer, IVideoDataConsumer
    {

    }

    public interface IInputConfigConsumer
    {
        void ConsumeInputConfig(object sender, InputConfigEventArgs args);
    }

    public interface IInputFrameConsumer
    {
        void ConsumeInputFrame(object sender, InputFrameEventArgs args);
    }

    public interface IInputConsumer : IInputConfigConsumer, IInputFrameConsumer
    {

    }

    public interface IConsumer : IAudioConsumer, IVideoConsumer, IInputConsumer
    {
    }
}
