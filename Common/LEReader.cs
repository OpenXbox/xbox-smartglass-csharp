using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace SmartGlass.Common
{
    internal class LEReader : BinaryReader
    {
        public LEReader(byte[] bytes) :
            this(new MemoryStream(bytes))
        {
        }

        public LEReader(Stream stream) :
            base(stream)
        {
        }

        public byte[] ReadBlobUInt16()
        {
            var length = ReadUInt16();
            return base.ReadBytes(length);
        }

        public byte[] ReadBlobUInt32()
        {
            var length = ReadUInt32();
            return base.ReadBytes((int)length);
        }

        /*
        public string ReadString()
        {
            var value = Encoding.ASCII.GetString(ReadBlob());
            base.ReadByte();

            return value;
        }
        */

        public uint[] ReadUInt32Array()
        {
            var count = ReadUInt32();
            var values = new uint[count];

            for (var i = 0; i < count; i++)
            {
                values[i] = ReadUInt32();
            }

            return values;
        }

        public T[] ReadArrayUInt16<T>()
            where T : ISerializableLE, new()
        {
            var count = ReadUInt16();
            var items = new List<T>();

            for (var i = 0; i < count; i++)
            {
                var item = new T();
                item.Deserialize(this);
                items.Add(item);
            }

            return items.ToArray();
        }

        public T[] ReadArrayUInt32<T>()
            where T : ISerializableLE, new()
        {
            var count = ReadUInt32();
            var items = new List<T>();

            for (var i = 0; i < count; i++)
            {
                var item = new T();
                item.Deserialize(this);
                items.Add(item);
            }

            return items.ToArray();
        }

        public byte[] ReadToEnd()
        {
            return base.ReadBytes((int)(base.BaseStream.Length - base.BaseStream.Position));
        }

        public BEReader CreateChild(int count)
        {
            return new BEReader(base.ReadBytes(count));
        }

        public void Seek(long offset, SeekOrigin origin)
        {
            base.BaseStream.Seek(offset, origin);
        }

        public byte[] ToArray()
        {
            return base.BaseStream.ToBytes();
        }
    }
}