using System;
using System.IO;
using DarkId.SmartGlass.Common;
using DarkId.SmartGlass.Nano;

namespace DarkId.SmartGlass.Nano.Packets
{
    [VideoPayloadType(VideoPayloadType.Control)]
    internal class VideoControl : ISerializableLE
    {
        public VideoControlFlags Flags { get; private set; }
        public uint LastDisplayedFrameId { get; private set; }
        public long Timestamp { get; private set; }
        public uint QueueDepth { get; private set; }
        public uint FirstLostFrame { get; private set; }
        public uint LastLostFrame { get; private set; }

        public VideoControl()
        {
        }
        
        public VideoControl(VideoControlFlags flags,
                            uint lastDisplayedFrameId=0, long timestamp=0,
                            uint queueDepth=0, uint firstLostFrame=0,
                            uint lastLostFrame=0)
        {
            Flags = flags;
            LastDisplayedFrameId = lastDisplayedFrameId;
            Timestamp = timestamp;
            QueueDepth = queueDepth;
            FirstLostFrame = firstLostFrame;
            LastLostFrame = lastLostFrame;
        }

        public void Deserialize(LEReader br)
        {
            Flags = (VideoControlFlags)br.ReadUInt32();

            if (Flags.HasFlag(VideoControlFlags.LastDisplayedFrame))
            {
                LastDisplayedFrameId = br.ReadUInt32();
                Timestamp = br.ReadInt64();
            }
            if (Flags.HasFlag(VideoControlFlags.QueueDepth))
            {
                QueueDepth = br.ReadUInt32();
            }
            if (Flags.HasFlag(VideoControlFlags.LostFrames))
            {
                FirstLostFrame = br.ReadUInt32();
                LastLostFrame = br.ReadUInt32();
            }
        }

        public void Serialize(LEWriter bw)
        {
            bw.Write((uint)Flags);
            if (Flags.HasFlag(VideoControlFlags.LastDisplayedFrame))
            {
                bw.Write(LastDisplayedFrameId);
                bw.Write(Timestamp);
            }
            if (Flags.HasFlag(VideoControlFlags.QueueDepth))
            {
                bw.Write(QueueDepth);
            }
            if (Flags.HasFlag(VideoControlFlags.LostFrames))
            {
                bw.Write(FirstLostFrame);
                bw.Write(LastLostFrame);
            }
        }
    }
}