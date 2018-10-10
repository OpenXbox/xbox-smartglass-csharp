using System;
using System.Linq;
using System.Collections.Generic;
using DarkId.SmartGlass.Nano.Packets;
using System.IO;

namespace DarkId.SmartGlass.Nano.Consumer
{
    public class VideoAssembler
    {
        private Dictionary<uint, List<VideoData>> _videoData;

        public VideoAssembler()
        {
            _videoData = new Dictionary<uint, List<VideoData>>();
        }

        public byte[] AssembleVideoFrame(VideoData data)
        {
            // Keyframe: Flags 0x6 (20 -70 packets)
            // Intermediate: Flags 0x4 (1-2 packets)
            uint frameId = data.FrameId;
            uint packetCount = data.PacketCount;

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

                    return ms.ToArray();
                }
            }

            return null;
        }
    }
}
