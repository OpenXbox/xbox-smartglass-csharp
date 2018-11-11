using System;
using DarkId.SmartGlass.Common;

namespace DarkId.SmartGlass.Channels.Broadcast
{
    [AttributeUsage(
        AttributeTargets.Class,
        AllowMultiple = false,
        Inherited = false)]
    class BroadcastMessageTypeAttribute : Attribute
    {
        private static AttributeTypeMapping<BroadcastMessageTypeAttribute, BroadcastMessageType> _typeMapping =
            new AttributeTypeMapping<BroadcastMessageTypeAttribute, BroadcastMessageType>(a => a.MessageType);

        public static Type GetTypeForMessageType(BroadcastMessageType messageType)
        {
            return _typeMapping.GetTypeForKey(messageType);
        }

        public static BroadcastMessageType GetMessageTypeForType(Type type)
        {
            return _typeMapping.GetKeyForType(type);
        }

        public BroadcastMessageType MessageType { get; private set; }

        public BroadcastMessageTypeAttribute(BroadcastMessageType messageType)
        {
            MessageType = messageType;
        }
    }
}