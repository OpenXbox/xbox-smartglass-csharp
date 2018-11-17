using System;
using SmartGlass.Common;

namespace SmartGlass.Nano
{
    [AttributeUsage(
        AttributeTargets.Class,
        AllowMultiple = false,
        Inherited = false)]
    internal class NanoPayloadTypeAttribute : Attribute
    {
        private static AttributeTypeMapping<NanoPayloadTypeAttribute, NanoPayloadType> _typeMapping =
            new AttributeTypeMapping<NanoPayloadTypeAttribute, NanoPayloadType>(a => a.MessageType);

        public static Type GetTypeForMessageType(NanoPayloadType messageType)
        {
            return _typeMapping.GetTypeForKey(messageType);
        }

        public static NanoPayloadType GetMessageTypeForType(Type type)
        {
            return _typeMapping.GetKeyForType(type);
        }

        public NanoPayloadType MessageType { get; private set; }

        public NanoPayloadTypeAttribute(NanoPayloadType messageType)
        {
            MessageType = messageType;
        }
    }
}