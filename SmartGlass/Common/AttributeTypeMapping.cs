using System;
using System.Collections.Generic;
using System.Reflection;

namespace SmartGlass.Common
{
    public class AttributeTypeMapping<T, TKey> where T : Attribute
    {
        private readonly object _lock = new object();

        private readonly Func<T, TKey> _keyFunc;

        private Dictionary<TKey, Type> _messageTypeToType;
        private Dictionary<Type, TKey> _typeToMessageType;

        public AttributeTypeMapping(Func<T, TKey> keyFunc)
        {
            _keyFunc = keyFunc;
        }

        private void LazyLoadTypes()
        {
            lock (_lock)
            {
                if (_messageTypeToType == null)
                {
                    _messageTypeToType = new Dictionary<TKey, Type>();
                    _typeToMessageType = new Dictionary<Type, TKey>();

                    var assembly = typeof(T).Assembly;
                    foreach (var type in assembly.GetTypes())
                    {
                        var attribute = type.GetCustomAttribute<T>();
                        if (attribute != null)
                        {
                            var value = _keyFunc(attribute);
                            _messageTypeToType.Add(value, type);
                            _typeToMessageType.Add(type, value);
                        }
                    }
                }
            }
        }

        public Type GetTypeForKey(TKey messageType)
        {
            LazyLoadTypes();

            Type matchingType;
            _messageTypeToType.TryGetValue(messageType, out matchingType);

            return matchingType;
        }

        public TKey GetKeyForType(Type type)
        {
            LazyLoadTypes();

            TKey matchingKey;
            _typeToMessageType.TryGetValue(type, out matchingKey);

            return matchingKey;
        }
    }
}