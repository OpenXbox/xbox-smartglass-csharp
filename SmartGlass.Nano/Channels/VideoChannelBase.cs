using System;
using SmartGlass.Nano.Packets;

namespace SmartGlass.Nano.Channels
{
    public abstract class VideoChannelBase : StreamingChannelBase, IStreamingChannel
    {
        public VideoChannelBase(NanoClient client, NanoChannel id)
            : base(client, id)
        {
        }

        public abstract void OnClientHandshake(VideoClientHandshake handshake);
        public abstract void OnServerHandshake(VideoServerHandshake handshake);
        public abstract void OnControl(VideoControl control);
        public abstract void OnData(VideoData data);

        public void OnPacket(IStreamerMessage packet)
        {
            switch ((VideoPayloadType)packet.StreamerHeader.PacketType)
            {
                case VideoPayloadType.ClientHandshake:
                    OnClientHandshake((VideoClientHandshake)packet);
                    break;
                case VideoPayloadType.ServerHandshake:
                    OnServerHandshake((VideoServerHandshake)packet);
                    break;
                case VideoPayloadType.Control:
                    OnControl((VideoControl)packet);
                    break;
                case VideoPayloadType.Data:
                    OnData((VideoData)packet);
                    break;
            }
        }
    }
}