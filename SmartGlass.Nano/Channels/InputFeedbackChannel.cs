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
        public override int ProtocolVersion => 3;
        public event EventHandler<InputConfigEventArgs> FeedInputFeedbackConfig;
        public event EventHandler<InputFrameEventArgs> FeedInputFeedbackFrame;

        public InputFeedbackChannel(NanoClient client, byte[] flags,
                                    uint desktopWidth, uint desktopHeight)
        {
            _client = client;
            Flags = flags;
            DesktopWidth = desktopWidth;
            DesktopHeight = desktopHeight;
        }

        public override void OnFrame(InputFrame frame)
        {
            FeedInputFeedbackFrame?.Invoke(this, new InputFrameEventArgs(frame));
        }

        public override void OnFrameAck(InputFrameAck ack)
        {
            throw new NotSupportedException("Input frameack on client side");
        }

        public async Task OpenAsync()
        {
            // -> Console to client
            // <- Client to console
            // Input Feedback
            // -> ChannelCreate
            // -> ChannelOpen
            // <- ChannelOpen
            // <- ServerHandshake
            // -> ClientHandshake
            Task<ChannelCreate> waitCreateTask = _client.WaitForMessageAsync<ChannelCreate>(
                TimeSpan.FromSeconds(3),
                startAction: null,
                filter: p => p.Channel == NanoChannel.InputFeedback);

            Task<ChannelOpen> openTask = _client.WaitForMessageAsync<ChannelOpen>(
                TimeSpan.FromSeconds(3),
                startAction: null,
                filter: open => open.Channel == NanoChannel.InputFeedback);

            await Task.WhenAll(waitCreateTask, openTask);

            await _client.SendOnControlSocketAsync(
                new ChannelOpen(openTask.Result.Flags));

            InputServerHandshake serverHandshake = new InputServerHandshake(
                (uint)ProtocolVersion,
                DesktopWidth,
                DesktopHeight,
                maxTouches: 0,
                initialFrameId: FrameId);

            InputClientHandshake handshake = await _client.WaitForMessageAsync<InputClientHandshake>(
                TimeSpan.FromSeconds(3),
                async () => await _client.SendOnControlSocketAsync(serverHandshake),
                p => p.Channel == NanoChannel.InputFeedback
            );

            MaxTouches = handshake.MaxTouches;
            ReferenceTimestamp = handshake.ReferenceTimestamp;
        }
    }
}