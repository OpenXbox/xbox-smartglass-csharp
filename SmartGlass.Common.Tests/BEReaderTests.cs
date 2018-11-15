using NUnit.Framework;
using System.Threading.Tasks;
using SmartGlass.Common;

namespace SmartGlass.Common.Tests
{
    public class BEReaderTests
    {
        private BEReader _reader;

        [SetUp]
        public void Setup()
        {
            byte[] data = new byte[]
            {
                0x04, 0x00, 0x00, 0x00, 0x41, 0x42, 0x43, 0x44,
                0x45, 0x46, 0x47, 0x48, 0x49, 0x4A, 0x4B, 0x4C
            };
            _reader = new BEReader(data);
        }

        [Test]
        public void TestByte()
        {
            Assert.AreEqual(4, _reader.ReadByte());
            Assert.AreEqual(0, _reader.ReadByte());
            Assert.AreEqual(0, _reader.ReadByte());
            Assert.AreEqual(0, _reader.ReadByte());
            Assert.AreEqual(0x41, _reader.ReadByte());
        }

        [Test]
        public void TestBytes()
        {
            byte[] expect = new byte[]
            {
                0x04, 0x00, 0x00, 0x00, 0x41, 0x42, 0x43, 0x44
            };
            Assert.AreEqual(expect, _reader.ReadBytes(8));
        }

        [Test]
        public void TestInt16()
        {
            Assert.AreEqual(0x400, _reader.ReadInt16());
        }

        [Test]
        public void TestUInt16()
        {
            Assert.AreEqual(0x400, _reader.ReadUInt16());
        }

        [Test]
        public void TestInt32()
        {
            Assert.AreEqual(0x4000000, _reader.ReadInt32());
        }

        [Test]
        public void TestUInt32()
        {
            Assert.AreEqual(0x4000000, _reader.ReadUInt32());
        }

        [Test]
        public void TestInt64()
        {
            Assert.AreEqual(0x400000041424344, _reader.ReadInt64());
        }

        [Test]
        public void TestUInt64()
        {
            Assert.AreEqual(0x400000041424344, _reader.ReadUInt64());
        }

        [Test]
        public void TestSingle()
        {
            Assert.AreEqual(1.50463277E-36f, _reader.ReadSingle());
        }

        [Test]
        public void TestReadToEnd()
        {
            byte[] bytes = _reader.ReadToEnd();
            Assert.AreEqual(16, bytes.Length);
            Assert.AreEqual(0x04, bytes[0]);
            Assert.AreEqual(0x00, bytes[3]);
            Assert.AreEqual(0x44, bytes[7]);
            Assert.AreEqual(0x4C, bytes[15]);
        }

        [Test]
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
            Assert.AreEqual(4, array.Length);
            Assert.AreEqual(0xDEADBEEF, array[0]);
            Assert.AreEqual(0xBEEFDEAD, array[1]);
            Assert.AreEqual(0xB00B4142, array[2]);
            Assert.AreEqual(0x40414243, array[3]);
        }

        [Test]
        public void TestUInt16PrefixedBlob()
        {
            byte[] data = new byte[]
            {
                0x00, 0x03, 0xDE, 0xAD, 0xBE
            };
            BEReader reader = new BEReader(data);
            byte[] result = reader.ReadUInt16PrefixedBlob();

            Assert.AreEqual(3, result.Length);
            Assert.AreEqual(new byte[] { 0xDE, 0xAD, 0xBE }, result);
        }

        [Test]
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

        [Test]
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
            Assert.AreEqual("ABCDZYXW", result);
        }

        [Test]
        public void TestSeek()
        {
            _reader.Seek(9, System.IO.SeekOrigin.Current);

            Assert.AreEqual(_reader.Position, 9);
            Assert.AreEqual(0x46, _reader.ReadByte());
            Assert.AreEqual(10, _reader.Position);

            _reader.Seek(0, System.IO.SeekOrigin.Begin);
            _reader.ReadBytes(4);
            Assert.AreEqual(4, _reader.Position);

            _reader.Seek(2, System.IO.SeekOrigin.Begin);
            Assert.AreEqual(2, _reader.Position);

            _reader.Seek(-2, System.IO.SeekOrigin.End);
            Assert.AreEqual(14, _reader.Position);
        }

        [Test]
        public void TestCreateChild()
        {

        }

        [Test]
        public void TestReadUInt16PrefixedArray()
        {

        }

        [Test]
        public void TestToBytes()
        {
            byte[] data = new byte[] { 0xBE, 0xEF, 0xDE, 0xAD };
            BEReader reader = new BEReader(data);

            Assert.AreEqual(data, reader.ToBytes());
        }
    }
}