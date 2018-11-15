using System;
using SmartGlass.Common;

namespace SmartGlass.Messaging.Session.Messages
{
    [SessionMessageType(SessionMessageType.TitleLaunch)]
    internal class TitleLaunchMessage : SessionMessageBase
    {
        public TitleLaunchMessage()
        {
            Header.RequestAcknowledge = true;
        }

        public ActiveTitleLocation Location { get; set; }
        public string Uri { get; set; }

        public override void Deserialize(BEReader reader)
        {
            throw new NotSupportedException();
        }

        public override void Serialize(BEWriter writer)
        {
            writer.Write((ushort)Location);
            writer.WriteUInt16Prefixed(Uri);
        }
    }
}