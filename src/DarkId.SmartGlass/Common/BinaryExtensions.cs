using System.IO;

namespace DarkId.SmartGlass.Common
{
    internal static class BinaryExtensions
    {
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

        public static int CalculatePaddingSize(int size, int alignmentSize)
        {
            return (alignmentSize - (size % alignmentSize)) % alignmentSize;
        }
    }
}