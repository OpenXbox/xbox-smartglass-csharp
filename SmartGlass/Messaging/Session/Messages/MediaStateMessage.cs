using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using SmartGlass.Common;

namespace SmartGlass.Messaging.Session.Messages
{
    [SessionMessageType(SessionMessageType.MediaState)]
    internal record MediaStateMessage : SessionMessageBase
    {
        public MediaState State { get; set; }

        public override void Deserialize(EndianReader reader)
        {
            State = new MediaState();

            State.TitleId = reader.ReadUInt32BE();
            State.AumId = reader.ReadUInt16BEPrefixedString();
            State.AssetId = reader.ReadUInt16BEPrefixedString();
            State.MediaType = (MediaType)reader.ReadUInt16BE();
            State.SoundLevel = (MediaSoundLevel)reader.ReadUInt16BE();
            State.EnabledCommands = (MediaControlCommands)reader.ReadUInt32BE();
            State.PlaybackStatus = (MediaPlaybackStatus)reader.ReadUInt16BE();
            State.PlaybackRate = reader.ReadSingleBE();
            State.Position = TimeSpan.FromTicks((long)reader.ReadUInt64BE());
            State.MediaStart = TimeSpan.FromTicks((long)reader.ReadUInt64BE());
            State.MediaEnd = TimeSpan.FromTicks((long)reader.ReadUInt64BE());
            State.MinimumSeek = TimeSpan.FromTicks((long)reader.ReadUInt64BE());
            State.MaximumSeek = TimeSpan.FromTicks((long)reader.ReadUInt64BE());

            State.Metadata = new ReadOnlyDictionary<string, string>(
                reader.ReadUInt16BEPrefixedArray<MediaMetadata>()
                    .ToDictionary(entry => entry.Name, entry => entry.Value));
        }

        public override void Serialize(EndianWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
