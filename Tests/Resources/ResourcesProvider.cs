using System;
using System.IO;
using System.Collections.Generic;
using Xunit;
using System.Threading.Tasks;

namespace Tests.Resources
{
    public enum Type
    {
        Json,
        Misc,
        Nano,
        SmartGlass,
    }

    public class ResourcesProvider
    {
        static readonly string ResourcePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../../Resources/Resources");

        public static byte[] GetContent(string file, Type type = Type.Misc)
        {
            if (File.Exists($"{ResourcePath}/{type}/{file}"))
            {
                return File.ReadAllBytes($"{ResourcePath}/{type}/{file}");
            }
            throw new FileNotFoundException($"{ResourcePath}/{type}/{file}");
        }

        public static async Task<byte[]> GetContentAsync(string file, Type type = Type.Misc)
        {
            if (File.Exists($"{ResourcePath}/{type}/{file}"))
            {
                return await File.ReadAllBytesAsync($"{ResourcePath}/{type}/{file}");
            }
            throw new FileNotFoundException($"{ResourcePath}/{type}/{file}");
        }
    }
}