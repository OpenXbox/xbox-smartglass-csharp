using System;
using SmartGlass.Nano.Packets;

namespace SmartGlass.Nano.Channels
{
    public abstract class AudioChannelBase : StreamingChannel, IStreamingChannel
    {
        public abstract void OnControl(AudioControl control);
        public abstract void OnData(AudioData data);

        public void OnPacket(IStreamerMessage packet)
        {
            switch ((AudioPayloadType)packet.StreamerHeader.PacketType)
            {
                case AudioPayloadType.Control:
                    OnControl((AudioControl)packet);
                    break;
                case AudioPayloadType.Data:
                    OnData((AudioData)packet);
                    break;
            }
        }
    }
}