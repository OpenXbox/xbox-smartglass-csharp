using System;
using System.Diagnostics;
using System.IO;
using SmartGlass.Nano.Packets;

namespace SmartGlass.Nano.Consumer
{
    public class FileConsumer : IConsumer, IDisposable
    {
        // AAC frame id of packet is always 0, need to keep track manually
        private int audioFrameCount = 0;

        private readonly string _fileName;
        private readonly bool _dumpSingleFrames;

        private VideoAssembler _videoAssembler;

        private FileStream _videoFile;
        private FileStream _audioFile;

        public FileConsumer(string filename, bool singleFrames = false)
        {
            _fileName = filename;
            _dumpSingleFrames = singleFrames;
            _videoAssembler = new VideoAssembler();

            if (!singleFrames)
            {
                _videoFile = new FileStream($"{filename}.video.raw", FileMode.Create);
                _audioFile = new FileStream($"{filename}.audio.raw", FileMode.Create);
            }
        }

        void IVideoFormatConsumer.ConsumeVideoFormat(object sender, VideoFormatEventArgs args)
        {
        }

        void IVideoDataConsumer.ConsumeVideoData(object sender, VideoDataEventArgs args)
        {
            H264Frame frame = _videoAssembler.AssembleVideoFrame(args.VideoData);
            if (frame == null)
            {
                return;
            }

            if (_dumpSingleFrames)
            {
                FileStream fs = new FileStream(
                    $"{_fileName}.video.{frame.FrameId}.{frame.TimeStamp}.raw",
                    FileMode.CreateNew);
                fs.Write(frame.RawData, 0, frame.RawData.Length);
                fs.Flush(); fs.Close();
            }
            else
            {
                _videoFile.Write(frame.RawData, 0, frame.RawData.Length);
            }
        }

        void IAudioFormatConsumer.ConsumeAudioFormat(object sender, AudioFormatEventArgs args)
        {
        }

        void IAudioDataConsumer.ConsumeAudioData(object sender, AudioDataEventArgs args)
        {
            AACFrame frame = AudioAssembler.AssembleAudioFrame(
                args.AudioData, AACProfile.LC, 48000, 2);

            if (frame == null)
            {
                return;
            }

            if (_dumpSingleFrames)
            {
                FileStream fs = new FileStream(
                    $"{_fileName}.audio.{audioFrameCount}.{frame.TimeStamp}.raw",
                    FileMode.CreateNew);
                fs.Write(frame.RawData, 0, frame.RawData.Length);
                fs.Flush(); fs.Close();
                audioFrameCount++;
            }
            else
            {
                _audioFile.Write(frame.RawData, 0, frame.RawData.Length);
            }
        }

        public void Dispose()
        {
            _audioFile.Dispose();
            _videoFile.Dispose();
        }

        public void ConsumeInputConfig(object sender, InputConfigEventArgs args)
        {
            throw new NotImplementedException();
        }

        public void ConsumeInputFrame(object sender, InputFrameEventArgs args)
        {
            throw new NotImplementedException();
        }
    }
}
