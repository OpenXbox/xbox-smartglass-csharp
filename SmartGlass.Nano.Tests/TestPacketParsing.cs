using System;
using System.IO;
using System.Collections.Generic;
using NUnit.Framework;
using SmartGlass.Common;
using SmartGlass.Nano.Packets;

namespace SmartGlass.Nano.Tests
{
    public class TestPacketParsing : TestDataProvider
    {
        private NanoChannelContext _context { get; set; }

        public TestPacketParsing()
            : base("Packets")
        {
            _context = new NanoChannelContext();
            _context.RegisterChannel(1024, NanoChannel.Video);
            _context.RegisterChannel(1025, NanoChannel.Audio);
            _context.RegisterChannel(1026, NanoChannel.ChatAudio);
            _context.RegisterChannel(1027, NanoChannel.Control);
            _context.RegisterChannel(1028, NanoChannel.Input);
            _context.RegisterChannel(1029, NanoChannel.InputFeedback);
        }

        [Test]
        public void TestRtpHeader()
        {
            BEReader reader = new BEReader(TestData["tcp_control_handshake.bin"]);
            RtpHeader header = new RtpHeader();
            header.Deserialize(reader);

            Assert.IsNotNull(header);
            Assert.IsTrue(header.Padding);
            Assert.IsFalse(header.Extension);
            Assert.AreEqual(0, header.CsrcCount);
            Assert.IsFalse(header.Marker);
            Assert.AreEqual(NanoPayloadType.ControlHandshake, header.PayloadType);
            Assert.AreEqual(0, header.SequenceNumber);
            Assert.AreEqual(2847619159, header.Timestamp);
            Assert.AreEqual(0, header.ConnectionId);
            Assert.AreEqual(0, header.ChannelId);
        }

        [Test]
        public void TestUnknownChannelParse()
        {
            // Create channel context without any registered channels
            NanoChannelContext localContext = new NanoChannelContext();

            byte[] packetData = TestData["tcp_channel_open_no_flags.bin"];
            Assert.Throws<NanoException>(() =>
            {
                NanoPacketFactory
                    .ParsePacket(packetData, localContext);
            });
        }

        [Test]
        public void TestControlHandshake()
        {
            ControlHandshake packet = NanoPacketFactory
                .ParsePacket(TestData["tcp_control_handshake.bin"], _context)
                    as ControlHandshake;

            Assert.IsNotNull(packet);
            Assert.IsNotNull(packet.Header);
            Assert.AreEqual(NanoChannel.TcpBase, packet.Channel);
            Assert.AreEqual(ControlHandshakeType.SYN, packet.Type);
            Assert.AreEqual(40084, packet.ConnectionId);
        }

        [Test]
        public void TestChannelCreate()
        {
            ChannelCreate packet = NanoPacketFactory
                .ParsePacket(TestData["tcp_channel_create.bin"], _context)
                    as ChannelCreate;

            Assert.IsNotNull(packet);
            Assert.IsNotNull(packet.Header);
            Assert.AreEqual(NanoChannel.Video, packet.Channel);
            Assert.AreEqual(ChannelControlType.Create, packet.Type);
            Assert.AreEqual(NanoChannelClass.Video, packet.Name);
            Assert.AreEqual(0, packet.Flags);
        }

        [Test]
        public void TestChannelOpenNoFlags()
        {
            ChannelOpen packet = NanoPacketFactory
                .ParsePacket(TestData["tcp_channel_open_no_flags.bin"], _context)
                    as ChannelOpen;

            Assert.IsNotNull(packet);
            Assert.IsNotNull(packet.Header);
            Assert.AreEqual(NanoChannel.Video, packet.Channel);
            Assert.AreEqual(ChannelControlType.Open, packet.Type);
            Assert.AreEqual(new byte[0], packet.Flags);
        }

        [Test]
        public void TestChannelOpenWithFlags()
        {
            ChannelOpen packet = NanoPacketFactory
                .ParsePacket(TestData["tcp_channel_open_with_flags.bin"], _context)
                    as ChannelOpen;

            Assert.IsNotNull(packet);
            Assert.IsNotNull(packet.Header);
            Assert.AreEqual(NanoChannel.Control, packet.Channel);
            Assert.AreEqual(ChannelControlType.Open, packet.Type);
            Assert.AreEqual(new byte[] { 0x01, 0x00, 0x02, 0x00 }, packet.Flags);
        }

        [Test]
        public void TestChannelClose()
        {
            ChannelClose packet = NanoPacketFactory
                .ParsePacket(TestData["tcp_channel_close.bin"], _context)
                    as ChannelClose;

            Assert.IsNotNull(packet);
            Assert.IsNotNull(packet.Header);
            Assert.AreEqual(NanoChannel.Audio, packet.Channel);
            Assert.AreEqual(ChannelControlType.Close, packet.Type);
            Assert.AreEqual(0, packet.Flags);
        }

        [Test]
        public void TestUdpHandshake()
        {
            UdpHandshake packet = NanoPacketFactory
                .ParsePacket(TestData["udp_handshake.bin"], _context)
                    as UdpHandshake;

            Assert.IsNotNull(packet);
            Assert.IsNotNull(packet.Header);
            Assert.AreEqual(NanoChannel.TcpBase, packet.Channel);
            Assert.AreEqual(ControlHandshakeType.ACK, packet.Type);
        }

        [Test]
        public void TestStreamerControlHeader()
        {
            RealtimeTelemetry packet = NanoPacketFactory
               .ParsePacket(TestData["tcp_control_msg_with_header_realtime_telemetry.bin"], _context)
                   as RealtimeTelemetry;

            Assert.IsNotNull(packet);
            Assert.IsNotNull(packet.Header);

            Assert.AreEqual(NanoChannel.Control, packet.Channel);
            Assert.AreEqual(NanoPayloadType.Streamer, packet.Header.PayloadType);

            Assert.AreEqual(0, packet.StreamerHeader.PacketType);
            Assert.AreEqual(StreamerFlags.GotSeqAndPrev | StreamerFlags.Unknown1,
                            packet.StreamerHeader.Flags);
            Assert.AreEqual(1, packet.StreamerHeader.SequenceNumber);
            Assert.AreEqual(0, packet.StreamerHeader.PreviousSequenceNumber);

            Assert.AreEqual(0, packet.ControlHeader.PreviousSequence);
            Assert.AreEqual(1, packet.ControlHeader.Unknown1);
            Assert.AreEqual(1406, packet.ControlHeader.Unknown2);
            Assert.AreEqual(ControlOpCode.RealtimeTelemetry, packet.ControlHeader.OpCode);
        }

        [Test]
        public void TestStreamerControlChangeVideoQuality()
        {
            ChangeVideoQuality packet = NanoPacketFactory
                .ParsePacket(TestData["tcp_control_msg_with_header_change_video_quality.bin"], _context)
                    as ChangeVideoQuality;

            Assert.IsNotNull(packet);
            Assert.IsNotNull(packet.Header);

            Assert.AreEqual(NanoChannel.Control, packet.Channel);
            Assert.AreEqual(NanoPayloadType.Streamer, packet.Header.PayloadType);

            Assert.AreEqual(0, packet.StreamerHeader.PacketType);
            Assert.AreEqual(StreamerFlags.GotSeqAndPrev | StreamerFlags.Unknown1,
                            packet.StreamerHeader.Flags);
            Assert.AreEqual(1, packet.StreamerHeader.SequenceNumber);
            Assert.AreEqual(0, packet.StreamerHeader.PreviousSequenceNumber);

            Assert.AreEqual(0, packet.ControlHeader.PreviousSequence);
            Assert.AreEqual(1, packet.ControlHeader.Unknown1);
            Assert.AreEqual(1406, packet.ControlHeader.Unknown2);
            Assert.AreEqual(ControlOpCode.ChangeVideoQuality, packet.ControlHeader.OpCode);
        }

        [Test]
        public void TestAudioClientHandshake()
        {
            AudioClientHandshake packet = NanoPacketFactory
                .ParsePacket(TestData["tcp_audio_client_handshake.bin"], _context)
                    as AudioClientHandshake;

            Assert.IsNotNull(packet);
            Assert.IsNotNull(packet.Header);

            Assert.AreEqual(NanoChannel.Audio, packet.Channel);
            Assert.AreEqual(NanoPayloadType.Streamer, packet.Header.PayloadType);

            Assert.AreEqual(AudioPayloadType.ClientHandshake,
                            (AudioPayloadType)packet.StreamerHeader.PacketType);
            Assert.AreEqual(StreamerFlags.GotSeqAndPrev | StreamerFlags.Unknown1,
                            packet.StreamerHeader.Flags);
            Assert.AreEqual(1, packet.StreamerHeader.SequenceNumber);
            Assert.AreEqual(0, packet.StreamerHeader.PreviousSequenceNumber);

            Assert.AreEqual(693041842, packet.InitialFrameID);
            Assert.AreEqual(2, packet.RequestedFormat.Channels);
            Assert.AreEqual(48000, packet.RequestedFormat.SampleRate);
            Assert.AreEqual(AudioCodec.AAC, packet.RequestedFormat.Codec);
        }

        [Test]
        public void TestAudioServerHandshake()
        {
            AudioServerHandshake packet = NanoPacketFactory
                .ParsePacket(TestData["tcp_audio_server_handshake.bin"], _context)
                    as AudioServerHandshake;

            Assert.IsNotNull(packet);
            Assert.IsNotNull(packet.Header);

            Assert.AreEqual(NanoChannel.Audio, packet.Channel);
            Assert.AreEqual(NanoPayloadType.Streamer, packet.Header.PayloadType);

            Assert.AreEqual(AudioPayloadType.ServerHandshake,
                            (AudioPayloadType)packet.StreamerHeader.PacketType);
            Assert.AreEqual(StreamerFlags.GotSeqAndPrev | StreamerFlags.Unknown1,
                            packet.StreamerHeader.Flags);
            Assert.AreEqual(1, packet.StreamerHeader.SequenceNumber);
            Assert.AreEqual(0, packet.StreamerHeader.PreviousSequenceNumber);

            Assert.AreEqual(4, packet.ProtocolVersion);
            Assert.AreEqual(1495315092424, packet.ReferenceTimestamp);
            Assert.AreEqual(1, packet.Formats.Length);

            Assert.AreEqual(2, packet.Formats[0].Channels);
            Assert.AreEqual(48000, packet.Formats[0].SampleRate);
            Assert.AreEqual(AudioCodec.AAC, packet.Formats[0].Codec);
        }

        [Test]
        public void TestAudioControl()
        {
            AudioControl packet = NanoPacketFactory
                .ParsePacket(TestData["tcp_audio_control.bin"], _context)
                    as AudioControl;

            Assert.IsNotNull(packet);
            Assert.IsNotNull(packet.Header);

            Assert.AreEqual(NanoChannel.Audio, packet.Channel);
            Assert.AreEqual(NanoPayloadType.Streamer, packet.Header.PayloadType);

            Assert.AreEqual(AudioPayloadType.Control,
                            (AudioPayloadType)packet.StreamerHeader.PacketType);
            Assert.AreEqual(StreamerFlags.GotSeqAndPrev | StreamerFlags.Unknown1,
                            packet.StreamerHeader.Flags);
            Assert.AreEqual(2, packet.StreamerHeader.SequenceNumber);
            Assert.AreEqual(1, packet.StreamerHeader.PreviousSequenceNumber);

            Assert.AreEqual(AudioControlFlags.StartStream, packet.Flags);
        }

        [Test]
        public void TestAudioData()
        {
            AudioData packet = NanoPacketFactory
                .ParsePacket(TestData["udp_audio_data.bin"], _context)
                    as AudioData;

            Assert.IsNotNull(packet);
            Assert.IsNotNull(packet.Header);

            Assert.AreEqual(NanoChannel.Audio, packet.Channel);
            Assert.AreEqual(NanoPayloadType.Streamer, packet.Header.PayloadType);

            Assert.AreEqual(AudioPayloadType.Data,
                            (AudioPayloadType)packet.StreamerHeader.PacketType);
            Assert.AreEqual((StreamerFlags)0, packet.StreamerHeader.Flags);

            Assert.AreEqual(4, packet.Flags);
            Assert.AreEqual(0, packet.FrameId);
            Assert.AreEqual(3365588462, packet.Timestamp);
            Assert.AreEqual(357, packet.Data.Length);
        }

        [Test]
        public void TestInputClientHandshake()
        {
            InputClientHandshake packet = NanoPacketFactory
               .ParsePacket(TestData["tcp_input_client_handshake.bin"], _context)
                   as InputClientHandshake;

            Assert.IsNotNull(packet);
            Assert.IsNotNull(packet.Header);

            Assert.AreEqual(NanoChannel.Input, packet.Channel);
            Assert.AreEqual(NanoPayloadType.Streamer, packet.Header.PayloadType);

            Assert.AreEqual(InputPayloadType.ClientHandshake,
                            (InputPayloadType)packet.StreamerHeader.PacketType);
            Assert.AreEqual(StreamerFlags.GotSeqAndPrev | StreamerFlags.Unknown1,
                            packet.StreamerHeader.Flags);
            Assert.AreEqual(1, packet.StreamerHeader.SequenceNumber);
            Assert.AreEqual(0, packet.StreamerHeader.PreviousSequenceNumber);

            Assert.AreEqual(10, packet.MaxTouches);
            Assert.AreEqual(1498690645999, packet.ReferenceTimestamp);
        }

        [Test]
        public void TestInputServerHandshake()
        {
            InputServerHandshake packet = NanoPacketFactory
               .ParsePacket(TestData["tcp_input_server_handshake.bin"], _context)
                   as InputServerHandshake;

            Assert.IsNotNull(packet);
            Assert.IsNotNull(packet.Header);

            Assert.AreEqual(NanoChannel.Input, packet.Channel);
            Assert.AreEqual(NanoPayloadType.Streamer, packet.Header.PayloadType);

            Assert.AreEqual(InputPayloadType.ServerHandshake,
                            (InputPayloadType)packet.StreamerHeader.PacketType);
            Assert.AreEqual(StreamerFlags.GotSeqAndPrev | StreamerFlags.Unknown1,
                            packet.StreamerHeader.Flags);
            Assert.AreEqual(1, packet.StreamerHeader.SequenceNumber);
            Assert.AreEqual(0, packet.StreamerHeader.PreviousSequenceNumber);

            Assert.AreEqual(3, packet.ProtocolVersion);
            Assert.AreEqual(1280, packet.DesktopWidth);
            Assert.AreEqual(720, packet.DesktopHeight);
            Assert.AreEqual(0, packet.MaxTouches);
            Assert.AreEqual(672208545, packet.InitialFrameId);
        }

        [Test]
        public void TestInputFrame()
        {
            InputFrame packet = NanoPacketFactory
                .ParsePacket(TestData["udp_input_frame.bin"], _context)
                    as InputFrame;

            Assert.IsNotNull(packet);
            Assert.IsNotNull(packet.Header);

            Assert.AreEqual(NanoChannel.Input, packet.Channel);
            Assert.AreEqual(NanoPayloadType.Streamer, packet.Header.PayloadType);

            Assert.AreEqual(InputPayloadType.Frame,
                            (InputPayloadType)packet.StreamerHeader.PacketType);
            Assert.AreEqual((StreamerFlags)0, packet.StreamerHeader.Flags);

            Assert.AreEqual(583706495, packet.CreatedTimestamp);
            Assert.AreEqual(583706515, packet.Timestamp);
            Assert.AreEqual(672208564, packet.FrameId);
        }

        [Test]
        public void TestInputFrameAck()
        {
            InputFrameAck packet = NanoPacketFactory
                .ParsePacket(TestData["udp_input_frame_ack.bin"], _context)
                    as InputFrameAck;

            Assert.IsNotNull(packet);
            Assert.IsNotNull(packet.Header);

            Assert.AreEqual(NanoChannel.Input, packet.Channel);
            Assert.AreEqual(NanoPayloadType.Streamer, packet.Header.PayloadType);

            Assert.AreEqual(InputPayloadType.FrameAck,
                            (InputPayloadType)packet.StreamerHeader.PacketType);
            Assert.AreEqual((StreamerFlags)0, packet.StreamerHeader.Flags);

            Assert.AreEqual(672208545, packet.AckedFrame);
        }

        [Test]
        public void TestVideoClientHandshake()
        {
            VideoClientHandshake packet = NanoPacketFactory
               .ParsePacket(TestData["tcp_video_client_handshake.bin"], _context)
                   as VideoClientHandshake;

            Assert.IsNotNull(packet);
            Assert.IsNotNull(packet.Header);

            Assert.AreEqual(NanoChannel.Video, packet.Channel);
            Assert.AreEqual(NanoPayloadType.Streamer, packet.Header.PayloadType);

            Assert.AreEqual(VideoPayloadType.ClientHandshake,
                            (VideoPayloadType)packet.StreamerHeader.PacketType);
            Assert.AreEqual(StreamerFlags.GotSeqAndPrev | StreamerFlags.Unknown1,
                            packet.StreamerHeader.Flags);
            Assert.AreEqual(1, packet.StreamerHeader.SequenceNumber);
            Assert.AreEqual(0, packet.StreamerHeader.PreviousSequenceNumber);

            Assert.AreEqual(3715731054, packet.InitialFrameId);
            Assert.AreEqual(30, packet.RequestedFormat.FPS);
            Assert.AreEqual(1280, packet.RequestedFormat.Width);
            Assert.AreEqual(720, packet.RequestedFormat.Height);
            Assert.AreEqual(VideoCodec.H264, packet.RequestedFormat.Codec);
        }

        [Test]
        public void TestVideoServerHandshake()
        {
            VideoServerHandshake packet = NanoPacketFactory
                .ParsePacket(TestData["tcp_video_server_handshake.bin"], _context)
                    as VideoServerHandshake;

            Assert.IsNotNull(packet);
            Assert.IsNotNull(packet.Header);
            Assert.AreEqual(NanoChannel.Video, packet.Channel);

            Assert.AreEqual(NanoPayloadType.Streamer, packet.Header.PayloadType);

            Assert.AreEqual(StreamerFlags.GotSeqAndPrev | StreamerFlags.Unknown1,
                            packet.StreamerHeader.Flags);
            Assert.AreEqual(1, packet.StreamerHeader.SequenceNumber);
            Assert.AreEqual(0, packet.StreamerHeader.PreviousSequenceNumber);
            Assert.AreEqual(VideoPayloadType.ServerHandshake, (VideoPayloadType)packet.StreamerHeader.PacketType);

            Assert.AreEqual(5, packet.ProtocolVersion);
            Assert.AreEqual(1280, packet.Width);
            Assert.AreEqual(720, packet.Height);
            Assert.AreEqual(30, packet.FPS);
            Assert.AreEqual(1495315092425, packet.ReferenceTimestamp);
            Assert.AreEqual(4, packet.Formats.Length);

            Assert.AreEqual(30, packet.Formats[0].FPS);
            Assert.AreEqual(1280, packet.Formats[0].Width);
            Assert.AreEqual(720, packet.Formats[0].Height);
            Assert.AreEqual(VideoCodec.H264, packet.Formats[0].Codec);

            Assert.AreEqual(30, packet.Formats[1].FPS);
            Assert.AreEqual(960, packet.Formats[1].Width);
            Assert.AreEqual(540, packet.Formats[1].Height);
            Assert.AreEqual(VideoCodec.H264, packet.Formats[1].Codec);

            Assert.AreEqual(30, packet.Formats[2].FPS);
            Assert.AreEqual(640, packet.Formats[2].Width);
            Assert.AreEqual(360, packet.Formats[2].Height);
            Assert.AreEqual(VideoCodec.H264, packet.Formats[2].Codec);

            Assert.AreEqual(30, packet.Formats[3].FPS);
            Assert.AreEqual(320, packet.Formats[3].Width);
            Assert.AreEqual(180, packet.Formats[3].Height);
            Assert.AreEqual(VideoCodec.H264, packet.Formats[3].Codec);
        }

        [Test]
        public void TestVideoControl()
        {
            VideoControl packet = NanoPacketFactory
                .ParsePacket(TestData["tcp_video_control.bin"], _context)
                    as VideoControl;

            Assert.IsNotNull(packet);
            Assert.IsNotNull(packet.Header);

            Assert.AreEqual(NanoChannel.Video, packet.Channel);
            Assert.AreEqual(NanoPayloadType.Streamer, packet.Header.PayloadType);

            Assert.AreEqual(VideoPayloadType.Control,
                            (VideoPayloadType)packet.StreamerHeader.PacketType);
            Assert.AreEqual(StreamerFlags.GotSeqAndPrev | StreamerFlags.Unknown1,
                            packet.StreamerHeader.Flags);
            Assert.AreEqual(2, packet.StreamerHeader.SequenceNumber);
            Assert.AreEqual(1, packet.StreamerHeader.PreviousSequenceNumber);

            Assert.AreEqual(VideoControlFlags.RequestKeyframe | VideoControlFlags.StartStream,
                            packet.Flags);
        }

        [Test]
        public void TestVideoData()
        {
            VideoData packet = NanoPacketFactory
                .ParsePacket(TestData["udp_video_data.bin"], _context)
                    as VideoData;

            Assert.IsNotNull(packet);
            Assert.IsNotNull(packet.Header);

            Assert.AreEqual(NanoChannel.Video, packet.Channel);
            Assert.AreEqual(NanoPayloadType.Streamer, packet.Header.PayloadType);

            Assert.AreEqual(VideoPayloadType.Data,
                            (VideoPayloadType)packet.StreamerHeader.PacketType);
            Assert.AreEqual((StreamerFlags)0, packet.StreamerHeader.Flags);

            Assert.AreEqual(4, packet.Flags);
            Assert.AreEqual(3715731054, packet.FrameId);
            Assert.AreEqual(3365613642, packet.Timestamp);
            Assert.AreEqual(5594, packet.TotalSize);
            Assert.AreEqual(5, packet.PacketCount);
            Assert.AreEqual(0, packet.Offset);
            Assert.AreEqual(1119, packet.Data.Length);
        }
    }
}