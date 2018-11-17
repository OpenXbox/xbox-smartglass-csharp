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
            BEReader reader = new BEReader(TestData["tcp_control_handshake"]);
            RtpHeader header = new RtpHeader();
            header.Deserialize(reader);

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
        public void TestControlHandshake()
        {
            ControlHandshake packet = NanoPacketFactory
                .ParsePacket(TestData["tcp_control_handshake"], _context)
                    as ControlHandshake;

            Assert.IsNotNull(packet.Header);
            Assert.AreEqual(NanoChannel.TcpBase, packet.Channel);
            Assert.AreEqual(ControlHandshakeType.SYN, packet.Type);
            Assert.AreEqual(40084, packet.ConnectionId);
        }

        [Test]
        public void TestChannelCreate()
        {
            ChannelCreate packet = NanoPacketFactory
                .ParsePacket(TestData["tcp_channel_create"], _context)
                    as ChannelCreate;

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
                .ParsePacket(TestData["tcp_channel_open_no_flags"], _context)
                    as ChannelOpen;

            Assert.IsNotNull(packet.Header);
            Assert.AreEqual(NanoChannel.Video, packet.Channel);
            Assert.AreEqual(ChannelControlType.Open, packet.Type);
            Assert.AreEqual(new byte[0], packet.Flags);
        }

        [Test]
        public void TestChannelOpenWithFlags()
        {
            ChannelOpen packet = NanoPacketFactory
                .ParsePacket(TestData["tcp_channel_open_with_flags"], _context)
                    as ChannelOpen;

            Assert.IsNotNull(packet.Header);
            Assert.AreEqual(NanoChannel.Control, packet.Channel);
            Assert.AreEqual(ChannelControlType.Open, packet.Type);
            Assert.AreEqual(new byte[] { 0x01, 0x00, 0x02, 0x00 }, packet.Flags);
        }

        [Test]
        public void TestChannelClose()
        {
            ChannelClose packet = NanoPacketFactory
                .ParsePacket(TestData["tcp_channel_close"], _context)
                    as ChannelClose;

            Assert.IsNotNull(packet.Header);
            Assert.AreEqual(NanoChannel.Audio, packet.Channel);
            Assert.AreEqual(ChannelControlType.Close, packet.Type);
            Assert.AreEqual(0, packet.Flags);
        }

        [Test]
        public void TestUdpHandshake()
        {
            UdpHandshake packet = NanoPacketFactory
                .ParsePacket(TestData["udp_handshake"], _context)
                    as UdpHandshake;

            Assert.IsNotNull(packet.Header);
            Assert.AreEqual(NanoChannel.TcpBase, packet.Channel);
            Assert.AreEqual(ControlHandshakeType.ACK, packet.Type);
        }

        [Test]
        public void TestVideoServerHandshake()
        {
            VideoServerHandshake packet = NanoPacketFactory
                .ParsePacket(TestData["tcp_video_server_handshake"], _context)
                    as VideoServerHandshake;

            Assert.IsNotNull(packet.Header);
            Assert.AreEqual(NanoChannel.Video, packet.Channel);

            Assert.AreEqual(NanoPayloadType.Streamer, packet.Header.PayloadType);

            Assert.AreEqual(3, (uint)packet.StreamerHeader.Flags);
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
    }
}