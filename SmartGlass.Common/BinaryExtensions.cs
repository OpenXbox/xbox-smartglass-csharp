using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace SmartGlass.Common
{
    public static class BinaryExtensions
    {
        public static byte[] ToBytes(this BinaryReader reader)
        {
            return reader.BaseStream.ToBytes();
        }

        public static byte[] ToBytes(this BinaryWriter writer)
        {
            return writer.BaseStream.ToBytes();
        }

        public static byte[] ToBytes(this Stream stream)
        {
            var memoryStream = stream as MemoryStream ?? new MemoryStream();
            if (memoryStream != stream)
            {
                stream.CopyTo(memoryStream);
            }

            return memoryStream.ToArray();
        }

        public static BinaryReader ReaderFromBytes(byte[] data)
        {
            return new BinaryReader(new MemoryStream(data));
        }

        public static byte[] ReadUInt16PrefixedBlob(this BinaryReader reader)
        {
            var length = reader.ReadUInt16();
            return reader.ReadBytes(length);
        }

        public static byte[] ReadUInt32PrefixedBlob(this BinaryReader reader)
        {
            var length = reader.ReadUInt32();
            return reader.ReadBytes((int)length);
        }

        public static void WriteUInt16PrefixedBlob(this BinaryWriter writer, byte[] data)
        {
            writer.Write((ushort)data.Length);
            writer.Write(data);
        }

        public static void WriteUInt32PrefixedBlob(this BinaryWriter writer, byte[] data)
        {
            writer.Write((uint)data.Length);
            writer.Write(data);
        }

        public static uint[] ReadUInt32PrefixedUInt32Array(this BinaryReader reader)
        {
            var count = reader.ReadUInt32();
            var values = new uint[count];

            for (var i = 0; i < count; i++)
            {
                values[i] = reader.ReadUInt32();
            }

            return values;
        }

        public static T[] ReadUInt16PrefixedArray<T>(this BinaryReader reader)
            where T : ISerializableLE, new()
        {
            var count = reader.ReadUInt16();
            var items = new List<T>();

            for (var i = 0; i < count; i++)
            {
                var item = new T();
                item.Deserialize(reader);
                items.Add(item);
            }

            return items.ToArray();
        }

        public static T[] ReadUInt32PrefixedArray<T>(this BinaryReader reader)
            where T : ISerializableLE, new()
        {
            var count = reader.ReadUInt32();
            var items = new List<T>();

            for (var i = 0; i < count; i++)
            {
                var item = new T();
                item.Deserialize(reader);
                items.Add(item);
            }

            return items.ToArray();
        }

        public static void WriteUInt16PrefixedArray(
            this BinaryWriter writer, IEnumerable<ISerializableLE> data)
        {
            int count = data.Count();
            writer.Write((ushort)count);

            for (var i = 0; i < count; i++)
            {
                var item = data.ElementAt(i);
                item.Serialize(writer);
            }
        }

        public static void WriteUInt32PrefixedArray(
            this BinaryWriter writer, IEnumerable<ISerializableLE> data)
        {
            int count = data.Count();
            writer.Write((uint)count);

            for (var i = 0; i < count; i++)
            {
                var item = data.ElementAt(i);
                item.Serialize(writer);
            }
        }

        public static void WriteUInt32Prefixed(
            this BinaryWriter writer, IEnumerable<uint> arr)
        {
            var values = arr.ToList();
            writer.Write((uint)values.Count);

            foreach (var value in values)
            {
                writer.Write(value);
            }
        }

        public static byte[] ReadToEnd(
            this BinaryReader reader)
        {
            int dataLength = (int)(reader.BaseStream.Length - reader.BaseStream.Position);
            return reader.ReadBytes(dataLength);
        }

        public static void Seek(
            this BinaryReader reader, long offset, SeekOrigin origin)
        {
            reader.BaseStream.Seek(offset, origin);
        }

        public static void Seek(
            this BinaryWriter writer, long offset, SeekOrigin origin)
        {
            writer.BaseStream.Seek(offset, origin);
        }
    }
}