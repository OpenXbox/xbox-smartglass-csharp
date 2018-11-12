using System;
using System.Diagnostics;
using SmartGlass.Nano;
using SmartGlass.Nano.Packets;
using SmartGlass.Nano.Consumer;

namespace SmartGlass.Nano.Channels
{
    internal class VideoChannel : VideoChannelBase
    {
        public bool HandshakeDone { get; internal set; }
        public Packets.VideoFormat[] AvailableFormats { get; internal set; }
        public Packets.VideoFormat ActiveFormat { get; internal set; }
        public event EventHandler<VideoFormatEventArgs> FeedVideoFormat;
        public event EventHandler<VideoDataEventArgs> FeedVideoData;

        public VideoChannel(NanoClient client)
            : base(client, NanoChannelId.Video)
        {
            HandshakeDone = false;
        }

        public void StartStream()
        {
            var controlData = new VideoControl(
                VideoControlFlags.StartStream | VideoControlFlags.RequestKeyframe);
            SendControl(controlData);
        }

        public void StopStream()
        {
            var controlData = new VideoControl(VideoControlFlags.StopStream);
            SendControl(controlData);
        }

        public void ReportLostFrames(uint firstFrame, uint lastFrame)
        {
            var controlData = new VideoControl(
                flags: VideoControlFlags.RequestKeyframe | VideoControlFlags.LostFrames,
                firstLostFrame: firstFrame,
                lastLostFrame: lastFrame
            );
            SendControl(controlData);
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

            FeedVideoFormat?.Invoke(this,
                new VideoFormatEventArgs(ActiveFormat));
        }

        public override void OnControl(VideoControl control)
        {
            throw new NotSupportedException("Control message on client side");
        }

        public override void OnData(VideoData data)
        {
            FeedVideoData?.Invoke(this,
                new VideoDataEventArgs(data));
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

        private void SendControl(VideoControl controlData)
        {
            var payload = new Streamer((uint)VideoPayloadType.Control)
            {
                Data = controlData
            };

            SendStreamerOnControlSocket(payload);
        }
    }
}