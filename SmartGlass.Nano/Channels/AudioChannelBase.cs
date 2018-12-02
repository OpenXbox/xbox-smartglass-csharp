using System;
using Microsoft.Extensions.Logging;
using SmartGlass.Common;
using SmartGlass.Nano.Packets;

namespace SmartGlass.Nano.Channels
{
    public abstract class AudioChannelBase : StreamingChannel, IStreamingChannel
    {
        private static readonly ILogger logger = Logging.Factory.CreateLogger<AudioChannelBase>();

        public abstract void OnControl(AudioControl control);
        public abstract void OnData(AudioData data);

        internal AudioChannelBase(NanoRdpTransport transport, byte[] flags)
            : base(transport, flags)
        {
            MessageReceived += OnMessage;
        }

        public void OnMessage(object sender, MessageReceivedEventArgs<INanoPacket> args)
        {
            IStreamerMessage packet = args.Message as IStreamerMessage;
            if (packet == null)
            {
                logger.LogTrace($"Not handling packet {args.Message.Header.PayloadType}");
                return;
            }

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