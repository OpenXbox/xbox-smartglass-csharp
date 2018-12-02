using System;
using System.Threading.Tasks;
using SmartGlass.Nano;
using SmartGlass.Nano.Packets;

namespace SmartGlass.Nano.Channels
{
    public class ChatAudioChannel : AudioChannelBase, IStreamingChannel
    {
        public override NanoChannel Channel => NanoChannel.ChatAudio;
        public override int ProtocolVersion => 4;
        public bool IsStarted { get; private set; } = false;
        public Packets.AudioFormat[] AvailableFormats { get; internal set; }
        public Packets.AudioFormat ActiveFormat { get; internal set; }

        internal ChatAudioChannel(NanoRdpTransport transport, byte[] flags)
            : base(transport, flags)
        {
            AvailableFormats = new AudioFormat[] { };
        }

        public async Task SendChatAudioData(byte[] sampleData)
        {
            AudioData data = new AudioData(0, FrameId, Timestamp, sampleData);
            await SendAsync(data);
        }

        public override void OnControl(AudioControl control)
        {

        }

        private async Task SendServerHandshakeAsync()
        {
            var packet = new AudioServerHandshake((uint)ProtocolVersion,
                                                    ReferenceTimestamp,
                                                    AvailableFormats);
            await SendAsync(packet);
        }

        public override void OnData(AudioData data)
        {
            throw new NotSupportedException("ChatAudio data on client side");
        }

        public async Task OpenAsync(AudioFormat format)
        {
            // -> Console to client
            // <- Client to console 
            // ChatAudio
            // -> ChannelOpen
            // <- ChannelOpen
            // <- Server handshake
            // -> Client handshake
            // -> AudioControl
            AvailableFormats = new AudioFormat[] { format };
            await _transport.SendChannelOpen(Channel, Flags);

            Task<AudioControl> controlStart = WaitForMessageAsync<AudioControl>(
                TimeSpan.FromSeconds(3),
                null,
                p => p.Channel == NanoChannel.ChatAudio
            );

            Task<AudioClientHandshake> handshake = WaitForMessageAsync<AudioClientHandshake>(
                TimeSpan.FromSeconds(3),
                async () => await SendServerHandshakeAsync(),
                p => p.Channel == NanoChannel.ChatAudio
            );

            await Task.WhenAll(handshake, controlStart);

            AudioFormat requestedFormat = handshake.Result.RequestedFormat;
            if (requestedFormat.Channels != AvailableFormats[0].Channels ||
                requestedFormat.SampleRate != AvailableFormats[0].SampleRate ||
                requestedFormat.Codec != AvailableFormats[0].Codec)
                throw new NanoException("ChatAudioChannel: Available / requested format mismatch!");

            FrameId = handshake.Result.InitialFrameID;
        }
    }
}