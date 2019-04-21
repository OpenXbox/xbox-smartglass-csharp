using System.Threading.Tasks;
using SmartGlass.Common;
using Xunit;

namespace SmartGlass.Tests
{
    public class EndianWriterTests
    {
        private EndianWriter _writer;

        public EndianWriterTests()
        {
            _writer = new EndianWriter();
        }

        [Fact]
        public void TestByte()
        {
            _writer.Write((byte)0xFE);
            _writer.Write((byte)0xFA);

            Assert.Equal<byte[]>(new byte[] { 0xFE, 0xFA }, _writer.ToBytes());
        }

        [Fact]
        public void TestBytes()
        {
            byte[] data = new byte[] { 0xDE, 0xAD, 0xBE, 0xEF };
            _writer.Write(data);

            Assert.Equal<byte[]>(data, _writer.ToBytes());
        }

        [Fact]
        public void TestUShortBE()
        {
            _writer.WriteBE((ushort)0xFB42);

            Assert.Equal<byte[]>(new byte[] { 0xFB, 0x42 }, _writer.ToBytes());
        }

        [Fact]
        public void TestUIntBE()
        {
            _writer.WriteBE((uint)0x425112);

            Assert.Equal<byte[]>(new byte[] { 0x00, 0x42, 0x51, 0x12 }, _writer.ToBytes());
        }

        [Fact]
        public void TestULongBE()
        {
            _writer.WriteBE((ulong)0x425112);

            Assert.Equal<byte[]>(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x42, 0x51, 0x12 },
                            _writer.ToBytes());
        }

        [Fact]
        public void TestShortBE()
        {
            _writer.WriteBE((short)-2);

            Assert.Equal<byte[]>(new byte[] { 0xFF, 0xFE }, _writer.ToBytes());
        }

        [Fact]
        public void TestIntBE()
        {
            _writer.WriteBE((int)-2);

            Assert.Equal<byte[]>(new byte[] { 0xFF, 0xFF, 0xFF, 0xFE }, _writer.ToBytes());
        }

        [Fact]
        public void TestLongBE()
        {
            _writer.WriteBE((long)-2);

            Assert.Equal<byte[]>(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFE },
                            _writer.ToBytes());
        }

        [Fact]
        public void TestUInt16BEPrefixedString()
        {
            _writer.WriteUInt16BEPrefixed("ABCXYZ");

            Assert.Equal<byte[]>(new byte[] { 0x00, 0x06, 0x41, 0x42, 0x43, 0x58, 0x59, 0x5A, 0x00 },
                            _writer.ToBytes());
        }

        [Fact]
        public void TestToBytes()
        {
            _writer.Write((byte)0xDE);
            _writer.Write((byte)0xAD);
            _writer.Write((byte)0xBE);
            _writer.Write((byte)0xEF);

            Assert.Equal<byte[]>(new byte[] { 0xDE, 0xAD, 0xBE, 0xEF },
                            _writer.ToBytes());
        }
    }
}