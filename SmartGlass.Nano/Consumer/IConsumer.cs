using System;
using SmartGlass.Nano.Packets;

namespace SmartGlass.Nano.Consumer
{
    public interface IAudioConsumer
    {
        void ConsumeAudioData(object sender, AudioDataEventArgs args);
    }

    public interface IVideoConsumer
    {
        void ConsumeVideoData(object sender, VideoDataEventArgs args);
    }

    public interface IInputFeedbackConsumer
    {
        void ConsumeInputFeedbackFrame(object sender, InputFrameEventArgs args);
    }

    public interface IConsumer
        : IAudioConsumer, IVideoConsumer, IInputFeedbackConsumer
    {
    }
}
