using System;
using System.IO;
using System.Collections.Generic;
using Xunit;

namespace SmartGlass.Nano.Tests
{
    public class TestDataProvider
    {
        public Dictionary<string, byte[]> TestData { get; internal set; }
        public TestDataProvider(string path)
        {
            TestData = new Dictionary<string, byte[]>();
            string rootPath = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(
                TestContext.CurrentContext.TestDirectory)));
            string directoryPath = Path.Combine(rootPath, "TestData", path);
            string[] files = Directory.GetFiles(directoryPath);
            foreach (string file in files)
            {
                TestData.Add(Path.GetFileName(file), File.ReadAllBytes(file));
            }
        }
    }
}