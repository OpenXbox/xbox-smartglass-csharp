using System;
using SmartGlass.Nano.Packets;

namespace SmartGlass.Nano
{
    public class VideoFormatEventArgs : EventArgs
    {
        public Packets.VideoFormat VideoFormat { get; private set; }
        public VideoFormatEventArgs(Packets.VideoFormat format)
        {
            VideoFormat = format;
        }
    }

    public class VideoDataEventArgs : EventArgs
    {
        public DateTime Timestamp { get; private set; }
        public Packets.VideoData VideoData { get; private set; }
        public VideoDataEventArgs(DateTime timestamp, Packets.VideoData data)
        {
            Timestamp = timestamp;
            VideoData = data;
        }
    }

    public class AudioFormatEventArgs : EventArgs
    {
        public Packets.AudioFormat Format { get; private set; }
        public AudioFormatEventArgs(Packets.AudioFormat format)
        {
            Format = format;
        }
    }

    public class AudioDataEventArgs : EventArgs
    {
        public DateTime Timestamp { get; private set; }
        public Packets.AudioData AudioData { get; private set; }
        public AudioDataEventArgs(DateTime timestamp, Packets.AudioData data)
        {
            Timestamp = timestamp;
            AudioData = data;
        }
    }

    public class InputConfigEventArgs : EventArgs
    {
        public ControllerEventType EventType { get; private set; }
        public int ControllerIndex { get; private set; }
        public InputConfigEventArgs(ControllerEventType type, int controllerIndex)
        {
            EventType = type;
            ControllerIndex = ControllerIndex;
        }
    }

    public class InputFrameEventArgs : EventArgs
    {
        public Packets.InputFrame InputFrame { get; private set; }
        public InputFrameEventArgs(Packets.InputFrame frame)
        {
            InputFrame = frame;
        }
    }
}