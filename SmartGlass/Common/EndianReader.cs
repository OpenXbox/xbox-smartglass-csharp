using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace SmartGlass.Common
{
    public class EndianReader : IDisposable
    {
        private bool _disposing = false;

        private readonly BinaryReader _reader;

        /// <summary>
        /// Gets or sets stream position
        /// </summary>
        /// <value>Position in stream</value>
        public long Position
        {
            get => _reader.BaseStream.Position;
            set => _reader.BaseStream.Position = value;
        }

        public long Length => _reader.BaseStream.Length;
        public Stream BaseStream => _reader.BaseStream;

        /// <summary>
        /// Initialize instance of EndianReader, reading from bytearray
        /// </summary>
        public EndianReader(byte[] bytes)
            : this(new MemoryStream(bytes))
        {
        }

        /// <summary>
        /// Initialize instance of EndianReader, reading from provided stream
        /// </summary>
        /// <param name="stream">Stream to read from</param>
        public EndianReader(Stream stream)
        {
            _reader = new BinaryReader(stream, new UTF8Encoding(), true);
        }

        public void Close() => _reader.Close();

        #region Generic functions
        /// <summary>
        /// Read single byte
        /// </summary>
        /// <returns></returns>
        public byte ReadByte()
        {
            return _reader.ReadByte();
        }

        /// <summary>
        /// Read count of bytes
        /// </summary>
        /// <param name="count">Bytecount</param>
        /// <returns>Array of bytes</returns>
        public byte[] ReadBytes(int count)
        {
            return _reader.ReadBytes(count);
        }

        /// <summary>
        /// Read stream from current position to its end
        /// </summary>
        /// <returns>bytearray</returns>
        public byte[] ReadToEnd()
        {
            return _reader.ReadBytes((int)(_reader.BaseStream.Length - _reader.BaseStream.Position));
        }

        /// <summary>
        /// Create child reader from current position + count of bytes
        /// </summary>
        /// <param name="count">Count of bytes</param>
        /// <returns>Child reader</returns>
        public EndianReader CreateChild(int count)
        {
            return new EndianReader(_reader.ReadBytes(count));
        }

        /// <summary>
        /// Seek to offset in stream
        /// </summary>
        /// <param name="offset">Offset to seek to</param>
        /// <param name="origin">Seek origin</param>
        public void Seek(long offset, SeekOrigin origin)
        {
            _reader.BaseStream.Seek(offset, origin);
        }

        /// <summary>
        /// Convert underlaying stream to bytearray
        /// </summary>
        /// <returns>Bytearray of stream</returns>
        public byte[] ToBytes()
        {
            return _reader.BaseStream.ToBytes();
        }
        #endregion

        #region Big endian functions
        /// <summary>
        /// Read uint16 in big endian
        /// </summary>
        /// <returns>Big-endian value</returns>
        public ushort ReadUInt16BE()
        {
            if (!BitConverter.IsLittleEndian)
            {
                return _reader.ReadUInt16();
            }

            return BitConverter.ToUInt16(BitConverter.GetBytes(_reader.ReadUInt16()).Reverse().ToArray(), 0);
        }

        /// <summary>
        /// Read uint32 in big endian
        /// </summary>
        /// <returns>Big-endian value</returns>
        public uint ReadUInt32BE()
        {
            if (!BitConverter.IsLittleEndian)
            {
                return _reader.ReadUInt32();
            }

            return BitConverter.ToUInt32(BitConverter.GetBytes(_reader.ReadUInt32()).Reverse().ToArray(), 0);
        }

        /// <summary>
        /// Read uint64 in big endian
        /// </summary>
        /// <returns>Big-endian value</returns>
        public ulong ReadUInt64BE()
        {
            if (!BitConverter.IsLittleEndian)
            {
                return _reader.ReadUInt64();
            }

            return BitConverter.ToUInt64(BitConverter.GetBytes(_reader.ReadUInt64()).Reverse().ToArray(), 0);
        }

        /// <summary>
        /// Read int16 in big endian
        /// </summary>
        /// <returns>Big-endian value</returns>
        public short ReadInt16BE()
        {
            return IPAddress.NetworkToHostOrder(_reader.ReadInt16());
        }

        /// <summary>
        /// Read int32 in big endian
        /// </summary>
        /// <returns>Big-endian value</returns>
        public int ReadInt32BE()
        {
            return IPAddress.NetworkToHostOrder(_reader.ReadInt32());
        }

        /// <summary>
        /// Read int64 in big endian
        /// </summary>
        /// <returns>Big-endian value</returns>
        public long ReadInt64BE()
        {
            return IPAddress.NetworkToHostOrder(_reader.ReadInt64());
        }

        /// <summary>
        /// Read single in big endian
        /// </summary>
        /// <returns>Big-endian value</returns>
        public float ReadSingleBE()
        {
            if (!BitConverter.IsLittleEndian)
            {
                return _reader.ReadSingle();
            }

            return BitConverter.ToSingle(BitConverter.GetBytes(_reader.ReadSingle()).Reverse().ToArray(), 0);
        }

        /// <summary>
        /// Read uint16-BE prefixed byteblob in big endian
        /// </summary>
        /// <returns>Big-endian value</returns>
        public byte[] ReadUInt16BEPrefixedBlob()
        {
            var length = ReadUInt16BE();
            return ReadBytes(length);
        }

        /// <summary>
        /// Read a UInt16-BE length prefixed string.
        /// Forwards the underlaying stream
        /// by length prefix + string length + 1 (null terminator)
        /// </summary>
        /// <returns>Result as string</returns>
        public string ReadUInt16BEPrefixedString()
        {
            var value = Encoding.ASCII.GetString(ReadUInt16BEPrefixedBlob());
            ReadByte();

            return value;
        }

        /// <summary>
        /// Read Array of uint32-BE, prefixed with uint32-BE length
        /// </summary>
        /// <returns>Array of uint</returns>
        public uint[] ReadUInt32BEArray()
        {
            var count = ReadUInt32BE();
            var values = new uint[count];

            for (var i = 0; i < count; i++)
            {
                values[i] = ReadUInt32BE();
            }

            return values;
        }

        /// <summary>
        /// Read Array of uint16-BE prefixed ISerializable structs
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>Array of deserialized structs</returns>
        public T[] ReadUInt16BEPrefixedArray<T>()
            where T : ISerializable, new()
        {
            var count = ReadUInt16BE();
            var items = new List<T>();

            for (var i = 0; i < count; i++)
            {
                var item = new T();
                item.Deserialize(this);
                items.Add(item);
            }

            return items.ToArray();
        }
        #endregion

        #region Little endian functions
        public ushort ReadUInt16LE()
            => _reader.ReadUInt16();
        
        public uint ReadUInt32LE()
            => _reader.ReadUInt32();
        
        public ulong ReadUInt64LE()
            => _reader.ReadUInt64();
        
        public short ReadInt16LE()
            => _reader.ReadInt16();

        public int ReadInt32LE()
            => _reader.ReadInt32();

        public long ReadInt64LE()
            => _reader.ReadInt64();

        public byte[] ReadUInt16LEPrefixedBlob()
        {
            var length = ReadUInt16LE();
            return ReadBytes(length);
        }

        public byte[] ReadUInt32LEPrefixedBlob()
        {
            var length = ReadUInt32LE();
            return ReadBytes((int)length);
        }

        public uint[] ReadUInt32LEPrefixedUInt32Array()
        {
            var count = ReadUInt32LE();
            var values = new uint[count];

            for (var i = 0; i < count; i++)
            {
                values[i] = ReadUInt32LE();
            }

            return values;
        }

        public T[] ReadUInt16LEPrefixedArray<T>()
            where T : ISerializable, new()
        {
            var count = ReadUInt16LE();
            var items = new List<T>();

            for (var i = 0; i < count; i++)
            {
                var item = new T();
                item.Deserialize(this);
                items.Add(item);
            }

            return items.ToArray();
        }

        public T[] ReadUInt32LEPrefixedArray<T>()
            where T : ISerializable, new()
        {
            var count = ReadUInt32LE();
            var items = new List<T>();

            for (var i = 0; i < count; i++)
            {
                var item = new T();
                item.Deserialize(this);
                items.Add(item);
            }

            return items.ToArray();
        }
        #endregion

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposing)
            {
                if (disposing)
                {
                    // TODO
                }

                _disposing = true;
            }
        }

        void IDisposable.Dispose()
        {
            Dispose(true);
        }
    }
}