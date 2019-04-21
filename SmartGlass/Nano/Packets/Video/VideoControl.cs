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

        internal override void DeserializeStreamer(EndianReader reader)
        {
            Flags = (VideoControlFlags)reader.ReadUInt32LE();

            if (Flags.HasFlag(VideoControlFlags.LastDisplayedFrame))
            {
                LastDisplayedFrameId = reader.ReadUInt32LE();
                Timestamp = reader.ReadInt64LE();
            }
            if (Flags.HasFlag(VideoControlFlags.QueueDepth))
            {
                QueueDepth = reader.ReadUInt32LE();
            }
            if (Flags.HasFlag(VideoControlFlags.LostFrames))
            {
                FirstLostFrame = reader.ReadUInt32LE();
                LastLostFrame = reader.ReadUInt32LE();
            }
        }

        internal override void SerializeStreamer(EndianWriter writer)
        {
            writer.WriteLE((uint)Flags);
            if (Flags.HasFlag(VideoControlFlags.LastDisplayedFrame))
            {
                writer.WriteLE(LastDisplayedFrameId);
                writer.WriteLE(Timestamp);
            }
            if (Flags.HasFlag(VideoControlFlags.QueueDepth))
            {
                writer.WriteLE(QueueDepth);
            }
            if (Flags.HasFlag(VideoControlFlags.LostFrames))
            {
                writer.WriteLE(FirstLostFrame);
                writer.WriteLE(LastLostFrame);
            }
        }
    }
}
