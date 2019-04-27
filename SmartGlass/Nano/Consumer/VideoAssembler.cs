using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly Dictionary<uint, VideoFrameData> _videoData = new Dictionary<uint, VideoFrameData>();

        private H264Frame _cachedFrame = null;

        private uint _lastProcessedFrameId;

        private uint _nextExpectedFrameId;

        private const int MaxBufferedFramePacketSize = 20;

        public H264Frame AssembleVideoFrame(VideoData data)
        {
            uint frameId = data.FrameId;
            if (frameId < _lastProcessedFrameId)
            {
                if (_lastProcessedFrameId - frameId >= 120) // at 60 fps max 4 secs
                {
                    // This is a fallback if the frameId gets reset at some point from the XBox
                    // (Don't know when this exactly happens)
                    _lastProcessedFrameId = 0;
                    _nextExpectedFrameId = 0;
                    _cachedFrame = null;
                }
                else
                {
                    // Ignore all frames which are lower than the last processed frame id
                    return null;
                }
            }


            if (!_videoData.ContainsKey(frameId))
            {
                if (_videoData.Count > MaxBufferedFramePacketSize)
                {
                    // Avoid overflow
                    _videoData.Clear();
                }

                var newData = new VideoFrameData()
                {
                    Flags = data.Flags,
                    Timestamp = data.Timestamp,
                    Data = new byte[data.TotalSize],
                    WrittenChunks = 0
                };

                _videoData.Add(frameId, newData);
            }

            Buffer.BlockCopy(data.Data, 0, _videoData[frameId].Data, (int)data.Offset, data.Data.Length);
            _videoData[frameId].WrittenChunks++;
            bool isFrameRead = _videoData[frameId].WrittenChunks >= data.PacketCount;
            return isFrameRead ? ProcessFrameIsReady(frameId) : ProcessFrameIsNotReady();
        }

        private H264Frame ProcessFrameIsReady(uint frameId)
        {
            H264Frame h264Frame = null;

            if (_nextExpectedFrameId == 0 || _nextExpectedFrameId == frameId)
            {
                if (!_videoData.TryGetValue(frameId, out var completeFrame))
                    return null;
                _videoData.Remove(frameId);
                _lastProcessedFrameId = frameId;
                _nextExpectedFrameId = _lastProcessedFrameId + 1;
                CleanVideoDataDictionary(_lastProcessedFrameId);
                CheckCachedFrame(_lastProcessedFrameId);
                h264Frame = new H264Frame(completeFrame.Data, frameId, completeFrame.Timestamp, completeFrame.Flags);
            }
            else
            {
                if (_cachedFrame == null)
                {
                    if (_videoData.TryGetValue(frameId, out var completeFrame))
                    {
                        // Cache current ready frame because it is not expected frameId
                        _videoData.Remove(frameId);
                        _cachedFrame = new H264Frame(completeFrame.Data, frameId, completeFrame.Timestamp, completeFrame.Flags);
                    }
                }
                else
                {
                    if (_cachedFrame.FrameId == _nextExpectedFrameId || _cachedFrame.FrameId < frameId)
                    {
                        // Offer cached frame
                        _lastProcessedFrameId = _cachedFrame.FrameId;
                        _nextExpectedFrameId = _lastProcessedFrameId + 1;
                        CleanVideoDataDictionary(_lastProcessedFrameId);
                        h264Frame = _cachedFrame;
                        _cachedFrame = null;

                        if (_videoData.TryGetValue(frameId, out var completeFrame))
                        {
                            // Cache current ready frame
                            _videoData.Remove(frameId);
                            _cachedFrame = new H264Frame(completeFrame.Data, frameId, completeFrame.Timestamp, completeFrame.Flags);
                        }
                    }
                    else
                    {
                        if (!_videoData.TryGetValue(frameId, out var completeFrame))
                            return null;
                        _videoData.Remove(frameId);
                        _lastProcessedFrameId = frameId;
                        _nextExpectedFrameId = _lastProcessedFrameId + 1;
                        CleanVideoDataDictionary(_lastProcessedFrameId);
                        h264Frame = new H264Frame(completeFrame.Data, frameId, completeFrame.Timestamp, completeFrame.Flags);
                        _cachedFrame = null;
                    }
                }
            }

            return h264Frame;
        }

        private H264Frame ProcessFrameIsNotReady()
        {
            H264Frame h264Frame = null;
            if (_cachedFrame != null && _cachedFrame.FrameId == _nextExpectedFrameId)
            {
                h264Frame = _cachedFrame;
                _lastProcessedFrameId = _cachedFrame.FrameId;
                _nextExpectedFrameId = _lastProcessedFrameId + 1;
                CleanVideoDataDictionary(_lastProcessedFrameId);
                _cachedFrame = null;
            }
            return h264Frame;
        }

        private void CleanVideoDataDictionary(uint lastProcessedFrameId)
        {
            foreach (var key in _videoData.Keys.ToList())
            {
                if (key <= lastProcessedFrameId)
                {
                    _videoData.Remove(key);
                }
            }
        }

        private void CheckCachedFrame(uint lastProcessedFrameId)
        {
            if (_cachedFrame != null && _cachedFrame.FrameId <= lastProcessedFrameId)
            {
                _cachedFrame = null;
            }
        }
    }
}