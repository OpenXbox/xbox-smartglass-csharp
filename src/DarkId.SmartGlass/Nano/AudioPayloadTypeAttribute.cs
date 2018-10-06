using System;
using DarkId.SmartGlass.Common;

namespace DarkId.SmartGlass.Nano
{
    [AttributeUsage(
        AttributeTargets.Class,
        AllowMultiple = false,
        Inherited = false)]
    internal class AudioPayloadTypeAttribute : Attribute
    {
        private static AttributeTypeMapping<AudioPayloadTypeAttribute, AudioPayloadType> _typeMapping =
            new AttributeTypeMapping<AudioPayloadTypeAttribute, AudioPayloadType>(a => a.MessageType);

        public static Type GetTypeForMessageType(AudioPayloadType messageType)
        {
            return _typeMapping.GetTypeForKey(messageType);
        }

        public static AudioPayloadType GetMessageTypeForType(Type type)
        {
            return _typeMapping.GetKeyForType(type);
        }

        public AudioPayloadType MessageType { get; private set; }

        public AudioPayloadTypeAttribute(AudioPayloadType messageType)
        {
            MessageType = messageType;
        }
    }
}