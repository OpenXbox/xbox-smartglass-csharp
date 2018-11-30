using System.Collections.Generic;
using Newtonsoft.Json;
using Xunit;


using SmartGlass.Common;
using SmartGlass.Channels;
using SmartGlass.Channels.Broadcast;
using SmartGlass.Channels.Broadcast.Messages;
using Tests.Resources;

namespace Tests.SmartGlass
{
    public class TestBroadcastJson
    {
        JsonSerializerSettings _serializerSettings;
        public TestBroadcastJson()
        {
            _serializerSettings = ChannelJsonSerializerSettings.GetBroadcastSettings();
        }

        BroadcastBaseMessage DeserializeJson(string json)
        {
            return JsonConvert.DeserializeObject<BroadcastBaseMessage>(json, _serializerSettings);
        }

        [Fact]
        public void TestGamestreamStateEmptyGuid()
        {
            byte[] data = ResourcesProvider.GetContent("gamestream_state_invalid.json");
            string json = System.Text.Encoding.UTF8.GetString(data);

            var msg = (GamestreamStateBaseMessage)DeserializeJson(json);

            Assert.Equal<BroadcastMessageType>(BroadcastMessageType.GamestreamState, msg.Type);
            Assert.Equal<GamestreamStateMessageType>(GamestreamStateMessageType.Invalid, msg.State);
            Assert.Equal<System.Guid>(System.Guid.Empty, msg.SessionId);
        }

        [Fact]
        public void TestGamestreamEnabled()
        {
            byte[] data = ResourcesProvider.GetContent("gamestream_enabled.json");
            string json = System.Text.Encoding.UTF8.GetString(data);

            var msg = (GamestreamEnabledMessage)DeserializeJson(json);

            Assert.Equal<BroadcastMessageType>(BroadcastMessageType.GamestreamEnabled, msg.Type);
            Assert.True(msg.CanBeEnabled);
            Assert.True(msg.Enabled);
            Assert.Equal<int>(6, msg.MajorProtocolVersion);
            Assert.Equal<int>(0, msg.MinorProtocolVersion);
        }

        [Fact]
        public void TestGamestreamStateInitializing()
        {
            byte[] data = ResourcesProvider.GetContent("gamestream_state_init.json");
            string json = System.Text.Encoding.UTF8.GetString(data);

            var msg = (GamestreamStateInitializingMessage)DeserializeJson(json);

            Assert.Equal<BroadcastMessageType>(BroadcastMessageType.GamestreamState, msg.Type);
            Assert.Equal<GamestreamStateMessageType>(GamestreamStateMessageType.Initializing, msg.State);
            Assert.Equal<string>("14608f3c-1c4a-4f32-9da6-179ce1001e4a", msg.SessionId.ToString());
            Assert.Equal<int>(53394, msg.TcpPort);
            Assert.Equal<int>(49665, msg.UdpPort);
        }

        [Fact]
        public void TestGamestreamStateStarted()
        {
            byte[] data = ResourcesProvider.GetContent("gamestream_state_started.json");
            string json = System.Text.Encoding.UTF8.GetString(data);

            var msg = (GamestreamStateStartedMessage)DeserializeJson(json);

            Assert.Equal<BroadcastMessageType>(BroadcastMessageType.GamestreamState, msg.Type);
            Assert.Equal<GamestreamStateMessageType>(GamestreamStateMessageType.Started, msg.State);
            Assert.Equal<string>("14608f3c-1c4a-4f32-9da6-179ce1001e4a", msg.SessionId.ToString());
            Assert.False(msg.IsWirelessConnection);
            Assert.Equal<int>(0, msg.WirelessChannel);
            Assert.Equal<int>(1000000000, msg.TransmitLinkSpeed);
        }

        [Fact]
        public void TestGamestreamStateStopped()
        {
            byte[] data = ResourcesProvider.GetContent("gamestream_state_stopped.json");
            string json = System.Text.Encoding.UTF8.GetString(data);

            var msg = (GamestreamStateStoppedMessage)DeserializeJson(json);

            Assert.Equal<BroadcastMessageType>(BroadcastMessageType.GamestreamState, msg.Type);
            Assert.Equal<GamestreamStateMessageType>(GamestreamStateMessageType.Stopped, msg.State);
            Assert.Equal<string>("14608f3c-1c4a-4f32-9da6-179ce1001e4a", msg.SessionId.ToString());
        }

        [Fact]
        public void TestGamestreamPreviewStatus()
        {
            byte[] data = ResourcesProvider.GetContent("gamestream_preview_status.json");
            string json = System.Text.Encoding.UTF8.GetString(data);

            var msg = (GamestreamPreviewStatusMessage)DeserializeJson(json);

            Assert.Equal<BroadcastMessageType>(BroadcastMessageType.PreviewStatus, msg.Type);
            Assert.False(msg.IsPublicPreview);
            Assert.False(msg.IsInternalPreview);
        }

        [Fact]
        public void TestGamestreamConfiguration()
        {
            byte[] data = ResourcesProvider.GetContent("gamestream_start_stream.json");
            string json = System.Text.Encoding.UTF8.GetString(data);

            GamestreamConfiguration config = GamestreamConfiguration.GetStandardConfig();
            var msg = new GamestreamStartMessage()
            {
                ReQueryPreviewStatus = false,
                Configuration = config
            };

            var result = JsonConvert.SerializeObject(msg, _serializerSettings) + '\n';

            Assert.Equal<string>(json, result);
        }
    }
}