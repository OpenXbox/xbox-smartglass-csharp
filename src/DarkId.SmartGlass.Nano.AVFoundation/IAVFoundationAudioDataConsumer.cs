using System;
using AVFoundation;
using DarkId.SmartGlass.Nano.Consumer;

namespace DarkId.SmartGlass.Nano.AVFoundation
{
    public interface IAVFoundationAudioDataConsumer : IAudioDataConsumer, IDisposable
    {
        AVAudioPcmBuffer Buffer { get; }
    }
}
