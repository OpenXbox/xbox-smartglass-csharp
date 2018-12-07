using System;
using System.Threading.Tasks;
using SmartGlass.Nano;
using SmartGlass.Nano.Packets;

namespace SmartGlass.Nano.Channels
{
    public class InputChannel : InputChannelBase, IStreamingChannel
    {
        public override NanoChannel Channel => NanoChannel.Input;
        public override int ProtocolVersion => 3;

        internal InputChannel(NanoRdpTransport transport, byte[] flags)
            : base(transport, flags)
        {
        }

        private ulong TimestampFromMillisecondsSinceEpoch(ulong millisecondsSinceEpoch)
        {
            return millisecondsSinceEpoch - ReferenceTimestamp;
        }

        /// <summary>
        /// Send controller input frame
        /// </summary>
        /// <param name="createdTimestampMs">Timestamp since epoch, in milliseconds.</param>
        /// <param name="buttons">Input button data.</param>
        /// <param name="analogue">Input analog axis data.</param>
        /// <param name="extension">Input extension data.</param>
        /// <returns></returns>
        public async Task SendInputFrame(ulong createdTimestampMs, InputButtons buttons,
                                   InputAnalogue analogue, InputExtension extension)
        {
            // Convert absolute timestamp (milliseconds since epoch) to relative timestamp
            // e.g. milliseconds since reference timestamp
            createdTimestampMs = TimestampFromMillisecondsSinceEpoch(createdTimestampMs);

            InputFrame frame = new InputFrame(FrameId, Timestamp, createdTimestampMs,
                                              buttons, analogue, extension);

            await WaitForMessageAsync<InputFrameAck>(
                TimeSpan.FromMilliseconds(10),
                async () => await SendAsync(frame)
            );
        }

        public override void OnFrame(InputFrame frame)
        {
            throw new NotSupportedException("Input frame on client side");
        }

        public override void OnFrameAck(InputFrameAck ack)
        {
        }

        private async Task SendClientHandshakeAsync()
        {
            InputClientHandshake packet = new InputClientHandshake(10, ReferenceTimestamp);
            await SendAsync(packet);
        }

        public async Task OpenAsync()
        {
            // -> Console to client
            // <- Client to console 
            // Input
            // <- ControlChannel.ControllerEvent
            // -> ChannelCreate
            // <- ChannelOpen
            // -> ChannelOpen
            // -> ServerHandshake
            // <- ClientHandshake

            // Start waiting for server handshake, it's received right after console's
            // ChannelOpen
            Task<InputServerHandshake> handshakeTask = WaitForMessageAsync<InputServerHandshake>(
                TimeSpan.FromSeconds(3),
                null,
                p => p.Channel == NanoChannel.Input
            );

            // Send ChannelOpen and wait for ChannelOpen response
            Task<ChannelOpen> openTask = WaitForMessageAsync<ChannelOpen>(
                TimeSpan.FromSeconds(3),
                async () => await _transport.SendChannelOpen(NanoChannel.Input, Flags),
                p => p.Channel == NanoChannel.Input
            );

            await Task.WhenAll(openTask, handshakeTask);

            var handshake = handshakeTask.Result;
            if (handshake.ProtocolVersion != ProtocolVersion)
                throw new NanoException("InputChannel: Protocol version mismatch!");

            FrameId = handshake.InitialFrameId;
            MaxTouches = handshake.MaxTouches;
            DesktopWidth = handshake.DesktopWidth;
            DesktopHeight = handshake.DesktopHeight;

            await SendClientHandshakeAsync();
        }
    }
}