using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace SmartGlass.Common
{
    internal class BEReader
    {
        private readonly BinaryReader _reader;

        public long Position
        {
            get => _reader.BaseStream.Position;
            set => _reader.BaseStream.Position = value;
        }

        public BEReader(byte[] bytes) :
            this(new MemoryStream(bytes))
        {
        }

        public BEReader(Stream stream)
        {
            _reader = new BinaryReader(stream, new UTF8Encoding(), true);
        }

        public byte ReadByte()
        {
            return _reader.ReadByte();
        }

        public byte[] ReadBytes(int count)
        {
            return _reader.ReadBytes(count);
        }

        public ushort ReadUInt16()
        {
            if (!BitConverter.IsLittleEndian)
            {
                return _reader.ReadUInt16();
            }

            return BitConverter.ToUInt16(BitConverter.GetBytes(_reader.ReadUInt16()).Reverse().ToArray(), 0);
        }

        public uint ReadUInt32()
        {
            if (!BitConverter.IsLittleEndian)
            {
                return _reader.ReadUInt32();
            }

            return BitConverter.ToUInt32(BitConverter.GetBytes(_reader.ReadUInt32()).Reverse().ToArray(), 0);
        }

        public ulong ReadUInt64()
        {
            if (!BitConverter.IsLittleEndian)
            {
                return _reader.ReadUInt64();
            }

            return BitConverter.ToUInt64(BitConverter.GetBytes(_reader.ReadUInt64()).Reverse().ToArray(), 0);
        }

        public short ReadInt16()
        {
            return IPAddress.NetworkToHostOrder(_reader.ReadInt16());
        }

        public int ReadInt32()
        {
            return IPAddress.NetworkToHostOrder(_reader.ReadInt32());
        }

        public long ReadInt64()
        {
            return IPAddress.NetworkToHostOrder(_reader.ReadInt64());
        }

        public float ReadSingle()
        {
            if (!BitConverter.IsLittleEndian)
            {
                return _reader.ReadSingle();
            }

            return BitConverter.ToSingle(BitConverter.GetBytes(_reader.ReadSingle()).Reverse().ToArray(), 0);
        }

        public byte[] ReadBlob()
        {
            var length = ReadUInt16();
            return _reader.ReadBytes(length);
        }

        public string ReadString()
        {
            var value = Encoding.ASCII.GetString(ReadBlob());
            _reader.ReadByte();

            return value;
        }

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

        public T[] ReadArray<T>()
            where T : ISerializable, new()
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

        public byte[] ReadToEnd()
        {
            return _reader.ReadBytes((int)(_reader.BaseStream.Length - _reader.BaseStream.Position));
        }

        public BEReader CreateChild(int count)
        {
            return new BEReader(_reader.ReadBytes(count));
        }

        public void Seek(long offset, SeekOrigin origin)
        {
            _reader.BaseStream.Seek(offset, origin);
        }

        public byte[] ToArray()
        {
            return _reader.BaseStream.ToBytes();
        }
    }
}