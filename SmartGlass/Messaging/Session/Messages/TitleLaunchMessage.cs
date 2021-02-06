using System;
using SmartGlass.Common;

namespace SmartGlass.Messaging.Session.Messages
{
    [SessionMessageType(SessionMessageType.TitleLaunch)]
    internal record TitleLaunchMessage : SessionMessageBase
    {
        public TitleLaunchMessage()
        {
            Header.RequestAcknowledge = true;
        }

        public ActiveTitleLocation Location { get; set; }
        public string Uri { get; set; }

        public override void Deserialize(EndianReader reader)
        {
            throw new NotSupportedException();
        }

        public override void Serialize(EndianWriter writer)
        {
            writer.WriteBE((ushort)Location);
            writer.WriteUInt16BEPrefixed(Uri);
        }
    }
}