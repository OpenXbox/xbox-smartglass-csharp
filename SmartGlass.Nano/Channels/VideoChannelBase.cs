using System;
using SmartGlass.Nano.Packets;

namespace SmartGlass.Nano.Channels
{
    internal abstract class VideoChannelBase : StreamingChannelBase, IStreamingChannel
    {
        public VideoChannelBase(NanoClient client, NanoChannelId id)
            : base(client, id)
        {
        }

        public abstract void OnClientHandshake(VideoClientHandshake handshake);
        public abstract void OnServerHandshake(VideoServerHandshake handshake);
        public abstract void OnControl(VideoControl control);
        public abstract void OnData(VideoData data);

        public void OnStreamer(Streamer streamer)
        {
            switch((VideoPayloadType)streamer.PacketType)
            {
                case VideoPayloadType.ClientHandshake:
                    streamer.DeserializeData(new VideoClientHandshake());
                    OnClientHandshake((VideoClientHandshake)streamer.Data);
                    break;
                case VideoPayloadType.ServerHandshake:
                    streamer.DeserializeData(new VideoServerHandshake());
                    OnServerHandshake((VideoServerHandshake)streamer.Data);
                    break;
                case VideoPayloadType.Control:
                    streamer.DeserializeData(new VideoControl());
                    OnControl((VideoControl)streamer.Data);
                    break;
                case VideoPayloadType.Data:
                    streamer.DeserializeData(new VideoData());
                    OnData((VideoData)streamer.Data);
                    break;
            }
        }
    }
}