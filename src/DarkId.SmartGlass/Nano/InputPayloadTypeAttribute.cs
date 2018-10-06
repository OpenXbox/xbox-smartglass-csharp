using System;
using DarkId.SmartGlass.Common;

namespace DarkId.SmartGlass.Nano
{
    [AttributeUsage(
        AttributeTargets.Class,
        AllowMultiple = false,
        Inherited = false)]
    internal class InputPayloadTypeAttribute : Attribute
    {
        private static AttributeTypeMapping<InputPayloadTypeAttribute, InputPayloadType> _typeMapping =
            new AttributeTypeMapping<InputPayloadTypeAttribute, InputPayloadType>(a => a.MessageType);

        public static Type GetTypeForMessageType(InputPayloadType messageType)
        {
            return _typeMapping.GetTypeForKey(messageType);
        }

        public static InputPayloadType GetMessageTypeForType(Type type)
        {
            return _typeMapping.GetKeyForType(type);
        }

        public InputPayloadType MessageType { get; private set; }

        public InputPayloadTypeAttribute(InputPayloadType messageType)
        {
            MessageType = messageType;
        }
    }
}