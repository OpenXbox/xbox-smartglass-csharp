using System;

namespace SmartGlass.Common
{
    public class GamestreamConfiguration
    {
        // TODO: Allow custom configuration.
        public static GamestreamConfiguration GetStandardConfig()
        {
            return new GamestreamConfiguration()
            {
                UrcpType = 0,
                UrcpFixedRate = -1,
                UrcpMaximumWindow = 1310720,
                UrcpMinimumRate = 256000,
                UrcpMaximumRate = 10000000,
                UrcpKeepAliveTimeoutMs = 0,
                AudioFecType = 0,
                VideoFecType = 0,
                VideoFecLevel = 3,
                VideoPacketUtilization = 0,
                EnableDynamicBitrate = false,
                DynamicBitrateScaleFactor = 1,
                DynamicBitrateUpdateMs = 5000,
                SendKeyframesOverTCP = false,
                VideoMaximumWidth = 1280,
                VideoMaximumHeight = 720,
                VideoMaximumFrameRate = 60,
                VideoPacketDefragTimeoutMs = 16,
                EnableVideoFrameAcks = false,
                EnableAudioChat = true,
                AudioBufferLengthHns = 10000000,
                AudioSyncPolicy = 1,
                AudioSyncMinLatency = 10,
                AudioSyncDesiredLatency = 40,
                AudioSyncMaxLatency = 170,
                AudioSyncCompressLatency = 100,
                AudioSyncCompressFactor = 0.99f,
                AudioSyncLengthenFactor = 1.01f,
                EnableOpusAudio = false,
                EnableOpusChatAudio = true,
                InputReadsPerSecond = 120,
                UdpMaxSendPacketsInWinsock = 250,
                UdpSubBurstGroups = 0,
                UdpBurstDurationMs = 12
            };
        }

        public int UrcpType { get; set; }
        public int UrcpFixedRate { get; set; }
        public int UrcpMaximumWindow { get; set; }
        public int UrcpMinimumRate { get; set; }
        public int UrcpMaximumRate { get; set; }
        public int UrcpKeepAliveTimeoutMs { get; set; }
        public int AudioFecType { get; set; }
        public int VideoFecType { get; set; }
        public int VideoFecLevel { get; set; }
        public int VideoPacketUtilization { get; set; }
        public bool EnableDynamicBitrate { get; set; }
        public int DynamicBitrateScaleFactor { get; set; }
        public int DynamicBitrateUpdateMs { get; set; }
        public bool SendKeyframesOverTCP { get; set; }
        public int VideoMaximumWidth { get; set; }
        public int VideoMaximumHeight { get; set; }
        public int VideoMaximumFrameRate { get; set; }
        public int VideoPacketDefragTimeoutMs { get; set; }
        public bool EnableVideoFrameAcks { get; set; }
        public bool EnableAudioChat { get; set; }
        public int AudioBufferLengthHns { get; set; }
        public int AudioSyncPolicy { get; set; }
        public int AudioSyncMinLatency { get; set; }
        public int AudioSyncDesiredLatency { get; set; }
        public int AudioSyncMaxLatency { get; set; }
        public int AudioSyncCompressLatency { get; set; }
        public float AudioSyncCompressFactor { get; set; }
        public float AudioSyncLengthenFactor { get; set; }
        public bool EnableOpusAudio { get; set; }
        public bool EnableOpusChatAudio { get; set; }
        public int InputReadsPerSecond { get; set; }
        public int UdpMaxSendPacketsInWinsock { get; set; }
        public int UdpSubBurstGroups { get; set; }
        public int UdpBurstDurationMs { get; set; }
    }
}