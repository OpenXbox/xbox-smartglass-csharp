using System;
using System.IO;
using DarkId.SmartGlass.Nano.Packets;

namespace DarkId.SmartGlass.Nano.Consumer
{
    public class FileConsumer : IConsumer, IDisposable
    {
        private FileStream _videoFile;
        private FileStream _audioFile;
        public FileConsumer(string filename)
        {
            _videoFile = new FileStream($"{filename}.video.raw", FileMode.Create);
            _audioFile = new FileStream($"{filename}.audio.raw", FileMode.Create);
        }
        void IConsumer.ConsumeVideoFormat(VideoFormat format)
        {
        }

        void IConsumer.ConsumeVideoData(VideoData data)
        {
            _videoFile.Write(data.Data, 0, data.Data.Length);
        }

        void IConsumer.ConsumeAudioFormat(AudioFormat format)
        {
        }

        void IConsumer.ConsumeAudioData(AudioData data)
        {
            _audioFile.Write(data.Data, 0, data.Data.Length);
        }

        public void Dispose()
        {
            _audioFile.Dispose();
            _videoFile.Dispose();
        }
    }
}
