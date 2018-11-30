using System.Collections.Generic;
using Newtonsoft.Json;
using SmartGlass.Common;
using SmartGlass.Channels;
using SmartGlass.Channels.Broadcast;
using SmartGlass.Channels.Broadcast.Messages;
using Xunit;

namespace SmartGlass.Tests
{
    public class TestBroadcastJson : TestDataProvider
    {
        private JsonSerializerSettings _serializerSettings;
        public TestBroadcastJson()
            : base("Json")
        {
            _serializerSettings = ChannelJsonSerializerSettings.GetBroadcastSettings();
        }

        private BroadcastBaseMessage DeserializeJson(string json)
        {
            return JsonConvert.DeserializeObject<BroadcastBaseMessage>(json, _serializerSettings);
        }

        [Fact]
        public void TestGamestreamStateEmptyGuid()
        {
            byte[] data = TestData["gamestream_state_invalid.json"];
            string json = System.Text.Encoding.UTF8.GetString(data);

            var msg = (GamestreamStateBaseMessage)DeserializeJson(json);

            Assert.Equal(BroadcastMessageType.GamestreamState, msg.Type);
            Assert.Equal(GamestreamStateMessageType.Invalid, msg.State);
            Assert.Equal(System.Guid.Empty, msg.SessionId);
        }

        [Fact]
        public void TestGamestreamEnabled()
        {
            byte[] data = TestData["gamestream_enabled.json"];
            string json = System.Text.Encoding.UTF8.GetString(data);

            var msg = (GamestreamEnabledMessage)DeserializeJson(json);

            Assert.Equal(BroadcastMessageType.GamestreamEnabled, msg.Type);
            Assert.True(msg.CanBeEnabled);
            Assert.True(msg.Enabled);
            Assert.Equal(6, msg.MajorProtocolVersion);
            Assert.Equal(0, msg.MinorProtocolVersion);
        }

        [Fact]
        public void TestGamestreamStateInitializing()
        {
            byte[] data = TestData["gamestream_state_init.json"];
            string json = System.Text.Encoding.UTF8.GetString(data);

            var msg = (GamestreamStateInitializingMessage)DeserializeJson(json);

            Assert.Equal(BroadcastMessageType.GamestreamState, msg.Type);
            Assert.Equal(GamestreamStateMessageType.Initializing, msg.State);
            Assert.Equal("14608f3c-1c4a-4f32-9da6-179ce1001e4a", msg.SessionId.ToString());
            Assert.Equal(53394, msg.TcpPort);
            Assert.Equal(49665, msg.UdpPort);
        }

        [Fact]
        public void TestGamestreamStateStarted()
        {
            byte[] data = TestData["gamestream_state_started.json"];
            string json = System.Text.Encoding.UTF8.GetString(data);

            var msg = (GamestreamStateStartedMessage)DeserializeJson(json);

            Assert.Equal(BroadcastMessageType.GamestreamState, msg.Type);
            Assert.Equal(GamestreamStateMessageType.Started, msg.State);
            Assert.Equal("14608f3c-1c4a-4f32-9da6-179ce1001e4a", msg.SessionId.ToString());
            Assert.False(msg.IsWirelessConnection);
            Assert.Equal(0, msg.WirelessChannel);
            Assert.Equal(1000000000, msg.TransmitLinkSpeed);
        }

        [Fact]
        public void TestGamestreamStateStopped()
        {
            byte[] data = TestData["gamestream_state_stopped.json"];
            string json = System.Text.Encoding.UTF8.GetString(data);

            var msg = (GamestreamStateStoppedMessage)DeserializeJson(json);

            Assert.Equal(BroadcastMessageType.GamestreamState, msg.Type);
            Assert.Equal(GamestreamStateMessageType.Stopped, msg.State);
            Assert.Equal("14608f3c-1c4a-4f32-9da6-179ce1001e4a", msg.SessionId.ToString());
        }

        [Fact]
        public void TestGamestreamPreviewStatus()
        {
            byte[] data = TestData["gamestream_preview_status.json"];
            string json = System.Text.Encoding.UTF8.GetString(data);

            var msg = (GamestreamPreviewStatusMessage)DeserializeJson(json);

            Assert.Equal(BroadcastMessageType.PreviewStatus, msg.Type);
            Assert.False(msg.IsPublicPreview);
            Assert.False(msg.IsInternalPreview);
        }

        [Fact]
        public void TestGamestreamConfiguration()
        {
            byte[] data = TestData["gamestream_start_stream.json"];
            string json = System.Text.Encoding.UTF8.GetString(data);

            GamestreamConfiguration config = GamestreamConfiguration.GetStandardConfig();
            var msg = new GamestreamStartMessage()
            {
                ReQueryPreviewStatus = false,
                Configuration = config
            };

            var result = JsonConvert.SerializeObject(msg, _serializerSettings) + '\n';

            Assert.Equal(json, result);
        }
    }
}