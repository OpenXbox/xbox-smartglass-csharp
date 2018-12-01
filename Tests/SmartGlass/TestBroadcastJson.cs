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

        T DeserializeJson<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, _serializerSettings);
        }

        [Fact]
        public void TestGamestreamStateEmptyGuid()
        {
            string json = ResourcesProvider.GetString("gamestream_state_invalid.json", ResourceType.Json);
            var msg = DeserializeJson<GamestreamStateBaseMessage>(json);

            Assert.Equal<BroadcastMessageType>(BroadcastMessageType.GamestreamState, msg.Type);
            Assert.Equal<GamestreamStateMessageType>(GamestreamStateMessageType.Invalid, msg.State);
            Assert.Equal<System.Guid>(System.Guid.Empty, msg.SessionId);
        }

        [Fact]
        public void TestGamestreamEnabled()
        {
            string json = ResourcesProvider.GetString("gamestream_enabled.json", ResourceType.Json);
            var msg = DeserializeJson<GamestreamEnabledMessage>(json);

            Assert.Equal<BroadcastMessageType>(BroadcastMessageType.GamestreamEnabled, msg.Type);
            Assert.True(msg.CanBeEnabled);
            Assert.True(msg.Enabled);
            Assert.Equal<int>(6, msg.MajorProtocolVersion);
            Assert.Equal<int>(0, msg.MinorProtocolVersion);
        }

        [Fact]
        public void TestGamestreamStateInitializing()
        {
            string json = ResourcesProvider.GetString("gamestream_state_init.json", ResourceType.Json);
            var msg = DeserializeJson<GamestreamStateInitializingMessage>(json);

            Assert.Equal<BroadcastMessageType>(BroadcastMessageType.GamestreamState, msg.Type);
            Assert.Equal<GamestreamStateMessageType>(GamestreamStateMessageType.Initializing, msg.State);
            Assert.Equal<string>("14608f3c-1c4a-4f32-9da6-179ce1001e4a", msg.SessionId.ToString());
            Assert.Equal<int>(53394, msg.TcpPort);
            Assert.Equal<int>(49665, msg.UdpPort);
        }

        [Fact]
        public void TestGamestreamStateStarted()
        {
            string json = ResourcesProvider.GetString("gamestream_state_started.json", ResourceType.Json);
            var msg = DeserializeJson<GamestreamStateStartedMessage>(json);

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
            string json = ResourcesProvider.GetString("gamestream_state_stopped.json", ResourceType.Json);

            var msg = DeserializeJson<GamestreamStateStoppedMessage>(json);

            Assert.Equal<BroadcastMessageType>(BroadcastMessageType.GamestreamState, msg.Type);
            Assert.Equal<GamestreamStateMessageType>(GamestreamStateMessageType.Stopped, msg.State);
            Assert.Equal<string>("14608f3c-1c4a-4f32-9da6-179ce1001e4a", msg.SessionId.ToString());
        }

        [Fact]
        public void TestGamestreamPreviewStatus()
        {
            string json = ResourcesProvider.GetString("gamestream_preview_status.json", ResourceType.Json);
            var msg = DeserializeJson<GamestreamPreviewStatusMessage>(json);

            Assert.Equal<BroadcastMessageType>(BroadcastMessageType.PreviewStatus, msg.Type);
            Assert.False(msg.IsPublicPreview);
            Assert.False(msg.IsInternalPreview);
        }

        [Fact]
        public void TestGamestreamStart()
        {
            string json = ResourcesProvider.GetString("gamestream_start_stream.json", ResourceType.Json);
            var origMsg = DeserializeJson<GamestreamStartMessage>(json);

            var msg = new GamestreamStartMessage()
            {
                ReQueryPreviewStatus = false,
                Configuration = GamestreamConfiguration.GetStandardConfig()
            };

            // TODO: check why this isn't working like expected
            Assert.Equal(JsonConvert.SerializeObject(origMsg), JsonConvert.SerializeObject(msg));
        }
    }
}