using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DarkId.SmartGlass.Channels.Broadcast
{
    class GamestreamConfiguration
    {
        // TODO: Allow custom configuration.
        public static GamestreamConfiguration GetStandardConfig()
        {
            return new GamestreamConfiguration()
            {
                UrcpType = "0",
                UrcpFixedRate = "-1",
                UrcpMaximumWindow = "1310720",
                UrcpMinimumRate = "256000",
                UrcpMaximumRate = "10000000",
                UrcpKeepAliveTimeoutMs = "0",
                AudioFecType = "0",
                VideoFecType = "0",
                VideoFecLevel = "3",
                VideoPacketUtilization = "0",
                EnableDynamicBitrate = "false",
                DynamicBitrateScaleFactor = "1",
                DynamicBitrateUpdateMs = "5000",
                SendKeyframesOverTCP = "false",
                VideoMaximumWidth = "1280",
                VideoMaximumHeight = "720",
                VideoMaximumFrameRate = "60",
                VideoPacketDefragTimeoutMs = "16",
                EnableVideoFrameAcks = "false",
                EnableAudioChat = "true",
                AudioBufferLengthHns = "10000000",
                AudioSyncPolicy = "1",
                AudioSyncMinLatency = "10",
                AudioSyncDesiredLatency = "40",
                AudioSyncMaxLatency = "170",
                AudioSyncCompressLatency = "100",
                AudioSyncCompressFactor = "0.99",
                AudioSyncLengthenFactor = "1.01",
                EnableOpusAudio = "false",
                EnableOpusChatAudio = "true",
                InputReadsPerSecond = "120",
                UdpMaxSendPacketsInWinsock = "250",
                UdpSubBurstGroups = "0",
                UdpBurstDurationMs = "12"
           };
        }

        public string UrcpType;
        public string UrcpFixedRate;
        public string UrcpMaximumWindow;
        public string UrcpMinimumRate;
        public string UrcpMaximumRate;
        public string UrcpKeepAliveTimeoutMs;
        public string AudioFecType;
        public string VideoFecType;
        public string VideoFecLevel;
        public string VideoPacketUtilization;
        public string EnableDynamicBitrate;
        public string DynamicBitrateScaleFactor;
        public string DynamicBitrateUpdateMs;
        public string SendKeyframesOverTCP;
        public string VideoMaximumWidth;
        public string VideoMaximumHeight;
        public string VideoMaximumFrameRate;
        public string VideoPacketDefragTimeoutMs;
        public string EnableVideoFrameAcks;
        public string EnableAudioChat;
        public string AudioBufferLengthHns;
        public string AudioSyncPolicy;
        public string AudioSyncMinLatency;
        public string AudioSyncDesiredLatency;
        public string AudioSyncMaxLatency;
        public string AudioSyncCompressLatency;
        public string AudioSyncCompressFactor;
        public string AudioSyncLengthenFactor;
        public string EnableOpusAudio;
        public string EnableOpusChatAudio;
        public string InputReadsPerSecond;
        public string UdpMaxSendPacketsInWinsock;
        public string UdpSubBurstGroups;
        public string UdpBurstDurationMs;
    }
}
