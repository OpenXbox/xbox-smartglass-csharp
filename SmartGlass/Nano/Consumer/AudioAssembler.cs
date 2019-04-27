using SmartGlass.Nano.Packets;

namespace SmartGlass.Nano.Consumer
{
    public class AudioAssembler
    {
        private ulong _lastProcessedTimestamp;

        public AACFrame AssembleAudioFrame(AudioData data, AACProfile profile,
                                                int samplingFreq, byte channels)
        {
            AACFrame aacFrame = null;
            ulong timestamp = data.Timestamp;


            if (timestamp > _lastProcessedTimestamp)
            {
                // Only process new audio frames
                aacFrame = new AACFrame(data.Data, data.Timestamp, data.FrameId, data.Flags,
                    profile, samplingFreq, channels);
                _lastProcessedTimestamp = timestamp;
            }

            return aacFrame;
        }
    }
}
