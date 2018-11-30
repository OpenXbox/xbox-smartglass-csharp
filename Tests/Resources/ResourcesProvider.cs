using System;
using System.IO;
using System.Collections.Generic;
using Xunit;
using System.Threading.Tasks;

namespace Tests.Resources
{
    public class ResourcesProvider
    {
        /* bad way to do this
        public Dictionary<string, byte[]> Resources { get; internal set; }
        public ResourcesProvider(string path)
        {
            Resources = new Dictionary<string, byte[]>();
            string rootPath = Directory.GetCurrentDirectory();
            string directoryPath = Path.Combine(rootPath, "Resources", path);
            string[] files = Directory.GetFiles(directoryPath);
            foreach (string file in files)
            {
                Resources.Add(Path.GetFileName(file), File.ReadAllBytes(file));
            }
        }
        */

        static readonly string ResourcePath = "./Resources/";

        public static byte[] GetContent(string file)
        {
            if (File.Exists($"{ResourcePath}{file}"))
            {
                return File.ReadAllBytes($"{ResourcePath}{file}");
            }
            return new byte[] { 0x00 };
        }

        public static async Task<byte[]> GetContentAsync(string file)
        {
            if (File.Exists($"{ResourcePath}{file}"))
            {
                return await File.ReadAllBytesAsync($"{ResourcePath}{file}");
            }
            return new byte[] { 0x00 };
        }
    }
}