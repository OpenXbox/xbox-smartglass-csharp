using System;
using System.Diagnostics;
using System.Threading.Tasks;
using DarkId.SmartGlass.Nano;
using DarkId.SmartGlass.Nano.Packets;

namespace DarkId.SmartGlass.Nano.Channels
{
    internal class AudioChannel : AudioChannelBase
    {
        public bool HandshakeDone { get; internal set; }
        public Packets.AudioFormat[] AvailableFormats { get; internal set; }
        public Packets.AudioFormat ActiveFormat { get; internal set; }

        public AudioChannel(NanoClient client)
            : base(client, NanoChannelId.Audio)
        {
            HandshakeDone = false;
        }

        public void StartStream()
        {
            SendControl(AudioControlFlags.StartStream);
        }

        public void StopStream()
        {
            SendControl(AudioControlFlags.StopStream);
        }

        public override void OnClientHandshake(AudioClientHandshake handshake)
        {
            throw new NotSupportedException("Client handshake on client side");
        }

        public override void OnServerHandshake(AudioServerHandshake handshake)
        {
            AvailableFormats = handshake.Formats;
            ActiveFormat = AvailableFormats[0];
            SendClientHandshake(ActiveFormat);
            HandshakeDone = true;

            // TODO: Kinda tight coupling here.
            // TODO: Need to make sure our threading model is stable
            _client._consumer.ConsumeAudioFormat(ActiveFormat);
        }

        public override void OnControl(AudioControl control)
        {
            throw new NotSupportedException("Control message on client side");
        }

        public override void OnData(AudioData data)
        {
            _client._consumer.ConsumeAudioData(data);
        }

        private void SendClientHandshake(AudioFormat format)
        {
            uint initialFrameId = GenerateInitialFrameId();
            var payload = new Streamer((uint)AudioPayloadType.ClientHandshake)
            {
                Data = new AudioClientHandshake(initialFrameId, format)
            };

            SendStreamerOnControlSocket(payload);
        }

        private void SendControl(AudioControlFlags flags)
        {
            var payload = new Streamer((uint)AudioPayloadType.Control)
            {
                Data = new AudioControl(flags)
            };

            SendStreamerOnControlSocket(payload);
        }
    }
}