using System;

namespace SmartGlass.Common
{
    /// <summary>
    /// Gamestream configuration.
    /// Sent to the console via BroadcastChannel to start
    /// a gamestream session.
    /// </summary>
    public class GamestreamConfiguration
    {
        /// <summary>
        /// Gets the standard gamestream configuration.
        /// TODO: Allow custom configuration.
        /// </summary>
        /// <returns>A standard gamestream config.</returns>
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

        /// <summary>
        /// URCP type
        /// </summary>
        /// <value>URCP type.</value>
        public int UrcpType { get; set; }
        /// <summary>
        /// URCP fixed rate
        /// </summary>
        /// <value>URCP fixed rate.</value>
        public int UrcpFixedRate { get; set; }
        /// <summary>
        /// URCP maximum timewindow
        /// </summary>
        /// <value>URCP maximum timewindow.</value>
        public int UrcpMaximumWindow { get; set; }
        /// <summary>
        /// URCP minimum rate
        /// </summary>
        /// <value>URCP minimum rate.</value>
        public int UrcpMinimumRate { get; set; }
        /// <summary>
        /// URCP maximum rate
        /// </summary>
        /// <value>URCP maximum rate.</value>
        public int UrcpMaximumRate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <value>.</value>
        public int UrcpKeepAliveTimeoutMs { get; set; }
        /// <summary>
        /// FEC type for Audio channel
        /// </summary>
        /// <value>Audio FEC type.</value>
        public int AudioFecType { get; set; }
        /// <summary>
        /// FEC type for Video channel
        /// </summary>
        /// <value>Video FEC type.</value>
        public int VideoFecType { get; set; }
        /// <summary>
        /// FEC level for Video Channel
        /// </summary>
        /// <value>Video FEC level.</value>
        public int VideoFecLevel { get; set; }
        /// <summary>
        /// Video packet utilization
        /// </summary>
        /// <value>Video packet utilization.</value>
        public int VideoPacketUtilization { get; set; }
        /// <summary>
        /// Wether to enable dynamic bitrate
        /// </summary>
        /// <value>true if enabled, false otherwise.</value>
        public bool EnableDynamicBitrate { get; set; }
        /// <summary>
        /// Dynamic bitrate scale factor
        /// </summary>
        /// <value>Dynamic bitrate scale factor.</value>
        public int DynamicBitrateScaleFactor { get; set; }
        /// <summary>
        /// Dynamic bitrate update in milliseconcds
        /// </summary>
        /// <value>Dynamic bitrate update in ms.</value>
        public int DynamicBitrateUpdateMs { get; set; }
        /// <summary>
        /// Wether to send video keyframes over TCP, instead of UDP
        /// </summary>
        /// <value>If true keyframes are sent over TCP, otherwise over UDP.</value>
        public bool SendKeyframesOverTCP { get; set; }
        /// <summary>
        /// Maximum width of video stream in pixels.
        /// </summary>
        /// <value>Maximum video width in pixels.</value>
        public int VideoMaximumWidth { get; set; }
        /// <summary>
        /// Maximum height of video stream in pixels.
        /// </summary>
        /// <value>Maximum video height in pixels.</value>
        public int VideoMaximumHeight { get; set; }
        /// <summary>
        /// Maximum framerate of video stream.
        /// </summary>
        /// <value>Maximum video framerate.</value>
        public int VideoMaximumFrameRate { get; set; }
        /// <summary>
        /// Video packet defrag timeout in milliseconds
        /// </summary>
        /// <value>Video packet defrag timeout in ms.</value>
        public int VideoPacketDefragTimeoutMs { get; set; }
        /// <summary>
        /// Wether to enable video frame acknowledgement
        /// </summary>
        /// <value>true is enabled, false otherwise .</value>
        public bool EnableVideoFrameAcks { get; set; }
        /// <summary>
        /// Wether to enable audio chat functionality
        /// </summary>
        /// <value>true if enabled, false otherwise.</value>
        public bool EnableAudioChat { get; set; }
        /// <summary>
        /// AudioBufferLengthHns
        /// </summary>
        /// <value>AudioBufferLengthHns.</value>
        public int AudioBufferLengthHns { get; set; }
        /// <summary>
        /// AudioSyncPolicy
        /// </summary>
        /// <value>AudioSyncPolicy.</value>
        public int AudioSyncPolicy { get; set; }
        /// <summary>
        /// AudioSyncMinLatency
        /// </summary>
        /// <value>AudioSyncMinLatency.</value>
        public int AudioSyncMinLatency { get; set; }
        /// <summary>
        /// AudioSyncDesiredLatency
        /// </summary>
        /// <value>AudioSyncDesiredLatency.</value>
        public int AudioSyncDesiredLatency { get; set; }
        /// <summary>
        /// AudioSyncMaxLatency
        /// </summary>
        /// <value>AudioSyncMaxLatency.</value>
        public int AudioSyncMaxLatency { get; set; }
        /// <summary>
        /// AudioSyncCompressLatency
        /// </summary>
        /// <value>AudioSyncCompressLatency.</value>
        public int AudioSyncCompressLatency { get; set; }
        /// <summary>
        /// AudioSyncCompressFactor
        /// </summary>
        /// <value>AudioSyncCompressFactor.</value>
        public float AudioSyncCompressFactor { get; set; }
        /// <summary>
        /// AudioSyncLengthenFactor
        /// </summary>
        /// <value>AudioSyncLengthenFactor.</value>
        public float AudioSyncLengthenFactor { get; set; }
        /// <summary>
        /// EnableOpusAudio
        /// </summary>
        /// <value>EnableOpusAudio.</value>
        public bool EnableOpusAudio { get; set; }
        /// <summary>
        /// EnableOpusChatAudio
        /// </summary>
        /// <value>EnableOpusChatAudio.</value>
        public bool EnableOpusChatAudio { get; set; }
        /// <summary>
        /// InputReadsPerSecond
        /// </summary>
        /// <value>InputReadsPerSecond.</value>
        public int InputReadsPerSecond { get; set; }
        /// <summary>
        /// UdpMaxSendPacketsInWinsock
        /// </summary>
        /// <value>UdpMaxSendPacketsInWinsock.</value>
        public int UdpMaxSendPacketsInWinsock { get; set; }
        /// <summary>
        /// UdpSubBurstGroups
        /// </summary>
        /// <value>UdpSubBurstGroups.</value>
        public int UdpSubBurstGroups { get; set; }
        /// <summary>
        /// UdpBurstDurationMs
        /// </summary>
        /// <value>UdpBurstDurationMs.</value>
        public int UdpBurstDurationMs { get; set; }
    }
}