﻿using System;
using SmartGlass.Common;

namespace SmartGlass
{
    internal class MediaMetadata : ISerializable
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public void Deserialize(BEReader reader)
        {
            Name = reader.ReadString();
            Value = reader.ReadString();
        }

        public void Serialize(BEWriter writer)
        {
            throw new NotSupportedException();
        }
    }
}
