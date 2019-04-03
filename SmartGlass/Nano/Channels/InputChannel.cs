using System;
using System.Threading.Tasks;
using SmartGlass.Common;
using SmartGlass.Nano;
using SmartGlass.Nano.Packets;

namespace SmartGlass.Nano.Channels
{
    public class InputChannel : InputChannelBase, IStreamingChannel
    {
        public override NanoChannel Channel => NanoChannel.Input;
        public override int ProtocolVersion => 3;

        internal InputChannel(NanoRdpTransport transport, ChannelOpen openPacket)
            : base(transport, openPacket)
        {
        }

        /// <summary>
        /// Send controller input frame
        /// </summary>
        /// <param name="createdTime">Creation timestamp of input data.</param>
        /// <param name="buttons">Input button data.</param>
        /// <param name="analogue">Input analog axis data.</param>
        /// <param name="extension">Input extension data.</param>
        /// <returns></returns>
        public async Task SendInputFrame(DateTime createdTime, InputButtons buttons,
                                   InputAnalogue analogue, InputExtension extension)
        {
            // Convert DateTime to relative timestamp
            // e.g. microSeconds since reference timestamp
            ulong createdTimestampMicroS = DateTimeHelper.ToTimestampMicroseconds(createdTime, ReferenceTimestamp);
            InputFrame frame = new InputFrame(NextFrameId, Timestamp, createdTimestampMicroS,
                                              buttons, analogue, extension);

            /*
            await WaitForMessageAsync<InputFrameAck>(
                TimeSpan.FromMilliseconds(10),
                async () => await SendAsync(frame)
            );
            */
            await SendAsync(frame);
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
            InputClientHandshake packet = new InputClientHandshake(10, ReferenceTimestampUlong);
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
                async () => await SendChannelOpen(NanoChannel.Input, _channelOpenData.Flags),
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