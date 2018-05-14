using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DarkId.SmartGlass.Common;

namespace DarkId.SmartGlass.Messaging.Session.Messages
{
    [SessionMessageType(SessionMessageType.MediaState)]
    internal class MediaStateMessage : SessionMessageBase
    {
        public MediaState State { get; set; }

        public override void Deserialize(BEReader reader) {
            State = new MediaState();

            State.TitleId = reader.ReadUInt32();
            State.AumId = reader.ReadString();
            State.AssetId = reader.ReadString();
            State.MediaType = (MediaType)reader.ReadUInt16();
            State.SoundLevel = (MediaSoundLevel)reader.ReadUInt16();
            State.EnabledCommands = (MediaControlCommands)reader.ReadUInt32();
            State.PlaybackStatus = (MediaPlaybackStatus)reader.ReadUInt16();
            State.PlaybackRate = reader.ReadSingle();
            State.Position = TimeSpan.FromTicks((long)reader.ReadUInt64());
            State.MediaStart = TimeSpan.FromTicks((long)reader.ReadUInt64());
            State.MediaEnd = TimeSpan.FromTicks((long)reader.ReadUInt64());
            State.MinimumSeek = TimeSpan.FromTicks((long)reader.ReadUInt64());
            State.MaximumSeek = TimeSpan.FromTicks((long)reader.ReadUInt64());

            State.Metadata = new ReadOnlyDictionary<string, string>(
                reader.ReadArray<MediaMetadata>().ToDictionary(entry => entry.Name, entry => entry.Value));
        }

        public override void Serialize(BEWriter writer) {
            throw new NotImplementedException();
        }
    }
}
