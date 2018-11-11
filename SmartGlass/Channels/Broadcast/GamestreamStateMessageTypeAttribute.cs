using System;
using SmartGlass.Common;

namespace SmartGlass.Channels.Broadcast
{
    [AttributeUsage(
        AttributeTargets.Class,
        AllowMultiple = false,
        Inherited = false)]
    class GamestreamStateMessageTypeAttribute : Attribute
    {
        private static AttributeTypeMapping<GamestreamStateMessageTypeAttribute, GamestreamStateMessageType> _typeMapping =
            new AttributeTypeMapping<GamestreamStateMessageTypeAttribute, GamestreamStateMessageType>(a => a.MessageType);

        public static Type GetTypeForMessageType(GamestreamStateMessageType messageType)
        {
            return _typeMapping.GetTypeForKey(messageType);
        }

        public static GamestreamStateMessageType GetMessageTypeForType(Type type)
        {
            return _typeMapping.GetKeyForType(type);
        }

        public GamestreamStateMessageType MessageType { get; private set; }

        public GamestreamStateMessageTypeAttribute(GamestreamStateMessageType messageType)
        {
            MessageType = messageType;
        }
    }
}