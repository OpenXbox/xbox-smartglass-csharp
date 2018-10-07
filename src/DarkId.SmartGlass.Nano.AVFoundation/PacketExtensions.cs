using System;
using DarkId.SmartGlass.Nano.Packets;
using AV = AVFoundation;
using AT = AudioToolbox;

namespace DarkId.SmartGlass.Nano.AVFoundation
{
    public static class PacketExtensions
    {                                 
        public static AV.AVAudioFormat ToAVAudioFormat(this AudioFormat format)
        {
            var settings = format.ToATAudioStreamDescription();
            return new AV.AVAudioFormat(ref settings);
        }

        public static AT.AudioStreamBasicDescription ToATAudioStreamDescription(this AudioFormat format)
        {
            return format.Codec == AudioCodec.PCM
                ? new AT.AudioStreamBasicDescription()
                {
                    Format = format.Codec.ToATFormatType(),
                    ChannelsPerFrame = (int)format.Channels,
                    SampleRate = format.SampleRate,
                    BytesPerFrame = (int)format.SampleSize,
                    BitsPerChannel = (int)format.SampleSize / 2 / 8,
                    FramesPerPacket = 1,
                    FormatFlags = AT.AudioFormatFlags.LinearPCMIsFloat,
                    BytesPerPacket = (int)format.SampleSize,
                }
                : new AT.AudioStreamBasicDescription()
                {
                    Format = format.Codec.ToATFormatType(),
                    ChannelsPerFrame = (int)format.Channels,
                    SampleRate = format.SampleRate,
                    // BytesPerFrame = (int)format.SampleSize,
                    BitsPerChannel = 16,
                    FramesPerPacket = 1,
                    FormatFlags = AT.AudioFormatFlags.IsFloat,
                    BytesPerPacket = (int)format.SampleSize
                };
        }

        public static AT.AudioFormatType ToATFormatType(this AudioCodec codec)
        {
            switch (codec)
            {
                case AudioCodec.AAC:
                    return AT.AudioFormatType.MPEG4AAC;
                case AudioCodec.PCM:
                    return AT.AudioFormatType.LinearPCM;
                case AudioCodec.Opus:
                    return AT.AudioFormatType.Opus;
                default:
                    throw new SmartGlassException("Unknown audio codec type.");
            }
        }
    }
}
