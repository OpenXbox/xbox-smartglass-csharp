using System;
using SmartGlass.Common;
using SmartGlass.Nano.Packets;

namespace SmartGlass.Nano.Channels
{
    public abstract class AudioChannelBase : StreamingChannel, IStreamingChannel
    {
        public abstract void OnControl(AudioControl control);
        public abstract void OnData(AudioData data);

        internal AudioChannelBase(NanoRdpTransport transport, byte[] flags)
            : base(transport, flags)
        {
            _transport.MessageReceived += OnMessage;
        }

        public void OnMessage(object sender, MessageReceivedEventArgs<INanoPacket> args)
        {
            IStreamerMessage packet = args.Message as IStreamerMessage;
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