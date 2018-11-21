using System;
using System.Diagnostics;
using System.Threading.Tasks;
using SmartGlass.Nano;
using SmartGlass.Nano.Packets;
using SmartGlass.Nano.Consumer;

namespace SmartGlass.Nano.Channels
{
    public class AudioChannel : AudioChannelBase, IStreamingChannel
    {
        public override NanoChannel Channel => NanoChannel.Audio;
        public override int ProtocolVersion => 4;
        public Packets.AudioFormat[] AvailableFormats { get; internal set; }

        public event EventHandler<AudioFormatEventArgs> FeedAudioFormat;
        public event EventHandler<AudioDataEventArgs> FeedAudioData;

        public AudioChannel(NanoClient client, byte[] flags)
        {
            _client = client;
            Flags = flags;
        }

        public async Task StartStreamAsync()
        {
            await SendControlAsync(AudioControlFlags.StartStream);
        }

        public async Task StopStreamAsync()
        {
            await SendControlAsync(AudioControlFlags.StopStream);
        }

        public override void OnControl(AudioControl control)
        {
            throw new NotSupportedException("Control message on client side");
        }

        public override void OnData(AudioData data)
        {
            FeedAudioData?.Invoke(this,
                new AudioDataEventArgs(data));
        }

        public async Task SendClientHandshakeAsync(AudioFormat format)
        {
            var packet = new AudioClientHandshake(FrameId, format);

            await SendStreamerOnControlSocket(packet);
        }

        private async Task SendControlAsync(AudioControlFlags flags)
        {
            var packet = new AudioControl(flags);
            await SendStreamerOnControlSocket(packet);
        }

        public async Task OpenAsync()
        {
            var handshake = await _client.WaitForMessageAsync<AudioServerHandshake>(
                TimeSpan.FromSeconds(1),
                async () => await _client.SendChannelOpenAsync(Channel, Flags)
            );

            ReferenceTimestamp = handshake.ReferenceTimestamp;
            AvailableFormats = handshake.Formats;
        }
    }
}
