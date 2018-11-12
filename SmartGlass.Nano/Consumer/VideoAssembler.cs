using System;
using System.Linq;
using System.Collections.Generic;
using SmartGlass.Nano.Packets;
using System.IO;

namespace SmartGlass.Nano.Consumer
{
    public class VideoAssembler
    {
        private Dictionary<uint, List<VideoData>> _videoData;

        public VideoAssembler()
        {
            _videoData = new Dictionary<uint, List<VideoData>>();
        }

        public H264Frame AssembleVideoFrame(VideoData data)
        {
            // Keyframe: Flags 0x6 (20 -70 packets)
            // Intermediate: Flags 0x4 (1-2 packets)
            uint frameId = data.FrameId;
            uint packetCount = data.PacketCount;
            long timeStamp = data.Timestamp;

            if (!_videoData.ContainsKey(frameId))
            {
                _videoData.Add(frameId, new List<VideoData>());
            }

            List<VideoData> frames = _videoData[frameId];
            frames.Add(data);

            if (packetCount != frames.Count)
            {
                // Frame is not complete, store what we have currently
                _videoData[frameId] = frames;
            }
            else
            {
                // Assemble final frame
                List<VideoData> sortedFrames = frames.OrderBy(fun => fun.Offset).ToList();
                _videoData.Remove(frameId);

                using (MemoryStream ms = new MemoryStream())
                {
                    foreach (VideoData frame in sortedFrames)
                    {
                        ms.Write(frame.Data, 0, frame.Data.Length);
                    }

                    return new H264Frame(ms.ToArray(), frameId, timeStamp);
                }
            }

            return null;
        }
    }
}
