using System;
using SmartGlass.Nano;
using SmartGlass.Nano.Packets;
using SmartGlass.Nano.Consumer;

namespace SmartGlass.Nano.Channels
{
    public class InputFeedbackChannel : InputChannelBase
    {
        public event EventHandler<InputConfigEventArgs> FeedInputFeedbackConfig;
        public event EventHandler<InputFrameEventArgs> FeedInputFeedbackFrame;

        public InputFeedbackChannel(NanoClient client)
            : base(client, NanoChannel.InputFeedback)
        {
        }

        public override void OnClientHandshake(InputClientHandshake handshake)
        {
        }

        public override void OnServerHandshake(InputServerHandshake handshake)
        {
        }

        public override void OnFrame(InputFrame frame)
        {
            FeedInputFeedbackFrame?.Invoke(this, new InputFrameEventArgs(frame));
        }

        public override void OnFrameAck(InputFrameAck ack)
        {
            throw new NotSupportedException("Input frameack on client side");
        }
    }
}