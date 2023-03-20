using System;
using SmartGlass.Common;

namespace SmartGlass.Messaging.Session.Messages
{
    [SessionMessageType(SessionMessageType.ConsoleStatus)]
    internal record ConsoleStatusMessage : SessionMessageBase
    {
        public ConsoleConfiguration Configuration { get; set; }
        public ActiveTitle[] ActiveTitles { get; set; }

        public override void Deserialize(EndianReader reader)
        {
            Configuration = new ConsoleConfiguration();
            ((ISerializable)Configuration).Deserialize(reader);

            ActiveTitles = reader.ReadUInt16BEPrefixedArray<ActiveTitle>();
        }

        public override void Serialize(EndianWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}