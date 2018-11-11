using System;
using SmartGlass.Common;

namespace SmartGlass.Messaging.Session.Messages
{
    [SessionMessageType(SessionMessageType.PairedIdentityStateChanged)]
    internal class PairedIdentityStateChangedMessage : SessionMessageBase
    {
        PairedIdentityState State { get; set; }

        public override void Deserialize(BEReader reader)
        {
            State = (PairedIdentityState)reader.ReadUInt16();
        }

        public override void Serialize(BEWriter writer)
        {
            throw new NotSupportedException();
        }
    }
}
