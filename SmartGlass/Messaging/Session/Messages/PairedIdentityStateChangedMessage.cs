using System;
using SmartGlass.Common;

namespace SmartGlass.Messaging.Session.Messages
{
    [SessionMessageType(SessionMessageType.PairedIdentityStateChanged)]
    internal class PairedIdentityStateChangedMessage : SessionMessageBase
    {
        public PairedIdentityState State { get; set; }

        public override void Deserialize(EndianReader reader)
        {
            State = (PairedIdentityState)reader.ReadUInt16BE();
        }

        public override void Serialize(EndianWriter writer)
        {
            throw new NotSupportedException();
        }
    }
}
