using SmartGlass.Common;

namespace SmartGlass.Messaging.Session
{
    /// <summary>
    /// Session message base.
    /// </summary>
    abstract record SessionMessageBase : ISerializable
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

        public abstract void Deserialize(EndianReader reader);
        public abstract void Serialize(EndianWriter writer);
    }
}
