using SmartGlass.Common;

namespace SmartGlass.Messaging.Session
{
    abstract class SessionMessageBase : ISerializable
    {
        public SessionMessageHeader Header { get; set; }

        public SessionMessageBase()
        {
            Header = new SessionMessageHeader()
            {
                SessionMessageType = SessionMessageTypeAttribute.GetMessageTypeForType(GetType()),
                Version = 2
            };
        }

        public abstract void Deserialize(BEReader reader);
        public abstract void Serialize(BEWriter writer);
    }
}