using System;
using SmartGlass.Common;

namespace SmartGlass.Messaging.Session.Messages
{
    class UnknownMessage : SessionMessageBase
    {
        public override void Deserialize(EndianReader reader)
        {

        }

        public override void Serialize(EndianWriter writer)
        {
            throw new InvalidOperationException();
        }
    }
}