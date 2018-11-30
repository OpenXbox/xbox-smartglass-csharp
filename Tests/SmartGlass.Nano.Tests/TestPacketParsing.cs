using System;
using System.IO;
using System.Collections.Generic;
using SmartGlass.Common;
using SmartGlass.Nano.Packets;
using Xunit;

namespace SmartGlass.Nano.Tests
{
    public class TestPacketParsing
    {
        private NanoChannelContext _context { get; set; }

        public TestPacketParsing()
        {
            _context = new NanoChannelContext();
            _context.RegisterChannel(1024, NanoChannel.Video);
            _context.RegisterChannel(1025, NanoChannel.Audio);
            _context.RegisterChannel(1026, NanoChannel.ChatAudio);
            _context.RegisterChannel(1027, NanoChannel.Control);
            _context.RegisterChannel(1028, NanoChannel.Input);
            _context.RegisterChannel(1029, NanoChannel.InputFeedback);
        }

        [Fact]
        public void TestRtpHeader()
        {
            BEReader reader = new BEReader(TestData["tcp_control_handshake.bin"]);
            RtpHeader header = new RtpHeader();
            header.Deserialize(reader);

            Assert.NotNull(header);
            Assert.True(header.Padding);
            Assert.False(header.Extension);
            Assert.Equal(0, header.CsrcCount);
            Assert.False(header.Marker);
            Assert.Equal(NanoPayloadType.ControlHandshake, header.PayloadType);
            Assert.Equal(0, header.SequenceNumber);
            Assert.Equal(2847619159, header.Timestamp);
            Assert.Equal(0, header.ConnectionId);
            Assert.Equal(0, header.ChannelId);
        }

        [Fact]
        public void TestUnknownChannelParse()
        {
            // Create channel context without any registered channels
            NanoChannelContext localContext = new NanoChannelContext();

            byte[] packetData = TestData["tcp_channel_open_no_flags.bin"];
            Assert.Throws<NanoPackingException>(() =>
            {
                NanoPacketFactory
                    .ParsePacket(packetData, localContext);
            });
        }

        [Fact]
        public void TestControlHandshake()
        {
            ControlHandshake packet = NanoPacketFactory
                .ParsePacket(TestData["tcp_control_handshake.bin"], _context)
                    as ControlHandshake;

            Assert.NotNull(packet);
            Assert.NotNull(packet.Header);
            Assert.Equal(NanoChannel.TcpBase, packet.Channel);
            Assert.Equal(ControlHandshakeType.SYN, packet.Type);
            Assert.Equal(40084, packet.ConnectionId);
        }

        [Fact]
        public void TestChannelCreate()
        {
            ChannelCreate packet = NanoPacketFactory
                .ParsePacket(TestData["tcp_channel_create.bin"], _context)
                    as ChannelCreate;

            Assert.NotNull(packet);
            Assert.NotNull(packet.Header);
            Assert.Equal(NanoChannel.Video, packet.Channel);
            Assert.Equal(ChannelControlType.Create, packet.Type);
            Assert.Equal(NanoChannelClass.Video, packet.Name);
            Assert.Equal(0, packet.Flags);
        }

        [Fact]
        public void TestChannelOpenNoFlags()
        {
            ChannelOpen packet = NanoPacketFactory
                .ParsePacket(TestData["tcp_channel_open_no_flags.bin"], _context)
                    as ChannelOpen;

            Assert.NotNull(packet);
            Assert.NotNull(packet.Header);
            Assert.Equal(NanoChannel.Video, packet.Channel);
            Assert.Equal(ChannelControlType.Open, packet.Type);
            Assert.Equal(new byte[0], packet.Flags);
        }

        [Fact]
        public void TestChannelOpenWithFlags()
        {
            ChannelOpen packet = NanoPacketFactory
                .ParsePacket(TestData["tcp_channel_open_with_flags.bin"], _context)
                    as ChannelOpen;

            Assert.NotNull(packet);
            Assert.NotNull(packet.Header);
            Assert.Equal(NanoChannel.Control, packet.Channel);
            Assert.Equal(ChannelControlType.Open, packet.Type);
            Assert.Equal(new byte[] { 0x01, 0x00, 0x02, 0x00 }, packet.Flags);
        }

        [Fact]
        public void TestChannelClose()
        {
            ChannelClose packet = NanoPacketFactory
                .ParsePacket(TestData["tcp_channel_close.bin"], _context)
                    as ChannelClose;

            Assert.NotNull(packet);
            Assert.NotNull(packet.Header);
            Assert.Equal(NanoChannel.Audio, packet.Channel);
            Assert.Equal(ChannelControlType.Close, packet.Type);
            Assert.Equal(0, packet.Flags);
        }

        [Fact]
        public void TestUdpHandshake()
        {
            UdpHandshake packet = NanoPacketFactory
                .ParsePacket(TestData["udp_handshake.bin"], _context)
                    as UdpHandshake;

            Assert.NotNull(packet);
            Assert.NotNull(packet.Header);
            Assert.Equal(NanoChannel.TcpBase, packet.Channel);
            Assert.Equal(ControlHandshakeType.ACK, packet.Type);
        }

        [Fact]
        public void TestStreamerControlHeader()
        {
            RealtimeTelemetry packet = NanoPacketFactory
               .ParsePacket(TestData["tcp_control_msg_with_header_realtime_telemetry.bin"], _context)
                   as RealtimeTelemetry;

            Assert.NotNull(packet);
            Assert.NotNull(packet.Header);

            Assert.Equal(NanoChannel.Control, packet.Channel);
            Assert.Equal(NanoPayloadType.Streamer, packet.Header.PayloadType);

            Assert.Equal(0, packet.StreamerHeader.PacketType);
            Assert.Equal(StreamerFlags.GotSeqAndPrev | StreamerFlags.Unknown1,
                            packet.StreamerHeader.Flags);
            Assert.Equal(1, packet.StreamerHeader.SequenceNumber);
            Assert.Equal(0, packet.StreamerHeader.PreviousSequenceNumber);

            Assert.Equal(0, packet.ControlHeader.PreviousSequence);
            Assert.Equal(1, packet.ControlHeader.Unknown1);
            Assert.Equal(1406, packet.ControlHeader.Unknown2);
            Assert.Equal(ControlOpCode.RealtimeTelemetry, packet.ControlHeader.OpCode);
        }

        [Fact]
        public void TestStreamerControlChangeVideoQuality()
        {
            ChangeVideoQuality packet = NanoPacketFactory
                .ParsePacket(TestData["tcp_control_msg_with_header_change_video_quality.bin"], _context)
                    as ChangeVideoQuality;

            Assert.NotNull(packet);
            Assert.NotNull(packet.Header);

            Assert.Equal(NanoChannel.Control, packet.Channel);
            Assert.Equal(NanoPayloadType.Streamer, packet.Header.PayloadType);

            Assert.Equal(0, packet.StreamerHeader.PacketType);
            Assert.Equal(StreamerFlags.GotSeqAndPrev | StreamerFlags.Unknown1,
                            packet.StreamerHeader.Flags);
            Assert.Equal(1, packet.StreamerHeader.SequenceNumber);
            Assert.Equal(0, packet.StreamerHeader.PreviousSequenceNumber);

            Assert.Equal(0, packet.ControlHeader.PreviousSequence);
            Assert.Equal(1, packet.ControlHeader.Unknown1);
            Assert.Equal(1406, packet.ControlHeader.Unknown2);
            Assert.Equal(ControlOpCode.ChangeVideoQuality, packet.ControlHeader.OpCode);
        }

        [Fact]
        public void TestAudioClientHandshake()
        {
            AudioClientHandshake packet = NanoPacketFactory
                .ParsePacket(TestData["tcp_audio_client_handshake.bin"], _context)
                    as AudioClientHandshake;

            Assert.NotNull(packet);
            Assert.NotNull(packet.Header);

            Assert.Equal(NanoChannel.Audio, packet.Channel);
            Assert.Equal(NanoPayloadType.Streamer, packet.Header.PayloadType);

            Assert.Equal(AudioPayloadType.ClientHandshake,
                            (AudioPayloadType)packet.StreamerHeader.PacketType);
            Assert.Equal(StreamerFlags.GotSeqAndPrev | StreamerFlags.Unknown1,
                            packet.StreamerHeader.Flags);
            Assert.Equal(1, packet.StreamerHeader.SequenceNumber);
            Assert.Equal(0, packet.StreamerHeader.PreviousSequenceNumber);

            Assert.Equal(693041842, packet.InitialFrameID);
            Assert.Equal(2, packet.RequestedFormat.Channels);
            Assert.Equal(48000, packet.RequestedFormat.SampleRate);
            Assert.Equal(AudioCodec.AAC, packet.RequestedFormat.Codec);
        }

        [Fact]
        public void TestAudioServerHandshake()
        {
            AudioServerHandshake packet = NanoPacketFactory
                .ParsePacket(TestData["tcp_audio_server_handshake.bin"], _context)
                    as AudioServerHandshake;

            Assert.NotNull(packet);
            Assert.NotNull(packet.Header);

            Assert.Equal(NanoChannel.Audio, packet.Channel);
            Assert.Equal(NanoPayloadType.Streamer, packet.Header.PayloadType);

            Assert.Equal(AudioPayloadType.ServerHandshake,
                            (AudioPayloadType)packet.StreamerHeader.PacketType);
            Assert.Equal(StreamerFlags.GotSeqAndPrev | StreamerFlags.Unknown1,
                            packet.StreamerHeader.Flags);
            Assert.Equal(1, packet.StreamerHeader.SequenceNumber);
            Assert.Equal(0, packet.StreamerHeader.PreviousSequenceNumber);

            Assert.Equal(4, packet.ProtocolVersion);
            Assert.Equal(1495315092424, packet.ReferenceTimestamp);
            Assert.Equal(1, packet.Formats.Length);

            Assert.Equal(2, packet.Formats[0].Channels);
            Assert.Equal(48000, packet.Formats[0].SampleRate);
            Assert.Equal(AudioCodec.AAC, packet.Formats[0].Codec);
        }

        [Fact]
        public void TestAudioControl()
        {
            AudioControl packet = NanoPacketFactory
                .ParsePacket(TestData["tcp_audio_control.bin"], _context)
                    as AudioControl;

            Assert.NotNull(packet);
            Assert.NotNull(packet.Header);

            Assert.Equal(NanoChannel.Audio, packet.Channel);
            Assert.Equal(NanoPayloadType.Streamer, packet.Header.PayloadType);

            Assert.Equal(AudioPayloadType.Control,
                            (AudioPayloadType)packet.StreamerHeader.PacketType);
            Assert.Equal(StreamerFlags.GotSeqAndPrev | StreamerFlags.Unknown1,
                            packet.StreamerHeader.Flags);
            Assert.Equal(2, packet.StreamerHeader.SequenceNumber);
            Assert.Equal(1, packet.StreamerHeader.PreviousSequenceNumber);

            Assert.Equal(AudioControlFlags.StartStream, packet.Flags);
        }

        [Fact]
        public void TestAudioData()
        {
            AudioData packet = NanoPacketFactory
                .ParsePacket(TestData["udp_audio_data.bin"], _context)
                    as AudioData;

            Assert.NotNull(packet);
            Assert.NotNull(packet.Header);

            Assert.Equal(NanoChannel.Audio, packet.Channel);
            Assert.Equal(NanoPayloadType.Streamer, packet.Header.PayloadType);

            Assert.Equal(AudioPayloadType.Data,
                            (AudioPayloadType)packet.StreamerHeader.PacketType);
            Assert.Equal((StreamerFlags)0, packet.StreamerHeader.Flags);

            Assert.Equal(4, packet.Flags);
            Assert.Equal(0, packet.FrameId);
            Assert.Equal(3365588462, packet.Timestamp);
            Assert.Equal(357, packet.Data.Length);
        }

        [Fact]
        public void TestInputClientHandshake()
        {
            InputClientHandshake packet = NanoPacketFactory
               .ParsePacket(TestData["tcp_input_client_handshake.bin"], _context)
                   as InputClientHandshake;

            Assert.NotNull(packet);
            Assert.NotNull(packet.Header);

            Assert.Equal(NanoChannel.Input, packet.Channel);
            Assert.Equal(NanoPayloadType.Streamer, packet.Header.PayloadType);

            Assert.Equal(InputPayloadType.ClientHandshake,
                            (InputPayloadType)packet.StreamerHeader.PacketType);
            Assert.Equal(StreamerFlags.GotSeqAndPrev | StreamerFlags.Unknown1,
                            packet.StreamerHeader.Flags);
            Assert.Equal(1, packet.StreamerHeader.SequenceNumber);
            Assert.Equal(0, packet.StreamerHeader.PreviousSequenceNumber);

            Assert.Equal(10, packet.MaxTouches);
            Assert.Equal(1498690645999, packet.ReferenceTimestamp);
        }

        [Fact]
        public void TestInputServerHandshake()
        {
            InputServerHandshake packet = NanoPacketFactory
               .ParsePacket(TestData["tcp_input_server_handshake.bin"], _context)
                   as InputServerHandshake;

            Assert.NotNull(packet);
            Assert.NotNull(packet.Header);

            Assert.Equal(NanoChannel.Input, packet.Channel);
            Assert.Equal(NanoPayloadType.Streamer, packet.Header.PayloadType);

            Assert.Equal(InputPayloadType.ServerHandshake,
                            (InputPayloadType)packet.StreamerHeader.PacketType);
            Assert.Equal(StreamerFlags.GotSeqAndPrev | StreamerFlags.Unknown1,
                            packet.StreamerHeader.Flags);
            Assert.Equal(1, packet.StreamerHeader.SequenceNumber);
            Assert.Equal(0, packet.StreamerHeader.PreviousSequenceNumber);

            Assert.Equal(3, packet.ProtocolVersion);
            Assert.Equal(1280, packet.DesktopWidth);
            Assert.Equal(720, packet.DesktopHeight);
            Assert.Equal(0, packet.MaxTouches);
            Assert.Equal(672208545, packet.InitialFrameId);
        }

        [Fact]
        public void TestInputFrame()
        {
            InputFrame packet = NanoPacketFactory
                .ParsePacket(TestData["udp_input_frame.bin"], _context)
                    as InputFrame;

            Assert.NotNull(packet);
            Assert.NotNull(packet.Header);

            Assert.Equal(NanoChannel.Input, packet.Channel);
            Assert.Equal(NanoPayloadType.Streamer, packet.Header.PayloadType);

            Assert.Equal(InputPayloadType.Frame,
                            (InputPayloadType)packet.StreamerHeader.PacketType);
            Assert.Equal((StreamerFlags)0, packet.StreamerHeader.Flags);

            Assert.Equal(583706495, packet.CreatedTimestamp);
            Assert.Equal(583706515, packet.Timestamp);
            Assert.Equal(672208564, packet.FrameId);
        }

        [Fact]
        public void TestInputFrameAck()
        {
            InputFrameAck packet = NanoPacketFactory
                .ParsePacket(TestData["udp_input_frame_ack.bin"], _context)
                    as InputFrameAck;

            Assert.NotNull(packet);
            Assert.NotNull(packet.Header);

            Assert.Equal(NanoChannel.Input, packet.Channel);
            Assert.Equal(NanoPayloadType.Streamer, packet.Header.PayloadType);

            Assert.Equal(InputPayloadType.FrameAck,
                            (InputPayloadType)packet.StreamerHeader.PacketType);
            Assert.Equal((StreamerFlags)0, packet.StreamerHeader.Flags);

            Assert.Equal(672208545, packet.AckedFrame);
        }

        [Fact]
        public void TestVideoClientHandshake()
        {
            VideoClientHandshake packet = NanoPacketFactory
               .ParsePacket(TestData["tcp_video_client_handshake.bin"], _context)
                   as VideoClientHandshake;

            Assert.NotNull(packet);
            Assert.NotNull(packet.Header);

            Assert.Equal(NanoChannel.Video, packet.Channel);
            Assert.Equal(NanoPayloadType.Streamer, packet.Header.PayloadType);

            Assert.Equal(VideoPayloadType.ClientHandshake,
                            (VideoPayloadType)packet.StreamerHeader.PacketType);
            Assert.Equal(StreamerFlags.GotSeqAndPrev | StreamerFlags.Unknown1,
                            packet.StreamerHeader.Flags);
            Assert.Equal(1, packet.StreamerHeader.SequenceNumber);
            Assert.Equal(0, packet.StreamerHeader.PreviousSequenceNumber);

            Assert.Equal(3715731054, packet.InitialFrameId);
            Assert.Equal(30, packet.RequestedFormat.FPS);
            Assert.Equal(1280, packet.RequestedFormat.Width);
            Assert.Equal(720, packet.RequestedFormat.Height);
            Assert.Equal(VideoCodec.H264, packet.RequestedFormat.Codec);
        }

        [Fact]
        public void TestVideoServerHandshake()
        {
            VideoServerHandshake packet = NanoPacketFactory
                .ParsePacket(TestData["tcp_video_server_handshake.bin"], _context)
                    as VideoServerHandshake;

            Assert.NotNull(packet);
            Assert.NotNull(packet.Header);
            Assert.Equal(NanoChannel.Video, packet.Channel);

            Assert.Equal(NanoPayloadType.Streamer, packet.Header.PayloadType);

            Assert.Equal(StreamerFlags.GotSeqAndPrev | StreamerFlags.Unknown1,
                            packet.StreamerHeader.Flags);
            Assert.Equal(1, packet.StreamerHeader.SequenceNumber);
            Assert.Equal(0, packet.StreamerHeader.PreviousSequenceNumber);
            Assert.Equal(VideoPayloadType.ServerHandshake, (VideoPayloadType)packet.StreamerHeader.PacketType);

            Assert.Equal(5, packet.ProtocolVersion);
            Assert.Equal(1280, packet.Width);
            Assert.Equal(720, packet.Height);
            Assert.Equal(30, packet.FPS);
            Assert.Equal(1495315092425, packet.ReferenceTimestamp);
            Assert.Equal(4, packet.Formats.Length);

            Assert.Equal(30, packet.Formats[0].FPS);
            Assert.Equal(1280, packet.Formats[0].Width);
            Assert.Equal(720, packet.Formats[0].Height);
            Assert.Equal(VideoCodec.H264, packet.Formats[0].Codec);

            Assert.Equal(30, packet.Formats[1].FPS);
            Assert.Equal(960, packet.Formats[1].Width);
            Assert.Equal(540, packet.Formats[1].Height);
            Assert.Equal(VideoCodec.H264, packet.Formats[1].Codec);

            Assert.Equal(30, packet.Formats[2].FPS);
            Assert.Equal(640, packet.Formats[2].Width);
            Assert.Equal(360, packet.Formats[2].Height);
            Assert.Equal(VideoCodec.H264, packet.Formats[2].Codec);

            Assert.Equal(30, packet.Formats[3].FPS);
            Assert.Equal(320, packet.Formats[3].Width);
            Assert.Equal(180, packet.Formats[3].Height);
            Assert.Equal(VideoCodec.H264, packet.Formats[3].Codec);
        }

        [Fact]
        public void TestVideoControl()
        {
            VideoControl packet = NanoPacketFactory
                .ParsePacket(TestData["tcp_video_control.bin"], _context)
                    as VideoControl;

            Assert.NotNull(packet);
            Assert.NotNull(packet.Header);

            Assert.Equal(NanoChannel.Video, packet.Channel);
            Assert.Equal(NanoPayloadType.Streamer, packet.Header.PayloadType);

            Assert.Equal(VideoPayloadType.Control,
                            (VideoPayloadType)packet.StreamerHeader.PacketType);
            Assert.Equal(StreamerFlags.GotSeqAndPrev | StreamerFlags.Unknown1,
                            packet.StreamerHeader.Flags);
            Assert.Equal(2, packet.StreamerHeader.SequenceNumber);
            Assert.Equal(1, packet.StreamerHeader.PreviousSequenceNumber);

            Assert.Equal(VideoControlFlags.RequestKeyframe | VideoControlFlags.StartStream,
                            packet.Flags);
        }

        [Fact]
        public void TestVideoData()
        {
            VideoData packet = NanoPacketFactory
                .ParsePacket(TestData["udp_video_data.bin"], _context)
                    as VideoData;

            Assert.NotNull(packet);
            Assert.NotNull(packet.Header);

            Assert.Equal(NanoChannel.Video, packet.Channel);
            Assert.Equal(NanoPayloadType.Streamer, packet.Header.PayloadType);

            Assert.Equal(VideoPayloadType.Data,
                            (VideoPayloadType)packet.StreamerHeader.PacketType);
            Assert.Equal((StreamerFlags)0, packet.StreamerHeader.Flags);

            Assert.Equal(4, packet.Flags);
            Assert.Equal(3715731054, packet.FrameId);
            Assert.Equal(3365613642, packet.Timestamp);
            Assert.Equal(5594, packet.TotalSize);
            Assert.Equal(5, packet.PacketCount);
            Assert.Equal(0, packet.Offset);
            Assert.Equal(1119, packet.Data.Length);
        }
    }
}