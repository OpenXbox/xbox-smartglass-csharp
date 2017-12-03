using System;
using DarkId.SmartGlass.Common;

namespace DarkId.SmartGlass.Messaging.Session
{
    [AttributeUsage(
        AttributeTargets.Class,
        AllowMultiple = false,
        Inherited = false)]
    internal class SessionMessageTypeAttribute : Attribute
    {
        private static AttributeTypeMapping<SessionMessageTypeAttribute, SessionMessageType> _typeMapping =
            new AttributeTypeMapping<SessionMessageTypeAttribute, SessionMessageType>(a => a.MessageType);

        public static Type GetTypeForMessageType(SessionMessageType messageType)
        {
            return _typeMapping.GetTypeForKey(messageType);
        }

        public static SessionMessageType GetMessageTypeForType(Type type)
        {
            return _typeMapping.GetKeyForType(type);
        }

        public SessionMessageType MessageType { get; private set; }

        public SessionMessageTypeAttribute(SessionMessageType messageType)
        {
            MessageType = messageType;
        }
    }
}