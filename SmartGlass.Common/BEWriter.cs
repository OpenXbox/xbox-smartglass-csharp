using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace SmartGlass.Common
{
    // TODO: Optimize byte order reversal.
    public class BEWriter
    {
        private readonly BinaryWriter _writer;

        public BEWriter() :
            this(new MemoryStream())
        {
        }

        public BEWriter(Stream stream)
        {
            _writer = new BinaryWriter(stream, new UTF8Encoding(), true);
        }

        public void Write(byte[] bytes)
        {
            _writer.Write(bytes);
        }

        public void Write(IEnumerable<uint> arr)
        {
            var values = arr.ToList();
            Write(values.Count);

            foreach (var value in values)
            {
                Write(value);
            }
        }

        public void Write(ushort value)
        {
            if (!BitConverter.IsLittleEndian)
            {
                _writer.Write(value);
            }

            _writer.Write(BitConverter.GetBytes(value).Reverse().ToArray());
        }

        public void Write(uint value)
        {
            if (!BitConverter.IsLittleEndian)
            {
                _writer.Write(value);
            }

            _writer.Write(BitConverter.GetBytes(value).Reverse().ToArray());
        }

        public void Write(ulong value)
        {
            if (!BitConverter.IsLittleEndian)
            {
                _writer.Write(value);
            }

            _writer.Write(BitConverter.GetBytes(value).Reverse().ToArray());
        }

        public void Write(byte value)
        {
            _writer.Write(value);
        }

        public void Write(short value)
        {
            _writer.Write(IPAddress.HostToNetworkOrder(value));
        }

        public void Write(int value)
        {
            _writer.Write(IPAddress.HostToNetworkOrder(value));
        }

        public void Write(long value)
        {
            _writer.Write(IPAddress.HostToNetworkOrder(value));
        }

        public void Write(float value)
        {
            if (!BitConverter.IsLittleEndian)
            {
                _writer.Write(value);
            }

            _writer.Write(BitConverter.GetBytes(value).Reverse().ToArray());
        }

        public void Write(string str)
        {
            var bytes = Encoding.ASCII.GetBytes(str ?? string.Empty);

            Write((ushort)bytes.Length);
            _writer.Write(bytes);
            _writer.Write(new byte[1]);
        }

        public void Seek(long offset, SeekOrigin origin)
        {
            _writer.BaseStream.Seek(offset, origin);
        }

        public byte[] ToBytes()
        {
            return _writer.BaseStream.ToBytes();
        }
    }
}