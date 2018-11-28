using System.Collections.Generic;
using NUnit.Framework;
using Newtonsoft.Json;
using SmartGlass.Channels;
using SmartGlass.Channels.Broadcast;
using SmartGlass.Channels.Broadcast.Messages;

namespace SmartGlass.Tests
{
    public class TestBroadcastJson : TestDataProvider
    {
        private JsonSerializerSettings _serializerSettings;
        public TestBroadcastJson()
            : base("Json")
        {
        }

        [SetUp]
        public void Setup()
        {
            _serializerSettings = ChannelJsonSerializerSettings.GetBroadcastSettings();
        }

        private BroadcastBaseMessage DeserializeJson(string json)
        {
            return JsonConvert.DeserializeObject<BroadcastBaseMessage>(json, _serializerSettings);
        }

        [Test]
        public void TestGamestreamStateEmptyGuid()
        {
            byte[] data = TestData["gamestream_state_invalid.json"];
            string json = System.Text.Encoding.UTF8.GetString(data);

            var msg = DeserializeJson(json);
        }

        [Test]
        public void TestGamestreamEnabled()
        {
            byte[] data = TestData["gamestream_enabled.json"];
            string json = System.Text.Encoding.UTF8.GetString(data);

            var msg = (GamestreamEnabledMessage)DeserializeJson(json);

            Assert.AreEqual(BroadcastMessageType.GamestreamEnabled, msg.Type);
            Assert.IsTrue(msg.CanBeEnabled);
            Assert.IsTrue(msg.Enabled);
            Assert.AreEqual(6, msg.MajorProtocolVersion);
            Assert.AreEqual(0, msg.MinorProtocolVersion);
        }

        [Test]
        public void TestGamestreamStateInitializing()
        {
            byte[] data = TestData["gamestream_state_init.json"];
            string json = System.Text.Encoding.UTF8.GetString(data);

            var msg = (GamestreamStateInitializingMessage)DeserializeJson(json);

            Assert.AreEqual(BroadcastMessageType.GamestreamState, msg.Type);
            Assert.AreEqual(GamestreamStateMessageType.Initializing, msg.State);
            Assert.AreEqual("14608f3c-1c4a-4f32-9da6-179ce1001e4a", msg.SessionId.ToString());
            Assert.AreEqual(53394, msg.TcpPort);
            Assert.AreEqual(49665, msg.UdpPort);
        }

        [Test]
        public void TestGamestreamStateStarted()
        {
            byte[] data = TestData["gamestream_state_started.json"];
            string json = System.Text.Encoding.UTF8.GetString(data);

            var msg = (GamestreamStateStartedMessage)DeserializeJson(json);

            Assert.AreEqual(BroadcastMessageType.GamestreamState, msg.Type);
            Assert.AreEqual(GamestreamStateMessageType.Started, msg.State);
            Assert.AreEqual("14608f3c-1c4a-4f32-9da6-179ce1001e4a", msg.SessionId.ToString());
            Assert.IsFalse(msg.IsWirelessConnection);
            Assert.AreEqual(0, msg.WirelessChannel);
            Assert.AreEqual(1000000000, msg.TransmitLinkSpeed);
        }

        [Test]
        public void TestGamestreamStateStopped()
        {
            byte[] data = TestData["gamestream_state_stopped.json"];
            string json = System.Text.Encoding.UTF8.GetString(data);

            var msg = (GamestreamStateStoppedMessage)DeserializeJson(json);

            Assert.AreEqual(BroadcastMessageType.GamestreamState, msg.Type);
            Assert.AreEqual(GamestreamStateMessageType.Stopped, msg.State);
            Assert.AreEqual("14608f3c-1c4a-4f32-9da6-179ce1001e4a", msg.SessionId.ToString());
        }

        [Test]
        public void TestGamestreamPreviewStatus()
        {
            byte[] data = TestData["gamestream_preview_status.json"];
            string json = System.Text.Encoding.UTF8.GetString(data);

            var msg = (GamestreamPreviewStatusMessage)DeserializeJson(json);

            Assert.AreEqual(BroadcastMessageType.PreviewStatus, msg.Type);
            Assert.IsFalse(msg.IsPublicPreview);
            Assert.IsFalse(msg.IsInternalPreview);
        }
    }
}