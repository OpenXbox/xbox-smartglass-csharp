using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace DarkId.SmartGlass.Common
{
    // TODO: Optimize byte order reversal.
    internal class LEWriter : BinaryWriter
    {
        public LEWriter() :
            this(new MemoryStream())
        {
        }

        public LEWriter(Stream stream) :
            base(stream)
        {
        }

        public void WriteWithPaddingAlignment(byte[] bytes, int alignment)
        {
            Write(bytes);
            var paddingSize = BinaryExtensions.CalculatePaddingSize(bytes.Length, alignment);
            Write(EnumerableExtensions.EnumerableOf((byte)paddingSize, paddingSize).ToArray());
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

        /*
        public void Write(string str)
        {
            var bytes = Encoding.ASCII.GetBytes(str ?? string.Empty);

            Write((ushort)bytes.Length);
            _writer.Write(bytes);
            _writer.Write(new byte[1]);
        }
        */

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