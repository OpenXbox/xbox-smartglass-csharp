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

    public interface IInputFeedbackConfigConsumer
    {
        void ConsumeInputFeedbackConfig(object sender, InputConfigEventArgs args);
    }

    public interface IInputFeedbackFrameConsumer
    {
        void ConsumeInputFeedbackFrame(object sender, InputFrameEventArgs args);
    }

    public interface IInputFeedbackConsumer
        : IInputFeedbackConfigConsumer, IInputFeedbackFrameConsumer
    {

    }

    public interface IConsumer
        : IAudioConsumer, IVideoConsumer, IInputFeedbackConsumer
    {
    }
}
