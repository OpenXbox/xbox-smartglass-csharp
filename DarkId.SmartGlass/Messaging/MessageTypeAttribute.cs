using System;
using DarkId.SmartGlass.Common;

namespace DarkId.SmartGlass.Messaging
{
    [AttributeUsage(
        AttributeTargets.Class,
        AllowMultiple = false,
        Inherited = false)]
    internal class MessageTypeAttribute : Attribute
    {
        private static AttributeTypeMapping<MessageTypeAttribute, MessageType> _typeMapping =
            new AttributeTypeMapping<MessageTypeAttribute, MessageType>(a => a.MessageType);

        public static Type GetTypeForMessageType(MessageType messageType)
        {
            return _typeMapping.GetTypeForKey(messageType);
        }

        public static MessageType GetMessageTypeForType(Type type)
        {
            return _typeMapping.GetKeyForType(type);
        }

        public MessageType MessageType { get; private set; }

        public MessageTypeAttribute(MessageType messageType)
        {
            MessageType = messageType;
        }
    }
}