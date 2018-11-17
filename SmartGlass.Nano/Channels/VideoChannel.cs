using System;
using System.Diagnostics;
using SmartGlass.Nano;
using SmartGlass.Nano.Packets;
using SmartGlass.Nano.Consumer;

namespace SmartGlass.Nano.Channels
{
    public class VideoChannel : VideoChannelBase
    {
        public Packets.VideoFormat[] AvailableFormats { get; internal set; }
        public Packets.VideoFormat ActiveFormat { get; internal set; }
        public event EventHandler<VideoFormatEventArgs> FeedVideoFormat;
        public event EventHandler<VideoDataEventArgs> FeedVideoData;

        public VideoChannel(NanoClient client)
            : base(client, NanoChannel.Video)
        {
        }

        public void StartStream()
        {
            var controlData = new VideoControl(
                VideoControlFlags.StartStream | VideoControlFlags.RequestKeyframe);
            SendStreamerOnControlSocket(controlData);
        }

        public void StopStream()
        {
            var controlData = new VideoControl(VideoControlFlags.StopStream);
            SendStreamerOnControlSocket(controlData);
        }

        public void ReportLostFrames(uint firstFrame, uint lastFrame)
        {
            var controlData = new VideoControl(
                flags: VideoControlFlags.RequestKeyframe | VideoControlFlags.LostFrames,
                firstLostFrame: firstFrame,
                lastLostFrame: lastFrame
            );
            SendStreamerOnControlSocket(controlData);
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
            HandshakeComplete = true;

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
            var packet = new VideoClientHandshake(initialFrameId, format);

            SendStreamerOnControlSocket(packet);
        }
    }
}