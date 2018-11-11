using System;
using DarkId.SmartGlass.Nano.Packets;

namespace DarkId.SmartGlass.Nano.Channels
{
    internal abstract class AudioChannelBase : StreamingChannelBase, IStreamingChannel
    {
        public AudioChannelBase(NanoClient client, NanoChannelId id)
            : base(client, id)
        {
        }

        public abstract void OnClientHandshake(AudioClientHandshake handshake);
        public abstract void OnServerHandshake(AudioServerHandshake handshake);
        public abstract void OnControl(AudioControl control);
        public abstract void OnData(AudioData data);

        public void OnStreamer(Streamer streamer)
        {
            switch((AudioPayloadType)streamer.PacketType)
            {
                case AudioPayloadType.ClientHandshake:
                    streamer.DeserializeData(new AudioClientHandshake());
                    OnClientHandshake((AudioClientHandshake)streamer.Data);
                    break;
                case AudioPayloadType.ServerHandshake:
                    streamer.DeserializeData(new AudioServerHandshake());
                    OnServerHandshake((AudioServerHandshake)streamer.Data);
                    break;
                case AudioPayloadType.Control:
                    streamer.DeserializeData(new AudioControl());
                    OnControl((AudioControl)streamer.Data);
                    break;
                case AudioPayloadType.Data:
                    streamer.DeserializeData(new AudioData());
                    OnData((AudioData)streamer.Data);
                    break;
            }
        }
    }
}