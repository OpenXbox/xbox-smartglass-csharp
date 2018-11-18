using System;
using SmartGlass.Nano;
using SmartGlass.Nano.Packets;
using SmartGlass.Nano.Consumer;
using System.Threading.Tasks;

namespace SmartGlass.Nano.Channels
{
    public class InputFeedbackChannel : InputChannelBase, IStreamingChannel
    {
        public override NanoChannel Channel => NanoChannel.InputFeedback;
        public event EventHandler<InputConfigEventArgs> FeedInputFeedbackConfig;
        public event EventHandler<InputFrameEventArgs> FeedInputFeedbackFrame;

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