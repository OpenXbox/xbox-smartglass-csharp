using System;
using SmartGlass.Common;

namespace SmartGlass.Nano
{
    [AttributeUsage(
        AttributeTargets.Class,
        AllowMultiple = false,
        Inherited = false)]
    internal class ChannelControlTypeAttribute : Attribute
    {
        private static AttributeTypeMapping<ChannelControlTypeAttribute, ChannelControlType> _typeMapping =
            new AttributeTypeMapping<ChannelControlTypeAttribute, ChannelControlType>(a => a.MessageType);

        public static Type GetTypeForMessageType(ChannelControlType messageType)
        {
            return _typeMapping.GetTypeForKey(messageType);
        }

        public static ChannelControlType GetMessageTypeForType(Type type)
        {
            return _typeMapping.GetKeyForType(type);
        }

        public ChannelControlType MessageType { get; private set; }

        public ChannelControlTypeAttribute(ChannelControlType messageType)
        {
            MessageType = messageType;
        }
    }
}