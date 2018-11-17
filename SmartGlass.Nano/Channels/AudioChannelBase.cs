using System;
using SmartGlass.Nano.Packets;

namespace SmartGlass.Nano.Channels
{
    public abstract class AudioChannelBase : StreamingChannelBase, IStreamingChannel
    {
        public AudioChannelBase(NanoClient client, NanoChannel id)
            : base(client, id)
        {
        }

        public abstract void OnClientHandshake(AudioClientHandshake handshake);
        public abstract void OnServerHandshake(AudioServerHandshake handshake);
        public abstract void OnControl(AudioControl control);
        public abstract void OnData(AudioData data);

        public void OnPacket(IStreamerMessage packet)
        {
            switch ((AudioPayloadType)packet.StreamerHeader.PacketType)
            {
                case AudioPayloadType.ClientHandshake:
                    OnClientHandshake((AudioClientHandshake)packet);
                    break;
                case AudioPayloadType.ServerHandshake:
                    OnServerHandshake((AudioServerHandshake)packet);
                    break;
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