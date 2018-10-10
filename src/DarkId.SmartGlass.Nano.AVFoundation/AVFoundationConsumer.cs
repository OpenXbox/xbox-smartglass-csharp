using System;
using System.Diagnostics;
using AVFoundation;
using DarkId.SmartGlass.Nano.Consumer;
using DarkId.SmartGlass.Nano.Packets;

namespace DarkId.SmartGlass.Nano.AVFoundation
{
    public class AVFoundationConsumer : IConsumer, IDisposable
    {
        VideoAssembler _videoAssembler;
        AudioEngineManager _audioEngineManager;
        VideoEngineManager _videoEngineManager;

        public AVFoundationConsumer()
        {
            _videoAssembler = new VideoAssembler();
        }

        public void ConsumeAudioData(AudioData data)
        {
            _audioEngineManager.ConsumeAudioData(data);
        }

        public void ConsumeAudioFormat(AudioFormat format)
        {
            try
            {
                _audioEngineManager = new AudioEngineManager(format);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        public void ConsumeVideoData(VideoData data)
        {
            byte[] frameData = _videoAssembler.AssembleVideoFrame(data);
            if (frameData != null)
            {
                _videoEngineManager.ConsumeVideoData(data.Timestamp, frameData);
            }
        }

        public void ConsumeVideoFormat(VideoFormat format)
        {
            try
            {
                _videoEngineManager = new VideoEngineManager(format);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        public void Dispose()
        {
            if (_audioEngineManager != null)
            {
                _audioEngineManager.Dispose();
            }
            if (_videoEngineManager != null)
            {
                _videoEngineManager.Dispose();
            }
        }
    }
}
