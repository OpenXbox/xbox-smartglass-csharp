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

        public InputChannel(NanoClient client, byte[] flags)
        {
            _client = client;
            Flags = flags;
        }

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

            // Send ControllerEvent.Added
            await _client.WaitForMessageAsync<ChannelCreate>(
                TimeSpan.FromSeconds(3),
                async () => await _client.Control.SendControllerEventAsync(ControllerEventType.Added, 0),
                p => p.Channel == NanoChannel.Input);

            // Start waiting for server handshake, it's received right after console's
            // ChannelOpen
            Task<InputServerHandshake> handshakeTask = _client.WaitForMessageAsync<InputServerHandshake>(
                TimeSpan.FromSeconds(3),
                null,
                p => p.Channel == NanoChannel.Input
            );

            // Send ChannelOpen and wait for ChannelOpen response
            Task<ChannelOpen> openTask = _client.WaitForMessageAsync<ChannelOpen>(
                TimeSpan.FromSeconds(3),
                async () => await _client.SendChannelOpenAsync(NanoChannel.Input, Flags),
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

            var clientHandshake = new InputClientHandshake(10, ReferenceTimestamp);
            await _client.SendOnControlSocketAsync(clientHandshake);
        }
    }
}