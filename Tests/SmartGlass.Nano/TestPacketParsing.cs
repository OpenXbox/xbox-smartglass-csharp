using System.IO;
using System.Collections.Generic;
using Xunit;

using SmartGlass.Nano;
using SmartGlass.Common;
using SmartGlass.Nano.Packets;
using Tests.Resources;

namespace Tests.SmartGlass.Nano
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
            BEReader reader = new BEReader(ResourcesProvider.GetBytes("tcp_control_handshake.bin", ResourceType.Nano));
            RtpHeader header = new RtpHeader();
            header.Deserialize(reader);

            Assert.NotNull(header);
            Assert.True(header.Padding);
            Assert.False(header.Extension);
            Assert.Equal<int>(0, header.CsrcCount);
            Assert.False(header.Marker);
            Assert.Equal<NanoPayloadType>(NanoPayloadType.ControlHandshake, header.PayloadType);
            Assert.Equal<ushort>(0, header.SequenceNumber);
            Assert.Equal<uint>(2847619159, header.Timestamp);
            Assert.Equal<ushort>(0, header.ConnectionId);
            Assert.Equal<ushort>(0, header.ChannelId);
        }

        [Fact]
        public void TestUnknownChannelParse()
        {
            // Create channel context without any registered channels
            NanoChannelContext localContext = new NanoChannelContext();

            byte[] packetData = ResourcesProvider.GetBytes("tcp_channel_open_no_flags.bin", ResourceType.Nano);
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
                .ParsePacket(ResourcesProvider.GetBytes("tcp_control_handshake.bin", ResourceType.Nano), _context)
                    as ControlHandshake;

            Assert.NotNull(packet);
            Assert.NotNull(packet.Header);
            Assert.Equal<NanoChannel>(NanoChannel.TcpBase, packet.Channel);
            Assert.Equal<ControlHandshakeType>(ControlHandshakeType.SYN, packet.Type);
            Assert.Equal<ushort>(40084, packet.ConnectionId);
        }

        [Fact]
        public void TestChannelCreate()
        {
            ChannelCreate packet = NanoPacketFactory
                .ParsePacket(ResourcesProvider.GetBytes("tcp_channel_create.bin", ResourceType.Nano), _context)
                    as ChannelCreate;

            Assert.NotNull(packet);
            Assert.NotNull(packet.Header);
            Assert.Equal<NanoChannel>(NanoChannel.Video, packet.Channel);
            Assert.Equal<ChannelControlType>(ChannelControlType.Create, packet.Type);
            Assert.Equal(NanoChannelClass.Video, packet.Name);
            Assert.Equal<uint>(0, packet.Flags);
        }

        [Fact]
        public void TestChannelOpenNoFlags()
        {
            ChannelOpen packet = NanoPacketFactory
                .ParsePacket(ResourcesProvider.GetBytes("tcp_channel_open_no_flags.bin", ResourceType.Nano), _context)
                    as ChannelOpen;

            Assert.NotNull(packet);
            Assert.NotNull(packet.Header);
            Assert.Equal<NanoChannel>(NanoChannel.Video, packet.Channel);
            Assert.Equal<ChannelControlType>(ChannelControlType.Open, packet.Type);
            Assert.Equal<byte[]>(new byte[0], packet.Flags);
        }

        [Fact]
        public void TestChannelOpenWithFlags()
        {
            ChannelOpen packet = NanoPacketFactory
                .ParsePacket(ResourcesProvider.GetBytes("tcp_channel_open_with_flags.bin", ResourceType.Nano), _context)
                    as ChannelOpen;

            Assert.NotNull(packet);
            Assert.NotNull(packet.Header);
            Assert.Equal<NanoChannel>(NanoChannel.Control, packet.Channel);
            Assert.Equal<ChannelControlType>(ChannelControlType.Open, packet.Type);
            Assert.Equal<byte[]>(new byte[] { 0x01, 0x00, 0x02, 0x00 }, packet.Flags);
        }

        [Fact]
        public void TestChannelClose()
        {
            ChannelClose packet = NanoPacketFactory
                .ParsePacket(ResourcesProvider.GetBytes("tcp_channel_close.bin", ResourceType.Nano), _context)
                    as ChannelClose;

            Assert.NotNull(packet);
            Assert.NotNull(packet.Header);
            Assert.Equal<NanoChannel>(NanoChannel.Audio, packet.Channel);
            Assert.Equal<ChannelControlType>(ChannelControlType.Close, packet.Type);
            Assert.Equal<uint>(0, packet.Flags);
        }

        [Fact]
        public void TestUdpHandshake()
        {
            UdpHandshake packet = NanoPacketFactory
                .ParsePacket(ResourcesProvider.GetBytes("udp_handshake.bin", ResourceType.Nano), _context)
                    as UdpHandshake;

            Assert.NotNull(packet);
            Assert.NotNull(packet.Header);
            Assert.Equal<NanoChannel>(NanoChannel.TcpBase, packet.Channel);
            Assert.Equal<ControlHandshakeType>(ControlHandshakeType.ACK, packet.Type);
        }

        [Fact]
        public void TestStreamerControlHeader()
        {
            RealtimeTelemetry packet = NanoPacketFactory
               .ParsePacket(ResourcesProvider.GetBytes("tcp_control_msg_with_header_realtime_telemetry.bin", ResourceType.Nano), _context)
                   as RealtimeTelemetry;

            Assert.NotNull(packet);
            Assert.NotNull(packet.Header);

            Assert.Equal<NanoChannel>(NanoChannel.Control, packet.Channel);
            Assert.Equal<NanoPayloadType>(NanoPayloadType.Streamer, packet.Header.PayloadType);

            Assert.Equal<uint>(0, packet.StreamerHeader.PacketType);
            Assert.Equal<StreamerFlags>(StreamerFlags.GotSeqAndPrev | StreamerFlags.Unknown1,
                            packet.StreamerHeader.Flags);
            Assert.Equal<uint>(1, packet.StreamerHeader.SequenceNumber);
            Assert.Equal<uint>(0, packet.StreamerHeader.PreviousSequenceNumber);

            Assert.Equal<uint>(0, packet.ControlHeader.PreviousSequence);
            Assert.Equal<ushort>(1, packet.ControlHeader.Unknown1);
            Assert.Equal<ushort>(1406, packet.ControlHeader.Unknown2);
            Assert.Equal<ControlOpCode>(ControlOpCode.RealtimeTelemetry, packet.ControlHeader.OpCode);
        }

        [Fact]
        public void TestStreamerControlChangeVideoQuality()
        {
            ChangeVideoQuality packet = NanoPacketFactory
                .ParsePacket(ResourcesProvider.GetBytes("tcp_control_msg_with_header_change_video_quality.bin", ResourceType.Nano), _context)
                    as ChangeVideoQuality;

            Assert.NotNull(packet);
            Assert.NotNull(packet.Header);

            Assert.Equal<NanoChannel>(NanoChannel.Control, packet.Channel);
            Assert.Equal<NanoPayloadType>(NanoPayloadType.Streamer, packet.Header.PayloadType);

            Assert.Equal<uint>(0, packet.StreamerHeader.PacketType);
            Assert.Equal<StreamerFlags>(StreamerFlags.GotSeqAndPrev | StreamerFlags.Unknown1,
                            packet.StreamerHeader.Flags);
            Assert.Equal<uint>(1, packet.StreamerHeader.SequenceNumber);
            Assert.Equal<uint>(0, packet.StreamerHeader.PreviousSequenceNumber);

            Assert.Equal<uint>(0, packet.ControlHeader.PreviousSequence);
            Assert.Equal<ushort>(1, packet.ControlHeader.Unknown1);
            Assert.Equal<ushort>(1406, packet.ControlHeader.Unknown2);
            Assert.Equal<ControlOpCode>(ControlOpCode.ChangeVideoQuality, packet.ControlHeader.OpCode);
        }

        [Fact]
        public void TestAudioClientHandshake()
        {
            AudioClientHandshake packet = NanoPacketFactory
                .ParsePacket(ResourcesProvider.GetBytes("tcp_audio_client_handshake.bin", ResourceType.Nano), _context)
                    as AudioClientHandshake;

            Assert.NotNull(packet);
            Assert.NotNull(packet.Header);

            Assert.Equal<NanoChannel>(NanoChannel.Audio, packet.Channel);
            Assert.Equal<NanoPayloadType>(NanoPayloadType.Streamer, packet.Header.PayloadType);

            Assert.Equal<AudioPayloadType>(AudioPayloadType.ClientHandshake,
                            (AudioPayloadType)packet.StreamerHeader.PacketType);
            Assert.Equal<StreamerFlags>(StreamerFlags.GotSeqAndPrev | StreamerFlags.Unknown1,
                            packet.StreamerHeader.Flags);
            Assert.Equal<uint>(1, packet.StreamerHeader.SequenceNumber);
            Assert.Equal<uint>(0, packet.StreamerHeader.PreviousSequenceNumber);

            Assert.Equal<uint>(693041842, packet.InitialFrameID);
            Assert.Equal<uint>(2, packet.RequestedFormat.Channels);
            Assert.Equal<uint>(48000, packet.RequestedFormat.SampleRate);
            Assert.Equal<AudioCodec>(AudioCodec.AAC, packet.RequestedFormat.Codec);
        }

        [Fact]
        public void TestAudioServerHandshake()
        {
            AudioServerHandshake packet = NanoPacketFactory
                .ParsePacket(ResourcesProvider.GetBytes("tcp_audio_server_handshake.bin", ResourceType.Nano), _context)
                    as AudioServerHandshake;

            Assert.NotNull(packet);
            Assert.NotNull(packet.Header);

            Assert.Equal<NanoChannel>(NanoChannel.Audio, packet.Channel);
            Assert.Equal<NanoPayloadType>(NanoPayloadType.Streamer, packet.Header.PayloadType);

            Assert.Equal<AudioPayloadType>(AudioPayloadType.ServerHandshake,
                            (AudioPayloadType)packet.StreamerHeader.PacketType);
            Assert.Equal<StreamerFlags>(StreamerFlags.GotSeqAndPrev | StreamerFlags.Unknown1,
                            packet.StreamerHeader.Flags);
            Assert.Equal<uint>(1, packet.StreamerHeader.SequenceNumber);
            Assert.Equal<uint>(0, packet.StreamerHeader.PreviousSequenceNumber);

            Assert.Equal<uint>(4, packet.ProtocolVersion);
            Assert.Equal<ulong>(1495315092424, packet.ReferenceTimestamp);
            Assert.Single(packet.Formats);

            Assert.Equal<uint>(2, packet.Formats[0].Channels);
            Assert.Equal<uint>(48000, packet.Formats[0].SampleRate);
            Assert.Equal<AudioCodec>(AudioCodec.AAC, packet.Formats[0].Codec);
        }

        [Fact]
        public void TestAudioControl()
        {
            AudioControl packet = NanoPacketFactory
                .ParsePacket(ResourcesProvider.GetBytes("tcp_audio_control.bin", ResourceType.Nano), _context)
                    as AudioControl;

            Assert.NotNull(packet);
            Assert.NotNull(packet.Header);

            Assert.Equal<NanoChannel>(NanoChannel.Audio, packet.Channel);
            Assert.Equal<NanoPayloadType>(NanoPayloadType.Streamer, packet.Header.PayloadType);

            Assert.Equal<AudioPayloadType>(AudioPayloadType.Control,
                            (AudioPayloadType)packet.StreamerHeader.PacketType);
            Assert.Equal<StreamerFlags>(StreamerFlags.GotSeqAndPrev | StreamerFlags.Unknown1,
                            packet.StreamerHeader.Flags);
            Assert.Equal<uint>(2, packet.StreamerHeader.SequenceNumber);
            Assert.Equal<uint>(1, packet.StreamerHeader.PreviousSequenceNumber);

            Assert.Equal<AudioControlFlags>(AudioControlFlags.StartStream, packet.Flags);
        }

        [Fact]
        public void TestAudioData()
        {
            AudioData packet = NanoPacketFactory
                .ParsePacket(ResourcesProvider.GetBytes("udp_audio_data.bin", ResourceType.Nano), _context)
                    as AudioData;

            Assert.NotNull(packet);
            Assert.NotNull(packet.Header);

            Assert.Equal<NanoChannel>(NanoChannel.Audio, packet.Channel);
            Assert.Equal<NanoPayloadType>(NanoPayloadType.Streamer, packet.Header.PayloadType);

            Assert.Equal<AudioPayloadType>(AudioPayloadType.Data,
                            (AudioPayloadType)packet.StreamerHeader.PacketType);
            Assert.Equal<StreamerFlags>((StreamerFlags)0, packet.StreamerHeader.Flags);

            Assert.Equal<uint>(4, packet.Flags);
            Assert.Equal<uint>(0, packet.FrameId);
            Assert.Equal<ulong>(3365588462, packet.Timestamp);
            Assert.Equal<int>(357, packet.Data.Length);
        }

        [Fact]
        public void TestInputClientHandshake()
        {
            InputClientHandshake packet = NanoPacketFactory
               .ParsePacket(ResourcesProvider.GetBytes("tcp_input_client_handshake.bin", ResourceType.Nano), _context)
                   as InputClientHandshake;

            Assert.NotNull(packet);
            Assert.NotNull(packet.Header);

            Assert.Equal<NanoChannel>(NanoChannel.Input, packet.Channel);
            Assert.Equal<NanoPayloadType>(NanoPayloadType.Streamer, packet.Header.PayloadType);

            Assert.Equal<InputPayloadType>(InputPayloadType.ClientHandshake,
                            (InputPayloadType)packet.StreamerHeader.PacketType);
            Assert.Equal<StreamerFlags>(StreamerFlags.GotSeqAndPrev | StreamerFlags.Unknown1,
                            packet.StreamerHeader.Flags);
            Assert.Equal<uint>(1, packet.StreamerHeader.SequenceNumber);
            Assert.Equal<uint>(0, packet.StreamerHeader.PreviousSequenceNumber);

            Assert.Equal<uint>(10, packet.MaxTouches);
            Assert.Equal<ulong>(1498690645999, packet.ReferenceTimestamp);
        }

        [Fact]
        public void TestInputServerHandshake()
        {
            InputServerHandshake packet = NanoPacketFactory
               .ParsePacket(ResourcesProvider.GetBytes("tcp_input_server_handshake.bin", ResourceType.Nano), _context)
                   as InputServerHandshake;

            Assert.NotNull(packet);
            Assert.NotNull(packet.Header);

            Assert.Equal<NanoChannel>(NanoChannel.Input, packet.Channel);
            Assert.Equal<NanoPayloadType>(NanoPayloadType.Streamer, packet.Header.PayloadType);

            Assert.Equal<InputPayloadType>(InputPayloadType.ServerHandshake,
                            (InputPayloadType)packet.StreamerHeader.PacketType);
            Assert.Equal<StreamerFlags>(StreamerFlags.GotSeqAndPrev | StreamerFlags.Unknown1,
                            packet.StreamerHeader.Flags);
            Assert.Equal<uint>(1, packet.StreamerHeader.SequenceNumber);
            Assert.Equal<uint>(0, packet.StreamerHeader.PreviousSequenceNumber);

            Assert.Equal<uint>(3, packet.ProtocolVersion);
            Assert.Equal<uint>(1280, packet.DesktopWidth);
            Assert.Equal<uint>(720, packet.DesktopHeight);
            Assert.Equal<uint>(0, packet.MaxTouches);
            Assert.Equal<uint>(672208545, packet.InitialFrameId);
        }

        [Fact]
        public void TestInputFrame()
        {
            InputFrame packet = NanoPacketFactory
                .ParsePacket(ResourcesProvider.GetBytes("udp_input_frame.bin", ResourceType.Nano), _context)
                    as InputFrame;

            Assert.NotNull(packet);
            Assert.NotNull(packet.Header);

            Assert.Equal<NanoChannel>(NanoChannel.Input, packet.Channel);
            Assert.Equal<NanoPayloadType>(NanoPayloadType.Streamer, packet.Header.PayloadType);

            Assert.Equal<InputPayloadType>(InputPayloadType.Frame,
                            (InputPayloadType)packet.StreamerHeader.PacketType);
            Assert.Equal<StreamerFlags>((StreamerFlags)0, packet.StreamerHeader.Flags);

            Assert.Equal<ulong>(583706495, packet.CreatedTimestamp);
            Assert.Equal<ulong>(583706515, packet.Timestamp);
            Assert.Equal<uint>(672208564, packet.FrameId);
        }

        [Fact]
        public void TestInputFrameAck()
        {
            InputFrameAck packet = NanoPacketFactory
                .ParsePacket(ResourcesProvider.GetBytes("udp_input_frame_ack.bin", ResourceType.Nano), _context)
                    as InputFrameAck;

            Assert.NotNull(packet);
            Assert.NotNull(packet.Header);

            Assert.Equal<NanoChannel>(NanoChannel.Input, packet.Channel);
            Assert.Equal<NanoPayloadType>(NanoPayloadType.Streamer, packet.Header.PayloadType);

            Assert.Equal<InputPayloadType>(InputPayloadType.FrameAck,
                            (InputPayloadType)packet.StreamerHeader.PacketType);
            Assert.Equal<StreamerFlags>((StreamerFlags)0, packet.StreamerHeader.Flags);

            Assert.Equal<uint>(672208545, packet.AckedFrame);
        }

        [Fact]
        public void TestVideoClientHandshake()
        {
            VideoClientHandshake packet = NanoPacketFactory
               .ParsePacket(ResourcesProvider.GetBytes("tcp_video_client_handshake.bin", ResourceType.Nano), _context)
                   as VideoClientHandshake;

            Assert.NotNull(packet);
            Assert.NotNull(packet.Header);

            Assert.Equal<NanoChannel>(NanoChannel.Video, packet.Channel);
            Assert.Equal<NanoPayloadType>(NanoPayloadType.Streamer, packet.Header.PayloadType);

            Assert.Equal<VideoPayloadType>(VideoPayloadType.ClientHandshake,
                            (VideoPayloadType)packet.StreamerHeader.PacketType);
            Assert.Equal<StreamerFlags>(StreamerFlags.GotSeqAndPrev | StreamerFlags.Unknown1,
                            packet.StreamerHeader.Flags);
            Assert.Equal<uint>(1, packet.StreamerHeader.SequenceNumber);
            Assert.Equal<uint>(0, packet.StreamerHeader.PreviousSequenceNumber);

            Assert.Equal<uint>(3715731054, packet.InitialFrameId);
            Assert.Equal<uint>(30, packet.RequestedFormat.FPS);
            Assert.Equal<uint>(1280, packet.RequestedFormat.Width);
            Assert.Equal<uint>(720, packet.RequestedFormat.Height);
            Assert.Equal<VideoCodec>(VideoCodec.H264, packet.RequestedFormat.Codec);
        }

        [Fact]
        public void TestVideoServerHandshake()
        {
            VideoServerHandshake packet = NanoPacketFactory
                .ParsePacket(ResourcesProvider.GetBytes("tcp_video_server_handshake.bin", ResourceType.Nano), _context)
                    as VideoServerHandshake;

            Assert.NotNull(packet);
            Assert.NotNull(packet.Header);
            Assert.Equal<NanoChannel>(NanoChannel.Video, packet.Channel);

            Assert.Equal<NanoPayloadType>(NanoPayloadType.Streamer, packet.Header.PayloadType);

            Assert.Equal<StreamerFlags>(StreamerFlags.GotSeqAndPrev | StreamerFlags.Unknown1,
                            packet.StreamerHeader.Flags);
            Assert.Equal<uint>(1, packet.StreamerHeader.SequenceNumber);
            Assert.Equal<uint>(0, packet.StreamerHeader.PreviousSequenceNumber);
            Assert.Equal<VideoPayloadType>(VideoPayloadType.ServerHandshake,
                (VideoPayloadType)packet.StreamerHeader.PacketType);

            Assert.Equal<uint>(5, packet.ProtocolVersion);
            Assert.Equal<uint>(1280, packet.Width);
            Assert.Equal<uint>(720, packet.Height);
            Assert.Equal<uint>(30, packet.FPS);
            Assert.Equal<ulong>(1495315092425, packet.ReferenceTimestamp);
            Assert.Equal<int>(4, packet.Formats.Length);

            Assert.Equal<uint>(30, packet.Formats[0].FPS);
            Assert.Equal<uint>(1280, packet.Formats[0].Width);
            Assert.Equal<uint>(720, packet.Formats[0].Height);
            Assert.Equal<VideoCodec>(VideoCodec.H264, packet.Formats[0].Codec);

            Assert.Equal<uint>(30, packet.Formats[1].FPS);
            Assert.Equal<uint>(960, packet.Formats[1].Width);
            Assert.Equal<uint>(540, packet.Formats[1].Height);
            Assert.Equal<VideoCodec>(VideoCodec.H264, packet.Formats[1].Codec);

            Assert.Equal<uint>(30, packet.Formats[2].FPS);
            Assert.Equal<uint>(640, packet.Formats[2].Width);
            Assert.Equal<uint>(360, packet.Formats[2].Height);
            Assert.Equal<VideoCodec>(VideoCodec.H264, packet.Formats[2].Codec);

            Assert.Equal<uint>(30, packet.Formats[3].FPS);
            Assert.Equal<uint>(320, packet.Formats[3].Width);
            Assert.Equal<uint>(180, packet.Formats[3].Height);
            Assert.Equal<VideoCodec>(VideoCodec.H264, packet.Formats[3].Codec);
        }

        [Fact]
        public void TestVideoControl()
        {
            VideoControl packet = NanoPacketFactory
                .ParsePacket(ResourcesProvider.GetBytes("tcp_video_control.bin", ResourceType.Nano), _context)
                    as VideoControl;

            Assert.NotNull(packet);
            Assert.NotNull(packet.Header);

            Assert.Equal<NanoChannel>(NanoChannel.Video, packet.Channel);
            Assert.Equal<NanoPayloadType>(NanoPayloadType.Streamer, packet.Header.PayloadType);

            Assert.Equal<VideoPayloadType>(VideoPayloadType.Control,
                            (VideoPayloadType)packet.StreamerHeader.PacketType);
            Assert.Equal<StreamerFlags>(StreamerFlags.GotSeqAndPrev | StreamerFlags.Unknown1,
                            packet.StreamerHeader.Flags);
            Assert.Equal<uint>(2, packet.StreamerHeader.SequenceNumber);
            Assert.Equal<uint>(1, packet.StreamerHeader.PreviousSequenceNumber);

            Assert.Equal<VideoControlFlags>(VideoControlFlags.RequestKeyframe | VideoControlFlags.StartStream,
                            packet.Flags);
        }

        [Fact]
        public void TestVideoData()
        {
            VideoData packet = NanoPacketFactory
                .ParsePacket(ResourcesProvider.GetBytes("udp_video_data.bin", ResourceType.Nano), _context)
                    as VideoData;

            Assert.NotNull(packet);
            Assert.NotNull(packet.Header);

            Assert.Equal<NanoChannel>(NanoChannel.Video, packet.Channel);
            Assert.Equal<NanoPayloadType>(NanoPayloadType.Streamer, packet.Header.PayloadType);

            Assert.Equal<VideoPayloadType>(VideoPayloadType.Data,
                            (VideoPayloadType)packet.StreamerHeader.PacketType);
            Assert.Equal<StreamerFlags>((StreamerFlags)0, packet.StreamerHeader.Flags);

            Assert.Equal<uint>(4, packet.Flags);
            Assert.Equal<uint>(3715731054, packet.FrameId);
            Assert.Equal<ulong>(3365613642, packet.Timestamp);
            Assert.Equal<uint>(5594, packet.TotalSize);
            Assert.Equal<uint>(5, packet.PacketCount);
            Assert.Equal<uint>(0, packet.Offset);
            Assert.Equal<int>(1119, packet.Data.Length);
        }
    }
}