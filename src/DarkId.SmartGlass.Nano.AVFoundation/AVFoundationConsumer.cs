using System;
using System.Diagnostics;
using AVFoundation;
using DarkId.SmartGlass.Nano.Consumer;
using DarkId.SmartGlass.Nano.Packets;

namespace DarkId.SmartGlass.Nano.AVFoundation
{
    public class AVFoundationConsumer : IConsumer, IDisposable
    {
        AudioEngineManager _audioEngineManager;

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
            // throw new NotImplementedException();
        }

        public void ConsumeVideoFormat(VideoFormat format)
        {
            // throw new NotImplementedException();
        }

        public void Dispose()
        {
            if (_audioEngineManager != null)
            {
                _audioEngineManager.Dispose();
            }
        }
    }
}
