using System;
using SmartGlass.Nano;
using SmartGlass.Nano.Packets;
using SmartGlass.Nano.Consumer;

namespace SmartGlass.Nano.Channels
{
    internal class InputFeedbackChannel : InputChannelBase
    {
        public bool HandshakeDone { get; internal set; }

        public event EventHandler<InputConfigEventArgs> FeedInputConfig;
        public event EventHandler<InputFrameEventArgs> FeedInputFrame;

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
            FeedInputFrame?.Invoke(this, new InputFrameEventArgs(frame));
        }

        public override void OnFrameAck(InputFrameAck ack)
        {
            throw new NotSupportedException("Input frameack on client side");
        }
    }
}