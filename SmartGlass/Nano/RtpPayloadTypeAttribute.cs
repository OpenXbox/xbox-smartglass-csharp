using System;
using DarkId.SmartGlass.Common;

namespace DarkId.SmartGlass.Nano
{
    [AttributeUsage(
        AttributeTargets.Class,
        AllowMultiple = false,
        Inherited = false)]
    internal class RtpPayloadTypeAttribute : Attribute
    {
        private static AttributeTypeMapping<RtpPayloadTypeAttribute, RtpPayloadType> _typeMapping =
            new AttributeTypeMapping<RtpPayloadTypeAttribute, RtpPayloadType>(a => a.MessageType);

        public static Type GetTypeForMessageType(RtpPayloadType messageType)
        {
            return _typeMapping.GetTypeForKey(messageType);
        }

        public static RtpPayloadType GetMessageTypeForType(Type type)
        {
            return _typeMapping.GetKeyForType(type);
        }

        public RtpPayloadType MessageType { get; private set; }

        public RtpPayloadTypeAttribute(RtpPayloadType messageType)
        {
            MessageType = messageType;
        }
    }
}