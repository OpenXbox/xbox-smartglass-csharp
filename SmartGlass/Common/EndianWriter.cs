using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace SmartGlass.Common
{
    /// <summary>
    /// Big endian writer, based on BinaryWriter.
    /// </summary>
    public class EndianWriter : IDisposable
    {
        private bool _disposing = false;

        // TODO: Optimize byte order reversal.
        private readonly BinaryWriter _writer;

        /// <summary>
        /// Gets or sets stream position
        /// </summary>
        /// <value>Position in stream</value>
        public long Position
        {
            get => _writer.BaseStream.Position;
            set => _writer.BaseStream.Position = value;
        }

        public long Length => _writer.BaseStream.Length;
        public Stream BaseStream => _writer.BaseStream;

        public EndianWriter() :
            this(new MemoryStream())
        {
        }

        public EndianWriter(byte[] buffer)
            : this(new MemoryStream(buffer))
        {
        }

        public EndianWriter(Stream stream)
        {
            _writer = new BinaryWriter(stream, new UTF8Encoding(), true);
        }

        public void Flush() => _writer.Flush();
        public void Close() => _writer.Close();

        #region Generic functions
        public void Write(byte value)
        {
            _writer.Write(value);
        }

        public void Write(byte[] bytes)
        {
            _writer.Write(bytes);
        }

        public void Seek(long offset, SeekOrigin origin)
        {
            _writer.BaseStream.Seek(offset, origin);
        }

        public byte[] ToBytes()
        {
            return _writer.BaseStream.ToBytes();
        }
        #endregion

        #region Big endian functions
        public void WriteBE(IEnumerable<uint> arr)
        {
            var values = arr.ToList();
            WriteBE(values.Count);

            foreach (var value in values)
            {
                WriteBE(value);
            }
        }

        public void WriteBE(ushort value)
        {
            if (!BitConverter.IsLittleEndian)
            {
                _writer.Write(value);
            }

            _writer.Write(BitConverter.GetBytes(value).Reverse().ToArray());
        }

        public void WriteBE(uint value)
        {
            if (!BitConverter.IsLittleEndian)
            {
                _writer.Write(value);
            }

            _writer.Write(BitConverter.GetBytes(value).Reverse().ToArray());
        }

        public void WriteBE(ulong value)
        {
            if (!BitConverter.IsLittleEndian)
            {
                _writer.Write(value);
            }

            _writer.Write(BitConverter.GetBytes(value).Reverse().ToArray());
        }

        public void WriteBE(short value)
        {
            _writer.Write(IPAddress.HostToNetworkOrder(value));
        }

        public void WriteBE(int value)
        {
            _writer.Write(IPAddress.HostToNetworkOrder(value));
        }

        public void WriteBE(long value)
        {
            _writer.Write(IPAddress.HostToNetworkOrder(value));
        }

        public void WriteBE(float value)
        {
            if (!BitConverter.IsLittleEndian)
            {
                _writer.Write(value);
            }

            _writer.Write(BitConverter.GetBytes(value).Reverse().ToArray());
        }

        /// <summary>
        /// Write a string, prefixed with it's length as UInt16.
        /// A null terminator byte is appended.
        /// </summary>
        /// <param name="str">The string to write</param>
        public void WriteUInt16BEPrefixed(string str)
        {
            var bytes = Encoding.ASCII.GetBytes(str ?? string.Empty);
            WriteBE((ushort)bytes.Length);
            Write(bytes);
            Write(new byte[1]);
        }
        #endregion

        #region Little endian functions
        public void WriteLE(ushort value)
            => _writer.Write(value);

        public void WriteLE(uint value)
            => _writer.Write(value);

        public void WriteLE(ulong value)
            => _writer.Write(value);

        public void WriteLE(short value)
            => _writer.Write(value);

        public void WriteLE(int value)
            => _writer.Write(value);
        
        public void WriteLE(long value)
            => _writer.Write(value);

        public void WriteUInt16LEPrefixedBlob(byte[] data)
        {
            WriteLE((ushort)data.Length);
            Write(data);
        }

        public void WriteUInt32LEPrefixedBlob(byte[] data)
        {
            WriteLE((uint)data.Length);
            Write(data);
        }



        public void WriteUInt16LEPrefixedArray(IEnumerable<ISerializable> data)
        {
            int count = data.Count();
            WriteLE((ushort)count);

            for (var i = 0; i < count; i++)
            {
                var item = data.ElementAt(i);
                item.Serialize(this);
            }
        }

        public void WriteUInt32LEPrefixedArray(IEnumerable<ISerializable> data)
        {
            int count = data.Count();
            WriteLE((uint)count);

            for (var i = 0; i < count; i++)
            {
                var item = data.ElementAt(i);
                item.Serialize(this);
            }
        }

        public void WriteUInt32LEPrefixed(IEnumerable<uint> arr)
        {
            var values = arr.ToList();
            WriteLE((uint)values.Count);

            foreach (var value in values)
            {
                WriteLE(value);
            }
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
