using NUnit.Framework;
using System.Threading.Tasks;
using SmartGlass.Common;

namespace SmartGlass.Common.Tests
{
    public class BEWriterTests
    {
        private BEWriter _writer;

        [SetUp]
        public void Setup()
        {
            _writer = new BEWriter();
        }

        [Test]
        public void TestByte()
        {
            _writer.Write((byte)0xFE);
            _writer.Write((byte)0xFA);

            Assert.AreEqual(new byte[] { 0xFE, 0xFA }, _writer.ToBytes());
        }

        [Test]
        public void TestBytes()
        {
            byte[] data = new byte[] { 0xDE, 0xAD, 0xBE, 0xEF };
            _writer.Write(data);

            Assert.AreEqual(data, _writer.ToBytes());
        }

        [Test]
        public void TestUShort()
        {
            _writer.Write((ushort)0xFB42);

            Assert.AreEqual(new byte[] { 0xFB, 0x42 }, _writer.ToBytes());
        }

        [Test]
        public void TestUInt()
        {
            _writer.Write((uint)0x425112);

            Assert.AreEqual(new byte[] { 0x00, 0x42, 0x51, 0x12 }, _writer.ToBytes());
        }

        [Test]
        public void TestULong()
        {
            _writer.Write((ulong)0x425112);

            Assert.AreEqual(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x42, 0x51, 0x12 },
                            _writer.ToBytes());
        }

        [Test]
        public void TestShort()
        {
            _writer.Write((short)-2);

            Assert.AreEqual(new byte[] { 0xFF, 0xFE }, _writer.ToBytes());
        }

        [Test]
        public void TestInt()
        {
            _writer.Write((int)-2);

            Assert.AreEqual(new byte[] { 0xFF, 0xFF, 0xFF, 0xFE }, _writer.ToBytes());
        }

        [Test]
        public void TestLong()
        {
            _writer.Write((long)-2);

            Assert.AreEqual(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFE },
                            _writer.ToBytes());
        }

        [Test]
        public void TestUInt16PrefixedString()
        {
            _writer.WriteUInt16Prefixed("ABCXYZ");

            Assert.AreEqual(new byte[] { 0x00, 0x06, 0x41, 0x42, 0x43, 0x58, 0x59, 0x5A, 0x00 },
                            _writer.ToBytes());
        }

        [Test]
        public void TestToBytes()
        {
            _writer.Write((byte)0xDE);
            _writer.Write((byte)0xAD);
            _writer.Write((byte)0xBE);
            _writer.Write((byte)0xEF);

            Assert.AreEqual(new byte[] { 0xDE, 0xAD, 0xBE, 0xEF },
                            _writer.ToBytes());
        }
    }
}