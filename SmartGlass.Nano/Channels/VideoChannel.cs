using System;
using System.Diagnostics;
using SmartGlass.Nano;
using SmartGlass.Nano.Packets;
using SmartGlass.Nano.Consumer;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SmartGlass.Common;

namespace SmartGlass.Nano.Channels
{
    public class VideoChannel : StreamingChannel, IStreamingChannel
    {
        private static readonly ILogger logger = Logging.Factory.CreateLogger<VideoChannel>();
        public override NanoChannel Channel => NanoChannel.Video;
        public override int ProtocolVersion => 5;
        public uint FPS { get; private set; }
        public uint Width { get; private set; }
        public uint Height { get; private set; }
        public Packets.VideoFormat[] AvailableFormats { get; internal set; }
        public Packets.VideoFormat ActiveFormat { get; internal set; }
        public event EventHandler<VideoDataEventArgs> FeedVideoData;

        internal VideoChannel(NanoRdpTransport transport, byte[] flags)
            : base(transport, flags)
        {
            MessageReceived += OnMessage;
        }

        public void OnMessage(object sender, MessageReceivedEventArgs<INanoPacket> args)
        {
            IStreamerMessage packet = args.Message as IStreamerMessage;
            if (packet == null)
            {
                logger.LogTrace($"Not handling packet {args.Message.Header.PayloadType}");
                return;
            }

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
            await SendAsync(controlData);
        }

        public async Task StopStreamAsync()
        {
            var controlData = new VideoControl(VideoControlFlags.StopStream);
            await SendAsync(controlData);
        }

        public async Task ReportLostFramesAsync(uint firstFrame, uint lastFrame)
        {
            var controlData = new VideoControl(
                flags: VideoControlFlags.RequestKeyframe | VideoControlFlags.LostFrames,
                firstLostFrame: firstFrame,
                lastLostFrame: lastFrame
            );
            await SendAsync(controlData);
        }

        public void OnControl(VideoControl control)
        {
            throw new NotSupportedException("Control message on client side");
        }

        public void OnData(VideoData data)
        {
            if (data.FrameId > (base.FrameId + 1))
            {
                uint lostFrameCount = data.FrameId - base.FrameId;
                logger.LogTrace($"Requesting lost frames, frame count: {lostFrameCount}");
                ReportLostFramesAsync(base.FrameId + 1, data.FrameId - 1)
                    .GetAwaiter().GetResult();
            }

            if (data.FrameId > FrameId)
                base.FrameId = data.FrameId;

            FeedVideoData?.Invoke(this,
                new VideoDataEventArgs(data));
        }

        public async Task SendClientHandshakeAsync(VideoFormat format)
        {
            var packet = new VideoClientHandshake(FrameId, format);
            await SendAsync(packet);
        }

        public async Task OpenAsync()
        {
            var handshake = await WaitForMessageAsync<VideoServerHandshake>(
                TimeSpan.FromSeconds(1),
                async () => await _transport.SendChannelOpen(Channel, Flags)
            );

            if (handshake.ProtocolVersion != ProtocolVersion)
                throw new NanoException("VideoChannel: Protocol version mismatch!");

            FPS = handshake.FPS;
            Width = handshake.Width;
            Height = handshake.Height;
            ReferenceTimestampUlong = handshake.ReferenceTimestamp;
            AvailableFormats = handshake.Formats;
        }
    }
}