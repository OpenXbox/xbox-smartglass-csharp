using System;
using SmartGlass.Common;

namespace SmartGlass.Messaging.Session.Messages
{
    [SessionMessageType(SessionMessageType.MediaCommand)]
    internal class MediaCommandMessage : SessionMessageBase
    {
        private static ulong requestId = 0;

        public MediaCommandState State { get; set; }

        public override void Deserialize(BEReader reader)
        {
            throw new NotImplementedException();
        }

        public override void Serialize(BEWriter writer)
        {
            var id = requestId++;

            writer.Write(id);
            writer.Write(State.TitleId);
            writer.Write((uint) State.Command);

            if (State.Command == MediaControlCommands.Seek) {
                writer.Write(State.SeekPosition);
            }
        }
    }
}
