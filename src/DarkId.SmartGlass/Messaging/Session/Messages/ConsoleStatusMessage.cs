using System;
using DarkId.SmartGlass.Common;

namespace DarkId.SmartGlass.Messaging.Session.Messages
{
    [SessionMessageType(SessionMessageType.ConsoleStatus)]
    internal class ConsoleStatusMessage : SessionMessageBase
    {
        public ConsoleConfiguration Configuration { get; set; }
        public ActiveTitle[] ActiveTitles { get; set; }

        public override void Deserialize(BEReader reader)
        {
            Configuration = new ConsoleConfiguration();
            ((ISerializable)Configuration).Deserialize(reader);

            ActiveTitles = reader.ReadArray<ActiveTitle>();
        }

        public override void Serialize(BEWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}