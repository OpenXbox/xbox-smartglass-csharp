using System;
using System.IO;
using SmartGlass.Common;
using SmartGlass.Nano;

namespace SmartGlass.Nano.Packets
{
    [VideoPayloadType(VideoPayloadType.Control)]
    public class VideoControl : StreamerMessage
    {
        public VideoControlFlags Flags { get; private set; }
        public uint LastDisplayedFrameId { get; private set; }
        public long Timestamp { get; private set; }
        public uint QueueDepth { get; private set; }
        public uint FirstLostFrame { get; private set; }
        public uint LastLostFrame { get; private set; }

        public VideoControl()
            : base((uint)VideoPayloadType.Control)
        {
        }

        public VideoControl(VideoControlFlags flags,
                            uint lastDisplayedFrameId = 0, long timestamp = 0,
                            uint queueDepth = 0, uint firstLostFrame = 0,
                            uint lastLostFrame = 0)
            : base((uint)VideoPayloadType.Control)
        {
            Flags = flags;
            LastDisplayedFrameId = lastDisplayedFrameId;
            Timestamp = timestamp;
            QueueDepth = queueDepth;
            FirstLostFrame = firstLostFrame;
            LastLostFrame = lastLostFrame;
        }

        public override void DeserializeStreamer(BinaryReader reader)
        {
            Flags = (VideoControlFlags)reader.ReadUInt32();

            if (Flags.HasFlag(VideoControlFlags.LastDisplayedFrame))
            {
                LastDisplayedFrameId = reader.ReadUInt32();
                Timestamp = reader.ReadInt64();
            }
            if (Flags.HasFlag(VideoControlFlags.QueueDepth))
            {
                QueueDepth = reader.ReadUInt32();
            }
            if (Flags.HasFlag(VideoControlFlags.LostFrames))
            {
                FirstLostFrame = reader.ReadUInt32();
                LastLostFrame = reader.ReadUInt32();
            }
        }

        public override void SerializeStreamer(BinaryWriter writer)
        {
            writer.Write((uint)Flags);
            if (Flags.HasFlag(VideoControlFlags.LastDisplayedFrame))
            {
                writer.Write(LastDisplayedFrameId);
                writer.Write(Timestamp);
            }
            if (Flags.HasFlag(VideoControlFlags.QueueDepth))
            {
                writer.Write(QueueDepth);
            }
            if (Flags.HasFlag(VideoControlFlags.LostFrames))
            {
                writer.Write(FirstLostFrame);
                writer.Write(LastLostFrame);
            }
        }
    }
}
