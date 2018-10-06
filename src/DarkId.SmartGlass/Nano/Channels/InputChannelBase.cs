using System;
using DarkId.SmartGlass.Nano.Packets;

namespace DarkId.SmartGlass.Nano.Channels
{
    internal abstract class InputChannelBase : StreamingChannelBase, IStreamingChannel
    {
        public InputChannelBase(NanoClient client, NanoChannelId id)
            : base(client, id)
        {
        }

        public abstract void OnClientHandshake(InputClientHandshake handshake);
        public abstract void OnServerHandshake(InputServerHandshake handshake);
        public abstract void OnFrame(InputFrame frame);
        public abstract void OnFrameAck(InputFrameAck ack);

        public void OnStreamer(Streamer streamer)
        {
            switch((InputPayloadType)streamer.PacketType)
            {
                case InputPayloadType.ClientHandshake:
                    streamer.DeserializeData(new InputClientHandshake());
                    OnClientHandshake((InputClientHandshake)streamer.Data);
                    break;
                case InputPayloadType.ServerHandshake:
                    streamer.DeserializeData(new InputServerHandshake());
                    OnServerHandshake((InputServerHandshake)streamer.Data);
                    break;
                case InputPayloadType.Frame:
                    streamer.DeserializeData(new InputFrame());
                    OnFrame((InputFrame)streamer.Data);
                    break;
                case InputPayloadType.FrameAck:
                    streamer.DeserializeData(new InputFrameAck());
                    OnFrameAck((InputFrameAck)streamer.Data);
                    break;
            }
        }
    }
}