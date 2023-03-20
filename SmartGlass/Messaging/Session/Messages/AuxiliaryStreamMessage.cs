using SmartGlass.Common;

namespace SmartGlass.Messaging.Session.Messages
{
    [SessionMessageType(SessionMessageType.AuxiliaryStream)]
    internal record AuxiliaryStreamMessage : SessionMessageBase
    {
        public AuxiliaryStreamConnectionInfo ConnectionInfo { get; set; }

        public override void Deserialize(EndianReader reader)
        {
            if (reader.ReadByte() == 1)
            {
                ConnectionInfo = new AuxiliaryStreamConnectionInfo();
                ConnectionInfo.Deserialize(reader);
            }
        }

        public override void Serialize(EndianWriter writer)
        {
            writer.Write((byte)0);
        }
    }
}