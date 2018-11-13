using System;
using SmartGlass.Nano;
using SmartGlass.Nano.Packets;

namespace SmartGlass.Nano.Channels
{
    internal class InputChannel : InputChannelBase
    {
        public bool HandshakeDone { get; internal set; }

        public InputChannel(NanoClient client)
            : base(client, NanoChannelId.Input)
        {
            HandshakeDone = false;
        }

        public void OnInputFrameReceived(object sender, InputFrameEventArgs args)
        {
            throw new NotImplementedException("");
        }

        public void OnInputConfigReceived(object sender, InputConfigEventArgs args)
        {
            throw new NotImplementedException("");
        }

        public override void OnClientHandshake(InputClientHandshake handshake)
        {
            throw new NotImplementedException("");
        }

        public override void OnServerHandshake(InputServerHandshake handshake)
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