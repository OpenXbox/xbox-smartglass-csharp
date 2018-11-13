using System;

namespace SmartGlass.Nano.Provider
{
    public interface IInputProvider
    {
        event EventHandler<InputConfigEventArgs> FeedInputConfig;
        event EventHandler<InputFrameEventArgs> FeedInputFrame;
    }

    public interface IChatAudioProvider
    {
        event EventHandler<AudioFormatEventArgs> FeedChatAudioFormat;
        event EventHandler<AudioDataEventArgs> FeedChatAudioData;
    }

    public interface IProvider : IInputProvider, IChatAudioProvider
    {
    }
}