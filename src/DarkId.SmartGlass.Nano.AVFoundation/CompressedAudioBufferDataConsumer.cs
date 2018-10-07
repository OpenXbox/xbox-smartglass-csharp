using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using AudioToolbox;
using AVFoundation;
using DarkId.SmartGlass.Nano.Consumer;
using DarkId.SmartGlass.Nano.Packets;
using Foundation;

namespace DarkId.SmartGlass.Nano.AVFoundation
{
    public class CompressedAudioBufferDataConsumer : IAVFoundationAudioDataConsumer
    {
        private readonly AVAudioCompressedBuffer _compressedBuffer;
        private readonly AVAudioConverter _converter;
        private readonly AudioStreamBasicDescription _pcmDescription;

        public AVAudioPcmBuffer Buffer { get; }

        // Python compressed packet header code:
        // header_id = 0  # MPEG4
        // adts_headers = bytearray(AACFrame.ADTS_HEADER_LEN)
        // frame_size += AACFrame.ADTS_HEADER_LEN
        // sampling_index = AACFrame.sampling_freq_index[sampling_freq]

        // adts_headers[0] = 0xFF
        // adts_headers[1] = 0xF0 | (header_id << 3) | 0x1
        // adts_headers[2] = (aac_profile << 6) | (sampling_index << 2) | 0x2 | \
        //                   (channels & 0x4)
        // adts_headers[3] = ((channels & 0x3) << 6) | 0x30 | (frame_size >> 11)
        // adts_headers[4] = ((frame_size >> 3) & 0x00FF)
        // adts_headers[5] = (((frame_size & 0x0007) << 5) + 0x1F)
        // adts_headers[6] = 0xFC

        // return adts_headers

        public CompressedAudioBufferDataConsumer(AVAudioFormat format)
        {
            _compressedBuffer = new AVAudioCompressedBuffer(format, 1024, 1024 * 2);

            _pcmDescription = AudioStreamBasicDescription.CreateLinearPCM();
            _converter = new AVAudioConverter(format, new AVAudioFormat(ref _pcmDescription));

            Buffer = new AVAudioPcmBuffer(new AVAudioFormat(ref _pcmDescription), 1024);
        }

        public void ConsumeAudioData(AudioData data)
        {
            // TODO: This is borked, but it doesn't matter until the stuff in the constructor doesn't crash, so...
            Marshal.Copy(data.Data,
                (int)(data.FrameId % _compressedBuffer.PacketCapacity) * (int)_compressedBuffer.MaximumPacketSize,
                _compressedBuffer.Data,
                (int)_compressedBuffer.MaximumPacketSize);

            _converter.ConvertToBuffer(Buffer, out NSError error, (uint inNumberOfPackets, out AVAudioConverterInputStatus outStatus) =>
             {
                 outStatus = AVAudioConverterInputStatus.HaveData;
                 return _compressedBuffer;
             });

            if (error != null)
            {
                Debug.WriteLine(error.ToString());
            }
        }

        public void Dispose()
        {
            Buffer.Dispose();
            _converter.Dispose();
            _compressedBuffer.Dispose();
        }
    }
}
