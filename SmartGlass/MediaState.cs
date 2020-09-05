using System;
using System.Collections.Generic;
using System.Text;

namespace SmartGlass
{

    public class MediaState
    {
        public MediaState()
        {
            AsOf = DateTime.Now;
        }
        public DateTime AsOf { get; internal set; }
        public uint TitleId { get; internal set; }
        public string AumId { get; internal set; }
        public string AssetId { get; internal set; }
        public MediaType MediaType { get; internal set; }
        public MediaSoundLevel SoundLevel { get; internal set; }
        public MediaControlCommands EnabledCommands { get; internal set; }
        public MediaPlaybackStatus PlaybackStatus { get; internal set; }
        public float PlaybackRate { get; internal set; }
        public TimeSpan Position { get; internal set; }
        public TimeSpan MediaStart { get; internal set; }
        public TimeSpan MediaEnd { get; internal set; }
        public TimeSpan MinimumSeek { get; internal set; }
        public TimeSpan MaximumSeek { get; internal set; }

        public IReadOnlyDictionary<string, string> Metadata { get; internal set; }

    }
}
