using DarkId.SmartGlass.Common;

namespace DarkId.SmartGlass.Messaging.Session.Messages
{
    [SessionMessageType(SessionMessageType.AuxiliaryStream)]
    internal class AuxiliaryStreamMessage : SessionMessageBase
    {
        public AuxiliaryStreamConnectionInfo ConnectionInfo { get; set; }

        public override void Deserialize(BEReader reader)
        {
            if (reader.ReadByte() == 1)
            {
                ConnectionInfo = new AuxiliaryStreamConnectionInfo();
                ConnectionInfo.Deserialize(reader);
            }
        }

        public override void Serialize(BEWriter writer)
        {
            writer.Write((byte)0);
        }
    }
}