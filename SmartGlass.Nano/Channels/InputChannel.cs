using System;
using System.Threading.Tasks;
using SmartGlass.Nano;
using SmartGlass.Nano.Packets;

namespace SmartGlass.Nano.Channels
{
    public class InputChannel : InputChannelBase, IStreamingChannel
    {
        public override NanoChannel Channel => NanoChannel.Input;

        public void OnInputFrameReceived(object sender, InputFrameEventArgs args)
        {
            throw new NotImplementedException("");
        }

        public void OnInputConfigReceived(object sender, InputConfigEventArgs args)
        {
            throw new NotImplementedException("");
        }

        public override void OnFrame(InputFrame frame)
        {
            throw new NotSupportedException("Input frame on client side");
        }

        public override void OnFrameAck(InputFrameAck ack)
        {
            throw new NotImplementedException("");
        }
    }
}