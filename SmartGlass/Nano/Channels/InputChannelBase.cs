using System;
using Microsoft.Extensions.Logging;
using SmartGlass.Common;
using SmartGlass.Nano.Packets;

namespace SmartGlass.Nano.Channels
{
    public abstract class InputChannelBase : StreamingChannel, IStreamingChannel
    {
        private static readonly ILogger logger = Logging.Factory.CreateLogger<InputChannelBase>();
        public uint MaxTouches { get; internal set; }
        public uint DesktopWidth { get; internal set; }
        public uint DesktopHeight { get; internal set; }
        public abstract void OnFrame(InputFrame frame);
        public abstract void OnFrameAck(InputFrameAck ack);

        internal InputChannelBase(NanoRdpTransport transport, ChannelOpen openPacket)
            : base(transport, openPacket)
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

            switch ((InputPayloadType)packet.StreamerHeader.PacketType)
            {
                case InputPayloadType.Frame:
                    OnFrame((InputFrame)packet);
                    break;
                case InputPayloadType.FrameAck:
                    OnFrameAck((InputFrameAck)packet);
                    break;
            }
        }
    }
}