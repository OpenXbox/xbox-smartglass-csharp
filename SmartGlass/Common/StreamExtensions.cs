using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace SmartGlass.Common
{
    /// <summary>
    /// BinaryReader/Writer extensions.
    /// </summary>
    public static class StreamExtensions
    {
        public static byte[] ToBytes(this Stream stream)
        {
            var memoryStream = stream as MemoryStream ?? new MemoryStream();
            if (memoryStream != stream)
            {
                stream.CopyTo(memoryStream);
            }
            return memoryStream.ToArray();
        }
    }
}
