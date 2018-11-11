using System;
using SmartGlass.Common;

namespace SmartGlass.Messaging.Session.Messages
{
    class UnknownMessage : SessionMessageBase
    {
        public override void Deserialize(BEReader reader)
        {

        }

        public override void Serialize(BEWriter writer)
        {
            throw new InvalidOperationException();
        }
    }
}