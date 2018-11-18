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
                case VideoPayloadType.ClientHandshake:
                    OnClientHandshake((VideoClientHandshake)packet);
                    break;
                case VideoPayloadType.ServerHandshake:
                    OnServerHandshake((VideoServerHandshake)packet);
                    break;
                case VideoPayloadType.Control:
                    OnControl((VideoControl)packet);
                    break;
                case VideoPayloadType.Data:
                    OnData((VideoData)packet);
                    break;
            }
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

        public void OnClientHandshake(VideoClientHandshake handshake)
        {
            throw new NotSupportedException("Client handshake on client side");
        }

        public void OnServerHandshake(VideoServerHandshake handshake)
        {
            AvailableFormats = handshake.Formats;
            ActiveFormat = AvailableFormats[0];
            SendClientHandshake(ActiveFormat);

            FeedVideoFormat?.Invoke(this,
                new VideoFormatEventArgs(ActiveFormat));
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

        private void SendClientHandshake(VideoFormat format)
        {
            uint initialFrameId = GenerateInitialFrameId();
            var packet = new VideoClientHandshake(initialFrameId, format);

            SendStreamerOnControlSocket(packet);
        }

        public async Task OpenAsync()
        {
            var handshake = await _client.WaitForMessageAsync<VideoServerHandshake>(
                TimeSpan.FromSeconds(1),
                async () => await SendChannelOpenAsync()
            );

            AvailableFormats = handshake.Formats;
        }
    }
}