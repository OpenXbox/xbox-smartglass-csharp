using System;
using DarkId.SmartGlass.Common;

namespace DarkId.SmartGlass.Nano
{
    [AttributeUsage(
        AttributeTargets.Class,
        AllowMultiple = false,
        Inherited = false)]
    internal class VideoPayloadTypeAttribute : Attribute
    {
        private static AttributeTypeMapping<VideoPayloadTypeAttribute, VideoPayloadType> _typeMapping =
            new AttributeTypeMapping<VideoPayloadTypeAttribute, VideoPayloadType>(a => a.MessageType);

        public static Type GetTypeForMessageType(VideoPayloadType messageType)
        {
            return _typeMapping.GetTypeForKey(messageType);
        }

        public static VideoPayloadType GetMessageTypeForType(Type type)
        {
            return _typeMapping.GetKeyForType(type);
        }

        public VideoPayloadType MessageType { get; private set; }

        public VideoPayloadTypeAttribute(VideoPayloadType messageType)
        {
            MessageType = messageType;
        }
    }
}