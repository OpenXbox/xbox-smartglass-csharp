using System;
using System.Diagnostics;
using System.IO;
using SmartGlass.Nano.Packets;

namespace SmartGlass.Nano.Consumer
{
    public class FileConsumer : IConsumer, IDisposable
    {
        private bool _disposed = false;
        // AAC frame id of packet is always 0, need to keep track manually
        private int audioFrameCount = 0;

        private readonly string _fileName;
        private readonly bool _dumpSingleFrames;

        private VideoAssembler _videoAssembler;
        private AudioAssembler _audioAssembler;

        private FileStream _videoFile;
        private FileStream _audioFile;

        public FileConsumer(string filename, bool singleFrames = false)
        {
            _fileName = filename;
            _dumpSingleFrames = singleFrames;
            _videoAssembler = new VideoAssembler();
            _audioAssembler = new AudioAssembler();

            if (!singleFrames)
            {
                _videoFile = new FileStream($"{filename}.video.raw", FileMode.Create);
                _audioFile = new FileStream($"{filename}.audio.raw", FileMode.Create);
            }
        }

        void IVideoConsumer.ConsumeVideoData(object sender, VideoDataEventArgs args)
        {
            H264Frame frame = _videoAssembler.AssembleVideoFrame(args.VideoData);
            if (frame == null)
            {
                return;
            }

            if (_dumpSingleFrames)
            {
                string frameFilename = $"{_fileName}.video.{frame.FrameId}.{frame.TimeStamp}.raw";
                using (FileStream fs = new FileStream(frameFilename, FileMode.CreateNew))
                {
                    fs.Write(frame.RawData, 0, frame.RawData.Length);
                }
            }
            else
            {
                _videoFile.Write(frame.RawData, 0, frame.RawData.Length);
            }
        }

        void IAudioConsumer.ConsumeAudioData(object sender, AudioDataEventArgs args)
        {
            AACFrame frame = _audioAssembler.AssembleAudioFrame(
                args.AudioData, AACProfile.LC, 48000, 2);

            if (frame == null)
            {
                return;
            }

            if (_dumpSingleFrames)
            {
                string frameFilename = $"{_fileName}.audio.{audioFrameCount}.{frame.TimeStamp}.raw";
                using (FileStream fs = new FileStream(frameFilename, FileMode.CreateNew))
                {
                    fs.Write(frame.RawData, 0, frame.RawData.Length);
                }
                audioFrameCount++;
            }
            else
            {
                _audioFile.Write(frame.RawData, 0, frame.RawData.Length);
            }
        }

        public void ConsumeInputFeedbackFrame(object sender, InputFrameEventArgs args)
        {
            throw new NotImplementedException();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _audioFile.Dispose();
                    _videoFile.Dispose();
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
