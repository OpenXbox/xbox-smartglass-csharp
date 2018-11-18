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
        public Packets.AudioFormat[] AvailableFormats { get; internal set; }
        public Packets.AudioFormat ActiveFormat { get; internal set; }
        public event EventHandler<AudioFormatEventArgs> FeedAudioFormat;
        public event EventHandler<AudioDataEventArgs> FeedAudioData;

        public void StartStream()
        {
            SendControl(AudioControlFlags.StartStream);
        }

        public void StopStream()
        {
            SendControl(AudioControlFlags.StopStream);
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

        private void SendClientHandshake(AudioFormat format)
        {
            uint initialFrameId = GenerateInitialFrameId();
            var packet = new AudioClientHandshake(initialFrameId, format);

            SendStreamerOnControlSocket(packet);
        }

        private void SendControl(AudioControlFlags flags)
        {
            var packet = new AudioControl(flags);
            SendStreamerOnControlSocket(packet);
        }

        public async Task OpenAsync()
        {
            var handshake = await _client.WaitForMessageAsync<AudioServerHandshake>(
                TimeSpan.FromSeconds(1),
                async () => await SendChannelOpenAsync()
            );

            AvailableFormats = handshake.Formats;
        }
    }
}
