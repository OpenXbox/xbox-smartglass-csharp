using System.Threading.Tasks;
using SmartGlass.Common;
using Xunit;

namespace SmartGlass.Common.Tests
{
    public class BEReaderTests
    {
        private BEReader _reader;

        public BEReaderTests()
        {
            byte[] data = new byte[]
            {
                0x04, 0x00, 0x00, 0x00, 0x41, 0x42, 0x43, 0x44,
                0x45, 0x46, 0x47, 0x48, 0x49, 0x4A, 0x4B, 0x4C
            };
            _reader = new BEReader(data);
        }

        [Fact]
        public void TestByte()
        {
            Assert.Equal<byte>(4, _reader.ReadByte());
            Assert.Equal<byte>(0, _reader.ReadByte());
            Assert.Equal<byte>(0, _reader.ReadByte());
            Assert.Equal<byte>(0, _reader.ReadByte());
            Assert.Equal<byte>(0x41, _reader.ReadByte());
        }

        [Fact]
        public void TestBytes()
        {
            byte[] expect = new byte[]
            {
                0x04, 0x00, 0x00, 0x00, 0x41, 0x42, 0x43, 0x44
            };
            Assert.Equal<byte[]>(expect, _reader.ReadBytes(8));
        }

        [Fact]
        public void TestInt16()
        {
            Assert.Equal<short>(0x400, _reader.ReadInt16());
        }

        [Fact]
        public void TestUInt16()
        {
            Assert.Equal<ushort>(0x400, _reader.ReadUInt16());
        }

        [Fact]
        public void TestInt32()
        {
            Assert.Equal<int>(0x4000000, _reader.ReadInt32());
        }

        [Fact]
        public void TestUInt32()
        {
            Assert.Equal<uint>(0x4000000, _reader.ReadUInt32());
        }

        [Fact]
        public void TestInt64()
        {
            Assert.Equal<long>(0x400000041424344, _reader.ReadInt64());
        }

        [Fact]
        public void TestUInt64()
        {
            Assert.Equal<ulong>(0x400000041424344, _reader.ReadUInt64());
        }

        [Fact]
        public void TestSingle()
        {
            Assert.Equal<float>(1.50463277E-36f, _reader.ReadSingle());
        }

        [Fact]
        public void TestReadToEnd()
        {
            byte[] bytes = _reader.ReadToEnd();
            Assert.Equal<int>(16, bytes.Length);
            Assert.Equal<byte>(0x04, bytes[0]);
            Assert.Equal<byte>(0x00, bytes[3]);
            Assert.Equal<byte>(0x44, bytes[7]);
            Assert.Equal<byte>(0x4C, bytes[15]);
        }

        [Fact]
        public void TestUInt32Array()
        {
            BEReader reader = new BEReader(new byte[]
            {
                0x00,0x00,0x00,0x04,
                0xDE,0xAD,0xBE,0xEF,
                0xBE,0xEF,0xDE,0xAD,
                0xB0,0x0B,0x41,0x42,
                0x40,0x41,0x42,0x43
            });

            uint[] array = reader.ReadUInt32Array();
            Assert.Equal<int>(4, array.Length);
            Assert.Equal<uint>(0xDEADBEEF, array[0]);
            Assert.Equal<uint>(0xBEEFDEAD, array[1]);
            Assert.Equal<uint>(0xB00B4142, array[2]);
            Assert.Equal<uint>(0x40414243, array[3]);
        }

        [Fact]
        public void TestUInt16PrefixedBlob()
        {
            byte[] data = new byte[]
            {
                0x00, 0x03, 0xDE, 0xAD, 0xBE
            };
            BEReader reader = new BEReader(data);
            byte[] result = reader.ReadUInt16PrefixedBlob();

            Assert.Equal<int>(3, result.Length);
            Assert.Equal<byte[]>(new byte[] { 0xDE, 0xAD, 0xBE }, result);
        }

        [Fact]
        public void TestInvalidUInt16PrefixedString()
        {
            byte[] data = new byte[]
            {
                0x00, 0x8, // string length
                // ABCDZYXW
                0x41, 0x42, 0x43, 0x44,
                0x5A, 0x59, 0x58, 0x57
            };
            BEReader reader = new BEReader(data);
            Assert.Throws<System.IO.EndOfStreamException>(
                () => { reader.ReadUInt16PrefixedString(); });
        }

        [Fact]
        public void TestUInt16PrefixedString()
        {
            byte[] data = new byte[]
            {
                0x00, 0x8, // string length
                // ABCDZYXW
                0x41, 0x42, 0x43, 0x44,
                0x5A, 0x59, 0x58, 0x57,
                // null terminator
                0x00
            };
            BEReader reader = new BEReader(data);
            string result = reader.ReadUInt16PrefixedString();
            Assert.Equal("ABCDZYXW", result);
        }

        [Fact]
        public void TestSeek()
        {
            _reader.Seek(9, System.IO.SeekOrigin.Current);

            Assert.Equal<long>(9, _reader.Position);
            Assert.Equal<byte>(0x46, _reader.ReadByte());
            Assert.Equal<long>(10, _reader.Position);

            _reader.Seek(0, System.IO.SeekOrigin.Begin);
            _reader.ReadBytes(4);
            Assert.Equal<long>(4, _reader.Position);

            _reader.Seek(2, System.IO.SeekOrigin.Begin);
            Assert.Equal<long>(2, _reader.Position);

            _reader.Seek(-2, System.IO.SeekOrigin.End);
            Assert.Equal<long>(14, _reader.Position);
        }

        [Fact]
        public void TestCreateChild()
        {

        }

        [Fact]
        public void TestReadUInt16PrefixedArray()
        {

        }

        [Fact]
        public void TestToBytes()
        {
            byte[] data = new byte[] { 0xBE, 0xEF, 0xDE, 0xAD };
            BEReader reader = new BEReader(data);

            Assert.Equal<byte[]>(data, reader.ToBytes());
        }
    }
}