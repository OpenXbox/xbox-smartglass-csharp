using System;
using System.Runtime.InteropServices;
using AudioToolbox;
using AVFoundation;
using DarkId.SmartGlass.Nano.Consumer;
using DarkId.SmartGlass.Nano.Packets;

namespace DarkId.SmartGlass.Nano.AVFoundation
{
    public class PcmAudioBufferDataConsumer : IAVFoundationAudioDataConsumer, IDisposable
    {
        public AVAudioPcmBuffer Buffer { get; }

        public PcmAudioBufferDataConsumer(AVAudioFormat format)
        {
            Buffer = new AVAudioPcmBuffer(format, 1024);
        }

        public void ConsumeAudioData(AudioData data)
        {
            Marshal.Copy(data.Data,
                (int)(data.FrameId % Buffer.FrameCapacity) * (int)Buffer.FrameLength,
                Buffer.FloatChannelData,
                (int)Buffer.FrameLength);
        }

        public void Dispose()
        {
            Buffer.Dispose();
        }
    }
}
