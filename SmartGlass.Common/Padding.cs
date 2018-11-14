using System;
using System.Linq;

namespace SmartGlass.Common
{
    public enum PaddingType
    {
        PKCS7,
        ANSI_X923
    }

    public static class Padding
    {
        /// <summary>
        /// Calculate padding size for given size
        /// </summary>
        /// <param name="size">Size of unpadded data</param>
        /// <param name="alignmentSize">Alignment bytecount</param>
        /// <returns></returns>
        public static int CalculatePaddingSize(int size, int alignmentSize)
        {
            return (alignmentSize - (size % alignmentSize)) % alignmentSize;
        }

        /// <summary>
        /// Create padding bytearray
        /// </summary>
        /// <param name="paddingType">Type of padding</param>
        /// <param name="bytes">Unpadded Data blob</param>
        /// <param name="alignment">Alignment size</param>
        /// <returns>Padding bytearray</returns>
        public static byte[] CreatePaddingData(
            PaddingType paddingType, byte[] bytes, int alignment)
        {
            byte[] paddingBytes;
            int paddingSize = CalculatePaddingSize(bytes.Length, alignment);

            if (paddingSize == 0)
            {
                // No padding necessary
                return new byte[0];
            }

            switch (paddingType)
            {
                case PaddingType.PKCS7:
                    // 03 03 03
                    paddingBytes = EnumerableExtensions.EnumerableOf((byte)paddingSize, paddingSize)
                        .ToArray();
                    break;
                case PaddingType.ANSI_X923:
                    // 00 00 03
                    paddingBytes = new byte[paddingSize];
                    paddingBytes[paddingBytes.Length - 1] = (byte)paddingSize;
                    break;
                default:
                    throw new NotSupportedException(
                        $"WriteWithPaddingAlignment: Invalid PaddingType: {paddingType}");
            }
            return paddingBytes;
        }
    }
}