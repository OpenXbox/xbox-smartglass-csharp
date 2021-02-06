using System;
using SmartGlass.Common;

namespace SmartGlass.Messaging.Session.Messages
{
    [SessionMessageType(SessionMessageType.MediaCommand)]
    internal record MediaCommandMessage : SessionMessageBase
    {
        private static ulong requestId = 0;

        public MediaCommandState State { get; set; }

        public override void Deserialize(EndianReader reader)
        {
            throw new NotImplementedException();
        }

        public override void Serialize(EndianWriter writer)
        {
            var id = requestId++;

            writer.WriteBE(id);
            writer.WriteBE(State.TitleId);
            writer.WriteBE((uint)State.Command);

            if (State.Command == MediaControlCommands.Seek)
            {
                writer.WriteBE(State.SeekPosition);
            }
        }
    }
}
