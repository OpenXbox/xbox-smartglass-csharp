using System;
using System.Linq;
using System.IO;
using System.Collections.Concurrent;

using SmartGlass.Nano.Packets;

namespace SmartGlass.Nano.Consumer
{

    public class VideoFrameData
    {
        // Keyframe: Flags & 0x2
        // Unknown: Flags & 0x4
        public bool IsKeyframe => (Flags & 2) == 2;
        public uint Flags;
        public ulong Timestamp;
        public byte[] Data;
        public int WrittenChunks;
    }

    public class VideoAssembler
    {
        private readonly ConcurrentDictionary<uint, VideoFrameData> _videoData
            = new ConcurrentDictionary<uint, VideoFrameData>();

        public H264Frame AssembleVideoFrame(VideoData data)
        {
            uint frameId = data.FrameId;
            if (!_videoData.ContainsKey(frameId))
            {
                var newData = new VideoFrameData()
                {
                    Flags = data.Flags,
                    Timestamp = data.Timestamp,
                    Data = new byte[data.TotalSize],
                    WrittenChunks = 0
                };

                if (!_videoData.TryAdd(frameId, newData))
                    return null;
            }

            Array.Copy(data.Data, 0, _videoData[frameId].Data, (int)data.Offset, data.Data.Length);
            _videoData[frameId].WrittenChunks++;

            if (_videoData[frameId].WrittenChunks != data.PacketCount)
                // Frame not ready
                return null;

            // Frame is ready
            if (!_videoData.TryRemove(frameId, out VideoFrameData completeFrame))
                return null;
            
            return new H264Frame(completeFrame.Data, frameId, completeFrame.Timestamp, completeFrame.Flags);
        }
    }
}