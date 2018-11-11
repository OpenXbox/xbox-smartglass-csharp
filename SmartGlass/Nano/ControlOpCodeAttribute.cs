using System;
using SmartGlass.Common;

namespace SmartGlass.Nano
{
    [AttributeUsage(
        AttributeTargets.Class,
        AllowMultiple = false,
        Inherited = false)]
    internal class ControlOpCodeAttribute : Attribute
    {
        private static AttributeTypeMapping<ControlOpCodeAttribute, ControlOpCode> _typeMapping =
            new AttributeTypeMapping<ControlOpCodeAttribute, ControlOpCode>(a => a.MessageType);

        public static Type GetTypeForMessageType(ControlOpCode messageType)
        {
            return _typeMapping.GetTypeForKey(messageType);
        }

        public static ControlOpCode GetMessageTypeForType(Type type)
        {
            return _typeMapping.GetKeyForType(type);
        }

        public ControlOpCode MessageType { get; private set; }

        public ControlOpCodeAttribute(ControlOpCode messageType)
        {
            MessageType = messageType;
        }
    }
}