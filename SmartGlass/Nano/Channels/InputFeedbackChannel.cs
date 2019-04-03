using System;
using SmartGlass.Nano;
using SmartGlass.Nano.Packets;
using SmartGlass.Nano.Consumer;
using System.Threading.Tasks;
using SmartGlass.Common;

namespace SmartGlass.Nano.Channels
{
    public class InputFeedbackChannel : InputChannelBase, IStreamingChannel
    {
        readonly Action<InputFrameEventArgs> _fireInputFrameEvent;
        public override NanoChannel Channel => NanoChannel.InputFeedback;
        public override int ProtocolVersion => 3;

        internal InputFeedbackChannel(NanoRdpTransport transport, ChannelOpen openPacket,
            Action<InputFrameEventArgs> fireEvent)
            : base(transport, openPacket)
        {
            _fireInputFrameEvent = fireEvent;
        }

        public override void OnFrame(InputFrame frame)
        {
            DateTime frameTime = DateTimeHelper.FromTimestampMicroseconds(frame.CreatedTimestamp, ReferenceTimestamp);
            SendAsync(new InputFrameAck(frame.FrameId)).GetAwaiter().GetResult();
            _fireInputFrameEvent(new InputFrameEventArgs(frameTime, frame));
        }

        public override void OnFrameAck(InputFrameAck ack)
        {
            throw new NotSupportedException("Input frameack on client side");
        }

        private async Task SendServerHandshakeAsync()
        {
            InputServerHandshake packet = new InputServerHandshake(
                (uint)ProtocolVersion,
                DesktopWidth,
                DesktopHeight,
                maxTouches: 0,
                initialFrameId: FrameId);
            await SendAsync(packet);
        }

        public async Task OpenAsync(uint desktopWidth, uint desktopHeight)
        {
            // -> Console to client
            // <- Client to console
            // Input Feedback
            // -> ChannelCreate
            // -> ChannelOpen
            // <- ChannelOpen
            // <- ServerHandshake
            // -> ClientHandshake
            DesktopWidth = desktopWidth;
            DesktopHeight = desktopHeight;

            Task<ChannelCreate> waitCreateTask = WaitForMessageAsync<ChannelCreate>(
                TimeSpan.FromSeconds(3),
                startAction: null,
                filter: p => p.Channel == NanoChannel.InputFeedback);

            Task<ChannelOpen> openTask = WaitForMessageAsync<ChannelOpen>(
                TimeSpan.FromSeconds(3),
                startAction: null,
                filter: open => open.Channel == NanoChannel.InputFeedback);

            await Task.WhenAll(waitCreateTask, openTask);

            await SendChannelOpen(NanoChannel.InputFeedback, openTask.Result.Flags);

            InputClientHandshake handshake = await WaitForMessageAsync<InputClientHandshake>(
                TimeSpan.FromSeconds(3),
                async () => await SendServerHandshakeAsync(),
                p => p.Channel == NanoChannel.InputFeedback
            );

            MaxTouches = handshake.MaxTouches;
            ReferenceTimestampUlong = handshake.ReferenceTimestamp;
        }
    }
}
