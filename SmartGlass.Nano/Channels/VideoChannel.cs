using System;
using System.Diagnostics;
using SmartGlass.Nano;
using SmartGlass.Nano.Packets;
using SmartGlass.Nano.Consumer;
using System.Threading.Tasks;

namespace SmartGlass.Nano.Channels
{
    public class VideoChannel : StreamingChannel, IStreamingChannel
    {
        public override NanoChannel Channel => NanoChannel.Video;
        public Packets.VideoFormat[] AvailableFormats { get; internal set; }
        public Packets.VideoFormat ActiveFormat { get; internal set; }
        public event EventHandler<VideoFormatEventArgs> FeedVideoFormat;
        public event EventHandler<VideoDataEventArgs> FeedVideoData;

        public void OnPacket(IStreamerMessage packet)
        {
            switch ((VideoPayloadType)packet.StreamerHeader.PacketType)
            {
                case VideoPayloadType.Control:
                    OnControl((VideoControl)packet);
                    break;
                case VideoPayloadType.Data:
                    OnData((VideoData)packet);
                    break;
            }
        }

        public async Task StartStreamAsync()
        {
            var controlData = new VideoControl(
                VideoControlFlags.StartStream | VideoControlFlags.RequestKeyframe);
            await SendStreamerOnControlSocket(controlData);
        }

        public async Task StopStreamAsync()
        {
            var controlData = new VideoControl(VideoControlFlags.StopStream);
            await SendStreamerOnControlSocket(controlData);
        }

        public async Task ReportLostFramesAsync(uint firstFrame, uint lastFrame)
        {
            var controlData = new VideoControl(
                flags: VideoControlFlags.RequestKeyframe | VideoControlFlags.LostFrames,
                firstLostFrame: firstFrame,
                lastLostFrame: lastFrame
            );
            await SendStreamerOnControlSocket(controlData);
        }

        public void OnControl(VideoControl control)
        {
            throw new NotSupportedException("Control message on client side");
        }

        public void OnData(VideoData data)
        {
            FeedVideoData?.Invoke(this,
                new VideoDataEventArgs(data));
        }

        public async Task SendClientHandshakeAsync(VideoFormat format)
        {
            uint initialFrameId = GenerateInitialFrameId();
            var packet = new VideoClientHandshake(initialFrameId, format);

            await SendStreamerOnControlSocket(packet);
        }

        public async Task OpenAsync()
        {
            var handshake = await _client.WaitForMessageAsync<VideoServerHandshake>(
                TimeSpan.FromSeconds(1),
                async () => await SendChannelOpenAsync(Channel)
            );

            AvailableFormats = handshake.Formats;
        }
    }
}