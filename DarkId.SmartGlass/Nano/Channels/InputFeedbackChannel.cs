using System;
using DarkId.SmartGlass.Nano;
using DarkId.SmartGlass.Nano.Packets;

namespace DarkId.SmartGlass.Nano.Channels
{
    internal class InputFeedbackChannel : InputChannelBase
    {
        public bool HandshakeDone { get; internal set; }

        public InputFeedbackChannel(NanoClient client)
            : base(client, NanoChannelId.InputFeedback)
        {
            HandshakeDone = false;
        }

        public override void OnClientHandshake(InputClientHandshake handshake)
        {
        }

        public override void OnServerHandshake(InputServerHandshake handshake)
        {
        }

        public override void OnFrame(InputFrame frame)
        {
        }

        public override void OnFrameAck(InputFrameAck ack)
        {
            throw new NotSupportedException("Input frameack on client side");
        }
    }
}