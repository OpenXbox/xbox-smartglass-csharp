using System;
using System.Diagnostics;
using DarkId.SmartGlass.Nano;
using DarkId.SmartGlass.Nano.Packets;

namespace DarkId.SmartGlass.Nano.Channels
{
    internal class VideoChannel : VideoChannelBase
    {
        public bool HandshakeDone { get; internal set; }
        public Packets.VideoFormat[] AvailableFormats { get; internal set; }
        public Packets.VideoFormat ActiveFormat { get; internal set; }

        public VideoChannel(NanoClient client)
            : base(client, NanoChannelId.Video)
        {
            HandshakeDone = false;
        }

        public void StartStream()
        {
            SendControl(VideoControlFlags.RequestKeyframe | VideoControlFlags.StartStream);
        }

        public void StopStream()
        {
            SendControl(VideoControlFlags.StopStream);
        }

        public override void OnClientHandshake(VideoClientHandshake handshake)
        {
            throw new NotSupportedException("Client handshake on client side");
        }

        public override void OnServerHandshake(VideoServerHandshake handshake)
        {
            AvailableFormats = handshake.Formats;
            ActiveFormat = AvailableFormats[0];
            SendClientHandshake(ActiveFormat);
            HandshakeDone = true;
        }

        public override void OnControl(VideoControl control)
        {
            throw new NotSupportedException("Control message on client side");
        }

        public override void OnData(VideoData data)
        {
            _client._consumer.ConsumeVideoData(data);
        }

        private void SendClientHandshake(VideoFormat format)
        {
            uint initialFrameId = GenerateInitialFrameId();
            var payload = new Streamer((uint)VideoPayloadType.ClientHandshake)
            {
                Data = new VideoClientHandshake(initialFrameId, format)
            };

            SendStreamerOnControlSocket(payload);
        }

        private void SendControl(VideoControlFlags flags)
        {
            var payload = new Streamer((uint)VideoPayloadType.Control)
            {
                Data = new VideoControl(flags)   
            };

            SendStreamerOnControlSocket(payload);
        }
    }
}