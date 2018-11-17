using System;
using SmartGlass.Nano.Packets;

namespace SmartGlass.Nano.Channels
{
    public abstract class InputChannelBase : StreamingChannelBase, IStreamingChannel
    {
        public InputChannelBase(NanoClient client, NanoChannel id)
            : base(client, id)
        {
        }

        public abstract void OnClientHandshake(InputClientHandshake handshake);
        public abstract void OnServerHandshake(InputServerHandshake handshake);
        public abstract void OnFrame(InputFrame frame);
        public abstract void OnFrameAck(InputFrameAck ack);

        public void OnPacket(IStreamerMessage packet)
        {
            switch ((InputPayloadType)packet.StreamerHeader.PacketType)
            {
                case InputPayloadType.ClientHandshake:
                    OnClientHandshake((InputClientHandshake)packet);
                    break;
                case InputPayloadType.ServerHandshake:
                    OnServerHandshake((InputServerHandshake)packet);
                    break;
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